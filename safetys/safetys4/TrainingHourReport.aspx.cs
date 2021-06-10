using NPOI.HSSF.UserModel;
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
    public partial class TrainingHourReport : System.Web.UI.Page
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

                    LinkButton link = (LinkButton)Master.FindControl("btTrainingHourReport");
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
            string filename = "TrainingHourReport.xlsx";
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

            string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
            string path = string.Format("{0}" + pathreport + "TrainingHour_Report.xlsx", Server.MapPath(@"\"));

            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("training hour");


            ICellStyle style = workbook.CreateCellStyle();
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BottomBorderColor = IndexedColors.Black.Index;

            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.LeftBorderColor = IndexedColors.Black.Index;


            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.RightBorderColor = IndexedColors.Black.Index;


            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.TopBorderColor = IndexedColors.Black.Index;



            ICellStyle styleg = workbook.CreateCellStyle();
            styleg.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            styleg.BottomBorderColor = IndexedColors.Black.Index;

            styleg.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            styleg.LeftBorderColor = IndexedColors.Black.Index;


            styleg.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            styleg.RightBorderColor = IndexedColors.Black.Index;


            styleg.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            styleg.TopBorderColor = IndexedColors.Black.Index;

            styleg.FillForegroundColor = IndexedColors.Yellow.Index;
            styleg.FillPattern = FillPattern.SolidForeground;



            ICellStyle stylec = workbook.CreateCellStyle();
            stylec.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            stylec.BottomBorderColor = IndexedColors.Black.Index;

            stylec.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            stylec.LeftBorderColor = IndexedColors.Black.Index;


            stylec.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            stylec.RightBorderColor = IndexedColors.Black.Index;


            stylec.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            stylec.TopBorderColor = IndexedColors.Black.Index;

            stylec.FillForegroundColor = IndexedColors.SeaGreen.Index;
            stylec.FillPattern = FillPattern.SolidForeground;



            ICellStyle stylef = workbook.CreateCellStyle();
            stylef.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            stylef.BottomBorderColor = IndexedColors.Black.Index;

            stylef.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            stylef.LeftBorderColor = IndexedColors.Black.Index;


            stylef.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            stylef.RightBorderColor = IndexedColors.Black.Index;


            stylef.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            stylef.TopBorderColor = IndexedColors.Black.Index;

            stylef.FillForegroundColor = IndexedColors.SkyBlue.Index;
            stylef.FillPattern = FillPattern.SolidForeground;



            ICellStyle styled = workbook.CreateCellStyle();
            styled.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            styled.BottomBorderColor = IndexedColors.Black.Index;

            styled.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            styled.LeftBorderColor = IndexedColors.Black.Index;


            styled.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            styled.RightBorderColor = IndexedColors.Black.Index;


            styled.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            styled.TopBorderColor = IndexedColors.Black.Index;

            styled.FillForegroundColor = IndexedColors.SeaGreen.Index;
            styled.FillPattern = FillPattern.SolidForeground;


            ICellStyle stylef2 = workbook.CreateCellStyle();
            stylef2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            stylef2.BottomBorderColor = IndexedColors.Black.Index;

            stylef2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            stylef2.LeftBorderColor = IndexedColors.Black.Index;


            stylef2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            stylef2.RightBorderColor = IndexedColors.Black.Index;


            stylef2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            stylef2.TopBorderColor = IndexedColors.Black.Index;

            stylef2.FillForegroundColor = IndexedColors.SkyBlue.Index;
            stylef2.FillPattern = FillPattern.SolidForeground;



            ICellStyle style2 = workbook.CreateCellStyle();
            style2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style2.BottomBorderColor = IndexedColors.Black.Index;

            style2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style2.LeftBorderColor = IndexedColors.Black.Index;


            style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style2.RightBorderColor = IndexedColors.Black.Index;


            style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style2.TopBorderColor = IndexedColors.Black.Index;


            ICellStyle styleg2 = workbook.CreateCellStyle();
            styleg2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            styleg2.BottomBorderColor = IndexedColors.Black.Index;

            styleg2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            styleg2.LeftBorderColor = IndexedColors.Black.Index;


            styleg2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            styleg2.RightBorderColor = IndexedColors.Black.Index;


            styleg2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            styleg2.TopBorderColor = IndexedColors.Black.Index;

            styleg2.FillForegroundColor = IndexedColors.Yellow.Index;
            styleg2.FillPattern = FillPattern.SolidForeground;



            ///////////////////////////////////////////////set value /////////////////////////////////////////////////////
            string companyid = Request.Form[ddcompany.UniqueID];
            string functionid = Request.Form[ddfunction.UniqueID];
            string departmentid = Request.Form[dddepartment.UniqueID];
            string year = Request.Form[ddyear.UniqueID];

            string date_start = "01/01/"+year;
            string date_end = "31/01/"+year;
           
            string lang = Session["lang"].ToString();


            string seach_by = searchBy(companyid,functionid, departmentid, year, lang);
            IRow row_seach = sheet1.GetRow(2);

            ICell cell_search = row_seach.GetCell(5);
            cell_search.SetCellValue(seach_by);

            


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
            int count = 6;

            string date_start_01 = "01/01/" + year;
            string date_end_01 = "31/01/" + year;

            int day_last_february = DateTime.DaysInMonth(Convert.ToInt16(year), 02);
            string date_start_02 = "01/02/" + year;
            string date_end_02 = day_last_february + "/02/" + year;

            string date_start_03 = "01/03/" + year;
            string date_end_03 = "31/03/" + year;

            string date_start_04 = "01/04/" + year;
            string date_end_04 = "30/04/" + year;

            string date_start_05 = "01/05/" + year;
            string date_end_05 = "31/05/" + year;

            string date_start_06 = "01/06/" + year;
            string date_end_06 = "30/06/" + year;

            string date_start_07 = "01/07/" + year;
            string date_end_07 = "31/07/" + year;

            string date_start_08 = "01/08/" + year;
            string date_end_08 = "31/08/" + year;

            string date_start_09 = "01/09/" + year;
            string date_end_09 = "30/09/" + year;

            string date_start_10 = "01/10/" + year;
            string date_end_10 = "31/10/" + year;

            string date_start_11 = "01/11/" + year;
            string date_end_11 = "30/11/" + year;

            string date_start_12 = "01/12/" + year;
            string date_end_12 = "31/12/" + year;


            int hc_all = 0;
            double hc_training_hour_all = 0;
         
            foreach (var rc in v1)
            {
              
                double training_hour_january_c = getTrainingHour(rc.company_id, "", "", date_start_01, date_end_01, lang);
                double training_hour_february_c = getTrainingHour(rc.company_id, "", "", date_start_02, date_end_02, lang);
                double training_hour_march_c = getTrainingHour(rc.company_id, "", "", date_start_03, date_end_03, lang);
                double training_hour_april_c = getTrainingHour(rc.company_id, "", "", date_start_04, date_end_04, lang);
                double training_hour_may_c = getTrainingHour(rc.company_id, "", "", date_start_05, date_end_05, lang);
                double training_hour_june_c = getTrainingHour(rc.company_id, "", "", date_start_06, date_end_06, lang);
                double training_hour_july_c = getTrainingHour(rc.company_id, "", "", date_start_07, date_end_07, lang);
                double training_hour_august_c = getTrainingHour(rc.company_id, "", "", date_start_08, date_end_08, lang);
                double training_hour_september_c = getTrainingHour(rc.company_id, "", "", date_start_09, date_end_09, lang);
                double training_hour_october_c = getTrainingHour(rc.company_id, "", "", date_start_10, date_end_10, lang);
                double training_hour_november_c = getTrainingHour(rc.company_id, "", "", date_start_11, date_end_11, lang);
                double training_hour_december_c = getTrainingHour(rc.company_id, "", "", date_start_12, date_end_12, lang);


                double total_training_hour_c = training_hour_january_c + training_hour_february_c + training_hour_march_c + training_hour_april_c +
                                             training_hour_may_c + training_hour_june_c + training_hour_july_c + training_hour_august_c +
                                             training_hour_september_c + training_hour_october_c + training_hour_november_c + training_hour_december_c;


                int hc_c = getHc(rc.company_id,"", ""); ; ;
                double hc_training_hour_c = 0;


                if (hc_c != 0)
                {
                    hc_training_hour_c = total_training_hour_c/hc_c;
                }

                hc_all = hc_all + hc_c;
                hc_training_hour_all = hc_training_hour_all + hc_training_hour_c;

                IDataFormat format_digit = workbook.CreateDataFormat();


                IRow row = sheet1.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(rc.company_name);
                cell.CellStyle = stylec;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue("-");
                cell1.CellStyle = stylec;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue("-");
                cell2.CellStyle = stylec;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(training_hour_january_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell3.CellStyle = styled;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(training_hour_february_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell4.CellStyle = styled;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(training_hour_march_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell5.CellStyle = styled;

                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(training_hour_april_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell6.CellStyle = styled;

                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(training_hour_may_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell7.CellStyle = styled;

                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(training_hour_june_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell8.CellStyle = styled;

                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue(training_hour_july_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell9.CellStyle = styled;

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(training_hour_august_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell10.CellStyle = styled;

                ICell cell11 = row.CreateCell(11);
                cell11.SetCellValue(training_hour_september_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell11.CellStyle = styled;


                ICell cell12 = row.CreateCell(12);
                cell12.SetCellValue(training_hour_october_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell12.CellStyle = styled;


                ICell cell13 = row.CreateCell(13);
                cell13.SetCellValue(training_hour_november_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell13.CellStyle = styled;

                ICell cell14 = row.CreateCell(14);
                cell14.SetCellValue(training_hour_december_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell14.CellStyle = styled;

                ICell cell15 = row.CreateCell(15);
                cell15.SetCellValue(total_training_hour_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell15.CellStyle = styled;

                ICell cell16 = row.CreateCell(16);
                cell16.SetCellValue(hc_c);
                cell16.CellStyle = stylec;

                ICell cell17 = row.CreateCell(17);
                cell17.SetCellValue(hc_training_hour_c);
                styled.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                cell17.CellStyle = styled;

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

                    double training_hour_january_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_01, date_end_01, lang);
                    double training_hour_february_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_02, date_end_02, lang);
                    double training_hour_march_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_03, date_end_03, lang);
                    double training_hour_april_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_04, date_end_04, lang);
                    double training_hour_may_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_05, date_end_05, lang);
                    double training_hour_june_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_06, date_end_06, lang);
                    double training_hour_july_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_07, date_end_07, lang);
                    double training_hour_august_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_08, date_end_08, lang);
                    double training_hour_september_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_09, date_end_09, lang);
                    double training_hour_october_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_10, date_end_10, lang);
                    double training_hour_november_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_11, date_end_11, lang);
                    double training_hour_december_f = getTrainingHour(rc.company_id, rc1.function_id, "", date_start_12, date_end_12, lang);


                    double total_training_hour_f = training_hour_january_f + training_hour_february_f + training_hour_march_f + training_hour_april_f +
                                                 training_hour_may_f + training_hour_june_f + training_hour_july_f + training_hour_august_f +
                                                 training_hour_september_f + training_hour_october_f + training_hour_november_f + training_hour_december_f;


                    int hc_group_f = getHc(rc.company_id, rc1.function_id, ""); ;
                    double hc_training_hour_f = 0;

                    if (hc_group_f != 0)
                    {
                        hc_training_hour_f = total_training_hour_f / hc_group_f;
                    }



                    IRow row_f = sheet1.CreateRow(count);
                    ICell cell1_f = row_f.CreateCell(1);
                    cell1_f.SetCellValue(rc1.function_name);
                    cell1_f.CellStyle = stylef;

                    ICell cell2_f = row_f.CreateCell(2);
                    cell2_f.SetCellValue("-");
                    cell2_f.CellStyle = stylef;

                    ICell cell3_f = row_f.CreateCell(3);
                    cell3_f.SetCellValue(training_hour_january_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell3_f.CellStyle = stylef2;

                    ICell cell4_f = row_f.CreateCell(4);
                    cell4_f.SetCellValue(training_hour_february_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell4_f.CellStyle = stylef2;

                    ICell cell5_f = row_f.CreateCell(5);
                    cell5_f.SetCellValue(training_hour_march_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell5_f.CellStyle = stylef2;


                    ICell cell6_f = row_f.CreateCell(6);
                    cell6_f.SetCellValue(training_hour_april_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell6_f.CellStyle = stylef2;

                    ICell cell7_f = row_f.CreateCell(7);
                    cell7_f.SetCellValue(training_hour_may_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell7_f.CellStyle = stylef2;

                    ICell cell8_f = row_f.CreateCell(8);
                    cell8_f.SetCellValue(training_hour_june_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell8_f.CellStyle = stylef2;

                    ICell cell9_f = row_f.CreateCell(9);
                    cell9_f.SetCellValue(training_hour_july_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell9_f.CellStyle = stylef2;

                    ICell cell10_f = row_f.CreateCell(10);
                    cell10_f.SetCellValue(training_hour_august_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell10_f.CellStyle = stylef2;

                    ICell cell11_f = row_f.CreateCell(11);
                    cell11_f.SetCellValue(training_hour_september_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell11_f.CellStyle = stylef2;

                    ICell cell12_f = row_f.CreateCell(12);
                    cell12_f.SetCellValue(training_hour_october_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell12_f.CellStyle = stylef2;

                    ICell cell13_f = row_f.CreateCell(13);
                    cell13_f.SetCellValue(training_hour_november_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell13_f.CellStyle = stylef2;

                    ICell cell14_f = row_f.CreateCell(14);
                    cell14_f.SetCellValue(training_hour_december_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell14_f.CellStyle = stylef2;

                    ICell cell15_f = row_f.CreateCell(15);
                    cell15_f.SetCellValue(total_training_hour_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell15_f.CellStyle = stylef2;

                    ICell cell16_f = row_f.CreateCell(16);
                    cell16_f.SetCellValue(hc_group_f);
                    cell16_f.CellStyle = stylef;

                    ICell cell17_f = row_f.CreateCell(17);
                    cell17_f.SetCellValue(hc_training_hour_f);
                    stylef2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    cell17_f.CellStyle = stylef2;


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
                                 d.department_en,
                                 d.department_id,
                             };



                    if (departmentid != "" && departmentid != "null")
                    {
                        v3 = v3.Where(c => c.department_id == departmentid);

                    }


                    foreach (var rc2 in v3)
                    {
                        //if (rc2.department_en !="other")
                        //{
                            double training_hour_january_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id , date_start_01, date_end_01, lang);
                            double training_hour_february_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_02, date_end_02, lang);
                            double training_hour_march_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_03, date_end_03, lang);
                            double training_hour_april_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_04, date_end_04, lang);
                            double training_hour_may_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_05, date_end_05, lang);
                            double training_hour_june_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_06, date_end_06, lang);
                            double training_hour_july_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_07, date_end_07, lang);
                            double training_hour_august_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_08, date_end_08, lang);
                            double training_hour_september_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_09, date_end_09, lang);
                            double training_hour_october_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_10, date_end_10, lang);
                            double training_hour_november_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_11, date_end_11, lang);
                            double training_hour_december_d = getTrainingHour(rc.company_id, rc1.function_id, rc2.department_id, date_start_12, date_end_12, lang);


                            double total_training_hour_d = training_hour_january_d + training_hour_february_d + training_hour_march_d + training_hour_april_d +
                                                         training_hour_may_d + training_hour_june_d + training_hour_july_d + training_hour_august_d +
                                                         training_hour_september_d + training_hour_october_d + training_hour_november_d + training_hour_december_d;


                            int hc_group_d = getHc(rc.company_id, rc1.function_id, rc2.department_id);
                            double hc_training_hour_d = 0;

                            if (hc_group_d != 0)
                            {
                                hc_training_hour_d = total_training_hour_d / hc_group_d;
                            }


                            IRow row_d = sheet1.CreateRow(count);
                            ICell cell2_d = row_d.CreateCell(2);
                            cell2_d.SetCellValue(rc2.department_name);
                            cell2_d.CellStyle = style;

                            ICell cell3_d = row_d.CreateCell(3);
                            cell3_d.SetCellValue(training_hour_january_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell3_d.CellStyle = style2;

                            ICell cell4_d = row_d.CreateCell(4);
                            cell4_d.SetCellValue(training_hour_february_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell4_d.CellStyle = style2;

                            ICell cell5_d = row_d.CreateCell(5);
                            cell5_d.SetCellValue(training_hour_march_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell5_d.CellStyle = style2;


                            ICell cell6_d = row_d.CreateCell(6);
                            cell6_d.SetCellValue(training_hour_april_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell6_d.CellStyle = style2;

                            ICell cell7_d = row_d.CreateCell(7);
                            cell7_d.SetCellValue(training_hour_may_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell7_d.CellStyle = style2;

                            ICell cell8_d = row_d.CreateCell(8);
                            cell8_d.SetCellValue(training_hour_june_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell8_d.CellStyle = style2;

                            ICell cell9_d = row_d.CreateCell(9);
                            cell9_d.SetCellValue(training_hour_july_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell9_d.CellStyle = style2;

                            ICell cell10_d = row_d.CreateCell(10);
                            cell10_d.SetCellValue(training_hour_august_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell10_d.CellStyle = style2;

                            ICell cell11_d = row_d.CreateCell(11);
                            cell11_d.SetCellValue(training_hour_september_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell11_d.CellStyle = style2;

                            ICell cell12_d = row_d.CreateCell(12);
                            cell12_d.SetCellValue(training_hour_october_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell12_d.CellStyle = style2;

                            ICell cell13_d = row_d.CreateCell(13);
                            cell13_d.SetCellValue(training_hour_november_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell13_d.CellStyle = style2;

                            ICell cell14_d = row_d.CreateCell(14);
                            cell14_d.SetCellValue(training_hour_december_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell14_d.CellStyle = style2;

                            ICell cell15_d = row_d.CreateCell(15);
                            cell15_d.SetCellValue(total_training_hour_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell15_d.CellStyle = style2;

                            ICell cell16_d = row_d.CreateCell(16);
                            cell16_d.SetCellValue(hc_group_d);
                            cell16_d.CellStyle = style;

                            ICell cell17_d = row_d.CreateCell(17);
                            cell17_d.SetCellValue(hc_training_hour_d);
                            style2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell17_d.CellStyle = style2;


                            count++;
                       // }

                    }


                }



            }//end foreach



            double training_hour_january_group = getTrainingHour("", "", "", date_start_01, date_end_01, lang);
            double training_hour_february_group = getTrainingHour("", "", "", date_start_02, date_end_02, lang);
            double training_hour_march_group = getTrainingHour("", "", "", date_start_03, date_end_03, lang);
            double training_hour_april_group = getTrainingHour("", "", "", date_start_04, date_end_04, lang);
            double training_hour_may_group = getTrainingHour("", "", "", date_start_05, date_end_05, lang);
            double training_hour_june_group = getTrainingHour("", "", "", date_start_06, date_end_06, lang);
            double training_hour_july_group = getTrainingHour("", "", "", date_start_07, date_end_07, lang);
            double training_hour_august_group = getTrainingHour("", "", "", date_start_08, date_end_08, lang);
            double training_hour_september_group = getTrainingHour("", "", "", date_start_09, date_end_09, lang);
            double training_hour_october_group = getTrainingHour("", "", "", date_start_10, date_end_10, lang);
            double training_hour_november_group = getTrainingHour("", "", "", date_start_11, date_end_11, lang);
            double training_hour_december_group = getTrainingHour("", "", "", date_start_12, date_end_12, lang);

            double total_training_hour_group = training_hour_january_group + training_hour_february_group + training_hour_march_group + training_hour_april_group +
                                            training_hour_may_group + training_hour_june_group + training_hour_july_group + training_hour_august_group +
                                            training_hour_september_group + training_hour_october_group + training_hour_november_group + training_hour_december_group;

            double training_hour_all_group = total_training_hour_group / hc_all;

            string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

            IRow row_g = sheet1.CreateRow(5);
            ICell cell_g = row_g.CreateCell(0);
            cell_g.SetCellValue(insee_group);
            cell_g.CellStyle = styleg;

            ICell cell_g1 = row_g.CreateCell(1);
            cell_g1.SetCellValue(insee_group);
            cell_g1.CellStyle = styleg;

            ICell cell_g2 = row_g.CreateCell(2);
            cell_g2.SetCellValue(insee_group);
            cell_g2.CellStyle = styleg;

            ICell cell_g3 = row_g.CreateCell(3);
            cell_g3.SetCellValue(training_hour_january_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g3.CellStyle = styleg2;

            ICell cell_g4 = row_g.CreateCell(4);
            cell_g4.SetCellValue(training_hour_february_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g4.CellStyle = styleg2;

            ICell cell_g5 = row_g.CreateCell(5);
            cell_g5.SetCellValue(training_hour_march_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g5.CellStyle = styleg2;


            ICell cell_g6 = row_g.CreateCell(6);
            cell_g6.SetCellValue(training_hour_april_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g6.CellStyle = styleg2;

            ICell cell_g7 = row_g.CreateCell(7);
            cell_g7.SetCellValue(training_hour_may_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g7.CellStyle = styleg2;

            ICell cell_g8 = row_g.CreateCell(8);
            cell_g8.SetCellValue(training_hour_june_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g8.CellStyle = styleg2;

            ICell cell_g9 = row_g.CreateCell(9);
            cell_g9.SetCellValue(training_hour_july_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g9.CellStyle = styleg2;

            ICell cell_g10 = row_g.CreateCell(10);
            cell_g10.SetCellValue(training_hour_august_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g10.CellStyle = styleg2;

            ICell cell_g11 = row_g.CreateCell(11);
            cell_g11.SetCellValue(training_hour_september_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g11.CellStyle = styleg2;

            ICell cell_g12 = row_g.CreateCell(12);
            cell_g12.SetCellValue(training_hour_october_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g12.CellStyle = styleg2;


            ICell cell_g13 = row_g.CreateCell(13);
            cell_g13.SetCellValue(training_hour_november_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g13.CellStyle = styleg2;


            ICell cell_g14 = row_g.CreateCell(14);
            cell_g14.SetCellValue(training_hour_december_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g14.CellStyle = styleg2;


            ICell cell_g15 = row_g.CreateCell(15);
            cell_g15.SetCellValue(total_training_hour_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g15.CellStyle = styleg2;

            ICell cell_g16 = row_g.CreateCell(16);
            cell_g16.SetCellValue(hc_all);
            cell_g16.CellStyle = styleg;


            ICell cell_g17 = row_g.CreateCell(17);
            cell_g17.SetCellValue(training_hour_all_group);
            styleg2.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            cell_g17.CellStyle = styleg2;

           

            setWidthColunm(workbook, sheet1, 17);
            // string path_write = string.Format("{0}\\report\\template\\TIFRReport.xlsx", Server.MapPath(@"\"));
            string path_write = string.Format("{0}" + pathreport + "TrainingHourReport.xlsx", Server.MapPath(@"\"));

            using (var f = File.Create(path_write))
            {
                workbook.Write(f);
            }
            Response.WriteFile(path_write);

            Response.Flush();
            Response.End();



        }







        protected void setWidthColunm(XSSFWorkbook workbook, ISheet sheet, int amountcolumn)
        {
            for (int i = 0; i < amountcolumn; i++)
            {
                sheet.AutoSizeColumn(i);

            }

        }


        protected string searchBy(string company_id,string function_id, string department_id, string year, string lang)
        {
            string searchby = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();


            if (company_id != "")
            {
                var f = from c in dbConnect.companies
                        where c.company_id == company_id
                        select new
                        {
                            company_name = chageDataLanguage(c.company_th, c.company_en, lang)
                        };

                foreach (var r in f)
                {
                    searchby = searchby + Resources.Traininghour.lbCompany + " :" + r.company_name;
                }

            }
            else
            {
                searchby = searchby + Resources.Traininghour.lbCompany + " :" + Resources.Main.all;

            }


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
                    searchby = searchby + Resources.Traininghour.lbfucntion + " :" + r.function_name;
                }

            }
            else
            {
                searchby = searchby + Resources.Traininghour.lbfucntion + " :" + Resources.Main.all;

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
                    searchby = searchby + ", " + Resources.Traininghour.lbdepartment + " :" + e.department_name;
                }
            }
            else
            {
                searchby = searchby + ", " + Resources.Traininghour.lbdepartment + " :" + Resources.Main.all;

            }





            if (year != "")
            {
                searchby = searchby + ", " + Resources.Traininghour.lbyear + " :" + year;
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





        public double getTrainingHour(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
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
                        c.training_hour,
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
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", "en");//work hour เก็บวันที่เป็น คศ.

                v = v.Where(c => c.created >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", "en");
                v = v.Where(c => c.created <= d_end);
            }

            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.training_hour);
            }

            return value;
        }





        public int getHc(string company_id, string function_id, string department_id)
        {
            int count = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.view_employee_actives
                   // join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                    where c.country == Session["country"].ToString()
                    
                    select new
                    {
                        c.function_id,
                        c.company_id,
                        c.department_id,
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


            count = v.Count();

            return count;
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