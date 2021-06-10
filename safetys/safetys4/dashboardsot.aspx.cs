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
    public partial class dashboardsot : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {

                if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
                {
                    if (!IsPostBack)
                    {
                        bool result = Permission.checkPermision("dashboard", Session["permission"] as ArrayList);
                        if (!result)
                        {
                            Response.Redirect("MainMenu.aspx?msg_err=permision");
                        }
                        setPermissionMenuSide();
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

        protected void setPermissionMenuSide()
        {
            Panel pn;
            pn = (Panel)Master.FindControl("menu_sidebar_dashboard");
            pn.Visible = true;

            LinkButton link1 = (LinkButton)Master.FindControl("btDashboardIncidentSide");
            link1.Visible = true;


            LinkButton link2 = (LinkButton)Master.FindControl("btDashboardHazardSide");
            link2.Visible = true;
           

            LinkButton link3 = (LinkButton)Master.FindControl("btDashboardSotSide");
            link3.Visible = true;
            link3.Attributes.CssStyle.Add("background-color", "#e6e6e8");



        }
    }
}