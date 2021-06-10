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
    public partial class LTIFRSrilankaReport : System.Web.UI.Page
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
            string path = string.Format("{0}" + pathreport + "LTIFR_Report.xlsx", Server.MapPath(@"\"));

            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("employee");

            ArrayList headers = new ArrayList();

            headers.Add(Resources.Sot.lbsite);
            headers.Add("Target");
            headers.Add("LTI");
            headers.Add("Work hours");
            headers.Add("LTIFR");


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
            string siteid = Request.Form[ddsite.UniqueID];
            string date_start = txtstart_date.Value;
            string date_end = txtend_date.Value;
            string lang = Session["lang"].ToString();


            string seach_by = searchBy(siteid,date_start, date_end, lang);
            IRow row_seach = sheet1.GetRow(1);

            ICell cell_search = row_seach.GetCell(1);
            cell_search.SetCellValue(seach_by);

            for (int i = 1; i <= 4; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 4)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach.GetCell(i);
                cell_n.CellStyle = style2;

            }

            CellRangeAddress range = new CellRangeAddress(1, 1, 1, 4);
            sheet1.AddMergedRegion(range);



            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = (from c in dbConnect.organizations
                      where c.country == Session["country"].ToString()
                      orderby c.personnel_subarea ascending
                      select new
                      {
                          site_id = c.personnel_subarea,
                          name = c.personnel_subarea_description

                      }).Distinct();

            if (siteid != "" && siteid != "null")
            {
                v1 = v1.Where(c => c.site_id == siteid);

            }


            int count = 4;

            foreach (var rc in v1)
            {
                double targetEmployee = getTargetLTIFREmployeeSrilanka(rc.site_id, date_start, date_end, lang);
                int lti = getLTIEmployeeSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployeeSrilanka(rc.site_id, date_start, date_end, lang);
                double multiplier = getMultiplierSrilanka(rc.site_id, date_start, date_end, lang);
                double ltifr = 0;

                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }


                IRow row = sheet1.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(rc.name);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(targetEmployee);
                cell1.CellStyle = style;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(lti);
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(workhour);
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(ltifr);
                cell4.CellStyle = style;



                if (count == 4)
                {

                    double targetEmployee_group = getTargetLTIFREmployeeSrilanka("00000000", date_start, date_end, lang);
                    int lti_group = getLTIEmployeeSrilanka("", date_start, date_end, lang,Session["country"].ToString());
                    double workhour_group = getWorkhourEmployeeSrilanka("", date_start, date_end, lang);
                    double multiplier_group = getMultiplierSrilanka("00000000", date_start, date_end, lang);
                    double ltifr_group = 0;

                    if (workhour_group != 0)
                    {
                        ltifr_group = caclLTIFRANDTIFR(lti_group, multiplier_group, workhour_group);
                    }
                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    IRow row_g = sheet1.CreateRow(count - 1);
                    ICell cell_g = row_g.CreateCell(0);
                    cell_g.SetCellValue(insee_group);
                    cell_g.CellStyle = style;

                    ICell cell_g1 = row_g.CreateCell(1);
                    cell_g1.SetCellValue(targetEmployee_group);
                    cell_g1.CellStyle = style;

                    ICell cell_g2 = row_g.CreateCell(2);
                    cell_g2.SetCellValue(lti_group);
                    cell_g2.CellStyle = style;

                    ICell cell_g3 = row_g.CreateCell(3);
                    cell_g3.SetCellValue(workhour_group);
                    cell_g3.CellStyle = style;

                    ICell cell_g4 = row_g.CreateCell(4);
                    cell_g4.SetCellValue(ltifr_group);
                    cell_g4.CellStyle = style;


                }


                count++;


            }//end foreach




            ////////////////////////////////////////end sheet employee////////////////////////////////////////////////


            ISheet sheet2 = workbook.GetSheet("contractor onsite");
            setHeader(workbook, sheet2, headers);

            string seach_by2 = searchBy(siteid, date_start, date_end, lang);
            IRow row_seach2 = sheet2.GetRow(1);

            ICell cell_search2 = row_seach2.GetCell(1);
            cell_search2.SetCellValue(seach_by2);

            for (int i = 1; i <= 4; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 4)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach2.GetCell(i);
                cell_n.CellStyle = style2;

            }

            CellRangeAddress range2 = new CellRangeAddress(1, 1, 1, 4);
            sheet2.AddMergedRegion(range);

            count = 4;
            foreach (var rc in v1)
            {
                double targetContractorOnsite = getTargetLTIFRContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang);
                int lti = getLTIContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang);
                double multiplier = getMultiplierContractorSrilanka(rc.site_id, date_start, date_end, lang);
                double ltifr = 0;


                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }


                IRow row = sheet2.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(rc.name);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(targetContractorOnsite);
                cell1.CellStyle = style;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(lti);
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(workhour);
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(ltifr);
                cell4.CellStyle = style;



                if (count == 4)
                {

                    double targetContractorOnsite_group = getTargetLTIFRContractorOnsiteSrilanka("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOnsiteSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourContractorOnsiteSrilanka("", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorSrilanka("00000000", date_start, date_end, lang);
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
                    cell_g1.SetCellValue(targetContractorOnsite_group);
                    cell_g1.CellStyle = style;

                    ICell cell_g2 = row_g.CreateCell(2);
                    cell_g2.SetCellValue(lti_group);
                    cell_g2.CellStyle = style;

                    ICell cell_g3 = row_g.CreateCell(3);
                    cell_g3.SetCellValue(workhour_group);
                    cell_g3.CellStyle = style;

                    ICell cell_g4 = row_g.CreateCell(4);
                    cell_g4.SetCellValue(ltifr_group);
                    cell_g4.CellStyle = style;


                }


                count++;


            }//end foreach




            ////////////////////////////////////////end sheet contractor onsite////////////////////////////////////////////////


            ArrayList headers3 = new ArrayList();

            headers3.Add(Resources.Sot.lbsite);
            headers3.Add("Target");
            headers3.Add("LTI");
            headers3.Add("Workhour");

            ISheet sheet3 = workbook.GetSheet("contractor offsite");

            setHeader(workbook, sheet3, headers3);

            string seach_by3 = searchBy(siteid, date_start, date_end, lang);
            IRow row_seach3 = sheet3.GetRow(1);

            ICell cell_search3 = row_seach3.GetCell(1);
            cell_search3.SetCellValue(seach_by3);

            for (int i = 1; i <= 3; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == 3)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach3.GetCell(i);
                cell_n.CellStyle = style2;

            }

            CellRangeAddress range3 = new CellRangeAddress(1, 1, 1, 3);
            sheet3.AddMergedRegion(range3);

            count = 4;
            foreach (var rc in v1)
            {
                double targetContractorOffsite = getTargetLTIFRContractorOffsiteSrilanka(rc.site_id, date_start, date_end, lang);
                int lti = getLTIContractorOffsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourContractorOffsiteSrilanka(rc.site_id, date_start, date_end, lang);
                double multiplier = getMultiplierContractorSrilanka(rc.site_id, date_start, date_end, lang);
                double ltifr = 0;
                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }


                IRow row = sheet3.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(rc.name);
                cell.CellStyle = style;


                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(targetContractorOffsite);
                cell1.CellStyle = style;

                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(lti);
                cell2.CellStyle = style;

                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(workhour);
                cell3.CellStyle = style;

                //ICell cell5 = row.CreateCell(5);
                //cell5.SetCellValue(ltifr);
                //cell5.CellStyle = style;



                if (count == 4)
                {

                    double targetContractorOffsite_group = getTargetLTIFRContractorOffsiteSrilanka("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOffsiteSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourContractorOffsiteSrilanka("", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorSrilanka("00000000", date_start, date_end, lang);
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
                    cell_g1.SetCellValue(targetContractorOffsite_group);
                    cell_g1.CellStyle = style;

                    ICell cell_g2 = row_g.CreateCell(2);
                    cell_g2.SetCellValue(lti_group);
                    cell_g2.CellStyle = style;

                    ICell cell_g3 = row_g.CreateCell(3);
                    cell_g3.SetCellValue(workhour_group);
                    cell_g3.CellStyle = style;

                    //ICell cell_g5 = row_g.CreateCell(5);
                    //cell_g5.SetCellValue(ltifr_group);
                    //cell_g5.CellStyle = style;


                }


                count++;


            }//end foreach




            ////////////////////////////////////////end sheet contractor offsite////////////////////////////////////////////////


            setWidthColunm(workbook, sheet1, headers);
            setWidthColunm(workbook, sheet2, headers);
            setWidthColunm(workbook, sheet3, headers);

            // string path_write = string.Format("{0}\\report\\template\\LTIFRReport.xlsx", Server.MapPath(@"\"));
            string path_write = string.Format("{0}" + pathreport + "LTIFRReport.xlsx", Server.MapPath(@"\"));

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


        protected string searchBy(string site_id,string date_start, string date_end, string lang)
        {
            string searchby = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            if (site_id != "")
            {
     
                var na = from a in dbConnect.organizations
                         where a.personnel_subarea == site_id
                         select new
                         {
                             name = a.personnel_subarea_description
                         };

                foreach (var q in na)
                {
                    searchby = searchby + Resources.Sot.lbsite + " :" + q.name;
                }

              
            }
            else
            {
                searchby = searchby + Resources.Sot.lbsite + " :" + Resources.Main.all;

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

        public double getTargetLTIFREmployeeSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_main_srilankas

                    select new
                    {
                        c.ltifr_employee,
                        c.created,
                        c.site_id

                    };

            if (site_id != "" && site_id != "null")
            {
                v = v.Where(c => c.site_id == site_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }




            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.ltifr_employee);
            }

            return value;
        }

        public int getLTIEmployeeSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();

            int value = 0;

            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where // c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 1 //1 is employee
                    && c.severity_injury_id == 3 //3 is LTI
                    && i.process_status != 3//3 is reject
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    select new
                    {
                        c.id,
                        c.department_id,
                        c.function_id,
                        i.incident_date

                    };

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



            foreach (var s in v)
            {

                var v2 = from c in dbConnect.organizations
                         where c.country == country
                         && c.org_unit_id == s.department_id
                         orderby c.personnel_subarea ascending
                         select new
                         {
                             id = c.personnel_subarea,
                             name = c.personnel_subarea_description

                         };
                if (v2.Count() > 0)
                {
                    foreach (var rc in v2)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {

                    var v3 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.function_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v3.Count() > 0)
                    {
                        foreach (var rc in v3)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }

                }


            }// end foreach


            if (site_id != "")
            {
                var o = ls_sub.Where(s => s == site_id);
                value = o.Count();
            }
            else
            {
                value = ls_sub.Count();
            }


            return value;
        }


        public double getWorkhourEmployeeSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.workhour_main_srilankas

                    select new
                    {
                        c.employee,
                        c.site_id,
                        c.created
                    };
            if (site_id != "" && site_id != "null")
            {
                v = v.Where(c => c.site_id == site_id);

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



        public double getMultiplierSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_main_srilankas
                    select new
                    {
                        c.multiplier,
                        c.created,
                        c.site_id

                    };

            if (site_id != "" && site_id != "null")
            {
                v = v.Where(c => c.site_id == site_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }




            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.multiplier);
            }

            return value;
        }


        public double caclLTIFRANDTIFR(int lti, double multiplier, double workhour)
        {

            double value = Math.Round((lti * multiplier) / workhour, 2);

            return value;
        }


        public double getTargetLTIFRContractorOnsiteSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_main_srilankas

                    select new
                    {
                        c.ltifr_contractor_onsite,
                        c.created,
                        c.site_id
                    };

            if (site_id != "" && site_id != "null")
            {
                v = v.Where(c => c.site_id == site_id);

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


        public int getLTIContractorOnsiteSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where
                        //c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 3 //3 is LTI
                    && i.responsible_area == "IN"//onsite
                    && i.process_status != 3//3 is reject
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    select new
                    {
                        c.id,
                        c.function_id,
                        c.department_id,
                        i.incident_date
                    };



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

            foreach (var s in v)
            {


                var v2 = from c in dbConnect.organizations
                         where c.country == country
                         && c.org_unit_id == s.department_id
                         orderby c.personnel_subarea ascending
                         select new
                         {
                             id = c.personnel_subarea,
                             name = c.personnel_subarea_description

                         };
                if (v2.Count() > 0)
                {
                    foreach (var rc in v2)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {

                    var v3 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.function_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v3.Count() > 0)
                    {
                        foreach (var rc in v3)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }

                }



            }// end foreach


            if (site_id != "")
            {
                var o = ls_sub.Where(s => s == site_id);
                value = o.Count();
            }
            else
            {
                value = ls_sub.Count();
            }



            return value;
        }



        public double getWorkhourContractorOnsiteSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.workhour_main_srilankas

                    select new
                    {
                        c.contractor_onsite,
                        c.site_id,
                        c.created
                    };

            if (site_id != "" && site_id != "null")
            {
                v = v.Where(c => c.site_id == site_id);

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



        public double getMultiplierContractorSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_main_srilankas
                    select new
                    {
                        c.multiplier_contractor,
                        c.created,
                        c.site_id

                    };

            if (site_id != "" && site_id != "null")
            {
                v = v.Where(c => c.site_id == site_id);

            }


            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v = v.Where(c => Convert.ToDateTime(c.created).Year == d_start.Year);
            }




            foreach (var rc in v)
            {
                value = value + Convert.ToDouble(rc.multiplier_contractor);
            }

            return value;
        }


        public double getTargetLTIFRContractorOffsiteSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_main_srilankas

                    select new
                    {
                        c.ltifr_contractor_offsite,
                        c.created,
                        c.site_id
                    };

            if (site_id != "" && site_id != "null")
            {
                v = v.Where(c => c.site_id == site_id);

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

        public int getLTIContractorOffsiteSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where
                        //c.function_id == function_id && c.department_id == department_id
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 3 //3 is LTI
                    && i.responsible_area == "OUT"//offsite
                    && i.process_status != 3//3 is reject
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    select new
                    {
                        c.id,
                        c.function_id,
                        c.department_id,
                        i.incident_date
                    };




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


            foreach (var s in v)
            {


                var v2 = from c in dbConnect.organizations
                         where c.country == country
                         && c.org_unit_id == s.department_id
                         orderby c.personnel_subarea ascending
                         select new
                         {
                             id = c.personnel_subarea,
                             name = c.personnel_subarea_description

                         };
                if (v2.Count() > 0)
                {
                    foreach (var rc in v2)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {

                    var v3 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.function_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v3.Count() > 0)
                    {
                        foreach (var rc in v3)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }

                }


            }// end foreach



            if (site_id != "")
            {
                var o = ls_sub.Where(s => s == site_id);
                value = o.Count();
            }
            else
            {
                value = ls_sub.Count();
            }

            value = v.Count();

            return value;
        }


        public double getWorkhourContractorOffsiteSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.workhour_main_srilankas

                    select new
                    {
                        c.contractor_offsite,
                        c.site_id,
                        c.created
                    };

            if (site_id != "" && site_id != "null")
            {
                v = v.Where(c => c.site_id == site_id);

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