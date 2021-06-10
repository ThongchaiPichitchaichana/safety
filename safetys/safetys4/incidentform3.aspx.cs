using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using safetys4.App_Code;
using System.Collections;

namespace safetys4
{
    public partial class incidentform3 : System.Web.UI.Page
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
                    bool result = Permission.checkPermision("report incident3 view", Session["permission"] as ArrayList);
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


        protected void setEdit()
        {

            btIncidentedit.Style["visibility"] = "hidden";
            btrequestclose.Style["visibility"] = "hidden";
       

        }


        protected void setView()
        {
            culpability1.Disabled = true;
            culpability2.Disabled = true;
            culpability3.Disabled = true;

            road_accident1.Disabled = true;
            road_accident2.Disabled = true;

            rootcause1.Disabled = true;
            rootcause2.Disabled = true;
            rootcause3.Disabled = true;
            rootcause4.Disabled = true;
            rootcause5.Disabled = true;
            rootcause6.Disabled = true;
            rootcause7.Disabled = true;
            rootcause8.Disabled = true;
            rootcause9.Disabled = true;
            rootcause10.Disabled = true;
            rootcause11.Disabled = true;
            rootcause12.Disabled = true;
            rootcause13.Disabled = true;
            rootcause14.Disabled = true;
            rootcause15.Disabled = true;
            rootcause16.Disabled = true;
            rootcause17.Disabled = true;
            rootcause18.Disabled = true;
            rootcause19.Disabled = true;
            ddfatality.Disabled = true;
            txtother_please.Disabled = true;

            txtcontributing_factor.Disabled = true;

            btUpdate.Style["visibility"] = "hidden";
            btAddFactFinding.Disabled = true;
            btCreateCorrectivePreentive.Disabled = true;
            btCreateRootCauseAction.Disabled = true;

            ddfunction.Disabled = true;
            dddepartment.Disabled = true;

        }


      

    

        protected void btrequestclose_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            bool close_all_action = true;

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var action = from c in dbConnect.corrective_prevention_action_incidents
                        where c.incident_id == Convert.ToInt32(id)
                        select c;

                foreach (corrective_prevention_action_incident rc in action)
                {
                    if (rc.action_status_id != 3 && rc.action_status_id != 5 && rc.action_status_id != 4)//complete and cancel and close
                    {
                        close_all_action = false;
                    }

                }


            var action2 = from c in dbConnect.preventive_action_incidents
                            where c.incident_id == Convert.ToInt32(id)
                            select c;

            foreach (preventive_action_incident rc in action2)
            {
                if (rc.action_status_id != 3 && rc.action_status_id != 5 && rc.action_status_id != 4)//complete and cancel and close
                {
                    close_all_action = false;
                }

            }



            var action3 = from c in dbConnect.consequence_management_incidents
                          where c.incident_id == Convert.ToInt32(id)
                          select c;

            foreach (consequence_management_incident rc in action3)
            {
                if (rc.action_status_id != 3 && rc.action_status_id != 5 && rc.action_status_id != 4)//complete and cancel and close
                {
                    close_all_action = false;
                }

            }

            if (close_all_action)//close all action
            {

                var query = from c in dbConnect.incidents
                        where c.id == Convert.ToInt32(id)
                        select c;

                foreach (incident rc in query)
                {
                    if (rc.step_form < 4)
                    {
                        rc.step_form = 4;
                        rc.incident_flow = 4;
                        rc.confirm_form_three_to_four_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                    }

                    if (Session["group_id"] != null)
                    {
                        rc.request_close_form3 = Convert.ToInt32(Session["group_id"]);

                    }
                }

                //////////////////////////////////by p.poo sent notification/////////////////////////////////
                dbConnect.SubmitChanges();

                var t = from c in dbConnect.close_step_incidents
                        where c.step == 1 && c.country == Session["country"].ToString()
                        select c;
               
                Class.SafetyNotification sn = new Class.SafetyNotification();
                string[] alert_to_groups = new string[1];
                string role_action = "";
                foreach(var r in t)
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
                     
                   
                }

                sn.InsertNotification(11, Convert.ToInt32(id), alert_to_groups, Session["timezone"].ToString(), role_action);
               
                ///////////////////////////////////end//////////////////////////////////////////////////////

                //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.success + "');", true);
               // ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + Resources.Main.success + "'); window.location='" + Request.ApplicationPath + "/incidentform3.aspx?pagetype=view&id=" + id + "';", true);
                Response.Redirect("incidentform3.aspx?pagetype=view&id=" + id);

            }
            else
            {

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.close_all_action + "');", true);


            }
            
          
        }

        protected void btIncidentedit_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform3.aspx?pagetype=edit&id=" + id);
        }




        protected void setStepForm()
        {

            id = Request.QueryString["id"];

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            byte step = 0;
            int status_form = 1;
            var query = from c in dbConnect.incidents
                        where c.id == Convert.ToInt32(id)
                        select c;

            foreach (incident rc in query)
            {
                step = Convert.ToByte(rc.step_form);
                status_form = rc.process_status;
            }

            if (step < 3)
            {
                Response.Redirect("incidentform" + step + ".aspx?pagetype=view&id=" + id);
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
                //        btIncidentedit.Style["visibility"] = "hidden";

                //    }

                //}


            }

        }
        protected void step1_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform.aspx?pagetype=view&id=" + id);
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

    }
}