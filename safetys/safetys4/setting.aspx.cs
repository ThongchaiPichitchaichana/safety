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
    public partial class setting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
             if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
            {
                if (!IsPostBack)
                {
                    bool result = Permission.checkPermision("setting", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
                    }
                    setPermissionMenuSide();
                }


                Panel secondPanel;
                secondPanel = (Panel)Master.FindControl("PanelSetting");
                secondPanel.Visible = true;

                String smenu = Request.QueryString["smenu"];
                if(smenu == "" || smenu == null)
                {
                    smenu = "TypeOfEmployee";
                }

                LinkButton LinkButton1;
                LinkButton1 = (LinkButton)Master.FindControl("Link" + smenu);
                LinkButton1.Attributes.CssStyle.Add("background-color", "#e6e6e8");
               /* <asp:LinkButton ID="LinkTypeOfEmployee" runat="server" CausesValidation="False" Text="<%$ Resources:Setting, LinkTypeOfEmployee %>" OnClick="LinkTypeOfEmployee_Click" ></asp:LinkButton>
                    <asp:LinkButton ID="LinkNatureOfInjury" runat="server" CausesValidation="False" Text="<%$ Resources:Setting, LinkNatureOfInjury %>" OnClick="LinkNatureOfInjury_Click" ></asp:LinkButton>
                    <asp:LinkButton ID="LinkBodyPart" runat="server" CausesValidation="False" Text="<%$ Resources:Setting, LinkBodyPart %>" OnClick="LinkBodyPart_Click" ></asp:LinkButton>
                    <asp:LinkButton ID="LinkSeverityOfInjury" runat="server" CausesValidation="False" Text="<%$ Resources:Setting, LinkSeverityOfInjury %>" OnClick="LinkSeverityOfInjury_Click" ></asp:LinkButton>
                    <asp:LinkButton ID="LinkSourceOfHazard" runat="server" CausesValidation="False" Text="<%$ Resources:Setting, LinkSourceOfHazard %>" OnClick="LinkSourceOfHazard_Click" ></asp:LinkButton>
                    <asp:LinkButton ID="LinkSourceOfIncident" runat="server" CausesValidation="False" Text="<%$ Resources:Setting, LinkSourceOfIncident %>" OnClick="LinkSourceOfIncident_Click" ></asp:LinkButton>
                    <asp:LinkButton ID="LinkEventOrExposure" runat="server" CausesValidation="False" Text="<%$ Resources:Setting, LinkEventOrExposure %>" OnClick="LinkEventOrExposure_Click" ></asp:LinkButton>
                    <asp:LinkButton ID="LinkFPE" runat="server" CausesValidation="False" Text="<%$ Resources:Setting, LinkFPE %>" OnClick="LinkFPE_Click" ></asp:LinkButton>*/

                if(smenu == "TypeOfEmployee")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkTypeOfEmployee;
                    //btAddArea.OnClientClick = "AddSettingData('Actionevent.asmx/createSetting','TypeOfEmployee')";
                }
                else if (smenu == "NatureOfInjury")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkNatureOfInjury;
                    //btAdd.OnClientClick = "AddEmpToNotifyGroup('Actionevent.asmx/createLegalDepartment',callLegalDepartment)";
                }
                else if (smenu == "SeverityOfInjury")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkSeverityOfInjury;
                }
                else if (smenu == "BodyPart")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkBodyPart;
                }
                else if (smenu == "SourceOfHazard")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkSourceOfHazard;
                }
                else if (smenu == "SourceOfIncident")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkSourceOfIncident;
                }
                else if (smenu == "EventOrExposure")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkEventOrExposure;
                }
                else if (smenu == "FPE")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkFPE;
                }
                else if (smenu == "RiskFactorRelateToWork")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkRiskFactorRelateToWork;
                }
                else if (smenu == "OccupationalHealthWork")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkOccupationalHealthWork;
                }
                else if (smenu == "TypeOfControl")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkTypeOfControl;
                }
                else if (smenu == "Hospital")
                {
                    lbHeaderNotifyGroup.Text = Resources.Setting.LinkHospital;
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
                link.Attributes.CssStyle.Add("background-color", "#e6e6e8");

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

            }



        }

    }
}