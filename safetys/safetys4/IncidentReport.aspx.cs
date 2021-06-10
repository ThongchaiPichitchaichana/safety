
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
    public partial class IncidentReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            try
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

                        LinkButton link = (LinkButton)Master.FindControl("btAllincidentReport");
                        link.Attributes.CssStyle.Add("background-color", "#e6e6e8");
                    }
                }
                else
                {
                    string original_url = Server.UrlEncode(Context.Request.RawUrl);
                    Response.Redirect("login.aspx?returnUrl=" + original_url);
                }
            }
            catch (Exception ex)
            {

                using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
                {

                    action_log objInsert = new action_log();
                    objInsert.function_name = "problem";
                    objInsert.file_name = "error";
                    objInsert.error_message = ex.ToString();
                    objInsert.created = DateTime.Now;


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                }

            }
        }

        protected void btExportExcel_Click(object sender, EventArgs e)
        {
            string filename = "IncidentReport.xlsx";
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = Encoding.Default.CodePage;
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));


            string pathreport = System.Configuration.ConfigurationManager.AppSettings["pathreport"];
            //string path = string.Format("{0}\\report\\template\\incident_report.xlsx", Server.MapPath(@"\"));
            string path = string.Format("{0}"+ pathreport +"incident_report.xlsx", Server.MapPath(@"\"));
          
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("incident_report");
            
           // sheet1.GetRow(1).GetCell(1).SetCellValue(1);
            //ICell cell = sheet1.GetRow(0).GetCell(0);
            //IRow row = sheet1.CreateRow(0);
            //ICell cell = row.CreateCell(0);
            // cell.SetCellValue(4);
            ArrayList headers = new ArrayList();

            headers.Add(Resources.Incident.sequence);
            headers.Add(Resources.Incident.doc_no);
            headers.Add(Resources.Incident.report_date);

  
            headers.Add(Resources.Incident.incidentarea);
            headers.Add(Resources.Incident.incidentname);

            headers.Add(Resources.Incident.name_injury);
            headers.Add(Resources.Incident.list_property_enviroment_damage);
            headers.Add(Resources.Incident.incident_date);
            headers.Add(Resources.Incident.incident_time);
            headers.Add(Resources.Incident.incident_type);                               
            headers.Add(Resources.Incident.severity_injury);

            headers.Add(Resources.Incident.lbfunction_injured);
            headers.Add(Resources.Incident.lbdepartment_injured);


            headers.Add(Resources.Incident.type_employment);
            
            headers.Add(Resources.Incident.lbCompany);
            headers.Add(Resources.Incident.lbfucntion);
            headers.Add(Resources.Incident.lbdepartment);
            headers.Add(Resources.Incident.lbdivision);
            headers.Add(Resources.Incident.lbsection);


            headers.Add(Resources.Incident.owner_activity);
            headers.Add(Resources.Incident.lbActivityCompany);
            headers.Add(Resources.Incident.lbActivityFunction);
            headers.Add(Resources.Incident.lbActivityDepartment);
            headers.Add(Resources.Incident.lbActivityDivision);
            headers.Add(Resources.Incident.lbActivitySection);

            int countColumn = 57;
            if (Session["country"].ToString() == "srilanka")
            {
                headers.Add(Resources.Incident.lbsite);
                countColumn++;
            }
            
            headers.Add(Resources.Incident.day_lost);
            headers.Add(Resources.Incident.nature_injury);
            headers.Add(Resources.Incident.body_parts);
            headers.Add(Resources.Incident.incidentdetail);
            headers.Add(Resources.Incident.source_incident);
            headers.Add(Resources.Incident.event_exposure);

            headers.Add(Resources.Incident.unsafe_action);
            headers.Add(Resources.Incident.unsafe_condition);
            headers.Add(Resources.Incident.root_cause_action);


            headers.Add(Resources.Incident.responsible_area);

            

            headers.Add(Resources.Incident.damage_cost);
            headers.Add(Resources.Incident.currency);
            headers.Add(Resources.Incident.impact_incident);
            
            headers.Add(Resources.Incident.corrective_preventive);
            //if (Session["country"].ToString() == "thailand")
            //{
                headers.Add(Resources.Incident.lb_preventive);
                headers.Add(Resources.Incident.lb_consequence);
             //   countColumn = countColumn + 2;
           // }
            
            headers.Add(Resources.Incident.other_impact);
            headers.Add(Resources.Incident.immediate_temporary_action);

            headers.Add(Resources.Incident.critical_incident);
            headers.Add(Resources.Incident.culpability);
            headers.Add(Resources.Incident.lbfunction_culpability);
            headers.Add(Resources.Incident.road_accident);

            headers.Add(Resources.Incident.fatality_prevention);
            headers.Add(Resources.Incident.other_please);
            headers.Add(Resources.Incident.root_cause);

            headers.Add(Resources.Incident.status);
            headers.Add(Resources.Main.lbareaohs);
            headers.Add(Resources.Main.lbareamanager);
            headers.Add(Resources.Main.lbareasupervisor);
            headers.Add(Resources.Incident.lbreasonreject);
            headers.Add(Resources.Incident.detailreject);
            headers.Add(Resources.Main.lbdateclose);
            headers.Add(Resources.Incident.type_reporter);
            headers.Add(Resources.Incident.delay_report);
            headers.Add(Resources.Incident.lbdepartment_culpability);

            if (Session["country"].ToString() == "thailand")
            {
                headers.Add(Resources.Incident.lbreasonexcept);
                headers.Add(Resources.Incident.detailexcept);
                countColumn = countColumn + 2;
            }
         
           
            setHeader(workbook,sheet1,headers);



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
            string type_area = "AREA";
            if (type_area2.Checked)
            {
                type_area = type_area2.Value;
            }



            string seach_by = searchBy(functionid, departmentid, divisionid, date_start, date_end, lang);
            IRow row_seach = sheet1.CreateRow(1);

            //ICell cell_s = row_seach.CreateCell(3);

            //cell_s.SetCellValue("Search by :");
            //cell_s.CellStyle = style;

            for (int i = 4; i <= countColumn; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                if (i == countColumn)
                {
                    style2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.RightBorderColor = IndexedColors.Black.Index;
                }

                ICell cell_n = row_seach.CreateCell(i);
                if (i == 4)
                {
                    style2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style2.LeftBorderColor = IndexedColors.Black.Index;
                    cell_n.SetCellValue(seach_by);
                }
                cell_n.CellStyle = style2;

            }


            CellRangeAddress range = new CellRangeAddress(1, 1, 4, countColumn);
            sheet1.AddMergedRegion(range);


            for (int i = 0; i <= 3; i++)
            {
                ICellStyle style2 = workbook.CreateCellStyle();

                style2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style2.TopBorderColor = IndexedColors.Black.Index;

                ICell cell_n = row_seach.CreateCell(i);
                if (i == 3)
                {
                  
                    cell_n.SetCellValue("Search by :");
                    cell_n.CellStyle = style2;
                
                }

                cell_n.CellStyle = style2;

            }

            //CellRangeAddress range2 = new CellRangeAddress(1, 1, 0, 3);
            //sheet1.AddMergedRegion(range2);
            

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v1 = from ip in dbConnect.injury_persons
                     //  where c.status == status.Trim()
                     join id in dbConnect.incidents on ip.incident_id equals id.id
                     join s in dbConnect.severity_injuries on ip.severity_injury_id equals s.id into joinS
                     join t in dbConnect.type_employments on ip.type_employment_id equals t.id into joinT
                     join n in dbConnect.nature_injuries on ip.nature_injury_id equals n.id into joinN
                     join b in dbConnect.body_parts on ip.body_parts_id equals b.id into joinB
                     join f in dbConnect.fatality_prevention_elements on id.fatality_prevention_element_id equals f.id into joinF
                    // join co in dbConnect.companies on  id.company_id equals co.company_id
                    // join fu in dbConnect.functions on id.function_id equals fu.function_id
                    // join de in dbConnect.departments on id.department_id equals de.department_id
                     //join di in dbConnect.divisions on id.division_id equals di.division_id
                     join st in dbConnect.incident_status on id.process_status equals st.id
                     join rs in dbConnect.reason_rejects on id.reason_reject_type equals rs.id into joinReason
                     join re in dbConnect.reason_excepts on id.reason_except_type equals re.id into joinReasonEx

                     from s in joinS.DefaultIfEmpty()
                     from t in joinT.DefaultIfEmpty()
                     from n in joinN.DefaultIfEmpty()
                     from b in joinB.DefaultIfEmpty()
                     from f in joinF.DefaultIfEmpty()
                     from rs in joinReason.DefaultIfEmpty()
                     from re in joinReasonEx.DefaultIfEmpty()
                     where id.injury_fatality_involve == "Y" && ip.status == "A"
                     orderby id.incident_date ascending
                     select new
                     {
                         //ip.id,
                         report_date = id.report_date,
                         name = ip.full_name,
                         incident_datetime = id.incident_date,
                         severity_injury_th = s.name_th,
                         severity_injury_en = s.name_en,
                         type_employment_th = t.name_th,
                         type_employment_en = t.name_en,
                         company_name = chageDataLanguage(id.location_company_name_th, id.location_company_name_en, lang),
                         function_name = chageDataLanguage(id.location_function_name_th, id.location_function_name_en, lang),
                         department_name = chageDataLanguage(id.location_department_name_th, id.location_department_name_en, lang),
                         division_name = chageDataLanguage(id.location_division_name_th, id.location_division_name_en, lang),
                         section_name = chageDataLanguage(id.location_section_name_th, id.location_section_name_en, lang),
                         day_lost = ip.day_lost == null ? "" : ip.day_lost.ToString(),
                         nature_injury_th = n.name_th,
                         nature_injury_en = n.name_en,
                         body_part_th = b.name_th,
                         body_part_en = b.name_en,
                         incident_detail = id.incident_detail,
                         responsible_area = id.responsible_area,
                         fatality_prevention = chageDataLanguage(f.name_th, f.name_en, lang),
                         critical = id.critical,
                         status = chageDataLanguage(st.name_th, st.name_en, lang),
                         company_id = id.company_id,
                         function_id = id.function_id,
                         department_id = id.department_id,
                         division_id = id.division_id,
                         section_id = id.section_id,
                         id.activity_company_id,
                         id.activity_function_id,
                         id.activity_department_id,
                         id.activity_division_id,
                         id.activity_section_id,
                         incident_id = id.id,
                         // damage_cost = "",
                         incident_type = "Injury or fatality",
                         id.doc_no,
                         id.culpability,
                         id.road_accident,
                         id.currency,
                         id.reason_reject,
                         reason_reject_type = chageDataLanguage(rs.name_th, rs.name_en, lang),
                         id.faltality_prevention_element_other,
                         //damage_list = ""
                         id.impact,
                         function_culpability = id.form2_function_id,
                         department_culpability = id.form3_department_id,
                         id.step_form,
                         id.process_status,
                         id.submit_report_form2,
                         id.confirm_investigate_form2,
                         id.id,
                         id.country,
                         id.close_incident_date,
                         id.typeuser_login,
                         property_environment_damage = "",
                         damage_cost = "",
                         damage_list = "",


                         id.incident_area,
                         id.incident_name,
                         id.owner_activity,
                         activity_company_name = chageDataLanguage(id.activity_location_company_name_th, id.activity_location_company_name_en, lang),
                         activity_function_name = chageDataLanguage(id.activity_location_function_name_th, id.activity_location_function_name_en, lang),
                         activity_department_name = chageDataLanguage(id.activity_location_department_name_th, id.activity_location_department_name_en, lang),
                         activity_division_name = chageDataLanguage(id.activity_location_division_name_th, id.activity_location_division_name_en, lang),
                         activity_section_name = chageDataLanguage(id.activity_location_section_name_th, id.activity_location_section_name_en, lang),

                         function_injured_id = ip.function_id,
                         department_injured_id = ip.department_id,

                         id.immediate_temporary,
                         id.reason_except,
                         reason_except_type = chageDataLanguage(re.name_th, re.name_en, lang),



                     };


            if (type_area == "AREA")
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

                v1 = v1.Where(c => c.owner_activity == "KNOWN");

            }

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v1 = v1.Where(c => c.incident_datetime >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v1 = v1.Where(c => c.incident_datetime <= d_end);
            }

            v1 = v1.Where(c => c.country == Session["country"].ToString());




            var v2 = from d in dbConnect.damage_lists
                     join id in dbConnect.incidents on d.incident_id equals id.id
                     join f in dbConnect.fatality_prevention_elements on id.fatality_prevention_element_id equals f.id into joinF
                    // join co in dbConnect.companies on id.company_id equals co.company_id                
                    // join fu in dbConnect.functions on id.function_id equals fu.function_id
                     //join de in dbConnect.departments on id.department_id equals de.department_id
                     //join di in dbConnect.divisions on id.division_id equals di.division_id
                     join st in dbConnect.incident_status on id.process_status equals st.id
                     join rs in dbConnect.reason_rejects on id.reason_reject_type equals rs.id into joinReason
                     join re in dbConnect.reason_excepts on id.reason_except_type equals re.id into joinReasonEx

                     from rs in joinReason.DefaultIfEmpty()
                     from f in joinF.DefaultIfEmpty()
                     from re in joinReasonEx.DefaultIfEmpty()
                     where id.effect_environment == "Y" && d.status == "A"
                     orderby id.incident_date ascending
                     select new
                     {
                         report_date = id.report_date,
                         name = "-",//dm.property_environment_damage,
                         incident_datetime = id.incident_date,
                         severity_injury_th = "",
                         severity_injury_en = "",
                         type_employment_th = "",
                         type_employment_en = "",
                         company_name = chageDataLanguage(id.location_company_name_th, id.location_company_name_en, lang),
                         function_name = chageDataLanguage(id.location_function_name_th, id.location_function_name_en, lang),
                         department_name = chageDataLanguage(id.location_department_name_th, id.location_department_name_en, lang),
                         division_name = chageDataLanguage(id.location_division_name_th, id.location_division_name_en, lang),
                         section_name = chageDataLanguage(id.location_section_name_th, id.location_section_name_en, lang),
                         day_lost = "",
                         nature_injury_th = "",
                         nature_injury_en = "",
                         body_part_th = "",
                         body_part_en = "",
                         incident_detail = id.incident_detail,
                         responsible_area = id.responsible_area,
                         fatality_prevention = chageDataLanguage(f.name_th, f.name_en, lang),
                         critical = id.critical,
                         status = chageDataLanguage(st.name_th, st.name_en, lang),
                         company_id = id.company_id,
                         function_id = id.function_id,
                         department_id = id.department_id,
                         division_id = id.division_id,
                         section_id = id.section_id,
                         id.activity_company_id,
                         id.activity_function_id,
                         id.activity_department_id,
                         id.activity_division_id,
                         id.activity_section_id,
                         incident_id = id.id,
                         // damage_cost = getDamagecost(id.id),
                         incident_type = "damage",
                         id.doc_no,
                         id.culpability,
                         id.road_accident,
                         id.currency,
                         id.reason_reject,
                         reason_reject_type = chageDataLanguage(rs.name_th, rs.name_en, lang),
                         id.faltality_prevention_element_other,
                         //damage_list = getDamageList(id.id)
                         id.impact,
                         function_culpability = id.form2_function_id,
                         department_culpability = id.form3_department_id,
                         id.step_form,
                         id.process_status,
                         id.submit_report_form2,
                         id.confirm_investigate_form2,
                         id.id,
                         id.country,
                         id.close_incident_date,
                         id.typeuser_login,
                         d.property_environment_damage,
                         damage_cost = d.damage_cost == null ? "" : d.damage_cost.ToString(),
                         damage_list = d.detail_damage,

                         id.incident_area,
                         id.incident_name,
                         id.owner_activity,
                         activity_company_name = chageDataLanguage(id.activity_location_company_name_th, id.activity_location_company_name_en, lang),
                         activity_function_name = chageDataLanguage(id.activity_location_function_name_th, id.activity_location_function_name_en, lang),
                         activity_department_name = chageDataLanguage(id.activity_location_department_name_th, id.activity_location_department_name_en, lang),
                         activity_division_name = chageDataLanguage(id.activity_location_division_name_th, id.activity_location_division_name_en, lang),
                         activity_section_name = chageDataLanguage(id.activity_location_section_name_th, id.activity_location_section_name_en, lang),

                         function_injured_id = "",
                         department_injured_id = "",

                         id.immediate_temporary,
                         id.reason_except,
                         reason_except_type = chageDataLanguage(re.name_th, re.name_en, lang),

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

                v2 = v2.Where(c => c.owner_activity == "KNOWN");

            }

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v2 = v2.Where(c => c.incident_datetime >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v2 = v2.Where(c => c.incident_datetime <= d_end);
            }


            v2 = v2.Where(c => c.country == Session["country"].ToString());



            var v3 = from id in dbConnect.incidents
                     join f in dbConnect.fatality_prevention_elements on id.fatality_prevention_element_id equals f.id into joinF
                    // join co in dbConnect.companies on id.company_id equals co.company_id
                    // join fu in dbConnect.functions on id.function_id equals fu.function_id
                    // join de in dbConnect.departments on id.department_id equals de.department_id
                   //  join di in dbConnect.divisions on id.division_id equals di.division_id
                     join st in dbConnect.incident_status on id.process_status equals st.id
                     join rs in dbConnect.reason_rejects on id.reason_reject_type equals rs.id into joinReason
                     join re in dbConnect.reason_excepts on id.reason_except_type equals re.id into joinReasonEx

                     from f in joinF.DefaultIfEmpty()
                     from rs in joinReason.DefaultIfEmpty()
                     from re in joinReasonEx.DefaultIfEmpty()
                     where (id.injury_fatality_involve == null && id.effect_environment == null)
                     || (id.injury_fatality_involve == "N" && id.effect_environment == "N")
                     || (id.injury_fatality_involve == "" && id.effect_environment == "")
                     || (id.injury_fatality_involve == "N" && id.effect_environment == "")
                     || (id.injury_fatality_involve == "" && id.effect_environment == "N")
                     orderby id.incident_date ascending
                     select new
                     {
                         report_date = id.report_date,
                         name = "-",//dm.property_environment_damage,
                         incident_datetime = id.incident_date,
                         severity_injury_th = "",
                         severity_injury_en = "",
                         type_employment_th = "",
                         type_employment_en = "",
                         company_name = chageDataLanguage(id.location_company_name_th, id.location_company_name_en, lang),
                         function_name = chageDataLanguage(id.location_function_name_th, id.location_function_name_en, lang),
                         department_name = chageDataLanguage(id.location_department_name_th, id.location_department_name_en, lang),
                         division_name = chageDataLanguage(id.location_division_name_th,id.location_division_name_en,lang),
                         section_name = chageDataLanguage(id.location_section_name_th, id.location_section_name_en, lang),
                         day_lost = "",
                         nature_injury_th = "",
                         nature_injury_en = "",
                         body_part_th = "",
                         body_part_en = "",
                         incident_detail = id.incident_detail,
                         responsible_area = id.responsible_area,
                         fatality_prevention = chageDataLanguage(f.name_th, f.name_en, lang),
                         critical = id.critical,
                         status = chageDataLanguage(st.name_th, st.name_en, lang),
                         company_id = id.company_id,
                         function_id = id.function_id,
                         department_id = id.department_id,
                         division_id = id.division_id,
                         section_id = id.section_id,
                         id.activity_company_id,
                         id.activity_function_id,
                         id.activity_department_id,
                         id.activity_division_id,
                         id.activity_section_id,
                         incident_id = id.id,
                         // damage_cost = "",
                         incident_type = "",
                         id.doc_no,
                         id.culpability,
                         id.road_accident,
                         id.currency,
                         id.reason_reject,
                         reason_reject_type = chageDataLanguage(rs.name_th, rs.name_en, lang),
                         id.faltality_prevention_element_other,
                         //damage_list = ""
                         id.impact,
                         function_culpability = id.form2_function_id,
                         department_culpability = id.form3_department_id,
                         id.step_form,
                         id.process_status,
                         id.submit_report_form2,
                         id.confirm_investigate_form2,
                         id.id,
                         id.country,
                         id.close_incident_date,
                         id.typeuser_login,
                         property_environment_damage = "",
                         damage_cost = "",
                         damage_list = "",

                         id.incident_area,
                         id.incident_name,
                         id.owner_activity,
                         activity_company_name = chageDataLanguage(id.activity_location_company_name_th, id.activity_location_company_name_en, lang),
                         activity_function_name = chageDataLanguage(id.activity_location_function_name_th, id.activity_location_function_name_en, lang),
                         activity_department_name = chageDataLanguage(id.activity_location_department_name_th, id.activity_location_department_name_en, lang),
                         activity_division_name = chageDataLanguage(id.activity_location_division_name_th, id.activity_location_division_name_en, lang),
                         activity_section_name = chageDataLanguage(id.activity_location_section_name_th, id.activity_location_section_name_en, lang),

                         function_injured_id = "",
                         department_injured_id = "",

                         id.immediate_temporary,
                         id.reason_except,
                         reason_except_type = chageDataLanguage(re.name_th, re.name_en, lang),
                         


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

                v3 = v3.Where(c => c.owner_activity == "KNOWN");

            }

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                v3 = v3.Where(c => c.incident_datetime >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v3 = v3.Where(c => c.incident_datetime <= d_end);
            }

            v3 = v3.Where(c => c.country == Session["country"].ToString());


            var v = v1.Concat(v2);
            var newV = v.Concat(v3).OrderBy(c => c.incident_datetime);

            ArrayList dataJson = new ArrayList();

            ICellStyle style3 = setBorder(workbook);
            ICellStyle style4 = setBorder(workbook);
            ICellStyle style5 = setBorder(workbook);
            ICellStyle style6 = setBorder(workbook);

            int no = 1;
            int count = 3;
            foreach (var rc in newV)
            {
                //string incident_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.incident_datetime), lang);
                //string incident_time = FormatDates.getTimeShowFromDate(Convert.ToDateTime(rc.incident_datetime), lang);
                //string report_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.report_date), lang);


                string status_delay = "";
                int count_date = 0;


                if (Session["country"].ToString() == "thailand")
                {
                    count_date = Convert.ToDateTime(rc.report_date).Subtract(Convert.ToDateTime(rc.incident_datetime)).Hours;

                    if (count_date > 24)//24 hour
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
                    count_date = Convert.ToDateTime(rc.report_date).Subtract(Convert.ToDateTime(rc.incident_datetime)).Days;

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

                string close_date = "";
                if (rc.close_incident_date!=null)
                {
                    close_date =FormatDates.getDatetimeShow(Convert.ToDateTime(rc.close_incident_date), lang);

                }
                string source_incident = getSourceIncident(rc.incident_id, lang);
                string event_incident = getEventExposure(rc.incident_id, lang);
                string unsafe_action = getUnsafeAction(rc.incident_id, lang);
                string unsafe_condition = getUnsafeCondition(rc.incident_id, lang);

                string root_cause_action = getRootCauseAction(rc.incident_id);

                string serverity_injury_name = chageDataLanguage(rc.severity_injury_th, rc.severity_injury_en, lang);
                string type_employment_name = chageDataLanguage(rc.type_employment_th, rc.type_employment_en, lang);
                string nature_injury_name = chageDataLanguage(rc.nature_injury_th, rc.nature_injury_en, lang);
                string body_part_name = chageDataLanguage(rc.body_part_th, rc.body_part_en, lang);

                string site = getSite(rc.id.ToString(), Session["country"].ToString());
 
                string resposible_area = "";
                if (rc.responsible_area == "OUT")
                {
                    resposible_area = chageDataLanguage("เกิดนอกพื้นที่ควบคุมกลุ่มบริษัทฯ", "Offsite", lang);
                }
                else if(rc.responsible_area =="IN")
                {
                    resposible_area = chageDataLanguage("เกิดในพื้นที่ควบคุมกลุ่มบริษัทฯ", "Onsite", lang);
                }




                string owner_activity = "";
                if (rc.owner_activity == "KNOWN")
                {
                    owner_activity = chageDataLanguage("ทราบ", "Known", lang);
                }
                else if (rc.owner_activity == "UNKNOWN")
                {
                    owner_activity = chageDataLanguage("ไม่ทราบ", "Unknown", lang);
                }
       

                string root_cause = getPyramid(rc.incident_id);
                string other_impacts = getOtherimpact(rc.incident_id, lang);
                string name_areaohs = getAreaOHS(rc.department_id, lang);
                string name_areamanager = getAreaManager(rc.division_id, lang);
                string name_areasupervisor = getAreaSupervisor(rc.section_id, lang);


                string critical = "";
                if (rc.critical == "Y")
                {
                    critical = chageDataLanguage("ใช่", "Yes", lang);
                }
                else if (rc.critical == "N")
                {
                    critical = chageDataLanguage("ไม่ใช่", "No", lang);
                }

                string culpability = "";

                if (rc.culpability == "G")
                {
                    culpability = Resources.Incident.guilty;

                }else if(rc.culpability == "P"){

                    culpability = Resources.Incident.partial;
                }
                else if (rc.culpability == "N")
                {
                    culpability = Resources.Incident.no_guilty;

                }


                string road_incident = "";

                if (rc.road_accident== "Y")
                {
                    road_incident = Resources.Incident.yes;

                }
                else if (rc.road_accident == "N")
                {

                    road_incident = Resources.Incident.no;
                }
               // string damage_cost = getDamageCost(rc.incident_id);
                string corrective_preventive = getAction(rc.incident_id,lang);
                string preventive = getPreventiveAction(rc.incident_id, lang);
                string consequence = getConsequenceAction(rc.incident_id, lang);

                string currency = "";

                if(!string.IsNullOrEmpty(rc.currency))
                {
                     var cu = from c in dbConnect.currencies
                         where c.id ==  Convert.ToInt16(rc.currency)
                         select c;
                    foreach (var r in cu)
                    {
                        currency = chageDataLanguage(r.name_th, r.name_en, lang);
                    }
                }

                string status = getStatusStep(rc, rc.id, lang);
                

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
                cell3.SetCellValue(rc.incident_area);
                cell3.CellStyle = style;

                ICell cell4 = row.CreateCell(4);
                cell4.SetCellValue(rc.incident_name);
                cell4.CellStyle = style;

                ICell cell5 = row.CreateCell(5);
                cell5.SetCellValue(rc.name);
                cell5.CellStyle = style;

                ICell cell6 = row.CreateCell(6);
                cell6.SetCellValue(rc.property_environment_damage);
                cell6.CellStyle = style;
            

                //if (rc.incident_type == "damage")
                //{
                //    string damage_list = getDamageList(rc.incident_id);
                //    cell4.SetCellValue(damage_list);
                //    cell4.CellStyle = style;
                //}
                //else
                //{
                //    cell4.SetCellValue("");
                //    cell4.CellStyle = style;

                //}



                
                ICell cell7 = row.CreateCell(7);
                cell7.SetCellValue(Convert.ToDateTime(rc.incident_datetime));
                IDataFormat format2 = workbook.CreateDataFormat();
                style4.DataFormat = format2.GetFormat("dd/mm/yyyy");
                cell7.CellStyle = style4;


               
                ICell cell8 = row.CreateCell(8);
                cell8.SetCellValue(Convert.ToDateTime(rc.incident_datetime));
                IDataFormat format3 = workbook.CreateDataFormat();
                style5.DataFormat = format3.GetFormat("h:mm");
                cell8.CellStyle = style5;



                ICell cell9 = row.CreateCell(9);
                cell9.SetCellValue(rc.incident_type);
                cell9.CellStyle = style;


                ICell cell10 = row.CreateCell(10);
                cell10.SetCellValue(serverity_injury_name);
                cell10.CellStyle = style;

                string function_injured = getFunctionByForm3(rc.function_injured_id, lang);
                ICell cell11 = row.CreateCell(11);
                cell11.SetCellValue(function_injured);
                cell11.CellStyle = style;

                string department_injured = getDepartmentByForm3(rc.department_injured_id, lang);
                ICell cell12 = row.CreateCell(12);
                cell12.SetCellValue(department_injured);
                cell12.CellStyle = style;







                ICell cell13 = row.CreateCell(13);
                cell13.SetCellValue(type_employment_name);
                cell13.CellStyle = style;

                ICell cell14 = row.CreateCell(14);
                cell14.SetCellValue(rc.company_name);
                cell14.CellStyle = style;

                ICell cell15 = row.CreateCell(15);
                cell15.SetCellValue(rc.function_name);
                cell15.CellStyle = style;

                ICell cell16 = row.CreateCell(16);
                cell16.SetCellValue(rc.department_name);
                cell16.CellStyle = style;

                ICell cell17 = row.CreateCell(17);
                cell17.SetCellValue(rc.division_name);
                cell17.CellStyle = style;

                ICell cell18 = row.CreateCell(18);
                cell18.SetCellValue(rc.section_name);
                cell18.CellStyle = style;


                ICell cell19 = row.CreateCell(19);
                cell19.SetCellValue(owner_activity);
                cell19.CellStyle = style;

                ICell cell20 = row.CreateCell(20);
                cell20.SetCellValue(rc.activity_company_name);
                cell20.CellStyle = style;

                ICell cell21 = row.CreateCell(21);
                cell21.SetCellValue(rc.activity_function_name);
                cell21.CellStyle = style;

                ICell cell22 = row.CreateCell(22);
                cell22.SetCellValue(rc.activity_department_name);
                cell22.CellStyle = style;

                ICell cell23 = row.CreateCell(23);
                cell23.SetCellValue(rc.activity_division_name);
                cell23.CellStyle = style;

                ICell cell24 = row.CreateCell(24);
                cell24.SetCellValue(rc.activity_section_name);
                cell24.CellStyle = style;


                int cell_site = 1;

                if (Session["country"].ToString() == "srilanka")
                {
                    ICell cell25 = row.CreateCell(25);
                    cell25.SetCellValue(site);
                    cell25.CellStyle = style;
                    cell_site = 0;
                }


                ICell cell26 = row.CreateCell(26 - cell_site);
                cell26.SetCellValue(rc.day_lost);
                cell26.CellStyle = style;

                ICell cell27 = row.CreateCell(27 - cell_site);
                cell27.SetCellValue(nature_injury_name);
                cell27.CellStyle = style;

                ICell cell28 = row.CreateCell(28 - cell_site);
                cell28.SetCellValue(body_part_name);
                cell28.CellStyle = style;

                ICell cell29 = row.CreateCell(29 - cell_site);
                cell29.SetCellValue(rc.incident_detail);
                cell29.CellStyle = style;


                ICell cell30 = row.CreateCell(30 - cell_site);
                cell30.SetCellValue(source_incident);
                cell30.CellStyle = style;

                ICell cell31 = row.CreateCell(31 - cell_site);
                cell31.SetCellValue(event_incident);
                cell31.CellStyle = style;


                ICell cell32 = row.CreateCell(32 - cell_site);
                cell32.SetCellValue(unsafe_action);
                cell32.CellStyle = style;

                ICell cell33 = row.CreateCell(33 - cell_site);
                cell33.SetCellValue(unsafe_condition);
                cell33.CellStyle = style;

                ICell cell34 = row.CreateCell(34 - cell_site);
                cell34.SetCellValue(root_cause_action);
                cell34.CellStyle = style;


                ICell cell35 = row.CreateCell(35 - cell_site);
                cell35.SetCellValue(resposible_area);
                cell35.CellStyle = style;

                IDataFormat format = workbook.CreateDataFormat();
                ICell cell36 = row.CreateCell(36 - cell_site);

                double damage_cost_number = 0;
                if (!string.IsNullOrEmpty(rc.damage_cost))
                {
                    damage_cost_number = Convert.ToDouble(rc.damage_cost);
                }
                string cost = String.Format("{0:n}", damage_cost_number);
                cell36.SetCellValue(cost);
                cell36.CellStyle = style;



                ICell cell37 = row.CreateCell(37 - cell_site);
                cell37.SetCellValue(currency);
                cell37.CellStyle = style;


                string impact = "";
                if (rc.impact == "Y")
                {
                    impact = Resources.Incident.impact;
                }
                else if(rc.impact=="N")
                {
                    impact = Resources.Incident.no_impact;
                }
                ICell cell38 = row.CreateCell(38 - cell_site);
                cell38.SetCellValue(impact);
                cell38.CellStyle = style;




                ICell cell39 = row.CreateCell(39 - cell_site);
                cell39.SetCellValue(corrective_preventive);
                cell39.CellStyle = style;

          
                ICell cell40 = row.CreateCell(40 - cell_site);
                cell40.SetCellValue(preventive);
                cell40.CellStyle = style;

                ICell cell41 = row.CreateCell(41 - cell_site);
                cell41.SetCellValue(consequence);
                cell41.CellStyle = style;
                


                ICell cell42 = row.CreateCell(42 - cell_site);
                cell42.SetCellValue(other_impacts);
                cell42.CellStyle = style;

                ICell cell43 = row.CreateCell(43 - cell_site);
                cell43.SetCellValue(rc.immediate_temporary);
                cell43.CellStyle = style;




                ICell cell44 = row.CreateCell(44 - cell_site);
                cell44.SetCellValue(critical);
                cell44.CellStyle = style;

                ICell cell45 = row.CreateCell(45 - cell_site);
                cell45.SetCellValue(culpability);
                cell45.CellStyle = style;


                string function_cul = getFunctionByForm3(rc.function_culpability, lang);
                ICell cell46 = row.CreateCell(46 - cell_site);
                cell46.SetCellValue(function_cul);
                cell46.CellStyle = style;


                ICell cell47 = row.CreateCell(47 - cell_site);
                cell47.SetCellValue(road_incident);
                cell47.CellStyle = style;

                ICell cell48 = row.CreateCell(48 - cell_site);
                cell48.SetCellValue(rc.fatality_prevention);
                cell48.CellStyle = style;



                ICell cell49 = row.CreateCell(49 - cell_site);
                cell49.SetCellValue(rc.faltality_prevention_element_other);
                cell49.CellStyle = style;


                ICell cell50 = row.CreateCell(50 - cell_site);
                cell50.SetCellValue(root_cause);
                cell50.CellStyle = style;


                ICell cell51 = row.CreateCell(51 - cell_site);
                cell51.SetCellValue(rc.status + " " + status);
                cell51.CellStyle = style;

                ICell cell52 = row.CreateCell(52 - cell_site);
                cell52.SetCellValue(name_areaohs);
                cell52.CellStyle = style;

                ICell cell53 = row.CreateCell(53 - cell_site);
                cell53.SetCellValue(name_areamanager);
                cell53.CellStyle = style;

                ICell cell54 = row.CreateCell(54 - cell_site);
                cell54.SetCellValue(name_areasupervisor);
                cell54.CellStyle = style;

                ICell cell55 = row.CreateCell(55 - cell_site);
                cell55.SetCellValue(rc.reason_reject_type);
                cell55.CellStyle = style;

                ICell cell56 = row.CreateCell(56 - cell_site);
                cell56.SetCellValue(rc.reason_reject);
                cell56.CellStyle = style;


                if (rc.close_incident_date != null)
                {

                    ICell cell57 = row.CreateCell(57 - cell_site);
                    cell57.SetCellValue(Convert.ToDateTime(rc.close_incident_date));
                    IDataFormat format4 = workbook.CreateDataFormat();
                    style6.DataFormat = format4.GetFormat("dd/mm/yyyy");
                    cell57.CellStyle = style6;
                }
                else
                {
                    ICell cell57 = row.CreateCell(57 - cell_site);
                    cell57.SetCellValue("");
                    cell57.CellStyle = style;
                }




                ICell cell58= row.CreateCell(58 - cell_site);
                cell58.SetCellValue(type_reporter);
                cell58.CellStyle = style;

                ICell cell59 = row.CreateCell(59 - cell_site);
                cell59.SetCellValue(status_delay);
                cell59.CellStyle = style;


                string department_cul = getDepartmentByForm3(rc.department_culpability, lang);
                ICell cell60 = row.CreateCell(60 - cell_site);
                cell60.SetCellValue(department_cul);
                cell60.CellStyle = style;


                if (Session["country"].ToString() == "thailand")
                {
                    ICell cell61 = row.CreateCell(61 - cell_site);
                    cell61.SetCellValue(rc.reason_except_type);
                    cell61.CellStyle = style;


                    ICell cell62 = row.CreateCell(62 - cell_site);
                    cell62.SetCellValue(rc.reason_except);
                    cell62.CellStyle = style;
                }

          
             
                
                no++;
                count++;

            }


            setWidthColunm(workbook, sheet1, headers);

           //string path_write = string.Format("{0}\\report\\template\\IncidentReport.xlsx", Server.MapPath(@"\"));
           string path_write = string.Format("{0}"+ pathreport +"IncidentReport.xlsx", Server.MapPath(@"\"));

            using (var f = File.Create(path_write))
            {
                workbook.Write(f);
            }
            Response.WriteFile(path_write);
            
            Response.Flush();
            Response.End();
           
          
        }
        public string  getSite(string incident_id, string country)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            string site = "";
            var n = from c in dbConnect.incidents
                    where c.id == Convert.ToInt32(incident_id)
                    select new
                    {
                        c.id,
                        c.section_id,
                        c.division_id,
                        c.department_id,
                        c.function_id
                    };

            


            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        site = rc.name;
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            site = rc.name;
                        }
                    }
                    else
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
                                site = rc.name;
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
                                    site = rc.name;
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return site;

        }
        protected string getStatusStep(dynamic rc, int id, string lang)
        {
            string step = "";

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {


                if (rc.process_status != 2 && rc.process_status != 3)//ไม่ใช้ close กับ reject
                {

                    if (rc.step_form == 1)//supervisor
                    {
                        string v_step = chageDataLanguage("รายงานอุบัติการณ์", "Incident Report", lang);
                        step = step + "(" + v_step + " - Area Supervisor)";

                    }
                    else if (rc.step_form == 2)
                    {
                        string v_step = chageDataLanguage("ตรวจสอบรายงานอุบัติการณ์", "Vetify Incident Report", lang);

                        if (rc.submit_report_form2 == null)
                        {
                            step = step + "(" + v_step + " - Area Supervisor)";
                        }


                        if (rc.submit_report_form2 != null && rc.confirm_investigate_form2 == null)
                        {
                            step = step + "(" + v_step + " - Area OH&S)";
                        }


                        if (rc.confirm_investigate_form2 != null)//กด confirm ไปละ แต่ยังอยู่ 2 แสดงว่ามีซีเรียสเคสรอ group มากด confirm
                        {
                            step = step + "(" + v_step + " - Group OH&S)";
                        }


                    }
                    else if (rc.step_form == 3)
                    {
                        string v_step = chageDataLanguage("สอบสวนและกำหนดมาตรการการแก้ไข", "Investigation and Corrective/Preventive Action", lang);

                        step = step + "(" + v_step + " - Area OH&S)";

                    }
                    else if (rc.step_form == 4)
                    {
                        string v_step = chageDataLanguage("ขอปิดรายงานอุบัติการณ์", "Request to Close Incident Report", lang);
                        bool close_manager = false;
                        bool close_admin = false;
                        bool close_group = false;

                        var w = from c in dbConnect.log_request_close_incidents
                                where c.incident_id == id && c.status == "A"
                                orderby c.created_at descending
                                select new
                                {
                                    id = c.id,
                                    c.employee_id,
                                    c.status,
                                    c.group_id

                                };

                        if (w.Count() > 0)
                        {
                            foreach (var k in w)
                            {
                                if (k.group_id == 10)// areamanage
                                {
                                    close_manager = true;
                                }

                                if (k.group_id == 4 || k.group_id == 5)// admin and delegate
                                {
                                    close_admin = true;
                                }

                                if (k.group_id == 8)
                                {
                                    close_group = true;
                                }

                            }

                            if (close_manager == true && close_admin == false && close_group == false)
                            {
                                step = step + "(" + v_step + " - Admin OH&S or Delegate OH&S Admin)";
                            }

                            if (close_admin == true && close_group == false)
                            {
                                step = step + "(" + v_step + " - Group OH&S)";
                            }

                            if (close_group)
                            {
                                step = "";
                            }
                        }
                        else
                        {
                            step = step + "(" + v_step + " - Area Manager)";

                        }



                    }

                }

            }//end using


            return step;
        }

        protected string getFunctionByForm3(string id,string lang)
        {
            string value = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.functions
                    where c.function_id == id
                    select c;

            foreach (var rc in v)
            {
                value = chageDataLanguage(rc.function_th, rc.function_en, lang);
            }


            return value;
        }


        protected string getDepartmentByForm3(string id, string lang)
        {
            string value = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.departments
                    where c.department_id == id
                    select c;

            foreach (var rc in v)
            {
                value = chageDataLanguage(rc.department_th, rc.department_en, lang);
            }


            return value;
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
                        first_name = chageDataLanguage(e.first_name_th,e.first_name_en,lang),
                        last_name = chageDataLanguage(e.last_name_th,e.last_name_en,lang),

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


        protected void setHeader(XSSFWorkbook workbook, ISheet sheet,ArrayList headers)
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

        protected void setWidthColunm(XSSFWorkbook workbook, ISheet sheet,ArrayList headers)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                sheet.AutoSizeColumn(i);

            }

        }

        protected string getDamagecost(int incident_id)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.damage_lists
                    where c.incident_id == incident_id
                    select c;

            foreach (var rc in v)
            {
                if (rc.damage_cost != null)
                {
                    value = value + Convert.ToDouble(rc.damage_cost);
                }
            }


            return value.ToString();
        }


        protected string getDamageList(int incident_id)
        {
            string so = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.damage_lists
                    where c.incident_id == incident_id
                    select c;

            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(so))
                {

                    so = rc.property_environment_damage;
                }
                else
                {

                    so = so + ", " + rc.property_environment_damage;
                }
            }


            return so;
        }

        protected string getOtherimpact(int incident_id, string lang)
        {
            string value = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.other_impacts
                    where c.incident_id == incident_id
                    select c;
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(value))
                {
                    string va = "";
                    if (rc.other_impact_value == "image")
                    {
                        va = chageDataLanguage("อาจจะมีผลต่อภาพลักษณ์", "Have the potential to image", lang);
                    }
                    else if (rc.other_impact_value == "legal")
                    {
                        va = chageDataLanguage(" อาจจะมีผลกระทบทางคดีความกฎหมาย", "Have the potential to legal ", lang);
                    }
                    else if (rc.other_impact_value == "manufacturing")
                    {
                        va = chageDataLanguage("อาจจะมีผลต่อกระบวนการผลิต", "Have the potential to productivity issue", lang);
                    }
                    value = va;
                }
                else
                {

                    string va = "";
                    if (rc.other_impact_value == "image")
                    {
                        va = chageDataLanguage("อาจจะมีผลต่อภาพลักษณ์", "Have the potential to image", lang);
                    }
                    else if (rc.other_impact_value == "legal")
                    {
                        va = chageDataLanguage("อาจจะมีผลกระทบทางคดีความกฎหมาย", "Have the potential to legal", lang);
                    }
                    else if (rc.other_impact_value == "manufacturing")
                    {
                        va = chageDataLanguage("อาจจะมีผลต่อกระบวนการผลิต", "Have the potential to productivity issue", lang);
                    }
                    value = value + ", " + va;
                }
            }



            return value;

        }


        protected string getSourceIncident(int incident_id, string lang)
        {
            string so = "";
            //string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.fact_findings
                    join s in dbConnect.source_incidents on c.source_incident_id equals s.id
                    // join e in dbConnect.event_exposures on c.event_exposure_id equals e.id
                    where c.incident_id == incident_id
                    select new
                    {
                        source_incident = chageDataLanguage(s.name_th, s.name_en, lang),
                        // event_exposure = chageDataLanguage(s.name_th,s.name_en,lang)

                    };
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(so))
                {

                    so = rc.source_incident;
                }
                else
                {

                    so = so + ", " + rc.source_incident;
                }
            }



            return so;

        }

        protected string getEventExposure(int incident_id, string lang)
        {
            // string so = "";
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.fact_findings

                    join e in dbConnect.event_exposures on c.event_exposure_id equals e.id
                    where c.incident_id == incident_id
                    select new
                    {
                        event_exposure = chageDataLanguage(e.name_th, e.name_en, lang)

                    };
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(ex))
                {

                    ex = rc.event_exposure;
                }
                else
                {

                    ex = ex + ", " + rc.event_exposure;
                }
            }



            return ex;

        }




        protected string getUnsafeAction(int incident_id, string lang)
        {
            string so = "";
            //string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.fact_findings
                    join s in dbConnect.source_incidents on c.source_incident_id equals s.id
                    // join e in dbConnect.event_exposures on c.event_exposure_id equals e.id
                    where c.incident_id == incident_id
                    select new
                    {
                        c.unsafe_action
                        // event_exposure = chageDataLanguage(s.name_th,s.name_en,lang)

                    };
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(so))
                {
                   if(rc.unsafe_action=="Y")
                   {
                       so = chageDataLanguage("ใช่","YES", lang);
                   }
                   else if (rc.unsafe_action == "N")
                   {
                       so = chageDataLanguage("ไม่", "NO", lang);
                   }
                   
                }
                else
                {
                    if (rc.unsafe_action == "Y")
                    {
                        so = so + ", " + chageDataLanguage("ใช่", "YES", lang);
                    }
                    else if (rc.unsafe_action == "N")
                    {
                        so = so + ", " + chageDataLanguage("ไม่", "NO", lang);
                    }
                  
                }
            }



            return so;

        }




        protected string getUnsafeCondition(int incident_id, string lang)
        {
            string so = "";
            //string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.fact_findings
                    join s in dbConnect.source_incidents on c.source_incident_id equals s.id
                    // join e in dbConnect.event_exposures on c.event_exposure_id equals e.id
                    where c.incident_id == incident_id
                    select new
                    {
                        c.unsafe_condition
                        // event_exposure = chageDataLanguage(s.name_th,s.name_en,lang)

                    };
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(so))
                {
                    if (rc.unsafe_condition == "Y")
                    {
                        so = chageDataLanguage("ใช่", "YES", lang);
                    }
                    else if (rc.unsafe_condition == "N")
                    {
                        so = chageDataLanguage("ไม่", "NO", lang);
                    }

                }
                else
                {
                    if (rc.unsafe_condition == "Y")
                    {
                        so = so + ", " + chageDataLanguage("ใช่", "YES", lang);
                    }
                    else if (rc.unsafe_condition == "N")
                    {
                        so = so + ", " + chageDataLanguage("ไม่", "NO", lang);
                    }

                }
            }



            return so;

        }



        protected string getRootCauseAction(int incident_id)
        {
            // string so = "";
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.root_cause_actions
                    where c.incident_id == incident_id && c.status == "A"
                    select new
                    {
                        c.name

                    };
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(ex))
                {

                    ex = rc.name;
                }
                else
                {

                    ex = ex + ", " + rc.name;
                }
            }



            return ex;

        }



        protected string getPyramid(int incident_id)
        {
            // string so = "";
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.root_cause_incidents
                    where c.incident_id == incident_id
                    select new
                    {
                        root_cause = c.root_cause

                    };
            foreach (var rc in v)
            {

                if (string.IsNullOrEmpty(ex))
                {
                    string value = "";
                    if (rc.root_cause == "policy_planning") value = Resources.Incident.policy_planning;
                    if (rc.root_cause == "document_reporting") value = Resources.Incident.document_reporting;
                    if (rc.root_cause == "responsibilities_resourses") value = Resources.Incident.responsibilities_resourses;
                    if (rc.root_cause == "standard_controls") value = Resources.Incident.standard_controls;
                    if (rc.root_cause == "hazard_assesment") value = Resources.Incident.hazard_assesment;
                    if (rc.root_cause == "inspection_monitoring") value = Resources.Incident.inspection_monitoring;
                    if (rc.root_cause == "legal_compliance") value = Resources.Incident.legal_compliance;
                    if (rc.root_cause == "emergency_preparation") value = Resources.Incident.emergency_preparation;
                    if (rc.root_cause == "safety_installation") value = Resources.Incident.safety_installation;
                    if (rc.root_cause == "management") value = Resources.Incident.management;
                    if (rc.root_cause == "purchasing_contractor") value = Resources.Incident.purchasing_contractor;
                    if (rc.root_cause == "occupational") value = Resources.Incident.occupational;
                    if (rc.root_cause == "selection_competency") value = Resources.Incident.selection_competency;
                    if (rc.root_cause == "corrective_preventive") value = Resources.Incident.corrective_preventive;
                    if (rc.root_cause == "incident_hazard") value = Resources.Incident.incident_hazard;
                    if (rc.root_cause == "health_wellness") value = Resources.Incident.health_wellness;
                    if (rc.root_cause == "hygience") value = Resources.Incident.hygience;
                    if (rc.root_cause == "system_performance") value = Resources.Incident.system_performance;
                    if (rc.root_cause == "communication_involvement") value = Resources.Incident.communication_involvement;
                    ex = value;
                }
                else
                {
                    string value = "";
                    if (rc.root_cause == "policy_planning") value = Resources.Incident.policy_planning;
                    if (rc.root_cause == "document_reporting") value = Resources.Incident.document_reporting;
                    if (rc.root_cause == "responsibilities_resourses") value = Resources.Incident.responsibilities_resourses;
                    if (rc.root_cause == "standard_controls") value = Resources.Incident.standard_controls;
                    if (rc.root_cause == "hazard_assesment") value = Resources.Incident.hazard_assesment;
                    if (rc.root_cause == "inspection_monitoring") value = Resources.Incident.inspection_monitoring;
                    if (rc.root_cause == "legal_compliance") value = Resources.Incident.legal_compliance;
                    if (rc.root_cause == "emergency_preparation") value = Resources.Incident.emergency_preparation;
                    if (rc.root_cause == "safety_installation") value = Resources.Incident.safety_installation;
                    if (rc.root_cause == "management") value = Resources.Incident.management;
                    if (rc.root_cause == "purchasing_contractor") value = Resources.Incident.purchasing_contractor;
                    if (rc.root_cause == "occupational") value = Resources.Incident.occupational;
                    if (rc.root_cause == "selection_competency") value = Resources.Incident.selection_competency;
                    if (rc.root_cause == "corrective_preventive") value = Resources.Incident.corrective_preventive;
                    if (rc.root_cause == "incident_hazard") value = Resources.Incident.incident_hazard;
                    if (rc.root_cause == "health_wellness") value = Resources.Incident.health_wellness;
                    if (rc.root_cause == "hygience") value = Resources.Incident.hygience;
                    if (rc.root_cause == "system_performance") value = Resources.Incident.system_performance;
                    if (rc.root_cause == "communication_involvement") value = Resources.Incident.communication_involvement;
                    ex = ex + ", " + value;
                }
            }



            return ex;

        }


        protected string getDamageCost(int incident_id)
        {
         
            string amount = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.damage_lists
                    where c.incident_id == incident_id
                    select new
                    {
                        c.damage_cost

                    };
            foreach (var rc in v)
            {

                amount = amount + rc.damage_cost;
               
            }



            return amount;

        }



        protected string getCorrectivePreventive(int incident_id)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.corrective_prevention_action_incidents
                    where c.incident_id == incident_id
                    select new
                    {
                        c.corrective_preventive_action

                    };
            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(ex))
                {

                    ex = rc.corrective_preventive_action;
                }
                else
                {

                    ex = ex + ", " + rc.corrective_preventive_action;
                }
            }



            return ex;


        }

        protected string searchBy(string function_id,string department_id,string division_id,string date_start,string date_end,string lang)
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
                    searchby = searchby +", " + Resources.Incident.lbdepartment + " :" + e.department_name;
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
                    searchby = searchby + ", " + Resources.Incident.lbdivision+ " :" + i.division_name;
                }
            }
            else
            {
                searchby = searchby + ", " + Resources.Incident.lbdivision + " :" + Resources.Main.all;

            }

               
            if(date_start!="")
            {
                searchby = searchby + ", " + Resources.Incident.date + " :" + date_start;
            }

            if (date_end != "")
            {
                searchby = searchby + " - " + date_end;
            }




            return searchby;
        }


        protected string getAction(int incident_id, string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.corrective_prevention_action_incidents
                    join s in dbConnect.action_status on c.action_status_id equals s.id
                    where c.incident_id == incident_id
                    select new
                    {
                        c.corrective_preventive_action,
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

                    ex = rc.corrective_preventive_action + " (" + status + ")";
                }
                else
                {

                    ex = ex + ", " + rc.corrective_preventive_action + " (" + status + ")";
                }
            }



            return ex;


        }




        protected string getPreventiveAction(int incident_id, string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.preventive_action_incidents
                    join s in dbConnect.action_status on c.action_status_id equals s.id
                    where c.incident_id == incident_id
                    select new
                    {
                        c.preventive_action,
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

                    ex = rc.preventive_action + " (" + status + ")";
                }
                else
                {

                    ex = ex + ", " + rc.preventive_action + " (" + status + ")";
                }
            }



            return ex;


        }



        protected string getConsequenceAction(int incident_id, string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.consequence_management_incidents
                    join s in dbConnect.action_status on c.action_status_id equals s.id
                    where c.incident_id == incident_id
                    select new
                    {
                        c.consequence_management,
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

                    ex = rc.consequence_management + " (" + status + ")";
                }
                else
                {

                    ex = ex + ", " + rc.consequence_management + " (" + status + ")";
                }
            }



            return ex;


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