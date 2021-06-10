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
    public partial class HealthReport : System.Web.UI.Page
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

                    LinkButton link = (LinkButton)Master.FindControl("btAllhealthReport");
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
            string filename = "HealthReport.xlsx";
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

            string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
            //string pathreport = "D:\\SourceCode\\safetys\\safetys\\safetys4\\report\\template";
            //string path = string.Format("{0}\\report\\template\\hazard_report.xlsx", Server.MapPath(@"\"));
           string path = string.Format("{0}" + pathreport + "health_report.xlsx", Server.MapPath(@"\"));
           // D:\SourceCode
             //   Response.Write(path); Response.End();
             FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("health_report");

            ArrayList headers = new ArrayList();

            headers.Add(Resources.Incident.sequence);
            headers.Add(Resources.Health.doc_no);
            headers.Add(Resources.Health.lbemployeeid);
            headers.Add(Resources.Health.lbemployee_name);
            headers.Add(Resources.Health.lbyear);
            headers.Add(Resources.Health.lbhospital);
            headers.Add(Resources.Health.report_date);
            headers.Add(Resources.Health.lbCompany);
            headers.Add(Resources.Health.lbfucntion);
            headers.Add(Resources.Health.lbdepartment);
            headers.Add(Resources.Health.lbdivision);
            headers.Add(Resources.Health.lbsection);
            headers.Add(Resources.Health.lbage);
            headers.Add(Resources.Health.lbserviceyear);
            headers.Add(Resources.Health.lbserviceyear_current);
            headers.Add(Resources.Health.lbjobtype);


            headers.Add(Resources.Health.lbNameReportHeader);
            headers.Add(Resources.Health.lbcompany_reporter);
            headers.Add(Resources.Health.lbfunction_reporter);
            headers.Add(Resources.Health.lbdepartment_reporter);
            headers.Add(Resources.Health.status);
            headers.Add(Resources.Health.lbreasonreject);
            headers.Add(Resources.Health.detailreject);
            headers.Add(Resources.Main.lbdateclose);
            headers.Add(Resources.Health.significantorinsignificant);


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



            for (int i = 4; i <= 24; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 24)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach.CreateCell(i);
                cell_n.CellStyle = style2;



            }

            CellRangeAddress range = new CellRangeAddress(1, 1, 4, 24);
            sheet1.AddMergedRegion(range);

            ICell cell_search = row_seach.GetCell(4);
            cell_search.SetCellValue(seach_by);

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from h in dbConnect.healths
                     join em in dbConnect.employees on h.health_employee_id equals em.employee_id
                     join ht in dbConnect.health_status on h.process_status equals ht.id
                     join rs in dbConnect.reason_rejects on h.reason_reject_type equals rs.id into joinReason
                     join ho in dbConnect.hospitals on h.hospital_id equals ho.id into joinHospital

                     from ho in joinHospital.DefaultIfEmpty()
                     from rs in joinReason.DefaultIfEmpty()
                     where h.country == Session["country"].ToString()
                     orderby h.report_date ascending
                     select new
                     {

                         report_date = h.report_date,
                         company_name = chageDataLanguage(h.location_company_name_th, h.location_company_name_en, lang),
                         function_name = chageDataLanguage(h.location_function_name_th, h.location_function_name_en, lang),
                         department_name = chageDataLanguage(h.location_department_name_th, h.location_department_name_en, lang),
                         division_name = chageDataLanguage(h.location_division_name_th, h.location_division_name_en, lang),
                         section_name = chageDataLanguage(h.location_section_name_th, h.location_section_name_en, lang),

                         hostpital_name = chageDataLanguage(ho.name_th,ho.name_en,lang),
                         reason_reject_type_name = chageDataLanguage(rs.name_th,rs.name_en,lang),

                         h.health_employee_id,
                         h.year_health,
                         h.age,
                         h.service_year_current,
                         h.service_year,
                       
                         h.employee_id,
                        

                         status = chageDataLanguage(ht.name_th, ht.name_en, lang),
                         company_id = h.company_id,
                         function_id = h.function_id,
                         department_id = h.department_id,
                         division_id = h.division_id,
                         section_id = h.section_id,
                         health_id = h.id,
                         h.job_type_machine_type,
                         h.doc_no,
                         h.typeuser_login,
                         h.id,
                         h.step_form,
                         h.process_status,
                         h.request_close_form,
                         h.confirm_form_one_to_two_at,
                         h.close_health_date,
                         h.reporter_company_name,
                         h.reporter_function_name,
                         h.reporter_department_name,
                         h.reporter_division_name,
                         h.reporter_section_name,
                         first_name = chageDataLanguage(em.first_name_th, em.first_name_en, lang),
                         last_name = chageDataLanguage(em.last_name_th, em.last_name_en, lang),
                         h.significant_insignificant,
                         h.reason_reject,

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

                v1 = v1.Where(c => c.report_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v1 = v1.Where(c => c.report_date <= d_end);
            }



            //////////////////////////////////////////////ภายใต้ area //////////////////////////////////////////////////////////////////////////////
            ArrayList fu = Session["area_function"] as ArrayList;
            var functions = fu.Cast<string>().ToList();

            ArrayList de = Session["area_department"] as ArrayList;
            var departments = de.Cast<string>().ToList();

            ArrayList department_fun = Session["area_department_functional"] as ArrayList;
            var department_functional = department_fun.Cast<string>().ToList();

            ArrayList di = Session["area_division"] as ArrayList;
            var divisions = di.Cast<string>().ToList();

            ArrayList sec = Session["area_section"] as ArrayList;
            var sections = sec.Cast<string>().ToList();

            ArrayList groups = Session["group"] as ArrayList;
            var group = groups.Cast<string>().ToList();
            // bool area_all = false;

            if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S") > -1)
            {
                //area_all = true;
            }
            else
            {


                if (group.IndexOf("Delegate OH&S Admin") > -1 || group.IndexOf("OH&S Admin") > -1 || group.IndexOf("Area Functional Manager") > -1)
                {

                    if (group.IndexOf("Delegate OH&S Admin") > -1 || group.IndexOf("OH&S Admin") > -1)
                    {
                        v1 = v1.Where(c => functions.Contains(c.function_id));
                    }
                    else
                    {
                        v1 = v1.Where(c => department_functional.Contains(c.department_id));

                    }

                }
                else
                {

                    if (group.IndexOf("Area OH&S") > -1)//area oh&s ดึงเฉพาะฟอร์มที่ตัวเองกรอกเท่านั้น
                    {
                        v1 = v1.Where(c => c.employee_id.Contains(Session["user_id"].ToString()));

                    }



                }



            }











            ICellStyle style3 = setBorder(workbook);
            //ICellStyle style4 = setBorder(workbook);
            //ICellStyle style5 = setBorder(workbook);
            //ICellStyle style6 = setBorder(workbook);
            ICellStyle style7 = setBorder(workbook);
            ICellStyle style8 = setBorder(workbook);

            ArrayList dataJson = new ArrayList();

            int no = 1;
            int count = 3;
            foreach (var rc in v1)
            {
                

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


                string process_action = getProcessAction(rc.health_id, lang);
                string occupational_health = getOccupationalHealth(rc.health_id, lang);
                string risk_factor = getRiskFactor(rc.health_id, lang);
                string action_name = getAction(rc.health_id, lang);
                string status = getStatusStepHealth(rc, rc.id, lang);



                IRow row = sheet1.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(no);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(rc.doc_no);
                cell1.CellStyle = style;


                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(rc.health_employee_id);
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(rc.first_name + " " + rc.last_name);
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(rc.year_health);
                cell4.CellStyle = style;


                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(rc.hostpital_name);
                cell5.CellStyle = style;

                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(rc.report_date);
                IDataFormat format1 = workbook.CreateDataFormat();
                style3.DataFormat = format1.GetFormat("dd/mm/yyyy h:mm:ss");
                cell6.CellStyle = style3;


                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(rc.company_name);
                cell7.CellStyle = style;


                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(rc.function_name);
                cell8.CellStyle = style;

                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue(rc.department_name);
                cell9.CellStyle = style;

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(rc.division_name);
                cell10.CellStyle = style;

                ICell cell11 = row.CreateCell(11);
                cell11.SetCellValue(rc.section_name);
                cell11.CellStyle = style;

                ICell cell12 = row.CreateCell(12);
                cell12.SetCellValue(rc.age.ToString());
                cell12.CellStyle = style;

                ICell cell13 = row.CreateCell(13);
                //Convert.ToString(rc.service_year)
                //change SetCellValue(rc.service_year.ToString()); 
                //to covert.tostring to handel null values 12/05/2020.
                cell13.SetCellValue(Convert.ToString(rc.service_year));
                    cell13.CellStyle = style;
                

                ICell cell14 = row.CreateCell(14);
                cell14.SetCellValue(rc.service_year_current.ToString());
                cell14.CellStyle = style;


                ICell cell15 = row.CreateCell(15);
                cell15.SetCellValue(rc.job_type_machine_type);
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
                cell20.SetCellValue(rc.status + " " + status);
                cell20.CellStyle = style;

                ICell cell21 = row.CreateCell(21);
                cell21.SetCellValue(rc.reason_reject_type_name);
                cell21.CellStyle = style;

                ICell cell22 = row.CreateCell(22);
                cell22.SetCellValue(rc.reason_reject);
                cell22.CellStyle = style;



                ICell cell23 = row.CreateCell(23);
                if (rc.close_health_date != null)
                {

                    cell23.SetCellValue(Convert.ToDateTime(rc.close_health_date));
                    IDataFormat format6 = workbook.CreateDataFormat();
                    style7.DataFormat = format6.GetFormat("dd/mm/yyyy h:mm");
                    cell23.CellStyle = style7;
                }
                else
                {
                    cell23.SetCellValue("");
                    cell23.CellStyle = style;
                }

                ICell cell24 = row.CreateCell(24);
             
                if (rc.significant_insignificant == "sign")
                {
                    cell24.SetCellValue(Resources.Health.significant);
                }
                else if (rc.significant_insignificant == "insign")
                {
                    cell24.SetCellValue(Resources.Health.insignificant);
                }else{
                     cell24.SetCellValue("");
                }
                cell24.CellStyle = style;


                no++;
                count++;

            }


            ////////////////////////////////////////////////end sheet1//////////////////////////////////////////////////////


            ISheet sheet2 = workbook.GetSheet("risk_factor_related_work");

            ArrayList headers2 = new ArrayList();

            headers2.Add(Resources.Incident.sequence);
            headers2.Add(Resources.Health.doc_no);
            headers2.Add(Resources.Health.lbemployeeid);
            headers2.Add(Resources.Health.lbCompany);
            headers2.Add(Resources.Health.lbemployee_name);
            headers2.Add(Resources.Health.risk_factor_relate_work);
            headers2.Add(Resources.Health.monitoringenvironment);
            headers2.Add(Resources.Health.year_risk_factor_relate_work);
            headers2.Add(Resources.Health.result_risk_factor);
            headers2.Add(Resources.Health.duration_risk_factor);
            headers2.Add(Resources.Health.monitoring_results);
           
            setHeader(workbook, sheet2, headers2);




            ///////////////////////////////////////////////set value /////////////////////////////////////////////////////
           

            string seach_by2 = searchBy(functionid, departmentid, divisionid, date_start, date_end, lang);
            IRow row_seach2 = sheet2.GetRow(1);



            for (int i = 2; i <= 10; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 10)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach2.CreateCell(i);
                cell_n.CellStyle = style2;



            }

            CellRangeAddress range2 = new CellRangeAddress(1, 1, 2, 10);
            sheet2.AddMergedRegion(range2);

            ICell cell_search2 = row_seach2.GetCell(2);
            cell_search2.SetCellValue(seach_by2);




            var v2 = from r in dbConnect.risk_factor_relate_work_actions
                     join h in dbConnect.healths on r.health_id equals h.id
                     join em in dbConnect.employees on h.health_employee_id equals em.employee_id
                     join ri in dbConnect.risk_factor_relate_works on r.risk_factor_relate_work_id equals ri.id
                     join d in dbConnect.duration_risk_factors on r.duration_risk_factor_id equals d.id
                     

                     where h.country == Session["country"].ToString() && r.status == "A"
                     orderby h.report_date ascending
                     select new
                     {
                         h.employee_id,
                         report_date = h.report_date,
                         h.health_employee_id,   
                         first_name = chageDataLanguage(em.first_name_th,em.first_name_en,lang),
                         last_name = chageDataLanguage(em.last_name_th, em.last_name_en, lang),
                         company_id = h.company_id,
                         function_id = h.function_id,
                         department_id = h.department_id,
                         division_id = h.division_id,
                         section_id = h.section_id,
                         h.location_company_name_en,
                         h.location_company_name_th,
                         health_id = h.id,
                         h.doc_no,
                         risk_factor_relate_work = chageDataLanguage(ri.name_th,ri.name_en,lang),
                         duration_risk_factor = chageDataLanguage(d.name_th,d.name_en,lang),
                         r.year,
                         r.result,
                         r.monitoring_environment,
                         r.monitoring_results,
                       

                     };

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

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v2 = v2.Where(c => c.report_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v2 = v2.Where(c => c.report_date <= d_end);
            }



            if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S") > -1)
            {
                //area_all = true;
            }
            else
            {


                if (group.IndexOf("Delegate OH&S Admin") > -1 || group.IndexOf("OH&S Admin") > -1 || group.IndexOf("Area Functional Manager") > -1)
                {

                    if (group.IndexOf("Delegate OH&S Admin") > -1 || group.IndexOf("OH&S Admin") > -1)
                    {
                        v2 = v2.Where(c => functions.Contains(c.function_id));
                    }
                    else
                    {
                        v2 = v2.Where(c => department_functional.Contains(c.department_id));

                    }

                }
                else
                {

                    if (group.IndexOf("Area OH&S") > -1)//area oh&s ดึงเฉพาะฟอร์มที่ตัวเองกรอกเท่านั้น
                    {
                        v2 = v2.Where(c => c.employee_id.Contains(Session["user_id"].ToString()));

                    }



                }



            }






            int no2 = 1;
            int count2 = 3;
            foreach (var rc in v2)
            {
                string company_name = chageDataLanguage(rc.location_company_name_th, rc.location_company_name_en, lang);
                string monitoring_environment = "";
                if (rc.monitoring_environment == "Y")
                {
                    monitoring_environment = Resources.Health.lbyes;
                }
                else if (rc.monitoring_environment == "N")
                {
                    monitoring_environment = Resources.Health.lbno;
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



                IRow row = sheet2.CreateRow(count2);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(no2);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(rc.doc_no);
                cell1.CellStyle = style;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(rc.health_employee_id);
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(company_name);
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(rc.first_name+" "+rc.last_name);
                cell4.CellStyle = style;


                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(rc.risk_factor_relate_work);
                cell5.CellStyle = style;


                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(monitoring_environment);
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(rc.year);
                cell7.CellStyle = style;


                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(rc.result);
                cell8.CellStyle = style;

                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue(rc.duration_risk_factor);
                cell9.CellStyle = style;

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(monitoring_results);
                cell10.CellStyle = style;


                no2++;
                count2++;

            }

            //////////////////////////////////////////////end sheet2///////////////////////////////////////////////



            ISheet sheet3 = workbook.GetSheet("health_check_report");

            ArrayList headers3 = new ArrayList();

            headers3.Add(Resources.Incident.sequence);
            headers3.Add(Resources.Health.doc_no);
            headers3.Add(Resources.Health.lbemployeeid);
            headers3.Add(Resources.Health.lbemployee_name);
            headers3.Add(Resources.Health.occupational_health_report);
            headers3.Add(Resources.Health.lbabnormalaudiogram);
            headers3.Add(Resources.Health.lbabnormal_pulmonary_function);
            headers3.Add(Resources.Health.lbhearing_threshold_level);     
            headers3.Add(Resources.Health.have_repeat_health_check);
            headers3.Add(Resources.Health.not_repeat_health_check);
            headers3.Add(Resources.Health.lbchronic_diseases_ear);
            headers3.Add(Resources.Health.lbsmoked_cigarettes);

            setHeader(workbook, sheet3, headers3);



            ///////////////////////////////////////////////set value /////////////////////////////////////////////////////


            string seach_by3 = searchBy(functionid, departmentid, divisionid, date_start, date_end, lang);
            IRow row_seach3 = sheet3.GetRow(1);



            for (int i = 2; i <= 11; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 11)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach3.CreateCell(i);
                cell_n.CellStyle = style2;



            }

            CellRangeAddress range3 = new CellRangeAddress(1, 1, 2, 11);
            sheet3.AddMergedRegion(range3);

            ICell cell_search3 = row_seach3.GetCell(2);
            cell_search3.SetCellValue(seach_by3);




            var v3 = from oc in dbConnect.occupational_health_report_actions
                     join h in dbConnect.healths on oc.health_id equals h.id
                     join em in dbConnect.employees on h.health_employee_id equals em.employee_id
                     join o in dbConnect.occupational_health_reports on oc.occupational_health_report_id equals o.id
                     


                     where h.country == Session["country"].ToString() && oc.status == "A"
                     orderby h.report_date ascending
                     select new
                     {
                         h.employee_id,
                         report_date = h.report_date,
                         h.health_employee_id,
                         first_name = chageDataLanguage(em.first_name_th, em.first_name_en, lang),
                         last_name = chageDataLanguage(em.last_name_th, em.last_name_en, lang),
                         company_id = h.company_id,
                         function_id = h.function_id,
                         department_id = h.department_id,
                         division_id = h.division_id,
                         section_id = h.section_id,
                         health_id = h.id,
                         h.doc_no,
                         occupational_health_reports = chageDataLanguage(o.name_th, o.name_en, lang),
                         oc.repeat_health_check,

                         oc.abnormal_audiogram,
                         oc.hearing_threshold_level,
                         oc.chronic_diseases_ear,
                         oc.specify_chronic_diseases_ear,
                         oc.abnormal_pulmonary_function,
                         oc.smoked_cigarettes,
                         oc.cigarette_per_day,
                         oc.smoked_months,
                         oc.smoked_years,
                         oc.smoked_cigarettes_other

                 

                     };

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

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v3 = v3.Where(c => c.report_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v3 = v3.Where(c => c.report_date <= d_end);
            }


            if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S") > -1)
            {
                //area_all = true;
            }
            else
            {


                if (group.IndexOf("Delegate OH&S Admin") > -1 || group.IndexOf("OH&S Admin") > -1 || group.IndexOf("Area Functional Manager") > -1)
                {

                    if (group.IndexOf("Delegate OH&S Admin") > -1 || group.IndexOf("OH&S Admin") > -1)
                    {
                        v3 = v3.Where(c => functions.Contains(c.function_id));
                    }
                    else
                    {
                        v3 = v3.Where(c => department_functional.Contains(c.department_id));

                    }

                }
                else
                {

                    if (group.IndexOf("Area OH&S") > -1)//area oh&s ดึงเฉพาะฟอร์มที่ตัวเองกรอกเท่านั้น
                    {
                        v3 = v3.Where(c => c.employee_id.Contains(Session["user_id"].ToString()));

                    }



                }



            }


            int no3 = 1;
            int count3 = 3;
            foreach (var rc in v3)
            {

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



                string chronic_diseases_ear = "";
                if (rc.chronic_diseases_ear == "N")
                {
                    chronic_diseases_ear = Resources.Health.lbno;

                }
                else if (rc.chronic_diseases_ear == "Y")
                {
                    chronic_diseases_ear = Resources.Health.lbyes + " : " + rc.specify_chronic_diseases_ear;
                }




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



                IRow row = sheet3.CreateRow(count3);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(no3);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(rc.doc_no);
                cell1.CellStyle = style;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(rc.health_employee_id);
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(rc.first_name + " " + rc.last_name);
                cell3.CellStyle = style;


                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(rc.occupational_health_reports);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(abnormal_audiogram);
                cell5.CellStyle = style;


                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(abnormal_pulmonary_function);
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(rc.hearing_threshold_level);
                cell7.CellStyle = style;

                ICell cell8 = row.CreateCell(8);

                if (rc.repeat_health_check == "Y")
                {
                    cell8.SetCellValue(Resources.Incident.yes);
                }
                else
                {
                    cell8.SetCellValue("");
                }
               
                cell8.CellStyle = style;


                ICell cell9 = row.CreateCell(9);
                if (rc.repeat_health_check == "N")
                {
                    cell9.SetCellValue(Resources.Incident.yes);
                }
                else
                {
                    cell9.SetCellValue("");
                }
               
                cell9.CellStyle = style;

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(chronic_diseases_ear);
                cell10.CellStyle = style;

                ICell cell11 = row.CreateCell(11);
                cell11.SetCellValue(smoked_cigarettes);
                cell11.CellStyle = style;


                no3++;
                count3++;

            }

            //////////////////////////////////////////////end sheet3///////////////////////////////////////////////


            ISheet sheet4 = workbook.GetSheet("health_rehabilitation_action");

            ArrayList headers4 = new ArrayList();

            headers4.Add(Resources.Incident.sequence);
            headers4.Add(Resources.Health.doc_no);
            headers4.Add(Resources.Health.lbemployeeid);
            headers4.Add(Resources.Health.lbemployee_name);
            headers4.Add(Resources.Health.typecontrol);
            headers4.Add(Resources.Health.action);
            headers4.Add(Resources.Health.responsible_person);

            headers4.Add(Resources.Health.due_date);
            headers4.Add(Resources.Health.status);
            headers4.Add(Resources.Health.date_complete);
            headers4.Add(Resources.Health.remark);


            setHeader(workbook, sheet4, headers4);




            ///////////////////////////////////////////////set value /////////////////////////////////////////////////////


            string seach_by4 = searchBy(functionid, departmentid, divisionid, date_start, date_end, lang);
            IRow row_seach4 = sheet4.GetRow(1);



            for (int i = 2; i <= 10; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 10)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach4.CreateCell(i);
                cell_n.CellStyle = style2;



            }

            CellRangeAddress range4 = new CellRangeAddress(1, 1, 2, 10);
            sheet4.AddMergedRegion(range4);

            ICell cell_search4 = row_seach4.GetCell(2);
            cell_search4.SetCellValue(seach_by4);




            var v4 = from oc in dbConnect.process_action_healths
                     join h in dbConnect.healths on oc.health_id equals h.id
                     join em in dbConnect.employees on h.health_employee_id equals em.employee_id
                     join t in dbConnect.type_control_healths on oc.type_control_id equals t.id
                     join s in dbConnect.action_health_status on oc.action_status_id equals s.id



                     where h.country == Session["country"].ToString()
                     orderby h.report_date ascending
                     select new
                     {
                         h.employee_id,
                         report_date = h.report_date,
                         h.health_employee_id,
                         first_name = chageDataLanguage(em.first_name_th, em.first_name_en, lang),
                         last_name = chageDataLanguage(em.last_name_th, em.last_name_en, lang),
                         company_id = h.company_id,
                         function_id = h.function_id,
                         department_id = h.department_id,
                         division_id = h.division_id,
                         section_id = h.section_id,
                         health_id = h.id,
                         h.doc_no,
                         type_control = chageDataLanguage(t.name_th, t.name_en, lang),
                         status = chageDataLanguage(s.name_th,s.name_en,lang),
                         oc.due_date,
                         oc.date_complete,
                         oc.remark,
                         oc.action,
                         oc.responsible_person
                         


                     };

            if (companyid != "" && companyid != "null")
            {
                v4 = v4.Where(c => c.company_id == companyid);

            }


            if (functionid != "" && functionid != "null")
            {
                v4 = v4.Where(c => c.function_id == functionid);

            }

            if (departmentid != "" && departmentid != "null")
            {
                v4 = v4.Where(c => c.department_id == departmentid);

            }

            if (divisionid != "" && divisionid != "null")
            {
                v4 = v4.Where(c => c.division_id == divisionid);

            }

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v4 = v4.Where(c => c.report_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v4 = v4.Where(c => c.report_date <= d_end);
            }


            if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S") > -1)
            {
                //area_all = true;
            }
            else
            {


                if (group.IndexOf("Delegate OH&S Admin") > -1 || group.IndexOf("OH&S Admin") > -1 || group.IndexOf("Area Functional Manager") > -1)
                {

                    if (group.IndexOf("Delegate OH&S Admin") > -1 || group.IndexOf("OH&S Admin") > -1)
                    {
                        v4 = v4.Where(c => functions.Contains(c.function_id));
                    }
                    else
                    {
                        v4 = v4.Where(c => department_functional.Contains(c.department_id));

                    }

                }
                else
                {

                    if (group.IndexOf("Area OH&S") > -1)//area oh&s ดึงเฉพาะฟอร์มที่ตัวเองกรอกเท่านั้น
                    {
                        v4 = v4.Where(c => c.employee_id.Contains(Session["user_id"].ToString()));

                    }



                }



            }


            int no4 = 1;
            int count4 = 3;
            foreach (var rc in v4)
            {
                IRow row = sheet4.CreateRow(count4);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(no4);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(rc.doc_no);
                cell1.CellStyle = style;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(rc.health_employee_id);
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(rc.first_name + " " + rc.last_name);
                cell3.CellStyle = style;


                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(rc.type_control);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(rc.action);
                 cell5.CellStyle = style;


                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(rc.responsible_person);
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(Convert.ToDateTime(rc.due_date));
                IDataFormat format1 = workbook.CreateDataFormat();
                style8.DataFormat = format1.GetFormat("dd/mm/yyyy");
                cell7.CellStyle = style8;

                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(rc.status);
                cell8.CellStyle = style;

                ICell cell9 = row.CreateCell(9);

                if (rc.date_complete != null)
                {
                    cell9.SetCellValue(Convert.ToDateTime(rc.date_complete));
                    IDataFormat format2 = workbook.CreateDataFormat();
                    style8.DataFormat = format2.GetFormat("dd/mm/yyyy");
                    cell9.CellStyle = style8;
                }
                else
                {
                    cell9.SetCellValue("");
                }

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(rc.remark);
                cell10.CellStyle = style;

                no4++;
                count4++;

            }

            //////////////////////////////////////////////end sheet4///////////////////////////////////////////////




            
            setWidthColunm(workbook, sheet1, headers);
            setWidthColunm(workbook, sheet2, headers2);
            setWidthColunm(workbook, sheet3, headers3);
            setWidthColunm(workbook, sheet4, headers4);


            //string path_write = string.Format("{0}\\report\\template\\HazardReport.xlsx", Server.MapPath(@"\"));
            string path_write = string.Format("{0}" + pathreport + "HealthReport.xlsx", Server.MapPath(@"\"));

            using (var f = File.Create(path_write))
            {
                workbook.Write(f);
            }
            Response.WriteFile(path_write);

            Response.Flush();
            Response.End();
        }




        protected string getStatusStepHealth(dynamic rc, int id, string lang)
        {
            string step = "";

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                if (rc.process_status != 2)//ไม่ใช้ close กับ reject
                {

                    if (rc.step_form == 1)//area oh&s
                    {
                        string v_step = chageDataLanguage("รายงานแผนฟื้นฟูสุขภาพพนักงาน", "Health Rehabilitation report", lang);

                        step = step + "(" + v_step + " - Area OH&S)";
                     

                    }
                  
                    else if (rc.step_form == 2)
                    {
                        string v_step = chageDataLanguage("ขอปิดรายงานแผนฟื้นฟูสุขภาพพนักงาน", "Request to Close Health Rehabilitation Report", lang);
                        bool check_close = true;

                        var s = from c in dbConnect.close_step_healths
                                where c.country == Session["country"].ToString()
                                orderby c.step descending
                                select c;

                        foreach (var r in s)
                        {
                            var w = from c in dbConnect.log_request_close_healths
                                    where c.health_id == id && c.status == "A"
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


            }//end using


            return step;
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



        protected string getProcessAction(int health_id, string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.process_action_healths
                    join t in dbConnect.type_control_healths on c.type_control_id equals t.id
                    where c.health_id == health_id
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



        protected string getRiskFactor(int health_id, string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.risk_factor_relate_work_actions
                    join t in dbConnect.risk_factor_relate_works on c.risk_factor_relate_work_id equals t.id
                    where c.health_id == health_id
                    select new
                    {
                        risk_factor = chageDataLanguage(t.name_th, t.name_en, lang)

                    };
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(ex))
                {

                    ex = rc.risk_factor;
                }
                else
                {

                    ex = ex + ", " + rc.risk_factor;
                }
            }



            return ex;


        }



        protected string getOccupationalHealth(int health_id, string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.occupational_health_report_actions
                    join t in dbConnect.occupational_health_reports on c.occupational_health_report_id equals t.id
                    where c.health_id == health_id
                    select new
                    {
                        occupational = chageDataLanguage(t.name_th, t.name_en, lang)

                    };
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(ex))
                {

                    ex = rc.occupational;
                }
                else
                {

                    ex = ex + ", " + rc.occupational;
                }
            }



            return ex;


        }


        protected string getAction(int health_id, string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.process_action_healths
                    join s in dbConnect.action_health_status on c.action_status_id equals s.id
                    where c.health_id == health_id
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
                else if (rc.action_status_id == 2)//close
                {

                    status = rc.status;
                }
                else if (rc.action_status_id == 3)//cancel
                {

                    status = rc.status;
                }


                if (string.IsNullOrEmpty(ex))
                {

                    ex = rc.action + " (" + status + ")";
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
                    searchby = searchby + Resources.Health.lbfucntion + " :" + r.function_name;
                }

            }
            else
            {
                searchby = searchby + Resources.Health.lbfucntion + " :" + Resources.Main.all;

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
                    searchby = searchby + ", " + Resources.Health.lbdepartment + " :" + e.department_name;
                }
            }
            else
            {
                searchby = searchby + ", " + Resources.Health.lbdepartment + " :" + Resources.Main.all;

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
                    searchby = searchby + ", " + Resources.Health.lbdivision + " :" + i.division_name;
                }
            }
            else
            {
                searchby = searchby + ", " + Resources.Health.lbdivision + " :" + Resources.Main.all;

            }


            if (date_start != "")
            {
                searchby = searchby + ", " + Resources.Health.date + " :" + date_start;
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