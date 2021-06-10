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
    public partial class workhour : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
            {
                if (!IsPostBack)
                {
                    //bool result = Permission.checkPermision("workhour", Session["permission"] as ArrayList);
                    //if (!result)
                    //{
                    //    Response.Redirect("MainMenu.aspx?msg_err=permision");
                    //}
                    setPermissionMenuSide();
                    setview();

                  
                }
            }
            else
            {
                string original_url = Server.UrlEncode(Context.Request.RawUrl);
                Response.Redirect("login.aspx?returnUrl=" + original_url);
            }
        }

        protected void setview()
        {
            bool result = Permission.checkPermision("work hour year search", Session["permission"] as ArrayList);
            if (!result)
            {
                ddyear.Disabled = true;
            }

        }

        protected void setPermissionMenuSide()
        {
            Panel pn;
            pn = (Panel)Master.FindControl("menu_sidebar_admin");
            pn.Visible = true;

            ArrayList per = new ArrayList();

            per = Session["permission"] as ArrayList;

            if (per.IndexOf("super admin") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btSuperAdminSide");
                link.Visible = true;

            }

            if (per.IndexOf("holiday") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btHolidaySide");
                link.Visible = true;

            }

            if (per.IndexOf("area management") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btAreaManagementSide");
                link.Visible = true;

            }

            if (per.IndexOf("setting") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btSettingSide");
                link.Visible = true;

            }

            if (per.IndexOf("notify group") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btNotifyGroupSide");
                link.Visible = true;

            }


            if (per.IndexOf("target") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btTargetSide");
                link.Visible = true;

            }

            if (per.IndexOf("work hour") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btWorkHourSide");
                link.Visible = true;
                link.Attributes.CssStyle.Add("background-color", "#e6e6e8");

            }



        }
    }
}