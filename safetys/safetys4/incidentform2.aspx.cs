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
    public partial class incidentform2 : System.Web.UI.Page
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

                    bool result = Permission.checkPermision("report incident2 view", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
                    }

                    setStepForm();

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
            
            btIncidentedit.Style["visibility"] = "hidden";
            btIncidentreject.Style["visibility"] = "hidden";
            btUpdateReport.Style["visibility"] = "hidden";
            btIncidentconfirm.Style["visibility"] = "hidden";
            btIncidentconfirmgroup.Style["visibility"] = "hidden";
           
        }

        protected void setView()
        {
          
            //txtphone.Disabled = true;
            //work_relate1.Disabled = true;
            //work_relate2.Disabled = true;

            if (Session["country"].ToString() == "srilanka")
            {
                responsible_area1.Disabled = true;
                responsible_area2.Disabled = true;
            }

            impact1.Disabled = true;
            impact2.Disabled = true;

            injury_fatality_involve1.Disabled = true;
            injury_fatality_involve2.Disabled = true;

            effect_environment1.Disabled = true;
            effect_environment2.Disabled = true;

            //ddLeveldamange.Disabled = true;
           // ddLevelenvironment.Disabled = true;

            other_impact1.Disabled = true;
            other_impact2.Disabled = true;
            other_impact3.Disabled = true;

            critical1.Disabled = true;
            critical2.Disabled = true;

            external_reportable1.Disabled = true;
            external_reportable2.Disabled = true;

            txtimmediate_temporary.Disabled = true;

            ddConsequencelevel.Disabled = true;
            ddCurrency.Disabled = true;
           
            btUpdate.Style["visibility"] = "hidden";
            btAddEffect.Disabled = true;
            btAddInjury.Disabled = true;



        }

        protected void btIncidentedit_Click(object sender, EventArgs e)
        {

            id = Request.QueryString["id"];
            Response.Redirect("incidentform2.aspx?pagetype=edit&id=" + id);
        }

     

        protected void btIncidentconfirm_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();




            if (Session["country"].ToString() == "thailand")
            {

                bool sendemail_injury = false;

                string[] alert_to_groups = new string[2];

                bool result = checkconfirmbygroup();

                var query = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (incident rc in query)
                {


                    if (Session["group_id"] != null)
                    {
                        rc.confirm_investigate_form2 = Convert.ToInt32(Session["group_id"]);

                    }


                    if (rc.injury_fatality_involve == "Y")
                    {
                        alert_to_groups[0] = "AreaSuperervisor";
                        sendemail_injury = true;
                    }




                    if (!result)//ไม่มีเคสส่งหา sml,tml,legal,corncomm
                    {
                        if (rc.step_form < 3)
                        {
                            rc.step_form = 3;
                            rc.incident_flow = 3;
                            rc.confirm_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        }

                    }
                    else
                    {
                        //ส่งเมล์หา group เพื่อแจ้งให้มา confirm กรณ๊มีเคส critical
                        ////////////////////////////////by p.poo sent notification/////////////////////////////////               
                        //  alert_to_groups[1] = "AdminOH&S";
                        alert_to_groups[1] = "GroupOH&S";
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        sn.InsertNotification(15, Convert.ToInt32(id), alert_to_groups, Session["timezone"].ToString(), "GroupOH&S");//รอใส่ template
                        //////////////////////////////////////////////////////////////////////////////////////////////////

                    }


                    if (sendemail_injury == true)//ถ้ามีผู้บาดเจ็บส่งหาแค่ areasupervisor
                    {
                        //////////////////////////////////by p.poo sent notification///////////////////////////////// 
                        string[] alert_to_groups2 = new string[1];
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        sn.InsertNotification(3, Convert.ToInt32(id), alert_to_groups2, Session["timezone"].ToString(),"");//รอใส่ template
                        //////////////////////////////////////////////////////////////////////////////////////////////////
                    }


                }


                dbConnect.SubmitChanges();

                if (!result)//ไม่มีเคสส่งหา sml,tml,legal,corncomm
                {
                    Response.Redirect("incidentform3.aspx?pagetype=view&id=" + id);

                }
                else
                {
                    Response.Redirect("incidentform2.aspx?pagetype=view&id=" + id);
                }
           


            }else if(Session["country"].ToString() == "srilanka")
            {
                bool sendemail = false;
                bool sml_tml = false;
                bool critical = false;

                bool sendemail_injury = false;

                string[] alert_to_groups = new string[7];

                bool result = checkconfirmbygroup();

                var query = from c in dbConnect.incidents
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (incident rc in query)
                {


                    if (rc.injury_fatality_involve == "Y")
                    {
                        //alert_to_groups[4] = "AreaSuperervisor";
                        sendemail_injury = true;
                    }




                    if (!result)//ไม่มีเคสส่งหา sml,tml,legal,corncomm
                    {
                        if (rc.step_form < 3)
                        {
                            rc.step_form = 3;
                            rc.incident_flow = 3;
                            rc.confirm_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                        }

                    }
                    else
                    {//
                        if (Session["group_id"] != null)
                        {
                            rc.confirm_investigate_form2 = Convert.ToInt32(Session["group_id"]);

                        }

                        if (rc.step_form < 3)
                        {
                            rc.step_form = 3;
                            rc.incident_flow = 3;
                            rc.confirm_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        }
       
                        var impacts = from c in dbConnect.other_impacts
                                      where c.incident_id == Convert.ToInt32(id)
                                      select c;

                        foreach (var v in impacts)
                        {

                            if (v.other_impact_value == "image")
                            {
                                alert_to_groups[0] = "GroupCommunicationVP";
                                sendemail = true;

                            }



                            if (v.other_impact_value == "legal")
                            {
                                alert_to_groups[1] = "LegalDepartment";
                                sendemail = true;
                            }


                        }//end foreach impact

                        var levels = from c in dbConnect.level_has_definition_level_incidents
                                     where c.incident_id == Convert.ToInt32(id)
                                     select c;

                        foreach (var v in levels)
                        {

                            //safety จากตาราง form2 incident
                            if (v.definition_level_incident_id == 1 && v.level_incident == 1)
                            {
                                sml_tml = true;
                            }

                            if (v.definition_level_incident_id == 2 && (v.level_incident == 1 || v.level_incident == 2 || v.level_incident == 3))
                            {
                                sml_tml = true;
                            }

                            if (v.definition_level_incident_id == 3 && v.level_incident == 1)
                            {
                                sml_tml = true;
                            }


                            if (v.definition_level_incident_id == 4 && (v.level_incident == 1 || v.level_incident == 2))
                            {
                                sml_tml = true;
                            }


                            if (v.definition_level_incident_id == 7 && v.level_incident == 1)
                            {
                                sml_tml = true;
                            }

                        }


                        if (rc.critical == "Y")//เป็นอุบัติการณ์ขั้นวิกฤต หรือเปล่า
                        {
                            critical = true;

                        }



                        if (critical == true || sml_tml == true)
                        {
                            alert_to_groups[2] = "SML";
                            alert_to_groups[3] = "TML";
                            sendemail = true;

                        }


                        if (sendemail == true)
                        {
                            ////////////////////////////////by p.poo sent notification/////////////////////////////////               
                            Class.SafetyNotification sn = new Class.SafetyNotification();
                            alert_to_groups[5] = "AdminOH&S";
                            alert_to_groups[6] = "GroupOH&S";                          
                            sn.InsertNotification(16, Convert.ToInt32(id), alert_to_groups, Session["timezone"].ToString(),"");
                            //////////////////////////////////////////////////////////////////////////////////////////////////
                        }



                    }


                    if (sendemail_injury == true)//ถ้ามีผู้บาดเจ็บส่งหาแค่ areasupervisor
                    {
                        //////////////////////////////////by p.poo sent notification///////////////////////////////// 
                        Class.SafetyNotification sn = new Class.SafetyNotification();
                        string[] alert_to_groups2 = new string[1];
                        sn.InsertNotification(3, Convert.ToInt32(id), alert_to_groups2, Session["timezone"].ToString(),"");//รอใส่ template
                        //////////////////////////////////////////////////////////////////////////////////////////////////
                    }


                }//end each


                dbConnect.SubmitChanges();

                if (!result)//ไม่มีเคสส่งหา sml,tml,legal,corncomm
                {
                    Response.Redirect("incidentform3.aspx?pagetype=view&id=" + id);

                }
                else
                {
                    Response.Redirect("incidentform2.aspx?pagetype=view&id=" + id);
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
            var query = from c in dbConnect.incidents
                        where c.id == Convert.ToInt32(id)
                        select c;

            foreach (incident rc in query)
            {
                step = Convert.ToByte(rc.step_form);
                reason_reject = rc.reason_reject;
              
                status_form = rc.process_status;
            }


            if (step < 2)
            {
                Response.Redirect("incidentform" + step + ".aspx?pagetype=view&id=" + id);
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
                //        btIncidentedit.Style["visibility"] = "hidden";

                //    }

                //}


            }
          
        }
        protected void step1_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform.aspx?pagetype=view&id=" + id);
        }

        protected void step2_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform2.aspx?pagetype=view&id=" + id);
        }

        protected void step3_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform3.aspx?pagetype=view&id=" + id);
        }

        protected void step4_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform4.aspx?pagetype=view&id=" + id);
        }

        protected void btIncidentconfirmgroup_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            bool sendemail = false;
            bool sml_tml = false;
            bool critical = false;

            string[] alert_to_groups = new string[9];
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var query = from c in dbConnect.incidents
                        where c.id == Convert.ToInt32(id)
                        select c;

            foreach (incident rc in query)
            {
                if (rc.step_form < 3)
                {
                    rc.step_form = 3;
                    rc.incident_flow = 3;
                    rc.confirm_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                }




                if (Session["group_id"] != null)
                {
                    rc.confirm_by_groupohs_form2 = Convert.ToInt32(Session["group_id"]);

                }


                var impacts = from c in dbConnect.other_impacts
                              where c.incident_id == Convert.ToInt32(id)
                              select c;

                foreach (var v in impacts)
                {

                    if (v.other_impact_value == "image")
                    {
                        alert_to_groups[0] = "GroupCommunicationVP";
                        sendemail = true;

                    }



                    if (v.other_impact_value == "legal")
                    {
                        alert_to_groups[1] = "LegalDepartment";
                        sendemail = true;
                    }


                }//end foreach impact

                var levels = from c in dbConnect.level_has_definition_level_incidents
                             where c.incident_id == Convert.ToInt32(id)
                             select c;

                foreach (var v in levels)
                {
                   
                    //safety จากตาราง form2 incident
                    if (v.definition_level_incident_id == 1 && v.level_incident == 1)
                    {
                        sml_tml = true;
                    }

                    if (v.definition_level_incident_id == 2 && (v.level_incident == 1 || v.level_incident == 2 || v.level_incident == 3))
                    {
                        sml_tml = true;
                        alert_to_groups[8] = "CEO";
                    }

                    if (v.definition_level_incident_id == 3 && v.level_incident == 1)
                    {
                        sml_tml = true;
                    }


                    if (v.definition_level_incident_id == 4 && (v.level_incident == 1 || v.level_incident == 2))
                    {
                        sml_tml = true;
                    }


                    if (v.definition_level_incident_id == 7 && v.level_incident == 1)
                    {
                        sml_tml = true;
                    }

                }

                //int LTI = 3;
                //int PD = 2;
                //int F = 1;
                //int RWC = 6;


                // var injurury = from c in dbConnect.injury_persons
                //             where c.incident_id == Convert.ToInt32(id)
                //             select c;

                // foreach (var v in injurury)
                // {
                //     if(v.severity_injury_id == LTI || v.severity_injury_id == PD || v.severity_injury_id == F || v.severity_injury_id == RWC)
                //     {
                //         sml_tml = true;
                //     }

                // }


                if (rc.critical == "Y")//เป็นอุบัติการณ์ขั้นวิกฤต หรือเปล่า
                {
                    critical = true;
                    alert_to_groups[8] = "CEO";
                   
                }



                if (critical == true || sml_tml == true)
                {
                    alert_to_groups[2] = "SML";
                    alert_to_groups[3] = "TML";
                    sendemail = true;

                }


            }//end foreach incident


            dbConnect.SubmitChanges();




            if (sendemail == true)
            {
                ////////////////////////////////by p.poo sent notification/////////////////////////////////               
                alert_to_groups[5] = "AdminOH&S";
                alert_to_groups[6] = "GroupOH&S";
                alert_to_groups[7] = "TML-EXCO";
                Class.SafetyNotification sn = new Class.SafetyNotification();
                sn.InsertNotification(16, Convert.ToInt32(id), alert_to_groups, Session["timezone"].ToString(),"");
                //////////////////////////////////////////////////////////////////////////////////////////////////

                ////////////////////////////////by p.poo sent notification/////////////////////////////////
                /////////////////ส่งกลับไปบอก area oh&s ว่า group กดไปแล้ว///////////////////////////////////////
                Class.SafetyNotification sn2 = new Class.SafetyNotification();
                if(Session["country"].ToString()=="thailand")
                {
                    string[] alert_to_groups2 = { "AreaOH&S" };
                    sn2.InsertNotification(17, Convert.ToInt32(id), alert_to_groups2, Session["timezone"].ToString(), "AreaOH&S");
                }
              
               
                //////////////////////////////////////////////////////////////////////////////////////////////////
            }
         

            // ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.success + "');", true);
            Response.Redirect("incidentform3.aspx?pagetype=view&id=" + id);

           
        }

        protected void setBtconfirmgroup()
        {
            bool show_bt = checkconfirmbygroup();
            if (!show_bt)
            {
                btIncidentconfirmgroup.Style["visibility"] = "hidden";
            }

        }

        protected bool checkconfirmbygroup()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
          
            bool show_bt = false;

            string id = Request.QueryString["id"];
            var query = from c in dbConnect.incidents
                        where c.id == Convert.ToInt32(id)
                        select c;

            foreach (incident rc in query)
            {
                if (rc.critical == "Y")//เป็นอุบัติการณ์ขั้นวิกฤต
                {
                    show_bt = true;
                }

            }

            var impacts = from c in dbConnect.other_impacts
                          where c.incident_id == Convert.ToInt32(id)
                          select c;

            foreach (var v in impacts)
            {

                if (v.other_impact_value == "image")
                {
                    show_bt = true;

                }



                if (v.other_impact_value == "legal")
                {
                    show_bt = true;
                }


            }//end foreach impact

            var levels = from c in dbConnect.level_has_definition_level_incidents
                         where c.incident_id == Convert.ToInt32(id)
                         select c;

            foreach (var v in levels)
            {
                //safety จากตาราง form2 incident
                if (v.definition_level_incident_id == 1 && v.level_incident == 1)
                {
                    show_bt = true;
                }

                if (v.definition_level_incident_id == 2 && (v.level_incident == 1 || v.level_incident == 2 || v.level_incident == 3))
                {
                    show_bt = true;
                }

                if (v.definition_level_incident_id == 3 && v.level_incident == 1)
                {
                    show_bt = true;
                }


                if (v.definition_level_incident_id == 4 && (v.level_incident == 1 || v.level_incident == 2))
                {
                    show_bt = true;
                }


                if (v.definition_level_incident_id == 7 && v.level_incident == 1)
                {
                    show_bt = true;
                }


            }//end foreach level


           
            return show_bt;
           
        }
    }
}