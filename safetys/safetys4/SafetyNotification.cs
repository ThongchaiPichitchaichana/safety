using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Globalization;
using System.IO;

namespace safetys4.Class
{
    public class SafetyNotification
    {
        public void InsertNotification(int type, int incident_id, String[] groups, string timezone, string role_action, int action_id = 0, string type_action = "")
        {

            //var section_id = "";
            //var type = 1;

            //getrelateuser
            safetys4.App_Code.SafetyEmail se = new safetys4.App_Code.SafetyEmail();
            //String body = se.setTemplate(type, incident_id, receiver, doc_no, title, occur_date, description);

            //String subject  = se.getSubjectName();

            //DataClasses1DataContext dbConnect = new DataClasses1DataContext();
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var incidents = from c in dbConnect.incidents
                            join co in dbConnect.companies on c.company_id equals co.company_id
                            join f in dbConnect.functions on c.function_id equals f.function_id
                            join d in dbConnect.departments on c.department_id equals d.department_id
                            join r in dbConnect.reason_rejects on c.reason_reject_type equals r.id into joinReason
                            join c2 in dbConnect.companies on c.activity_company_id equals c2.company_id into joinC2
                            join f2 in dbConnect.functions on c.activity_function_id equals f2.function_id into joinF2
                            join de2 in dbConnect.departments on c.activity_department_id equals de2.department_id into joinDe2
                            join di2 in dbConnect.divisions on c.activity_division_id equals di2.division_id into joinD2
                            join se2 in dbConnect.sections on c.activity_section_id equals se2.section_id into joinS2
                            from r in joinReason.DefaultIfEmpty()
                            from c2 in joinC2.DefaultIfEmpty()
                            from f2 in joinF2.DefaultIfEmpty()
                            from de2 in joinDe2.DefaultIfEmpty()
                            from di2 in joinD2.DefaultIfEmpty()
                            from se2 in joinS2.DefaultIfEmpty()
                            where c.id == incident_id
                            select new
                            {
                                company_id = c.company_id,
                                company_en = co.company_en,
                                company_th = co.company_th,
                                function_id = c.function_id,
                                function_en = f.function_en,
                                function_th = f.function_th,
                                department_id = c.department_id,
                                department_en = d.department_en,
                                department_th = d.department_th,
                                division_id = c.division_id,
                                section_id = c.section_id,

                                activity_company_id = c.activity_company_id,
                                activity_company_en = c2.company_en,
                                activity_company_th = c2.company_th,
                                activity_function_id = c.activity_function_id,
                                activity_function_en = f2.function_en,
                                activity_function_th = f2.function_th,
                                activity_department_id = c.activity_department_id,
                                activity_department_en = de2.department_en,
                                activity_department_th = de2.department_th,
                                activity_division_id = c.activity_division_id,
                                activity_section_id = c.activity_section_id,

                                incident_id = c.id,
                                incident_code = c.doc_no,
                                title = c.incident_name,
                                description = c.incident_detail,
                                occur_date = c.incident_date,
                                incident_area = c.incident_area,
                                immediate_temporary = c.immediate_temporary,
                                reason_reject_en = r.name_en,
                                reason_reject_th = r.name_th,
                                reason_comment = c.reason_reject,
                                c.country,
                                reporter_id = c.employee_id,
                                c.typeuser_login,
                                c.responsible_area,
                                c.owner_activity

                            };

            String company_id = "";
            String company_en = "";
            String company_th = "";
            String company_name = "";
            String function_id = "";
            String function_en = "";
            String function_th = "";
            String function_name = "";
            String department_id = "";
            String department_en = "";
            String department_th = "";
            String department_name = "";
            String division_id = "";
            String section_id = "";

            String activity_company_id = "";
            String activity_company_en = "";
            String activity_company_th = "";
            String activity_company_name = "";
            String activity_function_id = "";
            String activity_function_en = "";
            String activity_function_th = "";
            String activity_function_name = "";
            String activity_department_id = "";
            String activity_department_en = "";
            String activity_department_th = "";
            String activity_department_name = "";
            String activity_division_id = "";
            String activity_section_id = "";
            String incident_code = "";
            String title = "";
            String description = "";
            DateTime occur_date = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
            String incident_area = "";
            String immediate_temporary = "";
            String reason_reject_th = "";
            String reason_reject_en = "";
            String reason_comment = "";
            String country = "";
            String reporter_id = "";
            String typelogin = "";
            String responsible_area = "";
            String owner_activity = "";

            foreach (var v in incidents)
            {
                company_id = v.company_id;
                company_en = v.function_en;
                company_th = v.company_th;
                company_name = v.company_en;
                function_id = v.function_id;
                function_en = v.function_en;
                function_th = v.function_th;
                function_name = v.function_en;
                department_id = v.department_id;
                department_en = v.department_en;
                department_th = v.department_th;
                department_name = v.department_en;
                division_id = v.division_id;
                section_id = v.section_id;

                activity_company_id = v.activity_company_id;
                activity_company_en = v.activity_company_en;
                activity_company_th = v.activity_company_th;
                activity_company_name = v.activity_company_en;
                activity_function_id = v.activity_function_id;
                activity_function_en = v.activity_function_en;
                activity_function_th = v.activity_function_th;
                activity_function_name = v.activity_function_en;
                activity_department_id = v.activity_department_id;
                activity_department_en = v.activity_department_en;
                activity_department_th = v.activity_department_th;
                activity_department_name = v.activity_department_en;
                activity_division_id = v.activity_division_id;
                activity_section_id = v.activity_section_id;

                incident_code = v.incident_code;
                title = v.title;
                description = v.description;
                occur_date = (DateTime)v.occur_date;
                incident_area = v.incident_area;
                immediate_temporary = v.immediate_temporary;
                reason_reject_th = v.reason_reject_th;
                reason_reject_en = v.reason_reject_en;
                reason_comment = v.reason_comment;
                country = v.country;
                reporter_id = v.reporter_id;
                typelogin = v.typeuser_login;
                responsible_area = v.responsible_area;
                owner_activity = v.owner_activity;
            }


            ArrayList sq_type = new ArrayList();//type มากจาก alert
            sq_type.Add(9);
            sq_type.Add(10);
            sq_type.Add(18);
            sq_type.Add(20);
            sq_type.Add(21);
            sq_type.Add(22);
            sq_type.Add(23);
            String action_detail = "";
            String assign_action_by = "";
            DateTime? action_due_date = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
            ArrayList tmpEmail = new ArrayList();
            ArrayList tmpEmID = new ArrayList();

            String injury_person_name = "";
            String nature_injury_name = "";

            ArrayList limages = new ArrayList();
            limages = getImageIncident(incident_id);

            bool location_area = false;
            bool owner_activity_area = false;


            if (responsible_area == "IN")
            {
                if (owner_activity == "KNOWN")
                {
                    if (type == 1 || type == 16)//create and update incident ถ้าเข้าอันนี้มันจะส่งหาทั้งสอง พื้นที่,//serious incident ส่งหาผู้บริหารระดับ sml ขึ้นไป ของทั้งสถานที่เกิดเหตุ และผู้ควบคุม ถ้ามีค่าทั้งสองนะ
                    {
                        location_area = true;

                    }

                }
                else
                {
                    location_area = true;
                }

            }
            else if (responsible_area == "OUT")
            {
                if (String.IsNullOrEmpty(owner_activity))//ศรี่ลังกา คง flow เดิม
                {
                    location_area = true;
                }
                else
                {
                    location_area = false;
                }

            }
            else//ศรีลังกากรณี step 1 ยังไม่มีค่า หรือ มีค่าแล้วในฟอร์ม 2,3,4 ก็ต้องทำส่วนนี้ //คงการทำงานเดิม
            {
                location_area = true;

            }



            if (owner_activity == "KNOWN")
            {
                owner_activity_area = true;


                company_en = activity_function_en;
                company_th = activity_company_th;
                company_name = activity_company_en;

                function_en = activity_function_en;
                function_th = activity_function_th;
                function_name = activity_function_en;

                department_en = activity_department_en;
                department_th = activity_department_th;
                department_name = activity_department_en;


            }




            if (type == 3)
            {
                var sv_injuries = from c in dbConnect.injury_persons
                                  join b in dbConnect.employees on c.employee_id equals b.employee_id
                                  join d in dbConnect.employees on b.supervisor_id equals d.employee_id
                                  join inj in dbConnect.nature_injuries on c.nature_injury_id equals inj.id
                                  where c.incident_id == incident_id
                                  select new
                                  {
                                      name_en = d.first_name_en + " " + d.last_name_en,
                                      name_th = d.first_name_th + " " + d.last_name_th,
                                      user_id = d.employee_id,
                                      email = d.email,
                                      injury_person_name = b.first_name_en + " " + b.last_name_en,
                                      nature_injury_name = inj.name_en

                                  };

                foreach (var v in sv_injuries)
                {
                    injury_person_name = v.injury_person_name;
                    nature_injury_name = v.nature_injury_name;
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getSubjectName(type, incident_code, title);
                            String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                            se.SendEmail(Convert.ToString(type),v.user_id,v.name_en,v.email, subject, body, timezone);
                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);
                        tmpEmID.Add(v.user_id);
                    }
                }
            }

            if ((type >= 4 && type <= 10 && type != 6 && action_id > 0) || (type == 25 || type == 26 || type == 27))//6 is request to close action ไม่ส่งหาตัวเอง
            {

                if (type_action == "corrective")
                {

                    var actions = from c in dbConnect.corrective_prevention_action_incidents
                                  join em in dbConnect.employees on c.employee_id equals em.employee_id
                                  join aem in dbConnect.employees on c.assign_by_employee_id equals aem.employee_id
                                  where c.id == action_id
                                  select new
                                  {
                                      action_id = c.id,
                                      em.supervisor_id,
                                      em.mngt_level,
                                      action_detail = c.corrective_preventive_action,
                                      action_due_date = c.due_date,
                                      em_id = em.employee_id,
                                      em_email = em.email,
                                      em_name_en = em.first_name_en + " " + em.last_name_en,
                                      contractor_id = c.contractor_id,
                                      assign_action_by = aem.first_name_en + " " + aem.last_name_en,
                                      c.incident_id
                                  };
                    foreach (var v in actions)
                    {
                        action_detail = v.action_detail;
                        action_due_date = v.action_due_date;
                        assign_action_by = v.assign_action_by;
                        if (v.em_email != "")
                        {
                            if (!tmpEmail.Contains(v.em_email))
                            {

                                if(type == 25 || type == 26 || type == 27)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    
                                    
                                    stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.em_id);

                                }
                                else
                                {
                                    String subject = "(For your action) " + se.getSubjectName(type, incident_code, title);
                                    if (sq_type.Contains(type))
                                    {
                                        string sq = getSequenceEmail(type, v.incident_id, "incident", v.em_id, v.em_email, title, subject, timezone);
                                        subject = subject + " (" + sq + ")";
                                    }
                                    String body = se.setTemplate(type, incident_id, v.em_name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                    se.SendEmail(Convert.ToString(type),v.em_id,v.em_name_en,v.em_email, subject, body, timezone);
                                    tmpEmail.Add(v.em_email);

                                }

                            }
                        }

                        if (!tmpEmID.Contains(v.em_id))
                        {
                            se.insertIncidentNotify(type, incident_id, v.em_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);
                            tmpEmID.Add(v.em_id);
                        }

                        if (v.contractor_id != null)
                        {
                            var contractors = from c in dbConnect.contractors
                                              where c.id == v.contractor_id
                                              select new
                                              {
                                                  con_name_en = c.first_name_en + " " + c.last_name_en,
                                                  con_email = c.email
                                              };
                            foreach (var vv in contractors)
                            {
                                if (vv.con_email != "")
                                {
                                    if (!tmpEmail.Contains(vv.con_email))
                                    {
                                        String subject = se.getSubjectName(type, incident_code, title);
                                        String con_subject = se.getSubjectName(5, incident_code, title);
                                        String body = se.setTemplate(5, incident_id, vv.con_name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                        se.SendEmail(Convert.ToString(type),v.em_id,v.em_name_en,vv.con_email, subject, body, timezone);
                                        tmpEmail.Add(vv.con_email);
                                    }
                                }

                            }
                        }
                    }




                }
                else if (type_action == "preventive")
                {
                    var actions = from c in dbConnect.preventive_action_incidents
                                  join em in dbConnect.employees on c.employee_id equals em.employee_id
                                  join aem in dbConnect.employees on c.assign_by_employee_id equals aem.employee_id
                                  where c.id == action_id
                                  select new
                                  {
                                      action_id = c.id,
                                      em.supervisor_id,
                                      em.mngt_level,
                                      action_detail = c.preventive_action,
                                      action_due_date = c.due_date,
                                      em_id = em.employee_id,
                                      em_email = em.email,
                                      em_name_en = em.first_name_en + " " + em.last_name_en,
                                      contractor_id = c.contractor_id,
                                      assign_action_by = aem.first_name_en + " " + aem.last_name_en,
                                      c.incident_id
                                  };
                    foreach (var v in actions)
                    {
                        action_detail = v.action_detail;
                        action_due_date = v.action_due_date;
                        assign_action_by = v.assign_action_by;
                        if (v.em_email != "")
                        {
                            if (!tmpEmail.Contains(v.em_email))
                            {
                                if (type == 25 || type == 26 || type == 27)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.em_id);

                                }
                                else
                                {

                                    String subject = "(For your action) " + se.getSubjectName(type, incident_code, title);

                                    if (sq_type.Contains(type))
                                    {
                                        string sq = getSequenceEmail(type, v.incident_id, "incident", v.em_id, v.em_email, title, subject, timezone);
                                        subject = subject + " (" + sq + ")";

                                    }
                                    String body = se.setTemplate(type, incident_id, v.em_name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                    se.SendEmail(Convert.ToString(type), v.em_id,v.em_name_en,v.em_email, subject, body, timezone);
                                    tmpEmail.Add(v.em_email);
                                }


                            }
                        }

                        if (!tmpEmID.Contains(v.em_id))
                        {
                            se.insertIncidentNotify(type, incident_id, v.em_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);
                            tmpEmID.Add(v.em_id);
                        }

                        if (v.contractor_id != null)
                        {
                            var contractors = from c in dbConnect.contractors
                                              where c.id == v.contractor_id
                                              select new
                                              {
                                                  con_name_en = c.first_name_en + " " + c.last_name_en,
                                                  con_email = c.email
                                              };
                            foreach (var vv in contractors)
                            {
                                if (vv.con_email != "")
                                {
                                    if (!tmpEmail.Contains(vv.con_email))
                                    {
                                        String subject = se.getSubjectName(type, incident_code, title);
                                        String con_subject = se.getSubjectName(5, incident_code, title);
                                        String body = se.setTemplate(5, incident_id, vv.con_name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                        se.SendEmail(Convert.ToString(type), v.em_id, v.em_name_en, vv.con_email, subject, body, timezone);
                                        tmpEmail.Add(vv.con_email);
                                    }
                                }

                            }
                        }
                    }




                }
                else if (type_action == "consequence")
                {
                    var actions = from c in dbConnect.consequence_management_incidents
                                  join em in dbConnect.employees on c.employee_id equals em.employee_id
                                  join aem in dbConnect.employees on c.assign_by_employee_id equals aem.employee_id
                                  where c.id == action_id
                                  select new
                                  {
                                      action_id = c.id,
                                      em.supervisor_id,
                                      em.mngt_level,
                                      action_detail = c.consequence_management,
                                      action_due_date = c.due_date,
                                      em_id = em.employee_id,
                                      em_email = em.email,
                                      em_name_en = em.first_name_en + " " + em.last_name_en,
                                      contractor_id = c.contractor_id,
                                      assign_action_by = aem.first_name_en + " " + aem.last_name_en,
                                      c.incident_id
                                  };
                    foreach (var v in actions)
                    {
                        action_detail = v.action_detail;
                        action_due_date = v.action_due_date;
                        assign_action_by = v.assign_action_by;
                        if (v.em_email != "")
                        {
                            if (!tmpEmail.Contains(v.em_email))
                            {
                                if (type == 25 || type == 26 || type == 27)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.em_id);

                                }
                                else
                                {
                                    String subject = "(For your action) " + se.getSubjectName(type, incident_code, title);

                                    if (sq_type.Contains(type))
                                    {
                                        string sq = getSequenceEmail(type, v.incident_id, "incident", v.em_id, v.em_email, title, subject, timezone);
                                        subject = subject + " (" + sq + ")";

                                    }
                                    String body = se.setTemplate(type, incident_id, v.em_name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                    se.SendEmail(Convert.ToString(type), v.em_id, v.em_name_en, v.em_email, subject, body, timezone);
                                    tmpEmail.Add(v.em_email);
                                }


                            }
                        }

                        if (!tmpEmID.Contains(v.em_id))
                        {
                            se.insertIncidentNotify(type, incident_id, v.em_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);
                            tmpEmID.Add(v.em_id);
                        }

                        if (v.contractor_id != null)
                        {
                            var contractors = from c in dbConnect.contractors
                                              where c.id == v.contractor_id
                                              select new
                                              {
                                                  con_name_en = c.first_name_en + " " + c.last_name_en,
                                                  con_email = c.email
                                              };
                            foreach (var vv in contractors)
                            {
                                if (vv.con_email != "")
                                {
                                    if (!tmpEmail.Contains(vv.con_email))
                                    {
                                        String subject = se.getSubjectName(type, incident_code, title);
                                        String con_subject = se.getSubjectName(5, incident_code, title);
                                        String body = se.setTemplate(5, incident_id, vv.con_name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                        se.SendEmail(Convert.ToString(type), v.em_id, v.em_name_en, vv.con_email, subject, body, timezone);
                                        tmpEmail.Add(vv.con_email);
                                    }
                                }

                            }
                        }
                    }




                }




            }



            if (groups.Contains("Reporter"))//คนแจ้ง
            {

                if (typelogin == "contractor")
                {
                    var reporters = from c in dbConnect.contractors
                                    where c.id == Convert.ToInt32(reporter_id)
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.id.ToString(),
                                        email = c.email
                                    };
                    foreach (var v in reporters)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);
                                if (role_action == "Reporter")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);
                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);
                                }

                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "Reporter")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }



                }
                else
                {
                    var reporters = from c in dbConnect.employees
                                    where c.employee_id == reporter_id
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email
                                    };
                    foreach (var v in reporters)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);
                                if (role_action == "Reporter")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);
                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);
                                }

                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "Reporter")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }



                }

            }




            if (groups.Contains("AdminOH&S"))
            {

                if (location_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                    join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    where (b.group_id == 4 || b.group_id == 5) && b.function_id == function_id
                                    && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };

                    ArrayList lsSupervisor = new ArrayList();
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);


                                if (type == 22 || type == 23)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                    {
                                        stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.user_id);
                                        lsSupervisor.Add(v.supervisor_id);
                                    }

                                }
                                else
                                {

                                    if (sq_type.Contains(type))
                                    {
                                        string sqemail =     getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                        subject = subject + " (" + sqemail + ")";

                                    }

                                    if (role_action == "AdminOH&S")
                                    {
                                        subject = "(For your action) " + subject;
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);

                                        if (type == 16)//serios incident
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);
                                        }
                                        else
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);
                                        }
                                    }
                                    else
                                    {
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);

                                        if (type == 16)//serios incident
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);
                                        }
                                        else
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);
                                        }
                                    }
                                }

                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "AdminOH&S")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }



                ////////////////////////////////////////////////////////////owner activity//////////////////////////////////////////////////////////////////////////

                if (owner_activity_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                    join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    where (b.group_id == 4 || b.group_id == 5) && b.function_id == activity_function_id
                                    && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };

                    ArrayList lsSupervisor = new ArrayList();
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);


                                if (type == 22 || type == 23)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                    {
                                        stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.user_id);
                                        lsSupervisor.Add(v.supervisor_id);
                                    }

                                }
                                else
                                {

                                    if (sq_type.Contains(type))
                                    {
                                        string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                        subject = subject + " (" + sqemail + ")";

                                    }

                                    if (role_action == "AdminOH&S")
                                    {
                                        subject = "(For your action) " + subject;
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, true);

                                        if (type == 16)//serios incident
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);
                                        }
                                        else
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);
                                        }
                                    }
                                    else
                                    {
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, true);

                                        if (type == 16)//serios incident
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);
                                        }
                                        else
                                        {
                                            se.SendEmail(Convert.ToString(type),v.user_id, v.name_en,v.email, subject, body, timezone);
                                        }
                                    }
                                }

                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "AdminOH&S")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }

            }

            if (groups.Contains("AreaSuperervisor"))
            {

                if (location_area)
                {
                    var area_super = from c in dbConnect.employee_has_sections
                                     join b in dbConnect.employees on c.employee_id equals b.employee_id
                                     where c.section_id == section_id && c.country == country
                                     select new
                                     {
                                         name_en = b.first_name_en + " " + b.last_name_en,
                                         name_th = b.first_name_th + " " + b.last_name_th,
                                         user_id = b.employee_id,
                                         email = b.email,
                                         b.supervisor_id,
                                         b.mngt_level
                                     };
                    ArrayList lsSupervisor = new ArrayList();
                    foreach (var v in area_super)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);



                                if (type == 22 || type == 23)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                    {
                                        stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.user_id);
                                        lsSupervisor.Add(v.supervisor_id);
                                    }

                                }
                                else
                                {
                                    if (sq_type.Contains(type))
                                    {
                                        string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                        subject = subject + " (" + sqemail + ")";

                                    }

                                    if (role_action == "AreaSuperervisor")
                                    {
                                        subject = "(For your action) " + subject;
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                    }
                                    else
                                    {
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                    }
                                }
                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "AreaSuperervisor")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }

                //////////////////////////////////////////////////////owner activity//////////////////////////////////////////////////////////////////////////////


                if (owner_activity_area)
                {
                    var area_super = from c in dbConnect.employee_has_sections
                                     join b in dbConnect.employees on c.employee_id equals b.employee_id
                                     where c.section_id == activity_section_id && c.country == country
                                     select new
                                     {
                                         name_en = b.first_name_en + " " + b.last_name_en,
                                         name_th = b.first_name_th + " " + b.last_name_th,
                                         user_id = b.employee_id,
                                         email = b.email,
                                         b.supervisor_id,
                                         b.mngt_level
                                     };
                    ArrayList lsSupervisor = new ArrayList();
                    foreach (var v in area_super)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);



                                if (type == 22 || type == 23)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                    {
                                        stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.user_id);
                                        lsSupervisor.Add(v.supervisor_id);
                                    }

                                }
                                else
                                {
                                    if (sq_type.Contains(type))
                                    {
                                        string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                        subject = subject + " (" + sqemail + ")";

                                    }

                                    if (role_action == "AreaSuperervisor")
                                    {
                                        subject = "(For your action) " + subject;
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, true);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                    }
                                    else
                                    {
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, true);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                    }
                                }
                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "AreaSuperervisor")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }


            }

            if (groups.Contains("AreaManager"))
            {

                if (location_area)
                {
                    var area_manager = from c in dbConnect.employee_has_divisions
                                       join b in dbConnect.employees on c.employee_id equals b.employee_id
                                       where c.division_id == division_id && c.country == country
                                       select new
                                       {
                                           name_en = b.first_name_en + " " + b.last_name_en,
                                           name_th = b.first_name_th + " " + b.last_name_th,
                                           user_id = b.employee_id,
                                           email = b.email,
                                           b.supervisor_id,
                                           b.mngt_level
                                       };
                    ArrayList lsSupervisor = new ArrayList();
                    foreach (var v in area_manager)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);


                                if (type == 22 || type == 23)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                    {
                                        stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.user_id);
                                        lsSupervisor.Add(v.supervisor_id);
                                    }


                                }
                                else
                                {
                                    if (sq_type.Contains(type))
                                    {
                                        string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                        subject = subject + " (" + sqemail + ")";

                                    }

                                    if (role_action == "AreaManager")
                                    {
                                        subject = "(For your action) " + subject;
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                    }
                                    else
                                    {
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);


                                    }
                                }
                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "AreaManager")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }


                }


                ///////////////////////////////////////////////////////////////owner activity/////////////////////////////////////////////////////////////////////////


                if (owner_activity_area)
                {
                    var area_manager = from c in dbConnect.employee_has_divisions
                                       join b in dbConnect.employees on c.employee_id equals b.employee_id
                                       where c.division_id == activity_division_id && c.country == country
                                       select new
                                       {
                                           name_en = b.first_name_en + " " + b.last_name_en,
                                           name_th = b.first_name_th + " " + b.last_name_th,
                                           user_id = b.employee_id,
                                           email = b.email,
                                           b.supervisor_id,
                                           b.mngt_level
                                       };
                    ArrayList lsSupervisor = new ArrayList();
                    foreach (var v in area_manager)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);


                                if (type == 22 || type == 23)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                    {
                                        stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.user_id);
                                        lsSupervisor.Add(v.supervisor_id);
                                    }


                                }
                                else
                                {
                                    if (sq_type.Contains(type))
                                    {
                                        string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                        subject = subject + " (" + sqemail + ")";

                                    }

                                    if (role_action == "AreaManager")
                                    {
                                        subject = "(For your action) " + subject;
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, true);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                    }
                                    else
                                    {
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, true);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);


                                    }
                                }
                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "AreaManager")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }


                }


            }

            if (groups.Contains("AreaOH&S"))
            {

                if (location_area)
                {
                    var area_ohs = from c in dbConnect.employee_has_departments
                                   join b in dbConnect.employees on c.employee_id equals b.employee_id
                                   where c.department_id == department_id && c.country == country
                                   select new
                                   {
                                       name_en = b.first_name_en + " " + b.last_name_en,
                                       name_th = b.first_name_th + " " + b.last_name_th,
                                       user_id = b.employee_id,
                                       email = b.email,
                                       b.supervisor_id,
                                       b.mngt_level
                                   };

                    ArrayList lsSupervisor = new ArrayList();
                    foreach (var v in area_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);


                                if (type == 22 || type == 23)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                    {
                                        stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.user_id);
                                        lsSupervisor.Add(v.supervisor_id);
                                    }

                                }
                                else
                                {
                                    if (sq_type.Contains(type))
                                    {
                                        string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                        subject = subject + " (" + sqemail + ")";

                                    }


                                    if (role_action == "AreaOH&S")
                                    {
                                        subject = "(For your action) " + subject;
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                    }
                                    else
                                    {
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                    }
                                }
                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "AreaOH&S")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }

                //////////////////////////////////////////////////////////owner activity/////////////////////////////////////////////////////////////////////////////////////////////

                if (owner_activity_area)
                {
                    var area_ohs = from c in dbConnect.employee_has_departments
                                   join b in dbConnect.employees on c.employee_id equals b.employee_id
                                   where c.department_id == activity_department_id && c.country == country
                                   select new
                                   {
                                       name_en = b.first_name_en + " " + b.last_name_en,
                                       name_th = b.first_name_th + " " + b.last_name_th,
                                       user_id = b.employee_id,
                                       email = b.email,
                                       b.supervisor_id,
                                       b.mngt_level
                                   };

                    ArrayList lsSupervisor = new ArrayList();
                    foreach (var v in area_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);


                                if (type == 22 || type == 23)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                    {
                                        stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.user_id);
                                        lsSupervisor.Add(v.supervisor_id);
                                    }

                                }
                                else
                                {
                                    if (sq_type.Contains(type))
                                    {
                                        string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                        subject = subject + " (" + sqemail + ")";

                                    }


                                    if (role_action == "AreaOH&S")
                                    {
                                        subject = "(For your action) " + subject;
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, true);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                    }
                                    else
                                    {
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, true);
                                        se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                    }
                                }
                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "AreaOH&S")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }


            }

            if (groups.Contains("GroupOH&S"))
            {

                if (location_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                    join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where (b.group_id == 8) && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };
                    ArrayList lsSupervisor = new ArrayList();
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);

                                if (type == 22 || type == 23)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                    {
                                        stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.user_id);
                                        lsSupervisor.Add(v.supervisor_id);
                                    }

                                }
                                else
                                {
                                    if (sq_type.Contains(type))
                                    {
                                        string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                        subject = subject + " (" + sqemail + ")";

                                    }


                                    if (role_action == "GroupOH&S")
                                    {
                                        subject = "(For your action) " + subject;
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                        if (type == 16)//serios incident
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);
                                        }
                                        else
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);
                                        }


                                    }
                                    else
                                    {
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                        if (type == 16)//serios incident
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);
                                        }
                                        else
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);
                                        }

                                    }
                                }
                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "GroupOH&S")
                            {

                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }


                /////////////////////////////////////////////////////////////owner activity////////////////////////////////////////////////////////////////////////////////////

                if (owner_activity_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                    join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where (b.group_id == 8) && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };
                    ArrayList lsSupervisor = new ArrayList();
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);

                                if (type == 22 || type == 23)//แจ้งหัวหน้าถ้ามา type นี้
                                {
                                    if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                    {
                                        stepSupervisor(v.supervisor_id, v.mngt_level, type, title, Convert.ToDateTime(occur_date), incident_code, incident_id, description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, v.user_id);
                                        lsSupervisor.Add(v.supervisor_id);
                                    }

                                }
                                else
                                {
                                    if (sq_type.Contains(type))
                                    {
                                        string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                        subject = subject + " (" + sqemail + ")";

                                    }


                                    if (role_action == "GroupOH&S")
                                    {
                                        subject = "(For your action) " + subject;
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, true);
                                        if (type == 16)//serios incident
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);
                                        }
                                        else
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);
                                        }


                                    }
                                    else
                                    {
                                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, true);
                                        if (type == 16)//serios incident
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);
                                        }
                                        else
                                        {
                                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);
                                        }

                                    }
                                }
                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "GroupOH&S")
                            {

                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }

            }

            if (groups.Contains("LegalDepartment"))
            {

                if (location_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                    join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where (b.group_id == 7) && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);


                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }


                                if (role_action == "LegalDepartment")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                }


                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "LegalDepartment")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }


                }


                ///////////////////////////////////////////////////////////////////////owner activity/////////////////////////////////////////////////////////////////////////////////

                if (owner_activity_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                    join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where (b.group_id == 7) && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);


                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }


                                if (role_action == "LegalDepartment")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                }


                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "LegalDepartment")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }


                }


            }

            if (groups.Contains("GroupCommunicationVP"))
            {

                if (location_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                    join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where (b.group_id == 6) && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);


                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }


                                if (role_action == "GroupCommunicationVP")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                }

                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "GroupCommunicationVP")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }



                }

                /////////////////////////////////////////////////////////////////////owner activity/////////////////////////////////////////////////////////////////////////////////

                if (owner_activity_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                    join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where (b.group_id == 6) && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);


                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }


                                if (role_action == "GroupCommunicationVP")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                }

                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "GroupCommunicationVP")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }



                }


            }

            if (groups.Contains("SML"))
            {
                if (type == 17)//serious incident
                {
                    //กรณี serious incident จ้องส่งเมล์แจ้ง sml ของทั้งสายงานสถานที่เกิดเหตุ และสายงานเจ้าของกิจกรรม ถ้าเข้าเงือนไข insite กับ known
                    if (responsible_area == "IN")
                    {
                        location_area = true;
                    }

                }

                if (location_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                        //join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where (c.employee_subgroup_text == "SML" && c.emp_group_text != "Temporary - Monthly") && o.function_id == function_id
                                     && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);

                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }


                                if (role_action == "SML")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }


                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "SML")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }


                }

                /////////////////////////////////////////////////////////////////////////owner activtiy//////////////////////////////////////////////////////////////////////////

                if (owner_activity_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                        //join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where (c.employee_subgroup_text == "SML" && c.emp_group_text != "Temporary - Monthly") && o.function_id == activity_function_id
                                     && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);

                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }


                                if (role_action == "SML")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }


                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "SML")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }


                }

            }

            if (groups.Contains("TML"))
            {

                if (location_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                        //join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where ((c.employee_subgroup_text == "TML") && c.emp_group_text != "Temporary - Monthly") && o.function_id == function_id
                                    && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);

                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }

                                if (role_action == "TML")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }


                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "TML")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }

                //////////////////////////////////////////////////////////////////////////owner activity////////////////////////////////////////////////////////////////////////////

                if (owner_activity_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                        //join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where ((c.employee_subgroup_text == "TML") && c.emp_group_text != "Temporary - Monthly") && o.function_id == activity_function_id
                                    && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };
                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);

                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }

                                if (role_action == "TML")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }


                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "TML")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }

            }





            if (groups.Contains("TML-EXCO"))
            {

                if (type == 17)//serious incident
                {
                    //กรณี serious incident จ้องส่งเมล์แจ้ง tml-exo ของทั้งสายงานสถานที่เกิดเหตุ และสายงานเจ้าของกิจกรรม ถ้าเข้าเงือนไข insite กับ known
                    if (responsible_area == "IN")
                    {
                        location_area = true;
                    }

                }

                if (location_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                    join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where (b.group_id == 19) && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };


                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);

                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }

                                if (role_action == "TML-EXCO")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }


                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "TML-EXCO")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }

                //////////////////////////////////////////////////////////////////////////////owner activity//////////////////////////////////////////////////////////////////////////////

                if (owner_activity_area)
                {
                    var admin_ohs = from c in dbConnect.employees
                                    join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                    where (b.group_id == 19) && c.country == country
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email,
                                        c.supervisor_id,
                                        c.mngt_level
                                    };


                    foreach (var v in admin_ohs)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getSubjectName(type, incident_code, title);

                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }

                                if (role_action == "TML-EXCO")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }
                                else
                                {
                                    String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                                }


                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "TML-EXCO")
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }

                }

            }








            if (groups.Contains("CEO"))
            {

                var admin_ohs = from c in dbConnect.employees
                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                where (b.group_id == 20) && c.country == country
                                select new
                                {
                                    name_en = c.first_name_en + " " + c.last_name_en,
                                    name_th = c.first_name_th + " " + c.last_name_th,
                                    user_id = c.employee_id,
                                    email = c.email,
                                    c.supervisor_id,
                                    c.mngt_level
                                };


                foreach (var v in admin_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getSubjectName(type, incident_code, title);

                            if (sq_type.Contains(type))
                            {
                                string sqemail = getSequenceEmail(type, incident_id, "incident", v.user_id, v.email, title, subject, timezone);
                                subject = subject + " (" + sqemail + ")";

                            }

                            if (role_action == "CEO")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, true, false);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                            }
                            else
                            {
                                String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone, limages);

                            }


                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "CEO")
                        {
                            se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertIncidentNotify(type, incident_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }

            }


        }

        public void InsertHazardNotification(int type, int hazard_id, String[] groups, string timezone, string role_action, int action_id = 0)
        {


            //var section_id = "";
            //var type = 1;

            //getrelateuser
            safetys4.App_Code.SafetyEmail se = new safetys4.App_Code.SafetyEmail();
            //String body = se.setTemplate(type, incident_id, receiver, doc_no, title, occur_date, description);

            //String subject  = se.getSubjectName();

            //DataClasses1DataContext dbConnect = new DataClasses1DataContext();
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var incidents = from c in dbConnect.hazards
                            join co in dbConnect.companies on c.company_id equals co.company_id
                            join f in dbConnect.functions on c.function_id equals f.function_id
                            join d in dbConnect.departments on c.department_id equals d.department_id
                            where c.id == hazard_id
                            select new
                            {
                                company_id = c.company_id,
                                company_en = co.company_en,
                                company_th = co.company_th,
                                function_id = c.function_id,
                                function_en = f.function_en,
                                function_th = f.function_th,
                                department_id = c.department_id,
                                department_en = d.department_en,
                                department_th = d.department_th,
                                division_id = c.division_id,
                                section_id = c.section_id,
                                incident_id = c.id,
                                incident_code = c.doc_no,
                                title = c.hazard_name,
                                description = c.hazard_detail,
                                occur_date = c.hazard_date,
                                hazard_area = c.hazard_area,
                                reason_reject = c.reason_reject,
                                c.country,
                                reporter_id = c.employee_id,
                                typelogin = c.typeuser_login
                            };
            String company_id = "";
            String company_en = "";
            String company_th = "";
            String company_name = "";
            String function_id = "";
            String function_en = "";
            String function_th = "";
            String function_name = "";
            String department_id = "";
            String department_en = "";
            String department_th = "";
            String department_name = "";
            String division_id = "";
            String section_id = "";
            String incident_code = "";
            String title = "";
            String description = "";
            String hazard_area = "";
            DateTime occur_date = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
            String reason_reject = "";
            String country = "";
            String reporter_id = "";
            String typelogin = "";


            foreach (var v in incidents)
            {
                //function_id = v.function_id;
                //department_id = v.department_id;
                company_name = v.company_en;
                function_id = v.function_id;
                function_en = v.function_en;
                function_th = v.function_th;
                function_name = v.function_en;
                department_id = v.department_id;
                department_en = v.department_en;
                department_th = v.department_th;
                department_name = v.department_en;
                division_id = v.division_id;
                section_id = v.section_id;
                incident_code = v.incident_code;
                title = v.title;
                description = v.description;
                occur_date = (DateTime)v.occur_date;
                hazard_area = v.hazard_area;
                reason_reject = v.reason_reject;
                country = v.country;
                reporter_id = v.reporter_id;
                typelogin = v.typelogin;
            }

            ArrayList sq_type = new ArrayList();
            sq_type.Add(10);
            sq_type.Add(11);
            sq_type.Add(15);
            sq_type.Add(17);
            sq_type.Add(18);
            sq_type.Add(19);
            sq_type.Add(20);

            String action_detail = "";
            String assign_action_by = "";
            DateTime? action_due_date = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
            ArrayList tmpEmail = new ArrayList();
            ArrayList tmpEmID = new ArrayList();

            if ((type >= 3 && type <= 11 && type != 5 && action_id > 0) || type == 22)//5 is request to close actio ไม่ส่งหาตัวเอง
            {

                var actions = from c in dbConnect.process_actions
                              join em in dbConnect.employees on c.employee_id equals em.employee_id
                              join aem in dbConnect.employees on c.assign_by_employee_id equals aem.employee_id into joinAem
                              from aem in joinAem.DefaultIfEmpty()
                              where c.id == action_id
                              select new
                              {
                                  action_id = c.id,
                                  em.supervisor_id,
                                  em.mngt_level,
                                  action_detail = c.action,
                                  action_due_date = c.due_date,
                                  em_id = em.employee_id,
                                  em_email = em.email,
                                  em_name_en = em.first_name_en + " " + em.last_name_en,
                                  assign_action_by = aem.first_name_en + " " + aem.last_name_en,
                                  contractor_id = c.contractor_id,
                                  c.hazard_id
                              };
                foreach (var v in actions)
                {
                    action_detail = v.action_detail;
                    action_due_date = v.action_due_date;
                    assign_action_by = v.assign_action_by;
                    if (v.em_email != "")
                    {
                        if (!tmpEmail.Contains(v.em_email))
                        {
                            if (type == 22)//แจ้งหัวหน้าถ้ามา type นี้
                            {

                                stepSupervisorHazard(v.supervisor_id, v.mngt_level, type, hazard_id, incident_code, title, Convert.ToDateTime(occur_date), description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, timezone, v.em_id);

                            }
                            else
                            {
                                String subject = "(For your action) " + se.getHazardSubjectName(type, incident_code, title);

                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, v.hazard_id, "hazard", v.em_id, v.em_email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }

                                String body = se.setHazardTemplate(type, hazard_id, v.em_name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                se.SendEmail(Convert.ToString(type), v.em_id,v.em_name_en,v.em_email, subject, body, timezone);
                                tmpEmail.Add(v.em_email);
                            }


                        }
                    }

                    if (!tmpEmID.Contains(v.em_id))
                    {
                        se.insertHazardNotify(type, hazard_id, v.em_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);
                        tmpEmID.Add(v.em_id);
                    }

                    if (v.contractor_id != null)
                    {
                        var contractors = from c in dbConnect.contractors
                                          where c.id == v.contractor_id
                                          select new
                                          {
                                              con_name_en = c.first_name_en + " " + c.last_name_en,
                                              con_email = c.email
                                          };
                        foreach (var vv in contractors)
                        {
                            if (vv.con_email != "")
                            {
                                if (!tmpEmail.Contains(vv.con_email))
                                {
                                    String subject = se.getHazardSubjectName(type, incident_code, title);
                                    String con_subject = se.getHazardSubjectName(5, incident_code, title);
                                    String body = se.setHazardTemplate(5, hazard_id, vv.con_name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                    se.SendEmail(Convert.ToString(type), v.em_id, v.em_name_en, vv.con_email, subject, body, timezone);
                                    tmpEmail.Add(vv.con_email);
                                }
                            }

                        }
                    }
                }

            }

            if (groups.Contains("Reporter"))
            {
                if (typelogin == "contractor")
                {

                    var reporters = from c in dbConnect.contractors
                                    where c.id == Convert.ToInt32(reporter_id)
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.id.ToString(),
                                        email = c.email
                                    };
                    foreach (var v in reporters)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getHazardSubjectName(type, incident_code, title);
                                if (role_action == "Reporter")
                                {
                                    if (type != 21)//กรณีส่งแจ้งผู้รายงานตอนปิด hazard
                                    {
                                        subject = "(For your action) " + subject;
                                    }

                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "Reporter")
                            {
                                se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }


                }
                else
                {

                    var reporters = from c in dbConnect.employees
                                    where c.employee_id == reporter_id
                                    select new
                                    {
                                        name_en = c.first_name_en + " " + c.last_name_en,
                                        name_th = c.first_name_th + " " + c.last_name_th,
                                        user_id = c.employee_id,
                                        email = c.email
                                    };
                    foreach (var v in reporters)
                    {
                        if (v.email != "")
                        {
                            if (!tmpEmail.Contains(v.email))
                            {
                                String subject = se.getHazardSubjectName(type, incident_code, title);
                                if (role_action == "Reporter")
                                {
                                    if (type != 21)//กรณีส่งแจ้งผู้รายงานตอนปิด hazard
                                    {
                                        subject = "(For your action) " + subject;
                                    }

                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                                tmpEmail.Add(v.email);
                            }
                        }

                        if (!tmpEmID.Contains(v.user_id))
                        {
                            if (role_action == "Reporter")
                            {
                                se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            else
                            {
                                se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                            }
                            tmpEmID.Add(v.user_id);
                        }
                    }


                }

            }

            if (groups.Contains("AdminOH&S"))
            {
                var admin_ohs = from c in dbConnect.employees
                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                where (b.group_id == 4 || b.group_id == 5) && b.function_id == function_id
                                && c.country == country
                                select new
                                {
                                    name_en = c.first_name_en + " " + c.last_name_en,
                                    name_th = c.first_name_th + " " + c.last_name_th,
                                    user_id = c.employee_id,
                                    email = c.email,
                                    c.supervisor_id,
                                    c.mngt_level
                                };

                ArrayList lsSupervisor = new ArrayList();
                foreach (var v in admin_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHazardSubjectName(type, incident_code, title);

                            if (type == 19 || type == 20)//แจ้งหัวหน้าถ้ามา type นี้
                            {
                                if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                {
                                    stepSupervisorHazard(v.supervisor_id, v.mngt_level, type, hazard_id, incident_code, title, Convert.ToDateTime(occur_date), description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, timezone, v.user_id);
                                    lsSupervisor.Add(v.supervisor_id);
                                }

                            }
                            else
                            {


                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, hazard_id, "hazard", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }


                                if (role_action == "AdminOH&S")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }

                            }


                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "AdminOH&S")
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }

            if (groups.Contains("AreaSuperervisor"))
            {
                var area_super = from c in dbConnect.employee_has_sections
                                 join b in dbConnect.employees on c.employee_id equals b.employee_id
                                 where c.section_id == section_id && c.country == country
                                 select new
                                 {
                                     name_en = b.first_name_en + " " + b.last_name_en,
                                     name_th = b.first_name_th + " " + b.last_name_th,
                                     user_id = b.employee_id,
                                     email = b.email,
                                     b.supervisor_id,
                                     b.mngt_level
                                 };

                ArrayList lsSupervisor = new ArrayList();
                foreach (var v in area_super)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHazardSubjectName(type, incident_code, title);


                            if (type == 19 || type == 20)//แจ้งหัวหน้าถ้ามา type นี้
                            {
                                if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                {
                                    stepSupervisorHazard(v.supervisor_id, v.mngt_level, type, hazard_id, incident_code, title, Convert.ToDateTime(occur_date), description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, timezone, v.user_id);
                                    lsSupervisor.Add(v.supervisor_id);
                                }

                            }
                            else
                            {
                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, hazard_id, "hazard", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }

                                if (role_action == "AreaSuperervisor")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                            }
                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "AreaSuperervisor")
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }

            if (groups.Contains("AreaManager"))
            {
                var area_manager = from c in dbConnect.employee_has_divisions
                                   join b in dbConnect.employees on c.employee_id equals b.employee_id
                                   where c.division_id == division_id && c.country == country
                                   select new
                                   {
                                       name_en = b.first_name_en + " " + b.last_name_en,
                                       name_th = b.first_name_th + " " + b.last_name_th,
                                       user_id = b.employee_id,
                                       email = b.email,
                                       b.supervisor_id,
                                       b.mngt_level
                                   };

                ArrayList lsSupervisor = new ArrayList();
                foreach (var v in area_manager)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHazardSubjectName(type, incident_code, title);


                            if (type == 19 || type == 20)//แจ้งหัวหน้าถ้ามา type นี้
                            {
                                if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                {
                                    stepSupervisorHazard(v.supervisor_id, v.mngt_level, type, hazard_id, incident_code, title, Convert.ToDateTime(occur_date), description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, timezone, v.user_id);
                                    lsSupervisor.Add(v.supervisor_id);
                                }

                            }
                            else
                            {
                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, hazard_id, "hazard", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }

                                if (role_action == "AreaManager")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                            }

                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "AreaManager")
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }

            if (groups.Contains("AreaOH&S"))
            {
                var area_ohs = from c in dbConnect.employee_has_departments
                               join b in dbConnect.employees on c.employee_id equals b.employee_id
                               where c.department_id == department_id && c.country == country
                               select new
                               {
                                   name_en = b.first_name_en + " " + b.last_name_en,
                                   name_th = b.first_name_th + " " + b.last_name_th,
                                   user_id = b.employee_id,
                                   email = b.email,
                                   b.supervisor_id,
                                   b.mngt_level
                               };


                ArrayList lsSupervisor = new ArrayList();
                foreach (var v in area_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHazardSubjectName(type, incident_code, title);


                            if (type == 19 || type == 20)//แจ้งหัวหน้าถ้ามา type นี้
                            {
                                if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                {
                                    stepSupervisorHazard(v.supervisor_id, v.mngt_level, type, hazard_id, incident_code, title, Convert.ToDateTime(occur_date), description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, timezone, v.user_id);
                                    lsSupervisor.Add(v.supervisor_id);
                                }

                            }
                            else
                            {
                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, hazard_id, "hazard", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }

                                if (role_action == "AreaOH&S")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en,v.email, subject, body, timezone);

                                }
                            }

                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "AreaOH&S")
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }

            if (groups.Contains("GroupOH&SHazard"))
            {
                var admin_ohs = from c in dbConnect.employees
                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                where (b.group_id == 16) && c.country == country
                                select new
                                {
                                    name_en = c.first_name_en + " " + c.last_name_en,
                                    name_th = c.first_name_th + " " + c.last_name_th,
                                    user_id = c.employee_id,
                                    email = c.email,
                                    c.supervisor_id,
                                    c.mngt_level
                                };
                ArrayList lsSupervisor = new ArrayList();
                foreach (var v in admin_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHazardSubjectName(type, incident_code, title);


                            if (type == 19 || type == 20)//แจ้งหัวหน้าถ้ามา type นี้
                            {
                                if (lsSupervisor.IndexOf(v.supervisor_id) == -1)
                                {
                                    stepSupervisorHazard(v.supervisor_id, v.mngt_level, type, hazard_id, incident_code, title, Convert.ToDateTime(occur_date), description, action_detail, Convert.ToDateTime(action_due_date), assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, timezone, v.user_id);
                                    lsSupervisor.Add(v.supervisor_id);
                                }

                            }
                            else
                            {
                                if (sq_type.Contains(type))
                                {
                                    string sqemail = getSequenceEmail(type, hazard_id, "hazard", v.user_id, v.email, title, subject, timezone);
                                    subject = subject + " (" + sqemail + ")";

                                }

                                if (role_action == "GroupOH&SHazard")
                                {
                                    subject = "(For your action) " + subject;
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                                else
                                {
                                    String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                    se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                                }
                            }

                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "GroupOH&SHazard")
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }

            if (groups.Contains("LegalDepartment"))
            {
                var admin_ohs = from c in dbConnect.employees
                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                where (b.group_id == 9) && c.country == country
                                select new
                                {
                                    name_en = c.first_name_en + " " + c.last_name_en,
                                    name_th = c.first_name_th + " " + c.last_name_th,
                                    user_id = c.employee_id,
                                    email = c.email,
                                    c.supervisor_id,
                                    c.mngt_level
                                };
                foreach (var v in admin_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHazardSubjectName(type, incident_code, title);

                            if (sq_type.Contains(type))
                            {
                                string sqemail = getSequenceEmail(type, hazard_id, "hazard", v.user_id, v.email, title, subject, timezone);
                                subject = subject + " (" + sqemail + ")";

                            }


                            if (role_action == "LegalDepartment")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                            }



                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "LegalDepartment")
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }

            if (groups.Contains("GroupCommunicationVP"))
            {
                var admin_ohs = from c in dbConnect.employees
                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                where (b.group_id == 8) && c.country == country
                                select new
                                {
                                    name_en = c.first_name_en + " " + c.last_name_en,
                                    name_th = c.first_name_th + " " + c.last_name_th,
                                    user_id = c.employee_id,
                                    email = c.email,
                                    c.supervisor_id,
                                    c.mngt_level
                                };
                foreach (var v in admin_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHazardSubjectName(type, incident_code, title);

                            if (sq_type.Contains(type))
                            {
                                string sqemail = getSequenceEmail(type, hazard_id, "hazard", v.user_id, v.email, title, subject, timezone);
                                subject = subject + " (" + sqemail + ")";

                            }

                            if (role_action == "GroupCommunicationVP")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            else
                            {

                                String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en, v.email, subject, body, timezone);

                            }



                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "GroupCommunicationVP")
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }

            if (groups.Contains("SML"))
            {
                var admin_ohs = from c in dbConnect.employees
                                    //join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                where ((c.employee_subgroup_text == "SML") && c.emp_group_text != "Temporary - Monthly") && o.function_id == function_id
                                && c.country == country
                                select new
                                {
                                    name_en = c.first_name_en + " " + c.last_name_en,
                                    name_th = c.first_name_th + " " + c.last_name_th,
                                    user_id = c.employee_id,
                                    email = c.email,
                                    c.supervisor_id,
                                    c.mngt_level
                                };
                foreach (var v in admin_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHazardSubjectName(type, incident_code, title);


                            if (sq_type.Contains(type))
                            {
                                string sqemail = getSequenceEmail(type, hazard_id, "hazard", v.user_id, v.email, title, subject, timezone);
                                subject = subject + " (" + sqemail + ")";

                            }

                            if (role_action == "SML")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                            }


                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "SML")
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }

            if (groups.Contains("TML"))
            {
                var admin_ohs = from c in dbConnect.employees
                                    //join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                    //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                where ((c.employee_subgroup_text == "TML") && c.emp_group_text != "Temporary - Monthly")
                                && c.country == country
                                select new
                                {
                                    name_en = c.first_name_en + " " + c.last_name_en,
                                    name_th = c.first_name_th + " " + c.last_name_th,
                                    user_id = c.employee_id,
                                    email = c.email,
                                    c.supervisor_id,
                                    c.mngt_level
                                };
                foreach (var v in admin_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHazardSubjectName(type, incident_code, title);

                            if (sq_type.Contains(type))
                            {
                                string sqemail = getSequenceEmail(type, hazard_id, "hazard", v.user_id, v.email, title, subject, timezone);
                                subject = subject + " (" + sqemail + ")";

                            }

                            if (role_action == "TML")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, true);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setHazardTemplate(type, hazard_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                            }


                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "TML")
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertHazardNotify(type, hazard_id, v.user_id, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }



        }




        public void InsertSotNotification(int type, int sot_id, String[] groups, string timezone, string role_action, int action_id = 0)
        {

            //getrelateuser
            safetys4.App_Code.SafetyEmail se = new safetys4.App_Code.SafetyEmail();

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var sots = from c in dbConnect.sots
                       join f in dbConnect.functions on c.function_id equals f.function_id
                       join d in dbConnect.departments on c.department_id equals d.department_id
                       where c.id == sot_id
                       select new
                       {
                           function_id = c.function_id,
                           function_en = f.function_en,
                           function_th = f.function_th,
                           department_id = c.department_id,
                           department_en = d.department_en,
                           department_th = d.department_th,
                           division_id = c.division_id,
                           sot_id = c.id,
                           sot_code = c.doc_no,
                           type_work = c.type_work,
                           occur_date = c.sot_date,
                           occur_date_end = c.sot_date_end,
                           sot_area = c.location,
                           c.country
                       };

            String function_id = "";
            String function_en = "";
            String function_th = "";
            String function_name = "";
            String department_id = "";
            String department_en = "";
            String department_th = "";
            String department_name = "";
            String division_id = "";
            String sot_code = "";
            String type_work = "";
            String sot_area = "";
            DateTime occur_date = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
            DateTime occur_date_end = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
            String country = "";


            foreach (var v in sots)
            {
                //function_id = v.function_id;
                //department_id = v.department_id;
                function_id = v.function_id;
                function_en = v.function_en;
                function_th = v.function_th;
                function_name = v.function_en;
                department_id = v.department_id;
                department_en = v.department_en;
                department_th = v.department_th;
                department_name = v.department_en;
                division_id = v.division_id;

                sot_code = v.sot_code;
                type_work = v.type_work;

                occur_date = (DateTime)v.occur_date;
                occur_date_end = (DateTime)v.occur_date_end;
                sot_area = v.sot_area;
                country = v.country;
            }



            String action_detail = "";
            String assign_action_by = "";
            DateTime? action_due_date = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
            ArrayList tmpEmail = new ArrayList();
            ArrayList tmpEmID = new ArrayList();

            if (type >= 3 && type <= 7 && type != 5 && action_id > 0)//5 is request to close action ไม่ส่งหาตัวเอง
            {

                var actions = from c in dbConnect.process_action_sots
                              join em in dbConnect.employees on c.employee_id equals em.employee_id
                              join aem in dbConnect.employees on c.assign_by_employee_id equals aem.employee_id
                              where c.id == action_id
                              select new
                              {
                                  action_id = c.id,
                                  action_detail = c.action,
                                  action_due_date = c.due_date,
                                  em_id = em.employee_id,
                                  em_email = em.email,
                                  em_name_en = em.first_name_en + " " + em.last_name_en,
                                  assign_action_by = aem.first_name_en + " " + aem.last_name_en,
                                  contractor_id = c.contractor_id
                              };
                foreach (var v in actions)
                {
                    action_detail = v.action_detail;
                    assign_action_by = v.assign_action_by;
                    action_due_date = v.action_due_date;
                    if (v.em_email != "")
                    {
                        if (!tmpEmail.Contains(v.em_email))
                        {
                            String subject = "(For your action) " + se.getSotSubjectName(type, sot_code);

                            String body = se.setSotTemplate(type, sot_id, v.em_name_en, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm"), action_detail, action_due_date, assign_action_by, function_name, department_name, sot_area, country, true);
                            se.SendEmail(Convert.ToString(type), v.em_id, v.em_name_en, v.em_email, subject, body, timezone);
                            tmpEmail.Add(v.em_email);
                        }
                    }

                    if (!tmpEmID.Contains(v.em_id))
                    {
                        se.insertSotNotify(type, sot_id, v.em_id, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, function_name, department_name, timezone, true);
                        tmpEmID.Add(v.em_id);
                    }

                    if (v.contractor_id != null)
                    {
                        var contractors = from c in dbConnect.contractors
                                          where c.id == v.contractor_id
                                          select new
                                          {
                                              con_name_en = c.first_name_en + " " + c.last_name_en,
                                              con_email = c.email
                                          };
                        foreach (var vv in contractors)
                        {
                            if (vv.con_email != "")
                            {
                                if (!tmpEmail.Contains(vv.con_email))
                                {
                                    String subject = se.getSotSubjectName(type, sot_code);
                                    String con_subject = se.getSotSubjectName(5, sot_code);
                                    String body = se.setSotTemplate(5, sot_id, vv.con_name_en, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, assign_action_by, function_name, department_name, sot_area, country, false);
                                    se.SendEmail(Convert.ToString(type), v.em_id,v.em_name_en,vv.con_email, subject, body, timezone);
                                    tmpEmail.Add(vv.con_email);
                                }
                            }

                        }
                    }
                }

            }



            if (groups.Contains("AdminOH&S"))
            {
                var admin_ohs = from c in dbConnect.employees
                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                where (b.group_id == 4 || b.group_id == 5) && b.function_id == function_id
                                && c.country == country
                                select new
                                {
                                    name_en = c.first_name_en + " " + c.last_name_en,
                                    name_th = c.first_name_th + " " + c.last_name_th,
                                    user_id = c.employee_id,
                                    email = c.email
                                };
                foreach (var v in admin_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getSotSubjectName(type, sot_code);
                            if (role_action == "AdminOH&S")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setSotTemplate(type, sot_id, v.name_en, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, assign_action_by, function_name, department_name, sot_area, country, true);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setSotTemplate(type, sot_id, v.name_en, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, assign_action_by, function_name, department_name, sot_area, country, false);
                                se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "AdminOH&S")
                        {
                            se.insertSotNotify(type, sot_id, v.user_id, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertSotNotify(type, sot_id, v.user_id, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }



            if (groups.Contains("AreaManager"))
            {
                var area_manager = from c in dbConnect.employee_has_divisions
                                   join b in dbConnect.employees on c.employee_id equals b.employee_id
                                   where c.division_id == division_id && c.country == country
                                   select new
                                   {
                                       name_en = b.first_name_en + " " + b.last_name_en,
                                       name_th = b.first_name_th + " " + b.last_name_th,
                                       user_id = b.employee_id,
                                       email = b.email
                                   };

                foreach (var v in area_manager)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getSotSubjectName(type, sot_code);
                            if (role_action == "AreaManager")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setSotTemplate(type, sot_id, v.name_en, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, assign_action_by, function_name, department_name, sot_area, country, true);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setSotTemplate(type, sot_id, v.name_en, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, assign_action_by, function_name, department_name, sot_area, country, false);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "AreaManager")
                        {
                            se.insertSotNotify(type, sot_id, v.user_id, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertSotNotify(type, sot_id, v.user_id, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }

            if (groups.Contains("AreaOH&S"))
            {
                var area_ohs = from c in dbConnect.employee_has_departments
                               join b in dbConnect.employees on c.employee_id equals b.employee_id
                               where c.department_id == department_id && c.country == country
                               select new
                               {
                                   name_en = b.first_name_en + " " + b.last_name_en,
                                   name_th = b.first_name_th + " " + b.last_name_th,
                                   user_id = b.employee_id,
                                   email = b.email
                               };

                foreach (var v in area_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getSotSubjectName(type, sot_code);
                            if (role_action == "AreaOH&S")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setSotTemplate(type, sot_id, v.name_en, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, assign_action_by, function_name, department_name, sot_area, country, true);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setSotTemplate(type, sot_id, v.name_en, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, assign_action_by, function_name, department_name, sot_area, country, false);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "AreaOH&S")
                        {
                            se.insertSotNotify(type, sot_id, v.user_id, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertSotNotify(type, sot_id, v.user_id, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }


            if (groups.Contains("SOT"))
            {
                var area_ohs = from c in dbConnect.employee_has_sots
                               join b in dbConnect.employees on c.employee_id equals b.employee_id
                               where c.sot_id == sot_id
                               select new
                               {
                                   name_en = b.first_name_en + " " + b.last_name_en,
                                   name_th = b.first_name_th + " " + b.last_name_th,
                                   user_id = b.employee_id,
                                   email = b.email
                               };

                foreach (var v in area_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getSotSubjectName(type, sot_code);
                            if (role_action == "SOT")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setSotTemplate(type, sot_id, v.name_en, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, assign_action_by, function_name, department_name, sot_area, country, true);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setSotTemplate(type, sot_id, v.name_en, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, assign_action_by, function_name, department_name, sot_area, country, false);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            tmpEmail.Add(v.email);
                        }
                    }

                    if (!tmpEmID.Contains(v.user_id))
                    {
                        if (role_action == "SOT")
                        {
                            se.insertSotNotify(type, sot_id, v.user_id, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, function_name, department_name, timezone, true);

                        }
                        else
                        {
                            se.insertSotNotify(type, sot_id, v.user_id, sot_code, occur_date.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), occur_date_end.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB")), action_detail, action_due_date, function_name, department_name, timezone, false);

                        }
                        tmpEmID.Add(v.user_id);
                    }
                }
            }



        }



        public void InsertHealthNotification(int type, int health_id, String[] groups, string timezone, string role_action, int action_id = 0)
        {

            //getrelateuser
            safetys4.App_Code.SafetyEmail se = new safetys4.App_Code.SafetyEmail();

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var healths = from c in dbConnect.healths
                          join co in dbConnect.companies on c.company_id equals co.company_id
                          join f in dbConnect.functions on c.function_id equals f.function_id
                          join d in dbConnect.departments on c.department_id equals d.department_id
                          join di in dbConnect.divisions on c.division_id equals di.division_id into joinDi
                          from di in joinDi.DefaultIfEmpty()
                          where c.id == health_id
                          select new
                          {
                              company_id = co.company_id,
                              company_en = co.company_en,
                              company_th = co.company_th,
                              function_id = c.function_id,
                              function_en = f.function_en,
                              function_th = f.function_th,
                              department_id = c.department_id,
                              department_en = d.department_en,
                              department_th = d.department_th,
                              division_id = c.division_id,
                              division_en = di.division_en,
                              division_th = di.division_th,
                              section_id = c.section_id,
                              health_id = c.id,
                              health_code = c.doc_no,
                              health_employee_id = c.health_employee_id,
                              c.year_health,
                              occur_date = c.report_date,
                              c.age,
                              c.service_year,
                              c.service_year_current,
                              c.job_type_machine_type,
                              c.country,
                              reporter_id = c.employee_id,
                              c.typeuser_login

                          };

            String company_id = "";
            String company_en = "";
            String company_th = "";
            String company_name = "";
            String function_id = "";
            String function_en = "";
            String function_th = "";
            String function_name = "";
            String department_id = "";
            String department_en = "";
            String department_th = "";
            String department_name = "";
            String division_id = "";
            String division_en = "";
            String division_th = "";
            String division_name = "";
            String section_id = "";
            String health_code = "";
            String title = "";
            String description = "";
            DateTime occur_date = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));

            String country = "";
            String reporter_id = "";
            String typelogin = "";

            foreach (var v in healths)
            {
                company_id = v.company_id;
                company_en = v.company_en;
                company_th = v.company_th;
                company_name = v.company_en;
                function_id = v.function_id;
                function_en = v.function_en;
                function_th = v.function_th;
                function_name = v.function_en;
                department_id = v.department_id;
                department_en = v.department_en;
                department_th = v.department_th;
                department_name = v.department_en;
                division_id = v.division_id;
                division_en = v.division_en;
                division_th = v.division_th;
                division_name = v.division_en;

                section_id = v.section_id;
                health_code = v.health_code;

                occur_date = (DateTime)v.occur_date;

                country = v.country;
                reporter_id = v.reporter_id;
                typelogin = v.typeuser_login;
            }


            String action_detail = "";
            String assign_action_by = "";
            DateTime? action_due_date = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
            ArrayList tmpEmail = new ArrayList();
            ArrayList tmpEmID = new ArrayList();

            ArrayList sq_type = new ArrayList();//type มากจาก alert
            sq_type.Add(4);//overdue



            if ((type == 6 || type == 7 || type == 8) && action_id > 0)
            {

                var actions = from c in dbConnect.process_action_healths
                              join em in dbConnect.employees on c.employee_id equals em.employee_id
                              join aem in dbConnect.employees on c.assign_by_employee_id equals aem.employee_id into joinAem
                              from aem in joinAem.DefaultIfEmpty()
                              where c.id == action_id
                              select new
                              {
                                  action_id = c.id,
                                  action_detail = c.action,
                                  action_due_date = c.due_date,
                                  em_id = em.employee_id,
                                  em_email = em.email,
                                  em_name_en = em.first_name_en + " " + em.last_name_en,
                                  assign_action_by = aem.first_name_en + " " + aem.last_name_en,
                                  c.health_id
                              };
                foreach (var v in actions)
                {
                    action_detail = v.action_detail;
                    action_due_date = v.action_due_date;
                    assign_action_by = v.assign_action_by;
                    if (v.em_email != "")
                    {
                        if (!tmpEmail.Contains(v.em_email))
                        {
                            String subject = "(For your action) " + se.getHealthSubjectName(type, health_code, title);


                            if (sq_type.Contains(type))
                            {
                                string sqemail = getSequenceEmail(type, v.health_id, "health", v.em_id, v.em_email, title, subject, timezone);
                                subject = subject + " (" + sqemail + ")";

                            }

                            String body = se.setHealthTemplate(type, health_id, v.em_name_en, health_code, company_name, function_name, department_name, division_name, v.action_detail, assign_action_by, country, true);
                            se.SendEmail(Convert.ToString(type), v.em_id, v.em_name_en, v.em_email, subject, body, timezone);
                            tmpEmail.Add(v.em_email);
                        }
                    }


                }

            }

            if (groups.Contains("AdminOH&S"))
            {
                var admin_ohs = from c in dbConnect.employees
                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                where (b.group_id == 4 || b.group_id == 5) && b.function_id == function_id
                                && c.country == country
                                select new
                                {
                                    name_en = c.first_name_en + " " + c.last_name_en,
                                    name_th = c.first_name_th + " " + c.last_name_th,
                                    user_id = c.employee_id,
                                    email = c.email,
                                    c.supervisor_id,
                                    c.mngt_level
                                };


                foreach (var v in admin_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHealthSubjectName(type, health_code, title);




                            if (sq_type.Contains(type))
                            {
                                string sqemail = getSequenceEmail(type, health_id, "health", v.user_id, v.email, title, subject, timezone);
                                subject = subject + " (" + sqemail + ")";

                            }


                            if (role_action == "AdminOH&S")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setHealthTemplate(type, health_id, v.name_en, health_code, company_name, function_name, department_name, division_name, "", "", country, true);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en, v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setHealthTemplate(type, health_id, v.name_en, health_code, company_name, function_name, department_name, division_name, "", "", country, false);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en,v.email, subject, body, timezone);

                            }


                            tmpEmail.Add(v.email);
                        }
                    }


                }
            }


            if (groups.Contains("AreaOH&S"))
            {
                var area_ohs = from c in dbConnect.employee_has_departments
                               join b in dbConnect.employees on c.employee_id equals b.employee_id
                               where c.department_id == department_id && c.country == country
                               select new
                               {
                                   name_en = b.first_name_en + " " + b.last_name_en,
                                   name_th = b.first_name_th + " " + b.last_name_th,
                                   user_id = b.employee_id,
                                   email = b.email,
                                   b.supervisor_id,
                                   b.mngt_level
                               };



                foreach (var v in area_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHealthSubjectName(type, health_code, title);


                            if (sq_type.Contains(type))
                            {
                                string sqemail = getSequenceEmail(type, health_id, "health", v.user_id, v.email, title, subject, timezone);
                                subject = subject + " (" + sqemail + ")";

                            }

                            if (role_action == "AreaOH&S")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setHealthTemplate(type, health_id, v.name_en, health_code, company_name, function_name, department_name, division_name, "", "", country, true);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en,v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setHealthTemplate(type, health_id, v.name_en, health_code, company_name, function_name, department_name, division_name, "", "", country, false);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en,v.email, subject, body, timezone);

                            }


                            tmpEmail.Add(v.email);
                        }
                    }


                }
            }

            if (groups.Contains("GroupOH&SHealth"))
            {
                var admin_ohs = from c in dbConnect.employees
                                join b in dbConnect.employee_has_groups on c.employee_id equals b.employee_id
                                //join o in dbConnect.organizations on c.unit_id equals o.org_unit_id
                                where (b.group_id == 17) && c.country == country
                                select new
                                {
                                    name_en = c.first_name_en + " " + c.last_name_en,
                                    name_th = c.first_name_th + " " + c.last_name_th,
                                    user_id = c.employee_id,
                                    email = c.email,
                                    c.supervisor_id,
                                    c.mngt_level
                                };
                ArrayList lsSupervisor = new ArrayList();
                foreach (var v in admin_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHealthSubjectName(type, health_code, title);



                            if (sq_type.Contains(type))
                            {
                                string sqemail = getSequenceEmail(type, health_id, "hazard", v.user_id, v.email, title, subject, timezone);
                                subject = subject + " (" + sqemail + ")";

                            }

                            if (role_action == "GroupOH&SHealth")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setHealthTemplate(type, health_id, v.name_en, health_code, company_name, function_name, department_name, division_name, "", "", country, true);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en,v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setHealthTemplate(type, health_id, v.name_en, health_code, company_name, function_name, department_name, division_name, "", "", country, false);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en,v.email, subject, body, timezone);

                            }


                            tmpEmail.Add(v.email);
                        }
                    }


                }
            }


            if (groups.Contains("Functional"))
            {
                var area_ohs = from c in dbConnect.employee_has_department_functional_managers
                               join b in dbConnect.employees on c.employee_id equals b.employee_id
                               where c.department_id == department_id && c.country == country
                               select new
                               {
                                   name_en = b.first_name_en + " " + b.last_name_en,
                                   name_th = b.first_name_th + " " + b.last_name_th,
                                   user_id = b.employee_id,
                                   email = b.email,
                                   b.supervisor_id,
                                   b.mngt_level
                               };



                foreach (var v in area_ohs)
                {
                    if (v.email != "")
                    {
                        if (!tmpEmail.Contains(v.email))
                        {
                            String subject = se.getHealthSubjectName(type, health_code, title);


                            if (sq_type.Contains(type))
                            {
                                string sqemail = getSequenceEmail(type, health_id, "health", v.user_id, v.email, title, subject, timezone);
                                subject = subject + " (" + sqemail + ")";

                            }

                            if (role_action == "Functional")
                            {
                                subject = "(For your action) " + subject;
                                String body = se.setHealthTemplate(type, health_id, v.name_en, health_code, company_name, function_name, department_name, division_name, "", "", country, true);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en,v.email, subject, body, timezone);

                            }
                            else
                            {
                                String body = se.setHealthTemplate(type, health_id, v.name_en, health_code, company_name, function_name, department_name, division_name, "", "", country, false);
                                se.SendEmail(Convert.ToString(type),v.user_id, v.name_en,v.email, subject, body, timezone);

                            }


                            tmpEmail.Add(v.email);
                        }
                    }


                }
            }






        }



        public ArrayList getImageIncident(int id)
        {
            ArrayList ls = new ArrayList();

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var q = from c in dbConnect.incidents
                        where c.id == id
                        select new
                        {
                            user_id = c.employee_id,
                            reportdate = c.report_date

                        };

                foreach (var s in q)
                {
                    string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathupload"];
                    string name_folder = s.user_id + "_" + s.reportdate.ToString("yyyyMMddHHmmss", CultureInfo.CreateSpecificCulture("en-GB"));
                    // string pathfolder = string.Format("{0}\\upload\\incident\\" + name_folder, Server.MapPath(@"\"));
                    string pathfolder = string.Format("{0}" + pathupload + name_folder, HttpContext.Current.Server.MapPath(@"\"));



                    if (Directory.Exists(pathfolder))
                    {
                        string[] images = Directory.GetFiles(pathfolder, "*")
                                         .Select(Path.GetFileName).OrderByDescending(Path.GetFileName).Take(5)
                                         .ToArray();


                        foreach (var d in images)
                        {
                            Dictionary<string, string> v = new Dictionary<string, string>();

                            v.Add("path", pathfolder + "/" + d);
                            v.Add("folder", name_folder);
                            v.Add("name", d);


                            ls.Add(v);

                        }



                    }//end if 




                }


            }//end using

            return ls;
        }





        public string getSequenceEmail(int type, int case_id, string type_name, string employee_id, string email, string title, string subject, string timezone)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                int num = 0;

                var q = (from c in dbConnect.log_sequence_emails
                         where c.type == type && c.case_id == case_id
                         && c.type_name == type_name.Trim()
                         && c.employee_id == employee_id
                         orderby c.sequence descending
                         select c).Take(1);


                foreach (var s in q)
                {

                    num = Convert.ToInt32(s.sequence) + 1;

                }


                if (num == 0) num = 1;
                ////////////////////////////////////////////log////////////////////////////////////

                log_sequence_email objInsert = new log_sequence_email();

                objInsert.type = type;
                objInsert.case_id = case_id;
                objInsert.type_name = type_name;
                objInsert.employee_id = employee_id;
                objInsert.email = email;
                objInsert.title = Convert.ToString(title);
                objInsert.subject = Convert.ToString(subject);
                objInsert.sequence = num;
                objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));

                dbConnect.log_sequence_emails.InsertOnSubmit(objInsert);

                dbConnect.SubmitChanges();


                ///////////////////////////////////////////end//////////////////////////////////////


                switch (num % 100)
                {
                    case 11:
                    case 12:
                    case 13:
                        return num + "th";
                }

                switch (num % 10)
                {
                    case 1:
                        return num + "st";
                    case 2:
                        return num + "nd";
                    case 3:
                        return num + "rd";
                    default:
                        return num + "th";
                }


            }//end using

        }





        public void stepSupervisor(string supervisor_id, string level, int type, string title, DateTime occur_date, string incident_code, int incident_id, string description,
                                    string action_detail, DateTime action_due_date, string assign_action_by, string injury_person_name, string nature_injury_name, string company_name, string function_name, string department_name,
                                    string incident_area, string immediate_temporary, string reason_reject_en, string reason_reject_th, string reason_comment, string country, string timezone, string first_employee_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                safetys4.App_Code.SafetyEmail se = new safetys4.App_Code.SafetyEmail();
                ArrayList limages = new ArrayList();
                ArrayList sq_type = new ArrayList();//type มากจาก alert
                sq_type.Add(9);
                sq_type.Add(10);
                sq_type.Add(18);
                sq_type.Add(20);
                sq_type.Add(21);
                sq_type.Add(22);
                sq_type.Add(23);

                var g = from c in dbConnect.employees
                        where c.employee_id == supervisor_id
                        select new
                        {
                            name_en = c.first_name_en + " " + c.last_name_en,
                            name_th = c.first_name_th + " " + c.last_name_th,
                            employee_id = c.employee_id,
                            email = c.email,
                            level = c.mngt_level,
                            supervisor_id = c.supervisor_id
                        };


                foreach (var v in g)
                {

                    var q = from c in dbConnect.log_boss_emails
                            where c.type == type && c.case_id == incident_id
                            && c.type_name == "incident"
                            && c.employee_id == v.employee_id
                            && c.first_employee_id == first_employee_id
                            && c.status == "A"
                            select c;


                    string[] list = new string[1];
                    //changes for TML Incident
                    if (type == 25 || type == 26 || type == 27)
                    {
                        list[0] = "SML";//สุดที่ level นี้
                    }
                    else
                    {
                        list[0] = "SML";//สุดที่ level นี้
                    }
                    


                    if (q.Count() > 0)
                    {

                        stepSupervisor(v.supervisor_id, v.level, type, title, occur_date, incident_code, incident_id, description, action_detail, action_due_date, assign_action_by, nature_injury_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, timezone, first_employee_id);

                    }
                    else
                    {



                        log_boss_email objInsert = new log_boss_email();
                        objInsert.first_employee_id = first_employee_id;
                        objInsert.employee_id = v.employee_id;
                        objInsert.type = type;
                        objInsert.type_name = "incident";
                        objInsert.case_id = incident_id;
                        objInsert.email = v.email;
                        objInsert.mngt_level = v.level;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));

                        dbConnect.log_boss_emails.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();


                        var q2 = from c in dbConnect.log_boss_emails
                                 where c.type == type && c.case_id == incident_id
                                 && c.type_name == "incident"
                                 && c.first_employee_id == first_employee_id
                                 && list.Contains(c.mngt_level)
                                 && c.status == "A"
                                 select c;


                        if (q2.Count() > 0)//แสดงว่า step สุดท้ายมีแล้วก็ต้องกลับไปเริ่มวนใหม่แต่แรก step 1
                        {
                            var query = from c in dbConnect.log_boss_emails
                                        where c.type == type && c.case_id == incident_id
                                        && c.type_name == "incident"
                                        && c.first_employee_id == first_employee_id
                                        && c.status == "A"
                                        select c;

                            foreach (log_boss_email rc in query)
                            {
                                rc.status = "D";

                            }

                            dbConnect.SubmitChanges();

                        }



                        String subject = se.getSubjectName(type, incident_code, title);

                        if (sq_type.Contains(type))
                        {
                            string sqemail = getSequenceEmail(type, incident_id, "incident", v.employee_id, v.email, title, subject, timezone);
                            subject = subject + " (" + sqemail + ")";

                        }

                        String body = se.setTemplate(type, incident_id, v.name_en, incident_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, injury_person_name, nature_injury_name, company_name, function_name, department_name, incident_area, immediate_temporary, reason_reject_en, reason_reject_th, reason_comment, country, false, false);

                        //changes for TML Incidence Email.
                        if (v.level!="TML"  &&  v.level != "TML-EXCO")
                        {
                            se.SendEmail(Convert.ToString(type), v.employee_id, v.name_en, v.email, subject, body, timezone, limages);
                        }
                    }


                }



            }//end using




        }





        public void stepSupervisorHazard(string supervisor_id, string level, int type, int hazard_id, string hazard_code, string title, DateTime occur_date, string description,
                                        string action_detail, DateTime action_due_date, string assign_action_by, string company_name, string function_name, string department_name,
                                        string hazard_area, string reason_reject, string country, string timezone, string first_employee_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                safetys4.App_Code.SafetyEmail se = new safetys4.App_Code.SafetyEmail();
                ArrayList sq_type = new ArrayList();
                sq_type.Add(10);
                sq_type.Add(11);
                sq_type.Add(15);
                sq_type.Add(17);
                sq_type.Add(18);
                sq_type.Add(19);
                sq_type.Add(20);


                var g = from c in dbConnect.employees
                        where c.employee_id == supervisor_id
                        select new
                        {
                            name_en = c.first_name_en + " " + c.last_name_en,
                            name_th = c.first_name_th + " " + c.last_name_th,
                            user_id = c.employee_id,
                            email = c.email,
                            level = c.mngt_level,
                            supervisor_id = c.supervisor_id
                        };


                foreach (var v in g)
                {

                    var q = from c in dbConnect.log_boss_emails
                            where c.type == type && c.case_id == hazard_id
                            && c.type_name == "hazard"
                            && c.employee_id == v.user_id
                            && c.first_employee_id == first_employee_id
                            && c.status == "A"
                            select c;
                    // changes for TML Hazard
                    #region Supervisour Level
                    string[] list = new string[1];
                    if (type == 22)
                    {
                        list[0] = "SML";
                    }
                    else
                    {
                        list[0] = "SML";
                    }
                    #endregion
                    
                    //list[0] = "SML";
                    if (q.Count() > 0)
                    {

                        stepSupervisorHazard(v.supervisor_id, v.level, type, hazard_id, hazard_code, title, occur_date, description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, timezone, first_employee_id);

                    }
                    else
                    {

                        log_boss_email objInsert = new log_boss_email();
                        objInsert.first_employee_id = first_employee_id;
                        objInsert.employee_id = v.user_id;
                        objInsert.type = type;
                        objInsert.type_name = "hazard";
                        objInsert.case_id = hazard_id;
                        objInsert.email = v.email;
                        objInsert.mngt_level = v.level;
                        objInsert.status = "A";
                        objInsert.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));

                        dbConnect.log_boss_emails.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();


                        var q2 = from c in dbConnect.log_boss_emails
                                 where c.type == type && c.case_id == hazard_id
                                 && c.type_name == "hazard"
                                 && list.Contains(c.mngt_level)
                                 && c.first_employee_id == first_employee_id
                                 && c.status == "A"
                                 select c;


                        if (q2.Count() > 0)//แสดงว่า step สุดท้ายมีแล้วก็ต้องกลับไปเริ่มวนใหม่แต่แรก step 1
                        {
                            var query = from c in dbConnect.log_boss_emails
                                        where c.type == type && c.case_id == hazard_id
                                        && c.type_name == "hazard"
                                        && c.first_employee_id == first_employee_id
                                        && c.status == "A"
                                        select c;

                            foreach (log_boss_email rc in query)
                            {
                                rc.status = "D";

                            }

                            dbConnect.SubmitChanges();

                        }



                        String subject = se.getHazardSubjectName(type, hazard_code, title);

                        if (sq_type.Contains(type))
                        {
                            string sqemail = getSequenceEmail(type, hazard_id, "hazard", v.user_id, v.email, title, subject, timezone);
                            subject = subject + " (" + sqemail + ")";

                        }
                        String body = se.setHazardTemplate(type, hazard_id, v.name_en, hazard_code, title, occur_date.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")), description, action_detail, action_due_date, assign_action_by, company_name, function_name, department_name, hazard_area, reason_reject, country, false);

                        // Changes for For TML Hazard Email.
                        if (v.level != "TML" && v.level != "TML-EXCO")
                        {
                            se.SendEmail(Convert.ToString(type), v.user_id, v.name_en, v.email, subject, body, timezone);
                        }

                    }


                }



            }//end using




        }











    }

}
