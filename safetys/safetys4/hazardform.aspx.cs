using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Collections;
using safetys4.App_Code;

namespace safetys4
{
    public partial class hazardform : System.Web.UI.Page
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
                        bool result = Permission.checkPermision("report hazard", Session["permission"] as ArrayList);
                        if (!result)
                        {
                            Response.Redirect("MainMenu.aspx?msg_err=permision");
                        }
                       
                        setStepForm();

                        txtreport_date.Disabled = true;
                        txtname_surname.Disabled = true;

                       

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
           
            btHazardcheck.Style["visibility"] = "hidden";
            btHazardedit.Style["visibility"] = "hidden";
            btHazardreject.Style["visibility"] = "hidden";
            if (Session["lang"] !=null)
            {
                txtreport_date.Value = FormatDates.getDateNow(Session["lang"].ToString(), Session["timezone"].ToString());
            }
           
            setResumeHazard();
            lbshowimage.Style["visibility"] = "hidden";
            btUpdate.Style["visibility"] = "hidden";
            btReopenHazard.Style["visibility"] = "hidden";
          
           
            setInfoUser();

        }


        protected void setResumeHazard()
        {
            ///////////////////////กรณีคลิ๊กทำต่อ////////////////////////

            if (Session["resume_hazard_date"] != null)
            {
                txthazard_date.Value = Session["resume_hazard_date"].ToString();
                ddtimehour.Value = Session["resume_time_hour"].ToString();
                ddtimeminute.Value = Session["resume_time_minute"].ToString();
                txthazard_area.Value = Session["resume_hazard_area"].ToString();
                txtphone.Value = Session["resume_phone"].ToString();
                
            }
           
        }

        protected void setEdit()
        {
            lbshowimage.Style["visibility"] = "hidden";
            btHazardedit.Style["visibility"] = "hidden";
            btSubmit.Style["visibility"] = "hidden";
            btHazardcheck.Style["visibility"] = "hidden";
            btHazardreject.Style["visibility"] = "hidden";
            btReopenHazard.Style["visibility"] = "hidden";

        }

        protected void setView()
        {
            txthazard_date.Disabled = true;
            // txthazard_time.Disabled = true;
            ddtimehour.Disabled = true;
            ddtimeminute.Disabled = true;
            ddcompany.Disabled = true;
            ddfunction.Disabled = true;
            dddepartment.Disabled = true;
            dddivision.Disabled = true;
            ddsection.Disabled = true;
            ddhazardcharacteristic.Disabled = true;
            txthazard_area.Disabled = true;
            txthazardname.Disabled = true;
            txthazarddetail.Disabled = true;
            txtphone.Disabled = true;

            type_action1.Disabled = true;
            type_action2.Disabled = true;
            type_action3.Disabled = true;


            txtpreliminary_action.Disabled = true;

            btSubmit.Style["visibility"] = "hidden";
            //btCancel.Style["visibility"] = "hidden";
            btUpdate.Style["visibility"] = "hidden";


         


        }

        protected void setInfoUser()
        {
           
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            if (Session["typeLogin"] != null)
            {
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
                        txtname_surname.Value = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                        txtphone.Value = rc.phone;

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
                        txtname_surname.Value = rc.prefix + " " + rc.first_name + " " + rc.last_name;

                    }


                }
            }


        }



        protected void btHazardcheck_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string division_id = "";
            string section_id = "";

            var query = from c in dbConnect.hazards
                        where c.id == Convert.ToInt32(id)
                        select c;

            foreach (hazard rc in query)
            {
               
                if(rc.step_form < 2)
                {
                    rc.step_form = 2;
                    rc.hazard_flow = 2;
                }


                if (Session["group_id"] != null)
                {
                    rc.verify_report_form1 = Convert.ToInt32(Session["group_id"]);

                }

                division_id = rc.division_id;
                section_id = rc.section_id;

            }




            if (string.IsNullOrEmpty(division_id) || string.IsNullOrEmpty(section_id) || section_id == "00000000" || division_id == "00000000")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.select_division_section + "');", true);


            }
            else
            {
                dbConnect.SubmitChanges();
                Response.Redirect("hazardform2.aspx?pagetype=view&id=" + id);
            }
        }


        protected void btHazardedit_Click(object sender, EventArgs e)
        {

            id = Request.QueryString["id"];


            Response.Redirect("hazardform.aspx?pagetype=edit&id=" + id);
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
                step4.Enabled = false;
                step4.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                step4.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                setAdd();
            }
            else
            {
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
                    step4.Enabled = false;
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                    //if (!string.IsNullOrEmpty(reason_reject))
                    //{
                    //    btHazardreject.Style["visibility"] = "hidden";
                    //}

                }
                else if (step == 2)
                {

                    step3.Enabled = false;
                    step3.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step3.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                    step4.Enabled = false;
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";

                    //btHazardcheck.Style["visibility"] = "hidden";
                    //if (!string.IsNullOrEmpty(reason_reject))
                    //{
                    //    btHazardreject.Style["visibility"] = "hidden";
                    //}


                }
                else if (step == 3)
                {
                    step4.Enabled = false;
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Color] = "gray";
                    step4.Attributes.CssStyle[HtmlTextWriterStyle.Cursor] = "default";
                    //btHazardcheck.Style["visibility"] = "hidden";
                    //btHazardreject.Style["visibility"] = "hidden";
                }
                else if (step == 4)
                {
                    //btHazardcheck.Style["visibility"] = "hidden";
                    //btHazardreject.Style["visibility"] = "hidden";

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


                //if (status == 2)//close
                //{
                //    if (!(gr.IndexOf("Super Admin") > -1))//ไม่ใช่ super admin
                //    {
                //        btHazardedit.Style["visibility"] = "hidden";

                //    }

                //}


            }

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

        protected void btReopenHazard_Click(object sender, EventArgs e)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                id = Request.QueryString["id"];

                var query = from c in dbConnect.hazards
                            where c.id == Convert.ToInt32(id)
                            select c;


                foreach (hazard rc in query)
                {
                    if (rc.process_status == 2)
                    {//close
                        rc.process_status = 1; //on process              
                        rc.step_form = 3;
                        rc.hazard_flow = 3;
                        rc.request_close_form3 = null;
                        rc.edit_form3 = null;
                        

                        dbConnect.SubmitChanges();

                        var gr = from c in dbConnect.log_request_close_hazards
                                 where c.hazard_id == Convert.ToInt32(id)
                                 select c;
                        foreach (var a in gr)
                        {
                            a.status = "D";
                        }

                        dbConnect.SubmitChanges();

                    }
                    else if (rc.process_status == 3)//reject
                    {
                        rc.process_status = 1; //on process
                        rc.reason_reject = "";

                        dbConnect.SubmitChanges();
                    }

                }


            }
        }
    }
}