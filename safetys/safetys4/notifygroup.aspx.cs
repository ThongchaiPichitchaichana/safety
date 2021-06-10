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
    public partial class notifygroup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
            {
                if (!IsPostBack)
                {
                    bool result = Permission.checkPermision("notify group", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
                    }
                    setPermissionMenuSide();
                }

                Panel secondPanel;
                secondPanel = (Panel)Master.FindControl("PanelNotifyGroup");
                secondPanel.Visible = true;

                String smenu = Request.QueryString["smenu"];
                if(smenu == "")
                {
                    smenu = "GroupCommunicationVP";
                }

                LinkButton LinkButton1;
                LinkButton1 = (LinkButton)Master.FindControl("Link" + smenu);
                LinkButton1.Attributes.CssStyle.Add("background-color", "#e6e6e8");

                if(smenu == "GroupCommunicationVP")
                {
                    lbHeaderNotifyGroup.Text = "Corporate Reputation";
                    btAdd.OnClientClick = "AddEmpToNotifyGroup('Actionevent.asmx/createGroupCommunicationVP',callGCVP)";
                }
                else if (smenu == "LegalDepartment")
                {
                    lbHeaderNotifyGroup.Text = "Legal Department";
                    btAdd.OnClientClick = "AddEmpToNotifyGroup('Actionevent.asmx/createLegalDepartment',callLegalDepartment)";
                }
                else if (smenu == "GroupOHS")
                {
                    lbHeaderNotifyGroup.Text = "Group OH&S";
                    btAdd.OnClientClick = "AddEmpToNotifyGroup('Actionevent.asmx/createGroupOHS',callGroupOHS)";
                }
                else if (smenu == "GroupOHSHazard")
                {
                    lbHeaderNotifyGroup.Text = "Group OH&S Hazard Approve";
                    btAdd.OnClientClick = "AddEmpToNotifyGroup('Actionevent.asmx/createGroupOHSHazard',callGroupOHSHazard)";
                }
                else if (smenu == "GroupOHSHealth")
                {
                    lbHeaderNotifyGroup.Text = "Group OH&S Health Rehab.";
                    btAdd.OnClientClick = "AddEmpToNotifyGroup('Actionevent.asmx/createGroupOHSHealth',callGroupOHSHealth)";
                }
                else if (smenu == "GroupEXCO")
                {
                    lbHeaderNotifyGroup.Text = "Group EXCO";
                    btAdd.OnClientClick = "AddEmpToNotifyGroup('Actionevent.asmx/createGroupEXCO',callGroupEXCO)";
                }
                else if (smenu == "GroupCEO")
                {
                    lbHeaderNotifyGroup.Text = "Group CEO";
                    btAdd.OnClientClick = "AddEmpToNotifyGroup('Actionevent.asmx/createGroupCEO',callGroupCEO)";
                }

            }
            else
            {
                string original_url = Server.UrlEncode(Context.Request.RawUrl);
                Response.Redirect("login.aspx?returnUrl=" + original_url);
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
                link.Attributes.CssStyle.Add("background-color", "#e6e6e8");

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

            }



        }

    }
}