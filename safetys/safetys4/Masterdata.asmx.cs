using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using safetys4.App_Code;
using System.Collections;
using System.IO;
using System.Text;
using System.Data;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Threading;

namespace safetys4
{
    /// <summary>
    /// Summary description for Masterdata
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Masterdata : System.Web.Services.WebService
    {

      


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getArealist(string area,string company_id,string function_id,string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.area_managements
                        join f in dbConnect.functions on c.function_id equals f.function_id
                        where c.status == "A" && (c.name_en.Contains(area) || c.name_th.Contains(area))
                        && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            area_th = checkAreaForEmpty(c.name_th),
                            area_en = checkAreaForEmpty(c.name_en),
                            function = chageDataLanguageForEmpty(f.function_th, f.function_en, lang),
                            c.company_id,
                            c.function_id,
                            c.department_id,
                            c.division_id,
                            c.section_id

                        };

                if (company_id != "")
                {
                    v = v.Where(c => c.function_id == function_id);

                }

                if (function_id != "")
                {
                    v = v.Where(c => c.function_id == function_id);

                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }





        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeautocomplete(string term)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.employees
                         join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                         where c.status == "Active" && (c.first_name_en.Contains(term) || c.first_name_th.Contains(term))
                         && c.country == Session["country"].ToString()
                         select new
                         {
                             //id = c.id,
                             label = c.first_name_en.Contains(term) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                             value = c.first_name_en.Contains(term) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                             employee_id = c.employee_id,
                             first_name_en = c.first_name_en,
                             first_name_th = c.first_name_th,
                             function_id = o.function_id,
                             department_id = o.department_id,
                             sub_function_id = o.sub_function_id,

                         });//.Take(25);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeDropdown(string lang,string employee_id,string pagetype)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.employees
                         join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                         where c.country == Session["country"].ToString()
                         select new
                         {
                             employee_id = c.employee_id,
                             fullname = chageDataLanguageForEmpty(c.first_name_th+" "+c.last_name_th,c.first_name_en+" "+c.last_name_en,lang),
                             o.company_id,
                             o.function_id,
                             o.department_id,
                             o.division_id,
                             o.section_id,
                             c.status ,
                             birth_date = c.birth_date == null ? "" : FormatDates.getDateShowFromDate(Convert.ToDateTime(c.birth_date, CultureInfo.InvariantCulture), lang),
                             hiring_date = c.hiring_date == null ? "" : FormatDates.getDateShowFromDate(Convert.ToDateTime(c.hiring_date, CultureInfo.InvariantCulture), lang),
                           
                         });//.Take(25);

                if (pagetype == "view")
                {
                    v = v.Where(c => c.employee_id == employee_id);

                }
                else
                {
                    v = v.Where(c => c.status == "Active");
                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeautocompleteofaction(string term)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.employees
                        // join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                         where c.status == "Active" && (c.first_name_en.Contains(term) || c.first_name_th.Contains(term))
                         && c.country == Session["country"].ToString()
                         select new
                         {
                             //id = c.id,
                             label = c.first_name_en.Contains(term) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                             value = c.first_name_en.Contains(term) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                             employee_id = c.employee_id,
                             first_name_en = c.first_name_en,
                             first_name_th = c.first_name_th,
                          

                         });//.Take(25);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeautocompleteofactionhealth(string term)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.employees
                         // join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                         where c.status == "Active" && (c.first_name_en.Contains(term) || c.first_name_th.Contains(term))
                         && (c.mngt_level == "MML" || c.mngt_level == "SML" || c.mngt_level == "TML")
                         && c.country == Session["country"].ToString()
                         select new
                         {
                             //id = c.id,
                             label = c.first_name_en.Contains(term) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                             value = c.first_name_en.Contains(term) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                             employee_id = c.employee_id,
                             first_name_en = c.first_name_en,
                             first_name_th = c.first_name_th,


                         });//.Take(25);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeIDAutocomplete(string term)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.employees
                        join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                         where c.status == "Active" && c.employee_id.Contains(term)
                         && c.country == Session["country"].ToString()
                         select new
                         {
                             //id = c.id,
                             label = c.employee_id,
                             value = c.employee_id,
                             first_name = chageDataLanguage(c.first_name_th,c.first_name_en,Session["lang"].ToString()),
                             last_name = chageDataLanguage(c.last_name_th,c.last_name_en,Session["lang"].ToString()),
                             o.company_id,
                             o.function_id,
                             o.department_id,
                             o.division_id,
                             o.section_id


                         });//.Take(25);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeautocompletesot(string q)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.employees
                         // join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                         where c.status == "Active" && (c.first_name_en.Contains(q) || c.first_name_th.Contains(q))
                         && c.country == Session["country"].ToString()
                         select new
                         {
                             //id = c.id,
                             id = c.employee_id,
                             name = c.first_name_en.Contains(q) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                            // @readonly = true




                         });//.Take(25);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }




        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getContractorautocomplete(string term)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.contractors
                         where c.status == "Active" && (c.first_name_en.Contains(term) || c.first_name_th.Contains(term))
                         && c.country == Session["country"].ToString()
                         select new
                         {
                             id = c.id,
                             label = c.first_name_en.Contains(term) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                             value = c.first_name_en.Contains(term) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                             first_name_en = c.first_name_en,
                             first_name_th = c.first_name_th,

                         });//.Take(25);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getContractorautocompletesot(string term)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.contractors
                         where c.status == "Active" && (c.first_name_en.Contains(term) || c.first_name_th.Contains(term))
                         && c.country == Session["country"].ToString()
                         select new
                         {
                             id = c.id,
                             label = c.first_name_en.Contains(term) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                             value = c.first_name_en.Contains(term) == true ? (c.first_name_en + " " + c.last_name_en) : (c.first_name_th + " " + c.last_name_th),
                             first_name_en = c.first_name_en,
                             first_name_th = c.first_name_th,

                         });//.Take(25);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getFuctionlist(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.functions
                        join co in dbConnect.companies on c.company_id equals  co.company_id
                        where c.country == Session["country"].ToString()
                        && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        orderby c.function_en ascending
                        select new
                        {
                            id = c.function_id,
                            name = chageDataLanguage(c.function_th, c.function_en, lang),
                            company_name = chageDataLanguage(co.company_th,co.company_en,lang)

                        };


                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    var result = new
                    {
                       id = rc.id,
                       name = "("+rc.company_name+") "+ rc.name
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }

        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void checkOtherRiskFactor(string id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.risk_factor_relate_works
                        where c.id == Convert.ToInt16(id)
                        select new
                        {
                            c.code

                        };


                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    bool other = false;

                    if(rc.code=="other")
                    {
                        other = true;

                    }

                    var result = new
                    {
                        result = other
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void checkTypecontrolrecoveryplan(string id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.type_control_healths
                        where c.id == Convert.ToInt16(id)
                        select new
                        {
                            c.code

                        };


                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    bool other = false;

                    if (rc.code == "recovery")
                    {
                        other = true;

                    }

                    var result = new
                    {
                        result = other
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getFuctionlistform3(string lang,string pagetype)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.functions
                        join co in dbConnect.companies on c.company_id equals co.company_id
                        where c.country == Session["country"].ToString()
                        orderby c.function_en ascending
                        select new
                        {
                            id = c.function_id,
                            name = chageDataLanguage(c.function_th, c.function_en, lang),
                            company_name = chageDataLanguage(co.company_th,co.company_en,lang),
                            c.valid_from,
                            c.valid_to

                        };


                if (pagetype == "edit")
                {
                    v = v.Where(c => c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())));
                    v = v.Where(c => c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())));

                }


                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    var result = new
                    {
                        id = rc.id,
                        name = rc.company_name + " - " + rc.name
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }

        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDepartmentlistform3(string function_id,string lang, string pagetype)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.departments
                        where c.country == Session["country"].ToString()
                        && c.function_id == function_id
                        orderby c.department_en ascending
                        select new
                        {
                            id = c.department_id,
                            name = chageDataLanguage(c.department_th, c.department_en, lang),
                            c.valid_from,
                            c.valid_to

                        };


                if (pagetype == "edit")
                {
                    v = v.Where(c => c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())));
                    v = v.Where(c => c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())));

                }


                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    var result = new
                    {
                        id = rc.id,
                        name = rc.name
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getPersonnelSubareaList(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.organizations
                        where c.country == Session["country"].ToString()
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        }).Distinct();


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSitelist(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.sites
                        where c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.site_id,
                            name = chageDataLanguage(c.site_th, c.site_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getReasonReject(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.reason_rejects
                        where c.country == Session["country"].ToString() && c.status == "A"

                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getReasonRejectHealth(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.reason_reject_healths
                        where c.country == Session["country"].ToString() && c.status == "A"

                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getReasonExcept(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.reason_excepts
                        where c.country == Session["country"].ToString() && c.status == "A"

                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getReasonRejectHazard(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.reason_reject_hazards
                        where c.country == Session["country"].ToString()

                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getFuctionByCompany(string company,string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.functions
                        where c.company_id == company.Trim() && c.country == Session["country"].ToString()
                        orderby c.function_id ascending
                        select new
                        {
                            id = c.function_id,
                            name = chageDataLanguage(c.function_th, c.function_en, lang),
                            c.valid_to,
                            c.valid_from

                        };
                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    string inactive = "";

                    if (rc.valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        inactive = " (Inactive)";
                    }

                    var result = new
                    {
                        id = rc.id,
                        name = rc.name + inactive
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getFuctionFormByCompany(string company, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.functions
                        where c.company_id == company.Trim() && c.country == Session["country"].ToString()
                        && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
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


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getFuctionByCompanyAll(string company, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.functions
                        where c.company_id == company.Trim() && c.country == Session["country"].ToString()
                       // && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                      //  && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        orderby c.function_id ascending
                        select new
                        {
                            id = c.function_id,
                            name = chageDataLanguage(c.function_th, c.function_en, lang),
                            c.valid_from,
                            c.valid_to

                        };


                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    string inactive = "";

                    if (rc.valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        inactive = " (Inactive)";
                    }

                    var result = new
                    {
                        id = rc.id,
                        name = rc.name + inactive
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));

     
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getCountry(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.countries
                        select new
                        {
                            id = c.country_id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getFuctionAll(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.functions
                        // where c.company_id == company.Trim()
                        where c.country == Session["country"].ToString()
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


   

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDepartmentbyFunction(string function, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.departments
                        where c.function_id == function.Trim() && c.country == Session["country"].ToString()
                        orderby c.department_id ascending
                        select new
                        {
                            id = c.department_id,
                            name = chageDataLanguage(c.department_th, c.department_en, lang),
                            c.valid_from,
                            c.valid_to

                        };


                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    string inactive = "";

                    if (rc.valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        inactive = " (Inactive)";
                    }

                    var result = new
                    {
                        id = rc.id,
                        name = rc.name + inactive
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDepartmentFormbyFunction(string function, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.departments
                        where c.function_id == function.Trim() && c.country == Session["country"].ToString()
                        && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
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


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDivisionbyDepartment(string department, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.divisions
                        where c.department_id == department.Trim() && c.country == Session["country"].ToString()
                        orderby c.division_id ascending
                        select new
                        {
                            id = c.division_id,
                            name = chageDataLanguage(c.division_th, c.division_en, lang),
                            c.valid_from,
                            c.valid_to

                        };


                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    string inactive = "";

                    if (rc.valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        inactive = " (Inactive)";
                    }

                    var result = new
                    {
                        id = rc.id,
                        name = rc.name + inactive
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDivisionFormbyDepartment(string department, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.divisions
                        where c.department_id == department.Trim() && c.country == Session["country"].ToString()
                        && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
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

        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSectionbyDivision(string division, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.sections
                        where c.division_id == division.Trim() && c.country == Session["country"].ToString()
                        orderby c.section_id ascending
                        select new
                        {
                            id = c.section_id,
                            name = chageDataLanguage(c.section_th, c.section_en, lang),
                            c.valid_from,
                            c.valid_to

                        };


                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    string inactive = "";

                    if (rc.valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        inactive = " (Inactive)";
                    }

                    var result = new
                    {
                        id = rc.id,
                        name = rc.name + inactive
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSectionFormbyDivision(string division, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.sections
                        where c.division_id == division.Trim() && c.country == Session["country"].ToString()
                        && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
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


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getCompany(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.companies
                        where c.country == Session["country"].ToString()
                        orderby c.company_id ascending
                        select new
                        {
                            id = c.company_id,
                            name = chageDataLanguage(c.company_th, c.company_en, lang),
                            c.valid_to,
                            c.valid_from

                        };


                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    string inactive = "";

                    if (rc.valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        inactive = " (Inactive)";
                    }

                    var result = new
                    {
                        id = rc.id,
                        name = rc.name + inactive
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }
           

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getCompanyForm(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.companies
                        where c.country == Session["country"].ToString()
                        && c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
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


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getCompanyAll(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.companies
                        where c.country == Session["country"].ToString()
                        //&& c.valid_from <= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        //&& c.valid_to >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        orderby c.company_id ascending
                        select new
                        {
                            id = c.company_id,
                            name = chageDataLanguage(c.company_th, c.company_en, lang),
                            c.valid_from,
                            c.valid_to

                        };
                ArrayList dt = new ArrayList();

                foreach (var rc in v)
                {
                    string inactive = "";

                    if (rc.valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        inactive = " (Inactive)";
                    }

                    var result = new
                    {
                        id = rc.id,
                        name = rc.name + inactive
                    };

                    dt.Add(result);
                }

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }


        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getHospital(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.hospitals
                        where c.country == Session["country"].ToString()
                        && c.status == "A"
                        orderby c.name_th ascending
                        select new
                        {
                            c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),
                           
                        };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(v));
                
            }


        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeDepartment(string department_id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.employee_has_departments
                        where c.country == Session["country"].ToString()
                        && c.department_id == department_id
                        select new
                        {
                            c.employee_id

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }


        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeDivision(string division_id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.employee_has_divisions
                        where c.country == Session["country"].ToString()
                        && c.division_id == division_id
                        select new
                        {
                            c.employee_id

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }


        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeSection(string section_id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.employee_has_sections
                        where c.country == Session["country"].ToString()
                        && c.section_id == section_id
                        select new
                        {
                            c.employee_id

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }


        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeFunctionalManager(string department_id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.employee_has_department_functional_managers
                        where c.country == Session["country"].ToString()
                        && c.department_id == department_id
                        select new
                        {
                            c.employee_id

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }


        }






        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getStatuscontractor(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.status_contractors
                        where c.lang == lang.Trim()
                        select new
                        {
                            id = c.value,
                            name = c.name

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        public void generateSubMasterData(string country)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var com = from c in dbConnect.companies
                          where  c.country == country
                          select c;
                foreach (var m in com)
                {
                    dbConnect.companies.DeleteOnSubmit(m);
                }
                dbConnect.SubmitChanges();


                var df = from c in dbConnect.functions
                         where c.country == country
                         select c;
                foreach (var a in df)
                {
                    dbConnect.functions.DeleteOnSubmit(a);

                }
                dbConnect.SubmitChanges();


                var dd = from c in dbConnect.departments
                         where c.country == country
                         select c;
                foreach (var a in dd)
                {
                    dbConnect.departments.DeleteOnSubmit(a);

                }
                dbConnect.SubmitChanges();



                var ddi = from c in dbConnect.divisions
                          where c.country == country
                          select c;
                foreach (var a in ddi)
                {
                    dbConnect.divisions.DeleteOnSubmit(a);

                }
                dbConnect.SubmitChanges();



                var ds = from c in dbConnect.sections
                         where c.country == country
                         select c;
                foreach (var a in ds)
                {
                    dbConnect.sections.DeleteOnSubmit(a);

                }
                dbConnect.SubmitChanges();



                /////////////////////////////////////////////end delete///////////////////////////////////////////////////////////////

                var cm = from c in dbConnect.organizations
                         where c.org_unit_id == c.company_id
                         && c.country == country
                         select new
                         {
                             id = c.company_id,
                             name = c.company,
                             valid_from = c.valid_from,
                             valid_to = c.valid_to

                         };

                foreach (var g in cm)
                {
                        company objInsert = new company();
                        objInsert.company_id = g.id;
                        objInsert.company_en = g.name;
                        objInsert.country = country;
                        objInsert.valid_from = g.valid_from;
                        objInsert.valid_to = g.valid_to;
                        dbConnect.companies.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    
                    
                }


                ///////////////////////////////////////end insert company//////////////////////////////////////////////////////////

                var v = from c in dbConnect.organizations
                        where c.org_unit_id == c.function_id
                        && c.country == country
                        select new
                        {
                            id = c.function_id,
                            name = c.function,
                            company_id = c.company_id,
                            valid_from = c.valid_from,
                            valid_to = c.valid_to


                        };

                foreach (var s in v)
                {
                    function objInsert = new function();
                    objInsert.function_id = s.id;
                    objInsert.function_en = s.name;
                    objInsert.company_id = s.company_id;
                    objInsert.country = country;
                    objInsert.valid_from = s.valid_from;
                    objInsert.valid_to = s.valid_to;
                    dbConnect.functions.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                    department objInsert2 = new department();
                    objInsert2.department_id = s.id + "F";
                    objInsert2.department_en = "other";
                    objInsert2.department_th = "อื่นๆ";
                    objInsert2.function_id = s.id;
                    objInsert2.country = country;
                    objInsert2.valid_from = s.valid_from;
                    objInsert2.valid_to = s.valid_to;

                    dbConnect.departments.InsertOnSubmit(objInsert2);
                    dbConnect.SubmitChanges();


                }



                if (country == "thailand")//department
                {
                    var o = from c in dbConnect.organizations
                            where c.org_unit_id == c.department_id
                            && c.country == country
                            select new
                            {
                                id = c.department_id,
                                name = c.department,
                                fuction_id = c.function_id,
                                 valid_from = c.valid_from,
                                 valid_to = c.valid_to

                            };

                    foreach (var s in o)
                    {

                        department objInsert = new department();
                        objInsert.department_id = s.id;
                        objInsert.department_en = s.name;
                        objInsert.function_id = s.fuction_id;
                        objInsert.country = country;
                        objInsert.valid_from = s.valid_from;
                        objInsert.valid_to = s.valid_to;

                        dbConnect.departments.InsertOnSubmit(objInsert);
                        dbConnect.SubmitChanges();



                    }
                }
                else if (country == "srilanka")//sub function
                {

                    var o = from c in dbConnect.organizations
                            where c.org_unit_id == c.sub_function_id
                            && c.country == country
                            select new
                            {
                                id = c.sub_function_id,
                                name = c.sub_function,
                                fuction_id = c.function_id,
                                valid_from = c.valid_from,
                                valid_to = c.valid_to

                            };

                    foreach (var s in o)
                    {

                        department objInsert = new department();
                        objInsert.department_id = s.id;
                        objInsert.department_en = s.name;
                        objInsert.function_id = s.fuction_id;
                        objInsert.country = country;
                        objInsert.valid_from = s.valid_from;
                        objInsert.valid_to = s.valid_to;

                        dbConnect.departments.InsertOnSubmit(objInsert);
                        dbConnect.SubmitChanges();



                    }


                }

               

                //////////////////////////////////end insert department//////////////////////////////////////////////////

                 if (country == "thailand")//division
                {

                    var de = from c in dbConnect.departments
                             where c.country == country
                             select c;


                    foreach (var p in de)
                    {

                        division objInsert2 = new division();

                        objInsert2.division_id = p.department_id + "D";
                        objInsert2.division_en = "other";
                        objInsert2.division_th = "อื่นๆ";
                        objInsert2.department_id = p.department_id;
                        objInsert2.country = country;
                        objInsert2.valid_from = p.valid_from;
                        objInsert2.valid_to = p.valid_to;
                         


                        dbConnect.divisions.InsertOnSubmit(objInsert2);
                        dbConnect.SubmitChanges();

                    }


                    var di = from c in dbConnect.organizations
                             where c.org_unit_id == c.division_id
                             && c.country == country
                             select new
                             {
                                 id = c.division_id,
                                 name = c.division,
                                 department_id = c.department_id,
                                 fucntion_id = c.function_id,
                                 valid_from = c.valid_from,
                                 valid_to = c.valid_to

                             };

                    foreach (var i in di)
                    {

                        division objInsert = new division();
                        objInsert.division_id = i.id;
                        objInsert.division_en = i.name;

                        if (i.department_id == "00000000")
                        {

                            objInsert.department_id = i.fucntion_id + "F";

                        }
                        else
                        {
                            objInsert.department_id = i.department_id;
                        }


                        objInsert.country = country;
                        objInsert.valid_from = i.valid_from;
                        objInsert.valid_to = i.valid_to;
                        dbConnect.divisions.InsertOnSubmit(objInsert);
                        dbConnect.SubmitChanges();


                    }

                }
                 else if (country == "srilanka")//department
                 {


                     var de = from c in dbConnect.departments
                              where c.country == country
                              select c;


                     foreach (var p in de)
                     {

                         division objInsert2 = new division();

                         objInsert2.division_id = p.department_id + "D";
                         objInsert2.division_en = "other";
                         objInsert2.division_th = "อื่นๆ";
                         objInsert2.department_id = p.department_id;
                         objInsert2.country = country;
                         objInsert2.valid_from = p.valid_from;
                         objInsert2.valid_to = p.valid_to;


                         dbConnect.divisions.InsertOnSubmit(objInsert2);
                         dbConnect.SubmitChanges();

                     }


                     var di = from c in dbConnect.organizations
                              where c.org_unit_id == c.department_id
                              && c.country == country
                              select new
                              {
                                  id = c.department_id,
                                  name = c.department,
                                  sub_fucntion_id = c.sub_function_id,
                                  function_id = c.function_id,
                                  valid_from = c.valid_from,
                                  valid_to = c.valid_to

                              };

                     foreach (var i in di)
                     {

                         division objInsert = new division();
                         objInsert.division_id = i.id;
                         objInsert.division_en = i.name;

                         if (i.sub_fucntion_id == "00000000")
                         {
                             objInsert.department_id = i.function_id+"F";
                         }
                         else
                         {
                             objInsert.department_id = i.sub_fucntion_id;

                         }
                         

                         objInsert.country = country;
                         objInsert.valid_from = i.valid_from;
                         objInsert.valid_to = i.valid_to;
                         dbConnect.divisions.InsertOnSubmit(objInsert);
                         dbConnect.SubmitChanges();


                     }



                 }

                ////////////////////////////////end insert divsion///////////////////////////////////////////////




                 if (country == "thailand")//division
                 {
                     var div = from c in dbConnect.divisions
                               where c.country == country
                               select c;

                     foreach (var d in div)
                     {

                         section objInsert2 = new section();

                         objInsert2.section_id = d.division_id + "D";
                         objInsert2.section_en = "other";
                         objInsert2.section_th = "อื่นๆ";
                         objInsert2.division_id = d.division_id;
                         objInsert2.country = country;
                         objInsert2.valid_from = d.valid_from;
                         objInsert2.valid_to = d.valid_to;


                         dbConnect.sections.InsertOnSubmit(objInsert2);
                         dbConnect.SubmitChanges();

                     }


                     var se = from c in dbConnect.organizations
                              where c.org_unit_id == c.section_id
                              && c.country == country
                              select new
                              {
                                  id = c.section_id,
                                  name = c.section,
                                  department_id = c.department_id,
                                  fucntion_id = c.function_id,
                                  division_id = c.division_id,
                                  valid_from = c.valid_from,
                                  valid_to = c.valid_to

                              };

                     foreach (var s in se)
                     {

                         section objInsert = new section();
                         objInsert.section_id = s.id;
                         objInsert.section_en = s.name;

                         if (s.division_id == "00000000")
                         {
                             if (s.department_id == "00000000")
                             {
                                 var g = from c in dbConnect.divisions
                                         where c.department_id == (s.fucntion_id + "F")
                                         select c;

                                 foreach (var b in g)
                                 {
                                     objInsert.division_id = b.division_id;
                                 }


                             }
                             else
                             {
                                 objInsert.division_id = s.department_id + "D";

                             }
                         }
                         else
                         {
                             objInsert.division_id = s.division_id;

                         }

                         objInsert.country = country;
                         objInsert.valid_from = s.valid_from;
                         objInsert.valid_to = s.valid_to;

                         dbConnect.sections.InsertOnSubmit(objInsert);
                         dbConnect.SubmitChanges();


                     }//end each


                 }
                 else if (country == "srilanka")
                 {

                     var div = from c in dbConnect.divisions
                               where c.country == country
                               select c;

                     foreach (var d in div)
                     {

                         section objInsert2 = new section();

                         objInsert2.section_id = d.division_id + "D";
                         objInsert2.section_en = "other";
                         objInsert2.section_th = "อื่นๆ";
                         objInsert2.division_id = d.division_id;
                         objInsert2.country = country;
                         objInsert2.valid_from = d.valid_from;
                         objInsert2.valid_to = d.valid_to;


                         dbConnect.sections.InsertOnSubmit(objInsert2);
                         dbConnect.SubmitChanges();

                     }



                     var se = from c in dbConnect.organizations
                              where c.org_unit_id == c.division_id
                              && c.country == country
                              select new
                              {
                                  id = c.division_id,
                                  name = c.division,
                                  department_id = c.department_id,
                                  fucntion_id = c.function_id,
                                  valid_from = c.valid_from,
                                  valid_to = c.valid_to

                              };

                     foreach (var s in se)
                     {

                         section objInsert = new section();
                         objInsert.section_id = s.id;
                         objInsert.section_en = s.name;


                         if (s.department_id == "00000000")
                         {

                             objInsert.division_id = s.fucntion_id + "F";

                         }
                         else
                         {
                             objInsert.division_id = s.department_id;
                         }


                         objInsert.country = country;
                         objInsert.valid_from = s.valid_from;
                         objInsert.valid_to = s.valid_to;

                         dbConnect.sections.InsertOnSubmit(objInsert);
                         dbConnect.SubmitChanges();


                     }//end each





                 }






            }

        }

        [WebMethod]
        public void generateNewMasterData(string country)
        {

            //NetworkCredential theNetworkCredential = new NetworkCredential(username, password, domain);
            //CredentialCache theNetcache = new CredentialCache();
            //theNetCache.Add(@"\\computer", theNetworkCredential, "Basic", theNetworkCredential);
            ////then do whatever, such as getting a list of folders:
            //string[] theFolders = System.IO.Directory.GetDirectories("@\\computer\share");

            //https://msdn.microsoft.com/en-us/library/cc148994.aspx

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            //string pathfolder = string.Format("{0}\\upload\\incident\\" + name_folder, Server.MapPath(@"\"));
            string path = "";
            string timezone = "";
            if (country == "thailand")
            {
                path = @"C:\Users\warunee_kos.FREEWILLGROUP\Desktop\SCCO_SAFETY\data import\ZHRPAI003_14092017\Emp_info.txt";
                timezone = "+7";
            }
            else if (country == "srilanka")
            {
                path = @"C:\Users\warunee_kos.FREEWILLGROUP\Desktop\SCCO_SAFETY\import srilanka\INFO_HRP_31082017\Emp_info.txt";
               // path = @"C:\inetpub\wwwroot\import_safety\SIT_21072017\Emp_info.txt";
                timezone = "+5.5";
            }
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string[] emp_id = new string[] { "10000000", "10000001", "00000000" };//, "15900101"
                 //////////////////////////////////////////////delete employee old////////////////////////////
                var gr = from c in dbConnect.employees
                         where !(emp_id.Contains(c.employee_id))
                         && c.country == country
                         select c;
                            
                    foreach (var a in gr)
                    {
                        dbConnect.employees.DeleteOnSubmit(a);
                    }
                   
                    dbConnect.SubmitChanges();
                    //////////////////////////////////////////////end delete employee old////////////////////////////
                int count = 0;
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {

                    if(count!=0)
                    {
                        string[] split = line.Split('\t');

                        var e = from c in dbConnect.employees
                                where c.employee_id == split[0].Trim()
                                select c;

                        if (e.Count() == 0)
                        {
                            employee objInsert = new employee();

                            objInsert.employee_id = split[0].Trim();
                            objInsert.first_name_en = split[1].Trim();
                            objInsert.middle_name = split[2].Trim();
                            objInsert.last_name_en = split[3].Trim();
                            objInsert.personnel_area = split[4].Trim();
                            objInsert.cost_center = split[5].Trim();
                            objInsert.status = split[6].Trim();
                            objInsert.position_id = split[7].Trim();
                            objInsert.unit_id = split[8].Trim();
                            objInsert.supervisor_id = split[9].Trim();
                            objInsert.supervisor_dot_line_id = split[10].Trim();
                            objInsert.time_status = split[11].Trim();
                            objInsert.employee_subgroup = split[14].Trim();
                            objInsert.employee_subgroup_text = split[15].Trim();
                            objInsert.major_dept = split[16].Trim();
                            objInsert.pers_sub_area = split[17].Trim();
                            objInsert.payroll_area = split[18].Trim();
                            objInsert.supos_id = split[20].Trim();
                            objInsert.nick_name = split[21].Trim();
                            objInsert.birth_place = split[23].Trim();
                            objInsert.nationality = split[24].Trim();
                            objInsert.marital_status = split[25].Trim();
                            objInsert.religion = split[26].Trim();
                            objInsert.gender = split[27].Trim();
                            objInsert.company_code = split[28].Trim();
                            objInsert.prefix_en = split[29].Trim();

                            if (split[30].ToString() != "")
                            {
                                var names = (split[30].Trim()).Split(' ');
                                string prefix_th = "";
                                string first_name = "";
                                string last_name = "";

                                foreach (string s in names)
                                {
                                    if (String.IsNullOrEmpty(prefix_th) && s.Trim() != "")
                                    {
                                        prefix_th = s;
                                    }
                                    else if (String.IsNullOrEmpty(first_name) && s.Trim() != "")
                                    {
                                        first_name = s;
                                    }
                                    else if (string.IsNullOrEmpty(last_name) && s.Trim() != "")
                                    {
                                        last_name = s;
                                    }

                                }
                                //string[] name_th = 
                                objInsert.prefix_th = prefix_th;
                                objInsert.first_name_th = first_name;
                                objInsert.last_name_th = last_name;
                            }
                            else
                            {
                                objInsert.prefix_th = "";
                                objInsert.first_name_th = "";
                                objInsert.last_name_th = "";
                            }



                            objInsert.mngt_level = split[31].Trim();
                            objInsert.personal_grade = split[32].Trim();
                            objInsert.job_grade = split[33].Trim();
                            objInsert.email = split[34].Trim();
                            objInsert.domain_user = split[35].Trim();
                            objInsert.id_card = split[36].Trim();
                            objInsert.cost_center_text = split[37].Trim();
                            objInsert.emp_status_id = split[38].Trim();
                            objInsert.supervisor_name = split[39].Trim();
                            objInsert.emp_group_text = split[40].Trim();
                            objInsert.company_text = split[41].Trim();
                            objInsert.extension_no_office = split[42].Trim();

                            objInsert.country = country;
                            objInsert.timezone = timezone;


                            dbConnect.employees.InsertOnSubmit(objInsert);

                            dbConnect.SubmitChanges();


                        }

                        
                       
                    }

                    count++;
                }
            }//end employee



            ////////////////////////////////////////////////////end manage employee////////////////////////////////////

            string path2 = "";
            if (country == "thailand")
            {
                path2 = @"C:\Users\warunee_kos.FREEWILLGROUP\Desktop\SCCO_SAFETY\data import\ZHRPAI003_14092017\Org_info.txt";
                
            }
            else if (country == "srilanka")
            {
                path2 = @"C:\Users\warunee_kos.FREEWILLGROUP\Desktop\SCCO_SAFETY\import srilanka\INFO_HRP_31082017\Org_info.txt";
                //path2 = @"C:\inetpub\wwwroot\import_safety\SIT_21072017\Org_info.txt";
               
            }

            var fileStream2 = new FileStream(path2, FileMode.Open, FileAccess.Read);
            using (var streamReader2 = new StreamReader(fileStream2, Encoding.UTF8))
            {
                 //////////////////////////////////////////////delete org old////////////////////////////
                    var gr = from c in dbConnect.organizations
                             where  c.country == country
                             select c;
                            
                    foreach (var a in gr)
                    {
                        dbConnect.organizations.DeleteOnSubmit(a);
                    }

                    dbConnect.SubmitChanges();
                    //////////////////////////////////////////////end delete org old////////////////////////////
                int count2 = 0;
                string line2;
                
                while ((line2 = streamReader2.ReadLine()) != null)
                {
                    if (count2 != 0)
                    {
                        string[] splitOrg = line2.Split('\t');


                        int index = splitOrg[19].Trim().IndexOf("9999");
                        organization objInsert2 = new organization();


                        //if(index > -1)
                        //{
                                objInsert2.org_unit_id = splitOrg[0].Trim();
                                objInsert2.org_unit = splitOrg[1].Trim();
                                objInsert2.parent_id = splitOrg[2].Trim();
                                objInsert2.ou_abbr = splitOrg[3].Trim();
                                objInsert2.function_id = splitOrg[4].Trim();
                                objInsert2.function = splitOrg[5].Trim();
                                objInsert2.sub_function_id = splitOrg[6].Trim();
                                objInsert2.sub_function = splitOrg[7].Trim();
                                objInsert2.department_id = splitOrg[8].Trim();
                                objInsert2.department = splitOrg[9].Trim();
                                objInsert2.division_id = splitOrg[10].Trim();
                                objInsert2.division = splitOrg[11].Trim();
                                objInsert2.section_id = splitOrg[12].Trim();
                                objInsert2.section = splitOrg[13].Trim();
                                objInsert2.unit_id = splitOrg[14].Trim();
                                objInsert2.unit = splitOrg[15].Trim();
                                objInsert2.company_id = splitOrg[16].Trim();
                                objInsert2.company = splitOrg[17].Trim();
                               

                               objInsert2.valid_from =  FormatDates.changeDateTimeDB(splitOrg[18].Trim().Replace(".","/") + " 00:00", "en");
                               objInsert2.valid_to = FormatDates.changeDateTimeDB(splitOrg[19].Trim().Replace(".", "/") + " 00:00", "en");

                               if(country=="srilanka")
                               {
                                    objInsert2.personnel_area = splitOrg[20].Trim();
                                    objInsert2.personnel_area_description= splitOrg[21].Trim();
                                    objInsert2.personnel_subarea = splitOrg[22].Trim();
                                    objInsert2.personnel_subarea_description = splitOrg[23].Trim();
                               }
                              
                                objInsert2.country = country;

                               

                                dbConnect.organizations.InsertOnSubmit(objInsert2);

                                dbConnect.SubmitChanges();



                       // }
                       
                        

                        ////////////////////////////////////////////////////end manage org////////////////////////////////////
                    }

                    count2++;
                }
            }//end org



            generateSubMasterData(country);

            if (country == "thailand")
            {
                //  generateORGTH();
            }
         
        }




        [WebMethod]
        public void stampDataOld()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var query = from c in dbConnect.incidents
                            select c;

            foreach (incident rc in query)
            {


                //rc.location_company_id = rc.company_id;
                //rc.location_function_id = rc.function_id;
                //rc.location_department_id = rc.department_id;
                //rc.location_division_id = rc.division_id;
                //rc.location_section_id = rc.section_id;

                //string[] dt = new string[2];
                //dt = (string[])getMasterdataName(rc.company_id, "company");
                //rc.location_company_name_th = dt[0];
                //rc.location_company_name_en = dt[1];

                //dt = (string[])getMasterdataName(rc.function_id, "function");
                //rc.location_function_name_th = dt[0];
                //rc.location_function_name_en = dt[1];

                //dt = (string[])getMasterdataName(rc.department_id, "department");
                //rc.location_department_name_th = dt[0];
                //rc.location_department_name_en = dt[1];

                //dt = (string[])getMasterdataName(rc.division_id, "division");
                //rc.location_division_name_th = dt[0];
                //rc.location_division_name_en = dt[1];

                //dt = (string[])getMasterdataName(rc.section_id, "section");
                //rc.location_section_name_th = dt[0];
                //rc.location_section_name_en = dt[1];


                //////////////////////////////////////reporter///////////////////////////////////////////////////
                var t = from c in dbConnect.employees
                        join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                        where c.employee_id == rc.employee_id
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
                    rc.reporter_company_id = rc1.company_id;
                    rc.reporter_function_id = rc1.function_id;
                    rc.reporter_division_id = rc1.division_id;
                    rc.reporter_section_id = rc1.section_id;
                    rc.reporter_company_name = rc1.company;
                    rc.reporter_function_name = rc1.function;
                    rc.reporter_division_name = rc1.division;
                    rc.reporter_section_name = rc1.section;


                    if (rc.country == "thailand")
                    {
                        rc.reporter_department_id = rc1.department_id;
                        rc.reporter_department_name = rc1.department;
                    }
                    else if (rc.country == "srilanka")
                    {

                        rc.reporter_department_id = rc1.sub_function_id;
                        rc.reporter_department_name = rc1.sub_function;

                    }
                   

                }

            } 
            
            dbConnect.SubmitChanges();




            var query2 = from c in dbConnect.hazards
                        select c;

            foreach (hazard rc in query2)
            {


                //rc.location_company_id = rc.company_id;
                //rc.location_function_id = rc.function_id;
                //rc.location_department_id = rc.department_id;
                //rc.location_division_id = rc.division_id;
                //rc.location_section_id = rc.section_id;

                //string[] dt = new string[2];
                //dt = (string[])getMasterdataName(rc.company_id, "company");
                //rc.location_company_name_th = dt[0];
                //rc.location_company_name_en = dt[1];

                //dt = (string[])getMasterdataName(rc.function_id, "function");
                //rc.location_function_name_th = dt[0];
                //rc.location_function_name_en = dt[1];

                //dt = (string[])getMasterdataName(rc.department_id, "department");
                //rc.location_department_name_th = dt[0];
                //rc.location_department_name_en = dt[1];

                //dt = (string[])getMasterdataName(rc.division_id, "division");
                //rc.location_division_name_th = dt[0];
                //rc.location_division_name_en = dt[1];

                //dt = (string[])getMasterdataName(rc.section_id, "section");
                //rc.location_section_name_th = dt[0];
                //rc.location_section_name_en = dt[1];


                //////////////////////////////////////reporter///////////////////////////////////////////////////
                var t = from c in dbConnect.employees
                        join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                        where c.employee_id == rc.employee_id
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
                    rc.reporter_company_id = rc1.company_id;
                    rc.reporter_function_id = rc1.function_id;
                    rc.reporter_division_id = rc1.division_id;
                    rc.reporter_section_id = rc1.section_id;
                    rc.reporter_company_name = rc1.company;
                    rc.reporter_function_name = rc1.function;
                    rc.reporter_division_name = rc1.division;
                    rc.reporter_section_name = rc1.section;

                    if (rc.country == "thailand")
                    {
                        rc.reporter_department_id = rc1.department_id;
                        rc.reporter_department_name = rc1.department;
                    }
                    else if (rc.country == "srilanka")
                    {

                        rc.reporter_department_id = rc1.sub_function_id;
                        rc.reporter_department_name = rc1.sub_function;

                    }



                }

            }

            dbConnect.SubmitChanges();





            var query3 = from c in dbConnect.sots
                         select c;

            foreach (sot rc in query3)
            {


                //////////////////////////////////////reporter///////////////////////////////////////////////////
                var t = from c in dbConnect.employees
                        join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                        where c.employee_id == rc.employee_id
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
                    rc.reporter_company_id = rc1.company_id;
                    rc.reporter_function_id = rc1.function_id;
                    rc.reporter_division_id = rc1.division_id;
                    rc.reporter_company_name = rc1.company;
                    rc.reporter_function_name = rc1.function;
                    rc.reporter_division_name = rc1.division;

                    if (rc.country == "thailand")
                    {
                        rc.reporter_department_id = rc1.department_id;
                        rc.reporter_department_name = rc1.department;
                    }
                    else if (rc.country == "srilanka")
                    {

                        rc.reporter_department_id = rc1.sub_function_id;
                        rc.reporter_department_name = rc1.sub_function;

                    }



                }

            }

            dbConnect.SubmitChanges();


        }

        public void generateORGTH()
        {

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var fileStream2 = new FileStream(@"C:\Users\warunee_kos.FREEWILLGROUP\Desktop\SCCO_SAFETY\data import\Org_info_th.txt", FileMode.Open, FileAccess.Read);
            using (var streamReader2 = new StreamReader(fileStream2, Encoding.UTF8))
            {             
                int count2 = 0;
                string line2;
                while ((line2 = streamReader2.ReadLine()) != null)
                {
                    if (count2 != 0)
                    {
                        string[] splitOrg = line2.Split('\t');

                        string company_id = "";
                        string function_id = "";
                        string department_id = "";
                        string division_id = "";
                        string section_id = "";


                            if (splitOrg[0].Trim() != "")
                            {
                                var companys = from c in dbConnect.companies
                                               where c.company_id == splitOrg[0].Trim()
                                               select c;
                                if (companys.Count() > 0)
                                {

                                    company_id = splitOrg[0].Trim();

                                    var query = from c in dbConnect.companies
                                                where c.company_id == company_id
                                                select c;

                                    foreach (company rc in query)
                                    {

                                        if (splitOrg[17].Trim() != "" && splitOrg[17].Trim() != "-")
                                        {
                                            rc.company_th = splitOrg[17].Trim();
                                        }
                                        else
                                        {
                                            rc.company_th = rc.company_en;
                                        }

                                    }
                                    dbConnect.SubmitChanges();

                                }





                                var functions = from f in dbConnect.functions
                                                where f.function_id == splitOrg[0].Trim()
                                                select f;
                                if (functions.Count() > 0)
                                {

                                    function_id = splitOrg[0].Trim();

                                    var query = from c in dbConnect.functions
                                                where c.function_id == function_id
                                                select c;

                                    foreach (function rc in query)
                                    {

                                        if (splitOrg[5].Trim() != "" && splitOrg[5].Trim() != "-")
                                        {
                                            rc.function_th = splitOrg[5].Trim();
                                        }
                                        else
                                        {
                                            rc.function_th = rc.function_en;
                                        }

                                    }
                                    dbConnect.SubmitChanges();

                                }


                                var departments = from f in dbConnect.departments
                                                  where f.department_id == splitOrg[0].Trim()
                                                  select f;
                                if (departments.Count() > 0)
                                {

                                    department_id = splitOrg[0].Trim();

                                    var query = from c in dbConnect.departments
                                                where c.department_id == department_id
                                                select c;

                                    foreach (department rc in query)
                                    {
                                        if (splitOrg[9].Trim() != "" && splitOrg[9].Trim() != "-")
                                        {
                                            rc.department_th = splitOrg[9].Trim();
                                        }
                                        else
                                        {
                                            rc.department_th = rc.department_en;
                                        }

                                    }
                                    dbConnect.SubmitChanges();
                                }


                                var divisions = from f in dbConnect.divisions
                                                where f.division_id == splitOrg[0].Trim()
                                                select f;
                                if (divisions.Count() > 0)
                                {

                                    division_id = splitOrg[0].Trim();


                                    var query = from c in dbConnect.divisions
                                                where c.division_id == division_id
                                                select c;

                                    foreach (division rc in query)
                                    {
                                        if (splitOrg[11].Trim() != "" && splitOrg[11].Trim() != "-")
                                        {
                                            rc.division_th = splitOrg[11].Trim();
                                        }
                                        else
                                        {
                                            rc.division_th = rc.division_en;
                                        }

                                    }
                                    dbConnect.SubmitChanges();
                                }


                                var sections = from f in dbConnect.sections
                                               where f.section_id == splitOrg[0].Trim()
                                               select f;
                                if (sections.Count() > 0)
                                {

                                    section_id = splitOrg[0].Trim();

                                    var query = from c in dbConnect.sections
                                                where c.section_id == section_id
                                                select c;

                                    foreach (section rc in query)
                                    {
                                        if (splitOrg[13].Trim() != "" && splitOrg[13].Trim() != "-")
                                        {
                                            rc.section_th = splitOrg[13].Trim();
                                        }
                                        else
                                        {
                                            rc.section_th = rc.section_en;

                                        }

                                    }
                                    dbConnect.SubmitChanges();
                                }

                            }
                            
                        
                     }

                    count2++;
                }//end while

                updateNameTHtoNameEN();//update ค่าไทยที่ null เป็น eng
            }//end org
        }


        public void updateNameTHtoNameEN()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var query1 = from c in dbConnect.companies
                             where c.company_th == null
                             select c;

                foreach (company rc in query1)
                {
                    rc.company_th = rc.company_en;

                }
                dbConnect.SubmitChanges();

                ////////////////////////////////////end company//////////////////////////////////////////////////////

                var query2 = from c in dbConnect.functions
                             where c.function_th == null
                             select c;

                foreach (function rc in query2)
                {

                    rc.function_th = rc.function_en;


                }
                dbConnect.SubmitChanges();


                ////////////////////////////////////////end function/////////////////////////////////////////////////

                var query3 = from c in dbConnect.departments
                             where c.department_th == null
                             select c;

                foreach (department rc in query3)
                {

                    rc.department_th = rc.department_en;

                }
                dbConnect.SubmitChanges();

                ///////////////////////////////////////////////end department//////////////////////////////////////

                var query4 = from c in dbConnect.divisions
                             where c.division_th == null
                             select c;

                foreach (division rc in query4)
                {

                    rc.division_th = rc.division_en;

                }
                dbConnect.SubmitChanges();

                ////////////////////////////////////////////end section////////////////////////////////////////



                var query5 = from c in dbConnect.sections
                             where c.section_th == null
                             select c;

                foreach (section rc in query5)
                {

                    rc.section_th = rc.section_en;

                }
                dbConnect.SubmitChanges();

            }

        }



        [WebMethod]
        public void generateAreaManagement()
        {

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var fileStream2 = new FileStream(@"C:\Users\warunee_kos.FREEWILLGROUP\Desktop\import\areamanagement_import.txt", FileMode.Open, FileAccess.Read);
            using (var streamReader2 = new StreamReader(fileStream2, Encoding.UTF8))
            {
                int count2 = 0;
                string line2;
                while ((line2 = streamReader2.ReadLine()) != null)
                {
                    if (count2 != 0)
                    {
                        string[] splitOrg = line2.Split('\t');

                        string company_id = "52010000";//Siam City Cement Public Company Limited
                        string area_name = splitOrg[4].Trim();
                        string function_id = splitOrg[0].Trim();
                        string department_id = "";
                        string division_id = "";
                        string section_id = "";

                        string department_employee_id = "";
                        string division_employee_id = "";
                        string section_employee_id = "";

                        string department_first_th = "";
                        string department_lastname_th = "";
                        string division_first_th = "";
                        string division_lastname_th = "";
                        string section_first_th = "";
                        string section_lastname_th = "";


                        if (splitOrg[5].Trim() != "" && splitOrg[5].Trim() !="-")//fullname department
                        {
                               string name = splitOrg[5].Trim();
                               string[] name_split = name.Split(' ');


                            if(name_split.Count()>1)
                            {
                                   department_first_th= name_split[0]; 
                                   department_lastname_th = name_split[1];

                                   var emp = from c in dbConnect.employees
                                             where c.first_name_th == department_first_th.Trim()
                                             && c.last_name_th == department_lastname_th.Trim()
                                             select new
                                             {
                                                 c.employee_id
                                             };
                                foreach(var v in emp)
                                {
                                    department_employee_id = v.employee_id;
                                }


                                string department_name = splitOrg[1].Trim();

                                if (department_name!="" && department_name!="-")
                                {
                                    var de = from c in dbConnect.departments
                                             where c.department_en == department_name.Trim()
                                             && c.function_id == function_id
                                             select new
                                             {
                                                 c.department_id
                                             };
                                    foreach (var d in de)
                                    {
                                        department_id = d.department_id;
                                    }

                                }
                                else
                                {
                                    department_id = function_id + "F";//other
                                }


                                if (department_employee_id!="")
                                {
                                    // deleteEmployeeDepartment(department_id);

                                    employee_has_department objInsert = new employee_has_department();
                                    objInsert.employee_id = department_employee_id;
                                    objInsert.department_id = department_id;
                                    dbConnect.employee_has_departments.InsertOnSubmit(objInsert);

                                    dbConnect.SubmitChanges();
                                }
                               



                            }
                             

                        }




                        if (splitOrg[6].Trim() != "" && splitOrg[6].Trim() != "-")//fullname division
                        {
                            string name = splitOrg[6].Trim();
                            string[] name_split = name.Split(' ');


                            if (name_split.Count() > 1)
                            {
                                division_first_th = name_split[0];
                                division_lastname_th = name_split[1];

                                var emp = from c in dbConnect.employees
                                          where c.first_name_th == division_first_th.Trim()
                                          && c.last_name_th == division_lastname_th.Trim()
                                          select new
                                          {
                                              c.employee_id
                                          };
                                foreach (var v in emp)
                                {
                                    division_employee_id = v.employee_id;
                                }


                                string division_name = splitOrg[2].Trim();

                                if (division_name != "" && division_name != "-")
                                {
                                    var di = from c in dbConnect.divisions
                                             where c.division_en == division_name.Trim()
                                             && c.department_id == department_id
                                             select new
                                             {
                                                 c.division_id
                                             };
                                    foreach (var d in di)
                                    {
                                        division_id = d.division_id;
                                    }

                                }
                                else
                                {
                                    division_id = department_id + "D";//other
                                }

                                if (division_employee_id!="")
                                {

                                    // deleteEmployeeDivision(division_id);

                                    employee_has_division objInsert = new employee_has_division();
                                    objInsert.employee_id = division_employee_id;
                                    objInsert.division_id = division_id;
                                    dbConnect.employee_has_divisions.InsertOnSubmit(objInsert);

                                    dbConnect.SubmitChanges();
                                }
                               
                            }

                        }




                        if (splitOrg[7].Trim() != "" && splitOrg[7].Trim() != "-")//fullname section
                        {
                            string name = splitOrg[7].Trim();
                            string[] name_split = name.Split(' ');

                            if (name_split.Count() > 1)
                            {
                                section_first_th = name_split[0];
                                section_lastname_th = name_split[1];

                                var emp = from c in dbConnect.employees
                                          where c.first_name_th == section_first_th.Trim()
                                          && c.last_name_th == section_lastname_th.Trim()
                                          select new
                                          {
                                              c.employee_id
                                          };
                                foreach (var v in emp)
                                {
                                    section_employee_id = v.employee_id;
                                }


                                string section_name = splitOrg[3].Trim();

                                if (section_name != "" && section_name != "-")
                                {
                                    var se = from c in dbConnect.sections
                                             where c.section_en == section_name.Trim()
                                             && c.division_id == division_id
                                             select new
                                             {
                                                 c.section_id
                                             };
                                    foreach (var s in se)
                                    {
                                        section_id = s.section_id;
                                    }

                                }
                                else
                                {
                                    section_id = division_id + "D";//other
                                }


                                if (section_employee_id!="")
                                {
                                     //deleteEmployeeSection(section_id);

                                    employee_has_section objInsert = new employee_has_section();
                                    objInsert.employee_id = section_employee_id;
                                    objInsert.section_id = section_id;
                                    dbConnect.employee_has_sections.InsertOnSubmit(objInsert);

                                    dbConnect.SubmitChanges();
                                }
                               
                            }

                        }

               


                        ////////////////////////////////////////////create areamanage////////////////////////////////////////////////////////////
                        
                        if(area_name!="" && department_id!="")
                        {
                            area_management objInsert2 = new area_management();
                            objInsert2.name_th = area_name;
                            //objInsert.name_en = name_en;
                            objInsert2.company_id = company_id;
                            objInsert2.function_id = function_id;
                            objInsert2.department_id = department_id;
                            objInsert2.division_id = division_id;
                            objInsert2.section_id = section_id;
                            objInsert2.status = "A";
                          //  objInsert2.created_at = DateTime.Now;
                          //  objInsert2.updated_at = DateTime.Now;
                            dbConnect.area_managements.InsertOnSubmit(objInsert2);

                            dbConnect.SubmitChanges();

                        }
                        else
                        {
                        //    log_import_area objInsert3 = new log_import_area();

                        //    objInsert3.function_id = splitOrg[0].Trim();
                        //    objInsert3.department = splitOrg[1].Trim();
                        //    objInsert3.division = splitOrg[2].Trim();
                        //    objInsert3.section = splitOrg[3].Trim();
                        //    objInsert3.area = splitOrg[4].Trim();
                        //    objInsert3.area_ohs = splitOrg[5].Trim();
                        //    objInsert3.area_manager = splitOrg[6].Trim();
                        //    objInsert3.area_supervisor = splitOrg[7].Trim();
                        //    dbConnect.log_import_areas.InsertOnSubmit(objInsert3);

                        //    dbConnect.SubmitChanges();

                        }
                      

                   
                     //  rc.section_th = splitOrg[13].Trim();

                             

                    }

                    count2++;
                }
            }//end 
        }

        [WebMethod]
        public void importEmployeeFromExcel()
        {
             using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;

            //Context.Response.Clear();
            //Context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

            string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
            string path = string.Format("{0}" + pathreport + "srilanka_import.xlsx", Server.MapPath(@"\"));
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);


            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet = workbook.GetSheet("CMBO_List");


            for (int row = 1; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
    
                    var v = from c in dbConnect.employees
                            where c.domain_user == sheet.GetRow(row).GetCell(0).StringCellValue.Trim()
                            select c;

                    if (v.Count() > 0)
                    {
                        foreach (employee rc in v)
                        {
                            rc.email = sheet.GetRow(row).GetCell(3).StringCellValue.Trim();
                        }

                        dbConnect.SubmitChanges();
                    }
                    else
                    {
                        //test_import objInsert = new test_import();
                        //objInsert.first_name = sheet.GetRow(row).GetCell(1).StringCellValue.Trim();
                        //objInsert.last_name = sheet.GetRow(row).GetCell(2).StringCellValue.Trim();
                        //dbConnect.test_imports.InsertOnSubmit(objInsert);

                        //dbConnect.SubmitChanges();
                    }
                }
            }//end foreach



            ISheet sheet2 = workbook.GetSheet("PCWK_List");


            for (int row = 1; row <= sheet2.LastRowNum; row++)
            {
                if (sheet2.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    //MessageBox.Show(string.Format("Row {0} = {1}", row, sheet.GetRow(row).GetCell(0).StringCellValue));

                    var v = from c in dbConnect.employees
                            where c.domain_user == sheet2.GetRow(row).GetCell(0).StringCellValue.Trim()
                            select c;

                    if (v.Count() > 0)
                    {
                        foreach (employee rc in v)
                        {
                            rc.email = sheet2.GetRow(row).GetCell(3).StringCellValue.Trim();
                        }

                        dbConnect.SubmitChanges();
                    }
                    else
                    {
                        //test_import objInsert = new test_import();
                        //objInsert.first_name = sheet2.GetRow(row).GetCell(1).StringCellValue.Trim();
                        //objInsert.last_name = sheet2.GetRow(row).GetCell(2).StringCellValue.Trim();
                        //dbConnect.test_imports.InsertOnSubmit(objInsert);

                        //dbConnect.SubmitChanges();
                    }
                }
            }//end foreach



            ISheet sheet3 = workbook.GetSheet("RCWK_List");


            for (int row = 1; row <= sheet3.LastRowNum; row++)
            {
                if (sheet3.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    //MessageBox.Show(string.Format("Row {0} = {1}", row, sheet.GetRow(row).GetCell(0).StringCellValue));

                    var v = from c in dbConnect.employees
                            where c.domain_user == sheet3.GetRow(row).GetCell(0).StringCellValue.Trim()
                            select c;

                    if (v.Count() > 0)
                    {
                        foreach (employee rc in v)
                        {
                            rc.email = sheet3.GetRow(row).GetCell(3).StringCellValue.Trim();
                        }

                        dbConnect.SubmitChanges();
                    }
                    else
                    {
                        //test_import objInsert = new test_import();
                        //objInsert.first_name = sheet3.GetRow(row).GetCell(1).StringCellValue.Trim();
                        //objInsert.last_name = sheet3.GetRow(row).GetCell(2).StringCellValue.Trim();
                        //dbConnect.test_imports.InsertOnSubmit(objInsert);

                        //dbConnect.SubmitChanges();
                    }
                }
            }//end foreach







             }
        }

        [WebMethod]
        public void clearHazardForm4()
        {
            //เคลียร์กับไปอยู่ step form 4 โดยยังไม่ได้ปิดค้างอยู่ที่ใคร
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var query = from c in dbConnect.hazards
                            join l in dbConnect.log_request_close_hazards.Where(d => d.group_id == 10 && d.status == "A") on c.id equals l.hazard_id
                            where c.process_status == 1 && c.step_form == 4
                            select new { c.id };

                foreach (var r in query)
                {

                    var gr = from c in dbConnect.log_request_close_hazards
                             where c.hazard_id == Convert.ToInt32(r.id)
                             && c.status == "A"
                             select c;
                    foreach (var a in gr)
                    {
                        a.status = "D";
                        a.remark = "เคลียร์ค่า ก่อนเปลี่ยน step การปิด hazard";
                    }

                    dbConnect.SubmitChanges();
                }

            }


        }



        [WebMethod]
        public void importOldDataFromExcel()
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {



                ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;

                //Context.Response.Clear();
                //Context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
               

                string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
                string path = string.Format("{0}" + pathreport + "srilanka_data_old.xlsx", Server.MapPath(@"\"));
                FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);

            
                //try
                //{
                    XSSFWorkbook workbook = new XSSFWorkbook(file);

                    ISheet sheet = workbook.GetSheet("Incident");

                    int count = 0;
                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                        {
                            int process_status = 1;//getStatusForm(sheet.GetRow(row).GetCell(51).StringCellValue.Trim());

                            if (sheet.GetRow(row).GetCell(51).DateCellValue != DateTime.MinValue)
                            {
                                process_status = 2;//closed ถ้าไม่มี วันปิดก็แสดงยังไมได้ปิด
                            }

                            byte incident_flow = 1;
                            incident objInsert = new incident();
                            string userid = "00000000"; //system
                            string user_report = sheet.GetRow(row).GetCell(12).ToString().Trim();
                           

                            if (!string.IsNullOrEmpty(user_report))
                            {

                                string[] name_report = user_report.Split(' ');

                                if(name_report.Length > 1)
                                {
                                    var e = from em in dbConnect.employees
                                            where em.first_name_en == name_report[1] && em.last_name_en == name_report[0].Trim()
                                            select em;

                                    foreach (var ee in e)
                                    {

                                        userid = ee.employee_id;

                                    }


                                }
                                

                            }
                            string phone = sheet.GetRow(row).GetCell(13).ToString().Trim();
                            string incidentdate = sheet.GetRow(row).GetCell(0).DateCellValue.ToString("dd/MM/yyyy");
                            string incidenttime = "00:00";

                            if (sheet.GetRow(row).GetCell(1).DateCellValue != DateTime.MinValue)
                            {
                                incidenttime = sheet.GetRow(row).GetCell(1).DateCellValue.ToString("HH:mm");
                            }



                            string reportdate = sheet.GetRow(row).GetCell(2).DateCellValue.ToString("dd/MM/yyyy");
                            string reportyear = sheet.GetRow(row).GetCell(2).DateCellValue.ToString("yyyy");
                            string reporttime = "00:00";

                            if (sheet.GetRow(row).GetCell(3).DateCellValue != DateTime.MinValue)
                            {
                                reporttime = sheet.GetRow(row).GetCell(3).DateCellValue.ToString("HH:mm");
                            }

                            string company_id = getMasterdataID(sheet.GetRow(row).GetCell(4).ToString().Trim(), "company", "", "", "");
                            string function_id = getMasterdataID(sheet.GetRow(row).GetCell(5).ToString().Trim(), "function", "", "", "");
                            string department_id = getMasterdataID(sheet.GetRow(row).GetCell(6).ToString().Trim(), "department", function_id, "", "");
                            string division_id = getMasterdataID(sheet.GetRow(row).GetCell(7).ToString().Trim(), "division", function_id, department_id, "");
                            string section_id = getMasterdataID(sheet.GetRow(row).GetCell(8).ToString().Trim(), "section", function_id, department_id, division_id);

                            objInsert.doc_no = generateDocno(reportyear, "srilanka", "+5.5");
                            objInsert.incident_date = FormatDates.changeDateTimeDB(incidentdate + " " + incidenttime, "si");

                            //objInsert.report_date = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5")).AddMinutes(count);

                            objInsert.report_date = FormatDates.changeDateTimeDB(reportdate + " " + reporttime, "si");

                            objInsert.company_id = company_id;
                            objInsert.function_id = function_id;
                            objInsert.department_id = department_id;
                            objInsert.division_id = division_id;
                            objInsert.section_id = section_id;
                            objInsert.incident_area = sheet.GetRow(row).GetCell(9).ToString().Trim();
                            objInsert.incident_name = sheet.GetRow(row).GetCell(10).ToString().Trim();
                            objInsert.incident_detail = sheet.GetRow(row).GetCell(11).ToString().Trim();
                            objInsert.employee_id = userid;
                            objInsert.typeuser_login = "ad";
                            objInsert.phone = phone;
                            objInsert.process_status = process_status;
                            objInsert.step_form = 4;


                            objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                            objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                            objInsert.is_alert_over_due = 0;
                            objInsert.incident_flow = incident_flow;
                            objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                            objInsert.device_type = "import";
                            objInsert.country = "srilanka";

                            objInsert.location_company_id = company_id;
                            objInsert.location_function_id = function_id;
                            objInsert.location_department_id = department_id;
                            objInsert.location_division_id = division_id;
                            objInsert.location_section_id = section_id;



                            objInsert.location_company_name_en = sheet.GetRow(row).GetCell(4).ToString().Trim();
                            objInsert.location_function_name_en = sheet.GetRow(row).GetCell(5).ToString().Trim();


                            if (sheet.GetRow(row).GetCell(6).ToString().Trim() != "")
                            {
                                objInsert.location_department_name_en = sheet.GetRow(row).GetCell(6).ToString().Trim();
                            }
                            else
                            {
                                objInsert.location_department_name_en = "other";
                            }

                            if (sheet.GetRow(row).GetCell(7).ToString().Trim() != "")
                            {
                                objInsert.location_division_name_en = sheet.GetRow(row).GetCell(7).ToString().Trim();
                            }
                            else
                            {
                                objInsert.location_division_name_en = "other";
                            }

                            if (sheet.GetRow(row).GetCell(8).ToString().Trim() != "")
                            {
                                objInsert.location_section_name_en = sheet.GetRow(row).GetCell(8).ToString().Trim();
                            }
                            else
                            {
                                objInsert.location_section_name_en = "other";
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

                                objInsert.reporter_department_id = rc1.sub_function_id;
                                objInsert.reporter_department_name = rc1.sub_function;



                            }

                            //////////////////////////////////////end reporter//////////////////////////////////////////////

                            /////////////////////////////////////form2/////////////////////////////////////////////////////
                            if (sheet.GetRow(row).GetCell(14).ToString().Trim().ToUpper() == "ON-SITE")
                            {
                                objInsert.responsible_area = "IN";
                            }
                            else if (sheet.GetRow(row).GetCell(14).ToString().Trim().ToUpper() == "OFF-SITE")
                            {
                                objInsert.responsible_area = "OUT";
                            }


                            if (sheet.GetRow(row).GetCell(15).ToString().Trim().ToUpper() == "NO IMPACT (NEAR MISS)")
                            {
                                objInsert.impact = "N";
                            }
                            else if (sheet.GetRow(row).GetCell(15).ToString().Trim().ToUpper() == "HAVE AN IMPACT OR CONSEQUENCE")
                            {
                                objInsert.impact = "Y";
                            }


                            if (sheet.GetRow(row).GetCell(16).ToString().Trim().ToUpper() == "NO")
                            {
                                objInsert.injury_fatality_involve = "N";
                            }
                            else if (sheet.GetRow(row).GetCell(16).ToString().Trim().ToUpper() == "YES")
                            {
                                objInsert.injury_fatality_involve = "Y";
                            }


                            if (sheet.GetRow(row).GetCell(25).ToString().Trim().ToUpper() == "NO")
                            {
                                objInsert.effect_environment = "N";
                            }
                            else if (sheet.GetRow(row).GetCell(25).ToString().Trim().ToUpper() == "YES")
                            {
                                objInsert.effect_environment = "Y";
                            }


                            if (sheet.GetRow(row).GetCell(32).ToString().Trim().ToUpper() == "NO")
                            {
                                objInsert.critical = "N";
                            }
                            else if (sheet.GetRow(row).GetCell(32).ToString().Trim().ToUpper() == "YES")
                            {
                                objInsert.critical = "Y";
                            }

                            if (sheet.GetRow(row).GetCell(33).ToString().Trim().ToUpper() == "NO")
                            {
                                objInsert.external_reportable = "N";
                            }
                            else if (sheet.GetRow(row).GetCell(33).ToString().Trim().ToUpper() == "YES")
                            {
                                objInsert.external_reportable = "Y";
                            }

                            objInsert.immediate_temporary = sheet.GetRow(row).GetCell(34).ToString();
                            objInsert.consequence_level = getConsequenceLevelID(sheet.GetRow(row).GetCell(35).ToString().Trim());

                            objInsert.contributing_factor = sheet.GetRow(row).GetCell(42).ToString().Trim();

                            if (sheet.GetRow(row).GetCell(47).ToString().Trim().ToUpper() != "GUILTY OF INSEE GROUP")
                            {
                                objInsert.culpability = "G";
                            }
                            else if (sheet.GetRow(row).GetCell(47).ToString().Trim().ToUpper() != "PARTIAL GUILTY TO INSEE GROUP")
                            {
                                objInsert.culpability = "P";
                            }
                            else if (sheet.GetRow(row).GetCell(47).ToString().Trim().ToUpper() != "NO GUILTY TO INSEE GROUP")
                            {
                                objInsert.culpability = "N";
                            }


                            if (sheet.GetRow(row).GetCell(48).ToString().Trim() != "")
                            {
                                objInsert.form2_function_id = getMasterdataID(sheet.GetRow(row).GetCell(48).ToString().Trim(), "function", "", "", "");
                            }


                            if (sheet.GetRow(row).GetCell(49).ToString().Trim().ToUpper() != "NO")
                            {
                                objInsert.road_accident = "N";
                            }
                            else if (sheet.GetRow(row).GetCell(49).ToString().Trim().ToUpper() != "YES")
                            {
                                objInsert.road_accident = "Y";
                            }

                            if (sheet.GetRow(row).GetCell(50).ToString().Trim() != "")
                            {
                                objInsert.fatality_prevention_element_id = getFPEID(sheet.GetRow(row).GetCell(50).ToString().Trim());

                            }

                            objInsert.work_relate = "Y";

                            if (sheet.GetRow(row).GetCell(51).DateCellValue != DateTime.MinValue)
                            {
                                objInsert.close_incident_date = FormatDates.changeDateTimeDB(sheet.GetRow(row).GetCell(51).DateCellValue.ToString("dd/MM/yyyy" + " 00:00"), "si");
                            }

                            dbConnect.incidents.InsertOnSubmit(objInsert);
                            dbConnect.SubmitChanges();

                            //////////////////////////other impact/////////////////////////////


                            deleteOtherImpact(objInsert.id.ToString());

                            if (sheet.GetRow(row).GetCell(29).ToString().Trim().ToUpper() == "YES")
                            {
                                other_impact objInsert4 = new other_impact();
                                objInsert4.other_impact_value = "image";
                                objInsert4.incident_id = objInsert.id;
                                dbConnect.other_impacts.InsertOnSubmit(objInsert4);
                                dbConnect.SubmitChanges();
                            }


                            if (sheet.GetRow(row).GetCell(30).ToString().Trim().ToUpper() == "YES")
                            {
                                other_impact objInsert5 = new other_impact();
                                objInsert5.other_impact_value = "legal";
                                objInsert5.incident_id = objInsert.id;
                                dbConnect.other_impacts.InsertOnSubmit(objInsert5);
                                dbConnect.SubmitChanges();
                            }


                            if (sheet.GetRow(row).GetCell(31).ToString().Trim().ToUpper() == "YES")
                            {
                                other_impact objInsert6 = new other_impact();
                                objInsert6.other_impact_value = "manufacturing";
                                objInsert6.incident_id = objInsert.id;
                                dbConnect.other_impacts.InsertOnSubmit(objInsert6);
                                dbConnect.SubmitChanges();
                            }



                            /////////////////////////////////////////////injury///////////////////////////////////////////////


                            if (sheet.GetRow(row).GetCell(17).ToString().Trim() != "")
                            {
                                injury_person objInsert2 = new injury_person();

                                objInsert2.full_name = sheet.GetRow(row).GetCell(17).ToString().Trim();
                                objInsert2.employee_id = getEmployeeIDByName(sheet.GetRow(row).GetCell(17).ToString().Trim());


                                objInsert2.type_employment_id = getTypeEmployeeID(sheet.GetRow(row).GetCell(18).ToString().Trim());

                                objInsert.function_id = getMasterdataID(sheet.GetRow(row).GetCell(19).ToString().Trim(), "function", "", "", "");
                                // objInsert.department_id = department_id;

                                objInsert2.nature_injury_id = getNatureInjuryID(sheet.GetRow(row).GetCell(20).ToString().Trim());

                                objInsert2.body_parts_id = getBodyPartID(sheet.GetRow(row).GetCell(21).ToString().Trim());

                                objInsert2.severity_injury_id = getSeverityInjuryID(sheet.GetRow(row).GetCell(22).ToString().Trim());

                                if (sheet.GetRow(row).GetCell(23).StringCellValue.Trim() != "")
                                {
                                    objInsert2.day_lost = Convert.ToInt32(sheet.GetRow(row).GetCell(23).ToString().Trim());
                                }

                                objInsert2.remark = sheet.GetRow(row).GetCell(24).ToString().Trim();
                                objInsert2.incident_id = objInsert.id;
                                objInsert2.status = "A";
                                dbConnect.injury_persons.InsertOnSubmit(objInsert2);

                                dbConnect.SubmitChanges();
                            }



                            //////////////////////////////////////end injury/////////////////////////////////////////////

                            ///////////////////////////////////////fact finding////////////////////////////////////////////

                            if (sheet.GetRow(row).GetCell(36).ToString().Trim() != "")
                            {
                                fact_finding objInsert7 = new fact_finding();

                                objInsert7.fact_finding_name = sheet.GetRow(row).GetCell(36).ToString().Trim();
                                objInsert7.source_incident_id = getSourceOfIncidentID(sheet.GetRow(row).GetCell(37).ToString().Trim());
                                objInsert7.event_exposure_id = getEventExposureID(sheet.GetRow(row).GetCell(38).ToString().Trim());

                                if (sheet.GetRow(row).GetCell(39).ToString().Trim().ToUpper() == "NO")
                                {
                                    objInsert7.unsafe_action = "N";
                                }
                                else if (sheet.GetRow(row).GetCell(39).ToString().Trim().ToUpper() == "YES")
                                {
                                    objInsert7.unsafe_action = "Y";
                                }

                                if (sheet.GetRow(row).GetCell(40).ToString().Trim().ToUpper() == "NO")
                                {
                                    objInsert7.unsafe_condition = "N";
                                }
                                else if (sheet.GetRow(row).GetCell(40).ToString().Trim().ToUpper() == "YES")
                                {
                                    objInsert7.unsafe_condition = "Y";
                                }


                                objInsert7.incident_id = objInsert.id;
                                objInsert7.status = "A";


                                dbConnect.fact_findings.InsertOnSubmit(objInsert7);

                                dbConnect.SubmitChanges();
                            }


                            ///////////////////////////////end/////////////////////////////////

                            //////////////////////////////root cause/////////////////////////////


                            if (sheet.GetRow(row).GetCell(41).ToString().Trim() != "")
                            {
                                root_cause_action objInsert8 = new root_cause_action();

                                objInsert8.name = sheet.GetRow(row).GetCell(41).ToString().Trim();
                                objInsert8.incident_id = objInsert.id;
                                objInsert8.status = "A";

                                dbConnect.root_cause_actions.InsertOnSubmit(objInsert8);

                                dbConnect.SubmitChanges();
                            }

                            ////////////////////////////end//////////////////////////////////////

                            //////////////////////////create corrective/////////////////////////


                            if (sheet.GetRow(row).GetCell(43).ToString().Trim() != "")
                            {
                                corrective_prevention_action_incident objInsert9 = new corrective_prevention_action_incident();

                                objInsert9.corrective_preventive_action = sheet.GetRow(row).GetCell(43).ToString().Trim();
                                objInsert9.responsible_person = sheet.GetRow(row).GetCell(44).ToString().Trim();

                                //string duedate = sheet.GetRow(row).GetCell(45).DateCellValue.ToString();
                                if (sheet.GetRow(row).GetCell(45).DateCellValue != DateTime.MinValue)
                                {
                                    objInsert9.due_date = FormatDates.changeDateTimeDB(sheet.GetRow(row).GetCell(45).DateCellValue.ToString("dd/MM/yyyy" + " 00:00"), "si");
                                    objInsert9.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));

                                }

                                if (process_status == 2)//2 is closed
                                {
                                    objInsert9.action_status_id = 4;//close
                                }
                                else
                                {
                                    objInsert9.action_status_id = 1;//on process

                                }

                                objInsert9.incident_id = objInsert.id;
                                //objInsert9.action_status_id = getActionStatusID(sheet.GetRow(row).GetCell(46).ToString().Trim());//ไม่รู้ใช้คอลัมไหน
                                // objInsert9.attachment_file = attachment_file;
                                objInsert9.employee_id = getEmployeeIDByName(sheet.GetRow(row).GetCell(44).ToString().Trim());
                                // objInsert9.assign_by_employee_id = user_id;


                                objInsert9.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                                dbConnect.corrective_prevention_action_incidents.InsertOnSubmit(objInsert9);
                                dbConnect.SubmitChanges();

                            }




                            //////////////////////////end//////////////////////////////////////

                            incident_detail objInsert3 = new incident_detail();
                            objInsert3.employee_id = userid;
                            objInsert3.type_login = "ad";
                            objInsert3.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                            objInsert3.process_status = process_status;
                            objInsert3.incident_id = objInsert.id;

                            dbConnect.incident_details.InsertOnSubmit(objInsert3);

                            dbConnect.SubmitChanges();


                            count++;
                        }

                       

                    }//end foreach

                   

                

                //}
                //catch (Exception e)
                //{
                //    //log_import_data_old insertLog = new log_import_data_old();

                //    //insertLog.error_message = e.Message;
                //    //insertLog.row_number = row_number;

                //    //dbConnect.log_import_data_olds.InsertOnSubmit(insertLog);
                //    //dbConnect.SubmitChanges();
                   

                //}
               






            }
        }











        [WebMethod]
        public void importOldDataHazardFromExcel()
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;

                string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
                string path = string.Format("{0}" + pathreport + "srilanka_data_old_hazard.xlsx", Server.MapPath(@"\"));
                FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                //try
                //{
                XSSFWorkbook workbook = new XSSFWorkbook(file);
                int count = 0;

                ISheet sheet2 = workbook.GetSheet("Hazard");


                for (int row = 1; row <= sheet2.LastRowNum; row++)
                {
                    if (sheet2.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        int process_status = 1;//getStatusForm(sheet2.GetRow(row).GetCell(26).ToString().Trim());//ใช้คอบัมไหนยังไม่รุ้ใส่ไปก่อน

                        if (sheet2.GetRow(row).GetCell(27).DateCellValue != DateTime.MinValue)
                        {
                            process_status = 2;//closed ถ้าไม่มี วันปิดก็แสดงยังไมได้ปิด
                        }

                        byte incident_flow = 1;
                        hazard objInsert = new hazard();
                        string userid = "00000000";//system

                        string user_report = sheet2.GetRow(row).GetCell(14).ToString().Trim();


                        if (!string.IsNullOrEmpty(user_report))
                        {

                            string[] name_report = user_report.Split(' ');

                            if (name_report.Length > 1)
                            {
                                var e = from em in dbConnect.employees
                                        where em.first_name_en == name_report[1] && em.last_name_en == name_report[0].Trim()
                                        select em;

                                foreach (var ee in e)
                                {

                                    userid = ee.employee_id;

                                }


                            }


                        }


                        string phone = sheet2.GetRow(row).GetCell(15).ToString().Trim();
                        string hazarddate = sheet2.GetRow(row).GetCell(0).DateCellValue.ToString("dd/MM/yyyy");
                        string hazardtime = "00:00";

                        if (sheet2.GetRow(row).GetCell(1).DateCellValue != DateTime.MinValue)
                        {
                            hazardtime = sheet2.GetRow(row).GetCell(1).DateCellValue.ToString("HH:mm");
                        }

                        string reportdate = sheet2.GetRow(row).GetCell(2).DateCellValue.ToString("dd/MM/yyyy");
                        string reportyear = sheet2.GetRow(row).GetCell(2).DateCellValue.ToString("yyyy");
                        string reporttime = "00:00";

                        if (sheet2.GetRow(row).GetCell(3).DateCellValue != DateTime.MinValue)
                        {
                            reporttime = sheet2.GetRow(row).GetCell(3).DateCellValue.ToString("HH:mm");
                        }

                        string company_id = getMasterdataID(sheet2.GetRow(row).GetCell(4).ToString().Trim(), "company", "", "", "");
                        string function_id = getMasterdataID(sheet2.GetRow(row).GetCell(5).ToString().Trim(), "function", "", "", "");
                        string department_id = getMasterdataID(sheet2.GetRow(row).GetCell(6).ToString().Trim(), "department", function_id, "", "");
                        string division_id = getMasterdataID(sheet2.GetRow(row).GetCell(7).ToString().Trim(), "division", function_id, department_id, "");
                        string section_id = getMasterdataID(sheet2.GetRow(row).GetCell(8).ToString().Trim(), "section", function_id, department_id, division_id);

                        objInsert.doc_no = generateDocnoHazard(reportyear,"srilanka", "+5.5");
                        objInsert.hazard_date = FormatDates.changeDateTimeDB(hazarddate + " " + hazardtime, "si");

                        objInsert.report_date = FormatDates.changeDateTimeDB(reportdate + " " + reporttime, "si");
                        //objInsert.report_date = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5")).AddMinutes(count);

                        objInsert.company_id = company_id;
                        objInsert.function_id = function_id;
                        objInsert.department_id = department_id;
                        objInsert.division_id = division_id;
                        objInsert.section_id = section_id;
                        objInsert.hazard_area = sheet2.GetRow(row).GetCell(9).ToString().Trim();
                        objInsert.hazard_name = sheet2.GetRow(row).GetCell(10).ToString().Trim();
                        objInsert.hazard_detail = sheet2.GetRow(row).GetCell(11).ToString().Trim();
                        objInsert.preliminary_action = sheet2.GetRow(row).GetCell(12).ToString().Trim();

                        if (sheet2.GetRow(row).GetCell(13).ToString().Trim() == "Pending for action")
                        {
                            objInsert.type_action = "P";
                        }
                        else if (sheet2.GetRow(row).GetCell(13).ToString().Trim() == "Temporary control")
                        {
                            objInsert.type_action = "T";
                        }
                        else if (sheet2.GetRow(row).GetCell(13).ToString().Trim() == "Complete control")
                        {
                            objInsert.type_action = "C";
                        }

                        objInsert.employee_id = userid;
                        objInsert.typeuser_login = "ad";
                        objInsert.phone = phone;
                        objInsert.process_status = process_status;
                        objInsert.step_form = 4;

                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                        objInsert.is_alert_over_due = 0;
                        objInsert.hazard_flow = incident_flow;
                        objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                        objInsert.device_type = "import";
                        objInsert.country = "srilanka";

                        objInsert.location_company_id = company_id;
                        objInsert.location_function_id = function_id;
                        objInsert.location_department_id = department_id;
                        objInsert.location_division_id = division_id;
                        objInsert.location_section_id = section_id;

                        if (sheet2.GetRow(row).GetCell(4).ToString().Trim() != "")
                        {
                            objInsert.location_company_name_en = sheet2.GetRow(row).GetCell(4).ToString().Trim();
                        }
                        else
                        {
                            objInsert.location_company_name_en = "other";
                        }


                        if (sheet2.GetRow(row).GetCell(5).ToString().Trim() != "")
                        {
                            objInsert.location_function_name_en = sheet2.GetRow(row).GetCell(5).ToString().Trim();
                        }
                        else
                        {
                            objInsert.location_function_name_en = "other";
                        }


                        if (sheet2.GetRow(row).GetCell(6).ToString().Trim() != "")
                        {
                            objInsert.location_department_name_en = sheet2.GetRow(row).GetCell(6).ToString().Trim();
                        }
                        else
                        {
                            objInsert.location_department_name_en = "other";
                        }


                        if (sheet2.GetRow(row).GetCell(7).ToString().Trim() != "")
                        {
                            objInsert.location_division_name_en = sheet2.GetRow(row).GetCell(7).ToString().Trim();
                        }
                        else
                        {
                            objInsert.location_division_name_en = "other";
                        }

                        if (sheet2.GetRow(row).GetCell(8).ToString().Trim() != "")
                        {
                            objInsert.location_section_name_en = sheet2.GetRow(row).GetCell(8).ToString().Trim();
                        }
                        else
                        {
                            objInsert.location_section_name_en = "other";
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

                            objInsert.reporter_department_id = rc1.sub_function_id;
                            objInsert.reporter_department_name = rc1.sub_function;



                        }

                        //////////////////////////////////////end reporter//////////////////////////////////////////////

                        if (sheet2.GetRow(row).GetCell(16).DateCellValue != DateTime.MinValue)
                        {
                            string verifydate = sheet2.GetRow(row).GetCell(16).DateCellValue.ToString("dd/MM/yyyy");
                            string verifytime = "00:00";
                            if (sheet2.GetRow(row).GetCell(17).DateCellValue != DateTime.MinValue)
                            {
                                verifytime = sheet2.GetRow(row).GetCell(17).DateCellValue.ToString("HH:mm");
                            }

                            objInsert.verifying_date = FormatDates.changeDateTimeDB(verifydate + " " + verifytime, "si");
                        }

                        if (sheet2.GetRow(row).GetCell(18).ToString().Trim() != "")
                        {
                            objInsert.source_hazard = getSourceOfHazardID(sheet2.GetRow(row).GetCell(18).ToString().Trim());
                        }

                        if (sheet2.GetRow(row).GetCell(19).ToString().Trim() != "")
                        {
                            objInsert.fatality_prevention_element_id = getFPEID(sheet2.GetRow(row).GetCell(19).ToString().Trim());
                        }


                        if (sheet2.GetRow(row).GetCell(20).ToString().Trim() == "Low")
                        {
                            objInsert.type_action = "L";
                        }
                        else if (sheet2.GetRow(row).GetCell(20).ToString().Trim() == "Medium")
                        {
                            objInsert.type_action = "M";
                        }
                        else if (sheet2.GetRow(row).GetCell(20).ToString().Trim() == "High")
                        {
                            objInsert.type_action = "H";
                        }

                        if (sheet2.GetRow(row).GetCell(21).ToString().Trim() != "")
                        {
                            objInsert.safety_officer_id = getEmployeeIDByName(sheet2.GetRow(row).GetCell(21).ToString().Trim());
                        }



                        if (sheet2.GetRow(row).GetCell(27).DateCellValue != DateTime.MinValue)
                        {
                            objInsert.close_hazard_date = FormatDates.changeDateTimeDB(sheet2.GetRow(row).GetCell(27).DateCellValue.ToString("dd/MM/yyyy" + " 00:00"), "si");
                        }


                        dbConnect.hazards.InsertOnSubmit(objInsert);
                        dbConnect.SubmitChanges();

                        //////////////////////////////////////////action//////////////////////////////////////

                        if (sheet2.GetRow(row).GetCell(22).ToString().Trim() != "")
                        {
                            process_action objInsert2 = new process_action();

                            if (sheet2.GetRow(row).GetCell(23).ToString().Trim() != "")
                            {
                                objInsert2.type_control = getTypeControlID(sheet2.GetRow(row).GetCell(23).ToString().Trim());
                            }

                            objInsert2.action = sheet2.GetRow(row).GetCell(22).ToString().Trim();
                            objInsert2.responsible_person = sheet2.GetRow(row).GetCell(24).ToString().Trim();
                            objInsert2.employee_id = getEmployeeIDByName(sheet2.GetRow(row).GetCell(24).ToString().Trim());


                            if (sheet2.GetRow(row).GetCell(25).DateCellValue != DateTime.MinValue)
                            {
                                objInsert2.due_date = FormatDates.changeDateTimeDB(sheet2.GetRow(row).GetCell(25).DateCellValue.ToString("dd/MM/yyyy" + " 00:00"), "si");
                                objInsert2.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                              

                            }
                            


                            if (process_status == 2)//2 is closed
                            {
                                objInsert2.action_status_id = 4;//closed
                            }
                            else
                            {
                                objInsert2.action_status_id = 1;//on process
                            }

                            //objInsert2.notify_contractor = notify_contractor;
                            //objInsert2.remark = remark;
                            objInsert2.hazard_id = objInsert.id;
                           
                            //objInsert.attachment_file = attachment_file;
                            // objInsert.root_cause_action = root_cause_action;
                            objInsert2.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));


                            dbConnect.process_actions.InsertOnSubmit(objInsert2);

                            dbConnect.SubmitChanges();

                        }



                        ////////////////////////////////////end///////////////////////////////////////////

                    }





                }//end foreach




                //}
                //catch (Exception e)
                //{
                //    //log_import_data_old insertLog = new log_import_data_old();

                //    //insertLog.error_message = e.Message;
                //    //insertLog.row_number = row_number;

                //    //dbConnect.log_import_data_olds.InsertOnSubmit(insertLog);
                //    //dbConnect.SubmitChanges();


                //}







            }
        }













        [WebMethod]
        public void importOldDataSOTFromExcel()
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;

                string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
                string path = string.Format("{0}" + pathreport + "srilanka_data_old_sot.xlsx", Server.MapPath(@"\"));
                FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);

                //try
                //{

                XSSFWorkbook workbook = new XSSFWorkbook(file);
                int count = 0;
              
                ISheet sheet3 = workbook.GetSheet("SOT");


                for (int row = 1; row <= sheet3.LastRowNum; row++)
                {
                    if (sheet3.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        int process_status = 2;//getStatusForm(sheet.GetRow(row).GetCell(45).ToString().Trim());//ไม่รู้ใช้คอลัมไหน มันปนกับ status action ป่ะ

                        sot objInsert = new sot();
                        string userid = "00000000";//system
                        string sotdate = sheet3.GetRow(row).GetCell(0).DateCellValue.ToString("dd/MM/yyyy");
                        string sottime = "00:00";
                        string sottime_end = "00:00";
                        if (sheet3.GetRow(row).GetCell(1).DateCellValue != DateTime.MinValue)
                        {
                            sottime = sheet3.GetRow(row).GetCell(1).DateCellValue.ToString("HH:mm");
                        }


                        if (sheet3.GetRow(row).GetCell(2).DateCellValue != DateTime.MinValue)
                        {
                            sottime_end = sheet3.GetRow(row).GetCell(2).DateCellValue.ToString("HH:mm");
                        }


                        string reportdate = sheet3.GetRow(row).GetCell(11).DateCellValue.ToString("dd/MM/yyyy");
                        string reportyear = sheet3.GetRow(row).GetCell(11).DateCellValue.ToString("yyyy");
                        string reporttime = "00:00";

                        if (sheet3.GetRow(row).GetCell(11).DateCellValue != DateTime.MinValue)
                        {
                            reporttime = sheet3.GetRow(row).GetCell(11).DateCellValue.ToString("HH:mm");
                        }


                        string company_id = getMasterdataID(sheet3.GetRow(row).GetCell(4).ToString().Trim(), "company", "", "", "");
                        string function_id = getMasterdataID(sheet3.GetRow(row).GetCell(5).ToString().Trim(), "function", "", "", "");
                        string department_id = getMasterdataID(sheet3.GetRow(row).GetCell(6).ToString().Trim(), "department", function_id, "", "");
                        string division_id = getMasterdataID(sheet3.GetRow(row).GetCell(7).ToString().Trim(), "division", function_id, department_id, "");

                        objInsert.doc_no = generateDocnoSot(reportyear, "srilanka", "+5.5");
                        objInsert.sot_date = FormatDates.changeDateTimeDB(sotdate + " " + sottime, "si");
                        objInsert.sot_date_end = FormatDates.changeDateTimeDB(sotdate + " " + sottime_end, "si");

                        if (reportdate != "")
                        {
                            objInsert.report_date = FormatDates.changeDateTimeDB(reportdate + " " + reporttime, "si");

                        }
                        else
                        {
                            objInsert.report_date = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5")).AddMinutes(count);
                        }



                        objInsert.company_id = company_id;
                        objInsert.function_id = function_id;
                        objInsert.department_id = department_id;
                        objInsert.division_id = division_id;
                        objInsert.location = sheet3.GetRow(row).GetCell(8).ToString().Trim();
                        objInsert.type_work = sheet3.GetRow(row).GetCell(9).ToString().Trim();

                        if (sheet3.GetRow(row).GetCell(10).ToString().Trim() != "")
                        {
                            objInsert.type_employment_id = getTypeEmployeeID(sheet3.GetRow(row).GetCell(10).ToString().Trim());
                        }
                        objInsert.comment = sheet3.GetRow(row).GetCell(12).ToString().Trim();
                        objInsert.employee_id = userid;
                        objInsert.typeuser_login = "import";
                        objInsert.process_status = process_status;

                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                        objInsert.country = "srilanka";

                        objInsert.location_company_id = company_id;
                        objInsert.location_function_id = function_id;
                        objInsert.location_department_id = department_id;
                        objInsert.location_division_id = division_id;


                        if (sheet3.GetRow(row).GetCell(4).ToString().Trim() != "")
                        {
                            objInsert.location_company_name_en = sheet3.GetRow(row).GetCell(4).ToString().Trim();
                        }
                        else
                        {
                            objInsert.location_company_name_en = "other";
                        }

                        if (sheet3.GetRow(row).GetCell(5).ToString().Trim() != "")
                        {
                            objInsert.location_function_name_en = sheet3.GetRow(row).GetCell(5).ToString().Trim();
                        }
                        else
                        {
                            objInsert.location_function_name_en = "other";
                        }


                        if (sheet3.GetRow(row).GetCell(6).ToString().Trim() != "")
                        {
                            objInsert.location_department_name_en = sheet3.GetRow(row).GetCell(6).ToString().Trim();

                        }
                        else
                        {

                            objInsert.location_department_name_en = "other";
                        }


                        if (sheet3.GetRow(row).GetCell(7).ToString().Trim() != "")
                        {
                            objInsert.location_division_name_en = sheet3.GetRow(row).GetCell(7).ToString().Trim();

                        }
                        else
                        {

                            objInsert.location_division_name_en = "other";
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
                            objInsert.reporter_company_name = rc1.company;
                            objInsert.reporter_function_name = rc1.function;
                            objInsert.reporter_division_name = rc1.division;

                            objInsert.reporter_department_id = rc1.sub_function_id;
                            objInsert.reporter_department_name = rc1.sub_function;



                        }

                        //////////////////////////////////////end reporter//////////////////////////////////////////////



                        dbConnect.sots.InsertOnSubmit(objInsert);
                        dbConnect.SubmitChanges();

                        //////////////////////////////////////////////////////////////////////////////////////////

                        if (sheet3.GetRow(row).GetCell(3).ToString().Trim() != "")
                        {
                            deleteEmployeeSot(objInsert.id);

                            string name = sheet3.GetRow(row).GetCell(3).ToString().Trim();
                            string[] words = name.Split(' ');

                            string first_name = "";
                            string last_name = "";
                            string employee_id = "";

                            for (int i = 0; i < words.Length; i++)
                            {

                                if ((i % 2) == 1)
                                {
                                    first_name = words[i];

                                    var v = from c in dbConnect.employees
                                            where c.first_name_en == first_name && c.last_name_en == last_name
                                            select c;

                                    if (v.Count() != 0)
                                    {
                                        foreach (var rc in v)
                                        {
                                            employee_id = rc.employee_id;
                                        }

                                    }
                                    else
                                    {
                                        log_import_data_old insertLog = new log_import_data_old();

                                        insertLog.error_message = objInsert.id.ToString();
                                        insertLog.row_number = count;
                                        insertLog.title = "sot";

                                        dbConnect.log_import_data_olds.InsertOnSubmit(insertLog);
                                        dbConnect.SubmitChanges();
                                    }


                                    employee_has_sot objInsert3 = new employee_has_sot();
                                    objInsert3.employee_id = employee_id;
                                    objInsert3.sot_id = objInsert.id;
                                    dbConnect.employee_has_sots.InsertOnSubmit(objInsert3);


                                    dbConnect.SubmitChanges();
                                }
                                else
                                {
                                    last_name = words[i];
                                }

                            }


                        }



                        /////////////////////////////////////////////////////////////////////////////////

                        //sot_has_reactions_people objR = new sot_has_reactions_people();
                        //objR.changinge_position = sheet3.GetRow(row).GetCell(13).ToString().Trim();
                        //objR.stopping_work = sheet3.GetRow(row).GetCell(14).ToString().Trim();
                        //objR.rearranging_job = sheet3.GetRow(row).GetCell(15).ToString().Trim();
                        //objR.hiding_dodging = sheet3.GetRow(row).GetCell(16).ToString().Trim();
                        //objR.changing_tools = sheet3.GetRow(row).GetCell(17).ToString().Trim();
                        //objR.applying_lockout = sheet3.GetRow(row).GetCell(18).ToString().Trim();
                        //objR.adjusting_ppe = sheet3.GetRow(row).GetCell(19).ToString().Trim();
                        //objR.description = sheet3.GetRow(row).GetCell(12).ToString().Trim();
                        //objR.sot_id = objInsert.id;
                        //dbConnect.sot_has_reactions_peoples.InsertOnSubmit(objR);
                        //dbConnect.SubmitChanges();

                        //sot_has_position_people objP = new sot_has_position_people();
                        //objP.striking_against = sheet3.GetRow(row).GetCell(21).ToString().Trim();
                        //objP.caught_between = sheet3.GetRow(row).GetCell(22).ToString().Trim();
                        //objP.inhaling = sheet3.GetRow(row).GetCell(23).ToString().Trim();
                        //objP.absorbing = sheet3.GetRow(row).GetCell(24).ToString().Trim();
                        //objP.electricity = sheet3.GetRow(row).GetCell(25).ToString().Trim();
                        //objP.falling = sheet3.GetRow(row).GetCell(26).ToString().Trim();
                        //objP.struck_by = sheet3.GetRow(row).GetCell(27).ToString().Trim();
                        //objP.line_fire = sheet3.GetRow(row).GetCell(28).ToString().Trim();
                        //objP.eyes_tasks = sheet3.GetRow(row).GetCell(29).ToString().Trim();
                        //objP.lifting_lowering = sheet3.GetRow(row).GetCell(30).ToString().Trim();
                        //objP.posture = sheet3.GetRow(row).GetCell(31).ToString().Trim();
                        //objP.description = sheet3.GetRow(row).GetCell(20).ToString().Trim();
                        //objP.sot_id = objInsert.id;
                        //dbConnect.sot_has_position_peoples.InsertOnSubmit(objP);
                        //dbConnect.SubmitChanges();

                        //sot_has_personal_protection_equipment objPer = new sot_has_personal_protection_equipment();
                        //objPer.head = sheet3.GetRow(row).GetCell(33).ToString().Trim();
                        //objPer.ears_eyes = sheet3.GetRow(row).GetCell(34).ToString().Trim();
                        //objPer.face_respiratory = sheet3.GetRow(row).GetCell(35).ToString().Trim();
                        //objPer.hand_arms = sheet3.GetRow(row).GetCell(36).ToString().Trim();
                        //objPer.feet_legs = sheet3.GetRow(row).GetCell(37).ToString().Trim();
                        //objPer.description = sheet3.GetRow(row).GetCell(32).ToString().Trim();
                        //objPer.sot_id = objInsert.id;
                        //dbConnect.sot_has_personal_protection_equipments.InsertOnSubmit(objPer);
                        //dbConnect.SubmitChanges();

                        //sot_has_tools_equipment objT = new sot_has_tools_equipment();
                        //objT.right_job = sheet3.GetRow(row).GetCell(39).ToString().Trim();
                        //objT.used_correctly = sheet3.GetRow(row).GetCell(40).ToString().Trim();
                        //objT.safe_conditions = sheet3.GetRow(row).GetCell(41).ToString().Trim();
                        //objT.hamesses = sheet3.GetRow(row).GetCell(42).ToString().Trim();
                        //objT.barricades_warning_lights = sheet3.GetRow(43).GetCell(20).ToString().Trim();
                        //objT.chock_restraints = sheet3.GetRow(row).GetCell(44).ToString().Trim();
                        //objT.prejob_safety_checks = sheet3.GetRow(row).GetCell(45).ToString().Trim();
                        //objT.description = sheet3.GetRow(row).GetCell(38).ToString().Trim();
                        //objT.sot_id = objInsert.id;
                        //dbConnect.sot_has_tools_equipments.InsertOnSubmit(objT);
                        //dbConnect.SubmitChanges();

                        //sot_has_procedure objPro = new sot_has_procedure();
                        //objPro.standard_adequate_job = sheet3.GetRow(row).GetCell(47).ToString().Trim();
                        //objPro.standard_established = sheet3.GetRow(row).GetCell(48).ToString().Trim();
                        //objPro.standard_maintained = sheet3.GetRow(row).GetCell(49).ToString().Trim();
                        //objPro.isolation_lockout = sheet3.GetRow(row).GetCell(50).ToString().Trim();
                        //objPro.hot_work_permit = sheet3.GetRow(row).GetCell(51).ToString().Trim();
                        //objPro.confined_space_permit = sheet3.GetRow(row).GetCell(52).ToString().Trim();
                        //objPro.electrical_permit = sheet3.GetRow(row).GetCell(53).ToString().Trim();
                        //objPro.work_height_permit = sheet3.GetRow(row).GetCell(54).ToString().Trim();
                        //objPro.rescue_plan_place = sheet3.GetRow(row).GetCell(55).ToString().Trim();
                        //objPro.description = sheet3.GetRow(row).GetCell(46).ToString().Trim();
                        //objPro.sot_id = objInsert.id;
                        //dbConnect.sot_has_procedures.InsertOnSubmit(objPro);
                        //dbConnect.SubmitChanges();

                        //sot_has_orderliness_tidiness objO = new sot_has_orderliness_tidiness();
                        //objO.standards_established_understood = sheet3.GetRow(row).GetCell(57).ToString().Trim();
                        //objO.walkway_passageways = sheet3.GetRow(row).GetCell(58).ToString().Trim();
                        //objO.disorganized_tools_bench = sheet3.GetRow(row).GetCell(59).ToString().Trim();
                        //objO.materials_storage = sheet3.GetRow(row).GetCell(60).ToString().Trim();
                        //objO.obstructions_leaning_items = sheet3.GetRow(row).GetCell(61).ToString().Trim();
                        //objO.stairs_platforms = sheet3.GetRow(row).GetCell(62).ToString().Trim();
                        //objO.description = sheet3.GetRow(row).GetCell(56).ToString().Trim();
                        //objO.sot_id = objInsert.id;
                        //dbConnect.sot_has_orderliness_tidinesses.InsertOnSubmit(objO);
                        //dbConnect.SubmitChanges();

                        //sot_has_environment objE = new sot_has_environment();
                        //objE.housekeeping = sheet3.GetRow(row).GetCell(64).ToString().Trim();
                        //objE.chemical_storage = sheet3.GetRow(row).GetCell(65).ToString().Trim();
                        //objE.waste_diposal = sheet3.GetRow(row).GetCell(66).StringCellValue.Trim();
                        //objE.walking_working_surface = sheet3.GetRow(row).GetCell(67).StringCellValue.Trim();
                        //objE.sot_id = objInsert.id;
                        //objE.description = sheet3.GetRow(row).GetCell(63).StringCellValue.Trim();
                        //dbConnect.sot_has_environments.InsertOnSubmit(objE);
                        //dbConnect.SubmitChanges();


                        ////////////////////////////////////////////////sot detail///////////////////////////////////////////////
                        //sot_detail objInsert2 = new sot_detail();
                        //objInsert2.employee_id = userid;
                        //objInsert2.type_login = "ad";
                        //objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble("+5.5"));
                        //objInsert2.process_status = process_status;
                        //objInsert2.sot_id = objInsert.id;

                        //dbConnect.sot_details.InsertOnSubmit(objInsert2);

                        //dbConnect.SubmitChanges();

                        count++;

                    }




                }//end foreach


                //}
                //catch (Exception e)
                //{
                //    //log_import_data_old insertLog = new log_import_data_old();

                //    //insertLog.error_message = e.Message;
                //    //insertLog.row_number = row_number;

                //    //dbConnect.log_import_data_olds.InsertOnSubmit(insertLog);
                //    //dbConnect.SubmitChanges();


                //}


            }
        }

        [WebMethod]
        public void orderDocnoIncident()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var doc_no_max_import = dbConnect.incidents.Where(x => x.country == "srilanka").Where(x => x.doc_no.Contains("I2017")).Where(x=>x.device_type == "import").Max(t => t.doc_no);

                string[] last = doc_no_max_import.Split('-');
                string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathfile"];


               
                var i = from c in dbConnect.incidents
                        where c.country == "srilanka"
                        && c.device_type != "import"
                        orderby c.doc_no ascending
                        select c;


                //int count = 1;
                foreach (var rc in i)
                {
                    string[] docno_old = rc.doc_no.Split('-');
                    int number = Convert.ToInt32(last[1]) + Convert.ToInt32(docno_old[1]);
                    string docno_new = "I2017" + "-" + (number.ToString("D5"));
                   
                    string pathfile = string.Format("{0}" + pathupload + "incident" + "\\step3\\srilanka\\" + rc.doc_no, Server.MapPath(@"\"));
                    string pathfile_new = string.Format("{0}" + pathupload + "incident" + "\\step3\\srilanka\\" + docno_new, Server.MapPath(@"\"));

                    if (Directory.Exists(pathfile))
                    {

                        System.IO.Directory.Move(pathfile, pathfile_new);

                    }
                  

                    rc.doc_no = docno_new;


                    //count++;
                }

                dbConnect.SubmitChanges();


            }


        }



        [WebMethod]
        public void orderDocnoSot()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var doc_no_max_import = dbConnect.sots.Where(x => x.country == "srilanka").Where(x => x.doc_no.Contains("SOT2017")).Where(x => x.typeuser_login == "import").Max(t => t.doc_no);

                string[] last = doc_no_max_import.Split('-');
                string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathfile"];



                var s = from c in dbConnect.sots
                        where c.country == "srilanka"
                        && c.typeuser_login != "import"
                        orderby c.doc_no ascending
                        select c;


                //int count = 1;
                foreach (var rc in s)
                {
                    string[] docno_old = rc.doc_no.Split('-');
                    int number = Convert.ToInt32(last[1]) + Convert.ToInt32(docno_old[1]);
                    string docno_new = "SOT2017" + "-" + (number.ToString("D5"));

                    string pathfile = string.Format("{0}" + pathupload + "sot" + "\\srilanka\\action\\" + rc.doc_no, Server.MapPath(@"\"));
                    string pathfile_new = string.Format("{0}" + pathupload + "sot" + "\\srilanka\\action\\" + docno_new, Server.MapPath(@"\"));


                    if (Directory.Exists(pathfile))
                    {

                        System.IO.Directory.Move(pathfile, pathfile_new);

                    }

                    rc.doc_no = docno_new;


                    //count++;
                }

                dbConnect.SubmitChanges();


            }


        }



        [WebMethod]
        public void orderDocnoHazard()
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var doc_no_max_import = dbConnect.hazards.Where(x => x.country == "srilanka").Where(x => x.doc_no.Contains("H2017")).Where(x => x.device_type == "import").Max(t => t.doc_no);

                string[] last = doc_no_max_import.Split('-');
                string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathfile"];



                var i = from c in dbConnect.hazards
                        where c.country == "srilanka"
                        && c.device_type != "import"
                        orderby c.doc_no ascending
                        select c;


                //int count = 1;
                foreach (var rc in i)
                {
                    string[] docno_old = rc.doc_no.Split('-');
                    int number = Convert.ToInt32(last[1]) + Convert.ToInt32(docno_old[1]);
                    string docno_new = "H2017" + "-" + (number.ToString("D5"));

                    string pathfile = string.Format("{0}" + pathupload + "hazard" + "\\step3\\srilanka\\" + rc.doc_no, Server.MapPath(@"\"));
                    string pathfile_new = string.Format("{0}" + pathupload + "hazard" + "\\step3\\srilanka\\" + docno_new, Server.MapPath(@"\"));


                    if (Directory.Exists(pathfile))
                    {

                        System.IO.Directory.Move(pathfile, pathfile_new);

                    }

                    rc.doc_no = docno_new;


                    //count++;
                }

                dbConnect.SubmitChanges();


            }


        }


        public void deleteEmployeeSot(int id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_sots
                         where c.sot_id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_sots.DeleteOnSubmit(a);
                }
                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                    result = "false";
                }


            }
        }

        protected string generateDocnoSot(string year,string country, string timezone)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string docno = "";
                //string year = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                int number = 0;

                var doc_no = dbConnect.sots.Where(x => x.country == country).Where(x=>x.doc_no.Contains("SOT"+year)).Max(t => t.doc_no);

                if (doc_no != "" && doc_no != null)
                {
                    string[] last = doc_no.Split('-');
                    number = Convert.ToInt32(last[1]) + 1;



                    docno = "SOT" + year + "-" + (number.ToString("D5"));

                }
                else
                {
                    docno = "SOT" + year + "-" + "00001";

                }


                return docno;
            }
        }

        protected string generateDocnoHazard(string year,string country, string timezone)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string docno = "";
               // string year = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                int number = 0;

                var doc_no = dbConnect.hazards.Where(x => x.country == country).Where(x=>x.doc_no.Contains("H"+year)).Max(t => t.doc_no);

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
        public void deleteOtherImpact(string incident_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.other_impacts
                         where c.incident_id == Convert.ToInt32(incident_id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.other_impacts.DeleteOnSubmit(a);
                }
                try
                {
                    dbConnect.SubmitChanges();

                }
                catch (Exception e)
                {


                }
            }


        }

        protected int getFPEID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.fatality_prevention_elements
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }


        protected int getTypeControlID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.type_controls
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }

        protected int getCatergoryHazardID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.source_hazards
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }

        protected int getActionStatusID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.action_status
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }
        protected int getConsequenceLevelID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.level_incidents
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }


        protected int getEventExposureID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.event_exposures
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }


        protected int getSourceOfIncidentID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.source_incidents
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }


        protected int getSourceOfHazardID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.source_hazards
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }
        protected int getTypeEmployeeID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
              
                var v = from c in dbConnect.type_employments
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }

        protected int getNatureInjuryID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.nature_injuries
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }


        protected int getBodyPartID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.body_parts
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }


        protected int getSeverityInjuryID(string name)
        {
            int id = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.severity_injuries
                        where c.name_en == name
                        select new
                        {
                            c.id

                        };

                foreach (var rc in v)
                {
                    id = rc.id;

                }
            }

            return id;
        }

        protected string getEmployeeIDByName(string name)
        {
            string employee_id = "";

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                if (name!="")
                {
                    string[] words = name.Split(' ');

                    if (words.Count() == 2)
                    {
                        var v = from c in dbConnect.employees
                                where c.first_name_en.Contains(words[0]) && c.last_name_en.Contains(words[1])
                                select new
                                {
                                    c.employee_id

                                };

                        foreach (var rc in v)
                        {
                            employee_id = rc.employee_id;

                        }
                    }
                   
                }
                
            }

            return employee_id;
        }

        protected int getStatusForm(string name)
        {
            int status = 0;

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.incident_status
                        where c.name_en == name.ToUpper()
                        select new
                        {
                           c.id

                        };

                foreach (var rc in v)
                {
                    status = rc.id;

                }
            }

            return status;
        }


        protected string generateDocno(string year,string country, string timezone)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string docno = "";
                //string year = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                int number = 0;


                var doc_no = dbConnect.incidents.Where(x => x.country == country).Where(x=>x.doc_no.Contains("I"+year)).Max(x => x.doc_no);

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



        public string getMasterdataID(string name, string master_name,string function_id,string department_id,string division_id)
        {
            string id = "";

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                if (master_name == "company")
                {
                    var v = from c in dbConnect.companies
                            where c.company_en == name.ToUpper()
                            select new
                            {
                                c.company_id 

                            };

                    foreach (var rc in v)
                    {
                        id = rc.company_id;

                    }

                }
                else if (master_name == "function")
                {
                    var v = from c in dbConnect.functions
                            where c.function_en == name.ToUpper()
                            select new
                            {
                               c.function_id
                            };

                    foreach (var rc in v)
                    {
                        id = rc.function_id;
                    }

                }
                else if (master_name == "department")
                {
                    
                        var v = from c in dbConnect.departments
                                where c.department_en == name.ToUpper()
                                select new
                                {
                                    c.department_id

                                };

                    if (v.Count()!=0)
                    {
                        foreach (var rc in v)
                        {
                            id = rc.department_id;

                        }
                    }
                    else
                    {
                        id = function_id + "F";


                    }
                  
                }
                else if (master_name == "division")
                {
                   
                        var v = from c in dbConnect.divisions
                                where c.division_en == name.ToUpper()
                                select new
                                {
                                    c.division_id

                                };

                    if (v.Count()!=0)
                    {
                        foreach (var rc in v)
                        {
                            id = rc.division_id;

                        }
                    }
                      else
                      {
                          id = department_id + "D";


                      }

                }
                else if (master_name == "section")
                {

                    
                        var v = from c in dbConnect.sections
                                where c.section_en == name.ToUpper()
                                select new
                                {
                                    c.section_id

                                };

                    if (v.Count() != 0)
                    {
                        foreach (var rc in v)
                        {
                            id = rc.section_id;

                        }
                    }
                    else
                    {
                        id = division_id + "D";


                    }

                }

            }





            return id;
        }

        public void deleteEmployeeDepartment(string department_id)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.employee_has_departments
                         where c.department_id == department_id
                         select c;


                foreach (var a in gr)
                {
                    dbConnect.employee_has_departments.DeleteOnSubmit(a);
                }
                try
                {
                    dbConnect.SubmitChanges();

                }
                catch (Exception e)
                {


                }

            }

        }


        public void deleteEmployeeDivision(string division_id)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.employee_has_divisions
                         where c.division_id == division_id
                         select c;


                foreach (var a in gr)
                {
                    dbConnect.employee_has_divisions.DeleteOnSubmit(a);
                }
                try
                {
                    dbConnect.SubmitChanges();

                }
                catch (Exception e)
                {


                }

            }

        }


        public void deleteEmployeeSection(string section_id)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.employee_has_sections
                         where c.section_id == section_id
                         select c;


                foreach (var a in gr)
                {
                    dbConnect.employee_has_sections.DeleteOnSubmit(a);
                }
                try
                {
                    dbConnect.SubmitChanges();

                }
                catch (Exception e)
                {


                }

            }

        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getIncidentstatus(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.incident_status
                        //where c.country == Session["country"].ToString()
                        //orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));

            }
        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSotstatus(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.sot_status
                        where  c.status == "A" && c.id != 3 //reject
                        //orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));

            }
        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getHazardstatus(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.hazard_status
                        //where c.country == Session["country"].ToString()
                        //orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));

            }
        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getMonth(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.months
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.month_th, c.month_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getIncidentyear(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.incidents
                        where c.country == Session["country"].ToString()
                        group c by new { year = c.incident_date.Value.Year } into d
                        orderby d.Key.year ascending
                        select new
                        {
                            incident_year = d.Key.year
                        };

                ArrayList dataJson = new ArrayList();
                foreach (var r in v)
                {
                    int y = FormatDates.getYear(r.incident_year, lang);
                    var re = new
                    {
                        id = r.incident_year,
                        year = y

                    };
                    dataJson.Add(re);

                }

                if (dataJson.Count == 0)
                {
                    int y = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, lang);
                    var re = new
                    {
                        id = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en"),
                        year = y

                    };
                    dataJson.Add(re);
                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));

            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getRiskfactoryear(string lang)
        {
            ArrayList dataJson = new ArrayList();

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.year_health_check_ups
                        select new
                        {
                            year = c.year
                        };

                foreach (var r in v)
                {
                    int y = FormatDates.getYear(Convert.ToInt16(r.year), lang);
                    var re = new
                    {
                        id = r.year,
                        year = y

                    };
                    dataJson.Add(re);

                }




                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));

            }
        }




        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getHazardyear(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.hazards
                        where c.country == Session["country"].ToString()
                        group c by new { year = c.hazard_date.Value.Year } into d
                        orderby d.Key.year ascending
                        select new
                        {
                            hazard_year = d.Key.year
                        };

                ArrayList dataJson = new ArrayList();
                foreach (var r in v)
                {
                    int y = FormatDates.getYear(r.hazard_year, lang);
                    var re = new
                    {
                        id = r.hazard_year,
                        year = y

                    };
                    dataJson.Add(re);

                }

                if (dataJson.Count == 0)
                {
                    int y = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, lang);
                    var re = new
                    {
                        id = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en"),
                        year = y

                    };
                    dataJson.Add(re);
                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));
            }

        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getTargetYear(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                bool check_current_year = true;
                var v = from c in dbConnect.target_mains
                        group c by new { year = c.created.Value.Year } into d
                        orderby d.Key.year ascending
                        select new
                        {
                            target_year = d.Key.year
                        };

                ArrayList dataJson = new ArrayList();
                int current_year = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en");

                foreach (var r in v)
                {
                    int y = FormatDates.getYear(r.target_year, lang);
                    if (current_year == r.target_year)
                    {
                        check_current_year = false;
                    }
                    var re = new
                    {
                        id = r.target_year,
                        year = y

                    };
                    dataJson.Add(re);

                }

                if (check_current_year)
                {
                    int y = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, lang);
                    var re = new
                    {
                        id = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en"),
                        year = y

                    };
                    dataJson.Add(re);
                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getHealthYear(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                ArrayList dataJson = new ArrayList();

                //int y = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, lang);

               

                //int y_before_current = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, lang)-1;
                //var re2 = new
                //{
                //    id = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en")-1,
                //    year = y_before_current

                //};
                //dataJson.Add(re2);


                var v = from c in dbConnect.year_health_check_ups
                        select new
                        {
                            year = c.year
                        };

                foreach (var r in v)
                {
                    int y = FormatDates.getYear(Convert.ToInt16(r.year), lang);
                    var re = new
                    {
                        id = r.year,
                        year = y

                    };
                    dataJson.Add(re);

                }




                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));
            }

        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getHealthYearView(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                ArrayList dataJson = new ArrayList();

                var v = from c in dbConnect.healths
                        where c.country == Session["country"].ToString()
                        group c by new { year = c.year_health } into d
                        orderby d.Key.year ascending
                        select new
                        {
                            year = d.Key.year
                        };

                foreach (var r in v)
                {
                    int y = FormatDates.getYear(Convert.ToInt16(r.year), lang);
                    var re = new
                    {
                        id = r.year,
                        year = y

                    };
                    dataJson.Add(re);

                }




                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getTargetYearSrilanka(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                bool check_current_year = true;
                var v = from c in dbConnect.target_main_srilankas
                        group c by new { year = c.created.Value.Year } into d
                        orderby d.Key.year ascending
                        select new
                        {
                            target_year = d.Key.year
                        };

                ArrayList dataJson = new ArrayList();
                int current_year = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en");

                foreach (var r in v)
                {
                    int y = FormatDates.getYear(r.target_year, lang);
                    if (current_year == r.target_year)
                    {
                        check_current_year = false;
                    }
                    var re = new
                    {
                        id = r.target_year,
                        year = y

                    };
                    dataJson.Add(re);

                }

                if (check_current_year)
                {
                    int y = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, lang);
                    var re = new
                    {
                        id = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en"),
                        year = y

                    };
                    dataJson.Add(re);
                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));
            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getHolidayYear(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int y_current = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en");


                var v = from c in dbConnect.holidays
                        where c.holiday_date.Value.Year != y_current && c.country == Session["country"].ToString()
                        group c by new { year = c.holiday_date.Value.Year } into d
                        orderby d.Key.year ascending
                        select new
                        {
                            holiday_year = d.Key.year
                        };

                ArrayList dataJson = new ArrayList();
                foreach (var r in v)
                {
                    int y = FormatDates.getYear(r.holiday_year, lang);
                    var re = new
                    {
                        id = r.holiday_year,
                        year = y

                    };
                    dataJson.Add(re);

                }

                //if (dataJson.Count == 0)
                //{
                int y2 = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, lang);
                var re2 = new
                {
                    id = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en"),
                    year = y2

                };
                dataJson.Add(re2);
                // }




                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));
            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getWorkhourYear(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                bool check_current_year = true;
                var v = from c in dbConnect.workhour_mains
                        group c by new { year = c.created.Value.Year } into d
                        orderby d.Key.year ascending
                        select new
                        {
                            workhour_year = d.Key.year
                        };

                ArrayList dataJson = new ArrayList();
                int current_year = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en");

                foreach (var r in v)
                {
                    int y = FormatDates.getYear(r.workhour_year, lang);
                    if (current_year == r.workhour_year)
                    {
                        check_current_year = false;
                    }
                    var re = new
                    {
                        id = r.workhour_year,
                        year = y

                    };
                    dataJson.Add(re);

                }

                if (check_current_year)
                {
                    int y = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, lang);
                    var re = new
                    {
                        id = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en"),
                        year = y

                    };
                    dataJson.Add(re);
                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));
            }

        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getWorkhourYearSrilanka(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                bool check_current_year = true;
                var v = from c in dbConnect.workhour_main_srilankas
                        group c by new { year = c.created.Value.Year } into d
                        orderby d.Key.year ascending
                        select new
                        {
                            workhour_year = d.Key.year
                        };

                ArrayList dataJson = new ArrayList();
                int current_year = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en");

                foreach (var r in v)
                {
                    int y = FormatDates.getYear(r.workhour_year, lang);
                    if (current_year == r.workhour_year)
                    {
                        check_current_year = false;
                    }
                    var re = new
                    {
                        id = r.workhour_year,
                        year = y

                    };
                    dataJson.Add(re);

                }

                if (check_current_year)
                {
                    int y = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, lang);
                    var re = new
                    {
                        id = FormatDates.getYear(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year, "en"),
                        year = y

                    };
                    dataJson.Add(re);
                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dataJson));
            }

        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLevelDamage(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.level_property_damages
                        where c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLevelEnvironment(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.level_environment_impacts
                        where c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getLevelIncident(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.level_incidents
                        where c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }




        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getTypeEmployment(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.type_employments
                        where c.status == "A" && c.country == Session["country"].ToString()
                        orderby c.created_at ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)
                    
                        };

                  

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getNatureInjury(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.nature_injuries
                        where c.status == "A" && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getBodyParts(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.body_parts
                        where c.status == "A" && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSeverityInjury(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.severity_injuries
                        where c.status == "A" && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getCurrency(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.currencies
                        where c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSourceIncident(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.source_incidents
                        where c.status == "A" && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEventExposure(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.event_exposures
                        where c.status == "A" && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getFatalityPrevention(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.fatality_prevention_elements
                        where c.status == "A" && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSourceHazard(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.source_hazards
                        where c.status == "A" && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getRootCauseActionByIncidentID(string id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.root_cause_actions
                        where c.status == "A" && c.incident_id == Convert.ToInt32(id)
                        select new
                        {
                            id = c.id,
                            name = c.name,


                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }
        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getTypecontrol(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.type_controls
                        where c.country == Session["country"].ToString()
                        & c.status == "A"
                        orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDurationRiskFactor(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.duration_risk_factors
                        where  c.status == "A"
                        orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getTypecontrolHealth(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.type_control_healths
                        where c.country == Session["country"].ToString()
                        & c.status == "A"
                        orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getActionHealthStatus(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.action_health_status
                        orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getRiskFactorRelateWork(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.risk_factor_relate_works
                        where c.country == Session["country"].ToString()
                        & c.status == "A"
                        orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getOccupationalHealthReport(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.occupational_health_reports
                        where c.country == Session["country"].ToString()
                        & c.status == "A"
                        orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getHazardCharacteristic(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.hazard_characteristics
                        where c.country == Session["country"].ToString()
                        orderby c.id ascending
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }
        public string chageDataLanguage(string vTH, string vEN, string lang)
        {
            string vReturn = "";

            if (lang == "th")
            {

                vReturn = vTH;

            }
            else if (lang == "en")
            {

                vReturn = vEN;
            } 
            else if (lang == "si")
            {

                vReturn = vEN;

            }


            return vReturn;
        }



        public string chageDataLanguageForEmpty(string vTH, string vEN, string lang)
        {
            string vReturn = "";

            if (lang == "th")
            {
                if (string.IsNullOrEmpty(vTH))
                {
                    vReturn = "-";
                }
                else
                {
                    vReturn = vTH;
                }

                

            }
            else if (lang == "en")
            {

                if (string.IsNullOrEmpty(vEN))
                {
                    vReturn = "-";
                }
                else
                {
                    vReturn = vEN;
                }

            }
            else if (lang == "si")
            {
                if (string.IsNullOrEmpty(vEN))
                {
                    vReturn = "-";
                }
                else
                {
                    vReturn = vEN;
                }


            }


            return vReturn;
        }


        public string checkAreaForEmpty(string v)
        {
            string vReturn = "";

            if (string.IsNullOrEmpty(v))
            {
                vReturn = "-";
            }
            else
            {
                vReturn = v;
            }


            return vReturn;
        }




        [WebMethod]
        public void testnotification()
        {

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
           var v = from c in dbConnect.hazards
                        where c.id == 13026
                        select c;

            bool result = false;
            foreach(var rc in v)
            {
                result = addday(Convert.ToDateTime(rc.confirm_form_three_to_four_at),3,"+7");

            }

           

            HttpContext.Current.Response.Write(result);

        }



        public bool addday(DateTime dt, int day, string timezone)
        {
            bool result = false;
            int count = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();


            DateTime dtnew = dt;


            TimeSpan span = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Subtract(dtnew);
            int cday = Convert.ToInt16(span.TotalDays);

            //while(dtnew <= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)))
            //{

            //    dtnew = dtnew.AddDays(1);

            //    if (dtnew.DayOfWeek.ToString() != "Saturday" && dtnew.DayOfWeek.ToString() != "Sunday")
            //    {
            //        var holidays = from c in dbConnect.holidays
            //                       where c.holiday_date == dtnew.Date
            //                       select new
            //                       {
            //                           holiday_date = c.holiday_date,

            //                       };

            //        bool is_holiday = false;
            //        foreach (var v in holidays)
            //        {
            //            is_holiday = true;
            //        }

            //        if (!is_holiday)
            //        {
            //            count++;

            //        }

            //    }//end if


            //}//end for



            //if (count > day)
            //{
            //    result = true;
            //}


            return result;
        }

    }
}
