using safetys4.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace safetys4
{
    /// <summary>
    /// Summary description for Report
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Report : System.Web.Services.WebService
    {



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListIncidentReport(string companyid, string functionid, string departmentid, string divisionid, string date_start, string date_end, string lang, string type_area)
        {
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

                     from s in joinS.DefaultIfEmpty()
                     from t in joinT.DefaultIfEmpty()
                     from n in joinN.DefaultIfEmpty()
                     from b in joinB.DefaultIfEmpty()
                     from f in joinF.DefaultIfEmpty()
                     from rs in joinReason.DefaultIfEmpty()
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
                         incident_type = "Injury or fatality",
                         id.doc_no,
                         id.culpability,
                         id.road_accident,
                         id.currency,
                         id.reason_reject,
                         reason_reject_type = chageDataLanguage(rs.name_th, rs.name_en, lang),
                         id.faltality_prevention_element_other,
                         id.step_form,
                         id.process_status,
                         id.submit_report_form2,
                         id.confirm_investigate_form2,
                         id.id,
                         id.country,
                         id.typeuser_login,
                         id.close_incident_date,
                         function_culpability = id.form2_function_id,
                         department_culpability = id.form3_department_id,
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



                     };


            if(type_area=="AREA")
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

                     from rs in joinReason.DefaultIfEmpty()
                     from f in joinF.DefaultIfEmpty()
                     where id.effect_environment == "Y" && d.status == "A"
                     orderby id.incident_date ascending
                     select new
                     {
                         report_date = id.report_date,
                         name = "-",//d.property_environment_damage,
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
                         incident_type = "damage",
                         id.doc_no,
                         id.culpability,
                         id.road_accident,
                         id.currency,
                         id.reason_reject,
                         reason_reject_type = chageDataLanguage(rs.name_th, rs.name_en, lang),
                         id.faltality_prevention_element_other,
                         id.step_form,
                         id.process_status,
                         id.submit_report_form2,
                         id.confirm_investigate_form2,
                         id.id,
                         id.country,
                         id.typeuser_login,
                         id.close_incident_date,
                         function_culpability = id.form2_function_id,
                         department_culpability = id.form3_department_id,
                         d.property_environment_damage,
                         damage_cost = d.damage_cost == null?"":d.damage_cost.ToString(),
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
                         department_injured_id = ""



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

                     from f in joinF.DefaultIfEmpty()
                     from rs in joinReason.DefaultIfEmpty()
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
                         incident_type = "",
                         id.doc_no,
                         id.culpability,
                         id.road_accident,
                         id.currency,
                         id.reason_reject,
                         reason_reject_type = chageDataLanguage(rs.name_th, rs.name_en, lang),
                         id.faltality_prevention_element_other,
                         id.step_form,
                         id.process_status,
                         id.submit_report_form2,
                         id.confirm_investigate_form2,
                         id.id,
                         id.country,
                         id.typeuser_login,
                         id.close_incident_date,
                         function_culpability = id.form2_function_id,
                         department_culpability = id.form3_department_id,
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
                         department_injured_id = ""



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



          
            int lenght = Convert.ToInt32(Context.Request["length"].ToString());
            int start = Convert.ToInt32(Context.Request["start"].ToString());
            int draw = Convert.ToInt32(Context.Request["draw"].ToString());
            string word_search = Context.Request["search[value]"].ToString();

                       
            var v4 = v.Concat(v3);
            if (word_search != "")
            {
                v4 = v4.Where(c => c.doc_no.Contains(word_search));

            }
            

            var v5 = v4.OrderBy(c => c.incident_datetime);
            int totalRow = v5.Count();


            var newV = v5.Skip(start).Take(lenght);
            
            ArrayList dataJson = new ArrayList();

            int no = start+1;
            foreach (var rc in newV)
            {
                string incident_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.incident_datetime), lang);
                string incident_time = FormatDates.getTimeShowFromDate(Convert.ToDateTime(rc.incident_datetime), lang);
                string report_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.report_date), lang);
                string close_date = "";

                if (rc.close_incident_date != null)
                {
                    close_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.close_incident_date), lang);
                }
                
               // string damage_cost = getDamageCost(rc.incident_id);
                string corrective_preventive = getCorrectivePreventive(rc.incident_id);

                string serverity_injury_name = chageDataLanguage(rc.severity_injury_th, rc.severity_injury_en, lang);
                string type_employment_name = chageDataLanguage(rc.type_employment_th, rc.type_employment_en, lang);
                string nature_injury_name = chageDataLanguage(rc.nature_injury_th, rc.nature_injury_en, lang);
                string body_part_name = chageDataLanguage(rc.body_part_th, rc.body_part_en, lang);
                string status = getStatusStep(rc,rc.id,lang);


                string status_delay = "";
                int count_date = Convert.ToDateTime(rc.report_date).Subtract(Convert.ToDateTime(rc.incident_datetime)).Days;
                if (count_date > 14)//2 weeek
                {
                    status_delay = chageDataLanguage("ล่าช้า", "delay", lang);
                }
                else
                {
                    status_delay = chageDataLanguage("ปกติ", "normal", lang);
                }

                string type_reporter = "";
                if (rc.typeuser_login == "ad" || rc.typeuser_login == "employee")
                {
                    type_reporter = chageDataLanguage("employee", "employee", lang);
                }
                else if (rc.typeuser_login == "contractor")
                {
                    type_reporter = chageDataLanguage("contractor", "contractor", lang);
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

                string name_areaohs = getAreaOHS(rc.department_id, lang);
                string name_areamanager = getAreaManager(rc.division_id, lang);
                string name_areasupervisor = getAreaSupervisor(rc.section_id, lang);

                ArrayList dt = new ArrayList();
                dt.Add(no);
                dt.Add(rc.doc_no);
                dt.Add(report_date);

                dt.Add(rc.incident_area);
                dt.Add(rc.incident_name);

                dt.Add(rc.name);
                dt.Add(rc.property_environment_damage);

                //if (rc.incident_type == "damage")
                //{
                //    string damage_list = getDamageList(rc.incident_id);
                //    dt.Add(damage_list);
                //}
                //else
                //{
                //    dt.Add("");

                //}
                dt.Add(incident_date);
                dt.Add(incident_time);
                dt.Add(rc.incident_type);
                dt.Add(serverity_injury_name);

                string function_injured = getFunctionByForm3(rc.function_injured_id, lang);
                dt.Add(function_injured);
                string department_injured = getDepartmentByForm3(rc.department_injured_id, lang);
                dt.Add(department_injured);

                dt.Add(type_employment_name);
                dt.Add(rc.company_name);
                dt.Add(rc.function_name);
                dt.Add(rc.department_name);
                dt.Add(rc.division_name);
                dt.Add(rc.section_name);

                dt.Add(owner_activity);
                dt.Add(rc.activity_company_name);
                dt.Add(rc.activity_function_name);
                dt.Add(rc.activity_department_name);
                dt.Add(rc.activity_division_name);
                dt.Add(rc.activity_section_name);


                dt.Add(rc.day_lost);
                dt.Add(nature_injury_name);
                dt.Add(body_part_name);
                dt.Add(rc.incident_detail);

                string source_incident = getSourceIncident(rc.incident_id, lang);
                string event_incident = getEventExposure(rc.incident_id, lang);
                dt.Add(source_incident);
                dt.Add(event_incident);
                string resposible_area = "";
                if(rc.responsible_area=="OUT")
                {
                    resposible_area = chageDataLanguage("เกิดนอกพื้นที่ควบคุมกลุ่มบริษัทฯ", "Offsite", lang);

                }else if(rc.responsible_area=="IN"){

                    resposible_area = chageDataLanguage("เกิดในพื้นที่ควบคุมกลุ่มบริษัทฯ", "Onsite", lang);
                }
                dt.Add(resposible_area);
                dt.Add(rc.damage_cost);

                //if (rc.incident_type == "damage")
                //{
                //    string damage_cost = getDamagecost(rc.incident_id);
                //    dt.Add(damage_cost);
                //}
                //else
                //{
                //    dt.Add("");

                //}

                dt.Add(rc.fatality_prevention);
                dt.Add(corrective_preventive);
                string root_cause = getPyramid(rc.incident_id);
                dt.Add(root_cause);
                string other_impacts = getOtherimpact(rc.incident_id, lang);
                dt.Add(other_impacts);

                string critical = "";
                if(rc.critical=="Y"){
                    critical = chageDataLanguage("ใช่","Yes",lang);

                } if (rc.critical == "N")
                {
                    critical = chageDataLanguage("ไม่ใช่","No",lang);
                }
                dt.Add(critical);
                string function_cul = getFunctionByForm3(rc.function_culpability, lang);
                dt.Add(function_cul);
                dt.Add(rc.status+" "+status);
                dt.Add(name_areaohs);
                dt.Add(name_areamanager);
                dt.Add(name_areasupervisor);
                dt.Add(rc.reason_reject_type);
                dt.Add(rc.reason_reject);
                dt.Add(close_date);
                dt.Add(type_reporter);
                dt.Add(status_delay);
                string department_cul = getDepartmentByForm3(rc.department_culpability, lang);
                dt.Add(department_cul);


                dataJson.Add(dt);
                no++;

            }

            var result = new
            {
                draw = draw,
                recordsTotal = totalRow,
                recordsFiltered = totalRow,
                data = dataJson
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));


        }


        protected string getFunctionByForm3(string id, string lang)
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



        protected string getStatusStep(dynamic rc, int id,string lang)
        {
            string step = "";

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {


                if (rc.process_status != 2 && rc.process_status != 3 && rc.process_status != 4)//ไม่ใช้ close กับ reject กับ Exemption
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
                        string v_step = chageDataLanguage("ขอปิดรายงานอุบัติการณ์", "Request to Closen Incident Report", lang);
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

      

        protected string getDamagecost(int incident_id)
        {
            double value = 0;
             safetys4dbDataContext dbConnect = new safetys4dbDataContext();
             var v = from c in dbConnect.damage_lists
                     where c.incident_id == incident_id
                     && c.status == "A"
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
                    && c.status == "A"
                    select c;

            foreach (var rc in v)
            {
                if (string.IsNullOrEmpty(rc.property_environment_damage))
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

        protected string getSourceIncident(int incident_id, string lang)
        {
            string so = "";
            //string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.fact_findings
                    join s in dbConnect.source_incidents on c.source_incident_id equals s.id
                   // join e in dbConnect.event_exposures on c.event_exposure_id equals e.id
                    where c.incident_id == incident_id
                    select new{
                        source_incident = chageDataLanguage(s.name_th,s.name_en,lang),
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
                         event_exposure = chageDataLanguage(e.name_th,e.name_en,lang)

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




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getTotalinjuryFPE(    string company_id,
                                          string function_id,
                                          string department_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            ArrayList dataJson = new ArrayList();
            DataTable dt = new DataTable();
            dt.Columns.Add("label", typeof(string));
            dt.Columns.Add("value", typeof(int));

            var fpes = from f in dbConnect.fatality_prevention_elements
                       where f.country == Session["country"].ToString()
                            select f;


            foreach (var v in fpes)
            {

                var n = from i in dbConnect.injury_persons
                        join c in dbConnect.incidents on i.incident_id equals c.id
                        where i.type_employment_id != 3 && c.work_relate == "Y" && c.injury_fatality_involve == "Y" && c.country == Session["country"].ToString()
                        && (c.culpability == "G" || c.culpability == "P") && c.fatality_prevention_element_id == v.id
                        && i.status == "A"
                        
                       
                        select new
                        {
                            c.id,
                            c.company_id,
                            c.function_id,
                            c.department_id,
                            c.report_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.report_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.report_date <= d_end);
                }

                if (company_id != "")
                {
                    n = n.Where(c => c.company_id == company_id);
                }

                if (function_id != "")
                {
                    n = n.Where(c => c.function_id == function_id);
                }

                if (department_id != "")
                {
                    n = n.Where(c => c.department_id == department_id);
                }

                

                string label = chageDataLanguage(v.name_th,v.name_en,lang);



                dt.Rows.Add(label, n.Count());
             
            }

          
            dt.DefaultView.Sort = "value desc";
            dt = dt.DefaultView.ToTable();


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var result = new
                {
                    label = dt.Rows[i]["label"],
                    value = dt.Rows[i]["value"],
                };

                dataJson.Add(result);

            }




            var returnv = new
            {
                result = dataJson,
          
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));




        }




        [WebMethod(EnableSession= true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getRootCauseIncident( string company_id,
                                          string function_id,
                                          string department_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            string[]  rootcauses = new string[19];
            rootcauses[0] = "policy_planning";
            rootcauses[1] = "document_reporting";
            rootcauses[2] = "responsibilities_resourses";
            rootcauses[3] = "standard_controls";
            rootcauses[4] = "hazard_assesment";
            rootcauses[5] = "inspection_monitoring";
            rootcauses[6] = "legal_compliance";
            rootcauses[7] = "emergency_preparation";
            rootcauses[8] = "safety_installation";
            rootcauses[9] = "management";
            rootcauses[10] = "purchasing_contractor";
            rootcauses[11] = "occupational";
            rootcauses[12] = "selection_competency";
            rootcauses[13] = "corrective_preventive";
            rootcauses[14] = "incident_hazard";
            rootcauses[15] = "health_wellness";
            rootcauses[16] = "hygience";
            rootcauses[17] = "system_performance";
            rootcauses[18] = "communication_involvement";

            string[] root_names = new string[19];
            root_names[0] = Resources.Incident.policy_planning;
            root_names[1] = Resources.Incident.document_reporting;
            root_names[2] = Resources.Incident.responsibilities_resourses;
            root_names[3] = Resources.Incident.standard_controls;
            root_names[4] = Resources.Incident.hazard_assesment;
            root_names[5] = Resources.Incident.inspection_monitoring;
            root_names[6] = Resources.Incident.legal_compliance;
            root_names[7] = Resources.Incident.emergency_preparation;
            root_names[8] = Resources.Incident.safety_installation;
            root_names[9] = Resources.Incident.management;
            root_names[10] = Resources.Incident.purchasing_contractor;
            root_names[11] = Resources.Incident.occupational;
            root_names[12] = Resources.Incident.selection_competency;
            root_names[13] = Resources.Incident.corrective_preventive;
            root_names[14] = Resources.Incident.incident_hazard;
            root_names[15] = Resources.Incident.health_wellness;
            root_names[16] = Resources.Incident.hygience;
            root_names[17] = Resources.Incident.system_performance;
            root_names[18] = Resources.Incident.communication_involvement;


            DataTable dt = new DataTable();
            dt.Columns.Add("label", typeof(string));
            dt.Columns.Add("value", typeof(int));
            ArrayList dataJson = new ArrayList();

            for (int i = 0; i < rootcauses.Length;i++ )
            {

                var n = from c in dbConnect.incidents
                        join r in dbConnect.root_cause_incidents on c.id equals r.incident_id into joinRoot
                        from r in joinRoot.DefaultIfEmpty()
                        where c.work_relate == "Y" && c.injury_fatality_involve == "Y" && c.country == Session["country"].ToString()
                        && (c.culpability == "G" || c.culpability == "P") && r.root_cause == rootcauses[i]
                        select new
                        {
                            c.id,
                            c.company_id,
                            c.function_id,
                            c.department_id,
                            c.report_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.report_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.report_date <= d_end);
                }


                if (company_id != "")
                {
                    n = n.Where(c => c.company_id == company_id);
                }


                if (function_id != "")
                {
                    n = n.Where(c => c.function_id == function_id);
                }

                if (department_id != "")
                {
                    n = n.Where(c => c.department_id == department_id);
                }



                string label = root_names[i];

                dt.Rows.Add(label, n.Count());

            }


            dt.DefaultView.Sort = "value desc";
            dt = dt.DefaultView.ToTable();
           

            for (int i = 0;i< dt.Rows.Count;i++ )
            {
                var result = new
                {
                    label = dt.Rows[i]["label"],
                    value = dt.Rows[i]["value"],
                };

                dataJson.Add(result);

            }




            var returnv = new
            {
                result = dataJson,

            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));




        }




        protected string getDamageCost(int incident_id)
        {

            string amount = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.damage_lists
                    where c.incident_id == incident_id
                    && c.status == "A"
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



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListHazardReport(string companyid,string functionid, string departmentid, string divisionid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from h in dbConnect.hazards
                     join t in dbConnect.source_hazards on h.source_hazard equals t.id into joinT

                    // join co in dbConnect.companies on h.company_id equals co.company_id
                   //  join fu in dbConnect.functions on h.function_id equals fu.function_id
                    // join de in dbConnect.departments on h.department_id equals de.department_id
                    // join di in dbConnect.divisions on h.division_id equals di.division_id into joinDi
                    // join se in dbConnect.sections on h.section_id equals se.section_id into joinSe
                     join st in dbConnect.incident_status on h.process_status equals st.id
                     join rs in dbConnect.reason_reject_hazards on h.reason_reject_type equals rs.id into joinReason
                     join ch in dbConnect.hazard_characteristics on h.hazard_characteristic_id equals ch.id into joinCharacter

                    from t in joinT.DefaultIfEmpty()
                    from rs in joinReason.DefaultIfEmpty()
                    from ch in joinCharacter.DefaultIfEmpty()
                     //from di in joinDi.DefaultIfEmpty()
                    // from se in joinSe.DefaultIfEmpty()

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
                         source_hazard = chageDataLanguage(t.name_th,t.name_en,lang),
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
                         characteristic = chageDataLanguage(ch.name_th,ch.name_en,lang)
                       

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

                v1 = v1.Where(c => c.hazard_datetime >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                v1 = v1.Where(c => c.hazard_datetime <= d_end);
            }


          
            string word_search = Context.Request["search[value]"].ToString();
            if (word_search != "")
            {
                v1 = v1.Where(c => c.doc_no.Contains(word_search));

            }

            int totalRow = v1.Count();
            int lenght = Convert.ToInt32(Context.Request["length"].ToString());
            int start = Convert.ToInt32(Context.Request["start"].ToString());
            int draw = Convert.ToInt32(Context.Request["draw"].ToString());

            v1 = v1.Skip(start).Take(lenght);


            ArrayList dataJson = new ArrayList();

            int no = start + 1;
            foreach (var rc in v1)
            {
                string hazard_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.hazard_datetime), lang);
                string hazard_time = FormatDates.getTimeShowFromDate(Convert.ToDateTime(rc.hazard_datetime), lang);
                string report_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.report_date), lang);
                string close_date = "";
                if (rc.close_hazard_date != null)
                {
                    close_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.close_hazard_date), lang);
                }
              
                string verify_date = "";
                if (rc.verifying_date != null)
                {
                    verify_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.verifying_date), lang);
                }



                string type_login = rc.typeuser_login;
                string name_surname_reporter = "";
                string company_reporter = "";
                string function_reporter = "";
                string department_reporter = "";

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
                             //   join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                             //   join co in dbConnect.companies on o.company_id equals co.company_id into joinCo
                             //   join fu in dbConnect.functions on o.function_id equals fu.function_id into joinFu
                             //   join de in dbConnect.departments on o.department_id equals de.department_id into joinDe

                             //   from co in joinCo.DefaultIfEmpty()
                             //   from fu in joinFu.DefaultIfEmpty()
                             //   from de in joinDe.DefaultIfEmpty()
                                where c.employee_id == rc.employee_id
                                select new
                                {
                                    prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                                    first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                                    last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                                    //company_name = chageDataLanguage(co.company_th, co.company_en, Session["lang"].ToString()),
                                    //function_name = chageDataLanguage(fu.function_th, fu.function_en, Session["lang"].ToString()),
                                    //department_name = chageDataLanguage(de.department_th, de.department_en, Session["lang"].ToString()),

                                };


                        foreach (var rc2 in v)
                        {
                            name_surname_reporter = rc2.prefix + " " + rc2.first_name + " " + rc2.last_name;
                            //company_reporter = rc2.company_name;
                            //function_reporter = rc2.function_name;
                            //department_reporter = rc2.department_name;
                        }
                    }
                    else if (Session["country"].ToString() == "srilanka")
                    {
                        var v = from c in dbConnect.employees
                                //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                //join co in dbConnect.companies on o.company_id equals co.company_id into joinCo
                                //join fu in dbConnect.functions on o.function_id equals fu.function_id into joinFu
                                //join de in dbConnect.departments on o.sub_function_id equals de.department_id into joinDe

                               // from co in joinCo.DefaultIfEmpty()
                               // from fu in joinFu.DefaultIfEmpty()
                               // from de in joinDe.DefaultIfEmpty()
                                where c.employee_id == rc.employee_id
                                select new
                                {
                                    prefix = chageDataLanguage(c.prefix_th, c.prefix_en, Session["lang"].ToString()),
                                    first_name = chageDataLanguage(c.first_name_th, c.first_name_en, Session["lang"].ToString()),
                                    last_name = chageDataLanguage(c.last_name_th, c.last_name_en, Session["lang"].ToString()),
                                    //company_name = chageDataLanguage(co.company_th, co.company_en, Session["lang"].ToString()),
                                    //function_name = chageDataLanguage(fu.function_th, fu.function_en, Session["lang"].ToString()),
                                    //department_name = chageDataLanguage(de.department_th, de.department_en, Session["lang"].ToString()),

                                };


                        foreach (var rc2 in v)
                        {
                            name_surname_reporter = rc2.prefix + " " + rc2.first_name + " " + rc2.last_name;
                            //company_reporter = rc2.company_name;
                            //function_reporter = rc2.function_name;
                            //department_reporter = rc2.department_name;
                        }

                    }

                }


                string status_delay = "";
                int count_date = Convert.ToDateTime(rc.report_date).Subtract(Convert.ToDateTime(rc.hazard_datetime)).Days;
                if (count_date > 14)//2 weeek
                {
                    status_delay = chageDataLanguage("ล่าช้า", "delay", lang);
                }
                else
                {
                    status_delay = chageDataLanguage("ปกติ", "normal", lang);
                }

                string type_reporter = "";
                if (rc.typeuser_login == "ad" || rc.typeuser_login == "employee")
                {
                    type_reporter = chageDataLanguage("employee", "employee", lang);
                }
                else if (rc.typeuser_login == "contractor")
                {
                    type_reporter = chageDataLanguage("contractor", "contractor", lang);
                }

               
                string safety_name = getNameAction(rc.safety_officer_id, lang);
                //string report_name = getNameAction(rc.employee_id, lang);
                string area_owner_name = getNameAction(rc.area_owner_id, lang);
                //string function_report = getFunctionReport(rc.employee_id, lang);
                //string department_report = "";


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
                string status = getStatusStepHazard(rc, rc.id, lang);

                ArrayList dt = new ArrayList();
                dt.Add(no);
                dt.Add(rc.doc_no);
                dt.Add(report_date);
                dt.Add(hazard_date);
                dt.Add(hazard_time);
                dt.Add(rc.company_name);
                dt.Add(rc.function_name);
                dt.Add(rc.department_name);
                dt.Add(rc.division_name);
                dt.Add(rc.section_name);
                dt.Add(rc.hazard_area);
                dt.Add(rc.hazard_name);
                dt.Add(rc.hazard_detail);               

                dt.Add(rc.preliminary_action);
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
                dt.Add(type_action);

                dt.Add(type_reporter);
                dt.Add(name_surname_reporter);
                dt.Add(rc.reporter_company_name);
                dt.Add(rc.reporter_function_name);
                dt.Add(rc.reporter_department_name);
                dt.Add(verify_date);
                dt.Add(rc.source_hazard);
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


                dt.Add(level);
                dt.Add(safety_name);
                dt.Add(process_action);
                dt.Add(responsible_person_name);
                dt.Add(area_owner_name);
              
                dt.Add(rc.status +" "+ status);
                dt.Add(name_areamanager);
                dt.Add(name_areasupervisor);
                if (rc.reason_reject_type != "")
                {
                    dt.Add(rc.reason_reject_type);
                }
                else
                {
                    dt.Add(rc.reason_reject);
                }
                
                dt.Add(close_date);
                dt.Add(status_delay);
                dt.Add(rc.characteristic);


               


                dataJson.Add(dt);
                no++;

            }

            var result = new
            {
                draw = draw,
                recordsTotal = totalRow,
                recordsFiltered = totalRow,
                data = dataJson
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));


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

                    ex = rc.first_name + " " + rc.last_name;
                }
                else
                {

                    ex = ex + ", " + rc.first_name + " " + rc.last_name;
                }
            }



            return ex;


        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListSotReport(string companyid, string functionid, string departmentid, string divisionid, string date_start, string date_end, string lang)
        {
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
                         personal_catergory = chageDataLanguage(t.name_th, t.name_en, lang),
                         sot_team_first_name = chageDataLanguage(em.first_name_th, em.first_name_en, lang),
                         sot_team_last_name = chageDataLanguage(em.last_name_th, em.last_name_en, lang),
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



            string word_search = Context.Request["search[value]"].ToString();
            if (word_search != "")
            {
                v1 = v1.Where(c => c.doc_no.Contains(word_search));

            }

            int totalRow = v1.Count();
            int lenght = Convert.ToInt32(Context.Request["length"].ToString());
            int start = Convert.ToInt32(Context.Request["start"].ToString());
            int draw = Convert.ToInt32(Context.Request["draw"].ToString());

            v1 = v1.Skip(start).Take(lenght);


            ArrayList dataJson = new ArrayList();

            int no = start + 1;
            foreach (var rc in v1)
            {
                string sot_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.sot_date), lang);
                string sot_time = FormatDates.getTimeShowFromDate(Convert.ToDateTime(rc.sot_date), lang);
                string sot_time_end = FormatDates.getTimeShowFromDate(Convert.ToDateTime(rc.sot_date_end), lang);
                string report_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.report_date), lang);
                string close_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.close_sot_date), lang);


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
                           // from o in joinO.DefaultIfEmpty()
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


                string name_areamanager = getAreaManager(rc.division_id, lang);

                string status = getStatusStepSot(rc, rc.sot_id, lang);


                ArrayList dt = new ArrayList();
                dt.Add(no);
                dt.Add(rc.doc_no);
                dt.Add(report_date);
                dt.Add(sot_date);
                dt.Add(sot_time+"-"+sot_time_end);
                dt.Add(rc.company_name);
                dt.Add(rc.function_name);
                dt.Add(rc.department_name);
                dt.Add(rc.division_name);
                dt.Add(rc.location);
                dt.Add(rc.type_work);
                dt.Add(rc.personal_catergory);
                dt.Add(rc.comment);
                dt.Add(name_surname_reporter);   
                dt.Add(function_reporter);  
                dt.Add(department_reporter);
                dt.Add(name_surname_sotteam);
                dt.Add(rc.mngt_level);
                dt.Add(function_sotteam);
                dt.Add(department_sotteam);
                //dt.Add(division_sotteam);
                dt.Add(name_areamanager);
                dt.Add(rc.status + " " + status);

                if (rc.close_sot_date != null)
                {
                    dt.Add(close_date);
                }
                else
                {
                    dt.Add("");
                }
               

                dataJson.Add(dt);
                no++;

            }

            var result = new
            {
                draw = draw,
                recordsTotal = totalRow,
                recordsFiltered = totalRow,
                data = dataJson
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));


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

    

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListAllactionReport(string company_id,string function_id, string department_id, string division_id, string report_type, string date_start, string date_end, string lang,string type_area)
        {

            ArrayList dataJson = new ArrayList();
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            int totalRow = 0;
            int lenght = 0;
            int start = 0;
            int draw = 0;
            int REJECT_STATUS = 3;
            int EXEMPTION_STATUS = 4;

            if (report_type == "hazard")
            {

                var v1 = from h in dbConnect.hazards
                         join p in dbConnect.process_actions on h.id equals p.hazard_id
                         join t in dbConnect.type_controls on p.type_control equals t.id
                         join a in dbConnect.action_status on p.action_status_id equals a.id
                         join e in dbConnect.employees on p.employee_id equals e.employee_id into joinE
                         from e in joinE.DefaultIfEmpty()
                         join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                         from d in joinD.DefaultIfEmpty()
                         where h.country == Session["country"].ToString() && h.process_status != REJECT_STATUS
                         orderby h.hazard_date ascending
                         select new
                         {

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
                             h.company_id,
                             h.function_id,
                             h.department_id,
                             h.division_id,
                             department_name = chageDataLanguage(d.department_th, d.department_en, lang)


                         };

                if (company_id != "")
                {
                    v1 = v1.Where(c => c.company_id == company_id.Trim());

                }

                if (function_id != "")
                {
                    v1 = v1.Where(c => c.function_id == function_id.Trim());

                }

                if (department_id != "")
                {
                    v1 = v1.Where(c => c.department_id == department_id.Trim());

                }

                if (division_id != "")
                {
                    v1 = v1.Where(c => c.division_id == division_id.Trim());

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




                string word_search = Context.Request["search[value]"].ToString();
                if (word_search != "")
                {
                    v1 = v1.Where(c => c.doc_no.Contains(word_search));

                }

                 totalRow = v1.Count();
                 lenght = Convert.ToInt32(Context.Request["length"].ToString());
                 start = Convert.ToInt32(Context.Request["start"].ToString());
                 draw = Convert.ToInt32(Context.Request["draw"].ToString());

                v1 = v1.Skip(start).Take(lenght);



                int no = start + 1;
                foreach (var rc in v1)
                {
                    string due_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.due_date), lang);


                    ArrayList dt = new ArrayList();
                    dt.Add(no);
                    dt.Add(rc.doc_no);
                    dt.Add(rc.action);
                    dt.Add(rc.responsible_person);
                    dt.Add(rc.department_name);
                    dt.Add(rc.type_control);
                    dt.Add(due_date);


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


                    dt.Add(status);

                    dataJson.Add(dt);
                    no++;

                }

            }
            else if (report_type == "incident")
            {

                var v1 = from i in dbConnect.incidents
                         join c in dbConnect.corrective_prevention_action_incidents on i.id equals c.incident_id
                         join a in dbConnect.action_status on c.action_status_id equals a.id
                         join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                         from e in joinE.DefaultIfEmpty()
                         join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                         from d in joinD.DefaultIfEmpty()
                         where i.country == Session["country"].ToString() && i.process_status != REJECT_STATUS
                         && i.process_status != EXEMPTION_STATUS
                         orderby i.incident_date ascending
                         select new
                         {

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
                             status = chageDataLanguage(a.name_th,a.name_en,lang),
                             i.company_id,
                             i.function_id,
                             i.department_id,
                             i.division_id,
                             i.activity_company_id,
                             i.activity_function_id,
                             i.activity_department_id,
                             i.activity_division_id,
                             department_name = chageDataLanguage(d.department_th, d.department_en, lang),
                             incident_datetime = i.incident_date,
                             i.owner_activity


                         };

                if (type_area == "AREA")
                {
                    if (company_id != "")
                    {
                        v1 = v1.Where(c => c.company_id == company_id.Trim());

                    }

                    if (function_id != "")
                    {
                        v1 = v1.Where(c => c.function_id == function_id.Trim());

                    }

                    if (department_id != "")
                    {
                        v1 = v1.Where(c => c.department_id == department_id.Trim());

                    }

                    if (division_id != "")
                    {
                        v1 = v1.Where(c => c.division_id == division_id.Trim());

                    }

                }
                else
                {
                    if (company_id != "")
                    {
                        v1 = v1.Where(c => c.activity_company_id == company_id.Trim());

                    }

                    if (function_id != "")
                    {
                        v1 = v1.Where(c => c.activity_function_id == function_id.Trim());

                    }

                    if (department_id != "")
                    {
                        v1 = v1.Where(c => c.activity_department_id == department_id.Trim());

                    }

                    if (division_id != "")
                    {
                        v1 = v1.Where(c => c.activity_division_id == division_id.Trim());

                    }

                    v1 = v1.Where(c => c.owner_activity == "KNOWN");


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
                         join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                         from e in joinE.DefaultIfEmpty()
                         join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                         from d in joinD.DefaultIfEmpty()
                         where i.country == Session["country"].ToString() && i.process_status != REJECT_STATUS
                         && i.process_status != EXEMPTION_STATUS
                         orderby i.incident_date ascending
                         select new
                         {

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
                             i.company_id,
                             i.function_id,
                             i.department_id,
                             i.division_id,
                             i.activity_company_id,
                             i.activity_function_id,
                             i.activity_department_id,
                             i.activity_division_id,
                             department_name = chageDataLanguage(d.department_th, d.department_en, lang),
                             incident_datetime = i.incident_date,
                             i.owner_activity


                         };

                if (type_area == "AREA")
                {
                    if (company_id != "")
                    {
                        v2 = v2.Where(c => c.company_id == company_id.Trim());

                    }

                    if (function_id != "")
                    {
                        v2 = v2.Where(c => c.function_id == function_id.Trim());

                    }

                    if (department_id != "")
                    {
                        v2 = v2.Where(c => c.department_id == department_id.Trim());

                    }

                    if (division_id != "")
                    {
                        v2 = v2.Where(c => c.division_id == division_id.Trim());

                    }
                }
                else
                {
                    if (company_id != "")
                    {
                        v2 = v2.Where(c => c.activity_company_id == company_id.Trim());

                    }

                    if (function_id != "")
                    {
                        v2 = v2.Where(c => c.activity_function_id == function_id.Trim());

                    }

                    if (department_id != "")
                    {
                        v2 = v2.Where(c => c.activity_department_id == department_id.Trim());

                    }

                    if (division_id != "")
                    {
                        v2 = v2.Where(c => c.activity_division_id == division_id.Trim());

                    }

                    v2 = v2.Where(c => c.owner_activity == "KNOWN");
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
                         join e in dbConnect.employees on c.employee_id equals e.employee_id into joinE
                         from e in joinE.DefaultIfEmpty()
                         join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                         from d in joinD.DefaultIfEmpty()
                         where i.country == Session["country"].ToString() && i.process_status != REJECT_STATUS
                         && i.process_status != EXEMPTION_STATUS
                         orderby i.incident_date ascending
                         select new
                         {

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
                             i.company_id,
                             i.function_id,
                             i.department_id,
                             i.division_id,
                             i.activity_company_id,
                             i.activity_function_id,
                             i.activity_department_id,
                             i.activity_division_id,
                             department_name = chageDataLanguage(d.department_th, d.department_en, lang),
                             incident_datetime = i.incident_date,
                             i.owner_activity


                         };



                if (type_area == "AREA")
                {
                    if (company_id != "")
                    {
                        v3 = v3.Where(c => c.company_id == company_id.Trim());

                    }

                    if (function_id != "")
                    {
                        v3 = v3.Where(c => c.function_id == function_id.Trim());

                    }

                    if (department_id != "")
                    {
                        v3 = v3.Where(c => c.department_id == department_id.Trim());

                    }

                    if (division_id != "")
                    {
                        v3 = v3.Where(c => c.division_id == division_id.Trim());

                    }

                   
                }
                else
                {
                    if (company_id != "")
                    {
                        v3 = v3.Where(c => c.activity_company_id == company_id.Trim());

                    }

                    if (function_id != "")
                    {
                        v3 = v3.Where(c => c.activity_function_id == function_id.Trim());

                    }

                    if (department_id != "")
                    {
                        v3 = v3.Where(c => c.activity_department_id == department_id.Trim());

                    }

                    if (division_id != "")
                    {
                        v3 = v3.Where(c => c.activity_division_id == division_id.Trim());

                    }


                    v3 = v3.Where(c => c.owner_activity == "KNOWN");

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


                var newv = v.Concat(v3);


                string word_search = Context.Request["search[value]"].ToString();
                if (word_search != "")
                {
                    newv = newv.Where(c => c.doc_no.Contains(word_search));

                }

                 totalRow = newv.Count();
                 lenght = Convert.ToInt32(Context.Request["length"].ToString());
                 start = Convert.ToInt32(Context.Request["start"].ToString());
                 draw = Convert.ToInt32(Context.Request["draw"].ToString());

                 newv = newv.Skip(start).Take(lenght);

                newv = newv.OrderBy(c => c.incident_datetime);

                int no = start + 1;
                foreach (var rc in newv)
                {
                    string due_date = FormatDates.getDateShowFromDate(Convert.ToDateTime(rc.due_date), lang);

                    ArrayList dt = new ArrayList();
                    dt.Add(no);
                    dt.Add(rc.doc_no);
                    dt.Add(rc.action_name);
                    dt.Add(rc.responsible_person);
                    dt.Add(rc.department_name);
                    dt.Add(rc.type_control);
                    dt.Add(due_date);


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
                    dt.Add(status);

                    dataJson.Add(dt);
                    no++;

                }

            }
            else if (report_type == "sot")
            {
                var v1 = from h in dbConnect.sots
                         join p in dbConnect.process_action_sots on h.id equals p.sot_id
                         join t in dbConnect.type_controls on p.type_control equals t.id
                         join a in dbConnect.sot_action_status on p.action_status_id equals a.id
                         join e in dbConnect.employees on p.employee_id equals e.employee_id into joinE
                         from e in joinE.DefaultIfEmpty()
                         join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         join d in dbConnect.departments on o.department_id equals d.department_id into joinD
                         from d in joinD.DefaultIfEmpty()
                         where h.country == Session["country"].ToString()
                         orderby h.sot_date ascending
                         select new
                         {

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
                             h.company_id,
                             h.function_id,
                             h.department_id,
                             h.division_id,
                             department_name = chageDataLanguage(d.department_th, d.department_en, lang)



                         };

                if (company_id != "")
                {
                    v1 = v1.Where(c => c.company_id == company_id.Trim());

                }

                if (function_id != "")
                {
                    v1 = v1.Where(c => c.function_id == function_id.Trim());

                }

                if (department_id != "")
                {
                    v1 = v1.Where(c => c.department_id == department_id.Trim());

                }

                if (division_id != "")
                {
                    v1 = v1.Where(c => c.division_id == division_id.Trim());

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



                totalRow = v1.Count();
                lenght = Convert.ToInt32(Context.Request["length"].ToString());
                start = Convert.ToInt32(Context.Request["start"].ToString());
                draw = Convert.ToInt32(Context.Request["draw"].ToString());

                v1 = v1.Skip(start).Take(lenght);



                int no = start + 1;
                foreach (var rc in v1)
                {
                    string due_date = FormatDates.getDatetimeShow(Convert.ToDateTime(rc.due_date), lang);

                    ArrayList dt = new ArrayList();
                    dt.Add(no);
                    dt.Add(rc.doc_no);
                    dt.Add(rc.action);
                    dt.Add(rc.responsible_person);
                    dt.Add(rc.department_name);
                    dt.Add(rc.type_control);
                    dt.Add(due_date);

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
                    dt.Add(status);

                    dataJson.Add(dt);
                    no++;

                }


            }

            var result = new
            {
                draw = draw,
                recordsTotal = totalRow,
                recordsFiltered = totalRow,
                data = dataJson
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));


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
                       function_name = chageDataLanguage(f.function_th,f.function_en,lang)
                       

                    };
            foreach (var rc in v)
            {
                name = rc.function_name;
            }

            return name;
        }

        protected string getNameAction(string employee_id,string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            string name = "";
            var v = from c in dbConnect.employees              
                    where c.employee_id == employee_id
                    select new
                    {
                        first_name = chageDataLanguage(c.first_name_th,c.first_name_en,lang),
                        last_name = chageDataLanguage(c.last_name_th,c.last_name_en,lang)

                    };
            foreach (var rc in v)
            {
                name = rc.first_name + " " + rc.last_name;
            }

            return name;
        }



        protected string getProcessAction(int hazard_id,string lang)
        {
            string ex = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var v = from c in dbConnect.process_actions
                    join t in dbConnect.type_controls on c.type_control equals t.id
                    where c.hazard_id == hazard_id
                    select new
                    {
                        type_control = chageDataLanguage(t.name_th,t.name_en,lang)

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



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListFatalityEmployeeCompanyReport(string companyid, string date_start, string date_end, string lang)
        {
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
            int count = 0;
            ArrayList dataJson = new ArrayList();
            foreach (var rc in v1)
            {

                ArrayList dt = new ArrayList();

                if (count == 0)
                {

                    int fatality_group = getFatalityEmployee("", "", "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourEmployee("", "", "", date_start, date_end, lang);
                    double fatality_rate_group = 0;

                    if (workhour_group != 0)
                    {
                        fatality_rate_group = caclFatalityRate(fatality_group, workhour_group);
                    }

                   
                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    dt.Add("");
                    dt.Add(insee_group);
                    dt.Add(fatality_group);
                    dt.Add(String.Format("{0:n}", workhour_group));
                    dt.Add(fatality_rate_group.ToString("F2"));

                    dataJson.Add(dt);
                }

                dt = new ArrayList();

                int fatality = getFatalityEmployee(rc.company_id, "", "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployee(rc.company_id, "", "", date_start, date_end, lang);
                double fatality_rate = 0;

                if (workhour != 0)
                {
                    fatality_rate = caclFatalityRate(fatality, workhour);
                }

            

                dt.Add(rc.company_id);
                dt.Add(rc.company_name);
                dt.Add(fatality);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(fatality_rate.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }






            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFREmployeeCompanyReport(string companyid, string date_start, string date_end, string lang)
        {
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
            int count = 0;
            ArrayList dataJson = new ArrayList();
            foreach (var rc in v1)
            {

                ArrayList dt = new ArrayList();

                if (count == 0)
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

                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    dt.Add("");
                    dt.Add(insee_group);
                    dt.Add(targetEmployee_group.ToString("F2"));
                    dt.Add(lti_group);
                    dt.Add(String.Format("{0:n}", workhour_group));
                    dt.Add(ltifr_group.ToString("F2"));
                    dt.Add(day_lost_group);
                    dt.Add(ltisr_group.ToString("F2"));

                    dataJson.Add(dt);
                }

                dt = new ArrayList();  
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


                dt.Add(rc.company_id);
                dt.Add(rc.company_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(ltifr.ToString("F2"));
                dt.Add(day_lost);
                dt.Add(ltisr.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }


           



            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFREmployeeContractorOnsiteOffsiteCompanyReport(string companyid, string date_start, string date_end, string lang)
        {
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
            int count = 0;
            ArrayList dataJson = new ArrayList();
            foreach (var rc in v1)
            {

                ArrayList dt = new ArrayList();

                if (count == 0)
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

                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    dt.Add("");
                    dt.Add(insee_group);
                    dt.Add(targetEmployee_group.ToString("F2"));
                    dt.Add(lti_group);
                    dt.Add(String.Format("{0:n}", workhour_group));
                    dt.Add(day_lost_group);
                    dt.Add(ltifr_group.ToString("F2"));
             

                    dataJson.Add(dt);
                }

                dt = new ArrayList();
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

             

                dt.Add(rc.company_id);
                dt.Add(rc.company_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour)); 
                dt.Add(day_lost);
                dt.Add(ltifr.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }






            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListFatalityEmployeeFunctionReport(string companyid, string functionid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.functions
                     where d.company_id == companyid
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
                v1 = v1.Where(c => c.function_id == functionid);

            }
            int count = 0;
            ArrayList dataJson = new ArrayList();
            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                int fatality = getFatalityEmployee(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployee(companyid, rc.function_id, "", date_start, date_end, lang);
                double fatality_rate = 0;

                if (workhour != 0)
                {
                    fatality_rate = caclFatalityRate(fatality, workhour);
                }

               

                dt.Add(rc.function_id);
                dt.Add(rc.function_name);
                dt.Add(fatality);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(fatality_rate.ToString("F2"));


                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFREmployeeFunctionReport(string companyid, string functionid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.functions
                     where d.company_id == companyid
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
                v1 = v1.Where(c => c.function_id == functionid);

            }
            int count = 0;
            ArrayList dataJson = new ArrayList();
            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                double targetEmployee = getTargetLTIFREmployee(companyid, rc.function_id,"", date_start, date_end, lang);
                int lti = getLTIEmployee(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployee(companyid, rc.function_id, "", date_start, date_end, lang);
                double multiplier = getMultiplier(companyid, rc.function_id,"", date_start, date_end, lang);
                int day_lost = getDayLost(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
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

                dt.Add(rc.function_id);
                dt.Add(rc.function_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(ltifr.ToString("F2"));
                dt.Add(day_lost);
                dt.Add(ltisr.ToString("F2"));


                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }







        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFREmployeeContractorOnsiteOffsiteFunctionReport(string companyid, string functionid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.functions
                     where d.company_id == companyid
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
                v1 = v1.Where(c => c.function_id == functionid);

            }
            int count = 0;
            ArrayList dataJson = new ArrayList();
            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                double targetEmployee = getTargetLTIFREmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang);
                int lti = getLTIEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang);
                double multiplier = getMultiplierEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang);
                int day_lost = getDayLostEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;

                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }

                dt.Add(rc.function_id);
                dt.Add(rc.function_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(day_lost);
                dt.Add(ltifr.ToString("F2"));


                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListFatalityEmployeeDepartmentReport(string functionid, string departmentid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.departments
                     where d.function_id == functionid
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
                v1 = v1.Where(c => c.department_id == departmentid);

            }
            int count = 0;
            ArrayList dataJson = new ArrayList();
            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                int fatality = getFatalityEmployee("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployee("", functionid, rc.department_id, date_start, date_end, lang);
                double fatality_rate = 0;

                if (workhour != 0)
                {
                    fatality_rate = caclFatalityRate(fatality, workhour);
                }


                dt.Add(rc.department_id);
                dt.Add(rc.department_name);
                dt.Add(fatality);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(fatality_rate.ToString("F2"));



                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFREmployeeDepartmentReport(string functionid,string departmentid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.departments
                     where d.function_id == functionid
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
                v1 = v1.Where(c => c.department_id == departmentid);

            }
            int count = 0;
            ArrayList dataJson = new ArrayList();
            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                double targetEmployee = getTargetLTIFREmployee("", functionid,rc.department_id, date_start, date_end, lang);
                int lti = getLTIEmployee("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployee("", functionid, rc.department_id, date_start, date_end, lang);
                double multiplier = getMultiplier("", functionid,rc.department_id, date_start, date_end, lang);
                int day_lost = getDayLost("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
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

                dt.Add(rc.department_id);
                dt.Add(rc.department_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(ltifr.ToString("F2"));
                dt.Add(day_lost);
                dt.Add(ltisr.ToString("F2"));



                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFREmployeeContractorOnsiteOffsiteDepartmentReport(string functionid, string departmentid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.departments
                     where d.function_id == functionid
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
                v1 = v1.Where(c => c.department_id == departmentid);

            }
            int count = 0;
            ArrayList dataJson = new ArrayList();
            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                double targetEmployee = getTargetLTIFREmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang);
                int lti = getLTIEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang);
                double multiplier = getMultiplierEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang);
                int day_lost = getDayLostEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;

                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }



                dt.Add(rc.department_id);
                dt.Add(rc.department_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));   
                dt.Add(day_lost);
                dt.Add(ltifr.ToString("F2"));



                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }





        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFREmployeeReportSrilanka(string siteid,string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1  = (from c in dbConnect.organizations
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

            int count = 0;
            string thbody = "";

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

                if (count == 0)
                {

                    double targetEmployee_group = getTargetLTIFREmployeeSrilanka("00000000", date_start, date_end, lang);
                    int lti_group = getLTIEmployeeSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourEmployeeSrilanka("", date_start, date_end, lang);
                    double multiplier_group = getMultiplierSrilanka("00000000", date_start, date_end, lang);
                    double ltifr_group = 0;

                    if (workhour_group != 0)
                    {
                        ltifr_group = caclLTIFRANDTIFR(lti_group, multiplier_group, workhour_group);
                    }

                    thbody = "<tr>";
                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    thbody = thbody + "<td>" + insee_group + "</td>";
                    thbody = thbody + "<td>" + targetEmployee_group + "</td>";
                    thbody = thbody + "<td>" + lti_group + "</td>";
                    thbody = thbody + "<td>" + workhour_group + "</td>";
                    thbody = thbody + "<td>" + ltifr_group + "</td>";

                    thbody = thbody + "</tr>";
                }

                string tr_start = "<tr>";
                thbody = thbody + tr_start;

                thbody = thbody + "<td>" + rc.name + "</td>";
                thbody = thbody + "<td>" + targetEmployee + "</td>";
                thbody = thbody + "<td>" + lti + "</td>";
                thbody = thbody + "<td>" + workhour + "</td>";
                thbody = thbody + "<td>" + ltifr + "</td>";
                string tr_end = "</tr>";
                thbody = thbody + tr_end;
                count++;

            }

            var result = new
            {
                data = thbody
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFRContractorOnsiteCompanyReport(string companyid, string date_start, string date_end, string lang)
        {
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

            ArrayList dataJson = new ArrayList();
            int count = 0;
           
            foreach (var rc in v1)
            {

                ArrayList dt = new ArrayList();
                if (count == 0)
                {
                   
                    double targetContractorOnsite_group = getTargetLTIFRContractorOnsiteGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOnsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourContractorOnsite("","", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorGroup("00000000", date_start, date_end, lang);
                    int day_lost_group = getDayLostContractorOnsite("", "", "", date_start, date_end, lang, Session["country"].ToString());

                    double ltifr_group = 0;

                    if (workhour_group != 0)
                    {
                        ltifr_group = caclLTIFRANDTIFR(lti_group, multiplier_group, workhour_group);
                    }


                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    dt.Add("");
                    dt.Add(insee_group);
                    dt.Add(targetContractorOnsite_group.ToString("F2"));
                    dt.Add(lti_group);
                    dt.Add(String.Format("{0:n}", workhour_group));
                    dt.Add(day_lost_group);
                    dt.Add(ltifr_group.ToString("F2"));

                    dataJson.Add(dt);
                }


                dt = new ArrayList();  
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

                dt.Add(rc.company_id);
                dt.Add(rc.company_name);
                dt.Add(targetContractorOnsite.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(day_lost);
                dt.Add(ltifr.ToString("F2"));


                dataJson.Add(dt);

                count++;
            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));



        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFRContractorOnsiteFunctionReport(string companyid, string functionid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.functions
                     where d.company_id == companyid
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
                v1 = v1.Where(c => c.function_id == functionid);

            }

            ArrayList dataJson = new ArrayList();
            int count = 0;

            foreach (var rc in v1)
            {

                ArrayList dt = new ArrayList();

                double targetContractorOnsite = getTargetLTIFRContractorOnsite(companyid, rc.function_id,"", date_start, date_end, lang);
                int lti = getLTIContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang);
                double multiplier = getMultiplierContractor(companyid, rc.function_id,"", date_start, date_end, lang);
                int day_lost = getDayLostContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;


                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }

                dt.Add(rc.function_id);
                dt.Add(rc.function_name);
                dt.Add(targetContractorOnsite.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(day_lost);
                dt.Add(ltifr.ToString("F2"));


                dataJson.Add(dt);

                count++;
            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));



        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFRContractorOnsiteDepartmentReport(string functionid, string departmentid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.departments
                     where d.function_id == functionid
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
                v1 = v1.Where(c => c.department_id == departmentid);

            }

            ArrayList dataJson = new ArrayList();
            int count = 0;

            foreach (var rc in v1)
            {

                ArrayList dt = new ArrayList();

                double targetContractorOnsite = getTargetLTIFRContractorOnsite("", functionid,rc.department_id, date_start, date_end, lang);
                int lti = getLTIContractorOnsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourContractorOnsite("", functionid,rc.department_id, date_start, date_end, lang);
                double multiplier = getMultiplierContractor("", functionid,rc.department_id, date_start, date_end, lang);
                int day_lost = getDayLostContractorOnsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;


                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }

                dt.Add(rc.department_id);
                dt.Add(rc.department_name);
                dt.Add(targetContractorOnsite.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(day_lost);
                dt.Add(ltifr.ToString("F2"));


                dataJson.Add(dt);

                count++;
            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));



        }

      


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFRContractorOnsiteReportSrilanka(string siteid, string date_start, string date_end, string lang)
        {
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

           

            int count = 0;
            string thbody = "";


            foreach (var rc in v1)
            {
                double targetContractorOnsite = getTargetLTIFRContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang);
                int lti = getLTIContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang);
                double multiplier = getMultiplierContractorSrilanka(rc.site_id,date_start, date_end, lang);
                double ltifr = 0;


                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }

                if (count == 0)
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

                    thbody = "<tr>";
                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    thbody = thbody + "<td>" + insee_group + "</td>";
                    thbody = thbody + "<td>" + targetContractorOnsite_group + "</td>";
                    thbody = thbody + "<td>" + lti_group + "</td>";
                    thbody = thbody + "<td>" + workhour_group + "</td>";
                    thbody = thbody + "<td>" + ltifr_group + "</td>";

                    thbody = thbody + "</tr>";
                }

                string tr_start = "<tr>";
                thbody = thbody + tr_start;

                thbody = thbody + "<td>" + rc.name + "</td>";
                thbody = thbody + "<td>" + targetContractorOnsite + "</td>";
                thbody = thbody + "<td>" + lti + "</td>";
                thbody = thbody + "<td>" + workhour + "</td>";
                thbody = thbody + "<td>" + ltifr + "</td>";
                string tr_end = "</tr>";
                thbody = thbody + tr_end;
                count++;
            }

            var result = new
            {
                data = thbody
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));



        }





        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFRContractorOffsiteCompanyReport(string companyid, string date_start, string date_end, string lang)
        {
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

            ArrayList dataJson = new ArrayList();
            int count = 0;
          
            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                if (count == 0)
                {

                    double targetContractorOffsite_group = getTargetLTIFRContractorOffsiteGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOffsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    double workhour_group = getWorkhourContractorOffsite("", "","", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorOffsiteGroup("00000000", date_start, date_end, lang);
                    int day_lost_group = getDayLostContractorOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());

                    double ltifr_group = 0;

                    if (workhour_group != 0)
                    {
                        ltifr_group = caclLTIFRANDTIFR(lti_group, multiplier_group, workhour_group);
                    }


                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    dt.Add("");
                    dt.Add(insee_group);
                    dt.Add(targetContractorOffsite_group.ToString("F2"));
                    dt.Add(lti_group);
                    dt.Add(String.Format("{0:n}", workhour_group));
                    dt.Add(day_lost_group);
                    dt.Add(ltifr_group.ToString("F2"));

                    dataJson.Add(dt);
                }

                dt = new ArrayList();
                double targetContractorOffsite = getTargetLTIFRContractorOffsite(rc.company_id,"","", date_start, date_end, lang);
                int lti = getLTIContractorOffsite(rc.company_id,"","", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourContractorOffsite(rc.company_id,"","", date_start, date_end, lang);
                double multiplier = getMultiplierContractorOffsite(rc.company_id,"","", date_start, date_end, lang);
                int day_lost = getDayLostContractorOffsite("", "", "", date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;
                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }


                dt.Add(rc.company_id);
                dt.Add(rc.company_name);
                dt.Add(targetContractorOffsite.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(day_lost);
                dt.Add(ltifr.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFRContractorOffsiteFunctionReport(string companyid, string functionid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.functions
                     where d.company_id == companyid
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
                v1 = v1.Where(c => c.function_id == functionid);

            }

            ArrayList dataJson = new ArrayList();
            int count = 0;
            
            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();


                double targetContractorOffsite = getTargetLTIFRContractorOffsite(companyid, rc.function_id,"", date_start, date_end, lang);
                int lti = getLTIContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang);
                double multiplier = getMultiplierContractorOffsite(companyid, rc.function_id,"", date_start, date_end, lang);
                int day_lost = getDayLostContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;
                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }


                dt.Add(rc.function_id);
                dt.Add(rc.function_name);
                dt.Add(targetContractorOffsite.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(day_lost);
                dt.Add(ltifr.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public void getListLTIFRContractorOffsiteDepartmentReport(string functionid, string departmentid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.departments
                     where d.function_id == functionid
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
                v1 = v1.Where(c => c.department_id == departmentid);

            }

            ArrayList dataJson = new ArrayList();
            int count = 0;
       
            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();


                double targetContractorOffsite = getTargetLTIFRContractorOffsite("", functionid,rc.department_id, date_start, date_end, lang);
                int lti = getLTIContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                double workhour = getWorkhourContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang);
                double multiplier = getMultiplierContractorOffsite("", functionid,rc.department_id, date_start, date_end, lang);
                int day_lost = getDayLostContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                double ltifr = 0;
                if (workhour != 0)
                {
                    ltifr = caclLTIFRANDTIFR(lti, multiplier, workhour);
                }


                dt.Add(rc.department_id);
                dt.Add(rc.department_name);
                dt.Add(targetContractorOffsite.ToString("F2"));
                dt.Add(lti);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(day_lost);
                dt.Add(ltifr.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListLTIFRContractorOffsiteReportSrilanka(string siteid,string date_start, string date_end, string lang)
        {
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

           

            int count = 0;
            string thbody = "";

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

                if (count == 0)
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


                    thbody = "<tr>";
                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    thbody = thbody + "<td>" + insee_group + "</td>";
                    thbody = thbody + "<td>" + targetContractorOffsite_group + "</td>";
                    thbody = thbody + "<td>" + lti_group + "</td>";
                    thbody = thbody + "<td>" + workhour_group + "</td>";
                    // thbody = thbody + "<td>" + ltifr_group + "</td>";

                    thbody = thbody + "</tr>";
                }

                string tr_start = "<tr>";
                thbody = thbody + tr_start;

                thbody = thbody + "<td>" + rc.name + "</td>";
                thbody = thbody + "<td>" + targetContractorOffsite + "</td>";
                thbody = thbody + "<td>" + lti + "</td>";
                thbody = thbody + "<td>" + workhour + "</td>";
                // thbody = thbody + "<td>" + ltifr + "</td>";
                string tr_end = "</tr>";
                thbody = thbody + tr_end;
                count++;

            }

            var result = new
            {
                data = thbody
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFREmployeeCompanyReport(string companyid, string date_start, string date_end, string lang)
        {
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

            int count = 0;
            ArrayList dataJson = new ArrayList();

            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();


                if (count == 0)
                {

                    double targetEmployee_group = getTargetTIFREmployeeGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIEmployee("", "","", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_group = getFatalityEmployee("", "","", date_start, date_end, lang, Session["country"].ToString());
                    int pd_group = getPDEmployee("", "","", date_start, date_end, lang, Session["country"].ToString());
                    int mti_group = getMTIEmployee("", "","", date_start, date_end, lang, Session["country"].ToString());
                    int mi_group = getMIEmployee("", "","", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_group = getRWCEmployee("", "","", date_start, date_end, lang, Session["country"].ToString());
                    int total_group = lti_group + fatality_group + pd_group + mti_group + mi_group + rwc_group;
                    double workhour_group = getWorkhourEmployee("", "","", date_start, date_end, lang);
                    double multiplier_group = getMultiplierGroup("00000000", date_start, date_end, lang);
                    double tifr_group = 0;

                    if (workhour_group != 0)
                    {
                        tifr_group = caclLTIFRANDTIFR(total_group, multiplier_group, workhour_group);
                    }

                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    dt.Add("");
                    dt.Add(insee_group);
                    dt.Add(targetEmployee_group.ToString("F2"));
                    dt.Add(fatality_group);
                    dt.Add(pd_group);
                    dt.Add(lti_group);
                    dt.Add(rwc_group);
                    dt.Add(mti_group);
                    dt.Add(mi_group);
                    dt.Add(total_group);
                    dt.Add(String.Format("{0:n}", workhour_group));
                    dt.Add(tifr_group.ToString("F2"));

                    dataJson.Add(dt);
                }

                dt = new ArrayList(); 

                double targetEmployee = getTargetTIFREmployee(rc.company_id,"","", date_start, date_end, lang);
                int lti = getLTIEmployee(rc.company_id,"","", date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityEmployee(rc.company_id,"","", date_start, date_end, lang, Session["country"].ToString());
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

  
                dt.Add(rc.company_id);
                dt.Add(rc.company_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));

                dataJson.Add(dt);
                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));





        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFREmployeeContractorOnsiteOffsiteCompanyReport(string companyid, string date_start, string date_end, string lang)
        {
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

            int count = 0;
            ArrayList dataJson = new ArrayList();

            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();


                if (count == 0)
                {

                    double targetEmployee_group = getTargetTIFREmployeeContractorOnsiteOffsiteGroup("00000000", date_start, date_end, lang);
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

                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    dt.Add("");
                    dt.Add(insee_group);
                    dt.Add(targetEmployee_group.ToString("F2"));
                    dt.Add(fatality_group);
                    dt.Add(pd_group);
                    dt.Add(lti_group);
                    dt.Add(rwc_group);
                    dt.Add(mti_group);
                    dt.Add(mi_group);
                    dt.Add(total_group);
                    dt.Add(String.Format("{0:n}", workhour_group));
                    dt.Add(tifr_group.ToString("F2"));

                    dataJson.Add(dt);
                }

                dt = new ArrayList();

                double targetEmployee = getTargetTIFREmployeeContractorOnsiteOffsite(rc.company_id, "", "", date_start, date_end, lang);
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


                dt.Add(rc.company_id);
                dt.Add(rc.company_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));

                dataJson.Add(dt);
                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));





        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFREmployeeFunctionReport(string companyid, string functionid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.functions
                     where d.company_id == companyid
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
                v1 = v1.Where(c => c.function_id == functionid);

            }

            int count = 0;
            ArrayList dataJson = new ArrayList();

            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                double targetEmployee = getTargetTIFREmployee(companyid, rc.function_id, "", date_start, date_end, lang);
                int lti = getLTIEmployee(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityEmployee(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDEmployee(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIEmployee(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIEmployee(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCEmployee(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourEmployee(companyid, rc.function_id, "", date_start, date_end, lang);
                double multiplier = getMultiplier(companyid, rc.function_id, "", date_start, date_end, lang);
                double tifr = 0;

                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }


                dt.Add(rc.function_id);
                dt.Add(rc.function_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));





        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFREmployeeContractorOnsiteOffsiteFunctionReport(string companyid, string functionid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.functions
                     where d.company_id == companyid
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
                v1 = v1.Where(c => c.function_id == functionid);

            }

            int count = 0;
            ArrayList dataJson = new ArrayList();

            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                double targetEmployee = getTargetTIFREmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang);
                int lti = getLTIEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang);
                double multiplier = getMultiplierEmployeeContractorOnsiteOffsite(companyid, rc.function_id, "", date_start, date_end, lang);
                double tifr = 0;

                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }


                dt.Add(rc.function_id);
                dt.Add(rc.function_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));





        }





        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFREmployeeDepartmentReport(string functionid, string departmentid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.departments
                     where d.function_id == functionid
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
                v1 = v1.Where(c => c.department_id == departmentid);

            }

            int count = 0;
            ArrayList dataJson = new ArrayList();

            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                double targetEmployee = getTargetTIFREmployee("", functionid, rc.department_id, date_start, date_end, lang);
                int lti = getLTIEmployee("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityEmployee("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDEmployee("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIEmployee("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIEmployee("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCEmployee("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourEmployee("", functionid, rc.department_id,date_start, date_end, lang);
                double multiplier = getMultiplier("", functionid, rc.department_id, date_start, date_end, lang);
                double tifr = 0;

                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }


                dt.Add(rc.department_id);
                dt.Add(rc.department_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));


                dataJson.Add(dt);
                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));





        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFREmployeeContractorOnsiteOffsiteDepartmentReport(string functionid, string departmentid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.departments
                     where d.function_id == functionid
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
                v1 = v1.Where(c => c.department_id == departmentid);

            }

            int count = 0;
            ArrayList dataJson = new ArrayList();

            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                double targetEmployee = getTargetTIFREmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang);
                int lti = getLTIEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang);
                double multiplier = getMultiplierEmployeeContractorOnsiteOffsite("", functionid, rc.department_id, date_start, date_end, lang);
                double tifr = 0;

                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }


                dt.Add(rc.department_id);
                dt.Add(rc.department_name);
                dt.Add(targetEmployee.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));


                dataJson.Add(dt);
                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));





        }





        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFREmployeeReportSrilanka(string siteid, string date_start, string date_end, string lang)
        {
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

            int count = 0;
            string thbody = "";

            foreach (var rc in v1)
            {
                double targetEmployee = getTargetTIFREmployeeSrilanka(rc.site_id, date_start, date_end, lang);
                int lti = getLTIEmployeeSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityEmployeeSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDEmployeeSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIEmployeeSrilanka(rc.site_id,date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIEmployeeSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi;
                double workhour = getWorkhourEmployeeSrilanka(rc.site_id, date_start, date_end, lang);
                double multiplier = getMultiplierSrilanka(rc.site_id,date_start, date_end, lang);
                double tifr = 0;

                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }

                if (count == 0)
                {

                    double targetEmployee_group = getTargetTIFREmployeeSrilanka("00000000", date_start, date_end, lang);
                    int lti_group = getLTIEmployeeSrilanka("",date_start, date_end, lang, Session["country"].ToString());
                    int fatality_group = getFatalityEmployeeSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int pd_group = getPDEmployeeSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int mti_group = getMTIEmployeeSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int mi_group = getMIEmployeeSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int total_group = lti_group + fatality_group + pd_group + mti_group + mi_group;
                    double workhour_group = getWorkhourEmployeeSrilanka("", date_start, date_end, lang);
                    double multiplier_group = getMultiplierSrilanka("00000000", date_start, date_end, lang);
                    double tifr_group = 0;

                    if (workhour_group != 0)
                    {
                        tifr_group = caclLTIFRANDTIFR(total_group, multiplier_group, workhour_group);
                    }

                    thbody = "<tr>";
                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    thbody = thbody + "<td>" + insee_group + "</td>";
                    thbody = thbody + "<td>" + targetEmployee_group + "</td>";
                    thbody = thbody + "<td>" + fatality_group + "</td>";
                    thbody = thbody + "<td>" + pd_group + "</td>";
                    thbody = thbody + "<td>" + lti_group + "</td>";
                    thbody = thbody + "<td>" + mti_group + "</td>";
                    thbody = thbody + "<td>" + mi_group + "</td>";
                    thbody = thbody + "<td>" + total_group + "</td>";
                    thbody = thbody + "<td>" + workhour_group + "</td>";
                    thbody = thbody + "<td>" + tifr_group + "</td>";

                    thbody = thbody + "</tr>";
                }

                string tr_start = "<tr>";
                thbody = thbody + tr_start;

                thbody = thbody + "<td>" + rc.name+ "</td>";
                thbody = thbody + "<td>" + targetEmployee + "</td>";
                thbody = thbody + "<td>" + fatality + "</td>";
                thbody = thbody + "<td>" + pd + "</td>";
                thbody = thbody + "<td>" + lti + "</td>";
                thbody = thbody + "<td>" + mti + "</td>";
                thbody = thbody + "<td>" + mi + "</td>";
                thbody = thbody + "<td>" + total + "</td>";
                thbody = thbody + "<td>" + workhour + "</td>";
                thbody = thbody + "<td>" + tifr + "</td>";
                string tr_end = "</tr>";
                thbody = thbody + tr_end;
                count++;

            }

            var result = new
            {
                data = thbody
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFRContractorOnsiteCompanyReport(string companyid, string date_start, string date_end, string lang)
        {
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

            int count = 0;
            ArrayList dataJson = new ArrayList();


            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();

                if (count == 0)
                {

                    double targetContractorOnsite_group = getTargetTIFRContractorOnsiteGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOnsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_group = getFatalityContractorOnsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int pd_group = getPDContractorOnsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int mti_group = getMTIContractorOnsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int mi_group = getMIContractorOnsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_group = getRWCContractorOnsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int total_group = lti_group + fatality_group + pd_group + mti_group + mi_group + rwc_group;
                    double workhour_group = getWorkhourContractorOnsite("","", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorGroup("00000000", date_start, date_end, lang);
                    double tifr_group = 0;

                    if (workhour_group != 0)
                    {
                        tifr_group = caclLTIFRANDTIFR(total_group, multiplier_group, workhour_group);
                    }

                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    dt.Add("");
                    dt.Add(insee_group);
                    dt.Add(targetContractorOnsite_group.ToString("F2"));
                    dt.Add(fatality_group);
                    dt.Add(pd_group);
                    dt.Add(lti_group);
                    dt.Add(rwc_group);
                    dt.Add(mti_group);
                    dt.Add(mi_group);
                    dt.Add(total_group);
                    dt.Add(String.Format("{0:n}", workhour_group));
                    dt.Add(tifr_group.ToString("F2"));

                    dataJson.Add(dt);
                }


                dt = new ArrayList();
                double targetContractorOnsite = getTargetTIFRContractorOnsite(rc.company_id, "","", date_start, date_end, lang);
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


                dt.Add(rc.company_id);
                dt.Add(rc.company_name);
                dt.Add(targetContractorOnsite.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));

                dataJson.Add(dt);

                count++;
            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFRContractorOnsiteFunctionReport(string companyid, string functionid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.functions
                     where d.company_id == companyid
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
                v1 = v1.Where(c => c.function_id == functionid);

            }

            int count = 0;
            ArrayList dataJson = new ArrayList();


            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();


                double targetContractorOnsite = getTargetTIFRContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang);
                int lti = getLTIContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourContractorOnsite(companyid, rc.function_id, "", date_start, date_end, lang);
                double multiplier = getMultiplierContractor(companyid, rc.function_id, "", date_start, date_end, lang);
                double tifr = 0;


                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }


                dt.Add(rc.function_id);
                dt.Add(rc.function_name);
                dt.Add(targetContractorOnsite.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));

                dataJson.Add(dt);

                count++;
            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFRContractorOnsiteDepartmentReport(string functionid, string departmentid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.departments
                     where d.function_id == functionid
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
                v1 = v1.Where(c => c.department_id == departmentid);

            }

            int count = 0;
            ArrayList dataJson = new ArrayList();


            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();


                double targetContractorOnsite = getTargetTIFRContractorOnsite("", functionid, rc.department_id, date_start, date_end, lang);
                int lti = getLTIContractorOnsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityContractorOnsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDContractorOnsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIContractorOnsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIContractorOnsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCContractorOnsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourContractorOnsite("", functionid, rc.department_id, date_start, date_end, lang);
                double multiplier = getMultiplierContractor("", functionid, rc.department_id, date_start, date_end, lang);
                double tifr = 0;


                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }


                dt.Add(rc.department_id);
                dt.Add(rc.department_name);
                dt.Add(targetContractorOnsite.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));

                dataJson.Add(dt);

                count++;
            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFRContractorOnsiteReportSrilanka(string siteid, string date_start, string date_end, string lang)
        {
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

            int count = 0;
            string thbody = "";


            foreach (var rc in v1)
            {
                double targetContractorOnsite = getTargetTIFRContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang);
                int lti = getLTIContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi;
                double workhour = getWorkhourContractorOnsiteSrilanka(rc.site_id, date_start, date_end, lang);
                double multiplier = getMultiplierContractorSrilanka(rc.site_id, date_start, date_end, lang);
                double tifr = 0;


                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }

                if (count == 0)
                {

                    double targetContractorOnsite_group = getTargetTIFRContractorOnsiteSrilanka("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOnsiteSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_group = getFatalityContractorOnsiteSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int pd_group = getPDContractorOnsiteSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int mti_group = getMTIContractorOnsiteSrilanka("",date_start, date_end, lang, Session["country"].ToString());
                    int mi_group = getMIContractorOnsiteSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int total_group = lti_group + fatality_group + pd_group + mti_group + mi_group;
                    double workhour_group = getWorkhourContractorOnsiteSrilanka("", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorSrilanka("00000000", date_start, date_end, lang);
                    double tifr_group = 0;

                    if (workhour_group != 0)
                    {
                        tifr_group = caclLTIFRANDTIFR(total_group, multiplier_group, workhour_group);
                    }

                    thbody = "<tr>";
                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    thbody = thbody + "<td>" + insee_group + "</td>";
                    thbody = thbody + "<td>" + targetContractorOnsite_group + "</td>";
                    thbody = thbody + "<td>" + fatality_group + "</td>";
                    thbody = thbody + "<td>" + pd_group + "</td>";
                    thbody = thbody + "<td>" + lti_group + "</td>";
                    thbody = thbody + "<td>" + mti_group + "</td>";
                    thbody = thbody + "<td>" + mi_group + "</td>";
                    thbody = thbody + "<td>" + total_group + "</td>";
                    thbody = thbody + "<td>" + workhour_group + "</td>";
                    thbody = thbody + "<td>" + tifr_group + "</td>";

                    thbody = thbody + "</tr>";
                }

                string tr_start = "<tr>";
                thbody = thbody + tr_start;

                thbody = thbody + "<td>" + rc.name + "</td>";
                thbody = thbody + "<td>" + targetContractorOnsite + "</td>";
                thbody = thbody + "<td>" + fatality + "</td>";
                thbody = thbody + "<td>" + pd + "</td>";
                thbody = thbody + "<td>" + lti + "</td>";
                thbody = thbody + "<td>" + mti + "</td>";
                thbody = thbody + "<td>" + mi + "</td>";
                thbody = thbody + "<td>" + total + "</td>";
                thbody = thbody + "<td>" + workhour + "</td>";
                thbody = thbody + "<td>" + tifr + "</td>";
                string tr_end = "</tr>";
                thbody = thbody + tr_end;
                count++;
            }

            var result = new
            {
                data = thbody
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));



        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFRContractorOffsiteCompanyReport(string companyid, string date_start, string date_end, string lang)
        {
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


            int count = 0;
            ArrayList dataJson = new ArrayList();

            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();
                if (count == 0)
                {

                    double targetContractorOffsite_group = getTargetTIFRContractorOffsiteGroup("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOffsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_group = getFatalityContractorOffsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int pd_group = getPDContractorOffsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int mti_group = getMTIContractorOffsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int mi_group = getMIContractorOffsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int rwc_group = getRWCContractorOffsite("","", "", date_start, date_end, lang, Session["country"].ToString());
                    int total_group = lti_group + fatality_group + pd_group + mti_group + mi_group + rwc_group;
                    double workhour_group = getWorkhourContractorOffsite("","", "", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorOffsiteGroup("00000000", date_start, date_end, lang);
                    double tifr_group = 0;

                    if (workhour_group != 0)
                    {
                        tifr_group = caclLTIFRANDTIFR(total_group, multiplier_group, workhour_group);
                    }


 
                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);


                    dt.Add("");
                    dt.Add(insee_group);
                    dt.Add(targetContractorOffsite_group.ToString("F2"));
                    dt.Add(fatality_group);
                    dt.Add(pd_group);
                    dt.Add(lti_group);
                    dt.Add(rwc_group);
                    dt.Add(mti_group);
                    dt.Add(mi_group);
                    dt.Add(total_group);
                    dt.Add(String.Format("{0:n}", workhour_group));
                    dt.Add(tifr_group.ToString("F2"));

                    dataJson.Add(dt);
                }

                dt = new ArrayList();
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


                dt.Add(rc.company_id);
                dt.Add(rc.company_name);
                dt.Add(targetContractorOffsite.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFRContractorOffsiteFunctionReport(string companyid, string functionid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.functions
                     where d.company_id == companyid
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
                v1 = v1.Where(c => c.function_id == functionid);

            }


            int count = 0;
            ArrayList dataJson = new ArrayList();

            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();


                double targetContractorOffsite = getTargetTIFRContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang);
                int lti = getLTIContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang);
                double multiplier = getMultiplierContractorOffsite(companyid, rc.function_id, "", date_start, date_end, lang);
                double tifr = 0;
                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }


                dt.Add(rc.function_id);
                dt.Add(rc.function_name);
                dt.Add(targetContractorOffsite.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFRContractorOffsiteDepartmentReport(string functionid, string departmentid, string date_start, string date_end, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v1 = from d in dbConnect.departments
                     where d.function_id == functionid
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
                v1 = v1.Where(c => c.department_id == departmentid);

            }


            int count = 0;
            ArrayList dataJson = new ArrayList();

            foreach (var rc in v1)
            {
                ArrayList dt = new ArrayList();


                double targetContractorOffsite = getTargetTIFRContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang);
                int lti = getLTIContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int rwc = getRWCContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi + rwc;
                double workhour = getWorkhourContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang);
                double multiplier = getMultiplierContractorOffsite("", functionid, rc.department_id, date_start, date_end, lang);
                double tifr = 0;
                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }


                dt.Add(rc.department_id);
                dt.Add(rc.department_name);
                dt.Add(targetContractorOffsite.ToString("F2"));
                dt.Add(fatality);
                dt.Add(pd);
                dt.Add(lti);
                dt.Add(rwc);
                dt.Add(mti);
                dt.Add(mi);
                dt.Add(total);
                dt.Add(String.Format("{0:n}", workhour));
                dt.Add(tifr.ToString("F2"));

                dataJson.Add(dt);

                count++;

            }

            var result = new
            {
                rows = dataJson
            };


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]

        public void getListTIFRContractorOffsiteReportSrilanka(string siteid, string date_start, string date_end, string lang)
        {
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

           


            int count = 0;
            string thbody = "";

            foreach (var rc in v1)
            {
                double targetContractorOffsite = getTargetTIFRContractorOffsiteSrilanka(rc.site_id, date_start, date_end, lang);
                int lti = getLTIContractorOffsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int fatality = getFatalityContractorOffsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int pd = getPDContractorOffsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int mti = getMTIContractorOffsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int mi = getMIContractorOffsiteSrilanka(rc.site_id, date_start, date_end, lang, Session["country"].ToString());
                int total = lti + fatality + pd + mti + mi;
                double workhour = getWorkhourContractorOffsiteSrilanka(rc.site_id,date_start, date_end, lang);
                double multiplier = getMultiplierContractorSrilanka(rc.site_id, date_start, date_end, lang);
                double tifr = 0;
                if (workhour != 0)
                {
                    tifr = caclLTIFRANDTIFR(total, multiplier, workhour);
                }

                if (count == 0)
                {

                    double targetContractorOffsite_group = getTargetTIFRContractorOffsiteSrilanka("00000000", date_start, date_end, lang);
                    int lti_group = getLTIContractorOffsiteSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int fatality_group = getFatalityContractorOffsiteSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int pd_group = getPDContractorOffsiteSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int mti_group = getMTIContractorOffsiteSrilanka("", date_start, date_end, lang, Session["country"].ToString());
                    int mi_group = getMIContractorOffsiteSrilanka("",  date_start, date_end, lang, Session["country"].ToString());
                    int total_group = lti_group + fatality_group + pd_group + mti_group + mi_group;
                    double workhour_group = getWorkhourContractorOffsiteSrilanka("", date_start, date_end, lang);
                    double multiplier_group = getMultiplierContractorSrilanka("00000000", date_start, date_end, lang);
                    double tifr_group = 0;

                    if (workhour_group != 0)
                    {
                        tifr_group = caclLTIFRANDTIFR(total_group, multiplier_group, workhour_group);
                    }


                    thbody = "<tr>";
                    string insee_group = chageDataLanguage("กลุ่มบริษัทอินทรี", "INSEE Group Company", lang);

                    thbody = thbody + "<td>" + insee_group + "</td>";
                    thbody = thbody + "<td>" + targetContractorOffsite_group + "</td>";
                    thbody = thbody + "<td>" + fatality_group + "</td>";
                    thbody = thbody + "<td>" + pd_group + "</td>";
                    thbody = thbody + "<td>" + lti_group + "</td>";
                    thbody = thbody + "<td>" + mti_group + "</td>";
                    thbody = thbody + "<td>" + mi_group + "</td>";
                    thbody = thbody + "<td>" + total_group + "</td>";
                    thbody = thbody + "<td>" + workhour_group + "</td>";
                    thbody = thbody + "<td>" + tifr_group + "</td>";

                    thbody = thbody + "</tr>";
                }

                string tr_start = "<tr>";
                thbody = thbody + tr_start;

                thbody = thbody + "<td>" + rc.name + "</td>";
                thbody = thbody + "<td>" + targetContractorOffsite + "</td>";
                thbody = thbody + "<td>" + fatality + "</td>";
                thbody = thbody + "<td>" + pd + "</td>";
                thbody = thbody + "<td>" + lti + "</td>";
                thbody = thbody + "<td>" + mti + "</td>";
                thbody = thbody + "<td>" + mi + "</td>";
                thbody = thbody + "<td>" + total + "</td>";
                thbody = thbody + "<td>" + workhour + "</td>";
                thbody = thbody + "<td>" + tifr + "</td>";
                string tr_end = "</tr>";
                thbody = thbody + tr_end;
                count++;

            }

            var result = new
            {
                data = thbody
            };

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(result));




        }


        public double caclFatalityRate(int fatality, double workhour)
        {
            //10,000 ค่าจากพี่ออย
            double value = Math.Round((fatality * 10000) / workhour, 2);

            return value;
        }

        public double caclLTIFRANDTIFR(int lti,double multiplier, double workhour)
        {

            double value = Math.Round((lti * multiplier) / workhour,2);

            return value;
        }



        public double caclLTISR(int day_lost, double multiplier, double workhour)
        {

            double value = Math.Round((day_lost * multiplier) / workhour, 2);

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


        public double getMultiplierContractorSrilanka(string site_id,string date_start, string date_end, string lang)
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
                if (rc.multiplier_contractor > value)
                {
                    value = Convert.ToDouble(rc.multiplier_contractor);
                }
            }

            return value;
        }



        public double getTargetLTIFREmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
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





        public double getTargetLTIFRContractorOnsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
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



        public double getTargetLTIFRContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang)
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



       
        public double getTargetTIFREmployee(string company_id,string function_id,string department_id, string date_start, string date_end,string lang)
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




        public double getTargetTIFREmployeeSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_main_srilankas
                
                    select new
                    {
                        c.tifr_employee,
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



        public double getTargetTIFRContractorOnsiteGroup(string function_id,string date_start, string date_end, string lang)
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



        public double getTargetTIFRContractorOnsiteSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_main_srilankas
                 
                    select new
                    {
                        c.tifr_contractor_onsite,
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


        public double getTargetTIFRContractorOffsiteSrilanka(string site_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.target_main_srilankas
                  
                    select new
                    {
                        c.tifr_contractor_offsite,
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
                value = value + Convert.ToDouble(rc.tifr_contractor_offsite);
            }

            return value;
        }



        

        public int getLTIEmployee(string company_id,string function_id,string department_id, string date_start, string date_end,string lang,string country)
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




        public int getLTIEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    (c.type_employment_id == 5 || c.type_employment_id == 2 || c.type_employment_id == 1) //5 is offsite and 2 is contractor
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


        public int getDayLost(string company_id, string function_id, string department_id, string date_start, string date_end,string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where  i.country == country
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





        public int getDayLostEmployeeContractorOnsiteOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where i.country == country
                    && c.status == "A"
                    && (c.type_employment_id == 5 || c.type_employment_id == 2 || c.type_employment_id == 1)//1 is employee and 2 is contractor
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
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
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


        public int getFatalityEmployee(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where // c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 1 //1 is employee
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



        public int getFatalityEmployeeSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
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




        public int getPDEmployeeSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
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



        public int getMTIEmployeeSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();

            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
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




        public int getRWCEmployee(string company_id,string function_id, string department_id, string date_start, string date_end, string lang, string country)
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



        public int getMIEmployeeSrilanka(string site_id ,string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
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

        public int getLTIContractorOnsite(string company_id,string function_id,string department_id, string date_start, string date_end,string lang,string country)
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
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
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
                    && i.responsible_area == "IN"//onsite
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



        public int getFatalityContractorOnsiteSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 1 //1 is Fatality
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.responsible_area == "IN"//onsite
                    && i.country == country
                    && c.status == "A"
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
                    && i.responsible_area == "IN"//onsite
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



        public int getPDContractorOnsiteSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where
                        //c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 2//2 is PD
                    && i.responsible_area == "IN"//onsite
                    && i.process_status != 3//3 is reject
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
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
                    && i.responsible_area == "IN"//onsite
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



        public int getMTIContractorOnsiteSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where
                        //c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 4 //4 is MTI
                    && i.responsible_area == "IN"//onsite
                    && i.process_status != 3//3 is reject
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
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
                    && i.responsible_area == "IN"//onsite
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
                    && i.responsible_area == "IN"//onsite
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



        public int getMIContractorOnsiteSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where
                        //c.function_id == function_id && c.department_id == department_id 
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 5 //5 is MI
                    && i.responsible_area == "IN"//onsite
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
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




        public int getLTIContractorOffsite(string company_id,string function_id, string department_id, string date_start, string date_end,string lang,string country)
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
                    && c.status == "A"
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



        public int getFatalityContractorOffsiteSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 1 //1 is  Fatality
                    && i.responsible_area == "OUT"//offsite
                    && i.process_status != 3//3 is reject
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
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



        public int getPDContractorOffsiteSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where
                        //c.function_id == function_id && c.department_id == department_id
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 2 //2 is PD
                    && i.responsible_area == "OUT"//offsite
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
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


        public int getMTIContractorOffsiteSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where
                        //c.function_id == function_id && c.department_id == department_id
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 4 //4 is MTI
                    && i.responsible_area == "OUT"//offsite
                    && i.process_status != 3//3 is reject
                    && i.process_status != 4//4 is exemption
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
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


        public int getMIContractorOffsite(string company_id, string function_id, string department_id, string date_start, string date_end, string lang, string country)
        {
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join f in dbConnect.functions on c.function_id equals f.function_id
                    where
                        //c.function_id == function_id && c.department_id == department_id
                    c.type_employment_id == 5 //5 is contractor
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



        public int getMIContractorOffsiteSrilanka(string site_id, string date_start, string date_end, string lang, string country)
        {
            List<string> ls_sub = new List<string>();
            int value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where
                        //c.function_id == function_id && c.department_id == department_id
                    c.type_employment_id == 2 //1 is contractor
                    && c.severity_injury_id == 5 //5 is MI
                    && i.responsible_area == "OUT"//offsite
                    && i.process_status != 3//3 is reject
                    && (i.culpability == "G" || i.culpability == "P")
                    && i.country == country
                    && c.status == "A"
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

        public double getWorkhourEmployee(string company_id,string function_id,string department_id,string date_start,string date_end,string lang)
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


        public double getWorkhourContractorOnsite(string company_id,string function_id, string department_id, string date_start, string date_end,string lang)
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



        public double getWorkhourContractorOffsite(string company_id,string function_id, string department_id, string date_start, string date_end, string lang)
        {
            double value = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.workhour_subs
                    join di in dbConnect.divisions on c.division_id equals di.division_id
                    join de in dbConnect.departments on di.department_id equals de.department_id
                    join f in dbConnect.functions on de.function_id equals f.function_id
                    join co in dbConnect.companies on f.company_id equals co.company_id

                    select new
                    {   c.contractor_offsite,
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



        public double getWorkhourContractorOffsiteSrilanka(string site_id ,string date_start, string date_end, string lang)
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
                    value = value + ", " +  rc.first_name + " " + rc.last_name;
                   
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

            }else if (lang == "si")
            {

                vReturn = vEN;

            }


            return vReturn;
        }


    }
}
