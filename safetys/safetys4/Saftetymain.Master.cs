using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class Saftetymain : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
                {
                    setPermissionMenuTop();
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
                    objInsert.error_message = ex.ToString();
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();




                }

            }


          
        }


        protected void LinkLanguageTH_Click(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("lang");
            cookie.Value = "th";
            Response.Cookies.Add(cookie);

            Session["langShow"] = "<img src='template/img/language/th.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ไทย";
            Session["lang"] = "th";
            Session["name"] = Session["name_th"];

            Session["date_start_selected"] = "";
            Session["date_end_selected"] = "";

            Session["date_start_selected_incident"] = "";
            Session["date_end_selected_incident"] = "";

            Session["date_start_selected_health"] = "";
            Session["date_end_selected_health"] = "";

            Response.Redirect(Request.RawUrl);

        }

        protected void LinkLanguageEN_Click(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("lang");
            cookie.Value = "en";
            Response.Cookies.Add(cookie);
            Session["langShow"] = "<img src='template/img/language/gb.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English";
            Session["lang"] = "en";
            Session["name"] = Session["name_en"];


            Session["date_start_selected"] = "";
            Session["date_end_selected"] = "";

            Session["date_start_selected_incident"] = "";
            Session["date_end_selected_incident"] = "";

            Session["date_start_selected_health"] = "";
            Session["date_end_selected_health"] = "";

            Response.Redirect(Request.RawUrl);

        }


        protected void LinkLanguageSI_Click(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("lang");
            cookie.Value = "si";
            Response.Cookies.Add(cookie);
            Session["langShow"] = "<img src='template/img/language/si.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English";
            Session["lang"] = "si";


            Session["date_start_selected"] = "";
            Session["date_end_selected"] = "";

            Session["date_start_selected_incident"] = "";
            Session["date_end_selected_incident"] = "";

            Session["date_start_selected_health"] = "";
            Session["date_end_selected_health"] = "";

            Response.Redirect(Request.RawUrl);

        }


        protected void LinkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("login.aspx");

        }


        protected void btHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("MainMenu.aspx");
        }
        protected void btMyAction_Click(object sender, EventArgs e)
        {
            Response.Redirect("myactionincident.aspx");
        }

        protected void btAllIncident_Click(object sender, EventArgs e)
        {


            Response.Redirect("allincident.aspx");

        }

        protected void btAllHazard_Click(object sender, EventArgs e)
        {


            Response.Redirect("allhazard.aspx");

        }

        protected void btAllSot_Click(object sender, EventArgs e)
        {


            Response.Redirect("allsot.aspx");

        }

        protected void btDashboard_Click(object sender, EventArgs e)
        {


            Response.Redirect("dashboardincident.aspx");

        }

        protected void btReport_Click(object sender, EventArgs e)
        {


            Response.Redirect("IncidentReport.aspx");

        }

        protected void btContractor_Click(object sender, EventArgs e)
        {

            Response.Redirect("contractor.aspx");

        }

        protected void btAdmin_Click(object sender, EventArgs e)
        {


            getRedirectAdmin();

        }


        protected void btSuperAdminSide_Click(object sender, EventArgs e)
        {


            Response.Redirect("superadmin.aspx");

        }


        protected void btAreaManagementSide_Click(object sender, EventArgs e)
        {


            Response.Redirect("areamanagement.aspx");

        }

        protected void btNotifyGroupSide_Click(object sender, EventArgs e)
        {
            Response.Redirect("notifygroup.aspx?smenu=GroupCommunicationVP");
        }

        protected void btHolidaySide_Click(object sender, EventArgs e)
        {
            Response.Redirect("holiday.aspx");
        }

        protected void btSettingSide_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=TypeOfEmployee");
        }

        protected void btTargetSide_Click(object sender, EventArgs e)
        {

            if (Session["country"].ToString() == "thailand")
            {
                Response.Redirect("target.aspx");
            }
            else if (Session["country"].ToString() == "srilanka")
            {
                Response.Redirect("target2.aspx");
            }
            
        }

        protected void btWorkHourSide_Click(object sender, EventArgs e)
        {
            if (Session["country"].ToString() == "thailand")
            {
                Response.Redirect("workhour.aspx");
            }
            else if (Session["country"].ToString() == "srilanka")
            {
                Response.Redirect("workhour2.aspx");
            }
           
        }

        protected void LinkGroupCommunicationVP_Click(object sender, EventArgs e)
        {
            Response.Redirect("notifygroup.aspx?smenu=GroupCommunicationVP");
        }

        protected void LinkLegalDepartment_Click(object sender, EventArgs e)
        {
            Response.Redirect("notifygroup.aspx?smenu=LegalDepartment");
        }

        protected void LinkGroupOHS_Click(object sender, EventArgs e)
        {
            Response.Redirect("notifygroup.aspx?smenu=GroupOHS");
        }

        protected void LinkGroupOHSHazard_Click(object sender, EventArgs e)
        {
            Response.Redirect("notifygroup.aspx?smenu=GroupOHSHazard");
        }

        protected void LinkTypeOfEmployee_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=TypeOfEmployee");
        }

        protected void LinkNatureOfInjury_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=NatureOfInjury");
        }

        protected void LinkBodyPart_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=BodyPart");
        }

        protected void LinkSeverityOfInjury_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=SeverityOfInjury");
        }

        protected void LinkSourceOfHazard_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=SourceOfHazard");
        }

        protected void LinkSourceOfIncident_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=SourceOfIncident");
        }

        protected void LinkEventOrExposure_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=EventOrExposure");
        }

        protected void LinkFPE_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=FPE");
        }

        protected void btMyActionIncidentSide_Click(object sender, EventArgs e)
        {
            Response.Redirect("myactionincident.aspx");
        }

        protected void btMyActionHazardSide_Click(object sender, EventArgs e)
        {
            Response.Redirect("myactionhazard.aspx");
        }

        protected void btMyActionSotSide_Click(object sender, EventArgs e)
        {
            Response.Redirect("myactionsot.aspx");
        }


        protected void btAllactionReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("AllactionReport.aspx");
        }

        protected void btAllSOTReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("SOTReport.aspx");
        }

        protected void LinkGroupOHSHealth_Click(object sender, EventArgs e)
        {
            Response.Redirect("notifygroup.aspx?smenu=GroupOHSHealth");
         
        }

        protected void LinkGroupEXCO_Click(object sender, EventArgs e)
        {
            Response.Redirect("notifygroup.aspx?smenu=GroupEXCO");

        }


        protected void LinkGroupCEO_Click(object sender, EventArgs e)
        {
            Response.Redirect("notifygroup.aspx?smenu=GroupCEO");

        }


        protected void LinkRiskFactorRelateToWork_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=RiskFactorRelateToWork");
        }

        protected void LinkOccupationalHealthWork_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=OccupationalHealthWork");
        }

        protected void LinkTypeOfControl_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=TypeOfControl");
        }


        protected void LinkHospital_Click(object sender, EventArgs e)
        {
            Response.Redirect("setting.aspx?smenu=Hospital");
        }




        protected void setPermissionMenuTop()
        {
            ArrayList per = new ArrayList();

            per = Session["permission"] as ArrayList;

            if (per.IndexOf("admin") < 0)
            {
                btAdmin.Enabled= false;
                btAdmin.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                btAdmin.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "not-allowed";
                btAdmin.Attributes.CssStyle[HtmlTextWriterStyle.Color] = " #f2f3f4 ";
            }

            if (per.IndexOf("contractor") < 0)
            {

                btContractor.Enabled = false;
                btContractor.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                btContractor.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "not-allowed";
                btContractor.Attributes.CssStyle[HtmlTextWriterStyle.Color] = " #f2f3f4 ";
            }

            if (per.IndexOf("report") < 0)
            {

                btReport.Enabled = false;
                btReport.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                btReport.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "not-allowed";
                btReport.Attributes.CssStyle[HtmlTextWriterStyle.Color] = " #f2f3f4 ";
            }

            if (per.IndexOf("dashboard") < 0)
            {
                btDashboard.Enabled = false;
                btDashboard.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                btDashboard.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "not-allowed";
                btDashboard.Attributes.CssStyle[HtmlTextWriterStyle.Color] = " #f2f3f4 ";

            }

            if (per.IndexOf("all hazards") < 0)
            {
                btAllHazard.Enabled = false;
                btAllHazard.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                btAllHazard.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "not-allowed";
                btAllHazard.Attributes.CssStyle[HtmlTextWriterStyle.Color] = " #f2f3f4 ";

            }

            if (per.IndexOf("all incidents") < 0)
            {
                btAllIncident.Enabled = false;
                btAllIncident.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                btAllIncident.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "not-allowed";
                btAllIncident.Attributes.CssStyle[HtmlTextWriterStyle.Color] = " #f2f3f4 ";

            }

            if (per.IndexOf("all sots") < 0)
            {
                //btAllSot.Enabled = false;
                //btAllSot.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                //btAllSot.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "not-allowed";
                //btAllSot.Attributes.CssStyle[HtmlTextWriterStyle.Color] = " #f2f3f4 ";
                btAllSot.Visible = false;
                bar_allsot.Visible = false;

            }

            if (per.IndexOf("my action") < 0)
            {
                btMyAction.Enabled = false;
                btMyAction.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                btMyAction.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "not-allowed";
                btMyAction.Attributes.CssStyle[HtmlTextWriterStyle.Color] = " #f2f3f4 ";

            }


            if (per.IndexOf("all healths") < 0)
            {
                //btMyAction.Enabled = false;
                //btMyAction.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                //btMyAction.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "not-allowed";
                //btMyAction.Attributes.CssStyle[HtmlTextWriterStyle.Color] = " #f2f3f4 ";
                btAllHealth.Visible = false;
                bar_allhealth.Visible = false;

            }




        }



        protected void getRedirectAdmin()
        {
            ArrayList per = new ArrayList();

            per = Session["permission"] as ArrayList;

            if (per.IndexOf("super admin") > -1)
            {
                Response.Redirect("superadmin.aspx");

            }

            if (per.IndexOf("holiday") > -1)
            {
                Response.Redirect("holiday.aspx");

            }

            if (per.IndexOf("area management") > -1)
            {
                Response.Redirect("areamanagement.aspx");

            }

            if (per.IndexOf("setting") > -1)
            {
                Response.Redirect("setting.aspx");

            }

            if (per.IndexOf("notify group") > -1)
            {
                Response.Redirect("notifygroup.aspx");

            }


            if (per.IndexOf("target") > -1)
            {
                Response.Redirect("target.aspx");

            }

            if (per.IndexOf("work hour") > -1)
            {
                Response.Redirect("workhour.aspx");

            }



        }

        protected void btDashboardHazardSide_Click(object sender, EventArgs e)
        {
            Response.Redirect("dashboardhazard.aspx");
            
        }

        protected void btDashboardSotSide_Click(object sender, EventArgs e)
        {
            Response.Redirect("dashboardsot.aspx");

        }

        protected void btDashboardIncidentSide_Click(object sender, EventArgs e)
        {
            Response.Redirect("dashboardincident.aspx");
        }

        protected void btAllincidentReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("IncidentReport.aspx");
        }

        protected void btAllhazardReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("HazardReport.aspx");
        }


        protected void btFaltalityRateReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("FaltalityRateReport.aspx");
        }

        
        protected void btTotalInjuryReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("TotalinjuryFPEReport.aspx");
        }

        protected void btPyramidReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("RootCauseIncident.aspx");
        }

        protected void btLTIFRReport_Click(object sender, EventArgs e)
        {

            if (Session["country"].ToString() == "thailand")
            {
                Response.Redirect("LTIFRReport.aspx");
            }
            else if (Session["country"].ToString() == "srilanka")
            {
                Response.Redirect("LTIFRSrilankaReport.aspx");
            }
            
        }

        protected void btTIFRReport_Click(object sender, EventArgs e)
        {
            if (Session["country"].ToString() == "thailand")
            {
                Response.Redirect("TIFRReport.aspx");
            }
            else if (Session["country"].ToString() == "srilanka")
            {
                Response.Redirect("TIFRSrilankaReport.aspx");
            }
            
        }

        protected void btSourceHazard_Click(object sender, EventArgs e)
        {
            Response.Redirect("SourcehazardReport.aspx");
        }

        protected void btTrainingHourReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("TrainingHourReport.aspx");
        }

        protected void btAllHealth_Click(object sender, EventArgs e)
        {
            Response.Redirect("allhealth.aspx");
        }

        protected void btAllHealthReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("HealthReport.aspx");
        }


        protected void btHealthIndividualReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("HealthIndividualReport.aspx");
        }

    

       
     

    }
}