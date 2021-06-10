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
    public partial class FaltalityRateReport : System.Web.UI.Page
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

                    LinkButton link = (LinkButton)Master.FindControl("btFaltalityRateReport");
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
            string filename = "FatalityRateReport.xlsx";
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

            string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
            string path = string.Format("{0}" + pathreport + "FatalityRate_Report.xlsx", Server.MapPath(@"\"));

            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("employee");

            ArrayList headers = new ArrayList();

            headers.Add(Resources.Incident.lbCompany);
            headers.Add(Resources.Incident.lbfucntion);
            headers.Add(Resources.Hazard.lbdepartment);
            headers.Add("No. of fatality");
            headers.Add("Work hours");
            headers.Add("Fatality rate");


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

            for (int i = 1; i <= 5; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 5)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach.GetCell(i);
                cell_n.CellStyle = style2;

            }

            CellRangeAddress range = new CellRangeAddress(1, 1, 1, 5);
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


                    int fatality_group = getFatalityEmployee("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourEmployee("", "", "", date_start, date_end, lang);
                    double fatality_rate_group = 0;

                    if (workhour_group != 0)
                    {
                        fatality_rate_group = caclFatalityRate(fatality_group, workhour_group);
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
                    cell_g3.SetCellValue(fatality_group);
                    cell_g3.CellStyle = style;

                    ICell cell_g4 = row_g.CreateCell(4);
                    cell_g4.SetCellValue(String.Format("{0:n}", workhour_group));
                    cell_g4.CellStyle = style;


                    ICell cell_g5 = row_g.CreateCell(5);
                    cell_g5.SetCellValue(fatality_rate_group.ToString("F2"));
                    cell_g5.CellStyle = style;


                }


               
                int fatality = getFatalityEmployee(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployee(rc.company_id, "", "", date_start, date_end, lang);
                double fatality_rate = 0;

                if (workhour != 0)
                {
                    fatality_rate = caclFatalityRate(fatality, workhour);
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
                cell3.SetCellValue(fatality);
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(String.Format("{0:n}", workhour));
                cell4.CellStyle = style;


                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(fatality_rate.ToString("F2"));
                cell5.CellStyle = style;


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

                    int fatality_f = getFatalityEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_f = getWorkhourEmployee(rc.company_id, rc1.function_id, "", date_start, date_end, lang);

                    double fatality_rate_f = 0;

                    if (workhour_f != 0)
                    {
                        fatality_rate_f = caclFatalityRate(fatality_f, workhour_f);
                    }

                  
                    IRow row_f = sheet1.CreateRow(count);
                    ICell cell1_f = row_f.CreateCell(1);
                    cell1_f.SetCellValue(rc1.function_name);
                    cell1_f.CellStyle = style;

                    ICell cell2_f = row_f.CreateCell(2);
                    cell2_f.SetCellValue("-");
                    cell2_f.CellStyle = style;

                    ICell cell3_f = row_f.CreateCell(3);
                    cell3_f.SetCellValue(fatality_f);
                    cell3_f.CellStyle = style;

                    ICell cell4_f = row_f.CreateCell(4);
                    cell4_f.SetCellValue(String.Format("{0:n}", workhour_f));
                    cell4_f.CellStyle = style;


                    ICell cell5_f = row_f.CreateCell(5);
                    cell5_f.SetCellValue(fatality_rate_f.ToString("F2"));
                    cell5_f.CellStyle = style;


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
                          
                            int fatality_d = getFatalityEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang, Session["country"].ToString());
                            double workhour_d = getWorkhourEmployee("", rc1.function_id, rc2.department_id, date_start, date_end, lang);
                            double fatality_rate_d = 0;

                            if (workhour_d != 0)
                            {
                                fatality_rate_d = caclFatalityRate(fatality_d, workhour_d);
                            }

                          

                            IRow row_d = sheet1.CreateRow(count);
                            ICell cell2_d = row_d.CreateCell(2);
                            cell2_d.SetCellValue(rc2.department_name);
                            cell2_d.CellStyle = style;


                            ICell cell3_d = row_d.CreateCell(3);
                            cell3_d.SetCellValue(fatality_d);
                            cell3_d.CellStyle = style;

                            ICell cell4_d = row_d.CreateCell(4);
                            cell4_d.SetCellValue(String.Format("{0:n}", workhour_d));
                            cell4_d.CellStyle = style;

                            ICell cell5_d = row_d.CreateCell(5);
                            cell5_d.SetCellValue(fatality_d.ToString("F2"));
                            cell5_d.CellStyle = style;



                            count++;
                        }
                    }



                }



            }//end foreach




            ////////////////////////////////////////end sheet employee////////////////////////////////////////////////
           

            setWidthColunm(workbook, sheet1, headers);
          

            // string path_write = string.Format("{0}\\report\\template\\LTIFRReport.xlsx", Server.MapPath(@"\"));
            string path_write = string.Format("{0}" + pathreport + "FatalityRateReport.xlsx", Server.MapPath(@"\"));

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

        public int getFatalityEmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 1 //1 is employee
                    && c.severity_injury_id == 1 //3 is Fatality
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


        public double caclFatalityRate(int fatality, double workhour)
        {
            //10,000 ค่าจากพี่ออย
            double value = Math.Round((fatality * 10000) / workhour, 2);

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