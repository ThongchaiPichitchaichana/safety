using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class mobilelogin : System.Web.UI.Page
    {
        private const int EMPLOYEE = 12;
        private const int AD = 13;
        private const int CONTRACTOR = 14; 
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Context.Request["username"] != null && Context.Request["password"] != null && Context.Request["type"] != null)
            {
                string type = Context.Request["type"];
                if(type=="notify")
                {
                    notifyLogin(Context.Request["password"], Context.Request["username"]);

                }else if(type=="ad"){

                    authenticateLogin(Context.Request["username"], Context.Request["password"]);
                }
                else if (type == "contractor")
                {
                    contractorLogin(Context.Request["username"]);

                }
            }
            
        }



        protected void notifyLogin(string first_name,string employee_id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string mngt_level = "";
                string user_id = "";
                string company_id = "";
                string function_id = "";
                string department_id = "";
                string division_id = "";
                string section_id = "";
                string company_name_th = "";
                string function_name_th = "";
                string department_name_th = "";
                string division_name_th = "";
                string section_name_th = "";
                string company_name_en = "";
                string function_name_en = "";
                string department_name_en = "";
                string division_name_en = "";
                string section_name_en = "";
                string name_th = "";
                string name_en = "";
                string name_si = "";
                string phone = "";
                string result = "";
                string error = "";
                int group_id = 0;
                int group_v = 0;
                string country = "";
                string timezone = "";


                ArrayList permission = new ArrayList();

                if (first_name != "" && employee_id != "")
                {

                    var v = from c in dbConnect.employees
                            where c.status == "Active" && (c.employee_id == employee_id && c.first_name_en == first_name.Trim()) || (c.employee_id == employee_id.Trim() && c.first_name_th == first_name.Trim())
                            select new
                            {
                                name_en = c.first_name_en + " " + c.last_name_en,
                                name_th = c.first_name_th + " " + c.last_name_th,
                                name_si = c.first_name_en + " " + c.last_name_en,
                                user_id = c.employee_id,
                                org_id = c.unit_id,
                                mngt_level = c.mngt_level,
                                c.country,
                                c.timezone


                            };
                    bool checkData = false;


                    foreach (var rc in v)
                    {
                        checkData = true;
                        name_th = rc.name_th;
                        name_en = rc.name_en;
                        name_si = rc.name_si;
                        phone = "";
                        user_id = rc.user_id;
                        mngt_level = rc.mngt_level;
                        country = rc.country;
                        timezone = rc.timezone;

                        var o = from c in dbConnect.organizations
                                where c.org_unit_id == rc.org_id
                                select new
                                {
                                    c.function,
                                    c.function_id,
                                    c.company_id,
                                    c.division_id,
                                    c.department_id,
                                    c.sub_function_id,
                                    c.section_id,

                                };

                        foreach (var ro in o)
                        {
                            company_id = ro.company_id;
                            function_id = ro.function_id;
                           

                            if (rc.country == "thailand")
                            {
                                department_id = ro.department_id;

                                if (division_id != "00000000" && division_id != "")
                                {
                                    division_id = ro.division_id;
                                }
                                else
                                {
                                    division_id = department_id +"D";
                                }

                                if (section_id != "00000000" && section_id != "")
                                {
                                    section_id = ro.section_id;
                                }
                                else
                                {
                                    section_id = division_id + "D";
                                }
                            }
                            else if (rc.country == "srilanka")
                            {
                                department_id = ro.sub_function_id;

                                if (division_id != "00000000" && division_id != "")
                                {
                                    division_id = ro.department_id;
                                }

                                if (section_id != "00000000" && section_id != "")
                                {
                                    section_id = ro.division_id;
                                }
                            }

                           

                        }


                    }




                    var co = from c in dbConnect.companies
                             where c.company_id == company_id
                             select new
                             {
                                 company_name_th = c.company_th,
                                 company_name_en = c.company_en

                             };

                    foreach (var r in co)
                    {
                        company_name_th = r.company_name_th;
                        company_name_en = r.company_name_en;
                    }


                    var fu = from c in dbConnect.functions
                             where c.function_id == function_id
                             select new
                             {
                                 function_name_th = c.function_th,
                                 function_name_en = c.function_en

                             };

                    foreach (var rf in fu)
                    {
                        function_name_th = rf.function_name_th;
                        function_name_en = rf.function_name_en;
                    }


                    var de = from c in dbConnect.departments
                             where c.department_id == department_id
                             select new
                             {
                                 department_name_th = c.department_th,
                                 department_name_en = c.department_en

                             };

                    foreach (var d in de)
                    {
                        department_name_th = d.department_name_th;
                        department_name_en = d.department_name_en;
                    }


                    var di = from c in dbConnect.divisions
                             where c.division_id == division_id
                             select new
                             {
                                 division_name_th = c.division_th,
                                 division_name_en = c.division_en

                             };

                    foreach (var d in di)
                    {
                        division_name_th = d.division_name_th;
                        division_name_en = d.division_name_en;
                    }


                    var se = from c in dbConnect.sections
                             where c.section_id == section_id
                             select new
                             {
                                 section_name_th = c.section_th,
                                 section_name_en = c.section_en

                             };

                    foreach (var s in se)
                    {
                        section_name_th = s.section_name_th;
                        section_name_en = s.section_name_en;
                    }


                    if (checkData)//login true
                    {

                        if (country == "srilanka")
                        {
                            result = "false";
                            error = "You are not allowed to login with Employee ID, Please use Computer ID tab";
                        }
                        else if (country == "thailand")
                        {
                            permission = getPermission(EMPLOYEE, employee_id, mngt_level, out group_id, out group_v, country);
                            result = "true";
                            createLogLogin(employee_id, 0, "employee", timezone);
                        }
                    

                    }
                    else//รหัสอะไรไม่ถูก
                    {
                        result = "false";
                        error = "user or password incorrect";
                    }

                }//เป็นค่าว่างมา
                else
                {
                    result = "false";
                    error = "user or password require";
                }

                var return_value = new
                {
                    result = result,
                    error = error,
                    user_id = user_id,
                    company_id = company_id,
                    function_id = function_id,
                    department_id = department_id,
                    division_id = division_id,
                    section_id = section_id,
                    company_name_th = company_name_th,
                    function_name_th = function_name_th,
                    department_name_th = department_name_th,
                    division_name_th = division_name_th,
                    section_name_th = section_name_th,
                    company_name_en = company_name_en,
                    function_name_en = function_name_en,
                    department_name_en = department_name_en,
                    division_name_en = division_name_en,
                    section_name_en = section_name_en,
                    company_name_si = company_name_en,
                    function_name_si = function_name_en,
                    department_name_si = department_name_en,
                    division_name_si = division_name_en,
                    section_name_si = section_name_en,
                    name_th = name_th,
                    name_en = name_en,
                    phone = phone,
                    permission = permission,
                    group_id = group_id,
                    group_value = group_v,
                    country = country,
                    timezone = timezone

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));

            }   


        }



        protected void contractorLogin(string email)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string mngt_level = "";
                string user_id = "";
                string company_id = "";
                string function_id = "";
                string department_id = "";
                string division_id = "";
                string section_id = "";
                string company_name_th = "";
                string function_name_th = "";
                string department_name_th = "";
                string division_name_th = "";
                string section_name_th = "";
                string company_name_en = "";
                string function_name_en = "";
                string department_name_en = "";
                string division_name_en = "";
                string section_name_en = "";
                string name_th = "";
                string name_en = "";
                string name_si = "";
                string phone = "";
                string result = "";
                string error = "";
                int group_id = 0;
                int group_v = 0;
                string country = "";
                string timezone = "";

                ArrayList permission = new ArrayList();

                if (email != "")
                {

                    var v = from c in dbConnect.contractors
                            where c.email == email.Trim() && c.status == "valid"
                            select new
                            {
                                name_en = c.first_name_en + " " + c.last_name_en,
                                name_th = c.first_name_th + " " + c.last_name_th,
                                user_id = c.id,
                                phone = c.phone,
                                c.country,
                                c.timezone

                            };
                    bool checkData = false;


                    foreach (var rc in v)
                    {
                        checkData = true;
                        name_th = rc.name_th;
                        name_en = rc.name_en;
                        name_si = rc.name_en;
                        phone = rc.phone;
                        user_id = rc.user_id.ToString();
                        country = rc.country;
                        timezone = rc.timezone;


                    }

                    if (checkData)//login true
                    {
                        permission = getPermission(CONTRACTOR, "", mngt_level, out group_id, out group_v,country);
                        result = "true";
                        createLogLogin("", Convert.ToInt32(user_id), "contractor",timezone);

                    }
                    else//รหัสอะไรไม่ถูก
                    {
                        result = "false";
                        error = "user or password incorrect";
                    }

                }//เป็นค่าว่างมา
                else
                {
                    result = "false";
                    error = "user or password require";
                }

                var return_value = new
                {
                    result = result,
                    error = error,
                    user_id = user_id,
                    company_id = company_id != null ? company_id : "",
                    function_id = function_id != null ? function_id : "",
                    department_id = department_id != null ? department_id : "",
                    division_id = division_id != null ? division_id : "",
                    section_id = section_id != null ? section_id : "",
                    company_name_th = company_name_th,
                    function_name_th = function_name_th,
                    department_name_th = department_name_th,
                    division_name_th = division_name_th,
                    section_name_th = section_name_th,
                    company_name_en = company_name_en,
                    function_name_en = function_name_en,
                    department_name_en = department_name_en,
                    division_name_en = division_name_en,
                    section_name_en = section_name_en,
                    company_name_si = company_name_en,
                    function_name_si = function_name_en,
                    department_name_si = department_name_en,
                    division_name_si = division_name_en,
                    section_name_si = section_name_en,
                    name_th = name_th != null ? name_th : "",
                    name_en = name_en != null ? name_en : "",
                    name_si = name_en != null ? name_en : "",
                    phone = phone != null ? phone : "",
                    permission = permission,
                    group_id = group_id,
                    group_value = group_v,
                    country = country,
                    timezone = timezone

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));

            }


        }



        protected void authenticateLogin(string user_name, string password)
        {

            string mngt_level = "";
            string user_id = "";
            string company_id = "";
            string function_id = "";
            string department_id = "";
            string division_id = "";
            string section_id = "";
            string company_name_th = "";
            string function_name_th = "";
            string department_name_th = "";
            string division_name_th = "";
            string section_name_th = "";
            string company_name_en = "";
            string function_name_en = "";
            string department_name_en = "";
            string division_name_en = "";
            string section_name_en = "";
            string name_th = "";
            string name_en = "";
            string name_si = "";
            string phone = "";
            string result = "";
            string error = "";
            int group_id = 0;
            int group_v = 0;
            string country = "";
            string timezone = "";
            bool result_authen = false;
            ArrayList permission = new ArrayList();

            if (user_name != "" && password != "")
            {
                string dominName = string.Empty;
                string adPath = string.Empty;
                string userName = user_name.Trim().ToUpper();
                string strError = string.Empty;

                foreach (string key in ConfigurationSettings.AppSettings.Keys)
                {
                    //Response.Write(ConfigurationSettings.AppSettings[key]);

                    dominName = key.Contains("DirectoryDomain") ? ConfigurationSettings.AppSettings[key] : dominName;
                    adPath = key.Contains("DirectoryPath") ? ConfigurationSettings.AppSettings[key] : adPath;
                    if (!String.IsNullOrEmpty(dominName) && !String.IsNullOrEmpty(adPath))
                    {
                        result_authen = AuthenticateUser(dominName, userName, password, adPath, out strError);
                        if (result_authen==true)
                        {
                            CheckEmp(userName);
                        }
                        dominName = string.Empty;
                        adPath = string.Empty;
                        if (String.IsNullOrEmpty(strError)) break;
                    }

                }
                if (!string.IsNullOrEmpty(strError))
                {
                    result = "false";
                    error = strError;
                }

            }



            if(result_authen==false)
            {
                var return_value = new
                {
                    result = result,
                    error = error,
                    user_id = user_id,
                    company_id = company_id != null ? company_id : "",
                    function_id = function_id != null ? function_id : "",
                    department_id = department_id != null ? department_id : "",
                    division_id = division_id != null ? division_id : "",
                    section_id = section_id != null ? section_id : "",
                    name_th = name_th != null ? name_th : "",
                    name_en = name_en != null ? name_en : "",
                    name_si = name_en != null ? name_en : "",
                    company_name_th = company_name_th,
                    function_name_th = function_name_th,
                    department_name_th = department_name_th,
                    division_name_th = division_name_th,
                    section_name_th = section_name_th,
                    company_name_en = company_name_en,
                    function_name_en = function_name_en,
                    department_name_en = department_name_en,
                    division_name_en = division_name_en,
                    section_name_en = section_name_en,
                    company_name_si = company_name_en,
                    function_name_si = function_name_en,
                    department_name_si = department_name_en,
                    division_name_si = division_name_en,
                    section_name_si = section_name_en,
                    phone = phone != null ? phone : "",
                    permission = permission,
                    group_id = group_id,
                    group_value = group_v,
                    country = country,
                    timezone = timezone

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));



            }

          


        }


        public bool AuthenticateUser(string domain, string username, string password, string LdapPath, out string Errmsg)
        {
            Errmsg = "";
            string domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(LdapPath, domainAndUsername, password);
            try
            {
                // Bind to the native AdsObject to force authentication.
                Object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if (null == result)
                {
                    return false;
                }

                LdapPath = result.Path;
                string _filterAttribute = (String)result.Properties["cn"][0];
                Errmsg = _filterAttribute;
                Errmsg = LdapPath;
            }
            catch (Exception ex)
            {
                Errmsg = ex.Message; 
                return false;
                throw new Exception("Error authenticating user." + ex.Message);
            }
            return true;
        }

        public void CheckEmp(string adcn)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string mngt_level = "";
                string user_id = "";
                string company_id = "";
                string function_id = "";
                string department_id = "";
                string division_id = "";
                string section_id = "";
                string company_name_th = "";
                string function_name_th = "";
                string department_name_th = "";
                string division_name_th = "";
                string section_name_th = "";
                string company_name_en = "";
                string function_name_en = "";
                string department_name_en = "";
                string division_name_en = "";
                string section_name_en = "";
                string name_th = "";
                string name_en = "";
                string name_si = "";
                string phone = "";
                string result = "";
                string error = "";
                int group_id = 0;
                int group_v = 0;
                string country = "";
                string timezone = "";

                ArrayList permission = new ArrayList();

                if (adcn != "")
                {

                    var v = from c in dbConnect.employees
                            where c.status == "Active" && c.domain_user == adcn
                            select new
                            {
                                name_en = c.first_name_en + " " + c.last_name_en,
                                name_th = c.first_name_th + " " + c.last_name_th,
                                name_si = c.first_name_en + " " + c.last_name_en,
                                user_id = c.employee_id,
                                org_id = c.unit_id,
                                mngt_level = c.mngt_level,
                                c.country,
                                c.timezone

                            };
                    bool checkData = false;


                    foreach (var rc in v)
                    {
                        checkData = true;
                        name_th = rc.name_th;
                        name_en = rc.name_en;
                        phone = "";
                        user_id = rc.user_id;
                        mngt_level = rc.mngt_level;
                        country = rc.country;
                        timezone = rc.timezone;

                        var o = from c in dbConnect.organizations
                                where c.org_unit_id == rc.org_id
                                select new
                                {
                                    c.function,
                                    c.function_id,
                                    c.company_id,
                                    c.division_id,
                                    c.department_id,
                                    c.sub_function_id,
                                    c.section_id

                                };

                        foreach (var ro in o)
                        {
                            company_id = ro.company_id;
                            function_id = ro.function_id;
                            if (rc.country == "thailand")
                            {
                                department_id = ro.department_id;

                                if (division_id != "00000000" && division_id != "")
                                {
                                    division_id = ro.division_id;
                                }
                                else
                                {
                                    division_id = department_id + "D";
                                }

                                if (section_id != "00000000" && section_id != "")
                                {
                                    section_id = ro.section_id;
                                }
                                else
                                {
                                    section_id = division_id + "D";
                                }
                            }
                            else if (rc.country == "srilanka")
                            {
                                department_id = ro.sub_function_id;

                                if (division_id != "00000000" && division_id != "")
                                {
                                    division_id = ro.department_id;
                                }

                                if (section_id != "00000000" && section_id != "")
                                {
                                    section_id = ro.division_id;
                                }
                            }

                           

                        }


                    }



                    var co = from c in dbConnect.companies
                             where c.company_id == company_id
                             select new
                             {
                                 company_name_th = c.company_th,
                                 company_name_en = c.company_en

                             };

                    foreach (var r in co)
                    {
                        company_name_th = r.company_name_th;
                        company_name_en = r.company_name_en;
                    }


                    var fu = from c in dbConnect.functions
                             where c.function_id == function_id
                             select new
                             {
                                 function_name_th = c.function_th,
                                 function_name_en = c.function_en

                             };

                    foreach (var rf in fu)
                    {
                        function_name_th = rf.function_name_th;
                        function_name_en = rf.function_name_en;
                    }


                    var de = from c in dbConnect.departments
                             where c.department_id == department_id
                             select new
                             {
                                 department_name_th = c.department_th,
                                 department_name_en = c.department_en

                             };

                    foreach (var d in de)
                    {
                        department_name_th = d.department_name_th;
                        department_name_en = d.department_name_en;
                    }


                    var di = from c in dbConnect.divisions
                             where c.division_id == division_id
                             select new
                             {
                                 division_name_th = c.division_th,
                                 division_name_en = c.division_en

                             };

                    foreach (var d in di)
                    {
                        division_name_th = d.division_name_th;
                        division_name_en = d.division_name_en;
                    }


                    var se = from c in dbConnect.sections
                             where c.section_id == section_id
                             select new
                             {
                                 section_name_th = c.section_th,
                                 section_name_en = c.section_en

                             };

                    foreach (var s in se)
                    {
                        section_name_th = s.section_name_th;
                        section_name_en = s.section_name_en;
                    }




                    if (checkData)
                    {
                        permission = getPermission(AD, user_id, mngt_level, out group_id, out group_v,country);
                        result = "true";
                        createLogLogin(user_id, 0, "AD",timezone);

                    }
                    else
                    {
                        result = "false";
                        error = "user or password incorrect";
                    }

                }
                else
                {

                    result = "false";
                    error = "user or password require";
                }


                var return_value = new
                {
                    result = result,
                    error = error,
                    user_id = user_id,
                    company_id = company_id != null ? company_id : "",
                    function_id = function_id != null ? function_id : "",
                    department_id = department_id != null ? department_id : "",
                    division_id = division_id != null ? division_id : "",
                    section_id = section_id != null ? section_id : "",
                    company_name_th = company_name_th,
                    function_name_th = function_name_th,
                    department_name_th = department_name_th,
                    division_name_th = division_name_th,
                    section_name_th = section_name_th,
                    company_name_en = company_name_en,
                    function_name_en = function_name_en,
                    department_name_en = department_name_en,
                    division_name_en = division_name_en,
                    section_name_en = section_name_en,
                    company_name_si = company_name_en,
                    function_name_si = function_name_en,
                    department_name_si = department_name_en,
                    division_name_si = division_name_en,
                    section_name_si = section_name_en,
                    name_th = name_th != null ? name_th : "",
                    name_en = name_en != null ? name_en : "",
                    name_si = name_en != null ? name_en : "",
                    phone = phone != null ? phone : "",
                    permission = permission,
                    group_id = group_id,
                    group_value = group_v,
                    country = country,
                    timezone = timezone

                };

                ArrayList dt = new ArrayList();
                dt.Add(return_value);


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Write(js.Serialize(dt));
            }


        }







        protected ArrayList getPermission(int group_login, string employee_id, string mngt_level, out int group_id,out int group_v,string country)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
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
                            where c.id == 15 && m.country == country
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
                             where c.id == 17 && m.country == country
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
                            where c.employee_id == employee_id && g.country == country
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
                             where c.employee_id == employee_id
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
                                  where c.group_id == 9 && c.country == country//area oh&s
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
                              where c.employee_id == employee_id
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
                                  where c.group_id == 10 && c.country == country//area manager
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
                                  where c.group_id == 11 && c.country == country//area supervisor
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
                group_id = getGroupID(min_group_value);
                group_v = min_group_value;



                return per;
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


        protected void createLogLogin(string employee_id, int contractor_id, string type_login,string timezone)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                login_log objInsert = new login_log();

                if (employee_id != "")
                {
                    objInsert.employee_id = employee_id;
                }
                else
                {
                    objInsert.contractor_id = contractor_id;
                }

                objInsert.browser = "";
                objInsert.ip_address = "";
                objInsert.type_login = type_login;
                objInsert.type_device = "mobile";
                objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                dbConnect.login_logs.InsertOnSubmit(objInsert);

                dbConnect.SubmitChanges();
            }
        }


    }
}