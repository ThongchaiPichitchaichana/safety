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
    public partial class HazardExport1 : System.Web.UI.Page
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
             string hazard_doc_number = "";
             ArrayList action = new ArrayList();
             ArrayList img_form1 = new ArrayList();
             string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathuploadhazard"];
             string pathfolder = "";



             if (!IsPostBack)
             {
                 using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
                 {

                     var q = from c in dbConnect.hazards
                        //join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                        //from e in joinE.DefaultIfEmpty()
                        join h in dbConnect.hazard_characteristics on c.hazard_characteristic_id equals h.id into joinH
                        from h in joinH.DefaultIfEmpty()
                        join ca in dbConnect.source_hazards on c.source_hazard equals ca.id into joinCa
                        from ca in joinCa.DefaultIfEmpty()
                        join fa in dbConnect.fatality_prevention_elements on c.fatality_prevention_element_id equals fa.id into joinFa
                        from fa in joinFa.DefaultIfEmpty()
                        join s in dbConnect.hazard_status on c.process_status equals s.id
                        where c.id == Convert.ToInt32(id)
                        select new
                        {
                            hazard_datetime = c.hazard_date,
                            report_date = c.report_date,
                            c.company_id,
                            c.function_id,
                            c.department_id,
                            c.division_id,
                            c.section_id,
                            c.hazard_area,
                            c.hazard_name,
                            c.hazard_detail,
                            c.preliminary_action,
                            c.type_action,
                            c.employee_id,
                            c.process_status,
                            c.typeuser_login,
                            c.doc_no,
                            c.verifying_date,
                            c.source_hazard,
                            c.level_hazard,
                            c.safety_officer_id,
                            c.area_owner_id,
                            status = chageDataLanguage(s.name_th, s.name_en, lang),
                            phone = c.phone,
                            c.fatality_prevention_element_id,
                            c.faltality_prevention_element_other,
                            c.step_form,
                            c.submit_report_form2,
                            c.id,
                            c.hazard_characteristic_id,
                            location_company_name_en = c.location_company_name_en == null ? "" : c.location_company_name_en,
                            location_company_name_th= c.location_company_name_th == null ? "" : c.location_company_name_th,
                            location_function_name_en= c. location_function_name_en == null ? "" : c. location_function_name_en,
                            location_function_name_th= c.location_function_name_th == null ? "" : c.location_function_name_th,
                            location_department_name_en = c.location_company_name_en == null ? "" : c.location_department_name_en,
                            location_department_name_th= c.location_department_name_th == null ? "" : c.location_department_name_th,
                            location_division_name_en= c.location_division_name_en == null ? "" : c.location_division_name_en,
                            location_division_name_th= c.location_division_name_th == null ? "" : c.location_division_name_th,
                            location_section_name_en = c.location_section_name_en == null ? "" : c.location_section_name_en,
                            location_section_name_th = c.location_section_name_th == null ? "" : c.location_section_name_th,
                            hazard_characteristic = chageDataLanguage(h.name_th,h.name_en,lang),
                            category_hazard = chageDataLanguage(ca.name_th,ca.name_en,lang),
                            fpe = chageDataLanguage(fa.name_th,fa.name_en,lang)



                        };



                
                     ReportDocument cryRpt;
                     cryRpt = new ReportDocument();
                     cryRpt.Load(Server.MapPath("~/HazardExport.rpt"));
                   
            
                     TextObject report_name = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["ReportName"]);
                     TextObject lb_doc_no = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_doc_no"]);
                     TextObject lb_status = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_status"]);
                     TextObject lb_date_hazard = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_date_hazard"]);
                     TextObject lb_date_report = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_date_report"]);
                     TextObject lb_company = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_company"]);
                     TextObject lb_function = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_function"]);
                     TextObject lb_department = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_department"]);
                     TextObject lb_division = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_division"]);
                     TextObject lb_section = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_section"]);
                     TextObject lb_characteristic = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_characteristic"]);
                     TextObject lb_location = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_location"]);
                     TextObject lb_title = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_title"]);
                     TextObject lb_detail = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_detail"]);
                     TextObject lb_preliminary_action = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_preliminary_action"]);
                     TextObject lb_type_action = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_type_action"]);
                     TextObject lb_report_name = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_reporter_name"]);
                     TextObject lb_phone_number = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_phone"]);

                     report_name.Text = Resources.Main.lbHazardExport;
                     lb_doc_no.Text = Resources.Hazard.doc_no;
                     lb_status.Text = Resources.Hazard.status;
                     lb_date_hazard.Text = Resources.Hazard.hazard_date;
                     lb_date_report.Text = Resources.Hazard.report_date;
                     lb_company.Text = Resources.Hazard.lbCompany;
                     lb_function.Text = Resources.Hazard.lbfucntion;
                     lb_department.Text = Resources.Hazard.lbdepartment;
                     lb_division.Text = Resources.Hazard.lbdivision;
                     lb_section.Text = Resources.Hazard.lbsection;
                     lb_location.Text = Resources.Hazard.hazardarea;
                     lb_title.Text = Resources.Hazard.hazardname;
                     lb_detail.Text = (Resources.Hazard.hazarddetail).Replace("</br>", " "); ; ;
                     lb_report_name.Text = Resources.Hazard.lbNameReportHeader;
                     lb_phone_number.Text = Resources.Hazard.hazardphone;
                     lb_characteristic.Text = Resources.Hazard.lbhazardcharacteristic;
                     lb_preliminary_action.Text = Resources.Hazard.preliminary_action;
                     lb_type_action.Text = Resources.Hazard.type_action;



                     TextObject doc_number = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["doc_no"]);
                     TextObject status = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["status"]);
                     TextObject date_hazard = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["date_hazard"]);
                     TextObject date_report = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["date_report"]);
                     TextObject company = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["company"]);
                     TextObject function = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["function"]);
                     TextObject department = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["department"]);
                     TextObject division = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["division"]);
                     TextObject section = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["section"]);
                     TextObject characteristic = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["characteristic"]);
                     TextObject location = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["location"]);
                     TextObject title = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["title"]);
                     TextObject detail = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["detail"]);
                     TextObject preliminary_action = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["preliminary_action"]);
                     TextObject type_action = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["type_action"]);
                     TextObject reporter_name = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["reporter_name"]);
                     TextObject phone = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["phone"]);




                     TextObject lb_verifying_date = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_verifying_date"]);
                     TextObject lb_category_hazard = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_category_hazard"]);
                     TextObject lb_fpes = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_fpes"]);
                     TextObject lb_other_please = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_other_please"]);
                     TextObject lb_level_hazard = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_level_hazard"]);
                     TextObject lb_acknowledge_area_safety = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_acknowledge_area_safety"]);


                     lb_verifying_date.Text = Resources.Hazard.verifying_date;
                     lb_category_hazard.Text = Resources.Hazard.source_hazard;
                     lb_fpes.Text = Resources.Hazard.fatality_prevention;
                     lb_other_please.Text = Resources.Incident.other_please;
                     lb_level_hazard.Text = Resources.Hazard.level_hazard;
                     lb_acknowledge_area_safety.Text = Resources.Hazard.header_acknowledge;


                     TextObject verifying_date = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["verifying_date"]);
                     TextObject category_hazard = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["category_hazard"]);
                     TextObject fatality_prevention = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["fpes"]);
                     TextObject other_please = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["other_please"]);
                     TextObject level_hazard = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["level_hazard"]);
                     TextObject acknowledge_area_safety = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["acknowledge_area_safety"]);

                     TextObject lb_hazardnameareaowner = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_hazardnameareaowner"]);
                     lb_hazardnameareaowner.Text = Resources.Hazard.hazardnameareaowner;

                     TextObject hazardnameareaowner = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["hazardnameareaowner"]);


                     foreach (var v in q)
                     {
                         hazard_doc_number = v.doc_no;
                         string user_name_modify = "";
                         string datetime_modify = "";

                         var doc_no = dbConnect.hazard_details.Max(x => x.id);

                         var d = (from c in dbConnect.hazard_details
                                  where c.hazard_id == Convert.ToInt32(id)
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


                         string fullname_security = "";
                         if (v.safety_officer_id != null & v.safety_officer_id != "")
                         {
                             var es = from c in dbConnect.employees
                                      where c.employee_id == v.safety_officer_id
                                      select new
                                      {
                                          prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                          first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                          last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang)

                                      };

                             foreach (var b in es)
                             {
                                 fullname_security = b.prefix + " " + b.first_name + " " + b.last_name;

                             }
                         }


                         string fullname_area_owner = "";
                         if (v.area_owner_id != null & v.area_owner_id != "")
                         {
                             var es = from c in dbConnect.employees
                                      where c.employee_id == v.area_owner_id
                                      select new
                                      {
                                          prefix = chageDataLanguage(c.prefix_th, c.prefix_en, lang),
                                          first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                                          last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang)

                                      };

                             foreach (var b in es)
                             {
                                 fullname_area_owner = b.prefix + " " + b.first_name + " " + b.last_name;

                             }
                         }




                         string step = "";


                         if (v.process_status != 2 && v.process_status != 3)//ไม่ใช้ close กับ reject
                         {

                             if (v.step_form == 1)//area oh&s
                             {
                                 string v_step = chageDataLanguage("รายงานแหล่งอันตราย", "Hazard report", lang);

                                 if (Session["country"].ToString() == "thailand")
                                 {
                                     step = step + "(" + v_step + " - Area OH&S)";

                                 }
                                 else if (Session["country"].ToString() == "srilanka")
                                 {
                                     step = step + "(" + v_step + " - Area Supervisor)";
                                 }


                             }
                             else if (v.step_form == 2)
                             {
                                 string v_step = chageDataLanguage("ตรวจสอบรายงานแหล่งอันตราย", "Verify Hazard Report", lang);

                                 if (v.submit_report_form2 == null)
                                 {

                                     if (Session["country"].ToString() == "thailand")
                                     {
                                         step = step + "(" + v_step + " - Area OH&S)";

                                     }
                                     else if (Session["country"].ToString() == "srilanka")
                                     {
                                         step = step + "(" + v_step + " - Area Supervisor)";

                                     }

                                 }
                                 else
                                 {
                                     step = step + "(" + v_step + " - Area Supervisor)";
                                 }


                             }
                             else if (v.step_form == 3)
                             {
                                 string v_step = chageDataLanguage("ดำเนินการแก้ไข", "Process of Action", lang);

                                 step = step + "(" + v_step + " - Area Supervisor)";

                             }
                             else if (v.step_form == 4)
                             {
                                 string v_step = chageDataLanguage("ขอปิดรายงานแหล่งอันตราย", "Request to Close Hazard Report", lang);
                                 bool check_close = true;

                                 var s = from c in dbConnect.close_step_hazards
                                         where c.country == Session["country"].ToString()
                                         orderby c.step descending
                                         select c;

                                 foreach (var r in s)
                                 {
                                     var w = from c in dbConnect.log_request_close_hazards
                                             where c.hazard_id == v.id && c.status == "A"
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
                         date_hazard.Text = FormatDates.getDatetimeShow(Convert.ToDateTime(v.hazard_datetime), lang);
                         date_report.Text = FormatDates.getDatetimeShow(Convert.ToDateTime(v.report_date), lang);
                         company.Text = chageDataLanguage(v.location_company_name_th, v.location_company_name_en, lang);
                         function.Text = chageDataLanguage(v.location_function_name_th, v.location_function_name_en, lang);
                         department.Text = chageDataLanguage(v.location_department_name_th, v.location_department_name_en, lang);
                         division.Text = chageDataLanguage(v.location_division_name_th, v.location_division_name_en, lang);
                         section.Text = chageDataLanguage(v.location_section_name_th, v.location_section_name_en, lang);
                         location.Text = v.hazard_area == null ? "" : v.hazard_area;
                         title.Text = v.hazard_name;
                         detail.Text = v.hazard_detail;
                         reporter_name.Text = getEmployeeByTypeLogin(v.employee_id, v.typeuser_login, lang);
                         phone.Text = v.phone;
                         characteristic.Text = v.hazard_characteristic == null?"":v.hazard_characteristic;
                         preliminary_action.Text = v.preliminary_action == null ? "" : v.preliminary_action;

                         if (v.type_action == "P")
                         {
                             type_action.Text = Resources.Hazard.pending_action;
                         }else if(v.type_action == "T")
                         {
                             type_action.Text = Resources.Hazard.temporary_control;
                         }
                         else if (v.type_action == "C")
                         {
                             type_action.Text = Resources.Hazard.complete_control;
                         }
                         else
                         {
                             type_action.Text = "";
                         }
                         




                         string name_folder = v.employee_id + "_" + v.report_date.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB"));
                         pathfolder = string.Format("{0}" + pathupload + name_folder, Server.MapPath(@"\"));

                         DataTable dtMap = new DataTable("form1");  //*** DataTable Map DataSet.xsd ***//
                         DataRow dr = null;
                         dtMap.Columns.Add(new DataColumn("img1", typeof(System.Byte[])));
                         dtMap.Columns.Add(new DataColumn("img2", typeof(System.Byte[])));

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



                         verifying_date.Text = v.verifying_date == null ? FormatDates.getDateShowFromDate(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())), lang) : FormatDates.getDateShowFromDate(Convert.ToDateTime(v.verifying_date), lang);
                         category_hazard.Text = v.category_hazard == null?"":v.category_hazard;
                         fatality_prevention.Text = v.fpe == null ?"":v.fpe;
                         other_please.Text = v.faltality_prevention_element_other == null ?"":v.faltality_prevention_element_other;

                         if (v.level_hazard == "H")
                         {
                             level_hazard.Text = Resources.Hazard.high;
                         }
                         else if (v.level_hazard == "M")
                         {
                             level_hazard.Text = Resources.Hazard.medium;
                         }
                         else if (v.level_hazard == "L")
                         {
                             level_hazard.Text = Resources.Hazard.low;
                         }
                         else
                         {
                             level_hazard.Text = "";
                         }

                         acknowledge_area_safety.Text = fullname_security;
                         hazardnameareaowner.Text = fullname_area_owner;


                         TextObject lb_process_action = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["lb_process_action"];
                         TextObject lb_no = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["lb_no"];
                         TextObject typecontrol = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["typecontrol"];
                         TextObject action_hazard = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["action"];
                         TextObject responsible_person = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["responsible_person"];
                         TextObject lbdepartment_action = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["lbdepartment_action"];
                         TextObject due_date = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["due_date"];
                         TextObject status_cor = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["status"];
                         TextObject date_complete = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["date_complete"];
                         TextObject attachment = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["attachment"];
                         TextObject remark = (TextObject)cryRpt.Subreports["process_action_subreport"].ReportDefinition.ReportObjects["remark"];

                         lb_process_action.Text = Resources.Hazard.process_action_form;
                         lb_no.Text = "";
                         typecontrol.Text = Resources.Hazard.typecontrol;
                         action_hazard.Text = Resources.Hazard.action;
                         responsible_person.Text = Resources.Hazard.responsible_person;
                         lbdepartment_action.Text = (Resources.Hazard.lbdepartment_action).Replace("/", "/ ");
                         due_date.Text = Resources.Hazard.due_date;
                         status_cor.Text = Resources.Hazard.status;
                         date_complete.Text = Resources.Hazard.date_complete;
                         attachment.Text = Resources.Hazard.attachment;
                         remark.Text = Resources.Hazard.remark;




                         DataTable dtProcessAction = new DataTable("process_action");  //*** DataTable Map DataSet.xsd ***//

                         dtProcessAction.Columns.Add(new DataColumn("no", typeof(System.Int32)));
                         dtProcessAction.Columns.Add(new DataColumn("type_of_control", typeof(System.String)));
                         dtProcessAction.Columns.Add(new DataColumn("action", typeof(System.String)));
                         dtProcessAction.Columns.Add(new DataColumn("responsible_person", typeof(System.String)));
                         dtProcessAction.Columns.Add(new DataColumn("department", typeof(System.String)));
                         dtProcessAction.Columns.Add(new DataColumn("due_date", typeof(System.String)));
                         dtProcessAction.Columns.Add(new DataColumn("status", typeof(System.String)));
                         dtProcessAction.Columns.Add(new DataColumn("date_complete", typeof(System.String)));
                         dtProcessAction.Columns.Add(new DataColumn("attachment", typeof(System.String)));
                         dtProcessAction.Columns.Add(new DataColumn("remark", typeof(System.String)));

                         var pr = from c in dbConnect.process_actions
                                 join s in dbConnect.action_status on c.action_status_id equals s.id
                                 join t in dbConnect.type_controls on c.type_control equals t.id
                                 join em in dbConnect.employees on c.employee_id equals em.employee_id into joinE
                                 from em in joinE.DefaultIfEmpty()
                                 join o in dbConnect.organizations on em.unit_id equals o.org_unit_id into joinO
                                 from o in joinO.DefaultIfEmpty()
                                 join de in dbConnect.departments on o.department_id equals de.department_id into joinD
                                 from de in joinD.DefaultIfEmpty()
                                 where c.hazard_id == Convert.ToInt32(id)
                                 orderby c.id descending
                                 select new
                                 {
                                     c.id,
                                     type_control = chageDataLanguage(t.name_th, t.name_en, lang),
                                     c.action,
                                     c.responsible_person,
                                     due_date = c.due_date.ToString(),
                                     status = chageDataLanguage(s.name_th, s.name_en, lang),
                                     date_complete = c.date_complete.ToString(),
                                     c.attachment_file,
                                     c.notify_contractor,
                                     c.remark,
                                     c.action_status_id,
                                     c.root_cause_action,
                                     due_date2 = c.due_date,
                                     department_name = chageDataLanguage(de.department_th, de.department_en, lang)

                                 };


           
                         int count_process = 1;
                         foreach (var rc in pr)
                         {
                             DataRow drProcessAction = dtProcessAction.NewRow();
                             drProcessAction["no"] = count_process;
                             drProcessAction["type_of_control"] = rc.type_control;
                             drProcessAction["action"] = rc.action;
                             drProcessAction["responsible_person"] = rc.responsible_person;
                             drProcessAction["department"] = rc.department_name;
                            
                 

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
                             if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
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


                             drProcessAction["attachment"] = rc.attachment_file;
                             if (rc.attachment_file != "")
                             {
                                 action.Add(rc.attachment_file);
                             }

                             drProcessAction["remark"] = rc.remark;
                             dtProcessAction.Rows.Add(drProcessAction);

                             count_process++;

                         }

                         cryRpt.Subreports["process_action_subreport"].Database.Tables["process_action"].SetDataSource(dtProcessAction);


                         TextObject lb_form2 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_form2"]);
                         TextObject lb_form3 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_form3"]);
                         TextObject lb_form4 = ((CrystalDecisions.CrystalReports.Engine.TextObject)cryRpt.ReportDefinition.ReportObjects["lb_form4"]);
                         TextObject position = (TextObject)cryRpt.Subreports["form4_subreport"].ReportDefinition.ReportObjects["position"];
                         TextObject name_surname = (TextObject)cryRpt.Subreports["form4_subreport"].ReportDefinition.ReportObjects["name_surname"];
                         TextObject date_form4 = (TextObject)cryRpt.Subreports["form4_subreport"].ReportDefinition.ReportObjects["date"];
                         TextObject approval = (TextObject)cryRpt.Subreports["form4_subreport"].ReportDefinition.ReportObjects["approval"];
                         TextObject remark_form4 = (TextObject)cryRpt.Subreports["form4_subreport"].ReportDefinition.ReportObjects["remark"];

                         lb_form2.Text = Resources.Hazard.hazardstep2;
                         lb_form3.Text = Resources.Hazard.hazardstep3;
                         lb_form4.Text = Resources.Hazard.hazardstep4;
                         position.Text = Resources.Hazard.postion;
                         name_surname.Text = Resources.Hazard.name_surname;
                         date_form4.Text = Resources.Hazard.date;
                         approval.Text = Resources.Hazard.approval;
                         remark_form4.Text = Resources.Hazard.remark;


                         DataTable dtForm4 = new DataTable("form4");  //*** DataTable Map DataSet.xsd ***//


                         dtForm4.Columns.Add(new DataColumn("position", typeof(System.String)));
                         dtForm4.Columns.Add(new DataColumn("name_surname", typeof(System.String)));
                         dtForm4.Columns.Add(new DataColumn("date", typeof(System.String)));
                         dtForm4.Columns.Add(new DataColumn("approval", typeof(System.String)));
                         dtForm4.Columns.Add(new DataColumn("remark", typeof(System.String)));

                         var fo = from c in dbConnect.log_request_close_hazards
                                  join em in dbConnect.employees on c.employee_id equals em.employee_id
                                  join g in dbConnect.groups on c.group_id equals g.id

                                  where c.hazard_id == Convert.ToInt32(id)
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
                             string status_form4 = chageDataLanguageStatusHazard(rc.status_process, lang);


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
                     string path_write = string.Format("{0}" + pathexport + "hazard\\" + country + "_" + hazard_doc_number + ".pdf", Server.MapPath(@"\"));

                     CrDiskFileDestinationOptions.DiskFileName = path_write;
                     CrExportOptions = cryRpt.ExportOptions;
                     {
                         CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                         CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                         CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                         CrExportOptions.FormatOptions = CrFormatTypeOptions;
                     }
                     cryRpt.Export();


                     string path_zip = string.Format("{0}" + pathexport + "hazard\\" + country + "_" + hazard_doc_number + ".zip", Server.MapPath(@"\"));

                     //using (FileStream zipFileToOpen = new FileStream(path_zip, FileMode.OpenOrCreate))
                    // {

                         if (File.Exists(path_zip))
                         {
                             File.Delete(path_zip);
                         }

                         //using (ZipArchive archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Create))
                         using (ZipArchive archive = ZipFile.Open(path_zip, ZipArchiveMode.Create))
                         {
                             archive.CreateEntryFromFile(path_write, country + "_" + hazard_doc_number + ".pdf");
                             string path_step3 = string.Format("{0}" + pathupload + "step3\\" + country + "\\" + hazard_doc_number + "\\", Server.MapPath(@"\"));

                          

                             foreach (string a in action)
                             {
                                 if (File.Exists(path_step3 + a))
                                 {
                                     archive.CreateEntryFromFile(path_step3 + a, a);
                                 }
                             }


                             foreach (string d in img_form1)
                             {
                                 if (File.Exists(pathfolder + "\\" + d))
                                 {
                                     archive.CreateEntryFromFile(pathfolder + "\\" + d, d);
                                 }
                             }

                         }

                    // }


                     Response.ContentType = "application/zip";
                     Response.AddHeader("Content-Disposition", "filename=" + country + "_" + hazard_doc_number + ".zip");
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


        public string chageDataLanguageStatusHazard(string v, string lang)
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