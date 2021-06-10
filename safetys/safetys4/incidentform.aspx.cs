using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using safetys4.MasterdataService;
using safetys4.ActioneventService;
using Newtonsoft.Json;
using System.Net;
using System.Web.Script.Serialization;
using System.Globalization;
using safetys4.App_Code;
using System.Collections;


namespace safetys4
{
    public partial class incidentform : System.Web.UI.Page
    {
        string PageType = "add";
        string id = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
                {
                    if (!IsPostBack)
                    {

                        bool result = Permission.checkPermision("report incident", Session["permission"] as ArrayList);
                        if (!result)
                        {
                            Response.Redirect("MainMenu.aspx?msg_err=permision");
                        }


                        setStepForm();

                        txtreport_date.Disabled = true;
                        txtname_surname.Disabled = true;

                    }

                }
                else
                {
                    string original_url = Server.UrlEncode(Context.Request.RawUrl);
                    Response.Redirect("login.aspx?returnUrl=" + original_url);
                }
            }
            catch (Exception ex)
            {

                using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
                {

                    action_log objInsert = new action_log();
                    objInsert.function_name = "problem";
                    objInsert.file_name = "error";
                    objInsert.error_message = ex.Message;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                }

            }


           
           
        }

        protected void setAdd()
        {
            btIncidentcheck.Style["visibility"] = "hidden";
            btIncidentedit.Style["visibility"] = "hidden";
            btIncidentreject.Style["visibility"] = "hidden";
            if (Session["lang"] != null)
            {
                txtreport_date.Value = FormatDates.getDateNow(Session["lang"].ToString(), Session["timezone"].ToString());
            }
            lbshowimage.Style["visibility"] = "hidden";
            btUpdate.Style["visibility"] = "hidden";
            btReopenIncident.Style["visibility"] = "hidden";
            setInfoUser();

        }

        protected void setEdit()
        {
            lbshowimage.Style["visibility"] = "hidden";
            btIncidentedit.Style["visibility"] = "hidden";
            btSubmit.Style["visibility"] = "hidden";
            btIncidentcheck.Style["visibility"] = "hidden";
            btIncidentreject.Style["visibility"] = "hidden";
            btReopenIncident.Style["visibility"] = "hidden";

        }

        protected void setView()
        {
            txtincident_date.Disabled = true;
           // txtincident_time.Disabled = true;
            ddtimehour.Disabled = true;
            ddtimeminute.Disabled = true;
            ddcompany.Disabled = true;
            ddfunction.Disabled = true;
            dddepartment.Disabled = true;
            dddivision.Disabled = true;
            ddsection.Disabled = true;


            if (Session["country"].ToString()=="thailand")
            {
                ddactivitycompany.Disabled = true;
                ddactivityfunction.Disabled = true;
                ddactivitydepartment.Disabled = true;
                ddactivitydivision.Disabled = true;
                ddactivitysection.Disabled = true;

                owner_activity1.Disabled = true;
                owner_activity2.Disabled = true;
                responsible_area1.Disabled = true;
                responsible_area2.Disabled = true;

            }

            txtincident_area.Disabled = true;
            txtincidentname.Disabled = true;
            txtincidentdetail.Disabled = true;
            txtphone.Disabled = true;

            btSubmit.Style["visibility"] = "hidden";
            //btCancel.Style["visibility"] = "hidden";
            btUpdate.Style["visibility"] = "hidden";
           

            //ttps://www.dotnetperls.com/directory-getfiles
            //foreach (string s in Directory.GetFiles(path, "*.txt").Select(Path.GetFileName))
            //    Console.WriteLine(s);



        }

        protected void setInfoUser()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            string type_login = Session["typeLogin"].ToString();

            if(type_login=="contractor")
            {
                var v = from c in dbConnect.contractors
                        where c.id == Convert.ToInt32(Session["user_id"])
                        select new
                        {
                            prefix = chageDataLanguage(c.prefix_th,c.prefix_en,Session["lang"].ToString()),
                            first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                            last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                            phone = c.phone

                        };

              
                foreach (var rc in v)
                {
                    txtname_surname.Value = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                    txtphone.Value = rc.phone;

                }

            }
            else
            {
                var v = from c in dbConnect.employees
                        where c.employee_id  == Session["user_id"].ToString()
                        select new
                        {
                            prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                            first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                            last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                            

                        };


                foreach (var rc in v)
                {
                    txtname_surname.Value = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                   
                }


            }
          
            
        }


        protected string chageDataLanguage(string vTH, string vEN, string lang)
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


        protected void setDropdownCompany()
        {
           var urlCompany = "http://localhost/safetys4/Masterdata.asmx/getCompany?lang=" + Session["lang"];
           var syncClient = new WebClient();
           var contentCompany = syncClient.DownloadString(urlCompany);

           
           var jss = new JavaScriptSerializer();
           var dataCompany = jss.Deserialize<dynamic>(contentCompany);

            ddcompany.Items.Add(new ListItem("", ""));

           foreach (var v in dataCompany)
           {
               ddcompany.Items.Add(new ListItem(v["name"], v["id"]));

           }
         
        }

       
        protected void btIncidentedit_Click(object sender, EventArgs e)
        {
          
            id = Request.QueryString["id"];
           

            Response.Redirect("incidentform.aspx?pagetype=edit&id=" + id);
        }

        

        //protected void ddfunction_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    setDepartment(ddfunction.SelectedValue);
        //}

        //protected void ddcompany_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //    setFunction(ddcompany.SelectedValue);

        //}

        //protected void dddepartment_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    setDivision(dddepartment.SelectedValue);
        //}

        //protected void dddivision_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    setSection(dddivision.SelectedValue);

        //}

        //protected void txtincident_area_TextChanged(object sender, EventArgs e)
        //{
        //    string area_id = hddareaid.Value;

        //    if (area_id!="" && area_id !=null)
        //    {

        //          safetys4dbDataContext dbConnect = new safetys4dbDataContext();
        //            var v = from c in dbConnect.area_managements                
        //                    where c.id == Convert.ToInt32(area_id)
        //                    select c;

        //            foreach(var s in v)
        //            {

        //                ddcompany.ClearSelection(); //making sure the previous selection has been cleared
        //                ddcompany.Items.FindByValue(s.company_id).Selected = true;


        //                setFunction(s.company_id);
        //                ddfunction.Items.FindByValue(s.function_id).Selected = true;

        //                setDepartment(s.function_id);
        //                dddepartment.Items.FindByValue(s.department_id).Selected = true;

        //                setDivision(s.department_id);
        //                dddivision.Items.FindByValue(s.division_id).Selected = true;

        //                setSection(s.division_id);
        //                ddsection.Items.FindByValue(s.section_id).Selected = true;


        //            }

        //            hddareaid.Value = "";
                   

        //    }
          

        //}

        //protected void setFunction(string company_id)
        //{

        //    var url = "http://localhost/safetys4/Masterdata.asmx/getFuctionByCompany?company=" + company_id + "&lang=" + Session["lang"];
        //    var syncClient = new WebClient();
        //    var content = syncClient.DownloadString(url);


        //    var jss = new JavaScriptSerializer();
        //    var data = jss.Deserialize<dynamic>(content);
        //    ddfunction.Items.Clear();
        //    ddfunction.Items.Add(new ListItem("", ""));

        //    foreach (var v in data)
        //    {
        //        ddfunction.Items.Add(new ListItem(v["name"], v["id"]));

        //    }

        //    dddepartment.Items.Clear();
        //    dddivision.Items.Clear();
        //    ddsection.Items.Clear();
        //    dddepartment.Items.Add(new ListItem("", ""));
        //    dddivision.Items.Add(new ListItem("", ""));
        //    ddsection.Items.Add(new ListItem("", ""));

        //}
        //protected void setDepartment(string function)
        //{
        //    var url = "http://localhost/safetys4/Masterdata.asmx/getDepartmentbyFunction?function=" + function + "&lang=" + Session["lang"];
        //    var syncClient = new WebClient();
        //    var content = syncClient.DownloadString(url);


        //    var jss = new JavaScriptSerializer();
        //    var data = jss.Deserialize<dynamic>(content);
        //    dddepartment.Items.Clear();
        //    dddepartment.Items.Add(new ListItem("", ""));

        //    foreach (var v in data)
        //    {
        //        dddepartment.Items.Add(new ListItem(v["name"], v["id"]));

        //    }

        //    dddivision.Items.Clear();
        //    ddsection.Items.Clear();
        //    dddivision.Items.Add(new ListItem("", ""));
        //    ddsection.Items.Add(new ListItem("", ""));

        //}

        //protected void setDivision(string department)
        //{
        //    var url = "http://localhost/safetys4/Masterdata.asmx/getDivisionbyDepartment?department=" + department + "&lang=" + Session["lang"];
        //    var syncClient = new WebClient();
        //    var content = syncClient.DownloadString(url);

        //    var jss = new JavaScriptSerializer();
        //    var data = jss.Deserialize<dynamic>(content);
        //    dddivision.Items.Clear();
        //    dddivision.Items.Add(new ListItem("", ""));

        //    foreach (var v in data)
        //    {
        //        dddivision.Items.Add(new ListItem(v["name"], v["id"]));

        //    }
        //    ddsection.Items.Clear();
        //    ddsection.Items.Add(new ListItem("", ""));

        //}

        //protected void setSection(string division)
        //{
        //    var url = "http://localhost/safetys4/Masterdata.asmx/getSectionbyDivision?division=" + division + "&lang=" + Session["lang"];
        //    var syncClient = new WebClient();
        //    var content = syncClient.DownloadString(url);

        //    var jss = new JavaScriptSerializer();
        //    var data = jss.Deserialize<dynamic>(content);
        //    ddsection.Items.Clear();
        //    ddsection.Items.Add(new ListItem("", ""));

        //    foreach (var v in data)
        //    {
        //        ddsection.Items.Add(new ListItem(v["name"], v["id"]));

        //    }

        //}


        protected void btIncidentcheck_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var query = from c in dbConnect.incidents
                        where c.id == Convert.ToInt32(id)
                        select c;

            string division_id = "";
            string section_id = "";

            foreach (incident rc in query)
            {
               
                if(rc.step_form < 2)// ถ้าเลย step2 แล้วไม่ว่าสิทธิ์ไหนมากดก็ไม่ย้อนให้
                {
                    rc.step_form = 2;
                    rc.incident_flow = 2;
                }
               
                if (Session["group_id"]!=null)
                {
                    rc.verify_report_form1 = Convert.ToInt32(Session["group_id"]);

                }

                division_id = rc.division_id;
                section_id = rc.section_id;
                
            }

            
            //////////////////////////////////by p.poo sent notification/////////////////////////////////
            //เช็คตาม flow น่าจะไปส่งตอน submit form 2 
            //Class.SafetyNotification sn = new Class.SafetyNotification();
            //string[] alert_to_groups = { "AreaOH&S" };
            //sn.InsertNotification(2, Convert.ToInt32(id), alert_to_groups);
            ///////////////////////////////////end//////////////////////////////////////////////////////


            if (string.IsNullOrEmpty(division_id) || string.IsNullOrEmpty(section_id) || section_id == "00000000" || division_id == "00000000")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.select_division_section + "');", true);


            }else{
                dbConnect.SubmitChanges();
                Response.Redirect("incidentform2.aspx?pagetype=view&id=" + id);
            }
            
        }



        protected void setStepForm()
        {
            PageType = Request.QueryString["PageType"];
            id = Request.QueryString["id"];

            if (PageType.Equals("add"))
            {
                step2.Enabled = false;
                step2.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                step2.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                step3.Enabled = false;
                step3.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                step3.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                step4.Enabled = false;
                step4.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                step4.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                setAdd();
            }
            else
            {
                safetys4dbDataContext dbConnect = new safetys4dbDataContext();
                byte step = 0;
                int status_form = 1;
                string reason_reject = "";
                var query = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (incident rc in query)
                {
                    step = Convert.ToByte(rc.step_form);
                    reason_reject = rc.reason_reject;
                    status_form = rc.process_status;
                }

                

                if (step == 1)
                {
                    step2.Enabled = false;
                    step2.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step2.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                    step3.Enabled = false;
                    step3.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step3.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                    step4.Enabled = false;
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                    //if (!string.IsNullOrEmpty(reason_reject))
                    //{
                    //    btIncidentreject.Style["visibility"] = "hidden";
                    //}

                }
                else if (step == 2)
                {

                    step3.Enabled = false;
                    step3.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step3.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                    step4.Enabled = false;
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";

                    //btIncidentcheck.Style["visibility"] = "hidden";
                    //if (!string.IsNullOrEmpty(reason_reject))
                    //{
                    //    btIncidentreject.Style["visibility"] = "hidden";
                    //}
                   

                }
                else if (step == 3)
                {
                    step4.Enabled = false;
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                    //btIncidentcheck.Style["visibility"] = "hidden";
                    //btIncidentreject.Style["visibility"] = "hidden";
                }
                else if (step == 4)
                {
                    //btIncidentcheck.Style["visibility"] = "hidden";
                    //btIncidentreject.Style["visibility"] = "hidden";

                }

                setPermissionByStatus(status_form);
            
            }
               


        }


        protected void setPermissionByStatus(int status)
        {
            PageType = Request.QueryString["PageType"];
            id = Request.QueryString["id"];

            ArrayList gr = new ArrayList();

            gr = Session["group"] as ArrayList;

          

            if (PageType.Equals("edit"))
            {
                id = Request.QueryString["id"];
                setEdit();
            }
            else if (PageType.Equals("view"))
            {
                id = Request.QueryString["id"];
                setView();


                //if(status==2)//close
                //{
                //    if (!(gr.IndexOf("Super Admin") > -1))//ไม่ใช่ super admin
                //    {                     
                //        btIncidentedit.Style["visibility"] = "hidden";
                      
                //    }

                //}
                
                
            }
           
        }

        protected void step1_Click(object sender, EventArgs e)
        {
            PageType = Request.QueryString["PageType"];
            if (PageType.Equals("add"))
            {

                Response.Redirect("incidentform.aspx?pagetype=add");
            }
            else
            {

                id = Request.QueryString["id"];
                Response.Redirect("incidentform.aspx?pagetype=view&id=" + id);
            }
           
        }

        protected void step2_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform2.aspx?pagetype=view&id=" + id);
        }

        protected void step3_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform3.aspx?pagetype=view&id=" + id);
        }

        protected void step4_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform4.aspx?pagetype=view&id=" + id);
        }

        protected void btReopenIncident_Click(object sender, EventArgs e)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                id = Request.QueryString["id"];

                var query = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(id)
                            select c;


                foreach (incident rc in query)
                {
                    if (rc.process_status == 2)
                    {//close
                        rc.process_status = 1; //on process
                        rc.step_form = 3;
                        rc.incident_flow = 3;
                        rc.request_close_form3 = null;
                        rc.edit_form3 = null;

                        dbConnect.SubmitChanges();

                        var gr = from c in dbConnect.log_request_close_incidents
                                 where c.incident_id == Convert.ToInt32(id)
                                 select c;
                        foreach (var a in gr)
                        {
                            a.status = "D";
                        }

                        dbConnect.SubmitChanges();

                    }
                    else if (rc.process_status == 3)//reject
                    {
                        rc.process_status = 1; //on process
                        rc.reason_reject_type = null;
                        rc.reason_reject = "";

                        dbConnect.SubmitChanges();
                    }
                    else if (rc.process_status == 4)//Exemption
                    {
                        rc.process_status = 1; //on process
                        rc.reason_except_type = null;
                        rc.reason_except = "";

                        dbConnect.SubmitChanges();
                    }
                   
                }

               
            }
        }





     


    }
}