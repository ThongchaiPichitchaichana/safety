using safetys4.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class healthform2 : System.Web.UI.Page
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
                    bool result = Permission.checkPermision("report health2 view", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
                    }


                    setStepForm();
                }

            }
            else
            {
                string original_url = Server.UrlEncode(Context.Request.RawUrl);
                Response.Redirect("login.aspx?returnUrl=" + original_url);
            }
        }

        protected void btHealthedit_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("healthform2.aspx?pagetype=edit&id=" + id);
        }


         protected void setEdit()
        {

            btHealthedit.Style["visibility"] = "hidden";

        }


        protected void setView()
        {
            btUpdate.Style["visibility"] = "hidden";
            btCreateProcessAction.Disabled = true;
          

        }


        protected void setStepForm()
        {
            PageType = Request.QueryString["PageType"];
            id = Request.QueryString["id"];

            if (PageType.Trim().Equals("add"))
            {
                step2.Enabled = false;
                step2.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                step2.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                step3.Enabled = false;
                step3.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                step3.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
              
            }
            else
            {
                safetys4dbDataContext dbConnect = new safetys4dbDataContext();
                byte step = 0;
                int status_form = 1;
                var query = from c in dbConnect.healths
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (health rc in query)
                {
                    step = Convert.ToByte(rc.step_form);
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

                }
                else if (step == 2)
                {

                    step3.Enabled = false;
                    step3.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step3.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";

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


            }

        }


        protected void step1_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("healthform.aspx?pagetype=view&id=" + id);
        }

        protected void step3_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("healthform3.aspx?pagetype=view&id=" + id);
        }

        protected void step2_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("healthform2.aspx?pagetype=view&id=" + id);
        }

        protected void btrequestclose_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            bool close_all_action = true;

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var action = from c in dbConnect.process_action_healths
                         where c.health_id == Convert.ToInt32(id)
                         select c;

            foreach (process_action_health rc in action)
            {
                if (rc.action_status_id != 2 && rc.action_status_id != 3)//close and cancel
                {
                    close_all_action = false;
                }

            }


            if (action.Count() == 0)
            {
                close_all_action = false;
            }

            if (close_all_action)//close all action
            {

                var query = from c in dbConnect.healths
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (health rc in query)
                {
                    if (rc.step_form < 3)
                    {
                        rc.step_form = 3;
                        rc.confirm_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                    }

                    if (Session["group_id"] != null)
                    {
                        rc.request_close_form = Convert.ToInt32(Session["group_id"]);

                    }
                }

                //////////////////////////////////by p.poo sent notification/////////////////////////////////
                dbConnect.SubmitChanges();

                var t = from c in dbConnect.close_step_healths
                        where c.step == 1 && c.country == Session["country"].ToString()
                        select c;

                Class.SafetyNotification sn = new Class.SafetyNotification();
                string[] alert_to_groups = new string[1];
                string role_action = "";
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
                    else if (r.group_id == 18)
                    {
                        alert_to_groups[0] = "Functional";
                        role_action = "Functional";
                    }


                }

                sn.InsertHealthNotification(2, Convert.ToInt32(id), alert_to_groups, Session["timezone"].ToString(), role_action);

                ///////////////////////////////////end//////////////////////////////////////////////////////

                //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.success + "');", true);
                // ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + Resources.Main.success + "'); window.location='" + Request.ApplicationPath + "/incidentform3.aspx?pagetype=view&id=" + id + "';", true);
                Response.Redirect("healthform3.aspx?pagetype=view&id=" + id);

            }
            else
            {

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.close_all_action + "');", true);


            }
        }
    

    
    }
}