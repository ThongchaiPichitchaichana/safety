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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace safetys4
{
    public partial class HealthExport1 : System.Web.UI.Page
    {
        string id = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
            {

            }
            else
            {
                string original_url = Server.UrlEncode(Context.Request.RawUrl);
                Response.Redirect("login.aspx?returnUrl=" + original_url);
            }

            id = Request.QueryString["id"];
            string lang = Session["lang"].ToString();
            string country = Session["country"].ToString();
            string health_doc_number = "";
            ArrayList action = new ArrayList();
            ArrayList riskfiles = new ArrayList();
            ArrayList occufiles = new ArrayList();
            string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathuploadhealth"];
            string path_folder = "";
            string pathfolder = "";



            if (!IsPostBack)
            {
                using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
                {

                    var q = from c in dbConnect.healths
                            join em in dbConnect.employees on c.health_employee_id equals em.employee_id into joinE
                            from em in joinE.DefaultIfEmpty()
                            join h in dbConnect.hospitals on c.hospital_id equals h.id into joinH
                            from h in joinH.DefaultIfEmpty()
                            join s in dbConnect.health_status on c.process_status equals s.id
                           
                            
                            where c.id == Convert.ToInt32(id)
                            select new
                            {
      
                                report_date = c.report_date,
                                c.company_id,
                                c.function_id,
                                c.department_id,
                                c.division_id,
                                c.section_id,

                                c.year_health,
                                c.hospital_id,

                                c.health_employee_id,
                                c.employee_id,
                                c.process_status,
                                c.typeuser_login,
                                c.doc_no,
                            
                                status = chageDataLanguage(s.name_th, s.name_en, lang),
                               
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
                                fullname = chageDataLanguage(em.first_name_th + " " + em.last_name_th, em.first_name_en + " " + em.last_name_en, lang),
                                hospital_name = chageDataLanguage(h.name_th,h.name_en,lang),

                                c.birth_date,
                                c.hiring_date,
                                c.age,
                                c.service_year,
                                c.service_year_current,
                                c.job_type_machine_type,
                                c.significant_insignificant,
                                c.step_form

                            };




                    ReportDocument cryRpt;
                    cryRpt = new ReportDocument();
                    cryRpt.Load(Server.MapPath("~/HealthExport.rpt"));

                  

                    TextObject report_name = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["ReportName"]);
                    TextObject lb_doc_no = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_doc_no"]);
                    TextObject lb_status = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_status"]);
                    TextObject lbemployeeid = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lbemployeeid"]);
                    TextObject lbyear_checkup = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lbyear_checkup"]);
                    TextObject lbhospital = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lbhospital"]);
                    TextObject lb_employee_name = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_employee_name"]);
                    TextObject lb_date_report = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_date_report"]);
                    TextObject lb_company = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_company"]);
                    TextObject lb_function = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_function"]);
                    TextObject lb_department = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_department"]);
                    TextObject lb_division = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_division"]);
                    TextObject lb_section = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_section"]);


                    TextObject lb_date_birth = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_date_birth"]);
                    TextObject lb_hiring_date = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_hiring_date"]);
                    TextObject lb_age = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_age"]);
                    TextObject lb_service_year = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_service_year"]);
                    TextObject lb_service_year_current = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_service_year_current"]);
                    TextObject lb_work_description = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_work_description"]);
                    //TextObject lb_significantorinsignificant = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_significantorinsignificant"]);

                    TextObject lb_form2 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_form2"]);
                    TextObject lb_form3 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_form3"]);


                    report_name.Text = Resources.Main.lbHealthExport;
                    lb_doc_no.Text =  Resources.Health.doc_no;
                    lb_status.Text = Resources.Health.status;
                    lbemployeeid.Text = Resources.Health.lbemployeeid;
                    lbyear_checkup.Text = Resources.Health.lbyear_check;
                    lbhospital.Text = Resources.Health.lbhospital;
                    lb_employee_name.Text = Resources.Health.lbemployee_name;
                    lb_date_report.Text = Resources.Health.report_date;
                    lb_company.Text = Resources.Health.lbCompany;
                    lb_function.Text = Resources.Health.lbfucntion;
                    lb_department.Text = Resources.Health.lbdepartment;
                    lb_division.Text = Resources.Health.lbdivision;
                    lb_section.Text = Resources.Health.lbsection;

                    lb_date_birth.Text = Resources.Health.lbdateofbirth;
                    lb_hiring_date.Text = Resources.Health.lbhiringdate;
                    lb_age.Text = Resources.Health.lbage;
                    lb_service_year.Text = Resources.Health.lbserviceyear;
                    lb_service_year_current.Text = Resources.Health.lbserviceyear_current;
                    lb_work_description.Text = Resources.Health.lbjobtype;
                   // lb_significantorinsignificant.Text = Resources.Health.significantorinsignificant;

                    lb_form2.Text = Resources.Health.healthstep2;
                    lb_form3.Text = Resources.Health.healthstep3;
          



                    TextObject doc_number = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["doc_no"]);
                    TextObject status = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["status"]);
                    TextObject employeeid = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["employeeid"]);
                    TextObject year_checkup = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["year_checkup"]);
                    TextObject hospital = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["hospital"]);
                    TextObject employee_name = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["employee_name"]);
                    TextObject date_report = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["date_report"]);
                    TextObject company = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["company"]);
                    TextObject function = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["function"]);
                    TextObject department = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["department"]);
                    TextObject division = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["division"]);
                    TextObject section = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["section"]);

                    TextObject date_birth = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["date_birth"]);
                    TextObject hiring_date = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["hiring_date"]);
                    TextObject age = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["age"]);
                    TextObject service_year = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["service_year"]);
                    TextObject service_year_current = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["service_year_current"]);
                    TextObject work_description = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["work_description"]);
                    TextObject significantorinsignificant = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["significantorinsignificant"]);
                    TextObject lbmessage_form1 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lbmessage_form1"]);
                    




                    foreach (var v in q)
                    {
                        health_doc_number = v.doc_no;
                        path_folder = v.employee_id + "_" + v.report_date.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB"));


                        string step = "";


                        if (v.process_status != 2 && v.process_status != 3)//ไม่ใช้ close กับ reject
                        {

                            if (v.step_form == 1)//area oh&s
                            {
                                string v_step = chageDataLanguage("แผนฟื้นฟูสุขภาพพนักงาน", "Health Rehabilitation", lang);

                                step = step + "(" + v_step + " - Area OH&S)";



                            }
                            else if (v.step_form == 2)
                            {

                                string v_step = chageDataLanguage("มาตรการการฟื้นฟูสุขภาพ", "Health Rehabilitation Action", lang);

                                step = step + "(" + v_step + " - Area OH&S)";

                            }
                            else if (v.step_form == 3)
                            {
                                string v_step = chageDataLanguage("ขอปิดแผนฟื้นฟูสุขภาพพนักงาน", "Request to Close Health Rehabilitation", lang);
                                bool check_close = true;

                                var s = from c in dbConnect.close_step_healths
                                        where c.country == Session["country"].ToString()
                                        orderby c.step descending
                                        select c;

                                foreach (var r in s)
                                {
                                    var w = from c in dbConnect.log_request_close_healths
                                            where c.health_id == v.id && c.status == "A"
                                            && c.group_id == r.group_id
                                            select c;

                                    if (r.group_id == 4 || r.group_id == 5)
                                    {
                                        w = w.Where(c => c.group_id == 4 || c.group_id == 5);
                                    }
                                    else
                                    {
                                        w = w.Where(c => c.group_id == r.group_id);
                                    }

                                    if (w.Count() == 0)
                                    {
                                        check_close = false;


                                        if (r.group_id == 4 || r.group_id == 5)// admin and delegate
                                        {
                                            step = "(" + v_step + " - Admin OH&S or Delegate OH&S Admin)";
                                        }

                                        if (r.group_id == 8)
                                        {
                                            step = "(" + v_step + " - Group OH&S)";
                                        }


                                        if (r.group_id == 9)
                                        {
                                            step = "(" + v_step + " - Area OH&S)";
                                        }


                                        if (r.group_id == 10)// areamanage
                                        {
                                            step = "(" + v_step + " - Area Manager)";
                                        }

                                        if (r.group_id == 11)
                                        {
                                            step = "(" + v_step + " - Area Supervisor)";
                                        }


                                        if (r.group_id == 16)
                                        {
                                            step = "(" + v_step + " -  Group OH&S Hazard)";
                                        }


                                        if (r.group_id == 17)
                                        {
                                            step = "(" + v_step + " -  Group OH&S Health)";
                                        }


                                        if (r.group_id == 18)
                                        {
                                            step = "(" + v_step + " -  Area Functional Manager)";
                                        }


                                    }




                                }//end each


                                if (check_close == true)
                                {// แสดงว่าปิดแล้ว

                                    step = "";

                                }



                            }

                        }





                        doc_number.Text = v.doc_no;
                        status.Text = v.status + " " + step;
                        employeeid.Text = v.health_employee_id;
                        employee_name.Text = v.fullname;
                        year_checkup.Text = FormatDates.getYear(Convert.ToInt16(v.year_health), lang).ToString();
                        hospital.Text = v.hospital_name;
                      
                        date_report.Text = FormatDates.getDatetimeShow(Convert.ToDateTime(v.report_date), lang);
                        company.Text = chageDataLanguage(v.location_company_name_th, v.location_company_name_en, lang);
                        function.Text = chageDataLanguage(v.location_function_name_th, v.location_function_name_en, lang);
                        department.Text = chageDataLanguage(v.location_department_name_th, v.location_department_name_en, lang);
                        division.Text = chageDataLanguage(v.location_division_name_th, v.location_division_name_en, lang);
                        section.Text = chageDataLanguage(v.location_section_name_th, v.location_section_name_en, lang);

                        date_birth.Text = FormatDates.getDateShowFromDate(Convert.ToDateTime(v.birth_date), lang);
                        hiring_date.Text = FormatDates.getDateShowFromDate(Convert.ToDateTime(v.hiring_date), lang);
                        age.Text = v.age;
                        service_year.Text = v.service_year;
                        service_year_current.Text = v.service_year_current.ToString();

                        work_description.Text = v.job_type_machine_type;

                        string significant_insignificant = "";
                        if(v.significant_insignificant=="sign")
                        {
                            significant_insignificant = Resources.Health.significant;
                        }
                        else if (v.significant_insignificant == "insign")
                        {
                            significant_insignificant = Resources.Health.insignificant;
                        }

                        significantorinsignificant.Text = "* " + significant_insignificant;

                        lbmessage_form1.Text = Resources.Health.report_message_form1;





                        DataTable dtRisk = new DataTable("risk_factor");  //*** DataTable Map DataSet.xsd ***//

                        dtRisk.Columns.Add(new DataColumn("risk_factor_relate_work", typeof(System.String)));
                        dtRisk.Columns.Add(new DataColumn("monitoringenvironment", typeof(System.String)));
                        dtRisk.Columns.Add(new DataColumn("year_risk_factor_relate_work", typeof(System.String)));
                        dtRisk.Columns.Add(new DataColumn("result_risk_factor", typeof(System.String)));
                        dtRisk.Columns.Add(new DataColumn("duration_risk_factor", typeof(System.String)));
                        dtRisk.Columns.Add(new DataColumn("monitoring_results", typeof(System.String)));
                        dtRisk.Columns.Add(new DataColumn("file_risk_factor", typeof(System.String)));




                        var ri = from c in dbConnect.risk_factor_relate_work_actions
                                join r in dbConnect.risk_factor_relate_works on c.risk_factor_relate_work_id equals r.id
                                join d in dbConnect.duration_risk_factors on c.duration_risk_factor_id equals d.id into joinD
                                from d in joinD.DefaultIfEmpty()
                                where c.health_id == Convert.ToInt32(id) && c.status == "A"
                                select new
                                {

                                    duration_risk_factor = chageDataLanguage(d.name_th, d.name_en, lang),
                                    risk_factor_relate_work = chageDataLanguage(r.name_th, r.name_en, lang),
                                    c.file_risk_factor,
                                    c.year,
                                    c.monitoring_environment,
                                    c.monitoring_results,
                                    c.result,

                                };



                        foreach (var rc in ri)
                        {
                            DataRow drRisk = dtRisk.NewRow();

                            drRisk["risk_factor_relate_work"] = rc.risk_factor_relate_work;


                            string monitoring_environment = "-";
                            if (rc.monitoring_environment == "Y")
                            {
                                monitoring_environment = Resources.Health.lbyes;
                            }
                            else if (rc.monitoring_environment == "N")
                            {
                                monitoring_environment = Resources.Health.lbno;
                            }
                            string year = "-";

                            if (rc.year != "")
                            {
                                year = FormatDates.getYear(Convert.ToInt16(rc.year), lang).ToString();

                            }



                            string monitoring_results = "-";
                            if (rc.monitoring_results == "comply")
                            {
                                monitoring_results = Resources.Health.comply;
                            }
                            else if (rc.monitoring_results == "not_comply")
                            {
                                monitoring_results = Resources.Health.not_comply;
                            }


                            drRisk["monitoringenvironment"] = monitoring_environment;
                            drRisk["year_risk_factor_relate_work"] = year;

                            string result = "-";
                            if (rc.result != "") result = rc.result;
                            drRisk["result_risk_factor"] = result;
                            drRisk["duration_risk_factor"] = rc.duration_risk_factor;
                            drRisk["monitoring_results"] = monitoring_results;
                            drRisk["file_risk_factor"] = rc.file_risk_factor;
                            dtRisk.Rows.Add(drRisk);

                            if (rc.file_risk_factor != "")
                            {
                                string[] fhealth = rc.file_risk_factor.Split(',');
                                for (int i = 0; i < fhealth.Length; i++)
                                {
                                    riskfiles.Add(fhealth[i]);
                                }
                            }



                        }


                        cryRpt.Subreports["risk_factor_relate_work_subreport"].Database.Tables["risk_factor"].SetDataSource(dtRisk);

                        TextObject lb_risk_factor_relate_work = (TextObject)cryRpt.Subreports["risk_factor_relate_work_subreport"].ReportDefinition.ReportObjects["lb_risk_factor_relate_work"];
                        TextObject lbRiskFactorRelateWork = (TextObject)cryRpt.Subreports["risk_factor_relate_work_subreport"].ReportDefinition.ReportObjects["lbRiskFactorRelateWork"];
                        TextObject lbEnvironmentalMonitor = (TextObject)cryRpt.Subreports["risk_factor_relate_work_subreport"].ReportDefinition.ReportObjects["lbEnvironmentalMonitor"];
                        TextObject lbYearEnvironmentalInspection = (TextObject)cryRpt.Subreports["risk_factor_relate_work_subreport"].ReportDefinition.ReportObjects["lbYearEnvironmentalInspection"];
                        TextObject lbResultEnvironmentalInspection = (TextObject)cryRpt.Subreports["risk_factor_relate_work_subreport"].ReportDefinition.ReportObjects["lbResultEnvironmentalInspection"];
                        TextObject lbDurationRisk = (TextObject)cryRpt.Subreports["risk_factor_relate_work_subreport"].ReportDefinition.ReportObjects["lbDurationRisk"];
                        TextObject lbWorkplaceMonitoringResults = (TextObject)cryRpt.Subreports["risk_factor_relate_work_subreport"].ReportDefinition.ReportObjects["lbWorkplaceMonitoringResults"];
                        TextObject lbInspectionAttachment = (TextObject)cryRpt.Subreports["risk_factor_relate_work_subreport"].ReportDefinition.ReportObjects["lbInspectionAttachment"];

                        lb_risk_factor_relate_work.Text = Resources.Health.risk_factor_relate_work_form;
                        lbRiskFactorRelateWork.Text = Resources.Health.risk_factor_relate_work;
                        lbEnvironmentalMonitor.Text = Resources.Health.monitoringenvironment;
                        lbYearEnvironmentalInspection.Text = Resources.Health.year_risk_factor_relate_work;
                        lbResultEnvironmentalInspection.Text = Resources.Health.result_risk_factor;
                        lbDurationRisk.Text = Resources.Health.duration_risk_factor;
                        lbWorkplaceMonitoringResults.Text = Resources.Health.monitoring_results;
                        lbInspectionAttachment.Text = Resources.Health.file_risk_factor;




                        DataTable dtOccu = new DataTable("occupational_health");  //*** DataTable Map DataSet.xsd ***//

                        dtOccu.Columns.Add(new DataColumn("occupational_health_report", typeof(System.String)));
                        dtOccu.Columns.Add(new DataColumn("abnormalaudiogram", typeof(System.String)));
                        dtOccu.Columns.Add(new DataColumn("abnormal_pulmonary_function", typeof(System.String)));
                        dtOccu.Columns.Add(new DataColumn("hearing_threshold_level", typeof(System.String)));
                        dtOccu.Columns.Add(new DataColumn("file_health_check", typeof(System.String)));
                        dtOccu.Columns.Add(new DataColumn("have_repeat_health_check", typeof(System.String)));
                        dtOccu.Columns.Add(new DataColumn("not_repeat_health_check", typeof(System.String)));
                        dtOccu.Columns.Add(new DataColumn("flie_repeat_health_check", typeof(System.String)));
                        dtOccu.Columns.Add(new DataColumn("chronic_diseases_ear", typeof(System.String)));
                        dtOccu.Columns.Add(new DataColumn("smoked_cigarettes", typeof(System.String)));
                      




                        var o = from c in dbConnect.occupational_health_report_actions
                                join t in dbConnect.occupational_health_reports on c.occupational_health_report_id equals t.id

                                where c.health_id == Convert.ToInt32(id) && c.status == "A"
                                orderby c.id descending
                                select new
                                {
                                    c.id,
                                    occupational_health_reports = chageDataLanguage(t.name_th, t.name_en, lang),
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



                 
                        foreach (var rc in o)
                        {

                            DataRow drOccu = dtOccu.NewRow();

                            drOccu["occupational_health_report"] = rc.occupational_health_reports;


                            string abnormal_audiogram = "";
                            if (rc.abnormal_audiogram == "left")
                            {
                                abnormal_audiogram = Resources.Health.lbleft_ear;
                            }
                            else if (rc.abnormal_audiogram == "right")
                            {
                                abnormal_audiogram = Resources.Health.lbright_ear;
                            }
                            else if (rc.abnormal_audiogram == "both")
                            {
                                abnormal_audiogram = Resources.Health.lbboth_ear;
                            }

                            drOccu["abnormalaudiogram"] = abnormal_audiogram;



                            string abnormal_pulmonary_function = "";
                            if (rc.abnormal_pulmonary_function == "obstructive")
                            {
                                abnormal_pulmonary_function = Resources.Health.lbobstructive;
                            }
                            else if (rc.abnormal_pulmonary_function == "restrictive")
                            {
                                abnormal_pulmonary_function = Resources.Health.lbrestrictive;
                            }
                            else if (rc.abnormal_pulmonary_function == "obstructive_restrictive")
                            {
                                abnormal_pulmonary_function = Resources.Health.lbobstructive_restrictive;
                            }

                            drOccu["abnormal_pulmonary_function"] = abnormal_pulmonary_function;

                            drOccu["hearing_threshold_level"] = rc.hearing_threshold_level;
                            drOccu["file_health_check"] = rc.file_health_check;

                            string health_check_yes = "";
                            string health_check_no = "";
                            if (rc.repeat_health_check == "Y")
                            {
                                health_check_yes = Resources.Health.lbyes;
                            }


                            if (rc.repeat_health_check == "N")
                            {
                                health_check_no = Resources.Health.lbno;
                            }

                            drOccu["have_repeat_health_check"] = health_check_yes;
                            drOccu["not_repeat_health_check"] = health_check_no;
                            drOccu["flie_repeat_health_check"] = rc.flie_repeat_health_check;


                            string chronic_diseases_ear = "";
                            if (rc.chronic_diseases_ear == "N")
                            {
                                chronic_diseases_ear = Resources.Health.lbno;

                            }
                            else if (rc.chronic_diseases_ear == "Y")
                            {
                                chronic_diseases_ear = Resources.Health.lbyes + " : " + rc.specify_chronic_diseases_ear;
                            }

                           
                            drOccu["chronic_diseases_ear"] = chronic_diseases_ear;



                            string smoked_cigarettes = "";
                            if (rc.smoked_cigarettes == "NO")
                            {
                                smoked_cigarettes = Resources.Health.lbno;

                            }
                            else if (rc.smoked_cigarettes == "YES_SMOKING")
                            {
                                smoked_cigarettes = Resources.Health.lbyes_smoking + " " + rc.cigarette_per_day + " " + Resources.Health.lbcigarettes_per_day;
                            }
                            else if (rc.smoked_cigarettes == "YES_SMOKED")
                            {
                                smoked_cigarettes = Resources.Health.lbyes_smoked + " " + rc.smoked_years + " " + Resources.Health.lbyears + " " + rc.smoked_months + " " + Resources.Health.lbmonths;
                            }
                            else if (rc.smoked_cigarettes == "SMOKED_OTHER")
                            {
                                smoked_cigarettes = Resources.Health.lbsmoked_other + " : " + rc.smoked_cigarettes_other;
                            }


                            drOccu["smoked_cigarettes"] = smoked_cigarettes;





                            if (rc.file_health_check != "")
                            {
                                if (rc.file_health_check != "")
                                {
                                    string[] fhealth = rc.file_health_check.Split(',');

                                    for (int i = 0; i < fhealth.Length; i++)
                                    {
                                        occufiles.Add(fhealth[i]);

                                    }
                                    
                                }
                            }



                            if (rc.flie_repeat_health_check != "")
                            {
                                string[] frepeat = rc.flie_repeat_health_check.Split(',');

                                for (int i = 0; i < frepeat.Length; i++)
                                {
                                    occufiles.Add(frepeat[i]);
                                }//endfor

                            }



                            dtOccu.Rows.Add(drOccu);


                        }




                        cryRpt.Subreports["occupational_health_subreport"].Database.Tables["occupational_health"].SetDataSource(dtOccu);

                        TextObject lb_occupational_health = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lb_occupational_health"];
                        TextObject lboccupational_health_report = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lboccupational_health_report"];
                        TextObject lbabnormalaudiogram = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lbabnormalaudiogram"];
                        TextObject lbabnormal_pulmonary_function = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lbabnormal_pulmonary_function"];
                        TextObject lbhearing_threshold_level = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lbhearing_threshold_level"];
                        TextObject lbfile_health_check = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lbfile_health_check"];
                        TextObject lbhave_repeat_health_check = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lbhave_repeat_health_check"];
                        TextObject lbnot_repeat_health_check = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lbnot_repeat_health_check"];
                        TextObject lbfile_repeat_health_check = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lbfile_repeat_health_check"];
                        TextObject lbchronic_diseases_ear = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lbchronic_diseases_ear"];
                        TextObject lbsmoked_cigarettes = (TextObject)cryRpt.Subreports["occupational_health_subreport"].ReportDefinition.ReportObjects["lbsmoked_cigarettes"];

                        lb_occupational_health.Text = Resources.Health.occupational_health_form;
                        lboccupational_health_report.Text = Resources.Health.occupational_health_report;
                        lbabnormalaudiogram.Text = Resources.Health.lbabnormalaudiogram;
                        lbabnormal_pulmonary_function.Text = Resources.Health.lbabnormal_pulmonary_function;
                        lbhearing_threshold_level.Text = Resources.Health.lbhearing_threshold_level;
                        lbfile_health_check.Text = Resources.Health.file_health_check;
                        lbhave_repeat_health_check.Text = Resources.Health.have_repeat_health_check;
                        lbnot_repeat_health_check.Text = Resources.Health.not_repeat_health_check;
                        lbfile_repeat_health_check.Text = Resources.Health.file_repeat_result_health;
                        lbchronic_diseases_ear.Text = Resources.Health.lbchronic_diseases_ear;
                        lbsmoked_cigarettes.Text = Resources.Health.lbsmoked_cigarettes;






                        DataTable dtProcessAction = new DataTable("process_action_health");  //*** DataTable Map DataSet.xsd ***//

                        dtProcessAction.Columns.Add(new DataColumn("type_control", typeof(System.String)));
                        dtProcessAction.Columns.Add(new DataColumn("action", typeof(System.String)));
                        dtProcessAction.Columns.Add(new DataColumn("responsible_person", typeof(System.String)));
                        dtProcessAction.Columns.Add(new DataColumn("due_date", typeof(System.String)));
                        dtProcessAction.Columns.Add(new DataColumn("status", typeof(System.String)));
                        dtProcessAction.Columns.Add(new DataColumn("date_complete", typeof(System.String)));
                        dtProcessAction.Columns.Add(new DataColumn("doctor_opinion_file", typeof(System.String)));
                        dtProcessAction.Columns.Add(new DataColumn("recovery_plan_file", typeof(System.String)));
                        dtProcessAction.Columns.Add(new DataColumn("attachment_file", typeof(System.String)));
                        dtProcessAction.Columns.Add(new DataColumn("remark", typeof(System.String)));


                        var pr = from c in dbConnect.process_action_healths
                                join ee in dbConnect.employees on c.employee_id equals ee.employee_id into joinE
                                from ee in joinE.DefaultIfEmpty()
                                join s in dbConnect.action_health_status on c.action_status_id equals s.id
                                join t in dbConnect.type_control_healths on c.type_control_id equals t.id

                                where c.health_id == Convert.ToInt32(id) //&& r.status == "A"
                                orderby c.id descending
                                select new
                                {
                                    c.id,
                                    type_control = chageDataLanguage(t.name_th, t.name_en, lang),
                                    c.action,
                                    c.responsible_person,
                                    due_date = c.due_date.ToString(),
                                    status = chageDataLanguage(s.name_th, s.name_en, lang),
                                    c.doctor_opinion_file,
                                    c.recovery_plan_file,
                                    c.attachment_file,
                                    c.remark,
                                    c.action_status_id,
                                    due_date2 = c.due_date,
                                    date_complete = c.date_complete.ToString(),

                                };


                    
                        foreach (var rc in pr)
                        {
                            DataRow drProcessAction = dtProcessAction.NewRow();
                            drProcessAction["type_control"] = rc.type_control;
                            drProcessAction["action"] = rc.action;
                            drProcessAction["responsible_person"] = rc.responsible_person;
                            


                            if (!string.IsNullOrEmpty(rc.due_date))
                            {
                                string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date), lang);
                                drProcessAction["due_date"] = duedate;
                            }
                            else
                            {
                                drProcessAction["due_date"] = "";
                            }



                            string status_action = "";
                            if (rc.action_status_id != 2 && rc.action_status_id != 3)//cancel,close
                            {
                                status_action = rc.status;
                                if (string.IsNullOrEmpty(rc.date_complete))
                                {
                                    if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2).Date)
                                    {
                                        status_action = chageDataLanguage("ล่าช้า", "delay", lang);
                                    }
                                }
                                else
                                {
                                    if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                                    {
                                        status_action = chageDataLanguage("ล่าช้า", "delay", lang);
                                    }

                                }
                            }
                            else
                            {
                                status_action = rc.status;

                            }

                            drProcessAction["status"] = status_action;

                            if (!string.IsNullOrEmpty(rc.date_complete))
                            {
                                string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete), lang);
                                drProcessAction["date_complete"] = date;
                            }
                            else
                            {
                                drProcessAction["date_complete"] = "";
                            }

                            drProcessAction["doctor_opinion_file"] = rc.doctor_opinion_file;
                            drProcessAction["recovery_plan_file"] = rc.recovery_plan_file;
                            drProcessAction["attachment_file"] = rc.attachment_file;


                            if (rc.doctor_opinion_file != "")
                            {
                                action.Add(rc.doctor_opinion_file);
                            }

                            if (rc.recovery_plan_file != "")
                            {
                                action.Add(rc.recovery_plan_file);
                            }
                            if (rc.attachment_file != "")
                            {
                                action.Add(rc.attachment_file);
                            }

                            drProcessAction["remark"] = rc.remark;
                            dtProcessAction.Rows.Add(drProcessAction);

                    

                        }

                        cryRpt.Subreports["process_action_health_subreport"].Database.Tables["process_action_health"].SetDataSource(dtProcessAction);






                      //  TextObject lb_process_action = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_process_action"];

                        TextObject lb_type_control = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_type_control"];
                        TextObject lb_action = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_action"];
                        TextObject lb_responsible_person = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_responsible_person"];

                        TextObject lb_due_date = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_due_date"];
                        TextObject lb_status_pro = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_status"];
                        TextObject lb_date_complete = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_date_complete"];
                        TextObject lb_plan_attachment = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_plan_attachment"];
                        TextObject lb_opinion_attachment = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_opinion_attachment"];
                        TextObject lb_attachment = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_attachment"];
                        TextObject lb_remark = (TextObject)cryRpt.Subreports["process_action_health_subreport"].ReportDefinition.ReportObjects["lb_remark"];

                       // lb_process_action.Text = Resources.Health.process_action_form;
                        lb_type_control.Text = Resources.Health.typecontrol;
                        lb_action.Text = Resources.Health.action;
                        lb_responsible_person.Text = Resources.Health.responsible_person;
                        lb_due_date.Text = Resources.Health.due_date;
                        lb_status_pro.Text = Resources.Health.status;
                        lb_date_complete.Text = Resources.Health.date_complete;
                        lb_plan_attachment.Text = Resources.Health.file_recovery_plan;
                        lb_opinion_attachment.Text = Resources.Health.file_opinion_doctor;
                        lb_attachment.Text = Resources.Health.file_action_close;
                        lb_remark.Text = Resources.Health.remark;







                        TextObject position = (TextObject)cryRpt.Subreports["form3_subreport"].ReportDefinition.ReportObjects["lb_positon"];
                        TextObject name_surname = (TextObject)cryRpt.Subreports["form3_subreport"].ReportDefinition.ReportObjects["lb_name_surname"];
                        TextObject date_form3 = (TextObject)cryRpt.Subreports["form3_subreport"].ReportDefinition.ReportObjects["lb_date"];
                        TextObject approval = (TextObject)cryRpt.Subreports["form3_subreport"].ReportDefinition.ReportObjects["lb_approval"];
                        TextObject remark_form4 = (TextObject)cryRpt.Subreports["form3_subreport"].ReportDefinition.ReportObjects["lb_remark"];

                    
                        position.Text = Resources.Hazard.postion;
                        name_surname.Text = Resources.Hazard.name_surname;
                        date_form3.Text = Resources.Hazard.date;
                        approval.Text = Resources.Hazard.approval;
                        remark_form4.Text = Resources.Hazard.remark;


                        DataTable dtForm3 = new DataTable("form3");  //*** DataTable Map DataSet.xsd ***//


                        dtForm3.Columns.Add(new DataColumn("position", typeof(System.String)));
                        dtForm3.Columns.Add(new DataColumn("name_surname", typeof(System.String)));
                        dtForm3.Columns.Add(new DataColumn("date", typeof(System.String)));
                        dtForm3.Columns.Add(new DataColumn("approval", typeof(System.String)));
                        dtForm3.Columns.Add(new DataColumn("remark", typeof(System.String)));

                        var fo = from c in dbConnect.log_request_close_healths
                                 join em in dbConnect.employees on c.employee_id equals em.employee_id
                                 join g in dbConnect.groups on c.group_id equals g.id

                                 where c.health_id == Convert.ToInt32(id)
                                 orderby c.created_at descending
                                 select new
                                 {
                                     id = c.id,
                                     first_name = chageDataLanguage(em.first_name_th, em.first_name_en, lang),
                                     last_name = chageDataLanguage(em.last_name_th, em.last_name_en, lang),
                                     prefix = chageDataLanguage(em.prefix_th, em.prefix_en, lang),
                                     c.status_process,
                                     created_at = FormatDates.getDateShow(c.created_at.ToString(), lang),
                                     c.remark,
                                     g.name


                                 };




                        foreach (var rc in fo)
                        {
                            string name = rc.prefix + " " + rc.first_name + " " + rc.last_name;
                            string status_form3 = chageDataLanguageStatusHealth(rc.status_process, lang);


                            DataRow drForm3 = dtForm3.NewRow();
                            drForm3["position"] = rc.name;
                            drForm3["name_surname"] = name;
                            drForm3["date"] = rc.created_at;
                            drForm3["approval"] = status_form3;
                            drForm3["remark"] = rc.remark;

                            dtForm3.Rows.Add(drForm3);

                        }

                        cryRpt.Subreports["form3_subreport"].Database.Tables["form3"].SetDataSource(dtForm3);
        

                    }



                    ExportOptions CrExportOptions;
                    DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                    PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                    string pathexport = System.Configuration.ConfigurationManager.AppSettings["pathexport"];
                    string path_write = string.Format("{0}" + pathexport + "health\\" + health_doc_number + ".pdf", Server.MapPath(@"\"));

                    CrDiskFileDestinationOptions.DiskFileName = path_write;
                    CrExportOptions = cryRpt.ExportOptions;
                    {
                        CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                        CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                        CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                        CrExportOptions.FormatOptions = CrFormatTypeOptions;
                    }
                    cryRpt.Export();


                    string path_zip = string.Format("{0}" + pathexport + "health\\" + health_doc_number + ".zip", Server.MapPath(@"\"));

                   

                    if (File.Exists(path_zip))
                    {
                        File.Delete(path_zip);
                    }

                    using (ZipArchive archive = ZipFile.Open(path_zip, ZipArchiveMode.Create))
                    {
                        archive.CreateEntryFromFile(path_write, health_doc_number + ".pdf");

                        string path_table = string.Format("{0}" + pathupload + path_folder + "\\", Server.MapPath(@"\"));



                        foreach (string a in action)
                        {
                            if (File.Exists(path_table + a))
                            {
                                archive.CreateEntryFromFile(path_table + a, a);
                            }
                        }

                        foreach (string a in riskfiles)
                        {
                            if (File.Exists(path_table + a))
                            {
                                archive.CreateEntryFromFile(path_table + a, a);
                            }
                        }

                        foreach (string a in occufiles)
                        {
                            if (File.Exists(path_table + a))
                            {
                                archive.CreateEntryFromFile(path_table + a, a);
                            }
                        }



                    }

                   

                    Response.ContentType = "application/zip";
                    Response.AddHeader("Content-Disposition", "filename=" + country + "_" + health_doc_number + ".zip");
                    Response.TransmitFile(path_zip);



                }


            }//ispostback
        }




        public string getEmployeeByTypeLogin(string employee_id, string type_login, string lang)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string name = "";

                if (type_login == "contractor")
                {
                    var t = from c in dbConnect.contractors
                            where c.id == Convert.ToInt32(employee_id)
                            select new
                            {
                                prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),

                            };

                    foreach (var o in t)
                    {
                        name = o.prefix + " " + o.first_name + " " + o.last_name;

                    }

                }
                else
                {
                    var e = from c in dbConnect.employees
                            where c.employee_id == employee_id
                            select new
                            {
                                prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),

                            };

                    foreach (var o in e)
                    {
                        name = o.prefix + " " + o.first_name + " " + o.last_name;

                    }

                }


                return name;
            }
        }


        public string chageDataLanguage(string vTH, string vEN, string lang)
        {
            string vReturn = "";


            if (!string.IsNullOrEmpty(vTH) && !string.IsNullOrEmpty(vEN))
            {
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

            }



            return vReturn;
        }


        public string chageDataLanguageStatusHealth(string v, string lang)
        {
            string vReturn = "";

            if (lang == "th")
            {
                if (v == "P")
                {
                    vReturn = "อนุมัติ";
                }
                else
                {
                    vReturn = "ไม่อนุมัติ";
                }




            }
            else if (lang == "en")
            {

                if (v == "P")
                {
                    vReturn = "Approved";
                }
                else
                {
                    vReturn = "Not approved";
                }

            }
            else if (lang == "si")
            {

                if (v == "P")
                {
                    vReturn = "Approved";
                }
                else
                {
                    vReturn = "Not approved";
                }

            }


            return vReturn;
        }









    }
}