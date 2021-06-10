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
    public partial class LTIFRReport : System.Web.UI.Page
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

                    LinkButton link = (LinkButton)Master.FindControl("btLTIFRReport");
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
            string filename = "LTIFRReport.xlsx";
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

            string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
            //string path = string.Format("{0}\\report\\template\\LTIFR_Report.xlsx", Server.MapPath(@"\"));
            string path = string.Format("{0}"+ pathreport +"LTIFR_Report.xlsx", Server.MapPath(@"\"));

            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("employee");

            ArrayList headers = new ArrayList();

            headers.Add(Resources.Incident.lbCompany);
            headers.Add(Resources.Incident.lbfucntion);
            headers.Add(Resources.Hazard.lbdepartment);
            headers.Add("Target");
            headers.Add("No. of LTI");
            headers.Add("Hours worked");
            headers.Add("LTIFR");
            headers.Add("Day lost");
            headers.Add("LTISR");
            

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
            string date_start = txtstart_date.Value;
            string date_end = txtend_date.Value;
            string lang = Session["lang"].ToString();




            string seach_by = searchBy(functionid, departmentid, date_start, date_end, lang);
            IRow row_seach = sheet1.GetRow(1);

            ICell cell_search = row_seach.GetCell(1);
            cell_search.SetCellValue(seach_by);

            for (int i = 1; i <= 8; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 8)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach.GetCell(i);
                cell_n.CellStyle = style2;

            }

            CellRangeAddress range = new CellRangeAddress(1, 1, 1, 8);
            sheet1.AddMergedRegion(range);



            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.companies
                     where d.country == Session["country"].ToString()
                     && (d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_start, "en").Date
                     || d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_end, "en").Date)
                    

                     orderby d.company_en ascending
                     select new
                     {
                         company_name = chageDataLanguage(d.company_th, d.company_en, lang),
                         d.company_id,
                     };

            if (companyid != "" && companyid != "null")
            {
                v1 = v1.Where(c => c.company_id == companyid);

            }

            int count = 4;

            foreach (var rc in v1)
            {
                if (count == 4)
                {

                    double targetEmployee_group = getTargetLTIFREmployeeGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIEmployee("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourEmployee("", "", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierGroup("00000000", date_start, date_end, lang);
                    int day_lost_group = getDayLost("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    double ltifr_group = 0;

                    if (workhour_group != 0)
                    {
                        ltifr_group = caclLTIFRANDTIFR(lti_group, multiplier_group, workhour_group);
                    }

                    double ltisr_group = 0;

                    if (workhour_group != 0)
                    {
                        ltisr_group = caclLTISR(day_lost_group, multiplier_group, workhour_group);
                    }


                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    IRow row_g = sheet1.CreateRow(count - 1);
                    ICell cell_g = row_g.CreateCell(0);
                    cell_g.SetCellValue(insee_group);
                    cell_g.CellStyle = style;

                    ICell cell_g1 = row_g.CreateCell(1);
                    cell_g1.SetCellValue(insee_group);
                    cell_g1.CellStyle = style;

                    ICell cell_g2 = row_g.CreateCell(2);
                    cell_g2.SetCellValue(insee_group);
                    cell_g2.CellStyle = style;

                    ICell cell_g3 = row_g.CreateCell(3);
                    cell_g3.SetCellValue(targetEmployee_group.ToString("F2"));
                    cell_g3.CellStyle = style;

                    ICell cell_g4 = row_g.CreateCell(4);
                    cell_g4.SetCellValue(lti_group);
                    cell_g4.CellStyle = style;

                    ICell cell_g5 = row_g.CreateCell(5);
                    cell_g5.SetCellValue(String.Format("{0:n}", workhour_group));
                    cell_g5.CellStyle = style;


                    ICell cell_g6 = row_g.CreateCell(6);
                    cell_g6.SetCellValue(ltifr_group.ToString("F2"));
                    cell_g6.CellStyle = style;


                    ICell cell_g7 = row_g.CreateCell(7);
                    cell_g7.SetCellValue(day_lost_group);
                    cell_g7.CellStyle = style;


                    ICell cell_g8 = row_g.CreateCell(8);
                    cell_g8.SetCellValue(ltifr_group.ToString("F2"));
                    cell_g8.CellStyle = style;


                }


                double targetEmployee = getTargetLTIFREmployee(rc.company_id, "","", date_start, date_end, lang);
                int lti = getLTIEmployee(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployee(rc.company_id, "", "", date_start, date_end, lang);
                double multiplier = getMultiplier(rc.company_id, "","", date_start, date_end, lang);
                int day_lost = getDayLost(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;

                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }

                double ltisr = 0;

                if (workhour != 0)
                {
                    ltisr = caclLTISR(day_lost, multiplier, workhour);
                }


                IRow row = sheet1.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(rc.company_name);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("-");
                cell1.CellStyle = style;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("-");
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(targetEmployee.ToString("F2"));
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(lti);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(String.Format("{0:n}", workhour));
                cell5.CellStyle = style;


                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(ltifr.ToString("F2"));
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(day_lost);
                cell7.CellStyle = style;

                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(ltisr.ToString("F2"));
                cell8.CellStyle = style;


                count++;

                //////////////////////////////////////////////function//////////////////////////////////////////////////////////

                var v2 = from d in dbConnect.functions
                         where d.company_id == rc.company_id
                         && d.country == Session["country"].ToString()
                         && (d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_start, "en").Date
                         || d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_end, "en").Date)

                         orderby d.function_en ascending
                         select new
                         {
                             function_name = chageDataLanguage(d.function_th, d.function_en, lang),
                             d.function_id,
                         };



                if (functionid != "" && functionid != "null")
                {
                    v2 = v2.Where(c => c.function_id == functionid);

                }

                foreach (var rc1 in v2)
                {
                   
                    double targetEmployee_f = getTargetLTIFREmployee(rc.company_id, rc1.function_id,"", date_start, date_end, lang);
                    int lti_f = getLTIEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_f = getWorkhourEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double multiplier_f = getMultiplier(rc.company_id, rc1.function_id,"", date_start, date_end, lang);
                    int day_lost_f = getDayLost(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    double ltifr_f = 0;

                    if (workhour_f != 0)
                    {
                        ltifr_f = caclLTIFRANDTIFR(lti_f, multiplier_f, workhour_f);
                    }

                    double ltisr_f = 0;
                    if (workhour_f != 0)
                    {
                        ltisr_f = caclLTISR(day_lost_f, multiplier_f, workhour_f);
                    }

                    IRow row_f = sheet1.CreateRow(count);
                    ICell cell1_f = row_f.CreateCell(1);
                    cell1_f.SetCellValue(rc1.function_name);
                    cell1_f.CellStyle = style;

                    ICell cell2_f = row_f.CreateCell(2);
                    cell2_f.SetCellValue("-");
                    cell2_f.CellStyle = style;

                    ICell cell3_f = row_f.CreateCell(3);
                    cell3_f.SetCellValue(targetEmployee_f.ToString("F2"));
                    cell3_f.CellStyle = style;

                    ICell cell4_f = row_f.CreateCell(4);
                    cell4_f.SetCellValue(lti_f);
                    cell4_f.CellStyle = style;

                    ICell cell5_f = row_f.CreateCell(5);
                    cell5_f.SetCellValue(String.Format("{0:n}", workhour_f));
                    cell5_f.CellStyle = style;


                    ICell cell6_f = row_f.CreateCell(6);
                    cell6_f.SetCellValue(ltifr_f.ToString("F2"));
                    cell6_f.CellStyle = style;

                    ICell cell7_f = row_f.CreateCell(7);
                    cell7_f.SetCellValue(day_lost_f);
                    cell7_f.CellStyle = style;

                    ICell cell8_f = row_f.CreateCell(8);
                    cell8_f.SetCellValue(ltisr_f.ToString("F2"));
                    cell8_f.CellStyle = style;


                    count++;


                    //////////////////////////////////////////////////department////////////////////////////////////////////////
               
                    var v3 = from d in dbConnect.departments
                             where d.function_id == rc1.function_id
                             && d.country == Session["country"].ToString()
                             && (d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_start, "en").Date
                             || d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_end, "en").Date)

                             orderby d.department_en ascending
                             select new
                             {
                                 department_name = chageDataLanguage(d.department_th, d.department_en, lang),
                                 d.department_id,
                             };



                    if (departmentid != "" && departmentid != "null")
                    {
                        v3 = v3.Where(c => c.department_id == departmentid);

                    }


                    foreach (var rc2 in v3)
                    {
                        if (rc2.department_name != "-")
                        {
                            double targetEmployee_d = getTargetLTIFREmployee("", rc1.function_id,rc2.department_id, date_start, date_end, lang);
                            int lti_d = getLTIEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            double workhour_d = getWorkhourEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double multiplier_d = getMultiplier("", rc1.function_id,rc2.department_id, date_start, date_end, lang);
                            int day_lost_d = getDayLost("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            double ltifr_d = 0;

                            if (workhour_d != 0)
                            {
                                ltifr_d = caclLTIFRANDTIFR(lti_d, multiplier_d, workhour_d);
                            }

                            double ltisr_d = 0;
                            if (workhour_d != 0)
                            {
                                ltisr_d = caclLTISR(day_lost_d, multiplier_d, workhour_d);

                            }

                            IRow row_d = sheet1.CreateRow(count);
                            ICell cell2_d = row_d.CreateCell(2);
                            cell2_d.SetCellValue(rc2.department_name);
                            cell2_d.CellStyle = style;

                            ICell cell3_d = row_d.CreateCell(3);
                            cell3_d.SetCellValue(targetEmployee_d.ToString("F2"));
                            cell3_d.CellStyle = style;

                            ICell cell4_d = row_d.CreateCell(4);
                            cell4_d.SetCellValue(lti_d);
                            cell4_d.CellStyle = style;

                            ICell cell5_d = row_d.CreateCell(5);
                            cell5_d.SetCellValue(String.Format("{0:n}", workhour_d));
                            cell5_d.CellStyle = style;


                            ICell cell6_d = row_d.CreateCell(6);
                            cell6_d.SetCellValue(ltifr_d.ToString("F2"));
                            cell6_d.CellStyle = style;

                            ICell cell7_d = row_d.CreateCell(7);
                            cell7_d.SetCellValue(day_lost_d);
                            cell7_d.CellStyle = style;

                            ICell cell8_d = row_d.CreateCell(8);
                            cell8_d.SetCellValue(ltisr_d.ToString("F2"));
                            cell8_d.CellStyle = style;



                            count++;
                        }
                    }



                }

              

            }//end foreach

        


            ////////////////////////////////////////end sheet employee////////////////////////////////////////////////
            ArrayList headers2 = new ArrayList();

            headers2.Add(Resources.Incident.lbCompany);
            headers2.Add(Resources.Incident.lbfucntion);
            headers2.Add(Resources.Hazard.lbdepartment);
            headers2.Add("Target");
            headers2.Add("No. of LTI");
            headers2.Add("Hours worked");
            headers2.Add("Day lost");
            headers2.Add("LTIFR");

            ISheet sheet2 = workbook.GetSheet("contractor onsite");
            setHeader(workbook, sheet2, headers2);

            string seach_by2 = searchBy(functionid, departmentid, date_start, date_end, lang);
            IRow row_seach2 = sheet2.GetRow(1);

            ICell cell_search2 = row_seach2.GetCell(1);
            cell_search2.SetCellValue(seach_by2);

            for (int i = 1; i <= 7; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 7)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach2.GetCell(i);
                cell_n.CellStyle = style2;

            }

            CellRangeAddress range2 = new CellRangeAddress(1, 1, 1, 7);
            sheet2.AddMergedRegion(range2);

            count = 4;
            foreach (var rc in v1)
            {
                if (count == 4)
                {

                    double targetContractorOnsite_group = getTargetLTIFRContractorOnsiteGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOnsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourContractorOnsite("", "", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorGroup("00000000", date_start, date_end, lang);
                    int day_lost_group = getDayLostContractorOnsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                
                    double ltifr_group = 0;

                    if (workhour_group != 0)
                    {
                        ltifr_group = caclLTIFRANDTIFR(lti_group, multiplier_group, workhour_group);
                    }

                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);



                    IRow row_g = sheet2.CreateRow(count - 1);
                    ICell cell_g = row_g.CreateCell(0);
                    cell_g.SetCellValue(insee_group);
                    cell_g.CellStyle = style;

                    ICell cell_g1 = row_g.CreateCell(1);
                    cell_g1.SetCellValue(insee_group);
                    cell_g1.CellStyle = style;

                    ICell cell_g2 = row_g.CreateCell(2);
                    cell_g2.SetCellValue(insee_group);
                    cell_g2.CellStyle = style;

                    ICell cell_g3 = row_g.CreateCell(3);
                    cell_g3.SetCellValue(targetContractorOnsite_group.ToString("F2"));
                    cell_g3.CellStyle = style;

                    ICell cell_g4 = row_g.CreateCell(4);
                    cell_g4.SetCellValue(lti_group);
                    cell_g4.CellStyle = style;

                    ICell cell_g5 = row_g.CreateCell(5);
                    cell_g5.SetCellValue(String.Format("{0:n}", workhour_group));
                    cell_g5.CellStyle = style;


                    ICell cell_g6 = row_g.CreateCell(6);
                    cell_g6.SetCellValue(day_lost_group);
                    cell_g6.CellStyle = style;


                    ICell cell_g7 = row_g.CreateCell(7);
                    cell_g7.SetCellValue(ltifr_group.ToString("F2"));
                    cell_g7.CellStyle = style;

            

                }


                double targetContractorOnsite = getTargetLTIFRContractorOnsite(rc.company_id, "","", date_start, date_end, lang);
                int lti = getLTIContractorOnsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourContractorOnsite(rc.company_id, "", "", date_start, date_end, lang);
                double multiplier = getMultiplierContractor(rc.company_id, "","", date_start, date_end, lang);
                int day_lost = getDayLostContractorOnsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;


                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }


                IRow row = sheet2.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(rc.company_name);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("-");
                cell1.CellStyle = style;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("-");
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(targetContractorOnsite.ToString("F2"));
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(lti);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(String.Format("{0:n}", workhour));
                cell5.CellStyle = style;


                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(day_lost);
                cell6.CellStyle = style;


                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(ltifr.ToString("F2"));
                cell7.CellStyle = style;


                count++;




                //////////////////////////////////////////////function//////////////////////////////////////////////////////////

                var v2 = from d in dbConnect.functions
                         where d.company_id == rc.company_id
                         && d.country == Session["country"].ToString()
                         && (d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_start, "en").Date
                         || d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_end, "en").Date)

                         orderby d.function_en ascending
                         select new
                         {
                             function_name = chageDataLanguage(d.function_th, d.function_en, lang),
                             d.function_id,
                         };



                if (functionid != "" && functionid != "null")
                {
                    v2 = v2.Where(c => c.function_id == functionid);

                }

                foreach (var rc1 in v2)
                {

                    double targetContractorOnsite_f = getTargetLTIFRContractorOnsite(rc.company_id, rc1.function_id,"", date_start, date_end, lang);
                    int lti_f = getLTIContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_f = getWorkhourContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double multiplier_f = getMultiplierContractor(rc.company_id, rc1.function_id,"", date_start, date_end, lang);
                    int day_lost_f = getDayLostContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    double ltifr_f = 0;

                    if (workhour_f != 0)
                    {
                        ltifr_f = caclLTIFRANDTIFR(lti_f, multiplier_f, workhour_f);
                    }



                    IRow row_f = sheet2.CreateRow(count);
                    ICell cell1_f = row_f.CreateCell(1);
                    cell1_f.SetCellValue(rc1.function_name);
                    cell1_f.CellStyle = style;

                    ICell cell2_f = row_f.CreateCell(2);
                    cell2_f.SetCellValue("-");
                    cell2_f.CellStyle = style;

                    ICell cell3_f = row_f.CreateCell(3);
                    cell3_f.SetCellValue(targetContractorOnsite_f.ToString("F2"));
                    cell3_f.CellStyle = style;

                    ICell cell4_f = row_f.CreateCell(4);
                    cell4_f.SetCellValue(lti_f);
                    cell4_f.CellStyle = style;

                    ICell cell5_f = row_f.CreateCell(5);
                    cell5_f.SetCellValue(String.Format("{0:n}", workhour_f));
                    cell5_f.CellStyle = style;


                    ICell cell6_f = row_f.CreateCell(6);
                    cell6_f.SetCellValue(day_lost_f);
                    cell6_f.CellStyle = style;

                    ICell cell7_f = row_f.CreateCell(7);
                    cell7_f.SetCellValue(ltifr_f.ToString("F2"));
                    cell7_f.CellStyle = style;


                    count++;


                    //////////////////////////////////////////////////department////////////////////////////////////////////////

                    var v3 = from d in dbConnect.departments
                             where d.function_id == rc1.function_id
                             && d.country == Session["country"].ToString()
                             && (d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_start, "en").Date
                            || d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_end, "en").Date)

                             orderby d.department_en ascending
                             select new
                             {
                                 department_name = chageDataLanguage(d.department_th, d.department_en, lang),
                                 d.department_id,
                             };



                    if (departmentid != "" && departmentid != "null")
                    {
                        v3 = v3.Where(c => c.department_id == departmentid);

                    }


                    foreach (var rc2 in v3)
                    {


                        if (rc2.department_name != "-")
                        {
                            double targetContractorOnsite_d = getTargetLTIFRContractorOnsite("", rc1.function_id,rc2.department_id, date_start, date_end, lang);
                            int lti_d = getLTIContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            double workhour_d = getWorkhourContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double multiplier_d = getMultiplierContractor("", rc1.function_id,rc2.department_id, date_start, date_end, lang);
                            int day_lost_d = getDayLostContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            double ltifr_d = 0;

                            if (workhour_d != 0)
                            {
                                ltifr_d = caclLTIFRANDTIFR(lti_d, multiplier_d, workhour_d);
                            }


                            IRow row_d = sheet2.CreateRow(count);
                            ICell cell2_d = row_d.CreateCell(2);
                            cell2_d.SetCellValue(rc2.department_name);
                            cell2_d.CellStyle = style;

                            ICell cell3_d = row_d.CreateCell(3);
                            cell3_d.SetCellValue(targetContractorOnsite_d.ToString("F2"));
                            cell3_d.CellStyle = style;

                            ICell cell4_d = row_d.CreateCell(4);
                            cell4_d.SetCellValue(lti_d);
                            cell4_d.CellStyle = style;

                            ICell cell5_d = row_d.CreateCell(5);
                            cell5_d.SetCellValue(String.Format("{0:n}", workhour_d));
                            cell5_d.CellStyle = style;


                            ICell cell6_d = row_d.CreateCell(6);
                            cell6_d.SetCellValue(day_lost_d);
                            cell6_d.CellStyle = style;

                            ICell cell7_d = row_d.CreateCell(7);
                            cell7_d.SetCellValue(ltifr_d.ToString("F2"));
                            cell7_d.CellStyle = style;



                            count++;
                        }
                    }



                }



            }//end foreach




            ////////////////////////////////////////end sheet contractor onsite////////////////////////////////////////////////


            ArrayList headers3 = new ArrayList();

            headers3.Add(Resources.Incident.lbCompany);
            headers3.Add(Resources.Incident.lbfucntion);
            headers3.Add(Resources.Hazard.lbdepartment);
            headers3.Add("Target");
            headers3.Add("No. of LTI");
            headers3.Add("Hours worked");
            headers3.Add("Day lost");
            headers3.Add("LTIFR");

            ISheet sheet3 = workbook.GetSheet("contractor offsite");

            setHeader(workbook, sheet3, headers3);

            string seach_by3 = searchBy(functionid, departmentid, date_start, date_end, lang);
            IRow row_seach3 = sheet3.GetRow(1);

            ICell cell_search3 = row_seach3.GetCell(1);
            cell_search3.SetCellValue(seach_by3);

            for (int i =1; i <= 7; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 7)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach3.GetCell(i);
                cell_n.CellStyle = style2;

            }

            CellRangeAddress range3 = new CellRangeAddress(1, 1, 1, 7);
            sheet3.AddMergedRegion(range3);

            count = 4;
            foreach (var rc in v1)
            {

                if (count == 4)
                {

                    double targetContractorOffsite_group = getTargetLTIFRContractorOffsiteGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourContractorOffsite("", "", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorOffsiteGroup("00000000", date_start, date_end, lang);
                    int day_lost_group = getDayLostContractorOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                   
                    double ltifr_group = 0;

                    if (workhour_group != 0)
                    {
                        ltifr_group = caclLTIFRANDTIFR(lti_group, multiplier_group, workhour_group);
                    }


                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);


                    IRow row_g = sheet3.CreateRow(count - 1);
                    ICell cell_g = row_g.CreateCell(0);
                    cell_g.SetCellValue(insee_group);
                    cell_g.CellStyle = style;

                    ICell cell_g1 = row_g.CreateCell(1);
                    cell_g1.SetCellValue(insee_group);
                    cell_g1.CellStyle = style;

                    ICell cell_g2 = row_g.CreateCell(2);
                    cell_g2.SetCellValue(insee_group);
                    cell_g2.CellStyle = style;

                    ICell cell_g3 = row_g.CreateCell(3);
                    cell_g3.SetCellValue(targetContractorOffsite_group.ToString("F2"));
                    cell_g3.CellStyle = style;

                    ICell cell_g4 = row_g.CreateCell(4);
                    cell_g4.SetCellValue(lti_group);
                    cell_g4.CellStyle = style;

                    ICell cell_g5 = row_g.CreateCell(5);
                    cell_g5.SetCellValue(String.Format("{0:n}", workhour_group));
                    cell_g5.CellStyle = style;

                    ICell cell_g6 = row_g.CreateCell(6);
                    cell_g6.SetCellValue(day_lost_group);
                    cell_g6.CellStyle = style;

                    ICell cell_g7 = row_g.CreateCell(7);
                    cell_g7.SetCellValue(ltifr_group.ToString("F2"));
                    cell_g7.CellStyle = style;

                }

                double targetContractorOffsite = getTargetLTIFRContractorOffsite(rc.company_id, "","", date_start, date_end, lang);
                int lti = getLTIContractorOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourContractorOffsite(rc.company_id, "", "", date_start, date_end, lang);
                double multiplier = getMultiplierContractorOffsite(rc.company_id, "","", date_start, date_end, lang);
                int day_lost = getDayLostContractorOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;
                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }


                IRow row = sheet3.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(rc.company_name);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("-");
                cell1.CellStyle = style;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("-");
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(targetContractorOffsite.ToString("F2"));
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(lti);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(String.Format("{0:n}", workhour));
                cell5.CellStyle = style;

                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(day_lost);
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(ltifr.ToString("F2"));
                cell7.CellStyle = style;

                count++;



                //////////////////////////////////////////////function//////////////////////////////////////////////////////////

                var v2 = from d in dbConnect.functions
                         where d.company_id == rc.company_id
                         && d.country == Session["country"].ToString()
                         && (d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_start, "en").Date
                         || d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_end, "en").Date)

                         orderby d.function_en ascending
                         select new
                         {
                             function_name = chageDataLanguage(d.function_th, d.function_en, lang),
                             d.function_id,
                         };



                if (functionid != "" && functionid != "null")
                {
                    v2 = v2.Where(c => c.function_id == functionid);

                }

                foreach (var rc1 in v2)
                {

                    double targetContractorOffsite_f = getTargetLTIFRContractorOffsite(rc.company_id, rc1.function_id,"", date_start, date_end, lang);
                    int lti_f = getLTIContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_f = getWorkhourContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double multiplier_f = getMultiplierContractorOffsite(rc.company_id, rc1.function_id,"", date_start, date_end, lang);
                    int day_lost_f = getDayLostContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    double ltifr_f = 0;

                    if (workhour_f != 0)
                    {
                        ltifr_f = caclLTIFRANDTIFR(lti_f, multiplier_f, workhour_f);
                    }



                    IRow row_f = sheet3.CreateRow(count);
                    ICell cell1_f = row_f.CreateCell(1);
                    cell1_f.SetCellValue(rc1.function_name);
                    cell1_f.CellStyle = style;

                    ICell cell2_f = row_f.CreateCell(2);
                    cell2_f.SetCellValue("-");
                    cell2_f.CellStyle = style;

                    ICell cell3_f = row_f.CreateCell(3);
                    cell3_f.SetCellValue(targetContractorOffsite_f.ToString("F2"));
                    cell3_f.CellStyle = style;

                    ICell cell4_f = row_f.CreateCell(4);
                    cell4_f.SetCellValue(lti_f);
                    cell4_f.CellStyle = style;

                    ICell cell5_f = row_f.CreateCell(5);
                    cell5_f.SetCellValue(String.Format("{0:n}", workhour_f));
                    cell5_f.CellStyle = style;

                    ICell cell6_f = row_f.CreateCell(6);
                    cell6_f.SetCellValue(day_lost_f);
                    cell6_f.CellStyle = style;

                    ICell cell7_f = row_f.CreateCell(7);
                    cell7_f.SetCellValue(ltifr_f.ToString("F2"));
                    cell7_f.CellStyle = style;



                    count++;


                    //////////////////////////////////////////////////department////////////////////////////////////////////////

                    var v3 = from d in dbConnect.departments
                             where d.function_id == rc1.function_id
                             && d.country == Session["country"].ToString()
                             && (d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_start, "en").Date
                             || d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_end, "en").Date)
                             orderby d.department_en ascending
                             select new
                             {
                                 department_name = chageDataLanguage(d.department_th, d.department_en, lang),
                                 d.department_id,
                             };



                    if (departmentid != "" && departmentid != "null")
                    {
                        v3 = v3.Where(c => c.department_id == departmentid);

                    }


                    foreach (var rc2 in v3)
                    {


                        if (rc2.department_name != "-")
                        {
                            double targetContractorOffsite_d = getTargetLTIFRContractorOffsite("", rc1.function_id,rc2.department_id, date_start, date_end, lang);
                            int lti_d = getLTIContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            double workhour_d = getWorkhourContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double multiplier_d = getMultiplierContractorOffsite("", rc1.function_id,rc2.department_id, date_start, date_end, lang);
                            int day_lost_d = getDayLostContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            double ltifr_d = 0;

                            if (workhour_d != 0)
                            {
                                ltifr_d = caclLTIFRANDTIFR(lti_d, multiplier_d, workhour_d);
                            }


                            IRow row_d = sheet3.CreateRow(count);
                            ICell cell2_d = row_d.CreateCell(2);
                            cell2_d.SetCellValue(rc2.department_name);
                            cell2_d.CellStyle = style;

                            ICell cell3_d = row_d.CreateCell(3);
                            cell3_d.SetCellValue(targetContractorOffsite_d.ToString("F2"));
                            cell3_d.CellStyle = style;

                            ICell cell4_d = row_d.CreateCell(4);
                            cell4_d.SetCellValue(lti_d);
                            cell4_d.CellStyle = style;

                            ICell cell5_d = row_d.CreateCell(5);
                            cell5_d.SetCellValue(String.Format("{0:n}", workhour_d));
                            cell5_d.CellStyle = style;

                            ICell cell6_d = row_d.CreateCell(6);
                            cell6_d.SetCellValue(day_lost_d);
                            cell6_d.CellStyle = style;

                            ICell cell7_d = row_d.CreateCell(7);
                            cell7_d.SetCellValue(ltifr_d.ToString("F2"));
                            cell7_d.CellStyle = style;


                            count++;
                        }
                       
                    }



                }



            }//end foreach




            ////////////////////////////////////////end sheet contractor offsite////////////////////////////////////////////////


            ArrayList headers4 = new ArrayList();

            headers4.Add(Resources.Incident.lbCompany);
            headers4.Add(Resources.Incident.lbfucntion);
            headers4.Add(Resources.Hazard.lbdepartment);
            headers4.Add("Target");
            headers4.Add("No. of LTI");
            headers4.Add("Hours worked");
            headers4.Add("Day lost");
            headers4.Add("LTIFR");

            ISheet sheet4 = workbook.GetSheet("all");
            setHeader(workbook, sheet4, headers4);

            string seach_by4 = searchBy(functionid, departmentid, date_start, date_end, lang);
            IRow row_seach4 = sheet4.GetRow(1);

           
            ICellStyle style4 = workbook.CreateCellStyle();
            style4.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style4.TopBorderColor = IndexedColors.Black.Index;

            for (int i = 1; i <= 7; i++)
            {
              
                if (i == 7)
                {
                    style4.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style4.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach4.CreateCell(i);
                cell_n.CellStyle = style4;

            }

            ICell cell_search4 = row_seach4.GetCell(1);
            cell_search4.SetCellValue(seach_by4);


            CellRangeAddress range4 = new CellRangeAddress(1, 1, 1, 7);
            sheet4.AddMergedRegion(range4);

            count = 4;
            foreach (var rc in v1)
            {
                if (count == 4)
                {
                    
                    double targetEmployee_group = getTargetLTIFREmployeeContractorOnsiteOffsiteGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIEmployeeContractorOnsiteOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourEmployeeContractorOnsiteOffsite("", "", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierEmployeeContractorOnsiteOffsiteGroup("00000000", date_start, date_end, lang);
                    int day_lost_group = getDayLostEmployeeContractorOnsiteOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    double ltifr_group = 0;

                    if (workhour_group != 0)
                    {
                        ltifr_group = caclLTIFRANDTIFR(lti_group, multiplier_group, workhour_group);
                    }

                   


                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    IRow row_g = sheet4.CreateRow(count - 1);
                    ICell cell_g = row_g.CreateCell(0);
                    cell_g.SetCellValue(insee_group);
                    cell_g.CellStyle = style;

                    ICell cell_g1 = row_g.CreateCell(1);
                    cell_g1.SetCellValue(insee_group);
                    cell_g1.CellStyle = style;

                    ICell cell_g2 = row_g.CreateCell(2);
                    cell_g2.SetCellValue(insee_group);
                    cell_g2.CellStyle = style;

                    ICell cell_g3 = row_g.CreateCell(3);
                    cell_g3.SetCellValue(targetEmployee_group.ToString("F2"));
                    cell_g3.CellStyle = style;

                    ICell cell_g4 = row_g.CreateCell(4);
                    cell_g4.SetCellValue(lti_group);
                    cell_g4.CellStyle = style;

                    ICell cell_g5 = row_g.CreateCell(5);
                    cell_g5.SetCellValue(String.Format("{0:n}", workhour_group));
                    cell_g5.CellStyle = style;


                    ICell cell_g6 = row_g.CreateCell(6);
                    cell_g6.SetCellValue(day_lost_group);
                    cell_g6.CellStyle = style;

                    ICell cell_g7 = row_g.CreateCell(7);
                    cell_g7.SetCellValue(ltifr_group.ToString("F2"));
                    cell_g7.CellStyle = style;



                }


                double targetEmployee = getTargetLTIFREmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang);
                int lti = getLTIEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang);
                double multiplier = getMultiplierEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang);
                int day_lost = getDayLostEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;

                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }

        


                IRow row = sheet4.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(rc.company_name);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("-");
                cell1.CellStyle = style;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("-");
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(targetEmployee.ToString("F2"));
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(lti);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(String.Format("{0:n}", workhour));
                cell5.CellStyle = style;

                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(day_lost);
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(ltifr.ToString("F2"));
                cell7.CellStyle = style;


                count++;


                //////////////////////////////////////////////function//////////////////////////////////////////////////////////

                var v2 = from d in dbConnect.functions
                         where d.company_id == rc.company_id
                         && d.country == Session["country"].ToString()
                         && (d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_start, "en").Date
                         || d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_end, "en").Date)

                         orderby d.function_en ascending
                         select new
                         {
                             function_name = chageDataLanguage(d.function_th, d.function_en, lang),
                             d.function_id,
                         };



                if (functionid != "" && functionid != "null")
                {
                    v2 = v2.Where(c => c.function_id == functionid);

                }

                foreach (var rc1 in v2)
                {

                    double targetEmployee_f = getTargetLTIFREmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    int lti_f = getLTIEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_f = getWorkhourEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double multiplier_f = getMultiplierEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    int day_lost_f = getDayLostEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    double ltifr_f = 0;

                    if (workhour_f != 0)
                    {
                        ltifr_f = caclLTIFRANDTIFR(lti_f, multiplier_f, workhour_f);
                    }

                   

                    IRow row_f = sheet4.CreateRow(count);
                    ICell cell1_f = row_f.CreateCell(1);
                    cell1_f.SetCellValue(rc1.function_name);
                    cell1_f.CellStyle = style;

                    ICell cell2_f = row_f.CreateCell(2);
                    cell2_f.SetCellValue("-");
                    cell2_f.CellStyle = style;

                    ICell cell3_f = row_f.CreateCell(3);
                    cell3_f.SetCellValue(targetEmployee_f.ToString("F2"));
                    cell3_f.CellStyle = style;

                    ICell cell4_f = row_f.CreateCell(4);
                    cell4_f.SetCellValue(lti_f);
                    cell4_f.CellStyle = style;

                    ICell cell5_f = row_f.CreateCell(5);
                    cell5_f.SetCellValue(String.Format("{0:n}", workhour_f));
                    cell5_f.CellStyle = style;


                    ICell cell6_f = row_f.CreateCell(6);
                    cell6_f.SetCellValue(day_lost_f);
                    cell6_f.CellStyle = style;


                    ICell cell7_f = row_f.CreateCell(7);
                    cell7_f.SetCellValue(ltifr_f.ToString("F2"));
                    cell7_f.CellStyle = style;


                

                    count++;


                    //////////////////////////////////////////////////department////////////////////////////////////////////////

                    var v3 = from d in dbConnect.departments
                             where d.function_id == rc1.function_id
                             && d.country == Session["country"].ToString()
                             && (d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_start, "en").Date
                             || d.valid_to.Value.Date >= FormatDates.changeDateTimeDB(date_end, "en").Date)

                             orderby d.department_en ascending
                             select new
                             {
                                 department_name = chageDataLanguage(d.department_th, d.department_en, lang),
                                 d.department_id,
                             };



                    if (departmentid != "" && departmentid != "null")
                    {
                        v3 = v3.Where(c => c.department_id == departmentid);

                    }


                    foreach (var rc2 in v3)
                    {
                        if (rc2.department_name != "-")
                        {
                            double targetEmployee_d = getTargetLTIFREmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            int lti_d = getLTIEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            double workhour_d = getWorkhourEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double multiplier_d = getMultiplierEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            int day_lost_d = getDayLostEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            double ltifr_d = 0;

                            if (workhour_d != 0)
                            {
                                ltifr_d = caclLTIFRANDTIFR(lti_d, multiplier_d, workhour_d);
                            }


                            IRow row_d = sheet4.CreateRow(count);
                            ICell cell2_d = row_d.CreateCell(2);
                            cell2_d.SetCellValue(rc2.department_name);
                            cell2_d.CellStyle = style;

                            ICell cell3_d = row_d.CreateCell(3);
                            cell3_d.SetCellValue(targetEmployee_d.ToString("F2"));
                            cell3_d.CellStyle = style;

                            ICell cell4_d = row_d.CreateCell(4);
                            cell4_d.SetCellValue(lti_d);
                            cell4_d.CellStyle = style;

                            ICell cell5_d = row_d.CreateCell(5);
                            cell5_d.SetCellValue(String.Format("{0:n}", workhour_d));
                            cell5_d.CellStyle = style;


                            ICell cell6_d = row_d.CreateCell(6);
                            cell6_d.SetCellValue(day_lost_d);
                            cell6_d.CellStyle = style;

                            ICell cell7_d = row_d.CreateCell(7);
                            cell7_d.SetCellValue(ltifr_d.ToString("F2"));
                            cell7_d.CellStyle = style;


           
                            count++;
                        }
                    }



                }




            }//end foreach




            ////////////////////////////////////////end sheet contractor onsite////////////////////////////////////////////////








            setWidthColunm(workbook, sheet1, headers);
            setWidthColunm(workbook, sheet2, headers);
            setWidthColunm(workbook, sheet3, headers);
            setWidthColunm(workbook, sheet4, headers);

           // string path_write = string.Format("{0}\\report\\template\\LTIFRReport.xlsx", Server.MapPath(@"\"));
            string path_write = string.Format("{0}"+ pathreport +"LTIFRReport.xlsx", Server.MapPath(@"\"));

            using (var f = File.Create(path_write))
            {
                workbook.Write(f);
            }
            Response.WriteFile(path_write);

            Response.Flush();
            Response.End();


        }


        protected void setWidthColunm(XSSFWorkbook workbook, ISheet sheet, ArrayList headers)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                sheet.AutoSizeColumn(i);

            }

        }


        protected string searchBy(string function_id, string department_id,  string date_start, string date_end, string lang)
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
                    searchby = searchby + Resources.Incident.lbfucntion + " :" + r.function_name;
                }

            }
            else
            {
                searchby = searchby + Resources.Incident.lbfucntion + " :" + Resources.Main.all;

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
                    searchby = searchby + ", " + Resources.Incident.lbdepartment + " :" + e.department_name;
                }
            }
            else
            {
                searchby = searchby + ", " + Resources.Incident.lbdepartment + " :" + Resources.Main.all;

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

            style.FillForegroundColor = IndexedColors.Grey40Percent.Index;
            style.FillPattern = FillPattern.SolidForeground;


            IRow row = sheet.CreateRow(2);

            for (int i = 0; i < headers.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(headers[i].ToString());
                cell.CellStyle = style;

            }


        }



        public double caclLTIFRANDTIFR(int lti, double multiplier, double workhour)
        {

            double value = Math.Round((lti * multiplier) / workhour, 2);

            return value;
        }


        public double getMultiplier(string company_id, string function_id,string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.multiplier,
                            c.created,
                            c.department_id

                        };

                if (department_id != "" && department_id != "null")
                {
                    v = v.Where(c => c.department_id == department_id);

                }

               


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }


                foreach (var rc in v)
                {
                    value = Convert.ToDouble(rc.multiplier);
                }



            }
            else
            {
                var v = from c in dbConnect.target_mains
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        select new
                        {
                            c.multiplier,
                            c.created,
                            c.function_id,
                            fu.company_id

                        };

                if (company_id != "" && company_id != "null")
                {
                    v = v.Where(c => c.company_id == company_id);

                }

                if (function_id != "" && function_id != "null")
                {
                    v = v.Where(c => c.function_id == function_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }




                foreach (var rc in v)
                {
                    value = Convert.ToDouble(rc.multiplier);
                }



            }
      
            return value;
        }


        public double getMultiplierGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    select new
                    {
                        c.multiplier,
                        c.created,
                        c.function_id

                    };



            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }




            foreach (var rc in v)
            {
                if (rc.multiplier > value)
                {
                    value = Convert.ToDouble(rc.multiplier);
                }
            }

            return value;
        }


        public double getTargetLTIFREmployee(string company_id, string function_id,string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.ltifr_employee,
                            c.created,
                            c.department_id

                        };

                if (department_id != "" && department_id != "null")
                {
                    v = v.Where(c => c.department_id == department_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }

                //if (date_end != "")
                //{
                //    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //    v = v.Where(c => c.created <= d_end);
                //}


                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_employee);
                }


            }
            else
            {
                var v = from c in dbConnect.target_mains
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        select new
                        {
                            c.ltifr_employee,
                            c.created,
                            c.function_id,
                            fu.company_id

                        };

                if (company_id != "" && company_id != "null")
                {
                    v = v.Where(c => c.company_id == company_id);

                }


                if (function_id != "" && function_id != "null")
                {
                    v = v.Where(c => c.function_id == function_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }

                //if (date_end != "")
                //{
                //    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //    v = v.Where(c => c.created <= d_end);
                //}




                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_employee);
                }

                if (function_id == "")
                {
                    var a = from c in dbConnect.company_ignore_targets//target ของ sccc จะไม่นับรวมจาก function
                            where c.company_id.Contains(company_id)
                            select c;

                    if (a.Count() > 0)
                    {
                        value = 0;
                    }

                }


            }
           
            return value;
        }



        public double getTargetLTIFREmployeeGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    select new
                    {
                        c.ltifr_employee,
                        c.created,
                        c.function_id,

                    };


            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }

            //if (date_end != "")
            //{
            //    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
            //    v = v.Where(c => c.created <= d_end);
            //}




            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.ltifr_employee);
            }

            return value;
        }





        public double getTargetLTIFRContractorOnsite(string company_id, string function_id,string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.ltifr_contractor_onsite,
                            c.created,
                            c.department_id
                        };

                if (department_id != "" && department_id != "null")
                {
                    v = v.Where(c => c.department_id == department_id);

                }

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_contractor_onsite);
                }


            }
            else
            {
                var v = from c in dbConnect.target_mains
                        join f in dbConnect.functions on c.function_id equals f.function_id
                        select new
                        {
                            c.ltifr_contractor_onsite,
                            c.created,
                            c.function_id,
                            f.company_id
                        };

                if (company_id != "" && company_id != "null")
                {
                    v = v.Where(c => c.company_id == company_id);

                }

                if (function_id != "" && function_id != "null")
                {
                    v = v.Where(c => c.function_id == function_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_contractor_onsite);
                }

                if (function_id == "")
                {
                    var a = from c in dbConnect.company_ignore_targets//target ของ sccc จะไม่นับรวมจาก function
                            where c.company_id.Contains(company_id)
                            select c;

                    if (a.Count() > 0)
                    {
                        value = 0;
                    }

                }

            }
           

            return value;
        }


        public double getTargetLTIFRContractorOnsiteGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    select new
                    {
                        c.ltifr_contractor_onsite,
                        c.created,
                        c.function_id,
                    };


            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.ltifr_contractor_onsite);
            }

            return value;
        }





        public double getTargetLTIFRContractorOffsite(string company_id, string function_id,string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.ltifr_contractor_offsite,
                            c.created,
                            c.department_id
                        };



                if (department_id != "" && department_id != "null")
                {
                    v = v.Where(c => c.department_id == department_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_contractor_offsite);
                }





            }
            else
            {
                var v = from c in dbConnect.target_mains
                        join f in dbConnect.functions on c.function_id equals f.function_id
                        select new
                        {
                            c.ltifr_contractor_offsite,
                            c.created,
                            c.function_id,
                            f.company_id
                        };

                if (company_id != "" && company_id != "null")
                {
                    v = v.Where(c => c.company_id == company_id);

                }


                if (function_id != "" && function_id != "null")
                {
                    v = v.Where(c => c.function_id == function_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }

                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_contractor_offsite);
                }


                if (function_id == "")
                {
                    var a = from c in dbConnect.company_ignore_targets//target ของ sccc จะไม่นับรวมจาก function
                            where c.company_id.Contains(company_id)
                            select c;

                    if (a.Count() > 0)
                    {
                        value = 0;
                    }

                }

            }
           
            return value;
        }



        public double getTargetLTIFRContractorOffsiteGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    select new
                    {
                        c.ltifr_contractor_offsite,
                        c.created,
                        c.function_id
                    };



            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.ltifr_contractor_offsite);
            }

            return value;
        }




        public double getTargetTIFREmployee(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    //where c.function_id == function_id
                    // && (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_start).Year)
                    //|| (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_end).Year))
                    select new
                    {
                        c.tifr_employee,
                        c.created,
                        c.function_id
                    };

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }


            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.tifr_employee);
            }

            return value;
        }



        public double getTargetTIFRContractorOnsite(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    //where c.function_id == function_id
                    // && (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_start).Year) 
                    //|| (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_end).Year))
                    select new
                    {
                        c.tifr_contractor_onsite,
                        c.created,
                        c.function_id
                    };

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.tifr_contractor_onsite);
            }

            return value;
        }



        public double getTargetTIFRContractorOffsite(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    // where c.function_id == function_id
                    //  && (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_start).Year)
                    //|| (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_end).Year))
                    select new
                    {
                        c.tifr_contractor_offsite,
                        c.created,
                        c.function_id
                    };

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.tifr_contractor_offsite);
            }

            return value;
        }




        public int getLTIEmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 1 //1 is employee
                    && c.severity_injury_id == 3 //3 is LTI
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
                    select new
                    {
                        c.id,
                        f.company_id,
                        c.function_id,
                        c.department_id,
                        i.incident_date

                    };

            if (company_id != "" && company_id != "null")
            {
                v = v.Where(c => c.company_id == company_id);

            }

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }

            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.incident_date <= d_end);
            }

            value = v.Count();

            return value;
        }


        public int getLTIContractorOnsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 2 //1 is contractor onsite
                    && c.severity_injury_id == 3 //3 is LTI
                    //&& i.responsible_area == "IN"//onsite
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
                    select new
                    {
                        c.id,
                        f.company_id,
                        c.function_id,
                        c.department_id,
                        i.incident_date
                    };

            if (company_id != "" && company_id != "null")
            {
                v = v.Where(c => c.company_id == company_id);

            }

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }

            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.incident_date <= d_end);
            }


            value = v.Count();



            return value;
        }


        public int getLTIContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id
                    c.type_employment_id == 5 //5 is contractor offsite
                    && c.severity_injury_id == 3 //3 is LTI
                    //&& i.responsible_area == "OUT"//offsite
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
                    select new
                    {
                        c.id,
                        f.company_id,
                        c.function_id,
                        c.department_id,
                        i.incident_date,
                      
                    };


            if (company_id != "" && company_id != "null")
            {
                v = v.Where(c => c.company_id == company_id);

            }

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }

            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.incident_date <= d_end);
            }

            value = v.Count();

            return value;
        }




        public double getWorkhourEmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.workhour_subs
                    join di in dbConnect.divisions on c.division_id equals di.division_id
                    join de in dbConnect.departments on di.department_id equals de.department_id
                    join fu in dbConnect.functions on de.function_id equals fu.function_id
                    join co in dbConnect.companies on fu.company_id equals co.company_id
                   // where di.valid_to.Value.Year >= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year
                    select new
                    {
                        c.employee,
                        de.department_id,
                        fu.company_id,
                        fu.function_id,
                        c.created
                    };

            if (company_id != "" && company_id != "null")
            {

                v = v.Where(c => c.company_id == company_id);

            }


            if (function_id != "" && function_id != "null")
            {

                v = v.Where(c => c.function_id == function_id);

            }


            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.created >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.created <= d_end);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.employee);
            }

            return value;
        }

        public double getWorkhourContractorOnsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.workhour_subs
                    join di in dbConnect.divisions on c.division_id equals di.division_id
                    join de in dbConnect.departments on di.department_id equals de.department_id
                    join f in dbConnect.functions on de.function_id equals f.function_id
                    join co in dbConnect.companies on f.company_id equals co.company_id

                    select new
                    {
                        c.contractor_onsite,
                        de.department_id,
                        f.company_id,
                        f.function_id,
                        c.created
                    };


            if (company_id != "" && company_id != "null")
            {

                v = v.Where(c => c.company_id == company_id);

            }


            if (function_id != "" && function_id != "null")
            {

                v = v.Where(c => c.function_id == function_id);

            }

            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.created >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.created <= d_end);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.contractor_onsite);
            }

            return value;
        }



        public double getWorkhourContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.workhour_subs
                    join di in dbConnect.divisions on c.division_id equals di.division_id
                    join de in dbConnect.departments on di.department_id equals de.department_id
                    join f in dbConnect.functions on de.function_id equals f.function_id
                    join co in dbConnect.companies on f.company_id equals co.company_id

                    select new
                    {
                        c.contractor_offsite,
                        f.function_id,
                        f.company_id,
                        de.department_id,
                        c.created
                    };

            if (company_id != "" && company_id != "null")
            {
                v = v.Where(c => c.company_id == company_id);

            }

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }

            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.created >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.created <= d_end);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.contractor_offsite);
            }

            return value;
        }




        public double getMultiplierContractor(string company_id, string function_id,string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.multiplier_contractor,
                            c.created,
                            c.department_id

                        };

                if (department_id != "" && department_id != "null")
                {
                    v = v.Where(c => c.department_id == department_id);

                }

                
                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }




                foreach (var rc in v)
                {
                    if (rc.multiplier_contractor > value)
                    {
                        value = Convert.ToDouble(rc.multiplier_contractor);
                    }
                }

            }
            else
            {
                var v = from c in dbConnect.target_mains
                        join f in dbConnect.functions on c.function_id equals f.function_id
                        select new
                        {
                            c.multiplier_contractor,
                            c.created,
                            c.function_id,
                            f.company_id

                        };

                if (company_id != "" && company_id != "null")
                {
                    v = v.Where(c => c.company_id == company_id);

                }

                if (function_id != "" && function_id != "null")
                {
                    v = v.Where(c => c.function_id == function_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }




                foreach (var rc in v)
                {
                    if (rc.multiplier_contractor > value)
                    {
                        value = Convert.ToDouble(rc.multiplier_contractor);
                    }
                }


            }
            

            return value;
        }



        public double getMultiplierContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.multiplier_contractor_offsite,
                            c.created,
                            c.department_id

                        };

                if (department_id != "" && department_id != "null")
                {
                    v = v.Where(c => c.department_id == department_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }




                foreach (var rc in v)
                {
                    if (rc.multiplier_contractor_offsite > value)
                    {
                        value = Convert.ToDouble(rc.multiplier_contractor_offsite);
                    }
                }

            }
            else
            {
                var v = from c in dbConnect.target_mains
                        join f in dbConnect.functions on c.function_id equals f.function_id
                        select new
                        {
                            c.multiplier_contractor_offsite,
                            c.created,
                            c.function_id,
                            f.company_id

                        };

                if (company_id != "" && company_id != "null")
                {
                    v = v.Where(c => c.company_id == company_id);

                }

                if (function_id != "" && function_id != "null")
                {
                    v = v.Where(c => c.function_id == function_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }




                foreach (var rc in v)
                {
                    if (rc.multiplier_contractor_offsite > value)
                    {
                        value = Convert.ToDouble(rc.multiplier_contractor_offsite);
                    }
                }


            }


            return value;
        }




        public double getMultiplierContractorGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    select new
                    {
                        c.multiplier_contractor,
                        c.created,
                        c.function_id,

                    };



            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }




            foreach (var rc in v)
            {
                if (rc.multiplier_contractor > value)
                {
                    value = Convert.ToDouble(rc.multiplier_contractor);
                }
            }

            return value;
        }




        public double getMultiplierContractorOffsiteGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    select new
                    {
                        c.multiplier_contractor_offsite,
                        c.created,
                        c.function_id,

                    };



            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }




            foreach (var rc in v)
            {
                if (rc.multiplier_contractor_offsite > value)
                {
                    value = Convert.ToDouble(rc.multiplier_contractor_offsite);
                }
            }

            return value;
        }





        public int getDayLost(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where i.country == country
                    && c.status == "A"
                    && c.type_employment_id == 1 //1 is employee
                    && c.severity_injury_id == 3 //3 is LTI
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    select new
                    {
                        c.id,
                        f.company_id,
                        c.function_id,
                        c.department_id,
                        i.incident_date,
                        c.day_lost

                    };

            if (company_id != "" && company_id != "null")
            {
                v = v.Where(c => c.company_id == company_id);

            }

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }

            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.incident_date <= d_end);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToInt16(rc.day_lost);
            }

            return value;
        }


        public double caclLTISR(int day_lost, double multiplier, double workhour)
        {

            double value = Math.Round((day_lost * multiplier) / workhour, 2);

            return value;
        }




        public double getTargetLTIFREmployeeContractorOnsiteOffsiteGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    select new
                    {
                        c.ltifr_all,
                        c.created,
                        c.function_id,

                    };


            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }



            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.ltifr_all);
            }

            return value;
        }





        public int getLTIEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    (c.type_employment_id == 5 || c.type_employment_id == 2 || c.type_employment_id == 1) //5 off is employee and 2 is contractor onsite
                    && c.severity_injury_id == 3 //3 is LTI
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
                    select new
                    {
                        c.id,
                        f.company_id,
                        c.function_id,
                        c.department_id,
                        i.incident_date

                    };

            if (company_id != "" && company_id != "null")
            {
                v = v.Where(c => c.company_id == company_id);

            }

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }

            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.incident_date <= d_end);
            }

            value = v.Count();

            return value;
        }


        public double getWorkhourEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.workhour_subs
                    join di in dbConnect.divisions on c.division_id equals di.division_id
                    join de in dbConnect.departments on di.department_id equals de.department_id
                    join fu in dbConnect.functions on de.function_id equals fu.function_id
                    join co in dbConnect.companies on fu.company_id equals co.company_id
                    select new
                    {
                        c.employee,
                        c.contractor_offsite,
                        c.contractor_onsite,
                        de.department_id,
                        fu.company_id,
                        fu.function_id,
                        c.created
                    };

            if (company_id != "" && company_id != "null")
            {

                v = v.Where(c => c.company_id == company_id);

            }


            if (function_id != "" && function_id != "null")
            {

                v = v.Where(c => c.function_id == function_id);

            }


            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.created >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.created <= d_end);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.employee) + Convert.ToDouble(rc.contractor_onsite) + Convert.ToDouble(rc.contractor_offsite);
            }

            return value;
        }


        public double getMultiplierEmployeeContractorOnsiteOffsiteGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    select new
                    {
                        c.multiplier_all,
                        c.created,
                        c.function_id

                    };



            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }




            foreach (var rc in v)
            {
                if (rc.multiplier_all > value)
                {
                    value = Convert.ToDouble(rc.multiplier_all);
                }
            }

            return value;
        }


        public int getDayLostEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where i.country == country
                    && c.status == "A"
                    && (c.type_employment_id == 5 || c.type_employment_id == 2)//1 is employee and 2 is contractor
                    && c.severity_injury_id == 3 //3 is LTI
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    select new
                    {
                        c.id,
                        f.company_id,
                        c.function_id,
                        c.department_id,
                        i.incident_date,
                        c.day_lost

                    };

            if (company_id != "" && company_id != "null")
            {
                v = v.Where(c => c.company_id == company_id);

            }

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }

            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.incident_date <= d_end);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToInt16(rc.day_lost);
            }

            return value;
        }




        public int getDayLostContractorOnsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where i.country == country
                    && c.status == "A"
                    && (c.type_employment_id == 2)//2 is contractor
                    && c.severity_injury_id == 3 //3 is LTI
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    select new
                    {
                        c.id,
                        f.company_id,
                        c.function_id,
                        c.department_id,
                        i.incident_date,
                        c.day_lost

                    };

            if (company_id != "" && company_id != "null")
            {
                v = v.Where(c => c.company_id == company_id);

            }

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }

            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.incident_date <= d_end);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToInt16(rc.day_lost);
            }

            return value;
        }



        public int getDayLostContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where i.country == country
                    && c.status == "A"
                    && (c.type_employment_id == 5)//5 is contractor offsite
                    && c.severity_injury_id == 3 //3 is LTI
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    select new
                    {
                        c.id,
                        f.company_id,
                        c.function_id,
                        c.department_id,
                        i.incident_date,
                        c.day_lost

                    };

            if (company_id != "" && company_id != "null")
            {
                v = v.Where(c => c.company_id == company_id);

            }

            if (function_id != "" && function_id != "null")
            {
                v = v.Where(c => c.function_id == function_id);

            }

            if (department_id != "" && department_id != "null")
            {
                v = v.Where(c => c.department_id == department_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v = v.Where(c => c.incident_date <= d_end);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToInt16(rc.day_lost);
            }

            return value;
        }


        public double getTargetLTIFREmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.ltifr_all,
                            c.created,
                            c.department_id

                        };

                if (department_id != "" && department_id != "null")
                {
                    v = v.Where(c => c.department_id == department_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }

                //if (date_end != "")
                //{
                //    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //    v = v.Where(c => c.created <= d_end);
                //}


                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_all);
                }


            }
            else
            {
                var v = from c in dbConnect.target_mains
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        select new
                        {
                            c.ltifr_all,
                            c.created,
                            c.function_id,
                            fu.company_id

                        };

                if (company_id != "" && company_id != "null")
                {
                    v = v.Where(c => c.company_id == company_id);

                }


                if (function_id != "" && function_id != "null")
                {
                    v = v.Where(c => c.function_id == function_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }

                //if (date_end != "")
                //{
                //    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //    v = v.Where(c => c.created <= d_end);
                //}




                foreach (var rc in v)
                {
                    value = value + Convert.ToDouble(rc.ltifr_all);
                }


                if (function_id == "")
                {
                    var a = from c in dbConnect.company_ignore_targets//target ของ scco จะไม่นับรวมจาก function
                            where c.company_id.Contains(company_id)
                            select c;

                    if (a.Count() > 0)
                    {
                        value = 0;
                    }

                }



            }

            return value;
        }




        public double getMultiplierEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.multiplier_all,
                            c.created,
                            c.department_id

                        };

                if (department_id != "" && department_id != "null")
                {
                    v = v.Where(c => c.department_id == department_id);

                }




                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }


                foreach (var rc in v)
                {
                    if (rc.multiplier_all > value)
                    {
                        value = Convert.ToDouble(rc.multiplier_all);
                    }
                }



            }
            else
            {
                var v = from c in dbConnect.target_mains
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        select new
                        {
                            c.multiplier_all,
                            c.created,
                            c.function_id,
                            fu.company_id

                        };

                if (company_id != "" && company_id != "null")
                {
                    v = v.Where(c => c.company_id == company_id);

                }

                if (function_id != "" && function_id != "null")
                {
                    v = v.Where(c => c.function_id == function_id);

                }


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
                }




                foreach (var rc in v)
                {
                    if (rc.multiplier_all > value)
                    {
                        value = Convert.ToDouble(rc.multiplier_all);
                    }
                }



            }

            return value;
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


            return vReturn;
        }


    }
}