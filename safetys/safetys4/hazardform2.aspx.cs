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
    public partial class hazardform2 : System.Web.UI.Page
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

                    bool result = Permission.checkPermision("report hazard2 view", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
                    }

                    setStepForm();


                    txtname_security.Disabled = true;
                    txtverifydate.Disabled = true;


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
            btHazardedit.Style["visibility"] = "hidden";
            btHazardreject.Style["visibility"] = "hidden";
            btUpdateReport.Style["visibility"] = "hidden";
            btHazardprocessaction.Style["visibility"] = "hidden";
            
        }

        protected void setView()
        {
           
            txtname_security.Disabled = true;
            ddsourcehazard.Disabled = true;
            level_hazard1.Disabled = true;
            level_hazard2.Disabled = true;
            level_hazard3.Disabled = true;
            ddfatality.Disabled = true;
            txtother_please.Disabled = true;

            btUpdate.Style["visibility"] = "hidden";

        }

        protected void setInfoUser()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            string type_login = Session["typeLogin"].ToString();

            if (type_login == "contractor")
            {
                var v = from c in dbConnect.contractors
                        where c.id == Convert.ToInt32(Session["user_id"])
                        select new
                        {
                            prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                            first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                            last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                            phone = c.phone

                        };


                foreach (var rc in v)
                {
                    txtname_security.Value = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                   

                }

            }
            else
            {
                var v = from c in dbConnect.employees
                        where c.employee_id == Session["user_id"].ToString()
                        select new
                        {
                            prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                            first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                            last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),


                        };


                foreach (var rc in v)
                {
                    txtname_security.Value = rc.prefix + " " + rc.first_name + " " + rc.last_name;

                }


            }


        }


        protected void setStepForm()
        {

            id = Request.QueryString["id"];

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            byte step = 0;
            int status_form = 1;
            string reason_reject = "";
            var query = from c in dbConnect.hazards
                        where c.id == Convert.ToInt32(id)
                        select c;

            foreach (hazard rc in query)
            {
                step = Convert.ToByte(rc.step_form);
                reason_reject = rc.reason_reject;
                //if (rc.submit_report_form != null)
                //{
                //    btUpdateReport.Style["visibility"] = "hidden";

                //}
                status_form = rc.process_status;
            }

            if (step < 2)
            {
                Response.Redirect("hazardform" + step + ".aspx?pagetype=view&id=" + id);
            }
            else
            {

                if (step == 2)
                {
                    step3.Enabled = false;
                    step3.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step3.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                    step4.Enabled = false;
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";


                }
                else if (step == 3)
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
                //        btHazardedit.Style["visibility"] = "hidden";

                //    }

                //}


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

        protected void btHazardedit_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];


            Response.Redirect("hazardform2.aspx?pagetype=edit&id=" + id);
        }

       

        protected void btHazardprocessaction_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var query = from c in dbConnect.hazards
                        where c.id == Convert.ToInt32(id)
                        select c;

            foreach (hazard rc in query)
            {
             
                if (rc.step_form < 3)
                {
                    rc.step_form = 3;
                    rc.hazard_flow = 3;
                    rc.confirm_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                  
                }

                if (Session["group_id"] != null)
                {
                    rc.process_action_form2 = Convert.ToInt32(Session["group_id"]);

                }
            }

            dbConnect.SubmitChanges();
            Response.Redirect("hazardform3.aspx?pagetype=view&id=" + id);
        }

        protected void step1_Click(object sender, EventArgs e)
        {
            PageType = Request.QueryString["PageType"];
            if (PageType.Equals("add"))
            {

                Response.Redirect("hazardform.aspx?pagetype=add");
            }
            else
            {

                id = Request.QueryString["id"];
                Response.Redirect("hazardform.aspx?pagetype=view&id=" + id);
            }
        }

        protected void step2_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];

            Response.Redirect("hazardform2.aspx?pagetype=view&id=" + id);
        }

        protected void step3_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];

            Response.Redirect("hazardform3.aspx?pagetype=view&id=" + id);
        }

        protected void step4_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];

            Response.Redirect("hazardform4.aspx?pagetype=view&id=" + id);
        }

     
    }
}