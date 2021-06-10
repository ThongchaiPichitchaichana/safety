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
using safetys4.Class;

namespace safetys4
{
    /// <summary>
    /// Summary description for Actionevent
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Actionevent : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getFuctionlist(string lang)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.functions
                        where c.country == Session["country"].ToString()
                        select new
                        {
                            id = c.function_id,
                            name = chageDataLanguage(c.function_th,c.function_en,lang)

                        };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));
            }

        }




        [WebMethod(EnableSession= true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createContractor(  string function_id,
                                       string userid,
                                       string email,
                                       string prefixth,
                                       string firstnameth,
                                       string lastnameth,
                                       string prefixen,
                                       string firstnameen,
                                       string lastnameen,
                                       string companyname,
                                       string mobilephone,
                                       string phone,
                                       string active)
        {
            
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                contractor objInsert = new contractor();

                objInsert.email = email;
                objInsert.prefix_th = prefixth;
                objInsert.first_name_th = firstnameth;
                objInsert.last_name_th = lastnameth;
                objInsert.prefix_en = prefixen;
                objInsert.first_name_en = firstnameen;
                objInsert.last_name_en = lastnameen;
                objInsert.company = companyname;
                objInsert.mobile_phone = mobilephone;
                objInsert.phone = phone;
                objInsert.status = active;
                objInsert.function_id = function_id;
                objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                objInsert.employee_id = userid;
                objInsert.country = Session["country"].ToString();
                objInsert.timezone = Session["timezone"].ToString();



                dbConnect.contractors.InsertOnSubmit(objInsert);


                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";

                }
                catch (Exception e)
                {

                    // dbConnect.SubmitChanges();
                }


                Context.Response.Output.Write(result);
            }


        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateContractor(string function_id,
                                    string id,
                                    string userid,
                                    string email,
                                    string prefixth,
                                    string firstnameth,
                                    string lastnameth,
                                    string prefixen,
                                    string firstnameen,
                                    string lastnameen,
                                    string companyname,
                                    string mobilephone,
                                    string phone,
                                    string active
                                    )
        {
           
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var query = from c in dbConnect.contractors
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (contractor rc in query)
                {
                    rc.function_id = function_id;
                    rc.email = email;
                    rc.prefix_th = prefixth;
                    rc.first_name_th = firstnameth;
                    rc.last_name_th = lastnameth;
                    rc.prefix_en = prefixen;
                    rc.first_name_en = firstnameen;
                    rc.last_name_en = lastnameen;
                    rc.company = companyname;
                    rc.mobile_phone = mobilephone;
                    rc.phone = phone;
                    rc.status = active;
                    rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    rc.employee_id = userid;

                }


                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }

                Context.Response.Output.Write(result);
            }


         
           // return result;

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getContractorbyid(string id,string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var q = from c in dbConnect.contractors
                        join e in dbConnect.employees on c.employee_id equals e.employee_id //into joinE
                        join o in dbConnect.organizations on c.function_id equals o.function_id// into joinO
                        //from e in joinE.DefaultIfEmpty()
                        //from o in joinO.DefaultIfEmpty()
                        where c.id == Convert.ToInt32(id) && c.country == Session["country"].ToString()
                        select new
                        {
                            c.function_id,
                            email = c.email,
                            prefix_th = c.prefix_th.Trim(),
                            first_name_th = c.first_name_th,
                            last_name_th = c.last_name_th,
                            prefix_en = c.prefix_en.Trim(),
                            first_name_en = c.first_name_en,
                            last_name_en = c.last_name_en,
                            company = c.company,
                            mobile_phone = c.mobile_phone,
                            phone = c.phone,
                            status = c.status,
                            function_name = o.function,
                            employee_firstname = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            employee_lastname = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            update_at = chageDate(Convert.ToDateTime(c.updated_at), lang)



                        };



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(q));

            }

           // dbConnect.SubmitChanges();

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void checkEmailContractor(string email,string function_id ,string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var v = from c in dbConnect.contractors
                        where c.email == email.Trim() && c.function_id == function_id.Trim()
                        select new
                        {
                            c.id
                        };


                if (id != "")
                {
                    v = v.Where(c => c.id != Convert.ToInt32(id));

                }

                string result = "false";

                foreach (var rc in v)
                {
                    result = "true";

                }

                Context.Response.Output.Write(result);

            }
           // return result;

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


        public string chageDate(DateTime date, string lang)
        {
            string datetime = "";

            CultureInfo ThaiCulture = new CultureInfo("th-TH");
            CultureInfo UsaCulture = new CultureInfo("en-US");
  

            if (lang == "th")
            {
                datetime = date.ToString("dd/MM/yyyy HH:mm", ThaiCulture);

            }
            else if (lang == "en")
            {
               
                datetime = date.ToString("dd/MM/yyyy HH:mm", UsaCulture);
            }
            else if (lang == "si")
            {
                datetime = date.ToString("dd/MM/yyyy HH:mm", UsaCulture);

            }


            return datetime;
        }




        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
 
        public void createSuperadmin(List<string> employee_id,string country)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {
                    int group_super_admin = 2;//check on group table




                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();
                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_super_admin;
                        objInsert.country = country;
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }
        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createDelegateSuperadmin(List<object> employee_id,string country)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_delegate_super_admin = 3;//check on group table
                string result = "false";

                try
                {
                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();

                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_delegate_super_admin;
                        objInsert.country = country;
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);
                        dbConnect.SubmitChanges();


                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    // dbConnect.SubmitChanges();
                }

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

                // Context.Response.Output.Write(result);
            }

        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteSuperadmin(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }
          
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteDelegateSuperadmin(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }

        }





        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createOHS(List<string> employee_id,string function_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {
                    int group_ohs = 4;//check on group table



                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();
                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_ohs;
                        objInsert.function_id = function_id;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

        }




        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createDelegateOHS(List<object> employee_id, string function_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int group_delegate_ohs = 5;//check on group table
                string result = "false";

                try
                {
                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();

                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_delegate_ohs;
                        objInsert.function_id = function_id;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);
                        dbConnect.SubmitChanges();


                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    // dbConnect.SubmitChanges();
                }

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

                // Context.Response.Output.Write(result);
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteOHS(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }
        }


        [WebMethod(EnableSession= true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createArea(string name_th,string name_en,string company_id,string function_id,string department_id,string division_id,string section_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {


                    area_management objInsert = new area_management();
                    objInsert.name_th = name_th;
                    objInsert.name_en = name_en;
                    objInsert.company_id = company_id;
                    objInsert.function_id = function_id;
                    objInsert.department_id = department_id;
                    objInsert.division_id = division_id;
                    objInsert.section_id = section_id;
                    objInsert.status = "A";
                    objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.country = Session["country"].ToString();
                    dbConnect.area_managements.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Output.Write(result);
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteArea(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.area_managements
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.area_managements.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }

        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteDelegateOHS(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getAreaByid(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var q = from c in dbConnect.area_managements
                        where c.id == Convert.ToInt32(id)
                        select new
                        {

                            name_th = c.name_th,
                            name_en = c.name_en,


                        };

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(q));

            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateArea( string id,
                                string name_th,
                                string name_en
                                )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";
              
                var query = from c in dbConnect.area_managements
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (area_management rc in query)
                {
                    rc.name_th = name_th;
                    rc.name_en = name_en;

                }

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createAreaOHS(List<string> employee_id, string department_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {
                    // int group_area_ohs = 1;//check on group area table
                    deleteEmployeeDepartment(department_id);


                    foreach (var v in employee_id)
                    {
                        employee_has_department objInsert = new employee_has_department();
                        objInsert.employee_id = v.ToString();
                        objInsert.department_id = department_id;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_departments.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;

                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }
        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createAreaManager(List<string> employee_id, string department_id,string division_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {

                    // int group_area_manager = 2;//check on group area table
                    deleteEmployeeDivision(division_id);


                    foreach (var v in employee_id)
                    {
                        employee_has_division objInsert = new employee_has_division();
                        objInsert.employee_id = v.ToString();
                        //objInsert.area_group_id = group_area_manager;
                        //objInsert.department_id = department_id;
                        objInsert.division_id = division_id;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_divisions.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;

                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createAreaSupervisor(List<string> employee_id, string department_id, string division_id,string section_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {

                    //int group_area_supervisor = 3;//check on group area table
                    deleteEmployeeSection(section_id);

                    foreach (var v in employee_id)
                    {
                        employee_has_section objInsert = new employee_has_section();
                        objInsert.employee_id = v.ToString();
                        //objInsert.area_group_id = group_area_supervisor;
                        //objInsert.department_id = department_id;
                        //objInsert.division_id = division_id;
                        objInsert.section_id = section_id;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_sections.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;

                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

        }





        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createFunctionalManager(List<string> employee_id, string department_id, string division_id, string section_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {

                    //int group_area_supervisor = 3;//check on group area table
                    deleteEmployeeFunctionalManager(department_id);

                    foreach (var v in employee_id)
                    {
                        employee_has_department_functional_manager objInsert = new employee_has_department_functional_manager();
                        objInsert.employee_id = v.ToString();
                        //objInsert.area_group_id = group_area_supervisor;
                        //objInsert.department_id = department_id;
                        //objInsert.division_id = division_id;
                        objInsert.department_id = department_id;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_department_functional_managers.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;

                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

        }



        //public void deleteEmployeeAreaGroup(string department_id, string division_id, string section_id,int area_group_id)
        //{
           
        //    safetys4dbDataContext dbConnect = new safetys4dbDataContext();

        //    var gr = from c in dbConnect.employee_has_area_groups
        //             where c.area_group_id == area_group_id
        //             select c;

        //    if (department_id != "")
        //    {
        //        gr = gr.Where(c => c.department_id == department_id.Trim());

        //    }

        //    if (division_id != "")
        //    {
        //        gr = gr.Where(c => c.division_id == division_id.Trim());

        //    }

        //    if (section_id != "")
        //    {
        //        gr = gr.Where(c => c.section_id == section_id.Trim());

        //    }

        //    foreach (var a in gr)
        //    {
        //        dbConnect.employee_has_area_groups.DeleteOnSubmit(a);
        //    }
        //    try
        //    {
        //        dbConnect.SubmitChanges();
                
        //    }
        //    catch (Exception e)
        //    {

               
        //    }

            

        //}


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


        public void deleteEmployeeFunctionalManager(string department_id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.employee_has_department_functional_managers
                         where c.department_id == department_id
                         select c;


                foreach (var a in gr)
                {
                    dbConnect.employee_has_department_functional_managers.DeleteOnSubmit(a);
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

        [WebMethod(EnableSession= true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void testdattime(string reportdate)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                incident objInsert = new incident();
                //objInsert.report_date = Convert.ToDateTime(reportdate, CultureInfo.CreateSpecificCulture("th-TH"));
                //string d = Convert.ToDateTime(reportdate).ToString("dd/MM/yyyy HH:mm:ss"); 
                objInsert.doc_no = generateDocno(Session["country"].ToString(), Session["timezone"].ToString());
                //objInsert.incident_date = Convert.ToDateTime(incidentdate + " " + incidenttime, lang);
                objInsert.report_date = Convert.ToDateTime(reportdate, CultureInfo.CreateSpecificCulture("th-TH"));
                objInsert.process_status = 1;
               // objInsert.created_at = DateTime.Now;
                //objInsert.updated_at = DateTime.Now;
                dbConnect.incidents.InsertOnSubmit(objInsert);

                dbConnect.SubmitChanges();
            }
        }


        
        
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createIncident(    string incidentdate,
                                       string incidenttime,
                                       string reportdate,
                                       string company_id,
                                       string function_id,
                                       string department_id,
                                       string division_id,
                                       string section_id,
                                       string activity_company_id,
                                       string activity_function_id,
                                       string activity_department_id,
                                       string activity_division_id,
                                       string activity_section_id,
                                       string responsible_area,
                                       string owner_activity,
                                       string incidentarea,
                                       string incidentname,
                                       string incidentdetail,
                                       string userid,
                                       string typelogin,
                                       string phone)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                try
                {
                    int process_status = 1;//on process
                    byte incident_flow = 1;
                    incident objInsert = new incident();

                    objInsert.doc_no = generateDocno(Session["country"].ToString(), Session["timezone"].ToString());
                    objInsert.incident_date = FormatDates.changeDateTimeDB(incidentdate + " " + incidenttime, Session["lang"].ToString());

                    //objInsert.incident_date = Convert.ToDateTime(incidentdate + " " + incidenttime, lang);
                    objInsert.report_date = FormatDates.changeDateTimeDB(reportdate, Session["lang"].ToString());
                    //objInsert.report_date = Convert.ToDateTime(reportdate, CultureInfo.CreateSpecificCulture("th-TH")); 
                    //objInsert.report_date = DateTime.ParseExact(reportdate, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-GB")); 

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
                    objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.is_alert_over_due = 0;
                    objInsert.incident_flow = incident_flow;
                    objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.alert_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.alert_form_three_to_four_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.last_alerted_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.alert_form_two_to_three_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    
                    objInsert.device_type = "web";
                    objInsert.country = Session["country"].ToString();

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

                    if(Session["country"].ToString()=="thailand")
                    {
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

                        if (Session["country"].ToString() == "thailand")
                        {
                            objInsert.reporter_department_id = rc1.department_id;
                            objInsert.reporter_department_name = rc1.department;
                        }
                        else if (Session["country"].ToString() == "srilanka")
                        {

                            objInsert.reporter_department_id = rc1.sub_function_id;
                            objInsert.reporter_department_name = rc1.sub_function;

                        }

                    }
                    //////////////////////////////////////end reporter//////////////////////////////////////////////



                    dbConnect.incidents.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                    incident_detail objInsert2 = new incident_detail();
                    objInsert2.employee_id = userid;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.incident_id = objInsert.id;

                    dbConnect.incident_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();

                    //////////////////////////////////by p.poo sent notification/////////////////////////////////

                    Class.SafetyNotification sn = new Class.SafetyNotification();
                    string[] alert_to_groups = { "AdminOH&S", "AreaSuperervisor", "AreaManager", "AreaOH&S", "GroupOH&S" };
                    sn.InsertNotification(1, objInsert.id, alert_to_groups, Session["timezone"].ToString(), "AreaSuperervisor");
                    ///////////////////////////////////end//////////////////////////////////////////////////////


                    Context.Response.Output.Write(objInsert.id);

                }
                catch (Exception ex)
                {
                    action_log objInsert = new action_log();
                    objInsert.function_name = "createIncident";
                    objInsert.file_name = "Actionevent";
                    objInsert.error_message = ex.Message;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();
                }


             

            }
        }


        public Array getMasterdataName(string id,string master_name)
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
                                name_th = chageDataLanguage(c.company_th,c.company_en,"th"),
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
       


        public void MobileUploadedFileIncident(HttpFileCollection httpFileCollection,string user_id,string reportdate,string lang,string timezone)
        {
           
            reportdate = FormatDates.changeDateTimeUpload(reportdate,lang);         
            string name_folder = user_id + "_" + FormatDates.getDateTimeNoDash(reportdate.Trim());
            string file_name = FormatDates.getDateTimeMicro(DateTime.UtcNow.AddHours(Convert.ToDouble(timezone))) + ".jpg";
  

            foreach (string fileName in httpFileCollection)
            {
                HttpPostedFile file = httpFileCollection.Get(fileName);


                if (file != null && file.ContentLength > 0)
                {
                    string pathfolder = string.Format("{0}\\upload\\incident\\" + name_folder, Server.MapPath(@"\"));
                    //string pathfolder = string.Format("{0}\\safetys4\\safetys4\\upload\\incident\\" + name_folder, Server.MapPath(@"\"));
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



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateIncident(string incidentdate,
                                       string incidenttime,
                                       string reportdate,
                                       string company_id,
                                       string function_id,
                                       string department_id,
                                       string division_id,
                                       string section_id,
                                       string activity_company_id,
                                       string activity_function_id,
                                       string activity_department_id,
                                       string activity_division_id,
                                       string activity_section_id,
                                       string responsible_area,
                                       string owner_activity,
                                       string incidentarea,
                                       string incidentname,
                                       string incidentdetail,
                                       string user_id,
                                       string typelogin,
                                       string phone,
                                       string incidentid,
                                       string stepform,
                                       string group_id
                                    )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = incidentid;
                bool change_area = false;
                string[] alert_to_groups = new string[10];

                var query = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(incidentid)
                            select c;

                foreach (incident rc in query)
                {

                    rc.incident_date = FormatDates.changeDateTimeDB(incidentdate + " " + incidenttime, Session["lang"].ToString());
                    rc.report_date = FormatDates.changeDateTimeDB(reportdate, Session["lang"].ToString());
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
                    // rc.employee_id = user_id;
                    // rc.typeuser_login = typelogin;
                    rc.phone = phone;
                    // rc.step_form = Convert.ToByte(stepform);
                    rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
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



                    if (Session["country"].ToString()=="thailand")
                    {

                        if (rc.owner_activity != owner_activity)
                        {
                            if(owner_activity=="KNOWN")//ถ้าเปลี่ยนจากไม่ทราบ มาเป็นทราบผู้ควบคุมกิจกรรม ให้ส่งเมล์ใหม่
                            {
                                alert_to_groups[4] = "AreaSuperervisor";
                                change_area = true;
                            }
                            else
                            {
                                alert_to_groups[4] = "";
                            }
                        }

                        if (rc.activity_function_id != activity_function_id)
                        {
                            alert_to_groups[5] = "AdminOH&S";
                            change_area = true;
                        }
                        else
                        {
                            alert_to_groups[5] = "";

                        }

                        if (rc.activity_department_id != activity_department_id)
                        {
                            alert_to_groups[6] = "AreaOH&S";
                            change_area = true;
                        }
                        else
                        {
                            alert_to_groups[6] = "";

                        }

                        if (rc.activity_division_id != activity_division_id)
                        {
                            alert_to_groups[7] = "AreaManager";
                            change_area = true;
                        }
                        else
                        {
                            alert_to_groups[7] = "";

                        }

                        if (rc.activity_section_id != activity_section_id)
                        {
                            alert_to_groups[8] = "AreaSuperervisor";
                            change_area = true;
                        }
                        else
                        {
                            alert_to_groups[8] = "";

                        }

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

                try
                {
                    dbConnect.SubmitChanges();

                    int process_status = 1;//on process
                    byte incident_flow = 1;
                    incident_detail objInsert2 = new incident_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.incident_id = Convert.ToInt32(incidentid);

                    dbConnect.incident_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();



                    if (change_area == true)//change area new to sent notification
                    {
                        ///////////////////////////sent notify by change area////////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        //string[] alert_to_groups = { "AdminOH&S", "AreaSuperervisor", "AreaManager", "AreaOH&S" };
                        sn.InsertNotification(1, Convert.ToInt32(incidentid), alert_to_groups, Session["timezone"].ToString(), "AreaSuperervisor");
                        ////////////////////////////////////end/////////////////////////////////////////////////
                    }

                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
            }
            // return result;

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateIncident2(   string work_relate,
                                       string responsible_area,
                                       string impact,
                                       string injury_fatality_involve,
                                       string effect_environment,
                                     //  string level_environment,
                                      // string level_damange,
                                       List<string> other_impact,
                                       string critical,
                                       string external_reportable,
                                       string immediate_temporary,
                                       string consequence_level,
                                       string currency,
                                       string result_image,
                                       string result_safety,
                                       string result_environment,
                                       string result_damage,
                                       string result_process,
                                       string result_legal,
                                       string result_person,
                                       string user_id,
                                       string typelogin,
                                       string incidentid,
                                       string stepform,
                                       string typebutton,
                                       string group_id
                                       
                                    )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = incidentid;

               

                var query = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(incidentid)
                            select c;

                foreach (incident rc in query)
                {
                    rc.work_relate = work_relate;

                    if (Session["country"].ToString()=="srilanka")
                    {
                        rc.responsible_area = responsible_area;
                    }
                  
                    rc.impact = impact;
                    rc.injury_fatality_involve = injury_fatality_involve;
                    rc.effect_environment = effect_environment;

                    if (typebutton == "report")
                    {
                        rc.submit_report_form = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        rc.submit_report_form2 = Convert.ToInt32(group_id);
                    }
                    else
                    {

                        rc.edit_form2 = Convert.ToInt32(group_id);

                    }



                    deleteOtherImpact(incidentid);
                    foreach (var v in other_impact)
                    {


                        other_impact objInsert = new other_impact();
                        objInsert.other_impact_value = v.ToString();
                        objInsert.incident_id = Convert.ToInt32(incidentid);
                        dbConnect.other_impacts.InsertOnSubmit(objInsert);


                        dbConnect.SubmitChanges();

                    }


                    // rc.other_impact = other_impact;
                    rc.critical = critical;
                    rc.external_reportable = external_reportable;
                    rc.immediate_temporary = immediate_temporary;

                    if (consequence_level != "")
                    {
                        rc.consequence_level = Convert.ToInt32(consequence_level);
                    }

                    rc.currency = currency;
                    // rc.step_form = Convert.ToByte(stepform);
                    rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                    insertHasLevelIncident(result_image,
                                           result_safety,
                                           result_environment,
                                           result_damage,
                                           result_process,
                                           result_legal,
                                           result_person,
                                           incidentid
                                           );



                }

                try
                {
                    dbConnect.SubmitChanges();

                    int process_status = 1;//on process
                    incident_detail objInsert2 = new incident_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.incident_id = Convert.ToInt32(incidentid);

                    dbConnect.incident_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();

                    if (typebutton == "report")//เกิดจากกดปุ่ม submit report
                    {
                        ///มีแก้ template ไปใช้อันใหม่//////////////////////////////////////////////////////////
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();

                        if(Session["country"].ToString()=="thailand")
                        {
                            string[] alert_to_groups = { "AreaOH&S" };
                            sn.InsertNotification(14, Convert.ToInt32(incidentid), alert_to_groups, Session["timezone"].ToString(), "AreaOH&S");
                       
                        }
                        else if (Session["country"].ToString() == "srilanka")
                        {
                            string[] alert_to_groups = { "AreaManager" };
                            sn.InsertNotification(14, Convert.ToInt32(incidentid), alert_to_groups, Session["timezone"].ToString(), "AreaManager");
                       

                        }
                        ///////////////////////////////////end//////////////////////////////////////////////////////

                    }



                }
                catch (Exception e)
                {
                    result = e.Message;
                }

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }


        }




        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateIncident3(   string culpability,
                                       string road_accident,
                                       List<string> root_cause,
                                       string fatality_prevention_element_id,
                                       string faltality_prevention_element_other,
                                       string contributing_factor,
                                       string form2_function_id,
                                       string form3_department_id,
                                       string user_id,
                                       string typelogin,
                                       string incidentid,
                                       string investigation_committee_file,
                                       string group_id
                                      

                                    )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = incidentid;

                var query = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(incidentid)
                            select c;

                foreach (incident rc in query)
                {
                    rc.culpability = culpability;
                    rc.road_accident = road_accident;

                    if (fatality_prevention_element_id != "")
                    {
                        rc.fatality_prevention_element_id = Convert.ToInt32(fatality_prevention_element_id);
                    }
                    rc.faltality_prevention_element_other = faltality_prevention_element_other;


                    if (investigation_committee_file != "")
                    {
                        rc.investigation_committee_file = investigation_committee_file;
                    }

                    rc.contributing_factor = contributing_factor;
                    rc.form2_function_id = form2_function_id;
                    rc.form3_department_id = form3_department_id;

                    deleteRootcause(incidentid);
                    foreach (var v in root_cause)
                    {
                        root_cause_incident objInsert = new root_cause_incident();
                        objInsert.root_cause = v.ToString();
                        objInsert.incident_id = Convert.ToInt32(incidentid);
                        dbConnect.root_cause_incidents.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }

                    rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    rc.action_form_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    rc.edit_form3 = Convert.ToInt32(group_id);
                }

                try
                {
                    dbConnect.SubmitChanges();

                    int process_status = 1;//on process
                    incident_detail objInsert2 = new incident_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.incident_id = Convert.ToInt32(incidentid);

                    dbConnect.incident_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();



                }
                catch (Exception e)
                {
                    result = e.Message;
                }

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createLogRequestCloseIncident( string request_close,
                                                   string reason,
                                                   string employee_id,
                                                   string incidentid,
                                                   string stepform,
                                                   string typelogin,
                                                   string lang,
                                                   string group_id
                                                )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                int status = 1;

                try
                {


                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict = checkConditionCloseIncident(Convert.ToInt32(incidentid), employee_id, lang, request_close, Session["country"].ToString());


                    if (dict["result"] == "true")
                    {


                        log_request_close_incident objInsert = new log_request_close_incident();
                        objInsert.group_id = Convert.ToInt32(dict["group_id"]);
                        objInsert.employee_id = employee_id;
                        objInsert.remark = reason;
                        objInsert.status_process = request_close;
                        objInsert.incident_id = Convert.ToInt32(incidentid);
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.status = "A";

                        dbConnect.log_request_close_incidents.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                        /////////////////////////////////////////////////////////////////////////////////////

                        var q = from c in dbConnect.incidents
                                where c.id == Convert.ToInt32(incidentid)
                                select c;

                        foreach (incident r in q)
                        {
                            r.edit_form4 = Convert.ToInt32(group_id);

                        }

                        dbConnect.SubmitChanges();

                        var g = from c in dbConnect.close_step_incidents
                                where c.step == (Convert.ToInt16(dict["close_step"]) + 1)
                                && c.country == Session["country"].ToString()
                                select c;

                        foreach (var rc in g)
                        {
                            if (request_close == "C")
                            {
                                setGroupEmailStepClose(Convert.ToInt32(rc.group_id), 11, Convert.ToInt32(incidentid), Session["timezone"].ToString(),"incident");
                            }


                        }

                        ///////////////////////////////////////////////////////////////////////////////////////


                        //////////////////////////close incident////////////////////////////////////////////

                        bool result_close_incident = checkToCloseIncident(Convert.ToInt32(incidentid), Session["timezone"].ToString(), Session["country"].ToString());

                        if (result_close_incident)
                        {

                            status = 2;//close
                            var query = from c in dbConnect.incidents
                                        where c.id == Convert.ToInt32(incidentid)
                                        select c;

                            foreach (incident rc in query)
                            {

                                rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                                rc.process_status = status;
                                rc.close_incident_date = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                            }

                            dbConnect.SubmitChanges();



                        }


                        ////////////////////////////end check//////////////////////////////////////////////////////

                        incident_detail objInsert2 = new incident_detail();
                        objInsert2.employee_id = employee_id;
                        objInsert2.type_login = typelogin;
                        objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert2.process_status = status;
                        objInsert2.incident_id = Convert.ToInt32(incidentid);

                        dbConnect.incident_details.InsertOnSubmit(objInsert2);

                        dbConnect.SubmitChanges();



                        if (request_close == "NC")
                        {
                            var query = from c in dbConnect.incidents
                                        where c.id == Convert.ToInt32(incidentid)
                                        select c;

                            foreach (incident rc in query)
                            {
                                rc.step_form = 3;
                                rc.incident_flow = 3;
                                rc.request_close_form3 = null;
                                rc.edit_form3 = null;

                            }

                            dbConnect.SubmitChanges();

                            var gr = from c in dbConnect.log_request_close_incidents
                                     where c.incident_id == Convert.ToInt32(incidentid)
                                     select c;
                            foreach (var a in gr)
                            {
                                a.status = "D";
                            }

                            dbConnect.SubmitChanges();



                            if (Session["country"].ToString() == "thailand")
                            {
                                //////////////////////////////////by p.poo sent notification/////////////////////////////////

                                Class.SafetyNotification sn = new Class.SafetyNotification();
                                string[] alert_to_groups = { "AreaOH&S" };
                                bool send_areamanager = checkSendEmailAreamanager(Convert.ToInt16(group_id));

                                if (send_areamanager)
                                {
                                    alert_to_groups[1] = "AreaManager";
                                }

                                sn.InsertNotification(12, Convert.ToInt32(incidentid), alert_to_groups, Session["timezone"].ToString(), "AreaOH&S");
                                ///////////////////////////////////end//////////////////////////////////////////////////////

                            }
                            else if (Session["country"].ToString() == "srilanka")
                            {                             
                               //////////////////////////////////by p.poo sent notification/////////////////////////////////

                                Class.SafetyNotification sn = new Class.SafetyNotification();
                                string[] alert_to_groups = { "AreaManager" };
                                sn.InsertNotification(12, Convert.ToInt32(incidentid), alert_to_groups, Session["timezone"].ToString(), "AreaManager");
                                ///////////////////////////////////end//////////////////////////////////////////////////////

                            }
                          

                        }

                    }
                    else
                    {


                        result = dict["msg"];

                    }


                }
                catch (Exception e)
                {               
                    action_log objInsert = new action_log();
                    objInsert.function_name = "createLogRequestCloseIncident";
                    objInsert.file_name = "Actionevent";
                    objInsert.error_message = e.Message;
                    objInsert.id = Convert.ToInt32(incidentid);
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                    result = e.Message;
                }


                Context.Response.Write(result);


            }

          
        }




      
        public Dictionary<string, string> checkConditionCloseIncident(int incident_id, string employee_id, string lang, string status_close, string country)
        {
            //check คนเดิมห้ามกรอก
            //check เรียงคนปิด
            //area manager >>admin oh&s >>group oh&s ต้องเป็นคนที่อยู่ใต้ฟังก์ชันของเคสนั้นด้วย area manager>>division
            //ถ้า group oh&s ปกิเสะ จะทำการ reopen ให้กลับไปเริ่ม form 3 ใหม่

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                //check คนเดิมห้ามกรอก
                //check เรียงคนปิด
                //area manager >>admin oh&s >>group oh&s ต้องเป็นคนที่อยู่ใต้ฟังก์ชันของเคสนั้นด้วย area manager>>division
                //ถ้า group oh&s ปกิเสะ จะทำการ reopen ให้กลับไปเริ่ม form 3 ใหม่



                var incidents = from c in dbConnect.incidents
                                //join d in dbConnect.divisions on c.division_id equals d.division_id
                                //join f in dbConnect.functions on c.function_id equals f.function_id
                                //join s in dbConnect.sections on c.section_id equals s.section_id
                                //join de in dbConnect.departments on c.department_id equals de.department_id
                                where c.id == incident_id
                                select new
                                {
                                    function_id = c.function_id,
                                    department_id = c.department_id,
                                    division_id = c.division_id,
                                    section_id = c.section_id,

                                    activity_function_id = c.activity_function_id,
                                    activity_department_id = c.activity_department_id,
                                    activity_division_id = c.activity_division_id,
                                    activity_section_id = c.activity_section_id,
                                    incident_id = c.id,
                                    division_name = chageDataLanguage(c.location_division_name_th, c.location_division_name_en, lang),
                                    function_name = chageDataLanguage(c.location_function_name_th, c.location_function_name_en, lang),
                                    section_name = chageDataLanguage(c.location_section_name_th, c.location_section_name_en, lang),
                                    department_name = chageDataLanguage(c.location_department_name_th, c.location_department_name_en, lang),

                                    activity_division_name = chageDataLanguage(c.activity_location_division_name_th, c.activity_location_division_name_en, lang),
                                    activity_function_name = chageDataLanguage(c.activity_location_function_name_th, c.activity_location_function_name_en, lang),
                                    activity_section_name = chageDataLanguage(c.activity_location_section_name_th, c.activity_location_section_name_en, lang),
                                    activity_department_name = chageDataLanguage(c.activity_location_department_name_th, c.activity_location_department_name_en, lang),

                                    c.owner_activity,
                                    c.responsible_area

                                };
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string result_area_manager = "false";
                string result_ohs_admin = "false";
                string result_group_ohs = "false";
                string result_area_ohs = "false";
                string result_area_supervisor = "false";
                String function_id = "";
                String department_id = "";
                String division_id = "";
                String section_id = "";
                String division_name = "";
                String function_name = "";
                String section_name = "";
                String department_name = "";

                foreach (var v in incidents)
                {

                    if (v.owner_activity == "KNOWN")
                    {
                        function_id = v.activity_function_id;
                        department_id = v.activity_department_id;
                        division_id = v.activity_division_id;
                        section_id = v.activity_section_id;
                        division_name = v.activity_division_name;
                        function_name = v.activity_function_name;
                        section_name = v.activity_section_name;
                        department_name = v.activity_department_name;

                    }
                    else
                    {
                        function_id = v.function_id;
                        department_id = v.department_id;
                        division_id = v.division_id;
                        section_id = v.section_id;
                        division_name = v.division_name;
                        function_name = v.function_name;
                        section_name = v.section_name;
                        department_name = v.department_name;

                    }


                }



                var co = from c in dbConnect.close_step_incidents
                         where c.country == country
                         select c;


                foreach (var o in co)
                {

                    var r = from c in dbConnect.log_request_close_incidents
                            where c.incident_id == Convert.ToInt32(incident_id) & c.status == "A"
                            orderby c.created_at descending
                            select new
                            {
                                id = c.id,
                                c.employee_id,
                                c.status,

                            };


                    if (r.Count() == (o.step - 1))
                    {

                        if (o.group_id == 4)//admin oh&s
                        {
                            if (function_id != "")
                            {
                                var admin_ohs = from c in dbConnect.employees
                                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                                where (b.group_id == 4 || b.group_id == 5) && b.employee_id == employee_id
                                                && b.function_id == function_id
                                                select new
                                                {
                                                    c.employee_id
                                                };
                                //4,5 is admin oh&s and delegate hazard
                                if (admin_ohs.Count() > 0)//ตรวจสอบว่าเป็น admin or delegate oh&s 
                                {
                                    result_ohs_admin = "true";
                                    dict.Add("result", result_ohs_admin);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "4");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_ohs_admin);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ OH&S Admin หรือ Delegate OH&S Admin ที่ดูแล " + function_name + " จึงไม่มีสิทธิ์ในการปิด incident นี้", "You are not OH&S Admin or Delegate OH&S Admin in " + function_name + " so not close this incident.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }
                            }

                        }
                        else if (o.group_id == 8)
                        {
                            var group_ohs = from c in dbConnect.employees
                                            join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                            where (b.group_id == 8) && b.employee_id == employee_id
                                            select new
                                            {
                                                c.employee_id
                                            };
                            //16 is group oh&s hazard
                            if (group_ohs.Count() > 0)//ตรวจสอบว่าเป็น group oh&s 
                            {
                                result_group_ohs = "true";
                                dict.Add("result", result_group_ohs);
                                dict.Add("msg", "");
                                dict.Add("group_id", "8");
                                dict.Add("close_step", o.step.ToString());
                            }
                            else
                            {
                                dict.Add("result", result_group_ohs);
                                dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Group oh&s จึงไม่มีสิทธิ์ในการปิด incident นี้", "You are not group oh&s so not close this incident.", lang));
                                dict.Add("group_id", "");
                                dict.Add("close_step", o.step.ToString());
                            }


                        }
                        else if (o.group_id == 9)
                        {

                            if (department_id != "")//area oh&s
                            {
                                var area_ohs = from c in dbConnect.employee_has_departments
                                               where c.department_id == department_id && c.employee_id == employee_id
                                               select new
                                               {
                                                   c.employee_id
                                               };

                                if (area_ohs.Count() > 0)//ตรวจสอบว่าเป็น area oh&s 
                                {
                                    result_area_ohs = "true";
                                    dict.Add("result", result_area_ohs);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "9");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_area_ohs);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Area oh&s ที่ดูแล " + department_name + " จึงไม่มีสิทธิ์ในการปิด incident นี้", "You are not Area oh&s in " + department_name + " so not close this incident.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }
                            }



                        }
                        else if (o.group_id == 10)
                        {

                            if (division_id != "")
                            {


                                var area_manager = from c in dbConnect.employee_has_divisions
                                                   where c.division_id == division_id && c.employee_id == employee_id
                                                   select new
                                                   {
                                                       c.id
                                                   };



                                if (area_manager.Count() > 0)//ตรวจสอบว่าเป็น area manager .น division ในเคส incident นี้หรือเปล่า
                                {
                                    result_area_manager = "true";
                                    dict.Add("result", result_area_manager);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "10");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_area_manager);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ area manager ที่ดูแล " + division_name + " จึงไม่มีสิทธิ์ในการปิด incident นี้", "You are not area manager in " + division_name + " so not close this incident.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }



                            }


                        }
                        else if (o.group_id == 11)
                        {

                            if (section_id != "")
                            {


                                var area_supervisor = from c in dbConnect.employee_has_sections
                                                      where c.section_id == section_id && c.employee_id == employee_id
                                                      select new
                                                      {
                                                          c.id
                                                      };



                                if (area_supervisor.Count() > 0)//ตรวจสอบว่าเป็น area_supervisor .น section ในเคส incident นี้หรือเปล่า
                                {
                                    result_area_supervisor = "true";
                                    dict.Add("result", result_area_supervisor);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "10");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_area_supervisor);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Area Supervisor ที่ดูแล " + section_name + " จึงไม่มีสิทธิ์ในการปิด incident นี้", "You are not Area Supervisor in " + section_name + " so not close this incident.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }



                            }


                        }
                      

                    }//end step,count


                }//end foreach



                return dict;
            }

        }





        public bool checkToCloseIncident(int incident_id,string timezone,string country)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                //status C is close ,NC is not close
                bool check_close_all = true;

                var d = from c in dbConnect.close_step_incidents
                        where c.country == country
                        select c;


                foreach (var rc in d)
                {

                    var r = from c in dbConnect.log_request_close_incidents
                            where c.incident_id == Convert.ToInt32(incident_id) && c.status_process == "C" && c.status == "A"
                          //  && c.group_id == rc.group_id
                            // orderby c.created_at descending
                            select new
                            {
                                id = c.id,
                                c.employee_id,
                                c.status,
                                c.group_id

                            };

                    if (rc.group_id == 4 || rc.group_id == 5)
                    {
                        r = r.Where(c => c.group_id == 4 || c.group_id == 5);
                    }
                    else
                    {
                        r = r.Where(c => c.group_id == rc.group_id);
                    }

                    if (r.Count() == 0)
                    {
                        check_close_all = false;
                    }

                }

                /////////////////////////////////log////////////////////////////////////////////////////


                //action_log objInsert = new action_log();
                //objInsert.function_name = "checktocloseincident";
                //objInsert.file_name = "Actionevent";
                //objInsert.report_id = Convert.ToInt32(incident_id);
                //objInsert.type_report = "incident";
                //objInsert.area_manager_close = result_area_manager.ToString();
                //objInsert.admin_ohs_close = result_ohs_admin.ToString();
                //objInsert.group_incident_close = result_group_ohs.ToString();
                //objInsert.status = check_close_all.ToString();
                //objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));


                //dbConnect.action_logs.InsertOnSubmit(objInsert);

                //dbConnect.SubmitChanges();


                //////////////////////////////////end log////////////////////////////////////////////



                return check_close_all;

            }

        }



        public bool checkToCloseIncidentSrilanka(int incident_id, string timezone)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                //status C is close ,NC is not close
                bool check_close_all = false;
                bool result_area_ohs = false;
     
                int area_ohs = 9;

                var r = from c in dbConnect.log_request_close_incidents
                        where c.incident_id == Convert.ToInt32(incident_id) && c.status_process == "C" && c.status == "A"
                        orderby c.created_at descending
                        select new
                        {
                            id = c.id,
                            c.employee_id,
                            c.status,
                            c.group_id

                        };


                foreach (var rc in r)
                {
                    if (rc.group_id == area_ohs)
                    {
                        result_area_ohs = true;
                    }

                }


                if (result_area_ohs == true)
                {
                    check_close_all = true;
                }


                return check_close_all;

            }

        }




        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createLogRequestCloseHazard(string request_approve,
                                                   string reason,
                                                   string employee_id,
                                                   string hazardid,
                                                   string stepform,
                                                   string typelogin,
                                                   string lang,
                                                   string group_id
                                                )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                int status = 1;

                try
                {



                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict = checkConditionCloseHazard(Convert.ToInt32(hazardid), employee_id, lang, request_approve, Session["country"].ToString());


                    if (dict["result"] == "true")
                    {


                        log_request_close_hazard objInsert = new log_request_close_hazard();
                        // bool result_group = checkGroupOHSHazard(employee_id);

                        objInsert.group_id = Convert.ToInt32(dict["group_id"]);

                        objInsert.employee_id = employee_id;
                        objInsert.remark = reason;
                        objInsert.status_process = request_approve;
                        objInsert.hazard_id = Convert.ToInt32(hazardid);
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.status = "A";

                        dbConnect.log_request_close_hazards.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                        /////////////////////////////////////////////////////////////////////////////////////

                        var q = from c in dbConnect.hazards
                                where c.id == Convert.ToInt32(hazardid)
                                select c;

                        foreach (hazard r in q)
                        {
                            r.edit_form4 = Convert.ToInt32(dict["group_id"]);

                        }

                        dbConnect.SubmitChanges();




                        var g = from c in dbConnect.close_step_hazards
                                where c.step == (Convert.ToInt16(dict["close_step"]) + 1)
                                && c.country == Session["country"].ToString()
                                select c;

                        foreach (var rc in g)
                        {
                            if (request_approve == "P")
                            {
                                setGroupEmailStepClose(Convert.ToInt32(rc.group_id), 12, Convert.ToInt32(hazardid), Session["timezone"].ToString(),"hazard");
                            }


                        }


                        ///////////////////////////////////////////////////////////////////////////////////////



                        //////////////////////////close incident////////////////////////////////////////////

                        bool result_close_incident = checkToCloseHazard(Convert.ToInt32(hazardid), Session["timezone"].ToString(), Session["country"].ToString());

                        if (result_close_incident)
                        {

                            status = 2;//close
                            var query = from c in dbConnect.hazards
                                        where c.id == Convert.ToInt32(hazardid)
                                        select c;

                            foreach (hazard rc in query)
                            {

                                rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                                rc.process_status = status;
                                rc.close_hazard_date = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                            }

                            dbConnect.SubmitChanges();


                            //////////////////////////////////by p.poo sent notification/////////////////////////////////

                            if (Session["country"].ToString() == "thailand")
                            {
                                Class.SafetyNotification sn = new Class.SafetyNotification();
                                string[] alert_to_groups = { "AreaOH&S" };
                                sn.InsertHazardNotification(14, Convert.ToInt32(hazardid), alert_to_groups, Session["timezone"].ToString(), "AreaOH&S");


                                string[] alert_to_groups2 = { "Reporter" };
                                sn.InsertHazardNotification(21, Convert.ToInt32(hazardid), alert_to_groups2, Session["timezone"].ToString(), "");
                   
                            }
                            ///////////////////////////////////end//////////////////////////////////////////////////////



                        }


                        ////////////////////////////end check//////////////////////////////////////////////////////

                        hazard_detail objInsert2 = new hazard_detail();
                        objInsert2.employee_id = employee_id;
                        objInsert2.type_login = typelogin;
                        objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert2.process_status = status;
                        objInsert2.hazard_id = Convert.ToInt32(hazardid);

                        dbConnect.hazard_details.InsertOnSubmit(objInsert2);

                        dbConnect.SubmitChanges();


                        if (request_approve == "NP")// && dict["group_id"] == "8" ,8 is group oh&s
                        {
                            //reopen to step form 3

                            var query = from c in dbConnect.hazards
                                        where c.id == Convert.ToInt32(hazardid)
                                        select c;

                            foreach (hazard rc in query)
                            {
                                rc.step_form = 3;
                                rc.hazard_flow = 3;
                                rc.request_close_form3 = null;
                                rc.edit_form3 = null;
                            }

                            dbConnect.SubmitChanges();

                            var gr = from c in dbConnect.log_request_close_hazards
                                     where c.hazard_id == Convert.ToInt32(hazardid)
                                     select c;
                            foreach (var a in gr)
                            {
                                a.status = "D";
                            }

                            dbConnect.SubmitChanges();




                            //////////////////////////////////by p.poo sent notification/////////////////////////////////

                            Class.SafetyNotification sn = new Class.SafetyNotification();


                            if (Session["country"].ToString() == "thailand")
                            {
                               
                                string[] alert_to_groups = new string[3];
                                alert_to_groups[0] = "AreaOH&S";
                                alert_to_groups[1] = "AreaSuperervisor";

                                bool send_areamanager = checkSendEmailAreamanager(Convert.ToInt16(group_id));

                                if (send_areamanager)
                                {
                                    alert_to_groups[2] = "AreaManager";
                                }
                                sn.InsertHazardNotification(13, Convert.ToInt32(hazardid), alert_to_groups, Session["timezone"].ToString(), "AreaSuperervisor");
                    
                            }else if(Session["country"].ToString() == "srilanka")
                            {
                                string[] alert_to_groups = { "AreaSuperervisor" };
                                sn.InsertHazardNotification(13, Convert.ToInt32(hazardid), alert_to_groups, Session["timezone"].ToString(), "AreaSuperervisor");
                    
                            }
                                   ///////////////////////////////////end//////////////////////////////////////////////////////



                        }

                    }
                    else
                    {


                        result = dict["msg"];

                    }


                }
                catch (Exception e)
                {              
                    action_log objInsert = new action_log();
                    objInsert.function_name = "createLogRequestCloseHazard";
                    objInsert.file_name = "Actionevent";
                    objInsert.id = Convert.ToInt32(hazardid);
                    objInsert.error_message = e.Message;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);
                    dbConnect.SubmitChanges();

                    result = e.Message;
                }


                Context.Response.Write(result);


            }

        }





        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createLogRequestCloseHealth(string request_approve,
                                                   string reason,
                                                   string employee_id,
                                                   string healthid,
                                                   string stepform,
                                                   string typelogin,
                                                   string lang,
                                                   string group_id
                                                )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "";
                int status = 1;

                try
                {



                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict = checkConditionCloseHealth(Convert.ToInt32(healthid), employee_id, lang, request_approve, Session["country"].ToString());


                    if (dict["result"] == "true")
                    {


                        log_request_close_health objInsert = new log_request_close_health();
                        // bool result_group = checkGroupOHSHazard(employee_id);

                        objInsert.group_id = Convert.ToInt32(dict["group_id"]);

                        objInsert.employee_id = employee_id;
                        objInsert.remark = reason;
                        objInsert.status_process = request_approve;
                        objInsert.health_id = Convert.ToInt32(healthid);
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.status = "A";

                        var logjson = new JavaScriptSerializer().Serialize(objInsert);

                        action_log objInsertLog = new action_log();
                        objInsertLog.function_name = "createLogRequestCloseHealth";
                        objInsertLog.file_name = "Actionevent";
                        objInsertLog.receive = logjson;
                        objInsertLog.report_id = Convert.ToInt32(healthid);
                        objInsertLog.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                        dbConnect.action_logs.InsertOnSubmit(objInsertLog);
                        dbConnect.SubmitChanges();

                        dbConnect.log_request_close_healths.InsertOnSubmit(objInsert);
                        dbConnect.SubmitChanges();

                        /////////////////////////////////////////////////////////////////////////////////////

                        var q = from c in dbConnect.healths
                                where c.id == Convert.ToInt32(healthid)
                                select c;

                        foreach (health r in q)
                        {
                            r.edit_form3 = Convert.ToInt32(dict["group_id"]);

                        }

                        dbConnect.SubmitChanges();




                        var g = from c in dbConnect.close_step_healths
                                where c.step == (Convert.ToInt16(dict["close_step"]) + 1)
                                && c.country == Session["country"].ToString()
                                select c;

                        foreach (var rc in g)
                        {
                            if (request_approve == "P")
                            {
                                setGroupEmailStepClose(Convert.ToInt32(rc.group_id), 2, Convert.ToInt32(healthid), Session["timezone"].ToString(), "health");
                            }


                        }


                        ///////////////////////////////////////////////////////////////////////////////////////



                        //////////////////////////close health////////////////////////////////////////////

                        bool result_close_health = checkToCloseHealth(Convert.ToInt32(healthid), Session["timezone"].ToString(), Session["country"].ToString());

                        if (result_close_health)
                        {

                            status = 2;//close
                            var query = from c in dbConnect.healths
                                        where c.id == Convert.ToInt32(healthid)
                                        select c;

                            foreach (health rc in query)
                            {

                                rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                                rc.process_status = status;
                                rc.close_health_date = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                            }

                            dbConnect.SubmitChanges();


                       


                        }


                        ////////////////////////////end check//////////////////////////////////////////////////////

                        health_detail objInsert2 = new health_detail();
                        objInsert2.employee_id = employee_id;
                        objInsert2.type_login = typelogin;
                        objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert2.process_status = status;
                        objInsert2.health_id = Convert.ToInt32(healthid);

                        dbConnect.health_details.InsertOnSubmit(objInsert2);

                        dbConnect.SubmitChanges();


                        if (request_approve == "NP")// && dict["group_id"] == "8" ,8 is group oh&s
                        {
                            //reopen to step form 3

                            var query = from c in dbConnect.healths
                                        where c.id == Convert.ToInt32(healthid)
                                        select c;

                            foreach (health rc in query)
                            {
                                rc.step_form = 2;
                                rc.request_close_form = null;
                                rc.edit_form3 = null;
                            }

                            dbConnect.SubmitChanges();

                            var gr = from c in dbConnect.log_request_close_healths
                                     where c.health_id == Convert.ToInt32(healthid)
                                     select c;
                            foreach (var a in gr)
                            {
                                a.status = "D";
                            }

                            dbConnect.SubmitChanges();




                            //////////////////////////////////by p.poo sent notification//////h///////////////////////////

                            Class.SafetyNotification sn = new Class.SafetyNotification();


                            if (Session["country"].ToString() == "thailand")//not approve ส่งเมล์หา AreaOH&S
                            {

                                string[] alert_to_groups = new string[1];
                                alert_to_groups[0] = "AreaOH&S";


                                sn.InsertHealthNotification(3, Convert.ToInt32(healthid), alert_to_groups, Session["timezone"].ToString(), "AreaOH&S");

                            }
                          
                            ///////////////////////////////////end//////////////////////////////////////////////////////



                        }

                    }
                    else
                    {


                        result = dict["msg"];

                    }


                }
                catch (Exception e)
                {
                    action_log objInsert = new action_log();
                    objInsert.function_name = "createLogRequestCloseHealth";
                    objInsert.file_name = "Actionevent";
                    objInsert.id = Convert.ToInt32(healthid);
                    objInsert.error_message = e.Message;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);
                    dbConnect.SubmitChanges();

                    result = e.Message;
                }


                Context.Response.Write(result);


            }

        }


        public bool checkSendEmailAreamanager(int group_id)
        {

            Class.SafetyNotification sn = new Class.SafetyNotification();
            bool send_areamanager_email = false;

            if (group_id == 4 || group_id == 5)
            {
                send_areamanager_email = true;
              //  role_action = "AdminOH&S";
            }
            else if (group_id == 8)
            {
                send_areamanager_email = true;
               // role_action = "GroupOH&S";
            }
            else if (group_id == 9)
            {
                send_areamanager_email = true;
              //  role_action = "AreaOH&S";
            }
            else if (group_id == 10)
            {
               
               // role_action = "AreaManager";
            }
            else if (group_id == 11)
            {
                
                //role_action = "AreaSuperervisor";
            }
            else if (group_id == 16)
            {
                send_areamanager_email = true;
               // role_action = "GroupOH&SHazard";
            }



            return send_areamanager_email;


        }



        public void setGroupEmailStepClose(int group_id,int template,int id,string timezone,string type_report)
        {

            Class.SafetyNotification sn = new Class.SafetyNotification();
            string[] alert_to_groups = new string[1];
            string role_action = "";

            if (group_id == 4 || group_id==5)
            {
                alert_to_groups[0] = "AdminOH&S";
                role_action = "AdminOH&S";
            }
            else if (group_id == 8)
            {
                alert_to_groups[0] = "GroupOH&S";
                role_action = "GroupOH&S";
            }
            else if (group_id == 9)
            {
                alert_to_groups[0] = "AreaOH&S";
                role_action = "AreaOH&S";
            }
            else if (group_id == 10)
            {
                alert_to_groups[0] = "AreaManager";
                role_action = "AreaManager";
            }
            else if (group_id == 11)
            {
                alert_to_groups[0] = "AreaSuperervisor";
                role_action = "AreaSuperervisor";
            }
            else if (group_id == 16)
            {
                alert_to_groups[0] = "GroupOH&SHazard";
                role_action = "GroupOH&SHazard";
            }
            else if (group_id == 17)
            {
                alert_to_groups[0] = "GroupOH&SHealth";
                role_action = "GroupOH&SHealth";
            }
            else if (group_id == 18)
            {
                alert_to_groups[0] = "Functional";
                role_action = "Functional";
            }

            if (type_report == "incident")
            {
                sn.InsertNotification(template, id, alert_to_groups, timezone, role_action);

            }
            else if (type_report == "hazard")
            {
                sn.InsertHazardNotification(template, id, alert_to_groups, timezone, role_action);


            }
            else if (type_report == "health")
            {
                sn.InsertHealthNotification(template, id, alert_to_groups, timezone, role_action);


            }

           

        }





    



        public Dictionary<string, string> checkConditionCloseHazard(int hazard_id, string employee_id, string lang, string status_close,string country)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                //check คนเดิมห้ามกรอก
                //check เรียงคนปิด
                //area manager >>admin oh&s >>group oh&s ต้องเป็นคนที่อยู่ใต้ฟังก์ชันของเคสนั้นด้วย area manager>>division
                //ถ้า group oh&s ปกิเสะ จะทำการ reopen ให้กลับไปเริ่ม form 3 ใหม่



                var incidents = from c in dbConnect.hazards
                                //join d in dbConnect.divisions on c.division_id equals d.division_id into joinD
                                //join f in dbConnect.functions on c.function_id equals f.function_id into joinF
                                //join s in dbConnect.sections on c.section_id equals s.section_id into joinS
                                //join de in dbConnect.departments on c.department_id equals de.department_id into joinDe
                                //from d in joinD.DefaultIfEmpty()
                                //from f in joinF.DefaultIfEmpty()
                                //from s in joinS.DefaultIfEmpty()
                                //from de in joinDe.DefaultIfEmpty()
                                where c.id == hazard_id
                                select new
                                {
                                    function_id = c.function_id,
                                    department_id = c.department_id,
                                    division_id = c.division_id,
                                    section_id = c.section_id,
                                    incident_id = c.id,
                                    division_name = chageDataLanguage(c.location_division_name_th, c.location_division_name_en, lang),
                                    function_name = chageDataLanguage(c.location_function_name_th, c.location_function_name_en, lang),
                                    section_name = chageDataLanguage(c.location_section_name_th, c.location_section_name_en, lang),
                                    department_name = chageDataLanguage(c.location_department_name_th,c.location_department_name_en, lang)

                                };
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string result_area_manager = "false";
                string result_ohs_admin = "false";
                string result_group_ohs = "false";
                string result_area_ohs = "false";
                string result_area_supervisor = "false";
                String function_id = "";
                String department_id = "";
                String division_id = "";
                String section_id = "";
                String division_name = "";
                String function_name = "";
                String section_name = "";
                String department_name = "";

                foreach (var v in incidents)
                {
                    function_id = v.function_id;
                    department_id = v.department_id;
                    division_id = v.division_id;
                    section_id = v.section_id;
                    division_name = v.division_name;
                    function_name = v.function_name;
                    section_name = v.section_name;
                    department_name = v.department_name;

                }



                var co = from c in dbConnect.close_step_hazards
                         where c.country == country
                         select c;


                foreach (var o in co)
                {

                    var r = from c in dbConnect.log_request_close_hazards
                            where c.hazard_id == Convert.ToInt32(hazard_id) & c.status == "A"
                            orderby c.created_at descending
                            select new
                            {
                                id = c.id,
                                c.employee_id,
                                c.status,

                            };


                    if (r.Count() == (o.step-1))
                    {
                       
                        if (o.group_id == 4)//admin oh&s
                        {
                            if (function_id != "")
                            {
                                var admin_ohs = from c in dbConnect.employees
                                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                                where (b.group_id == 4 || b.group_id == 5) && b.employee_id == employee_id
                                                && b.function_id == function_id
                                                select new
                                                {
                                                    c.employee_id
                                                };
                                //4,5 is admin oh&s and delegate hazard
                                if (admin_ohs.Count() > 0)//ตรวจสอบว่าเป็น admin or delegate oh&s 
                                {
                                    result_ohs_admin = "true";
                                    dict.Add("result", result_ohs_admin);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "4");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_ohs_admin);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ OH&S Admin หรือ Delegate OH&S Admin ที่ดูแล " + function_name + " จึงไม่มีสิทธิ์ในการปิด hazard นี้", "You are not OH&S Admin or Delegate OH&S Admin in " + function_name + " so not close this hazard.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }
                            }

                        }
                        else if (o.group_id == 9)
                        {

                            if (department_id != "")//area oh&s
                            {
                                var area_ohs = from c in dbConnect.employee_has_departments
                                               where c.department_id == department_id && c.employee_id == employee_id
                                               select new
                                               {
                                                   c.employee_id
                                               };

                                if (area_ohs.Count() > 0)//ตรวจสอบว่าเป็น area oh&s 
                                {
                                    result_area_ohs = "true";
                                    dict.Add("result", result_area_ohs);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "9");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_area_ohs);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Area oh&s ที่ดูแล " + department_name + " จึงไม่มีสิทธิ์ในการปิด hazard นี้", "You are not Area oh&s in " + department_name + " so not close this hazard.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }
                            }



                        }
                        else if (o.group_id == 10)
                        {

                            if (division_id != "")
                            {


                                var area_manager = from c in dbConnect.employee_has_divisions
                                                   where c.division_id == division_id && c.employee_id == employee_id
                                                   select new
                                                   {
                                                       c.id
                                                   };



                                if (area_manager.Count() > 0)//ตรวจสอบว่าเป็น area manager .น division ในเคส incident นี้หรือเปล่า
                                {
                                    result_area_manager = "true";
                                    dict.Add("result", result_area_manager);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "10");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_area_manager);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ area manager ที่ดูแล " + division_name + " จึงไม่มีสิทธิ์ในการปิด hazard นี้", "You are not area manager in " + division_name + " so not close this hazard.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }



                            }


                        }
                        else if (o.group_id == 11)
                        {

                            if (section_id != "")
                            {


                                var area_supervisor = from c in dbConnect.employee_has_sections
                                                   where c.section_id == section_id && c.employee_id == employee_id
                                                   select new
                                                   {
                                                       c.id
                                                   };



                                if (area_supervisor.Count() > 0)//ตรวจสอบว่าเป็น area_supervisor .น section ในเคส incident นี้หรือเปล่า
                                {
                                    result_area_supervisor = "true";
                                    dict.Add("result", result_area_supervisor);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "10");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_area_supervisor);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Area Supervisor ที่ดูแล " + section_name + " จึงไม่มีสิทธิ์ในการปิด hazard นี้", "You are not Area Supervisor in " + section_name + " so not close this hazard.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }



                            }


                        }
                        else if (o.group_id == 16)
                        {
                            var group_ohs = from c in dbConnect.employees
                                            join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                            where (b.group_id == 16) && b.employee_id == employee_id
                                            select new
                                            {
                                                c.employee_id
                                            };
                            //16 is group oh&s hazard
                            if (group_ohs.Count() > 0)//ตรวจสอบว่าเป็น group oh&s 
                            {
                                result_group_ohs = "true";
                                dict.Add("result", result_group_ohs);
                                dict.Add("msg", "");
                                dict.Add("group_id", "16");
                                dict.Add("close_step", o.step.ToString());
                            }
                            else
                            {
                                dict.Add("result", result_group_ohs);
                                dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Group oh&s จึงไม่มีสิทธิ์ในการปิด hazard นี้", "You are not group oh&s so not close this hazard.", lang));
                                dict.Add("group_id", "");
                                dict.Add("close_step", o.step.ToString());
                            }


                        }

                    }//end step,count


                }//end foreach



                return dict;
            }

        }










        public Dictionary<string, string> checkConditionCloseHealth(int health_id, string employee_id, string lang, string status_close, string country)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                //check คนเดิมห้ามกรอก
                //check เรียงคนปิด
                //functional >>group oh&s health 
      



                var incidents = from c in dbConnect.healths
                                where c.id == health_id
                                select new
                                {
                                    function_id = c.function_id,
                                    department_id = c.department_id,
                                    division_id = c.division_id,
                                    section_id = c.section_id,
                                    incident_id = c.id,
                                    division_name = chageDataLanguage(c.location_division_name_th, c.location_division_name_en, lang),
                                    function_name = chageDataLanguage(c.location_function_name_th, c.location_function_name_en, lang),
                                    section_name = chageDataLanguage(c.location_section_name_th, c.location_section_name_en, lang),
                                    department_name = chageDataLanguage(c.location_department_name_th, c.location_department_name_en, lang)

                                };
                Dictionary<string, string> dict = new Dictionary<string, string>();
                string result_area_manager = "false";
                string result_ohs_admin = "false";
                string result_group_ohs = "false";
                string result_area_ohs = "false";
                string result_area_supervisor = "false";
                string result_functional = "false";
                string result_group_health = "false";
                String function_id = "";
                String department_id = "";
                String division_id = "";
                String section_id = "";
                String division_name = "";
                String function_name = "";
                String section_name = "";
                String department_name = "";

                foreach (var v in incidents)
                {
                    function_id = v.function_id;
                    department_id = v.department_id;
                    division_id = v.division_id;
                    section_id = v.section_id;
                    division_name = v.division_name;
                    function_name = v.function_name;
                    section_name = v.section_name;
                    department_name = v.department_name;

                }



                var co = from c in dbConnect.close_step_healths
                         where c.country == country
                         select c;


                foreach (var o in co)
                {

                    var r = from c in dbConnect.log_request_close_healths
                            where c.health_id == Convert.ToInt32(health_id) & c.status == "A"
                            orderby c.created_at descending
                            select new
                            {
                                id = c.id,
                                c.employee_id,
                                c.status,

                            };


                    if (r.Count() == (o.step - 1))
                    {

                        if (o.group_id == 4)//admin oh&s
                        {
                            if (function_id != "")
                            {
                                var admin_ohs = from c in dbConnect.employees
                                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                                where (b.group_id == 4 || b.group_id == 5) && b.employee_id == employee_id
                                                && b.function_id == function_id
                                                select new
                                                {
                                                    c.employee_id
                                                };
                                //4,5 is admin oh&s and delegate hazard
                                if (admin_ohs.Count() > 0)//ตรวจสอบว่าเป็น admin or delegate oh&s 
                                {
                                    result_ohs_admin = "true";
                                    dict.Add("result", result_ohs_admin);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "4");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_ohs_admin);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ OH&S Admin หรือ Delegate OH&S Admin ที่ดูแล " + function_name + " จึงไม่มีสิทธิ์ในการปิด hazard นี้", "You are not OH&S Admin or Delegate OH&S Admin in " + function_name + " so not close this hazard.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }
                            }

                        }
                        else if (o.group_id == 9)
                        {

                            if (department_id != "")//area oh&s
                            {
                                var area_ohs = from c in dbConnect.employee_has_departments
                                               where c.department_id == department_id && c.employee_id == employee_id
                                               select new
                                               {
                                                   c.employee_id
                                               };

                                if (area_ohs.Count() > 0)//ตรวจสอบว่าเป็น area oh&s 
                                {
                                    result_area_ohs = "true";
                                    dict.Add("result", result_area_ohs);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "9");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_area_ohs);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Area oh&s ที่ดูแล " + department_name + " จึงไม่มีสิทธิ์ในการปิด hazard นี้", "You are not Area oh&s in " + department_name + " so not close this hazard.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }
                            }



                        }
                        else if (o.group_id == 10)
                        {

                            if (division_id != "")
                            {


                                var area_manager = from c in dbConnect.employee_has_divisions
                                                   where c.division_id == division_id && c.employee_id == employee_id
                                                   select new
                                                   {
                                                       c.id
                                                   };



                                if (area_manager.Count() > 0)//ตรวจสอบว่าเป็น area manager .น division ในเคส incident นี้หรือเปล่า
                                {
                                    result_area_manager = "true";
                                    dict.Add("result", result_area_manager);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "10");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_area_manager);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ area manager ที่ดูแล " + division_name + " จึงไม่มีสิทธิ์ในการปิด hazard นี้", "You are not area manager in " + division_name + " so not close this hazard.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }



                            }


                        }
                        else if (o.group_id == 11)
                        {

                            if (section_id != "")
                            {


                                var area_supervisor = from c in dbConnect.employee_has_sections
                                                      where c.section_id == section_id && c.employee_id == employee_id
                                                      select new
                                                      {
                                                          c.id
                                                      };



                                if (area_supervisor.Count() > 0)//ตรวจสอบว่าเป็น area_supervisor .น section ในเคส incident นี้หรือเปล่า
                                {
                                    result_area_supervisor = "true";
                                    dict.Add("result", result_area_supervisor);
                                    dict.Add("msg", "");
                                    dict.Add("group_id", "10");
                                    dict.Add("close_step", o.step.ToString());
                                }
                                else
                                {
                                    dict.Add("result", result_area_supervisor);
                                    dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Area Supervisor ที่ดูแล " + section_name + " จึงไม่มีสิทธิ์ในการปิด hazard นี้", "You are not Area Supervisor in " + section_name + " so not close this hazard.", lang));
                                    dict.Add("group_id", "");
                                    dict.Add("close_step", o.step.ToString());
                                }



                            }


                        }
                        else if (o.group_id == 16)
                        {
                            var group_ohs = from c in dbConnect.employees
                                            join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                            where (b.group_id == 16) && b.employee_id == employee_id
                                            select new
                                            {
                                                c.employee_id
                                            };
                            //16 is group oh&s hazard
                            if (group_ohs.Count() > 0)//ตรวจสอบว่าเป็น group oh&s 
                            {
                                result_group_ohs = "true";
                                dict.Add("result", result_group_ohs);
                                dict.Add("msg", "");
                                dict.Add("group_id", "16");
                                dict.Add("close_step", o.step.ToString());
                            }
                            else
                            {
                                dict.Add("result", result_group_ohs);
                                dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Group oh&s จึงไม่มีสิทธิ์ในการปิด hazard นี้", "You are not group oh&s so not close this hazard.", lang));
                                dict.Add("group_id", "");
                                dict.Add("close_step", o.step.ToString());
                            }


                        }
                        else if (o.group_id == 17)
                        {
                            var group_ohs_health = from c in dbConnect.employees
                                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                                where (b.group_id == 17) && b.employee_id == employee_id
                                                select new
                                                {
                                                    c.employee_id
                                                };
                            
                            if (group_ohs_health.Count() > 0)
                            {
                                result_group_health = "true";
                                dict.Add("result", result_group_health);
                                dict.Add("msg", "");
                                dict.Add("group_id", "17");
                                dict.Add("close_step", o.step.ToString());
                            }
                            else
                            {
                                dict.Add("result", result_group_health);
                                dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Group oh&s จึงไม่มีสิทธิ์ในการปิด health นี้", "You are not group oh&s so not close this health.", lang));
                                dict.Add("group_id", "");
                                dict.Add("close_step", o.step.ToString());
                            }


                        }
                        else if (o.group_id == 18)
                        {
                            var functional = from c in dbConnect.employee_has_department_functional_managers
                                           where c.department_id == department_id && c.employee_id == employee_id
                                           select new
                                           {
                                               c.employee_id
                                           };

                            
                            if (functional.Count() > 0)
                            {
                                result_functional = "true";
                                dict.Add("result", result_functional);
                                dict.Add("msg", "");
                                dict.Add("group_id", "18");
                                dict.Add("close_step", o.step.ToString());
                            }
                            else
                            {
                                dict.Add("result", result_functional);
                                dict.Add("msg", chageDataLanguage("คุณไม่ใช่ Functional จึงไม่มีสิทธิ์ในการปิด health นี้", "You are not functional so not close this health.", lang));
                                dict.Add("group_id", "");
                                dict.Add("close_step", o.step.ToString());
                            }


                        }

                    }//end step,count


                }//end foreach



                return dict;
            }

        }





        public bool checkToCloseHazard(int hazard_id,string timezone,string country)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                //status P is ppprove ,NP is not approve
                bool check_close_all = true;

                var d = from c in dbConnect.close_step_hazards
                        where c.country == country
                        select c;


                foreach (var rc in d)
                {

                    var r = from c in dbConnect.log_request_close_hazards
                            where c.hazard_id == Convert.ToInt32(hazard_id) && c.status_process == "P" && c.status == "A"
                            //&& c.group_id == rc.group_id
                           // orderby c.created_at descending
                            select new
                            {
                                id = c.id,
                                c.employee_id,
                                c.status,
                                c.group_id

                            };

                    if (rc.group_id == 4 || rc.group_id == 5)
                    {
                        r = r.Where(c => c.group_id == 4 || c.group_id == 5);
                    }
                    else
                    {
                        r = r.Where(c => c.group_id == rc.group_id);
                    }

                    if (r.Count() == 0)
                    {
                        check_close_all = false;
                    }

                }

               


                /////////////////////////////////log////////////////////////////////////////////////////


                //action_log objInsert = new action_log();
                //objInsert.function_name = "checktoclosehazard";
                //objInsert.file_name = "Actionevent";
                //objInsert.report_id = Convert.ToInt32(hazard_id);
                //objInsert.type_report = "hazard";
                //objInsert.area_manager_close = result_area_manager.ToString();

                //objInsert.group_hazard_close = result_group_ohs.ToString();
                //objInsert.status = check_close_all.ToString();
                //objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));


                //dbConnect.action_logs.InsertOnSubmit(objInsert);

                //dbConnect.SubmitChanges();


                //////////////////////////////////end log////////////////////////////////////////////


                return check_close_all;
            }

        }




        public bool checkToCloseHealth(int health_id, string timezone, string country)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                //status P is ppprove ,NP is not approve
                bool check_close_all = true;

                var d = from c in dbConnect.close_step_healths
                        where c.country == country
                        select c;


                foreach (var rc in d)
                {

                    var r = from c in dbConnect.log_request_close_healths
                            where c.health_id == Convert.ToInt32(health_id) && c.status_process == "P" && c.status == "A"
                            //&& c.group_id == rc.group_id
                            // orderby c.created_at descending
                            select new
                            {
                                id = c.id,
                                c.employee_id,
                                c.status,
                                c.group_id

                            };

                    if (rc.group_id == 4 || rc.group_id == 5)
                    {
                        r = r.Where(c => c.group_id == 4 || c.group_id == 5);
                    }
                    else
                    {
                        r = r.Where(c => c.group_id == rc.group_id);
                    }

                    if (r.Count() == 0)
                    {
                        check_close_all = false;
                    }

                }



                return check_close_all;
            }

        }


       

        public void insertHasLevelIncident( string result_image,
                                           string result_safety,
                                           string result_environment,
                                           string result_damage,
                                           string result_process,
                                           string result_legal,
                                           string result_person,
                                           string incidentid
                                          )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                deleteLevelIncident(incidentid);


                // if (result_image != "0")
                // {
                level_has_definition_level_incident objInsert = new level_has_definition_level_incident();

                objInsert.level_incident = findLevel(result_image);
                objInsert.definition_level_incident_id = 1;//1
                objInsert.incident_id = Convert.ToInt32(incidentid);
                dbConnect.level_has_definition_level_incidents.InsertOnSubmit(objInsert);
                dbConnect.SubmitChanges();

                //  }


                // if (result_safety != "0")
                // {
                level_has_definition_level_incident objInsert2 = new level_has_definition_level_incident();
                objInsert2.level_incident = findLevel(result_safety);
                objInsert2.definition_level_incident_id = 2;
                objInsert2.incident_id = Convert.ToInt32(incidentid);
                dbConnect.level_has_definition_level_incidents.InsertOnSubmit(objInsert2);
                dbConnect.SubmitChanges();
                //   }

                // if (result_environment != "0")
                // {
                level_has_definition_level_incident objInsert3 = new level_has_definition_level_incident();
                objInsert3.level_incident = findLevel(result_environment);
                objInsert3.definition_level_incident_id = 3;
                objInsert3.incident_id = Convert.ToInt32(incidentid);
                dbConnect.level_has_definition_level_incidents.InsertOnSubmit(objInsert3);
                dbConnect.SubmitChanges();
                // }


                // if (result_damage != "0")
                // {
                level_has_definition_level_incident objInsert4 = new level_has_definition_level_incident();
                objInsert4.level_incident = findLevel(result_damage);
                objInsert4.definition_level_incident_id = 4;
                objInsert4.incident_id = Convert.ToInt32(incidentid);
                dbConnect.level_has_definition_level_incidents.InsertOnSubmit(objInsert4);
                dbConnect.SubmitChanges();
                //  }

                //  if (result_process != "0")
                //  {
                level_has_definition_level_incident objInsert5 = new level_has_definition_level_incident();
                objInsert5.level_incident = findLevel(result_process);
                objInsert5.definition_level_incident_id = 5;
                objInsert5.incident_id = Convert.ToInt32(incidentid);
                dbConnect.level_has_definition_level_incidents.InsertOnSubmit(objInsert5);
                dbConnect.SubmitChanges();
                // }


                // if (result_legal != "0")
                // {
                level_has_definition_level_incident objInsert6 = new level_has_definition_level_incident();
                objInsert6.level_incident = findLevel(result_legal);
                objInsert6.definition_level_incident_id = 6;
                objInsert6.incident_id = Convert.ToInt32(incidentid);
                dbConnect.level_has_definition_level_incidents.InsertOnSubmit(objInsert6);
                dbConnect.SubmitChanges();
                // }

                //if(result_person != "0")
                //{
                level_has_definition_level_incident objInsert7 = new level_has_definition_level_incident();
                objInsert7.level_incident = findLevel(result_person);
                objInsert7.definition_level_incident_id = 7;
                objInsert7.incident_id = Convert.ToInt32(incidentid);
                dbConnect.level_has_definition_level_incidents.InsertOnSubmit(objInsert7);
                dbConnect.SubmitChanges();
                /// }
            }
                
        }

        public byte findLevel(string v)//แปลงค่าจากตารางเป็น level มันเรียงสลับกัน
        {
            byte result = 0;

            if (v == "5")
            {
                result = 1;//level

            }else if(v == "4")
            {
                result = 2;//level

            }else if(v=="3")
            {
                result = 3;//level

            }else if(v=="2")
            {
                result = 4;//level

            }else if(v=="1")
            {

                result = 5;//level
            }
            return result;
        }


        public string findColumn(string v)//แปลงค่ level เป็นค่าโชว์ในตาราง มันเรียงสลับกัน
        {
            string result = "0";

            if (v == "1")
            {
                result = "5";

            }
            else if (v == "2")
            {
                result = "4";

            }
            else if (v == "3")
            {
                result = "3";

            }
            else if (v == "4")
            {
                result = "2";

            }
            else if (v == "5")
            {

                result = "1";
            }
            return result;
        }


        public string getLevelTable(string incident_id,string level_definition_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "0";

                var gr = from c in dbConnect.level_has_definition_level_incidents
                         where c.incident_id == Convert.ToInt32(incident_id) &&
                         c.definition_level_incident_id == Convert.ToInt32(level_definition_id)
                         select c;
                foreach (var r in gr)
                {
                    result = findColumn(r.level_incident.ToString());

                }

                return result;
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

        public void deleteRootcause(string incident_id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.root_cause_incidents
                         where c.incident_id == Convert.ToInt32(incident_id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.root_cause_incidents.DeleteOnSubmit(a);
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


        public void deleteLevelIncident(string incident_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.level_has_definition_level_incidents
                         where c.incident_id == Convert.ToInt32(incident_id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.level_has_definition_level_incidents.DeleteOnSubmit(a);
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
        public void updateReasonRejectIncident(
                                               string incidentid,
                                               string reason_reject_type,
                                               string reasonreject,
                                               string userid,
                                               string typelogin,
                                               string step_form,
                                               string group_id
                                            )
        {
           
                
                using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
                {
                    string result = incidentid;
                    int status = 3;//reject

                    // dbConnect.Log = Console.Out;

                    var query = from c in dbConnect.incidents
                                where c.id == Convert.ToInt32(incidentid)
                                select c;

                    foreach (incident rc in query)
                    {
                        rc.reason_reject_type = Convert.ToInt16(reason_reject_type);
                        rc.reason_reject = reasonreject;
                        rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        rc.process_status = status;
                        //rc.work_relate = "";//เผื่อเค้าลง saveเพราะ default คือ Y
                        if(step_form=="1")
                        {
                            rc.reject_report_form1 = Convert.ToInt32(group_id);

                        }else{

                            rc.reject_report_form2 = Convert.ToInt32(group_id);

                        }
                       
                    }

                   
                    try
                    {

                        dbConnect.SubmitChanges();

                        incident_detail objInsert2 = new incident_detail();
                        objInsert2.employee_id = userid;
                        objInsert2.type_login = typelogin;
                        objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert2.process_status = status;
                        objInsert2.incident_id = Convert.ToInt32(incidentid);

                        dbConnect.incident_details.InsertOnSubmit(objInsert2);

                        dbConnect.SubmitChanges();
                        dbConnect.Dispose();

                        if (Session["country"].ToString() == "srilanka")
                        {
                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            string[] alert_to_groups = { "AreaOH&S", "AreaSuperervisor" };//, "AdminOH&S", "GroupOH&S" 
                            sn.InsertNotification(19, Convert.ToInt32(incidentid), alert_to_groups, Session["timezone"].ToString(), "AreaOH&S");
                            ///////////////////////////////////end//////////////////////////////////////////////////////

                        }
                        else if (Session["country"].ToString() == "thailand")//พี่ออยสั่งปิดไม่ส่งแล้ว 24.01.2018   สั่งเปิดใหม่ 22.07.2019
                        {
                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            string[] alert_to_groups = { "GroupOH&S" };//, "AdminOH&S", "GroupOH&S" 
                            sn.InsertNotification(19, Convert.ToInt32(incidentid), alert_to_groups, Session["timezone"].ToString(), "");
                            ///////////////////////////////////end//////////////////////////////////////////////////////



                        }

                    }
                    catch (Exception e)
                    {
                        action_log objInsert = new action_log();
                        objInsert.function_name = "updateReasonRejectIncident";
                        objInsert.file_name = "Actionevent";
                        objInsert.error_message = e.Message;
                        objInsert.id = Convert.ToInt32(incidentid);
                        objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                        dbConnect.action_logs.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }

                    Context.Response.Output.Write(result);
                }
              
          


           
            // return result;

        }







        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateReasonRejectHealth(
                                               string healthid,
                                               string reason_reject_type,
                                               string reasonreject,
                                               string userid,
                                               string typelogin,
                                               string step_form,
                                               string group_id
                                            )
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = healthid;
                int status = 3;//reject

                var query = from c in dbConnect.healths
                            where c.id == Convert.ToInt32(healthid)
                            select c;

                foreach (health rc in query)
                {
                    rc.reason_reject_type = Convert.ToInt16(reason_reject_type);
                    rc.reason_reject = reasonreject;
                    rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    rc.process_status = status;
                    rc.reject_report_form1 = Convert.ToInt32(group_id);

                }


                try
                {

                    dbConnect.SubmitChanges();

                    health_detail objInsert2 = new health_detail();
                    objInsert2.employee_id = userid;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = status;
                    objInsert2.health_id = Convert.ToInt32(healthid);

                    dbConnect.health_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();
                    dbConnect.Dispose();
                  

                }
                catch (Exception e)
                {
                    action_log objInsert = new action_log();
                    objInsert.function_name = "updateReasonRejectHealth";
                    objInsert.file_name = "Actionevent";
                    objInsert.error_message = e.Message;
                    objInsert.id = Convert.ToInt32(healthid);
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();
                }

                Context.Response.Output.Write(result);
            }



        }





        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateReasonExceptIncident(
                                               string incidentid,
                                               string reason_except_type,
                                               string reasonexcept,
                                               string userid,
                                               string typelogin,
                                               string step_form,
                                               string group_id
                                            )
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = incidentid;
                int status = 4;//Exemption


                var query = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(incidentid)
                            select c;

                foreach (incident rc in query)
                {
                    rc.reason_except_type = Convert.ToInt16(reason_except_type);
                    rc.reason_except = reasonexcept;
                    rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    rc.process_status = status;
               

                }


                try
                {

                    dbConnect.SubmitChanges();

                    incident_detail objInsert2 = new incident_detail();
                    objInsert2.employee_id = userid;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = status;
                    objInsert2.incident_id = Convert.ToInt32(incidentid);

                    dbConnect.incident_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();
                    dbConnect.Dispose();

                    if (Session["country"].ToString() == "thailand")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { "GroupOH&S" };//, "AdminOH&S", "GroupOH&S" 
                        sn.InsertNotification(24, Convert.ToInt32(incidentid), alert_to_groups, Session["timezone"].ToString(), "");
                        ///////////////////////////////////end//////////////////////////////////////////////////////



                    }

                }
                catch (Exception e)
                {
                    action_log objInsert = new action_log();
                    objInsert.function_name = "updateReasonExceptIncident";
                    objInsert.file_name = "Actionevent";
                    objInsert.error_message = e.Message;
                    objInsert.id = Convert.ToInt32(incidentid);
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();
                }

                Context.Response.Output.Write(result);
            }





            // return result;

        }




        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateReasonRejectHazard(
                                               string hazardid,
                                               string reason_reject_type,
                                               string reasonreject,
                                               string userid,
                                               string typelogin,
                                               string step_form,
                                               string group_id
                                            )
        {

            
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = hazardid;
                int status = 3;//reject


                // dbConnect.Log = Console.Out;

                var query = from c in dbConnect.hazards
                            where c.id == Convert.ToInt32(hazardid)
                            select c;

                foreach (hazard rc in query)
                {
                    rc.reason_reject_type = Convert.ToInt16(reason_reject_type);
                    rc.reason_reject = reasonreject;
                    rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
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


                try
                {

                    dbConnect.SubmitChanges();

                    hazard_detail objInsert2 = new hazard_detail();
                    objInsert2.employee_id = userid;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = status;
                    objInsert2.hazard_id = Convert.ToInt32(hazardid);

                    dbConnect.hazard_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();
                    dbConnect.Dispose();


                    if (Session["country"].ToString() == "srilanka")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////

                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { "AreaOH&S" };
                        sn.InsertHazardNotification(16, Convert.ToInt32(hazardid), alert_to_groups, Session["timezone"].ToString(), "AreaOH&S");
                        ///////////////////////////////////end//////////////////////////////////////////////////////

                    }
                    else if (Session["country"].ToString() == "thailand")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////

                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { "Reporter","GroupOH&SHazard" };
                        sn.InsertHazardNotification(16, Convert.ToInt32(hazardid), alert_to_groups, Session["timezone"].ToString(), "Reporter");
                        ///////////////////////////////////end//////////////////////////////////////////////////////


                    }
                  


                }
                catch (Exception e)
                {
                    action_log objInsert = new action_log();
                    objInsert.function_name = "updateReasonRejectHazard";
                    objInsert.file_name = "Actionevent";
                    objInsert.error_message = e.Message;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();
                }


                Context.Response.Output.Write(result);
            }




            // return result;

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getIncidentbyid(string id, string lang)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var q = from c in dbConnect.incidents
                        //join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                        //from e in joinE.DefaultIfEmpty()
                        join s in dbConnect.incident_status on c.process_status equals s.id
                        where c.id == Convert.ToInt32(id)
                        select new
                        {
                            incident_datetime = c.incident_date,
                            report_date = c.report_date,
                            c.company_id,
                            c.function_id,
                            c.department_id,
                            c.division_id,
                            c.section_id,
                            c.activity_company_id,
                            c.activity_function_id,
                            c.activity_department_id,
                            c.activity_division_id,
                            c.activity_section_id,
                            c.incident_area,
                            c.incident_name,
                            c.incident_detail,
                            c.employee_id,
                            c.process_status,
                            c.typeuser_login,
                            c.doc_no,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            phone = c.phone,
                            c.work_relate,
                            c.responsible_area,
                            c.owner_activity,
                            c.impact,
                            c.injury_fatality_involve,
                            c.effect_environment,
                            c.level_environment,
                            c.level_damange,
                            c.other_impact,
                            c.critical,
                            c.external_reportable,
                            c.immediate_temporary,
                            c.consequence_level,
                            c.currency,
                            c.culpability,
                            c.road_accident,
                            c.fatality_prevention_element_id,
                            c.faltality_prevention_element_other,
                            c.contributing_factor,
                            c.form2_function_id,
                            c.form3_department_id,
                            c.step_form,
                            c.submit_report_form2,
                            c.confirm_investigate_form2,
                            c.confirm_by_groupohs_form2,
                            c.request_close_form3,
                            c.id,
                            c.location_company_name_en,
                            c.location_company_name_th,
                            c.location_function_name_en,
                            c.location_function_name_th,
                            c.location_department_name_en,
                            c.location_department_name_th,
                            c.location_division_name_en,
                            c.location_division_name_th,
                            c.location_section_name_en,
                            c.location_section_name_th,
                            c.activity_location_company_name_en,
                            c.activity_location_company_name_th,
                            c.activity_location_function_name_en,
                            c.activity_location_function_name_th,
                            c.activity_location_department_name_en,
                            c.activity_location_department_name_th,
                            c.activity_location_division_name_en,
                            c.activity_location_division_name_th,
                            c.activity_location_section_name_en,
                            c.activity_location_section_name_th,
                            c.reporter_company_id,
                            c.reporter_function_id,
                            c.reporter_department_id,
                            c.reporter_division_id,
                            c.reporter_section_id

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
                    string code_status = "";
                    if (v.process_status == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";

                    }
                    //else if(v.process_status == 2){//delay

                    //    code_status = "<i class=\"fa fa-circle text-danger\"></i>";

                    else if (v.process_status == 3)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-reject\"></i>";
                    }
                    else if (v.process_status == 2)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i>";

                    }
                    else if (v.process_status == 4)
                    {//exemption

                        code_status = "<i class=\"fa fa-circle text-reject\"></i>";
                    }


                    string step = "";


                    if (v.process_status != 2 && v.process_status != 3 && v.process_status != 4)//ไม่ใช้ close กับ reject and exemption
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

                            if (Session["country"].ToString() == "thailand")
                            {
                                if (v.submit_report_form2 != null && v.confirm_investigate_form2 == null)
                                {
                                    step = step + "(" + v_step + " - Area OH&S)";
                                }
                            }
                            else if (Session["country"].ToString() == "srilanka")
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

                            if (Session["country"].ToString() == "thailand")
                            {
                                step = step + "(" + v_step + " - Area OH&S)";
                            }
                            else if (Session["country"].ToString() == "srilanka")
                            {
                                step = step + "(" + v_step + " - Area Manager)";
                            }



                        }
                        else if (v.step_form == 4)
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
                                        where c.incident_id == v.id && c.status == "A"
                                       // && c.group_id == r.group_id
                                        select c;

                                if (r.group_id == 4 || r.group_id == 5)
                                {
                                    w = w.Where(c => c.group_id == 4 || c.group_id ==5);
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
                    var p = from c in dbConnect.root_cause_incidents
                            where c.incident_id == Convert.ToInt32(id)
                            select new
                            {
                                c.root_cause

                            };

                    foreach (var m in p)
                    {
                        cr.Add(m.root_cause);
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

                        activity_company_id = v.activity_company_id,
                        activity_function_id = v.activity_function_id,
                        activity_department_id = v.activity_department_id,
                        activity_division_id = v.activity_division_id,
                        activity_section_id = v.activity_section_id,
                        incident_area = v.incident_area,
                        incident_name = v.incident_name,
                        incident_detail = v.incident_detail,
                        name_modify = user_name_modify,
                        datetime_modify = datetime_modify,
                        phone = v.phone,
                        status = code_status + " " + v.status + " " + step,
                        employee_name = getEmployeeByTypeLogin(v.employee_id, v.typeuser_login, lang),
                        doc_no = v.doc_no,
                        v.work_relate,
                        v.responsible_area,
                        v.owner_activity,
                        v.impact,
                        v.injury_fatality_involve,
                        v.effect_environment,
                        v.level_environment,
                        v.level_damange,
                        other_impact = im,
                        v.critical,
                        v.external_reportable,
                        v.immediate_temporary,
                        v.consequence_level,
                        v.currency,
                        result_image = getLevelTable(id, "1"),
                        result_safety = getLevelTable(id, "2"),
                        result_environment = getLevelTable(id, "3"),
                        result_damage = getLevelTable(id, "4"),
                        result_process = getLevelTable(id, "5"),
                        result_legal = getLevelTable(id, "6"),
                        result_person = getLevelTable(id, "7"),
                        v.culpability,
                        v.road_accident,
                        v.fatality_prevention_element_id,
                        v.faltality_prevention_element_other,
                        root_cause = cr,
                        v.contributing_factor,
                        v.form2_function_id,
                        v.form3_department_id,
                        v.process_status,
                        v.location_company_name_en,
                        v.location_company_name_th,
                        v.location_function_name_en,
                        v.location_function_name_th,
                        v.location_department_name_en,
                        v.location_department_name_th,
                        v.location_division_name_en,
                        v.location_division_name_th,
                        v.location_section_name_en,
                        v.location_section_name_th,

                        v.activity_location_company_name_en,
                        v.activity_location_company_name_th,
                        v.activity_location_function_name_en,
                        v.activity_location_function_name_th,
                        v.activity_location_department_name_en,
                        v.activity_location_department_name_th,
                        v.activity_location_division_name_en,
                        v.activity_location_division_name_th,
                        v.activity_location_section_name_en,
                        v.activity_location_section_name_th,

                        v.reporter_company_id,
                        v.reporter_function_id,
                        v.reporter_department_id,
                        v.reporter_division_id,
                        v.reporter_section_id




                    };

                    ArrayList dt = new ArrayList();
                    dt.Add(result);


                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(dt));

                }
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


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getImageIncident(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                ArrayList ls = new ArrayList();

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
                   // string pathfolder = string.Format("{0}\\upload\\incident\\" + name_folder, Server.MapPath(@"\"));
                    string pathfolder = string.Format("{0}"+ pathupload + name_folder, Server.MapPath(@"\"));





                    string[] images = Directory.GetFiles(pathfolder, "*")
                                             .Select(Path.GetFileName).OrderByDescending(Path.GetFileName).Take(5)
                                             .ToArray();

                    // FileInfo[] files = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();

                    foreach (var d in images)
                    {
                        string pathfolder_new = pathfolder + "\\" + d;
                        FileInfo f = new FileInfo(pathfolder_new);
                        string size = f.Length.ToString();
                        var v = new Dictionary<string, string>
                       {
                           { "path", "upload/incident/"+name_folder+"/"+d },
                           { "folder", name_folder},
                           { "name", d },
                           { "size", size },
                
                       };

                        ls.Add(v);

                    }


                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(ls));
            }
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createInjuryPerson(string name_injury,
                                       string employee_id,
                                       string type_employment_id,
                                       string function_id,
                                       string department_id,
                                       string nature_injury_id,
                                       string body_part_id,
                                       string severity_injury_id,
                                       string day_cost,
                                       string remark,
                                       string incident_id
                                       )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                try
                {

                    injury_person objInsert = new injury_person();

                    objInsert.full_name = name_injury;
                    objInsert.employee_id = employee_id;

                    if (type_employment_id != "")
                    {
                        objInsert.type_employment_id = Convert.ToInt32(type_employment_id);
                    }

                    objInsert.function_id = function_id;
                    objInsert.department_id = department_id;

                    if (nature_injury_id != "")
                    {
                        objInsert.nature_injury_id = Convert.ToInt32(nature_injury_id);
                    }

                    if (body_part_id != "")
                    {
                        objInsert.body_parts_id = Convert.ToInt32(body_part_id);
                    }

                    if (severity_injury_id != "")
                    {
                        objInsert.severity_injury_id = Convert.ToInt32(severity_injury_id);
                    }

                    if (day_cost != "")
                    {
                        objInsert.day_lost = Convert.ToInt32(day_cost);
                    }

                    objInsert.remark = remark;
                    objInsert.incident_id = Convert.ToInt32(incident_id);
                    objInsert.status = "A";
                    dbConnect.injury_persons.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();


                    Context.Response.Output.Write(objInsert.id);

                }
                catch (Exception ex)
                {



                }

               
            }

        }




        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createDamageList(  string property_enviroment,
                                       string detail_damage,
                                       string damage_cost,
                                       string incident_id
                                       )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                try
                {
                    damage_list objInsert = new damage_list();

                    objInsert.property_environment_damage = property_enviroment;
                    objInsert.detail_damage = detail_damage;
                    objInsert.damage_cost = Convert.ToDouble(damage_cost);
                    objInsert.incident_id = Convert.ToInt32(incident_id);
                    objInsert.status = "A";

                    dbConnect.damage_lists.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();


                    Context.Response.Output.Write(objInsert.id);

                }
                catch (Exception e)
                {



                }
               

            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateInjuryPerson(string name_injury,
                                       string employee_id,
                                       string type_employment_id,
                                       string function_id,
                                       string department_id,
                                       string nature_injury_id,
                                       string body_part_id,
                                       string severity_injury_id,
                                       string day_cost,
                                       string remark,
                                       string id
                                      )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var query = from c in dbConnect.injury_persons
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (injury_person rc in query)
                {
                    rc.full_name = name_injury;
                    rc.employee_id = employee_id;
                    rc.type_employment_id = Convert.ToInt32(type_employment_id);
                    rc.function_id = function_id;
                    rc.department_id = department_id;
                    if (nature_injury_id != "")
                    {
                        rc.nature_injury_id = Convert.ToInt32(nature_injury_id);
                    }

                    if (body_part_id != "")
                    {
                        rc.body_parts_id = Convert.ToInt32(body_part_id);
                    }
                    else
                    {
                        rc.body_parts_id = null;
                    }

                    if (severity_injury_id != "")
                    {
                        rc.severity_injury_id = Convert.ToInt32(severity_injury_id);
                    }

                    if (day_cost != "")
                    {
                        rc.day_lost = Convert.ToInt32(day_cost);
                    }
                    else
                    {
                        rc.day_lost = null;
                    }

                    rc.remark = remark;


                }

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }



         [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateDamageList(string property_enviroment,
                                       string detail_damage,
                                       string damage_cost,
                                       string id
                                      )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var query = from c in dbConnect.damage_lists
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (damage_list rc in query)
                {
                    rc.property_environment_damage = property_enviroment;
                    rc.detail_damage = detail_damage;
                    rc.damage_cost = Convert.ToDouble(damage_cost);


                }

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
            }
            // return result;

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getInjuryPersonByID(string id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.injury_persons
                         where c.id == Convert.ToInt32(id)
                         select c;


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(gr));
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDamageListByID(string id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.damage_lists
                         where c.id == Convert.ToInt32(id)
                         select c;



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(gr));

            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteInjuryPerson(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.injury_persons
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    a.status = "D";
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

                Context.Response.Output.Write(result);
            }
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteDamageList(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.damage_lists
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    a.status = "D";
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

                Context.Response.Output.Write(result);
            }
        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getFileInvestigation(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                ArrayList ls = new ArrayList();

                var q = from c in dbConnect.incidents
                        where c.id == Convert.ToInt32(id)
                        select new
                        {
                            doc_no = c.doc_no,
                            c.investigation_committee_file

                        };

                string file = "";
                string name_folder = "";
                string country = Session["country"].ToString();
                foreach (var s in q)
                {
                    if (s.investigation_committee_file != "" && s.investigation_committee_file != null)
                    {
                        name_folder = s.doc_no;
                        string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathupload"];
                        //string pathfolder = string.Format("{0}\\upload\\incident\\" + name_folder, Server.MapPath(@"\"));
                        string pathfolder = string.Format("{0}" + pathupload + "step3\\" + country + "\\" + name_folder, Server.MapPath(@"\"));//"investigation_committee.pdf"

                        string[] files = Directory.GetFiles(pathfolder, "investigation_committee.*")
                                             .Select(Path.GetFileName).OrderByDescending(Path.GetFileName).Take(1)
                                             .ToArray();


                        foreach (var d in files)
                        {
                            file = d;
                        }

                    }



                }
                string name_full = "";
                if (file != "")
                {
                    name_full = "upload/incident/step3/" + country + "/" + name_folder + "/" + file;
                }
                var result = new
                {
                    name = name_full

                };
                ArrayList dt = new ArrayList();
                dt.Add(result);



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createFactFinding(string fact_finding,
                                       string source_incident,
                                       string event_exposure,
                                       string unsafe_action,
                                       string unsafe_condition,
                                       string evidence_file,
                                       string incident_id
                                       )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                try
                {
                    fact_finding objInsert = new fact_finding();

                    objInsert.fact_finding_name = fact_finding;
                    objInsert.source_incident_id = Convert.ToInt32(source_incident);
                    objInsert.event_exposure_id = Convert.ToInt32(event_exposure);
                    objInsert.unsafe_action = unsafe_action;
                    objInsert.unsafe_condition = unsafe_condition;
                    objInsert.evidence_file = evidence_file;
                    objInsert.incident_id = Convert.ToInt32(incident_id);
                    objInsert.status = "A";


                    dbConnect.fact_findings.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();


                    Context.Response.Output.Write(objInsert.id);
                }
                catch (Exception e)
                {

                }
               

            }
        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createCorrectivePreventive( string corrective_preventive,
                                                string responsible_person,
                                                string due_date,
                                                string date_complete,
                                                string notify_contractor,
                                                string remark,
                                                string attachment_file,
                                                string employee_id,
                                                string contractor_id,
                                                string root_cause_action,
                                                string user_id,
                                                string incident_id,
                                                string type_action
                                                )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                try
                {
                    if(type_action=="corrective")
                    {
                        corrective_prevention_action_incident objInsert = new corrective_prevention_action_incident();
                             
                        objInsert.corrective_preventive_action = corrective_preventive;
                        objInsert.responsible_person = responsible_person;
                        objInsert.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());
                        if (date_complete.Trim() != "")
                        {
                            objInsert.date_complete = FormatDates.changeDateTimeDB(date_complete, Session["lang"].ToString());
                        }

                        objInsert.notify_contractor = notify_contractor;
                        objInsert.remark = remark;
                        objInsert.incident_id = Convert.ToInt32(incident_id);
                        objInsert.action_status_id = 1;//on process
                        objInsert.attachment_file = attachment_file;
                        objInsert.employee_id = employee_id;
                        objInsert.root_cause_action = root_cause_action;
                        objInsert.assign_by_employee_id = user_id;

                        if (contractor_id != "")
                        {
                            objInsert.contractor_id = Convert.ToInt32(contractor_id);
                        }

                        objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                        dbConnect.corrective_prevention_action_incidents.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();


                        ////////////////////////////////////////////log ตรวจสอบการ attach file/////////////////////////////////////////
                        action_log objInsert2 = new action_log();
                        objInsert2.function_name = "createCorrective";
                        objInsert2.file_name = "incident_id=" + incident_id;
                        objInsert2.error_message = "action_id:" + objInsert.id + ", employee_id:" + employee_id + ", assign_employee_id:" + Session["user_id"].ToString() + ", attach_file:" + attachment_file;
                        objInsert2.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                        dbConnect.action_logs.InsertOnSubmit(objInsert2);

                        dbConnect.SubmitChanges();
                        //////////////////////////////////////////////////////////////////////////////////end log////////////////////////////

                        //////////////////////////////////by p.poo sent notification/////////////////////////////////

                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { };
                        sn.InsertNotification(4, Convert.ToInt32(incident_id), alert_to_groups, Session["timezone"].ToString(), "", objInsert.id,"corrective");

                        ////////////////////////////////////////end///////////////////////////////////

                        Context.Response.Output.Write(objInsert.id);

                    }else if(type_action=="preventive"){

                        preventive_action_incident objInsert = new preventive_action_incident();
                             
                        objInsert.preventive_action = corrective_preventive;
                        objInsert.responsible_person = responsible_person;
                        objInsert.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());
                        if (date_complete.Trim() != "")
                        {
                            objInsert.date_complete = FormatDates.changeDateTimeDB(date_complete, Session["lang"].ToString());
                        }

                        objInsert.notify_contractor = notify_contractor;
                        objInsert.remark = remark;
                        objInsert.incident_id = Convert.ToInt32(incident_id);
                        objInsert.action_status_id = 1;//on process
                        objInsert.attachment_file = attachment_file;
                        objInsert.employee_id = employee_id;
                        objInsert.root_cause_action = root_cause_action;
                        objInsert.assign_by_employee_id = user_id;

                        if (contractor_id != "")
                        {
                            objInsert.contractor_id = Convert.ToInt32(contractor_id);
                        }

                        objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                        dbConnect.preventive_action_incidents.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();


                        ////////////////////////////////////////////log ตรวจสอบการ attach file/////////////////////////////////////////
                        action_log objInsert2 = new action_log();
                        objInsert2.function_name = "createPreventive";
                        objInsert2.file_name = "incident_id=" + incident_id;
                        objInsert2.error_message = "action_id:" + objInsert.id + ", employee_id:" + employee_id + ", assign_employee_id:" + Session["user_id"].ToString() + ", attach_file:" + attachment_file;
                        objInsert2.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                        dbConnect.action_logs.InsertOnSubmit(objInsert2);

                        dbConnect.SubmitChanges();
                        //////////////////////////////////////////////////////////////////////////////////end log////////////////////////////

                        //////////////////////////////////by p.poo sent notification/////////////////////////////////

                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { };
                        sn.InsertNotification(4, Convert.ToInt32(incident_id), alert_to_groups, Session["timezone"].ToString(), "", objInsert.id, "preventive");

                        ////////////////////////////////////////end///////////////////////////////////

                        Context.Response.Output.Write(objInsert.id);

                    }else if(type_action=="consequence"){

                        consequence_management_incident objInsert = new consequence_management_incident();
                             
                        objInsert.consequence_management = corrective_preventive;
                        objInsert.responsible_person = responsible_person;
                        objInsert.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());
                        if (date_complete.Trim() != "")
                        {
                            objInsert.date_complete = FormatDates.changeDateTimeDB(date_complete, Session["lang"].ToString());
                        }

                        objInsert.notify_contractor = notify_contractor;
                        objInsert.remark = remark;
                        objInsert.incident_id = Convert.ToInt32(incident_id);
                        objInsert.action_status_id = 1;//on process
                        objInsert.attachment_file = attachment_file;
                        objInsert.employee_id = employee_id;
                        objInsert.root_cause_action = root_cause_action;
                        objInsert.assign_by_employee_id = user_id;

                        if (contractor_id != "")
                        {
                            objInsert.contractor_id = Convert.ToInt32(contractor_id);
                        }

                        objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                        dbConnect.consequence_management_incidents.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();


                        ////////////////////////////////////////////log ตรวจสอบการ attach file/////////////////////////////////////////
                        action_log objInsert2 = new action_log();
                        objInsert2.function_name = "createConsequence";
                        objInsert2.file_name = "incident_id=" + incident_id;
                        objInsert2.error_message = "action_id:" + objInsert.id + ", employee_id:" + employee_id + ", assign_employee_id:" + Session["user_id"].ToString() + ", attach_file:" + attachment_file;
                        objInsert2.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                        dbConnect.action_logs.InsertOnSubmit(objInsert2);

                        dbConnect.SubmitChanges();
                        //////////////////////////////////////////////////////////////////////////////////end log////////////////////////////

                        //////////////////////////////////by p.poo sent notification/////////////////////////////////

                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { };
                        sn.InsertNotification(4, Convert.ToInt32(incident_id), alert_to_groups, Session["timezone"].ToString(), "", objInsert.id, "consequence");

                        ////////////////////////////////////////end///////////////////////////////////

                        Context.Response.Output.Write(objInsert.id);
                    }
               
                }
                catch (Exception e)
                {

                }

               
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createRootCauseAction(string root_cause_action,
                                          string incident_id
                                         )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                root_cause_action objInsert = new root_cause_action();

                objInsert.name = root_cause_action;
                objInsert.incident_id = Convert.ToInt32(incident_id);
                objInsert.root_cause_number = getRootCauseActionNumber(Convert.ToInt32(incident_id));

                objInsert.status = "A";


                dbConnect.root_cause_actions.InsertOnSubmit(objInsert);

                dbConnect.SubmitChanges();


                Context.Response.Output.Write(objInsert.id);
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateFactFinding(string fact_finding,
                                       string source_incident,
                                       string event_exposure,
                                       string unsafe_action,
                                       string unsafe_condition,
                                       string evidence_file,
                                       string id
                                       )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var query = from c in dbConnect.fact_findings
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (fact_finding rc in query)
                {
                    rc.fact_finding_name = fact_finding;
                    rc.source_incident_id = Convert.ToInt32(source_incident);
                    rc.event_exposure_id = Convert.ToInt32(event_exposure);
                    rc.unsafe_action = unsafe_action;
                    rc.unsafe_condition = unsafe_condition;

                    if (evidence_file != "")
                    {
                        rc.evidence_file = evidence_file;
                    }


                }

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateCorrectivePreventive(string corrective_preventive,
                                                string responsible_person,
                                                string due_date,
                                                string date_complete,
                                                string notify_contractor,
                                                string remark,
                                                string attachment_file,
                                                string employee_id,
                                                string contractor_id,
                                                string root_cause_action,
                                                string id,
                                                string type_action
                                                )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                
                    bool change_employee = false;
                    int incident_id = 0;
                    int action_id = 0;
                    string result = "false";


                    if (type_action == "corrective")
                    {
                        var query = from c in dbConnect.corrective_prevention_action_incidents
                                    where c.id == Convert.ToInt32(id)
                                    select c;

                        foreach (corrective_prevention_action_incident rc in query)
                        {
                            rc.corrective_preventive_action = corrective_preventive;
                            rc.responsible_person = responsible_person;
                            rc.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());
                            if (date_complete.Trim() != "")
                            {
                                rc.date_complete = FormatDates.changeDateTimeDB(date_complete, Session["lang"].ToString());
                            }

                            rc.notify_contractor = notify_contractor;
                            rc.root_cause_action = root_cause_action;


                            if (rc.employee_id != employee_id)//change employee
                            {
                                rc.employee_id = employee_id;
                                change_employee = true;

                            }

                            if (contractor_id != "")
                            {
                                rc.contractor_id = Convert.ToInt32(contractor_id);
                            }
                            if (attachment_file != "")
                            {
                                rc.attachment_file = attachment_file;

                            }
                            rc.remark = remark;

                            incident_id = rc.incident_id;
                            action_id = rc.id;

                        }


                    }
                    else if(type_action=="preventive")
                    {
                        var query = from c in dbConnect.preventive_action_incidents
                                    where c.id == Convert.ToInt32(id)
                                    select c;

                        foreach (preventive_action_incident rc in query)
                        {
                            rc.preventive_action = corrective_preventive;
                            rc.responsible_person = responsible_person;
                            rc.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());
                            if (date_complete.Trim() != "")
                            {
                                rc.date_complete = FormatDates.changeDateTimeDB(date_complete, Session["lang"].ToString());
                            }

                            rc.notify_contractor = notify_contractor;
                            rc.root_cause_action = root_cause_action;


                            if (rc.employee_id != employee_id)//change employee
                            {
                                rc.employee_id = employee_id;
                                change_employee = true;

                            }

                            if (contractor_id != "")
                            {
                                rc.contractor_id = Convert.ToInt32(contractor_id);
                            }
                            if (attachment_file != "")
                            {
                                rc.attachment_file = attachment_file;

                            }
                            rc.remark = remark;

                            incident_id = rc.incident_id;
                            action_id = rc.id;

                        }


                    }
                    else if (type_action == "consequence")
                    {
                        var query = from c in dbConnect.consequence_management_incidents
                                    where c.id == Convert.ToInt32(id)
                                    select c;

                        foreach (consequence_management_incident rc in query)
                        {
                            rc.consequence_management = corrective_preventive;
                            rc.responsible_person = responsible_person;
                            rc.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());
                            if (date_complete.Trim() != "")
                            {
                                rc.date_complete = FormatDates.changeDateTimeDB(date_complete, Session["lang"].ToString());
                            }

                            rc.notify_contractor = notify_contractor;
                            rc.root_cause_action = root_cause_action;


                            if (rc.employee_id != employee_id)//change employee
                            {
                                rc.employee_id = employee_id;
                                change_employee = true;

                            }

                            if (contractor_id != "")
                            {
                                rc.contractor_id = Convert.ToInt32(contractor_id);
                            }
                            if (attachment_file != "")
                            {
                                rc.attachment_file = attachment_file;

                            }
                            rc.remark = remark;

                            incident_id = rc.incident_id;
                            action_id = rc.id;  
                            
                        } 


                    }

                    
               try
               {
                    dbConnect.SubmitChanges();
                    result = "true";

                    if (change_employee==true)//change employee
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////

                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { };
                        sn.InsertNotification(4, incident_id, alert_to_groups, Session["timezone"].ToString(), "", action_id);

                        ////////////////////////////////////////end///////////////////////////////////

                    }

                    action_log objInsert = new action_log();
                    objInsert.function_name = "updateCorrectivePreventive";
                    objInsert.file_name = "incident_id="+incident_id;
                    objInsert.error_message = "action_id:" + action_id + ", employee_id:" + employee_id + ", assign_employee_id:" + Session["user_id"].ToString()+", attach_file:"+attachment_file;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();


                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
            }
            // return result;

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateRootCauseAction(string root_cause_action,
                                          string id
                                       )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var query = from c in dbConnect.root_cause_actions
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (root_cause_action rc in query)
                {
                    rc.name = root_cause_action;


                }

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateMyactionAttachFile(
                                            string attachment_file,
                                            string id,
                                            string type_action
                                            )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";
                int incident_id = 0;

                if(type_action=="corrective")
                {
                     var query = from c in dbConnect.corrective_prevention_action_incidents
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (corrective_prevention_action_incident rc in query)
                    {
                        if (attachment_file != "")
                        {
                            rc.attachment_file = attachment_file;

                        }

                        incident_id = rc.incident_id;
                    }


                }else if(type_action=="preventive"){

                     var query = from c in dbConnect.preventive_action_incidents
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (preventive_action_incident rc in query)
                    {
                        if (attachment_file != "")
                        {
                            rc.attachment_file = attachment_file;

                        }

                        incident_id = rc.incident_id;
                    }

                }else if(type_action=="consequence"){

                     var query = from c in dbConnect.consequence_management_incidents
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (consequence_management_incident rc in query)
                    {
                        if (attachment_file != "")
                        {
                            rc.attachment_file = attachment_file;

                        }

                        incident_id = rc.incident_id;
                    }

                }

               
              

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";

                    action_log objInsert = new action_log();
                    objInsert.function_name = "updateMyactionAttachFile";
                    objInsert.file_name = "incident_id=" + incident_id;
                    objInsert.error_message = "assign_employee_id:" + Session["user_id"].ToString() + ", attach_file:" + attachment_file;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateMyactionAttachFileAction(
                                            string attachment_file,
                                            string id
                                            )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var query = from c in dbConnect.process_actions
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (process_action rc in query)
                {
                    if (attachment_file != "")
                    {
                        rc.attachment_file = attachment_file;

                    }

                }

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
            }
            // return result;

        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateMyactionAttachFileActionSot(
                                                        string attachment_file,
                                                        string id
                                                        )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var query = from c in dbConnect.process_action_sots
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (process_action_sot rc in query)
                {
                    if (attachment_file != "")
                    {
                        rc.attachment_file = attachment_file;

                    }

                }

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
            }
            // return result;

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteFactFinding(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.fact_findings
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    a.status = "D";
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

                Context.Response.Output.Write(result);
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteRiskFactor(string id,string page_type)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                if (page_type == "add")
                {
                    if (Session["risk_factor"] != null)
                    {
                        //Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["risk_factor"];

                        //dict.Remove(Convert.ToInt32(id));
                        //Session["risk_factor"] = dict;

                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["risk_factor"];
                        RiskFactorEntity rc = (RiskFactorEntity)dict[Convert.ToInt32(id)];

                        rc.Status = "D";

                        Session["risk_factor"] = dict;


                       
                    }

                    Context.Response.Output.Write(true);
                }
                else
                {

                    string result = "false";

                    var gr = from c in dbConnect.risk_factor_relate_work_actions
                             where c.id == Convert.ToInt32(id)
                             select c;
                    foreach (var a in gr)
                    {
                        a.status = "D";
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

                    Context.Response.Output.Write(result);
                }


            }


        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteOccupationalHealth(string id,string page_type)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                if (page_type == "add")
                {
                    if (Session["occupational_health"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["occupational_health"];

                        //dict.Remove(Convert.ToInt32(id));
                        //Session["occupational_health"] = dict;

                        OccupationalHealthEntity rc = (OccupationalHealthEntity)dict[Convert.ToInt32(id)];

                        rc.Status = "D";

                        Session["occupational_health"] = dict;

                       
                    }

                    Context.Response.Output.Write(true);
                }
                else
                {

                    string result = "false";

                    var gr = from c in dbConnect.occupational_health_report_actions
                             where c.id == Convert.ToInt32(id)
                             select c;
                    foreach (var a in gr)
                    {
                        a.status = "D";
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

                    Context.Response.Output.Write(result);
                }


            }


        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void changeStatusCorrectivePreventive(string id,string status_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.corrective_prevention_action_incidents
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    a.action_status_id = Convert.ToInt32(status_id);
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

                Context.Response.Output.Write(result);
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteRootCauseAction(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";
                int incident_id = 0;

                var gr = from c in dbConnect.root_cause_actions
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    a.status = "D";
                    incident_id =  Convert.ToInt32(a.incident_id);
                }
                try
                {
                    dbConnect.SubmitChanges();


                    var gr2 = from c in dbConnect.root_cause_actions
                              where c.incident_id == incident_id
                              && c.status == "A"
                              orderby c.id ascending
                              select c;

                    int count = 1;
                    foreach (var rc in gr2)
                    {
                        rc.root_cause_number = count;

                        count++;
                    }

                    dbConnect.SubmitChanges();

                    result = "true";
                }
                catch (Exception e)
                {

                    result = "false";
                }

                Context.Response.Output.Write(result);
            }
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getFactFindingByID(string id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.fact_findings
                         where c.id == Convert.ToInt32(id)
                         select c;



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(gr));
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getCorrectivePreventiveByID(string id,string lang,string type_action)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                if(type_action=="corrective")
                {
                    var gr = from c in dbConnect.corrective_prevention_action_incidents
                             where c.id == Convert.ToInt32(id)
                             select new
                             {
                                 c.id,
                                 c.corrective_preventive_action,
                                 c.responsible_person,
                                 c.root_cause_action,
                                 due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date), lang),
                                 date_complete = c.date_complete != null ? FormatDates.getDateShowFromDate(Convert.ToDateTime(c.date_complete), lang) : null,
                                 c.notify_contractor,
                                 c.remark,
                                 c.contractor_id,
                                 c.employee_id



                             };



                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(gr));

                }else if(type_action=="preventive")
                {

                    var gr = from c in dbConnect.preventive_action_incidents
                             where c.id == Convert.ToInt32(id)
                             select new
                             {
                                 c.id,
                                 corrective_preventive_action = c.preventive_action,
                                 c.responsible_person,
                                 c.root_cause_action,
                                 due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date), lang),
                                 date_complete = c.date_complete != null ? FormatDates.getDateShowFromDate(Convert.ToDateTime(c.date_complete), lang) : null,
                                 c.notify_contractor,
                                 c.remark,
                                 c.contractor_id,
                                 c.employee_id



                             };


                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(gr));

                }
                else if (type_action == "consequence")
                {
                    var gr = from c in dbConnect.consequence_management_incidents
                             where c.id == Convert.ToInt32(id)
                             select new
                             {
                                 c.id,
                                 corrective_preventive_action = c.consequence_management,
                                 c.responsible_person,
                                 c.root_cause_action,
                                 due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date), lang),
                                 date_complete = c.date_complete != null ? FormatDates.getDateShowFromDate(Convert.ToDateTime(c.date_complete), lang) : null,
                                 c.notify_contractor,
                                 c.remark,
                                 c.contractor_id,
                                 c.employee_id



                             };


                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(gr));

                }
               

            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getProcessActionByID(string id, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.process_actions
                         where c.id == Convert.ToInt32(id)
                         select new
                         {
                             c.id,
                             c.type_control,
                             c.action,
                             c.responsible_person,
                             due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date), lang),
                             date_complete = c.date_complete != null ? FormatDates.getDateShowFromDate(Convert.ToDateTime(c.date_complete), lang) : null,
                             c.notify_contractor,
                             c.remark,
                             c.contractor_id,
                             c.employee_id



                         };



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(gr));
            }

        }

        [WebMethod(EnableSession= true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getProcessActionSotByID(string id,string page_type, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                if(page_type=="add")
                {
                    if (Session["process_action_sot"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["process_action_sot"];


                        ProcessActionSotEntity rc = (ProcessActionSotEntity)dict[Convert.ToInt32(id)];

                        var result = new
                        {
                            id = rc.id,
                            type_control = rc.TypeControl,
                            action = rc.Action,
                            responsible_person = rc.ResponsiblePerson,
                            due_date = rc.DueDate,
                            date_complete = rc.DateComplete,
                            notify_contractor = rc.NotifyContractor,
                            remark = rc.Remark,
                            contractor_id = rc.Contractor_id,
                            employee_id = rc.Employee_id

                        };

                        ArrayList dt = new ArrayList();
                        dt.Add(result);


                        
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        Context.Response.Write(js.Serialize(dt));
                    }


                }else{

                     var gr = from c in dbConnect.process_action_sots
                         where c.id == Convert.ToInt32(id)
                         select new
                         {
                             c.id,
                             c.type_control,
                             c.action,
                             c.responsible_person,
                             due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date), lang),
                             date_complete = c.date_complete != null ? FormatDates.getDateShowFromDate(Convert.ToDateTime(c.date_complete), lang) : null,
                             c.notify_contractor,
                             c.remark,
                             c.contractor_id,
                             c.employee_id

                         };



                     JavaScriptSerializer js = new JavaScriptSerializer();
                     Context.Response.Write(js.Serialize(gr));
                }

             
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getRiskFactorByID(string id, string page_type, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                if (page_type == "add")
                {
                    if (Session["risk_factor"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["risk_factor"];


                        RiskFactorEntity rc = (RiskFactorEntity)dict[Convert.ToInt32(id)];

                        var result = new
                        {
                            id = rc.id,
                            risk_factor_relate_work_id = rc.risk_factor_relate_work_id,
                            other = rc.Other,
                            year = rc.Year,
                            duration_risk_factor_id = rc.Duration_risk_factor_id,
                            file_risk_factor = rc.File_risk_factor,
                            result = rc.Result,
                            rc.monitoring_environment,
                            rc.monitoring_results
                           


                        };

                        ArrayList dt = new ArrayList();
                        dt.Add(result);



                        JavaScriptSerializer js = new JavaScriptSerializer();
                        Context.Response.Write(js.Serialize(dt));
                    }


                }
                else
                {

                    var gr = from c in dbConnect.risk_factor_relate_work_actions
                             where c.id == Convert.ToInt32(id)
                             select new
                             {
                                 c.id,
                                 c.risk_factor_relate_work_id,
                                 c.other,
                                 c.year,
                                 c.duration_risk_factor_id,
                                 c.file_risk_factor,
                                 c.result,
                                 c.monitoring_environment,
                                 c.monitoring_results

                             };



                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(gr));
                }


            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getOccupationalHealthByID(string id, string page_type, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                if (page_type == "add")
                {
                    if (Session["occupational_health"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["occupational_health"];


                        OccupationalHealthEntity rc = (OccupationalHealthEntity)dict[Convert.ToInt32(id)];

                        var result = new
                        {
                            id = rc.id,
                            occupational_health_report_id = rc.occupational_health_report_id,
                            repeat_health_check = rc.RepeatHealthCheck,
                            file_health_check = rc.FileHealthCheck,
                            flie_repeat_health_check = rc.FlieRepeatHealthCheck,
                            rc.abnormal_pulmonary_function,
                            rc.smoked_cigarettes,
                            rc.cigarette_per_day,
                            rc.smoked_years,
                            rc.smoked_months,
                            rc.smoked_cigarettes_other,
                            rc.hearing_threshold_level,
                            rc.specify_chronic_diseases_ear,
                            rc.abnormal_audiogram,
                            rc.chronic_diseases_ear,
                           
                            

                        };

                        ArrayList dt = new ArrayList();
                        dt.Add(result);



                        JavaScriptSerializer js = new JavaScriptSerializer();
                        Context.Response.Write(js.Serialize(dt));
                    }


                }
                else
                {

                    var gr = from c in dbConnect.occupational_health_report_actions
                             where c.id == Convert.ToInt32(id)
                             select new
                             {
                                 c.id,
                                 c.occupational_health_report_id,
                                 c.repeat_health_check,
                                 c.file_health_check,
                                 c.flie_repeat_health_check,
                                 c.abnormal_pulmonary_function,
                                 c.smoked_cigarettes,
                                 c.cigarette_per_day,
                                 c.smoked_years,
                                 c.smoked_months,
                                 c.smoked_cigarettes_other,
                                 c.specify_chronic_diseases_ear,
                                 c.abnormal_audiogram,
                                 c.chronic_diseases_ear,
                                 c.hearing_threshold_level,
                           
                           


                             };



                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(gr));
                }


            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getProcessActionHealthByID(string id, string page_type, string lang)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                

                    var gr = from c in dbConnect.process_action_healths
                             where c.id == Convert.ToInt32(id)
                             select new
                             {
                                 c.id,
                                 c.type_control_id,
                                 c.action,
                                 c.responsible_person,
                                 due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(c.due_date), lang),
                                 date_complete = c.date_complete != null ? FormatDates.getDateShowFromDate(Convert.ToDateTime(c.date_complete), lang) : null,
                                 c.remark,
                                 c.doctor_opinion_file,
                                 c.recovery_plan_file,
                                 c.attachment_file,
                                 c.action_status_id,
                                 c.employee_id


                             };



                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(gr));
            
            }

        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getRootCauseActionByID(string id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var gr = from c in dbConnect.root_cause_actions
                         where c.id == Convert.ToInt32(id)
                         select c;



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(gr));
            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void requestActionIncident(string id,string type,string remark,string type_action)
                                                
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";
                int incident_id = 0;


                if(type_action=="corrective")
                {
                    
                    var query = from c in dbConnect.corrective_prevention_action_incidents
                                where c.id == Convert.ToInt32(id)
                                select c;

                    foreach (corrective_prevention_action_incident rc in query)
                    {
                        if (type == "close")
                        {
                            rc.action_status_id = 4;
                            rc.remark = "";

                            if (rc.date_complete == null)
                            {
                                rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                            }

                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            string[] alert_to_groups = { };
                            sn.InsertNotification(7, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "", rc.id, "corrective");

                            ////////////////////////////////////////end///////////////////////////////////

                        }
                        else if (type == "reject")
                        {

                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            rc.remark = remark;
                            rc.action_status_id = 6;//reject
                            rc.attachment_file = "";
                            rc.date_complete = null;
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            string[] alert_to_groups = { };
                            sn.InsertNotification(8, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "", rc.id, "corrective");

                            ////////////////////////////////////////end///////////////////////////////////


                        }
                        else if (type == "cancel")
                        {
                            rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                            rc.action_status_id = 5;


                        }
                        else if (type == "request close")
                        {
                            rc.action_status_id = 2;
                            rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            if(Session["country"].ToString()=="thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S" };
                                sn.InsertNotification(6, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "AreaOH&S",rc.id,"corrective");

                            }
                             else if (Session["country"].ToString() == "srilanka")
                             {
                                 string[] alert_to_groups = { "AreaManager" };
                                 sn.InsertNotification(6, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "AreaManager", rc.id, "corrective");

                             }
                       
                      
                            ////////////////////////////////////////end///////////////////////////////////

                        }

                        incident_id = rc.incident_id;
                    }

                }else if(type_action=="preventive")
                {
                      var query = from c in dbConnect.preventive_action_incidents
                                where c.id == Convert.ToInt32(id)
                                select c;

                    foreach (preventive_action_incident rc in query)
                    {
                        if (type == "close")
                        {
                            rc.action_status_id = 4;
                            rc.remark = "";

                            if (rc.date_complete == null)
                            {
                                rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                            }

                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            string[] alert_to_groups = { };
                            sn.InsertNotification(7, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "", rc.id, "preventive");

                            ////////////////////////////////////////end///////////////////////////////////

                        }
                        else if (type == "reject")
                        {

                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            rc.remark = remark;
                            rc.action_status_id = 6;//reject
                            rc.attachment_file = "";
                            rc.date_complete = null;
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            string[] alert_to_groups = { };
                            sn.InsertNotification(8, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "", rc.id, "preventive");

                            ////////////////////////////////////////end///////////////////////////////////


                        }
                        else if (type == "cancel")
                        {
                            rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                            rc.action_status_id = 5;


                        }
                        else if (type == "request close")
                        {
                            rc.action_status_id = 2;
                            rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            if(Session["country"].ToString()=="thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S" };
                                sn.InsertNotification(6, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "AreaOH&S", rc.id, "preventive");

                            }
                             else if (Session["country"].ToString() == "srilanka")
                             {
                                 string[] alert_to_groups = { "AreaManager" };
                                 sn.InsertNotification(6, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "AreaManager", rc.id, "preventive");

                             }
                       
                      
                            ////////////////////////////////////////end///////////////////////////////////

                        }

                        incident_id = rc.incident_id;
                    }

                }else if(type_action=="consequence")
                {

                      var query = from c in dbConnect.consequence_management_incidents
                                where c.id == Convert.ToInt32(id)
                                select c;

                    foreach (consequence_management_incident rc in query)
                    {
                        if (type == "close")
                        {
                            rc.action_status_id = 4;
                            rc.remark = "";

                            if (rc.date_complete == null)
                            {
                                rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                            }

                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            string[] alert_to_groups = { };
                            sn.InsertNotification(7, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "", rc.id, "consequence");

                            ////////////////////////////////////////end///////////////////////////////////

                        }
                        else if (type == "reject")
                        {

                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            rc.remark = remark;
                            rc.action_status_id = 6;//reject
                            rc.attachment_file = "";
                            rc.date_complete = null;
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            string[] alert_to_groups = { };
                            sn.InsertNotification(8, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "", rc.id, "consequence");

                            ////////////////////////////////////////end///////////////////////////////////


                        }
                        else if (type == "cancel")
                        {
                            rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                            rc.action_status_id = 5;


                        }
                        else if (type == "request close")
                        {
                            rc.action_status_id = 2;
                            rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                            //////////////////////////////////by p.poo sent notification/////////////////////////////////
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            if(Session["country"].ToString()=="thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S" };
                                sn.InsertNotification(6, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "AreaOH&S", rc.id, "consequence");

                            }
                             else if (Session["country"].ToString() == "srilanka")
                             {
                                 string[] alert_to_groups = { "AreaManager" };
                                 sn.InsertNotification(6, rc.incident_id, alert_to_groups, Session["timezone"].ToString(), "AreaManager", rc.id, "consequence");

                             }
                       
                      
                            ////////////////////////////////////////end///////////////////////////////////

                        }

                        incident_id = rc.incident_id;
                    }


                }



                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";

                    action_log objInsert = new action_log();
                    objInsert.function_name = "requestActionIncident";
                    objInsert.file_name = "incident_id=" + incident_id;
                    objInsert.error_message = "assign_employee_id:" + Session["user_id"].ToString() + ", type:" + type;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }






        //////////////////////////////////////////////////////////////////////////Hazard/////////////////////////////////////////////////////////////////////////////////////////

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createHazard(string hazarddate,
                                       string hazardtime,
                                       string reportdate,
                                       string company_id,
                                       string function_id,
                                       string department_id,
                                       string division_id,
                                       string section_id,
                                       string hazardarea,
                                       string hazardname,
                                       string characteristic_id,
                                       string hazarddetail,
                                       string preliminary_action,
                                       string type_action,
                                       string userid,
                                       string typelogin,
                                       string phone)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                try
                {
                    int process_status = 1;//on process
                    hazard objInsert = new hazard();

                    objInsert.doc_no = generateDocnoHazard(Session["country"].ToString(), Session["timezone"].ToString());
                    objInsert.hazard_date = FormatDates.changeDateTimeDB(hazarddate + " " + hazardtime, Session["lang"].ToString());
                    objInsert.report_date = FormatDates.changeDateTimeDB(reportdate, Session["lang"].ToString());
                    objInsert.company_id = company_id;
                    objInsert.function_id = function_id;
                    objInsert.department_id = department_id;
                    objInsert.division_id = division_id;
                    objInsert.section_id = section_id;
                    objInsert.hazard_area = hazardarea;
                    objInsert.hazard_name = hazardname;
                    objInsert.hazard_characteristic_id = Convert.ToInt16(characteristic_id);
                    objInsert.hazard_detail = hazarddetail;
                    objInsert.preliminary_action = preliminary_action;
                    objInsert.type_action = type_action;
                    objInsert.employee_id = userid;
                    objInsert.typeuser_login = typelogin;
                    objInsert.phone = phone;
                    objInsert.process_status = process_status;
                    objInsert.step_form = 1;
                    objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.is_alert_over_due = 0;
                    objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.alert_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.alert_form_three_to_four_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.last_alerted_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.alert_form_two_to_three_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                
                    objInsert.device_type = "web";
                    objInsert.country = Session["country"].ToString();


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

                        if (Session["country"].ToString() == "thailand")
                        {
                            objInsert.reporter_department_id = rc1.department_id;
                            objInsert.reporter_department_name = rc1.department;
                        }
                        else if (Session["country"].ToString() == "srilanka")
                        {

                            objInsert.reporter_department_id = rc1.sub_function_id;
                            objInsert.reporter_department_name = rc1.sub_function;

                        }

                    }
                    //////////////////////////////////////end reporter//////////////////////////////////////////////


                    /////////////////////กรณีคลิ๊กทำต่อ////////////////////////
                    Session["resume_hazard_date"] = hazarddate;
                    string[] sp = hazardtime.Split(':');
                    Session["resume_time_hour"] = sp[0];
                    Session["resume_time_minute"] = sp[1];
                    Session["resume_company_id"] = company_id;
                    Session["resume_function_id"] = function_id;
                    Session["resume_department_id"] = department_id;
                    Session["resume_division_id"] = division_id;
                    Session["resume_section_id"] = section_id;
                    Session["resume_hazard_area"] = hazardarea;
                    Session["resume_phone"] = phone;

                    dbConnect.hazards.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                    hazard_detail objInsert2 = new hazard_detail();
                    objInsert2.employee_id = userid;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.hazard_id = objInsert.id;

                    dbConnect.hazard_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();

                    Class.SafetyNotification sn = new Class.SafetyNotification();
                    if (Session["country"].ToString() == "thailand")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////                  
                        string[] alert_to_groups = { "AreaSuperervisor", "AreaOH&S" };//GroupOH&SHazard
                        sn.InsertHazardNotification(1, objInsert.id, alert_to_groups, Session["timezone"].ToString(), "AreaOH&S");
                        ///////////////////////////////////end//////////////////////////////////////////////////////

                    }
                    else if (Session["country"].ToString() == "srilanka")
                    {
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        string[] alert_to_groups = { "AreaSuperervisor", "AreaOH&S", "AreaManager" };//GroupOH&SHazard , 
                        sn.InsertHazardNotification(1, objInsert.id, alert_to_groups, Session["timezone"].ToString(), "AreaSuperervisor");
                        ///////////////////////////////////end//////////////////////////////////////////////////////

                    }


                    Context.Response.Output.Write(objInsert.id);



                }
                catch (Exception ex)
                {
                    action_log objInsert = new action_log();
                    objInsert.function_name = "createHazard";
                    objInsert.file_name = "Actionevent";
                    objInsert.error_message = ex.Message;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();
                }


               
            }


        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createHealth(      string health_employee_id,
                                       string year_health,
                                       string report_date,
                                       string company_id,
                                       string function_id,
                                       string department_id,
                                       string division_id,
                                       string section_id,
                                       string birth_date,
                                       string hiring_date,
                                       string age,
                                       string service_year,
                                       string service_year_current,
                                       string job_type_machine_type,
                                       string significant_insignificant,
                                       string userid,
                                       string typelogin,
                                       string hospital)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                try
                {
                    int process_status = 1;//on process
                    health objInsert = new health();

                    objInsert.doc_no = generateDocnoHealth(Session["country"].ToString(), Session["timezone"].ToString());
                    objInsert.health_employee_id = health_employee_id;
                    objInsert.year_health = year_health;
                    objInsert.report_date = FormatDates.changeDateTimeDB(report_date, Session["lang"].ToString());
                    objInsert.company_id = company_id;
                    objInsert.function_id = function_id;
                    objInsert.department_id = department_id;
                    objInsert.division_id = division_id;
                    objInsert.section_id = section_id;
                    objInsert.birth_date = FormatDates.changeDateTimeDB(birth_date, Session["lang"].ToString());
                    objInsert.hiring_date = FormatDates.changeDateTimeDB(hiring_date, Session["lang"].ToString());
                    objInsert.age = age;
                    objInsert.service_year = service_year;
                    objInsert.service_year_current = Convert.ToDouble(service_year_current);
                    objInsert.job_type_machine_type = job_type_machine_type;
                    objInsert.employee_id = userid;
                    objInsert.hospital_id = Convert.ToInt16(hospital);
                    objInsert.typeuser_login = typelogin;
                    objInsert.process_status = process_status;
                    objInsert.significant_insignificant = significant_insignificant;
                    objInsert.step_form = 2;//กด submmit แล้วไป step_form2 เลย
                    objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.is_alert_over_due = 0;
                    objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.country = Session["country"].ToString();


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

                        if (Session["country"].ToString() == "thailand")
                        {
                            objInsert.reporter_department_id = rc1.department_id;
                            objInsert.reporter_department_name = rc1.department;
                        }
                        else if (Session["country"].ToString() == "srilanka")
                        {

                            objInsert.reporter_department_id = rc1.sub_function_id;
                            objInsert.reporter_department_name = rc1.sub_function;

                        }

                    }

                    var logjson = new JavaScriptSerializer().Serialize(objInsert);
                   
                    dbConnect.healths.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                    //////////////////////////////////////end reporter//////////////////////////////////////////////


                    health_detail objInsert2 = new health_detail();
                    objInsert2.employee_id = userid;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.health_id = objInsert.id;

                    dbConnect.health_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();


                    //////////////////////////////////////////////////end detail///////////////////////////////////////////////



                    if (Session["risk_factor"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["risk_factor"];


                        for (int i = 1; i <= dict.Count(); i++)
                        {
                            RiskFactorEntity obj = (RiskFactorEntity)dict[i];

                            risk_factor_relate_work_action objInsert3 = new risk_factor_relate_work_action();

                            objInsert3.risk_factor_relate_work_id = Convert.ToInt32(obj.risk_factor_relate_work_id);
                            objInsert3.other = obj.Other;
                            objInsert3.year = obj.Year;
                            objInsert3.result = obj.Result;
                            

                            objInsert3.duration_risk_factor_id = obj.Duration_risk_factor_id;
                            objInsert3.file_risk_factor = obj.File_risk_factor;
                            objInsert3.status = obj.Status;
                            objInsert3.monitoring_environment = obj.monitoring_environment;
                            objInsert3.monitoring_results = obj.monitoring_results;
                            objInsert3.health_id = Convert.ToInt32(objInsert.id);
                            
                            dbConnect.risk_factor_relate_work_actions.InsertOnSubmit(objInsert3);

                            dbConnect.SubmitChanges();


                        
                        }


                        Session["risk_factor"] = null;

                    }




                    if (Session["occupational_health"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["occupational_health"];


                        for (int i = 1; i <= dict.Count(); i++)
                        {
                            OccupationalHealthEntity obj = (OccupationalHealthEntity)dict[i];

                            occupational_health_report_action objInsert3 = new occupational_health_report_action();

                            objInsert3.occupational_health_report_id = Convert.ToInt32(obj.occupational_health_report_id);
                            objInsert3.repeat_health_check = obj.RepeatHealthCheck;
                            objInsert3.file_health_check= obj.FileHealthCheck;
                            objInsert3.flie_repeat_health_check = obj.FlieRepeatHealthCheck;
                            objInsert3.abnormal_audiogram = obj.abnormal_audiogram;
                            objInsert3.hearing_threshold_level = obj.hearing_threshold_level;
                            objInsert3.chronic_diseases_ear = obj.chronic_diseases_ear;
                            objInsert3.specify_chronic_diseases_ear = obj.specify_chronic_diseases_ear;

                            objInsert3.abnormal_pulmonary_function = obj.abnormal_pulmonary_function;
                            objInsert3.smoked_cigarettes = obj.smoked_cigarettes;

                            objInsert3.cigarette_per_day = obj.cigarette_per_day;
                            objInsert3.smoked_years = obj.smoked_years;
                            objInsert3.smoked_months = obj.smoked_months;
                            objInsert3.smoked_cigarettes_other = obj.smoked_cigarettes_other;


                            objInsert3.status = obj.Status;

                            objInsert3.health_id = Convert.ToInt32(objInsert.id);

                            dbConnect.occupational_health_report_actions.InsertOnSubmit(objInsert3);

                            dbConnect.SubmitChanges();



                        }


                        Session["occupational_health"] = null;

                    }



                    Class.SafetyNotification sn = new Class.SafetyNotification();

                    //////////////////////////////////by p.poo sent notification/////////////////////////////////                  
                    string[] alert_to_groups = { "AreaOH&S", "AdminOH&S", "Functional", "GroupOH&SHealth" };
                    sn.InsertHealthNotification(1, objInsert.id, alert_to_groups, Session["timezone"].ToString(), "AreaOH&S");
                    ///////////////////////////////////end//////////////////////////////////////////////////////

                    action_log objInsertLog = new action_log();
                    objInsertLog.function_name = "createHealth";
                    objInsertLog.file_name = "Actionevent";
                    objInsertLog.receive = logjson;                   
                    objInsertLog.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                    dbConnect.action_logs.InsertOnSubmit(objInsertLog);
                    dbConnect.SubmitChanges();

                    Context.Response.Output.Write(objInsert.id);



                }
                catch (Exception ex)
                {
                    action_log objInsert = new action_log();
                    objInsert.function_name = "createHealth";
                    objInsert.file_name = "Actionevent";
                    objInsert.error_message = ex.Message;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();
                }



            }


        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void removeEmployeeInArea(
                                        string department_id,
                                        string division_id,
                                        string section_id,
                                        string type
                                      )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";


                if (type == "AreaOHS")
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
                        result = "true";
                    }
                    catch (Exception e)
                    {

                        result = "false";
                    }
                }
                else if (type == "AreaManager")
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
                        result = "true";
                    }
                    catch (Exception e)
                    {

                        result = "false";
                    }

                }
                else if (type == "AreaSupervisor")
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
                        result = "true";
                    }
                    catch (Exception e)
                    {

                        result = "false";
                    }


                }
                else if (type == "FunctionalManager")
                {
                    var gr = from c in dbConnect.employee_has_department_functional_managers
                             where c.department_id == department_id
                             select c;
                    foreach (var a in gr)
                    {
                        dbConnect.employee_has_department_functional_managers.DeleteOnSubmit(a);
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
                else if (type == "All")
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
                        result = "true";
                    }
                    catch (Exception e)
                    {

                        result = "false";
                    }

             


                    var gr2 = from c in dbConnect.employee_has_divisions
                             where c.division_id == division_id
                             select c;
                    foreach (var a in gr2)
                    {
                        dbConnect.employee_has_divisions.DeleteOnSubmit(a);
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



                    var gr3 = from c in dbConnect.employee_has_sections
                             where c.section_id == section_id
                             select c;
                    foreach (var a in gr3)
                    {
                        dbConnect.employee_has_sections.DeleteOnSubmit(a);
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


                    var gr4 = from c in dbConnect.employee_has_department_functional_managers
                             where c.department_id == department_id
                             select c;
                    foreach (var a in gr4)
                    {
                        dbConnect.employee_has_department_functional_managers.DeleteOnSubmit(a);
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


           
              

                Context.Response.Output.Write(result);

            }


        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createSot(  string country_id,
                                string company_id,
                                string function_id,
                                string department_id,
                                string division_id,
                                string site_id,
                                string sot_date,
                                string reportdate,
                                string sot_time,
                                string sot_time_end,
                                string location,
                                string typework,
                                string comment,
                                List<string> sotteam,
                                string user_id,
                                string typelogin,
                                string type_employment,

                                string changing_position,
                                string stopping_work,
                                string rearranging_job,
                                string hiding_dodging,
                                string changing_tools,
                                string applying_lockout,
                                string adjusting_ppe,

                                string striking_against,
                                string caught_between,
                                string inhaling,
                                string absorbing,
                                string electricity,
                                string falling,
                                string struck_by,
                                string line_fire,
                                string eyes_tasks,
                                string lifting_lowering,
                                string posture,

                                string head,
                                string ears_eyes,
                                string face_respiratory,
                                string hands_arms,
                                string feet_legs,

                                string right_job,
                                string used_correctly,
                                string in_safe_conditions,
                                string hamesses,
                                string barricade_warning_lights,
                                string chocks_restraints,
                                string pre_job_safe_checks,

                                string standard_adequate_job,
                                string standard_established,
                                string standard_being_maintained,
                                string isolation_lockout,
                                string hot_work_permit,
                                string confined_space_permit,
                                string electrical_permit,
                                string work_height_permit,
                                string rescue_plan_place,

                                string standards_established_understood,
                                string walkway_passageways,
                                string disorganized_tools_bench,
                                string materials_storage,
                                string obstruction_leaning_items,
                                string stairs_platforms,


                                string housekeeping,
                                string chemical_storage,
                                string waste_diposal,
                                string walking_working_surface,

                                string reactions_people,
                                string postion_people,
                                string personal_protection_equipment,
                                string tools_equipment,
                                string procedures,
                                string orderliness_tidiness,
                                string environment


                              )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                try
                {
                    int process_status = 1;//on process
                    sot objInsert = new sot();

                    objInsert.doc_no = generateDocnoSot(Session["country"].ToString(), Session["timezone"].ToString());
                    objInsert.sot_date = FormatDates.changeDateTimeDB(sot_date + " " + sot_time, Session["lang"].ToString());
                    objInsert.sot_date_end = FormatDates.changeDateTimeDB(sot_date + " " + sot_time_end, Session["lang"].ToString());
                    objInsert.report_date = FormatDates.changeDateTimeDB(reportdate, Session["lang"].ToString());

                    objInsert.country_id = country_id;
                    objInsert.company_id = company_id;
                    objInsert.function_id = function_id;
                    objInsert.department_id = department_id;
                    objInsert.division_id = division_id;
                    // objInsert.site_id = site_id;
                    objInsert.location = location.Trim();
                    objInsert.type_work = typework;
                    objInsert.comment = comment.Trim();
                    objInsert.type_employment_id = Convert.ToInt32(type_employment);
                    objInsert.employee_id = user_id;
                    objInsert.typeuser_login = typelogin;
                    objInsert.process_status = process_status;
                    objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert.country = Session["country"].ToString();
                    objInsert.status_form = 1;


                    objInsert.location_company_id = company_id;
                    objInsert.location_function_id = function_id;
                    objInsert.location_department_id = department_id;
                    objInsert.location_division_id = division_id;

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




                    //////////////////////////////////////reporter///////////////////////////////////////////////////
                    var t = from c in dbConnect.employees
                            join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                            where c.employee_id == user_id
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

                        if (Session["country"].ToString() == "thailand")
                        {
                            objInsert.reporter_department_id = rc1.department_id;
                            objInsert.reporter_department_name = rc1.department;
                        }
                        else if (Session["country"].ToString() == "srilanka")
                        {

                            objInsert.reporter_department_id = rc1.sub_function_id;
                            objInsert.reporter_department_name = rc1.sub_function;

                        }

                    }
                    //////////////////////////////////////end reporter//////////////////////////////////////////////


                    dbConnect.sots.InsertOnSubmit(objInsert);
                    dbConnect.SubmitChanges();

                    deleteEmployeeSot(objInsert.id);
                    foreach (var v in sotteam)
                    {
                        employee_has_sot objInsert3 = new employee_has_sot();
                        objInsert3.employee_id = v.ToString();
                        objInsert3.sot_id = objInsert.id;
                        dbConnect.employee_has_sots.InsertOnSubmit(objInsert3);


                        dbConnect.SubmitChanges();

                    }




                    sot_has_reactions_people objR = new sot_has_reactions_people();
                    objR.changinge_position = changing_position;
                    objR.stopping_work = stopping_work;
                    objR.rearranging_job = rearranging_job;
                    objR.hiding_dodging = hiding_dodging;
                    objR.changing_tools = changing_tools;
                    objR.applying_lockout = applying_lockout;
                    objR.adjusting_ppe = adjusting_ppe;
                    objR.description = reactions_people;
                    objR.sot_id = objInsert.id;
                    dbConnect.sot_has_reactions_peoples.InsertOnSubmit(objR);
                    dbConnect.SubmitChanges();

                    sot_has_position_people objP = new sot_has_position_people();
                    objP.striking_against = striking_against;
                    objP.caught_between = caught_between;
                    objP.inhaling = inhaling;
                    objP.absorbing = absorbing;
                    objP.electricity = electricity;
                    objP.falling = falling;
                    objP.struck_by = struck_by;
                    objP.line_fire = line_fire;
                    objP.eyes_tasks = eyes_tasks;
                    objP.lifting_lowering = lifting_lowering;
                    objP.posture = posture;
                    objP.description = postion_people;
                    objP.sot_id = objInsert.id;
                    dbConnect.sot_has_position_peoples.InsertOnSubmit(objP);
                    dbConnect.SubmitChanges();

                    sot_has_personal_protection_equipment objPer = new sot_has_personal_protection_equipment();
                    objPer.head = head;
                    objPer.ears_eyes = ears_eyes;
                    objPer.face_respiratory = face_respiratory;
                    objPer.hand_arms = hands_arms;
                    objPer.feet_legs = feet_legs;
                    objPer.description = personal_protection_equipment;
                    objPer.sot_id = objInsert.id;
                    dbConnect.sot_has_personal_protection_equipments.InsertOnSubmit(objPer);
                    dbConnect.SubmitChanges();

                    sot_has_tools_equipment objT = new sot_has_tools_equipment();
                    objT.right_job = right_job;
                    objT.used_correctly = used_correctly;
                    objT.safe_conditions = in_safe_conditions;
                    objT.hamesses = hamesses;
                    objT.barricades_warning_lights = barricade_warning_lights;
                    objT.chock_restraints = chocks_restraints;
                    objT.prejob_safety_checks = pre_job_safe_checks;
                    objT.description = tools_equipment;
                    objT.sot_id = objInsert.id;
                    dbConnect.sot_has_tools_equipments.InsertOnSubmit(objT);
                    dbConnect.SubmitChanges();

                    sot_has_procedure objPro = new sot_has_procedure();
                    objPro.standard_adequate_job = standard_adequate_job;
                    objPro.standard_established = standard_established;
                    objPro.standard_maintained = standard_being_maintained;
                    objPro.isolation_lockout = isolation_lockout;
                    objPro.hot_work_permit = hot_work_permit;
                    objPro.confined_space_permit = confined_space_permit;
                    objPro.electrical_permit = electrical_permit;
                    objPro.work_height_permit = work_height_permit;
                    objPro.rescue_plan_place = rescue_plan_place;
                    objPro.description = procedures;
                    objPro.sot_id = objInsert.id;
                    dbConnect.sot_has_procedures.InsertOnSubmit(objPro);
                    dbConnect.SubmitChanges();

                    sot_has_orderliness_tidiness objO = new sot_has_orderliness_tidiness();
                    objO.standards_established_understood = standards_established_understood;
                    objO.walkway_passageways = walkway_passageways;
                    objO.disorganized_tools_bench = disorganized_tools_bench;
                    objO.materials_storage = materials_storage;
                    objO.obstructions_leaning_items = obstruction_leaning_items;
                    objO.stairs_platforms = stairs_platforms;
                    objO.description = orderliness_tidiness;
                    objO.sot_id = objInsert.id;
                    dbConnect.sot_has_orderliness_tidinesses.InsertOnSubmit(objO);
                    dbConnect.SubmitChanges();

                    sot_has_environment objE = new sot_has_environment();
                    objE.housekeeping = housekeeping;
                    objE.chemical_storage = chemical_storage;
                    objE.waste_diposal = waste_diposal;
                    objE.walking_working_surface = walking_working_surface;
                    objE.sot_id = objInsert.id;
                    objE.description = environment;
                    dbConnect.sot_has_environments.InsertOnSubmit(objE);
                    dbConnect.SubmitChanges();



                    sot_detail objInsert2 = new sot_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.sot_id = objInsert.id;

                    dbConnect.sot_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();



                    if (Session["process_action_sot"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["process_action_sot"];


                        for (int i = 1; i <= dict.Count(); i++)
                        {
                            ProcessActionSotEntity obj = (ProcessActionSotEntity)dict[i];

                            process_action_sot objInsert3 = new process_action_sot();

                            objInsert3.type_control = Convert.ToInt32(obj.TypeControl);
                            objInsert3.action = obj.Action;
                            objInsert3.responsible_person = obj.ResponsiblePerson;
                            objInsert3.due_date = FormatDates.changeDateTimeDB(obj.DueDate, Session["lang"].ToString());
                            if (obj.DateComplete.Trim() != "")
                            {
                                objInsert3.date_complete = FormatDates.changeDateTimeDB(obj.DateComplete, Session["lang"].ToString());
                            }

                            objInsert3.notify_contractor = obj.NotifyContractor;
                            objInsert3.remark = obj.Remark;
                            objInsert3.sot_id = Convert.ToInt32(objInsert.id);
                            objInsert3.action_status_id = 1;//on process
                            objInsert3.attachment_file = "";
                            objInsert3.employee_id = obj.Employee_id;
                            // objInsert.root_cause_action = root_cause_action;
                            objInsert3.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                            if (obj.Contractor_id != "")
                            {
                                objInsert3.contractor_id = Convert.ToInt32(obj.Contractor_id);
                            }

                            dbConnect.process_action_sots.InsertOnSubmit(objInsert3);

                            dbConnect.SubmitChanges();


                            ////////////////////////////////////by p.poo sent notification/////////////////////////////////

                            Class.SafetyNotification sn3 = new Class.SafetyNotification();
                            string[] alert_to_groups3 = { "AreaManager" };
                            sn3.InsertSotNotification(3, objInsert.id, alert_to_groups3, Session["timezone"].ToString(), "AreaManager", objInsert3.id);

                            ////////////////////////////////////////end///////////////////////////////////
                        }


                        Session["process_action_sot"] = null;

                    }



                    Class.SafetyNotification sn = new Class.SafetyNotification();

                    //////////////////////////////////by p.poo sent notification/////////////////////////////////
                    string[] alert_to_groups = { "AreaManager", "AdminOH&S", "SOT" };//GroupOH&SHazard
                    sn.InsertSotNotification(1, objInsert.id, alert_to_groups, Session["timezone"].ToString(), "AreaManager");
                    ///////////////////////////////////end//////////////////////////////////////////////////////


                    Context.Response.Clear();
                    Context.Response.ContentType = "application/json";
                    Context.Response.AddHeader("content-length", objInsert.id.ToString().Length.ToString());
                    Context.Response.Flush();
                    Context.Response.Write(objInsert.id);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();


                }
                catch (Exception ex)
                {
                    action_log objInsert = new action_log();
                    objInsert.function_name = "createSot";
                    objInsert.file_name = "Actionevent";
                    objInsert.error_message = ex.Message;
                  
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();
                }

            
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


        protected string generateDocnoHealth(string country, string timezone)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string docno = "";
                string year = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                int number = 0;

                var doc_no = dbConnect.healths.Where(x => x.country == country).Where(x => x.doc_no.Contains("R" + year)).Max(t => t.doc_no);

                if (doc_no != "" && doc_no != null)
                {
                    string[] last = doc_no.Split('-');
                    number = Convert.ToInt32(last[1]) + 1;



                    docno = "R" + year + "-" + (number.ToString("D5"));

                }
                else
                {
                    docno = "R" + year + "-" + "00001";

                }


                return docno;
            }
        }

        protected string generateDocnoSot(string country, string timezone)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string docno = "";
                string year = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).ToString("yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                int number = 0;

                var doc_no = dbConnect.sots.Where(x => x.country == country).Where(x => x.doc_no.Contains("VFL" + year)).Max(t => t.doc_no);

                if (doc_no != "" && doc_no != null)
                {
                    string[] last = doc_no.Split('-');
                    number = Convert.ToInt32(last[1]) + 1;



                    docno = "VFL" + year + "-" + (number.ToString("D5"));

                }
                else
                {
                    docno = "VFL" + year + "-" + "00001";

                }


                return docno;
            }
        }

        protected int getRootCauseActionNumber(int incident_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int number = 0;
                var no = from c in dbConnect.root_cause_actions
                         where c.status == "A" && c.incident_id == incident_id
                         select c;

                //var no = dbConnect.root_cause_actions.Where(v => v.incident_id == incident_id).Max(x => x.root_cause_number);

               number = no.Count() + 1;

                return number;
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateHazard(string hazarddate,
                                       string hazardtime,
                                       string reportdate,
                                       string company_id,
                                       string function_id,
                                       string department_id,
                                       string division_id,
                                       string section_id,
                                       string hazardarea,
                                       string hazardname,
                                       string characteristic_id,
                                       string hazarddetail,
                                       string preliminary_action,
                                       string type_action,
                                       string user_id,
                                       string typelogin,
                                       string phone,
                                       string hazardid,
                                       string stepform,     
                                       string group_id
                                    )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = hazardid;
                bool change_area = false;
                //string[] alert_to_groups = new string[4];
                string[] alert_to_groups = new string[3];

                var query = from c in dbConnect.hazards
                            where c.id == Convert.ToInt32(hazardid)
                            select c;

                foreach (hazard rc in query)
                {

                    rc.hazard_date = FormatDates.changeDateTimeDB(hazarddate + " " + hazardtime, Session["lang"].ToString());
                    rc.report_date = FormatDates.changeDateTimeDB(reportdate, Session["lang"].ToString());
                    rc.company_id = company_id;

                    if (rc.function_id != function_id || rc.department_id != department_id
                        || rc.division_id != division_id || rc.section_id != section_id)
                    {

                        alert_to_groups[0] = "AreaOH&S";
                        alert_to_groups[1] = "AreaSuperervisor";

                         if (Session["country"].ToString() == "srilanka")
                        {
                            alert_to_groups[2] = "AreaManager";

                        }else if (Session["country"].ToString() == "thailand")
                        {
                             alert_to_groups[2] = "";
                        }


                        // alert_to_groups[2] = "GroupOH&SHazard";
                        change_area = true;
                    }
                    else
                    {
                        alert_to_groups[0] = "";
                        alert_to_groups[1] = "";
                        alert_to_groups[2] = "";
                        //  alert_to_groups[2] = "";
                    }


                    //if (rc.function_id != function_id)
                    //{
                    //    alert_to_groups[0] = "AdminOH&S";
                    //    change_area = true;
                    //}
                    //else
                    //{
                    //    alert_to_groups[0] = "";

                    //}
                    rc.function_id = function_id;


                    //if (rc.department_id != department_id)
                    //{
                    //    alert_to_groups[1] = "AreaOH&S";
                    //    change_area = true;
                    //}
                    //else
                    //{
                    //    alert_to_groups[1] = "";

                    //}
                    rc.department_id = department_id;


                    //if (rc.division_id != division_id)
                    //{
                    //    alert_to_groups[2] = "AreaManager";
                    //    change_area = true;
                    //}
                    //else
                    //{
                    //    alert_to_groups[2] = "";

                    //}
                    rc.division_id = division_id;


                    //if (rc.section_id != section_id)
                    //{
                    //    alert_to_groups[3] = "AreaSuperervisor";
                    //    change_area = true;
                    //}
                    //else
                    //{
                    //    alert_to_groups[3] = "";

                    //}
                    rc.section_id = section_id;


                    rc.hazard_area = hazardarea;
                    rc.hazard_name = hazardname;
                    rc.hazard_characteristic_id = Convert.ToInt16(characteristic_id);
                    rc.hazard_detail = hazarddetail;
                    rc.preliminary_action = preliminary_action;
                    rc.type_action = type_action;
                    // rc.employee_id = user_id;
                    // rc.typeuser_login = typelogin;
                    rc.phone = phone;
                    // rc.step_form = Convert.ToByte(stepform);
                    rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
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

                try
                {
                    dbConnect.SubmitChanges();

                    int process_status = 1;//on process
                    hazard_detail objInsert2 = new hazard_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.hazard_id = Convert.ToInt32(hazardid);

                    dbConnect.hazard_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();



                    if (change_area == true)//change area new to sent notification
                    {
                        ///////////////////////////sent notify by change area////////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        //string[] alert_to_groups = { "AdminOH&S", "AreaSuperervisor", "AreaManager", "AreaOH&S" };

                        if (Session["country"].ToString() == "srilanka")
                        {
                            sn.InsertHazardNotification(1, Convert.ToInt32(hazardid), alert_to_groups, Session["timezone"].ToString(), "AreaSuperervisor");

                        }
                        else if (Session["country"].ToString() == "thailand")
                        {
                            sn.InsertHazardNotification(1, Convert.ToInt32(hazardid), alert_to_groups, Session["timezone"].ToString(),"AreaOH&S");

                        }
                        ////////////////////////////////////end/////////////////////////////////////////////////
                    }

                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateHealth(      string health_employee_id,
                                       string year_health,
                                       string report_date,
                                       string company_id,
                                       string function_id,
                                       string department_id,
                                       string division_id,
                                       string section_id,
                                       string birth_date,
                                       string hiring_date,
                                       string age,
                                       string service_year,
                                       string service_year_current,
                                       string job_type_machine_type,
                                       string significant_insignificant,
                                       string userid,
                                       string typelogin,
                                       string health_id,
                                       string hospital
                                    )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = health_id;
                bool change_area = false;
              
                var query = from c in dbConnect.healths
                            where c.id == Convert.ToInt32(health_id)
                            select c;

                foreach (health rc in query)
                {

                    rc.report_date = FormatDates.changeDateTimeDB(report_date, Session["lang"].ToString());
                    rc.company_id = company_id;
                    rc.function_id = function_id;
                    rc.department_id = department_id;
                    rc.division_id = division_id;
                    rc.section_id = section_id;

                    rc.health_employee_id = health_employee_id;
                    rc.hospital_id = Convert.ToInt16(hospital);
                    rc.year_health = year_health;

                    rc.birth_date = FormatDates.changeDateTimeDB(birth_date, Session["lang"].ToString());
                    rc.hiring_date = FormatDates.changeDateTimeDB(hiring_date, Session["lang"].ToString());
                    rc.age = age;
                    rc.service_year = service_year;
                    rc.service_year_current = Convert.ToDouble(service_year_current);
                    rc.job_type_machine_type = job_type_machine_type;
                    rc.significant_insignificant = significant_insignificant;
                   
                    rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

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

                    var logjson = new JavaScriptSerializer().Serialize(rc);

                    action_log objInsertLog = new action_log();
                    objInsertLog.function_name = "updateHealth";
                    objInsertLog.file_name = "Actionevent";
                    objInsertLog.receive = logjson;
                    objInsertLog.report_id = Convert.ToInt32(health_id);
                    objInsertLog.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                    dbConnect.action_logs.InsertOnSubmit(objInsertLog);
                    dbConnect.SubmitChanges();


                }

                try
                {
                    dbConnect.SubmitChanges();

                    int process_status = 1;//on process
                    health_detail objInsert2 = new health_detail();
                    objInsert2.employee_id = userid;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.health_id = Convert.ToInt32(health_id);

                    dbConnect.health_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();



                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateHealth2(
                                       string userid,
                                       string typelogin,
                                       string health_id,
                                       string group_id
                                    )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = health_id;
                int edit_form2 = 0;
                var query = from c in dbConnect.healths
                            where c.id == Convert.ToInt32(health_id)
                            select c;

                foreach (health rc in query)
                {
                    if(rc.edit_form2!=null)
                    {
                        edit_form2 = Convert.ToInt16(rc.edit_form2);
                    }
                     
                     rc.edit_form2 = Convert.ToInt16(group_id);
                        
               
                }

                try
                {
                     dbConnect.SubmitChanges();

                    int process_status = 1;//on process
                    health_detail objInsert2 = new health_detail();
                    objInsert2.employee_id = userid;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.health_id = Convert.ToInt32(health_id);

                    dbConnect.health_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();





                    if(edit_form2==0)//first submit form2
                    {
                         Class.SafetyNotification sn = new Class.SafetyNotification();

                        //////////////////////////////////by p.poo sent notification/////////////////////////////////                  
                        string[] alert_to_groups = { "AreaOH&S", "AdminOH&S", "Functional", "GroupOH&SHealth" };
                        sn.InsertHealthNotification(5, Convert.ToInt32(health_id), alert_to_groups, Session["timezone"].ToString(), "");
                        ///////////////////////////////////end//////////////////////////////////////////////////////

                    }

                   

                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateSot(string country_id,
                                string company_id,
                                string function_id,
                                string department_id,
                                string division_id,
                                string site_id,
                                string sot_date,
                                string sot_time,
                                string sot_time_end,
                                string location,
                                string typework,
                                string comment,
                                List<string> sotteam,
                                string user_id,
                                string typelogin,
                                string sotid,
                                string group_id,
                                string type,
                                string type_employment,

                                string changing_position,
                                string stopping_work,
                                string rearranging_job,
                                string hiding_dodging,
                                string changing_tools,
                                string applying_lockout,
                                string adjusting_ppe,

                                string striking_against,
                                string caught_between,
                                string inhaling,
                                string absorbing,
                                string electricity,
                                string falling,
                                string struck_by,
                                string line_fire,
                                string eyes_tasks,
                                string lifting_lowering,
                                string posture,

                                string head,
                                string ears_eyes,
                                string face_respiratory,
                                string hands_arms,
                                string feet_legs,

                                string right_job,
                                string used_correctly,
                                string in_safe_conditions,
                                string hamesses,
                                string barricade_warning_lights,
                                string chocks_restraints,
                                string pre_job_safe_checks,

                                string standard_adequate_job,
                                string standard_established,
                                string standard_being_maintained,
                                string isolation_lockout,
                                string hot_work_permit,
                                string confined_space_permit,
                                string electrical_permit,
                                string work_height_permit,
                                string rescue_plan_place,

                                string standards_established_understood,
                                string walkway_passageways,
                                string disorganized_tools_bench,
                                string materials_storage,
                                string obstruction_leaning_items,
                                string stairs_platforms,

                                string housekeeping,
                                string chemical_storage,
                                string waste_diposal,
                                string walking_working_surface, 
    
                                string reactions_people,
                                string postion_people,
                                string personal_protection_equipment,
                                string tools_equipment,
                                string procedures,
                                string orderliness_tidiness,
                                string environment
                               )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = sotid;
                var query = from c in dbConnect.sots
                            where c.id == Convert.ToInt32(sotid)
                            select c;

                foreach (sot rc in query)
                {
                    rc.sot_date = FormatDates.changeDateTimeDB(sot_date + " " + sot_time, Session["lang"].ToString());
                    rc.sot_date_end = FormatDates.changeDateTimeDB(sot_date + " " + sot_time_end, Session["lang"].ToString());

                   // rc.country_id = country_id;
                    rc.company_id = company_id;
                    rc.function_id = function_id;
                    rc.department_id = department_id;
                    rc.division_id = division_id;
                  //  rc.site_id = site_id;
                    rc.location = location.Trim();
                    rc.type_work = typework;
                    rc.comment = comment;
                    rc.type_employment_id = Convert.ToInt32(type_employment);

                    rc.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    rc.edit_form = Convert.ToInt32(group_id);

                    rc.location_company_id = company_id;
                    rc.location_function_id = function_id;
                    rc.location_department_id = department_id;
                    rc.location_division_id = division_id;
            

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

                 

                    //if (type == "submit")
                    //{
                    //    rc.status_form = 2;//not edit
                    //}

                }

                try
                {
                    dbConnect.SubmitChanges();


                    deleteEmployeeSot(Convert.ToInt32(sotid));
                    foreach (var v in sotteam)
                    {
                        employee_has_sot objInsert3 = new employee_has_sot();
                        objInsert3.employee_id = v.ToString();
                        objInsert3.sot_id = Convert.ToInt32(sotid);
                        dbConnect.employee_has_sots.InsertOnSubmit(objInsert3);


                        dbConnect.SubmitChanges();

                    }





                    var reaction = from c in dbConnect.sot_has_reactions_peoples
                                   where c.sot_id == Convert.ToInt32(sotid)
                                   select c;

                    foreach (sot_has_reactions_people rc in reaction)
                    {
                        rc.changinge_position = changing_position;
                        rc.stopping_work = stopping_work;
                        rc.rearranging_job = rearranging_job;
                        rc.hiding_dodging = hiding_dodging;
                        rc.changing_tools = changing_tools;
                        rc.applying_lockout = applying_lockout;
                        rc.adjusting_ppe = adjusting_ppe;
                        rc.description = reactions_people;
                      
                       
                    }

                    dbConnect.SubmitChanges();


                    var postion = from c in dbConnect.sot_has_position_peoples
                                   where c.sot_id == Convert.ToInt32(sotid)
                                   select c;

                    foreach (sot_has_position_people rc in postion)
                    {
                   
                        rc.striking_against = striking_against;
                        rc.caught_between = caught_between;
                        rc.inhaling = inhaling;
                        rc.absorbing = absorbing;
                        rc.electricity = electricity;
                        rc.falling = falling;
                        rc.struck_by = struck_by;
                        rc.line_fire = line_fire;
                        rc.eyes_tasks = eyes_tasks;
                        rc.lifting_lowering = lifting_lowering;
                        rc.posture = posture;
                        rc.description = postion_people;
                        
                       
                       
                    }
                    dbConnect.SubmitChanges();

                var personal = from c in dbConnect.sot_has_personal_protection_equipments
                                   where c.sot_id == Convert.ToInt32(sotid)
                                   select c;

                foreach (sot_has_personal_protection_equipment rc in personal)
                {
                   
                    rc.ears_eyes = ears_eyes;
                    rc.face_respiratory = face_respiratory;
                    rc.hand_arms = hands_arms;
                    rc.feet_legs = feet_legs;
                    rc.description = personal_protection_equipment;

                }
                dbConnect.SubmitChanges();


                var tools = from c in dbConnect.sot_has_tools_equipments
                                   where c.sot_id == Convert.ToInt32(sotid)
                                   select c;

                foreach (sot_has_tools_equipment rc in tools)
                {
                    rc.right_job = right_job;
                    rc.used_correctly = used_correctly;
                    rc.safe_conditions = in_safe_conditions;
                    rc.hamesses = hamesses;
                    rc.barricades_warning_lights = barricade_warning_lights;
                    rc.chock_restraints = chocks_restraints;
                    rc.prejob_safety_checks = pre_job_safe_checks;
                    rc.description = tools_equipment;
  
                   
                }
                dbConnect.SubmitChanges();

                    var procedure = from c in dbConnect.sot_has_procedures
                                   where c.sot_id == Convert.ToInt32(sotid)
                                   select c;

                    foreach (sot_has_procedure rc in procedure)
                    {

                        rc.standard_adequate_job = standard_adequate_job;
                        rc.standard_established = standard_established;
                        rc.standard_maintained = standard_being_maintained;
                        rc.isolation_lockout = isolation_lockout;
                        rc.hot_work_permit = hot_work_permit;
                        rc.confined_space_permit = confined_space_permit;
                        rc.electrical_permit = electrical_permit;
                        rc.work_height_permit = work_height_permit;
                        rc.rescue_plan_place = rescue_plan_place;
                        rc.description = procedures;

                    }
                    dbConnect.SubmitChanges();



                    var order = from c in dbConnect.sot_has_orderliness_tidinesses
                                   where c.sot_id == Convert.ToInt32(sotid)
                                   select c;

                    foreach (sot_has_orderliness_tidiness rc in order)
                    {
                        
                        rc.standards_established_understood = standards_established_understood;
                        rc.walkway_passageways = walkway_passageways;
                        rc.disorganized_tools_bench = disorganized_tools_bench;
                        rc.materials_storage = materials_storage;
                        rc.obstructions_leaning_items = obstruction_leaning_items;
                        rc.stairs_platforms = stairs_platforms;
                        rc.description = orderliness_tidiness;
                       
                    }
                    dbConnect.SubmitChanges();



                    var en = from c in dbConnect.sot_has_environments
                                where c.sot_id == Convert.ToInt32(sotid)
                                select c;

                    foreach (sot_has_environment rc in en)
                    {

                        rc.housekeeping = housekeeping;
                        rc.chemical_storage = chemical_storage;
                        rc.waste_diposal = waste_diposal;
                        rc.walking_working_surface = walking_working_surface;
                        rc.description = environment;

                    }
                    dbConnect.SubmitChanges();






                    int process_status = 1;//on process
                    sot_detail objInsert2 = new sot_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.sot_id = Convert.ToInt32(sotid);

                    dbConnect.sot_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();



                 

                }
                catch (Exception e)
                {

                }



                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                // return result;
            }
        }



        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getHazardbyid(string id, string user_id,string lang)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var q = from c in dbConnect.hazards
                        //join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                        //from e in joinE.DefaultIfEmpty()
                        join s in dbConnect.hazard_status on c.process_status equals s.id
                        where c.id == Convert.ToInt32(id)
                        select new
                        {
                            hazard_datetime = c.hazard_date,
                            report_date = c.report_date,
                            c.company_id,
                            c.function_id,
                            c.department_id,
                            c.division_id,
                            c.section_id,
                            c.hazard_area,
                            c.hazard_name,
                            c.hazard_detail,
                            c.preliminary_action,
                            c.type_action,
                            c.employee_id,
                            c.process_status,
                            c.typeuser_login,
                            c.doc_no,
                            c.verifying_date,
                            c.source_hazard,
                            c.level_hazard,
                            c.safety_officer_id,
                            c.area_owner_id,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            phone = c.phone,
                            c.fatality_prevention_element_id,
                            c.faltality_prevention_element_other,
                            c.step_form,
                            c.submit_report_form2,
                            c.id,
                            c.hazard_characteristic_id,
                            c.location_company_name_en,
                            c.location_company_name_th,
                            c.location_function_name_en,
                            c.location_function_name_th,
                            c.location_department_name_en,
                            c.location_department_name_th,
                            c.location_division_name_en,
                            c.location_division_name_th,
                            c.location_section_name_en,
                            c.location_section_name_th



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
                    string code_status = "";
                    if (v.process_status == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";

                    }
                    //else if (v.process_status == 2)
                    //{//delay

                    //    code_status = "<i class=\"fa fa-circle text-danger\"></i>";

                    //}
                    else if (v.process_status == 3)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-reject\"></i>";
                    }
                    else if (v.process_status == 2)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i>";

                    }

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

                            if (Session["country"].ToString() == "thailand")
                            {
                                step = step + "(" + v_step + " - Area OH&S)";

                            }
                            else if (Session["country"].ToString() == "srilanka")
                            {
                                step = step + "(" + v_step + " - Area Supervisor)";
                            }


                        }
                        else if (v.step_form == 2)
                        {
                            string v_step = chageDataLanguage("ตรวจสอบรายงานแหล่งอันตราย", "Verify Hazard Report", lang);

                            if (v.submit_report_form2 == null)
                            {

                                if (Session["country"].ToString() == "thailand")
                                {
                                    step = step + "(" + v_step + " - Area OH&S)";

                                }
                                else if (Session["country"].ToString() == "srilanka")
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
                                    where c.country == Session["country"].ToString()
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
                        hazard_area = v.hazard_area,
                        hazard_name = v.hazard_name,
                        hazard_detail = v.hazard_detail,
                        preliminary_action = v.preliminary_action,
                        type_action = v.type_action,
                        name_modify = user_name_modify,
                        datetime_modify = datetime_modify,
                        phone = v.phone,
                        status = code_status + " " + v.status + " " + step,
                        employee_name = getEmployeeByTypeLogin(v.employee_id, v.typeuser_login, lang),
                        doc_no = v.doc_no,
                        v.source_hazard,
                        verifying_date = v.verifying_date == null ? FormatDates.getDateShowFromDate(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())), lang) : FormatDates.getDateShowFromDate(Convert.ToDateTime(v.verifying_date), lang),
                        v.level_hazard,
                        name_security = fullname_security,
                        name_area_owner = fullname_area_owner,
                        v.fatality_prevention_element_id,
                        v.faltality_prevention_element_other,
                        v.process_status,
                        v.hazard_characteristic_id,
                        v.location_company_name_en,
                        v.location_company_name_th,
                        v.location_function_name_en,
                        v.location_function_name_th,
                        v.location_department_name_en,
                        v.location_department_name_th,
                        v.location_division_name_en,
                        v.location_division_name_th,
                        v.location_section_name_en,
                        v.location_section_name_th,
                        v.area_owner_id


                    };

                    ArrayList dt = new ArrayList();
                    dt.Add(result);


                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(dt));

                }

            }

        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getHealthbyid(string id, string user_id, string lang)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var q = from c in dbConnect.healths
                        join e in dbConnect.employees on c.health_employee_id equals e.employee_id
                        //from e in joinE.DefaultIfEmpty()
                        join s in dbConnect.hazard_status on c.process_status equals s.id
                        where c.id == Convert.ToInt32(id)
                        select new
                        {
                            report_date = c.report_date,
                            c.company_id,
                            c.function_id,
                            c.department_id,
                            c.division_id,
                            c.section_id,
                            c.age,
                            c.year_health,
                            c.service_year,
                            c.service_year_current,
                            c.job_type_machine_type,
                            c.employee_id,
                            c.health_employee_id,
                            c.process_status,
                            c.typeuser_login,
                            c.doc_no,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            c.step_form,
                            c.id,
                            c.location_company_name_en,
                            c.location_company_name_th,
                            c.location_function_name_en,
                            c.location_function_name_th,
                            c.location_department_name_en,
                            c.location_department_name_th,
                            c.location_division_name_en,
                            c.location_division_name_th,
                            c.location_section_name_en,
                            c.location_section_name_th,
                            first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                            last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),
                            c.significant_insignificant,
                            c.birth_date,
                            c.hiring_date,
                            c.hospital_id



                        };



                foreach (var v in q)
                {

                    string user_name_modify = "";
                    string datetime_modify = "";

                    var doc_no = dbConnect.health_details.Max(x => x.id);

                    var d = (from c in dbConnect.health_details
                             where c.health_id == Convert.ToInt32(id)
                             orderby c.id descending
                             select c).Take(1);



                    foreach (var g in d)
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


                        

                    }//end for each

                    string code_status = "";
                    if (v.process_status == 1)//on process
                    {
                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";

                    }
                    else if (v.process_status == 2)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i>";

                    }
                    else if (v.process_status == 3)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-reject\"></i>";
                    }
                






                    string step = "";


                    if (v.process_status != 2 && v.process_status != 3)//ไม่ใช้ close กับ reject
                    {

                        if (v.step_form == 1)//area oh&s
                        {
                            string v_step = chageDataLanguage("แผนฟื้นฟูสุขภาพพนักงาน", "Health Rehabilitation", lang);

                            step = step + "(" + v_step + " - Area OH&S)";



                        }
                        else if (v.step_form == 2)
                        {

                            string v_step = chageDataLanguage("มาตรการการฟื้นฟูสุขภาพ", "Health Rehabilitation Action", lang);

                            step = step + "(" + v_step + " - Area OH&S)";

                        }
                        else if (v.step_form == 3)
                        {
                            string v_step = chageDataLanguage("ขอปิดแผนฟื้นฟูสุขภาพพนักงาน", "Request to Close Health Rehabilitation", lang);
                            bool check_close = true;

                            var s = from c in dbConnect.close_step_healths
                                    where c.country == Session["country"].ToString()
                                    orderby c.step descending
                                    select c;

                            foreach (var r in s)
                            {
                                var w = from c in dbConnect.log_request_close_healths
                                        where c.health_id == v.id && c.status == "A"
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




                    var result = new
                    {

                        health_report = FormatDates.getDatetimeShow(Convert.ToDateTime(v.report_date), lang),

                        company_id = v.company_id,
                        function_id = v.function_id,
                        department_id = v.department_id,
                        division_id = v.division_id,
                        section_id = v.section_id,
                        v.health_employee_id,
                        age = v.age,
                        v.year_health,
                        v.service_year,
                        v.service_year_current, 
                        v.job_type_machine_type,
                        name_modify = user_name_modify,
                        datetime_modify = datetime_modify,
                        status = code_status + " " + v.status + " " + step,
                        employee_name = getEmployeeByTypeLogin(v.employee_id, v.typeuser_login, lang),
                        doc_no = v.doc_no,                     
                        v.process_status,
                        v.location_company_name_en,
                        v.location_company_name_th,
                        v.location_function_name_en,
                        v.location_function_name_th,
                        v.location_department_name_en,
                        v.location_department_name_th,
                        v.location_division_name_en,
                        v.location_division_name_th,
                        v.location_section_name_en,
                        v.location_section_name_th,
                        v.first_name,
                        v.last_name,
                        v.significant_insignificant,
                        birth_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(v.birth_date), lang),
                        hiring_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(v.hiring_date), lang),
                        v.hospital_id


                    };

                    ArrayList dt = new ArrayList();
                    dt.Add(result);


                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(dt));

                }

            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSotbyid(string id, string user_id,string pagetype, string lang)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var q = from c in dbConnect.sots
                        join s in dbConnect.sot_status on c.process_status equals s.id into joinS
                        join r in dbConnect.sot_has_reactions_peoples on c.id equals r.sot_id into joinR
                        join po in dbConnect.sot_has_position_peoples on c.id equals po.sot_id into joinPo
                        join per in dbConnect.sot_has_personal_protection_equipments on c.id equals per.sot_id into joinPer
                        join t in dbConnect.sot_has_tools_equipments on c.id equals t.sot_id into joinT
                        join pro in dbConnect.sot_has_procedures on c.id equals pro.sot_id into joinPro
                        join o in dbConnect.sot_has_orderliness_tidinesses on c.id equals o.sot_id into joinO
                        join e in dbConnect.sot_has_environments on c.id equals e.sot_id into joinE
                        from s in joinS.DefaultIfEmpty()
                        from r in joinR.DefaultIfEmpty()
                        from po in joinPo.DefaultIfEmpty()
                        from per in joinPer.DefaultIfEmpty()
                        from t in joinT.DefaultIfEmpty()
                        from pro in joinPro.DefaultIfEmpty()
                        from o in joinO.DefaultIfEmpty()
                        from e in joinE.DefaultIfEmpty()

                        where c.id == Convert.ToInt32(id)
                        select new
                        {
                            sot_datetime = c.sot_date,
                            c.report_date,
                            c.country_id,
                            c.company_id,
                            c.function_id,
                            c.department_id,
                            c.division_id,
                            c.site_id,                         
                            c.process_status,                           
                            c.doc_no,
                            c.location,
                            c.comment,
                            c.type_work,                        
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            c.edit_form,
                            c.status_form,
                            c.sot_date_end,
                            c.type_employment_id,

                            r.changinge_position,
                            r.stopping_work,
                            r.rearranging_job,
                            r.hiding_dodging,
                            r.changing_tools,
                            r.applying_lockout,
                            r.adjusting_ppe,
                            reactions_people = r.description,

                            po.striking_against,
                            po.caught_between,
                            po.inhaling,
                            po.absorbing,
                            po.electricity,
                            po.falling,
                            po.struck_by,
                            po.line_fire,
                            po.eyes_tasks,
                            po.lifting_lowering,
                            po.posture,
                            postion_people = po.description,

                            per.head,
                            per.ears_eyes,
                            per.face_respiratory,
                            per.hand_arms,
                            per.feet_legs,
                            personal_protection_equipment = per.description,

                            t.right_job,
                            t.used_correctly,
                            t.safe_conditions,
                            t.hamesses,
                            t.barricades_warning_lights,
                            t.chock_restraints,
                            t.prejob_safety_checks,
                            tools_equipment = t.description,

                            pro.standard_adequate_job,
                            pro.standard_established,
                            pro.standard_maintained,
                            pro.isolation_lockout,
                            pro.hot_work_permit,
                            pro.confined_space_permit,
                            pro.electrical_permit,
                            pro.work_height_permit,
                            pro.rescue_plan_place,
                            procedures = pro.description,

                            o.standards_established_understood,
                            o.walkway_passageways,
                            o.disorganized_tools_bench,
                            o.materials_storage,
                            o.obstructions_leaning_items,
                            o.stairs_platforms,
                            orderliness_tidiness = o.description,


                            e.housekeeping,
                            e.chemical_storage,
                            e.waste_diposal,
                            e.walking_working_surface,
                            environment = e.description,

                            c.location_company_name_en,
                            c.location_company_name_th,
                            c.location_function_name_en,
                            c.location_function_name_th,
                            c.location_department_name_en,
                            c.location_department_name_th,
                            c.location_division_name_en,
                            c.location_division_name_th,
          
                   

                        };



                foreach (var v in q)
                {
                    //string[] incident_datetime = (v.incident_date.ToString()).Split(' ');
                    string sot_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(v.sot_datetime), lang);
                    string sot_time = FormatDates.getTimeShowFromDate(Convert.ToDateTime(v.sot_datetime), lang);
                    string sot_time_end = FormatDates.getTimeShowFromDate(Convert.ToDateTime(v.sot_date_end), lang);
                    string report_date = FormatDates.getDatetimeShow(Convert.ToDateTime(v.report_date), lang);

                    string user_name_modify = "";
                    string datetime_modify = "";

                    var doc_no = dbConnect.sot_details.Max(x => x.id);

                    var d = (from c in dbConnect.sot_details
                             where c.sot_id == Convert.ToInt32(id)
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

                    }
                    
                    //end for each
                    string code_status = "";
                    if (v.process_status == 1)//on process
                    {

                        code_status = "<i class=\"fa fa-circle text-warning\"></i>";

                    }
                    else if (v.process_status == 3)
                    {//reject

                        code_status = "<i class=\"fa fa-circle text-reject\"></i>";
                    }
                    else if (v.process_status == 2)//close
                    {
                        code_status = "<i class=\"fa fa-circle text-info\"></i>";

                    }

               




                   // string[] split_sot_time = sot_time.Split(':');
                    //string[] split_sot_time_end = sot_time_end.Split(':');
                    var result = new
                    {
                        sot_date = sot_date,
                        sot_time = sot_time,
                        sot_time_end = sot_time_end,
                        report_date,
                        //sot_hour = split_sot_time[0],
                        //sot_minute = split_sot_time[1],
                        //sot_hour_end = split_sot_time_end[0],
                        //sot_minute_end = split_sot_time_end[1],

                        country_id = v.country_id,
                        company_id = v.company_id,
                        function_id = v.function_id,
                        department_id = v.department_id,
                        division_id = v.division_id,
                        site_id = v.site_id,
                        v.location,
                        v.comment,
                        v.type_work,
                        name_modify = user_name_modify,
                        datetime_modify = datetime_modify,
                        status = code_status + " " + v.status,
                        doc_no = v.doc_no,
                        sotteam = getEmployeeSot(id,pagetype,lang),
                        v.status_form,
                        v.type_employment_id,
                        v.process_status,

                        v.changinge_position,
                        v.stopping_work,
                        v.rearranging_job,
                        v.hiding_dodging,
                        v.changing_tools,
                        v.applying_lockout,
                        v.adjusting_ppe,
                        v.reactions_people,

                        v.striking_against,
                        v.caught_between,
                        v.inhaling,
                        v.absorbing,
                        v.electricity,
                        v.falling,
                        v.struck_by,
                        v.line_fire,
                        v.eyes_tasks,
                        v.lifting_lowering,
                        v.posture,
                        v.postion_people,

                        v.head,
                        v.ears_eyes,
                        v.face_respiratory,
                        v.hand_arms,
                        v.feet_legs,
                        v.personal_protection_equipment,

                        v.right_job,
                        v.used_correctly,
                        v.safe_conditions,
                        v.hamesses,
                        v.barricades_warning_lights,
                        v.chock_restraints,
                        v.prejob_safety_checks,
                        v.tools_equipment,

                        v.standard_adequate_job,
                        v.standard_established,
                        v.standard_maintained,
                        v.isolation_lockout,
                        v.hot_work_permit,
                        v.confined_space_permit,
                        v.electrical_permit,
                        v.work_height_permit,
                        v.rescue_plan_place,
                        v.procedures,

                        v.standards_established_understood,
                        v.walkway_passageways,
                        v.disorganized_tools_bench,
                        v.materials_storage,
                        v.obstructions_leaning_items,
                        v.stairs_platforms,
                        v.orderliness_tidiness,

                        v.housekeeping,
                        v.chemical_storage,
                        v.waste_diposal,
                        v.walking_working_surface,
                        v.environment,

                        v.location_company_name_en,
                        v.location_company_name_th,
                        v.location_function_name_en,
                        v.location_function_name_th,
                        v.location_department_name_en,
                        v.location_department_name_th,
                        v.location_division_name_en,
                        v.location_division_name_th,
                      


                    };

                    ArrayList dt = new ArrayList();
                    dt.Add(result);


                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(dt));

                }

            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getEmployeeDefault(string user_id, string lang)
        {
     
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.employees
                         where c.employee_id == user_id
                         select new
                         {
                             id = c.employee_id,
                             name = chageDataLanguage(c.first_name_th + " " + c.last_name_th, c.first_name_en + " " + c.last_name_en, lang),
                             @readonly = true
                             
                         });

                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(v));

            }

           
        }



        public string getEmployeeSot(string sot_id,string pagetype,string lang)
        {
            string json = "";
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = (from c in dbConnect.employee_has_sots
                         join e in dbConnect.employees on c.employee_id equals e.employee_id
                         where c.sot_id == Convert.ToInt32(sot_id)
                         select new
                         {
                             id = c.employee_id,
                             name = chageDataLanguage(e.first_name_th+" "+e.last_name_th,e.first_name_en+" "+e.last_name_en,lang),
                            @readonly = pagetype == "view"?true:false
                         });//.Take(25);

             

                JavaScriptSerializer js = new JavaScriptSerializer();
                json = js.Serialize(v);
            }

            return json;
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getImageHazard(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                ArrayList ls = new ArrayList();

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
                    //string pathfolder = string.Format("{0}\\upload\\hazard\\" + name_folder, Server.MapPath(@"\"));
                    string pathfolder = string.Format("{0}" + pathupload + name_folder, Server.MapPath(@"\"));





                    string[] images = Directory.GetFiles(pathfolder, "*")
                                             .Select(Path.GetFileName).OrderByDescending(Path.GetFileName).Take(2)
                                             .ToArray();

                    // FileInfo[] files = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();

                    foreach (var d in images)
                    {
                        string pathfolder_new = pathfolder + "\\" + d;
                        FileInfo f = new FileInfo(pathfolder_new);
                        string size = f.Length.ToString();
                        var v = new Dictionary<string, string>
                       {
                           { "path", "upload/hazard/"+name_folder+"/"+d },
                           { "folder", name_folder},
                           { "name", d },
                           { "size", size },
                
                       };

                        ls.Add(v);
                    }


                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(ls));
            }
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getImageHealth(string report_date,string file_name,string user_id,string id ,string lang)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                
                ArrayList ls = new ArrayList();
                string rp_date = "";
                if (id != "")
                {
       

                    var q = from c in dbConnect.healths
                            where c.id == Convert.ToInt32(id)
                            select new
                            {
                                user_id = c.employee_id,
                                report_date = FormatDates.getDatetimeShow(Convert.ToDateTime(c.report_date), "en"),
                            };



                    foreach (var v in q)
                    {
                        user_id = v.user_id;
                        rp_date = v.report_date.ToString();
                    }

                }
              
                string[] fname = file_name.Split(',');
                string reportdate = "";


                if (report_date != "")
                {
                    reportdate = FormatDates.changeDateTimeUpload(report_date, lang); ;
                }
                else
                {
                    reportdate = rp_date;
                }
                    
                    
                string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathuploadhealth"];
                string name_folder = user_id + "_" + FormatDates.getDateTimeNoDash(reportdate.Trim());
                
                //string pathfolder = string.Format("{0}\\upload\\hazard\\" + name_folder, Server.MapPath(@"\"));
                string pathfolder = string.Format("{0}" + pathupload + name_folder, Server.MapPath(@"\"));





                string[] images = Directory.GetFiles(pathfolder, "*")
                                            .Select(Path.GetFileName).OrderByDescending(Path.GetFileName)
                                            .ToArray();

                // FileInfo[] files = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();

                foreach (var d in images)
                {
                    if (Array.IndexOf(fname,d)!=-1)
                    {
                        string pathfolder_new = pathfolder + "\\" + d;
                        FileInfo f = new FileInfo(pathfolder_new);
                        string size = f.Length.ToString();
                        var v = new Dictionary<string, string>
                        {
                            { "path", "upload/health/"+name_folder+"/"+d },
                            { "folder", name_folder},
                            { "name", d },
                            { "size", size },
                
                        };

                        ls.Add(v);

                    }
                   
                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(ls));
            }
        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateHazard2(  string verifying_date,
                                    string source_hazard, 
                                    string level_hazard,
                                    string safety_officer_id,   
                                    string fatality_prevention_element_id,
                                    string faltality_prevention_element_other,
                                    string user_id,
                                    string typelogin,
                                    string hazardid,
                                    string stepform,
                                    string typebutton,          
                                    string group_id

                                    )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = hazardid;

                var query = from c in dbConnect.hazards
                            where c.id == Convert.ToInt32(hazardid)
                            select c;

                foreach (hazard rc in query)
                {
                    rc.verifying_date = FormatDates.changeDateTimeDB(verifying_date, Session["lang"].ToString());
                    rc.source_hazard = Convert.ToInt32(source_hazard);
                    rc.level_hazard = level_hazard;

                    if (fatality_prevention_element_id != "")
                    {
                        rc.fatality_prevention_element_id = Convert.ToInt16(fatality_prevention_element_id);
                    }

                    rc.faltality_prevention_element_other = faltality_prevention_element_other;
                    // rc.step_form = Convert.ToByte(stepform);
                    if (typebutton == "report")
                    {
                        rc.submit_report_form = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        rc.submit_report_form2 = Convert.ToInt32(group_id);
                    }
                    else
                    {
                        rc.edit_form2 = Convert.ToInt32(group_id);

                    }
                    rc.safety_officer_id = safety_officer_id;



                }

                try
                {
                    dbConnect.SubmitChanges();

                    int process_status = 1;//on process
                    hazard_detail objInsert2 = new hazard_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.hazard_id = Convert.ToInt32(hazardid);

                    dbConnect.hazard_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();


                    if (typebutton == "report")
                    {
                        if (Session["country"].ToString() == "thailand")
                        {
                            //////////////////////////////////by p.poo sent notification/////////////////////////////////

                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            string[] alert_to_groups = { "AreaSuperervisor" };
                            sn.InsertHazardNotification(2, Convert.ToInt32(hazardid), alert_to_groups, Session["timezone"].ToString(), "AreaSuperervisor");
                            ///////////////////////////////////end//////////////////////////////////////////////////////
                        }
                      
                    }


                }
                catch (Exception e)
                {
                    result = e.Message;
                }

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateHazard3  (
                                    string area_owner_id,
                                    string user_id,
                                    string typelogin,
                                    string hazardid,
                                    string stepform,
                                    string typebutton,  
                                    string group_id

                                    )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = hazardid;

                var query = from c in dbConnect.hazards
                            where c.id == Convert.ToInt32(hazardid)
                            select c;

                foreach (hazard rc in query)
                {

                    rc.area_owner_id = area_owner_id;
                    rc.edit_form3 = Convert.ToInt32(group_id);


                }

                try
                {
                    dbConnect.SubmitChanges();

                    int process_status = 1;//on process
                    hazard_detail objInsert2 = new hazard_detail();
                    objInsert2.employee_id = user_id;
                    objInsert2.type_login = typelogin;
                    objInsert2.action_time = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    objInsert2.process_status = process_status;
                    objInsert2.hazard_id = Convert.ToInt32(hazardid);

                    dbConnect.hazard_details.InsertOnSubmit(objInsert2);

                    dbConnect.SubmitChanges();




                }
                catch (Exception e)
                {
                    result = e.Message;
                }

                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void requestActionHazard(string id, string type, string remark)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var query = from c in dbConnect.process_actions
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (process_action rc in query)
                {
                    if (type == "close")
                    {
                        rc.action_status_id = 4;
                        rc.remark = "";
                        if (rc.date_complete == null)
                        {
                            rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        }
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { };
                        sn.InsertHazardNotification(6, rc.hazard_id, alert_to_groups, Session["timezone"].ToString(),"", rc.id);
                        //  sn.InsertHazardNotification(7, rc.hazard_id, alert_to_groups, rc.id);

                        ////////////////////////////////////////end///////////////////////////////////

                    }
                    else if (type == "reject")
                    {

                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        rc.remark = remark;
                        rc.action_status_id = 6;//reject
                        rc.attachment_file = "";
                        rc.date_complete = null;
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { };
                        sn.InsertHazardNotification(7, rc.hazard_id, alert_to_groups, Session["timezone"].ToString(),"", rc.id);

                        ////////////////////////////////////////end///////////////////////////////////


                    }
                    else if (type == "cancel")
                    {
                        rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        rc.action_status_id = 5;
                    }
                    else if (type == "request close")
                    {
                        rc.action_status_id = 2;
                        rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { "AreaSuperervisor" };
                        sn.InsertHazardNotification(5, rc.hazard_id, alert_to_groups, Session["timezone"].ToString(), "AreaSuperervisor", rc.id);

                        ////////////////////////////////////////end///////////////////////////////////

                    }
                }

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void requestActionSot(string id, string type, string remark)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var query = from c in dbConnect.process_action_sots
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (process_action_sot rc in query)
                {
                    if (type == "close")
                    {
                        rc.action_status_id = 4;
                        rc.remark = "";
                        if (rc.date_complete == null)
                        {
                            rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        }
                        ////////////////////////////////////by p.poo sent notification/////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { };
                        sn.InsertSotNotification(6, rc.sot_id, alert_to_groups, Session["timezone"].ToString(),"", rc.id);
                        //  sn.InsertHazardNotification(7, rc.hazard_id, alert_to_groups, rc.id);

                        //////////////////////////////////////////end///////////////////////////////////

                    }
                    else if (type == "reject")
                    {

                        //////////////////////////////////by p.poo sent notification/////////////////////////////////
                        rc.remark = remark;
                        rc.action_status_id = 6;//reject
                        rc.attachment_file = "";
                        rc.date_complete = null;


                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { };
                        sn.InsertSotNotification(7, rc.sot_id, alert_to_groups,Session["timezone"].ToString(),"", rc.id);

                        ////////////////////////////////////////end///////////////////////////////////


                    }
                    else if (type == "cancel")
                    {
                        rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        rc.action_status_id = 5;
                    }
                    else if (type == "request close")
                    {
                        rc.action_status_id = 2;
                        rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                        ////////////////////////////////by p.poo sent notification/////////////////////////////////
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { "AreaManager" };
                        sn.InsertSotNotification(5, rc.sot_id, alert_to_groups, Session["timezone"].ToString(),"AreaManager", rc.id);

                        //////////////////////////////////////end///////////////////////////////////

                    }
                }

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }






        [WebMethod(EnableSession= true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createGroupCommunicationVP(List<string> employee_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {
                    int group_super_admin = 6;//check on group table old is 8

                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();
                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_super_admin;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }
        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createLegalDepartment(List<string> employee_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {
                    int group_super_admin = 7;//check on group table old is 9



                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();
                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_super_admin;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

        }
        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createGroupOHS(List<string> employee_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {
                    int group_super_admin = 8;//check on group table old is 10


                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();
                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_super_admin;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createGroupOHSHazard(List<string> employee_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {
                    int group_super_admin = 16;//check on group table old is 10



                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();
                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_super_admin;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createGroupOHSHealth(List<string> employee_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {
                    int group_super_admin = 17;//check on group table



                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();
                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_super_admin;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createGroupEXCO(List<string> employee_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {
                    int group_exco = 19;//check on group table



                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();
                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_exco;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createGroupCEO(List<string> employee_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {
                    int group_ceo = 20;//check on group table



                    foreach (var v in employee_id)
                    {
                        employee_has_group objInsert = new employee_has_group();
                        objInsert.employee_id = v.ToString();
                        objInsert.group_id = group_ceo;
                        objInsert.country = Session["country"].ToString();
                        dbConnect.employee_has_groups.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();

                    }


                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Clear();
                Context.Response.ContentType = "application/json";
                Context.Response.AddHeader("content-length", result.Length.ToString());
                Context.Response.Flush();
                Context.Response.Write(result);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteGroupCommunicationVP(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
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


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteLegalDepartment(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteGroupOHS(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteGroupOHSHazard(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteGroupOHSHealth(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteGroupEXCO(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteGroupCEO(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                var gr = from c in dbConnect.employee_has_groups
                         where c.id == Convert.ToInt32(id)
                         select c;
                foreach (var a in gr)
                {
                    dbConnect.employee_has_groups.DeleteOnSubmit(a);
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

                Context.Response.Output.Write(result);
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createSetting(string name_th, string name_en,string code, string setting_page_type)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {

                    String smenu = setting_page_type;

                    if (smenu == "TypeOfEmployee")
                    {
                        type_employment objInsert = new type_employment();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.type_employments.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "NatureOfInjury")
                    {
                        nature_injury objInsert = new nature_injury();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.nature_injuries.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "SeverityOfInjury")
                    {
                        severity_injury objInsert = new severity_injury();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.severity_injuries.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "BodyPart")
                    {
                        body_part objInsert = new body_part();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.body_parts.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "SourceOfHazard")
                    {
                        source_hazard objInsert = new source_hazard();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.source_hazards.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "SourceOfIncident")
                    {
                        source_incident objInsert = new source_incident();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.source_incidents.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "EventOrExposure")
                    {
                        event_exposure objInsert = new event_exposure();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.event_exposures.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "FPE")
                    {
                        fatality_prevention_element objInsert = new fatality_prevention_element();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.fatality_prevention_elements.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "RiskFactorRelateToWork")
                    {
                        risk_factor_relate_work objInsert = new risk_factor_relate_work();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.code = code;
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.risk_factor_relate_works.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "OccupationalHealthWork")
                    {
                        occupational_health_report objInsert = new occupational_health_report();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.occupational_health_reports.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "TypeOfControl")
                    {
                        type_control_health objInsert = new type_control_health();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.type_control_healths.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else if (smenu == "Hospital")
                    {
                        hospital objInsert = new hospital();
                        objInsert.name_th = name_th;
                        objInsert.name_en = name_en;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.updated_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        objInsert.country = Session["country"].ToString();
                        dbConnect.hospitals.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }




                    result = "true";

                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Output.Write(result);
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getSettingByid(string id, string setting_page_type)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                String smenu = setting_page_type;

                if (smenu == "TypeOfEmployee")
                {
                    var q = from c in dbConnect.type_employments
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "NatureOfInjury")
                {
                    var q = from c in dbConnect.nature_injuries
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "SeverityOfInjury")
                {
                    var q = from c in dbConnect.severity_injuries
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "BodyPart")
                {
                    var q = from c in dbConnect.body_parts
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "SourceOfHazard")
                {
                    var q = from c in dbConnect.source_hazards
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "SourceOfIncident")
                {
                    var q = from c in dbConnect.source_incidents
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "EventOrExposure")
                {
                    var q = from c in dbConnect.event_exposures
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "FPE")
                {
                    var q = from c in dbConnect.fatality_prevention_elements
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "RiskFactorRelateToWork")
                {
                    var q = from c in dbConnect.risk_factor_relate_works
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = c.code,


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "OccupationalHealthWork")
                {
                    var q = from c in dbConnect.occupational_health_reports
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "TypeOfControl")
                {
                    var q = from c in dbConnect.type_control_healths
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }
                else if (smenu == "Hospital")
                {
                    var q = from c in dbConnect.hospitals
                            where c.id == Convert.ToInt32(id)
                            select new
                            {

                                name_th = c.name_th,
                                name_en = c.name_en,
                                code = ""


                            };

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(q));
                }


            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateSetting(string id,
                                string name_th,
                                string name_en,
                                string code,
                                string setting_page_type
                                )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                String smenu = setting_page_type;

                if (smenu == "TypeOfEmployee")
                {
                    var q = from c in dbConnect.type_employments
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (type_employment rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;

                    }
                }
                else if (smenu == "NatureOfInjury")
                {
                    var q = from c in dbConnect.nature_injuries
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (nature_injury rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;

                    }
                }
                else if (smenu == "SeverityOfInjury")
                {
                    var q = from c in dbConnect.severity_injuries
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (severity_injury rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;

                    }
                }
                else if (smenu == "BodyPart")
                {
                    var q = from c in dbConnect.body_parts
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (body_part rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;

                    }
                }
                else if (smenu == "SourceOfHazard")
                {
                    var q = from c in dbConnect.source_hazards
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (source_hazard rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;

                    }
                }
                else if (smenu == "SourceOfIncident")
                {
                    var q = from c in dbConnect.source_incidents
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (source_incident rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;

                    }
                }
                else if (smenu == "EventOrExposure")
                {
                    var q = from c in dbConnect.event_exposures
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (event_exposure rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;

                    }
                }
                else if (smenu == "FPE")
                {
                    var q = from c in dbConnect.fatality_prevention_elements
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (fatality_prevention_element rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;

                    }
                }
                else if (smenu == "RiskFactorRelateToWork")
                {
                    var q = from c in dbConnect.risk_factor_relate_works
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (risk_factor_relate_work rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;
                        rc.code = code;

                    }
                }
                else if (smenu == "OccupationalHealthWork")
                {
                    var q = from c in dbConnect.occupational_health_reports
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (occupational_health_report rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;
                        rc.code = code;

                    }
                }
                else if (smenu == "TypeOfControl")
                {
                    var q = from c in dbConnect.type_control_healths
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (type_control_health rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;
                        rc.code = code;

                    }
                }
                else if (smenu == "Hospital")
                {
                    var q = from c in dbConnect.hospitals
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (hospital rc in q)
                    {
                        rc.name_th = name_th;
                        rc.name_en = name_en;
                        rc.code = code;

                    }
                }



                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void deleteSetting(string id,
                                string setting_page_type
                                )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                String smenu = setting_page_type;

                if (smenu == "TypeOfEmployee")
                {
                    var q = from c in dbConnect.type_employments
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (type_employment rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "NatureOfInjury")
                {
                    var q = from c in dbConnect.nature_injuries
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (nature_injury rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "SeverityOfInjury")
                {
                    var q = from c in dbConnect.severity_injuries
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (severity_injury rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "BodyPart")
                {
                    var q = from c in dbConnect.body_parts
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (body_part rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "SourceOfHazard")
                {
                    var q = from c in dbConnect.source_hazards
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (source_hazard rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "SourceOfIncident")
                {
                    var q = from c in dbConnect.source_incidents
                            where c.id == Convert.ToInt32(id)
                            select c;
                    foreach (source_incident rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "EventOrExposure")
                {
                    var q = from c in dbConnect.event_exposures
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (event_exposure rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "FPE")
                {
                    var q = from c in dbConnect.fatality_prevention_elements
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (fatality_prevention_element rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "RiskFactorRelateToWork")
                {
                    var q = from c in dbConnect.risk_factor_relate_works
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (risk_factor_relate_work rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "OccupationalHealthWork")
                {
                    var q = from c in dbConnect.occupational_health_reports
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (occupational_health_report rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "TypeOfControl")
                {
                    var q = from c in dbConnect.type_control_healths
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (type_control_health rc in q)
                    {
                        rc.status = "D";

                    }
                }
                else if (smenu == "Hospital")
                {
                    var q = from c in dbConnect.hospitals
                            where c.id == Convert.ToInt32(id)
                            select c;

                    foreach (hospital rc in q)
                    {
                        rc.status = "D";

                    }
                }



                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createProcessAction(string type_control,
                                        string action,
                                        string responsible_person,
                                        string due_date,
                                        string date_complete,
                                        string notify_contractor,
                                        string remark,
                                        string attachment_file,
                                        string employee_id,
                                        string contractor_id,
                                        string user_id,
                                        string hazard_id
                                        )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                process_action objInsert = new process_action();

                objInsert.type_control = Convert.ToInt32(type_control);
                objInsert.action = action;
                objInsert.responsible_person = responsible_person;
                objInsert.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());
                if (date_complete.Trim() != "")
                {
                    objInsert.date_complete = FormatDates.changeDateTimeDB(date_complete, Session["lang"].ToString());
                }

                objInsert.notify_contractor = notify_contractor;
                objInsert.remark = remark;
                objInsert.hazard_id = Convert.ToInt32(hazard_id);
                objInsert.action_status_id = 1;//on process
                objInsert.attachment_file = attachment_file;
                objInsert.employee_id = employee_id;
                // objInsert.root_cause_action = root_cause_action;
                objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                if (contractor_id != "")
                {
                    objInsert.contractor_id = Convert.ToInt32(contractor_id);
                }
                objInsert.assign_by_employee_id = user_id;

                dbConnect.process_actions.InsertOnSubmit(objInsert);

                dbConnect.SubmitChanges();


                //////////////////////////////////by p.poo sent notification/////////////////////////////////

                Class.SafetyNotification sn = new Class.SafetyNotification();
               

                if (Session["country"].ToString() == "srilanka")
                {
                    string[] alert_to_groups = { };
                    sn.InsertHazardNotification(3, Convert.ToInt32(hazard_id), alert_to_groups, Session["timezone"].ToString(),"", objInsert.id);

                }else if (Session["country"].ToString() == "thailand"){

                    string[] alert_to_groups = { "GroupOH&S" };
                    sn.InsertHazardNotification(3, Convert.ToInt32(hazard_id), alert_to_groups, Session["timezone"].ToString(),"", objInsert.id);

                }
              
                ////////////////////////////////////////end///////////////////////////////////

                Context.Response.Output.Write(objInsert.id);
            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createProcessActionSot( string type_control,
                                            string action,
                                            string responsible_person,
                                            string due_date,
                                            string date_complete,
                                            string notify_contractor,
                                            string remark,
                                            string attachment_file,
                                            string employee_id,
                                            string contractor_id,
                                            string user_id,
                                            string sot_id,
                                            string page_type
                                        )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                if (page_type == "add")
                {
                    if (Session["process_action_sot"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["process_action_sot"];
                        int count = dict.Count()+1;

                        ProcessActionSotEntity objInsert = new ProcessActionSotEntity();

                        objInsert.TypeControl = type_control;
                        objInsert.Action = action;
                        objInsert.ResponsiblePerson = responsible_person;
                        objInsert.DueDate = due_date;                     
                        objInsert.DateComplete = date_complete;
                        
                        objInsert.NotifyContractor = notify_contractor;
                        objInsert.Remark = remark;
                        objInsert.Action_status_id= 1;//on process
                        objInsert.Employee_id= employee_id;
                        objInsert.Contractor_id = contractor_id;
                        objInsert.assign_by_employee_id = user_id;
                        objInsert.id = count;
                        dict.Add(count, objInsert);
                        Session["process_action_sot"] = dict;

                        Context.Response.Output.Write(1);
                    }
                    else
                    {
                        Dictionary<Int32, object> dict = new Dictionary<Int32, object>();

                        ProcessActionSotEntity objInsert = new ProcessActionSotEntity();

                        objInsert.TypeControl = type_control;
                        objInsert.Action = action;
                        objInsert.ResponsiblePerson = responsible_person;
                        objInsert.DueDate = due_date;
                        objInsert.DateComplete = date_complete;

                        objInsert.NotifyContractor = notify_contractor;
                        objInsert.Remark = remark;
                        objInsert.Action_status_id = 1;//on process
                        objInsert.Employee_id = employee_id;
                        objInsert.Contractor_id = contractor_id;
                        objInsert.assign_by_employee_id = user_id;
                        objInsert.id = 1;
                        dict.Add(1,objInsert);
                        Session["process_action_sot"] = dict;


                        Context.Response.Output.Write(1);
                    }

                   
                }
                else
                {
                    process_action_sot objInsert = new process_action_sot();

                    objInsert.type_control = Convert.ToInt32(type_control);
                    objInsert.action = action;
                    objInsert.responsible_person = responsible_person;
                    objInsert.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());
                    if (date_complete.Trim() != "")
                    {
                        objInsert.date_complete = FormatDates.changeDateTimeDB(date_complete, Session["lang"].ToString());
                    }

                    objInsert.notify_contractor = notify_contractor;
                    objInsert.remark = remark;
                    objInsert.sot_id = Convert.ToInt32(sot_id);
                    objInsert.action_status_id = 1;//on process
                    objInsert.attachment_file = attachment_file;
                    objInsert.employee_id = employee_id;
                    objInsert.assign_by_employee_id = user_id;
                    // objInsert.root_cause_action = root_cause_action;
                    objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                    if (contractor_id != "")
                    {
                        objInsert.contractor_id = Convert.ToInt32(contractor_id);
                    }

                    dbConnect.process_action_sots.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();


                    ////////////////////////////////////by p.poo sent notification/////////////////////////////////

                    Class.SafetyNotification sn3 = new Class.SafetyNotification();
                    string[] alert_to_groups3 = { "AreaManager" };
                    sn3.InsertSotNotification(3, Convert.ToInt32(sot_id), alert_to_groups3, Session["timezone"].ToString(),"AreaManager", objInsert.id);


                    //////////////////////////////////////////end///////////////////////////////////
                    Context.Response.Output.Write(objInsert.id);

                }


               
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createRiskFactor(       string risk_factor_relate_work,
                                            string other_risk_factor,
                                            string year_factor,
                                            string duration_risk_factor,
                                            string file_risk_factor,
                                            string result_risk_factor,
                                            string user_id,
                                            string health_id,
                                            string page_type,
                                            string monitoring_environment,
                                            string monitoring_results
                                        )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                if (page_type == "add")
                {
                    if (Session["risk_factor"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["risk_factor"];
                        int count = dict.Count() + 1;

                        RiskFactorEntity objInsert = new RiskFactorEntity();

                        objInsert.risk_factor_relate_work_id = Convert.ToInt16(risk_factor_relate_work);
                        objInsert.Other = other_risk_factor;
                        objInsert.Year = year_factor;
                        objInsert.Result = result_risk_factor;
                        objInsert.Duration_risk_factor_id = Convert.ToInt16(duration_risk_factor);
                        objInsert.File_risk_factor = file_risk_factor;
                        objInsert.monitoring_environment = monitoring_environment;
                        objInsert.monitoring_results = monitoring_results;
                        objInsert.Status = "A";

                        objInsert.id = count;
                        dict.Add(count, objInsert);
                        Session["risk_factor"] = dict;

                        Context.Response.Output.Write(1);
                    }
                    else
                    {
                        Dictionary<Int32, object> dict = new Dictionary<Int32, object>();

                        RiskFactorEntity objInsert = new RiskFactorEntity();

                        objInsert.risk_factor_relate_work_id = Convert.ToInt16(risk_factor_relate_work);
                        objInsert.Other = other_risk_factor;
                        objInsert.Year = year_factor;
                        objInsert.Result = result_risk_factor;
                        objInsert.Duration_risk_factor_id = Convert.ToInt16(duration_risk_factor);
                        objInsert.File_risk_factor = file_risk_factor;
                        objInsert.monitoring_environment = monitoring_environment;
                        objInsert.monitoring_results = monitoring_results;
                        objInsert.id = 1;
                        objInsert.Status = "A";
                        dict.Add(1, objInsert);
                        Session["risk_factor"] = dict;


                        Context.Response.Output.Write(1);
                    }


                }
                else
                {
                    risk_factor_relate_work_action objInsert = new risk_factor_relate_work_action();

                    objInsert.risk_factor_relate_work_id = Convert.ToInt16(risk_factor_relate_work);
                    objInsert.other = other_risk_factor;
                    objInsert.year = year_factor;
                    objInsert.result = result_risk_factor;
                    objInsert.duration_risk_factor_id = Convert.ToInt16(duration_risk_factor);
                    objInsert.file_risk_factor = file_risk_factor;
                    objInsert.health_id = Convert.ToInt32(health_id);
                    objInsert.monitoring_environment = monitoring_environment;
                    objInsert.monitoring_results = monitoring_results;
                    objInsert.status = "A";
                    dbConnect.risk_factor_relate_work_actions.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();


                    Context.Response.Output.Write(objInsert.id);

                }



            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createOccupationalHealth(string occupational_health,
                                            string repeat_health_check,
                                            string filename_result_health,
                                            string filename_repeat_result_health,
                                            string abnormal_audiogram,
                                            string hearing_threshold_level,
                                            string chronic_diseases_ear,
                                            string specify_chronic_diseases_ear,
                                            string abnormal_pulmonary_function,
                                            string smoked_cigarettes,
                                            string cigarettes_per_day,
                                            string years,
                                            string months,
                                            string smoked_other,
                                            string health_id,
                                            string page_type
                                        )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                if (page_type == "add")
                {
                    if (Session["occupational_health"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["occupational_health"];
                        int count = dict.Count() + 1;

                        OccupationalHealthEntity objInsert = new OccupationalHealthEntity();

                        objInsert.occupational_health_report_id = Convert.ToInt16(occupational_health);
                        objInsert.RepeatHealthCheck = repeat_health_check;
                        objInsert.FileHealthCheck = filename_result_health;
                        objInsert.FlieRepeatHealthCheck = filename_repeat_result_health;
                        objInsert.abnormal_audiogram = abnormal_audiogram;
                        objInsert.hearing_threshold_level = hearing_threshold_level;
                        objInsert.chronic_diseases_ear = chronic_diseases_ear;
                        objInsert.specify_chronic_diseases_ear = specify_chronic_diseases_ear;
                        objInsert.abnormal_pulmonary_function = abnormal_pulmonary_function;
                        objInsert.smoked_cigarettes = smoked_cigarettes;

                        if (cigarettes_per_day != "") objInsert.cigarette_per_day = Convert.ToInt16(cigarettes_per_day);
                        if (years != "") objInsert.smoked_years = Convert.ToInt16(years);
                        if (months != "") objInsert.smoked_months = Convert.ToInt16(months);
                        objInsert.smoked_cigarettes_other = smoked_other;
               
                        objInsert.Status = "A";
                     
                        objInsert.id = count;
                        dict.Add(count, objInsert);
                        Session["occupational_health"] = dict;

                        Context.Response.Output.Write(1);
                    }
                    else
                    {
                        Dictionary<Int32, object> dict = new Dictionary<Int32, object>();

                        OccupationalHealthEntity objInsert = new OccupationalHealthEntity();

                        objInsert.occupational_health_report_id = Convert.ToInt16(occupational_health);
                        objInsert.RepeatHealthCheck = repeat_health_check;
                        objInsert.FileHealthCheck = filename_result_health;
                        objInsert.FlieRepeatHealthCheck = filename_repeat_result_health;
                        objInsert.abnormal_audiogram = abnormal_audiogram;
                        objInsert.hearing_threshold_level = hearing_threshold_level;
                        objInsert.chronic_diseases_ear = chronic_diseases_ear;
                        objInsert.specify_chronic_diseases_ear = specify_chronic_diseases_ear;

                        objInsert.abnormal_pulmonary_function = abnormal_pulmonary_function;
                        objInsert.smoked_cigarettes = smoked_cigarettes;

                        if (cigarettes_per_day != "") objInsert.cigarette_per_day = Convert.ToInt16(cigarettes_per_day);
                        if (years != "") objInsert.smoked_years = Convert.ToInt16(years);
                        if (months != "") objInsert.smoked_months = Convert.ToInt16(months);
                        objInsert.smoked_cigarettes_other = smoked_other;

                        objInsert.id = 1;
                        objInsert.Status = "A";
                        dict.Add(1, objInsert);
                        Session["occupational_health"] = dict;


                        Context.Response.Output.Write(1);
                    }


                }
                else
                {
                    occupational_health_report_action objInsert = new occupational_health_report_action();

                    objInsert.occupational_health_report_id = Convert.ToInt16(occupational_health);
                    objInsert.repeat_health_check = repeat_health_check;
                    objInsert.file_health_check = filename_result_health;
                    objInsert.flie_repeat_health_check = filename_repeat_result_health;
                    objInsert.abnormal_audiogram = abnormal_audiogram;
                    objInsert.hearing_threshold_level = hearing_threshold_level;
                    objInsert.chronic_diseases_ear = chronic_diseases_ear;
                    objInsert.specify_chronic_diseases_ear = specify_chronic_diseases_ear;

                    objInsert.abnormal_pulmonary_function = abnormal_pulmonary_function;
                    objInsert.smoked_cigarettes = smoked_cigarettes;

                    if (cigarettes_per_day != "") objInsert.cigarette_per_day = Convert.ToInt16(cigarettes_per_day);
                    if (years != "") objInsert.smoked_years = Convert.ToInt16(years);
                    if (months != "") objInsert.smoked_months = Convert.ToInt16(months);
                    objInsert.smoked_cigarettes_other = smoked_other;
                    objInsert.health_id = Convert.ToInt32(health_id);
                    objInsert.status = "A";

                    dbConnect.occupational_health_report_actions.InsertOnSubmit(objInsert);
                    dbConnect.SubmitChanges();


                    Context.Response.Output.Write(objInsert.id);

                }



            }

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void createProcessActionHealth(string type_control,
                                            string action,
                                            string responsible_person,
                                            string employee_id,
                                            string due_date,
                                            string remark,
                                            string status,
                                            string filename_opinion_doctor,
                                            string filename_recovery_plan,
                                            string filename_process_action,
                                            string user_id,
                                            string health_id,
                                            string page_type
                                        )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                    process_action_health objInsert = new process_action_health();

                    objInsert.type_control_id = Convert.ToInt32(type_control);
                    objInsert.action = action;
                    objInsert.responsible_person = responsible_person;
                    objInsert.employee_id = employee_id;
                    objInsert.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());
                    objInsert.action_status_id = Convert.ToInt16(status);
                    objInsert.remark = remark;
                    objInsert.health_id = Convert.ToInt32(health_id);
                    objInsert.doctor_opinion_file = filename_opinion_doctor;
                    objInsert.recovery_plan_file = filename_recovery_plan;
                    objInsert.attachment_file = filename_process_action;
                    objInsert.assign_by_employee_id = user_id;
                    objInsert.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    if (status == "2")//close
                    {
                        objInsert.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                    }

                    var logjson = new JavaScriptSerializer().Serialize(objInsert);

                    action_log objInsertLog = new action_log();
                    objInsertLog.function_name = "createProcessActionHealth";
                    objInsertLog.file_name = "Actionevent";
                    objInsertLog.receive = logjson;
                    objInsertLog.report_id = Convert.ToInt32(health_id);
                    objInsertLog.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                    dbConnect.action_logs.InsertOnSubmit(objInsertLog);
                    dbConnect.SubmitChanges();

                
                    dbConnect.process_action_healths.InsertOnSubmit(objInsert);
                    dbConnect.SubmitChanges();

                    if(status=="1")//on process
                    {
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups = { };
                        sn.InsertHealthNotification(8, Convert.ToInt32(health_id), alert_to_groups, Session["timezone"].ToString(), "", objInsert.id);

                    }
                   
             
                    Context.Response.Output.Write(objInsert.id);

                



            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateProcessAction(string type_control,
                                        string action,
                                        string responsible_person,
                                        string due_date,
                                        string date_complete,
                                        string notify_contractor,
                                        string remark,
                                        string attachment_file,
                                        string employee_id,
                                        string contractor_id,
                                        string hazard_id,
                                        string id
                                        )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";
                bool change_employee = false;
                int system_hazard_id = 0;
                int action_id = 0;

                var query = from c in dbConnect.process_actions
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (process_action rc in query)
                {
                    rc.type_control = Convert.ToInt32(type_control);
                    rc.action = action;
                    rc.responsible_person = responsible_person;
                    rc.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());

                    if (date_complete.Trim() != "")
                    {

                        rc.date_complete = FormatDates.changeDateTimeDB(date_complete, Session["lang"].ToString());

                    }

                    rc.notify_contractor = notify_contractor;
                    // rc.root_cause_action = root_cause_action;


                    if (rc.employee_id != employee_id)//change employee
                    {
                        rc.employee_id = employee_id;
                        change_employee = true;

                    }
                   
                    if (contractor_id != "")
                    {
                        rc.contractor_id = Convert.ToInt32(contractor_id);
                    }
                    if (attachment_file != "")
                    {
                        rc.attachment_file = attachment_file;

                    }
                    rc.remark = remark;
                    system_hazard_id = rc.hazard_id;
                    action_id = rc.id;

                }

                try
                {
                    dbConnect.SubmitChanges();
                    result = "true";

                    if (change_employee == true)//change employee
                    {
 
                        //////////////////////////////////by p.poo sent notification/////////////////////////////////

                        Class.SafetyNotification sn = new Class.SafetyNotification();


                        if (Session["country"].ToString() == "srilanka")
                        {
                            string[] alert_to_groups = { };
                            sn.InsertHazardNotification(3, system_hazard_id, alert_to_groups, Session["timezone"].ToString(), "", action_id);

                        }
                        else if (Session["country"].ToString() == "thailand")
                        {

                            string[] alert_to_groups = { "GroupOH&S" };
                            sn.InsertHazardNotification(3, system_hazard_id, alert_to_groups, Session["timezone"].ToString(), "", action_id);

                        }

                        ////////////////////////////////////////end///////////////////////////////////

                    }
                }
                catch (Exception e)
                {

                }


                Context.Response.Output.Write(result);
                // return result;
            }
        }





        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateProcessActionSot( string type_control,
                                            string action,
                                            string responsible_person,
                                            string due_date,
                                            string date_complete,
                                            string notify_contractor,
                                            string remark,
                                            string attachment_file,
                                            string employee_id,
                                            string contractor_id,
                                            string sot_id,
                                            string id,
                                            string page_type
                                        )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                if (page_type == "add")
                {

                    if (Session["process_action_sot"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["process_action_sot"];


                        ProcessActionSotEntity rc = (ProcessActionSotEntity)dict[Convert.ToInt32(id)];


                        rc.TypeControl = type_control;
                        rc.Action = action;
                        rc.ResponsiblePerson = responsible_person;
                        rc.DueDate = due_date;
                        rc.DateComplete = date_complete;

                        rc.NotifyContractor = notify_contractor;
                        rc.Remark = remark;
                        rc.Action_status_id = 1;//on process
                        rc.Employee_id = employee_id;
                        rc.Contractor_id = contractor_id;

                        Session["process_action_sot"] = dict;

                        Context.Response.Output.Write("true");
                    }

                }
                else
                {
                    bool change_employee = false;
                    int system_sot_id = 0;
                    int action_id = 0;
                    var query = from c in dbConnect.process_action_sots
                                where c.id == Convert.ToInt32(id)
                                select c;

                    foreach (process_action_sot rc in query)
                    {
                        rc.type_control = Convert.ToInt32(type_control);
                        rc.action = action;
                        rc.responsible_person = responsible_person;
                        rc.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());

                        if (date_complete.Trim() != "")
                        {

                            rc.date_complete = FormatDates.changeDateTimeDB(date_complete, Session["lang"].ToString());

                        }

                        rc.notify_contractor = notify_contractor;

                        if (rc.employee_id != employee_id)//change employee
                        {
                            rc.employee_id = employee_id;
                            change_employee = true;

                        }


                        if (contractor_id != "")
                        {
                            rc.contractor_id = Convert.ToInt32(contractor_id);
                        }

                        rc.remark = remark;
                        system_sot_id = rc.sot_id;
                        action_id = rc.id;

                    }

                    try
                    {
                        dbConnect.SubmitChanges();
                        result = "true";

                        if (change_employee == true)//change employee
                        {
                          
                            ////////////////////////////////////by p.poo sent notification/////////////////////////////////

                            Class.SafetyNotification sn3 = new Class.SafetyNotification();
                            string[] alert_to_groups3 = { "AreaManager" };
                            sn3.InsertSotNotification(3, system_sot_id, alert_to_groups3, Session["timezone"].ToString(), "AreaManager", action_id);


                            //////////////////////////////////////////end///////////////////////////////////

                        }
                    }
                    catch (Exception e)
                    {

                    }


                    Context.Response.Output.Write(result);


                }


            
                // return result;
            }
        }





        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateProcessActionHealth(string type_control,
                                                string action,
                                                string responsible_person,
                                                string employee_id,
                                                string due_date,
                                                string status,
                                                string remark,
                                                string filename_opinion_doctor,
                                                string filename_recovery_plan,
                                                string filename_process_action,
                                                string health_id,
                                                string id,
                                                string page_type
                                        )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                    string result = "false";

           
                    var query = from c in dbConnect.process_action_healths
                                where c.id == Convert.ToInt32(id)
                                select c;

                    foreach (process_action_health rc in query)
                    {
                        rc.type_control_id = Convert.ToInt32(type_control);
                        rc.action = action;
                        rc.responsible_person = responsible_person;
                        rc.employee_id = employee_id;
                        rc.due_date = FormatDates.changeDateTimeDB(due_date, Session["lang"].ToString());
                        rc.action_status_id = Convert.ToInt16(status);
                        if (status == "2")//close
                        {
                            rc.date_complete = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                        }

                        rc.remark = remark;
                        rc.doctor_opinion_file = filename_opinion_doctor;
                        rc.recovery_plan_file = filename_recovery_plan;
                        rc.attachment_file = filename_process_action;


                    }

                    try
                    {
                        dbConnect.SubmitChanges();
                        result = "true";

                    }
                    catch (Exception e)
                    {

                    }


                    Context.Response.Output.Write(result);


          

                // return result;
            }
        }






       


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateRiskFactor(       string risk_factor_relate_work,
                                            string other_risk_factor,
                                            string year_factor,
                                            string duration_risk_factor,
                                            string file_risk_factor,
                                            string result_risk_factor,
                                            string user_id,
                                            string health_id,
                                            string page_type,
                                            string monitoring_environment,
                                            string monitoring_results,
                                            string id
                                        )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                if (page_type == "add")
                {

                    if (Session["risk_factor"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["risk_factor"];


                        RiskFactorEntity rc = (RiskFactorEntity)dict[Convert.ToInt32(id)];


                        rc.risk_factor_relate_work_id = Convert.ToInt16(risk_factor_relate_work);
                        rc.Other = other_risk_factor;
                        rc.Year = year_factor;
                        rc.Duration_risk_factor_id = Convert.ToInt16(duration_risk_factor);
                        rc.File_risk_factor = file_risk_factor;
                        rc.Result = result_risk_factor;
                        rc.monitoring_environment = monitoring_environment;
                        rc.monitoring_results = monitoring_results;

                        Session["risk_factor"] = dict;

                        Context.Response.Output.Write("true");
                    }

                }
                else
                {
   
                    var query = from c in dbConnect.risk_factor_relate_work_actions
                                where c.id == Convert.ToInt32(id)
                                select c;

                    foreach (risk_factor_relate_work_action rc in query)
                    {
                        rc.risk_factor_relate_work_id = Convert.ToInt16(risk_factor_relate_work);
                        rc.other = other_risk_factor;
                        rc.year= year_factor;
                        rc.duration_risk_factor_id = Convert.ToInt16(duration_risk_factor);
                        rc.file_risk_factor = file_risk_factor;
                        rc.result = result_risk_factor;
                        rc.monitoring_environment = monitoring_environment;
                        rc.monitoring_results = monitoring_results;

                    }

                    try
                    {
                        dbConnect.SubmitChanges();
                        result = "true";

                   
                    }
                    catch (Exception e)
                    {

                    }


                    Context.Response.Output.Write(result);


                }



                // return result;
            }

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void updateOccupationalHealth(
                                            string occupational_health,
                                            string repeat_health_check,
                                            string filename_result_health,
                                            string filename_repeat_result_health,
                                            string abnormal_audiogram,
                                            string hearing_threshold_level,
                                            string chronic_diseases_ear,
                                            string specify_chronic_diseases_ear,
                                            string abnormal_pulmonary_function,
                                            string smoked_cigarettes,
                                            string cigarettes_per_day,
                                            string years,
                                            string months,
                                            string smoked_other,
                                            string health_id,
                                            string id,
                                            string page_type
                                        )
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                if (page_type == "add")
                {

                    if (Session["occupational_health"] != null)
                    {
                        Dictionary<Int32, object> dict = (Dictionary<Int32, object>)Session["occupational_health"];


                        OccupationalHealthEntity rc = (OccupationalHealthEntity)dict[Convert.ToInt32(id)];


                        rc.occupational_health_report_id = Convert.ToInt16(occupational_health);
                        rc.RepeatHealthCheck = repeat_health_check;
                        rc.FileHealthCheck = filename_result_health;
                        rc.FlieRepeatHealthCheck = filename_repeat_result_health;
                        rc.abnormal_audiogram = abnormal_audiogram;
                        rc.hearing_threshold_level = hearing_threshold_level;
                        rc.chronic_diseases_ear = chronic_diseases_ear;
                        rc.specify_chronic_diseases_ear = specify_chronic_diseases_ear;


                        rc.abnormal_pulmonary_function = abnormal_pulmonary_function;
                        rc.smoked_cigarettes = smoked_cigarettes;

                        if (cigarettes_per_day != "") rc.cigarette_per_day = Convert.ToInt16(cigarettes_per_day);
                        if (years != "") rc.smoked_years = Convert.ToInt16(years);
                        if (months != "") rc.smoked_months = Convert.ToInt16(months);
                        rc.smoked_cigarettes_other = smoked_other;

                        Session["occupational_health"] = dict;

                        Context.Response.Output.Write("true");
                    }

                }
                else
                {

                    var query = from c in dbConnect.occupational_health_report_actions
                                where c.id == Convert.ToInt32(id)
                                select c;

                    foreach (occupational_health_report_action rc in query)
                    {
                        rc.occupational_health_report_id = Convert.ToInt16(occupational_health);
                        rc.repeat_health_check = repeat_health_check;
                        rc.file_health_check = filename_result_health;
                        rc.flie_repeat_health_check = filename_repeat_result_health;
                        rc.abnormal_audiogram = abnormal_audiogram;
                        rc.hearing_threshold_level = hearing_threshold_level;
                        rc.chronic_diseases_ear = chronic_diseases_ear;
                        rc.specify_chronic_diseases_ear = specify_chronic_diseases_ear;

                        rc.abnormal_pulmonary_function = abnormal_pulmonary_function;
                        rc.smoked_cigarettes = smoked_cigarettes;

                        if (cigarettes_per_day != "") rc.cigarette_per_day = Convert.ToInt16(cigarettes_per_day);
                        if (years != "") rc.smoked_years = Convert.ToInt16(years);
                        if (months != "") rc.smoked_months = Convert.ToInt16(months);
                        rc.smoked_cigarettes_other = smoked_other;

                    }

                    try
                    {
                        dbConnect.SubmitChanges();
                        result = "true";


                    }
                    catch (Exception e)
                    {

                    }


                    Context.Response.Output.Write(result);


                }



                // return result;
            }
        }









        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void checkDuedate(string duedate,string lang)
        {
            string result = "false";
            DateTime due_date = FormatDates.changeDateTimeDB(duedate, lang);
            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) > due_date.Date)
            {
                result = "true";
            }
        
               

            Context.Response.Output.Write(result);
            // return result;

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getAge(string birthdate, string lang)
        {
            string  age = "";
            DateTime birth_date = FormatDates.changeDateTimeDB(birthdate, lang);
            DateTime date_current = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
            TimeSpan diff = date_current.Subtract(birth_date);
            DateTime ages = DateTime.MinValue + diff;
            int years = (ages.Year - 1);
            int months = (ages.Month - 1);
            
            age = years + " " + Resources.Health.lbyears + " " + months + " " + Resources.Health.lbmonth;

            Context.Response.Output.Write(age);

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getServiceyear(string hiringdate, string lang)
        {
            string year = "";
            DateTime hiring_date = FormatDates.changeDateTimeDB(hiringdate, lang);
            DateTime date_current = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
            TimeSpan diff = date_current.Subtract(hiring_date);
            DateTime ages = DateTime.MinValue + diff;
            int years = (ages.Year - 1);
            int months = (ages.Month - 1);


            year = years + " " + Resources.Health.lbyears + " " + months + " " + Resources.Health.lbmonth;

            Context.Response.Output.Write(year);

        }


        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void checkDateIncidentHazard(string casedate, string lang)
        {
            string result = "false";
            DateTime case_date = FormatDates.changeDateTimeDB(casedate, lang);
            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= case_date.Date)
            {
                result = "true";
            }



            Context.Response.Output.Write(result);
            // return result;

        }

         

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createTargetMain(string tifr_employee, 
                                     string tifr_contractor_onsite, 
                                     string tifr_contractor_offsite, 
                                     string tifr_all,
                                     string ltifr_employee,
                                     string ltifr_contractor_onsite, 
                                     string ltifr_contractor_offsite,
                                     string ltifr_all,
                                     string multiplier,
                                     string multiplier_contractor_onsite,
                                     string multiplier_contractor_offsite,
                                     string multiplier_all,
                                     string function_id,
                                     string year,
                                     string lang
                                     )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {

                    string date_select = "1/01/" + FormatDates.getYear(Convert.ToInt16(year), lang) + " 00:00";

                    var v = from c in dbConnect.target_mains
                            where c.function_id == function_id &&
                            (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_select).Year)
                            select c;

                    if (v.Count() > 0)
                    {
                        foreach (var rc in v)
                        {
                            rc.tifr_employee = Convert.ToDouble(tifr_employee);
                            rc.tifr_contractor_onsite = Convert.ToDouble(tifr_contractor_onsite);
                            rc.tifr_contractor_offsite = Convert.ToDouble(tifr_contractor_offsite);
                            rc.tifr_all = Convert.ToDouble(tifr_all);
                            rc.ltifr_employee = Convert.ToDouble(ltifr_employee);
                            rc.ltifr_contractor_onsite = Convert.ToDouble(ltifr_contractor_onsite);
                            rc.ltifr_contractor_offsite = Convert.ToDouble(ltifr_contractor_offsite);
                            rc.ltifr_all = Convert.ToDouble(ltifr_all);
                            rc.multiplier = Convert.ToDouble(multiplier);
                            rc.multiplier_contractor = Convert.ToDouble(multiplier_contractor_onsite);
                            rc.multiplier_contractor_offsite = Convert.ToDouble(multiplier_contractor_offsite);
                            rc.multiplier_all = Convert.ToDouble(multiplier_all);
                            rc.function_id = function_id;
                            dbConnect.SubmitChanges();


                        }


                    }
                    else
                    {

                        target_main objInsert = new target_main();
                        objInsert.tifr_employee = Convert.ToDouble(tifr_employee);
                        objInsert.tifr_contractor_onsite = Convert.ToDouble(tifr_contractor_onsite);
                        objInsert.tifr_contractor_offsite = Convert.ToDouble(tifr_contractor_offsite);
                        objInsert.tifr_all = Convert.ToDouble(tifr_all);
                        objInsert.ltifr_employee = Convert.ToDouble(ltifr_employee);
                        objInsert.ltifr_contractor_onsite = Convert.ToDouble(ltifr_contractor_onsite);
                        objInsert.ltifr_contractor_offsite = Convert.ToDouble(ltifr_contractor_offsite);
                        objInsert.ltifr_all = Convert.ToDouble(ltifr_all);
                        objInsert.multiplier = Convert.ToDouble(multiplier);
                        objInsert.multiplier_contractor = Convert.ToDouble(multiplier_contractor_onsite);
                        objInsert.multiplier_contractor_offsite = Convert.ToDouble(multiplier_contractor_offsite);
                        objInsert.multiplier_all = Convert.ToDouble(multiplier_all);

                        objInsert.function_id = function_id;
                        objInsert.created = Convert.ToDateTime(date_select);
                        dbConnect.target_mains.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }




                    result = "true";
                }


                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Output.Write(result);

            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createTargetMainSrilanka(string tifr_employee,
                                             string tifr_contractor_onsite,
                                             string tifr_contractor_offsite,
                                             string ltifr_employee,
                                             string ltifr_contractor_onsite,
                                             string ltifr_contractor_offsite,
                                             string multiplier,
                                             string multiplier_contractor,
                                             string site_id,
                                             string year,
                                             string lang
                                             )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {

                    string date_select = "1/01/" + FormatDates.getYear(Convert.ToInt16(year), lang) + " 00:00";

                    var v = from c in dbConnect.target_main_srilankas
                            where c.site_id == site_id &&
                            (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_select).Year)
                            select c;

                    if (v.Count() > 0)
                    {
                        foreach (var rc in v)
                        {
                            rc.tifr_employee = Convert.ToDouble(tifr_employee);
                            rc.tifr_contractor_onsite = Convert.ToDouble(tifr_contractor_onsite);
                            rc.tifr_contractor_offsite = Convert.ToDouble(tifr_contractor_offsite);
                            rc.ltifr_employee = Convert.ToDouble(ltifr_employee);
                            rc.ltifr_contractor_onsite = Convert.ToDouble(ltifr_contractor_onsite);
                            rc.ltifr_contractor_offsite = Convert.ToDouble(ltifr_contractor_offsite);
                            rc.multiplier = Convert.ToDouble(multiplier);
                            rc.multiplier_contractor = Convert.ToDouble(multiplier_contractor);

                            rc.site_id = site_id;
                            dbConnect.SubmitChanges();


                        }


                    }
                    else
                    {

                        target_main_srilanka objInsert = new target_main_srilanka();
                        objInsert.tifr_employee = Convert.ToDouble(tifr_employee);
                        objInsert.tifr_contractor_onsite = Convert.ToDouble(tifr_contractor_onsite);
                        objInsert.tifr_contractor_offsite = Convert.ToDouble(tifr_contractor_offsite);
                        objInsert.ltifr_employee = Convert.ToDouble(ltifr_employee);
                        objInsert.ltifr_contractor_onsite = Convert.ToDouble(ltifr_contractor_onsite);
                        objInsert.ltifr_contractor_offsite = Convert.ToDouble(ltifr_contractor_offsite);
                        objInsert.multiplier = Convert.ToDouble(multiplier);
                        objInsert.multiplier_contractor = Convert.ToDouble(multiplier_contractor);

                        objInsert.site_id = site_id;
                        objInsert.created = Convert.ToDateTime(date_select);
                        dbConnect.target_main_srilankas.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }




                    result = "true";
                }


                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Output.Write(result);

            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createTargetSub(string tifr_employee,
                                     string tifr_contractor_onsite,
                                     string tifr_contractor_offsite,
                                     string tifr_all,
                                     string ltifr_employee,
                                     string ltifr_contractor_onsite,
                                     string ltifr_contractor_offsite,
                                     string ltifr_all,
                                     string multiplier,
                                     string multiplier_contractor_onsite,
                                     string multiplier_contractor_offsite,
                                     string multiplier_all,
                                     string function_id,
                                     string department_id,
                                     string year,
                                     string lang,
                                     string type
                                     )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {


                    string date_select = "1/01/" + FormatDates.getYear(Convert.ToInt16(year), lang) + " 00:00";

                    if (type == "function")
                    {
                        var v2 = from c in dbConnect.target_mains
                                 where c.function_id == function_id &&
                                 (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_select).Year)
                                 select c;

                        if (v2.Count() > 0)
                        {
                            foreach (var rc2 in v2)
                            {
                                rc2.tifr_employee = Convert.ToDouble(tifr_employee);
                                rc2.tifr_contractor_onsite = Convert.ToDouble(tifr_contractor_onsite);
                                rc2.tifr_contractor_offsite = Convert.ToDouble(tifr_contractor_offsite);
                                rc2.tifr_all = Convert.ToDouble(tifr_all);
                                rc2.ltifr_employee = Convert.ToDouble(ltifr_employee);
                                rc2.ltifr_contractor_onsite = Convert.ToDouble(ltifr_contractor_onsite);
                                rc2.ltifr_contractor_offsite = Convert.ToDouble(ltifr_contractor_offsite);
                                rc2.ltifr_all = Convert.ToDouble(ltifr_all);
                                rc2.multiplier = Convert.ToDouble(multiplier);
                                rc2.multiplier_contractor = Convert.ToDouble(multiplier_contractor_onsite);
                                rc2.multiplier_contractor_offsite = Convert.ToDouble(multiplier_contractor_offsite);
                                rc2.multiplier_all = Convert.ToDouble(multiplier_all);

                                rc2.function_id = function_id;
                                dbConnect.SubmitChanges();


                            }


                        }
                        else
                        {

                            target_main objInsert = new target_main();
                            objInsert.tifr_employee = Convert.ToDouble(tifr_employee);
                            objInsert.tifr_contractor_onsite = Convert.ToDouble(tifr_contractor_onsite);
                            objInsert.tifr_contractor_offsite = Convert.ToDouble(tifr_contractor_offsite);
                            objInsert.tifr_all = Convert.ToDouble(tifr_all);
                            objInsert.ltifr_employee = Convert.ToDouble(ltifr_employee);
                            objInsert.ltifr_contractor_onsite = Convert.ToDouble(ltifr_contractor_onsite);
                            objInsert.ltifr_contractor_offsite = Convert.ToDouble(ltifr_contractor_offsite);
                            objInsert.ltifr_all = Convert.ToDouble(ltifr_all);
                            objInsert.multiplier = Convert.ToDouble(multiplier);
                            objInsert.multiplier_contractor = Convert.ToDouble(multiplier_contractor_onsite);
                            objInsert.multiplier_contractor_offsite = Convert.ToDouble(multiplier_contractor_offsite);
                            objInsert.multiplier_all = Convert.ToDouble(multiplier_all);

                            objInsert.function_id = function_id;
                            objInsert.created = Convert.ToDateTime(date_select);
                            dbConnect.target_mains.InsertOnSubmit(objInsert);

                            dbConnect.SubmitChanges();
                        }


                    }
                    else
                    {
                        var v = from c in dbConnect.target_subs
                                where c.department_id == department_id &&
                                (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_select).Year)
                                select c;

                        if (v.Count() > 0)
                        {
                            foreach (var rc in v)
                            {
                                rc.tifr_employee = Convert.ToDouble(tifr_employee);
                                rc.tifr_contractor_onsite = Convert.ToDouble(tifr_contractor_onsite);
                                rc.tifr_contractor_offsite = Convert.ToDouble(tifr_contractor_offsite);
                                rc.tifr_all = Convert.ToDouble(tifr_all);
                                rc.ltifr_employee = Convert.ToDouble(ltifr_employee);
                                rc.ltifr_contractor_onsite = Convert.ToDouble(ltifr_contractor_onsite);
                                rc.ltifr_contractor_offsite = Convert.ToDouble(ltifr_contractor_offsite);
                                rc.ltifr_all = Convert.ToDouble(ltifr_all);
                                rc.multiplier = Convert.ToDouble(multiplier);
                                rc.multiplier_contractor = Convert.ToDouble(multiplier_contractor_onsite);
                                rc.multiplier_contractor_offsite = Convert.ToDouble(multiplier_contractor_offsite);
                                rc.multiplier_all = Convert.ToDouble(multiplier_all);

                                rc.department_id = department_id;
                                dbConnect.SubmitChanges();


                            }


                        }
                        else
                        {

                            target_sub objInsert = new target_sub();
                            objInsert.tifr_employee = Convert.ToDouble(tifr_employee);
                            objInsert.tifr_contractor_onsite = Convert.ToDouble(tifr_contractor_onsite);
                            objInsert.tifr_contractor_offsite = Convert.ToDouble(tifr_contractor_offsite);
                            objInsert.tifr_all = Convert.ToDouble(tifr_all);
                            objInsert.ltifr_employee = Convert.ToDouble(ltifr_employee);
                            objInsert.ltifr_contractor_onsite = Convert.ToDouble(ltifr_contractor_onsite);
                            objInsert.ltifr_contractor_offsite = Convert.ToDouble(ltifr_contractor_offsite);
                            objInsert.ltifr_all = Convert.ToDouble(ltifr_all);
                            objInsert.multiplier = Convert.ToDouble(multiplier);
                            objInsert.multiplier_contractor = Convert.ToDouble(multiplier_contractor_onsite);
                            objInsert.multiplier_contractor_offsite = Convert.ToDouble(multiplier_contractor_offsite);
                            objInsert.multiplier_all = Convert.ToDouble(multiplier_all);

                            objInsert.department_id = department_id;
                            objInsert.created = Convert.ToDateTime(date_select);
                            dbConnect.target_subs.InsertOnSubmit(objInsert);

                            dbConnect.SubmitChanges();
                        }



                    }




                    result = "true";
                }


                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Output.Write(result);

            }
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createWorkhourMain(  string employee,
                                         string contractor_onsite,
                                         string contractor_offsite,
                                         string function_id,
                                         string month,
                                         string year,
                                         string lang
                                         )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {


                    string date_select = "1/" + month + "/" + FormatDates.getYear(Convert.ToInt16(year), lang) + " 00:00";

                    var v = from c in dbConnect.workhour_mains
                            where c.function_id == function_id &&
                            (Convert.ToDateTime(c.created).Year == FormatDates.changeDateTimeDB(date_select, lang).Year) &&
                            (Convert.ToDateTime(c.created).Month == FormatDates.changeDateTimeDB(date_select, lang).Month)
                            select c;

                    if (v.Count() > 0)
                    {
                        foreach (var rc in v)
                        {

                            rc.employee = Convert.ToDouble(employee);
                            rc.contractor_onsite = Convert.ToDouble(contractor_onsite);
                            rc.contractor_offsite = Convert.ToDouble(contractor_offsite);

                            rc.function_id = function_id;
                            dbConnect.SubmitChanges();


                        }


                    }
                    else
                    {

                        workhour_main objInsert = new workhour_main();
                        objInsert.employee = Convert.ToDouble(employee);
                        objInsert.contractor_onsite = Convert.ToDouble(contractor_onsite);
                        objInsert.contractor_offsite = Convert.ToDouble(contractor_offsite);

                        objInsert.function_id = function_id;
                        objInsert.created = FormatDates.changeDateTimeDB(date_select, lang);
                        dbConnect.workhour_mains.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }




                    result = "true";


                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Output.Write(result);
            }

        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createWorkhourMainSrilanka(  string employee,
                                                 string contractor_onsite,
                                                 string contractor_offsite,
                                                 string site_id,
                                                 string month,
                                                 string year,
                                                 string lang
                                                 )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {


                    string date_select = "1/" + month + "/" + FormatDates.getYear(Convert.ToInt16(year), lang) + " 00:00";

                    var v = from c in dbConnect.workhour_main_srilankas
                            where c.site_id == site_id &&
                            (Convert.ToDateTime(c.created).Year == FormatDates.changeDateTimeDB(date_select, lang).Year) &&
                            (Convert.ToDateTime(c.created).Month == FormatDates.changeDateTimeDB(date_select, lang).Month)
                            select c;

                    if (v.Count() > 0)
                    {
                        foreach (var rc in v)
                        {

                            rc.employee = Convert.ToDouble(employee);
                            rc.contractor_onsite = Convert.ToDouble(contractor_onsite);
                            rc.contractor_offsite = Convert.ToDouble(contractor_offsite);

                            rc.site_id = site_id;
                            dbConnect.SubmitChanges();


                        }


                    }
                    else
                    {

                        workhour_main_srilanka objInsert = new workhour_main_srilanka();
                        objInsert.employee = Convert.ToDouble(employee);
                        objInsert.contractor_onsite = Convert.ToDouble(contractor_onsite);
                        objInsert.contractor_offsite = Convert.ToDouble(contractor_offsite);

                        objInsert.site_id = site_id;
                        objInsert.created = FormatDates.changeDateTimeDB(date_select, lang);
                        dbConnect.workhour_main_srilankas.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }




                    result = "true";


                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Output.Write(result);
            }

        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public void createWorkhourSub(string employee,
                                         string contractor_onsite,
                                         string contractor_offsite,
                                         string training_hour,
                                         string function_id,
                                         string division_id,
                                         string month,
                                         string year,
                                         string lang
                                         )
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "false";

                try
                {


                    string date_select = "1/" + month + "/" + FormatDates.getYear(Convert.ToInt16(year), lang) + " 00:00";
                   


                    var v = from c in dbConnect.workhour_subs
                            where c.division_id == division_id &&
                            (Convert.ToDateTime(c.created).Year == FormatDates.changeDateTimeDB(date_select, lang).Year) &&
                            (Convert.ToDateTime(c.created).Month == FormatDates.changeDateTimeDB(date_select, lang).Month)
                            select c;

                    if (v.Count() > 0)
                    {
                        foreach (var rc in v)
                        {
                            rc.employee = Convert.ToDouble(employee);
                            rc.contractor_onsite = Convert.ToDouble(contractor_onsite);
                            rc.contractor_offsite = Convert.ToDouble(contractor_offsite);
                            rc.training_hour = Convert.ToDouble(training_hour);

                            rc.division_id = division_id;
                            dbConnect.SubmitChanges();


                        }


                    }
                    else
                    {

                        workhour_sub objInsert = new workhour_sub();
                        objInsert.employee = Convert.ToDouble(employee);
                        objInsert.contractor_onsite = Convert.ToDouble(contractor_onsite);
                        objInsert.contractor_offsite = Convert.ToDouble(contractor_offsite);
                        objInsert.training_hour = Convert.ToDouble(training_hour);

                        objInsert.division_id = division_id;
                        objInsert.created = FormatDates.changeDateTimeDB(date_select, lang);
                        dbConnect.workhour_subs.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }


                    //////////////////////////////////sum all set to workhour main////////////////////////////////


                    if (!string.IsNullOrEmpty(function_id))
                    {
                        var g = from c in dbConnect.workhour_subs
                                join di in dbConnect.divisions on c.division_id equals di.division_id
                                join de in dbConnect.departments on di.department_id equals de.department_id
                                where de.function_id == function_id &&
                                (Convert.ToDateTime(c.created).Year == FormatDates.changeDateTimeDB(date_select, lang).Year) &&
                                (Convert.ToDateTime(c.created).Month == FormatDates.changeDateTimeDB(date_select, lang).Month)
                                select new
                                {
                                    de.function_id,
                                    c.employee,
                                    c.contractor_onsite,
                                    c.contractor_offsite,
                                    c.training_hour

                                };


                        double all_employee = 0;
                        double all_contractor_onsite = 0;
                        double all_contractor_offsite = 0;
                        double all_training_hour = 0;
                        foreach (var rc in g)
                        {
                            all_employee = all_employee + Convert.ToDouble(rc.employee);
                            all_contractor_onsite = all_contractor_onsite + Convert.ToDouble(rc.contractor_onsite);
                            all_contractor_offsite = all_contractor_offsite + Convert.ToDouble(rc.contractor_offsite);
                            all_training_hour = all_training_hour + Convert.ToDouble(rc.training_hour);


                        }



                        var d = from c in dbConnect.workhour_mains
                                where c.function_id == function_id &&
                                (Convert.ToDateTime(c.created).Year == FormatDates.changeDateTimeDB(date_select, lang).Year) &&
                                (Convert.ToDateTime(c.created).Month == FormatDates.changeDateTimeDB(date_select, lang).Month)
                                select c;


                        if (d.Count() > 0)
                        {
                            foreach (var r in d)
                            {
                                r.employee = Math.Round(all_employee, 2);
                                r.contractor_onsite = Math.Round(all_contractor_onsite, 2);
                                r.contractor_offsite = Math.Round(all_contractor_offsite, 2);
                                r.training_hour = Math.Round(all_training_hour, 2);
                                dbConnect.SubmitChanges();

                            }


                        }
                        else
                        {

                            workhour_main objInsert1 = new workhour_main();
                            objInsert1.employee = Math.Round(all_employee, 2);
                            objInsert1.contractor_onsite = Math.Round(all_contractor_onsite, 2);
                            objInsert1.contractor_offsite = Math.Round(all_contractor_offsite, 2);
                            objInsert1.training_hour = Math.Round(all_training_hour,2);

                            objInsert1.function_id = function_id;
                            objInsert1.created = FormatDates.changeDateTimeDB(date_select, lang);
                            dbConnect.workhour_mains.InsertOnSubmit(objInsert1);

                            dbConnect.SubmitChanges();



                        }


                    }

                    ///////////////////////////////////////////////////////////////////////////////////////////

                    result = "true";






                }
                catch (Exception e)
                {

                    result = e.Message;
                    // dbConnect.SubmitChanges();
                }


                Context.Response.Output.Write(result);
            }

        }






        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getImageSot(string id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                ArrayList ls = new ArrayList();

                string country = Session["country"].ToString();

                var q = from c in dbConnect.sots
                        where c.id == Convert.ToInt32(id)
                        select new
                        {
                            user_id = c.employee_id,
                            report_date = c.report_date

                        };

                foreach (var s in q)
                {
                    string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathfile"];
                    string name_folder = s.user_id + "_" + s.report_date.Value.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB"));
                    //string pathfolder = string.Format("{0}\\upload\\hazard\\" + name_folder, Server.MapPath(@"\"));
                    string pathfolder = string.Format("{0}" + pathupload + "sot\\" + country+"\\" + name_folder, Server.MapPath(@"\"));





                    string[] images = Directory.GetFiles(pathfolder, "*")
                                             .Select(Path.GetFileName).OrderByDescending(Path.GetFileName).Take(2)
                                             .ToArray();

                    // FileInfo[] files = dir.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();

                    foreach (var d in images)
                    {
                        string pathfolder_new = pathfolder + "\\" + d;
                        FileInfo f = new FileInfo(pathfolder_new);
                        string size = f.Length.ToString();
                        var v = new Dictionary<string, string>
                       {
                           { "path", "upload/sot/"+country+"/"+name_folder+"/"+d },
                           { "folder", name_folder},
                           { "name", d },
                           { "size", size },
                
                       };

                        ls.Add(v);

                    }


                }



                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(ls));
            }
        }



    }
}
