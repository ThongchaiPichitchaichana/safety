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
    public partial class SOTReport : System.Web.UI.Page
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

                    LinkButton link = (LinkButton)Master.FindControl("btAllSOTReport");
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
            string filename = "SOTReport.xlsx";
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));

            string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
            //string path = string.Format("{0}\\report\\template\\hazard_report.xlsx", Server.MapPath(@"\"));
            string path = string.Format("{0}" + pathreport + "sot_report.xlsx", Server.MapPath(@"\"));
            //   Response.Write(path); Response.End();
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("sot_report");

            // sheet1.GetRow(1).GetCell(1).SetCellValue(1);
            //ICell cell = sheet1.GetRow(0).GetCell(0);
            //IRow row = sheet1.CreateRow(0);
            //ICell cell = row.CreateCell(0);
            // cell.SetCellValue(4);
            ArrayList headers = new ArrayList();

            headers.Add(Resources.Incident.sequence);
            headers.Add(Resources.Sot.doc_no);
            headers.Add(Resources.Sot.report_date);
            headers.Add(Resources.Sot.lbdate_observation);
            headers.Add(Resources.Sot.lbsottime);
            headers.Add(Resources.Sot.lbcompany);
            headers.Add(Resources.Sot.lbfunction);
            headers.Add(Resources.Sot.lbdepartment);
            headers.Add(Resources.Sot.lbdivision);
            headers.Add(Resources.Sot.lblocation);
            headers.Add(Resources.Sot.lbtypework);
            headers.Add(Resources.Sot.type_employment);
            headers.Add(Resources.Sot.lbcomment);
            headers.Add(Resources.Sot.lbname_reporter);
            headers.Add(Resources.Sot.lbfunction);
            headers.Add(Resources.Sot.lbdepartment);
            headers.Add(Resources.Sot.lbdivision);

            headers.Add(Resources.Sot.lbsotteam);
            headers.Add(Resources.Sot.lbmng_level);
            headers.Add(Resources.Sot.lbfunction);
            headers.Add(Resources.Sot.lbdepartment);
            headers.Add(Resources.Sot.lbdivision);
            headers.Add(Resources.Main.lbareamanager);
            headers.Add(Resources.Sot.lbstatus);
            headers.Add(Resources.Main.lbdateclose);


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





            ICellStyle style3 = workbook.CreateCellStyle();
            style3.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style3.BottomBorderColor = IndexedColors.Black.Index;

            style3.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style3.LeftBorderColor = IndexedColors.Black.Index;


            style3.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style3.RightBorderColor = IndexedColors.Black.Index;


            style3.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style3.TopBorderColor = IndexedColors.Black.Index;



            ICellStyle style4 = workbook.CreateCellStyle();
            style4.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style4.BottomBorderColor = IndexedColors.Black.Index;

            style4.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style4.LeftBorderColor = IndexedColors.Black.Index;


            style4.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style4.RightBorderColor = IndexedColors.Black.Index;


            style4.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style4.TopBorderColor = IndexedColors.Black.Index;



            ICellStyle style5 = workbook.CreateCellStyle();
            style5.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style5.BottomBorderColor = IndexedColors.Black.Index;

            style5.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style5.LeftBorderColor = IndexedColors.Black.Index;


            style5.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style5.RightBorderColor = IndexedColors.Black.Index;


            style5.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style5.TopBorderColor = IndexedColors.Black.Index;



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

            ICell cell_search = row_seach.GetCell(4);
            cell_search.SetCellValue(seach_by);

            ICellStyle style2 = workbook.CreateCellStyle();

            for (int i = 4; i <= 24; i++)
            {
             
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



            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from h in dbConnect.employee_has_sots
                     join s in dbConnect.sots on h.sot_id equals s.id
                     join t in dbConnect.type_employments on s.type_employment_id equals t.id
                     join em in dbConnect.employees on h.employee_id equals em.employee_id
                     join o in dbConnect.organizations on em.unit_id equals o.org_unit_id into joinO
                    
                     join co in dbConnect.companies on s.company_id equals co.company_id
                     join fu in dbConnect.functions on s.function_id equals fu.function_id
                     join de in dbConnect.departments on s.department_id equals de.department_id
                     join di in dbConnect.divisions on s.division_id equals di.division_id into joinDi
                     join st in dbConnect.sot_status on s.process_status equals st.id
                     
            
                     from di in joinDi.DefaultIfEmpty()
                     from o in joinO.DefaultIfEmpty()


                     where s.country == Session["country"].ToString()
                     orderby s.report_date ascending
                     select new
                     {

                         report_date = s.report_date,
                         company_name = chageDataLanguage(co.company_th, co.company_en, lang),
                         function_name = chageDataLanguage(fu.function_th, fu.function_en, lang),
                         department_name = chageDataLanguage(de.department_th, de.department_en, lang),
                         division_name = chageDataLanguage(di.division_th, di.division_en, lang),
                         
                         status = chageDataLanguage(st.name_th, st.name_en, lang),
                         company_id = s.company_id,
                         function_id = s.function_id,
                         department_id = s.department_id,
                         division_id = s.division_id,
                         sot_id = s.id,
                         s.doc_no,
                         s.sot_date,
                         s.sot_date_end,
                         s.close_sot_date,
                         s.type_employment_id,
                         s.type_work,
                         s.comment,
                         s.location,
                         s.employee_id,
                         personal_catergory = chageDataLanguage(t.name_th,t.name_en,lang),
                         sot_team_first_name = chageDataLanguage(em.first_name_th,em.first_name_en,lang),
                         sot_team_last_name = chageDataLanguage(em.last_name_th, em.last_name_en,lang),
                         em.mngt_level,
                         sotteam_employee_id = h.employee_id,
                         s.typeuser_login,
                         s.process_status,
                         prefix_sotteam = chageDataLanguage(em.prefix_th, em.prefix_en, Session["lang"].ToString()),
                         first_name_sotteam = chageDataLanguage(em.first_name_th, em.first_name_en, Session["lang"].ToString()),
                         last_name_sotteam = chageDataLanguage(em.last_name_th, em.last_name_en, Session["lang"].ToString()),

                       


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
            foreach (var rc in v1)
            {
               
                string name_surname_reporter = "";
                string company_reporter = "";
                string function_reporter = "";
                string department_reporter = "";

                if (rc.typeuser_login == "contractor")
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
                                join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                join co in dbConnect.companies on o.company_id equals co.company_id into joinCo
                                join fu in dbConnect.functions on o.function_id equals fu.function_id into joinFu
                                join de in dbConnect.departments on o.department_id equals de.department_id into joinDe

                                from co in joinCo.DefaultIfEmpty()
                                from fu in joinFu.DefaultIfEmpty()
                                from de in joinDe.DefaultIfEmpty()
                                where c.employee_id == rc.employee_id
                                select new
                                {
                                    prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                                    first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                                    last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                                    company_name = chageDataLanguage(co.company_th, co.company_en, Session["lang"].ToString()),
                                    function_name = chageDataLanguage(fu.function_th, fu.function_en, Session["lang"].ToString()),
                                    department_name = chageDataLanguage(de.department_th, de.department_en, Session["lang"].ToString()),

                                };


                        foreach (var rc2 in v)
                        {
                            name_surname_reporter = rc2.prefix + " " + rc2.first_name + " " + rc2.last_name;
                            company_reporter = rc2.company_name;
                            function_reporter = rc2.function_name;
                            department_reporter = rc2.department_name;
                        }
                    }
                    else if (Session["country"].ToString() == "srilanka")
                    {
                        var v = from c in dbConnect.employees
                                join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                join co in dbConnect.companies on o.company_id equals co.company_id into joinCo
                                join fu in dbConnect.functions on o.function_id equals fu.function_id into joinFu
                                join de in dbConnect.departments on o.sub_function_id equals de.department_id into joinDe

                                from co in joinCo.DefaultIfEmpty()
                                from fu in joinFu.DefaultIfEmpty()
                                from de in joinDe.DefaultIfEmpty()
                                where c.employee_id == rc.employee_id
                                select new
                                {
                                    prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                                    first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                                    last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                                    company_name = chageDataLanguage(co.company_th, co.company_en, Session["lang"].ToString()),
                                    function_name = chageDataLanguage(fu.function_th, fu.function_en, Session["lang"].ToString()),
                                    department_name = chageDataLanguage(de.department_th, de.department_en, Session["lang"].ToString()),

                                };


                        foreach (var rc2 in v)
                        {
                            name_surname_reporter = rc2.prefix + " " + rc2.first_name + " " + rc2.last_name;
                            company_reporter = rc2.company_name;
                            function_reporter = rc2.function_name;
                            department_reporter = rc2.department_name;
                        }

                    }
                }

                string name_surname_sotteam = rc.prefix_sotteam + " " + rc.first_name_sotteam + " " + rc.last_name_sotteam;
                string company_sotteam = "";
                string function_sotteam = "";
                string department_sotteam = "";
                string division_sotteam = "";


                if (Session["country"].ToString() == "thailand")
                {
                    var v = from c in dbConnect.employees
                            join o in dbConnect.organizations on c.unit_id equals o.org_unit_id //into joinO
                            join co in dbConnect.companies on o.company_id equals co.company_id into joinCo
                            join fu in dbConnect.functions on o.function_id equals fu.function_id into joinFu
                            join de in dbConnect.departments on o.department_id equals de.department_id into joinDe
                            join di in dbConnect.divisions on o.division_id equals di.division_id into joinDi


                            from co in joinCo.DefaultIfEmpty()
                            from fu in joinFu.DefaultIfEmpty()
                            from de in joinDe.DefaultIfEmpty()
                            from di in joinDi.DefaultIfEmpty()
                            where c.employee_id == rc.sotteam_employee_id
                            select new
                            {
                                company_name = chageDataLanguage(co.company_th, co.company_en, Session["lang"].ToString()),
                                function_name = chageDataLanguage(fu.function_th, fu.function_en, Session["lang"].ToString()),
                                department_name = chageDataLanguage(de.department_th, de.department_en, Session["lang"].ToString()),
                                division_name = chageDataLanguage(di.division_th, di.division_en, Session["lang"].ToString()),

                            };


                    foreach (var rc2 in v)
                    {
                        
                        company_sotteam = rc2.company_name;
                        function_sotteam = rc2.function_name;
                        department_sotteam = rc2.department_name;
                        division_sotteam = rc2.division_name;
                    }
                }
                else if (Session["country"].ToString() == "srilanka")
                {
                    var v = from c in dbConnect.employees
                            join o in dbConnect.organizations on c.unit_id equals o.org_unit_id //into joinO
                            join co in dbConnect.companies on o.company_id equals co.company_id into joinCo
                            join fu in dbConnect.functions on o.function_id equals fu.function_id into joinFu
                            join de in dbConnect.departments on o.sub_function_id equals de.department_id into joinDe
                            join di in dbConnect.divisions on o.department_id equals di.division_id into joinDi

                           // from o in joinO.DefaultIfEmpty()
                            from co in joinCo.DefaultIfEmpty()
                            from fu in joinFu.DefaultIfEmpty()
                            from de in joinDe.DefaultIfEmpty()
                            from di in joinDi.DefaultIfEmpty()
                            where c.employee_id == rc.sotteam_employee_id
                            select new
                            {
                                company_name = chageDataLanguage(co.company_th, co.company_en, Session["lang"].ToString()),
                                function_name = chageDataLanguage(fu.function_th, fu.function_en, Session["lang"].ToString()),
                                department_name = chageDataLanguage(de.department_th, de.department_en, Session["lang"].ToString()),
                                division_name = chageDataLanguage(di.division_th, di.division_en, Session["lang"].ToString()),

                            };


                    foreach (var rc2 in v)
                    {
                       
                        company_sotteam = rc2.company_name;
                        function_sotteam = rc2.function_name;
                        department_sotteam = rc2.department_name;
                        division_sotteam = rc2.division_name;
                    }

                }


                 string sot_time = FormatDates.getTimeShowFromDate(Convert.ToDateTime(rc.sot_date), lang);
                 string sot_time_end = FormatDates.getTimeShowFromDate(Convert.ToDateTime(rc.sot_date_end), lang);

                string name_areamanager = getAreaManager(rc.division_id, lang);
             
                string status = getStatusStepSot(rc, rc.sot_id, lang);

               


                IRow row = sheet1.CreateRow(count);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(no);
                cell.CellStyle = style;

                ICell cell1 = row.CreateCell(1);
                cell1.SetCellValue(rc.doc_no);
                cell1.CellStyle = style;


               // ICellStyle style2 = setBorder(workbook);
                ICell cell2 = row.CreateCell(2);
                cell2.SetCellValue(Convert.ToDateTime(rc.report_date));
                IDataFormat format1 = workbook.CreateDataFormat();
                style3.DataFormat = format1.GetFormat("dd/mm/yyyy h:mm:ss");
                cell2.CellStyle = style3;


              //  ICellStyle style3 = setBorder(workbook);
                ICell cell3 = row.CreateCell(3);
                cell3.SetCellValue(Convert.ToDateTime(rc.sot_date));
                IDataFormat format2 = workbook.CreateDataFormat();
                style4.DataFormat = format2.GetFormat("dd/mm/yyyy");
                cell3.CellStyle = style4;


                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(sot_time+"-"+sot_time_end);
                cell4.CellStyle = style;


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
                cell9.SetCellValue(rc.location);
                cell9.CellStyle = style;

                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(rc.type_work);
                cell10.CellStyle = style;

                ICell cell11 = row.CreateCell(11);
                cell11.SetCellValue(rc.personal_catergory);
                cell11.CellStyle = style;

                ICell cell12 = row.CreateCell(12);
                cell12.SetCellValue(rc.comment);
                cell12.CellStyle = style;

                ICell cell13 = row.CreateCell(13);
                cell13.SetCellValue(name_surname_reporter);
                cell13.CellStyle = style;

                ICell cell14 = row.CreateCell(14);
                cell14.SetCellValue(company_reporter);
                cell14.CellStyle = style;

                ICell cell15 = row.CreateCell(15);
                cell15.SetCellValue(function_reporter);
                cell15.CellStyle = style;

                ICell cell16 = row.CreateCell(16);
                cell16.SetCellValue(department_reporter);
                cell16.CellStyle = style;

           

                ICell cell17 = row.CreateCell(17);             
                cell17.SetCellValue(name_surname_sotteam);
                cell17.CellStyle = style;
                


                ICell cell18 = row.CreateCell(18);
                cell18.SetCellValue(rc.mngt_level);
                cell18.CellStyle = style;

                ICell cell19 = row.CreateCell(19);
                cell19.SetCellValue(company_sotteam);
                cell19.CellStyle = style;

                ICell cell20 = row.CreateCell(20);
                cell20.SetCellValue(function_sotteam);
                cell20.CellStyle = style;


                ICell cell21 = row.CreateCell(21);
                cell21.SetCellValue(department_sotteam);
                cell21.CellStyle = style;


                ICell cell22 = row.CreateCell(22);
                cell22.SetCellValue(name_areamanager);
                cell22.CellStyle = style;

                ICell cell23 = row.CreateCell(23);
                cell23.SetCellValue(rc.status + " " + status);
                cell23.CellStyle = style;


                ICell cell24 = row.CreateCell(24);
                if (rc.close_sot_date != null)
                {
                    //ICellStyle style7 = setBorder(workbook);
                    cell24.SetCellValue(Convert.ToDateTime(rc.close_sot_date));
                    IDataFormat format6 = workbook.CreateDataFormat();
                    style5.DataFormat = format6.GetFormat("dd/mm/yyyy h:mm");
                    cell24.CellStyle = style5;
                }
                else
                {
                    cell24.SetCellValue("");
                    cell24.CellStyle = style;
                }


                no++;
                count++;

            }


            setWidthColunm(workbook, sheet1, headers);

            //string path_write = string.Format("{0}\\report\\template\\HazardReport.xlsx", Server.MapPath(@"\"));
            string path_write = string.Format("{0}" + pathreport + "SOTReport.xlsx", Server.MapPath(@"\"));

            using (var f = File.Create(path_write))
            {
                workbook.Write(f);
            }
            Response.WriteFile(path_write);

            Response.Flush();
            Response.End();


        }

        protected string getStatusStepSot(dynamic rc, int id, string lang)
        {
            string step = "";

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                    if (rc.process_status == 1)//onprocess
                    {
                         step = step + "(SOT Report - Area Manager)";

                    }


            }//end using


            return step;
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
                    searchby = searchby + ", " + Resources.Incident.lbdivision + " :" + i.division_name;
                }
            }
            else
            {
                searchby = searchby + ", " + Resources.Incident.lbdivision + " :" + Resources.Main.all;

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