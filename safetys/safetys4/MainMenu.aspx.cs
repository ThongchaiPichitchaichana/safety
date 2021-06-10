using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class MainMenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
                {

                    if (Request.QueryString.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.nopermission + "');", true);

                    }
                    setPermissionMenu();
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


        protected void LinkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("login.aspx");

        }

        protected void LinkLanguageTH_Click(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("lang");
            cookie.Value = "th";
            Response.Cookies.Add(cookie);

            Session["langShow"] = "<img src='template/img/language/th.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ไทย";
            Session["lang"] = "th";
            Session["name"] = Session["name_th"];	

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

            Response.Redirect(Request.RawUrl);

        }


        protected void LinkLanguageSI_Click(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("lang");
            cookie.Value = "si";
            Response.Cookies.Add(cookie);
            Session["langShow"] = "<img src='template/img/language/si.png'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English";
            Session["lang"] = "si";

            Response.Redirect(Request.RawUrl);

        }

        protected void btIncident_Click(object sender, EventArgs e)
        {


            Response.Redirect("incidentform.aspx?pagetype=add");

        }

        protected void btHazard_Click(object sender, EventArgs e)
        {

            Response.Redirect("hazardform.aspx?pagetype=add");

        }

        protected void btSot_Click(object sender, EventArgs e)
        {

            Response.Redirect("sotform.aspx?pagetype=add");

        }
        protected void btMyAction_Click(object sender, EventArgs e)
        {


            Response.Redirect("myactionincident.aspx");

        }


        protected void btAdmin_Click(object sender, EventArgs e)
        {


            getRedirectAdmin();

        }


        protected void btContractor_Click(object sender, EventArgs e)
        {

            Response.Redirect("contractor.aspx");

        }


        protected void btAllIncident_Click(object sender, EventArgs e)
        {

            Response.Redirect("allincident.aspx");

        }

        protected void btAllHazard_Click(object sender, EventArgs e)
        {

            Response.Redirect("allhazard.aspx");

        }


        protected void btAllHealth_Click(object sender, EventArgs e)
        {

            Response.Redirect("allhealth.aspx");

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


        protected void btHealth_Click(object sender, EventArgs e)
        {

            Response.Redirect("healthform.aspx?pagetype=add");

        }



        protected void setPermissionMenu()
        {
            ArrayList per = new ArrayList();

            per = Session["permission"] as ArrayList;

            if (per.IndexOf("admin") < 0)
            {
                btAdmin.Disabled = true;
                btAdmin.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
            }

            if (per.IndexOf("contractor") < 0)
            {

                btContractor.Disabled = true;
                btContractor.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
            }

            if (per.IndexOf("report") < 0)
            {

                btReport.Disabled = true;
                btReport.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
            }

            if (per.IndexOf("dashboard") < 0)
            {
                btDashboard.Disabled = true;
                btDashboard.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";

            }

            if (per.IndexOf("all hazards") < 0)
            {
                btAllHazard.Disabled = true;
                btAllHazard.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";

            }

            if (per.IndexOf("all incidents") < 0)
            {
                btAllIncident.Disabled = true;
                btAllIncident.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";

            }

            if (per.IndexOf("all sots") < 0)
            {
                //btAllSot.Disabled = true;
                //btAllSot.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                btAllSot.Visible = false;
                td_sot.Visible = false;

            }

            if (per.IndexOf("my action") < 0)
            {
                btMyAction.Disabled = true;
                btMyAction.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";

            }

            if (per.IndexOf("report sot") < 0)
            {              
                //btReportSOT.Disabled = true;
                //btReportSOT.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                btReportSOT.Visible = false;
            }

            if (per.IndexOf("report incident") < 0)
            {
                btReportIncident.Disabled = true;
                btReportIncident.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
            }
          

            if (per.IndexOf("report hazard") < 0)
            {
                btReportHazard.Disabled = true;
                btReportHazard.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
            }



            if (per.IndexOf("report health") < 0)
            {
                //btReportHealth.Disabled = true;
                //btReportHealth.Attributes.CssStyle[HtmlTextWriterStyle.Filter] = "grayscale(1)";
                btReportHealth.Visible = false;
                
            }


            if (per.IndexOf("all healths") < 0)
            {
                btAllHealth.Visible = false;
                td_health.Visible = false;
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


    }
}