using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using safetys4.App_Code;
using safetys4.Class;
using System.Globalization;

namespace safetys4
{
    /// <summary>
    /// Summary description for Datatablelist
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Datatablelist : System.Web.Services.WebService
    {

        [WebMethod(EnableSession=true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
       
        public void getListcontractor(string functionid, string status,string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                string word_search = Context.Request["search[value]"].ToString();

                var v = from c in dbConnect.contractors
                        //  where c.status == status.Trim()
                        join cs in dbConnect.status_contractors on c.status equals cs.value into joinCS
                        from cs in joinCS.DefaultIfEmpty()
                        where cs.lang == lang.Trim() && c.country == Session["country"].ToString()
                        orderby c.updated_at ascending
                        select new
                        {
                            id = c.id,
                            fucntion_id = c.function_id,
                            //company_name = chageDataLanguage(c.company_th,c.company_en,lang), 
                            company_name = c.company,
                            first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                            last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),
                            prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                            c.first_name_en,
                            c.last_name_en,
                            c.first_name_th,
                            c.last_name_th,
                            phone = c.phone,
                            email = c.email,
                            status = cs.name


                        };

                if (functionid != "")
                {
                    v = v.Where(c => c.fucntion_id == functionid);

                }

                if (status != "")
                {
                    v = v.Where(c => c.status == status.Trim());

                }


                if (word_search != "")
                {

                    v = v.Where(c => c.company_name.Contains(word_search) || c.first_name_th.Contains(word_search) || c.first_name_th.Contains(word_search) || c.first_name_en.Contains(word_search)
                        || c.first_name_en.Contains(word_search) || c.email.Contains(word_search));


                }


                int totalRow = v.Count();
                int lenght = Convert.ToInt32(Context.Request["length"].ToString());
                int start = Convert.ToInt32(Context.Request["start"].ToString());
                int draw = Convert.ToInt32(Context.Request["draw"].ToString());

                v = v.Skip(start).Take(lenght);

                ArrayList dataJson = new ArrayList();

                int no = 1;
                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    ArrayList dt = new ArrayList();
                    dt.Add(no);
                    dt.Add(rc.id);
                    dt.Add(rc.company_name);
                    dt.Add(name);
                    dt.Add(rc.phone);
                    dt.Add(rc.email);
                    dt.Add(rc.status);
                    dataJson.Add(dt);
                    no++;

                }

                var result = new
                {
                    draw = draw,
                    recordsTotal = totalRow,
                    recordsFiltered = totalRow,
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));

            }

            //var json = "{'data':"+(JsonConvert.SerializeObject(dataJson))+'}';
            //return json;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
      
        public void getSuperadmin(string lang,string country)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_super_admin = 2;//check on group table
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        where e.status == "Active" && c.group_id == group_super_admin
                         && c.country == country
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };
                // '<li class="list-group-item">First item<span class="pull-right" style="font-size:18px;"><a href="javascript:DeleteSuperAdmin('1');"><i class="fa fa-trash"></i></a></span></li>'

                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;

                    if (Session["permission"] != null)
                    {
                        string delete = "";
                        ArrayList per = Session["permission"] as ArrayList;
                        if (per.IndexOf("super admin delete") > -1)
                        {
                            delete = "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteSuperAdmin(" + rc.id + ");'><i class='fa fa-trash'></i></a></span>";
                        }
                        list += "<li class='list-group-item'>" + name + delete + "</li>";

                    }

                }


                Context.Response.Output.Write(list);
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        
        public void getDelegateSuperadmin(string lang,string country)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_delegate_super_admin = 3;//check on group table
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        where e.status == "Active" && c.group_id == group_delegate_super_admin
                         && c.country == country
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };
                // '<li class="list-group-item">First item<span class="pull-right" style="font-size:18px;"><a href="javascript:DeleteSuperAdmin('1');"><i class="fa fa-trash"></i></a></span></li>'

                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    if (Session["permission"] != null)
                    {
                        string delete = "";
                        ArrayList per = Session["permission"] as ArrayList;
                        if (per.IndexOf("delegate super admin delete") > -1)
                        {
                            delete = "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteDelegateSuperAdmin(" + rc.id + ");'><i class='fa fa-trash'></i></a></span>";
                        }
                        list += "<li class='list-group-item'>" + name + delete + "</li>";

                    }

                }


                Context.Response.Output.Write(list);
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getOHS(string function_id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int ohs_admin = 4;//check on group table
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        join o in dbConnect.functions on c.function_id equals o.function_id
                        where e.status == "Active" && c.group_id == ohs_admin && c.function_id == function_id.Trim()
                         && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };

                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    string delete = "";
                    ArrayList per = Session["permission"] as ArrayList;
                    if (per.IndexOf("area management oh&s admin delete") > -1)
                    {
                        delete = "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span>";
                    }

                    list += "<li class='list-group-item'>" + name + delete + "</li>";

                }


                Context.Response.Output.Write(list);
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getDelegateOHS(string function_id,string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int delegate_ohs = 5;//check on group table
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        join o in dbConnect.functions on c.function_id equals o.function_id
                        where e.status == "Active" && c.group_id == delegate_ohs && c.function_id == function_id
                         && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };

                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    string delete = "";
                    ArrayList per = Session["permission"] as ArrayList;
                    if (per.IndexOf("area management delegate oh&s admin delete") > -1)
                    {
                        delete = "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteDelegateOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span>";
                    }
                    list += "<li class='list-group-item'>" + name + delete + "</li>";

                }


                Context.Response.Output.Write(list);
            }
        }



        [WebMethod(EnableSession =true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListemployee(string country)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string ceo_office_function = "52010001";//function_id
                string compliance_group_oh = "53000146";//function_id


                var v = from e in dbConnect.employees
                        join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinOrg
                        from o in joinOrg.DefaultIfEmpty()
                        where e.status == "Active"
                        && e.country == country
                        select new
                        {
                            o.function_id,
                            o.sub_function_id,
                            employee_id = e.employee_id,
                            first_name_th = e.first_name_th,
                            first_name_en = e.first_name_en,
                            last_name_th = e.last_name_th,
                            last_name_en = e.last_name_en,
                            prefix_th = e.prefix_th,
                            prefix_en = e.prefix_en,


                        };


                if (Session["country"].ToString() == "thailand")
                {

                    v = v.Where(c => c.function_id == ceo_office_function);
                    v = v.Where(c => c.sub_function_id == compliance_group_oh);               

                }

                


                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    string name_th = rc.prefix_th + " " + rc.first_name_th + " " + rc.last_name_th;
                    string name_en = rc.prefix_en + " " + rc.first_name_en + " " + rc.last_name_en;
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.employee_id);
                    dt.Add(name_th);
                    dt.Add(name_en);

                    dataJson.Add(dt);


                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }




        [WebMethod(EnableSession=true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListAreaManagement(string function_id,string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_area_ohs = 1;//check on group area table
                int group_area_manager = 2;//check on group area table
                int group_area_supervisor = 3;

               // string word_search = Context.Request["search[value]"].ToString();

                var v = from d in dbConnect.departments
                        join di in dbConnect.divisions on d.department_id equals di.department_id //into joinDi
                        //from di in joinDi.DefaultIfEmpty()
                        join s in dbConnect.sections on di.division_id equals s.division_id // into joinS
                        //from s in joinS.DefaultIfEmpty()
                        where d.function_id == function_id.Trim() && d.country == Session["country"].ToString()
                        orderby d.department_id, di.division_id, s.section_id ascending

                        select new
                        {
                            d.department_id,
                            di.division_id,
                            s.section_id,
                            d.department_th,
                            d.department_en,
                            di.division_th,
                            di.division_en,
                            s.section_th,
                            s.section_en,
                            //department = chageDataLanguageArea(d.department_th, d.department_en, lang),
                            //division = chageDataLanguageArea(di.division_th, di.division_en, lang),
                            //section = chageDataLanguageArea(s.section_th, s.section_en, lang),
                            functional_manager = getEmployeeInDepartmentFunctionalManager(d.department_id, lang),
                            area_ohs = getEmployeeInDepartment(d.department_id, lang),
                            area_manager = getEmployeeInDivision(di.division_id, lang),
                            area_supervisor = getEmployeeInSection(s.section_id, lang),
                            department_valid_from = d.valid_from,
                            department_valid_to = d.valid_to,
                            division_valid_from = di.valid_from,
                            division_valid_to = di.valid_to,
                            section_valid_from = s.valid_from,
                            section_valid_to = s.valid_to,


                        };

                //if (word_search != "")
                //{

                //    v = v.Where(c => c.department_th.Contains(word_search) || c.department_en.Contains(word_search) ||
                //        c.division_th.Contains(word_search) || c.division_en.Contains(word_search) ||
                //        c.section_th.Contains(word_search) || c.section_en.Contains(word_search));

                //}


                //int totalRow = v.Count();
                //int lenght = Convert.ToInt32(Context.Request["length"].ToString());
                //int start = Convert.ToInt32(Context.Request["start"].ToString());
                //int draw = Convert.ToInt32(Context.Request["draw"].ToString());

                //v = v.Skip(start).Take(lenght);

                // v = v.OrderBy(c => c.department).ThenBy(x => x.division).ThenBy(d => d.section);

                ArrayList dataJson = new ArrayList();


                foreach (var rc in v)
                {
                    bool check_invalid = false;

                    if (rc.department_valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.department_valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        check_invalid = true;
                    }

                    if (rc.division_valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.division_valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        check_invalid = true;

                    }

                    if (rc.section_valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.section_valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        check_invalid = true;

                    }

                    if (check_invalid == false)
                    {
                        if (rc.area_ohs != "" && rc.area_manager != "" && rc.area_supervisor != "")
                        {
                            if (Session["country"].ToString() == "thailand")
                            {

                                if (rc.functional_manager != "")//ต้อง check column นี้เพิ่มด้วย
                                {
                                    ArrayList dt = new ArrayList();
                                    dt.Add(rc.department_id);
                                    dt.Add(rc.division_id);
                                    dt.Add(rc.section_id);
                                    string department = chageDataLanguageArea(rc.department_th, rc.department_en, lang);
                                    string division = chageDataLanguageArea(rc.division_th, rc.division_en, lang);
                                    string section = chageDataLanguageArea(rc.section_th, rc.section_en, lang);
                                    dt.Add(department + rc.functional_manager);

                                    dt.Add(department + rc.area_ohs);
                                    dt.Add(division + rc.area_manager);
                                    dt.Add(section + rc.area_supervisor);

                                    dataJson.Add(dt);
                                }
                                


                            }
                            else
                            {


                                ArrayList dt = new ArrayList();
                                dt.Add(rc.department_id);
                                dt.Add(rc.division_id);
                                dt.Add(rc.section_id);
                                string department = chageDataLanguageArea(rc.department_th, rc.department_en, lang);
                                string division = chageDataLanguageArea(rc.division_th, rc.division_en, lang);
                                string section = chageDataLanguageArea(rc.section_th, rc.section_en, lang);
                
                                dt.Add(department + rc.area_ohs);
                                dt.Add(division + rc.area_manager);
                                dt.Add(section + rc.area_supervisor);

                                dataJson.Add(dt);

                            }



                          


                        }
                    }


                }

                var result = new
                {

                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));


            }

        }



        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListAreaManagement2(string function_id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
              

                var v = from d in dbConnect.departments
                        join di in dbConnect.divisions on d.department_id equals di.department_id //into joinDi
                        //from di in joinDi.DefaultIfEmpty()
                        join s in dbConnect.sections on di.division_id equals s.division_id // into joinS
                        //from s in joinS.DefaultIfEmpty()
                        where d.function_id == function_id.Trim() && d.country == Session["country"].ToString()
                        orderby d.department_id, di.division_id, s.section_id ascending

                        select new
                        {
                            d.department_id,
                            di.division_id,
                            s.section_id,
                            d.department_th,
                            d.department_en,
                            di.division_th,
                            di.division_en,
                            s.section_th,
                            s.section_en,
                            department_valid_from = d.valid_from,
                            department_valid_to = d.valid_to,
                            division_valid_from = di.valid_from,
                            division_valid_to = di.valid_to,
                            section_valid_from = s.valid_from,
                            section_valid_to = s.valid_to,

                            functional_manager = getEmployeeInDepartmentFunctionalManager(d.department_id, lang),
                            area_ohs = getEmployeeInDepartment(d.department_id, lang),
                            area_manager = getEmployeeInDivision(di.division_id, lang),
                            area_supervisor = getEmployeeInSection(s.section_id, lang),


                        };

             

                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    bool check_invalid = false;
                    bool check_department_invalid = false;
                    bool check_division_invalid = false;
                    bool check_section_invalid = false;


                    if (rc.department_valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.department_valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        check_invalid = true;
                        check_department_invalid = true;
                    }

                    if (rc.division_valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.division_valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        check_invalid = true;
                        check_division_invalid = true;

                    }

                    if (rc.section_valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.section_valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        check_invalid = true;
                        check_section_invalid = true;

                    }




                    if (check_invalid)
                    {
                        if (Session["country"].ToString() == "thailand")
                        {

                            if ((rc.area_ohs != "" && check_department_invalid) ||
                                (rc.functional_manager != "" && check_department_invalid) ||
                                (rc.area_manager != "" && check_division_invalid) ||
                                (rc.area_supervisor != "" && check_section_invalid))
                            {

                                dt.Add(rc.department_id);
                                dt.Add(rc.division_id);
                                dt.Add(rc.section_id);
                                string department = chageDataLanguageArea(rc.department_th, rc.department_en, lang);
                                string division = chageDataLanguageArea(rc.division_th, rc.division_en, lang);
                                string section = chageDataLanguageArea(rc.section_th, rc.section_en, lang);




                                if (check_department_invalid)
                                {
                                    dt.Add(department + rc.functional_manager);
                                }
                                else
                                {
                                    dt.Add(department);
                                }


                                if (check_department_invalid)
                                {
                                    dt.Add(department + rc.area_ohs);
                                }
                                else
                                {
                                    dt.Add(department);
                                }


                                if (check_division_invalid)
                                {
                                    dt.Add(division + rc.area_manager);
                                }
                                else
                                {
                                    dt.Add(division);
                                }


                                if (check_section_invalid)
                                {
                                    dt.Add(section + rc.area_supervisor);
                                }
                                else
                                {
                                    dt.Add(section);
                                }


                                dataJson.Add(dt);

                            }

                        }
                        else
                        {

                            if ((rc.area_ohs != "" && check_department_invalid) ||
                                (rc.area_manager != "" && check_division_invalid) ||
                                (rc.area_supervisor != "" && check_section_invalid))
                            {

                                dt.Add(rc.department_id);
                                dt.Add(rc.division_id);
                                dt.Add(rc.section_id);
                                string department = chageDataLanguageArea(rc.department_th, rc.department_en, lang);
                                string division = chageDataLanguageArea(rc.division_th, rc.division_en, lang);
                                string section = chageDataLanguageArea(rc.section_th, rc.section_en, lang);



                                if (check_department_invalid)
                                {
                                    dt.Add(department + rc.area_ohs);
                                }
                                else
                                {
                                    dt.Add(department);
                                }


                                if (check_division_invalid)
                                {
                                    dt.Add(division + rc.area_manager);
                                }
                                else
                                {
                                    dt.Add(division);
                                }


                                if (check_section_invalid)
                                {
                                    dt.Add(section + rc.area_supervisor);
                                }
                                else
                                {
                                    dt.Add(section);
                                }


                                dataJson.Add(dt);


                            }


                        }//check country


                    }




                }

                var result = new
                {
                
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }




        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListAreaManagement3(string function_id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
           

                var v = from d in dbConnect.departments
                        join di in dbConnect.divisions on d.department_id equals di.department_id //into joinDi
                        //from di in joinDi.DefaultIfEmpty()
                        join s in dbConnect.sections on di.division_id equals s.division_id // into joinS
                        //from s in joinS.DefaultIfEmpty()
                        where d.function_id == function_id.Trim() && d.country == Session["country"].ToString()
                      
                        orderby d.department_id, di.division_id, s.section_id ascending

                        select new
                        {
                            d.department_id,
                            di.division_id,
                            s.section_id,
                            d.department_th,
                            d.department_en,
                            di.division_th,
                            di.division_en,
                            s.section_th,
                            s.section_en,
                            functional_manager = getEmployeeInDepartmentFunctionalManager(d.department_id, lang),
                            area_ohs = getEmployeeInDepartment(d.department_id, lang),
                            area_manager = getEmployeeInDivision(di.division_id, lang),
                            area_supervisor = getEmployeeInSection(s.section_id, lang),
                            department_valid_from = d.valid_from,
                            department_valid_to = d.valid_to,
                            division_valid_from = di.valid_from,
                            division_valid_to = di.valid_to,
                            section_valid_from = s.valid_from,
                            section_valid_to = s.valid_to,


                        };



                ArrayList dataJson = new ArrayList();
              
                foreach (var rc in v)
                {
                    bool check_invalid = false;
                  
                    if (rc.department_valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.department_valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        check_invalid = true;
                    }

                    if (rc.division_valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.division_valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        check_invalid = true;

                    }

                    if (rc.section_valid_from < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()))
                        && rc.section_valid_to < DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())))
                    {
                        check_invalid = true;

                    }

                    if (check_invalid == false)
                    {
                        if (Session["country"].ToString() == "thailand")
                        {
                            if (rc.functional_manager == "" || rc.area_ohs == "" || rc.area_manager == "" || rc.area_supervisor == "")
                            {
                                ArrayList dt = new ArrayList();
                                dt.Add(rc.department_id);
                                dt.Add(rc.division_id);
                                dt.Add(rc.section_id);
                                string department = chageDataLanguageArea(rc.department_th, rc.department_en, lang);
                                string division = chageDataLanguageArea(rc.division_th, rc.division_en, lang);
                                string section = chageDataLanguageArea(rc.section_th, rc.section_en, lang);
                         
                                dt.Add(department + rc.functional_manager);

                                dt.Add(department + rc.area_ohs);
                                dt.Add(division + rc.area_manager);
                                dt.Add(section + rc.area_supervisor);

                                dataJson.Add(dt);


                            }


                        }
                        else
                        {

                            if (rc.area_ohs == "" || rc.area_manager == "" || rc.area_supervisor == "")
                            {
                                ArrayList dt = new ArrayList();
                                dt.Add(rc.department_id);
                                dt.Add(rc.division_id);
                                dt.Add(rc.section_id);
                                string department = chageDataLanguageArea(rc.department_th, rc.department_en, lang);
                                string division = chageDataLanguageArea(rc.division_th, rc.division_en, lang);
                                string section = chageDataLanguageArea(rc.section_th, rc.section_en, lang);

                                dt.Add(department + rc.area_ohs);
                                dt.Add(division + rc.area_manager);
                                dt.Add(section + rc.area_supervisor);

                                dataJson.Add(dt);


                            }


                        }




                     
                    }
                   

                }

                var result = new
                {

                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }

        public string getEmployeeInDepartment(string department_id,string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";

                if (!String.IsNullOrWhiteSpace(department_id))
                {


                    var gr = from d in dbConnect.employee_has_departments
                             join e in dbConnect.employees on d.employee_id equals e.employee_id // into joinE
                             // from e in joinE.DefaultIfEmpty()
                             where d.department_id == department_id && d.country == Session["country"].ToString()

                             select new
                             {

                                 first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                                 last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                                 prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang)


                             };


                    int count = 1;
                    result = "<div class='name_area'>";
                    foreach (var rc in gr)
                    {
                        if (count != 1)
                        {
                            result += "</br>" + count + ". " + rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        }
                        else
                        {

                            result += count + ". " + rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        }

                        count++;
                    }
                    result += "</div>";

                    if (gr.Count() == 0)
                    {
                        result = "";
                    }

                }



                return result;
            }

        }



        public string getEmployeeInDepartmentFunctionalManager(string department_id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";

                if (!String.IsNullOrWhiteSpace(department_id))
                {


                    var gr = from d in dbConnect.employee_has_department_functional_managers
                             join e in dbConnect.employees on d.employee_id equals e.employee_id // into joinE
                             // from e in joinE.DefaultIfEmpty()
                             where d.department_id == department_id && d.country == Session["country"].ToString()

                             select new
                             {

                                 first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                                 last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                                 prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang)


                             };


                    int count = 1;
                    result = "<div class='name_area'>";
                    foreach (var rc in gr)
                    {
                        if (count != 1)
                        {
                            result += "</br>" + count + ". " + rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        }
                        else
                        {

                            result += count + ". " + rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        }

                        count++;
                    }
                    result += "</div>";

                    if (gr.Count() == 0)
                    {
                        result = "";
                    }

                }



                return result;
            }

        }


        public string getEmployeeInDivision(string division_id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";

                if (!String.IsNullOrWhiteSpace(division_id))
                {


                    var gr = from d in dbConnect.employee_has_divisions
                             join e in dbConnect.employees on d.employee_id equals e.employee_id // into joinE
                             // from e in joinE.DefaultIfEmpty()
                             where d.division_id == division_id && d.country == Session["country"].ToString()

                             select new
                             {

                                 first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                                 last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                                 prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang)


                             };


                    int count = 1;
                    result = "<div class='name_area'>";
                    foreach (var rc in gr)
                    {
                        if (count != 1)
                        {
                            result += "</br>" + count + ". " + rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        }
                        else
                        {

                            result += count + ". " + rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        }

                        count++;
                    }
                    result += "</div>";

                    if (gr.Count() == 0)
                    {
                        result = "";
                    }

                }



                return result;
            }
        }


        public string getEmployeeInSection(string section_id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";

                if (!String.IsNullOrWhiteSpace(section_id))
                {


                    var gr = from d in dbConnect.employee_has_sections
                             join e in dbConnect.employees on d.employee_id equals e.employee_id // into joinE
                             // from e in joinE.DefaultIfEmpty()
                             where d.section_id == section_id && d.country == Session["country"].ToString()

                             select new
                             {

                                 first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                                 last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                                 prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang)


                             };


                    int count = 1;
                    result = "<div class='name_area'>";
                    foreach (var rc in gr)
                    {
                        if (count != 1)
                        {
                            result += "</br>" + count + ". " + rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        }
                        else
                        {

                            result += count + ". " + rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        }

                        count++;
                    }
                    result += "</div>";

                    if (gr.Count() == 0)
                    {
                        result = "";
                    }

                }



                return result;
            }

        }


        [WebMethod(EnableSession= true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListemployeeByfunction(string function_id,string type)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                List<string> lswithdrawn = new List<string>();
       

                var de = from d in dbConnect.employee_has_departments
                         join e in dbConnect.employees on d.employee_id equals e.employee_id
                         where d.country == Session["country"].ToString() && e.status == "Withdrawn"
                         select new
                        {
                            d.employee_id
                        };

                 foreach (var rc in de)
                 {
                     lswithdrawn.Add(rc.employee_id);

                 }


                var di = from d in dbConnect.employee_has_divisions
                         join e in dbConnect.employees on d.employee_id equals e.employee_id
                         where d.country == Session["country"].ToString() && e.status == "Withdrawn"
                         select new
                         {
                             d.employee_id
                         };

                foreach (var rc in di)
                {
                    lswithdrawn.Add(rc.employee_id);

                }


                var se = from d in dbConnect.employee_has_sections
                         join e in dbConnect.employees on d.employee_id equals e.employee_id
                         where d.country == Session["country"].ToString() && e.status == "Withdrawn"
                         select new
                         {
                             d.employee_id
                         };

                foreach (var rc in se)
                {
                    lswithdrawn.Add(rc.employee_id);

                }


                var g = from d in dbConnect.employee_has_groups
                         join e in dbConnect.employees on d.employee_id equals e.employee_id
                         where d.country == Session["country"].ToString() && e.status == "Withdrawn"
                         select new
                         {
                             d.employee_id
                         };

                foreach (var rc in g)
                {
                    lswithdrawn.Add(rc.employee_id);

                }


                List<string> lslevel = new List<string>();
                lslevel.Add("SML");
                lslevel.Add("TML");
                lslevel.Add("TML-EXCO");

                var v = from e in dbConnect.employees
                       // join o in dbConnect.organizations on e.unit_id equals o.org_unit_id// into joinOrg
                        //from o in joinOrg.DefaultIfEmpty()
                        where (e.status == "Active" || lswithdrawn.Contains(e.employee_id))
                         && e.country == Session["country"].ToString()
                        select new
                        {
                            employee_id = e.employee_id,
                            first_name_th = e.first_name_th,
                            first_name_en = e.first_name_en,
                            last_name_th = e.last_name_th,
                            last_name_en = e.last_name_en,
                            prefix_th = e.prefix_th,
                            prefix_en = e.prefix_en,
                            e.mngt_level


                        };

                if (type == "functional")
                {
                     v = v.Where(c => lslevel.Contains(c.mngt_level));

                }


                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    string name_th = rc.prefix_th + " " + rc.first_name_th + " " + rc.last_name_th;
                    string name_en = rc.prefix_en + " " + rc.first_name_en + " " + rc.last_name_en;
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.employee_id);
                    dt.Add(name_th);
                    dt.Add(name_en);

                    dataJson.Add(dt);


                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getArea(string company_id,string function_id,string department_id,string division_id,string section_id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.area_managements
                        where c.status == "A" && c.company_id == company_id.Trim() && c.function_id == function_id.Trim() && c.department_id == department_id.Trim()
                        && c.division_id == division_id.Trim() && c.section_id == section_id.Trim()
                         && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            name = chageDataLanguage(c.name_th, c.name_en, lang),


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
                if (vTH != null)
                {
                    vReturn = vTH;
                }


            }
            else if (lang == "en")
            {

                vReturn = vEN;

            }else if(lang == "si")
            {

                 vReturn = vEN;
           

            }

            
            return vReturn;
        }



        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListAllIncident(string company_id,
                                       string function_id,
                                       string department_id,
                                       string division_id,
                                       string section_id,
                                       string status,
                                       string date_start,
                                       string date_end,
                                       string employee_id,
                                       string type,
                                       string form,
                                       string day,
                                       string type_area,
                                       string lang)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                if (type == "filter")
                {
                    Session["company_id_selected_incident"] = company_id == null ? "" : company_id;
                    Session["function_id_selected_incident"] = function_id == null ? "" : function_id;
                    Session["department_id_selected_incident"] = department_id == null ? "" : department_id;
                    Session["division_id_selected_incident"] = division_id == null ? "" : division_id;
                    Session["section_id_selected_incident"] = section_id == null ? "" : section_id;
                    Session["status_selected_incident"] = status == null ? "" : status;
                    Session["date_start_selected_incident"] = date_start == null ? "" : date_start;
                    Session["date_end_selected_incident"] = date_end == null ? "" : date_end;


                }

                string word_search = Context.Request["search[value]"].ToString();

               
                var v = from d in dbConnect.incidents
                        join s in dbConnect.incident_status on d.process_status equals s.id
                        let c = dbConnect.corrective_prevention_action_incidents.Where(p2 => d.id == p2.incident_id).FirstOrDefault()
                        let f = dbConnect.fact_findings.Where(p2 => d.id == p2.incident_id).FirstOrDefault()
                        where d.country == Session["country"].ToString()
                        orderby d.report_date descending

                        select new
                        {
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
                            d.employee_id,
                            d.incident_area,
                            d.report_date,
                            d.confirm_form_two_to_three_at,
                            d.action_form_three_at,
                            incident_id = c == null ? null : c.incident_id.ToString(),
                            incident_id2 = f == null ? null : f.incident_id.ToString(),
                            d.owner_activity

                        };

                if (form != "")
                {

                    if (form == "1to2")
                    {

                        v = v.Where(c => c.process_status == 1 && c.step_form == 1);
                        List<int> ls1 = new List<int>();
                        List<int> ls2 = new List<int>();
                        List<int> ls3 = new List<int>();
                        List<int> ls4 = new List<int>();

                        foreach (var rc in v)
                        {
                            DateTime dtnew = rc.report_date;
                            DateTime current = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                            TimeSpan span = current.Subtract(dtnew);
                            int cday = Convert.ToInt16(span.TotalDays);
                            int count_day = 0;


                             if (cday < 30)
                             {
                                    while (dtnew <= current)
                                    {
                                            dtnew = dtnew.AddDays(1);

                                            if (dtnew.DayOfWeek.ToString() != "Saturday" && dtnew.DayOfWeek.ToString() != "Sunday")
                                            {
                                                var holidays = from c in dbConnect.holidays
                                                               where c.holiday_date == dtnew.Date
                                                               select new
                                                               {
                                                                   holiday_date = c.holiday_date,

                                                               };

                                                bool is_holiday = false;
                                                foreach (var r in holidays)
                                                {
                                                    is_holiday = true;
                                                }

                                                if (!is_holiday)
                                                {
                                                    count_day++;

                                                }

                                            }//end if

                                    }//end while


                                    ////////////////////////////////////////////////////////////////////////////////////////////
                                    if (count_day == 1)
                                    {
                                        ls1.Add(rc.id);
                                    }
                                    else if (count_day == 2)
                                    {
                                        ls2.Add(rc.id);

                                    }
                                    else if (count_day == 3)
                                    {

                                        ls3.Add(rc.id);

                                    }
                                    else if (count_day > 3)
                                    {
                                        ls4.Add(rc.id);
                                       
                                    }


                                }
                                else
                                {
                                    ls4.Add(rc.id);
                                }


                        }//end foreach


                        if (day == "1")
                        {
                              v = v.Where(c => ls1.Contains(c.id));

                        }
                        else if (day == "2")
                        {

                            v = v.Where(c => ls2.Contains(c.id));
                        }
                        else if (day == "3")
                        {
                            v = v.Where(c => ls3.Contains(c.id));
                        }
                        else if (day == "4")//>3
                        {
                            v = v.Where(c => ls4.Contains(c.id));  

                        }


                    }
                    else if (form == "2to3")
                    {
                         v = v.Where(c => c.incident_id == null);
                         v = v.Where(c => c.incident_id2 == null);

                         v = v.Where(c => c.process_status == 1 && c.step_form == 3
                                && c.confirm_form_two_to_three_at != null
                                && c.action_form_three_at == null);




                         List<int> ls5 = new List<int>();
                         List<int> ls8 = new List<int>();
                         List<int> ls11 = new List<int>();
                         List<int> ls14 = new List<int>();
                         List<int> ls16 = new List<int>();

                         foreach (var rc in v)
                         {
                             DateTime dtnew = rc.confirm_form_two_to_three_at.Value;
                             DateTime current = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                             TimeSpan span = current.Subtract(dtnew);
                             int cday = Convert.ToInt16(span.TotalDays);
                             int count_day = 0;


                             if (cday < 30)
                             {
                                 while (dtnew <= current)
                                 {
                                     dtnew = dtnew.AddDays(1);

                                     if (dtnew.DayOfWeek.ToString() != "Saturday" && dtnew.DayOfWeek.ToString() != "Sunday")
                                     {
                                         var holidays = from c in dbConnect.holidays
                                                        where c.holiday_date == dtnew.Date
                                                        select new
                                                        {
                                                            holiday_date = c.holiday_date,

                                                        };

                                         bool is_holiday = false;
                                         foreach (var r in holidays)
                                         {
                                             is_holiday = true;
                                         }

                                         if (!is_holiday)
                                         {
                                             count_day++;

                                         }

                                     }//end if

                                 }//end while


                                 ////////////////////////////////////////////////////////////////////////////////////////////
                                 if (count_day >= 5 & count_day <= 7)
                                 {
                                     ls5.Add(rc.id);
                                 }
                                 else if (count_day >= 8 & count_day <= 10)
                                 {
                                     ls8.Add(rc.id);
                                 }
                                 else if (count_day >= 11 & count_day <= 13)
                                 {
                                     ls11.Add(rc.id);
                                 }
                                 else if (count_day >= 14 & count_day <= 15)
                                 {
                                     ls14.Add(rc.id);
                                 }
                                 else if (count_day > 15)
                                 {
                                     ls16.Add(rc.id);
                                     
                                 }


                             }
                             else
                             {
                                 ls16.Add(rc.id);
                             }


                         }//end foreach


                         if (day == "5")
                         {
                             v = v.Where(c => ls5.Contains(c.id));

                         }
                         else if (day == "8")
                         {

                             v = v.Where(c => ls8.Contains(c.id));
                         }
                         else if (day == "11")
                         {
                             v = v.Where(c => ls11.Contains(c.id));
                         }
                         else if (day == "14")
                         {
                             v = v.Where(c => ls14.Contains(c.id));

                         }
                         else if (day == "16")//>15
                         {

                             v = v.Where(c => ls16.Contains(c.id));

                         }

                    }


                    if(type_area=="AREA")
                    {
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

                        if (section_id != "")
                        {
                            v = v.Where(c => c.section_id == section_id.Trim());

                        }


                    }
                    else
                    {

                        if (company_id != "")
                        {
                            v = v.Where(c => c.activity_company_id == company_id.Trim());

                        }

                        if (function_id != "")
                        {
                            v = v.Where(c => c.activity_function_id == function_id.Trim());

                        }

                        if (department_id != "")
                        {
                            v = v.Where(c => c.activity_department_id == department_id.Trim());

                        }

                        if (division_id != "")
                        {
                            v = v.Where(c => c.activity_division_id == division_id.Trim());

                        }

                        if (section_id != "")
                        {
                            v = v.Where(c => c.activity_section_id == section_id.Trim());

                        }


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


                    if (employee_id != "")
                    {
                        v = v.Where(c => c.employee_id.Contains(employee_id));

                    }

                }
                else
                {

                    if (type_area == "AREA")
                    {

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

                        if (section_id != "")
                        {
                            v = v.Where(c => c.section_id == section_id.Trim());

                        }
                    }
                    else
                    {
                        if (company_id != "")
                        {
                            v = v.Where(c => c.activity_company_id == company_id.Trim());

                        }

                        if (function_id != "")
                        {
                            v = v.Where(c => c.activity_function_id == function_id.Trim());

                        }

                        if (department_id != "")
                        {
                            v = v.Where(c => c.activity_department_id == department_id.Trim());

                        }

                        if (division_id != "")
                        {
                            v = v.Where(c => c.activity_division_id == division_id.Trim());

                        }

                        if (section_id != "")
                        {
                            v = v.Where(c => c.activity_section_id == section_id.Trim());

                        }

                        v = v.Where(c => c.owner_activity == "KNOWN");

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


                    if (employee_id != "")
                    {
                        v = v.Where(c => c.employee_id.Contains(employee_id));

                    }

                    if (date_start == "" && date_end == "")//default ปีปัจจุบัน
                    {

                        v = v.Where(c => c.incident_date2.Value.Year == DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year);


                    }



                }




                if (word_search != "")
                {

                    v = v.Where(c => c.doc_no.Contains(word_search) || c.incident_name.Contains(word_search) || c.incident_detail.Contains(word_search));


                }




                int totalRow = v.Count();
                int lenght = Convert.ToInt32(Context.Request["length"].ToString());
                int start = Convert.ToInt32(Context.Request["start"].ToString());
                int draw = Convert.ToInt32(Context.Request["draw"].ToString());

                v = v.Skip(start).Take(lenght);



                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    string incident_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.incident_date, CultureInfo.InvariantCulture), lang);
                    dt.Add(rc.id);
                    dt.Add(rc.step_form);
                    dt.Add(rc.doc_no);
                    dt.Add(rc.incident_name);
                    dt.Add(rc.incident_detail);
                    dt.Add(rc.incident_area);
                    dt.Add(incident_date);

                    string code_status = "";
                    if (rc.process_status == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i> ";

                    }
                    //else if (rc.process_status == 2)
                    //{//delay

                    //    code_status = "<i class=\"fa fa-circle text-info\"></i> ";

                    //}
                    else if (rc.process_status == 3)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-reject\"></i> ";
                    }
                    else if (rc.process_status == 2)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i> ";

                    }
                    else if (rc.process_status == 4)
                    {//exemption

                        code_status = "<i class=\"fa fa-circle text-reject\"></i> ";
                    }





                    string step = "";


                    if (rc.process_status != 2 && rc.process_status != 3 && rc.process_status != 4)//ไม่ใช้ close กับ reject กับ Exemption
                    {

                        if (rc.step_form == 1)//supervisor
                        {
                            string v_step = chageDataLanguage("รายงานอุบัติการณ์", "Incident Report", lang);
                            step = step + "(" + v_step + " - Area Supervisor)";

                        }
                        else if (rc.step_form == 2)
                        {
                            string v_step = chageDataLanguage("ตรวจสอบรายงานอุบัติการณ์", "Verify Incident Report", lang);

                            if (rc.submit_report_form2 == null && rc.confirm_investigate_form2 == null)
                            {
                                step = step + "(" + v_step + " - Area Supervisor)";
                            }

                            if (Session["country"].ToString() == "thailand")
                            {
                                if (rc.submit_report_form2 != null && rc.confirm_investigate_form2 == null)
                                {
                                    step = step + "(" + v_step + " - Area OH&S)";
                                }
                            }
                            else if (Session["country"].ToString() == "srilanka")
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

                            if (Session["country"].ToString() == "thailand")
                            {
                                step = step + "(" + v_step + " - Area OH&S)";
                            }
                            else if (Session["country"].ToString() == "srilanka")
                            {
                                step = step + "(" + v_step + " - Area Manager)";
                            }
                                
                            

                        }
                        else if (rc.step_form == 4)
                        {
                            string v_step = chageDataLanguage("ขอปิดรายงานอุบัติการณ์", "Request to Close Incident Report", lang);

                            bool check_close = true;

                            var s = from c in dbConnect.close_step_incidents
                                    where c.country == Session["country"].ToString()
                                    orderby c.step descending
                                    select c;

                            foreach (var r in s)
                            {
                                var w = from c in dbConnect.log_request_close_incidents
                                        where c.incident_id == rc.id && c.status == "A"
                                       // && c.group_id == r.group_id
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


                                if (check_close==true)
                                {// แสดงว่าปิดแล้ว

                                    step = "";

                                }



                        }

                    }



                    string pathhost = System.Configuration.ConfigurationManager.AppSettings["pathhost"];
                    string export = "<a href='" + pathhost + "//IncidentExport.aspx?id="+rc.id + "'><i class='fa fa-download fa-2x'></i></a> ";
                    dt.Add(code_status + rc.status + " " + step);

                    dt.Add(export);

                    dataJson.Add(dt);


                }

                var result = new
                {
                    draw = draw,
                    recordsTotal = totalRow,
                    recordsFiltered = totalRow,
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));

            }
        }





        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]

    public void getListAllsot(     string company_id,
                                    string function_id,
                                    string department_id,
                                    string status,
                                    string date_start,
                                    string date_end,
                                    string employee_id,
                                    string type,
                                    string lang)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                if (type == "filter")
                {
                    Session["company_id_selected_sot"] = company_id == null ? "" : company_id;
                    Session["function_id_selected_sot"] = function_id == null ? "" : function_id;
                    Session["department_id_selected_sot"] = department_id == null ? "" : department_id;
                    Session["date_start_selected_sot"] = date_start == null ? "" : date_start;
                    Session["date_end_selected_sot"] = date_end == null ? "" : date_end;
                    Session["status_selected_sot"] = status == null ? "" : status;

                }

                string word_search = Context.Request["search[value]"].ToString();

                var v = from d in dbConnect.sots
                        join s in dbConnect.sot_status on d.process_status equals s.id
                        where d.country == Session["country"].ToString()
                        orderby d.doc_no descending

                        select new
                        {
                            d.company_id,
                            d.function_id,
                            d.department_id,
                            d.id,
                            d.type_work,
                            d.process_status,
                            d.doc_no,
                            d.comment,
                            sot_date = d.sot_date,
                            sot_date_end = d.sot_date_end,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            d.location,
                            d.employee_id

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



                if (status != "" && status != "null")
                {
                    v = v.Where(c => c.process_status == Convert.ToByte(status));

                }
           


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.sot_date >= d_start);
                }


                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.sot_date_end <= d_end);
                }


                if (date_start == "" && date_end == "")//default ปีปัจจุบัน
                {

                   v = v.Where(c => c.sot_date.Value.Year == DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year);


                }


                if (employee_id != "")
                {
                    var v1 = from d in dbConnect.employee_has_sots
                             where d.employee_id.Contains(employee_id)
                             select new
                             {
                                 d.sot_id
                             };

                    List<int> ls = new List<int>();
                    foreach (var rc in v1)
                    {
                        ls.Add(rc.sot_id);
                    }

                
                    v = v.Where(c =>ls.Contains(c.id));

                }




                if (word_search != "")
                {

                    v = v.Where(c => c.doc_no.Contains(word_search) ||c.comment.Contains(word_search));


                }


                int totalRow = v.Count();
                int lenght = Convert.ToInt32(Context.Request["length"].ToString());
                int start = Convert.ToInt32(Context.Request["start"].ToString());
                int draw = Convert.ToInt32(Context.Request["draw"].ToString());

                v = v.Skip(start).Take(lenght);



                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    string sot_date = FormatDates.getDatetimeShowShot(Convert.ToDateTime(rc.sot_date, CultureInfo.InvariantCulture), lang);
                    string sot_date_time_end = FormatDates.getTimeShowFromDate(Convert.ToDateTime(rc.sot_date_end, CultureInfo.InvariantCulture), lang);
       
                    dt.Add(rc.id);
                    dt.Add(rc.doc_no);
                    dt.Add(rc.type_work);
                    //dt.Add(rc.observation);
                    dt.Add(sot_date + " - " + sot_date_time_end);

                    string code_status = "";
                    if (rc.process_status == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i> ";

                    }
                    else if (rc.process_status == 3)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-reject\"></i> ";
                    }
                    else if (rc.process_status == 2)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i> ";

                    }




                    dt.Add(code_status + rc.status);

                    dataJson.Add(dt);


                }

                var result = new
                {
                    draw = draw,
                    recordsTotal = totalRow,
                    recordsFiltered = totalRow,
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));

            }
        }




       [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        
        [ScriptMethod(UseHttpGet = true)]

        public void getListAllHazard(string company_id,
                                       string function_id,
                                       string department_id,
                                       string division_id,
                                       string section_id,
                                       string status,
                                       string date_start,
                                       string date_end,
                                       string employee_id,
                                       string type,
                                       string form,
                                       string day,
                                       string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                if (type == "filter")
                {
                    Session["company_id_selected"] = company_id == null ? "" : company_id;
                    Session["function_id_selected"] = function_id == null ? "" : function_id;
                    Session["department_id_selected"] = department_id == null ? "" : department_id;
                    Session["section_id_selected"] = section_id == null ? "" : section_id;
                    Session["division_id_selected"] = division_id == null ? "" : division_id;
                    Session["status_selected"] = status == null ? "" : status;
                    // Session["year_selected"] = year == null ? "" : year;
                    Session["date_start_selected"] = date_start == null ? "" : date_start;
                    Session["date_end_selected"] = date_end == null ? "" : date_end;


                }

                string word_search = Context.Request["search[value]"].ToString();

                var v = from d in dbConnect.hazards
                        join s in dbConnect.hazard_status on d.process_status equals s.id
                        let p = dbConnect.process_actions.Where(p2=>d.id == p2.hazard_id).FirstOrDefault()
                        where d.country == Session["country"].ToString()
                        orderby d.report_date descending

                        select new
                        {
                            d.company_id,
                            d.function_id,
                            d.department_id,
                            d.division_id,
                            d.section_id,
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
                            d.employee_id,
                            d.hazard_area,
                            d.report_date,
                            d.confirm_form_two_to_three_at,
                            hazard_id = p == null ? null :p.hazard_id.ToString()



                        };

               



                if (form != "")
                {

                    if (form == "1to2")
                    {

                        v = v.Where(c => c.process_status == 1 && c.step_form == 1);

                        List<int> ls6 = new List<int>();
                        List<int> ls7 = new List<int>();
                        List<int> ls8 = new List<int>();
                        List<int> ls11 = new List<int>();
                        List<int> ls16 = new List<int>();

                        foreach (var rc in v)
                        {
                            DateTime dtnew = rc.report_date;
                            DateTime current = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                            TimeSpan span = current.Subtract(dtnew);
                            int cday = Convert.ToInt16(span.TotalDays);
                            int count_day = 0;


                            if (cday < 30)
                            {
                                while (dtnew <= current)
                                {
                                    dtnew = dtnew.AddDays(1);

                                    if (dtnew.DayOfWeek.ToString() != "Saturday" && dtnew.DayOfWeek.ToString() != "Sunday")
                                    {
                                        var holidays = from c in dbConnect.holidays
                                                       where c.holiday_date == dtnew.Date
                                                       select new
                                                       {
                                                           holiday_date = c.holiday_date,

                                                       };

                                        bool is_holiday = false;
                                        foreach (var r in holidays)
                                        {
                                            is_holiday = true;
                                        }

                                        if (!is_holiday)
                                        {
                                            count_day++;

                                        }

                                    }//end if

                                }//end while


                                ////////////////////////////////////////////////////////////////////////////////////////////
                                if (count_day == 6)
                                {
                                    ls6.Add(rc.id);
                                }
                                else if (count_day == 7)
                                {
                                    ls7.Add(rc.id);

                                }
                                else if (count_day >= 8 & count_day <= 10)
                                {
                                    ls8.Add(rc.id);

                                }
                                else if (count_day >= 11 & count_day <= 15)
                                {
                                    ls11.Add(rc.id);
                                }
                                else if (count_day > 15)
                                {
                                    ls16.Add(rc.id);
                                    
                                }


                            }
                            else
                            {
                                ls16.Add(rc.id);
                            }


                        }//end foreach


                       if (day == "6")
                        {
                            v = v.Where(c => ls6.Contains(c.id));
                        }
                        else if (day == "7")
                        {
                            v = v.Where(c => ls7.Contains(c.id));
                        }
                        else if (day == "8")
                        {
                            v = v.Where(c => ls8.Contains(c.id));

                        }
                       else if (day == "11")
                       {
                           v = v.Where(c => ls11.Contains(c.id));
                       }
                        else if (day == "16")//>15
                        {
                            v = v.Where(c => ls16.Contains(c.id));
                        }


                    }
                    else if (form == "2to3")
                    {
                        v = v.Where(c => c.hazard_id == null);
                        v = v.Where(c => c.process_status == 1 && c.step_form == 3 && c.confirm_form_two_to_three_at != null);

                        List<int> ls6 = new List<int>();
                        List<int> ls7 = new List<int>();
                        List<int> ls8 = new List<int>();
                        List<int> ls11 = new List<int>();
                        List<int> ls16 = new List<int>();

                        foreach (var rc in v)
                        {
                            DateTime dtnew = rc.confirm_form_two_to_three_at.Value;
                            DateTime current = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                            TimeSpan span = current.Subtract(dtnew);
                            int cday = Convert.ToInt16(span.TotalDays);
                            int count_day = 0;


                            if (cday < 30)
                            {
                                while (dtnew <= current)
                                {
                                    dtnew = dtnew.AddDays(1);

                                    if (dtnew.DayOfWeek.ToString() != "Saturday" && dtnew.DayOfWeek.ToString() != "Sunday")
                                    {
                                        var holidays = from c in dbConnect.holidays
                                                       where c.holiday_date == dtnew.Date
                                                       select new
                                                       {
                                                           holiday_date = c.holiday_date,

                                                       };

                                        bool is_holiday = false;
                                        foreach (var r in holidays)
                                        {
                                            is_holiday = true;
                                        }

                                        if (!is_holiday)
                                        {
                                            count_day++;

                                        }

                                    }//end if

                                }//end while


                                ////////////////////////////////////////////////////////////////////////////////////////////
                                if (count_day == 6)
                                {
                                    ls6.Add(rc.id);
                                }
                                else if (count_day == 7)
                                {
                                    ls7.Add(rc.id);

                                }
                                else if (count_day >= 8 & count_day <= 10)
                                {
                                    ls8.Add(rc.id);

                                }
                                else if (count_day >= 11 & count_day <= 15)
                                {
                                    ls11.Add(rc.id);
                                }
                                else if (count_day > 15)
                                {
                                    ls16.Add(rc.id);

                                }


                            }
                            else
                            {
                                ls16.Add(rc.id);
                            }


                        }//end foreach


                        if (day == "6")
                        {
                            v = v.Where(c => ls6.Contains(c.id));
                        }
                        else if (day == "7")
                        {
                            v = v.Where(c => ls7.Contains(c.id));
                        }
                        else if (day == "8")
                        {
                            v = v.Where(c => ls8.Contains(c.id));

                        }
                        else if (day == "11")
                        {
                            v = v.Where(c => ls11.Contains(c.id));
                        }
                        else if (day == "16")//>15
                        {
                            v = v.Where(c => ls16.Contains(c.id));
                        }



                    }


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

                    if (section_id != "")
                    {
                        v = v.Where(c => c.section_id == section_id.Trim());

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


                    if (employee_id != "")
                    {
                        v = v.Where(c => c.employee_id.Contains(employee_id));

                    }


                }
                else
                {

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

                    if (section_id != "")
                    {
                        v = v.Where(c => c.section_id == section_id.Trim());

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

                        v = v.Where(c => c.hazard_date2.Value.Year == DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year);

                    }

                    if (employee_id != "")
                    {
                        v = v.Where(c => c.employee_id.Contains(employee_id));

                    }

                }

                if (word_search != "")
                {

                    v = v.Where(c => c.doc_no.Contains(word_search) || c.hazard_name.Contains(word_search) || c.hazard_detail.Contains(word_search));


                }


                int totalRow = v.Count();
                int lenght = Convert.ToInt32(Context.Request["length"].ToString());
                int start = Convert.ToInt32(Context.Request["start"].ToString());
                int draw = Convert.ToInt32(Context.Request["draw"].ToString());

                 v = v.Skip(start).Take(lenght);



                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    string hazard_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.hazard_date, CultureInfo.InvariantCulture), lang);
                    dt.Add(rc.id);
                    dt.Add(rc.step_form);
                    dt.Add(rc.doc_no);
                    dt.Add(rc.hazard_name);
                    dt.Add(rc.hazard_detail);
                    dt.Add(rc.hazard_area);
                    dt.Add(hazard_date);

                    string code_status = "";
                    if (rc.process_status == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i> ";

                    }
                    //else if (rc.process_status == 2)
                    //{//delay

                    //    code_status = "<i class=\"fa fa-circle text-danger\"></i> ";

                    //}
                    else if (rc.process_status == 3)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-reject\"></i> ";
                    }
                    else if (rc.process_status == 2)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i> ";

                    }





                    string step = "";


                    if (rc.process_status != 2 && rc.process_status != 3)//ไม่ใช้ close กับ reject
                    {

                        if (rc.step_form == 1)//area oh&s
                        {
                            string v_step = chageDataLanguage("รายงานแหล่งอันตราย", "Hazard report", lang);

                            if (Session["country"].ToString() == "thailand")
                            {
                                step = step + "(" + v_step + " - Area OH&S)";

                            }else if (Session["country"].ToString() == "srilanka")
                            {
                                step = step + "(" + v_step + " - Area Supervisor)";
                            }
    

                        }
                        else if (rc.step_form == 2)
                        {
                            string v_step = chageDataLanguage("ตรวจสอบรายงานแหล่งอันตราย", "Verify Hazard Report", lang);

                            if (rc.submit_report_form2 == null)
                            {

                                if (Session["country"].ToString() == "thailand")
                                {
                                      step = step + "(" + v_step + " - Area OH&S)";

                                }else if (Session["country"].ToString() == "srilanka")
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
                                    where c.country == Session["country"].ToString()
                                    orderby c.step descending
                                    select c;

                            foreach (var r in s)
                            {
                                var w = from c in dbConnect.log_request_close_hazards
                                        where c.hazard_id== rc.id && c.status == "A"
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



                    dt.Add(code_status + rc.status + " " + step);

                    string pathhost = System.Configuration.ConfigurationManager.AppSettings["pathhost"];
                    string export = "<a href='" + pathhost + "//HazardExport.aspx?id=" + rc.id + "'><i class='fa fa-download fa-2x'></i></a> ";
                    dt.Add(export);

                    dataJson.Add(dt);


                }


                var result = new
                {
                    draw= draw,
                    recordsTotal = totalRow,
                    recordsFiltered = totalRow,
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }




       [WebMethod(EnableSession = true)]
       //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]

       [ScriptMethod(UseHttpGet = true)]

       public void getListAllHealth(string company_id,
                                      string function_id,
                                      string department_id,
                                      string division_id,
                                      string section_id,
                                      string status,
                                      string date_start,
                                      string date_end,
                                      string employee_id,
                                      string type,
                                      string lang)
       {

           using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
           {
               if (type == "filter")
               {
                   Session["company_id_selected_health"] = company_id == null ? "" : company_id;
                   Session["function_id_selected_health"] = function_id == null ? "" : function_id;
                   Session["department_id_selected_health"] = department_id == null ? "" : department_id;
                   Session["section_id_selected_health"] = section_id == null ? "" : section_id;
                   Session["division_id_selected_health"] = division_id == null ? "" : division_id;
                   Session["status_selected_health"] = status == null ? "" : status;
                   // Session["year_selected"] = year == null ? "" : year;
                   Session["date_start_selected_health"] = date_start == null ? "" : date_start;
                   Session["date_end_selected_health"] = date_end == null ? "" : date_end;


               }

               string word_search = Context.Request["search[value]"].ToString();

               var v = from d in dbConnect.healths
                       join s in dbConnect.health_status on d.process_status equals s.id
                       join e in dbConnect.employees on d.health_employee_id equals e.employee_id
                       where d.country == Session["country"].ToString()
                       orderby d.report_date descending

                       select new
                       {
                           d.company_id,
                           d.function_id,
                           d.department_id,
                           d.division_id,
                           d.section_id,
                           d.id,
                           d.edit_form1,
                           d.edit_form2,
                           d.request_close_form,
                           d.age,
                           d.health_employee_id,
                           d.year_health,
                           d.job_type_machine_type,
                           d.service_year,
                           d.service_year_current,
                           d.step_form,
                           d.process_status,
                           d.doc_no,
                           status = chageDataLanguage(s.name_th, s.name_en, lang),
                           d.employee_id,
                           d.report_date,
                           report_date2 = d.report_date,
                           d.confirm_form_one_to_two_at,
                           first_name = chageDataLanguage(e.first_name_th,e.first_name_en,lang),
                           last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),




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

                if (section_id != "")
                {
                    v = v.Where(c => c.section_id == section_id.Trim());

                }

                if (status != "" && status != "null")
                {
                    v = v.Where(c => c.process_status == Convert.ToByte(status));

                }

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.report_date >= d_start);
                }


                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.report_date <= d_end);
                }


                if (date_start == "" && date_end == "")//default ปีปัจจุบัน
                {

                    v = v.Where(c => c.report_date.Year == DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year);

                }

                if (employee_id != "")
                {
                    v = v.Where(c => c.employee_id.Contains(employee_id));

                }

               

               if (word_search != "")
               {

                   v = v.Where(c => c.doc_no.Contains(word_search) || c.year_health.Contains(word_search) || c.health_employee_id.Contains(word_search));


               }

               //////////////////////////////////////////////ภายใต้ area //////////////////////////////////////////////////////////////////////////////
               ArrayList functions_arr = Session["area_function"] as ArrayList;
               ArrayList departments_arr = Session["area_department"] as ArrayList;
               ArrayList department_functional_arr = Session["area_department_functional"] as ArrayList;
               ArrayList divisions_arr = Session["area_division"] as ArrayList;
               ArrayList sections_arr = Session["area_section"] as ArrayList;
               ArrayList group_arr = Session["group"] as ArrayList;


               List<string> functions = functions_arr.Cast<string>().ToList();
               List<string> departments = departments_arr.Cast<string>().ToList();
               List<string> department_functional = department_functional_arr.Cast<string>().ToList();
               List<string> divisions = divisions_arr.Cast<string>().ToList();
               List<string> sections = sections_arr.Cast<string>().ToList();
               List<string> group = group_arr.Cast<string>().ToList();
               // bool area_all = false;

                if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S") > -1)
                {
                    //area_all = true;
                }
                else
                {


                    if (group.IndexOf("Delegate OH&S Admin") > -1 || group.IndexOf("OH&S Admin") > -1 || group.IndexOf("Area Functional Manager") > -1)
                    {

                        if (group.IndexOf("Delegate OH&S Admin") > -1 || group.IndexOf("OH&S Admin") > -1)
                        {
                            v = v.Where(c => functions.Contains(c.function_id));
                        }
                        else
                        {
                            //v = v.Where(c => department_functional.Contains(c.department_id));////แก้ให้เห็นทั้ง function ก่อน เพราะตอนนี้มีปัญหา พนักงานบางคนเปลี่ยนเลข department ทำให้ไปฟิลเตอร์ข้อมูล arg ใหม่ ต่อให้เลือกตัว active ก็ตาม
                            v = v.Where(c => c.function_id == Session["function_id"].ToString());

                        }

                    }
                    else
                    {

                        if (group.IndexOf("Area OH&S") > -1)//area oh&s ดึงเฉพาะฟอร์มที่ตัวเองกรอกเท่านั้น
                        {
                            //v = v.Where(c => c.employee_id.Contains(Session["user_id"].ToString()));//พี่ออยสั่งแก้ เพราะเคยมีเคส area oh&s ทำฟอร์ม 2 ไม่ได้
                            //v = v.Where(c => departments.Contains(c.department_id));//แก้ให้เห็นทั้ง function ก่อน เพราะตอนนี้มีปัญหา พนักงานบางคนเปลี่ยนเลข department ทำให้ไปฟิลเตอร์ข้อมูล arg ใหม่ ต่อให้เลือกตัว active ก็ตาม
                            v = v.Where(c => c.function_id == Session["function_id"].ToString());
                        }



                    }

                    

                }
               


               int totalRow = v.Count();
               int lenght = Convert.ToInt32(Context.Request["length"].ToString());
               int start = Convert.ToInt32(Context.Request["start"].ToString());
               int draw = Convert.ToInt32(Context.Request["draw"].ToString());

               v = v.Skip(start).Take(lenght);



               ArrayList dataJson = new ArrayList();

               foreach (var rc in v)
               {
                   ArrayList dt = new ArrayList();
                   string report_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.report_date2, CultureInfo.InvariantCulture), lang);
                   dt.Add(rc.id);
                   dt.Add(rc.step_form);
                   dt.Add(rc.doc_no);
                   dt.Add(rc.health_employee_id);
                   dt.Add(rc.first_name+" "+rc.last_name);
                   dt.Add(rc.year_health);
                  
                   dt.Add(report_date);

                   string code_status = "";

                   if (rc.process_status == 1)//on process
                   {

                       code_status = "<i class=\"fa fa-circle text-warning\"></i> ";

                   }
                  
                   else if (rc.process_status == 2)//close
                   {
                       code_status = "<i class=\"fa fa-circle text-info\"></i> ";

                   }
                   else if (rc.process_status == 3)
                   {//reject

                       code_status = "<i class=\"fa fa-circle text-reject\"></i>";
                   }





                   string step = "";


                   if (rc.process_status != 2 && rc.process_status != 3)//ไม่ใช้ close กับ reject
                   {

                       if (rc.step_form == 1)//area oh&s
                       {
                           string v_step = chageDataLanguage("รายงานแผนฟื้นฟูสุขภาพพนักงาน", "Health Rehabilitation report", lang);

                           step = step + "(" + v_step + " - Area OH&S)";


                       }
                       else if (rc.step_form == 2)
                       {

                           string v_step = chageDataLanguage("มาตรการการฟื้นฟูสุขภาพ", "Health Rehabilitation Action", lang);

                           step = step + "(" + v_step + " - Area OH&S)";

                       }
                       else if (rc.step_form == 3)
                       {
                           string v_step = chageDataLanguage("ขอปิดรายงานแผนฟื้นฟูสุขภาพพนักงาน", "Request to Close Health Rehabilitation Report", lang);
                           bool check_close = true;

                           var s = from c in dbConnect.close_step_healths
                                   where c.country == Session["country"].ToString()
                                   orderby c.step descending
                                   select c;

                           foreach (var r in s)
                           {
                               var w = from c in dbConnect.log_request_close_healths
                                       where c.health_id == rc.id && c.status == "A"
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

                                   if (r.group_id == 17)
                                   {
                                       step = "(" + v_step + " -  Group OH&S Health)";
                                   }

                                   if (r.group_id == 18)
                                   {
                                       step = "(" + v_step + " -  Area Functional Manager)";
                                   }


                               }




                           }//end each


                           if (check_close == true)
                           {// แสดงว่าปิดแล้ว

                               step = "";

                           }



                       }

                   }



                   dt.Add(code_status + rc.status + " " + step);

                   string pathhost = System.Configuration.ConfigurationManager.AppSettings["pathhost"];
                   string export = "<a href='" + pathhost + "//HealthExport.aspx?id=" + rc.id + "'><i class='fa fa-download fa-2x'></i></a> ";
                   dt.Add(export);

                   dataJson.Add(dt);


               }


               var result = new
               {
                   draw = draw,
                   recordsTotal = totalRow,
                   recordsFiltered = totalRow,
                   data = dataJson
               };

               JavaScriptSerializer js = new JavaScriptSerializer();
               Context.Response.Write(js.Serialize(result));
           }

       }



        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListInjuryPerson(string incident_id,string lang,string pagetype)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.injury_persons
                        join t in dbConnect.type_employments on c.type_employment_id equals t.id
                        join f in dbConnect.functions on c.function_id equals f.function_id into joinF
                        join n in dbConnect.nature_injuries on c.nature_injury_id equals n.id
                        join b in dbConnect.body_parts on c.body_parts_id equals b.id into boDefalt
                        join s in dbConnect.severity_injuries on c.severity_injury_id equals s.id
                        from f in joinF.DefaultIfEmpty()
                        from b in boDefalt.DefaultIfEmpty()
                        where c.incident_id == Convert.ToInt32(incident_id) && c.status == "A"
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.full_name,
                            type_employment_name = chageDataLanguage(t.name_th, t.name_en, lang),
                            function_name = chageDataLanguage(f.function_th, f.function_en, lang),
                            nature_injury_name = chageDataLanguage(n.name_th, n.name_en, lang),
                            body_parts_id = chageDataLanguage(b.name_th, b.name_en, lang),
                            severity_injury_name = chageDataLanguage(s.name_th, s.name_en, lang),
                            c.day_lost,
                            c.remark

                        };


                ArrayList dataJson = new ArrayList();
                int count = 1;
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(count);
                    dt.Add(rc.full_name);
                    dt.Add(rc.type_employment_name);
                    dt.Add(rc.function_name);
                    dt.Add(rc.nature_injury_name);
                    dt.Add(rc.body_parts_id);
                    dt.Add(rc.severity_injury_name);
                    dt.Add(rc.day_lost);
                    dt.Add(rc.remark);
                    if (pagetype == "view")
                    {
                        dt.Add("");

                    }
                    else
                    {
                        string edit = "<a href='javascript:ShowEditInjuryPerson(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";
                        string delete = "<a href='javascript:DeleteInjuryPerson(" + rc.id + ");'><i class='fa fa-trash fa-2x'></i></a>";
                        dt.Add(edit + delete);
                    }

                    dataJson.Add(dt);
                    count++;

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));

            }
        }



        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListDamageList(string incident_id,string pagetype)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.damage_lists
                        where c.incident_id == Convert.ToInt32(incident_id) && c.status == "A"
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.property_environment_damage,
                            c.detail_damage,
                            c.damage_cost


                        };


                ArrayList dataJson = new ArrayList();
                int count = 1;
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(count);
                    dt.Add(rc.property_environment_damage);
                    dt.Add(rc.detail_damage);
                    dt.Add(rc.damage_cost);

                    if (pagetype == "view")
                    {
                        dt.Add("");

                    }
                    else
                    {
                        string edit = "<a href='javascript:ShowEditDamageList(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";
                        string delete = "<a href='javascript:DeleteDamageList(" + rc.id + ");'><i class='fa fa-trash fa-2x'></i></a>";
                        dt.Add(edit + delete);
                    }

                    dataJson.Add(dt);
                    count++;

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }


        [WebMethod(EnableSession=true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListFactFinding(string incident_id,string lang,string pagetype)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.fact_findings
                        join s in dbConnect.source_incidents on c.source_incident_id equals s.id
                        join e in dbConnect.event_exposures on c.event_exposure_id equals e.id
                        where c.incident_id == Convert.ToInt32(incident_id) && c.status == "A"
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.fact_finding_name,
                            source_incident = chageDataLanguage(s.name_th, s.name_en, lang),
                            event_exposure = chageDataLanguage(e.name_th, e.name_en, lang),
                            c.unsafe_action,
                            c.unsafe_condition,
                            c.evidence_file

                        };


                ArrayList dataJson = new ArrayList();
                int count = 1;
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(count);
                    dt.Add(rc.fact_finding_name);
                    dt.Add(rc.source_incident);
                    dt.Add(rc.event_exposure);

                    string unsafe_action = "";
                    if (rc.unsafe_action == "N")
                    {
                        unsafe_action = "<i class='fa fa-close fa-2x'></i>";
                    }
                    else
                    {
                        unsafe_action = "<i class='fa fa-check fa-2x'></i>";
                    }

                    string unsafe_condition = "";
                    if (rc.unsafe_condition == "N")
                    {
                        unsafe_condition = "<i class='fa fa-close fa-2x'></i>";
                    }
                    else
                    {
                        unsafe_condition = "<i class='fa fa-check fa-2x'></i>";
                    }
                    dt.Add(unsafe_action);
                    dt.Add(unsafe_condition);

                    string path_file = "";
                    string country = Session["country"].ToString();
                    var d = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(incident_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        if (rc.evidence_file != "")
                        {
                            path_file = "<a target='_blank' href='upload/incident/step3/" + country + "/" + p.doc_no + "/" + rc.evidence_file + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                        }

                    }

                    dt.Add(path_file);

                    if (pagetype == "view")
                    {
                        dt.Add("");

                    }
                    else
                    {
                        string edit = "<a href='javascript:ShowEditFactFinding(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";
                        string delete = "<a href='javascript:DeleteFactFinding(" + rc.id + ");'><i class='fa fa-trash fa-2x'></i></a>";
                        string action = edit + delete;
                        dt.Add(action);
                    }

                    dataJson.Add(dt);
                    count++;

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));

            }
        }




        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListCorrectivePreventive(string incident_id, string lang, string pagetype)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                        from e in joinE.DefaultIfEmpty()
                        join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinO
                        from o in joinO.DefaultIfEmpty()
                        join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                        from d in joinD.DefaultIfEmpty()
                        //  join r in dbConnect.root_cause_actions on c.root_cause_action_id equals r.id into joinR
                        // from r in joinR.DefaultIfEmpty()
                        where c.incident_id == Convert.ToInt32(incident_id) //&& r.status == "A"
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.corrective_preventive_action,
                            c.responsible_person,
                            due_date = c.due_date,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete,
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.root_cause_action,
                            due_date2 = c.due_date,
                            department_name = chageDataLanguage(d.department_th,d.department_en,lang)

                        };


                ArrayList dataJson = new ArrayList();
                int count = 1;
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(count);
                    dt.Add(rc.corrective_preventive_action);
                    dt.Add(rc.responsible_person);
                    dt.Add(rc.department_name);
                    if (rc.due_date!=null)
                    {
                        string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date, CultureInfo.InvariantCulture), lang);
                        dt.Add(duedate);
                    }
                    else
                    {
                        dt.Add("");
                    }

                    string status = "";
                    if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                    {
                        status = rc.status;

                        if (rc.date_complete !=null)
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                            
                        }
                        else
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }

                    }
                    else
                    {
                        status = rc.status;

                    }

                    dt.Add(status);
                    if (rc.date_complete!=null)
                    {
                        string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                        dt.Add(date);
                    }
                    else
                    {
                        dt.Add("");
                    }


                    string path_file = "";
                    string country = Session["country"].ToString();
                    var d = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(incident_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        if (rc.attachment_file != "")
                        {
                            path_file = "<a target='_blank' href='upload/incident/step3/" + country + "/" + p.doc_no + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                        }

                    }

                    dt.Add(path_file);
                    dt.Add(rc.notify_contractor);
                    dt.Add(rc.root_cause_action);




                    if (pagetype == "view")
                    {
                        dt.Add("");
                        dt.Add(rc.remark);
                        dt.Add("");

                    }
                    else
                    {
                        if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                        {
                            string div_start = "<div class='form-inline'>";
                            string action_close = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return closeAction(" + rc.id + ",\"" + rc.attachment_file + "\",\"" + "corrective" + "\")' >" + chageDataLanguage("ปิด", "Close", lang) + "</button>";
                            string action_reject = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return rejectAction(" + rc.id + ",\"" + "corrective" + "\")' >" + chageDataLanguage("ปฏิเสธ", "Reject", lang) + "</button>";
                            string action_cancel = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return cancelAction(" + rc.id + ",\"" + "corrective" + "\")' >" + chageDataLanguage("ยกเลิก", "Cancel", lang) + "</button>";
                            string div_end = "</div>";

                            string action = div_start;

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action close") > -1)
                                {
                                    action = action + action_close;
                                }
                            }

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action reject") > -1)
                                {
                                    action = action + action_reject;
                                }
                            }


                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action cancel") > -1)
                                {
                                    action = action + action_cancel;
                                }
                            }

                            action = action + div_start + div_end;
                            dt.Add(action);

                        }
                        else if (rc.action_status_id == 4)//close
                        {
                            string div_start = "<div class='form-inline'>";

                            string action_reject = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return rejectAction(" + rc.id + ",\"" + "corrective" + "\")' >" + chageDataLanguage("ปฏิเสธ", "Reject", lang) + "</button>";
                            string action_cancel = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return cancelAction(" + rc.id + ",\"" + "corrective" + "\")' >" + chageDataLanguage("ยกเลิก", "Cancel", lang) + "</button>";
                            string div_end = "</div>";

                            string action = div_start;

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action reject") > -1)
                                {
                                    action = action + action_reject;
                                }
                            }


                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action cancel") > -1)
                                {
                                    action = action + action_cancel;
                                }
                            }

                            action = action + div_start + div_end;
                            dt.Add(action);

                        }
                        else
                        {
                            dt.Add("");
                        }
                        string edit = "<a href='javascript:ShowEditCorrectivePreventive(" + rc.id + ",\"" + "corrective" + "\");'><i class='fa fa-pencil fa-2x'></i></a> ";

                        dt.Add(rc.remark);
                        dt.Add(edit);
                    }

                    dataJson.Add(dt);
                    count++;

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }











        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListPreventive(string incident_id, string lang, string pagetype)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.preventive_action_incidents
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                        from e in joinE.DefaultIfEmpty()
                        join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinO
                        from o in joinO.DefaultIfEmpty()
                        join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                        from d in joinD.DefaultIfEmpty()
                        //  join r in dbConnect.root_cause_actions on c.root_cause_action_id equals r.id into joinR
                        // from r in joinR.DefaultIfEmpty()
                        where c.incident_id == Convert.ToInt32(incident_id) //&& r.status == "A"
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.preventive_action,
                            c.responsible_person,
                            due_date = c.due_date,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete,
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.root_cause_action,
                            due_date2 = c.due_date,
                            department_name = chageDataLanguage(d.department_th, d.department_en, lang)

                        };


                ArrayList dataJson = new ArrayList();
                int count = 1;
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(count);
                    dt.Add(rc.preventive_action);
                    dt.Add(rc.responsible_person);
                    dt.Add(rc.department_name);
                    if (rc.due_date != null)
                    {
                        string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date, CultureInfo.InvariantCulture), lang);
                        dt.Add(duedate);
                    }
                    else
                    {
                        dt.Add("");
                    }

                    string status = "";
                    if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                    {
                        status = rc.status;

                        if (rc.date_complete != null)
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                        else
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }

                    }
                    else
                    {
                        status = rc.status;

                    }

                    dt.Add(status);
                    if (rc.date_complete != null)
                    {
                        string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                        dt.Add(date);
                    }
                    else
                    {
                        dt.Add("");
                    }


                    string path_file = "";
                    string country = Session["country"].ToString();
                    var d = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(incident_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        if (rc.attachment_file != "")
                        {
                            path_file = "<a target='_blank' href='upload/incident/step3/" + country + "/" + p.doc_no + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                        }

                    }

                    dt.Add(path_file);
                    dt.Add(rc.notify_contractor);
                    dt.Add(rc.root_cause_action);




                    if (pagetype == "view")
                    {
                        dt.Add("");
                        dt.Add(rc.remark);
                        dt.Add("");

                    }
                    else
                    {
                        if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                        {
                            string div_start = "<div class='form-inline'>";
                            string action_close = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return closeAction(" + rc.id + ",\"" + rc.attachment_file + "\",\"" + "preventive" + "\")' >" + chageDataLanguage("ปิด", "Close", lang) + "</button>";
                            string action_reject = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return rejectAction(" + rc.id + ",\"" + "preventive" + "\")' >" + chageDataLanguage("ปฏิเสธ", "Reject", lang) + "</button>";
                            string action_cancel = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return cancelAction(" + rc.id + ",\"" + "preventive" + "\")' >" + chageDataLanguage("ยกเลิก", "Cancel", lang) + "</button>";
                            string div_end = "</div>";

                            string action = div_start;

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action close") > -1)
                                {
                                    action = action + action_close;
                                }
                            }

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action reject") > -1)
                                {
                                    action = action + action_reject;
                                }
                            }


                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action cancel") > -1)
                                {
                                    action = action + action_cancel;
                                }
                            }

                            action = action + div_start + div_end;
                            dt.Add(action);

                        }
                        else if (rc.action_status_id == 4)//close
                        {
                            string div_start = "<div class='form-inline'>";

                            string action_reject = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return rejectAction(" + rc.id + ",\"" + "preventive" + "\")' >" + chageDataLanguage("ปฏิเสธ", "Reject", lang) + "</button>";
                            string action_cancel = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return cancelAction(" + rc.id + ",\"" + "preventive" + "\")' >" + chageDataLanguage("ยกเลิก", "Cancel", lang) + "</button>";
                            string div_end = "</div>";

                            string action = div_start;

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action reject") > -1)
                                {
                                    action = action + action_reject;
                                }
                            }


                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action cancel") > -1)
                                {
                                    action = action + action_cancel;
                                }
                            }

                            action = action + div_start + div_end;
                            dt.Add(action);

                        }
                        else
                        {
                            dt.Add("");
                        }
                        string edit = "<a href='javascript:ShowEditCorrectivePreventive(" + rc.id + ",\"" + "preventive" + "\");'><i class='fa fa-pencil fa-2x'></i></a> ";

                        dt.Add(rc.remark);
                        dt.Add(edit);
                    }

                    dataJson.Add(dt);
                    count++;

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }



        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListConsequence(string incident_id, string lang, string pagetype)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.consequence_management_incidents
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                        from e in joinE.DefaultIfEmpty()
                        join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinO
                        from o in joinO.DefaultIfEmpty()
                        join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                        from d in joinD.DefaultIfEmpty()
                        //  join r in dbConnect.root_cause_actions on c.root_cause_action_id equals r.id into joinR
                        // from r in joinR.DefaultIfEmpty()
                        where c.incident_id == Convert.ToInt32(incident_id) //&& r.status == "A"
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.consequence_management,
                            c.responsible_person,
                            due_date = c.due_date,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete,
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.root_cause_action,
                            due_date2 = c.due_date,
                            department_name = chageDataLanguage(d.department_th, d.department_en, lang)

                        };


                ArrayList dataJson = new ArrayList();
                int count = 1;
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(count);
                    dt.Add(rc.consequence_management);
                    dt.Add(rc.responsible_person);
                    dt.Add(rc.department_name);
                    if (rc.due_date != null)
                    {
                        string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date, CultureInfo.InvariantCulture), lang);
                        dt.Add(duedate);
                    }
                    else
                    {
                        dt.Add("");
                    }

                    string status = "";
                    if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                    {
                        status = rc.status;

                        if (rc.date_complete != null)
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                        else
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }

                    }
                    else
                    {
                        status = rc.status;

                    }

                    dt.Add(status);
                    if (rc.date_complete != null)
                    {
                        string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                        dt.Add(date);
                    }
                    else
                    {
                        dt.Add("");
                    }


                    string path_file = "";
                    string country = Session["country"].ToString();
                    var d = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(incident_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        if (rc.attachment_file != "")
                        {
                            path_file = "<a target='_blank' href='upload/incident/step3/" + country + "/" + p.doc_no + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                        }

                    }

                    dt.Add(path_file);
                    dt.Add(rc.notify_contractor);
                    dt.Add(rc.root_cause_action);




                    if (pagetype == "view")
                    {
                        dt.Add("");
                        dt.Add(rc.remark);
                        dt.Add("");

                    }
                    else
                    {
                        if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                        {
                            string div_start = "<div class='form-inline'>";
                            string action_close = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return closeAction(" + rc.id + ",\"" + rc.attachment_file + "\",\"" + "consequence" + "\")' >" + chageDataLanguage("ปิด", "Close", lang) + "</button>";
                            string action_reject = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return rejectAction(" + rc.id + ",\"" + "consequence" + "\")' >" + chageDataLanguage("ปฏิเสธ", "Reject", lang) + "</button>";
                            string action_cancel = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return cancelAction(" + rc.id + ",\"" + "consequence" + "\")' >" + chageDataLanguage("ยกเลิก", "Cancel", lang) + "</button>";
                            string div_end = "</div>";

                            string action = div_start;

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action close") > -1)
                                {
                                    action = action + action_close;
                                }
                            }

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action reject") > -1)
                                {
                                    action = action + action_reject;
                                }
                            }


                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action cancel") > -1)
                                {
                                    action = action + action_cancel;
                                }
                            }

                            action = action + div_start + div_end;
                            dt.Add(action);

                        }
                        else if (rc.action_status_id == 4)//close
                        {
                            string div_start = "<div class='form-inline'>";

                            string action_reject = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return rejectAction(" + rc.id + ",\"" + "consequence" + "\")' >" + chageDataLanguage("ปฏิเสธ", "Reject", lang) + "</button>";
                            string action_cancel = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return cancelAction(" + rc.id + ",\"" + "consequence" + "\")' >" + chageDataLanguage("ยกเลิก", "Cancel", lang) + "</button>";
                            string div_end = "</div>";

                            string action = div_start;

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action reject") > -1)
                                {
                                    action = action + action_reject;
                                }
                            }


                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report incident3 action cancel") > -1)
                                {
                                    action = action + action_cancel;
                                }
                            }

                            action = action + div_start + div_end;
                            dt.Add(action);

                        }
                        else
                        {
                            dt.Add("");
                        }
                        string edit = "<a href='javascript:ShowEditCorrectivePreventive(" + rc.id + ",\"" + "consequence" + "\");'><i class='fa fa-pencil fa-2x'></i></a> ";
                        
                        dt.Add(rc.remark);
                        dt.Add(edit);
                    }

                    dataJson.Add(dt);
                    count++;

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }



        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListProcessAction(string hazard_id, string lang, string pagetype)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.process_actions
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        join t in dbConnect.type_controls on c.type_control equals t.id
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                        from e in joinE.DefaultIfEmpty()
                        join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinO
                        from o in joinO.DefaultIfEmpty()
                        join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                        from d in joinD.DefaultIfEmpty()
                        where c.hazard_id == Convert.ToInt32(hazard_id)
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            type_control = chageDataLanguage(t.name_th, t.name_en, lang),
                            c.action,
                            c.responsible_person,
                            due_date = c.due_date.ToString(),
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete.ToString(),
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.root_cause_action,
                            due_date2 = c.due_date,
                            department_name = chageDataLanguage(d.department_th,d.department_en,lang)

                        };


                ArrayList dataJson = new ArrayList();
                int count = 1;
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(count);
                    dt.Add(rc.type_control);
                    dt.Add(rc.action);
                    dt.Add(rc.responsible_person);
                    dt.Add(rc.department_name);

                    if (!string.IsNullOrEmpty(rc.due_date))
                    {
                        string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date,CultureInfo.InvariantCulture), lang);
                        dt.Add(duedate);
                    }
                    else
                    {
                        dt.Add("");
                    }
                   


                    string status = "";
                    if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                    {
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else
                    {
                        status = rc.status;

                    }

                    dt.Add(status);

                    if (!string.IsNullOrEmpty(rc.date_complete))
                    {
                        string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                        dt.Add(date);
                    }
                    else
                    {
                        dt.Add("");
                    }


                    string path_file = "";
                    string country = Session["country"].ToString();
                    var d = from c in dbConnect.hazards
                            where c.id == Convert.ToInt32(hazard_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        if (rc.attachment_file != "")
                        {
                            path_file = "<a target='_blank' href='upload/hazard/step3/" + country + "/" + p.doc_no + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                        }

                    }

                    dt.Add(path_file);
                    dt.Add(rc.notify_contractor);



                    if (pagetype == "view")
                    {
                        dt.Add("");
                        dt.Add(rc.remark);
                        dt.Add("");

                    }
                    else
                    {
                        if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                        {
                            string div_start = "<div class='form-inline'>";
                            string action_close = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return closeAction(" + rc.id + ",\"" + rc.attachment_file + "\")' >" + chageDataLanguage("ปิด", "Close", lang) + "</button>";
                            string action_reject = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return rejectAction(" + rc.id + ")' >" + chageDataLanguage("ปฏิเสธ", "Reject", lang) + "</button>";
                            string action_cancel = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return cancelAction(" + rc.id + ")' >" + chageDataLanguage("ยกเลิก", "Cancel", lang) + "</button>";
                            string div_end = "</div>";

                            string action = div_start;

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report hazard3 action close") > -1)
                                {
                                    action = action + action_close;
                                }
                            }

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report hazard3 action reject") > -1)
                                {
                                    action = action + action_reject;
                                }
                            }


                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report hazard3 action cancel") > -1)
                                {
                                    action = action + action_cancel;
                                }
                            }

                            action = action + div_start + div_end;
                            dt.Add(action);

                        }
                        else if (rc.action_status_id == 4)//close
                        {
                            string div_start = "<div class='form-inline'>";

                            string action_reject = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return rejectAction(" + rc.id + ")' >" + chageDataLanguage("ปฏิเสธ", "Reject", lang) + "</button>";
                            string action_cancel = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return cancelAction(" + rc.id + ")' >" + chageDataLanguage("ยกเลิก", "Cancel", lang) + "</button>";
                            string div_end = "</div>";

                            string action = div_start;

                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report hazard3 action reject") > -1)
                                {
                                    action = action + action_reject;
                                }
                            }


                            if (Session["permission"] != null)
                            {
                                ArrayList per = Session["permission"] as ArrayList;
                                if (per.IndexOf("report hazard3 action cancel") > -1)
                                {
                                    action = action + action_cancel;
                                }
                            }

                            action = action + div_start + div_end;
                            dt.Add(action);

                        }
                        else
                        {
                            dt.Add("");
                        }
                        string edit = "<a href='javascript:ShowEditProcessAction(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";

                        dt.Add(rc.remark);
                        dt.Add(edit);
                    }

                    dataJson.Add(dt);
                    count++;

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }






        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListProcessActionHealth(string health_id, string lang, string pagetype)
        {



            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                     ArrayList dataJson = new ArrayList();

                    var v = from c in dbConnect.process_action_healths
                            join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                            from e in joinE.DefaultIfEmpty()
                            join s in dbConnect.action_health_status on c.action_status_id equals s.id
                            join t in dbConnect.type_control_healths on c.type_control_id equals t.id

                            where c.health_id == Convert.ToInt32(health_id) //&& r.status == "A"
                            orderby c.id descending
                            select new
                            {
                                c.id,
                                type_control = chageDataLanguage(t.name_th, t.name_en, lang),
                                c.action,
                                c.responsible_person,
                                due_date = c.due_date,
                                status = chageDataLanguage(s.name_th, s.name_en, lang),
                                c.doctor_opinion_file,
                                c.recovery_plan_file,
                                c.attachment_file,
                                c.remark,
                                c.action_status_id,
                                due_date2 = c.due_date,
                                date_complete = c.date_complete,

                            };



                    int count = 1;
                    foreach (var rc in v)
                    {
                        ArrayList dt = new ArrayList();
                        dt.Add(rc.id);
                       // dt.Add(count);
                        dt.Add(rc.type_control);
                        dt.Add(rc.action);
                        dt.Add(rc.responsible_person);
                        if (rc.due_date!=null)
                        {
                            string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date, CultureInfo.InvariantCulture), lang);                       
                            dt.Add(duedate);
                        }
                        else
                        {
                            dt.Add("");
                        }




                        string status = "";
                        if (rc.action_status_id != 2 && rc.action_status_id != 3)//cancel,close
                        {
                            status = rc.status;
                            if (rc.date_complete!=null)
                            {
                                if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                                {
                                    status = chageDataLanguage("ล่าช้า", "delay", lang);
                                }
                            }
                            else
                            {
                                if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                                {
                                    status = chageDataLanguage("ล่าช้า", "delay", lang);
                                }
                              

                            }
                        }
                        else
                        {
                            status = rc.status;

                        }

                        dt.Add(status);


                        if (rc.date_complete !=null)
                        {
                            string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                            dt.Add(date);
                        }
                        else
                        {
                            dt.Add("");
                        }
                       


                        string doctor_opinion_file = "";
                        string recovery_plan_file = "";
                        string attach_file = "";
                        string country = Session["country"].ToString();
                        var d = from c in dbConnect.healths
                                where c.id == Convert.ToInt32(health_id)
                                select new
                                {
                                    c.report_date,
                                    c.employee_id
                                };

                        foreach (var p in d)
                        {
                            string path_folder = p.employee_id + "_" + p.report_date.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB"));
                            if (rc.attachment_file != "")
                            {
                                attach_file = "<a target='_blank' href='upload/health/" + path_folder + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                            }

                            if (rc.doctor_opinion_file != "")
                            {
                                doctor_opinion_file = "<a target='_blank' href='upload/health/" + path_folder + "/" + rc.doctor_opinion_file + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";


                            }

                            if (rc.recovery_plan_file != "")
                            {
                                recovery_plan_file = "<a target='_blank' href='upload/health/" + path_folder + "/" + rc.recovery_plan_file + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";


                            }

                        }


                        dt.Add(recovery_plan_file);
                        dt.Add(doctor_opinion_file);
                        dt.Add(attach_file);



                        if (pagetype == "view")
                        {                      
                            dt.Add(rc.remark);                           
                            dt.Add("");

                        }
                        else
                        {                           
                            string edit = "<a href='javascript:ShowEditProcessAction(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";

                            dt.Add(rc.remark);
                            dt.Add(edit);
                        }

                        dataJson.Add(dt);
                        count++;

                    }

             

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }



        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListRiskFactorRelate(string health_id,string folder_name, string lang, string pagetype)
        {



            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                ArrayList dataJson = new ArrayList();

                if (pagetype != "add")
                {
                    var v = from c in dbConnect.risk_factor_relate_work_actions
                            join t in dbConnect.risk_factor_relate_works on c.risk_factor_relate_work_id equals t.id
                            join d in dbConnect.duration_risk_factors on c.duration_risk_factor_id equals d.id

                            where c.health_id == Convert.ToInt32(health_id) && c.status == "A"
                            orderby c.id descending
                            select new
                            {
                                c.id,
                                risk_factor_relate_work = chageDataLanguage(t.name_th, t.name_en, lang),
                                c.other,
                                c.year,
                                c.result,
                                duration_risk_factor_name = chageDataLanguage(d.name_th,d.name_en,lang),
                                c.file_risk_factor,
                                c.monitoring_results,
                                c.monitoring_environment
                               

                            };



                    int count = 1;
                    foreach (var rc in v)
                    {
                        ArrayList dt = new ArrayList();
                        dt.Add(rc.id);
                       // dt.Add(count);
                        dt.Add(rc.risk_factor_relate_work);
  
                        string monitoring_environment = "-";
                        if(rc.monitoring_environment=="Y")
                        {
                            monitoring_environment = Resources.Health.lbyes;
                        }
                        else if (rc.monitoring_environment == "N")
                        {
                            monitoring_environment = Resources.Health.lbno;
                        }
                        dt.Add(monitoring_environment);


                        if (rc.year != "")
                        {
                            int year = FormatDates.getYear(Convert.ToInt16(rc.year), lang);
                            dt.Add(year);
                        }
                        else
                        {
                            dt.Add("-");
                        }

                        if (rc.result!="")
                        {
                            dt.Add(rc.result);
                        }
                        else
                        {
                            dt.Add("-");
                        }
                        
                       


                        dt.Add(rc.duration_risk_factor_name);
                        string monitoring_results = "-";
                        if (rc.monitoring_results == "comply")
                        {
                            monitoring_results = Resources.Health.comply;
                        }
                        else if (rc.monitoring_results == "not_comply")
                        {
                            monitoring_results = Resources.Health.not_comply;
                        }

                        dt.Add(monitoring_results);




                        var d = from c in dbConnect.healths
                                where c.id == Convert.ToInt32(health_id)
                                select new
                                {
                                    c.report_date,
                                    c.employee_id
                                };

                        string file_risk_factor = "";
                        foreach (var p in d)
                        {
                            string path_folder = p.employee_id + "_" + p.report_date.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB"));
                           
                            if (rc.file_risk_factor != "")
                            {
                                string[] fhealth = rc.file_risk_factor.Split(',');

                                for (int i = 0; i < fhealth.Length; i++)
                                {
                                    if (file_risk_factor != "")
                                    {
                                        file_risk_factor = file_risk_factor + "</br>" + "<a target='_blank' href='upload/health/" + path_folder + "/" + fhealth[i] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";
                                    }
                                    else
                                    {
                                        file_risk_factor = "<a target='_blank' href='upload/health/" + path_folder + "/" + fhealth[i] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                                    }

                                }


                            }


                        }//end foreach

                


                        dt.Add(file_risk_factor);


                        if (pagetype == "view")
                        {
                          
                            dt.Add("");

                        }
                        else
                        {
                   
                            string edit = "<a href='javascript:ShowEditRiskFactor(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";
                            string delete = "<a href='javascript:DeleteRiskFactor(" + rc.id + ");'><i class='fa fa-trash fa-2x'></i></a>";
                            string action = edit + delete;
                            dt.Add(action);
                        }

                        dataJson.Add(dt);
                        count++;

                    }

                }
                else
                {// use session

                    if (Session["risk_factor"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["risk_factor"];
                        int count = 1;



                        for (int i = 1; i <= dict.Count(); i++)
                        {
                            RiskFactorEntity rc = (RiskFactorEntity)dict[i];

                            ArrayList dt = new ArrayList();

                            if (rc.Status == "A")
                            {
                                var d = from c in dbConnect.risk_factor_relate_works
                                        where c.id == Convert.ToInt32(rc.risk_factor_relate_work_id)
                                        select new
                                        {
                                            name = chageDataLanguage(c.name_th, c.name_en, lang)
                                        };

                                dt.Add(i);
                                // dt.Add(count);

                                string risk_factor_relate_work = "";
                                foreach (var t in d)
                                {
                                    risk_factor_relate_work = t.name;
                                }

                                dt.Add(risk_factor_relate_work);
                                string monitoring_environment = "-";
                                if (rc.monitoring_environment == "Y")
                                {
                                    monitoring_environment = Resources.Health.lbyes;
                                }
                                else if (rc.monitoring_environment == "N")
                                {
                                    monitoring_environment = Resources.Health.lbno;
                                }
                                dt.Add(monitoring_environment);


                                if (rc.Year != "")
                                {
                                    int year = FormatDates.getYear(Convert.ToInt16(rc.Year), lang);
                                    dt.Add(year);
                                }
                                else
                                {
                                    dt.Add("-");
                                }

                                if (rc.Result != "")
                                {
                                    dt.Add(rc.Result);
                                }
                                else
                                {
                                    dt.Add("-");
                                }


                                var du = from c in dbConnect.duration_risk_factors
                                        where c.id == Convert.ToInt32(rc.Duration_risk_factor_id)
                                        select new
                                        {
                                            name = chageDataLanguage(c.name_th, c.name_en, lang)
                                        };

                                string duration_risk_factor_name = "-";
                                foreach (var t in du)
                                {
                                    duration_risk_factor_name = t.name;
                                }

                                dt.Add(duration_risk_factor_name);
                                string monitoring_results = "-";
                                if (rc.monitoring_results == "comply")
                                {
                                    monitoring_results = Resources.Health.comply;
                                }
                                else if (rc.monitoring_results == "not_comply")
                                {
                                    monitoring_results = Resources.Health.not_comply;
                                }

                                dt.Add(monitoring_results);

                               

                                string file_risk_factor = "";

                                string path_folder = folder_name;
                          
                                if (rc.File_risk_factor != "")
                                {
                                    string[] fhealth = rc.File_risk_factor.Split(',');

                                    for (int j = 0; j < fhealth.Length; j++)
                                    {
                                        if (file_risk_factor != "")
                                        {
                                            file_risk_factor = file_risk_factor + "</br>" + "<a target='_blank' href='upload/health/" + path_folder + "/" + fhealth[j] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";
                                        }
                                        else
                                        {
                                            file_risk_factor = "<a target='_blank' href='upload/health/" + path_folder + "/" + fhealth[j] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                                        }

                                    }


                                }



                                dt.Add(file_risk_factor);



                                string edit = "<a href='javascript:ShowEditRiskFactor(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";
                                string delete = "<a href='javascript:DeleteRiskFactor(" + rc.id + ");'><i class='fa fa-trash fa-2x'></i></a>";
                                string action = edit + delete;
                                dt.Add(action);

                                dataJson.Add(dt);

                                count++;

                            }

                          

                        }

                    }




                }


                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }





        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListOccupationalHealth(string health_id,string folder_name, string lang, string pagetype)
        {



            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                ArrayList dataJson = new ArrayList();

                if (pagetype != "add")
                {
                    var v = from c in dbConnect.occupational_health_report_actions
                            join t in dbConnect.occupational_health_reports on c.occupational_health_report_id equals t.id

                            where c.health_id == Convert.ToInt32(health_id) && c.status == "A"
                            orderby c.id descending
                            select new
                            {
                                c.id,
                                occupational_health_reports = chageDataLanguage(t.name_th, t.name_en, lang),
                                c.repeat_health_check,
                                c.file_health_check,
                                c.flie_repeat_health_check,
                                c.abnormal_audiogram,
                                c.hearing_threshold_level,
                                c.chronic_diseases_ear,
                                c.specify_chronic_diseases_ear,
                                c.abnormal_pulmonary_function,
                                c.smoked_cigarettes,
                                c.cigarette_per_day,
                                c.smoked_months,
                                c.smoked_years,
                                c.smoked_cigarettes_other

                         


                            };



                    int count = 1;
                    foreach (var rc in v)
                    {
                        ArrayList dt = new ArrayList();
                        dt.Add(rc.id);
                       // dt.Add(count);
                        dt.Add(rc.occupational_health_reports);

                     
                        string abnormal_audiogram = "";
                        if (rc.abnormal_audiogram == "left")
                        {
                            abnormal_audiogram = Resources.Health.lbleft_ear;
                        }
                        else if (rc.abnormal_audiogram == "right")
                        {
                            abnormal_audiogram = Resources.Health.lbright_ear;
                        }
                        else if (rc.abnormal_audiogram == "both")
                        {
                            abnormal_audiogram = Resources.Health.lbboth_ear;
                        }

                        dt.Add(abnormal_audiogram);
                        string abnormal_pulmonary_function = "";
                        if (rc.abnormal_pulmonary_function == "obstructive")
                        {
                            abnormal_pulmonary_function = Resources.Health.lbobstructive;
                        }
                        else if (rc.abnormal_pulmonary_function == "restrictive")
                        {
                            abnormal_pulmonary_function = Resources.Health.lbrestrictive;
                        }
                        else if (rc.abnormal_pulmonary_function == "obstructive_restrictive")
                        {
                            abnormal_pulmonary_function = Resources.Health.lbobstructive_restrictive;
                        }
                        dt.Add(abnormal_pulmonary_function);


                        dt.Add(rc.hearing_threshold_level);






                        string file_health_check = "";
                        string flie_repeat_health_check = "";
              
                        var d = from c in dbConnect.healths
                                where c.id == Convert.ToInt32(health_id)
                                select new
                                {
                                    c.report_date,
                                    c.employee_id
                                };

                        foreach (var p in d)
                        {
                            string path_folder = p.employee_id + "_" + p.report_date.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB"));

                            
                            if (rc.file_health_check != "")
                            {
                                string[] fhealth = rc.file_health_check.Split(',');

                                for (int i = 0; i < fhealth.Length; i++)
                                {
                                    if (file_health_check != "")
                                    {
                                        file_health_check = file_health_check + "</br>" + "<a target='_blank' href='upload/health/" + path_folder + "/" + fhealth[i] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";
                                    }
                                    else
                                    {
                                        file_health_check = "<a target='_blank' href='upload/health/" + path_folder + "/" + fhealth[i] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                                    }
                                 
                                }

                                  
                            }

                            if (rc.flie_repeat_health_check != "")
                            {
                                string[] frepeat = rc.flie_repeat_health_check.Split(',');

                                for (int i = 0; i < frepeat.Length; i++)
                                {
                                    if (flie_repeat_health_check != "")
                                    {
                                        flie_repeat_health_check = flie_repeat_health_check + "</br>" + "<a target='_blank' href='upload/health/" + path_folder + "/" + frepeat[i] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                                    }
                                    else
                                    {

                                        flie_repeat_health_check = "<a target='_blank' href='upload/health/" + path_folder + "/" + frepeat[i] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                                    }
                                }//endfor
                              
                            }


                        }

                        dt.Add(file_health_check);

                        string health_check_yes = "";
                        string health_check_no = "";
                        if (rc.repeat_health_check == "Y")
                        {
                            health_check_yes = "<i class='fa fa-check fa-2x'></i>";
                        }
                        

                        if (rc.repeat_health_check == "N")
                        {
                            health_check_no = "<i class='fa fa-check fa-2x'></i>";
                        }


                        dt.Add(health_check_yes);
                        dt.Add(health_check_no);

                        dt.Add(flie_repeat_health_check);

                        string chronic_diseases_ear = "";
                        if(rc.chronic_diseases_ear=="N")
                        {
                            chronic_diseases_ear = Resources.Health.lbno;
                            
                        }
                        else if (rc.chronic_diseases_ear == "Y")
                        {
                            chronic_diseases_ear = Resources.Health.lbyes + " : "+ rc.specify_chronic_diseases_ear;
                        }

                        dt.Add(chronic_diseases_ear);


                        string smoked_cigarettes = "";
                        if (rc.smoked_cigarettes == "NO")
                        {
                            smoked_cigarettes = Resources.Health.lbno;

                        }
                        else if (rc.smoked_cigarettes == "YES_SMOKING")
                        {
                            smoked_cigarettes = Resources.Health.lbyes_smoking + " "+ rc.cigarette_per_day + " " +Resources.Health.lbcigarettes_per_day;
                        }
                        else if (rc.smoked_cigarettes == "YES_SMOKED")
                        {
                            smoked_cigarettes = Resources.Health.lbyes_smoked + " " + rc.smoked_years + " " + Resources.Health.lbyears+" "+rc.smoked_months +" "+Resources.Health.lbmonths;
                        }
                        else if (rc.smoked_cigarettes == "SMOKED_OTHER")
                        {
                            smoked_cigarettes = Resources.Health.lbsmoked_other + " : " + rc.smoked_cigarettes_other;
                        }

                        dt.Add(smoked_cigarettes);




                        if (pagetype == "view")
                        {

                            dt.Add("");

                        }
                        else
                        {

                            string edit = "<a href='javascript:ShowEditOccupationalHealth(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";
                            string delete = "<a href='javascript:DeleteOccupationalHealth(" + rc.id + ");'><i class='fa fa-trash fa-2x'></i></a>";
                            string action = edit + delete;
                            dt.Add(action);
                        }

                        dataJson.Add(dt);
                        count++;

                    }

                }
                else
                {// use session

                    if (Session["occupational_health"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["occupational_health"];
                        int count = 1;

                        

                        for (int i = 1; i <= dict.Count(); i++)
                        {
                            OccupationalHealthEntity rc = (OccupationalHealthEntity)dict[i];
                            ArrayList dt = new ArrayList();
                            if (rc.Status == "A")
                            {
                                var d = from c in dbConnect.occupational_health_reports
                                        where c.id == Convert.ToInt32(rc.occupational_health_report_id)
                                        select new
                                        {
                                            name = chageDataLanguage(c.name_th, c.name_en, lang)
                                        };

                                dt.Add(i);
                                //  dt.Add(count);

                                string occupational_health_report = "";
                                foreach (var t in d)
                                {
                                    occupational_health_report = t.name;
                                }

                                dt.Add(occupational_health_report);

                              

                                string abnormal_audiogram = "";
                                if (rc.abnormal_audiogram == "left")
                                {
                                    abnormal_audiogram = Resources.Health.lbleft_ear;
                                }
                                else if (rc.abnormal_audiogram == "right")
                                {
                                    abnormal_audiogram = Resources.Health.lbright_ear;
                                }
                                else if (rc.abnormal_audiogram == "both")
                                {
                                    abnormal_audiogram = Resources.Health.lbboth_ear;
                                }

                                dt.Add(abnormal_audiogram);

                                string abnormal_pulmonary_function = "";
                                if (rc.abnormal_pulmonary_function == "obstructive")
                                {
                                    abnormal_pulmonary_function = Resources.Health.lbobstructive;
                                }
                                else if (rc.abnormal_pulmonary_function == "restrictive")
                                {
                                    abnormal_pulmonary_function = Resources.Health.lbrestrictive;
                                }
                                else if (rc.abnormal_pulmonary_function == "obstructive_restrictive")
                                {
                                    abnormal_pulmonary_function = Resources.Health.lbobstructive_restrictive;
                                }
                                dt.Add(abnormal_pulmonary_function);

                                dt.Add(rc.hearing_threshold_level);


                                string file_health_check = "";
                                string flie_repeat_health_check = "";


                                string path_folder = folder_name;
                                if (rc.FileHealthCheck!= "")
                                {
                                    string[] fhealth = rc.FileHealthCheck.Split(',');

                                    for (int j = 0; j < fhealth.Length; j++)
                                    {
                                        if (file_health_check != "")
                                        {
                                            file_health_check = file_health_check + "</br>" + "<a target='_blank' href='upload/health/" + path_folder + "/" + fhealth[j] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";
                                        }
                                        else
                                        {
                                            file_health_check = "<a target='_blank' href='upload/health/" + path_folder + "/" + fhealth[j] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                                        }

                                    }


                                }

                                if (rc.FlieRepeatHealthCheck != "")
                                {
                                    string[] frepeat = rc.FlieRepeatHealthCheck.Split(',');

                                    for (int j = 0; j < frepeat.Length; j++)
                                    {
                                        if (flie_repeat_health_check != "")
                                        {
                                            flie_repeat_health_check = flie_repeat_health_check + "</br>" + "<a target='_blank' href='upload/health/" + path_folder + "/" + frepeat[j] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                                        }
                                        else
                                        {

                                            flie_repeat_health_check = "<a target='_blank' href='upload/health/" + path_folder + "/" + frepeat[j] + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                                        }
                                    }//endfor

                                }



                                dt.Add(file_health_check);

                                string health_check_yes = "";
                                string health_check_no = "";
                                if (rc.RepeatHealthCheck == "Y")
                                {
                                    health_check_yes = "<i class='fa fa-check fa-2x'></i>";
                                }


                                if (rc.RepeatHealthCheck == "N")
                                {
                                    health_check_no = "<i class='fa fa-check fa-2x'></i>";
                                }

                                dt.Add(health_check_yes);
                                dt.Add(health_check_no);
                                dt.Add(flie_repeat_health_check);

                                string chronic_diseases_ear = "";
                                if (rc.chronic_diseases_ear == "N")
                                {
                                    chronic_diseases_ear = Resources.Health.lbno;

                                }
                                else if (rc.chronic_diseases_ear == "Y")
                                {
                                    chronic_diseases_ear = Resources.Health.lbyes + " : " + rc.specify_chronic_diseases_ear;
                                }

                                dt.Add(chronic_diseases_ear);

                                string smoked_cigarettes = "";
                                if (rc.smoked_cigarettes == "NO")
                                {
                                    smoked_cigarettes = Resources.Health.lbno;

                                }
                                else if (rc.smoked_cigarettes == "YES_SMOKING")
                                {
                                    smoked_cigarettes = Resources.Health.lbyes_smoking + " " + rc.cigarette_per_day + " " + Resources.Health.lbcigarettes_per_day;
                                }
                                else if (rc.smoked_cigarettes == "YES_SMOKED")
                                {
                                    smoked_cigarettes = Resources.Health.lbyes_smoked + " " + rc.smoked_years + " " + Resources.Health.lbyears + " " + rc.smoked_months + " " + Resources.Health.lbmonths;
                                }
                                else if (rc.smoked_cigarettes == "SMOKED_OTHER")
                                {
                                    smoked_cigarettes = Resources.Health.lbsmoked_other + " " + rc.smoked_cigarettes_other;
                                }

                                dt.Add(smoked_cigarettes);

                                string edit = "<a href='javascript:ShowEditOccupationalHealth(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";
                                string delete = "<a href='javascript:DeleteOccupationalHealth(" + rc.id + ");'><i class='fa fa-trash fa-2x'></i></a>";
                                string action = edit + delete;
                                dt.Add(action);

                                dataJson.Add(dt);

                                count++;


                            }
                           

                            

                        }

                    }




                }


                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }











        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListProcessActionSot(string sot_id, string lang, string pagetype)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                ArrayList dataJson = new ArrayList();

                if (pagetype != "add")
                {
                    var v = from c in dbConnect.process_action_sots
                            join s in dbConnect.sot_action_status on c.action_status_id equals s.id
                            join t in dbConnect.type_controls on c.type_control equals t.id
                            // from r in joinR.DefaultIfEmpty()
                            where c.sot_id == Convert.ToInt32(sot_id) //&& r.status == "A"
                            orderby c.id descending
                            select new
                            {
                                c.id,
                                type_control = chageDataLanguage(t.name_th, t.name_en, lang),
                                c.action,
                                c.responsible_person,
                                due_date = c.due_date,
                                status = chageDataLanguage(s.name_th, s.name_en, lang),
                                date_complete = c.date_complete,
                                c.attachment_file,
                                c.notify_contractor,
                                c.remark,
                                c.action_status_id,
                                c.root_cause_action,
                                due_date2 = c.due_date

                            };


                   
                    int count = 1;
                    foreach (var rc in v)
                    {
                        ArrayList dt = new ArrayList();
                        dt.Add(rc.id);
                        dt.Add(count);
                        dt.Add(rc.type_control);
                        dt.Add(rc.action);
                        dt.Add(rc.responsible_person);
                        if (rc.due_date!=null)
                        {
                            string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date, CultureInfo.InvariantCulture), lang);
                            dt.Add(duedate);
                        }
                        else
                        {
                            dt.Add("");
                        }


                        string status = "";
                        if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                        {
                            status = rc.status;
                            if (rc.date_complete!=null)
                            {
                                if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                                {
                                    status = chageDataLanguage("ล่าช้า", "delay", lang);
                                }
                               
                            }
                            else
                            {
                                if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                                {
                                    status = chageDataLanguage("ล่าช้า", "delay", lang);
                                }

                            }
                        }
                        else
                        {
                            status = rc.status;

                        }

                        dt.Add(status);

                        if (rc.date_complete!=null)
                        {
                            string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                            dt.Add(date);
                        }
                        else
                        {
                            dt.Add("");
                        }


                        string path_file = "";
                        string country = Session["country"].ToString();
                        var d = from c in dbConnect.sots
                                where c.id == Convert.ToInt32(sot_id)
                                select new
                                {
                                    c.doc_no
                                };

                        foreach (var p in d)
                        {
                            if (rc.attachment_file != "")
                            {
                                path_file = "<a target='_blank' href='upload/sot/" + country + "/action/" + p.doc_no + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูหลักฐานแนบ", "View Evidence", lang) + "</a> ";

                            }

                        }

                        dt.Add(path_file);
                        dt.Add(rc.notify_contractor);



                        if (pagetype == "view")
                        {
                            dt.Add("");
                            dt.Add(rc.remark);
                            dt.Add("");

                        }
                        else
                        {
                            if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                            {
                                string div_start = "<div class='form-inline'>";
                                string action_close = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return closeAction(" + rc.id + ")' >" + chageDataLanguage("ปิด", "Close", lang) + "</button>";
                                string action_reject = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return rejectAction(" + rc.id + ")' >" + chageDataLanguage("ปฏิเสธ", "Reject", lang) + "</button>";
                                string action_cancel = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return cancelAction(" + rc.id + ")' >" + chageDataLanguage("ยกเลิก", "Cancel", lang) + "</button>";
                                string div_end = "</div>";

                                string action = div_start;

                                if (Session["permission"] != null)
                                {
                                    ArrayList per = Session["permission"] as ArrayList;
                                    if (per.IndexOf("report sot action close") > -1)
                                    {
                                        action = action + action_close;
                                    }
                                }

                                if (Session["permission"] != null)
                                {
                                    ArrayList per = Session["permission"] as ArrayList;
                                    if (per.IndexOf("report sot action reject") > -1)
                                    {
                                        action = action + action_reject;
                                    }
                                }


                                if (Session["permission"] != null)
                                {
                                    ArrayList per = Session["permission"] as ArrayList;
                                    if (per.IndexOf("report sot action cancel") > -1)
                                    {
                                        action = action + action_cancel;
                                    }
                                }

                                action = action + div_start + div_end;
                                dt.Add(action);

                            }
                            else if (rc.action_status_id == 4)//close
                            {
                                string div_start = "<div class='form-inline'>";

                                string action_reject = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return rejectAction(" + rc.id + ")' >" + chageDataLanguage("ปฏิเสธ", "Reject", lang) + "</button>";
                                string action_cancel = " <button style='padding-right:10px;' class='btn btn-sm btn-primary' onclick='return cancelAction(" + rc.id + ")' >" + chageDataLanguage("ยกเลิก", "Cancel", lang) + "</button>";
                                string div_end = "</div>";

                                string action = div_start;

                                if (Session["permission"] != null)
                                {
                                    ArrayList per = Session["permission"] as ArrayList;
                                    if (per.IndexOf("report sot action reject") > -1)
                                    {
                                        action = action + action_reject;
                                    }
                                }


                                if (Session["permission"] != null)
                                {
                                    ArrayList per = Session["permission"] as ArrayList;
                                    if (per.IndexOf("report sot action cancel") > -1)
                                    {
                                        action = action + action_cancel;
                                    }
                                }

                                action = action + div_start + div_end;
                                dt.Add(action);

                            }
                            else
                            {
                                dt.Add("");
                            }
                            string edit = "<a href='javascript:ShowEditProcessAction(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";

                            dt.Add(rc.remark);
                            dt.Add(edit);
                        }

                        dataJson.Add(dt);
                        count++;

                    }

                }
                else
                {// use session

                    if (Session["process_action_sot"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["process_action_sot"];
                        int count = 1;



                        for (int i = 1; i <= dict.Count(); i++)
                        {
                            ProcessActionSotEntity rc = (ProcessActionSotEntity)dict[i];

                            ArrayList dt = new ArrayList();

                            var d = from c in dbConnect.type_controls
                                    where c.id == Convert.ToInt32(rc.TypeControl)
                                    select new
                                    {
                                        name = chageDataLanguage(c.name_th,c.name_en,lang)
                                    };

                            dt.Add(i);
                            dt.Add(count);

                            string type_control = "";
                            foreach (var t in d)
                            {
                                type_control = t.name;
                            }

                            dt.Add(type_control);
                            dt.Add(rc.Action);
                            dt.Add(rc.ResponsiblePerson);
                            dt.Add(rc.DueDate);


                            string status = "";
                            if (rc.Action_status_id != 5 && rc.Action_status_id != 4)//cancel,close
                            {
                                status = chageDataLanguage("ดำเนินการ", "On process", lang);
                                if (rc.DateComplete == null || rc.DateComplete == "")
                                {

                                    if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(FormatDates.changeDateTimeDB(rc.DueDate, Session["lang"].ToString()), CultureInfo.InvariantCulture).Date)
                                    {
                                        status = chageDataLanguage("ล่าช้า", "delay", lang);
                                    }
                                }
                                else
                                {
                                    if (Convert.ToDateTime(FormatDates.changeDateTimeDB(rc.DateComplete, Session["lang"].ToString()), CultureInfo.InvariantCulture).Date > Convert.ToDateTime(FormatDates.changeDateTimeDB(rc.DueDate, Session["lang"].ToString()), CultureInfo.InvariantCulture).Date)
                                    {
                                        status = chageDataLanguage("ล่าช้า", "delay", lang);
                                    }

                                }
                            }
                            else
                            {
                                status = status = chageDataLanguage("ดำเนินการ", "On process", lang);

                            }
                            dt.Add(status);

                            if (rc.DateComplete!= null && rc.DateComplete!="")
                            {
                                string date = rc.DateComplete;
                                dt.Add(date);
                            }
                            else
                            {
                                dt.Add("");
                            }


                            string path_file = "";


                            dt.Add(path_file);
                            dt.Add(rc.NotifyContractor);

                               
                            string action = "";
                            dt.Add(action);


                            string edit = "<a href='javascript:ShowEditProcessAction(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";

                            dt.Add(rc.Remark);
                            dt.Add(edit);
                            

                            dataJson.Add(dt);

                            count++;

                        }

                    }
                 
                    
                    
                    
                }
               

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }

        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListRootCauseAction(string incident_id, string lang, string pagetype)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.root_cause_actions
                        where c.incident_id == Convert.ToInt32(incident_id) && c.status == "A"
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.name,
                            c.root_cause_number


                        };


                ArrayList dataJson = new ArrayList();
                int count = 1;
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.root_cause_number);
                    dt.Add(count);
                    dt.Add(rc.name);

                    if (pagetype == "view")
                    {
                        dt.Add("");

                    }
                    else
                    {
                        string edit = "<a href='javascript:ShowEditRootCauseAction(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";
                        string delete = "<a href='javascript:DeleteRootCauseAction(" + rc.id + ");'><i class='fa fa-trash fa-2x'></i></a>";
                        string action = edit + delete;
                        dt.Add(action);
                    }

                    dataJson.Add(dt);
                    count++;

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }


        [WebMethod(EnableSession=true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListMyActionIncident(string company_id,
                                            string function_id,
                                            string department_id, 
                                            string employee_id,
                                            string doc_no,
                                            string lang
                                            )
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        where c.employee_id == employee_id.Trim() && (c.action_status_id == 1
                        || c.action_status_id == 6)//onprocess and reject
                        && i.process_status == 1
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.corrective_preventive_action,
                            c.responsible_person,
                            due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date.ToString(), CultureInfo.InvariantCulture), lang),
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete.ToString(),
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.incident_id,
                            due_date2 = c.due_date,
                            i.company_id,
                            i.function_id,
                            i.department_id,
                            i.doc_no

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

                if (doc_no != "")
                {
                    v = v.Where(c => c.doc_no.Contains(doc_no));
                }

                ArrayList dataJson = new ArrayList();
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(rc.action_status_id);
                    dt.Add(rc.incident_id);



                    string path_file = "";
                    string document_no = "";
                    string country = Session["country"].ToString();
                    var d = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(rc.incident_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        document_no = p.doc_no;
                        if (rc.attachment_file != "" & rc.attachment_file != null)
                        {
                            path_file = "<a href='upload/incident/step3/" + country + "/" + document_no + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูไฟล์แนบ", "View", lang) + "</a> ";

                        }
                        else
                        {
                            path_file = "<a href='javascript:showAttachfile(" + rc.id + "," + rc.incident_id + ",\"corrective\")'>" + chageDataLanguageEvidence("แนบไฟล์", "Attach", lang) + "</a> ";

                        }

                    }

                    dt.Add(document_no);
                    dt.Add(rc.corrective_preventive_action);
                    dt.Add(rc.due_date);

                    string code_status = "";
                    string status = "";
                    if (rc.action_status_id == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }


                    }
                    //else if (rc.action_status_id == 2)
                    //{//delay

                    //    code_status = "<i class=\"fa fa-circle text-danger\"></i>";

                    //}

                    else if (rc.action_status_id == 4)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i>";
                        status = rc.status;

                    }
                    else if (rc.action_status_id == 5)//cancel
                    {
                        code_status = "<i class=\"fa fa-circle text-navy\"></i>";
                        status = rc.status;

                    }
                    else if (rc.action_status_id == 6)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else if (rc.action_status_id == 2)
                    {//request close

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }


                    dt.Add(code_status + " " + status);

                    if (!string.IsNullOrEmpty(rc.date_complete))
                    {
                        string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                        dt.Add(date);
                    }
                    else
                    {
                        dt.Add("");
                    }



                    dt.Add(path_file);
                    dt.Add(rc.notify_contractor);

                    if (rc.action_status_id != 5 && rc.action_status_id != 4 && rc.action_status_id != 2)//cancel ,close and request close
                    {
                        string action_close = " <button class='btn btn-sm btn-primary' onclick='return closeAction(" + rc.id + ")' >" + chageDataLanguage("ส่งข้อมูล", "Submit", lang) + "</button>";

                        string action = action_close;
                        dt.Add(action);
                    }
                    else
                    {
                        dt.Add("");

                    }


                    dt.Add(rc.remark);

                    string view = "<a href='javascript:redirectincident(" + rc.incident_id + ");'><i class='fa fa-file-text-o fa-2x'></i></a> ";
                    dt.Add(view);

                    dataJson.Add(dt);


                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }



        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListMyActionPreventiveIncident(string company_id,
                                                      string function_id,
                                                      string department_id,
                                                      string employee_id,
                                                      string doc_no,
                                                      string lang
                                                    )
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.preventive_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        where c.employee_id == employee_id.Trim() && (c.action_status_id == 1
                        || c.action_status_id == 6)//onprocess and reject
                        && i.process_status == 1
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.preventive_action,
                            c.responsible_person,
                            due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date.ToString(), CultureInfo.InvariantCulture), lang),
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete.ToString(),
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.incident_id,
                            due_date2 = c.due_date,
                            i.company_id,
                            i.function_id,
                            i.department_id,
                            i.doc_no

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

                if (doc_no != "")
                {
                    v = v.Where(c => c.doc_no.Contains(doc_no));
                }

                ArrayList dataJson = new ArrayList();
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(rc.action_status_id);
                    dt.Add(rc.incident_id);



                    string path_file = "";
                    string document_no = "";
                    string country = Session["country"].ToString();
                    var d = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(rc.incident_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        document_no = p.doc_no;
                        if (rc.attachment_file != "" & rc.attachment_file != null)
                        {
                            path_file = "<a href='upload/incident/step3/" + country + "/" + document_no + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูไฟล์แนบ", "View", lang) + "</a> ";

                        }
                        else
                        {
                            path_file = "<a href='javascript:showAttachfile(" + rc.id + "," + rc.incident_id + ",\"preventive\")'>" + chageDataLanguageEvidence("แนบไฟล์", "Attach", lang) + "</a> ";

                        }

                    }

                    dt.Add(document_no);
                    dt.Add(rc.preventive_action);
                    dt.Add(rc.due_date);

                    string code_status = "";
                    string status = "";
                    if (rc.action_status_id == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }


                    }
                    //else if (rc.action_status_id == 2)
                    //{//delay

                    //    code_status = "<i class=\"fa fa-circle text-danger\"></i>";

                    //}

                    else if (rc.action_status_id == 4)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i>";
                        status = rc.status;

                    }
                    else if (rc.action_status_id == 5)//cancel
                    {
                        code_status = "<i class=\"fa fa-circle text-navy\"></i>";
                        status = rc.status;

                    }
                    else if (rc.action_status_id == 6)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else if (rc.action_status_id == 2)
                    {//request close

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }


                    dt.Add(code_status + " " + status);

                    if (!string.IsNullOrEmpty(rc.date_complete))
                    {
                        string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                        dt.Add(date);
                    }
                    else
                    {
                        dt.Add("");
                    }



                    dt.Add(path_file);
                    dt.Add(rc.notify_contractor);

                    if (rc.action_status_id != 5 && rc.action_status_id != 4 && rc.action_status_id != 2)//cancel ,close and request close
                    {
                        string action_close = " <button class='btn btn-sm btn-primary' onclick='return closeAction(" + rc.id + ")' >" + chageDataLanguage("ส่งข้อมูล", "Submit", lang) + "</button>";

                        string action = action_close;
                        dt.Add(action);
                    }
                    else
                    {
                        dt.Add("");

                    }


                    dt.Add(rc.remark);

                    string view = "<a href='javascript:redirectincident(" + rc.incident_id + ");'><i class='fa fa-file-text-o fa-2x'></i></a> ";
                    dt.Add(view);

                    dataJson.Add(dt);


                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }





        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListMyActionConsequenceIncident(string company_id,
                                                      string function_id,
                                                      string department_id,
                                                      string employee_id,
                                                      string doc_no,
                                                      string lang
                                                    )
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.consequence_management_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        where c.employee_id == employee_id.Trim() && (c.action_status_id == 1
                        || c.action_status_id == 6)//onprocess and reject
                        && i.process_status == 1
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.consequence_management,
                            c.responsible_person,
                            due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date.ToString(), CultureInfo.InvariantCulture), lang),
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete.ToString(),
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.incident_id,
                            due_date2 = c.due_date,
                            i.company_id,
                            i.function_id,
                            i.department_id,
                            i.doc_no

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

                if (doc_no != "")
                {
                    v = v.Where(c => c.doc_no.Contains(doc_no));
                }

                ArrayList dataJson = new ArrayList();
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(rc.action_status_id);
                    dt.Add(rc.incident_id);



                    string path_file = "";
                    string document_no = "";
                    string country = Session["country"].ToString();
                    var d = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(rc.incident_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        document_no = p.doc_no;
                        if (rc.attachment_file != "" & rc.attachment_file != null)
                        {
                            path_file = "<a href='upload/incident/step3/" + country + "/" + document_no + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูไฟล์แนบ", "View", lang) + "</a> ";

                        }
                        else
                        {
                            path_file = "<a href='javascript:showAttachfile(" + rc.id + "," + rc.incident_id + ",\"consequence\")'>" + chageDataLanguageEvidence("แนบไฟล์", "Attach", lang) + "</a> ";

                        }

                    }

                    dt.Add(document_no);
                    dt.Add(rc.consequence_management);
                    dt.Add(rc.due_date);

                    string code_status = "";
                    string status = "";
                    if (rc.action_status_id == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }


                    }
                    //else if (rc.action_status_id == 2)
                    //{//delay

                    //    code_status = "<i class=\"fa fa-circle text-danger\"></i>";

                    //}

                    else if (rc.action_status_id == 4)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i>";
                        status = rc.status;

                    }
                    else if (rc.action_status_id == 5)//cancel
                    {
                        code_status = "<i class=\"fa fa-circle text-navy\"></i>";
                        status = rc.status;

                    }
                    else if (rc.action_status_id == 6)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else if (rc.action_status_id == 2)
                    {//request close

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }


                    dt.Add(code_status + " " + status);

                    if (!string.IsNullOrEmpty(rc.date_complete))
                    {
                        string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                        dt.Add(date);
                    }
                    else
                    {
                        dt.Add("");
                    }



                    dt.Add(path_file);
                    dt.Add(rc.notify_contractor);

                    if (rc.action_status_id != 5 && rc.action_status_id != 4 && rc.action_status_id != 2)//cancel ,close and request close
                    {
                        string action_close = " <button class='btn btn-sm btn-primary' onclick='return closeAction(" + rc.id + ")' >" + chageDataLanguage("ส่งข้อมูล", "Submit", lang) + "</button>";

                        string action = action_close;
                        dt.Add(action);
                    }
                    else
                    {
                        dt.Add("");

                    }


                    dt.Add(rc.remark);

                    string view = "<a href='javascript:redirectincident(" + rc.incident_id + ");'><i class='fa fa-file-text-o fa-2x'></i></a> ";
                    dt.Add(view);

                    dataJson.Add(dt);


                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }


        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]

        public void getAllWorkInMyActionIncident(string employee_id,
                                                 string company_id,
                                                 string function_id,
                                                 string department_id,
                                                 string doc_no,
                                                 string lang)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from d in dbConnect.incidents
                        join s in dbConnect.incident_status on d.process_status equals s.id
                        where d.process_status != 3 && d.process_status != 2 && d.process_status != 4
                        && d.country == Session["country"].ToString()
                        orderby d.report_date ascending

                        select new
                        {
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
                            d.responsible_area,
                            d.owner_activity

                        };


               

                if (company_id != "")
                {
                    v = v.Where(c => (c.owner_activity=="KNOWN"?c.activity_company_id==company_id.Trim():c.company_id==company_id.Trim()));

                }

                if (function_id != "")
                {
                    v = v.Where(c => (c.owner_activity == "KNOWN" ? c.activity_function_id == function_id.Trim() : c.function_id == function_id.Trim()));

                }

                if (department_id != "")
                {
                    v = v.Where(c => (c.owner_activity == "KNOWN" ? c.activity_department_id == department_id.Trim() : c.department_id == department_id.Trim()));

                }

                if (doc_no != "")
                {
                    v = v.Where(c => c.doc_no.Contains(doc_no));
                }


        
            

                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    string incident_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.incident_date, CultureInfo.InvariantCulture), lang);
                    dt.Add(rc.id);
                    string doc_no_link = "<a style='color:blue;' target='_blank' href='incidentform.aspx?pagetype=view&id=" + rc.id + "'>" + rc.doc_no + "</a> ";
                    dt.Add(doc_no_link);
                    dt.Add(rc.incident_name);
                    dt.Add(rc.incident_detail);
                    dt.Add(incident_date);

                    string code_status = "";
                    if (rc.process_status == 1)//on process
                    {
                        code_status = "<i class=\"fa fa-circle text-warning\"></i> ";

                    }



                    string step = "";


                    if (rc.step_form == 1)//supervisor
                    {
                        string v_step = chageDataLanguage("รายงานอุบัติการณ์", "Incident Report", lang);
                        step = step + "(" + v_step + " - Area Supervisor)";


                        var st1 = from d in dbConnect.employee_has_sections
                                  where d.employee_id == employee_id //&& d.section_id == rc.section_id
                                   && d.country == Session["country"].ToString()
                                  select new
                                  {
                                      d.id,
                                      d.section_id

                                  };


                        if (rc.owner_activity == "KNOWN")
                        {
                            st1 = st1.Where(c => c.section_id == rc.activity_section_id);
                        }else{
                            st1 = st1.Where(c => c.section_id == rc.section_id);
                        }

                        

                        if (st1.Count() > 0)
                        {
                            string edit = "<a href='incidentform.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                            dt.Add(code_status + rc.status + " " + step);
                            dt.Add(edit);

                            dataJson.Add(dt);

                        }



                    }
                    else if (rc.step_form == 2)
                    {
                        string v_step = chageDataLanguage("ตรวจสอบรายงานอุบัติการณ์", "Vetify Incident Report", lang);

                        if (rc.submit_report_form2 == null && rc.confirm_investigate_form2==null)
                        {
                            step = step + "(" + v_step + " - Area Supervisor)";

                                var st2 = from d in dbConnect.employee_has_sections
                                          where d.employee_id == employee_id //&& d.section_id == rc.section_id
                                           && d.country == Session["country"].ToString()
                                          select new
                                          {
                                              d.id,
                                              d.section_id

                                          };

                                if (rc.owner_activity == "KNOWN")
                                {
                                    st2 = st2.Where(c => c.section_id == rc.activity_section_id);
                                }
                                else
                                {
                                    st2 = st2.Where(c => c.section_id == rc.section_id);
                                }

                                if (st2.Count() > 0)
                                {
                                    string edit = "<a href='incidentform2.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                    dt.Add(code_status + rc.status + " " + step);
                                    dt.Add(edit);

                                    dataJson.Add(dt);

                                }
                           
                        }



                        if (Session["country"].ToString() == "thailand")
                        {
                            if (rc.submit_report_form2 != null && rc.confirm_investigate_form2 == null)
                            {
                                step = step + "(" + v_step + " - Area OH&S)";


                                var st2 = from d in dbConnect.employee_has_departments
                                          where d.employee_id == employee_id //&& d.department_id == rc.department_id
                                           && d.country == Session["country"].ToString()
                                          select new
                                          {
                                              d.id,
                                              d.department_id

                                          };

                                if (rc.owner_activity == "KNOWN")
                                {
                                    st2 = st2.Where(c => c.department_id == rc.activity_department_id);
                                }
                                else
                                {
                                    st2 = st2.Where(c => c.department_id == rc.department_id);
                                }

                                if (st2.Count() > 0)
                                {
                                    string edit = "<a href='incidentform2.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                    dt.Add(code_status + rc.status + " " + step);
                                    dt.Add(edit);

                                    dataJson.Add(dt);

                                }

                            }
                        }
                        else if (Session["country"].ToString() == "srilanka")
                        {
                            if (rc.submit_report_form2 != null && rc.confirm_investigate_form2 == null)
                            {
                                step = step + "(" + v_step + " - Area Manager)";

                                var st2 = from d in dbConnect.employee_has_divisions
                                          where d.employee_id == employee_id && d.division_id == rc.division_id
                                           && d.country == Session["country"].ToString()
                                          select new
                                          {
                                              d.id

                                          };

                                if (st2.Count() > 0)
                                {
                                    string edit = "<a href='incidentform2.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                    dt.Add(code_status + rc.status + " " + step);
                                    dt.Add(edit);

                                    dataJson.Add(dt);

                                }
                            }

                        }

                    

                        if (rc.confirm_investigate_form2 != null)//กด confirm ไปละ แต่ยังอยู่ 2 แสดงว่ามีซีเรียสเคสรอ group มากด confirm
                        {
                            step = step + "(" + v_step + " - Group OH&S)";

                            var st2 = from d in dbConnect.employee_has_groups
                                      where d.employee_id == employee_id && d.group_id == 8//8 is group
                                       && d.country == Session["country"].ToString()
                                      select new
                                      {
                                          d.id

                                      };


                            if (st2.Count() > 0)
                            {
                                string edit = "<a href='incidentform2.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                dt.Add(code_status + rc.status + " " + step);
                                dt.Add(edit);

                                dataJson.Add(dt);

                            }


                        }


                    }
                    else if (rc.step_form == 3)
                    {
                        string v_step = chageDataLanguage("สอบสวนและกำหนดมาตรการการแก้ไข", "Investigation and Corrective/Preventive Action", lang);


                        if (Session["country"].ToString() == "thailand")
                        {
                            step = step + "(" + v_step + " - Area OH&S)";


                            var st2 = from d in dbConnect.employee_has_departments
                                      where d.employee_id == employee_id //&& d.department_id == rc.department_id
                                       && d.country == Session["country"].ToString()
                                      select new
                                      {
                                          d.id,
                                          d.department_id

                                      };

                            if (rc.owner_activity == "KNOWN")
                            {
                                st2 = st2.Where(c => c.department_id == rc.activity_department_id);
                            }
                            else
                            {
                                st2 = st2.Where(c => c.department_id == rc.department_id);
                            }

                            if (st2.Count() > 0)
                            {
                                string edit = "<a href='incidentform3.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                dt.Add(code_status + rc.status + " " + step);
                                dt.Add(edit);

                                dataJson.Add(dt);

                            }



                        }
                        else if (Session["country"].ToString() == "srilanka")
                        {
                            step = step + "(" + v_step + " - Area Manager)";


                            var st2 = from d in dbConnect.employee_has_divisions
                                      where d.employee_id == employee_id && d.division_id == rc.division_id
                                      && d.country == Session["country"].ToString()
                                      select new
                                      {
                                          d.id

                                      };

                            if (st2.Count() > 0)
                            {
                                string edit = "<a href='incidentform3.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                dt.Add(code_status + rc.status + " " + step);
                                dt.Add(edit);

                                dataJson.Add(dt);

                            }
                        }


                    }
                    else if (rc.step_form == 4)
                    {
                        string v_step = chageDataLanguage("ขอปิดรายงานอุบัติการณ์", "Request to Close Incident Report", lang);
                        string edit = "";

                        bool check_close = true;

                        var s = from c in dbConnect.close_step_incidents
                                where c.country == Session["country"].ToString()
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


                                    var st3 = from d in dbConnect.employee_has_groups
                                              where d.employee_id == employee_id 
                                             // && d.function_id == rc.function_id
                                              && (d.group_id == 4 || d.group_id == 5)
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id,
                                                  d.function_id

                                              };

                                    if (rc.owner_activity == "KNOWN")
                                    {
                                        st3 = st3.Where(c => c.function_id == rc.activity_function_id);
                                    }
                                    else
                                    {
                                        st3 = st3.Where(c => c.function_id == rc.function_id);
                                    }

                                    if (st3.Count() > 0)
                                    {
                                        edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";
                                        

                                    }else{
                                        edit = "";
                                    }

                                }

                                if (r.group_id == 8)
                                {
                                    step = "(" + v_step + " - Group OH&S)";

                                    var st3 = from d in dbConnect.employee_has_groups
                                              where d.employee_id == employee_id && (d.group_id == 8)//8 is group
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id

                                              };

                                    if (st3.Count() > 0)
                                    {
                                        edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";

                                    }
                                    else
                                    {
                                        edit = "";
                                    }
                                }


                                if (r.group_id == 9)
                                {
                                    step = "(" + v_step + " - Area OH&S)";

                                    var st3 = from d in dbConnect.employee_has_departments
                                              where d.employee_id == employee_id //&& d.department_id == rc.department_id
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id,
                                                  d.department_id

                                              };

                                    if (rc.owner_activity == "KNOWN")
                                    {
                                        st3 = st3.Where(c => c.department_id== rc.activity_department_id);
                                    }
                                    else
                                    {
                                        st3 = st3.Where(c => c.department_id == rc.department_id);
                                    }

                                    if (st3.Count() > 0)
                                    {
                                        edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";


                                    }
                                    else
                                    {
                                        edit = "";
                                    }
                                }


                                if (r.group_id == 10)// areamanage
                                {
                                    step = "(" + v_step + " - Area Manager)";

                                    var st3 = from d in dbConnect.employee_has_divisions
                                              where d.employee_id == employee_id// && d.division_id == rc.division_id
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id,
                                                  d.division_id

                                              };


                                    if (rc.owner_activity == "KNOWN")
                                    {
                                        st3 = st3.Where(c => c.division_id == rc.activity_division_id);
                                    }
                                    else
                                    {
                                        st3 = st3.Where(c => c.division_id == rc.division_id);
                                    }

                                    if (st3.Count() > 0)
                                    {
                                        edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";


                                    }
                                    else
                                    {
                                        edit = "";
                                    }
                                }

                                if (r.group_id == 11)
                                {
                                    step = "(" + v_step + " - Area Supervisor)";

                                    var st3 = from d in dbConnect.employee_has_sections
                                              where d.employee_id == employee_id //&& d.section_id == rc.section_id
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id,
                                                  d.section_id

                                              };

                                    if (rc.owner_activity == "KNOWN")
                                    {
                                        st3 = st3.Where(c => c.section_id == rc.activity_section_id);
                                    }
                                    else
                                    {
                                        st3 = st3.Where(c => c.section_id == rc.section_id);
                                    }



                                    if (st3.Count() > 0)
                                    {
                                       edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";


                                    }
                                    else
                                    {
                                        edit = "";
                                    }

                                }



                            }

                        }//end each


                        if (edit != "")
                        {
                            dt.Add(code_status + rc.status + " " + step);
                            dt.Add(edit);

                            dataJson.Add(dt);


                        }

                     }
                   

                 }


                //int totalRow = dataJson.Count;
                //int lenght = Convert.ToInt32(Context.Request["length"].ToString());
                //int start = Convert.ToInt32(Context.Request["start"].ToString());
                //int draw = Convert.ToInt32(Context.Request["draw"].ToString());




                var result = new
                {
                    //draw = draw,
                    //recordsTotal = totalRow,
                    //recordsFiltered = totalRow,
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));

            }
        }



        [WebMethod(EnableSession=true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListMyActionHazard(string employee_id,
                                          string company_id,
                                          string function_id,
                                          string department_id,
                                          string doc_no,
                                          string lang)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.process_actions
                        join h in dbConnect.hazards on c.hazard_id equals h.id
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        where c.employee_id == employee_id.Trim() && (c.action_status_id == 1
                        || c.action_status_id == 6)//onprocess and reject
                        && h.process_status == 1
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.action,
                            c.responsible_person,
                            due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date.ToString(), CultureInfo.InvariantCulture), lang),
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete.ToString(),
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.hazard_id,
                            due_date2 = c.due_date,
                            h.company_id,
                            h.function_id,
                            h.department_id,
                            h.doc_no

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


                if (doc_no != "")
                {
                    v = v.Where(c => c.doc_no.Contains(doc_no));
                }


                ArrayList dataJson = new ArrayList();
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(rc.action_status_id);
                    dt.Add(rc.hazard_id);



                    string path_file = "";
                    string document_no = "";
                    string country = Session["country"].ToString();
                    var d = from c in dbConnect.hazards
                            where c.id == Convert.ToInt32(rc.hazard_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        document_no = p.doc_no;
                        if (rc.attachment_file != "" & rc.attachment_file != null)
                        {
                            path_file = "<a href='upload/hazard/step3/" + country + "/" + document_no + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูไฟล์แนบ", "View", lang) + "</a> ";

                        }
                        else
                        {
                            path_file = "<a href='javascript:showAttachfile(" + rc.id + "," + rc.hazard_id + ")'>" + chageDataLanguageEvidence("แนบไฟล์", "Attach", lang) + "</a> ";

                        }

                    }

                    dt.Add(document_no);
                    dt.Add(rc.action);
                    dt.Add(rc.due_date);

                    string code_status = "";
                    string status = "";
                    if (rc.action_status_id == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }

                    }
                    //else if (rc.action_status_id == 2)
                    //{//delay

                    //    code_status = "<i class=\"fa fa-circle text-danger\"></i>";

                    //}
                    //else if (rc.action_status_id == 3)
                    //{//complete

                    //    code_status = "<span class='<i class=\"fa fa-circle text-navy\"></i>";
                    //}
                    else if (rc.action_status_id == 4)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i>";
                        status = rc.status;
                    }
                    else if (rc.action_status_id == 5)//cancel
                    {
                        code_status = "<i class=\"fa fa-circle text-navy\"></i>";
                        status = rc.status;
                    }
                    else if (rc.action_status_id == 6)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else if (rc.action_status_id == 2)
                    {//request close

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }


                    dt.Add(code_status + " " + status);

                    if (!string.IsNullOrEmpty(rc.date_complete))
                    {
                        string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                        dt.Add(date);
                    }
                    else
                    {
                        dt.Add("");
                    }



                    dt.Add(path_file);
                    dt.Add(rc.notify_contractor);

                    if (rc.action_status_id != 2 && rc.action_status_id != 5 && rc.action_status_id != 4)//request close,cancel and close
                    {
                        string action_close = " <button class='btn btn-sm btn-primary' onclick='return closeAction(" + rc.id + ")' >" + chageDataLanguage("ส่งข้อมูล", "Submit", lang) + "</button>";

                        string action = action_close;
                        dt.Add(action);
                    }
                    else
                    {
                        dt.Add("");

                    }


                    dt.Add(rc.remark);

                    string view = "<a href='javascript:redirecthazard(" + rc.hazard_id + ");'><i class='fa fa-file-text-o fa-2x'></i></a> ";
                    dt.Add(view);

                    dataJson.Add(dt);


                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }



        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]

        public void getAllWorkInMyActionHazard(string employee_id,
                                               string company_id,
                                               string function_id,
                                               string department_id,
                                               string doc_no,
                                               string lang)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from d in dbConnect.hazards
                        join s in dbConnect.hazard_status on d.process_status equals s.id
                        where d.process_status != 2 && d.process_status != 3
                        && d.country == Session["country"].ToString()
                        orderby d.report_date ascending

                        select new
                        {
                            d.company_id,
                            d.function_id,
                            d.department_id,
                            d.division_id,
                            d.section_id,
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

                if (doc_no != "")
                {
                    v = v.Where(c => c.doc_no.Contains(doc_no));
                }


                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    string hazard_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.hazard_date, CultureInfo.InvariantCulture), lang);
                    dt.Add(rc.id);
                    string doc_no_link = "<a style='color:blue;' target='_blank' href='hazardform.aspx?pagetype=view&id=" + rc.id + "'>" + rc.doc_no + "</a> ";
                    dt.Add(doc_no_link);
                    dt.Add(rc.hazard_name);
                    dt.Add(rc.hazard_detail);
                    dt.Add(hazard_date);

                    string code_status = "";
                    if (rc.process_status == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i> ";

                    }



                    string step = "";


                    if (rc.step_form == 1)//area oh&s
                    {
                        string v_step = chageDataLanguage("รายงานแหล่งอันตราย", "Hazard report", lang);
                        
                        if (Session["country"].ToString() == "thailand")
                        {
                            step = step + "(" + v_step + " - Area OH&S)";

                            var st1 = from d in dbConnect.employee_has_departments
                                      where d.employee_id == employee_id && d.department_id == rc.department_id
                                      && d.country == Session["country"].ToString()
                                      select new
                                      {
                                          d.id

                                      };

                            if (st1.Count() > 0)
                            {
                                string edit = "<a href='hazardform.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                dt.Add(code_status + rc.status + " " + step);
                                dt.Add(edit);

                                dataJson.Add(dt);

                            }

                        }
                        else if (Session["country"].ToString() == "srilanka")
                        {
                            step = step + "(" + v_step + " - Area Supervisor)";

                            var st1 = from d in dbConnect.employee_has_sections
                                      where d.employee_id == employee_id && d.section_id == rc.section_id
                                      && d.country == Session["country"].ToString()
                                      select new
                                      {
                                          d.id

                                      };

                            if (st1.Count() > 0)
                            {
                                string edit = "<a href='hazardform.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                dt.Add(code_status + rc.status + " " + step);
                                dt.Add(edit);

                                dataJson.Add(dt);

                            }
                        }


                    }
                    else if (rc.step_form == 2)
                    {
                        string v_step = chageDataLanguage("ตรวจสอบรายงานแหล่งอันตราย", "Verify Hazard Report", lang);

                        if (rc.submit_report_form2 == null)
                        {

                            if (Session["country"].ToString() == "thailand")
                            {
                                step = step + "(" + v_step + " - Area OH&S)";

                                var st2 = from d in dbConnect.employee_has_departments
                                          where d.employee_id == employee_id && d.department_id == rc.department_id
                                          && d.country == Session["country"].ToString()
                                          select new
                                          {
                                              d.id

                                          };

                                if (st2.Count() > 0)
                                {
                                    string edit = "<a href='hazardform2.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                    dt.Add(code_status + rc.status + " " + step);
                                    dt.Add(edit);

                                    dataJson.Add(dt);

                                }

                            }
                            else if (Session["country"].ToString() == "srilanka")
                            {
                                step = step + "(" + v_step + " - Area Supervisor)";

                                var st2 = from d in dbConnect.employee_has_sections
                                          where d.employee_id == employee_id && d.section_id == rc.section_id
                                          && d.country == Session["country"].ToString()
                                          select new
                                          {
                                              d.id

                                          };

                                if (st2.Count() > 0)
                                {
                                    string edit = "<a href='hazardform2.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                    dt.Add(code_status + rc.status + " " + step);
                                    dt.Add(edit);

                                    dataJson.Add(dt);

                                }

                            }

                        }
                        else
                        {
                            step = step + "(" + v_step + " - Area Supervisor)";

                            var st2 = from d in dbConnect.employee_has_sections
                                      where d.employee_id == employee_id && d.section_id == rc.section_id
                                      && d.country == Session["country"].ToString()
                                      select new
                                      {
                                          d.id

                                      };

                            if (st2.Count() > 0)
                            {
                                string edit = "<a href='hazardform2.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                                dt.Add(code_status + rc.status + " " + step);
                                dt.Add(edit);

                                dataJson.Add(dt);

                            }
                        }


                    }
                    else if (rc.step_form == 3)
                    {
                        string v_step = chageDataLanguage("ดำเนินการแก้ไข", "Process of Action", lang);

                        step = step + "(" + v_step + " - Area Supervisor)";

                        var st3 = from d in dbConnect.employee_has_sections
                                  where d.employee_id == employee_id && d.section_id == rc.section_id
                                  && d.country == Session["country"].ToString()
                                  select new
                                  {
                                      d.id

                                  };

                        if (st3.Count() > 0)
                        {
                            string edit = "<a href='hazardform3.aspx?pagetype=view&id=" + rc.id + "'><i class='fa fa-pencil fa-2x'></i></a> ";
                            dt.Add(code_status + rc.status + " " + step);
                            dt.Add(edit);

                            dataJson.Add(dt);

                        }

                    }
                    else if (rc.step_form == 4)
                    {
                        string v_step = chageDataLanguage("ขอปิดรายงานแหล่งอันตราย", "Request to Close Hazard Report", lang);
                        string edit = "";
                        bool check_close = true;

                        var s = from c in dbConnect.close_step_hazards
                                where c.country == Session["country"].ToString()
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


                                    var st3 = from d in dbConnect.employee_has_groups
                                              where d.employee_id == employee_id && (d.group_id == 4 || d.group_id == 5)
                                              && d.function_id == rc.function_id
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id

                                              };

                                    if (st3.Count() > 0)
                                    {
                                        edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";


                                    }
                                    else
                                    {
                                        edit = "";

                                    }
                                }

                                if (r.group_id == 8)
                                {
                                    step = "(" + v_step + " - Group OH&S)";


                                    var st3 = from d in dbConnect.employee_has_groups
                                              where d.employee_id == employee_id && (d.group_id == 8)
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id

                                              };

                                    if (st3.Count() > 0)
                                    {
                                        edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";


                                    }
                                    else
                                    {
                                        edit = "";

                                    }
                                }


                                if (r.group_id == 9)
                                {
                                    step = "(" + v_step + " - Area OH&S)";

                                    var st3 = from d in dbConnect.employee_has_departments
                                              where d.employee_id == employee_id && d.department_id == rc.department_id
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id

                                              };

                                    if (st3.Count() > 0)
                                    {
                                        edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";


                                    }
                                    else
                                    {
                                        edit = "";

                                    }
                                }


                                if (r.group_id == 10)// areamanage
                                {
                                    step = "(" + v_step + " - Area Manager)";

                                    var st3 = from d in dbConnect.employee_has_divisions
                                              where d.employee_id == employee_id && d.division_id == rc.division_id
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id

                                              };

                                    if (st3.Count() > 0)
                                    {
                                        edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";

                                    }
                                    else
                                    {
                                        edit = "";

                                    }
                                }

                                if (r.group_id == 11)
                                {
                                    step = "(" + v_step + " - Area Supervisor)";

                                    var st3 = from d in dbConnect.employee_has_sections
                                              where d.employee_id == employee_id && d.section_id == rc.section_id
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id

                                              };

                                    if (st3.Count() > 0)
                                    {
                                        edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";


                                    }
                                    else
                                    {
                                        edit = "";

                                    }
                                }


                                if (r.group_id == 16)
                                {
                                    step = "(" + v_step + " -  Group OH&S Hazard)";

                                    var st3 = from d in dbConnect.employee_has_groups
                                              where d.employee_id == employee_id && (d.group_id == 16)//16 is group
                                              && d.country == Session["country"].ToString()
                                              select new
                                              {
                                                  d.id

                                              };

                                    if (st3.Count() > 0)
                                    {
                                         edit = "<a href='javascript:showCloseWork(" + rc.id + ");'><i class='fa fa-pencil fa-2x'></i></a> ";


                                    }
                                    else
                                    {
                                        edit = "";

                                    }
                                }


                            }
                    

                        }//end each


                        if (edit != "")
                        {
                            dt.Add(code_status + rc.status + " " + step);
                            dt.Add(edit);

                            dataJson.Add(dt);


                        }
                     


                    }




                }


                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }
        }

        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListMyActionSot(string employee_id, string lang)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.process_action_sots
                        join s in dbConnect.action_status on c.action_status_id equals s.id
                        where c.employee_id == employee_id.Trim() && (c.action_status_id == 1
                        || c.action_status_id == 6)//onprocess and reject
                        orderby c.id descending
                        select new
                        {
                            c.id,
                            c.action,
                            c.responsible_person,
                            due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date.ToString(), CultureInfo.InvariantCulture), lang),
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            date_complete = c.date_complete.ToString(),
                            c.attachment_file,
                            c.notify_contractor,
                            c.remark,
                            c.action_status_id,
                            c.sot_id,
                            due_date2 = c.due_date

                        };


                ArrayList dataJson = new ArrayList();
                foreach (var rc in v)
                {
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(rc.action_status_id);
                    dt.Add(rc.sot_id);



                    string path_file = "";
                    string doc_no = "";
                    string country = Session["country"].ToString();
                    var d = from c in dbConnect.sots
                            where c.id == Convert.ToInt32(rc.sot_id)
                            select new
                            {
                                c.doc_no
                            };

                    foreach (var p in d)
                    {
                        doc_no = p.doc_no;
                        if (rc.attachment_file != "" & rc.attachment_file != null)
                        {
                            path_file = "<a href='upload/sot/" + country + "/action/" + doc_no + "/" + rc.attachment_file + "'>" + chageDataLanguageEvidence("ดูไฟล์แนบ", "View", lang) + "</a> ";

                        }
                        else
                        {
                            path_file = "<a href='javascript:showAttachfile(" + rc.id + "," + rc.sot_id + ")'>" + chageDataLanguageEvidence("แนบไฟล์", "Attach", lang) + "</a> ";

                        }

                    }

                    dt.Add(doc_no);
                    dt.Add(rc.action);
                    dt.Add(rc.due_date);

                    string code_status = "";
                    string status = "";
                    if (rc.action_status_id == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }

                    }
                
                    else if (rc.action_status_id == 4)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i>";
                        status = rc.status;
                    }
                    else if (rc.action_status_id == 5)//cancel
                    {
                        code_status = "<i class=\"fa fa-circle text-navy\"></i>";
                        status = rc.status;
                    }
                    else if (rc.action_status_id == 6)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else if (rc.action_status_id == 2)
                    {//request close

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture).Date > Convert.ToDateTime(rc.due_date2, CultureInfo.InvariantCulture).Date)
                            {
                                code_status = "<i class=\"fa fa-circle text-danger\"></i>";
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }


                    dt.Add(code_status + " " + status);

                    if (!string.IsNullOrEmpty(rc.date_complete))
                    {
                        string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete, CultureInfo.InvariantCulture), lang);
                        dt.Add(date);
                    }
                    else
                    {
                        dt.Add("");
                    }



                    dt.Add(path_file);
                    dt.Add(rc.notify_contractor);

                    if (rc.action_status_id != 2 && rc.action_status_id != 5 && rc.action_status_id != 4)//request close,cancel and close
                    {
                        string action_close = " <button class='btn btn-sm btn-primary' onclick='return closeAction(" + rc.id + ")' >" + chageDataLanguage("ส่งข้อมูล", "Submit", lang) + "</button>";

                        string action = action_close;
                        dt.Add(action);
                    }
                    else
                    {
                        dt.Add("");

                    }


                    dt.Add(rc.remark);

                    string view = "<a href='javascript:redirecthazard(" + rc.sot_id + ");'><i class='fa fa-file-text-o fa-2x'></i></a> ";
                    dt.Add(view);

                    dataJson.Add(dt);


                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }


        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListLogRequestCloseIncident(string incident_id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.log_request_close_incidents
                        join e in dbConnect.employees on c.employee_id equals e.employee_id
                        join g in dbConnect.groups on c.group_id equals g.id

                        where c.incident_id == Convert.ToInt32(incident_id)
                        orderby c.created_at descending
                        select new
                        {
                            id = c.id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),
                            c.status_process,
                            created_at = FormatDates.getDateShow(c.created_at.ToString(), lang),
                            c.remark,
                            g.name


                        };



                ArrayList dataJson = new ArrayList();


                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    string status = chageDataLanguageStatus(rc.status_process, lang);
                    // string postion = chageDataLanguage("ผู้จัดการพื้นที่", "Area manager", lang);
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(rc.name);//for postion
                    dt.Add(name);
                    dt.Add(rc.created_at);
                    dt.Add(status);
                    dt.Add(rc.remark);
                    dataJson.Add(dt);

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }


        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListLogRequestCloseHazard(string hazard_id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.log_request_close_hazards
                        join e in dbConnect.employees on c.employee_id equals e.employee_id
                        join g in dbConnect.groups on c.group_id equals g.id

                        where c.hazard_id == Convert.ToInt32(hazard_id)
                        orderby c.created_at descending
                        select new
                        {
                            id = c.id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),
                            c.status_process,
                            created_at = FormatDates.getDateShow(c.created_at.ToString(), lang),
                            c.remark,
                            g.name


                        };



                ArrayList dataJson = new ArrayList();


                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    string status = chageDataLanguageStatusHazard(rc.status_process, lang);
                    // string postion = chageDataLanguage("ผู้จัดการพื้นที่", "Area manager", lang);
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(rc.name);//for postion
                    dt.Add(name);
                    dt.Add(rc.created_at);
                    dt.Add(status);
                    dt.Add(rc.remark);
                    dataJson.Add(dt);

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));

            }
        }



        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListLogRequestCloseHealth(string health_id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.log_request_close_healths
                        join e in dbConnect.employees on c.employee_id equals e.employee_id
                        join g in dbConnect.groups on c.group_id equals g.id

                        where c.health_id == Convert.ToInt32(health_id)
                        orderby c.created_at descending
                        select new
                        {
                            id = c.id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),
                            c.status_process,
                            created_at = FormatDates.getDateShow(c.created_at.ToString(), lang),
                            c.remark,
                            g.name


                        };



                ArrayList dataJson = new ArrayList();


                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    string status = chageDataLanguageStatusHazard(rc.status_process, lang);
                    // string postion = chageDataLanguage("ผู้จัดการพื้นที่", "Area manager", lang);
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.id);
                    dt.Add(rc.name);//for postion
                    dt.Add(name);
                    dt.Add(rc.created_at);
                    dt.Add(status);
                    dt.Add(rc.remark);
                    dataJson.Add(dt);

                }

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));

            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getGCVP(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_super_admin = 6;//check on group table old is 8
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        where e.status == "Active" && c.group_id == group_super_admin
                        && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };
                // '<li class="list-group-item">First item<span class="pull-right" style="font-size:18px;"><a href="javascript:DeleteSuperAdmin('1');"><i class="fa fa-trash"></i></a></span></li>'

                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGCVP(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";
                }


                Context.Response.Output.Write(list);
            }
        }

        [WebMethod(EnableSession= true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getLegalDepartment(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_super_admin = 7;//check on group table old is 9
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        where e.status == "Active" && c.group_id == group_super_admin
                        && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };
                // '<li class="list-group-item">First item<span class="pull-right" style="font-size:18px;"><a href="javascript:DeleteSuperAdmin('1');"><i class="fa fa-trash"></i></a></span></li>'

                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteLegalDepartment(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";
                }


                Context.Response.Output.Write(list);
            }
        }

        [WebMethod(EnableSession =true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getGroupOHS(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_super_admin = 8;//check on group table old is 10
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        where e.status == "Active" && c.group_id == group_super_admin
                        && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };
                // '<li class="list-group-item">First item<span class="pull-right" style="font-size:18px;"><a href="javascript:DeleteSuperAdmin('1');"><i class="fa fa-trash"></i></a></span></li>'

                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";
                }


                Context.Response.Output.Write(list);
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getGroupOHSHazard(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_super_admin = 16;//check on group table old is 10
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        where e.status == "Active" && c.group_id == group_super_admin
                        && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };
                // '<li class="list-group-item">First item<span class="pull-right" style="font-size:18px;"><a href="javascript:DeleteSuperAdmin('1');"><i class="fa fa-trash"></i></a></span></li>'

                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHSHazard(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";
                }


                Context.Response.Output.Write(list);
            }
        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getGroupOHSHealth(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_super_admin = 17;//check on group table old is 10
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        where e.status == "Active" && c.group_id == group_super_admin
                        && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };
               
                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHSHealth(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";
                }


                Context.Response.Output.Write(list);
            }
        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getGroupEXCO(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_exco = 19;//check on group table old is 10
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        where e.status == "Active" && c.group_id == group_exco
                        && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };

                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupEXCO(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";
                }


                Context.Response.Output.Write(list);
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getGroupCEO(string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_ceo = 20;//check on group table
                string list = "";


                var v = from c in dbConnect.employee_has_groups
                        join e in dbConnect.employees on c.employee_id equals e.employee_id into joinEm
                        from e in joinEm.DefaultIfEmpty()
                        where e.status == "Active" && c.group_id == group_ceo
                        && c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.id,
                            employee_id = c.employee_id,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            prefix = chageDataLanguage(e.prefix_th, e.prefix_en, lang),

                        };

                foreach (var rc in v)
                {
                    string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupCEO(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";
                }


                Context.Response.Output.Write(list);
            }
        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getSettingList(string lang, string setting_page_type)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string list = "";
                string smenu = setting_page_type;
                //var v;


                if (smenu == "TypeOfEmployee")
                {
                    var v = from c in dbConnect.type_employments where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "NatureOfInjury")
                {
                    var v = from c in dbConnect.nature_injuries where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "SeverityOfInjury")
                {
                    var v = from c in dbConnect.severity_injuries where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "BodyPart")
                {
                    var v = from c in dbConnect.body_parts where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "SourceOfHazard")
                {
                    var v = from c in dbConnect.source_hazards where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "SourceOfIncident")
                {
                    var v = from c in dbConnect.source_incidents where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "EventOrExposure")
                {
                    var v = from c in dbConnect.event_exposures where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "FPE")
                {
                    var v = from c in dbConnect.fatality_prevention_elements where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "RiskFactorRelateToWork")
                {
                    var v = from c in dbConnect.risk_factor_relate_works where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en,c.code };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "OccupationalHealthWork")
                {
                    var v = from c in dbConnect.occupational_health_reports where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en,c.code };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "TypeOfControl")
                {
                    var v = from c in dbConnect.type_control_healths where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en,c.code };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else if (smenu == "Hospital")
                {
                    var v = from c in dbConnect.hospitals where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en, c.code };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }
                else
                {
                    var v = from c in dbConnect.type_employments where c.status == "A" && c.country == Session["country"].ToString() select new { id = c.id, name_th = c.name_th, name_en = c.name_en };
                    foreach (var rc in v)
                    {
                        //string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        //list += "<li class='list-group-item'>" + name + "<span class='pull-right' style='font-size:18px;'><a href='javascript:DeleteGroupOHS(" + rc.id + ");'><i class='fa fa-trash'></i></a></span></li>";

                        list += "<tr style='background-color:#FFF'>";
                        list += "<td>" + rc.name_th + "</td>";
                        list += "<td>" + rc.name_en + "</td>";
                        list += "<td style='width:35px'><a href='javascript:EditArea(" + rc.id + ");'><i class='fa fa-pencil'></i></a></td>";
                        list += "<td style='width:35px'><a href='javascript:DeleteSetting(" + rc.id + ");'><i class='fa fa-trash'></i></a></td>";
                        list += "</tr>";

                    }
                }

                //list += "<table class='table table-bordered'>";

                //list += "</table>";

                if (list == "")
                {
                    Context.Response.Output.Write("");
                }
                else
                {
                    Context.Response.Output.Write("<table class='table table-bordered'>" + list + "</table>");
                }
                //Context.Response.Output.Write(list);
            }
        }






        [WebMethod(EnableSession =true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListnotifyGroupEmployee(string group_type)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string ceo_office_function = "52010001";//function_id
                //string compliance_group_oh = "53000146";//function_id


                var v = from e in dbConnect.employees
                        join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinOrg
                        from o in joinOrg.DefaultIfEmpty()
                        where  e.status == "Active" && e.country == Session["country"].ToString()
                        select new
                        {
                            employee_id = e.employee_id,
                            first_name_th = e.first_name_th,
                            first_name_en = e.first_name_en,
                            last_name_th = e.last_name_th,
                            last_name_en = e.last_name_en,
                            prefix_th = e.prefix_th,
                            prefix_en = e.prefix_en,
                            e.mngt_level,
                            o.function_id,
                            e.status,

                        };

                if (group_type == "GroupEXCO")
                {

                    v = v.Where(c => (new[] { "TML-EXCO", "TML" }).Contains(c.mngt_level));

                }else if (group_type == "GroupCEO")
                {

                   // v = v.Where(c => (new[] { "-" }).Contains(c.mngt_level));
                }
                else
                {

                    v = v.Where(c => (new[] { "FML", "MML", "SML", "TML" }).Contains(c.mngt_level) || c.function_id == ceo_office_function);

                }


                ArrayList dataJson = new ArrayList();

                foreach (var rc in v)
                {
                    string name_th = rc.prefix_th + " " + rc.first_name_th + " " + rc.last_name_th;
                    string name_en = rc.prefix_en + " " + rc.first_name_en + " " + rc.last_name_en;
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.employee_id);
                    dt.Add(name_th);
                    dt.Add(name_en);

                    dataJson.Add(dt);


                }
                /*
                var vv = from e in dbConnect.employees
                         join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinOrg
                         from o in joinOrg.DefaultIfEmpty()
                         where new[] { "FML", "MML", "SML", "TML" }.Contains(e.mngt_level) && e.status == "Active"
                         select new
                         {
                             employee_id = e.employee_id,
                             first_name_th = e.first_name_th,
                             first_name_en = e.first_name_en,
                             last_name_th = e.last_name_th,
                             last_name_en = e.last_name_en,
                             prefix_th = e.prefix_th,
                             prefix_en = e.prefix_en,


                         };
                foreach (var rc in vv)
                {
                    string name_th = rc.prefix_th + " " + rc.first_name_th + " " + rc.last_name_th;
                    string name_en = rc.prefix_en + " " + rc.first_name_en + " " + rc.last_name_en;
                    ArrayList dt = new ArrayList();
                    dt.Add(rc.employee_id);
                    dt.Add(name_th);
                    dt.Add(name_en);

                    dataJson.Add(dt);


                }*/

                var result = new
                {
                    data = dataJson
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getListTarget(string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from f in dbConnect.functions
                        join c in dbConnect.companies on f.company_id equals c.company_id
                        where f.country == Session["country"].ToString()
                        && c.country == Session["country"].ToString()
                        && f.valid_to.Value.Year >= Convert.ToInt16(year)

                        select new
                        {
                            company_id = c.company_id,
                            function_id = f.function_id,
                            company = chageDataLanguage(c.company_th, c.company_en, lang),
                            function = chageDataLanguage(f.function_th, f.function_en, lang),

                        };


                string thbody = "";
                int count = 0;
                foreach (var rc in v)
                {
                    double tifr_employee = getTifrEmployee(rc.function_id, year, lang);
                    double tifr_contractor_onsite = getTifrContractOnsite(rc.function_id, year, lang);
                    double tifr_contractor_offsite = getTifrContractOffsite(rc.function_id, year, lang);
                    double tifr_all = getTifrAll(rc.function_id, year, lang);

                    double ltifr_employee = getLtifrEmployee(rc.function_id, year, lang);
                    double ltifr_contractor_onsite = getLtifrContractOnsite(rc.function_id, year, lang);
                    double ltifr_contractor_offsite = getLtifrContractOffsite(rc.function_id, year, lang);
                    double ltifr_all = getLtifrAll(rc.function_id, year, lang);
                    double multiplier = getMultiplier(rc.function_id, year, lang);
                    double multiplier_contractor_onsite = getMultiplierContractor(rc.function_id, year, lang);
                    double multiplier_contractor_offsite = getMultiplierContractorOffsite(rc.function_id, year, lang);
                    double multiplier_all = getMultiplierAll(rc.function_id, year, lang);

                    if (count == 0)
                    {
                        double tifr_employee_group = getTifrEmployee("00000000", year, lang);
                        double tifr_contractor_onsite_group = getTifrContractOnsite("00000000", year, lang);
                        double tifr_contractor_offsite_group = getTifrContractOffsite("00000000", year, lang);
                        double tifr_all_group = getTifrAll("00000000", year, lang);

                        double ltifr_employee_group = getLtifrEmployee("00000000", year, lang);
                        double ltifr_contractor_onsite_group = getLtifrContractOnsite("00000000", year, lang);
                        double ltifr_contractor_offsite_group = getLtifrContractOffsite("00000000", year, lang);
                        double ltifr_all_group = getLtifrAll("00000000", year, lang);
                        double multiplier_group = getMultiplier("00000000", year, lang);
                        double multiplier_contractor_onsite_group = getMultiplierContractor("00000000", year, lang);
                        double multiplier_contractor_offsite_group = getMultiplierContractorOffsite("00000000", year, lang);
                        double multiplier_all_group = getMultiplierAll("00000000", year, lang);



                        thbody = "<tr>";
                        string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                        thbody = thbody + "<td>" + insee_group + "</td>";
                        thbody = thbody + "<td style=\"display:none;\">" + "00000000" + "</td>";
                        thbody = thbody + "<td></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "ltifr_employee" + "\" value=\"" + ltifr_employee_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "tifr_employee" + "\" value=\"" + tifr_employee_group.ToString("F2") + "\"></td>";            
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "multiplier" + "\" value=\"" + multiplier_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "ltifr_contractor_onsite" + "\" value=\"" + ltifr_contractor_onsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "ltifr_contractor_offsite" + "\" value=\"" + ltifr_contractor_offsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input style=\"width:70px;!important;\" class=\"form-control\" type=\"text\" id=\"" + "00000000" + "ltifr_all" + "\" value=\"" + ltifr_all_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "tifr_contractor_onsite" + "\" value=\"" + tifr_contractor_onsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "tifr_contractor_offsite" + "\" value=\"" + tifr_contractor_offsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input style=\"width:70px;!important;\" class=\"form-control\" type=\"text\" id=\"" + "00000000" + "tifr_all" + "\" value=\"" + tifr_all_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "multiplier_contractor_onsite" + "\" value=\"" + multiplier_contractor_onsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "multiplier_contractor_offsite" + "\" value=\"" + multiplier_contractor_offsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "multiplier_all" + "\" value=\"" + multiplier_all_group.ToString("F2") + "\"></td>";


                        thbody = thbody + "</tr>";
                    }

                    string tr_start = "<tr>";
                    thbody = thbody + tr_start;

                    thbody = thbody + "<td>" + rc.company + "</td>";
                    thbody = thbody + "<td style=\"display:none;\">" + rc.function_id + "</td>";
                    thbody = thbody + "<td>" + "<a href=\"\"  onclick=\"return getTargetSub('" + rc.function_id + "');\">" + rc.function + "</a></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "ltifr_employee" + "\" value=\"" + ltifr_employee.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "tifr_employee" + "\" value=\"" + tifr_employee.ToString("F2") + "\"></td>";                 
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "multiplier" + "\" value=\"" + multiplier.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "ltifr_contractor_onsite" + "\" value=\"" + ltifr_contractor_onsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "ltifr_contractor_offsite" + "\" value=\"" + ltifr_contractor_offsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input style=\"width:70px;!important;\" class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "ltifr_all" + "\" value=\"" + ltifr_all.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "tifr_contractor_onsite" + "\" value=\"" + tifr_contractor_onsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "tifr_contractor_offsite" + "\" value=\"" + tifr_contractor_offsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input style=\"width:70px;!important;\" class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "tifr_all" + "\" value=\"" + tifr_all.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "multiplier_contractor_onsite" + "\" value=\"" + multiplier_contractor_onsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "multiplier_contractor_offsite" + "\" value=\"" + multiplier_contractor_offsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "multiplier_all" + "\" value=\"" + multiplier_all.ToString("F2") + "\"></td>";



                    string tr_end = "</tr>";
                    thbody = thbody + tr_end;
                    count++;
                }

                var result = new
                {
                    data = thbody
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }
        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getListTargetSrilanka(string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                 var v = (from c in dbConnect.organizations
                        where c.country == Session["country"].ToString()
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            site_id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        }).Distinct();



                string thbody = "";
                int count = 0;
                foreach (var rc in v)
                {
                    double tifr_employee = getTifrEmployeeSrilanka(rc.site_id, year, lang);
                    double tifr_contractor_onsite = getTifrContractOnsiteSrilanka(rc.site_id, year, lang);
                    double tifr_contractor_offsite = getTifrContractOffsiteSrilanka(rc.site_id, year, lang);

                    double ltifr_employee = getLtifrEmployeeSrilanka(rc.site_id, year, lang);
                    double ltifr_contractor_onsite = getLtifrContractOnsiteSrilanka(rc.site_id, year, lang);
                    double ltifr_contractor_offsite = getLtifrContractOffsiteSrilanka(rc.site_id, year, lang);
                    double multiplier = getMultiplierSrilanka(rc.site_id, year, lang);
                    double multiplier_contractor = getMultiplierContractorSrilanka(rc.site_id, year, lang);

                    if (count == 0)
                    {
                        double tifr_employee_group = getTifrEmployeeSrilanka("00000000", year, lang);
                        double tifr_contractor_onsite_group = getTifrContractOnsiteSrilanka("00000000", year, lang);
                        double tifr_contractor_offsite_group = getTifrContractOffsiteSrilanka("00000000", year, lang);

                        double ltifr_employee_group = getLtifrEmployeeSrilanka("00000000", year, lang);
                        double ltifr_contractor_onsite_group = getLtifrContractOnsiteSrilanka("00000000", year, lang);
                        double ltifr_contractor_offsite_group = getLtifrContractOffsiteSrilanka("00000000", year, lang);
                        double multiplier_group = getMultiplierSrilanka("00000000", year, lang);
                        double multiplier_contractor_group = getMultiplierContractorSrilanka("00000000", year, lang);
                        thbody = "<tr>";
                        string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรีศรีลังกา", "INSEE Lanka", lang);

                      
                        thbody = thbody + "<td style=\"display:none;\">" + "00000000" + "</td>";
                        thbody = thbody + "<td>" + insee_group + "</td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "tifr_employee" + "\" value=\"" + tifr_employee_group + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "ltifr_employee" + "\" value=\"" + ltifr_employee_group + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "multiplier" + "\" value=\"" + multiplier_group + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "tifr_contractor_onsite" + "\" value=\"" + tifr_contractor_onsite_group + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "tifr_contractor_offsite" + "\" value=\"" + tifr_contractor_offsite_group + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "ltifr_contractor_onsite" + "\" value=\"" + ltifr_contractor_onsite_group + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "ltifr_contractor_offsite" + "\" value=\"" + ltifr_contractor_offsite_group + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + "00000000" + "multiplier_contractor" + "\" value=\"" + multiplier_contractor_group + "\"></td>";


                        thbody = thbody + "</tr>";
                    }

                    string tr_start = "<tr>";
                    thbody = thbody + tr_start;

                   // thbody = thbody + "<td>" + rc.company + "</td>";
                    thbody = thbody + "<td style=\"display:none;\">" + rc.site_id + "</td>";
                    thbody = thbody + "<td>" + rc.name + "</td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "tifr_employee" + "\" value=\"" + tifr_employee + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "ltifr_employee" + "\" value=\"" + ltifr_employee + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "multiplier" + "\" value=\"" + multiplier + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "tifr_contractor_onsite" + "\" value=\"" + tifr_contractor_onsite + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "tifr_contractor_offsite" + "\" value=\"" + tifr_contractor_offsite + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "ltifr_contractor_onsite" + "\" value=\"" + ltifr_contractor_onsite + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "ltifr_contractor_offsite" + "\" value=\"" + ltifr_contractor_offsite + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "multiplier_contractor" + "\" value=\"" + multiplier_contractor + "\"></td>";



                    string tr_end = "</tr>";
                    thbody = thbody + tr_end;
                    count++;
                }

                var result = new
                {
                    data = thbody
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }
        }


        public double getTifrEmployee(string function_id,string year,string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    //int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_employee);
                }


                return Math.Round(value, 2);
            }
        }


        public double getTifrEmployeeSrilanka(string site_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    //int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_employee);
                }


                return Math.Round(value, 2);
            }
        }


        public double getTifrContractOnsite(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    //  int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_contractor_onsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getTifrContractOnsiteSrilanka(string site_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    //  int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_contractor_onsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getTifrContractOffsite(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getTifrAll(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_all);
                }


                return Math.Round(value, 2);
            }
        }



        public double getTifrContractOffsiteSrilanka(string site_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getLtifrEmployee(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    //int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_employee);
                }


                return Math.Round(value, 2);
            }
        }


        public double getLtifrEmployeeSrilanka(string site_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    //int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_employee);
                }


                return Math.Round(value, 2);
            }
        }


        public double getLtifrContractOnsite(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    //int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_contractor_onsite);
                }


                return Math.Round(value, 2);
            }
        }




        public double getLtifrContractOnsiteSrilanka(string site_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    //int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_contractor_onsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getLtifrContractOffsite(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getLtifrAll(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_all);
                }


                return Math.Round(value, 2);
            }
        }


        public double getLtifrContractOffsiteSrilanka(string site_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }

        public double getMultiplier(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.multiplier);
                }


                return Math.Round(value, 2);
            }
        }




        public double getMultiplierSrilanka(string site_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.multiplier);
                }


                return Math.Round(value, 2);
            }
        }

        public double getMultiplierContractor(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.multiplier_contractor);
                }


                return Math.Round(value, 2);
            }
        }


        public double getMultiplierContractorOffsite(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.multiplier_contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getMultiplierAll(string function_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.multiplier_all);
                }


                return Math.Round(value, 2);
            }
        }



        public double getMultiplierContractorSrilanka(string site_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.multiplier_contractor);
                }


                return Math.Round(value, 2);
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getListTargetSub(string function_id,string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from d in dbConnect.departments
                        join f in dbConnect.functions on d.function_id equals f.function_id
                        where d.function_id == function_id
                        && d.valid_to.Value.Year >= Convert.ToInt16(year)
                        select new
                        {
                            function_id = f.function_id,
                            department_id = d.department_id,
                            function = chageDataLanguage(f.function_th, f.function_en, lang),
                            department = chageDataLanguage(d.department_th, d.department_en, lang),

                        };


                string thbody = "";
                int count = 0;
                foreach (var rc in v)
                {
                    double tifr_employee = getTifrEmployeeSub(rc.department_id, year, lang);
                    double tifr_contractor_onsite = getTifrContractOnsiteSub(rc.department_id, year, lang);
                    double tifr_contractor_offsite = getTifrContractOffsiteSub(rc.department_id, year, lang);
                    double tifr_all = getTifrAllSub(rc.department_id, year, lang);

                    double ltifr_employee = getLtifrEmployeeSub(rc.department_id, year, lang);
                    double ltifr_contractor_onsite = getLtifrContractOnsiteSub(rc.department_id, year, lang);
                    double ltifr_contractor_offsite = getLtifrContractOffsiteSub(rc.department_id, year, lang);
                    double ltifr_all = getLtifrAllSub(rc.department_id, year, lang);
                    double multiplier = getMultiplierSub(rc.department, year, lang);
                    double multiplier_contractor_onsite = getMultiplierContractorSub(rc.department_id, year, lang);
                    double multiplier_contractor_offsite = getMultiplierContractorOffsiteSub(rc.department_id, year, lang);
                    double multiplier_all = getMultiplierAllSub(rc.department_id, year, lang);


                    if (count == 0)
                    {
                        double tifr_employee_group = getTifrEmployee(function_id, year, lang);
                        double tifr_contractor_onsite_group = getTifrContractOnsite(function_id, year, lang);
                        double tifr_contractor_offsite_group = getTifrContractOffsite(function_id, year, lang);
                        double tifr_all_group = getTifrAll(function_id, year, lang);

                        double ltifr_employee_group = getLtifrEmployee(function_id, year, lang);
                        double ltifr_contractor_onsite_group = getLtifrContractOnsite(function_id, year, lang);
                        double ltifr_contractor_offsite_group = getLtifrContractOffsite(function_id, year, lang);
                        double ltifr_all_group = getLtifrAll(function_id, year, lang);
                        double multiplier_group = getMultiplier(function_id, year, lang);
                        double multiplier_contractor_onsite_group = getMultiplierContractor(function_id, year, lang);
                        double multiplier_contractor_offsite_group = getMultiplierContractorOffsite(function_id, year, lang);
                        double multiplier_all_group = getMultiplierAll(function_id, year, lang);

                        thbody = "<tr>";

                        thbody = thbody + "<td>" + rc.function + "</td>";
                        thbody = thbody + "<td style=\"display:none;\">" + function_id + "</td>";
                        thbody = thbody + "<td></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_ltifr_employee" + "\" value=\"" + ltifr_employee_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_tifr_employee" + "\" value=\"" + tifr_employee_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_multiplier" + "\" value=\"" + multiplier_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_ltifr_contractor_onsite" + "\" value=\"" + ltifr_contractor_onsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_ltifr_contractor_offsite" + "\" value=\"" + ltifr_contractor_offsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input style=\"width:70px;!important;\" class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_ltifr_all" + "\" value=\"" + ltifr_all_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_tifr_contractor_onsite" + "\" value=\"" + tifr_contractor_onsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_tifr_contractor_offsite" + "\" value=\"" + tifr_contractor_offsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input style=\"width:70px;!important;\" class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_tifr_all" + "\" value=\"" + tifr_all_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_multiplier_contractor_onsite" + "\" value=\"" + multiplier_contractor_onsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_multiplier_contractor_offsite" + "\" value=\"" + multiplier_contractor_offsite_group.ToString("F2") + "\"></td>";
                        thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + function_id + "sub_multiplier_all" + "\" value=\"" + multiplier_all_group.ToString("F2") + "\"></td>";

                        thbody = thbody + "</tr>";
                    }


                    string tr_start = "<tr>";
                    thbody = thbody + tr_start;


                    thbody = thbody + "<td>" + rc.function + "</td>";
                    thbody = thbody + "<td style=\"display:none;\">" + rc.department_id + "</td>";
                    thbody = thbody + "<td>" + rc.department + "</td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_ltifr_employee" + "\" value=\"" + ltifr_employee.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_tifr_employee" + "\" value=\"" + tifr_employee.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_multiplier" + "\" value=\"" + multiplier.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_ltifr_contractor_onsite" + "\" value=\"" + ltifr_contractor_onsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_ltifr_contractor_offsite" + "\" value=\"" + ltifr_contractor_offsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input style=\"width:70px;!important;\" class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_ltifr_all" + "\" value=\"" + ltifr_all.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_tifr_contractor_onsite" + "\" value=\"" + tifr_contractor_onsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_tifr_contractor_offsite" + "\" value=\"" + tifr_contractor_offsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input style=\"width:70px;!important;\" class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_tifr_all" + "\" value=\"" + tifr_all.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_multiplier_contractor_onsite" + "\" value=\"" + multiplier_contractor_onsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_multiplier_contractor_offsite" + "\" value=\"" + multiplier_contractor_offsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.department_id + "sub_multiplier_all" + "\" value=\"" + multiplier_all.ToString("F2") + "\"></td>";


                    string tr_end = "</tr>";
                    thbody = thbody + tr_end;
                    count++;
                }

                var result = new
                {
                    data = thbody
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }



        public double getTifrEmployeeSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_employee);
                }


                return Math.Round(value, 2);
            }
        }


        public double getTifrContractOnsiteSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_contractor_onsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getTifrContractOffsiteSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    //int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getTifrAllSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    //int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.tifr_all);
                }


                return Math.Round(value, 2);
            }
        }


        public double getLtifrEmployeeSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    //int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_employee);
                }


                return Math.Round(value, 2);
            }
        }


        public double getLtifrContractOnsiteSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_contractor_onsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getLtifrContractOffsiteSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }




        public double getLtifrAllSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_all);
                }


                return Math.Round(value, 2);
            }
        }

        public double getMultiplierSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.multiplier);
                }


                return Math.Round(value, 2);
            }
        }


        public double getMultiplierAllSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.multiplier_all);
                }


                return Math.Round(value, 2);
            }
        }



        public double getMultiplierContractorSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.multiplier_contractor);
                }


                return Math.Round(value, 2);
            }
        }



        public double getMultiplierContractorOffsiteSub(string department_id, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.target_subs
                        where c.department_id == department_id
                        select c;

                if (year != "" && year != "null")
                {
                    // int new_year = FormatDates.getYear(Convert.ToInt16(year), lang);
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.multiplier_contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }






        




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getListWorkhour(string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from f in dbConnect.functions
                        join c in dbConnect.companies on f.company_id equals c.company_id
                        where f.country == Session["country"].ToString()
                        && c.country == Session["country"].ToString()
                        && f.valid_to.Value.Year >= Convert.ToInt16(year)

                        select new
                        {
                            company_id = c.company_id,
                            function_id = f.function_id,
                            company = chageDataLanguage(c.company_th, c.company_en, lang),
                            function = chageDataLanguage(f.function_th, f.function_en, lang),

                        };


                string thbody = "";

                foreach (var rc in v)
                {
                    double employee = getEmployeeMain(rc.function_id, month, year, lang);
                    double contractor_onsite = getContractorOnsiteMain(rc.function_id, month, year, lang);
                    double contractor_offsite = getContractorOffsiteMain(rc.function_id, month, year, lang);
                    double training_hour = getTrainingHourMain(rc.function_id, month, year, lang);



                    string tr_start = "<tr>";
                    thbody = thbody + tr_start;

                    thbody = thbody + "<td>" + rc.company + "</td>";
                    thbody = thbody + "<td style=\"display:none;\">" + rc.function_id + "</td>";
                    thbody = thbody + "<td>" + "<a href=\"\"  onclick=\"return getWorkhourSub('" + rc.function_id + "');\">" + rc.function + "</a></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "employee" + "\" value=\"" + employee.ToString("F2") + "\"  disabled></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "contractor_onsite" + "\" value=\"" + contractor_onsite.ToString("F2") + "\"  disabled></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "contractor_offsite" + "\" value=\"" + contractor_offsite.ToString("F2") + "\"  disabled></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.function_id + "training_hour" + "\" value=\"" + training_hour.ToString("F2") + "\"  disabled></td>";

                    string tr_end = "</tr>";
                    thbody = thbody + tr_end;

                }

                var result = new
                {
                    data = thbody
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getListWorkhourSrilanka(string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.organizations
                         where c.country == Session["country"].ToString()
                         orderby c.personnel_subarea ascending
                         select new
                         {
                             site_id = c.personnel_subarea,
                             name = c.personnel_subarea_description

                         }).Distinct();


                string thbody = "";

                foreach (var rc in v)
                {
                    double employee = getEmployeeMainSrilanka(rc.site_id, month, year, lang);
                    double contractor_onsite = getContractorOnsiteMainSrilanka(rc.site_id, month, year, lang);
                    double contractor_offsite = getContractorOffsiteMainSrilanka(rc.site_id, month, year, lang);



                    string tr_start = "<tr>";
                    thbody = thbody + tr_start;

                    //thbody = thbody + "<td>" + rc.company + "</td>";
                    thbody = thbody + "<td style=\"display:none;\">" + rc.site_id + "</td>";
                    thbody = thbody + "<td>" + rc.name + "</td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "employee" + "\" value=\"" + employee + "\" ></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "contractor_onsite" + "\" value=\"" + contractor_onsite + "\" ></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.site_id + "contractor_offsite" + "\" value=\"" + contractor_offsite + "\" ></td>";

                    string tr_end = "</tr>";
                    thbody = thbody + tr_end;

                }

                var result = new
                {
                    data = thbody
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }

        public double getEmployeeMain(string function_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.employee);
                }


                return Math.Round(value, 2);
            }
        }



        public double getEmployeeMainSrilanka(string site_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.employee);
                }


                return Math.Round(value, 2);
            }
        }


        public double getContractorOnsiteMain(string function_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.contractor_onsite);
                }


                return Math.Round(value, 2);
            }
        }



        public double getContractorOnsiteMainSrilanka(string site_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.contractor_onsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getContractorOffsiteMain(string function_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }



        public double getTrainingHourMain(string function_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_mains
                        where c.function_id == function_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.training_hour);
                }


                return Math.Round(value, 2);
            }
        }


        public double getContractorOffsiteMainSrilanka(string site_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_main_srilankas
                        where c.site_id == site_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void getListWorkhourSub(string function_id,string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from di in dbConnect.divisions
                        join d in dbConnect.departments on di.department_id equals d.department_id
                        where d.function_id == function_id
                        && di.valid_to.Value.Year >= Convert.ToInt16(year)


                        select new
                        {
                            department_id = d.department_id,
                            division_id = di.division_id,
                            division = chageDataLanguage(di.division_th, di.division_en, lang),
                            department = chageDataLanguage(d.department_th, d.department_en, lang),

                        };


                string thbody = "";
                int count = 0;
                foreach (var rc in v)
                {
                    double employee = getEmployeeSub(rc.division_id, month, year, lang);
                    double contractor_onsite = getContractorOnsiteSub(rc.division_id, month, year, lang);
                    double contractor_offsite = getContractorOffsiteSub(rc.division_id, month, year, lang);
                    double training_hour = getTrainingHourSub(rc.division_id, month, year, lang);

                    string tr_start = "<tr>";
                    thbody = thbody + tr_start;


                    thbody = thbody + "<td>" + rc.department + "</td>";
                    thbody = thbody + "<td style=\"display:none;\">" + rc.division_id + "</td>";
                    thbody = thbody + "<td>" + rc.division + "</td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.division_id + "employee" + "\" value=\"" + employee.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.division_id + "contractor_onsite" + "\" value=\"" + contractor_onsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.division_id + "contractor_offsite" + "\" value=\"" + contractor_offsite.ToString("F2") + "\"></td>";
                    thbody = thbody + "<td><input class=\"form-control\" type=\"text\" id=\"" + rc.division_id + "training_hour" + "\" value=\"" + training_hour.ToString("F2") + "\"></td>";


                    string tr_end = "</tr>";
                    thbody = thbody + tr_end;
                    count++;
                }

                var result = new
                {
                    data = thbody
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(result));
            }

        }


        public double getEmployeeSub(string division_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_subs
                        where c.division_id == division_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.employee);
                }


                return Math.Round(value, 2);
            }
        }


        public double getContractorOnsiteSub(string division_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_subs
                        where c.division_id == division_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.contractor_onsite);
                }


                return Math.Round(value, 2);
            }
        }



        public double getContractorOffsiteSub(string division_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_subs
                        where c.division_id == division_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.contractor_offsite);
                }


                return Math.Round(value, 2);
            }
        }


        public double getTrainingHourSub(string division_id, string month, string year, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                double value = Math.Round(0.0, 2);

                var v = from c in dbConnect.workhour_subs
                        where c.division_id == division_id
                        select c;

                if (year != "" && year != "null")
                {
                    v = v.Where(c => c.created.Value.Year == Convert.ToInt16(year));

                }

                if (month != "" && month != "null")
                {
                    v = v.Where(c => c.created.Value.Month == Convert.ToInt16(month));

                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.training_hour);
                }


                return Math.Round(value, 2);
            }
        }

        public string chageDataLanguageStatusHazard(string v, string lang)
        {
            string vReturn = "";

            if (lang == "th")
            {
                if (v == "P")
                {
                    vReturn = "อนุมัติ";
                }
                else
                {
                    vReturn = "ไม่อนุมัติ";
                }




            }
            else if (lang == "en")
            {

                if (v == "P")
                {
                    vReturn = "Approved";
                }
                else
                {
                    vReturn = "Not approved";
                }

            }
            else if (lang == "si")
            {

                if (v == "P")
                {
                    vReturn = "Approved";
                }
                else
                {
                    vReturn = "Not approved";
                }

            }


            return vReturn;
        }




        public string chageDataLanguageStatus(string v, string lang)
        {
            string vReturn = "";

            if (lang == "th")
            {
                if(v=="C")
                {
                    vReturn = "ปิด";
                }
                else
                {
                    vReturn = "ไม่ปิด";
                }
                



            }
            else if (lang == "en")
            {

                if (v == "C")
                {
                    vReturn = "Closed";
                }
                else
                {
                    vReturn = "Not closed";
                }

            }
            else if (lang == "si")
            {

                if (v == "C")
                {
                    vReturn = "Closed";
                }
                else
                {
                    vReturn = "Not closed";
                }

            }


            return vReturn;
        }
        public string chageDataLanguageEvidence(string vTH, string vEN, string lang)
        {
            string vReturn = "";

            if (lang == "th")
            {

                vReturn = vTH;



            }
            else if (lang == "en")
            {

                vReturn = vEN;

            } if (lang == "si")
            {

                vReturn = vEN;



            }


            return vReturn;
        }








        public string chageDataLanguageArea(string vTH, string vEN, string lang)
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



        public string setEmptyDefault(string lang)
        {
            string vReturn = "";

            if (lang == "th")
            {
                vReturn = "ไม่ระบุ";

            }
            else if (lang == "en")
            {

                vReturn = "NA";
            }
            else if (lang == "si")
            {

                vReturn = "NA";
            }


            return vReturn;
        }

       



     
    }
}
