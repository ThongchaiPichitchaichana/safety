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
    public partial class IncidentExport1 : System.Web.UI.Page
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
             string incident_doc_number = "";
             ArrayList factfinding = new ArrayList();
             ArrayList action = new ArrayList();
             ArrayList img_form1 = new ArrayList();
             string commitee = "";
             string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathupload"];
             string pathfolder = "";

             if (!IsPostBack)
             {
                 using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
                 {

                     var q = from c in dbConnect.incidents
                             //join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                             //from e in joinE.DefaultIfEmpty()
                             join s in dbConnect.incident_status on c.process_status equals s.id
                             join co in dbConnect.level_incidents on c.consequence_level equals co.id into joinCo
                             from co in joinCo.DefaultIfEmpty()
                             join fu in dbConnect.functions on c.form2_function_id equals fu.function_id into joinFu
                             from fu in joinFu.DefaultIfEmpty()
                             join de in dbConnect.departments on c.form3_department_id equals de.department_id into joinDe
                             from de in joinDe.DefaultIfEmpty()
                             join fa in dbConnect.fatality_prevention_elements on c.fatality_prevention_element_id equals fa.id into joinFa
                             from fa in joinFa.DefaultIfEmpty()
                             where c.id == Convert.ToInt32(id)
                             select new
                             {
                                 incident_datetime = c.incident_date,
                                 report_date = c.report_date,
                                 c.company_id,
                                 c.function_id,
                                 c.department_id,
                                 c.division_id,
                                 c.section_id,
                                 c.incident_area,
                                 c.incident_name,
                                 c.incident_detail,
                                 c.employee_id,
                                 c.process_status,
                                 c.typeuser_login,
                                 c.doc_no,
                                 status = chageDataLanguage(s.name_th, s.name_en, lang),
                                 phone = c.phone,
                                 c.work_relate,
                                 c.responsible_area,
                                 c.impact,
                                 c.injury_fatality_involve,
                                 c.effect_environment,
                                 c.level_environment,
                                 c.level_damange,
                                 c.other_impact,
                                 c.critical,
                                 c.external_reportable,
                                 c.immediate_temporary,
                                 c.consequence_level,
                                 c.currency,
                                 c.culpability,
                                 c.road_accident,
                                 c.fatality_prevention_element_id,
                                 c.faltality_prevention_element_other,
                                 c.contributing_factor,
                                 c.form2_function_id,
                                 c.form3_department_id,
                                 c.step_form,
                                 c.submit_report_form2,
                                 c.confirm_investigate_form2,
                                 c.confirm_by_groupohs_form2,
                                 c.request_close_form3,
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

                                 activity_location_company_name_en = c.activity_location_company_name_en == null ? "" : c.activity_location_company_name_en,
                                 activity_location_company_name_th = c.activity_location_company_name_th == null ? "" : c.activity_location_company_name_th,
                                 activity_location_function_name_en = c.activity_location_function_name_en == null ? "" : c.activity_location_function_name_en,
                                 activity_location_function_name_th = c.activity_location_function_name_th == null ? "" : c.activity_location_function_name_th,
                                 activity_location_department_name_en = c.activity_location_company_name_en == null ? "" : c.activity_location_department_name_en,
                                 activity_location_department_name_th = c.activity_location_department_name_th == null ? "" : c.activity_location_department_name_th,
                                 activity_location_division_name_en = c.activity_location_division_name_en == null ? "" : c.activity_location_division_name_en,
                                 activity_location_division_name_th = c.activity_location_division_name_th == null ? "" : c.activity_location_division_name_th,
                                 activity_location_section_name_en = c.activity_location_section_name_en == null ? "" : c.activity_location_section_name_en,
                                 activity_location_section_name_th = c.activity_location_section_name_th == null ? "" : c.activity_location_section_name_th,

                                 level_incident = chageDataLanguage(co.name_th, co.name_en, lang),
                                 form3_function_name = chageDataLanguage(fu.function_th, fu.function_en, lang),
                                 form3_department_name = chageDataLanguage(de.department_th, de.department_en, lang),
                                 fatality_prevention_element = chageDataLanguage(fa.name_th, fa.name_en, lang),
                                 c.investigation_committee_file,

                                 c.owner_activity



                             };

                  
                     ReportDocument cryRpt;
                     cryRpt = new ReportDocument();
                     cryRpt.Load(Server.MapPath("~/IncidentExport.rpt"));


                     TextObject report_name = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["ReportName"]);
                     TextObject lb_doc_no = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_doc_no"]);
                     TextObject lb_status = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_status"]);
                     TextObject lb_date_incident = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_date_incident"]);
                     TextObject lb_date_report = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_date_report"]);
                     TextObject lb_company = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_company"]);
                     TextObject lb_function = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_function"]);
                     TextObject lb_department = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_department"]);
                     TextObject lb_division = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_division"]);
                     TextObject lb_section = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_section"]);
                     TextObject lb_location = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_location"]);
                     TextObject lb_title = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_title"]);
                     TextObject lb_detail = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_detail"]);
                     TextObject lb_report_name = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_reporter_name"]);
                     TextObject lb_phone_number = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_phone"]);

                     TextObject lb_activity_company = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_activity_company"]);
                     TextObject lb_activity_function = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_activity_function"]);
                     TextObject lb_activity_department = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_activity_department"]);
                     TextObject lb_activity_division = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_activity_division"]);
                     TextObject lb_activity_section = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_activity_section"]);

                     TextObject lbowner_activity = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lbowner_activity"]);
                     TextObject lb_area = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_area"]);

                     report_name.Text = Resources.Main.lbIncidentExport;
                     lb_doc_no.Text = Resources.Incident.doc_no;
                     lb_status.Text = Resources.Incident.status;
                     lb_date_incident.Text = Resources.Incident.incident_date;
                     lb_date_report.Text = Resources.Incident.report_date;
                     lb_company.Text = Resources.Incident.lbCompany;
                     lb_function.Text = Resources.Incident.lbfucntion;
                     lb_department.Text = Resources.Incident.lbdepartment;
                     lb_division.Text = Resources.Incident.lbdivision;
                     lb_section.Text = Resources.Incident.lbsection;
                     lb_location.Text = Resources.Incident.incidentarea;
                     lb_title.Text = Resources.Incident.incidentname;
                     lb_detail.Text = Resources.Incident.incidentdetail;
                     lb_report_name.Text = Resources.Incident.lbNameReportHeader;
                     lb_phone_number.Text = Resources.Incident.incidentphone;

                     lb_activity_company.Text = Resources.Incident.lbActivityCompany;
                     lb_activity_function.Text = Resources.Incident.lbActivityFunction;
                     lb_activity_department.Text = Resources.Incident.lbActivityDepartment;
                     lb_activity_division.Text = Resources.Incident.lbActivityDivision;
                     lb_activity_section.Text = Resources.Incident.lbActivitySection;

                     lbowner_activity.Text = Resources.Incident.owner_activity;


                     lb_area.Text = Resources.Incident.responsible_area;


                     TextObject doc_number = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["doc_no"]);
                     TextObject status = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["status"]);
                     TextObject date_incident = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["date_incident"]);
                     TextObject date_report = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["date_report"]);
                     TextObject company = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["company"]);
                     TextObject function = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["function"]);
                     TextObject department = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["department"]);
                     TextObject division = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["division"]);
                     TextObject section = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["section"]);
                     TextObject location = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["location"]);
                     TextObject title = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["title"]);
                     TextObject detail = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["detail"]);
                     TextObject reporter_name = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["reporter_name"]);
                     TextObject phone = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["phone"]);

                     TextObject activity_company = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["activity_company"]);
                     TextObject activity_function = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["activity_function"]);
                     TextObject activity_department = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["activity_department"]);
                     TextObject activity_division = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["activity_division"]);
                     TextObject activity_section = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["activity_section"]);

                     TextObject owner_activity = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["owner_activity"]);
                     TextObject area = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["area"]);


                     foreach (var v in q)
                     {
                         incident_doc_number = v.doc_no;
                         string user_name_modify = "";
                         string datetime_modify = "";

                         var doc_no = dbConnect.incident_details.Max(x => x.id);

                         var d = (from c in dbConnect.incident_details
                                  where c.incident_id == Convert.ToInt32(id)
                                  orderby c.id descending
                                  select c).Take(1);



                         foreach (var g in d)
                         {
                             if (g.type_login == "contractor")
                             {
                                 var t = from c in dbConnect.contractors
                                         where c.id == Convert.ToInt32(g.employee_id)
                                         select new
                                         {
                                             prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                             first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                             last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),
                                             action_time = FormatDates.getDatetimeShow(Convert.ToDateTime(g.action_time), lang),
                                         };

                                 foreach (var o in t)
                                 {
                                     user_name_modify = o.prefix + " " + o.first_name + " " + o.last_name;
                                     datetime_modify = o.action_time;
                                 }

                             }
                             else
                             {
                                 var ee = from c in dbConnect.employees
                                          where c.employee_id == g.employee_id
                                          select new
                                          {
                                              prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                              first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                              last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang),
                                              action_time = FormatDates.getDatetimeShow(Convert.ToDateTime(g.action_time), lang),
                                          };

                                 foreach (var o in ee)
                                 {
                                     user_name_modify = o.prefix + " " + o.first_name + " " + o.last_name;
                                     datetime_modify = o.action_time;
                                 }


                             }

                         }//end for each



                         string step = "";


                         if (v.process_status != 2 && v.process_status != 3)//ไม่ใช้ close กับ reject
                         {

                             if (v.step_form == 1)//supervisor
                             {
                                 string v_step = chageDataLanguage("รายงานอุบัติการณ์", "Incident Report", lang);
                                 step = step + "(" + v_step + " - Area Supervisor)";

                             }
                             else if (v.step_form == 2)
                             {
                                 string v_step = chageDataLanguage("ตรวจสอบรายงานอุบัติการณ์", "Verify Incident Report", lang);

                                 if (v.submit_report_form2 == null)
                                 {
                                     step = step + "(" + v_step + " - Area Supervisor)";
                                 }

                                 if (Session["country"].ToString() == "thailand")
                                 {
                                     if (v.submit_report_form2 != null && v.confirm_investigate_form2 == null)
                                     {
                                         step = step + "(" + v_step + " - Area OH&S)";
                                     }
                                 }
                                 else if (Session["country"].ToString() == "srilanka")
                                 {
                                     if (v.submit_report_form2 != null && v.confirm_investigate_form2 == null)
                                     {
                                         step = step + "(" + v_step + " - Area Manager)";
                                     }

                                 }


                                 if (v.confirm_investigate_form2 != null)//กด confirm ไปละ แต่ยังอยู่ 2 แสดงว่ามีซีเรียสเคสรอ group มากด confirm
                                 {
                                     step = step + "(" + v_step + " - Group OH&S)";
                                 }


                             }
                             else if (v.step_form == 3)
                             {
                                 string v_step = chageDataLanguage("สอบสวนและกำหนดมาตรการการแก้ไข", "Investigation and Corrective/Preventive Action", lang);

                                 if (Session["country"].ToString() == "thailand")
                                 {
                                     step = step + "(" + v_step + " - Area OH&S)";
                                 }
                                 else if (Session["country"].ToString() == "srilanka")
                                 {
                                     step = step + "(" + v_step + " - Area Manager)";
                                 }



                             }
                             else if (v.step_form == 4)
                             {
                                 string v_step = chageDataLanguage("ขอปิดรายงานอุบัติการณ์", "Request to Close Incident Report", lang);

                                 bool check_close = true;

                                 var s = from c in dbConnect.close_step_incidents
                                         where c.country == Session["country"].ToString()
                                         orderby c.step descending
                                         select c;

                                 foreach (var r in s)
                                 {
                                     var w = from c in dbConnect.log_request_close_incidents
                                             where c.incident_id == v.id && c.status == "A"
                                             // && c.group_id == r.group_id
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
                         date_incident.Text = FormatDates.getDatetimeShow(Convert.ToDateTime(v.incident_datetime), lang);
                         date_report.Text = FormatDates.getDatetimeShow(Convert.ToDateTime(v.report_date), lang);

                         location.Text = v.incident_area;
                         title.Text = v.incident_name;
                         detail.Text = v.incident_detail;
                         reporter_name.Text = getEmployeeByTypeLogin(v.employee_id, v.typeuser_login, lang);
                         phone.Text = v.phone;


                         if (v.responsible_area == "IN")
                         {
                             area.Text = Resources.Incident.onsite;
                             company.Text = chageDataLanguage(v.location_company_name_th, v.location_company_name_en, lang);
                             function.Text = chageDataLanguage(v.location_function_name_th, v.location_function_name_en, lang);
                             department.Text = chageDataLanguage(v.location_department_name_th, v.location_department_name_en, lang);
                             division.Text = chageDataLanguage(v.location_division_name_th, v.location_division_name_en, lang);
                             section.Text = chageDataLanguage(v.location_section_name_th, v.location_section_name_en, lang);
                         }
                         else if (v.responsible_area == "OUT")
                         {
                             area.Text = Resources.Incident.offsite;
                             company.Text = "-";
                             function.Text = "-";
                             department.Text = "-";
                             division.Text = "-";
                             section.Text = "-";
                         }
                         else
                         {
                             area.Text = "";
                             company.Text = chageDataLanguage(v.location_company_name_th, v.location_company_name_en, lang);
                             function.Text = chageDataLanguage(v.location_function_name_th, v.location_function_name_en, lang);
                             department.Text = chageDataLanguage(v.location_department_name_th, v.location_department_name_en, lang);
                             division.Text = chageDataLanguage(v.location_division_name_th, v.location_division_name_en, lang);
                             section.Text = chageDataLanguage(v.location_section_name_th, v.location_section_name_en, lang);

                         }


                         if (v.owner_activity == "KNOWN")
                         {
                             owner_activity.Text = Resources.Incident.owner_activity_known;
                             activity_company.Text = chageDataLanguage(v.activity_location_company_name_th, v.activity_location_company_name_en, lang);
                             activity_function.Text = chageDataLanguage(v.activity_location_function_name_th, v.activity_location_function_name_en, lang);
                             activity_department.Text = chageDataLanguage(v.activity_location_department_name_th, v.activity_location_department_name_en, lang);
                             activity_division.Text = chageDataLanguage(v.activity_location_division_name_th, v.activity_location_division_name_en, lang);
                             activity_section.Text = chageDataLanguage(v.activity_location_section_name_th, v.activity_location_section_name_en, lang);


                         }
                         else if (v.owner_activity == "UNKNOWN")
                         {
                             owner_activity.Text = Resources.Incident.owner_activity_unknown;
                             activity_company.Text = "-";
                             activity_function.Text = "-";
                             activity_department.Text = "-";
                             activity_division.Text = "-";
                             activity_section.Text = "-";


                         }
                         else
                         {
                             owner_activity.Text = "";
                             activity_company.Text = "-";
                             activity_function.Text = "-";
                             activity_department.Text = "-";
                             activity_division.Text = "-";
                             activity_section.Text = "-";
                         }



                         string name_folder = v.employee_id + "_" + v.report_date.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB"));
                         pathfolder = string.Format("{0}" + pathupload + name_folder, Server.MapPath(@"\"));

                         DataTable dtMap = new DataTable("form1");  //*** DataTable Map DataSet.xsd ***//
                         DataRow dr = null;
                         dtMap.Columns.Add(new DataColumn("img1", typeof(System.Byte[])));
                         dtMap.Columns.Add(new DataColumn("img2", typeof(System.Byte[])));
                         dtMap.Columns.Add(new DataColumn("img3", typeof(System.Byte[])));
                         dtMap.Columns.Add(new DataColumn("img4", typeof(System.Byte[])));
                         dtMap.Columns.Add(new DataColumn("img5", typeof(System.Byte[])));

                         if (Directory.Exists(pathfolder))
                         {

                             string[] images = Directory.GetFiles(pathfolder, "*")
                                                      .Select(Path.GetFileName).OrderByDescending(Path.GetFileName).Take(5)
                                                      .ToArray();




                             dr = dtMap.NewRow();

                             int count_img = 1;
                             foreach (var de in images)
                             {
                                 string pathfolder_new = pathfolder + "\\" + de;

                                 FileStream fiStream = new FileStream(pathfolder_new, FileMode.Open);
                                 BinaryReader binReader = new BinaryReader(fiStream);
                                 byte[] pic = { };
                                 pic = binReader.ReadBytes((int)fiStream.Length);
                                 dr["img" + count_img] = pic;

                                 count_img++;
                                 img_form1.Add(de);
                                 fiStream.Close();
                                 binReader.Close();


                             }

                             dtMap.Rows.Add(dr);

                         }

                         cryRpt.Database.Tables["form1"].SetDataSource(dtMap);


                         TextObject lb_impact = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_impact"]);
                         TextObject lb_injury = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_injury"]);
                         TextObject lb_property = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_property"]);


                         lb_impact.Text = Resources.Incident.impact_incident;
                         lb_injury.Text = Resources.Incident.injury_fatality;
                         lb_property.Text = Resources.Incident.consequence_property_environment;



                         TextObject impact = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["impact"]);
                         TextObject injury = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["injury"]);
                         TextObject property = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["property"]);





                         if (v.impact == "N")
                         {
                             impact.Text = Resources.Incident.no_impact;
                         }
                         else if (v.impact == "Y")
                         {
                             impact.Text = Resources.Incident.impact;
                         }
                         else
                         {
                             impact.Text = "";
                         }



                         if (v.injury_fatality_involve == "N")
                         {
                             injury.Text = Resources.Incident.no_injury_fatality;
                             SubreportObject sub_injury = ((CrystalDecisions.CrystalReports.Engine.SubreportObject)cryRpt.ReportDefinition.ReportObjects["injury_subreport"]);
                             sub_injury.ObjectFormat.EnableSuppress = true;

                         }
                         else if (v.injury_fatality_involve == "Y")
                         {
                             injury.Text = Resources.Incident.injury_fatality_involve;
                         }
                         else
                         {
                             injury.Text = "";
                         }


                         if (v.effect_environment == "N")
                         {
                             property.Text = Resources.Incident.no_effect_property_environment;
                             SubreportObject sub_prop = ((CrystalDecisions.CrystalReports.Engine.SubreportObject)cryRpt.ReportDefinition.ReportObjects["property_subreport"]);
                             sub_prop.ObjectFormat.EnableSuppress = true;

                         }
                         else if (v.effect_environment == "Y")
                         {
                             property.Text = Resources.Incident.effect_property_environment;
                         }
                         else
                         {
                             property.Text = "";
                         }



                         DataTable dtInjury = new DataTable("injury");  //*** DataTable Map DataSet.xsd ***//

                         dtInjury.Columns.Add(new DataColumn("no", typeof(System.Int32)));
                         dtInjury.Columns.Add(new DataColumn("full_name", typeof(System.String)));
                         dtInjury.Columns.Add(new DataColumn("type_employment", typeof(System.String)));
                         dtInjury.Columns.Add(new DataColumn("function", typeof(System.String)));
                         dtInjury.Columns.Add(new DataColumn("department", typeof(System.String)));
                         dtInjury.Columns.Add(new DataColumn("nature_injury", typeof(System.String)));
                         dtInjury.Columns.Add(new DataColumn("body_part", typeof(System.String)));
                         dtInjury.Columns.Add(new DataColumn("severity_injury", typeof(System.String)));
                         dtInjury.Columns.Add(new DataColumn("day_lost", typeof(System.Int16)));
                         dtInjury.Columns.Add(new DataColumn("remark", typeof(System.String)));




                         var j = from c in dbConnect.injury_persons
                                 join t in dbConnect.type_employments on c.type_employment_id equals t.id
                                 join f in dbConnect.functions on c.function_id equals f.function_id into joinF
                                 join n in dbConnect.nature_injuries on c.nature_injury_id equals n.id
                                 join b in dbConnect.body_parts on c.body_parts_id equals b.id into boDefalt
                                 join s in dbConnect.severity_injuries on c.severity_injury_id equals s.id
                                 from f in joinF.DefaultIfEmpty()
                                 from b in boDefalt.DefaultIfEmpty()
                                 where c.incident_id == Convert.ToInt32(id) && c.status == "A"
                                 orderby c.id descending
                                 select new
                                 {
                                     c.id,
                                     c.full_name,
                                     type_employment_name = chageDataLanguage(t.name_th, t.name_en, lang),
                                     function_name = chageDataLanguage(f.function_th, f.function_en, lang),
                                     nature_injury_name = chageDataLanguage(n.name_th, n.name_en, lang),
                                     body_parts_id = chageDataLanguage(b.name_th, b.name_en, lang),
                                     severity_injury_name = chageDataLanguage(s.name_th, s.name_en, lang),
                                     c.day_lost,
                                     c.remark

                                 };

                         int count_injury = 1;
                         foreach (var rc in j)
                         {
                             DataRow drInjury = dtInjury.NewRow();
                             drInjury["no"] = count_injury;
                             drInjury["full_name"] = rc.full_name;
                             drInjury["type_employment"] = rc.type_employment_name;
                             drInjury["function"] = rc.function_name;
                             drInjury["nature_injury"] = rc.nature_injury_name;
                             drInjury["body_part"] = rc.body_parts_id;
                             drInjury["severity_injury"] = rc.severity_injury_name;
                             drInjury["day_lost"] = rc.day_lost == null ? 0 : rc.day_lost;
                             drInjury["remark"] = rc.remark;
                             dtInjury.Rows.Add(drInjury);
                             count_injury++;
                         }


                         cryRpt.Subreports["injury_subreport"].Database.Tables["injury"].SetDataSource(dtInjury);



                         TextObject no = (TextObject)cryRpt.Subreports["injury_subreport"].ReportDefinition.ReportObjects["no"];
                         TextObject name_injury = (TextObject)cryRpt.Subreports["injury_subreport"].ReportDefinition.ReportObjects["name_injury"];
                         TextObject type_employment = (TextObject)cryRpt.Subreports["injury_subreport"].ReportDefinition.ReportObjects["type_employment"];
                         TextObject lbfunction = (TextObject)cryRpt.Subreports["injury_subreport"].ReportDefinition.ReportObjects["lbfunction"];
                         TextObject nature_injury = (TextObject)cryRpt.Subreports["injury_subreport"].ReportDefinition.ReportObjects["nature_injury"];
                         TextObject body_parts = (TextObject)cryRpt.Subreports["injury_subreport"].ReportDefinition.ReportObjects["body_parts"];
                         TextObject severity_injury = (TextObject)cryRpt.Subreports["injury_subreport"].ReportDefinition.ReportObjects["severity_injury"];
                         TextObject day_lost = (TextObject)cryRpt.Subreports["injury_subreport"].ReportDefinition.ReportObjects["day_lost"];
                         TextObject total_lost_day = (TextObject)cryRpt.Subreports["injury_subreport"].ReportDefinition.ReportObjects["total_lost_day"];
                         // TextObject remark = (TextObject)cryRpt.Subreports["injury_subreport"].ReportDefinition.ReportObjects["remark"];
                         no.Text = "";
                         name_injury.Text = Resources.Incident.name_injury;
                         type_employment.Text = Resources.Incident.type_employment;
                         lbfunction.Text = Resources.Incident.lbfunction_injured;
                         nature_injury.Text = Resources.Incident.nature_injury;
                         body_parts.Text = Resources.Incident.body_parts;
                         severity_injury.Text = Resources.Incident.severity_injury;
                         day_lost.Text = Resources.Incident.day_lost;
                         total_lost_day.Text = Resources.Incident.total_lost_day;

                         //day_lost.Border.BackgroundColor = System.Drawing.Color.Red;
                         // remark.Text = Resources.Incident.remark;



                         TextObject no_property = (TextObject)cryRpt.Subreports["property_subreport"].ReportDefinition.ReportObjects["no"];
                         TextObject list_property_enviroment_damage = (TextObject)cryRpt.Subreports["property_subreport"].ReportDefinition.ReportObjects["list_property_enviroment_damage"];
                         TextObject detail_damage = (TextObject)cryRpt.Subreports["property_subreport"].ReportDefinition.ReportObjects["detail_damage"];
                         TextObject damage_cost = (TextObject)cryRpt.Subreports["property_subreport"].ReportDefinition.ReportObjects["damage_cost"];
                         TextObject total_damage_cost = (TextObject)cryRpt.Subreports["property_subreport"].ReportDefinition.ReportObjects["total_damage_cost"];
                         TextObject lb_currency = (TextObject)cryRpt.Subreports["property_subreport"].ReportDefinition.ReportObjects["currency"];

                         if (v.currency == "1")
                         {
                             lb_currency.Text = chageDataLanguage("บาท", "baht", lang);
                         }
                         else if (v.currency == "2")
                         {
                             lb_currency.Text = chageDataLanguage("ดอลล่า", "dollar", lang);
                         }
                         else if (v.currency == "3")
                         {
                             lb_currency.Text = chageDataLanguage("", "rupee", lang);
                         }

                         total_damage_cost.Text = Resources.Incident.total_damage_cost;
                         no_property.Text = "";
                         list_property_enviroment_damage.Text = Resources.Incident.list_property_enviroment_damage;
                         detail_damage.Text = Resources.Incident.detail_damage;
                         damage_cost.Text = Resources.Incident.damage_cost;

                         DataTable dtProperty = new DataTable("property");  //*** DataTable Map DataSet.xsd ***//

                         dtProperty.Columns.Add(new DataColumn("no", typeof(System.Int32)));
                         dtProperty.Columns.Add(new DataColumn("property_environment_damage", typeof(System.String)));
                         dtProperty.Columns.Add(new DataColumn("detail_damage", typeof(System.String)));
                         dtProperty.Columns.Add(new DataColumn("damage_cost", typeof(System.Double)));


                         var da = from c in dbConnect.damage_lists
                                  where c.incident_id == Convert.ToInt32(id) && c.status == "A"
                                  orderby c.id descending
                                  select new
                                  {
                                      c.id,
                                      c.property_environment_damage,
                                      c.detail_damage,
                                      c.damage_cost


                                  };



                         int count_damage = 1;
                         foreach (var rc in da)
                         {
                             DataRow drProperty = dtProperty.NewRow();
                             drProperty["no"] = count_damage;
                             drProperty["property_environment_damage"] = rc.property_environment_damage;
                             drProperty["detail_damage"] = rc.detail_damage;
                             drProperty["damage_cost"] = rc.damage_cost == null ? 0 : rc.damage_cost;

                             dtProperty.Rows.Add(drProperty);
                             count_damage++;

                         }


                         cryRpt.Subreports["property_subreport"].Database.Tables["property"].SetDataSource(dtProperty);
                         setLevelTable(cryRpt, id);













                         TextObject lb_other_impact = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_other_impact"]);
                         TextObject lb_critical_incident = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_critical_incident"]);
                         TextObject lb_external_reportable = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_external_reportable"]);
                         TextObject lb_immediate_temporary_action = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_immediate_temporary_action"]);
                         TextObject lb_consequence_level = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_consequence_level"]);

                         lb_other_impact.Text = Resources.Incident.other_impact;
                         lb_critical_incident.Text = Resources.Incident.critical_incident;
                         lb_external_reportable.Text = Resources.Incident.external_reportable;
                         lb_immediate_temporary_action.Text = Resources.Incident.immediate_temporary_action;
                         lb_consequence_level.Text = Resources.Incident.consequence_level;



                         TextObject other_impact = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["other_impact"]);
                         TextObject critical_incident = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["critical_incident"]);
                         TextObject external_reportable = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["external_reportable"]);
                         TextObject immediate_temporary_action = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["immediate_temporary_action"]);
                         TextObject consequence_level = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["consequence_level"]);



                         string all_other_impacts = "";
                         var i = from c in dbConnect.other_impacts
                                 where c.incident_id == Convert.ToInt32(id)
                                 select new
                                 {
                                     c.other_impact_value

                                 };

                         foreach (var m in i)
                         {
                             string name_other_impact = "";
                             if (m.other_impact_value == "image")
                             {
                                 name_other_impact = Resources.Incident.potential_image;

                             }
                             else if (m.other_impact_value == "legal")
                             {
                                 name_other_impact = Resources.Incident.potential_legal;

                             }
                             else if (m.other_impact_value == "manufacturing")
                             {

                                 name_other_impact = Resources.Incident.potential_issue;
                             }

                             if (all_other_impacts != "")
                             {

                                 all_other_impacts = all_other_impacts + ", " + name_other_impact;

                             }
                             else
                             {
                                 all_other_impacts = name_other_impact;

                             }

                         }

                         other_impact.Text = all_other_impacts;

                         if (v.critical == "N")
                         {
                             critical_incident.Text = Resources.Incident.no;

                         }
                         else if (v.critical == "Y")
                         {
                             critical_incident.Text = Resources.Incident.yes;
                         }
                         else
                         {
                             critical_incident.Text = "";
                         }


                         if (v.external_reportable == "N")
                         {
                             external_reportable.Text = Resources.Incident.no;

                         }
                         else if (v.critical == "Y")
                         {
                             external_reportable.Text = Resources.Incident.yes;
                         }
                         else
                         {
                             external_reportable.Text = "";
                         }

                         immediate_temporary_action.Text = v.immediate_temporary == null ? "" : v.immediate_temporary;
                         consequence_level.Text = v.level_incident == null ? "" : v.level_incident;







                         TextObject lb_fact_finding = (TextObject)cryRpt.Subreports["factfinding_subreport"].ReportDefinition.ReportObjects["lb_fact_finding"];
                         TextObject no_factfinding = (TextObject)cryRpt.Subreports["factfinding_subreport"].ReportDefinition.ReportObjects["no"];
                         TextObject fact_finding = (TextObject)cryRpt.Subreports["factfinding_subreport"].ReportDefinition.ReportObjects["fact_finding"];
                         TextObject source_incident = (TextObject)cryRpt.Subreports["factfinding_subreport"].ReportDefinition.ReportObjects["source_incident"];
                         TextObject event_exposure = (TextObject)cryRpt.Subreports["factfinding_subreport"].ReportDefinition.ReportObjects["event_exposure"];
                         TextObject unsafe_action = (TextObject)cryRpt.Subreports["factfinding_subreport"].ReportDefinition.ReportObjects["unsafe_action"];
                         TextObject unsafe_condition = (TextObject)cryRpt.Subreports["factfinding_subreport"].ReportDefinition.ReportObjects["unsafe_condition"];
                         TextObject evidence = (TextObject)cryRpt.Subreports["factfinding_subreport"].ReportDefinition.ReportObjects["evidence"];

                         lb_fact_finding.Text = Resources.Incident.lb_fact_finding;
                         no_factfinding.Text = "";
                         fact_finding.Text = Resources.Incident.fact_finding;
                         source_incident.Text = Resources.Incident.source_incident;
                         event_exposure.Text = Resources.Incident.event_exposure;
                         unsafe_action.Text = Resources.Incident.unsafe_action;
                         unsafe_condition.Text = Resources.Incident.unsafe_condition;
                         evidence.Text = Resources.Incident.evidence;

                         DataTable dtFactfinding = new DataTable("factfinding");  //*** DataTable Map DataSet.xsd ***//

                         dtFactfinding.Columns.Add(new DataColumn("no", typeof(System.Int32)));
                         dtFactfinding.Columns.Add(new DataColumn("fact_finding", typeof(System.String)));
                         dtFactfinding.Columns.Add(new DataColumn("source_incident", typeof(System.String)));
                         dtFactfinding.Columns.Add(new DataColumn("event_exposure", typeof(System.String)));
                         dtFactfinding.Columns.Add(new DataColumn("unsafe_action", typeof(System.String)));
                         dtFactfinding.Columns.Add(new DataColumn("unsafe_condition", typeof(System.String)));
                         dtFactfinding.Columns.Add(new DataColumn("evidence", typeof(System.String)));

                         var fa = from c in dbConnect.fact_findings
                                  join s in dbConnect.source_incidents on c.source_incident_id equals s.id
                                  join ee in dbConnect.event_exposures on c.event_exposure_id equals ee.id
                                  where c.incident_id == Convert.ToInt32(id) && c.status == "A"
                                  orderby c.id descending
                                  select new
                                  {
                                      c.id,
                                      c.fact_finding_name,
                                      source_incident = chageDataLanguage(s.name_th, s.name_en, lang),
                                      event_exposure = chageDataLanguage(ee.name_th, ee.name_en, lang),
                                      c.unsafe_action,
                                      c.unsafe_condition,
                                      c.evidence_file

                                  };



                         int count_factfinding = 1;
                         foreach (var rc in fa)
                         {

                             DataRow drFactfinding = dtFactfinding.NewRow();
                             drFactfinding["no"] = count_factfinding;
                             drFactfinding["fact_finding"] = rc.fact_finding_name;
                             drFactfinding["source_incident"] = rc.source_incident;
                             drFactfinding["event_exposure"] = rc.event_exposure;

                             if (rc.unsafe_action == "N")
                             {
                                 drFactfinding["unsafe_action"] = "X";
                             }
                             else
                             {
                                 drFactfinding["unsafe_action"] = "/";
                             }

                             if (rc.unsafe_condition == "N")
                             {
                                 drFactfinding["unsafe_condition"] = "X";
                             }
                             else
                             {
                                 drFactfinding["unsafe_condition"] = "/";
                             }

                             //string country = Session["country"].ToString();
                             //string path_factfinding = string.Format("{0}" + pathupload + "\\step3\\" + country+"\\"+v.doc_no, Server.MapPath(@"\"));

                             if (rc.evidence_file != "")
                             {
                                 //if (rc.evidence_file.IndexOf(".pdf") == -1)
                                 //{
                                 //    string pathfolder_new = path_factfinding + "\\" + rc.evidence_file;

                                 //    FileStream fiStream = new FileStream(pathfolder_new, FileMode.Open);
                                 //    BinaryReader binReader = new BinaryReader(fiStream);
                                 //    byte[] pic = { };
                                 //    pic = binReader.ReadBytes((int)fiStream.Length);

                                 //    drFactfinding["evidence"] = pic;
                                 //}
                                 //else
                                 //{

                                 drFactfinding["evidence"] = rc.evidence_file;
                                 //}

                                 factfinding.Add(rc.evidence_file);
                             }


                             dtFactfinding.Rows.Add(drFactfinding);
                             count_factfinding++;



                         }


                         cryRpt.Subreports["factfinding_subreport"].Database.Tables["factfinding"].SetDataSource(dtFactfinding);


                         TextObject lb_root_cause_action = (TextObject)cryRpt.Subreports["rootcauseaction_subreport"].ReportDefinition.ReportObjects["lb_root_cause_action"];
                         TextObject root_cause_action_no = (TextObject)cryRpt.Subreports["rootcauseaction_subreport"].ReportDefinition.ReportObjects["root_cause_action_no"];
                         TextObject root_cause_action = (TextObject)cryRpt.Subreports["rootcauseaction_subreport"].ReportDefinition.ReportObjects["root_cause_action"];
                         lb_root_cause_action.Text = Resources.Incident.lb_root_cause_action;
                         root_cause_action_no.Text = Resources.Incident.root_cause_action_no;
                         root_cause_action.Text = Resources.Incident.root_cause_action;

                         DataTable dtRootcause = new DataTable("rootcauseaction");  //*** DataTable Map DataSet.xsd ***//

                         dtRootcause.Columns.Add(new DataColumn("root_cause_action_no", typeof(System.String)));
                         dtRootcause.Columns.Add(new DataColumn("root_cause_action", typeof(System.String)));


                         var ro = from c in dbConnect.root_cause_actions
                                  where c.incident_id == Convert.ToInt32(id) && c.status == "A"
                                  orderby c.id descending
                                  select new
                                  {
                                      c.id,
                                      c.name,
                                      c.root_cause_number


                                  };


                         foreach (var rc in ro)
                         {
                             DataRow drRootcause = dtRootcause.NewRow();
                             drRootcause["root_cause_action_no"] = rc.root_cause_number;
                             drRootcause["root_cause_action"] = rc.name;
                             dtRootcause.Rows.Add(drRootcause);
                         }

                         cryRpt.Subreports["rootcauseaction_subreport"].Database.Tables["rootcauseaction"].SetDataSource(dtRootcause);



                         TextObject lb_contributing_factor = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_contributing_factor"]);
                         TextObject contributing_factor = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["contributing_factor"]);
                         TextObject lb_corrective_preventive = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["lb_corrective_preventive"];
                         TextObject no_corrective_preventive = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["no"];

                         TextObject tb_corrective_preventive = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["tb_corrective_preventive"];
                         TextObject responsible_person = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["responsible_person"];
                         TextObject lbdepartment_action = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["lbdepartment_action"];
                         TextObject due_date = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["due_date"];
                         TextObject status_cor = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["status"];
                         TextObject date_complete = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["date_complete"];
                         TextObject attachment = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["attachment"];
                         TextObject root_cause_action_cor = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["root_cause_action"];
                         TextObject remark = (TextObject)cryRpt.Subreports["corrective_preventive_subreport"].ReportDefinition.ReportObjects["remark"];

                         lb_contributing_factor.Text = Resources.Incident.contributing_factor;
                         contributing_factor.Text = v.contributing_factor == null ? "" : v.contributing_factor;
                         no_corrective_preventive.Text = "";
                         lb_corrective_preventive.Text = Resources.Incident.lb_corrective_preventive;
                         tb_corrective_preventive.Text = Resources.Incident.tb_corrective_preventive;
                         responsible_person.Text = Resources.Incident.responsible_person;
                         lbdepartment_action.Text = (Resources.Incident.lbdepartment_action).Replace("/", "/ ");
                         due_date.Text = Resources.Incident.due_date;
                         status_cor.Text = Resources.Incident.status;
                         date_complete.Text = Resources.Incident.date_complete;
                         attachment.Text = Resources.Incident.attachment;
                         root_cause_action_cor.Text = Resources.Incident.root_cause_action;
                         remark.Text = Resources.Incident.remark;




                         DataTable dtCorrectivePreventive = new DataTable("corrective_preventive");  //*** DataTable Map DataSet.xsd ***//

                         dtCorrectivePreventive.Columns.Add(new DataColumn("no", typeof(System.Int32)));
                         dtCorrectivePreventive.Columns.Add(new DataColumn("tb_corrective_preventive", typeof(System.String)));
                         dtCorrectivePreventive.Columns.Add(new DataColumn("responsible_person", typeof(System.String)));
                         dtCorrectivePreventive.Columns.Add(new DataColumn("lbdepartment_action", typeof(System.String)));
                         dtCorrectivePreventive.Columns.Add(new DataColumn("due_date", typeof(System.String)));
                         dtCorrectivePreventive.Columns.Add(new DataColumn("status", typeof(System.String)));
                         dtCorrectivePreventive.Columns.Add(new DataColumn("date_complete", typeof(System.String)));
                         dtCorrectivePreventive.Columns.Add(new DataColumn("attachment", typeof(System.String)));
                         dtCorrectivePreventive.Columns.Add(new DataColumn("root_cause_action", typeof(System.String)));
                         dtCorrectivePreventive.Columns.Add(new DataColumn("remark", typeof(System.String)));

                         var co = from c in dbConnect.corrective_prevention_action_incidents
                                  join s in dbConnect.action_status on c.action_status_id equals s.id
                                  join em in dbConnect.employees on c.employee_id equals em.employee_id into joinE
                                  from em in joinE.DefaultIfEmpty()
                                  join o in dbConnect.organizations on em.unit_id equals o.org_unit_id into joinO
                                  from o in joinO.DefaultIfEmpty()
                                  join de in dbConnect.departments on o.department_id equals de.department_id into joinD
                                  from de in joinD.DefaultIfEmpty()
                                  where c.incident_id == Convert.ToInt32(id)
                                  orderby c.id descending
                                  select new
                                  {
                                      c.id,
                                      c.corrective_preventive_action,
                                      c.responsible_person,
                                      due_date = c.due_date,
                                      status = chageDataLanguage(s.name_th, s.name_en, lang),
                                      date_complete = c.date_complete,
                                      c.attachment_file,
                                      c.notify_contractor,
                                      c.remark,
                                      c.action_status_id,
                                      c.root_cause_action,
                                      due_date2 = c.due_date,
                                      department_name = chageDataLanguage(de.department_th, de.department_en, lang)

                                  };


                         int count_corrective = 1;
                         foreach (var rc in co)
                         {
                             DataRow drCorrectivePreventive = dtCorrectivePreventive.NewRow();
                             drCorrectivePreventive["no"] = count_corrective;
                             drCorrectivePreventive["tb_corrective_preventive"] = rc.corrective_preventive_action;
                             drCorrectivePreventive["responsible_person"] = rc.responsible_person;
                             drCorrectivePreventive["lbdepartment_action"] = rc.department_name;


                             if (rc.due_date != null)
                             {
                                 string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date), lang);
                                 drCorrectivePreventive["due_date"] = duedate;
                             }
                             else
                             {
                                 drCorrectivePreventive["due_date"] = "";
                             }

                             string status_action = "";
                             if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                             {
                                 status_action = rc.status;

                                 if (rc.date_complete != null)
                                 {
                                     if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                                     {
                                         status_action = chageDataLanguage("ล่าช้า", "delay", lang);
                                     }

                                 }
                                 else
                                 {
                                     if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2).Date)
                                     {
                                         status_action = chageDataLanguage("ล่าช้า", "delay", lang);
                                     }

                                 }

                             }
                             else
                             {
                                 status_action = rc.status;

                             }
                             drCorrectivePreventive["status"] = status_action;

                             if (rc.date_complete != null)
                             {
                                 string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete), lang);
                                 drCorrectivePreventive["date_complete"] = date;

                             }
                             else
                             {
                                 drCorrectivePreventive["date_complete"] = "";

                             }



                             drCorrectivePreventive["attachment"] = rc.attachment_file;
                             if (rc.attachment_file != "")
                             {
                                 action.Add(rc.attachment_file);
                             }
                             drCorrectivePreventive["root_cause_action"] = rc.root_cause_action;
                             drCorrectivePreventive["remark"] = rc.remark;
                             dtCorrectivePreventive.Rows.Add(drCorrectivePreventive);

                             count_corrective++;

                         }




                         cryRpt.Subreports["corrective_preventive_subreport"].Database.Tables["corrective_preventive"].SetDataSource(dtCorrectivePreventive);






                         TextObject lb_corrective_preventive2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["lb_preventive"];
                         TextObject no_corrective_preventive2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["no"];

                         TextObject tb_corrective_preventive2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["tb_corrective_preventive"];
                         TextObject responsible_person2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["responsible_person"];
                         TextObject lbdepartment_action2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["lbdepartment_action"];
                         TextObject due_date2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["due_date"];
                         TextObject status_cor2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["status"];
                         TextObject date_complete2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["date_complete"];
                         TextObject attachment2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["attachment"];
                         TextObject root_cause_action_cor2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["root_cause_action"];
                         TextObject remark2 = (TextObject)cryRpt.Subreports["preventive_subreport"].ReportDefinition.ReportObjects["remark"];

                      
                         no_corrective_preventive2.Text = "";
                         lb_corrective_preventive2.Text = Resources.Incident.lb_preventive;
                         tb_corrective_preventive2.Text = Resources.Incident.tb_preventive;
                         responsible_person2.Text = Resources.Incident.responsible_person;
                         lbdepartment_action2.Text = (Resources.Incident.lbdepartment_action).Replace("/", "/ ");
                         due_date2.Text = Resources.Incident.due_date;
                         status_cor2.Text = Resources.Incident.status;
                         date_complete2.Text = Resources.Incident.date_complete;
                         attachment2.Text = Resources.Incident.attachment;
                         root_cause_action_cor2.Text = Resources.Incident.root_cause_action;
                         remark2.Text = Resources.Incident.remark;




                         DataTable dtCorrectivePreventive2 = new DataTable("corrective_preventive");  //*** DataTable Map DataSet.xsd ***//

                         dtCorrectivePreventive2.Columns.Add(new DataColumn("no", typeof(System.Int32)));
                         dtCorrectivePreventive2.Columns.Add(new DataColumn("tb_corrective_preventive", typeof(System.String)));
                         dtCorrectivePreventive2.Columns.Add(new DataColumn("responsible_person", typeof(System.String)));
                         dtCorrectivePreventive2.Columns.Add(new DataColumn("lbdepartment_action", typeof(System.String)));
                         dtCorrectivePreventive2.Columns.Add(new DataColumn("due_date", typeof(System.String)));
                         dtCorrectivePreventive2.Columns.Add(new DataColumn("status", typeof(System.String)));
                         dtCorrectivePreventive2.Columns.Add(new DataColumn("date_complete", typeof(System.String)));
                         dtCorrectivePreventive2.Columns.Add(new DataColumn("attachment", typeof(System.String)));
                         dtCorrectivePreventive2.Columns.Add(new DataColumn("root_cause_action", typeof(System.String)));
                         dtCorrectivePreventive2.Columns.Add(new DataColumn("remark", typeof(System.String)));

                         var co2 = from c in dbConnect.preventive_action_incidents
                                  join s in dbConnect.action_status on c.action_status_id equals s.id
                                  join em in dbConnect.employees on c.employee_id equals em.employee_id into joinE
                                  from em in joinE.DefaultIfEmpty()
                                  join o in dbConnect.organizations on em.unit_id equals o.org_unit_id into joinO
                                  from o in joinO.DefaultIfEmpty()
                                  join de in dbConnect.departments on o.department_id equals de.department_id into joinD
                                  from de in joinD.DefaultIfEmpty()
                                  where c.incident_id == Convert.ToInt32(id)
                                  orderby c.id descending
                                  select new
                                  {
                                      c.id,
                                      c.preventive_action,
                                      c.responsible_person,
                                      due_date = c.due_date,
                                      status = chageDataLanguage(s.name_th, s.name_en, lang),
                                      date_complete = c.date_complete,
                                      c.attachment_file,
                                      c.notify_contractor,
                                      c.remark,
                                      c.action_status_id,
                                      c.root_cause_action,
                                      due_date2 = c.due_date,
                                      department_name = chageDataLanguage(de.department_th, de.department_en, lang)

                                  };


                         int count_corrective2 = 1;
                         foreach (var rc in co2)
                         {
                             DataRow drCorrectivePreventive = dtCorrectivePreventive2.NewRow();
                             drCorrectivePreventive["no"] = count_corrective;
                             drCorrectivePreventive["tb_corrective_preventive"] = rc.preventive_action;
                             drCorrectivePreventive["responsible_person"] = rc.responsible_person;
                             drCorrectivePreventive["lbdepartment_action"] = rc.department_name;


                             if (rc.due_date != null)
                             {
                                 string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date), lang);
                                 drCorrectivePreventive["due_date"] = duedate;
                             }
                             else
                             {
                                 drCorrectivePreventive["due_date"] = "";
                             }

                             string status_action = "";
                             if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                             {
                                 status_action = rc.status;

                                 if (rc.date_complete != null)
                                 {
                                     if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                                     {
                                         status_action = chageDataLanguage("ล่าช้า", "delay", lang);
                                     }

                                 }
                                 else
                                 {
                                     if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2).Date)
                                     {
                                         status_action = chageDataLanguage("ล่าช้า", "delay", lang);
                                     }

                                 }

                             }
                             else
                             {
                                 status_action = rc.status;

                             }
                             drCorrectivePreventive["status"] = status_action;

                             if (rc.date_complete != null)
                             {
                                 string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete), lang);
                                 drCorrectivePreventive["date_complete"] = date;

                             }
                             else
                             {
                                 drCorrectivePreventive["date_complete"] = "";

                             }



                             drCorrectivePreventive["attachment"] = rc.attachment_file;
                             if (rc.attachment_file != "")
                             {
                                 action.Add(rc.attachment_file);
                             }
                             drCorrectivePreventive["root_cause_action"] = rc.root_cause_action;
                             drCorrectivePreventive["remark"] = rc.remark;
                             dtCorrectivePreventive2.Rows.Add(drCorrectivePreventive);

                             count_corrective2++;

                         }




                         cryRpt.Subreports["preventive_subreport"].Database.Tables["corrective_preventive"].SetDataSource(dtCorrectivePreventive2);







                         TextObject lb_corrective_preventive3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["lb_consequence"];
                         TextObject no_corrective_preventive3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["no"];

                         TextObject tb_corrective_preventive3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["tb_corrective_preventive"];
                         TextObject responsible_person3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["responsible_person"];
                         TextObject lbdepartment_action3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["lbdepartment_action"];
                         TextObject due_date3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["due_date"];
                         TextObject status_cor3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["status"];
                         TextObject date_complete3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["date_complete"];
                         TextObject attachment3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["attachment"];
                         TextObject root_cause_action_cor3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["root_cause_action"];
                         TextObject remark3 = (TextObject)cryRpt.Subreports["consequence_subreport"].ReportDefinition.ReportObjects["remark"];


                         no_corrective_preventive3.Text = "";
                         lb_corrective_preventive3.Text = Resources.Incident.lb_consequence;
                         tb_corrective_preventive3.Text = Resources.Incident.tb_consequence;
                         responsible_person3.Text = Resources.Incident.responsible_person;
                         lbdepartment_action3.Text = (Resources.Incident.lbdepartment_action).Replace("/", "/ ");
                         due_date3.Text = Resources.Incident.due_date;
                         status_cor3.Text = Resources.Incident.status;
                         date_complete3.Text = Resources.Incident.date_complete;
                         attachment3.Text = Resources.Incident.attachment;
                         root_cause_action_cor3.Text = Resources.Incident.root_cause_action;
                         remark3.Text = Resources.Incident.remark;




                         DataTable dtCorrectivePreventive3 = new DataTable("corrective_preventive");  //*** DataTable Map DataSet.xsd ***//

                         dtCorrectivePreventive3.Columns.Add(new DataColumn("no", typeof(System.Int32)));
                         dtCorrectivePreventive3.Columns.Add(new DataColumn("tb_corrective_preventive", typeof(System.String)));
                         dtCorrectivePreventive3.Columns.Add(new DataColumn("responsible_person", typeof(System.String)));
                         dtCorrectivePreventive3.Columns.Add(new DataColumn("lbdepartment_action", typeof(System.String)));
                         dtCorrectivePreventive3.Columns.Add(new DataColumn("due_date", typeof(System.String)));
                         dtCorrectivePreventive3.Columns.Add(new DataColumn("status", typeof(System.String)));
                         dtCorrectivePreventive3.Columns.Add(new DataColumn("date_complete", typeof(System.String)));
                         dtCorrectivePreventive3.Columns.Add(new DataColumn("attachment", typeof(System.String)));
                         dtCorrectivePreventive3.Columns.Add(new DataColumn("root_cause_action", typeof(System.String)));
                         dtCorrectivePreventive3.Columns.Add(new DataColumn("remark", typeof(System.String)));

                         var co3 = from c in dbConnect.consequence_management_incidents
                                   join s in dbConnect.action_status on c.action_status_id equals s.id
                                   join em in dbConnect.employees on c.employee_id equals em.employee_id into joinE
                                   from em in joinE.DefaultIfEmpty()
                                   join o in dbConnect.organizations on em.unit_id equals o.org_unit_id into joinO
                                   from o in joinO.DefaultIfEmpty()
                                   join de in dbConnect.departments on o.department_id equals de.department_id into joinD
                                   from de in joinD.DefaultIfEmpty()
                                   where c.incident_id == Convert.ToInt32(id)
                                   orderby c.id descending
                                   select new
                                   {
                                       c.id,
                                       c.consequence_management,
                                       c.responsible_person,
                                       due_date = c.due_date,
                                       status = chageDataLanguage(s.name_th, s.name_en, lang),
                                       date_complete = c.date_complete,
                                       c.attachment_file,
                                       c.notify_contractor,
                                       c.remark,
                                       c.action_status_id,
                                       c.root_cause_action,
                                       due_date2 = c.due_date,
                                       department_name = chageDataLanguage(de.department_th, de.department_en, lang)

                                   };


                         int count_corrective3 = 1;
                         foreach (var rc in co3)
                         {
                             DataRow drCorrectivePreventive = dtCorrectivePreventive3.NewRow();
                             drCorrectivePreventive["no"] = count_corrective;
                             drCorrectivePreventive["tb_corrective_preventive"] = rc.consequence_management;
                             drCorrectivePreventive["responsible_person"] = rc.responsible_person;
                             drCorrectivePreventive["lbdepartment_action"] = rc.department_name;


                             if (rc.due_date != null)
                             {
                                 string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date), lang);
                                 drCorrectivePreventive["due_date"] = duedate;
                             }
                             else
                             {
                                 drCorrectivePreventive["due_date"] = "";
                             }

                             string status_action = "";
                             if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                             {
                                 status_action = rc.status;

                                 if (rc.date_complete != null)
                                 {
                                     if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                                     {
                                         status_action = chageDataLanguage("ล่าช้า", "delay", lang);
                                     }

                                 }
                                 else
                                 {
                                     if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2).Date)
                                     {
                                         status_action = chageDataLanguage("ล่าช้า", "delay", lang);
                                     }

                                 }

                             }
                             else
                             {
                                 status_action = rc.status;

                             }
                             drCorrectivePreventive["status"] = status_action;

                             if (rc.date_complete != null)
                             {
                                 string date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.date_complete), lang);
                                 drCorrectivePreventive["date_complete"] = date;

                             }
                             else
                             {
                                 drCorrectivePreventive["date_complete"] = "";

                             }



                             drCorrectivePreventive["attachment"] = rc.attachment_file;
                             if (rc.attachment_file != "")
                             {
                                 action.Add(rc.attachment_file);
                             }
                             drCorrectivePreventive["root_cause_action"] = rc.root_cause_action;
                             drCorrectivePreventive["remark"] = rc.remark;
                             dtCorrectivePreventive3.Rows.Add(drCorrectivePreventive);

                             count_corrective3++;

                         }




                         cryRpt.Subreports["consequence_subreport"].Database.Tables["corrective_preventive"].SetDataSource(dtCorrectivePreventive3);




                         TextObject lb_culpability = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_culpability"]);
                         TextObject lbfunction_culpability = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lbfunction_culpability"]);
                         TextObject lbdepartment_culpability = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lbdepartment_culpability"]);
                         TextObject lb_road_accident = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_road_accident"]);
                         TextObject lb_root_cause = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_root_cause"]);
                         TextObject lb_fatality_prevention = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_fatality_prevention"]);
                         TextObject lb_other_please = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_other_please"]);
                         TextObject lb_investigation_committee = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_investigation_committee"]);

                         TextObject culpability = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["culpability"]);
                         TextObject function_culpability = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["function_culpability"]);
                         TextObject department_culpability = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["department_culpability"]);
                         TextObject road_accident = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["road_accident"]);
                         TextObject root_cause = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["root_cause"]);
                         TextObject fatality_prevention = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["fatality_prevention"]);
                         TextObject other_please = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["other_please"]);
                         TextObject investigation_committee = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["investigation_committee"]);

                         lb_culpability.Text = Resources.Incident.culpability;
                         lbfunction_culpability.Text = Resources.Incident.lbfunction_culpability;
                         lbdepartment_culpability.Text = Resources.Incident.lbdepartment_culpability;
                         lb_road_accident.Text = Resources.Incident.road_accident;
                         lb_root_cause.Text = Resources.Incident.root_cause;
                         lb_fatality_prevention.Text = Resources.Incident.fatality_prevention;
                         lb_other_please.Text = Resources.Incident.other_please;
                         lb_investigation_committee.Text = Resources.Incident.investigation_committee;

                         if (v.culpability == "G")
                         {
                             culpability.Text = Resources.Incident.guilty;
                         }
                         else if (v.culpability == "P")
                         {
                             culpability.Text = Resources.Incident.partial;
                         }
                         else if (v.culpability == "N")
                         {
                             culpability.Text = Resources.Incident.no_guilty;
                         }
                         else
                         {
                             culpability.Text = "";
                         }

                         function_culpability.Text = v.form3_function_name == null ? "" : v.form3_function_name;
                         department_culpability.Text = v.form3_department_name == null ? "" : v.form3_department_name;

                         if (v.road_accident == "N")
                         {
                             road_accident.Text = Resources.Incident.no;
                         }
                         else if (v.road_accident == "Y")
                         {
                             road_accident.Text = Resources.Incident.yes;
                         }
                         else
                         {
                             road_accident.Text = "";
                         }


                         ArrayList cr = new ArrayList();
                         var p = from c in dbConnect.root_cause_incidents
                                 where c.incident_id == Convert.ToInt32(id)
                                 select new
                                 {
                                     c.root_cause

                                 };

                         foreach (var m in p)
                         {
                             cr.Add(m.root_cause);
                         }

                         string all_other_root_causes = "";

                         var root = from c in dbConnect.root_cause_incidents
                                    where c.incident_id == Convert.ToInt32(id)
                                    select new
                                    {
                                        c.root_cause

                                    };

                         foreach (var m in root)
                         {
                             string name_root_cause = "";
                             if (m.root_cause == "policy_planning")
                             {
                                 name_root_cause = Resources.Incident.policy_planning;

                             }
                             else if (m.root_cause == "document_reporting")
                             {
                                 name_root_cause = Resources.Incident.document_reporting;

                             }
                             else if (m.root_cause == "responsibilities_resourses")
                             {

                                 name_root_cause = Resources.Incident.responsibilities_resourses;
                             }
                             else if (m.root_cause == "standard_controls")
                             {

                                 name_root_cause = Resources.Incident.standard_controls;
                             }
                             else if (m.root_cause == "hazard_assesment")
                             {

                                 name_root_cause = Resources.Incident.hazard_assesment;
                             }
                             else if (m.root_cause == "inspection_monitoring")
                             {

                                 name_root_cause = Resources.Incident.inspection_monitoring;
                             }
                             else if (m.root_cause == "legal_compliance")
                             {

                                 name_root_cause = Resources.Incident.legal_compliance;
                             }
                             else if (m.root_cause == "emergency_preparation")
                             {

                                 name_root_cause = Resources.Incident.emergency_preparation;
                             }
                             else if (m.root_cause == "safety_installation")
                             {

                                 name_root_cause = Resources.Incident.safety_installation;
                             }
                             else if (m.root_cause == "management")
                             {

                                 name_root_cause = Resources.Incident.management;
                             }
                             else if (m.root_cause == "purchasing_contractor")
                             {

                                 name_root_cause = Resources.Incident.purchasing_contractor;
                             }
                             else if (m.root_cause == "occupational")
                             {

                                 name_root_cause = Resources.Incident.occupational;
                             }
                             else if (m.root_cause == "selection_competency")
                             {

                                 name_root_cause = Resources.Incident.selection_competency;
                             }
                             else if (m.root_cause == "corrective_preventive")
                             {

                                 name_root_cause = Resources.Incident.corrective_preventive;
                             }
                             else if (m.root_cause == "incident_hazard")
                             {

                                 name_root_cause = Resources.Incident.incident_hazard;
                             }
                             else if (m.root_cause == "health_wellness")
                             {

                                 name_root_cause = Resources.Incident.health_wellness;
                             }
                             else if (m.root_cause == "hygience")
                             {

                                 name_root_cause = Resources.Incident.hygience;
                             }
                             else if (m.root_cause == "system_performance")
                             {

                                 name_root_cause = Resources.Incident.system_performance;
                             }
                             else if (m.root_cause == "communication_involvement")
                             {

                                 name_root_cause = Resources.Incident.communication_involvement;
                             }



                             if (all_other_root_causes != "")
                             {

                                 all_other_root_causes = all_other_root_causes + ", " + name_root_cause;

                             }
                             else
                             {
                                 all_other_root_causes = name_root_cause;

                             }

                         }


                         root_cause.Text = all_other_root_causes;
                         fatality_prevention.Text = v.fatality_prevention_element == null ? "" : v.fatality_prevention_element;
                         other_please.Text = v.faltality_prevention_element_other == null ? "" : v.faltality_prevention_element_other;
                         investigation_committee.Text = v.investigation_committee_file == null ? "" : v.investigation_committee_file;
                         commitee = v.investigation_committee_file == null ? "" : v.investigation_committee_file;


                         TextObject lb_form2 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_form2"]);
                         TextObject lb_form3 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_form3"]);
                         TextObject lb_form4 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_form4"]);
                         TextObject position = (TextObject)cryRpt.Subreports["form4_subreport"].ReportDefinition.ReportObjects["position"];
                         TextObject name_surname = (TextObject)cryRpt.Subreports["form4_subreport"].ReportDefinition.ReportObjects["name_surname"];
                         TextObject date_form4 = (TextObject)cryRpt.Subreports["form4_subreport"].ReportDefinition.ReportObjects["date"];
                         TextObject approval = (TextObject)cryRpt.Subreports["form4_subreport"].ReportDefinition.ReportObjects["approval"];
                         TextObject remark_form4 = (TextObject)cryRpt.Subreports["form4_subreport"].ReportDefinition.ReportObjects["remark"];

                         lb_form2.Text = Resources.Incident.incidentstep2;
                         lb_form3.Text = Resources.Incident.incidentstep3;
                         lb_form4.Text = Resources.Incident.incidentstep4;
                         position.Text = Resources.Incident.postion;
                         name_surname.Text = Resources.Incident.name_surname;
                         date_form4.Text = Resources.Incident.date;
                         approval.Text = Resources.Incident.approval;
                         remark_form4.Text = Resources.Incident.remark;


                         DataTable dtForm4 = new DataTable("form4");  //*** DataTable Map DataSet.xsd ***//


                         dtForm4.Columns.Add(new DataColumn("position", typeof(System.String)));
                         dtForm4.Columns.Add(new DataColumn("name_surname", typeof(System.String)));
                         dtForm4.Columns.Add(new DataColumn("date", typeof(System.String)));
                         dtForm4.Columns.Add(new DataColumn("approval", typeof(System.String)));
                         dtForm4.Columns.Add(new DataColumn("remark", typeof(System.String)));

                         var fo = from c in dbConnect.log_request_close_incidents
                                  join em in dbConnect.employees on c.employee_id equals em.employee_id
                                  join g in dbConnect.groups on c.group_id equals g.id

                                  where c.incident_id == Convert.ToInt32(id)
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
                             string status_form4 = chageDataLanguageStatus(rc.status_process, lang);


                             DataRow drForm4 = dtForm4.NewRow();
                             drForm4["position"] = rc.name;
                             drForm4["name_surname"] = name;
                             drForm4["date"] = rc.created_at;
                             drForm4["approval"] = status_form4;
                             drForm4["remark"] = rc.remark;

                             dtForm4.Rows.Add(drForm4);

                         }

                         cryRpt.Subreports["form4_subreport"].Database.Tables["form4"].SetDataSource(dtForm4);
                         CrystalReportViewer1.ReportSource = cryRpt;



                     }



                     ExportOptions CrExportOptions;
                     DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                     PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                     string pathexport = System.Configuration.ConfigurationManager.AppSettings["pathexport"];
                     string path_write = string.Format("{0}" + pathexport+"incident\\"+country+"_"+incident_doc_number+".pdf", Server.MapPath(@"\"));

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


                     string path_zip = string.Format("{0}" +pathexport + "incident\\"+country+"_" +incident_doc_number+ ".zip", Server.MapPath(@"\"));

                  //   using (FileStream zipFileToOpen = new FileStream(path_zip, FileMode.OpenOrCreate))
                  //   {

                       //  using (ZipArchive archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Create))

                         if (File.Exists(path_zip))
                         {
                             File.Delete(path_zip);
                         }
                         using (ZipArchive archive = ZipFile.Open(path_zip, ZipArchiveMode.Create))
                         {
                            
                             string path_step3 = string.Format("{0}" + pathupload + "step3\\" + country + "\\" + incident_doc_number + "\\", Server.MapPath(@"\"));

                             foreach (string f in factfinding)
                             {
                                 if (File.Exists(path_step3 + f))
                                 {
                                     archive.CreateEntryFromFile(path_step3 + f, f);
                                 }
                             }


                             foreach (string a in action)
                             {
                                 if (File.Exists(path_step3 + a))
                                 {
                                     archive.CreateEntryFromFile(path_step3 + a, a);
                                 }
                             }

                             if (commitee != "")
                             {
                                 if (File.Exists(path_step3 + commitee))
                                 {
                                     archive.CreateEntryFromFile(path_step3 + commitee, commitee);
                                 }
                                 
                             }

                             foreach (string d in img_form1)
                             {
                                 if (File.Exists(pathfolder + "\\" + d))
                                 {
                                     archive.CreateEntryFromFile(pathfolder + "\\" + d, d);
                                 }
                             }
                            
                             archive.CreateEntryFromFile(path_write, country + "_" + incident_doc_number + ".pdf");
                             
                         }
                  //   }


                     Response.ContentType = "application/zip";
                     Response.AddHeader("Content-Disposition", "filename=" + country + "_" + incident_doc_number + ".zip");
                     Response.TransmitFile(path_zip);



                 }














             }//ispostback
        }


        public void setLevelTable(ReportDocument cryRpt,string incident_id)
        {
            string result_image = getLevelTable(id, "1");
            string result_safety = getLevelTable(id, "2");
            string result_environment = getLevelTable(id, "3");
            string result_damage = getLevelTable(id, "4");
            string result_process = getLevelTable(id, "5");
            string result_legal = getLevelTable(id, "6");
            string result_person = getLevelTable(id, "7");

            TextObject level_incident_table = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level_incident_table"]);
            level_incident_table.Text = Resources.Incident.level_incident_table;

            TextObject level5 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level5"]);
            TextObject level4 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level4"]);
            TextObject level3 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level3"]);
            TextObject level2 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level2"]);
            TextObject level1 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level1"]);
            TextObject definition_level = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["definition_level"]);

            level5.Text = Resources.Incident.level5;
            level4.Text = Resources.Incident.level4;
            level3.Text = Resources.Incident.level3;
            level2.Text = Resources.Incident.level2;
            level1.Text = Resources.Incident.level1;
            definition_level.Text = Resources.Incident.definition_level;


            TextObject level5_image = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level5_image"]);
            TextObject level4_image = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level4_image"]);
            TextObject level3_image = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level3_image"]);
            TextObject level2_image = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level2_image"]);
            TextObject level1_image = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level1_image"]);
            TextObject definition_image = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["definition_image"]);


            level5_image.Text = Resources.Incident.level5_image;
            level4_image.Text = Resources.Incident.level4_image;
            level3_image.Text = Resources.Incident.level3_image;
            level2_image.Text = Resources.Incident.level2_image;
            level1_image.Text = Resources.Incident.level1_image;
            definition_image.Text = Resources.Incident.definition_image;



            TextObject level5_safety = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level5_safety"]);
            TextObject level4_safety = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level4_safety"]);
            TextObject level3_safety = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level3_safety"]);
            TextObject level2_safety = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level2_safety"]);
            TextObject level1_safety = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level1_safety"]);
            TextObject definition_safety = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["definition_safety"]);

            level5_safety.Text = (Resources.Incident.level5_safety).Replace("</br>", " "); ;
            level4_safety.Text = Resources.Incident.level4_safety;
            level3_safety.Text = (Resources.Incident.level3_safety).Replace("</br>", " ");
            level2_safety.Text = Resources.Incident.level2_safety;
            level1_safety.Text = Resources.Incident.level1_safety;
            definition_safety.Text = Resources.Incident.definition_safety;




            TextObject level5_environment = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level5_environment"]);
            TextObject level4_environment = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level4_environment"]);
            TextObject level3_environment = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level3_environment"]);
            TextObject level2_environment = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level2_environment"]);
            TextObject level1_environment = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level1_environment"]);
            TextObject definition_environment = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["definition_environment"]);

            level5_environment.Text = Resources.Incident.level5_environment;
            level4_environment.Text = Resources.Incident.level4_environment;
            level3_environment.Text = Resources.Incident.level3_environment;
            level2_environment.Text = Resources.Incident.level2_environment;
            level1_environment.Text = Resources.Incident.level1_environment;
            definition_environment.Text = Resources.Incident.definition_environment;


            TextObject level5_damage = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level5_damage"]);
            TextObject level4_damage = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level4_damage"]);
            TextObject level3_damage = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level3_damage"]);
            TextObject level2_damage = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level2_damage"]);
            TextObject level1_damage = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level1_damage"]);
            TextObject definition_damage = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["definition_damage"]);

            level5_damage.Text = Resources.Incident.level5_damage;
            level4_damage.Text = Resources.Incident.level4_damage;
            level3_damage.Text = Resources.Incident.level3_damage;
            level2_damage.Text = Resources.Incident.level2_damage;
            level1_damage.Text = Resources.Incident.level1_damage;
            definition_damage.Text = Resources.Incident.definition_damage;



            TextObject level5_process = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level5_process"]);
            TextObject level4_process = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level4_process"]);
            TextObject level3_process = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level3_process"]);
            TextObject level2_process = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level2_process"]);
            TextObject level1_process = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level1_process"]);
            TextObject definition_process = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["definition_process"]);

            level5_process.Text = Resources.Incident.level5_process;
            level4_process.Text = Resources.Incident.level4_process;
            level3_process.Text = Resources.Incident.level3_process;
            level2_process.Text = Resources.Incident.level2_process;
            level1_process.Text = Resources.Incident.level1_process;
            definition_process.Text = Resources.Incident.definition_process;




            TextObject level5_legal = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level5_legal"]);
            TextObject level4_legal = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level4_legal"]);
            TextObject level3_legal = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level3_legal"]);
            TextObject level2_legal = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level2_legal"]);
            TextObject level1_legal = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level1_legal"]);
            TextObject definition_legal = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["definition_legal"]);

            level5_legal.Text = Resources.Incident.level5_legal;
            level4_legal.Text = Resources.Incident.level4_legal;
            level3_legal.Text = Resources.Incident.level3_legal;
            level2_legal.Text = Resources.Incident.level2_legal;
            level1_legal.Text = Resources.Incident.level1_legal;
            definition_legal.Text = Resources.Incident.definition_legal;



            TextObject level5_person = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level5_person"]);
            TextObject level4_person = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level4_person"]);
            TextObject level3_person = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level3_person"]);
            TextObject level2_person = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level2_person"]);
            TextObject level1_person = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level1_person"]);
            TextObject definition_person = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["definition_person"]);

            level5_person.Text = Resources.Incident.level5_person;
            level4_person.Text = Resources.Incident.level4_person;
            level3_person.Text = Resources.Incident.level3_person;
            level2_person.Text = Resources.Incident.level2_person;
            level1_person.Text = Resources.Incident.level1_person;
            definition_person.Text = Resources.Incident.definition_person;



            if (result_image == "5")
            {
                level1_image.Border.BackgroundColor = System.Drawing.Color.Red;
                level1_image.Color = System.Drawing.Color.White; ;
            }
            else if (result_image == "4")
            {
                level2_image.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_image == "3")
            {
                level3_image.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_image == "2")
            {
                level4_image.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_image == "1")
            {
                level5_image.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
               
            }




            if (result_safety == "5")
            {
                level1_safety.Border.BackgroundColor = System.Drawing.Color.Red;
                level1_safety.Color = System.Drawing.Color.White;
            }
            else if (result_safety == "4")
            {
                level2_safety.Border.BackgroundColor = System.Drawing.Color.Red;
                level2_safety.Color = System.Drawing.Color.White;
            }
            else if (result_safety == "3")
            {
                level3_safety.Border.BackgroundColor = System.Drawing.Color.Red;
                level3_safety.Color = System.Drawing.Color.White;
            }
            else if (result_safety == "2")
            {
                level4_safety.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
               
            }
            else if (result_safety == "1")
            {
                level5_safety.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
               
            }



            if (result_environment == "5")
            {
                level1_environment.Border.BackgroundColor = System.Drawing.Color.Red;
                level1_environment.Color = System.Drawing.Color.White;
            }
            else if (result_environment == "4")
            {
                level2_environment.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;     
            }
            else if (result_environment == "3")
            {
                level3_environment.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_environment == "2")
            {
                level4_environment.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_environment == "1")
            {
                level5_environment.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }





            if (result_damage == "5")
            {
                level1_damage.Border.BackgroundColor = System.Drawing.Color.Red;
                level1_damage.Color = System.Drawing.Color.White;
            }
            else if (result_damage == "4")
            {
                level2_damage.Border.BackgroundColor = System.Drawing.Color.Red;
                level2_damage.Color = System.Drawing.Color.White;
            }
            else if (result_damage == "3")
            {
                level3_damage.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_damage == "2")
            {
                level4_damage.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_damage == "1")
            {
                level5_damage.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }





            if (result_process == "5")
            {
                level1_process.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_process == "4")
            {
                level2_process.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_process == "3")
            {
                level3_process.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_process == "2")
            {
                level4_process.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_process == "1")
            {
                level5_process.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }




            if (result_legal == "5")
            {
                level1_legal.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_legal == "4")
            {
                level2_legal.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_legal == "3")
            {
                level3_legal.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_legal == "2")
            {
                level4_legal.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_legal == "1")
            {
                level5_legal.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }





            if (result_person == "5")
            {
                level1_person.Border.BackgroundColor = System.Drawing.Color.Red;
                level1_person.Color = System.Drawing.Color.White; ;
            }
            else if (result_person == "4")
            {
                level2_person.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_person == "3")
            {
                level3_person.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_person == "2")
            {
                level4_person.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;
            }
            else if (result_person == "1")
            {
                level5_person.Border.BackgroundColor = System.Drawing.Color.PaleTurquoise;

            }
            


        }


        public string getLevelTable(string incident_id, string level_definition_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                string result = "0";

                var gr = from c in dbConnect.level_has_definition_level_incidents
                         where c.incident_id == Convert.ToInt32(incident_id) &&
                         c.definition_level_incident_id == Convert.ToInt32(level_definition_id)
                         select c;
                foreach (var r in gr)
                {
                    result = findColumn(r.level_incident.ToString());

                }

                return result;
            }
        }

        public string findColumn(string v)//แปลงค่ level เป็นค่าโชว์ในตาราง มันเรียงสลับกัน
        {
            string result = "0";

            if (v == "1")
            {
                result = "5";

            }
            else if (v == "2")
            {
                result = "4";

            }
            else if (v == "3")
            {
                result = "3";

            }
            else if (v == "4")
            {
                result = "2";

            }
            else if (v == "5")
            {

                result = "1";
            }
            return result;
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


        public string chageDataLanguageStatus(string v, string lang)
        {
            string vReturn = "";

            if (lang == "th")
            {
                if (v == "C")
                {
                    vReturn = "ปิด";
                }
                else
                {
                    vReturn = "ไม่ปิด";
                }




            }
            else if (lang == "en")
            {

                if (v == "C")
                {
                    vReturn = "Closed";
                }
                else
                {
                    vReturn = "Not closed";
                }

            }
            else if (lang == "si")
            {

                if (v == "C")
                {
                    vReturn = "Closed";
                }
                else
                {
                    vReturn = "Not closed";
                }

            }


            return vReturn;
        }
    }
}