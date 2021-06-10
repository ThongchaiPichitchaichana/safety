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
    public partial class TIFRReport : System.Web.UI.Page
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

                    LinkButton link = (LinkButton)Master.FindControl("btTIFRReport");
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
            string filename = "TIFRReport.xlsx";
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

            string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
            //string path = string.Format("{0}\\report\\template\\TIFR_Report.xlsx", Server.MapPath(@"\"));
            string path = string.Format("{0}"+ pathreport +"TIFR_Report.xlsx", Server.MapPath(@"\"));

            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("employee");

            ArrayList headers = new ArrayList();

            headers.Add(Resources.Incident.lbCompany);
            headers.Add(Resources.Incident.lbfucntion);
            headers.Add(Resources.Hazard.lbdepartment);
            headers.Add("Target");
            headers.Add("Fatality");
            headers.Add("PD");
            headers.Add("LTI");
            headers.Add("RWC");
            headers.Add("MTI");
            headers.Add("MI");
            headers.Add("Total");
            headers.Add("Hours worked");
            headers.Add("TIFR");


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

            for (int i = 1; i <= 12; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 12)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach.GetCell(i);
                cell_n.CellStyle = style2;

            }

            CellRangeAddress range = new CellRangeAddress(1, 1, 1, 12);
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

                    double targetEmployee_group = getTargetTIFREmployeeGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIEmployee("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_group = getFatalityEmployee("","", "", date_start, date_end, lang,Session["country"].ToString());
                    int pd_group = getPDEmployee("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int mti_group = getMTIEmployee("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int mi_group = getMIEmployee("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_group = getRWCEmployee("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int total_group = lti_group + fatality_group + pd_group + mti_group + mi_group + rwc_group;
                    double workhour_group = getWorkhourEmployee("","", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierGroup("00000000", date_start, date_end, lang);
                    double tifr_group = 0;

                    if (workhour_group != 0)
                    {
                        tifr_group = caclLTIFRANDTIFR(total_group, multiplier_group, workhour_group);
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
                    cell_g4.SetCellValue(fatality_group);
                    cell_g4.CellStyle = style;

                    ICell cell_g5 = row_g.CreateCell(5);
                    cell_g5.SetCellValue(pd_group);
                    cell_g5.CellStyle = style;


                    ICell cell_g6 = row_g.CreateCell(6);
                    cell_g6.SetCellValue(lti_group);
                    cell_g6.CellStyle = style;

                    ICell cell_g7 = row_g.CreateCell(7);
                    cell_g7.SetCellValue(rwc_group);
                    cell_g7.CellStyle = style;

                    ICell cell_g8 = row_g.CreateCell(8);
                    cell_g8.SetCellValue(mti_group);
                    cell_g8.CellStyle = style;

                    ICell cell_g9 = row_g.CreateCell(9);
                    cell_g9.SetCellValue(mi_group);
                    cell_g9.CellStyle = style;

                    ICell cell_g10 = row_g.CreateCell(10);
                    cell_g10.SetCellValue(total_group);
                    cell_g10.CellStyle = style;

                    ICell cell_g11 = row_g.CreateCell(11);
                    cell_g11.SetCellValue(String.Format("{0:n}", workhour_group));
                    cell_g11.CellStyle = style;

                    ICell cell_g12 = row_g.CreateCell(12);
                    cell_g12.SetCellValue(tifr_group.ToString("F2"));
                    cell_g12.CellStyle = style;


                }

                double targetEmployee = getTargetTIFREmployee(rc.company_id, "", "", date_start, date_end, lang);
                int lti = getLTIEmployee(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityEmployee(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDEmployee(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIEmployee(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIEmployee(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCEmployee(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourEmployee(rc.company_id, "", "", date_start, date_end, lang);
                double multiplier = getMultiplier(rc.company_id, "", "", date_start, date_end, lang);
                double tifr = 0;

                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
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
                cell4.SetCellValue(fatality);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(pd);
                cell5.CellStyle = style;

                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(lti);
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(rwc);
                cell7.CellStyle = style;

                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(mti);
                cell8.CellStyle = style;

                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue(mi);
                cell9.CellStyle = style;

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(total);
                cell10.CellStyle = style;

                ICell cell11 = row.CreateCell(11);
                cell11.SetCellValue(String.Format("{0:n}", workhour));
                cell11.CellStyle = style;


                ICell cell12 = row.CreateCell(12);
                cell12.SetCellValue(tifr.ToString("F2"));
                cell12.CellStyle = style;

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
                    double targetEmployee_f = getTargetTIFREmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    int lti_f = getLTIEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_f = getFatalityEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int pd_f = getPDEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int mti_f = getMTIEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int mi_f = getMIEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_f = getRWCEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int total_f = lti_f + fatality_f + pd_f + mti_f + mi_f + rwc_f;
                    double workhour_f = getWorkhourEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double multiplier_f = getMultiplier(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double tifr_f = 0;

                    if (workhour_f != 0)
                    {
                        tifr_f = caclLTIFRANDTIFR(total_f, multiplier_f, workhour_f);
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
                    cell4_f.SetCellValue(fatality_f);
                    cell4_f.CellStyle = style;

                    ICell cell5_f = row_f.CreateCell(5);
                    cell5_f.SetCellValue(pd_f);
                    cell5_f.CellStyle = style;


                    ICell cell6_f = row_f.CreateCell(6);
                    cell6_f.SetCellValue(lti_f);
                    cell6_f.CellStyle = style;

                    ICell cell7_f = row_f.CreateCell(7);
                    cell7_f.SetCellValue(rwc_f);
                    cell7_f.CellStyle = style;

                    ICell cell8_f = row_f.CreateCell(8);
                    cell8_f.SetCellValue(mti_f);
                    cell8_f.CellStyle = style;

                    ICell cell9_f = row_f.CreateCell(9);
                    cell9_f.SetCellValue(mi_f);
                    cell9_f.CellStyle = style;

                    ICell cell10_f = row_f.CreateCell(10);
                    cell10_f.SetCellValue(total_f);
                    cell10_f.CellStyle = style;

                    ICell cell11_f = row_f.CreateCell(11);
                    cell11_f.SetCellValue(String.Format("{0:n}", workhour_f));
                    cell11_f.CellStyle = style;

                    ICell cell12_f = row_f.CreateCell(12);
                    cell12_f.SetCellValue(tifr_f.ToString("F2"));
                    cell12_f.CellStyle = style;


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
                            double targetEmployee_d = getTargetTIFREmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            int lti_d = getLTIEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int fatality_d = getFatalityEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int pd_d = getPDEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int mti_d = getMTIEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int mi_d = getMIEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int rwc_d = getRWCEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int total_d = lti_d + fatality_d + pd_d + mti_d + mi_d + rwc_d;
                            double workhour_d = getWorkhourEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double multiplier_d = getMultiplier("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double tifr_d = 0;

                            if (workhour_d != 0)
                            {
                                tifr_d = caclLTIFRANDTIFR(total_d, multiplier_d, workhour_d);
                            }


                            IRow row_d = sheet1.CreateRow(count);
                            ICell cell2_d = row_d.CreateCell(2);
                            cell2_d.SetCellValue(rc2.department_name);
                            cell2_d.CellStyle = style;

                            ICell cell3_d = row_d.CreateCell(3);
                            cell3_d.SetCellValue(targetEmployee_d.ToString("F2"));
                            cell3_d.CellStyle = style;

                            ICell cell4_d = row_d.CreateCell(4);
                            cell4_d.SetCellValue(fatality_d);
                            cell4_d.CellStyle = style;

                            ICell cell5_d = row_d.CreateCell(5);
                            cell5_d.SetCellValue(pd_d);
                            cell5_d.CellStyle = style;


                            ICell cell6_d = row_d.CreateCell(6);
                            cell6_d.SetCellValue(lti_d);
                            cell6_d.CellStyle = style;

                            ICell cell7_d = row_d.CreateCell(7);
                            cell7_d.SetCellValue(rwc_d);
                            cell7_d.CellStyle = style;

                            ICell cell8_d = row_d.CreateCell(8);
                            cell8_d.SetCellValue(mti_d);
                            cell8_d.CellStyle = style;

                            ICell cell9_d = row_d.CreateCell(9);
                            cell9_d.SetCellValue(mi_d);
                            cell9_d.CellStyle = style;

                            ICell cell10_d = row_d.CreateCell(10);
                            cell10_d.SetCellValue(total_d);
                            cell10_d.CellStyle = style;

                            ICell cell11_d = row_d.CreateCell(11);
                            cell11_d.SetCellValue(String.Format("{0:n}", workhour_d));
                            cell11_d.CellStyle = style;

                            ICell cell12_d = row_d.CreateCell(12);
                            cell12_d.SetCellValue(tifr_d.ToString("F2"));
                            cell12_d.CellStyle = style;


                            count++;
                        }

                    }





                }


            }//end foreach




            ////////////////////////////////////////end sheet employee////////////////////////////////////////////////


            ISheet sheet2 = workbook.GetSheet("contractor onsite");
            setHeader(workbook, sheet2, headers);

            string seach_by2 = searchBy(functionid, departmentid, date_start, date_end, lang);
            IRow row_seach2 = sheet2.GetRow(1);

            ICell cell_search2 = row_seach2.GetCell(1);
            cell_search2.SetCellValue(seach_by2);

            for (int i = 1; i <= 12; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 12)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach2.GetCell(i);
                cell_n.CellStyle = style2;

            }

            CellRangeAddress range2 = new CellRangeAddress(1, 1, 1, 12);
            sheet2.AddMergedRegion(range);

            count = 4;
            foreach (var rc in v1)
            {
                if (count == 4)
                {

                    double targetContractorOnsite_group = getTargetTIFRContractorOnsiteGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOnsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_group = getFatalityContractorOnsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int pd_group = getPDContractorOnsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int mti_group = getMTIContractorOnsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int mi_group = getMIContractorOnsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_group = getRWCContractorOnsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int total_group = lti_group + fatality_group + pd_group + mti_group + mi_group + rwc_group;
                    double workhour_group = getWorkhourContractorOnsite("", "", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorGroup("00000000", date_start, date_end, lang);
                    double tifr_group = 0;

                    if (workhour_group != 0)
                    {
                        tifr_group = caclLTIFRANDTIFR(total_group, multiplier_group, workhour_group);
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
                    cell_g4.SetCellValue(fatality_group);
                    cell_g4.CellStyle = style;

                    ICell cell_g5 = row_g.CreateCell(5);
                    cell_g5.SetCellValue(pd_group);
                    cell_g5.CellStyle = style;

                    ICell cell_g6 = row_g.CreateCell(6);
                    cell_g6.SetCellValue(lti_group);
                    cell_g6.CellStyle = style;

                    ICell cell_g7 = row_g.CreateCell(7);
                    cell_g7.SetCellValue(rwc_group);
                    cell_g7.CellStyle = style;

                    ICell cell_g8 = row_g.CreateCell(8);
                    cell_g8.SetCellValue(mti_group);
                    cell_g8.CellStyle = style;

                    ICell cell_g9 = row_g.CreateCell(9);
                    cell_g9.SetCellValue(mi_group);
                    cell_g9.CellStyle = style;

                    ICell cell_g10 = row_g.CreateCell(10);
                    cell_g10.SetCellValue(total_group);
                    cell_g10.CellStyle = style;

                    ICell cell_g11 = row_g.CreateCell(11);
                    cell_g11.SetCellValue(String.Format("{0:n}", workhour_group));
                    cell_g11.CellStyle = style;


                    ICell cell_g12 = row_g.CreateCell(12);
                    cell_g12.SetCellValue(tifr_group.ToString("F2"));
                    cell_g12.CellStyle = style;

                }


                double targetContractorOnsite = getTargetTIFRContractorOnsite(rc.company_id, "", "", date_start, date_end, lang);
                int lti = getLTIContractorOnsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityContractorOnsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDContractorOnsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIContractorOnsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIContractorOnsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCContractorOnsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourContractorOnsite(rc.company_id, "", "", date_start, date_end, lang);
                double multiplier = getMultiplierContractor(rc.company_id, "", "", date_start, date_end, lang);
                double tifr = 0;


                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
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
                cell4.SetCellValue(fatality);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(pd);
                cell5.CellStyle = style;

                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(lti);
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(rwc);
                cell7.CellStyle = style;

                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(mti);
                cell8.CellStyle = style;

                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue(mi);
                cell9.CellStyle = style;

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(total);
                cell10.CellStyle = style;

                ICell cell11 = row.CreateCell(11);
                cell11.SetCellValue(String.Format("{0:n}", workhour));
                cell11.CellStyle = style;


                ICell cell12 = row.CreateCell(12);
                cell12.SetCellValue(tifr.ToString("F2"));
                cell12.CellStyle = style;


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
                    double targetContractorOnsite_f = getTargetTIFRContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    int lti_f = getLTIContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_f = getFatalityContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int pd_f = getPDContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int mti_f = getMTIContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int mi_f = getMIContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_f = getRWCContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int total_f = lti_f + fatality_f + pd_f + mti_f + mi_f + rwc_f;
                    double workhour_f = getWorkhourContractorOnsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double multiplier_f = getMultiplierContractor(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double tifr_f = 0;


                    if (workhour_f != 0)
                    {
                        tifr_f = caclLTIFRANDTIFR(total_f, multiplier_f, workhour_f);
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
                    cell4_f.SetCellValue(fatality_f);
                    cell4_f.CellStyle = style;

                    ICell cell5_f = row_f.CreateCell(5);
                    cell5_f.SetCellValue(pd_f);
                    cell5_f.CellStyle = style;


                    ICell cell6_f = row_f.CreateCell(6);
                    cell6_f.SetCellValue(lti_f);
                    cell6_f.CellStyle = style;

                    ICell cell7_f = row_f.CreateCell(7);
                    cell7_f.SetCellValue(rwc_f);
                    cell7_f.CellStyle = style;

                    ICell cell8_f = row_f.CreateCell(8);
                    cell8_f.SetCellValue(mti_f);
                    cell8_f.CellStyle = style;

                    ICell cell9_f = row_f.CreateCell(9);
                    cell9_f.SetCellValue(mi_f);
                    cell9_f.CellStyle = style;

                    ICell cell10_f = row_f.CreateCell(10);
                    cell10_f.SetCellValue(total_f);
                    cell10_f.CellStyle = style;

                    ICell cell11_f = row_f.CreateCell(11);
                    cell11_f.SetCellValue(String.Format("{0:n}", workhour_f));
                    cell11_f.CellStyle = style;

                    ICell cell12_f = row_f.CreateCell(12);
                    cell12_f.SetCellValue(tifr_f.ToString("F2"));
                    cell12_f.CellStyle = style;


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
                            double targetContractorOnsite_d = getTargetTIFRContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            int lti_d = getLTIContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int fatality_d = getFatalityContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int pd_d = getPDContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int mti_d = getMTIContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int mi_d = getMIContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int rwc_d = getRWCContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int total_d = lti_d + fatality_d + pd_d + mti_d + mi_d + rwc_d;
                            double workhour_d = getWorkhourContractorOnsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double multiplier_d = getMultiplierContractor("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double tifr_d = 0;


                            if (workhour_d != 0)
                            {
                                tifr_d = caclLTIFRANDTIFR(total_d, multiplier_d, workhour_d);
                            }




                            IRow row_d = sheet2.CreateRow(count);
                            ICell cell2_d = row_d.CreateCell(2);
                            cell2_d.SetCellValue(rc2.department_name);
                            cell2_d.CellStyle = style;

                            ICell cell3_d = row_d.CreateCell(3);
                            cell3_d.SetCellValue(targetContractorOnsite_d.ToString("F2"));
                            cell3_d.CellStyle = style;

                            ICell cell4_d = row_d.CreateCell(4);
                            cell4_d.SetCellValue(fatality_d);
                            cell4_d.CellStyle = style;

                            ICell cell5_d = row_d.CreateCell(5);
                            cell5_d.SetCellValue(pd_d);
                            cell5_d.CellStyle = style;


                            ICell cell6_d = row_d.CreateCell(6);
                            cell6_d.SetCellValue(lti_d);
                            cell6_d.CellStyle = style;

                            ICell cell7_d = row_d.CreateCell(7);
                            cell7_d.SetCellValue(rwc_d);
                            cell7_d.CellStyle = style;

                            ICell cell8_d = row_d.CreateCell(8);
                            cell8_d.SetCellValue(mti_d);
                            cell8_d.CellStyle = style;

                            ICell cell9_d = row_d.CreateCell(9);
                            cell9_d.SetCellValue(mi_d);
                            cell9_d.CellStyle = style;

                            ICell cell10_d = row_d.CreateCell(10);
                            cell10_d.SetCellValue(total_d);
                            cell10_d.CellStyle = style;

                            ICell cell11_d = row_d.CreateCell(11);
                            cell11_d.SetCellValue(String.Format("{0:n}", workhour_d));
                            cell11_d.CellStyle = style;

                            ICell cell12_d = row_d.CreateCell(12);
                            cell12_d.SetCellValue(tifr_d.ToString("F2"));
                            cell12_d.CellStyle = style;


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
            headers3.Add("Fatality");
            headers3.Add("PD");
            headers3.Add("LTI");
            headers3.Add("RWC");
            headers3.Add("MTI");
            headers3.Add("MI");
            headers3.Add("Total");
            headers3.Add("Hours worked");
            headers3.Add("TIFR");


            ISheet sheet3 = workbook.GetSheet("contractor offsite");

            setHeader(workbook, sheet3, headers3);

            string seach_by3 = searchBy(functionid, departmentid, date_start, date_end, lang);
            IRow row_seach3 = sheet3.GetRow(1);

            ICell cell_search3 = row_seach3.GetCell(1);
            cell_search3.SetCellValue(seach_by3);

            for (int i = 1; i <= 12; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 12)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach3.GetCell(i);
                cell_n.CellStyle = style2;

            }

            CellRangeAddress range3 = new CellRangeAddress(1, 1, 1, 12);
            sheet3.AddMergedRegion(range3);

            count = 4;
            foreach (var rc in v1)
            {
                if (count == 4)
                {

                    double targetContractorOffsite_group = getTargetTIFRContractorOffsiteGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_group = getFatalityContractorOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int pd_group = getPDContractorOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int mti_group = getMTIContractorOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int mi_group = getMIContractorOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_group = getRWCContractorOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int total_group = lti_group + fatality_group + pd_group + mti_group + mi_group + rwc_group;
                    double workhour_group = getWorkhourContractorOffsite("", "", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorOffsiteGroup("00000000", date_start, date_end, lang);
                    double tifr_group = 0;

                    if (workhour_group != 0)
                    {
                        tifr_group = caclLTIFRANDTIFR(total_group, multiplier_group, workhour_group);
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
                    cell_g4.SetCellValue(fatality_group);
                    cell_g4.CellStyle = style;

                    ICell cell_g5 = row_g.CreateCell(5);
                    cell_g5.SetCellValue(pd_group);
                    cell_g5.CellStyle = style;

                    ICell cell_g6 = row_g.CreateCell(6);
                    cell_g6.SetCellValue(lti_group);
                    cell_g6.CellStyle = style;

                    ICell cell_g7 = row_g.CreateCell(7);
                    cell_g7.SetCellValue(rwc_group);
                    cell_g7.CellStyle = style;

                    ICell cell_g8 = row_g.CreateCell(8);
                    cell_g8.SetCellValue(mti_group);
                    cell_g8.CellStyle = style;

                    ICell cell_g9 = row_g.CreateCell(9);
                    cell_g9.SetCellValue(mi_group);
                    cell_g9.CellStyle = style;

                    ICell cell_g10 = row_g.CreateCell(10);
                    cell_g10.SetCellValue(total_group);
                    cell_g10.CellStyle = style;

                    ICell cell_g11 = row_g.CreateCell(11);
                    cell_g11.SetCellValue(String.Format("{0:n}", workhour_group));
                    cell_g11.CellStyle = style;

                    ICell cell_g12 = row_g.CreateCell(12);
                    cell_g12.SetCellValue(tifr_group.ToString("F2"));
                    cell_g12.CellStyle = style;


                }


                double targetContractorOffsite = getTargetTIFRContractorOffsite(rc.company_id, "", "", date_start, date_end, lang);
                int lti = getLTIContractorOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityContractorOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDContractorOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIContractorOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIContractorOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCContractorOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourContractorOffsite(rc.company_id, "", "", date_start, date_end, lang);
                double multiplier = getMultiplierContractorOffsite(rc.company_id, "", "", date_start, date_end, lang);
                double tifr = 0;
                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
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
                cell4.SetCellValue(fatality);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(pd);
                cell5.CellStyle = style;

                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(lti);
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(rwc);
                cell7.CellStyle = style;

                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(mti);
                cell8.CellStyle = style;

                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue(mi);
                cell9.CellStyle = style;

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(total);
                cell10.CellStyle = style;

                ICell cell11 = row.CreateCell(11);
                cell11.SetCellValue(String.Format("{0:n}", workhour));
                cell11.CellStyle = style;

                ICell cell12 = row.CreateCell(12);
                cell12.SetCellValue(tifr.ToString("F2"));
                cell12.CellStyle = style;


                count++;


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
                    double targetContractorOffsite_f = getTargetTIFRContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    int lti_f = getLTIContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_f = getFatalityContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int pd_f = getPDContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int mti_f = getMTIContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int mi_f = getMIContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_f = getRWCContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int total_f = lti_f + fatality_f + pd_f + mti_f + mi_f + rwc_f;
                    double workhour_f = getWorkhourContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double multiplier_f = getMultiplierContractorOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double tifr_f = 0;
                   
                    if (workhour_f != 0)
                    {
                        tifr_f = caclLTIFRANDTIFR(total_f, multiplier_f, workhour_f);
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
                    cell4_f.SetCellValue(fatality_f);
                    cell4_f.CellStyle = style;

                    ICell cell5_f = row_f.CreateCell(5);
                    cell5_f.SetCellValue(pd_f);
                    cell5_f.CellStyle = style;


                    ICell cell6_f = row_f.CreateCell(6);
                    cell6_f.SetCellValue(lti_f);
                    cell6_f.CellStyle = style;

                    ICell cell7_f = row_f.CreateCell(7);
                    cell7_f.SetCellValue(rwc_f);
                    cell7_f.CellStyle = style;

                    ICell cell8_f = row_f.CreateCell(8);
                    cell8_f.SetCellValue(mti_f);
                    cell8_f.CellStyle = style;

                    ICell cell9_f = row_f.CreateCell(9);
                    cell9_f.SetCellValue(mi_f);
                    cell9_f.CellStyle = style;

                    ICell cell10_f = row_f.CreateCell(10);
                    cell10_f.SetCellValue(total_f);
                    cell10_f.CellStyle = style;

                    ICell cell11_f = row_f.CreateCell(11);
                    cell11_f.SetCellValue(String.Format("{0:n}", workhour_f));
                    cell11_f.CellStyle = style;

                    ICell cell12_f = row_f.CreateCell(12);
                    cell12_f.SetCellValue(tifr_f.ToString("F2"));
                    cell12_f.CellStyle = style;


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
                            double targetContractorOffsite_d = getTargetTIFRContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            int lti_d = getLTIContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int fatality_d = getFatalityContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int pd_d = getPDContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int mti_d = getMTIContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int mi_d = getMIContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int rwc_d = getRWCContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int total_d = lti_d + fatality_d + pd_d + mti_d + mi_d + rwc_d;
                            double workhour_d = getWorkhourContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double multiplier_d = getMultiplierContractorOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double tifr_d = 0;
                            if (workhour_d != 0)
                            {
                                tifr_d = caclLTIFRANDTIFR(total_d, multiplier_d, workhour_d);
                            }


                            IRow row_d = sheet3.CreateRow(count);
                            ICell cell2_d = row_d.CreateCell(2);
                            cell2_d.SetCellValue(rc2.department_name);
                            cell2_d.CellStyle = style;

                            ICell cell3_d = row_d.CreateCell(3);
                            cell3_d.SetCellValue(targetContractorOffsite_d.ToString("F2"));
                            cell3_d.CellStyle = style;

                            ICell cell4_d = row_d.CreateCell(4);
                            cell4_d.SetCellValue(fatality_d);
                            cell4_d.CellStyle = style;

                            ICell cell5_d = row_d.CreateCell(5);
                            cell5_d.SetCellValue(pd_d);
                            cell5_d.CellStyle = style;


                            ICell cell6_d = row_d.CreateCell(6);
                            cell6_d.SetCellValue(lti_d);
                            cell6_d.CellStyle = style;

                            ICell cell7_d = row_d.CreateCell(7);
                            cell7_d.SetCellValue(rwc_d);
                            cell7_d.CellStyle = style;

                            ICell cell8_d = row_d.CreateCell(8);
                            cell8_d.SetCellValue(mti_d);
                            cell8_d.CellStyle = style;

                            ICell cell9_d = row_d.CreateCell(9);
                            cell9_d.SetCellValue(mi_d);
                            cell9_d.CellStyle = style;

                            ICell cell10_d = row_d.CreateCell(10);
                            cell10_d.SetCellValue(total_d);
                            cell10_d.CellStyle = style;

                            ICell cell11_d = row_d.CreateCell(11);
                            cell11_d.SetCellValue(String.Format("{0:n}", workhour_d));
                            cell11_d.CellStyle = style;

                            ICell cell12_d = row_d.CreateCell(12);
                            cell12_d.SetCellValue(tifr_d.ToString("F2"));
                            cell12_d.CellStyle = style;


                            count++;
                        }
                    }



                }


            }//end foreach




            ////////////////////////////////////////end sheet contractor offsite////////////////////////////////////////////////




            ISheet sheet4 = workbook.GetSheet("all");
            setHeader(workbook, sheet4, headers);

            string seach_by4 = searchBy(functionid, departmentid, date_start, date_end, lang);
            IRow row_seach4 = sheet4.GetRow(1);



        
            for (int i = 1; i <= 12; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 12)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach4.CreateCell(i);
                cell_n.CellStyle = style2;

            }

            ICell cell_search4 = row_seach4.GetCell(1);
            cell_search4.SetCellValue(seach_by4);


            CellRangeAddress range4 = new CellRangeAddress(1, 1, 1, 12);
            sheet4.AddMergedRegion(range);


            count = 4;
            foreach (var rc in v1)
            {
                if (count == 4)
                {

                    double targetContractorOnsite_group = getTargetTIFREmployeeContractorOnsiteOffsiteGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIEmployeeContractorOnsiteOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_group = getFatalityEmployeeContractorOnsiteOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int pd_group = getPDEmployeeContractorOnsiteOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int mti_group = getMTIEmployeeContractorOnsiteOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int mi_group = getMIEmployeeContractorOnsiteOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_group = getRWCEmployeeContractorOnsiteOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    int total_group = lti_group + fatality_group + pd_group + mti_group + mi_group + rwc_group;
                    double workhour_group = getWorkhourEmployeeContractorOnsiteOffsite("", "", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierEmployeeContractorOnsiteOffsiteGroup("00000000", date_start, date_end, lang);
                    double tifr_group = 0;

                    if (workhour_group != 0)
                    {
                        tifr_group = caclLTIFRANDTIFR(total_group, multiplier_group, workhour_group);
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
                    cell_g3.SetCellValue(targetContractorOnsite_group.ToString("F2"));
                    cell_g3.CellStyle = style;

                    ICell cell_g4 = row_g.CreateCell(4);
                    cell_g4.SetCellValue(fatality_group);
                    cell_g4.CellStyle = style;

                    ICell cell_g5 = row_g.CreateCell(5);
                    cell_g5.SetCellValue(pd_group);
                    cell_g5.CellStyle = style;

                    ICell cell_g6 = row_g.CreateCell(6);
                    cell_g6.SetCellValue(lti_group);
                    cell_g6.CellStyle = style;

                    ICell cell_g7 = row_g.CreateCell(7);
                    cell_g7.SetCellValue(rwc_group);
                    cell_g7.CellStyle = style;

                    ICell cell_g8 = row_g.CreateCell(8);
                    cell_g8.SetCellValue(mti_group);
                    cell_g8.CellStyle = style;

                    ICell cell_g9 = row_g.CreateCell(9);
                    cell_g9.SetCellValue(mi_group);
                    cell_g9.CellStyle = style;

                    ICell cell_g10 = row_g.CreateCell(10);
                    cell_g10.SetCellValue(total_group);
                    cell_g10.CellStyle = style;

                    ICell cell_g11 = row_g.CreateCell(11);
                    cell_g11.SetCellValue(String.Format("{0:n}", workhour_group));
                    cell_g11.CellStyle = style;


                    ICell cell_g12 = row_g.CreateCell(12);
                    cell_g12.SetCellValue(tifr_group.ToString("F2"));
                    cell_g12.CellStyle = style;

                }


                double targetContractorOnsite = getTargetTIFREmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang);
                int lti = getLTIEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang);
                double multiplier = getMultiplierEmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang);
                double tifr = 0;


                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
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
                cell3.SetCellValue(targetContractorOnsite.ToString("F2"));
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(fatality);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(pd);
                cell5.CellStyle = style;

                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(lti);
                cell6.CellStyle = style;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(rwc);
                cell7.CellStyle = style;

                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(mti);
                cell8.CellStyle = style;

                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue(mi);
                cell9.CellStyle = style;

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(total);
                cell10.CellStyle = style;

                ICell cell11 = row.CreateCell(11);
                cell11.SetCellValue(String.Format("{0:n}", workhour));
                cell11.CellStyle = style;


                ICell cell12 = row.CreateCell(12);
                cell12.SetCellValue(tifr.ToString("F2"));
                cell12.CellStyle = style;


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
                    double targetContractorOnsite_f = getTargetTIFREmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    int lti_f = getLTIEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_f = getFatalityEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int pd_f = getPDEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int mti_f = getMTIEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int mi_f = getMIEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_f = getRWCEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    int total_f = lti_f + fatality_f + pd_f + mti_f + mi_f + rwc_f;
                    double workhour_f = getWorkhourEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double multiplier_f = getMultiplierEmployeeContractorOnsiteOffsite(rc.company_id, rc1.function_id, "", date_start, date_end, lang);
                    double tifr_f = 0;


                    if (workhour_f != 0)
                    {
                        tifr_f = caclLTIFRANDTIFR(total_f, multiplier_f, workhour_f);
                    }


                    IRow row_f = sheet4.CreateRow(count);
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
                    cell4_f.SetCellValue(fatality_f);
                    cell4_f.CellStyle = style;

                    ICell cell5_f = row_f.CreateCell(5);
                    cell5_f.SetCellValue(pd_f);
                    cell5_f.CellStyle = style;


                    ICell cell6_f = row_f.CreateCell(6);
                    cell6_f.SetCellValue(lti_f);
                    cell6_f.CellStyle = style;

                    ICell cell7_f = row_f.CreateCell(7);
                    cell7_f.SetCellValue(rwc_f);
                    cell7_f.CellStyle = style;

                    ICell cell8_f = row_f.CreateCell(8);
                    cell8_f.SetCellValue(mti_f);
                    cell8_f.CellStyle = style;

                    ICell cell9_f = row_f.CreateCell(9);
                    cell9_f.SetCellValue(mi_f);
                    cell9_f.CellStyle = style;

                    ICell cell10_f = row_f.CreateCell(10);
                    cell10_f.SetCellValue(total_f);
                    cell10_f.CellStyle = style;

                    ICell cell11_f = row_f.CreateCell(11);
                    cell11_f.SetCellValue(String.Format("{0:n}", workhour_f));
                    cell11_f.CellStyle = style;

                    ICell cell12_f = row_f.CreateCell(12);
                    cell12_f.SetCellValue(tifr_f.ToString("F2"));
                    cell12_f.CellStyle = style;


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
                            double targetContractorOnsite_d = getTargetTIFREmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            int lti_d = getLTIEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int fatality_d = getFatalityEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int pd_d = getPDEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int mti_d = getMTIEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int mi_d = getMIEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int rwc_d = getRWCEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            int total_d = lti_d + fatality_d + pd_d + mti_d + mi_d + rwc_d;
                            double workhour_d = getWorkhourEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double multiplier_d = getMultiplierEmployeeContractorOnsiteOffsite("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double tifr_d = 0;


                            if (workhour_d != 0)
                            {
                                tifr_d = caclLTIFRANDTIFR(total_d, multiplier_d, workhour_d);
                            }




                            IRow row_d = sheet4.CreateRow(count);
                            ICell cell2_d = row_d.CreateCell(2);
                            cell2_d.SetCellValue(rc2.department_name);
                            cell2_d.CellStyle = style;

                            ICell cell3_d = row_d.CreateCell(3);
                            cell3_d.SetCellValue(targetContractorOnsite_d.ToString("F2"));
                            cell3_d.CellStyle = style;

                            ICell cell4_d = row_d.CreateCell(4);
                            cell4_d.SetCellValue(fatality_d);
                            cell4_d.CellStyle = style;

                            ICell cell5_d = row_d.CreateCell(5);
                            cell5_d.SetCellValue(pd_d);
                            cell5_d.CellStyle = style;


                            ICell cell6_d = row_d.CreateCell(6);
                            cell6_d.SetCellValue(lti_d);
                            cell6_d.CellStyle = style;

                            ICell cell7_d = row_d.CreateCell(7);
                            cell7_d.SetCellValue(rwc_d);
                            cell7_d.CellStyle = style;

                            ICell cell8_d = row_d.CreateCell(8);
                            cell8_d.SetCellValue(mti_d);
                            cell8_d.CellStyle = style;

                            ICell cell9_d = row_d.CreateCell(9);
                            cell9_d.SetCellValue(mi_d);
                            cell9_d.CellStyle = style;

                            ICell cell10_d = row_d.CreateCell(10);
                            cell10_d.SetCellValue(total_d);
                            cell10_d.CellStyle = style;

                            ICell cell11_d = row_d.CreateCell(11);
                            cell11_d.SetCellValue(String.Format("{0:n}", workhour_d));
                            cell11_d.CellStyle = style;

                            ICell cell12_d = row_d.CreateCell(12);
                            cell12_d.SetCellValue(tifr_d.ToString("F2"));
                            cell12_d.CellStyle = style;


                            count++;

                        }

                    }

                }


            }//end foreach




            ////////////////////////////////////////end sheet employee and contractor onsite & offsite////////////////////////////////////////////////


            setWidthColunm(workbook, sheet1, headers);
            setWidthColunm(workbook, sheet2, headers);
            setWidthColunm(workbook, sheet3, headers);
            setWidthColunm(workbook, sheet4, headers);

           // string path_write = string.Format("{0}\\report\\template\\TIFRReport.xlsx", Server.MapPath(@"\"));
            string path_write = string.Format("{0}"+ pathreport +"TIFRReport.xlsx", Server.MapPath(@"\"));

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


        protected string searchBy(string function_id, string department_id, string date_start, string date_end, string lang)
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


        public double getMultiplier(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
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
                    if (rc.multiplier > value)
                    {
                        value = Convert.ToDouble(rc.multiplier);
                    }
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
                    if (rc.multiplier > value)
                    {
                        value = Convert.ToDouble(rc.multiplier);
                    }
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
                    value =  Convert.ToDouble(rc.multiplier);
                }
               
            }

            return value;
        }



        public double getMultiplierContractor(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
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

        public double getTargetLTIFREmployee(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    //where c.function_id == function_id
                    // && (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_start).Year)
                    //|| (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_end).Year))
                    select new
                    {
                        c.ltifr_employee,
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





        public double getTargetLTIFRContractorOnsite(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    //where c.function_id == function_id
                    //&& (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_start).Year)
                    //|| (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_end).Year))
                    select new
                    {
                        c.ltifr_contractor_onsite,
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
                value = value + Convert.ToDouble(rc.ltifr_contractor_onsite);
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


        public double getTargetLTIFRContractorOffsite(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_mains
                    //where c.function_id == function_id
                    //&& (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_start).Year)
                    //|| (Convert.ToDateTime(c.created).Year == Convert.ToDateTime(date_end).Year))
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




        public double getTargetTIFREmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.tifr_employee,
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
                    value = value + Convert.ToDouble(rc.tifr_employee);
                }


            }
            else
            {

                var v = from c in dbConnect.target_mains
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        select new
                        {
                            c.tifr_employee,
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
                    value = value + Convert.ToDouble(rc.tifr_employee);
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




        public double getTargetTIFREmployeeGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.target_mains
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





        public double getTargetTIFRContractorOnsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.tifr_contractor_onsite,
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
                    value = value + Convert.ToDouble(rc.tifr_contractor_onsite);
                }


            }
            else
            {

                var v = from c in dbConnect.target_mains
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        select new
                        {
                            c.tifr_contractor_onsite,
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
                    value = value + Convert.ToDouble(rc.tifr_contractor_onsite);
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



        public double getTargetTIFRContractorOnsiteGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.target_mains
                    select new
                    {
                        c.tifr_contractor_onsite,
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
                value = value + Convert.ToDouble(rc.tifr_contractor_onsite);
            }



            return value;
        }




        public double getTargetTIFRContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.tifr_contractor_offsite,
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
                    value = value + Convert.ToDouble(rc.tifr_contractor_offsite);
                }


            }
            else
            {

                var v = from c in dbConnect.target_mains
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        select new
                        {
                            c.tifr_contractor_offsite,
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
                    value = value + Convert.ToDouble(rc.tifr_contractor_offsite);
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



        public double getTargetTIFRContractorOffsiteGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.target_mains
                    join fu in dbConnect.functions on c.function_id equals fu.function_id
                    select new
                    {
                        c.tifr_contractor_offsite,
                        c.created,
                        c.function_id,
                        fu.company_id
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


        public int getFatalityEmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 1 //1 is employee
                    && c.severity_injury_id == 1 //3 is fatality เสียชีวิต
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == Session["country"].ToString()
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


        public int getPDEmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 1 //1 is employee
                    && c.severity_injury_id == 2 //3 is PD
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


        public int getMTIEmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 1 //1 is employee
                    && c.severity_injury_id == 4 //3 is MTI
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


        public int getMIEmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 1 //1 is employee
                    && c.severity_injury_id == 5 //5 is MI
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


        public int getRWCEmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 1 //1 is employee
                    && c.severity_injury_id == 6 //6 is RWC
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
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 3 //3 is LTI
                   // && i.responsible_area == "IN"//onsite
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

        public int getFatalityContractorOnsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 1 //1 is Fatality
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                  //  && i.responsible_area == "IN"//onsite
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





        public int getPDContractorOnsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 2//2 is PD
                   // && i.responsible_area == "IN"//onsite
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



        public int getMTIContractorOnsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 4 //4 is MTI
                   // && i.responsible_area == "IN"//onsite
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


        public int getMIContractorOnsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 5 //5 is MI
                   // && i.responsible_area == "IN"//onsite
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



        public int getRWCContractorOnsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 6 //6 is RWC
                   // && i.responsible_area == "IN"//onsite
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
                    c.type_employment_id == 5 //5 is contractor
                    && c.severity_injury_id == 3 //3 is LTI
                   // && i.responsible_area == "OUT"//offsite
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


        public int getFatalityContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                    c.type_employment_id == 5 //5 is contractor
                    && c.severity_injury_id == 1 //1 is  Fatality
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


        public int getPDContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id
                    c.type_employment_id == 5 //5 is contractor
                    && c.severity_injury_id == 2 //2 is PD
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


        public int getMTIContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id
                    c.type_employment_id == 5 //5 is contractor
                    && c.severity_injury_id == 4 //4 is MTI
                   // && i.responsible_area == "OUT"//offsite
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


        public int getMIContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id
                    c.type_employment_id == 5//5 is contractor
                    && c.severity_injury_id == 5 //5 is MI
                   // && i.responsible_area == "OUT"//offsite
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



        public int getRWCContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id
                    c.type_employment_id == 5 //5 is contractor
                    && c.severity_injury_id == 6 //6 is RWC
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





        public double getWorkhourEmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
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


        public double getTargetTIFREmployeeContractorOnsiteOffsiteGroup(string function_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.target_mains
                    select new
                    {
                        c.tifr_all,
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
                value = value + Convert.ToDouble(rc.tifr_all);
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
                    (c.type_employment_id == 5 || c.type_employment_id == 2 || c.type_employment_id == 1) //1 is employee and 2 is contractor
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



        public int getFatalityEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    (c.type_employment_id == 5 || c.type_employment_id == 2 || c.type_employment_id == 1) //1 is employee or contractor
                    && c.severity_injury_id == 1 //1 is fatality เสียชีวิต
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == Session["country"].ToString()
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


        public int getPDEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    (c.type_employment_id == 5 || c.type_employment_id == 2 || c.type_employment_id == 1)//1 is employee or 2 is contractor
                    && c.severity_injury_id == 2 //3 is PD
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


        public int getMTIEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    (c.type_employment_id == 5 || c.type_employment_id == 2 || c.type_employment_id == 1)//1 is employee or 2 is contractor
                    && c.severity_injury_id == 4 //3 is MTI
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


        public int getMIEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    (c.type_employment_id == 5 || c.type_employment_id == 2 || c.type_employment_id == 1)//1 is employee
                    && c.severity_injury_id == 5 //5 is MI
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


        public int getRWCEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    (c.type_employment_id == 5 || c.type_employment_id == 2 || c.type_employment_id == 1) //1 is employee
                    && c.severity_injury_id == 6 //6 is RWC
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



        public double getTargetTIFREmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (department_id != "")
            {
                var v = from c in dbConnect.target_subs
                        select new
                        {
                            c.tifr_all,
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
                    value = value + Convert.ToDouble(rc.tifr_all);
                }


            }
            else
            {

                var v = from c in dbConnect.target_mains
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        select new
                        {
                            c.tifr_all,
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
                    value = value + Convert.ToDouble(rc.tifr_all);
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