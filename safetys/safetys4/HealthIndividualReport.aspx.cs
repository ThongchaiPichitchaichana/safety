using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using safetys4.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class HealthIndividualReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
            {
                if (!IsPostBack)
                {
                    bool result = Permission.checkPermision("report", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
                    }
                    Panel secondPanel;
                    secondPanel = (Panel)Master.FindControl("menu_sidebar_report");
                    secondPanel.Visible = true;

                    LinkButton link = (LinkButton)Master.FindControl("btHealthIndividualReport");
                    link.Attributes.CssStyle.Add("background-color", "#e6e6e8");
                }
            }
            else
            {
                string original_url = Server.UrlEncode(Context.Request.RawUrl);
                Response.Redirect("login.aspx?returnUrl=" + original_url);
            }
        }

        protected void btExportIndividual_Click(object sender, EventArgs e)
        {
            string lang = Session["lang"].ToString();
            string country = Session["country"].ToString();
            ArrayList files = new ArrayList();
            string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathuploadhealth"];
            string employee_id = Request.Form[ddemployee.UniqueID];
   

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int REJECT = 3;

                var q = (from c in dbConnect.healths
                         join em in dbConnect.employees on c.health_employee_id equals em.employee_id into joinE
                         from em in joinE.DefaultIfEmpty()
                         where c.health_employee_id == employee_id && c.process_status != REJECT
                         orderby c.report_date descending
                         select new
                         {
                             fullname = chageDataLanguage(em.first_name_th + " " + em.last_name_th, em.first_name_en + " " + em.last_name_en, lang),
                             c.report_date,
                             c.year_health,
                             c.company_id,
                             c.function_id,
                             c.department_id,
                             c.division_id,
                             c.section_id,
                             c.health_employee_id,
                             c.employee_id,
                             c.birth_date,
                             c.service_year,
                             c.service_year_current,
                             c.age,

                             c.id,
                             location_company_name_en = c.location_company_name_en == null ? "" : c.location_company_name_en,
                             location_company_name_th = c.location_company_name_th == null ? "" : c.location_company_name_th,
                             location_function_name_en = c.location_function_name_en == null ? "" : c.location_function_name_en,
                             location_function_name_th = c.location_function_name_th == null ? "" : c.location_function_name_th,
                             location_department_name_en = c.location_company_name_en == null ? "" : c.location_department_name_en,
                             location_department_name_th = c.location_department_name_th == null ? "" : c.location_department_name_th,
                             location_division_name_en = c.location_division_name_en == null ? "" : c.location_division_name_en,
                             location_division_name_th = c.location_division_name_th == null ? "" : c.location_division_name_th,
                             location_section_name_en = c.location_section_name_en == null ? "" : c.location_section_name_en,
                             location_section_name_th = c.location_section_name_th == null ? "" : c.location_section_name_th,



                         }).Take(1);


                ReportDocument cryRpt;
                cryRpt = new ReportDocument();
                cryRpt.Load(Server.MapPath("~/HealthpersonExport.rpt"));



                TextObject report_name = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["ReportName"]);
                TextObject lbProfile = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lbProfile"]);
                TextObject lbName = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lbName"]);
                TextObject lbEmployeeID = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lbEmployeeID"]);
                TextObject lb_company = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_company"]);
                TextObject lb_function = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_function"]);
                TextObject lb_department = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_department"]);
                TextObject lb_division = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_division"]);
                TextObject lb_section = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_section"]);
                TextObject lb_birthday = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_birthday"]);
                TextObject lb_age = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_age"]);
                TextObject lb_service_year = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_service_year"]);
                TextObject lb_service_year_current = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_service_year_current"]);

                report_name.Text = Resources.Health.lbhealth_rehabilitation_individual_report;
                lbProfile.Text = Resources.Health.lbprofile;
                lbName.Text = Resources.Health.lbemployee_name;
                lbEmployeeID.Text = Resources.Health.lbemployeeid;
                lb_company.Text = Resources.Health.lbCompany;
                lb_function.Text = Resources.Health.lbfucntion;
                lb_department.Text = Resources.Health.lbdepartment;
                lb_division.Text = Resources.Health.lbdivision;
                lb_section.Text = Resources.Health.lbsection;
                lb_birthday.Text = Resources.Health.lbdateofbirth;
                lb_age.Text = Resources.Health.lbage;
                lb_service_year.Text = Resources.Health.lbserviceyear;
                lb_service_year_current.Text = Resources.Health.lbserviceyear_current;



                foreach (var hc in q)
                {
                    TextObject fullname = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["fullname"]);
                    TextObject employeeid = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["employeeid"]);
                    TextObject company = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["company"]);
                    TextObject function = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["function"]);
                    TextObject department = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["department"]);
                    TextObject division = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["division"]);
                    TextObject section = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["section"]);
                    TextObject date_birthday = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["date_birthday"]);
                    TextObject age = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["age"]);
                    TextObject service_year = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["service_year"]);
                    TextObject service_year_current = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["service_year_current"]);

                    fullname.Text = hc.fullname;
                    employeeid.Text = hc.health_employee_id;
                    company.Text = chageDataLanguage(hc.location_company_name_th, hc.location_company_name_en, lang);
                    function.Text = chageDataLanguage(hc.location_function_name_th, hc.location_function_name_en, lang);
                    department.Text = chageDataLanguage(hc.location_department_name_th, hc.location_department_name_en, lang);
                    division.Text = chageDataLanguage(hc.location_division_name_th, hc.location_division_name_en, lang);
                    section.Text = chageDataLanguage(hc.location_section_name_th, hc.location_section_name_en, lang);

                    date_birthday.Text = FormatDates.getDateShowFromDate(Convert.ToDateTime(hc.birth_date), lang);
                    age.Text = hc.age;
                    service_year.Text = hc.service_year;
                    service_year_current.Text = hc.service_year_current.ToString();

                   
                   

                }


                DataTable dtWorking = new DataTable("working_background");  //*** DataTable Map DataSet.xsd ***//

                dtWorking.Columns.Add(new DataColumn("year", typeof(System.String)));
                dtWorking.Columns.Add(new DataColumn("working_description", typeof(System.String)));
                dtWorking.Columns.Add(new DataColumn("service_year", typeof(System.String)));
                dtWorking.Columns.Add(new DataColumn("risk_factor_relate_work", typeof(System.String)));
                dtWorking.Columns.Add(new DataColumn("environment_monitor", typeof(System.String)));
                dtWorking.Columns.Add(new DataColumn("year_environmental_inspection", typeof(System.String)));
                dtWorking.Columns.Add(new DataColumn("result_environmental_inspection", typeof(System.String)));
                dtWorking.Columns.Add(new DataColumn("duration_risk", typeof(System.String)));
                dtWorking.Columns.Add(new DataColumn("workplace_monitoring_results", typeof(System.String)));
                dtWorking.Columns.Add(new DataColumn("inspection_attachment", typeof(System.String)));




                var w = from c in dbConnect.risk_factor_relate_work_actions
                        join h in dbConnect.healths on c.health_id equals h.id
                        join r in dbConnect.risk_factor_relate_works on c.risk_factor_relate_work_id equals r.id
                        join d in dbConnect.duration_risk_factors on c.duration_risk_factor_id equals d.id
                        where h.health_employee_id == employee_id && c.status == "A"
                        && h.process_status != REJECT
                        orderby h.report_date ascending
                        select new
                        {

                            h.year_health,
                            h.job_type_machine_type,
                            h.service_year_current,
                            h.report_date,
                            h.employee_id,

                            duration_risk_factor = chageDataLanguage(d.name_th, d.name_en, lang),
                            risk_factor_relate_work = chageDataLanguage(r.name_th, r.name_en, lang),
                            c.file_risk_factor,
                            c.year,
                            c.monitoring_environment,
                            c.monitoring_results,
                            c.result,


                        };



                foreach (var rc in w)
                {
                    DataRow drWorking = dtWorking.NewRow();
                    drWorking["year"] = rc.year_health;
                    drWorking["working_description"] = rc.job_type_machine_type;
                    drWorking["service_year"] = rc.service_year_current;
                    drWorking["risk_factor_relate_work"] = rc.risk_factor_relate_work;


                    string monitoring_environment = "";
                    if (rc.monitoring_environment == "Y")
                    {
                        monitoring_environment = Resources.Health.lbyes;
                    }
                    else if (rc.monitoring_environment == "N")
                    {
                        monitoring_environment = Resources.Health.lbno;
                    }
                    string year = "";

                    if (rc.year != "")
                    {
                        year = FormatDates.getYear(Convert.ToInt16(rc.year), lang).ToString();

                    }



                    string monitoring_results = "";
                    if (rc.monitoring_results == "comply")
                    {
                        monitoring_results = Resources.Health.comply;
                    }
                    else if (rc.monitoring_results == "not_comply")
                    {
                        monitoring_results = Resources.Health.not_comply;
                    }


                    drWorking["environment_monitor"] = monitoring_environment;
                    drWorking["year_environmental_inspection"] = rc.year;
                    drWorking["result_environmental_inspection"] = rc.result;
                    drWorking["duration_risk"] = rc.duration_risk_factor;
                    drWorking["workplace_monitoring_results"] = monitoring_results;
                    drWorking["inspection_attachment"] = rc.file_risk_factor;
                    dtWorking.Rows.Add(drWorking);

                    if (rc.file_risk_factor != "")
                    {                     
                        string[] fhealth = rc.file_risk_factor.Split(',');
                        for (int i = 0; i < fhealth.Length; i++)
                        {
                            string file = rc.employee_id + "_" + rc.report_date.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB")) + "\\" + fhealth[i];
                            files.Add(file);
                        }
                    }

                }


                cryRpt.Subreports["working_background"].Database.Tables["working_background"].SetDataSource(dtWorking);

                TextObject lb_working_background = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lb_working_background"];
                TextObject lbYear = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lbYear"];
                TextObject lbWorking_description = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lbWorking_description"];
                TextObject lbServiceYear = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lbServiceYear"];
                TextObject lbRiskFactorRelateWork = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lbRiskFactorRelateWork"];
                TextObject lbEnvironmentalMonitor = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lbEnvironmentalMonitor"];
                TextObject lbYearEnvironmentalInspection = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lbYearEnvironmentalInspection"];
                TextObject lbResultEnvironmentalInspection = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lbResultEnvironmentalInspection"];
                TextObject lbDurationRisk = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lbDurationRisk"];
                TextObject lbWorkplaceMonitoringResults = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lbWorkplaceMonitoringResults"];
                TextObject lbInspectionAttachment = (TextObject)cryRpt.Subreports["working_background"].ReportDefinition.ReportObjects["lbInspectionAttachment"];

                lb_working_background.Text = Resources.Health.lbworking_background;
                lbYear.Text = Resources.Health.lbyears;
                lbWorking_description.Text = Resources.Health.lbjobtype;
                lbServiceYear.Text = Resources.Health.lbserviceyear_current;
                lbRiskFactorRelateWork.Text = Resources.Health.risk_factor_relate_work;
                lbEnvironmentalMonitor.Text = Resources.Health.monitoringenvironment;
                lbYearEnvironmentalInspection.Text = Resources.Health.year_risk_factor_relate_work;
                lbResultEnvironmentalInspection.Text = Resources.Health.result_risk_factor;
                lbDurationRisk.Text = Resources.Health.duration_risk_factor;
                lbWorkplaceMonitoringResults.Text = Resources.Health.monitoring_results;
                lbInspectionAttachment.Text = Resources.Health.file_risk_factor;



                DataTable dtHealthcheck = new DataTable("health_check_report");  //*** DataTable Map DataSet.xsd ***//

                dtHealthcheck.Columns.Add(new DataColumn("name_check", typeof(System.String)));
                dtHealthcheck.Columns.Add(new DataColumn("data_year1", typeof(System.String)));
                dtHealthcheck.Columns.Add(new DataColumn("data_year2", typeof(System.String)));
                dtHealthcheck.Columns.Add(new DataColumn("data_year3", typeof(System.String)));
                dtHealthcheck.Columns.Add(new DataColumn("data_year4", typeof(System.String)));
                dtHealthcheck.Columns.Add(new DataColumn("data_year5", typeof(System.String)));





                var he = from c in dbConnect.healths
                         join ho in dbConnect.hospitals on c.hospital_id equals ho.id into joinHospital
                         from ho in joinHospital.DefaultIfEmpty()
                         where c.health_employee_id == employee_id && c.process_status != REJECT
                         orderby c.report_date descending
                         select new
                         {
                             c.year_health,
                             hostpital = chageDataLanguage(ho.name_th, ho.name_en, lang),
                             c.id,


                         };


                int count = 1;

                TextObject datayear1 = (TextObject)cryRpt.Subreports["health_check_report"].ReportDefinition.ReportObjects["lbYear1"];
                TextObject datayear2 = (TextObject)cryRpt.Subreports["health_check_report"].ReportDefinition.ReportObjects["lbYear2"];
                TextObject datayear3 = (TextObject)cryRpt.Subreports["health_check_report"].ReportDefinition.ReportObjects["lbYear3"];
                TextObject datayear4 = (TextObject)cryRpt.Subreports["health_check_report"].ReportDefinition.ReportObjects["lbYear4"];
                TextObject datayear5 = (TextObject)cryRpt.Subreports["health_check_report"].ReportDefinition.ReportObjects["lbYear5"];

                DataRow drCheck = dtHealthcheck.NewRow();
                drCheck["name_check"] = Resources.Health.lbhealth_check_up_hospital;
                drCheck["data_year1"] = "-";
                drCheck["data_year2"] = "-";
                drCheck["data_year3"] = "-";
                drCheck["data_year4"] = "-";
                drCheck["data_year5"] = "-";

                dtHealthcheck.Rows.Add(drCheck);

                DataRow drCheck2 = dtHealthcheck.NewRow();
                drCheck2["name_check"] = Resources.Health.lbabnormal_health_check_report;
                drCheck2["data_year1"] = "";
                drCheck2["data_year2"] = "";
                drCheck2["data_year3"] = "";
                drCheck2["data_year4"] = "";
                drCheck2["data_year5"] = "";

                dtHealthcheck.Rows.Add(drCheck2);

                DataRow drCheck3 = dtHealthcheck.NewRow();
                drCheck3["name_check"] = "- " + Resources.Health.lbpulmonary_function;
                drCheck3["data_year1"] = "-";
                drCheck3["data_year2"] = "-";
                drCheck3["data_year3"] = "-";
                drCheck3["data_year4"] = "-";
                drCheck3["data_year5"] = "-";

                dtHealthcheck.Rows.Add(drCheck3);

                DataRow drCheck4 = dtHealthcheck.NewRow();
                drCheck4["name_check"] = "- " + Resources.Health.lbaudiogram;
                drCheck4["data_year1"] = "-";
                drCheck4["data_year2"] = "-";
                drCheck4["data_year3"] = "-";
                drCheck4["data_year4"] = "-";
                drCheck4["data_year5"] = "-";

                dtHealthcheck.Rows.Add(drCheck4);


                DataRow drCheck5 = dtHealthcheck.NewRow();
                drCheck5["name_check"] = "- " + Resources.Health.lboccupational_vision;
                drCheck5["data_year1"] = "-";
                drCheck5["data_year2"] = "-";
                drCheck5["data_year3"] = "-";
                drCheck5["data_year4"] = "-";
                drCheck5["data_year5"] = "-";

                dtHealthcheck.Rows.Add(drCheck5);


                DataRow drCheck6 = dtHealthcheck.NewRow();
                drCheck6["name_check"] = "- " + Resources.Health.lbimbalance_body_chemistry;
                drCheck6["data_year1"] = "-";
                drCheck6["data_year2"] = "-";
                drCheck6["data_year3"] = "-";
                drCheck6["data_year4"] = "-";
                drCheck6["data_year5"] = "-";

                dtHealthcheck.Rows.Add(drCheck6);


                DataRow drCheck7 = dtHealthcheck.NewRow();
                drCheck7["name_check"] = Resources.Health.lbpersonal_health_problem;
                drCheck7["data_year1"] = "-";
                drCheck7["data_year2"] = "-";
                drCheck7["data_year3"] = "-";
                drCheck7["data_year4"] = "-";
                drCheck7["data_year5"] = "-";

                dtHealthcheck.Rows.Add(drCheck7);

                DataRow drCheck8 = dtHealthcheck.NewRow();
                drCheck8["name_check"] = Resources.Health.lbsmoking_record;
                drCheck8["data_year1"] = "-";
                drCheck8["data_year2"] = "-";
                drCheck8["data_year3"] = "-";
                drCheck8["data_year4"] = "-";
                drCheck8["data_year5"] = "-";

                dtHealthcheck.Rows.Add(drCheck8);


                DataRow drCheck9 = dtHealthcheck.NewRow();
                drCheck9["name_check"] = Resources.Health.lbenvironment_report;
                drCheck9["data_year1"] = "-";
                drCheck9["data_year2"] = "-";
                drCheck9["data_year3"] = "-";
                drCheck9["data_year4"] = "-";
                drCheck9["data_year5"] = "-";

                dtHealthcheck.Rows.Add(drCheck9);



                int year_start = FormatDates.getYear(DateTime.Now.Year, lang);
                foreach (var rc in he)
                {
                    string abnormal_pulmonary_function = "";
                    string smoked_cigarettes = "";
                    string environmental_monitoring_results = Resources.Health.comply;
                    string abnormal_vision = "";
                    string abnormal_audiogram = "";
                    string personal_health_problem = "";
                    string imbalance_body_chemistry = "";

                    var v = from c in dbConnect.occupational_health_report_actions
                            join t in dbConnect.occupational_health_reports on c.occupational_health_report_id equals t.id

                            where c.health_id == Convert.ToInt32(rc.id) && c.status == "A"
                            orderby c.id descending
                            select new
                            {
                                c.id,
                                c.occupational_health_report_id,
                                c.repeat_health_check,
                                c.file_health_check,
                                c.flie_repeat_health_check,
                                c.abnormal_audiogram,
                                c.hearing_threshold_level,
                                c.chronic_diseases_ear,
                                c.specify_chronic_diseases_ear,
                                c.abnormal_pulmonary_function,
                                c.smoked_cigarettes,
                                c.cigarette_per_day,
                                c.smoked_months,
                                c.smoked_years,
                                c.smoked_cigarettes_other




                            };

                    foreach (var vc in v)
                    {

                        if (vc.occupational_health_report_id == 2)//ปอดผิดปกติ
                        {

                            if (vc.abnormal_pulmonary_function == "obstructive")
                            {
                                abnormal_pulmonary_function = Resources.Health.lbobstructive;
                            }
                            else if (vc.abnormal_pulmonary_function == "restrictive")
                            {
                                abnormal_pulmonary_function = Resources.Health.lbrestrictive;
                            }
                            else if (vc.abnormal_pulmonary_function == "obstructive_restrictive")
                            {
                                abnormal_pulmonary_function = Resources.Health.lbobstructive_restrictive;
                            }
                            else
                            {
                                abnormal_pulmonary_function = Resources.Health.lbabnormal;
                            }




                            if (vc.smoked_cigarettes == "NO")
                            {
                                smoked_cigarettes = Resources.Health.lbno;

                            }
                            else if (vc.smoked_cigarettes == "YES_SMOKING")
                            {
                                smoked_cigarettes = Resources.Health.lbyes_smoking + " " + vc.cigarette_per_day + " " + Resources.Health.lbcigarettes_per_day;
                            }
                            else if (vc.smoked_cigarettes == "YES_SMOKED")
                            {
                                smoked_cigarettes = Resources.Health.lbyes_smoked + " " + vc.smoked_years + " " + Resources.Health.lbyears + " " + vc.smoked_months + " " + Resources.Health.lbmonths;
                            }
                            else if (vc.smoked_cigarettes == "SMOKED_OTHER")
                            {
                                smoked_cigarettes = Resources.Health.lbsmoked_other + " : " + vc.smoked_cigarettes_other;
                            }
                           


                        }
                     


                        if (vc.occupational_health_report_id == 4)//การมองเห็นสายตาอาชีวอนามัยผิดปกติ
                        {
                            abnormal_vision = Resources.Health.lbabnormal;

                        }
                       



                        if (vc.occupational_health_report_id == 3)//การได้ยินผิดปกติ
                        {

                            if (vc.abnormal_audiogram == "left")
                            {
                                abnormal_audiogram = Resources.Health.lbleft_ear;
                            }
                            else if (vc.abnormal_audiogram == "right")
                            {
                                abnormal_audiogram = Resources.Health.lbright_ear;
                            }
                            else if (vc.abnormal_audiogram == "both")
                            {
                                abnormal_audiogram = Resources.Health.lbboth_ear;
                            }
                          


                           
                            if(abnormal_audiogram!="")
                            {
                                abnormal_audiogram = abnormal_audiogram + ", " + Resources.Health.lbhearing_threshold_level + " : " + vc.hearing_threshold_level;
                            }
                            else
                            {
                                abnormal_audiogram = Resources.Health.lbabnormal;

                            }

                            if (vc.chronic_diseases_ear == "N")
                            {
                                personal_health_problem = Resources.Health.lbno;

                            }
                            else if (vc.chronic_diseases_ear == "Y")
                            {
                                personal_health_problem = Resources.Health.lbyes + " : " + vc.specify_chronic_diseases_ear;
                            }
                            else
                            {
                                personal_health_problem = "-";
                            }


                        }
                       

                        if (vc.occupational_health_report_id == 5)//ระดับสารเคมีในร่างกายไม่เป็นไปตามมาตรฐาน
                        {
                            imbalance_body_chemistry = Resources.Health.lbabnormal;

                        }
                        




                    }//end occupational



                    var r = from c in dbConnect.risk_factor_relate_work_actions
                            join t in dbConnect.risk_factor_relate_works on c.risk_factor_relate_work_id equals t.id
                            join d in dbConnect.duration_risk_factors on c.duration_risk_factor_id equals d.id

                            where c.health_id == Convert.ToInt32(rc.id) && c.status == "A"
                            orderby c.id descending
                            select new
                            {
                                c.id,
                                risk_factor_relate_work = chageDataLanguage(t.name_th, t.name_en, lang),
                                c.other,
                                c.year,
                                c.result,
                                duration_risk_factor_name = chageDataLanguage(d.name_th, d.name_en, lang),
                                c.file_risk_factor,
                                c.monitoring_results,
                                c.monitoring_environment


                            };




                    foreach (var g in r)
                    {
                        string comply = "";
                        if (environmental_monitoring_results != "")
                        {
                           
                            if (g.monitoring_results == "not_comply")
                            {
                                comply = Resources.Health.not_comply;
                            }
                            else if (g.monitoring_results == "comply")
                            {
                                comply = Resources.Health.comply;
                            }

                            environmental_monitoring_results = g.risk_factor_relate_work+":"+comply;
                        }
                        else
                        {
                            environmental_monitoring_results = environmental_monitoring_results +", "+g.risk_factor_relate_work + ":" + comply;
                        }
                       

                    }




                    if (count == 5)
                    {
                      
                        dtHealthcheck.Rows[0]["data_year1"] = rc.hostpital == "" ? "-" : rc.hostpital;
                        dtHealthcheck.Rows[2]["data_year1"] = abnormal_pulmonary_function == "" ? "-" : abnormal_pulmonary_function;
                        dtHealthcheck.Rows[3]["data_year1"] = abnormal_audiogram == "" ? "-" : abnormal_audiogram;
                        dtHealthcheck.Rows[4]["data_year1"] = abnormal_vision == "" ? "-" : abnormal_vision;
                        dtHealthcheck.Rows[5]["data_year1"] = imbalance_body_chemistry == "" ? "-" : imbalance_body_chemistry;
                        dtHealthcheck.Rows[6]["data_year1"] = personal_health_problem == "" ? "-" : personal_health_problem;
                        dtHealthcheck.Rows[7]["data_year1"] = smoked_cigarettes == "" ? "-" : smoked_cigarettes;
                        dtHealthcheck.Rows[8]["data_year1"] = environmental_monitoring_results == "" ? "-" : environmental_monitoring_results;


                    }
                    else if (count == 4)
                    {
                        dtHealthcheck.Rows[0]["data_year2"] = rc.hostpital==""  ? "-" : rc.hostpital;
                        dtHealthcheck.Rows[2]["data_year2"] = abnormal_pulmonary_function == "" ? "-" : abnormal_pulmonary_function;
                        dtHealthcheck.Rows[3]["data_year2"] = abnormal_audiogram == "" ? "-" : abnormal_audiogram;
                        dtHealthcheck.Rows[4]["data_year2"] = abnormal_vision == "" ? "-" : abnormal_vision;
                        dtHealthcheck.Rows[5]["data_year2"] = imbalance_body_chemistry == "" ? "-" : imbalance_body_chemistry;
                        dtHealthcheck.Rows[6]["data_year2"] = personal_health_problem == "" ? "-" : personal_health_problem;
                        dtHealthcheck.Rows[7]["data_year2"] = smoked_cigarettes == "" ? "-" : smoked_cigarettes;
                        dtHealthcheck.Rows[8]["data_year2"] = environmental_monitoring_results == "" ? "-" : environmental_monitoring_results;

                    }
                    else if (count == 3)
                    {
                        dtHealthcheck.Rows[0]["data_year3"] = rc.hostpital == "" ? "-" : rc.hostpital;
                        dtHealthcheck.Rows[2]["data_year3"] = abnormal_pulmonary_function == "" ? "-" : abnormal_pulmonary_function;
                        dtHealthcheck.Rows[3]["data_year3"] = abnormal_audiogram == "" ? "-" : abnormal_audiogram;
                        dtHealthcheck.Rows[4]["data_year3"] = abnormal_vision == "" ? "-" : abnormal_vision;
                        dtHealthcheck.Rows[5]["data_year3"] = imbalance_body_chemistry == "" ? "-" : imbalance_body_chemistry;
                        dtHealthcheck.Rows[6]["data_year3"] = personal_health_problem == "" ? "-" : personal_health_problem;
                        dtHealthcheck.Rows[7]["data_year3"] = smoked_cigarettes == "" ? "-" : smoked_cigarettes;
                        dtHealthcheck.Rows[8]["data_year3"] = environmental_monitoring_results == "" ? "-" : environmental_monitoring_results;

                    }
                    else if (count == 2)
                    {
                        dtHealthcheck.Rows[0]["data_year4"] = rc.hostpital == "" ? "-" : rc.hostpital;
                        dtHealthcheck.Rows[2]["data_year4"] = abnormal_pulmonary_function == "" ? "-" : abnormal_pulmonary_function;
                        dtHealthcheck.Rows[3]["data_year4"] = abnormal_audiogram == "" ? "-" : abnormal_audiogram;
                        dtHealthcheck.Rows[4]["data_year4"] = abnormal_vision == "" ? "-" : abnormal_vision;
                        dtHealthcheck.Rows[5]["data_year4"] = imbalance_body_chemistry == "" ? "-" : imbalance_body_chemistry;
                        dtHealthcheck.Rows[6]["data_year4"] = personal_health_problem == "" ? "-" : personal_health_problem;
                        dtHealthcheck.Rows[7]["data_year4"] = smoked_cigarettes == "" ? "-" : smoked_cigarettes;
                        dtHealthcheck.Rows[8]["data_year4"] = environmental_monitoring_results == "" ? "-" : environmental_monitoring_results;

                    }
                    else if (count == 1)
                    {
                        year_start = FormatDates.getYear(Convert.ToInt16(rc.year_health), lang);
                        dtHealthcheck.Rows[0]["data_year5"] = rc.hostpital == "" ? "-" : rc.hostpital;
                        dtHealthcheck.Rows[2]["data_year5"] = abnormal_pulmonary_function == "" ? "-" : abnormal_pulmonary_function;
                        dtHealthcheck.Rows[3]["data_year5"] = abnormal_audiogram == "" ? "-" : abnormal_audiogram;
                        dtHealthcheck.Rows[4]["data_year5"] = abnormal_vision == "" ? "-" : abnormal_vision;
                        dtHealthcheck.Rows[5]["data_year5"] = imbalance_body_chemistry == "" ? "-" : imbalance_body_chemistry;
                        dtHealthcheck.Rows[6]["data_year5"] = personal_health_problem == "" ? "-" : personal_health_problem;
                        dtHealthcheck.Rows[7]["data_year5"] = smoked_cigarettes == "" ? "-" : smoked_cigarettes;
                        dtHealthcheck.Rows[8]["data_year5"] = environmental_monitoring_results == "" ? "-" : environmental_monitoring_results;

                    }


                    if (count == 5) break;//เอาแค่ 5 ปี

                    count++;
                }

                datayear5.Text = year_start.ToString();
                datayear4.Text = (year_start - 1).ToString();
                datayear3.Text = (year_start - 2).ToString();
                datayear2.Text = (year_start - 3).ToString();
                datayear1.Text = (year_start - 4).ToString();




                cryRpt.Subreports["health_check_report"].Database.Tables["health_check_report"].SetDataSource(dtHealthcheck);


                TextObject lb_health_check_report = (TextObject)cryRpt.Subreports["health_check_report"].ReportDefinition.ReportObjects["lb_health_check_report"];
                TextObject lbHealthCheck = (TextObject)cryRpt.Subreports["health_check_report"].ReportDefinition.ReportObjects["lbHealthCheck"];
                TextObject lbYearCheck = (TextObject)cryRpt.Subreports["health_check_report"].ReportDefinition.ReportObjects["lbYearCheck"];



                lb_health_check_report.Text = Resources.Health.lbhealth_check_report;
                lbHealthCheck.Text = Resources.Health.lbhealth_check;
                lbYearCheck.Text = Resources.Health.lbyear_check;



                ExportOptions CrExportOptions;
                DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                string pathexport = System.Configuration.ConfigurationManager.AppSettings["pathexport"];
                string path_write = string.Format("{0}" + pathexport + "health\\individual\\" + employee_id + ".pdf", Server.MapPath(@"\"));

                CrDiskFileDestinationOptions.DiskFileName = path_write;
                CrExportOptions = cryRpt.ExportOptions;
                {
                    CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                    CrExportOptions.FormatOptions = CrFormatTypeOptions;
                }
                cryRpt.Export();
                cryRpt.Dispose();







                string path_zip = string.Format("{0}" + pathexport + "health\\individual\\" + employee_id + ".zip", Server.MapPath(@"\"));



                if (File.Exists(path_zip))
                {
                    File.Delete(path_zip);
                }
                using (ZipArchive archive = ZipFile.Open(path_zip, ZipArchiveMode.Create))
                {

                    string path_file = string.Format("{0}" + pathupload +"\\", Server.MapPath(@"\"));

                    foreach (string f in files)
                    {
                        if (File.Exists(path_file + f))
                        {
                            archive.CreateEntryFromFile(path_file + f, f.Split('\\').Last());
                        }
                    }




                    archive.CreateEntryFromFile(path_write, employee_id + ".pdf");

                }


                Response.ContentType = "application/zip";
                Response.AddHeader("Content-Disposition", "filename=" + employee_id + ".zip");
                Response.TransmitFile(path_zip);


            }

        }







        public string chageDataLanguage(string vTH, string vEN, string lang)
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

    }




    
}