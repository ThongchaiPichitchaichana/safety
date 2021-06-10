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
    public partial class Sotform : System.Web.UI.Page
    {
        string PageType = "add";
        string id = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
                {
                    if (!IsPostBack)
                    {

                        //bool result = Permission.checkPermision("report sot", Session["permission"] as ArrayList);
                        //if (!result)
                        //{
                        //    Response.Redirect("MainMenu.aspx?msg_err=permision");
                        //}
                        PageType = Request.QueryString["PageType"];

                        if (PageType.Equals("view"))
                        {
                            setView();

                        }
                        else if (PageType.Equals("add"))
                        {
                            Session["process_action_sot"] = null;
                            if (Session["lang"] != null)
                            {

                                txtreport_date.Value = FormatDates.getDateNow(Session["lang"].ToString(), Session["timezone"].ToString());
                            }

                            btSotformedit.Style["visibility"] = "hidden";
                            btSubmit.Style["visibility"] = "hidden";
                        }
                        else
                        {
                            btSotformedit.Style["visibility"] = "hidden";
                            btSubmit.Style["visibility"] = "hidden";
                            
                        }


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

               
            }


        }


        protected void setView()
        {
            
            ddtimehour.Disabled = true;
           // ddtimeminute.Disabled = true;
            ddcompany.Disabled = true;
            ddfunction.Disabled = true;
            dddepartment.Disabled = true;
            dddivision.Disabled = true;
            //ddsite.Disabled = true;
           // ddcountry.Disabled = true;
            ddtimehour2.Disabled = true;
            //ddtimeminute2.Disabled = true;
            txtsot_date.Disabled = true;
            txtlocation.Disabled = true;
            txttypework.Disabled = true;
            //txtobservation.Disabled = true;
            txtsotteam.Disabled = true;
            btCreateProcessAction.Disabled = true;
            txtcomment.Disabled = true;
            ddlTypeemployment.Disabled = true;

            btUpdate.Style["visibility"] = "hidden";



            txtreactions_people.Disabled = true;
            changing_position_risk.Disabled = true;
            changing_position_safe.Disabled = true;
            changing_position_na.Disabled = true;

            stopping_work_risk.Disabled = true;
            stopping_work_safe.Disabled = true;
            stopping_work_na.Disabled = true;

            rearranging_job_risk.Disabled = true;
            rearranging_job_safe.Disabled = true;
            rearranging_job_na.Disabled = true;

            hiding_dodging_risk.Disabled = true;
            hiding_dodging_safe.Disabled = true;
            hiding_dodging_na.Disabled = true;

            changing_tools_risk.Disabled = true;
            changing_tools_safe.Disabled = true;
            changing_tools_na.Disabled = true;

            applying_lockout_risk.Disabled = true;
            applying_lockout_safe.Disabled = true;
            applying_lockout_na.Disabled = true;

            adjusting_ppe_risk.Disabled = true;
            adjusting_ppe_safe.Disabled = true;
            adjusting_ppe_na.Disabled = true;



            striking_against_risk.Disabled = true;
            striking_against_safe.Disabled = true;
            striking_against_na.Disabled = true;

            caught_between_risk.Disabled = true;
            caught_between_safe.Disabled = true;
            caught_between_na.Disabled = true;

            inhaling_na.Disabled = true;
            inhaling_risk.Disabled = true;
            inhaling_safe.Disabled = true;

            absorbing_na.Disabled = true;
            absorbing_risk.Disabled = true;
            absorbing_safe.Disabled = true;

            electricity_na.Disabled = true;
            electricity_risk.Disabled = true;
            electricity_safe.Disabled = true;

            falling_na.Disabled = true;
            falling_risk.Disabled = true;
            falling_safe.Disabled = true;

            struck_by_na.Disabled = true;
            struck_by_risk.Disabled = true;
            struck_by_safe.Disabled = true;

            line_fire_by_na.Disabled = true;
            line_fire_by_risk.Disabled = true;
            line_fire_by_safe.Disabled = true;

            eyes_tasks_by_na.Disabled = true;
            eyes_tasks_by_risk.Disabled = true;
            eyes_tasks_by_safe.Disabled = true;

            lifting_lowering_by_na.Disabled = true;
            lifting_lowering_by_risk.Disabled = true;
            lifting_lowering_by_safe.Disabled = true;

            posture_by_na.Disabled = true;
            posture_by_risk.Disabled = true;
            posture_by_safe.Disabled = true;

            



            head_na.Disabled = true;
            head_risk.Disabled = true;
            head_safe.Disabled = true;

            ears_eyes_na.Disabled = true;
            ears_eyes_risk.Disabled = true;
            ears_eyes_safe.Disabled = true;

            face_respiratory_na.Disabled = true;
            face_respiratory_risk.Disabled = true;
            face_respiratory_safe.Disabled = true;

            hands_arms_na.Disabled = true;
            hands_arms_risk.Disabled = true;
            hands_arms_safe.Disabled = true;

            feet_legs_na.Disabled = true;
            feet_legs_risk.Disabled = true;
            feet_legs_safe.Disabled = true;



            right_job_na.Disabled = true;
            right_job_risk.Disabled = true;
            right_job_safe.Disabled = true;

            used_correctly_na.Disabled = true;
            used_correctly_risk.Disabled = true;
            used_correctly_safe.Disabled = true;

            in_safe_conditions_na.Disabled = true;
            in_safe_conditions_risk.Disabled = true;
            in_safe_conditions_safe.Disabled = true;

            hamesses_na.Disabled = true;
            hamesses_safe.Disabled = true;
            hamesses_risk.Disabled = true;

            barricade_warning_lights_na.Disabled = true;
            barricade_warning_lights_risk.Disabled = true;
            barricade_warning_lights_safe.Disabled = true;

            chocks_restraints_na.Disabled = true;
            chocks_restraints_risk.Disabled = true;
            chocks_restraints_safe.Disabled = true;

            pre_job_safe_checks_na.Disabled = true;
            pre_job_safe_checks_risk.Disabled = true;
            pre_job_safe_checks_safe.Disabled = true;



            standard_adequate_job_na.Disabled = true;
            standard_adequate_job_risk.Disabled = true;
            standard_adequate_job_safe.Disabled = true;

            standard_established_na.Disabled = true;
            standard_established_risk.Disabled = true;
            standard_established_safe.Disabled = true;

            standard_being_maintained_safe.Disabled = true;
            standard_being_maintained_risk.Disabled = true;
            standard_being_maintained_na.Disabled = true;

            isolation_lockout_na.Disabled = true;
            isolation_lockout_risk.Disabled = true;
            isolation_lockout_safe.Disabled = true;

            hot_work_permit_na.Disabled = true;
            hot_work_permit_risk.Disabled = true;
            hot_work_permit_safe.Disabled = true;

            confined_space_permit_na.Disabled = true;
            confined_space_permit_risk.Disabled = true;
            confined_space_permit_safe.Disabled = true;

            electrical_permit_na.Disabled = true;
            electrical_permit_risk.Disabled = true;
            electrical_permit_safe.Disabled = true;

            work_height_permit_na.Disabled = true;
            work_height_permit_risk.Disabled = true;
            work_height_permit_safe.Disabled = true;

            standards_established_understood_na.Disabled = true;
            standards_established_understood_risk.Disabled = true;
            standards_established_understood_safe.Disabled = true;

            walkway_passageways_na.Disabled = true;
            walkway_passageways_risk.Disabled = true;
            walkway_passageways_safe.Disabled = true;

            disorganized_tools_bench_na.Disabled = true;
            disorganized_tools_bench_risk.Disabled = true;
            disorganized_tools_bench_safe.Disabled = true;

            materials_storage_na.Disabled = true;
            materials_storage_risk.Disabled = true;
            materials_storage_safe.Disabled = true;

            obstruction_leaning_items_na.Disabled = true;
            obstruction_leaning_items_risk.Disabled = true;
            obstruction_leaning_items_safe.Disabled = true;

            stairs_platforms_na.Disabled = true;
            stairs_platforms_risk.Disabled = true;
            stairs_platforms_safe.Disabled = true;

            housekeeping_by_na.Disabled = true;
            housekeeping_by_risk.Disabled = true;
            housekeeping_by_safe.Disabled = true;

            chemical_storage_by_na.Disabled = true;
            chemical_storage_by_risk.Disabled = true;
            chemical_storage_by_safe.Disabled = true;

            waste_diposal_by_na.Disabled = true;
            waste_diposal_by_risk.Disabled = true;
            waste_diposal_by_safe.Disabled = true;

            walking_working_surface_by_na.Disabled = true;
            walking_working_surface_by_risk.Disabled = true;
            walking_working_surface_by_safe.Disabled = true;

            txtpostion_people.Disabled = true;
            txtpersonal_protection_equipment.Disabled = true;
            txttools_equipment.Disabled = true;
            txtprocedures.Disabled = true;
            txtorderliness_tidiness.Disabled = true;
            txtenvironment.Disabled = true;

           






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

        protected void btSotformedit_Click(object sender, EventArgs e)
        {

            id = Request.QueryString["id"];


            Response.Redirect("sotform.aspx?pagetype=edit&id=" + id);
        }

        protected void btSubmit_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            bool close_all_action = true;

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var action = from c in dbConnect.process_action_sots
                         where c.sot_id == Convert.ToInt32(id)
                         select c;

            foreach (process_action_sot rc in action)
            {
                if (rc.action_status_id != 3 && rc.action_status_id != 5 && rc.action_status_id != 4)//complete and cancel and close
                {
                    close_all_action = false;
                }

            }

            if (close_all_action)//close all action
            {

                var query = from c in dbConnect.sots
                            where c.id == Convert.ToInt32(id)
                            select c;

                foreach (sot rc in query)
                {
                   // rc.status_form = 2;//not edit
                    rc.process_status = 2;//close not edit
                    rc.close_sot_date = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                }

                dbConnect.SubmitChanges();
               
                Response.Redirect("sotform.aspx?pagetype=view&id=" + id);

            }
            else
            {

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "err_msg", "alert('" + Resources.Main.close_all_action + "');", true);


            }
        }



    }
}