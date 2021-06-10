using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using safetys4.App_Code;
namespace safetys4
{
    public partial class hazardform3 : System.Web.UI.Page
    {
        string PageType = "add";
        string id = "";
        protected void Page_Load(object sender, EventArgs e)
        {


            if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
            {
                PageType = Request.QueryString["PageType"];


                if (!IsPostBack)
                {


                    bool result = Permission.checkPermision("report hazard3 view", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
                    }

                    setStepForm();

                    txtname_areaowner.Disabled = true;
              
                }
            }
            else
            {
                string original_url = Server.UrlEncode(Context.Request.RawUrl);
                Response.Redirect("login.aspx?returnUrl=" + original_url);
            }
        }


        protected void setEdit()
        {
            btHazardedit.Style["visibility"] = "hidden";        
            btrequestclose.Style["visibility"] = "hidden";

        }

        protected void setView()
        {
            btCreateProcessAction.Disabled = true;
            btUpdate.Style["visibility"] = "hidden";

        }

        protected void setInfoUser()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            string type_login = Session["typeLogin"].ToString();

            if (type_login == "contractor")
            {
                var v = from c in dbConnect.contractors
                        where c.id == Convert.ToInt32(Session["user_id"])
                        select new
                        {
                            prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                            first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                            last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                            phone = c.phone

                        };


                foreach (var rc in v)
                {

                    txtname_areaowner.Value = rc.prefix + " " + rc.first_name + " " + rc.last_name;


                }

            }
            else
            {
                var v = from c in dbConnect.employees
                        where c.employee_id == Session["user_id"].ToString()
                        select new
                        {
                            prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                            first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                            last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),


                        };


                foreach (var rc in v)
                {
                    txtname_areaowner.Value = rc.prefix + " " + rc.first_name + " " + rc.last_name;

                }


            }


        }


        protected void setStepForm()
        {

            id = Request.QueryString["id"];

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            byte step = 0;
            int status_form = 1;
            var query = from c in dbConnect.hazards
                        where c.id == Convert.ToInt32(id)
                        select c;

            foreach (hazard rc in query)
            {
                step = Convert.ToByte(rc.step_form);
                status_form = rc.process_status;
            }

            if (step < 3)
            {
                Response.Redirect("hazardform" + step + ".aspx?pagetype=view&id=" + id);
            }
            else
            {
                if (step == 3)
                {
                    step4.Enabled = false;
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                }
            }
            


            setPermissionByStatus(status_form);

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


                //if (status == 2)//close
                //{
                //    if (!(gr.IndexOf("Super Admin") > -1))//ไม่ใช่ super admin
                //    {
                //        btHazardedit.Style["visibility"] = "hidden";

                //    }

                //}


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

        protected void btHazardedit_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];


            Response.Redirect("hazardform3.aspx?pagetype=edit&id=" + id);
        }




        protected void step1_Click(object sender, EventArgs e)
        {
            PageType = Request.QueryString["PageType"];
            if (PageType.Equals("add"))
            {

                Response.Redirect("hazardform.aspx?pagetype=add");
            }
            else
            {

                id = Request.QueryString["id"];
                Response.Redirect("hazardform.aspx?pagetype=view&id=" + id);
            }
        }

        protected void step2_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];

            Response.Redirect("hazardform2.aspx?pagetype=view&id=" + id);
        }

        protected void step3_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];

            Response.Redirect("hazardform3.aspx?pagetype=view&id=" + id);
        }

        protected void step4_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];

            Response.Redirect("hazardform4.aspx?pagetype=view&id=" + id);
        }

        protected void btrequestclose_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            bool close_all_action = true;

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var action = from c in dbConnect.process_actions
                         where c.hazard_id == Convert.ToInt32(id)
                         select c;


            if (action.Count() > 0)
            {
                foreach (process_action rc in action)
                {
                    if (rc.action_status_id != 3 && rc.action_status_id != 5 && rc.action_status_id != 4)//complete and cancel and close
                    {
                        close_all_action = false;
                    }

                }



                if (close_all_action)//close all action
                {

                    var query = from c in dbConnect.hazards
                                where c.id == Convert.ToInt32(id)
                                select c;

                    foreach (hazard rc in query)
                    {

                        if (rc.step_form < 4)
                        {
                            rc.step_form = 4;
                            rc.hazard_flow = 4;
                            rc.confirm_form_three_to_four_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        }


                        if (Session["group_id"] != null)
                        {
                            rc.request_close_form3 = Convert.ToInt32(Session["group_id"]);

                        }
                    }

                    dbConnect.SubmitChanges();



                    //////////////////////////////////by p.poo sent notification/////////////////////////////////

                    Class.SafetyNotification sn = new Class.SafetyNotification();
                    string[] alert_to_groups = new string[1];
                    string role_action = "";


                    var t = from c in dbConnect.close_step_hazards
                            where c.step == 1 && c.country == Session["country"].ToString()
                            select c;


                    foreach (var r in t)
                    {
                        if (r.group_id == 4 || r.group_id == 5)
                        {
                            alert_to_groups[0] = "AdminOH&S";
                            role_action = "AdminOH&S";

                        }
                        else if (r.group_id == 8)
                        {
                            alert_to_groups[0] = "GroupOH&S";
                            role_action = "GroupOH&S";
                        }
                        else if (r.group_id == 9)
                        {
                            alert_to_groups[0] = "AreaOH&S";
                            role_action = "AreaOH&S";
                        }
                        else if (r.group_id == 10)
                        {
                            alert_to_groups[0] = "AreaManager";
                            role_action = "AreaManager";
                        }
                        else if (r.group_id == 11)
                        {
                            alert_to_groups[0] = "AreaSuperervisor";
                            role_action = "AreaSuperervisor";
                        }
                        else if (r.group_id == 16)
                        {
                            alert_to_groups[0] = "GroupOH&SHazard";
                            role_action = "GroupOH&SHazard";
                        }


                    }

                    sn.InsertHazardNotification(12, Convert.ToInt32(id), alert_to_groups, Session["timezone"].ToString(), role_action);
                    ///////////////////////////////////end//////////////////////////////////////////////////////


                    //  ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + Resources.Main.success + "'); window.location='" + Request.ApplicationPath + "/hazardform3.aspx?pagetype=view&id=" + id + "';", true);
                    Response.Redirect("hazardform3.aspx?pagetype=view&id=" + id);

                }
                else
                {

                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.close_all_action + "');", true);


                }


            }
            else
            {

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.take_action + "');", true);



            }
           
           
        }

    }
}