using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.DirectoryServices;
using System.Collections;
using System.Net;

namespace safetys4
{
    public partial class login : System.Web.UI.Page
    {

        private const int EMPLOYEE = 12;
        private const int AD = 13;
        private const int CONTRACTOR = 14; 
        protected void Page_Load(object sender, EventArgs e)
        {
         
            if (Session["user_id"] != null && Session["lang"] != null)
            {

                Response.Redirect("MainMenu.aspx");

            }
            else
            {


                if (!IsPostBack)
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies["lang"];

                    if (cookie != null && cookie.Value != null)
                    {
                        setLanguage(cookie.Value);
                    }
                    else
                    {
                        setLanguage("en");
                    }


                }

            }






        }

        protected void setLanguage(string lang)
        {
            if (lang == "th")
            {
                Session["typeLogin"] = "employee";
                Session["lang"] = "th";
                Session["langShow"] = "<img src='template/img/language/th.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ไทย";
                lbLanguageShow.Text = Session["langShow"].ToString();

            }
            else if (lang == "en")
            {
                Session["typeLogin"] = "employee";
                Session["lang"] = "en";
                Session["langShow"] = "<img src='template/img/language/gb.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English";
                lbLanguageShow.Text = Session["langShow"].ToString();

            }
            else if (lang == "si")
            {
                Session["typeLogin"] = "employee";
                Session["lang"] = "si";
                Session["langShow"] = "<img src='template/img/language/si.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English";
                lbLanguageShow.Text = Session["langShow"].ToString();

            }

        }

        protected void LinkLanguageTH_Click(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("lang");
            cookie.Value = "th";
            Response.Cookies.Add(cookie);

            Session["langShow"] = "<img src='template/img/language/th.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ไทย";
            Session["lang"] = "th";

            Response.Redirect(Request.RawUrl);

        }

        protected void LinkLanguageEN_Click(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("lang");
            cookie.Value = "en";
            Response.Cookies.Add(cookie);
            Session["langShow"] = "<img src='template/img/language/gb.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English";
            Session["lang"] = "en";

            Response.Redirect(Request.RawUrl);

        }


        protected void LinkLanguageSI_Click(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("lang");
            cookie.Value = "si";
            Response.Cookies.Add(cookie);
            Session["langShow"] = "<img src='template/img/language/si.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English";
            Session["lang"] = "si";

            Response.Redirect(Request.RawUrl);

        }

        protected void btLoginNotify_Click(object sender, EventArgs e)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                Session["typeLogin"] = "employee";

                if (txtFirstnameNotify.Text != "" && txtUserNotify.Text != "")
                {

                    var v = from c in dbConnect.employees
                            where c.status == "Active" && (c.employee_id == txtUserNotify.Text.Trim() && c.first_name_en == txtFirstnameNotify.Text.Trim()) || (c.employee_id == txtUserNotify.Text.Trim() && c.first_name_th == txtFirstnameNotify.Text.Trim())
                            select new
                            {
                                name_en = c.first_name_en + " " + c.last_name_en,
                                name_th = c.first_name_th + " " + c.last_name_th,
                                user_id = c.employee_id,
                                org_id = c.unit_id,
                                mngt_level = c.mngt_level,
                                c.country,
                                c.timezone


                            };
                    bool checkData = false;
                    string employee_id = "";
                    string mngt_level = "";
                    foreach (var rc in v)
                    {
                        checkData = true;
                        Session["user_id"] = rc.user_id;
                        Session["phone"] = "";
                        Session["country"] = rc.country;
                        Session["timezone"] = rc.timezone;
                        getNameShowLogin(rc.name_en, rc.name_th);
                        employee_id = rc.user_id;
                        mngt_level = rc.mngt_level;
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
                            Session["function_id"] = ro.function_id;
                            Session["function"] = ro.function;
                            Session["company_id"] = ro.company_id;

                            if (rc.country == "thailand")
                            {
                                Session["department_id"] = ro.department_id;
                                Session["division_id"] = ro.division_id;
                                Session["section_id"] = ro.section_id;
                            }
                            else if (rc.country == "srilanka")
                            {
                                Session["department_id"] = ro.sub_function_id;
                                Session["division_id"] = ro.department_id;
                                Session["section_id"] = ro.division_id;
                              
                            }
                            
                            

                        }


                    }

                    if (checkData)
                    {

                        if (Session["country"].ToString() == "thailand")
                        {
                            getPermission(EMPLOYEE, employee_id, mngt_level);
                            Session["system_admin"] = checkSystemAdmin(employee_id);


                            if (Request.QueryString.Count > 0)
                            {
                                string return_url = Request.QueryString["returnUrl"];
                                Response.Redirect(return_url);

                            }
                            else
                            {
                                createLogLogin(employee_id, 0, "employee");
                                Response.Redirect("MainMenu.aspx");

                            }
                        }
                        else if (Session["country"].ToString() == "srilanka")
                        {
                            // Session.Clear();
                            Session["user_id"] = null;
                            txtUserNotify.Enabled = false;
                            txtFirstnameNotify.Enabled = false;
                            errorNotify.Text = Resources.Main.force_login_ad;

                        }
                        
                    }
                    else
                    {
                        errorNotify.Text = Resources.Main.errorNotify;
                    }

                }
                else
                {
                    errorNotify.Text = Resources.Main.errorNotify;

                }

            }

        }

        protected void btLoginContractor_Click(object sender, EventArgs e)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                Session["typeLogin"] = "contractor";

                if (txtEmailRegister.Text != "")
                {


                    var v = from c in dbConnect.contractors
                            where c.email == txtEmailRegister.Text.Trim() && c.status == "valid"
                            select new
                            {
                                name_en = c.first_name_en + " " + c.last_name_en,
                                name_th = c.first_name_th + " " + c.last_name_th,
                                user_id = c.id,
                                c.function_id,
                                phone = c.phone,
                                c.country,
                                c.timezone

                            };

                    bool checkData = false;
                    string employee_id = "";
                    string mngt_level = "";
                    int contractor_id = 0;
                    foreach (var rc in v)
                    {
                        checkData = true;
                        contractor_id = rc.user_id;
                        Session["user_id"] = rc.user_id;
                        Session["phone"] = rc.phone;
                        Session["country"] = rc.country;
                        Session["timezone"] = rc.timezone;
                        Session["function_id"] = rc.function_id;

                        var co = from c in dbConnect.functions
                                 where c.function_id == rc.function_id
                                 select new
                                 {
                                     c.company_id
                                 };

                        foreach (var r in co)
                        {
                            Session["company_id"] = r.company_id;
                        }
                        getNameShowLogin(rc.name_en, rc.name_th);


                    }
                   
                    Session["function"] = "";                 
                    Session["department_id"] = "";
                    Session["division_id"] = "";
                    Session["section_id"] = "";


                    if (checkData)
                    {
                        getPermission(CONTRACTOR, employee_id, mngt_level);
                        Session["system_admin"] = checkSystemAdmin(employee_id);

                        if (Session["country"].ToString() == "srilanka")
                        {
                            //////////////////////default language si/////////////////////////////////////////
                            HttpCookie cookie = new HttpCookie("lang");
                            cookie.Value = "si";
                            Response.Cookies.Add(cookie);
                            Session["langShow"] = "<img src='template/img/language/si.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English";
                            Session["lang"] = "si";
                            /////////////////////////////////////////////////////////////////////////////////

                        }

                        if (Request.QueryString.Count > 0)
                        {
                            string return_url = Request.QueryString["returnUrl"];
                            Response.Redirect(return_url);

                        }
                        else
                        {
                            createLogLogin(employee_id, contractor_id, "contractor");
                            Response.Redirect("MainMenu.aspx");

                        }

                    }
                    else
                    {
                        errorContractor.Text = Resources.Main.errorContractor;
                    }
                }
                else
                {
                    errorContractor.Text = Resources.Main.errorContractor;

                }

            }
        }

        protected void btLoginCheck_Click(object sender, EventArgs e)
        {
            Session["typeLogin"] = "ad";
            //Response.Write("A"); Response.End();
            if (txtUsernameCheck.Text != "" && txtPasswordCheck.Text != "")
            {

				string dominName = string.Empty;
				string adPath = string.Empty;
				string userName = txtUsernameCheck.Text.Trim().ToUpper();
				string strError = string.Empty;
				//try
				//{
                foreach (string key in ConfigurationSettings.AppSettings.Keys)
                {
                    //Response.Write(ConfigurationSettings.AppSettings[key]);

                    dominName = key.Contains("DirectoryDomain") ? ConfigurationSettings.AppSettings[key] : dominName;
                    adPath = key.Contains("DirectoryPath") ? ConfigurationSettings.AppSettings[key] : adPath;
                    if (!String.IsNullOrEmpty(dominName) && !String.IsNullOrEmpty(adPath))
                    {
                        if (true == AuthenticateUser(dominName, userName, txtPasswordCheck.Text, adPath, out strError))
                        {
                          // Response.Write("Succ=" + strError); Response.End();
                            //Response.Redirect("Default.aspx");// Authenticated user redirects to default.aspx
                            CheckEmp(userName);
                        }
                        dominName = string.Empty;
                        adPath = string.Empty;
                        if (String.IsNullOrEmpty(strError)) break;
                    }

                }
                if (!string.IsNullOrEmpty(strError))
                {
                    errorCheck.Text = Resources.Main.errorCheck;
                }
				/*}
				catch
				{

				}
				finally
				{

				} */
               
            }
            else
            {
                errorCheck.Text = Resources.Main.errorCheck;

            }
        }


        public void getNameShowLogin(string name_en, string name_th)
        {

            if (Session["lang"] != null)
            {
                string lang = Session["lang"].ToString();
                Session["name_en"] = name_en;
                Session["name_th"] = name_th;

                switch (lang)
                {
                    case "th":
                        Session["name"] = name_th;
                        break;
                    case "en":
                        Session["name"] = name_en;
                        break;
                    case "si":
                        Session["name"] = name_en;
                        break;

                }

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

                // Update the new path to the user in the directory
                LdapPath = result.Path;
                string _filterAttribute = (String)result.Properties["cn"][0];
                Errmsg = _filterAttribute;
                Errmsg = LdapPath; 
               // Response.Write("Succ=" + Errmsg); Response.End();
            }
            catch (Exception ex)
            {
                safetys4dbDataContext dbConnect = new safetys4dbDataContext();
                Errmsg = ex.Message;
              
                action_log objInsert = new action_log();
                objInsert.file_name = "login";
                objInsert.function_name = "AuthenticateUser";
                objInsert.error_message = Errmsg;
                dbConnect.action_logs.InsertOnSubmit(objInsert);
                objInsert.created = DateTime.UtcNow;

                dbConnect.SubmitChanges();
                Response.Write("Err=" + Errmsg); Response.End();
                return false;
                throw new Exception("Error authenticating user." + ex.Message);
            }
            return true;
        }

        public void CheckEmp(string adcn)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                /*String[] fullname = adcn.Split(' ');
                String firstname = fullname[0];
                String lastname = fullname[1];*/

                if (adcn != "")
                {

                    var v = from c in dbConnect.employees
                            where c.status == "Active" && c.domain_user == adcn
                            select new
                            {
                                name_en = c.first_name_en + " " + c.last_name_en,
                                name_th = c.first_name_th + " " + c.last_name_th,
                                user_id = c.employee_id,
                                org_id = c.unit_id,
                                mngt_level = c.mngt_level,
                                c.country,
                                c.timezone

                            };
                    bool checkData = false;
                    string employee_id = "";
                    string mngt_level = "";

                    foreach (var rc in v)
                    {
                        checkData = true;
                        Session["user_id"] = rc.user_id;
                        Session["phone"] = "";
                        Session["country"] = rc.country;
                        Session["timezone"] = rc.timezone;
                        getNameShowLogin(rc.name_en, rc.name_th);
                        employee_id = rc.user_id;
                        mngt_level = rc.mngt_level;

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
                            Session["function_id"] = ro.function_id;
                            Session["function"] = ro.function;
                            Session["company_id"] = ro.company_id;
                            if (rc.country == "thailand")
                            {
                                Session["department_id"] = ro.department_id;
                                Session["division_id"] = ro.division_id;
                                Session["section_id"] = ro.section_id;
                            }
                            else if (rc.country == "srilanka")
                            {
                                Session["department_id"] = ro.sub_function_id;
                                Session["division_id"] = ro.department_id;
                                Session["section_id"] = ro.division_id;
                            }
                           
                        }


                    }



                    if (checkData)
                    {
                        getPermission(AD, employee_id, mngt_level);
                        Session["system_admin"] = checkSystemAdmin(employee_id);

                        if(Session["country"].ToString()=="srilanka")
                        {
                            //////////////////////default language si/////////////////////////////////////////
                            HttpCookie cookie = new HttpCookie("lang");
                            cookie.Value = "si";
                            Response.Cookies.Add(cookie);
                            Session["langShow"] = "<img src='template/img/language/si.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English";
                            Session["lang"] = "si";
                            /////////////////////////////////////////////////////////////////////////////////

                        }
                       
                        if (Request.QueryString.Count > 0)
                        {
                            string return_url = Request.QueryString["returnUrl"];
                            Response.Redirect(return_url);

                        }
                        else
                        {
                            createLogLogin(employee_id, 0, "AD");
                            Response.Redirect("MainMenu.aspx");

                        }
                    }
                    else
                    {
                        errorCheck.Text = Resources.Main.errorCheck;
                    }

                }
                else
                {
                    errorCheck.Text = Resources.Main.errorCheck;

                }

            }
        }





        protected void getPermission(int group_login, string employee_id, string mngt_level)
        {


            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.groups
                        join m in dbConnect.group_has_permissions on c.id equals m.group_id
                        join pe in dbConnect.permissions on m.permission_id equals pe.id
                        where c.id == group_login && m.country == Session["country"].ToString()
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
                ArrayList de_fu = new ArrayList();
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
                            where c.id == 15 && m.country == Session["country"].ToString()
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
                            where c.id == 19 && m.country == Session["country"].ToString()
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
                            where c.employee_id == employee_id && g.country == Session["country"].ToString()
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
                        if (!group.Contains(d.group_name))
                        {
                            group.Add(d.group_name);
                        }

                        if (!group_value.Contains(d.group_value))
                        {
                            group_value.Add(d.group_value);
                        }
                       

                        if (d.function_id != null)
                        {
                            if (d.function_id != "")
                            {
                                if (!fu.Contains(d.function_id))
                                {
                                    fu.Add(d.function_id);
                                }
                                
                            }
                            
                        }
                    }

                    //////////////////////////Area Functional Manager////////////////////////////////
                    var fm = from c in dbConnect.employee_has_department_functional_managers
                             where c.employee_id == employee_id
                             select new
                             {
                                 c.id,
                                 c.department_id
                             };

                    if (fm.Count() > 0)
                    {
                        foreach (var j in fm)
                        {
                            if (j.department_id != "")
                            {
                                if (!de_fu.Contains(j.department_id))
                                {
                                    de_fu.Add(j.department_id);
                                }

                            }
                        }

                        var gpf1 = from c in dbConnect.group_has_permissions
                                   join pe in dbConnect.permissions on c.permission_id equals pe.id
                                   where c.group_id == 18 && c.country == Session["country"].ToString()
                                   select new
                                   {
                                       permission_name = pe.name,

                                   };


                        foreach (var df2 in gpf1)
                        {
                            per.Add(df2.permission_name);

                        }

                        group.Add("Area Functional Manager");
                        group_value.Add(4);//on tabl group
                    }
                    //////////////////////////end////////////////////////////////

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
                      
                            if (j.department_id != "")
                            {
                                if (!de.Contains(j.department_id))
                                {
                                    de.Add(j.department_id);
                                }

                            }
                        }

                        var gp1 = from c in dbConnect.group_has_permissions
                                  join pe in dbConnect.permissions on c.permission_id equals pe.id
                                  where c.group_id == 9 && c.country == Session["country"].ToString()
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
                                  where c.group_id == 10 && c.country == Session["country"].ToString()
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
                                  where c.group_id == 11 && c.country == Session["country"].ToString()
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

                //var test = group.ToArray().Distinct();
                Session["permission"] = per;
                Session["group"] = group;
                int min_group_value = GetMinValue(group_value);
                Session["group_value"] = min_group_value;
                //int group_id = getGroupID(min_group_value,group);
                Session["group_id"] = getGroupID(min_group_value, group);
                Session["area_function"] = fu;
                Session["area_department"] = de;
                Session["area_department_functional"] = de_fu;
                Session["area_division"] = di;
                Session["area_section"] = se;

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

        protected int getGroupID(int min_group_value,ArrayList groups)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                var v = from c in dbConnect.groups
                        where c.value == min_group_value && groups.ToArray().Contains(c.name)
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


        protected bool checkSystemAdmin(string employee_id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                bool result = false;


                var v = from c in dbConnect.employee_has_groups
                        where c.group_id == 1 && c.employee_id == employee_id
                        select new
                        {
                            c.id

                        };

                ArrayList per = new ArrayList();

                foreach (var r in v)
                {
                    result = true;

                }


                return result;
            }
        }


        protected void createLogLogin(string employee_id,int contractor_id,string type_login)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                HttpBrowserCapabilities brObject = Request.Browser;

                login_log objInsert = new login_log();

                if (employee_id != "")
                {
                    objInsert.employee_id = employee_id;
                }
                else
                {
                    objInsert.contractor_id = contractor_id;
                }

                objInsert.browser = brObject.Type;
                objInsert.ip_address = Request.UserHostAddress;
                objInsert.type_login = type_login;
                objInsert.type_device = "web";
                objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                dbConnect.login_logs.InsertOnSubmit(objInsert);

                dbConnect.SubmitChanges();
            }
        }



           
    }
}