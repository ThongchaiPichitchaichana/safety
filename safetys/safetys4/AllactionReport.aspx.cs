using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
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
    public partial class AllactionReport : System.Web.UI.Page
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

                    LinkButton link = (LinkButton)Master.FindControl("btAllactionReport");
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


            string filename = "ActionAllReport.xlsx";
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

           string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
           string path = string.Format("{0}" + pathreport + "all_action.xlsx", Server.MapPath(@"\"));
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("action");

            ArrayList headers = new ArrayList();

            headers.Add(Resources.Incident.sequence);
            headers.Add(Resources.Main.doc_no);
            headers.Add(Resources.Hazard.action);
            headers.Add(Resources.Hazard.responsible_person);
            headers.Add(Resources.Hazard.lbdepartment_action);
            headers.Add(Resources.Hazard.typecontrol);
            headers.Add(Resources.Hazard.due_date);
            headers.Add(Resources.Hazard.status);


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
            string report_type = Request.Form[ddReporttype.UniqueID];
            string date_start = txtstart_date.Value;
            string date_end = txtend_date.Value;
            string type_area = "AREA";
            if(type_area2.Checked)
            {
                type_area = type_area2.Value;
            }
          
       
            string lang = Session["lang"].ToString();
            int REJECT_STATUS = 3;
            int EXEMPTION_STATUS = 4;

            string seach_by = searchBy(report_type, date_start, date_end, lang);
            IRow row_seach = sheet1.GetRow(1);

            ICell cell_search = row_seach.GetCell(1);
            cell_search.SetCellValue(seach_by);

            //for (int i = 2; i <= 4; i++)
            //{
            //    ICellStyle style2 = workbook.CreateCellStyle();

            //    style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            //    style2.TopBorderColor = IndexedColors.Black.Index;

            //    if (i == 4)
            //    {
            //        style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            //        style2.RightBorderColor = IndexedColors.Black.Index;
            //    }

            //    ICell cell_n = row_seach.CreateCell(i);
            //    cell_n.CellStyle = style2;



            //}

            //CellRangeAddress range = new CellRangeAddress(1, 1, 1, 4);
            //sheet1.AddMergedRegion(range);
;


            safetys4dbDataContext dbConnect = new safetys4dbDataContext();



            if (report_type == "hazard")
            {
                var v1 = from h in dbConnect.hazards
                         join p in dbConnect.process_actions on h.id equals p.hazard_id
                         join t in dbConnect.type_controls on p.type_control equals t.id
                         join a in dbConnect.action_status on p.action_status_id equals a.id
                         join em in dbConnect.employees on p.employee_id equals em.employee_id into joinE
                         from em in joinE.DefaultIfEmpty()
                         join o in dbConnect.organizations on em.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                         from d in joinD.DefaultIfEmpty()

                         where h.country == Session["country"].ToString() && h.process_status != REJECT_STATUS
                         orderby h.hazard_date ascending
                         select new
                         {
                             h.company_id,
                             h.function_id,
                             h.department_id,
                             h.division_id,
                             h.doc_no,
                             h.hazard_date,
                             h.id,
                             p.responsible_person,
                             type_control = chageDataLanguage(t.name_th, t.name_en, lang),
                             p.action,
                             p.due_date,
                             date_complete = p.date_complete.ToString(),
                             due_date2 = p.due_date,
                             p.action_status_id,
                             status = chageDataLanguage(a.name_th,a.name_en,lang),
                             department_name = chageDataLanguage(d.department_th, d.department_en, lang)



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




                ArrayList dataJson = new ArrayList();

                int no = 1;
                int count = 3;

                ICellStyle style2 = setBorder(workbook);
                foreach (var rc in v1)
                {
                   // string due_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.due_date), lang);

                    IRow row = sheet1.CreateRow(count);
                    ICell cell = row.CreateCell(0);
                    cell.SetCellValue(no);
                    cell.CellStyle = style;

                    ICell cell1 = row.CreateCell(1);
                    cell1.SetCellValue(rc.doc_no);
                    cell1.CellStyle = style;

                    ICell cell2 = row.CreateCell(2);
                    cell2.SetCellValue(rc.action);
                    cell2.CellStyle = style;

                    ICell cell3 = row.CreateCell(3);
                    cell3.SetCellValue(rc.responsible_person);
                    cell3.CellStyle = style;


                    ICell cell4 = row.CreateCell(4);
                    cell4.SetCellValue(rc.department_name);
                    cell4.CellStyle = style;

                    ICell cell5 = row.CreateCell(5);
                    cell5.SetCellValue(rc.type_control);
                    cell5.CellStyle = style;

                   
                    IDataFormat format = workbook.CreateDataFormat();
                    ICell cell6 = row.CreateCell(6);
                    cell6.SetCellValue(Convert.ToDateTime(rc.due_date));
                    style2.DataFormat = format.GetFormat("dd/mm/yyyy");
                    cell6.CellStyle = style2;


                    string status = "";
                    if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                    {
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else
                    {
                        status = rc.status;

                    }



                    ICell cell7 = row.CreateCell(7);
                    cell7.SetCellValue(status);
                    cell7.CellStyle = style;




                    no++;
                    count++;

                }




            }
            else if (report_type == "incident")
            {

                var v1 = from i in dbConnect.incidents
                         join c in dbConnect.corrective_prevention_action_incidents on i.id equals c.incident_id
                         join a in dbConnect.action_status on c.action_status_id equals a.id
                         join em in dbConnect.employees on c.employee_id equals em.employee_id into joinE
                         from em in joinE.DefaultIfEmpty()
                         join o in dbConnect.organizations on em.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                         from d in joinD.DefaultIfEmpty()

                         where i.country == Session["country"].ToString() && i.process_status != REJECT_STATUS
                         && i.process_status != EXEMPTION_STATUS
                         orderby i.incident_date ascending
                         select new
                         {
                             i.company_id,
                             i.function_id,
                             i.department_id,
                             i.division_id,
                             i.activity_company_id,
                             i.activity_department_id,
                             i.activity_division_id,
                             i.activity_function_id,
                           
                             i.doc_no,
                             i.incident_date,
                             i.id,
                             c.responsible_person,
                             type_control = "",
                             action_name = c.corrective_preventive_action,
                             c.due_date,
                             date_complete = c.date_complete.ToString(),
                             due_date2 = c.due_date,
                             c.action_status_id,
                             status = chageDataLanguage(a.name_th, a.name_en, lang),
                             department_name = chageDataLanguage(d.department_th, d.department_en, lang),
                             incident_datetime = i.incident_date,


                         };

                if (type_area=="AREA")
                {
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


                }
                else
                {

                    if (companyid != "" && companyid != "null")
                    {
                        v1 = v1.Where(c => c.activity_company_id == companyid);

                    }

                    if (functionid != "" && functionid != "null")
                    {
                        v1 = v1.Where(c => c.activity_function_id == functionid);

                    }

                    if (departmentid != "" && departmentid != "null")
                    {
                        v1 = v1.Where(c => c.activity_department_id == departmentid);

                    }

                    if (divisionid != "" && divisionid != "null")
                    {
                        v1 = v1.Where(c => c.activity_division_id == divisionid);

                    }


                }

     

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v1 = v1.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v1 = v1.Where(c => c.incident_date <= d_end);
                }





                var v2 = from i in dbConnect.incidents
                         join c in dbConnect.preventive_action_incidents on i.id equals c.incident_id
                         join a in dbConnect.action_status on c.action_status_id equals a.id
                         join em in dbConnect.employees on c.employee_id equals em.employee_id into joinE
                         from em in joinE.DefaultIfEmpty()
                         join o in dbConnect.organizations on em.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                         from d in joinD.DefaultIfEmpty()

                         where i.country == Session["country"].ToString() && i.process_status != REJECT_STATUS
                         && i.process_status != EXEMPTION_STATUS
                         orderby i.incident_date ascending
                         select new
                         {
                             i.company_id,
                             i.function_id,
                             i.department_id,
                             i.division_id,
                             i.activity_company_id,
                             i.activity_department_id,
                             i.activity_division_id,
                             i.activity_function_id,

                             i.doc_no,
                             i.incident_date,
                             i.id,
                             c.responsible_person,
                             type_control = "",
                             action_name = c.preventive_action,
                             c.due_date,
                             date_complete = c.date_complete.ToString(),
                             due_date2 = c.due_date,
                             c.action_status_id,
                             status = chageDataLanguage(a.name_th, a.name_en, lang),
                             department_name = chageDataLanguage(d.department_th, d.department_en, lang),
                             incident_datetime = i.incident_date,


                         };

                if (type_area == "AREA")
                {

                    if (companyid != "" && companyid != "null")
                    {
                        v2 = v2.Where(c => c.company_id == companyid);

                    }

                    if (functionid != "" && functionid != "null")
                    {
                        v2 = v2.Where(c => c.function_id == functionid);

                    }

                    if (departmentid != "" && departmentid != "null")
                    {
                        v2 = v2.Where(c => c.department_id == departmentid);

                    }

                    if (divisionid != "" && divisionid != "null")
                    {
                        v2 = v2.Where(c => c.division_id == divisionid);

                    }
                }
                else
                {
                    if (companyid != "" && companyid != "null")
                    {
                        v2 = v2.Where(c => c.activity_company_id == companyid);

                    }

                    if (functionid != "" && functionid != "null")
                    {
                        v2 = v2.Where(c => c.activity_function_id == functionid);

                    }

                    if (departmentid != "" && departmentid != "null")
                    {
                        v2 = v2.Where(c => c.activity_department_id == departmentid);

                    }

                    if (divisionid != "" && divisionid != "null")
                    {
                        v2 = v2.Where(c => c.activity_division_id == divisionid);

                    }



                }

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v2 = v2.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v2 = v2.Where(c => c.incident_date <= d_end);
                }




                var v = v1.Concat(v2);




                var v3 = from i in dbConnect.incidents
                         join c in dbConnect.consequence_management_incidents on i.id equals c.incident_id
                         join a in dbConnect.action_status on c.action_status_id equals a.id
                         join em in dbConnect.employees on c.employee_id equals em.employee_id into joinE
                         from em in joinE.DefaultIfEmpty()
                         join o in dbConnect.organizations on em.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                         from d in joinD.DefaultIfEmpty()

                         where i.country == Session["country"].ToString() && i.process_status != REJECT_STATUS
                         && i.process_status != EXEMPTION_STATUS
                         orderby i.incident_date ascending
                         select new
                         {
                             i.company_id,
                             i.function_id,
                             i.department_id,
                             i.division_id,

                             i.activity_company_id,
                             i.activity_department_id,
                             i.activity_division_id,
                             i.activity_function_id,
                             i.doc_no,
                             i.incident_date,
                             i.id,
                             c.responsible_person,
                             type_control = "",
                             action_name = c.consequence_management,
                             c.due_date,
                             date_complete = c.date_complete.ToString(),
                             due_date2 = c.due_date,
                             c.action_status_id,
                             status = chageDataLanguage(a.name_th, a.name_en, lang),
                             department_name = chageDataLanguage(d.department_th, d.department_en, lang),
                             incident_datetime = i.incident_date,


                         };


                if (type_area == "AREA")
                {

                    if (companyid != "" && companyid != "null")
                    {
                        v3 = v3.Where(c => c.company_id == companyid);

                    }

                    if (functionid != "" && functionid != "null")
                    {
                        v3 = v3.Where(c => c.function_id == functionid);

                    }

                    if (departmentid != "" && departmentid != "null")
                    {
                        v3 = v3.Where(c => c.department_id == departmentid);

                    }

                    if (divisionid != "" && divisionid != "null")
                    {
                        v3 = v3.Where(c => c.division_id == divisionid);

                    }

                }
                else
                {
                    if (companyid != "" && companyid != "null")
                    {
                        v3 = v3.Where(c => c.activity_company_id == companyid);

                    }

                    if (functionid != "" && functionid != "null")
                    {
                        v3 = v3.Where(c => c.activity_function_id == functionid);

                    }

                    if (departmentid != "" && departmentid != "null")
                    {
                        v3 = v3.Where(c => c.activity_department_id == departmentid);

                    }

                    if (divisionid != "" && divisionid != "null")
                    {
                        v3 = v3.Where(c => c.activity_division_id == divisionid);

                    }


                }

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v3 = v3.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v3 = v3.Where(c => c.incident_date <= d_end);
                }


                var newv = v.Concat(v3).OrderBy(c => c.incident_datetime);

                ArrayList dataJson = new ArrayList();

                int no = 1;
                int count = 3;

                ICellStyle style2 = setBorder(workbook);
                foreach (var rc in newv)
                {
                   // string due_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.due_date), lang);

                    IRow row = sheet1.CreateRow(count);
                    ICell cell = row.CreateCell(0);
                    cell.SetCellValue(no);
                    cell.CellStyle = style;

                    ICell cell1 = row.CreateCell(1);
                    cell1.SetCellValue(rc.doc_no);
                    cell1.CellStyle = style;

                    ICell cell2 = row.CreateCell(2);
                    cell2.SetCellValue(rc.action_name);
                    cell2.CellStyle = style;

                    ICell cell3 = row.CreateCell(3);
                    cell3.SetCellValue(rc.responsible_person);
                    cell3.CellStyle = style;

                    ICell cell4 = row.CreateCell(4);
                    cell4.SetCellValue(rc.department_name);
                    cell4.CellStyle = style;

                    ICell cell5 = row.CreateCell(5);
                    cell5.SetCellValue(rc.type_control);
                    cell5.CellStyle = style;


                    IDataFormat format = workbook.CreateDataFormat();
                    ICell cell6 = row.CreateCell(6);
                    cell6.SetCellValue(Convert.ToDateTime(rc.due_date));
                    style2.DataFormat = format.GetFormat("dd/mm/yyyy");
                    cell6.CellStyle = style2;


                    string status = "";
                    if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                    {
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else
                    {
                        status = rc.status;

                    }

                    ICell cell7 = row.CreateCell(7);
                    cell7.SetCellValue(status);
                    cell7.CellStyle = style;


                    no++;
                    count++;

                }








            }else if (report_type == "sot")
            {
                var v1 = from h in dbConnect.sots
                         join p in dbConnect.process_action_sots on h.id equals p.sot_id
                         join t in dbConnect.type_controls on p.type_control equals t.id
                         join a in dbConnect.sot_action_status on p.action_status_id equals a.id
                         join em in dbConnect.employees on p.employee_id equals em.employee_id into joinE
                         from em in joinE.DefaultIfEmpty()
                         join o in dbConnect.organizations on em.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                         from d in joinD.DefaultIfEmpty()

                         where h.country == Session["country"].ToString()
                         orderby h.sot_date ascending
                         select new
                         {
                             h.company_id,
                             h.function_id,
                             h.department_id,
                             h.division_id,
                             h.doc_no,
                             h.sot_date,
                             h.sot_date_end,
                             h.id,
                             p.responsible_person,
                             type_control = chageDataLanguage(t.name_th, t.name_en, lang),
                             p.action,
                             p.due_date,
                             date_complete = p.date_complete.ToString(),
                             due_date2 = p.due_date,
                             p.action_status_id,
                             status = chageDataLanguage(a.name_th, a.name_en, lang),
                             department_name = chageDataLanguage(d.department_th, d.department_en, lang)


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

                    v1 = v1.Where(c => c.sot_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v1 = v1.Where(c => c.sot_date_end <= d_end);
                }




                ArrayList dataJson = new ArrayList();

                int no = 1;
                int count = 3;
                ICellStyle style2 = setBorder(workbook);
                foreach (var rc in v1)
                {
                    //string due_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.due_date), lang);

                    IRow row = sheet1.CreateRow(count);
                    ICell cell = row.CreateCell(0);
                    cell.SetCellValue(no);
                    cell.CellStyle = style;

                    ICell cell1 = row.CreateCell(1);
                    cell1.SetCellValue(rc.doc_no);
                    cell1.CellStyle = style;

                    ICell cell2 = row.CreateCell(2);
                    cell2.SetCellValue(rc.action);
                    cell2.CellStyle = style;

                    ICell cell3 = row.CreateCell(3);
                    cell3.SetCellValue(rc.responsible_person);
                    cell3.CellStyle = style;


                    ICell cell4 = row.CreateCell(4);
                    cell4.SetCellValue(rc.department_name);
                    cell4.CellStyle = style;

                    ICell cell5 = row.CreateCell(5);
                    cell5.SetCellValue(rc.type_control);
                    cell5.CellStyle = style;

                    //ICell cell5 = row.CreateCell(5);
                    //cell5.SetCellValue(due_date);
                    //cell5.CellStyle = style;

                    IDataFormat format = workbook.CreateDataFormat();
                    ICell cell6 = row.CreateCell(6);
                    cell6.SetCellValue(Convert.ToDateTime(rc.due_date));
                    style2.DataFormat = format.GetFormat("dd/mm/yyyy");
                    cell6.CellStyle = style2;



                    string status = "";
                    if (rc.action_status_id != 5 && rc.action_status_id != 4)//cancel,close
                    {
                        status = rc.status;
                        if (string.IsNullOrEmpty(rc.date_complete))
                        {
                            if (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }
                        }
                        else
                        {
                            if (Convert.ToDateTime(rc.date_complete).Date > Convert.ToDateTime(rc.due_date2).Date)
                            {
                                status = chageDataLanguage("ล่าช้า", "delay", lang);
                            }

                        }
                    }
                    else
                    {
                        status = rc.status;

                    }

                    ICell cell7 = row.CreateCell(7);
                    cell7.SetCellValue(status);
                    cell7.CellStyle = style;


                    no++;
                    count++;

                }




            }

           


            setWidthColunm(workbook, sheet1, headers);

            string path_write = string.Format("{0}" + pathreport + "AllAcitonReport.xlsx", Server.MapPath(@"\"));

            using (var f = File.Create(path_write))
            {
                workbook.Write(f);
            }
            Response.WriteFile(path_write);

            Response.Flush();
            Response.End();


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



 


        protected string searchBy(string report_type, string date_start, string date_end, string lang)
        {
            string searchby = "";
           
            if (report_type == "incident")
            {
                searchby = searchby + Resources.Main.lbtypereport + " : Incident";

            }else if(report_type == "hazard")
            {
                searchby = searchby + Resources.Main.lbtypereport + " : Hazard" ;
            }
            else if (report_type == "sot")
            {
                searchby = searchby + Resources.Main.lbtypereport + " : SOT";
            }
       




            if (date_start != "")
            {
                searchby = searchby + ", " + Resources.Incident.date + " :" + date_start;
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