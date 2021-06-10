using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using safetys4;
using safetys4.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class HazardReport : System.Web.UI.Page
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

                    LinkButton link = (LinkButton)Master.FindControl("btAllhazardReport");
                    link.Attributes.CssStyle.Add("background-color", "#e6e6e8");
                }
            }
            else
            {
                string original_url = Server.UrlEncode(Context.Request.RawUrl);
                Response.Redirect("login.aspx?returnUrl=" + original_url);
            }
        }



        protected void btExportExcel_Click(object sender, EventArgs e)
        {
            string filename = "HazardReport.xlsx";
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

           string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
           //string path = string.Format("{0}\\report\\template\\hazard_report.xlsx", Server.MapPath(@"\"));
           string path = string.Format("{0}" + pathreport + "hazard_report.xlsx", Server.MapPath(@"\"));
        //   Response.Write(path); Response.End();
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("hazard_report");

            // sheet1.GetRow(1).GetCell(1).SetCellValue(1);
            //ICell cell = sheet1.GetRow(0).GetCell(0);
            //IRow row = sheet1.CreateRow(0);
            //ICell cell = row.CreateCell(0);
            // cell.SetCellValue(4);
            ArrayList headers = new ArrayList();

            headers.Add(Resources.Incident.sequence);
            headers.Add(Resources.Hazard.doc_no);
            headers.Add(Resources.Hazard.report_date);
            headers.Add(Resources.Hazard.hazard_date);
            headers.Add(Resources.Hazard.hazard_time);
            headers.Add(Resources.Hazard.lbCompany);
            headers.Add(Resources.Hazard.lbfucntion);
            headers.Add(Resources.Hazard.lbdepartment);  
            headers.Add(Resources.Hazard.lbdivision); 
            headers.Add(Resources.Hazard.lbsection);
            headers.Add(Resources.Hazard.hazardarea);
            headers.Add(Resources.Hazard.hazardname); 
            headers.Add(Resources.Hazard.hazarddetail); 
            headers.Add(Resources.Hazard.preliminary_action); 
            headers.Add(Resources.Hazard.type_action);
            headers.Add(Resources.Hazard.type_reporter);
            headers.Add(Resources.Hazard.lbNameReportHeader);
            headers.Add(Resources.Hazard.lbcompany_reporter);
            headers.Add(Resources.Hazard.lbfunction_reporter);
            headers.Add(Resources.Hazard.lbdepartment_reporter);
            headers.Add(Resources.Hazard.verifying_date); 
            headers.Add(Resources.Hazard.source_hazard); 
            headers.Add(Resources.Hazard.level_hazard);
            headers.Add(Resources.Incident.fatality_prevention);
            headers.Add(Resources.Incident.other_please);
            headers.Add(Resources.Hazard.header_acknowledge);
            headers.Add(Resources.Hazard.typecontrol);
            headers.Add(Resources.Hazard.action);
            headers.Add(Resources.Hazard.responsible_person); 
            headers.Add(Resources.Hazard.hazardnameareaowner);                                                               
            headers.Add(Resources.Hazard.status);
            headers.Add(Resources.Main.lbareamanager);
            headers.Add(Resources.Main.lbareasupervisor);
            headers.Add(Resources.Hazard.lbreasonreject);
            headers.Add(Resources.Main.lbdateclose);
            headers.Add(Resources.Hazard.delay_report);
            headers.Add(Resources.Hazard.lbhazardcharacteristic);
           

            setHeader(workbook, sheet1, headers);


            ICellStyle style = workbook.CreateCellStyle();
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BottomBorderColor = IndexedColors.Black.Index;

            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.LeftBorderColor = IndexedColors.Black.Index;


            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.RightBorderColor = IndexedColors.Black.Index;


            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.TopBorderColor = IndexedColors.Black.Index;



            ///////////////////////////////////////////////set value /////////////////////////////////////////////////////
            string companyid = Request.Form[ddcompany.UniqueID];
            string functionid = Request.Form[ddfunction.UniqueID];
            string departmentid = Request.Form[dddepartment.UniqueID];
            string divisionid = Request.Form[dddivision.UniqueID];
            string date_start = txtstart_date.Value;
            string date_end = txtend_date.Value;
            string lang = Session["lang"].ToString();


            string seach_by = searchBy(functionid, departmentid, divisionid, date_start, date_end, lang);
            IRow row_seach = sheet1.GetRow(1);

           

            for (int i = 4; i <= 36; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 36)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach.CreateCell(i);
                cell_n.CellStyle = style2;



            }

            CellRangeAddress range = new CellRangeAddress(1, 1, 4, 36);
            sheet1.AddMergedRegion(range);

            ICell cell_search = row_seach.GetCell(4);
            cell_search.SetCellValue(seach_by);

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from h in dbConnect.hazards
                     join t in dbConnect.source_hazards on h.source_hazard equals t.id into joinT

                     //join co in dbConnect.companies on h.company_id equals co.company_id
                     //join fu in dbConnect.functions on h.function_id equals fu.function_id
                     //join de in dbConnect.departments on h.department_id equals de.department_id
                     //join di in dbConnect.divisions on h.division_id equals di.division_id into joinDi
                     //join se in dbConnect.sections on h.section_id equals se.section_id into joinSe
                     join st in dbConnect.hazard_status on h.process_status equals st.id
                     join f in dbConnect.fatality_prevention_elements on h.fatality_prevention_element_id equals f.id into joinF
                     join rs in dbConnect.reason_reject_hazards on h.reason_reject_type equals rs.id into joinReason
                     join ch in dbConnect.hazard_characteristics on h.hazard_characteristic_id equals ch.id into joinCharacter

                     from t in joinT.DefaultIfEmpty()
                     //from di in joinDi.DefaultIfEmpty()
                    // from se in joinSe.DefaultIfEmpty()
                     from f in joinF.DefaultIfEmpty()
                     from rs in joinReason.DefaultIfEmpty()
                     from ch in joinCharacter.DefaultIfEmpty()

                     where h.country == Session["country"].ToString()
                     orderby h.report_date ascending
                     select new
                     {

                         report_date = h.report_date,
                         hazard_datetime = h.hazard_date,
                         company_name = chageDataLanguage(h.location_company_name_th, h.location_company_name_en, lang),
                         function_name = chageDataLanguage(h.location_function_name_th, h.location_function_name_en, lang),
                         department_name = chageDataLanguage(h.location_department_name_th, h.location_department_name_en, lang),
                         division_name = chageDataLanguage(h.location_division_name_th, h.location_division_name_en, lang),
                         section_name = chageDataLanguage(h.location_section_name_th, h.location_section_name_en, lang),

                         h.hazard_area,
                         h.hazard_name,
                         h.hazard_detail,
                         h.preliminary_action,
                         h.type_action,
                         h.employee_id,
                         h.safety_officer_id,
                         h.area_owner_id,
                         source_hazard = chageDataLanguage(t.name_th, t.name_en, lang),
                         h.verifying_date,
                         h.level_hazard,

                         status = chageDataLanguage(st.name_th, st.name_en, lang),
                         company_id = h.company_id,
                         function_id = h.function_id,
                         department_id = h.department_id,
                         division_id = h.division_id,
                         section_id = h.section_id,
                         hazard_id = h.id,
                         h.doc_no,
                         h.reason_reject,
                         reason_reject_type = chageDataLanguage(rs.name_th, rs.name_en, lang),
                         h.faltality_prevention_element_other,
                         faltality_prevention_element = chageDataLanguage(f.name_th,f.name_en,lang),
                         h.hazard_date,
                         h.id,
                         h.step_form,
                         h.process_status,
                         h.submit_report_form2,
                         h.typeuser_login,
                         h.close_hazard_date,
                         h.reporter_company_name,
                         h.reporter_function_name,
                         h.reporter_department_name,
                         h.reporter_division_name,
                         h.reporter_section_name,
                         characteristic = chageDataLanguage(ch.name_th, ch.name_en, lang)


                     };

            if (companyid != "" && companyid != "null")
            {
                v1 = v1.Where(c => c.company_id == companyid);

            }


            if (functionid != "" && functionid != "null")
            {
                v1 = v1.Where(c => c.function_id == functionid);

            }

            if (departmentid != "" && departmentid != "null")
            {
                v1 = v1.Where(c => c.department_id == departmentid);

            }

            if (divisionid != "" && divisionid != "null")
            {
                v1 = v1.Where(c => c.division_id == divisionid);

            }

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v1 = v1.Where(c => c.hazard_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v1 = v1.Where(c => c.hazard_date <= d_end);
            }


            ICellStyle style3 = setBorder(workbook);
            ICellStyle style4 = setBorder(workbook);
            ICellStyle style5 = setBorder(workbook);
            ICellStyle style6 = setBorder(workbook);
            ICellStyle style7 = setBorder(workbook);

            ArrayList dataJson = new ArrayList();

            int no = 1;
            int count = 3;
            foreach (var rc in v1)
            {
                //string hazard_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.hazard_datetime), lang);
               // string hazard_time = FormatDates.getTimeShowFromDate(Convert.ToDateTime(rc.hazard_datetime), lang);
                //string report_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.report_date), lang);
                //string verify_date = "";
                //if (rc.verifying_date != null)
                //{
                //    verify_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.verifying_date), lang);
                //}

                string type_login = rc.typeuser_login;
                string name_surname_reporter = "";
               

                if (type_login == "contractor")
                {
                    var v = from c in dbConnect.contractors
                            where c.id == Convert.ToInt32(rc.employee_id)
                            select new
                            {
                                prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                                first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                                last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                               
                            };


                    foreach (var rc1 in v)
                    {
                        name_surname_reporter = rc1.prefix + " " + rc1.first_name + " " + rc1.last_name;
                        

                    }

                }
                else
                {
                    if (Session["country"].ToString() == "thailand")
                  {
                        var v = from c in dbConnect.employees
                                where c.employee_id == rc.employee_id
                                select new
                                {
                                    prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                                    first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                                    last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                            
                                };


                        foreach (var rc2 in v)
                        {
                            name_surname_reporter = rc2.prefix + " " + rc2.first_name + " " + rc2.last_name;
                        } 
                    }
                    else if (Session["country"].ToString() == "srilanka")
                    {
                        var v = from c in dbConnect.employees
                               
                                where c.employee_id == rc.employee_id
                                select new
                                {
                                    prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                                    first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                                    last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                               
                                };


                        foreach (var rc2 in v)
                        {
                            name_surname_reporter = rc2.prefix + " " + rc2.first_name + " " + rc2.last_name;
                           
                        }

                    }

                }

               
                string status_delay = "";
                int count_date = Convert.ToDateTime(rc.report_date).Subtract(Convert.ToDateTime(rc.hazard_datetime)).Days;

                if (Session["country"].ToString() == "thailand")
                {
                    if (count_date > 7)
                    {
                        status_delay = chageDataLanguage("ล่าช้า", "delay", lang);
                    }
                    else
                    {
                        status_delay = chageDataLanguage("ปกติ", "normal", lang);
                    }


                }
                else if (Session["country"].ToString() == "srilanka")
                {
                    if (count_date > 14)//2 weeek
                    {
                        status_delay = chageDataLanguage("ล่าช้า", "delay", lang);
                    }
                    else
                    {
                        status_delay = chageDataLanguage("ปกติ", "normal", lang);
                    }


                }

                string type_reporter = "";
                if (rc.typeuser_login == "ad" || rc.typeuser_login == "employee" || rc.typeuser_login == "notify" || rc.typeuser_login == "super")
                {
                    type_reporter = chageDataLanguage("employee", "employee", lang);
                }
                else if (rc.typeuser_login == "contractor")
                {
                    type_reporter = chageDataLanguage("contractor", "contractor", lang);
                }


                string safety_name = getNameAction(rc.safety_officer_id, lang);
               // string report_name = getNameAction(rc.employee_id, lang);
                string area_owner_name = getNameAction(rc.area_owner_id, lang);
               // string function_report = getFunctionReport(rc.employee_id, lang);
               // string department_report = "";

                string name_areamanager = getAreaManager(rc.division_id, lang);
                string name_areasupervisor = getAreaSupervisor(rc.section_id, lang);

                //if (Session["country"].ToString() == "thailand")
                //{
                //    department_report = getDepartmentReport(rc.employee_id, lang);
                //}
                //else if (Session["country"].ToString() == "srilanka")
                //{
                //    department_report = getSubfunctionReport(rc.employee_id, lang);
                //}
                
                

                string process_action = getProcessAction(rc.hazard_id, lang);
                string responsible_person_name = getResponsiblePersonAction(rc.hazard_id, lang);
                string action_name = getAction(rc.hazard_id, lang);
                string status = getStatusStepHazard(rc, rc.id, lang);

                string level = "";
                if (rc.level_hazard == "H")
                {
                    level = Resources.Hazard.high;
                }
                else if (rc.level_hazard == "M")
                {
                    level = Resources.Hazard.medium;
                }
                else if (rc.level_hazard == "L")
                {
                    level = Resources.Hazard.low;
                }

                string type_action = "";
                if (rc.type_action == "P")
                {
                    type_action = Resources.Hazard.pending_action;
                }
                else if (rc.type_action == "T")
                {
                    type_action = Resources.Hazard.temporary_control;
                }
                else if (rc.type_action == "C")
                {
                    type_action = Resources.Hazard.complete_control;
                }

            
                IRow row = sheet1.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(no);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(rc.doc_no);
                cell1.CellStyle = style;


                
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(rc.report_date);
                IDataFormat format1 = workbook.CreateDataFormat();
                style3.DataFormat = format1.GetFormat("dd/mm/yyyy h:mm:ss");
                cell2.CellStyle = style3;


                
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(Convert.ToDateTime(rc.hazard_date));
                IDataFormat format2 = workbook.CreateDataFormat();
                style4.DataFormat = format2.GetFormat("dd/mm/yyyy");
                cell3.CellStyle = style4;


               
                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(Convert.ToDateTime(rc.hazard_date));
                IDataFormat format3 = workbook.CreateDataFormat();
                style5.DataFormat = format3.GetFormat("h:mm");
                cell4.CellStyle = style5;



                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(rc.company_name);
                cell5.CellStyle = style;


                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(rc.function_name);
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(rc.department_name);
                cell7.CellStyle = style;

                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(rc.division_name);
                cell8.CellStyle = style;

                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue(rc.section_name);
                cell9.CellStyle = style;

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(rc.hazard_area);
                cell10.CellStyle = style;

                ICell cell11 = row.CreateCell(11);
                cell11.SetCellValue(rc.hazard_name);
                cell11.CellStyle = style;

                ICell cell12 = row.CreateCell(12);
                cell12.SetCellValue(rc.hazard_detail);
                cell12.CellStyle = style;

                ICell cell13 = row.CreateCell(13);
                cell13.SetCellValue(rc.preliminary_action);
                cell13.CellStyle = style;

                ICell cell14 = row.CreateCell(14);
                cell14.SetCellValue(type_action);
                cell14.CellStyle = style;

                ICell cell15 = row.CreateCell(15);
                cell15.SetCellValue(type_reporter);
                cell15.CellStyle = style;

                ICell cell16 = row.CreateCell(16);
                cell16.SetCellValue(name_surname_reporter);
                cell16.CellStyle = style;

                ICell cell17 = row.CreateCell(17);
                cell17.SetCellValue(rc.reporter_company_name);
                cell17.CellStyle = style;

                ICell cell18 = row.CreateCell(18);
                cell18.SetCellValue(rc.reporter_function_name);
                cell18.CellStyle = style;

                ICell cell19 = row.CreateCell(19);
                cell19.SetCellValue(rc.reporter_department_name);
                cell19.CellStyle = style;

                ICell cell20 = row.CreateCell(20);
                if (rc.verifying_date != null) 
                {
                   
                    cell20.SetCellValue(Convert.ToDateTime(rc.verifying_date));
                    IDataFormat format4 = workbook.CreateDataFormat();
                    style6.DataFormat = format4.GetFormat("dd/mm/yyyy");
                    cell20.CellStyle = style6;
                }
                else
                {
                    cell20.SetCellValue("");
                    cell20.CellStyle = style;
                }
                


                ICell cell21 = row.CreateCell(21);
                cell21.SetCellValue(rc.source_hazard);
                cell21.CellStyle = style;

                ICell cell22 = row.CreateCell(22);
                cell22.SetCellValue(level);
                cell22.CellStyle = style;

                ICell cell23 = row.CreateCell(23);
                cell23.SetCellValue(rc.faltality_prevention_element);
                cell23.CellStyle = style;


                ICell cell24 = row.CreateCell(24);
                cell24.SetCellValue(rc.faltality_prevention_element_other);
                cell24.CellStyle = style;


                ICell cell25 = row.CreateCell(25);
                cell25.SetCellValue(safety_name);
                cell25.CellStyle = style;

                ICell cell26 = row.CreateCell(26);
                cell26.SetCellValue(process_action);
                cell26.CellStyle = style;


                ICell cell27 = row.CreateCell(27);
                cell27.SetCellValue(action_name);
                cell27.CellStyle = style;


                ICell cell28 = row.CreateCell(28);
                cell28.SetCellValue(responsible_person_name);
                cell28.CellStyle = style;


                ICell cell29 = row.CreateCell(29);
                cell29.SetCellValue(area_owner_name);
                cell29.CellStyle = style;

                ICell cell30 = row.CreateCell(30);
                cell30.SetCellValue(rc.status + " " + status);
                cell30.CellStyle = style;


                ICell cell31 = row.CreateCell(31);
                cell31.SetCellValue(name_areamanager);
                cell31.CellStyle = style;

                ICell cell32 = row.CreateCell(32);
                cell32.SetCellValue(name_areasupervisor);
                cell32.CellStyle = style;

                ICell cell33 = row.CreateCell(33);

                if (rc.reason_reject_type != "")
                {
                    cell33.SetCellValue(rc.reason_reject_type);
                }
                else
                {
                    cell33.SetCellValue(rc.reason_reject);
                }
                
                cell33.CellStyle = style;

                ICell cell34 = row.CreateCell(34);
                if (rc.close_hazard_date != null)
                {
                    
                    cell34.SetCellValue(Convert.ToDateTime(rc.close_hazard_date));
                    IDataFormat format6 = workbook.CreateDataFormat();
                    style7.DataFormat = format6.GetFormat("dd/mm/yyyy h:mm");
                    cell34.CellStyle = style7;
                }
                else
                {
                    cell34.SetCellValue("");
                    cell34.CellStyle = style;
                }

              

                ICell cell35 = row.CreateCell(35);
                cell35.SetCellValue(status_delay);
                cell35.CellStyle = style;


                ICell cell36 = row.CreateCell(36);
                cell36.SetCellValue(rc.characteristic);
                cell36.CellStyle = style;


                no++;
                count++;

            }


            setWidthColunm(workbook, sheet1, headers);

           //string path_write = string.Format("{0}\\report\\template\\HazardReport.xlsx", Server.MapPath(@"\"));
            string path_write = string.Format("{0}" + pathreport + "HazardReport.xlsx", Server.MapPath(@"\"));

            using (var f = File.Create(path_write))
            {
                workbook.Write(f);
            }
            Response.WriteFile(path_write);

            Response.Flush();
            Response.End();


        }


        protected string getStatusStepHazard(dynamic rc, int id, string lang)
        {
            string step = "";

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                if (rc.process_status != 2 && rc.process_status != 3)//ไม่ใช้ close กับ reject
                {

                    if (rc.step_form == 1)//area oh&s
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
                    else if (rc.step_form == 2)
                    {
                        string v_step = chageDataLanguage("ตรวจสอบรายงานแหล่งอันตราย", "Verify Hazard Report", lang);

                        if (rc.submit_report_form2 == null)
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
                    else if (rc.step_form == 3)
                    {
                        string v_step = chageDataLanguage("ดำเนินการแก้ไข", "Process of Action", lang);

                        step = step + "(" + v_step + " - Area Supervisor)";

                    }
                    else if (rc.step_form == 4)
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
                                    where c.hazard_id == id && c.status == "A"
                                    && c.group_id == r.group_id
                                    select c;

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


            }//end using


            return step;
        }


        protected string getAreaOHS(string department_id, string lang)
        {
            string value = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.employee_has_departments
                    join e in dbConnect.employees on c.employee_id equals e.employee_id
                    where c.department_id == department_id
                    select new
                    {
                        first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                        last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),

                    };

            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(value))
                {

                    value = rc.first_name + " " + rc.last_name;
                }
                else
                {
                    value = value + ", " + rc.first_name + " " + rc.last_name;

                }
            }


            return value;
        }


        protected string getAreaManager(string division_id, string lang)
        {
            string value = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.employee_has_divisions
                    join e in dbConnect.employees on c.employee_id equals e.employee_id
                    where c.division_id == division_id
                    select new
                    {
                        first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                        last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),

                    };

            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(value))
                {

                    value = rc.first_name + " " + rc.last_name;
                }
                else
                {
                    value = value + ", " + rc.first_name + " " + rc.last_name;

                }
            }


            return value;
        }


        protected string getAreaSupervisor(string section_id, string lang)
        {
            string value = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.employee_has_sections
                    join e in dbConnect.employees on c.employee_id equals e.employee_id
                    where c.section_id == section_id
                    select new
                    {
                        first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                        last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang),

                    };

            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(value))
                {

                    value = rc.first_name + " " + rc.last_name;
                }
                else
                {
                    value = value + ", " + rc.first_name + " " + rc.last_name;

                }
            }


            return value;
        }
        protected ICellStyle setBorder(XSSFWorkbook workbook)
        {

            ICellStyle style = workbook.CreateCellStyle();
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BottomBorderColor = IndexedColors.Black.Index;

            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.LeftBorderColor = IndexedColors.Black.Index;


            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.RightBorderColor = IndexedColors.Black.Index;


            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.TopBorderColor = IndexedColors.Black.Index;

            return style;
        }


     


        protected void setHeader(XSSFWorkbook workbook, ISheet sheet, ArrayList headers)
        {
            ICellStyle style = workbook.CreateCellStyle();
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BottomBorderColor = IndexedColors.Black.Index;

            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.LeftBorderColor = IndexedColors.Black.Index;


            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.RightBorderColor = IndexedColors.Black.Index;


            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.TopBorderColor = IndexedColors.Black.Index;
            IRow row = sheet.CreateRow(2);

            for (int i = 0; i < headers.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(headers[i].ToString());
                cell.CellStyle = style;

            }


        }

        protected void setWidthColunm(XSSFWorkbook workbook, ISheet sheet, ArrayList headers)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                sheet.AutoSizeColumn(i);

            }

        }



        protected string getFunctionReport(string employee_id, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string name = "";
            var v = from c in dbConnect.employees
                    join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                    join f in dbConnect.functions on o.function_id equals f.function_id
                    where c.employee_id == employee_id
                    select new
                    {
                        function_name = chageDataLanguage(f.function_th, f.function_en, lang)


                    };
            foreach (var rc in v)
            {
                name = rc.function_name;
            }

            return name;
        }


        protected string getDepartmentReport(string employee_id, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string name = "";
            var v = from c in dbConnect.employees
                    join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                    join d in dbConnect.departments on o.department_id equals d.department_id
                    where c.employee_id == employee_id
                    select new
                    {
                        department_name = chageDataLanguage(d.department_th, d.department_en, lang)


                    };
            foreach (var rc in v)
            {
                name = rc.department_name;
            }

            return name;
        }


        protected string getSubfunctionReport(string employee_id, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string name = "";
            var v = from c in dbConnect.employees
                    join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                    join d in dbConnect.departments on o.sub_function_id equals d.department_id
                    where c.employee_id == employee_id
                    select new
                    {
                        department_name = chageDataLanguage(d.department_th, d.department_en, lang)


                    };
            foreach (var rc in v)
            {
                name = rc.department_name;
            }

            return name;
        }

        protected string getNameAction(string employee_id, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            string name = "";
            var v = from c in dbConnect.employees
                    where c.employee_id == employee_id
                    select new
                    {
                        first_name = chageDataLanguage(c.first_name_th, c.first_name_en, lang),
                        last_name = chageDataLanguage(c.last_name_th, c.last_name_en, lang)

                    };
            foreach (var rc in v)
            {
                name = rc.first_name + " " + rc.last_name;
            }

            return name;
        }



        protected string getProcessAction(int hazard_id, string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.process_actions
                    join t in dbConnect.type_controls on c.type_control equals t.id
                    where c.hazard_id == hazard_id
                    select new
                    {
                        type_control = chageDataLanguage(t.name_th, t.name_en, lang)

                    };
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(ex))
                {

                    ex = rc.type_control;
                }
                else
                {

                    ex = ex + ", " + rc.type_control;
                }
            }



            return ex;


        }



        protected string getResponsiblePersonAction(int hazard_id, string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.process_actions
                    join e in dbConnect.employees on c.employee_id equals e.employee_id
                    where c.hazard_id == hazard_id
                    select new
                    {
                        first_name = chageDataLanguage(e.first_name_th, e.first_name_en, lang),
                        last_name = chageDataLanguage(e.last_name_th, e.last_name_en, lang)

                    };
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(ex))
                {

                    ex = rc.first_name+" "+rc.last_name;
                }
                else
                {

                    ex = ex + ", " + rc.first_name + " " + rc.last_name;
                }
            }



            return ex;


        }



        protected string getAction(int hazard_id, string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.process_actions
                    join s in dbConnect.action_status on c.action_status_id equals s.id
                    where c.hazard_id == hazard_id
                    select new
                    {
                        c.action,
                        date_complete = c.date_complete.ToString(),
                        c.due_date,
                        c.action_status_id,
                        status = chageDataLanguage(s.name_th, s.name_en, lang),

                    };
           
            foreach (var rc in v)
            {
                    string status = "";          
                    status = rc.status;
                    if (rc.action_status_id == 1)//on process
                    { 
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.Now.Date > Convert.ToDateTime(rc.due_date).Date)
                            {
                              
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date).Date)
                            {
                               
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }

                    }
                    else if (rc.action_status_id == 4)//close
                    {
                       
                        status = rc.status;
                    }
                    else if (rc.action_status_id == 5)//cancel
                    {
                       
                        status = rc.status;
                    }
                    else if (rc.action_status_id == 6)
                    {//reject

                       
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.Now.Date > Convert.ToDateTime(rc.due_date).Date)
                            {
                              
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date).Date)
                            {
                                
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else if (rc.action_status_id == 2)
                    {//request close

                       
                        status = rc.status;

                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.Now.Date > Convert.ToDateTime(rc.due_date).Date)
                            {
                               
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date).Date)
                            {
                                
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }

                if (string.IsNullOrEmpty(ex))
                {  

                    ex = rc.action +" ("+status+")";
                }
                else
                {

                    ex = ex + ", " + rc.action + " (" + status + ")";
                }
            }



            return ex;


        }



        protected string searchBy(string function_id, string department_id, string division_id, string date_start, string date_end, string lang)
        {
            string searchby = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (function_id != "")
            {
                var f = from c in dbConnect.functions
                        where c.function_id == function_id
                        select new
                        {
                            function_name = chageDataLanguage(c.function_th, c.function_en, lang)
                        };

                foreach (var r in f)
                {
                    searchby = searchby + Resources.Hazard.lbfucntion + " :" + r.function_name;
                }

            }
            else
            {
                searchby = searchby + Resources.Hazard.lbfucntion + " :" + Resources.Main.all;

            }




            if (department_id != "")
            {
                var de = from c in dbConnect.departments
                         where c.department_id == department_id
                         select new
                         {
                             department_name = chageDataLanguage(c.department_th, c.department_en, lang)
                         };

                foreach (var e in de)
                {
                    searchby = searchby + ", " + Resources.Hazard.lbdepartment + " :" + e.department_name;
                }
            }
            else
            {
                searchby = searchby + ", " + Resources.Hazard.lbdepartment + " :" + Resources.Main.all;

            }




            if (division_id != "")
            {
                var di = from c in dbConnect.divisions
                         where c.division_id == division_id
                         select new
                         {
                             division_name = chageDataLanguage(c.division_th, c.division_en, lang)
                         };

                foreach (var i in di)
                {
                    searchby = searchby + ", " + Resources.Hazard.lbdivision + " :" + i.division_name;
                }
            }
            else
            {
                searchby = searchby + ", " + Resources.Hazard.lbdivision + " :" + Resources.Main.all;

            }


            if (date_start != "")
            {
                searchby = searchby + ", " + Resources.Hazard.date + " :" + date_start;
            }

            if (date_end != "")
            {
                searchby = searchby + " - " + date_end;
            }




            return searchby;
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