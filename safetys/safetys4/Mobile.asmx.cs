using Newtonsoft.Json;
using safetys4.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace safetys4
{
     
    /// <summary>
    /// Summary description for Mobile
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Mobile : System.Web.Services.WebService
    {
   
        public string checkPermission(string id,string group_value,string permission_name,string type)
        {
            string return_value = "false";

            bool pa = safetys4.Class.SafetyPermission.checkPermisionAction(permission_name, id, type, Convert.ToInt32(group_value));
           
            if (pa == true)
            {
                return_value = "true";

            }

            return return_value;
        }


        public ArrayList getArealist(string company_id, string function_id)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.area_managements
                        join de in dbConnect.departments on c.department_id equals de.department_id into joinDe
                        join di in dbConnect.divisions on c.division_id equals di.division_id into joinDi
                        join s in dbConnect.sections on c.section_id equals s.section_id into joinSe
                        // join f in dbConnect.functions on c.function_id equals f.function_id
                        from de in joinDe.DefaultIfEmpty()
                        from di in joinDi.DefaultIfEmpty()
                        from s in joinSe.DefaultIfEmpty()
                        where c.status == "A" //&& c.name_en.Contains(area) || c.name_th.Contains(area)
                        select new
                        {
                            id = c.id,
                            name_th = chageDataLanguage(c.name_th, c.name_en, "th"),
                            name_en = chageDataLanguage(c.name_th, c.name_en, "en"),
                            name_si = chageDataLanguage(c.name_th, c.name_en, "si"),
                            // function = chageDataLanguage(c.name_th,c.name_en,lang),
                            c.company_id,
                            c.function_id,
                            c.department_id,
                            c.division_id,
                            c.section_id,
                            department_name_th = chageDataLanguage(de.department_th, de.department_en, "th"),
                            division_name_th = chageDataLanguage(di.division_th, di.division_en, "th"),
                            section_name_th = chageDataLanguage(s.section_th, s.section_en, "th"),
                            department_name_en = chageDataLanguage(de.department_th, de.department_en, "en"),
                            division_name_en = chageDataLanguage(di.division_th, di.division_en, "en"),
                            section_name_en = chageDataLanguage(s.section_th, s.section_en, "en"),
                            department_name_si = chageDataLanguage(de.department_th, de.department_en, "si"),
                            division_name_si = chageDataLanguage(di.division_th, di.division_en, "si"),
                            section_name_si = chageDataLanguage(s.section_th, s.section_en, "si")


                        };

                if (company_id != "")
                {
                    v = v.Where(c => c.function_id == function_id);

                }

                if (function_id != "")
                {
                    v = v.Where(c => c.function_id == function_id);

                }
                ArrayList dt = new ArrayList();
                foreach (var d in v)
                {

                    var result = new
                    {
                        id = d.id,
                        name_th = d.name_th,
                        name_en = d.name_en,
                        department_name_th = d.department_name_th,
                        division_name_th = d.division_name_th,
                        section_name_th = d.section_name_th,
                        department_name_en = d.department_name_en,
                        division_name_en = d.division_name_en,
                        section_name_en = d.section_name_en,
                        department_id = d.department_id,
                        division_id = d.division_id,
                        section_id = d.section_id,

                    };

                    dt.Add(result);
                }


                return dt;
            }
        }

        [WebMethod]
        public void getCompany()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string lang = Context.Request["lang"].ToString();
                string country = "";
                
                if (Context.Request["country"] == null)
                {
                    country = "thailand";
                    
                }
                else
                {
                    country = Context.Request["country"].ToString();
                  
                }


                var v = from c in dbConnect.companies
                        where c.country == country
                        orderby c.company_id ascending
                        select new
                        {
                            id = c.company_id,
                            name = chageDataLanguage(c.company_th, c.company_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }


        [WebMethod]
        public void getAllMasterdata()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string country = "";
                string timezone = "";

                if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                {
                    country = "thailand";
                    timezone = "+7";
                }
                else
                {
                    country = Context.Request["country"].ToString();
                    timezone = Context.Request["timezone"].ToString();
                }
                

                if (Context.Request["user_id"] != null && Context.Request["type_login"] != null)
                {
                    string type_login = Context.Request["type_login"].ToString();
                    string user_id = Context.Request["user_id"].ToString();
                    createLogLogin(user_id, type_login,timezone);

                }
                else
                {

                    createLogLogin("", "",timezone);
                }


                var v = from c in dbConnect.companies
                        where c.country == country
                        && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))
                        && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))
                        orderby c.company_id ascending
                        select new
                        {
                            id = c.company_id,
                            name_th = chageDataLanguage(c.company_th, c.company_en, "th"),
                            name_en = chageDataLanguage(c.company_th, c.company_en, "en"),
                            name_si = chageDataLanguage(c.company_th, c.company_en, "si"),
                            function = getFuctionByCompany(c.company_id,timezone)

                        };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }

        [WebMethod]
        public void getAllMasterdataHazard()
        {
            string country = "";
            // string lang = "";

            if (Context.Request["country"] == null)
            {
                country = "thailand";
                // lang = "en";

            }
            else
            {
                country = Context.Request["country"].ToString();
                // lang = Context.Request["lang"].ToString();

            }

            var result = new
            {
                category_hazard = getSourceHazard(country),
                fpe = getFatalityPrevention(country),
                characteristic = getHazardCharacteristic(country),
            };

            ArrayList dt = new ArrayList();
            dt.Add(result);


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(dt));

        }

        [WebMethod]
        public void getAllMasterdataIncident()
        {
            string country = "";

            if (Context.Request["country"] == null)
            {
                country = "thailand";

            }
            else
            {
                country = Context.Request["country"].ToString();

            }

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.reason_rejects
                        where c.status == "A" && c.country == country
                        select new
                        {
                            id = c.id,
                            name_th = chageDataLanguage(c.name_th, c.name_en, "th"),
                            name_en = chageDataLanguage(c.name_th, c.name_en, "en"),
                            name_si = chageDataLanguage(c.name_th, c.name_en, "si"),


                        };



                var result = new
                {
                    list_reject = v.ToArray()
                };

                ArrayList dt = new ArrayList();
                dt.Add(result);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }

        }

        public Array getFuctionByCompany(string company_id,string timezone)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.functions
                        where c.company_id == company_id
                        && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))
                        && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))
                        orderby c.function_id ascending
                        select new
                        {
                            id = c.function_id,
                            area = getArealist(company_id, c.function_id),
                            name_th = chageDataLanguage(c.function_th, c.function_en, "th"),
                            name_en = chageDataLanguage(c.function_th, c.function_en, "en"),
                            name_si = chageDataLanguage(c.function_th, c.function_en, "si"),
                            department = getDepartmentbyFunction(c.function_id,timezone),

                        };
                return v.ToArray();
            }
        }


         public Array getDepartmentbyFunction(string function_id,string timezone)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.departments
                        where c.function_id == function_id
                        && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))
                        && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))
                        orderby c.department_id ascending
                        select new
                        {
                            id = c.department_id,
                            name_th = chageDataLanguage(c.department_th, c.department_en, "th"),
                            name_en = chageDataLanguage(c.department_th, c.department_en, "en"),
                            name_si = chageDataLanguage(c.department_th, c.department_en, "si"),
                            division = getDivisionbyDepartment(c.department_id,timezone)
                        };

                return v.ToArray();
            }
        }


         public Array getDivisionbyDepartment(string department_id,string timezone)
         {


             using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
             {
                 var v = from c in dbConnect.divisions
                         where c.department_id == department_id
                         && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))
                         && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))
                         orderby c.division_id ascending
                         select new
                         {
                             id = c.division_id,
                             name_th = chageDataLanguage(c.division_th, c.division_en, "th"),
                             name_en = chageDataLanguage(c.division_th, c.division_en, "en"),
                             name_si = chageDataLanguage(c.division_th, c.division_en, "si"),
                             section = getSectionbyDivision(c.division_id,timezone)

                         };

                 return v.ToArray();
             }

         }


         public Array getSectionbyDivision(string division_id,string timezone)
         {


             using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
             {
                 var v = from c in dbConnect.sections
                         where c.division_id == division_id
                         && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))
                         && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))
                         orderby c.section_id ascending
                         select new
                         {
                             id = c.section_id,
                             name_th = chageDataLanguage(c.section_th, c.section_en, "th"),
                             name_en = chageDataLanguage(c.section_th, c.section_en, "en"),
                             name_si = chageDataLanguage(c.section_th, c.section_en, "si")

                         };

                 return v.ToArray();
             }

         }


         [WebMethod]
         public void getHazardstatus()
         {

             using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
             {
                 string lang = Context.Request["lang"].ToString();
                 string country = "";

                 if (Context.Request["country"] == null)
                 {
                     country = "thailand";

                 }
                 else
                 {
                     country = Context.Request["country"].ToString();

                 }


                 var v = from c in dbConnect.hazard_status
                         //orderby c.id ascending
                       //  where c.country == country
                         select new
                         {
                             id = c.id,
                             name = chageDataLanguage(c.name_th, c.name_en, lang)

                         };


                 JavaScriptSerializer js = new JavaScriptSerializer();
                 Context.Response.Write(js.Serialize(v));
             }

         }



         [WebMethod]
         public void getIncidentstatus()
         {

             using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
             {
                 string lang = Context.Request["lang"].ToString();
                 string country = "";

                 if (Context.Request["country"] == null)
                 {
                     country = "thailand";

                 }
                 else
                 {
                     country = Context.Request["country"].ToString();

                 }


                 var v = from c in dbConnect.incident_status
                         //orderby c.id ascending
                         //where c.country == country
                         select new
                         {
                             id = c.id,
                             name = chageDataLanguage(c.name_th, c.name_en, lang)

                         };


                 JavaScriptSerializer js = new JavaScriptSerializer();
                 Context.Response.Write(js.Serialize(v));
             }


         }








        [WebMethod]
       public void getFuctionByCompany()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string company = Context.Request["company"].ToString();
                string lang = Context.Request["lang"].ToString();


                var v = from c in dbConnect.functions
                        where c.company_id == company.Trim()
                        orderby c.function_id ascending
                        select new
                        {
                            id = c.function_id,
                            name = chageDataLanguage(c.function_th, c.function_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        public Array getHazardCharacteristic(string country)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.hazard_characteristics
                        where c.country == country
                        orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name_th = chageDataLanguage(c.name_th, c.name_en, "th"),
                            name_en = chageDataLanguage(c.name_th, c.name_en, "en"),
                            name_si = chageDataLanguage(c.name_th, c.name_en, "si"),

                        };

                return v.ToArray();

            }

        }


        [WebMethod]
        public void getDepartmentbyFunction()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string function = Context.Request["function"].ToString();
                string lang = Context.Request["lang"].ToString();



                var v = from c in dbConnect.departments
                        where c.function_id == function.Trim()
                        orderby c.department_id ascending
                        select new
                        {
                            id = c.department_id,
                            name = chageDataLanguage(c.department_th, c.department_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod]
       public void getDivisionbyDepartment()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string department = Context.Request["department"].ToString();
                string lang = Context.Request["lang"].ToString();



                var v = from c in dbConnect.divisions
                        where c.department_id == department.Trim()
                        orderby c.division_id ascending
                        select new
                        {
                            id = c.division_id,
                            name = chageDataLanguage(c.division_th, c.division_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod]
        public void getSectionbyDivision()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string division = Context.Request["division"].ToString();
                string lang = Context.Request["lang"].ToString();



                var v = from c in dbConnect.sections
                        where c.division_id == division.Trim()
                        orderby c.section_id ascending
                        select new
                        {
                            id = c.section_id,
                            name = chageDataLanguage(c.section_th, c.section_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }




        [WebMethod]
        public void createIncidentOnMobile()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                string error = "";
                string id = "";

                try
                {
                    string company_id = Context.Request["company_id"].ToString();
                    string incidentdate = Context.Request["incidentdate"].ToString();
                    string incidenttime = Context.Request["incidenttime"].ToString();
                    string reportdate = Context.Request["reportdate"].ToString();

                    string function_id = Context.Request["function_id"].ToString();
                    string department_id = Context.Request["department_id"].ToString();
                    string division_id = Context.Request["division_id"].ToString();
                    string section_id = Context.Request["section_id"].ToString();
                    string incidentarea = Context.Request["incidentarea"].ToString();
                    string incidentname = Context.Request["incidentname"].ToString();
                    string incidentdetail = Context.Request["incidentdetail"].ToString();
                    string userid = Context.Request["userid"].ToString();
                    string typelogin = Context.Request["typelogin"].ToString();
                    string phone = Context.Request["phone"].ToString();
                    string lang = Context.Request["lang"].ToString();
                    string lat = Context.Request["lat"].ToString();
                    string lng = Context.Request["lng"].ToString();

                  
                    string country = "";
                    string timezone = "";

                    if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                    {
                        country = "thailand";
                        timezone = "+7";
                    }
                    else
                    {
                        country = Context.Request["country"].ToString();
                        timezone = Context.Request["timezone"].ToString();
                    }


                    int process_status = 1;//on process
                    byte incident_flow = 1;
                    incident objInsert = new incident();

                    objInsert.doc_no = generateDocno(country,timezone);
                    objInsert.incident_date = FormatDates.changeDateTimeDB(incidentdate + " " + incidenttime, lang);

                    objInsert.report_date = FormatDates.changeDateTimeDB(reportdate, lang);

                    objInsert.company_id = company_id;
                    objInsert.function_id = function_id;
                    objInsert.department_id = department_id;
                    objInsert.division_id = division_id;
                    objInsert.section_id = section_id;
                    objInsert.incident_area = incidentarea;
                    objInsert.incident_name = incidentname;
                    objInsert.incident_detail = incidentdetail;
                    objInsert.employee_id = userid;
                    objInsert.typeuser_login = typelogin;
                    objInsert.phone = phone;
                    objInsert.process_status = process_status;
                    objInsert.step_form = 1;
                    objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.is_alert_over_due = 0;
                    objInsert.incident_flow = incident_flow;
                    if (!string.IsNullOrEmpty(lat.Trim()))
                    {
                        objInsert.lat = Convert.ToDecimal(lat);
                    }
                    else
                    {
                        objInsert.lat = 0;
                    }

                    if (!string.IsNullOrEmpty(lng.Trim()))
                    {
                        objInsert.lng = Convert.ToDecimal(lng);
                    }
                    else
                    {
                        objInsert.lng = 0;
                    }
                    objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.alert_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.alert_form_three_to_four_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.last_alerted_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.alert_form_two_to_three_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.device_type = "mobile";
                    objInsert.country = country;


                    objInsert.location_company_id = company_id;
                    objInsert.location_function_id = function_id;
                    objInsert.location_department_id = department_id;
                    objInsert.location_division_id = division_id;
                    objInsert.location_section_id = section_id;

                    string[] dt = new string[2];
                    dt = (string[])getMasterdataName(company_id, "company");
                    objInsert.location_company_name_th = dt[0];
                    objInsert.location_company_name_en = dt[1];

                    dt = (string[])getMasterdataName(function_id, "function");
                    objInsert.location_function_name_th = dt[0];
                    objInsert.location_function_name_en = dt[1];

                    dt = (string[])getMasterdataName(department_id, "department");
                    objInsert.location_department_name_th = dt[0];
                    objInsert.location_department_name_en = dt[1];

                    dt = (string[])getMasterdataName(division_id, "division");
                    objInsert.location_division_name_th = dt[0];
                    objInsert.location_division_name_en = dt[1];

                    dt = (string[])getMasterdataName(section_id, "section");
                    objInsert.location_section_name_th = dt[0];
                    objInsert.location_section_name_en = dt[1];



                    if (country == "thailand")
                    {
                        string responsible_area = Context.Request["responsible_area"].ToString();
                        string owner_activity = Context.Request["owner_activity"].ToString();

                        string activity_company_id = Context.Request["activity_company_id"].ToString();
                        string activity_function_id = Context.Request["activity_function_id"].ToString();
                        string activity_department_id = Context.Request["activity_department_id"].ToString();
                        string activity_division_id = Context.Request["activity_division_id"].ToString();
                        string activity_section_id = Context.Request["activity_section_id"].ToString();

                        objInsert.responsible_area = responsible_area;
                        objInsert.owner_activity = owner_activity;

                        objInsert.activity_company_id = activity_company_id;
                        objInsert.activity_function_id = activity_function_id;
                        objInsert.activity_department_id = activity_department_id;
                        objInsert.activity_division_id = activity_division_id;
                        objInsert.activity_section_id = activity_section_id;

                        objInsert.activity_location_company_id = activity_company_id;
                        objInsert.activity_location_function_id = activity_function_id;
                        objInsert.activity_location_department_id = activity_department_id;
                        objInsert.activity_location_division_id = activity_division_id;
                        objInsert.activity_location_section_id = activity_section_id;

                        string[] dt2 = new string[2];
                        dt2 = (string[])getMasterdataName(activity_company_id, "company");
                        objInsert.activity_location_company_name_th = dt2[0];
                        objInsert.activity_location_company_name_en = dt2[1];

                        dt2 = (string[])getMasterdataName(activity_function_id, "function");
                        objInsert.activity_location_function_name_th = dt2[0];
                        objInsert.activity_location_function_name_en = dt2[1];

                        dt2 = (string[])getMasterdataName(activity_department_id, "department");
                        objInsert.activity_location_department_name_th = dt2[0];
                        objInsert.activity_location_department_name_en = dt2[1];

                        dt2 = (string[])getMasterdataName(activity_division_id, "division");
                        objInsert.activity_location_division_name_th = dt2[0];
                        objInsert.activity_location_division_name_en = dt2[1];

                        dt2 = (string[])getMasterdataName(activity_section_id, "section");
                        objInsert.activity_location_section_name_th = dt2[0];
                        objInsert.activity_location_section_name_en = dt2[1];


                    }


                    //////////////////////////////////////reporter///////////////////////////////////////////////////
                    var t = from c in dbConnect.employees
                            join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                            where c.employee_id == userid
                            select new
                            {
                                o.company_id,
                                o.function_id,
                                o.department_id,
                                o.sub_function_id,
                                o.division_id,
                                o.section_id,
                                o.company,
                                o.function,
                                o.department,
                                o.sub_function,
                                o.division,
                                o.section
                            };


                    foreach (var rc1 in t)
                    {
                        objInsert.reporter_company_id = rc1.company_id;
                        objInsert.reporter_function_id = rc1.function_id;
                        objInsert.reporter_division_id = rc1.division_id;
                        objInsert.reporter_section_id = rc1.section_id;
                        objInsert.reporter_company_name = rc1.company;
                        objInsert.reporter_function_name = rc1.function;
                        objInsert.reporter_division_name = rc1.division;
                        objInsert.reporter_section_name = rc1.section;

                        if (country == "thailand")
                        {
                            objInsert.reporter_department_id = rc1.department_id;
                            objInsert.reporter_department_name = rc1.department;
                        }
                        else if (country == "srilanka")
                        {

                            objInsert.reporter_department_id = rc1.sub_function_id;
                            objInsert.reporter_department_name = rc1.sub_function;

                        }

                    }
                    //////////////////////////////////////end reporter//////////////////////////////////////////////

                    dbConnect.incidents.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                    MobileUploadedFileIncident(HttpContext.Current.Request.Files, userid, reportdate, lang,timezone);//upload image

                    incident_detail objInsert2 = new incident_detail();
                    objInsert2.employee_id = userid;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert2.process_status = process_status;
                    objInsert2.incident_id = objInsert.id;

                    dbConnect.incident_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();

                    //////////////////////////////////by p.poo sent notification/////////////////////////////////

                    Class.SafetyNotification sn = new Class.SafetyNotification();
                    string[] alert_to_groups = { "AdminOH&S", "AreaSuperervisor", "AreaManager", "AreaOH&S", "GroupOH&S" };
                    sn.InsertNotification(1, objInsert.id, alert_to_groups, timezone, "AreaSuperervisor");
                    ///////////////////////////////////end//////////////////////////////////////////////////////

                    result = "success";
                    id = objInsert.id.ToString();

                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    id = id

                };

                ArrayList dt3 = new ArrayList();
                dt3.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt3));
            }

        }


        public Array getMasterdataName(string id, string master_name)
        {
            string[] dt = new string[2];

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                if (master_name == "company")
                {
                    var v = from c in dbConnect.companies
                            where c.company_id == id
                            select new
                            {
                                name_th = chageDataLanguage(c.company_th, c.company_en, "th"),
                                name_en = chageDataLanguage(c.company_th, c.company_en, "en"),

                            };

                    foreach (var rc in v)
                    {
                        dt[0] = rc.name_th;
                        dt[1] = rc.name_en;

                    }

                }
                else if (master_name == "function")
                {
                    var v = from c in dbConnect.functions
                            where c.function_id == id
                            select new
                            {
                                name_th = chageDataLanguage(c.function_th, c.function_en, "th"),
                                name_en = chageDataLanguage(c.function_th, c.function_en, "en"),

                            };

                    foreach (var rc in v)
                    {
                        dt[0] = rc.name_th;
                        dt[1] = rc.name_en;

                    }

                }
                else if (master_name == "department")
                {

                    var v = from c in dbConnect.departments
                            where c.department_id == id
                            select new
                            {
                                name_th = chageDataLanguage(c.department_th, c.department_en, "th"),
                                name_en = chageDataLanguage(c.department_th, c.department_en, "en"),

                            };

                    foreach (var rc in v)
                    {
                        dt[0] = rc.name_th;
                        dt[1] = rc.name_en;

                    }
                }
                else if (master_name == "division")
                {
                    var v = from c in dbConnect.divisions
                            where c.division_id == id
                            select new
                            {
                                name_th = chageDataLanguage(c.division_th, c.division_en, "th"),
                                name_en = chageDataLanguage(c.division_th, c.division_en, "en"),

                            };

                    foreach (var rc in v)
                    {
                        dt[0] = rc.name_th;
                        dt[1] = rc.name_en;

                    }

                }
                else if (master_name == "section")
                {
                    var v = from c in dbConnect.sections
                            where c.section_id == id
                            select new
                            {
                                name_th = chageDataLanguage(c.section_th, c.section_en, "th"),
                                name_en = chageDataLanguage(c.section_th, c.section_en, "en"),

                            };

                    foreach (var rc in v)
                    {
                        dt[0] = rc.name_th;
                        dt[1] = rc.name_en;

                    }

                }

            }





            return dt;
        }
       


        public void MobileUploadedFileIncident(HttpFileCollection httpFileCollection, string user_id, string reportdate, string lang,string timezone)
        {

            reportdate = FormatDates.changeDateTimeUpload(reportdate, lang);
            string name_folder = user_id + "_" + FormatDates.getDateTimeNoDash(reportdate.Trim());

            for (int i = 0; i < httpFileCollection.Count;i++)
            {
                HttpPostedFile file = httpFileCollection[i];

                string file_name = FormatDates.getDateTimeMicro(DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))) + "_"+(i+1)+".jpg";


                if (file != null && file.ContentLength > 0)
                {
                    string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathupload"];
                    //string pathfolder = string.Format("{0}\\upload\\incident\\" + name_folder, Server.MapPath(@"\"));
                    string pathfolder = string.Format("{0}"+ pathupload + name_folder, Server.MapPath(@"\"));
                    if (!Directory.Exists(pathfolder))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(pathfolder);
                    }



                    var originalDirectory = new DirectoryInfo(pathfolder);

                    string pathString = System.IO.Path.Combine(originalDirectory.ToString());
                    bool isExists = System.IO.Directory.Exists(pathString);

                    if (!isExists)
                        System.IO.Directory.CreateDirectory(pathString);

                    var path = string.Format("{0}\\{1}", pathString, file_name);
                    file.SaveAs(path);

                }

            }


        }


        protected string generateDocno(string country,string timezone)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string docno = "";
                string year = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                int number = 0;


                var doc_no = dbConnect.incidents.Where(x => x.country == country).Where(x => x.doc_no.Contains("I" + year)).Max(x => x.doc_no);

                if (doc_no != "" && doc_no != null)
                {
                    string[] last = doc_no.Split('-');
                    number = Convert.ToInt32(last[1]) + 1;



                    docno = "I" + year + "-" + (number.ToString("D5"));

                }
                else
                {
                    docno = "I" + year + "-" + "00001";

                }


                return docno;
            }
        }





        [WebMethod]
        public void createHazardMobile()
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                string error = "";
                string id = "";

                string input = "";
                action_log insertLog = new action_log();

                try
                {
                    string hazarddate = Context.Request["hazarddate"].ToString();
                    string hazardtime = Context.Request["hazardtime"].ToString();
                    string reportdate = Context.Request["reportdate"].ToString();
                    string company_id = Context.Request["company_id"].ToString();
                    string function_id = Context.Request["function_id"].ToString();
                    string department_id = Context.Request["department_id"].ToString();
                    string division_id = Context.Request["division_id"].ToString();
                    string section_id = Context.Request["section_id"].ToString();
                    string hazardarea = Context.Request["hazardarea"].ToString();
                    string hazardname = Context.Request["hazardname"].ToString();
                    string hazarddetail = Context.Request["hazarddetail"].ToString();
                    string preliminary_action = Context.Request["preliminary_action"].ToString();
                    string type_action = Context.Request["type_action"].ToString();
                    string userid = Context.Request["userid"].ToString();
                    string typelogin = Context.Request["typelogin"].ToString();
                    string phone = Context.Request["phone"].ToString();
                    string lang = Context.Request["lang"].ToString();
                    string lat = Context.Request["lat"].ToString();
                    string lng = Context.Request["lng"].ToString();

                    string characteristic_id = "";
                    if(Context.Request["characteristic_id"] != null)
                    {
                        characteristic_id = Context.Request["characteristic_id"].ToString();
                    }

                    string country = "";
                    string timezone = "";

                    if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                    {
                        country = "thailand";
                        timezone = "+7";
                    }
                    else
                    {
                        country = Context.Request["country"].ToString();
                        timezone = Context.Request["timezone"].ToString();
                    }


                  
                    insertLog.function_name = "createHazardMobile";
                    insertLog.file_name = "Mobile";
                    insertLog.created = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    input = "hazarddate:" + hazarddate + "|hazardtime:" + hazardtime + "|reportdate:" + reportdate + "|company_id:" + company_id +
                             "|function_id:" + function_id + "|department_id:" + department_id + "|division_id:" + division_id +
                             "|section_id:" + section_id + "|hazardarea:" + hazardarea + "|preliminary_action:" + preliminary_action +
                             "|type_action:" + type_action + "|userid:" + userid + "|typelogin:" + typelogin + "|phone:" + phone +
                             "|lang:" + lang + "|lat:" + lat + "|lng:" + lng;


                    int process_status = 1;//on process
                    hazard objInsert = new hazard();


                    objInsert.doc_no = generateDocnoHazard(country,timezone);
                    objInsert.hazard_date = FormatDates.changeDateTimeDB(hazarddate + " " + hazardtime, lang);
                    objInsert.report_date = FormatDates.changeDateTimeDB(reportdate, lang);
                    objInsert.company_id = company_id;
                    objInsert.function_id = function_id;
                    objInsert.department_id = department_id;
                    objInsert.division_id = division_id;
                    objInsert.section_id = section_id;
                    objInsert.hazard_area = hazardarea;
                    objInsert.hazard_name = hazardname;
                    objInsert.hazard_detail = hazarddetail;
                    objInsert.preliminary_action = preliminary_action;
                    objInsert.type_action = type_action;
                    objInsert.employee_id = userid;
                    objInsert.typeuser_login = typelogin;
                    objInsert.phone = phone;
                    objInsert.process_status = process_status;
                    objInsert.step_form = 1;
                    objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.is_alert_over_due = 0;
                    if (characteristic_id != "")
                    {
                        objInsert.hazard_characteristic_id = Convert.ToInt32(characteristic_id);
                    }
                   

                    if (!string.IsNullOrEmpty(lat.Trim()))
                    {
                        objInsert.lat = Convert.ToDecimal(lat);
                    }
                    else
                    {
                        objInsert.lat = 0;
                    }

                    if (!string.IsNullOrEmpty(lng.Trim()))
                    {
                        objInsert.lng = Convert.ToDecimal(lng);
                    }
                    else
                    {
                        objInsert.lng = 0;
                    }

                    objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.alert_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.alert_form_three_to_four_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.last_alerted_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert.alert_form_two_to_three_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                
                    objInsert.device_type = "mobile";
                    objInsert.country = country;

                    objInsert.location_company_id = company_id;
                    objInsert.location_function_id = function_id;
                    objInsert.location_department_id = department_id;
                    objInsert.location_division_id = division_id;
                    objInsert.location_section_id = section_id;

                    string[] dt = new string[2];
                    dt = (string[])getMasterdataName(company_id, "company");
                    objInsert.location_company_name_th = dt[0];
                    objInsert.location_company_name_en = dt[1];

                    dt = (string[])getMasterdataName(function_id, "function");
                    objInsert.location_function_name_th = dt[0];
                    objInsert.location_function_name_en = dt[1];

                    dt = (string[])getMasterdataName(department_id, "department");
                    objInsert.location_department_name_th = dt[0];
                    objInsert.location_department_name_en = dt[1];

                    dt = (string[])getMasterdataName(division_id, "division");
                    objInsert.location_division_name_th = dt[0];
                    objInsert.location_division_name_en = dt[1];

                    dt = (string[])getMasterdataName(section_id, "section");
                    objInsert.location_section_name_th = dt[0];
                    objInsert.location_section_name_en = dt[1];

                    //////////////////////////////////////reporter///////////////////////////////////////////////////
                    var t = from c in dbConnect.employees
                            join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                            where c.employee_id == userid
                            select new
                            {
                                o.company_id,
                                o.function_id,
                                o.department_id,
                                o.sub_function_id,
                                o.division_id,
                                o.section_id,
                                o.company,
                                o.function,
                                o.department,
                                o.sub_function,
                                o.division,
                                o.section
                            };


                    foreach (var rc1 in t)
                    {
                        objInsert.reporter_company_id = rc1.company_id;
                        objInsert.reporter_function_id = rc1.function_id;
                        objInsert.reporter_division_id = rc1.division_id;
                        objInsert.reporter_section_id = rc1.section_id;
                        objInsert.reporter_company_name = rc1.company;
                        objInsert.reporter_function_name = rc1.function;
                        objInsert.reporter_division_name = rc1.division;
                        objInsert.reporter_section_name = rc1.section;

                        if (country == "thailand")
                        {
                            objInsert.reporter_department_id = rc1.department_id;
                            objInsert.reporter_department_name = rc1.department;
                        }
                        else if (country == "srilanka")
                        {

                            objInsert.reporter_department_id = rc1.sub_function_id;
                            objInsert.reporter_department_name = rc1.sub_function;

                        }

                    }
                    //////////////////////////////////////end reporter//////////////////////////////////////////////



                    dbConnect.hazards.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                    MobileUploadedFileHazard(HttpContext.Current.Request.Files, userid, reportdate, lang,timezone);//upload image

                    hazard_detail objInsert2 = new hazard_detail();
                    objInsert2.employee_id = userid;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert2.process_status = process_status;
                    objInsert2.hazard_id = objInsert.id;

                    dbConnect.hazard_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();

                    //////////////////////////////////by p.poo sent notification/////////////////////////////////

            
                    Class.SafetyNotification sn = new Class.SafetyNotification();
                    if (country == "thailand")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////                  
                        string[] alert_to_groups = { "AreaSuperervisor", "AreaOH&S" };//GroupOH&SHazard
                        sn.InsertHazardNotification(1, objInsert.id, alert_to_groups, timezone, "AreaOH&S");
                        ///////////////////////////////////end//////////////////////////////////////////////////////

                    }
                    else if (country == "srilanka")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        string[] alert_to_groups = { "AreaSuperervisor", "AreaOH&S", "AreaManager" };//GroupOH&SHazard , 
                        sn.InsertHazardNotification(1, objInsert.id, alert_to_groups, timezone, "AreaSuperervisor");
                        ///////////////////////////////////end//////////////////////////////////////////////////////

                    }
                    ///////////////////////////////////end//////////////////////////////////////////////////////

                    result = "success";
                    error = "";
                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();
                    input = input + "|error:" + error;
                    insertLog.error_message = input;
                    dbConnect.action_logs.InsertOnSubmit(insertLog);
                    dbConnect.SubmitChanges();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    id = id

                };

                ArrayList dt2 = new ArrayList();
                dt2.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt2));
            }

        }



        [WebMethod]

        public void updateHazard2()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                string result = "";
                string error = "";
                string id = "";

                try
                {
                    string verifying_date = Context.Request["verifying_date"].ToString();
                    string source_hazard = Context.Request["source_hazard"].ToString();
                    string level_hazard = Context.Request["level_hazard"].ToString();
                    string safety_officer_id = Context.Request["safety_officer_id"].ToString();
                    string user_id = Context.Request["user_id"].ToString();
                    string typelogin = Context.Request["typelogin"].ToString();
                    string hazardid = Context.Request["hazardid"].ToString();
                    string typebutton = Context.Request["typebutton"].ToString();
                    string group_id = Context.Request["group_id"].ToString();
                    string lang = Context.Request["lang"].ToString();
                    string fatality_prevention_element_id = Context.Request["fatality_prevention_element_id"].ToString();
                    string faltality_prevention_element_other = Context.Request["faltality_prevention_element_other"].ToString();

                    string country = "";
                    string timezone = "";

                    if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                    {
                        country = "thailand";
                        timezone = "+7";
                    }
                    else
                    {
                        country = Context.Request["country"].ToString();
                        timezone = Context.Request["timezone"].ToString();
                    }


                    var query = from c in dbConnect.hazards
                                where c.id == Convert.ToInt32(hazardid)
                                select c;

                    foreach (hazard rc in query)
                    {
                        rc.verifying_date = FormatDates.changeDateTimeDB(verifying_date, lang);
                        rc.source_hazard = Convert.ToInt32(source_hazard);
                        rc.level_hazard = level_hazard;

                        if (fatality_prevention_element_id != "")
                        {
                            rc.fatality_prevention_element_id = Convert.ToInt16(fatality_prevention_element_id);
                        }

                        rc.faltality_prevention_element_other = faltality_prevention_element_other;

                        if (typebutton == "report")
                        {
                            rc.submit_report_form = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            rc.submit_report_form2 = Convert.ToInt32(group_id);
                        }
                        else
                        {
                            rc.edit_form2 = Convert.ToInt32(group_id);

                        }
                        rc.safety_officer_id = safety_officer_id;


                    }


                    dbConnect.SubmitChanges();

                    int process_status = 1;//on process
                    hazard_detail objInsert2 = new hazard_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert2.process_status = process_status;
                    objInsert2.hazard_id = Convert.ToInt32(hazardid);

                    dbConnect.hazard_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();


                    if (typebutton == "report")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        if (country == "thailand")
                        {
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            string[] alert_to_groups = { "AreaSuperervisor" };
                            sn.InsertHazardNotification(2, Convert.ToInt32(hazardid), alert_to_groups, timezone, "AreaSuperervisor");
                        }
                        ///////////////////////////////////end//////////////////////////////////////////////////////
                    }

                    result = "success";

                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    id = id

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }
         

        }



        [WebMethod]      
        public void updateActionIncident()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                string error = "";
                string id = "";

                try
                {
                    string corrective_id = Context.Request["id"].ToString();
                    string country = "";
                    string timezone = "";

                    if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                    {
                        country = "thailand";
                        timezone = "+7";
                    }
                    else
                    {
                        country = Context.Request["country"].ToString();
                        timezone = Context.Request["timezone"].ToString();
                    }

                    var query = from c in dbConnect.corrective_prevention_action_incidents
                                where c.id == Convert.ToInt32(corrective_id)
                                select c;

                    foreach (corrective_prevention_action_incident rc in query)
                    {

                        rc.action_status_id = 2;
                        rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));


                        rc.attachment_file = MobileUploadedFileIncidentAction(HttpContext.Current.Request.Files, rc.incident_id,timezone,country);

                        dbConnect.SubmitChanges();
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        if (country == "thailand")
                        {
                            string[] alert_to_groups = { "AreaOH&S" };
                            sn.InsertNotification(6, rc.incident_id, alert_to_groups, timezone,"AreaOH&S", rc.id);

                        }
                        else if (country == "srilanka")
                        {
                            string[] alert_to_groups = { "AreaManager" };
                            sn.InsertNotification(6, rc.incident_id, alert_to_groups, timezone, "AreaManager", rc.id);

                        }

                        ////////////////////////////////////////end///////////////////////////////////

                    }

                    result = "success";
                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    id = id

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));

            }

        }

        [WebMethod]
        
        public void updateActionHazard()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                string error = "";
                string id = "";

                try
                {
                    string process_id = Context.Request["id"].ToString();
                    string country = "";
                    string timezone = "";

                    if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                    {
                        country = "thailand";
                        timezone = "+7";
                    }
                    else
                    {
                        country = Context.Request["country"].ToString();
                        timezone = Context.Request["timezone"].ToString();
                    }

                    var query = from c in dbConnect.process_actions
                                where c.id == Convert.ToInt32(process_id)
                                select c;

                    foreach (process_action rc in query)
                    {

                        rc.action_status_id = 2;
                        rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));


                        rc.attachment_file = MobileUploadedFileHazardAction(HttpContext.Current.Request.Files, rc.hazard_id,timezone,country);
                        dbConnect.SubmitChanges();

                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { "AreaSuperervisor" };
                        sn.InsertHazardNotification(5, rc.hazard_id, alert_to_groups, timezone,"AreaSuperervisor", rc.id);

                        ////////////////////////////////////////end///////////////////////////////////


                    }

                    result = "success";


                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    id = id

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }


        }






        public Array getSourceHazard(string country)
        {
            //string lang = Context.Request["lang"].ToString();


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.source_hazards
                        where c.status == "A" && c.country == country
                        select new
                        {
                            id = c.id,
                            name_th = chageDataLanguage(c.name_th, c.name_en, "th"),
                            name_en = chageDataLanguage(c.name_th, c.name_en, "en"),
                            name_si = chageDataLanguage(c.name_th, c.name_en, "si"),


                        };


                return v.ToArray();
            }
        }


     
        public Array getFatalityPrevention(string country)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.fatality_prevention_elements
                        where c.status == "A" && c.country == country
                        select new
                        {
                            id = c.id,
                            name_th = chageDataLanguage(c.name_th, c.name_en, "th"),
                            name_en = chageDataLanguage(c.name_th, c.name_en, "en"),
                            name_si = chageDataLanguage(c.name_th, c.name_en, "si"),


                        };


                return v.ToArray();
            }

        }

        protected string generateDocnoHazard(string country,string timezone)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string docno = "";
                string year = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                int number = 0;

                var doc_no = dbConnect.hazards.Where(x => x.country == country).Where(x => x.doc_no.Contains("H" + year)).Max(t => t.doc_no);

                if (doc_no != "" && doc_no != null)
                {
                    string[] last = doc_no.Split('-');
                    number = Convert.ToInt32(last[1]) + 1;



                    docno = "H" + year + "-" + (number.ToString("D5"));

                }
                else
                {
                    docno = "H" + year + "-" + "00001";

                }


                return docno;
            }
        }




        public void MobileUploadedFileHazard(HttpFileCollection httpFileCollection, string user_id, string reportdate, string lang,string timezone)
        {


            reportdate = FormatDates.changeDateTimeUpload(reportdate, lang);
            string name_folder = user_id + "_" + FormatDates.getDateTimeNoDash(reportdate.Trim());
            
         
            for (int i = 0; i < httpFileCollection.Count; i++)
            {
                HttpPostedFile file = httpFileCollection[i];

                string file_name = FormatDates.getDateTimeMicro(DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))) + "_" + (i + 1) + ".jpg";


                if (file != null && file.ContentLength > 0)
                {
                    string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathuploadhazard"];
                   // string pathfolder = string.Format("{0}\\upload\\hazard\\" + name_folder, Server.MapPath(@"\"));
                    string pathfolder = string.Format("{0}"+ pathupload + name_folder, Server.MapPath(@"\"));
                    if (!Directory.Exists(pathfolder))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(pathfolder);
                    }



                    var originalDirectory = new DirectoryInfo(pathfolder);

                    string pathString = System.IO.Path.Combine(originalDirectory.ToString());
                    bool isExists = System.IO.Directory.Exists(pathString);

                    if (!isExists)
                        System.IO.Directory.CreateDirectory(pathString);

                    var path = string.Format("{0}\\{1}", pathString, file_name);
                    file.SaveAs(path);

                }

            }


        }
        public string MobileUploadedFileHazardAction(HttpFileCollection httpFileCollection, int id,string timezone,string country)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string doc_no = "";

                var q = from c in dbConnect.hazards
                        where c.id == id
                        select new
                        {
                            doc_no = c.doc_no
                        };



                foreach (var v in q)
                {
                    doc_no = v.doc_no;
                }


                string name_folder = doc_no;
                string file_name = "action_" + FormatDates.getDateTimeMicro(DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)));
                bool isSavedSuccessfully = false;
                string fName = "";
                string typefile = "";


                Array list = httpFileCollection.AllKeys;

                for (int i = 0; i < list.Length; i++)
                {
                    HttpPostedFile file = httpFileCollection[i];

                    fName = file.FileName;
                    typefile = Path.GetExtension(file.FileName);

                    if (file != null && file.ContentLength > 0)
                    {
                        string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathuploadhazard"];
                       // string pathfolder = string.Format("{0}\\upload\\hazard\\step3\\" + name_folder, Server.MapPath(@"\"));
                        string pathfolder = string.Format("{0}"+pathupload+"step3\\" + country +"\\"+ name_folder, Server.MapPath(@"\"));
                        if (!Directory.Exists(pathfolder))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(pathfolder);
                        }


                        var originalDirectory = new DirectoryInfo(pathfolder);

                        string pathString = System.IO.Path.Combine(originalDirectory.ToString());


                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                        {
                            System.IO.Directory.CreateDirectory(pathString);
                        }


                        file_name = file_name + typefile;
                        var path = string.Format("{0}\\{1}", pathString, file_name);
                        file.SaveAs(path);
                        isSavedSuccessfully = true;
                    }

                }


                string return_value = "";
                if (isSavedSuccessfully)
                {
                    return_value = file_name;
                }
                else
                {
                    return_value = "errror";
                }


                return return_value;
            }

        }


        public string MobileUploadedFileIncidentAction(HttpFileCollection httpFileCollection, int id,string timezone,string country)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string doc_no = "";

                var q = from c in dbConnect.incidents
                        where c.id == id
                        select new
                        {
                            doc_no = c.doc_no
                        };



                foreach (var v in q)
                {
                    doc_no = v.doc_no;
                }



                string name_folder = doc_no;
                string file_name = "corrective_" + FormatDates.getDateTimeMicro(DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)));
                bool isSavedSuccessfully = false;
                string fName = "";
                string typefile = "";


                Array list = httpFileCollection.AllKeys;

                for (int i = 0; i < list.Length; i++)
                {
                    HttpPostedFile file = httpFileCollection[i];

                    fName = file.FileName;
                    typefile = Path.GetExtension(file.FileName);

                    if (file != null && file.ContentLength > 0)
                    {
                        string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathupload"];
                        //string pathfolder = string.Format("{0}\\upload\\incident\\step3\\" + name_folder, Server.MapPath(@"\"));
                        string pathfolder = string.Format("{0}" + pathupload + "step3\\" + country + "\\" + name_folder, Server.MapPath(@"\"));
                        if (!Directory.Exists(pathfolder))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(pathfolder);
                        }


                        var originalDirectory = new DirectoryInfo(pathfolder);

                        string pathString = System.IO.Path.Combine(originalDirectory.ToString());


                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                        {
                            System.IO.Directory.CreateDirectory(pathString);
                        }


                        file_name = file_name + typefile;
                        var path = string.Format("{0}\\{1}", pathString, file_name);
                        file.SaveAs(path);
                        isSavedSuccessfully = true;
                    }

                }


                string return_value = "";
                if (isSavedSuccessfully)
                {
                    return_value = file_name;
                }
                else
                {
                    return_value = "errror";
                }


                return return_value;
            }

        }





        [WebMethod]   
        public void getListAllIncident()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                string company_id = Context.Request["company_id"].ToString();
                string function_id = Context.Request["function_id"].ToString();
                string department_id = Context.Request["department_id"].ToString();
                string division_id = Context.Request["division_id"].ToString();
                string status = Context.Request["status"].ToString();
                string date_start = Context.Request["date_start"].ToString();
                string date_end = Context.Request["date_end"].ToString();
                // string type = Context.Request["type"].ToString();
                string lang = Context.Request["lang"].ToString();

                string country = "";
                string timezone = "";

                if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                {
                    country = "thailand";
                    timezone = "+7";
                }
                else
                {
                    country = Context.Request["country"].ToString();
                    timezone = Context.Request["timezone"].ToString();
                }

                var v = from d in dbConnect.incidents
                        join c in dbConnect.companies on d.company_id equals c.company_id
                        join f in dbConnect.functions on d.function_id equals f.function_id
                        join de in dbConnect.departments on d.department_id equals de.department_id
                        join di in dbConnect.divisions on d.division_id equals di.division_id into joinD
                        join se in dbConnect.sections on d.section_id equals se.section_id into joinS
                        join s in dbConnect.incident_status on d.process_status equals s.id
                        from di in joinD.DefaultIfEmpty()
                        from se in joinS.DefaultIfEmpty()
                        where d.country == country
                        orderby d.report_date descending

                        select new
                        {
                            d.company_id,
                            d.function_id,
                            d.department_id,
                            d.division_id,
                            d.section_id,
                            company_name = chageDataLanguage(c.company_th, c.company_en, lang),
                            function_name = chageDataLanguage(f.function_th, f.function_en, lang),
                            department_name = chageDataLanguage(de.department_th, de.department_en, lang),
                            division_name = chageDataLanguage(di.division_th, di.division_en, lang),
                            section_name = chageDataLanguage(se.section_th, se.section_en, lang),
                            d.id,
                            d.step_form,
                            d.process_status,
                            d.doc_no,
                            d.incident_name,
                            d.incident_detail,
                            incident_date = d.incident_date,
                            incident_date2 = d.incident_date,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            d.submit_report_form2,
                            d.confirm_investigate_form2,
                            d.confirm_by_groupohs_form2,
                            d.request_close_form3,

                        };

                if (company_id != "")
                {
                    v = v.Where(c => c.company_id == company_id.Trim());

                }

                if (function_id != "")
                {
                    v = v.Where(c => c.function_id == function_id.Trim());

                }

                if (department_id != "")
                {
                    v = v.Where(c => c.department_id == department_id.Trim());

                }

                if (division_id != "")
                {
                    v = v.Where(c => c.division_id == division_id.Trim());

                }

                if (status != "" && status != "null")
                {
                    v = v.Where(c => c.process_status == Convert.ToByte(status));

                }



                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date2 >= d_start);
                }


                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date2 <= d_end);
                }


                if (date_start == "" && date_end == "")//default ปีปัจจุบัน
                {

                    v = v.Where(c => c.incident_date2.Value.Year == DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Year);


                }


                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();



                    string step = "";


                    if (rc.process_status != 2 && rc.process_status != 3)//ไม่ใช้ close กับ reject
                    {

                        if (rc.step_form == 1)//supervisor
                        {
                            string v_step = chageDataLanguage("รายงานอุบัติการณ์", "Incident Report", lang);
                            step = step + "(" + v_step + " - Area Supervisor)";

                        }
                        else if (rc.step_form == 2)
                        {
                            string v_step = chageDataLanguage("ตรวจสอบรายงานอุบัติการณ์", "Verify Incident Report", lang);

                            if (rc.submit_report_form2 == null)
                            {
                                step = step + "(" + v_step + " - Area Supervisor)";
                            }

                            if (country == "thailand")
                            {
                                if (rc.submit_report_form2 != null && rc.confirm_investigate_form2 == null)
                                {
                                    step = step + "(" + v_step + " - Area OH&S)";
                                }
                            }
                            else if (country == "srilanka")
                            {
                                if (rc.submit_report_form2 != null && rc.confirm_investigate_form2 == null)
                                {
                                    step = step + "(" + v_step + " - Area Manager)";
                                }

                            }


                            if (rc.confirm_investigate_form2 != null)//กด confirm ไปละ แต่ยังอยู่ 2 แสดงว่ามีซีเรียสเคสรอ group มากด confirm
                            {
                                step = step + "(" + v_step + " - Group OH&S)";
                            }


                        }
                        else if (rc.step_form == 3)
                        {
                            string v_step = chageDataLanguage("สอบสวนและกำหนดมาตรการการแก้ไข", "Investigation and Corrective/Preventive Action", lang);

                            if (country == "thailand")
                            {
                                step = step + "(" + v_step + " - Area OH&S)";
                            }
                            else if (country == "srilanka")
                            {
                                step = step + "(" + v_step + " - Area Manager)";
                            }



                        }
                        else if (rc.step_form == 4)
                        {
                            string v_step = chageDataLanguage("ขอปิดรายงานอุบัติการณ์", "Request to Close Incident Report", lang);

                            bool check_close = true;

                            var s = from c in dbConnect.close_step_incidents
                                    where c.country == country 
                                    orderby c.step descending
                                    select c;

                            foreach (var r in s)
                            {
                                var w = from c in dbConnect.log_request_close_incidents
                                        where c.incident_id == rc.id && c.status == "A"
                                        && c.group_id == r.group_id
                                        select c;


                                if (r.group_id == 4 || r.group_id == 5)
                                {
                                    w = w.Where(c => c.group_id == 4 || c.group_id == 5);
                                }
                                else
                                {
                                    w = w.Where(c => c.group_id == r.group_id);
                                }

                                if (w.Count() == 0)
                                {
                                    check_close = false;


                                    if (r.group_id == 4 || r.group_id == 5)// admin and delegate
                                    {
                                        step = "(" + v_step + " - Admin OH&S or Delegate OH&S Admin)";
                                    }

                                    if (r.group_id == 8)
                                    {
                                        step = "(" + v_step + " - Group OH&S)";
                                    }


                                    if (r.group_id == 9)
                                    {
                                        step = "(" + v_step + " - Area OH&S)";
                                    }


                                    if (r.group_id == 10)// areamanage
                                    {
                                        step = "(" + v_step + " - Area Manager)";
                                    }

                                    if (r.group_id == 11)
                                    {
                                        step = "(" + v_step + " - Area Supervisor)";
                                    }



                                }




                            }//end each


                            if (check_close == true)
                            {// แสดงว่าปิดแล้ว

                                step = "";

                            }



                        }

                    }



                    var lc = new
                    {
                        id = rc.company_id,
                        name = rc.company_name
                    };


                    var lf = new
                    {
                        id = rc.function_id,
                        name = rc.function_name
                    };


                    var lde = new
                    {
                        id = rc.department_id,
                        name = rc.department_name
                    };


                    var ldi = new
                    {
                        id = rc.division_id,
                        name = rc.division_name
                    };

                    var ls = new
                    {
                        id = rc.section_id,
                        name = rc.section_name
                    };

                    string incident_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.incident_date), lang);

                    var result = new
                    {
                        id = rc.id,
                        doc_no = rc.doc_no,
                        incident_name = rc.incident_name,
                        incident_detail = rc.incident_detail,
                        incident_date = incident_date,
                        status = rc.status + " " + step,
                        company = lc,
                        function = lf,
                        department = lde,
                        division = ldi,
                        section = ls,


                    };


                    dataJson.Add(result);


                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));

            }
        }



        [WebMethod]
     
        public void getListAllHazard()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                string company_id = Context.Request["company_id"].ToString();
                string function_id = Context.Request["function_id"].ToString();
                string department_id = Context.Request["department_id"].ToString();
                string division_id = Context.Request["division_id"].ToString();
                string status = Context.Request["status"].ToString();
                string date_start = Context.Request["date_start"].ToString();
                string date_end = Context.Request["date_end"].ToString();
                // string type = Context.Request["type"].ToString();
                string lang = Context.Request["lang"].ToString();

                string country = "";
                string timezone = "";

                if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                {
                    country = "thailand";
                    timezone = "+7";
                }
                else
                {
                    country = Context.Request["country"].ToString();
                    timezone = Context.Request["timezone"].ToString();
                }


                var v = from d in dbConnect.hazards
                        join c in dbConnect.companies on d.company_id equals c.company_id
                        join f in dbConnect.functions on d.function_id equals f.function_id
                        join de in dbConnect.departments on d.department_id equals de.department_id
                        join di in dbConnect.divisions on d.division_id equals di.division_id into joinD
                        join se in dbConnect.sections on d.section_id equals se.section_id into joinS
                        join s in dbConnect.hazard_status on d.process_status equals s.id
                        from di in joinD.DefaultIfEmpty()
                        from se in joinS.DefaultIfEmpty()
                        where d.country == country
                        orderby d.report_date descending

                        select new
                        {
                            d.company_id,
                            d.function_id,
                            d.department_id,
                            d.division_id,
                            d.section_id,
                            company_name = chageDataLanguage(c.company_th, c.company_en, lang),
                            function_name = chageDataLanguage(f.function_th, f.function_en, lang),
                            department_name = chageDataLanguage(de.department_th, de.department_en, lang),
                            division_name = chageDataLanguage(di.division_th, di.division_en, lang),
                            section_name = chageDataLanguage(se.section_th, se.section_en, lang),
                            d.id,
                            d.step_form,
                            d.process_status,
                            d.doc_no,
                            d.hazard_name,
                            d.hazard_detail,
                            hazard_date = d.hazard_date,
                            hazard_date2 = d.hazard_date,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            d.submit_report_form2,



                        };

                if (company_id != "")
                {
                    v = v.Where(c => c.company_id == company_id.Trim());

                }

                if (function_id != "")
                {
                    v = v.Where(c => c.function_id == function_id.Trim());

                }

                if (department_id != "")
                {
                    v = v.Where(c => c.department_id == department_id.Trim());

                }

                if (division_id != "")
                {
                    v = v.Where(c => c.division_id == division_id.Trim());

                }

                if (status != "" && status != "null")
                {
                    v = v.Where(c => c.process_status == Convert.ToByte(status));

                }

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.hazard_date2 >= d_start);
                }


                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.hazard_date2 <= d_end);
                }


                if (date_start == "" && date_end == "")//default ปีปัจจุบัน
                {

                    v = v.Where(c => c.hazard_date2.Value.Year == DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Year);


                }



                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();


                    string step = "";


                    if (rc.process_status != 2 && rc.process_status != 3)//ไม่ใช้ close กับ reject
                    {

                        if (rc.step_form == 1)//area oh&s
                        {
                            string v_step = chageDataLanguage("รายงานแหล่งอันตราย", "Hazard report", lang);

                            if (country == "thailand")
                            {
                                step = step + "(" + v_step + " - Area OH&S)";

                            }
                            else if (country == "srilanka")
                            {
                                step = step + "(" + v_step + " - Area Supervisor)";
                            }


                        }
                        else if (rc.step_form == 2)
                        {
                            string v_step = chageDataLanguage("ตรวจสอบรายงานแหล่งอันตราย", "Verify Hazard Report", lang);

                            if (rc.submit_report_form2 == null)
                            {

                                if (country == "thailand")
                                {
                                    step = step + "(" + v_step + " - Area OH&S)";

                                }
                                else if (country == "srilanka")
                                {
                                    step = step + "(" + v_step + " - Area Supervisor)";

                                }

                            }
                            else
                            {
                                step = step + "(" + v_step + " - Area Supervisor)";
                            }


                        }
                        else if (rc.step_form == 3)
                        {
                            string v_step = chageDataLanguage("ดำเนินการแก้ไข", "Process of Action", lang);

                            step = step + "(" + v_step + " - Area Supervisor)";

                        }
                        else if (rc.step_form == 4)
                        {
                            string v_step = chageDataLanguage("ขอปิดรายงานแหล่งอันตราย", "Request to Close Hazard Report", lang);
                            bool check_close = true;

                            var s = from c in dbConnect.close_step_hazards
                                    where c.country == country
                                    orderby c.step descending
                                    select c;

                            foreach (var r in s)
                            {
                                var w = from c in dbConnect.log_request_close_hazards
                                        where c.hazard_id == rc.id && c.status == "A"
                                        && c.group_id == r.group_id
                                        select c;


                                if (r.group_id == 4 || r.group_id == 5)
                                {
                                    w = w.Where(c => c.group_id == 4 || c.group_id == 5);
                                }
                                else
                                {
                                    w = w.Where(c => c.group_id == r.group_id);
                                }

                                if (w.Count() == 0)
                                {
                                    check_close = false;


                                    if (r.group_id == 4 || r.group_id == 5)// admin and delegate
                                    {
                                        step = "(" + v_step + " - Admin OH&S or Delegate OH&S Admin)";
                                    }

                                    if (r.group_id == 8)
                                    {
                                        step = "(" + v_step + " - Group OH&S)";
                                    }


                                    if (r.group_id == 9)
                                    {
                                        step = "(" + v_step + " - Area OH&S)";
                                    }


                                    if (r.group_id == 10)// areamanage
                                    {
                                        step = "(" + v_step + " - Area Manager)";
                                    }

                                    if (r.group_id == 11)
                                    {
                                        step = "(" + v_step + " - Area Supervisor)";
                                    }


                                    if (r.group_id == 16)
                                    {
                                        step = "(" + v_step + " -  Group OH&S Hazard)";
                                    }


                                }




                            }//end each


                            if (check_close == true)
                            {// แสดงว่าปิดแล้ว

                                step = "";

                            }



                        }

                    }





                    var lc = new
                    {
                        id = rc.company_id,
                        name = rc.company_name
                    };


                    var lf = new
                    {
                        id = rc.function_id,
                        name = rc.function_name
                    };


                    var lde = new
                    {
                        id = rc.department_id,
                        name = rc.department_name
                    };


                    var ldi = new
                    {
                        id = rc.division_id,
                        name = rc.division_name
                    };

                    var ls = new
                    {
                        id = rc.section_id,
                        name = rc.section_name
                    };

                    string hazard_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.hazard_date), lang);

                    var result = new
                    {
                        id = rc.id,
                        doc_no = rc.doc_no,
                        hazard_name = rc.hazard_name,
                        hazard_detail = rc.hazard_detail,
                        hazard_date = hazard_date,
                        status = rc.status + " " + step,
                        company = lc,
                        function = lf,
                        department = lde,
                        division = ldi,
                        section = ls,


                    };

                    dataJson.Add(result);


                }


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));

            }
        }


        [WebMethod]

        public void getListIncidentAction()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string user_id = Context.Request["user_id"].ToString();
                string lang = Context.Request["lang"].ToString();
                string country = "";
                string timezone = "";

                if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                {
                    country = "thailand";
                    timezone = "+7";
                }
                else
                {
                    country = Context.Request["country"].ToString();
                    timezone = Context.Request["timezone"].ToString();
                }



                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        where c.employee_id == user_id && (c.action_status_id == 1
                        || c.action_status_id == 6)//onprocess and reject
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.corrective_preventive_action,
                            c.responsible_person,
                            due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date.ToString()), lang),
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete.ToString(),
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.incident_id,
                            due_date2 = c.due_date

                        };


                ArrayList dataJson = new ArrayList();
                string domain_name = System.Configuration.ConfigurationManager.AppSettings["pathhost"];
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();


                    string path_file = "";
                    string doc_no = "";
                    var d = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(rc.incident_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        doc_no = p.doc_no;

                        if (rc.attachment_file != "" & rc.attachment_file != null)
                        {
                            path_file = domain_name + "/upload/incident/step3/" + country + "/" + doc_no + "/" + rc.attachment_file;

                        }
                        else
                        {
                            path_file = "";

                        }

                    }

                    string status = "";
                    if (rc.action_status_id == 1)//on process
                    {
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }


                    }


                    else if (rc.action_status_id == 4)//close
                    {

                        status = rc.status;

                    }
                    else if (rc.action_status_id == 5)//cancel
                    {

                        status = rc.status;

                    }
                    else if (rc.action_status_id == 6)
                    {//reject


                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else if (rc.action_status_id == 2)
                    {//request close


                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }


                    string date_complete = "";
                    if (!string.IsNullOrEmpty(rc.date_complete))
                    {
                        date_complete = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete), lang);

                    }




                    string action = "";

                    if (rc.action_status_id != 5 && rc.action_status_id != 4 && rc.action_status_id != 2)//cancel ,close and request close
                    {

                        action = "YES";
                    }
                    else
                    {
                        action = "NO";

                    }



                    var result = new
                    {
                        id = rc.id,
                        name = rc.corrective_preventive_action,
                        incident_id = rc.incident_id,
                        rc.action_status_id,
                        doc_no = doc_no,
                        rc.corrective_preventive_action,
                        rc.due_date,
                        date_complete = date_complete,
                        file = path_file,
                        rc.notify_contractor,
                        status = status,
                        remark = rc.remark != null ? "" : rc.remark,
                        action = action



                    };

                    dataJson.Add(result);


                }


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));
            }

        }

        [WebMethod]

        public void getListHazardAction()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string user_id = Context.Request["user_id"].ToString();
                string lang = Context.Request["lang"].ToString();
                string country = "";
                string timezone = "";

                if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                {
                    country = "thailand";
                    timezone = "+7";
                }
                else
                {
                    country = Context.Request["country"].ToString();
                    timezone = Context.Request["timezone"].ToString();
                }



                var v = from c in dbConnect.process_actions
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        where c.employee_id == user_id && (c.action_status_id == 1
                        || c.action_status_id == 6)//onprocess and reject
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.type_control,
                            c.responsible_person,
                            due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date.ToString()), lang),
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete.ToString(),
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.hazard_id,
                            due_date2 = c.due_date,
                            c.action

                        };


                ArrayList dataJson = new ArrayList();
                string domain_name = System.Configuration.ConfigurationManager.AppSettings["pathhost"];
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();


                    string path_file = "";
                    string doc_no = "";
                    var d = from c in dbConnect.hazards
                            where c.id == Convert.ToInt32(rc.hazard_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        doc_no = p.doc_no;

                        if (rc.attachment_file != "" & rc.attachment_file != null)
                        {
                            path_file = domain_name + "/upload/hazard/step3/" + country + "/" + doc_no + "/" + rc.attachment_file;

                        }
                        else
                        {
                            path_file = "";

                        }

                    }

                    string status = "";
                    if (rc.action_status_id == 1)//on process
                    {
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }


                    }


                    else if (rc.action_status_id == 4)//close
                    {

                        status = rc.status;

                    }
                    else if (rc.action_status_id == 5)//cancel
                    {

                        status = rc.status;

                    }
                    else if (rc.action_status_id == 6)
                    {//reject


                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else if (rc.action_status_id == 2)
                    {//request close


                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {

                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }


                    string date_complete = "";
                    if (!string.IsNullOrEmpty(rc.date_complete))
                    {
                        date_complete = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete), lang);

                    }




                    string action = "";

                    if (rc.action_status_id != 5 && rc.action_status_id != 4 && rc.action_status_id != 2)//cancel ,close and request close
                    {

                        action = "YES";
                    }
                    else
                    {
                        action = "NO";

                    }



                    var result = new
                    {
                        id = rc.id,
                        name = rc.action,
                        incident_id = rc.hazard_id,
                        rc.action_status_id,
                        doc_no = doc_no,
                        rc.type_control,
                        rc.due_date,
                        date_complete = date_complete,
                        file = path_file,
                        rc.notify_contractor,
                        status = status,
                        remark = rc.remark != null ? "" : rc.remark,
                        action = action



                    };

                    dataJson.Add(result);


                }


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));
            }

        }



        [WebMethod]
        public void getIncidentbyid()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                string id = Context.Request["id"].ToString();
                string type_login = Context.Request["typelogin"].ToString();
                string user_id = Context.Request["user_id"].ToString();
                string lang = Context.Request["lang"].ToString();
                string country = "";

                if (Context.Request["country"] == null)
                {
                    country = "thailand";

                }
                else
                {
                    country = Context.Request["country"].ToString();

                }

                string mngt_level = "";

                var kk = from em in dbConnect.employees
                         where em.employee_id == user_id
                         select new
                         {

                             em.mngt_level
                         };
                foreach (var z in kk)
                {
                    mngt_level = z.mngt_level;
                }


                var q = from d in dbConnect.incidents
                        join c in dbConnect.companies on d.company_id equals c.company_id
                        join f in dbConnect.functions on d.function_id equals f.function_id
                        join de in dbConnect.departments on d.department_id equals de.department_id
                        join di in dbConnect.divisions on d.division_id equals di.division_id into joinD
                        join se in dbConnect.sections on d.section_id equals se.section_id into joinS
                        join s in dbConnect.incident_status on d.process_status equals s.id
                        join c2 in dbConnect.companies on d.activity_company_id equals c2.company_id  into joinC2
                        join f2 in dbConnect.functions on d.activity_function_id equals f2.function_id  into joinF2
                        join de2 in dbConnect.departments on d.activity_department_id equals de2.department_id into joinDe2
                        join di2 in dbConnect.divisions on d.activity_division_id equals di2.division_id into joinD2
                        join se2 in dbConnect.sections on d.activity_section_id equals se2.section_id into joinS2
           
                        from di in joinD.DefaultIfEmpty()
                        from se in joinS.DefaultIfEmpty()
                        from c2 in joinC2.DefaultIfEmpty()
                        from f2 in joinF2.DefaultIfEmpty()
                        from de2 in joinDe2.DefaultIfEmpty()
                        from di2 in joinD2.DefaultIfEmpty()
                        from se2 in joinS2.DefaultIfEmpty()
                        where d.id == Convert.ToInt32(id)
                        select new
                        {
                            incident_datetime = d.incident_date,
                            report_date = d.report_date,
                            d.company_id,
                            d.function_id,
                            d.department_id,
                            d.division_id,
                            d.section_id,
                            d.activity_company_id,
                            d.activity_function_id,
                            d.activity_department_id,
                            d.activity_division_id,
                            d.activity_section_id,
                            company_name = chageDataLanguage(c.company_th, c.company_en, lang),
                            function_name = chageDataLanguage(f.function_th, f.function_en, lang),
                            department_name = chageDataLanguage(de.department_th, de.department_en, lang),
                            division_name = chageDataLanguage(di.division_th, di.division_en, lang),
                            section_name = chageDataLanguage(se.section_th, se.section_en, lang),
                            activity_company_name = chageDataLanguage(c2.company_th, c2.company_en, lang),
                            activity_function_name = chageDataLanguage(f2.function_th, f2.function_en, lang),
                            activity_department_name = chageDataLanguage(de2.department_th, de2.department_en, lang),
                            activity_division_name = chageDataLanguage(di2.division_th, di2.division_en, lang),
                            activity_section_name = chageDataLanguage(se2.section_th, se2.section_en, lang),
                            d.incident_area,
                            d.incident_name,
                            d.incident_detail,
                            d.employee_id,
                            d.process_status,
                            d.typeuser_login,
                            d.doc_no,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            phone = d.phone,
                            d.work_relate,
                            d.responsible_area,
                            d.owner_activity,
                            d.impact,
                            d.injury_fatality_involve,
                            d.effect_environment,
                            d.level_environment,
                            d.level_damange,
                            d.other_impact,
                            d.critical,
                            d.external_reportable,
                            d.immediate_temporary,
                            d.consequence_level,
                            d.currency,
                            d.culpability,
                            d.road_accident,
                            d.fatality_prevention_element_id,
                            d.faltality_prevention_element_other,
                            d.contributing_factor,
                            d.form2_function_id,
                            d.step_form,
                            d.submit_report_form2,
                            d.confirm_investigate_form2,
                            d.confirm_by_groupohs_form2,
                            d.request_close_form3,
                            d.id

                        };



                foreach (var v in q)
                {
                    //string[] incident_datetime = (v.incident_date.ToString()).Split(' ');
                    string incident_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(v.incident_datetime), lang);
                    string incident_time = FormatDates.getTimeShowFromDate(Convert.ToDateTime(v.incident_datetime), lang);

                    string user_name_modify = "";
                    string datetime_modify = "";

                    var doc_no = dbConnect.incident_details.Max(x => x.id);

                    var d = (from c in dbConnect.incident_details
                             where c.incident_id == Convert.ToInt32(id)
                             orderby c.id descending
                             select c).Take(1);



                    foreach (var g in d)
                    {
                        if (g.type_login == "contractor")
                        {
                            var t = from c in dbConnect.contractors
                                    where c.id == Convert.ToInt32(g.employee_id)
                                    select new
                                    {
                                        prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                        first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                        last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),
                                        action_time = FormatDates.getDatetimeShow(Convert.ToDateTime(g.action_time), lang),

                                    };

                            foreach (var o in t)
                            {
                                user_name_modify = o.prefix + " " + o.first_name + " " + o.last_name;
                                datetime_modify = o.action_time;
                            }

                        }
                        else
                        {
                            var e = from c in dbConnect.employees
                                    where c.employee_id == g.employee_id
                                    select new
                                    {
                                        prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                        first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                        last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),
                                        action_time = FormatDates.getDatetimeShow(Convert.ToDateTime(g.action_time), lang),

                                    };

                            foreach (var o in e)
                            {
                                user_name_modify = o.prefix + " " + o.first_name + " " + o.last_name;
                                datetime_modify = o.action_time;

                            }


                        }

                    }//end for each




                    string step = "";


                    if (v.process_status != 2 && v.process_status != 3)//ไม่ใช้ close กับ reject
                    {

                        if (v.step_form == 1)//supervisor
                        {
                            string v_step = chageDataLanguage("รายงานอุบัติการณ์", "Incident Report", lang);
                            step = step + "(" + v_step + " - Area Supervisor)";

                        }
                        else if (v.step_form == 2)
                        {
                            string v_step = chageDataLanguage("ตรวจสอบรายงานอุบัติการณ์", "Verify Incident Report", lang);

                            if (v.submit_report_form2 == null)
                            {
                                step = step + "(" + v_step + " - Area Supervisor)";
                            }

                            if (country == "thailand")
                            {
                                if (v.submit_report_form2 != null && v.confirm_investigate_form2 == null)
                                {
                                    step = step + "(" + v_step + " - Area OH&S)";
                                }
                            }
                            else if (country == "srilanka")
                            {
                                if (v.submit_report_form2 != null && v.confirm_investigate_form2 == null)
                                {
                                    step = step + "(" + v_step + " - Area Manager)";
                                }

                            }


                            if (v.confirm_investigate_form2 != null)//กด confirm ไปละ แต่ยังอยู่ 2 แสดงว่ามีซีเรียสเคสรอ group มากด confirm
                            {
                                step = step + "(" + v_step + " - Group OH&S)";
                            }


                        }
                        else if (v.step_form == 3)
                        {
                            string v_step = chageDataLanguage("สอบสวนและกำหนดมาตรการการแก้ไข", "Investigation and Corrective/Preventive Action", lang);

                            if (country == "thailand")
                            {
                                step = step + "(" + v_step + " - Area OH&S)";
                            }
                            else if (country == "srilanka")
                            {
                                step = step + "(" + v_step + " - Area Manager)";
                            }



                        }
                        else if (v.step_form == 4)
                        {
                            string v_step = chageDataLanguage("ขอปิดรายงานอุบัติการณ์", "Request to Close Incident Report", lang);

                            bool check_close = true;

                            var s = from c in dbConnect.close_step_incidents
                                    where c.country == country
                                    orderby c.step descending
                                    select c;

                            foreach (var r in s)
                            {
                                var w = from c in dbConnect.log_request_close_incidents
                                        where c.incident_id == v.id && c.status == "A"
                                        && c.group_id == r.group_id
                                        select c;


                                if (r.group_id == 4 || r.group_id == 5)
                                {
                                    w = w.Where(c => c.group_id == 4 || c.group_id == 5);
                                }
                                else
                                {
                                    w = w.Where(c => c.group_id == r.group_id);
                                }

                                if (w.Count() == 0)
                                {
                                    check_close = false;


                                    if (r.group_id == 4 || r.group_id == 5)// admin and delegate
                                    {
                                        step = "(" + v_step + " - Admin OH&S or Delegate OH&S Admin)";
                                    }

                                    if (r.group_id == 8)
                                    {
                                        step = "(" + v_step + " - Group OH&S)";
                                    }


                                    if (r.group_id == 9)
                                    {
                                        step = "(" + v_step + " - Area OH&S)";
                                    }


                                    if (r.group_id == 10)// areamanage
                                    {
                                        step = "(" + v_step + " - Area Manager)";
                                    }

                                    if (r.group_id == 11)
                                    {
                                        step = "(" + v_step + " - Area Supervisor)";
                                    }


                                }




                            }//end each


                            if (check_close == true)
                            {// แสดงว่าปิดแล้ว

                                step = "";

                            }



                        }

                    }





                    ArrayList im = new ArrayList();
                    var i = from c in dbConnect.other_impacts
                            where c.incident_id == Convert.ToInt32(id)
                            select new
                            {
                                c.other_impact_value

                            };

                    foreach (var m in i)
                    {
                        im.Add(m.other_impact_value);
                    }



                    ArrayList cr = new ArrayList();
                    var z = from c in dbConnect.root_cause_incidents
                            where c.incident_id == Convert.ToInt32(id)
                            select new
                            {
                                c.root_cause

                            };

                    foreach (var m in z)
                    {
                        cr.Add(m.root_cause);
                    }

                    int group_login = 0;
                    if (type_login == "employee")
                    {
                        group_login = 12;

                    }
                    else if (type_login == "ad")
                    {

                        group_login = 13;

                    }
                    else if (type_login == "contracter")
                    {
                        group_login = 14;
                    }

                    string[] split_incident_time = incident_time.Split(':');
                    var result = new
                    {
                        incident_date = incident_date,
                        incident_hour = split_incident_time[0],
                        incident_minute = split_incident_time[1],
                        incident_report = FormatDates.getDatetimeShow(Convert.ToDateTime(v.report_date), lang),

                        company_id = v.company_id,
                        function_id = v.function_id,
                        department_id = v.department_id,
                        division_id = v.division_id,
                        section_id = v.section_id,
                        company_name = v.company_name,
                        function_name = v.function_name,
                        department_name = v.department_name,
                        division_name = v.division_name,
                        section_name = v.section_name,
                        activity_company_id = v.activity_company_id,
                        activity_function_id = v.activity_function_id,
                        activity_department_id = v.activity_department_id,
                        activity_division_id = v.activity_division_id,
                        activity_section_id = v.activity_section_id,
                        activity_company_name = v.activity_company_name,
                        activity_function_name = v.activity_function_name,
                        activity_department_name = v.activity_department_name,
                        activity_division_name = v.activity_division_name,
                        activity_section_name = v.activity_section_name,
                        incident_area = v.incident_area,
                        incident_name = v.incident_name,
                        incident_detail = v.incident_detail,
                        name_modify = user_name_modify,
                        datetime_modify = datetime_modify,
                        phone = v.phone,
                        status = v.status + " " + step,
                        employee_name = getEmployeeByTypeLogin(v.employee_id, v.typeuser_login, lang),
                        doc_no = v.doc_no,
                        step_form = v.step_form,
                        images = getImageIncident(id),
                        bt_verify_form1 = getPermission(group_login, user_id, mngt_level, "report incident1 verify", id, "incident", "",country),
                        bt_reject_form1 = getPermission(group_login, user_id, mngt_level, "report incident1 reject", id, "incident", "", country),
                        bt_edit_form1 = getPermission(group_login, user_id, mngt_level, "report incident1 edit", id, "incident", "", country),
                        view_form1 = getPermission(group_login, user_id, mngt_level, "report incident1 view", id, "incident", "view", country),
                        // v.work_relate,
                         v.responsible_area,
                         v.owner_activity,
                        // v.impact,
                        // v.injury_fatality_involve,
                        // v.effect_environment,
                        //  v.level_environment,
                        //  v.level_damange,
                        // other_impact = im,
                        //  v.critical,
                        //  v.external_reportable,
                        // v.immediate_temporary,
                        //  v.consequence_level,
                        //  v.currency,
                        //  v.culpability,
                        //  v.road_accident,
                        // v.fatality_prevention_element_id,
                        //  v.faltality_prevention_element_other,
                        // root_cause = cr,
                        // v.contributing_factor,
                        // v.form2_function_id



                    };

                    ArrayList dt = new ArrayList();
                    dt.Add(result);


                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(dt));

                }
            }

        }



        [WebMethod]
        public void getHazardbyid()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string type_login = Context.Request["typelogin"].ToString();
                string id = Context.Request["id"].ToString();
                string user_id = Context.Request["user_id"].ToString();
                string lang = Context.Request["lang"].ToString();
                string country = "";
                string timezone = "";

                if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                {
                    country = "thailand";
                    timezone = "+7";
                }
                else
                {
                    country = Context.Request["country"].ToString();
                    timezone = Context.Request["timezone"].ToString();
                }

                string mngt_level = "";

                var kk = from em in dbConnect.employees
                         where em.employee_id == user_id
                         select new
                         {

                             em.mngt_level
                         };
                foreach (var z in kk)
                {
                    mngt_level = z.mngt_level;
                }

                var q = from d in dbConnect.hazards
                        join c in dbConnect.companies on d.company_id equals c.company_id
                        join f in dbConnect.functions on d.function_id equals f.function_id
                        join de in dbConnect.departments on d.department_id equals de.department_id
                        join di in dbConnect.divisions on d.division_id equals di.division_id into joinD
                        join se in dbConnect.sections on d.section_id equals se.section_id into joinS
                        join s in dbConnect.hazard_status on d.process_status equals s.id
                        join so in dbConnect.source_hazards on d.source_hazard equals so.id into joinSo
                        join fa in dbConnect.fatality_prevention_elements on d.fatality_prevention_element_id equals fa.id into joinFa

                        from di in joinD.DefaultIfEmpty()
                        from se in joinS.DefaultIfEmpty()
                        from so in joinSo.DefaultIfEmpty()
                        from fa in joinFa.DefaultIfEmpty()

                        where d.id == Convert.ToInt32(id)
                        select new
                        {
                            hazard_datetime = d.hazard_date,
                            report_date = d.report_date,
                            d.company_id,
                            d.function_id,
                            d.department_id,
                            d.division_id,
                            d.section_id,
                            company_name = chageDataLanguage(c.company_th, c.company_en, lang),
                            function_name = chageDataLanguage(f.function_th, f.function_en, lang),
                            department_name = chageDataLanguage(de.department_th, de.department_en, lang),
                            division_name = chageDataLanguage(di.division_th, di.division_en, lang),
                            section_name = chageDataLanguage(se.section_th, se.section_en, lang),
                            d.hazard_area,
                            d.hazard_name,
                            d.hazard_detail,
                            d.preliminary_action,
                            d.type_action,
                            d.employee_id,
                            d.process_status,
                            d.typeuser_login,
                            d.doc_no,
                            d.verifying_date,
                            d.source_hazard,
                            d.level_hazard,
                            d.safety_officer_id,
                            d.area_owner_id,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            phone = d.phone,
                            d.fatality_prevention_element_id,
                            d.faltality_prevention_element_other,
                            d.step_form,
                            d.submit_report_form2,
                            source_hazard_name = chageDataLanguage(so.name_th, so.name_en, lang),
                            fpe_name = chageDataLanguage(fa.name_th, fa.name_en, lang),
                            d.id,
                            d.hazard_characteristic_id




                        };



                foreach (var v in q)
                {
                    //string[] incident_datetime = (v.incident_date.ToString()).Split(' ');
                    string hazard_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(v.hazard_datetime), lang);
                    string hazard_time = FormatDates.getTimeShowFromDate(Convert.ToDateTime(v.hazard_datetime), lang);

                    string user_name_modify = "";
                    string datetime_modify = "";

                    var doc_no = dbConnect.hazard_details.Max(x => x.id);

                    var d = (from c in dbConnect.hazard_details
                             where c.hazard_id == Convert.ToInt32(id)
                             orderby c.id descending
                             select c).Take(1);



                    foreach (var g in d)
                    {
                        if (g.type_login == "contractor")
                        {
                            var t = from c in dbConnect.contractors
                                    where c.id == Convert.ToInt32(g.employee_id)
                                    select new
                                    {
                                        prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                        first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                        last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),
                                        action_time = FormatDates.getDatetimeShow(Convert.ToDateTime(g.action_time), lang),
                                    };

                            foreach (var o in t)
                            {
                                user_name_modify = o.prefix + " " + o.first_name + " " + o.last_name;
                                datetime_modify = o.action_time;
                            }

                        }
                        else
                        {
                            var e = from c in dbConnect.employees
                                    where c.employee_id == g.employee_id
                                    select new
                                    {
                                        prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                        first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                        last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),
                                        action_time = FormatDates.getDatetimeShow(Convert.ToDateTime(g.action_time), lang),
                                    };

                            foreach (var o in e)
                            {
                                user_name_modify = o.prefix + " " + o.first_name + " " + o.last_name;
                                datetime_modify = o.action_time;
                            }


                        }

                    }//end for each


                    string fullname_security = "";
                    if (v.safety_officer_id != null & v.safety_officer_id != "")
                    {
                        var es = from c in dbConnect.employees
                                 where c.employee_id == v.safety_officer_id
                                 select new
                                 {
                                     prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                     first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                     last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang)

                                 };

                        foreach (var b in es)
                        {
                            fullname_security = b.prefix + " " + b.first_name + " " + b.last_name;

                        }
                    }
                    else
                    {

                        var es = from c in dbConnect.employees
                                 where c.employee_id == user_id
                                 select new
                                 {
                                     prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                     first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                     last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang)

                                 };

                        foreach (var b in es)
                        {
                            fullname_security = b.prefix + " " + b.first_name + " " + b.last_name;

                        }


                    }

                    string fullname_area_owner = "";
                    if (v.area_owner_id != null & v.area_owner_id != "")
                    {
                        var es = from c in dbConnect.employees
                                 where c.employee_id == v.area_owner_id
                                 select new
                                 {
                                     prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                     first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                     last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang)

                                 };

                        foreach (var b in es)
                        {
                            fullname_area_owner = b.prefix + " " + b.first_name + " " + b.last_name;

                        }
                    }
                    else
                    {
                        var es = from c in dbConnect.employees
                                 where c.employee_id == user_id
                                 select new
                                 {
                                     prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                     first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                     last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang)

                                 };

                        foreach (var b in es)
                        {
                            fullname_area_owner = b.prefix + " " + b.first_name + " " + b.last_name;

                        }

                    }





                    string step = "";


                    if (v.process_status != 2 && v.process_status != 3)//ไม่ใช้ close กับ reject
                    {

                        if (v.step_form == 1)//area oh&s
                        {
                            string v_step = chageDataLanguage("รายงานแหล่งอันตราย", "Hazard report", lang);

                            if (country == "thailand")
                            {
                                step = step + "(" + v_step + " - Area OH&S)";

                            }
                            else if (country == "srilanka")
                            {
                                step = step + "(" + v_step + " - Area Supervisor)";
                            }


                        }
                        else if (v.step_form == 2)
                        {
                            string v_step = chageDataLanguage("ตรวจสอบรายงานแหล่งอันตราย", "Verify Hazard Report", lang);

                            if (v.submit_report_form2 == null)
                            {

                                if (country == "thailand")
                                {
                                    step = step + "(" + v_step + " - Area OH&S)";

                                }
                                else if (country == "srilanka")
                                {
                                    step = step + "(" + v_step + " - Area Supervisor)";

                                }

                            }
                            else
                            {
                                step = step + "(" + v_step + " - Area Supervisor)";
                            }


                        }
                        else if (v.step_form == 3)
                        {
                            string v_step = chageDataLanguage("ดำเนินการแก้ไข", "Process of Action", lang);

                            step = step + "(" + v_step + " - Area Supervisor)";

                        }
                        else if (v.step_form == 4)
                        {
                            string v_step = chageDataLanguage("ขอปิดรายงานแหล่งอันตราย", "Request to Close Hazard Report", lang);
                            bool check_close = true;

                            var s = from c in dbConnect.close_step_hazards
                                    where c.country == country
                                    orderby c.step descending
                                    select c;

                            foreach (var r in s)
                            {
                                var w = from c in dbConnect.log_request_close_hazards
                                        where c.hazard_id == v.id && c.status == "A"
                                        && c.group_id == r.group_id
                                        select c;


                                if (r.group_id == 4 || r.group_id == 5)
                                {
                                    w = w.Where(c => c.group_id == 4 || c.group_id == 5);
                                }
                                else
                                {
                                    w = w.Where(c => c.group_id == r.group_id);
                                }

                                if (w.Count() == 0)
                                {
                                    check_close = false;


                                    if (r.group_id == 4 || r.group_id == 5)// admin and delegate
                                    {
                                        step = "(" + v_step + " - Admin OH&S or Delegate OH&S Admin)";
                                    }

                                    if (r.group_id == 8)
                                    {
                                        step = "(" + v_step + " - Group OH&S)";
                                    }


                                    if (r.group_id == 9)
                                    {
                                        step = "(" + v_step + " - Area OH&S)";
                                    }


                                    if (r.group_id == 10)// areamanage
                                    {
                                        step = "(" + v_step + " - Area Manager)";
                                    }

                                    if (r.group_id == 11)
                                    {
                                        step = "(" + v_step + " - Area Supervisor)";
                                    }


                                    if (r.group_id == 16)
                                    {
                                        step = "(" + v_step + " -  Group OH&S Hazard)";
                                    }


                                }




                            }//end each


                            if (check_close == true)
                            {// แสดงว่าปิดแล้ว

                                step = "";

                            }



                        }

                    }

                    int group_login = 0;
                    if (type_login == "employee")
                    {
                        group_login = 12;

                    }
                    else if (type_login == "ad")
                    {

                        group_login = 13;

                    }
                    else if (type_login == "contracter")
                    {
                        group_login = 14;
                    }

                    string type_action_name = "";

                    if (v.type_action == "P")
                    {
                        type_action_name = chageDataLanguage("ยังไม่สามารถแก้ไขได้ทันที", "Pending for action", lang);
                    }
                    else if (v.type_action == "T")
                    {
                        type_action_name = chageDataLanguage("เป็นการป้องกันแบบชั่วคราว", "Temporary control", lang);
                    }
                    else if (v.type_action == "T")
                    {
                        type_action_name = chageDataLanguage("ขจัดและป้องกันอันตรายแล้ว", "Complete control", lang);
                    }


                    string level_name = "";

                    if (v.level_hazard == "H")
                    {
                        level_name = chageDataLanguage("สูง", "High", lang);
                    }
                    else if (v.level_hazard == "M")
                    {
                        level_name = chageDataLanguage("กลาง", "Medium", lang);
                    }
                    else if (v.level_hazard == "L")
                    {
                        level_name = chageDataLanguage("ต่ำ", "Low", lang);
                    }

                    string[] split_hazard_time = hazard_time.Split(':');
                    var result = new
                    {
                        hazard_date = hazard_date,
                        hazard_hour = split_hazard_time[0],
                        hazard_minute = split_hazard_time[1],
                        hazard_report = FormatDates.getDatetimeShow(Convert.ToDateTime(v.report_date), lang),

                        company_id = v.company_id,
                        function_id = v.function_id,
                        department_id = v.department_id,
                        division_id = v.division_id,
                        section_id = v.section_id,
                        company_name = v.company_name,
                        function_name = v.function_name,
                        department_name = v.department_name,
                        division_name = v.division_name,
                        section_name = v.section_name,
                        hazard_area = v.hazard_area,
                        hazard_name = v.hazard_name,
                        hazard_detail = v.hazard_detail,
                        preliminary_action = v.preliminary_action,
                        type_action = v.type_action,
                        type_action_name = type_action_name,
                        name_modify = user_name_modify,
                        datetime_modify = datetime_modify,
                        characteristic_id = v.hazard_characteristic_id,
                        phone = v.phone,
                        status = v.status + " " + step,
                        step_form = v.step_form,
                        employee_name = getEmployeeByTypeLogin(v.employee_id, v.typeuser_login, lang),
                        doc_no = v.doc_no,
                        source_hazard = v.source_hazard == null ? 0 : v.source_hazard,
                        v.source_hazard_name,
                        verifying_date = v.verifying_date == null ? FormatDates.getDateShowFromDate(DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)), lang) : FormatDates.getDateShowFromDate(Convert.ToDateTime(v.verifying_date), lang),
                        level_hazard = v.level_hazard == null ? "" : v.level_hazard,
                        level_hazard_name = level_name,
                        name_security = fullname_security,
                        name_area_owner = fullname_area_owner,
                        fatality_prevention_element_id = v.fatality_prevention_element_id == null ? 0 : v.fatality_prevention_element_id,
                        v.fpe_name,
                        faltality_prevention_element_other = v.faltality_prevention_element_other == null ? "" : v.faltality_prevention_element_other,
                        images = getImageHazard(id),
                        bt_verify_form1 = getPermission(group_login, user_id, mngt_level, "report hazard1 verify", id, "hazard", "", country),
                        bt_reject_form1 = getPermission(group_login, user_id, mngt_level, "report hazard1 reject", id, "hazard", "", country),
                        bt_edit_form1 = getPermission(group_login, user_id, mngt_level, "report hazard1 edit", id, "hazard", "", country),
                        bt_process_form2 = getPermission(group_login, user_id, mngt_level, "report hazard2 process", id, "hazard", "", country),
                        bt_reject_form2 = getPermission(group_login, user_id, mngt_level, "report hazard2 reject", id, "hazard", "", country),
                        bt_submit_form2 = getPermission(group_login, user_id, mngt_level, "report hazard2 submit", id, "hazard", "", country),
                        bt_edit_form2 = getPermission(group_login, user_id, mngt_level, "report hazard2 edit", id, "hazard", "", country),
                        view_form1 = getPermission(group_login, user_id, mngt_level, "report hazard1 view", id, "hazard", "view", country),
                        view_form2 = getPermission(group_login, user_id, mngt_level, "report hazard2 view", id, "hazard", "view", country),

                    };

                    ArrayList dt = new ArrayList();
                    dt.Add(result);


                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(dt));

                }

            }


        }

     
        public ArrayList getImageHazard(string id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string domain_name = System.Configuration.ConfigurationManager.AppSettings["pathhost"];
                ArrayList ls = new ArrayList();

                try
                {

                    var q = from c in dbConnect.hazards
                            where c.id == Convert.ToInt32(id)
                            select new
                            {
                                user_id = c.employee_id,
                                reportdate = c.report_date

                            };

                    foreach (var s in q)
                    {
                        string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathuploadhazard"];
                        string name_folder = s.user_id + "_" + s.reportdate.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB"));
                       // string pathfolder = string.Format("{0}\\upload\\hazard\\" + name_folder, Server.MapPath(@"\"));
                        string pathfolder = string.Format("{0}"+ pathupload + name_folder, Server.MapPath(@"\"));





                        string[] images = Directory.GetFiles(pathfolder, "*")
                                                 .Select(Path.GetFileName).OrderByDescending(Path.GetFileName).Take(2)
                                                 .ToArray();

                        foreach (var d in images)
                        {
                            var v = new Dictionary<string, string>
                           {
                               { "name", domain_name+"/upload/hazard/"+name_folder+"/"+d },
                
                           };

                            ls.Add(v);

                        }


                    }


                }
                catch (Exception ex)
                {

                }


                return ls;
            }

        }

        public ArrayList getImageIncident(string id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string domain_name = System.Configuration.ConfigurationManager.AppSettings["pathhost"];
                ArrayList ls = new ArrayList();

                try
                {
                    var q = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(id)
                            select new
                            {
                                user_id = c.employee_id,
                                reportdate = c.report_date

                            };

                    foreach (var s in q)
                    {
                        string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathupload"];
                        string name_folder = s.user_id + "_" + s.reportdate.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB"));
                        //string pathfolder = string.Format("{0}\\upload\\incident\\" + name_folder, Server.MapPath(@"\"));
                        string pathfolder = string.Format("{0}"+ pathupload + name_folder, Server.MapPath(@"\"));





                        string[] images = Directory.GetFiles(pathfolder, "*")
                                                 .Select(Path.GetFileName).OrderByDescending(Path.GetFileName).Take(5)
                                                 .ToArray();

                        foreach (var d in images)
                        {
                            var v = new Dictionary<string, string>
                           {
                               { "name", domain_name+"/upload/incident/"+name_folder+"/"+d },
                
                           };

                            ls.Add(v);

                        }


                    }
                }
                catch (Exception ex)
                {

                }

                return ls;
            }

        }



        [WebMethod]
   
        public void updateHazard()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                string error = "";
                string id = "";

                try
                {
                    string hazardid = Context.Request["hazardid"].ToString();
                    string hazarddate = Context.Request["hazarddate"].ToString();
                    string hazardtime = Context.Request["hazardtime"].ToString();
                    string reportdate = Context.Request["reportdate"].ToString();
                    string company_id = Context.Request["company_id"].ToString();
                    string function_id = Context.Request["function_id"].ToString();
                    string department_id = Context.Request["department_id"].ToString();
                    string division_id = Context.Request["division_id"].ToString();
                    string section_id = Context.Request["section_id"].ToString();
                    string hazardarea = Context.Request["hazardarea"].ToString();
                    string hazardname = Context.Request["hazardname"].ToString();
                    string hazarddetail = Context.Request["hazarddetail"].ToString();
                    string preliminary_action = Context.Request["preliminary_action"].ToString();
                    string type_action = Context.Request["type_action"].ToString();
                    string userid = Context.Request["userid"].ToString();
                    string typelogin = Context.Request["typelogin"].ToString();
                    string phone = Context.Request["phone"].ToString();
                    string lang = Context.Request["lang"].ToString();
                    string group_id = Context.Request["group_id"].ToString();
                    string user_id = Context.Request["user_id"].ToString();

                    string country = "";
                    string timezone = "";

                    if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                    {
                        country = "thailand";
                        timezone = "+7";
                    }
                    else
                    {
                        country = Context.Request["country"].ToString();
                        timezone = Context.Request["timezone"].ToString();
                    }


                    bool change_area = false;

                    string[] alert_to_groups = new string[3];

                    var query = from c in dbConnect.hazards
                                where c.id == Convert.ToInt32(hazardid)
                                select c;

                    foreach (hazard rc in query)
                    {

                        rc.hazard_date = FormatDates.changeDateTimeDB(hazarddate + " " + hazardtime, lang);
                        rc.report_date = FormatDates.changeDateTimeDB(reportdate, lang);
                        rc.company_id = company_id;

                        if (rc.function_id != function_id || rc.department_id != department_id
                            || rc.division_id != division_id || rc.section_id != section_id)
                        {

                            alert_to_groups[0] = "AreaOH&S";
                            alert_to_groups[1] = "AreaSuperervisor";
                            // alert_to_groups[2] = "GroupOH&SHazard";

                            if (country == "srilanka")
                            {
                                alert_to_groups[2] = "AreaManager";

                            }
                            else if (country == "thailand")
                            {
                                alert_to_groups[2] = "";
                            }
                            change_area = true;
                        }
                        else
                        {
                            alert_to_groups[0] = "";
                            alert_to_groups[1] = "";
                            alert_to_groups[2] = "";
                            //  alert_to_groups[2] = "";
                        }

                        rc.function_id = function_id;

                        rc.department_id = department_id;

                        rc.division_id = division_id;


                        rc.section_id = section_id;


                        rc.hazard_area = hazardarea;
                        rc.hazard_name = hazardname;
                        rc.hazard_detail = hazarddetail;
                        rc.preliminary_action = preliminary_action;
                        rc.type_action = type_action;
                        rc.phone = phone;
                        rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                        rc.edit_form1 = Convert.ToInt32(group_id);

                        rc.location_company_id = company_id;
                        rc.location_function_id = function_id;
                        rc.location_department_id = department_id;
                        rc.location_division_id = division_id;
                        rc.location_section_id = section_id;

                        string[] dt = new string[2];
                        dt = (string[])getMasterdataName(company_id, "company");
                        rc.location_company_name_th = dt[0];
                        rc.location_company_name_en = dt[1];

                        dt = (string[])getMasterdataName(function_id, "function");
                        rc.location_function_name_th = dt[0];
                        rc.location_function_name_en = dt[1];

                        dt = (string[])getMasterdataName(department_id, "department");
                        rc.location_department_name_th = dt[0];
                        rc.location_department_name_en = dt[1];

                        dt = (string[])getMasterdataName(division_id, "division");
                        rc.location_division_name_th = dt[0];
                        rc.location_division_name_en = dt[1];

                        dt = (string[])getMasterdataName(section_id, "section");
                        rc.location_section_name_th = dt[0];
                        rc.location_section_name_en = dt[1];



                    }


                    dbConnect.SubmitChanges();
                    MobileUploadedFileHazard(HttpContext.Current.Request.Files, userid, reportdate, lang,timezone);//upload image

                    int process_status = 1;//on process
                    hazard_detail objInsert2 = new hazard_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert2.process_status = process_status;
                    objInsert2.hazard_id = Convert.ToInt32(hazardid);

                    dbConnect.hazard_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();



                    if (change_area == true)//change area new to sent notification
                    {
                        ///////////////////////////sent notify by change area////////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        if (country == "srilanka")
                        {
                            sn.InsertHazardNotification(1, Convert.ToInt32(hazardid), alert_to_groups, timezone, "AreaSuperervisor");
                        }
                        else if (country == "thailand")
                        {

                            sn.InsertHazardNotification(1, Convert.ToInt32(hazardid), alert_to_groups, timezone, "AreaOH&S");
                        }
                      
                        ////////////////////////////////////end/////////////////////////////////////////////////
                    }

                    result = "success";

                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    id = id

                };

                ArrayList dt2 = new ArrayList();
                dt2.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt2));
            }

        }


        [WebMethod]
        public void updateIncident()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                string error = "";
                string id = "";

                try
                {
                    string incidentid = Context.Request["incidentid"].ToString();
                    string company_id = Context.Request["company_id"].ToString();
                    string incidentdate = Context.Request["incidentdate"].ToString();
                    string incidenttime = Context.Request["incidenttime"].ToString();
                    string reportdate = Context.Request["reportdate"].ToString();

                    string function_id = Context.Request["function_id"].ToString();
                    string department_id = Context.Request["department_id"].ToString();
                    string division_id = Context.Request["division_id"].ToString();
                    string section_id = Context.Request["section_id"].ToString();
                    string incidentarea = Context.Request["incidentarea"].ToString();
                    string incidentname = Context.Request["incidentname"].ToString();
                    string incidentdetail = Context.Request["incidentdetail"].ToString();
                    string user_id = Context.Request["user_id"].ToString();
                    string typelogin = Context.Request["typelogin"].ToString();
                    string phone = Context.Request["phone"].ToString();
                    string lang = Context.Request["lang"].ToString();
                    string group_id = Context.Request["group_id"].ToString();

                    string country = "";
                    string timezone = "";

                    if (Context.Request["country"] == null || Context.Request["timezone"] == null)
                    {
                        country = "thailand";
                        timezone = "+7";
                    }
                    else
                    {
                        country = Context.Request["country"].ToString();
                        timezone = Context.Request["timezone"].ToString();
                    }

                    bool change_area = false;
                    string[] alert_to_groups = new string[4];

                    var query = from c in dbConnect.incidents
                                where c.id == Convert.ToInt32(incidentid)
                                select c;

                    foreach (incident rc in query)
                    {

                        rc.incident_date = FormatDates.changeDateTimeDB(incidentdate + " " + incidenttime, lang);
                        rc.report_date = FormatDates.changeDateTimeDB(reportdate, lang);
                        rc.company_id = company_id;

                        if (rc.function_id != function_id)
                        {
                            alert_to_groups[0] = "AdminOH&S";
                            change_area = true;
                        }
                        else
                        {
                            alert_to_groups[0] = "";

                        }
                        rc.function_id = function_id;


                        if (rc.department_id != department_id)
                        {
                            alert_to_groups[1] = "AreaOH&S";
                            change_area = true;
                        }
                        else
                        {
                            alert_to_groups[1] = "";

                        }
                        rc.department_id = department_id;


                        if (rc.division_id != division_id)
                        {
                            alert_to_groups[2] = "AreaManager";
                            change_area = true;
                        }
                        else
                        {
                            alert_to_groups[2] = "";

                        }
                        rc.division_id = division_id;


                        if (rc.section_id != section_id)
                        {
                            alert_to_groups[3] = "AreaSuperervisor";
                            change_area = true;
                        }
                        else
                        {
                            alert_to_groups[3] = "";

                        }
                        rc.section_id = section_id;


                        rc.incident_area = incidentarea;
                        rc.incident_name = incidentname;
                        rc.incident_detail = incidentdetail;
                        rc.phone = phone;
                        rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                        rc.edit_form1 = Convert.ToInt32(group_id);

                        rc.location_company_id = company_id;
                        rc.location_function_id = function_id;
                        rc.location_department_id = department_id;
                        rc.location_division_id = division_id;
                        rc.location_section_id = section_id;

                        string[] dt = new string[2];
                        dt = (string[])getMasterdataName(company_id, "company");
                        rc.location_company_name_th = dt[0];
                        rc.location_company_name_en = dt[1];

                        dt = (string[])getMasterdataName(function_id, "function");
                        rc.location_function_name_th = dt[0];
                        rc.location_function_name_en = dt[1];

                        dt = (string[])getMasterdataName(department_id, "department");
                        rc.location_department_name_th = dt[0];
                        rc.location_department_name_en = dt[1];

                        dt = (string[])getMasterdataName(division_id, "division");
                        rc.location_division_name_th = dt[0];
                        rc.location_division_name_en = dt[1];

                        dt = (string[])getMasterdataName(section_id, "section");
                        rc.location_section_name_th = dt[0];
                        rc.location_section_name_en = dt[1];

                        if (country == "thailand")
                        {
                            string responsible_area = Context.Request["responsible_area"].ToString();
                            string owner_activity = Context.Request["owner_activity"].ToString();

                            string activity_company_id = Context.Request["activity_company_id"].ToString();
                            string activity_function_id = Context.Request["activity_function_id"].ToString();
                            string activity_department_id = Context.Request["activity_department_id"].ToString();
                            string activity_division_id = Context.Request["activity_division_id"].ToString();
                            string activity_section_id = Context.Request["activity_section_id"].ToString();

                            rc.responsible_area = responsible_area;
                            rc.owner_activity = owner_activity;

                            rc.activity_company_id = activity_company_id;
                            rc.activity_function_id = activity_function_id;
                            rc.activity_department_id = activity_department_id;
                            rc.activity_division_id = activity_division_id;
                            rc.activity_section_id = activity_section_id;

                            rc.activity_location_company_id = activity_company_id;
                            rc.activity_location_function_id = activity_function_id;
                            rc.activity_location_department_id = activity_department_id;
                            rc.activity_location_division_id = activity_division_id;
                            rc.activity_location_section_id = activity_section_id;

                            string[] dt2 = new string[2];
                            dt2 = (string[])getMasterdataName(activity_company_id, "company");
                            rc.activity_location_company_name_th = dt2[0];
                            rc.activity_location_company_name_en = dt2[1];

                            dt2 = (string[])getMasterdataName(activity_function_id, "function");
                            rc.activity_location_function_name_th = dt2[0];
                            rc.activity_location_function_name_en = dt2[1];

                            dt2 = (string[])getMasterdataName(activity_department_id, "department");
                            rc.activity_location_department_name_th = dt2[0];
                            rc.activity_location_department_name_en = dt2[1];

                            dt2 = (string[])getMasterdataName(activity_division_id, "division");
                            rc.activity_location_division_name_th = dt2[0];
                            rc.activity_location_division_name_en = dt2[1];

                            dt2 = (string[])getMasterdataName(activity_section_id, "section");
                            rc.activity_location_section_name_th = dt2[0];
                            rc.activity_location_section_name_en = dt2[1];

                        }


                    }


                    dbConnect.SubmitChanges();

                    MobileUploadedFileIncident(HttpContext.Current.Request.Files, user_id, reportdate, lang,timezone);//upload image




                    int process_status = 1;//on process
                    byte incident_flow = 1;
                    incident_detail objInsert2 = new incident_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert2.process_status = process_status;
                    objInsert2.incident_id = Convert.ToInt32(incidentid);

                    dbConnect.incident_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();



                    if (change_area == true)//change area new to sent notification
                    {
                        ///////////////////////////sent notify by change area////////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        sn.InsertNotification(incident_flow, Convert.ToInt32(incidentid), alert_to_groups, timezone, "AreaSuperervisor");
                        ////////////////////////////////////end/////////////////////////////////////////////////
                    }

                    result = "success";
                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    id = id

                };

                ArrayList dt3 = new ArrayList();
                dt3.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt3));

            }
        }




        public string getEmployeeByTypeLogin(string employee_id, string type_login, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string name = "";

                if (type_login == "contractor")
                {
                    var t = from c in dbConnect.contractors
                            where c.id == Convert.ToInt32(employee_id)
                            select new
                            {
                                prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),

                            };

                    foreach (var o in t)
                    {
                        name = o.prefix + " " + o.first_name + " " + o.last_name;

                    }

                }
                else
                {
                    var e = from c in dbConnect.employees
                            where c.employee_id == employee_id
                            select new
                            {
                                prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),

                            };

                    foreach (var o in e)
                    {
                        name = o.prefix + " " + o.first_name + " " + o.last_name;

                    }

                }


                return name;
            }
        }





        protected string getPermission(int group_login, string employee_id, string mngt_level,string permission_name,string id,string type_report,string type_per,string country)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int EMPLOYEE = 12;
                int AD = 13;
                int CONTRACTOR = 14;


                var v = from c in dbConnect.groups
                        join m in dbConnect.group_has_permissions on c.id equals m.group_id
                        join pe in dbConnect.permissions on m.permission_id equals pe.id
                        where c.id == group_login && m.country == country
                        select new
                        {
                            per_name = pe.name,
                            group_name = c.name,
                            group_value = c.value

                        };

                ArrayList per = new ArrayList();
                ArrayList group = new ArrayList();
                ArrayList group_value = new ArrayList();

                ArrayList fu = new ArrayList();
                ArrayList de = new ArrayList();
                ArrayList di = new ArrayList();
                ArrayList se = new ArrayList();

                foreach (var r in v)
                {
                    per.Add(r.per_name);
                    group.Add(r.group_name);
                    group_value.Add(r.group_value);
                }


                if (mngt_level == "SML")//SML group 15 on group table
                {
                    var s = from c in dbConnect.groups
                            join m in dbConnect.group_has_permissions on c.id equals m.group_id
                            join pe in dbConnect.permissions on m.permission_id equals pe.id
                            where c.id == 15
                            select new
                            {
                                per_name = pe.name,
                                group_name = c.name,
                                group_value = c.value

                            };


                    foreach (var c in s)
                    {
                        per.Add(c.per_name);
                        group.Add(c.group_name);
                        group_value.Add(c.group_value);
                    }

                }


                if (mngt_level == "TML")//SML group 15 on group table
                {
                    var s1 = from c in dbConnect.groups
                             join m in dbConnect.group_has_permissions on c.id equals m.group_id
                             join pe in dbConnect.permissions on m.permission_id equals pe.id
                             where c.id == 17 && m.country == Session["country"].ToString()
                             select new
                             {
                                 per_name = pe.name,
                                 group_name = c.name,
                                 group_value = c.value

                             };


                    foreach (var c in s1)
                    {
                        per.Add(c.per_name);
                        group.Add(c.group_name);
                        group_value.Add(c.group_value);
                    }

                }


                if (group_login != EMPLOYEE && group_login != CONTRACTOR)//when is contractor value is empty
                {

                    var p = from c in dbConnect.employee_has_groups
                            join g in dbConnect.group_has_permissions on c.group_id equals g.group_id
                            join pe in dbConnect.permissions on g.permission_id equals pe.id
                            join g2 in dbConnect.groups on c.group_id equals g2.id
                            where c.employee_id == employee_id && c.country == country
                            select new
                            {
                                permission_name = pe.name,
                                group_name = g2.name,
                                group_value = g2.value,
                                function_id = c.function_id
                            };


                    foreach (var d in p)
                    {
                        per.Add(d.permission_name);
                        group.Add(d.group_name);
                        group_value.Add(d.group_value);

                        if (d.function_id != null)
                        {
                            fu.Add(d.function_id);
                        }
                    }

                    //////////////////////////Area oh&s////////////////////////////////
                    var ed = from c in dbConnect.employee_has_departments
                             where c.employee_id == employee_id && c.country == country
                             select new
                             {
                                 c.id,
                                 c.department_id
                             };

                    if (ed.Count() > 0)
                    {
                        foreach (var j in ed)
                        {
                            de.Add(j.department_id);
                        }

                        var gp1 = from c in dbConnect.group_has_permissions
                                  join pe in dbConnect.permissions on c.permission_id equals pe.id
                                  where c.group_id == 9//area oh&s
                                  && c.country == country
                                  select new
                                  {
                                      permission_name = pe.name,

                                  };


                        foreach (var d2 in gp1)
                        {
                            per.Add(d2.permission_name);

                        }

                        group.Add("Area OH&S");
                        group_value.Add(4);//on tabl group
                    }
                    //////////////////////////end////////////////////////////////

                    //////////////////////////Area Manager////////////////////////////////
                    var edi = from c in dbConnect.employee_has_divisions
                              where c.employee_id == employee_id && c.country == country
                              select new
                              {
                                  c.id,
                                  c.division_id
                              };

                    if (edi.Count() > 0)
                    {
                        foreach (var k in edi)
                        {
                            di.Add(k.division_id);

                        }

                        var gp2 = from c in dbConnect.group_has_permissions
                                  join pe in dbConnect.permissions on c.permission_id equals pe.id
                                  where c.group_id == 10//area manager
                                  && c.country == country
                                  select new
                                  {
                                      permission_name = pe.name,

                                  };


                        foreach (var d3 in gp2)
                        {
                            per.Add(d3.permission_name);

                        }

                        group.Add("Area Manager");
                        group_value.Add(5);//on tabl group
                    }
                    //////////////////////////end////////////////////////////////



                    //////////////////////////Area Supervisor////////////////////////////////
                    var es = from c in dbConnect.employee_has_sections
                             where c.employee_id == employee_id
                             && c.country == country
                             select new
                             {
                                 c.id,
                                 c.section_id
                             };

                    if (es.Count() > 0)
                    {
                        foreach (var z in es)
                        {
                            se.Add(z.section_id);

                        }

                        var gp3 = from c in dbConnect.group_has_permissions
                                  join pe in dbConnect.permissions on c.permission_id equals pe.id
                                  where c.group_id == 11//area supervisor
                                  && c.country == country
                                  select new
                                  {
                                      permission_name = pe.name,

                                  };


                        foreach (var d4 in gp3)
                        {
                            per.Add(d4.permission_name);

                        }

                        group.Add("Area Supervisor");
                        group_value.Add(6);//on tabl group
                    }
                    //////////////////////////end////////////////////////////////

                }//end if


                int min_group_value = GetMinValue(group_value);
                // int group_id = getGroupID(min_group_value);
                int group_v = min_group_value;

                bool per_area = safetys4.Class.SafetyPermission.checkPermisionInAreaMobile(id, type_report, fu, de, di, se, group);

                string return_per = checkPermission(id, group_v.ToString(), permission_name, type_report);
                string access_per = "NO";

                if (type_per == "view")
                {
                    if (per.IndexOf(permission_name) > -1)
                    {
                        access_per = "YES";
                    }
                }
                else
                {
                    if (per.IndexOf(permission_name) > -1 && return_per == "true" && per_area == true)
                    {
                        access_per = "YES";
                    }
                }




                return access_per;
            }
        }


        protected int GetMinValue(ArrayList arrList)
        {

            ArrayList sortArrayList = arrList;

            sortArrayList.Sort();
            int minV = 0;
            if (sortArrayList[0] != null)
            {

                minV = Convert.ToInt32(sortArrayList[0]);

            }


            return minV;

        }

        protected int getGroupID(int min_group_value)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.groups
                        where c.value == min_group_value
                        orderby c.id descending
                        select new
                        {
                            c.id

                        };

                int group_id = 0;

                foreach (var r in v)
                {
                    group_id = r.id;

                }

                return group_id;
            }

        }



        [WebMethod]
       
        public void updateReasonRejectIncident()
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int status = 3;//reject

                string result = "";
                string error = "";
                string id = "";

                try
                {
                    string incidentid = Context.Request["incidentid"].ToString();
                    string reason_reject_type = Context.Request["reason_reject_type"].ToString();
                    string reasonreject = Context.Request["reasonreject"].ToString();
                    string user_id = Context.Request["user_id"].ToString();
                    string typelogin = Context.Request["typelogin"].ToString();
                    string group_id = Context.Request["group_id"].ToString();

                    string timezone = "";

                    if (Context.Request["timezone"] == null)
                    {
                        timezone = "+7";

                    }
                    else
                    {
                        timezone = Context.Request["timezone"].ToString();

                    }


                    var query = from c in dbConnect.incidents
                                where c.id == Convert.ToInt32(incidentid)
                                select c;

                    foreach (incident rc in query)
                    {
                        rc.reason_reject_type = Convert.ToInt16(reason_reject_type);
                        rc.reason_reject = reasonreject;
                        rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                        rc.process_status = status;
                        rc.work_relate = "";//เผื่อเค้าลง saveเพราะ default คือ Y                   
                        rc.reject_report_form1 = Convert.ToInt32(group_id);

                    }



                    dbConnect.SubmitChanges();

                    incident_detail objInsert2 = new incident_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert2.process_status = status;
                    objInsert2.incident_id = Convert.ToInt32(incidentid);

                    dbConnect.incident_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();
                    dbConnect.Dispose();

                    result = "success";


                    if (timezone == "+5.5")//ยังไม่มั่นใจว่าส่ง country มาหรือเปล่าใช้อันนี้แทนไปก่อน
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { "AreaOH&S" };//, "AdminOH&S", "GroupOH&S" 
                        sn.InsertNotification(19, Convert.ToInt32(incidentid), alert_to_groups, timezone, "AreaOH&S");
                        ///////////////////////////////////end//////////////////////////////////////////////////////

                    }
                    else if (timezone == "+7")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { "Reporter" };//, "AdminOH&S", "GroupOH&S" 
                        sn.InsertNotification(19, Convert.ToInt32(incidentid), alert_to_groups, timezone, "Reporter");
                        ///////////////////////////////////end//////////////////////////////////////////////////////



                    }

                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    id = id

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }


        }


        [WebMethod]

        public void updateReasonRejectHazard()
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int status = 3;//reject

                string result = "";
                string error = "";
                string id = "";

                try
                {
                    string hazardid = Context.Request["hazardid"].ToString();
                    string reasonreject = Context.Request["reasonreject"].ToString();
                    string user_id = Context.Request["user_id"].ToString();
                    string typelogin = Context.Request["typelogin"].ToString();
                    string group_id = Context.Request["group_id"].ToString();
                    string step_form = Context.Request["step_form"].ToString();

                    string timezone = "";

                    if (Context.Request["timezone"] == null)
                    {
                        timezone = "+7";

                    }
                    else
                    {
                        timezone = Context.Request["timezone"].ToString();

                    }

                    var query = from c in dbConnect.hazards
                                where c.id == Convert.ToInt32(hazardid)
                                select c;

                    foreach (hazard rc in query)
                    {
                        rc.reason_reject = reasonreject;
                        rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                        rc.process_status = status;
                        if (step_form == "1")
                        {
                            rc.reject_report_form1 = Convert.ToInt32(group_id);

                        }
                        else
                        {

                            rc.reject_report_form2 = Convert.ToInt32(group_id);

                        }

                    }


                    dbConnect.SubmitChanges();

                    hazard_detail objInsert2 = new hazard_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    objInsert2.process_status = status;
                    objInsert2.hazard_id = Convert.ToInt32(hazardid);

                    dbConnect.hazard_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();
                    dbConnect.Dispose();

                    result = "success";


                    if (timezone == "+5.5")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////

                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { "AreaOH&S" };
                        sn.InsertHazardNotification(16, Convert.ToInt32(hazardid), alert_to_groups, timezone, "AreaOH&S");
                        ///////////////////////////////////end//////////////////////////////////////////////////////

                    }
                    else if (timezone == "+7")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////

                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { "Reporter" };
                        sn.InsertHazardNotification(16, Convert.ToInt32(hazardid), alert_to_groups, timezone, "Reporter");
                        ///////////////////////////////////end//////////////////////////////////////////////////////


                    }

                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    id = id

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }


        }


        
        [WebMethod]

        public void verifyIncident()
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                string error = "";
                string action = "";

                try
                {
                    string incidentid = Context.Request["incidentid"].ToString();
                    string group_id = Context.Request["group_id"].ToString();

                    string division_id = "";
                    string section_id = "";

                    var query = from c in dbConnect.incidents
                                where c.id == Convert.ToInt32(incidentid)
                                select c;


                    foreach (incident rc in query)
                    {

                        if (rc.step_form < 2)// ถ้าเลย step2 แล้วไม่ว่าสิทธิ์ไหนมากดก็ไม่ย้อนให้
                        {
                            rc.step_form = 2;
                            rc.incident_flow = 2;
                        }

                        if (group_id != null)
                        {
                            rc.verify_report_form1 = Convert.ToInt32(group_id);

                        }

                        division_id = rc.division_id;
                        section_id = rc.section_id;

                    }



                    if (string.IsNullOrEmpty(division_id) || string.IsNullOrEmpty(section_id) || section_id == "00000000" || division_id == "00000000")
                    {
                        action = "NO";
                        result = "no success";

                    }
                    else
                    {
                        dbConnect.SubmitChanges();
                        action = "YES";
                        result = "success";

                    }

                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    action = action

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }
        }


        [WebMethod]

        public void verifyHazard()
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                string error = "";
                string action = "";

                try
                {
                    string hazardid = Context.Request["hazardid"].ToString();
                    string group_id = Context.Request["group_id"].ToString();

                    string division_id = "";
                    string section_id = "";

                    var query = from c in dbConnect.hazards
                                where c.id == Convert.ToInt32(hazardid)
                                select c;

                    foreach (hazard rc in query)
                    {

                        if (rc.step_form < 2)
                        {
                            rc.step_form = 2;
                            rc.hazard_flow = 2;
                        }


                        if (group_id != null)
                        {
                            rc.verify_report_form1 = Convert.ToInt32(group_id);

                        }

                        division_id = rc.division_id;
                        section_id = rc.section_id;

                    }




                    if (string.IsNullOrEmpty(division_id) || string.IsNullOrEmpty(section_id) || section_id == "00000000" || division_id == "00000000")
                    {
                        action = "NO";
                        result = "no success";

                    }
                    else
                    {
                        dbConnect.SubmitChanges();
                        action = "YES";
                        result = "success";

                    }

                }
                catch (Exception ex)
                {

                    result = "no success";
                    error = ex.Message.ToString();

                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    action = action

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }
        }


        public string chageDataLanguage(string vTH, string vEN, string lang)
        {
            string vReturn = "";

            if (lang == "th")
            {
                if (vTH != null)
                {
                    vReturn = vTH;
                }
               

            }
            else if (lang == "en")
            {
                if (vEN != null)
                {
                    vReturn = vEN;
                }
            }
            else if (lang == "si")
            {
                if (vEN != null)
                {
                    vReturn = vEN;
                }
               
            }


            return vReturn;
        }


        protected void createLogLogin(string user_id, string type_login,string timezone)
        {

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            login_log objInsert = new login_log();


            if(type_login!="")
            {
                if (type_login != "contractor")
                {
                    objInsert.employee_id = user_id;
                }
                else
                {
                    objInsert.contractor_id = Convert.ToInt32(user_id);
                }
            }
            

            objInsert.type_login = type_login;
            objInsert.type_device = "mobile";
            objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
            dbConnect.login_logs.InsertOnSubmit(objInsert);

            dbConnect.SubmitChanges();
        }
    }
}
