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
    public partial class healthform : System.Web.UI.Page
    {
        string PageType = "add";
        string id = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
                {
                    PageType = Request.QueryString["PageType"];


                    if (!IsPostBack)
                    {
                        

                        if (PageType == "add")
                        {
                            bool result = Permission.checkPermision("report health", Session["permission"] as ArrayList);
                            if (!result)
                            {
                                Response.Redirect("MainMenu.aspx?msg_err=permision");
                            }
                        }
                        else
                        {
                            bool result = Permission.checkPermision("report health1 view", Session["permission"] as ArrayList);
                            if (!result)
                            {
                                Response.Redirect("MainMenu.aspx?msg_err=permision");
                            }

                        }


                       
                     

                        setStepForm();

                        txtreport_date.Disabled = true;
           



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
                    objInsert.created = DateTime.Now;


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                }

            }
        }


        protected void setAdd()
        {

            btHealthedit.Style["visibility"] = "hidden";
            if (Session["lang"] != null)
            {
                txtreport_date.Value = FormatDates.getDateNow(Session["lang"].ToString(), Session["timezone"].ToString());
            }

           
            btUpdate.Style["visibility"] = "hidden";


        }


        protected void setEdit()
        {

            btHealthedit.Style["visibility"] = "hidden";
            btSubmit.Style["visibility"] = "hidden";
         
        }

        protected void setView()
        {
            ddyear.Disabled = true;
            ddcompany.Disabled = true;
            ddfunction.Disabled = true;
            dddepartment.Disabled = true;
            dddivision.Disabled = true;
            ddsection.Disabled = true;
            txtage.Disabled = true;
            txtservice_year.Disabled = true;
            ddhospital.Disabled = true;
            txtservice_year_current.Disabled = true;
            txtjob_type.Disabled = true;
            ddemployee.Disabled = true;
           // txtemployee_id.Disabled = true;
            txtemployee_name.Disabled = true;
            rdInsignificant.Disabled = true;
            rdSignificant.Disabled = true;
            btShowCreateRiskFactor.Disabled = true;
            btCreateOccupationalHealth.Disabled = true;
            txtdateofbirth.Disabled = true;
            txthiring_date.Disabled = true;
            //btCreateProcessAction.Disabled = true;
          


            btSubmit.Style["visibility"] = "hidden";
            //btCancel.Style["visibility"] = "hidden";
            btUpdate.Style["visibility"] = "hidden";





        }

     

        protected void btHealthedit_Click(object sender, EventArgs e)
        {

            id = Request.QueryString["id"];


            Response.Redirect("healthform.aspx?pagetype=edit&id=" + id);
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
                Session["occupational_health"] = null;
                Session["risk_factor"] = null;
               
                setAdd();
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
            PageType = Request.QueryString["PageType"];
            if (PageType.Equals("add"))
            {

                Response.Redirect("healthform.aspx?pagetype=add");
            }
            else
            {

                id = Request.QueryString["id"];
                Response.Redirect("healthform.aspx?pagetype=view&id=" + id);
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

      

    

        protected void step2_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];

            Response.Redirect("healthform2.aspx?pagetype=view&id=" + id);
        }

        protected void step3_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];

            Response.Redirect("healthform3.aspx?pagetype=view&id=" + id);
        }
    }
}