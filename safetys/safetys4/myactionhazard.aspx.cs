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
    public partial class myactionhazard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            try
            {

                if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
                {
                    if (!IsPostBack)
                    {
                        bool result = Permission.checkPermision("my action hazard", Session["permission"] as ArrayList);
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
            pn = (Panel)Master.FindControl("menu_sidebar_myaction");
            pn.Visible = true;

            ArrayList per = new ArrayList();

            per = Session["permission"] as ArrayList;

            if (per.IndexOf("my action incident") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btMyActionIncidentSide");
                link.Visible = true;

            }

            if (per.IndexOf("my action hazard") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btMyActionHazardSide");
                link.Visible = true;
                link.Attributes.CssStyle.Add("background-color", "#e6e6e8");

            }


            if (per.IndexOf("my action sot") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btMyActionSotSide");
                link.Visible = true;

            }


        }
    }
}