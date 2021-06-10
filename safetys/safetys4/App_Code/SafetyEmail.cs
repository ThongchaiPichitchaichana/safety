using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Net.Mail;
using System.Globalization;
using System.Collections;

namespace safetys4.App_Code
{
    public class SafetyEmail
    {
        string Emp_Status = "";
        public String SendEmail(String Type,String EmployeeId, String UserNameEN, String sendTo, String subject , String body,string timezone,ArrayList limages = null)
        {
            string Emp_Status = "";
            String ret = "";
            String postData = "send_to=" + sendTo + "&subject=" + subject + "&body=" + body;
            safetys4.App_Code.Utilitys u = new Utilitys();

            ThreadPool.QueueUserWorkItem(delegate
            {
                using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
                {

                    try
                    {
                        #region Fetching Employee Status
                        var q = from c in dbConnect.employees
                                where c.employee_id == Convert.ToString(EmployeeId)
                                select new
                                {
                                    c.status
                                };

                        foreach (var rc in q)
                        {
                            Emp_Status = Convert.ToString(rc.status);

                        }

                        #endregion
                        System.Threading.Thread.Sleep(500);
                        //ret = u.HttpPost("http://61.91.80.198/safety_send_email.php", postData);

                        MailMessage mail = new MailMessage();
                        SmtpClient client = new SmtpClient();
                        client.Port = 25;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        //client.Credentials = new System.Net.NetworkCredential("TERMINUS.REPORT@sccc.co.th", "");
                        mail.From = new MailAddress("ohsnotification@siamcitycement.com");
                        #region Code for QA= Don't send to actual users
                        ////Code for QA= Don't send to actual users.
                        //if (sendTo == "-")
                        //{
                        //    mail.To.Add("surendra.singh@siamcitycement.com");
                        //}
                        //else
                        //{
                        //    mail.To.Add("surendra.singh@siamcitycement.com");

                        //}
                        //  Code for PROD = When you need to mornitoring, please unremark this code.

                        #endregion
                       //mail.To.Add("ravi.kant@niit-tech.com");
                        mail.To.Add(sendTo);                       
                        //client.EnableSsl = true;
                        client.Host = "10.254.1.244";
                        mail.Subject = subject;
                        mail.Body = body;
                        mail.IsBodyHtml = true;

                        if (limages != null)
                        {
                            foreach (Dictionary<string, string> dict in limages)
                            {
                                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(dict["path"]);
                                mail.Attachments.Add(attachment);

                            }

                        }                        
                          
                        log_email objInsert = new log_email();


                        #region Dev_testing
                        //if (sendTo == "-")
                        //{
                        //    objInsert.send_to = UserNameEN;
                        //    objInsert.error_message = " UserName -" + UserNameEN + " \n with User ID-" + EmployeeId + " has no email id " + " \n Stats: " + Emp_Status + "";

                        //}
                        //else
                        //{
                        //    objInsert.send_to = sendTo;
                        //    objInsert.error_message = "Send Email|Type value=" + Type + " UserName -" + UserNameEN;

                        //}
                        //objInsert.error_message = "SendEmail";

                        // objInsert.error_message = "Send Email|Type";

                        #endregion
                        objInsert.subject_email = subject;

                        //objInsert.send_to = sendTo+"||Username="+UserNameEN+"";
                        objInsert.send_to = sendTo;
                        objInsert.error_message = "Send Email|Type value=" + Type + "";

                        objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));

                        dbConnect.log_emails.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();


                        client.Send(mail);
                    }
                    catch (Exception ex)
                    {

                        /////////////////////////////////log////////////////////////////////////////////////////


                        log_email objInsert = new log_email();
                        
                        objInsert.send_to = UserNameEN;
                        objInsert.error_message = " UserName -" + UserNameEN + " \n with User ID-" + EmployeeId + " has no email id " + " \n Stats: " + Emp_Status + "\n Error-"+ ex.Message+"";

                        objInsert.subject_email = subject;
                        objInsert.created = DateTime.Now;

                       
                        dbConnect.log_emails.InsertOnSubmit(objInsert);
                        dbConnect.SubmitChanges();


                        //////////////////////////////////end log////////////////////////////////////////////
                    }



                }

                ret = "";
            });
            return ret;
        }

        public String setTemplate(int type, int incident_id, String receiver, String incedent_code, String title, String occur_date, String description, String action_detail, DateTime? action_due_date, String assign_action_by, String injury_person_name, String nature_injury_name,String company_name, String function_name, String department_name, String incident_area, String immediate_temporary, String reason_reject_en, String reason_reject_th, String reason_comment, String country, bool role_action,bool check_activity)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var query = from c in dbConnect.incidents
                        join e in dbConnect.reason_excepts on c.reason_except_type equals e.id into joinExcept
                        from e in joinExcept.DefaultIfEmpty()
                        where c.id == Convert.ToInt32(incident_id)
                        select new
                        {
                            c.typeuser_login,
                            c.employee_id,
                            c.phone,
                            c.owner_activity,
                            c.responsible_area,
                            reason_except_name_en = e.name_en ,
                            reason_except_name_th = e.name_th,
                            reason_except_comment = c.reason_except

                        };
             string reporter_name_en = "";
             string reporter_name_th = "";
             string reporter_phone = "";
             string type_login = "";
             string employee_id = "";
             string phone = "";
             string owner_activity = "";
             string responsible_area = "";
             string reason_except_name_en = "";
             string reason_except_name_th = "";
             string reason_except_comment = "";

             foreach (var rc in query)
             {
                 type_login = rc.typeuser_login;
                 employee_id = rc.employee_id;
                 phone = rc.phone;
                 owner_activity = rc.owner_activity;
                 responsible_area = rc.responsible_area;
                 reason_except_name_en = rc.reason_except_name_en;
                 reason_except_name_th = rc.reason_except_name_th;
                 reason_except_comment = rc.reason_except_comment;

             }



             if (type_login == "contractor")
             {
                 var t = from c in dbConnect.contractors
                         where c.id == Convert.ToInt32(employee_id)
                         select new
                         {
                             report_name_en = c.first_name_en + " " + c.last_name_en,
                             report_name_th = c.first_name_th + " " + c.last_name_th,
                             report_phone = c.mobile_phone
                         };

                 foreach (var o in t)
                 {
                     reporter_name_en = o.report_name_en;
                     reporter_name_th = o.report_name_th;
                     reporter_phone =  o.report_phone;
                 }

             }
             else
             {
                 var e = from c in dbConnect.employees
                         where c.employee_id == employee_id
                         select new
                         {
                             report_name_en = c.first_name_en + " " + c.last_name_en,
                             report_name_th = c.first_name_th + " " + c.last_name_th,
                         };

                 foreach (var o in e)
                 {
                     reporter_name_en = o.report_name_en;
                     reporter_name_th = o.report_name_th;
                     reporter_phone = phone;
                 }


             }


            String ret = "";
            String domain_name = System.Configuration.ConfigurationManager.AppSettings["pathhost"];

            ret += this.setHeaderEmailTemplate("OH&amp;S Reporting Incident Notification");
            /*
             * Dear (receiver),
Incident Notification: I2016-00001
Function: ()
Department: ()
Title: ()
Location of incident: ()
Occur date: ()
Brief Description: ()

Action required: Please verify the incident report 
Link to the system

             * */
            String dear = "Dear " + receiver;
            String dear_th = "เรียน " + receiver;
            String incident_report = "Incident report: " + incedent_code;
            String incident_report_th = "รายงานอุบัติการณ์: " + incedent_code;
            //String function_name = "";
            //String department_name = "";
            //string title = "";
            String location = incident_area;
            //string occur_date;
            //String description = "";
            String action_required = "";
            String action_required_th = "";
            String action_detail_email = "";
            String action_detail_email_th = "";
            String link_to_system = "";
            String link_to_system_th = "";
            String due_date_en = "";
            String due_date_th = "";

            ret += this.addBodyEmailTemplate("Dear ", receiver, "เรียน ", receiver);
            ret += this.addBodyEmailTemplate("Incident report: ", incedent_code, "รายงานอุบัติการณ์: ", incedent_code);

            if (country == "thailand")
            {
                ret += this.addBodyEmailTemplate("Company: ", company_name, "บริษัท: ", company_name);
                ret += this.addBodyEmailTemplate("Function: ", function_name, "สายงาน: ", function_name);
                ret += this.addBodyEmailTemplate("Department: ", department_name, "ฝ่าย: ", department_name);


            }
            else if (country == "srilanka")
            {
                ret += this.addBodyEmailTemplate("Company: ", company_name, "บริษัท: ", company_name);
                ret += this.addBodyEmailTemplate("Function: ", function_name, "สายงาน: ", function_name);
                ret += this.addBodyEmailTemplate("Sub function: ", department_name, "ฝ่าย: ", department_name);


            }
          
            ret += this.addBodyEmailTemplate("Title: ", title, "ชื่อเรื่อง: ", title);
            ret += this.addBodyEmailTemplate("Location of incident: ", incident_area, "สถานที่เกิดเหตุ: ", incident_area);
            ret += this.addBodyEmailTemplate("Occur date: ", occur_date, "วันเวลาที่เกิดเหตุ: ", occur_date);
            ret += this.addBodyEmailTemplate("Brief Description: ", description, "ลักษณะการเกิดเหตุ: ", description);
            

            /*ret = "Dear " + receiver + "<br /><br />";
            ret += "Incident report: " + incedent_code + "<br />";
            ret += "Function: " + function_name + "<br />";
            ret += "Department: " + department_name + "<br />";*/

            if (type == 1 || type == 50)
            {
                if (owner_activity == "KNOWN")
                {
                    if(check_activity)
                    {
                        action_required = "Please verify the incident report";
                        action_required_th = "กรุณาตรวจสอบรายงานอุบัติการณ์";

                    }
                    else
                    {
                        action_required = "NA";
                        action_required_th = "ไม่มี";
                    }
                  
                   
                }
                else if (owner_activity == "UNKNOWN")
                {
                    if (check_activity)
                    {
                        action_required = "NA";
                        action_required_th = "ไม่มี";
                       
                    }
                    else
                    {
                        action_required = "Please verify the incident report";
                        action_required_th = "กรุณาตรวจสอบรายงานอุบัติการณ์";

                    }
                  
                }
               
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                ret = this.setHeaderEmailTemplate("OH&amp;S Reporting Incident Notification");
                ret += this.addBodyEmailTemplate("Dear ", receiver, "เรียน ", receiver);
                ret += this.addBodyEmailTemplate("Incident report: ", incedent_code, "รายงานอุบัติการณ์: ", incedent_code);
                ret += this.addBodyEmailTemplate("Function: ", function_name, "สายงาน: ", function_name);
                ret += this.addBodyEmailTemplate("Department: ", department_name, "ฝ่าย: ", department_name);
                ret += this.addBodyEmailTemplate("Title: ", title, "ชื่อเรื่อง: ", title);
                ret += this.addBodyEmailTemplate("Location of incident: ", incident_area, "สถานที่เกิดเหตุ: ", incident_area);
                ret += this.addBodyEmailTemplate("Occur date: ", occur_date, "วันเวลาที่เกิดเหตุ: ", occur_date);
                ret += this.addBodyEmailTemplate("Brief Description: ", description, "ลักษณะการเกิดเหตุ: ", description);
                ret += this.addBodyEmailTemplate("Name of reporter: ", reporter_name_en, "ผู้รายงานอุบัติการณ์: ", reporter_name_th);
                ret += this.addBodyEmailTemplate("Phone number: ", reporter_phone, "เบอร์โทรศัพท์ : ", reporter_phone);

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);

                }
                else
                {
                    ret += this.addBodyEmailTemplate("Action required: ", "NA", "งานในระบบที่ต้องทำ: ","ไม่มี");

                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br /><br />";
                ret += "Please verify the incedent report <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 2)
            {
                action_required = "Please verify Incident form 1&2";
                action_required_th = "กรุณาตรวจสอบรายงานอุบัติการณ์ฟอร์ม 1 และ 2";
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";
                
                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br /><br />";
                ret += "Request to verify <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 3)
            {
                action_required = "NA";
                action_required_th = "ไม่มี";
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";
				
				ret += this.addBodyEmailTemplate("Injury person name: ", injury_person_name, "ผู้บาดเจ็บ: ", injury_person_name);
				ret += this.addBodyEmailTemplate("Nature injury: ", nature_injury_name, "ประเภทการบาดเจ็บ: ", nature_injury_name);

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br /><br />";
                ret += "Please be informed that the serious incident was reported <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 4)
            {
                action_required = "Please complete your action";
                action_required_th = "กรุณาดำเนินงานของคุณให้เสร็จสิ้น";

                link_to_system = "<a href=\"" + domain_name + "/myactionincident\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/myactionincident\" target='_blank'>เข้าสู่ระบบ</a>";

				ret += this.addBodyEmailTemplate("Action assign by: ", assign_action_by, "สร้างงานโดย: ", assign_action_by);
                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                
                ret += this.addBodyEmailTemplate("Action detail: ", action_detail, "รายละเอียดของงาน: ", action_detail);
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "New Aciton has been created<br />";
                ret += "Action detail: " + action_detail + "<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 5)
            {
                action_required = "Please complete your action";
                action_required_th = "กรุณาดำเนินงานของคุณให้เสร็จสิ้น";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate("Action detail: ", action_detail, "รายละเอียดของงาน: ", action_detail);
                ret += this.addBodyEmailTemplate("Due date: ", action_due_date.ToString(), "กำหนดเสร็จ: ", action_due_date.ToString());
               
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
               /* ret += "Title: " + title + "<br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that you/ your company are responsible for<br />";
                ret += "Action detail: " + action_detail + "<br />";
                ret += "due date: " + action_due_date.ToString();*/
            }
            else if (type == 6)
            {
                action_required = "Please close action \"" + action_detail + "\"";
                action_required_th = "กรุณาปิดงาน \"" + action_detail + "\"";

                link_to_system = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Request to close action \"" + action_detail + "\"<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 7)
            {
                action_required = "NA";
                action_required_th = "ไม่มี";

                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that action \"" + action_detail + "\" was closed<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 8)
            {
                action_required = "Action \"" + action_detail + "\" was rejected. Please review your action again.";
                action_required_th = "งาน \"" + action_detail + "\" ถูกปฏิเสธการปิดงาน. กรุณาตรวจสอบงานของคุณอีกครั้ง";

                link_to_system = "<a href=\"" + domain_name + "/myactionincident\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/myactionincident\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close Action  \"" + action_detail + "\" was rejected<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 9)
            {
                action_required = "Action \"" + action_detail + "\" is due today.  Please proceed immediately.";
                action_required_th = "งาน \"" + action_detail + "\" กำหนดเสร็จวันนี้. กรุณาดำเนินการทันที";
                link_to_system = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that action  \"" + action_detail + "\" is due today<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 10)
            {
                action_required = "Overdue! Action \"" + action_detail + "\" is overdue today.  Please proceed immediately.";
                action_required_th = "งาน \"" + action_detail + "\" เลยกำหนดเสร็จวันนี้. กรุณาดำเนินการทันที";
                link_to_system = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that action  \"" + action_detail + "\" is overdue today<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 11)
            {
                action_required = "Please close the incident report";
                action_required_th = "กรุณาปิดรายงานอุบัติการณ์";
                link_to_system = "<a href=\"" + domain_name + "/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Request to close the incident report ";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 12)
            {
                action_required = "Your request has been rejected, please verify and resubmit";
                action_required_th = "การร้องขอปิดรายงานอุบัติการณ์ของคุณถูกปฏิเสธ กรุณาตรวจสอบและส่งการร้องขอปิดรายงานอีกครั้ง";
                link_to_system = "<a href=\"" + domain_name + "/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
				
				var query_rej = (from c in dbConnect.log_request_close_incidents
							where c.incident_id == Convert.ToInt32(incident_id)
							select new
							{
								remark = c.remark,
								created_at = c.created_at
							}).OrderByDescending(x => x.created_at).Take(1);
				string remark = "";

				foreach (var rcj in query_rej)
				{
					remark = rcj.remark;
				}
			 
				ret += this.addBodyEmailTemplate("Reject reason : ", remark, "เหตุผล : ", remark);
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the incident has been rejected";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 13)
            {
                action_required = "NA";
                action_required_th = "ไม่มี";
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                     ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the incident has been completed";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 14)
            {
                action_required = "Please verify and confirm";
                action_required_th = "กรุณาตรวจสอบและยืนยัน";
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";


                ret += this.addBodyEmailTemplate("Immediate/temporary action: ", immediate_temporary, "Immediate/temporary action: ", immediate_temporary);
                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the incident has been completed";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 15)
            {
                action_required = "Please verify and confirm";
                action_required_th = "กรุณาตรวจสอบและยืนยัน";
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                ret += this.addBodyEmailTemplate("Immediate/temporary action: ", immediate_temporary, "Immediate/temporary action: ", immediate_temporary);
                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
               
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the incident has been completed";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 16)
            {
                action_required = "NA";
                action_required_th = "ไม่มี";
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                ret += this.addBodyEmailTemplate("Immediate/temporary action: ", immediate_temporary, "Immediate/temporary action: ", immediate_temporary);

                //if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                //{
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                //}
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the incident has been completed";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 17)
            {
                action_required = "Please investigate and create corrective and preventive action";
                action_required_th = "Please investigate and create corrective and preventive action";
                link_to_system = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the incident has been completed";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 18)
            {
                action_required = "Overdue! Please verify the incident report immediately.";
                action_required_th = "Overdue! Please verify the incident report immediately.";
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the incident has been completed";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 19)
            {
                action_required = "NA";
                action_required_th = "ไม่มี";
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                ret += this.addBodyEmailTemplate("Reason : ", reason_reject_en, "เหตุผล : ", reason_reject_th);
                ret += this.addBodyEmailTemplate("Comment : ", reason_comment, "รายละเอียดเพิ่มเติม : ", reason_comment);
                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Incident report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the incident has been completed";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 20)
            {
                action_required = "Overdue! Please investigate and create corrective and preventive action immediately.";
                action_required_th = "Overdue! Please investigate and create corrective and preventive action immediately.";
                link_to_system = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

            } else if (type == 21)
            {
                action_required = "Overdue! Please close the incident report immediately.";
                action_required_th = "Overdue! Please close the incident report immediately.";
                link_to_system = "<a href=\"" + domain_name + "/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

            }
            else if (type == 22)//step supervisor 1 to 2
            {
                action_required = "Please be informed that Area Supervisor did not verify the incident report within timeframe.";
                action_required_th = "Please be informed that Area Supervisor did not verify the incident report within timeframe.";
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
               
            }
            else if (type == 23)//step supervisor 2 to 3
            {
                action_required = "Please be informed that Area OH&S did not investigate and/or fill in action within timeframe.";
                action_required_th = "Please be informed that Area OH&S did not investigate and/or fill in action within timeframe.";
                link_to_system = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

            }
            else if (type == 24)
            {
                action_required = "NA";
                action_required_th = "ไม่มี";
                link_to_system = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                ret += this.addBodyEmailTemplate("Reason : ", reason_except_name_en, "เหตุผล : ", reason_except_name_th);
                ret += this.addBodyEmailTemplate("Comment : ", reason_except_comment, "รายละเอียดเพิ่มเติม : ", reason_except_comment);
                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

            }
            else if (type == 25)//supervisorn 
            {
                action_required = "Overdue! Corrective Action \"" + action_detail + "\" is overdue today.  Please proceed immediately.";
                action_required_th = "งาน \"" + action_detail + "\" เลยกำหนดเสร็จวันนี้. กรุณาดำเนินการทันที";
                link_to_system = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }

                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
      
            }
            else if (type == 26)//supervisorn 
            {
                action_required = "Overdue! Preventive Action \"" + action_detail + "\" is overdue today.  Please proceed immediately.";
                action_required_th = "งาน \"" + action_detail + "\" เลยกำหนดเสร็จวันนี้. กรุณาดำเนินการทันที";
                link_to_system = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }

                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

            }
            else if (type == 27)//supervisorn Consequence Management
            {
                action_required = "Overdue! Consequence Management \"" + action_detail + "\" is overdue today.  Please proceed immediately.";
                action_required_th = "งาน \"" + action_detail + "\" เลยกำหนดเสร็จวันนี้. กรุณาดำเนินการทันที";
                link_to_system = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }

                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

            }
            else
            {
                //ret = "";
            }

            ret += this.setFooterEmailTemplate();

            //return System.Web.HttpUtility.UrlEncode(ret);
            return ret; 
        }

        public String setHazardTemplate(int type, int incident_id, String receiver, String incedent_code, String title, String occur_date, String description, String action_detail, DateTime? action_due_date, String assign_action_by,String company_name, String function_name, String department_name, String incident_area,String reason_reject,String country,bool role_action)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var query = from c in dbConnect.hazards
                        join r in dbConnect.reason_reject_hazards on c.reason_reject_type equals r.id into joinReason
                        from r in joinReason.DefaultIfEmpty()
                        where c.id == Convert.ToInt32(incident_id)
                        select new
                        {
                            c.typeuser_login,
                            c.employee_id,
                            c.phone,
                            reason_reject_en = r.name_en,
                            reason_reject_th = r.name_th,
                            reason_comment = c.reason_reject,

                        };
            string reporter_name_en = "";
            string reporter_name_th = "";
            string reporter_phone = "";
            string type_login = "";
            string employee_id = "";
            string phone = "";
            string reason_reject_en = "";
            string reason_reject_th = "";
            string reason_comment = "";
            foreach (var rc in query)
            {
                type_login = rc.typeuser_login;
                employee_id = rc.employee_id;
                phone = rc.phone;
                reason_reject_th = rc.reason_reject_th;
                reason_reject_en = rc.reason_reject_en;
                reason_comment = rc.reason_comment;
               
            }



            if (type_login == "contractor")
            {
                var t = from c in dbConnect.contractors
                        where c.id == Convert.ToInt32(employee_id)
                        select new
                        {
                            report_name_en = c.first_name_en + " " + c.last_name_en,
                            report_name_th = c.first_name_th + " " + c.last_name_th,
                            report_phone = c.mobile_phone
                        };

                foreach (var o in t)
                {
                    reporter_name_en = o.report_name_en;
                    reporter_name_th = o.report_name_th;
                    reporter_phone = o.report_phone;
                }

            }
            else
            {
                var e = from c in dbConnect.employees
                        where c.employee_id == employee_id
                        select new
                        {
                            report_name_en = c.first_name_en + " " + c.last_name_en,
                            report_name_th = c.first_name_th + " " + c.last_name_th,
                        };

                foreach (var o in e)
                {
                    reporter_name_en = o.report_name_en;
                    reporter_name_th = o.report_name_th;
                    reporter_phone = phone;
                }


            }



            String ret = "";
            String domain_name = System.Configuration.ConfigurationManager.AppSettings["pathhost"];

            ret += this.setHeaderEmailTemplate("OH&amp;S Reporting Hazard Notification");

            String dear = "Dear " + receiver;
            String dear_th = "เรียน " + receiver;
            String incident_report = "Incident report: " + incedent_code;
            String incident_report_th = "รายงานอุบัติการณ์: " + incedent_code;
            //String function_name = "";
            //String department_name = "";
            //string title = "";
            String location = incident_area;
            //string occur_date;
            //String description = "";
            String action_required = "";
            String action_required_th = "";
            String action_detail_email = "";
            String action_detail_email_th = "";
            String link_to_system = "";
            String link_to_system_th = "";
            String due_date_en = "";
            String due_date_th = "";

            ret += this.addBodyEmailTemplate("Dear ", receiver, "เรียน ", receiver);
            ret += this.addBodyEmailTemplate("Hazard report:: ", incedent_code, "รายงานแหล่งอันตราย: ", incedent_code);

            if (country == "thailand")
            {
                ret += this.addBodyEmailTemplate("Company: ", company_name, "บริษัท: ", company_name);
                ret += this.addBodyEmailTemplate("Function: ", function_name, "สายงาน: ", function_name);
                ret += this.addBodyEmailTemplate("Department: ", department_name, "ฝ่าย: ", department_name);


            }
            else if (country == "srilanka")
            {
                ret += this.addBodyEmailTemplate("Company: ", company_name, "บริษัท: ", company_name);
                ret += this.addBodyEmailTemplate("Function: ", function_name, "สายงาน: ", function_name);
                ret += this.addBodyEmailTemplate("Sub function: ", department_name, "ฝ่าย: ", department_name);

            }
         
            ret += this.addBodyEmailTemplate("Title: ", title, "ชื่อเรื่อง: ", title);
            ret += this.addBodyEmailTemplate("Location of finding: ", incident_area, "สถานที่สำรวจ: ", incident_area);
            ret += this.addBodyEmailTemplate("Date of finding: ", occur_date, "วันที่สำรวจ: ", occur_date);
            ret += this.addBodyEmailTemplate("Brief Description: ", description, "ลักษณะการเกิดเหตุ: ", description);

            //ret = "Dear " + receiver + "<br /><br />";
            //ret += "Hazard report: " + incedent_code + "<br />";
            //ret += "Function: " + function_name + "<br />";
            //ret += "Department: " + department_name + "<br />";

            if (type == 1 || type == 50)
            {
                action_required = "Please verify the hazard report";
                action_required_th = "กรุณาตรวจสอบรายงานแหล่งอันตราย";
                link_to_system = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                ret += this.addBodyEmailTemplate("Name of reporter: ", reporter_name_en, "ผู้รายงานแหล่งอันตราย: ", reporter_name_th);
                ret += this.addBodyEmailTemplate("Phone number: ", reporter_phone, "เบอร์โทรศัพท์ : ", reporter_phone);

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
               // ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br /><br />";
                ret += "Please verify the hazard report <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 2)
            {
                action_required = "Please create action";
                action_required_th = "ดำเนินการสร้างมาตรการแก้ไขและป้องกัน";
                link_to_system = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br /><br />";
                ret += "Hazard report waiting for create action <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 3)
            {
                action_required = "Please complete your action";
                action_required_th = "กรุณาดำเนินการแก้ไขและป้องกัน";

                link_to_system = "<a href=\"" + domain_name + "/myactionhazard.aspx\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/myactionhazard.aspx\" target='_blank'>เข้าสู่ระบบ</a>";
				
				ret += this.addBodyEmailTemplate("Action assign by: ", assign_action_by, "สร้างงานโดย: ", assign_action_by);

                ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action detail: ", action_detail, "รายละเอียดของงาน: ", action_detail);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                

                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/myactionhazard.aspx\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br /><br />";
                ret += "New action has been created<br />";
                ret += "Action detail: " + action_detail + "<br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/myactionhazard.aspx\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 4)
            {
                action_required = "Please complete your action";
                action_required_th = "กรุณาดำเนินการแก้ไขและป้องกัน";

                link_to_system = "<a href=\"" + domain_name + "/myactionhazard.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/myactionhazard.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate("Action detail: ", action_detail, "รายละเอียดของงาน: ", action_detail);
                ret += this.addBodyEmailTemplate("Due date: ", action_due_date.ToString(), "กำหนดเสร็จ: ", action_due_date.ToString());
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: " + title + "<br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that you/ your company are responsible for<br />";
                ret += "Action detail: " + action_detail + "<br />";
                ret += "due date: " + action_due_date.ToString();*/
            }
            else if (type == 5)
            {
                action_required = "Please close action \"" + action_detail + "\"";
                action_required_th = "กรุณาปิดงาน \"" + action_detail + "\"";
                link_to_system = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Request to close action \"" + action_detail + "\"<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 6)
            {
                action_required = "NA";
                action_required_th = "ไม่มี";
                link_to_system = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/myactionhazard.aspx\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that action \"" + action_detail + "\" was closed<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/myactionhazard.aspx\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 7)
            {
                action_required = "Action \"" + action_detail + "\" was rejected. Please review your action again";
                action_required_th = "การขอปิดงาน \"" + action_detail + "\" ถูกปฏิเสธ กรุณาตรวจสอบงานของคุณอีกครั้ง";
                link_to_system = "<a href=\"" + domain_name + "/myactionhazard.aspx\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/myactionhazard.aspx\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/myactionhazard.aspx\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close Action  \"" + action_detail + "\" was rejected<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/myactionhazard.aspx\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 10)
            {
                action_required = "Action \"" + action_detail + "\" is due today. Please proceed immediately";
                action_required_th = "งาน \"" + action_detail + "\" มีกำหนดเสร็จวันนี้ กรุณาดำเนินการโดยด่วน";
                link_to_system = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that action  \"" + action_detail + "\" is due today<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/hazardform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 11)
            {
                action_required = "Overdue! Action \"" + action_detail + "\". Please proceed immediately";
                action_required_th = "งาน \"" + action_detail + "\" เลยเสร็จวันนี้ กรุณาดำเนินการโดยด่วน";
                link_to_system = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that action  \"" + action_detail + "\" is overdue today<br /><br />";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 12)
            {
                action_required = "Please close hazard report";
                action_required_th = "กรุณาปิดรายงานแหล่งอันตราย";
                link_to_system = "<a href=\"" + domain_name + "/hazardform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Request to close the hazard report ";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/hazardform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 13)
            {
                action_required = "Your request has been rejected, please verify and resubmit";
                action_required_th = "การร้องขอปิดรายงานแหล่งอันตรายของคุณถูกปฏิเสธ กรุณาตรวจสอบและขอปิดรายงานอีกครั้ง";
                link_to_system = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
				
				var query_rej = (from c in dbConnect.log_request_close_hazards
							where c.hazard_id == Convert.ToInt32(incident_id)
							select new
							{
								remark = c.remark,
								created_at = c.created_at
							}).OrderByDescending(x => x.created_at).Take(1);
				string remark = "";

				foreach (var rcj in query_rej)
				{
					remark = rcj.remark;
				}
			 
				ret += this.addBodyEmailTemplate("Reject reason : ", remark, "เหตุผล : ", remark);
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the hazard has been rejected";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 14)
            {
                action_required = "NA";
                action_required_th = "ไม่มี";
                link_to_system = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the hazard has been completed";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 15)
            {
                action_required = "Overdue! Please verify the hazard report immediately.";
                action_required_th = "Overdue! Please verify the hazard report immediately.";
                link_to_system = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
               ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the hazard has been completed";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 16)
            {
                action_required = "NA";
                action_required_th = "ไม่มี";
                link_to_system = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                ret += this.addBodyEmailTemplate("Reason : ", reason_reject_en, "เหตุผล : ", reason_reject_th);
                ret += this.addBodyEmailTemplate("Comment : ", reason_comment, "รายละเอียดเพิ่มเติม : ", reason_comment);
             
                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
                //ret = "Dear " + receiver + "<br /><br />";
                //ret += "Hazard report: " + incedent_code + "<br />";
                /*ret += "Title: <a href=\"http://ohsreport-qa.siamcitycement.com/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>" + title + "</a><br />";
                ret += "Occur date: " + occur_date + "<br />";
                ret += "Brief Description: " + description + "<br />";
                ret += "Please be informed that your request to close the hazard has been completed";
                ret += "<a href=\"http://ohsreport-qa.siamcitycement.com/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Click on the title to open the report</a><br />";*/
            }
            else if (type == 17)
            {
                action_required = "Overdue! Please create action immediately.";
                action_required_th = "Overdue! Please create action immediately.";
                link_to_system = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 18)
            {
                action_required = "Overdue! Please close hazard report immediately.";
                action_required_th = "Overdue! Please close hazard report immediately.";
                link_to_system = "<a href=\"" + domain_name + "/hazardform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform4.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 19)//step supervisor 1 to 2
            {
                action_required = "Please be informed that Area OH&S did not verify the hazard report within timeframe.";
                action_required_th = "Please be informed that Area OH&S did not verify the hazard report within timeframe.";
                link_to_system = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 20)//step supervisor 2 to 3
            {
                action_required = "Please be informed that Area Supervisor did not process of action within timeframe.";
                action_required_th = "Please be informed that Area Supervisor did not process of action within timeframe.";
                link_to_system = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 21)//แจ้งกลับ reporter when closed
            {
                action_required = "NA";
                action_required_th = "ไม่มี";
                link_to_system = "";
                link_to_system_th = "";

                ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);

            }
            else if (type == 22)
            {
                action_required = "Overdue! Action \"" + action_detail + "\". Please proceed immediately";
                action_required_th = "งาน \"" + action_detail + "\" เลยเสร็จวันนี้ กรุณาดำเนินการโดยด่วน";
                link_to_system = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/hazardform3.aspx?pagetype=view&id=" + incident_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
             
            }
            else
            {
                //ret = "";
            }

            ret += this.setFooterEmailTemplate();

            //return System.Web.HttpUtility.UrlEncode(ret);
            return ret;
        }




        public String setSotTemplate(int type, int sot_id, String receiver, String sot_code, String occur_date, String occur_date_end, String action_detail, DateTime? action_due_date, String assign_action_by, String function_name, String department_name, String sot_area, String country,bool role_action)
        {
            String ret = "";
            String domain_name = System.Configuration.ConfigurationManager.AppSettings["pathhost"];

            ret += this.setHeaderEmailTemplate("OH&amp;S Reporting SOT Notification");

            String dear = "Dear " + receiver;
            String dear_th = "เรียน " + receiver;
            String sot_report = "SOT report: " + sot_code;
            String sot_report_th = "รายงาน SOT: " + sot_code;
            //String function_name = "";
            //String department_name = "";
            //string title = "";
            String location = sot_area;
            //string occur_date;
            //String description = "";
            String action_required = "";
            String action_required_th = "";
            String action_detail_email = "";
            String action_detail_email_th = "";
            String link_to_system = "";
            String link_to_system_th = "";
            String due_date_en = "";
            String due_date_th = "";

            ret += this.addBodyEmailTemplate("Dear ", receiver, "เรียน ", receiver);
            ret += this.addBodyEmailTemplate("SOT report:: ", sot_code, "รายงาน SOT: ", sot_code);

            if (country == "thailand")
            {
                ret += this.addBodyEmailTemplate("Function: ", function_name, "สายงาน: ", function_name);
                ret += this.addBodyEmailTemplate("Department: ", department_name, "ฝ่าย: ", department_name);


            }
            else if (country == "srilanka")
            {
                ret += this.addBodyEmailTemplate("Function: ", function_name, "สายงาน: ", function_name);
                ret += this.addBodyEmailTemplate("Sub function: ", department_name, "ฝ่าย: ", department_name);

            }

           // ret += this.addBodyEmailTemplate("Title: ", title, "ชื่อเรื่อง: ", title);
            ret += this.addBodyEmailTemplate("Location: ", sot_area, "สถานที่: ", sot_area);
            ret += this.addBodyEmailTemplate("Date: ", occur_date +" - "+ occur_date_end, "วันที่: ", occur_date);
          

            if (type == 1 || type == 50)
            {
                action_required = "Please verify the SOT report";
                action_required_th = "กรุณาตรวจสอบรายงาน SOT";
                link_to_system = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                 ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 2)
            {
                action_required = "Please create action";
                action_required_th = "ดำเนินการสร้างมาตรการแก้ไขและป้องกัน";
                link_to_system = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 3)
            {
                action_required = "Please complete your action";
                action_required_th = "กรุณาดำเนินการแก้ไขและป้องกัน";

                link_to_system = "<a href=\"" + domain_name + "/myactionsot.aspx\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/myactionsot.aspx\" target='_blank'>เข้าสู่ระบบ</a>";
				
				ret += this.addBodyEmailTemplate("Action assign by: ", assign_action_by, "ดสร้างงานโดย: ", assign_action_by);
                
                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate("Action detail: ", action_detail, "รายละเอียดของงาน: ", action_detail);
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

            }
            else if (type == 4)
            {
                action_required = "Please complete your action";
                action_required_th = "กรุณาดำเนินการแก้ไขและป้องกัน";

                link_to_system = "<a href=\"" + domain_name + "/myactionsot.aspx\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/myactionsot.aspx\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate("Action detail: ", action_detail, "รายละเอียดของงาน: ", action_detail);
                ret += this.addBodyEmailTemplate("Due date: ", action_due_date.ToString(), "กำหนดเสร็จ: ", action_due_date.ToString());
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 5)
            {
                action_required = "Please close action \"" + action_detail + "\"";
                action_required_th = "กรุณาปิดงาน \"" + action_detail + "\"";
                link_to_system = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 6)
            {
                action_required = "NA";
                action_required_th = "ไม่มี";
                link_to_system = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 7)
            {
                action_required = "Action \"" + action_detail + "\" was rejected. Please review your action again";
                action_required_th = "การขอปิดงาน \"" + action_detail + "\" ถูกปฏิเสธ กรุณาตรวจสอบงานของคุณอีกครั้ง";
                link_to_system = "<a href=\"" + domain_name + "/myactionsot.aspx\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/myactionsot.aspx\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 10)
            {
                action_required = "Action \"" + action_detail + "\" is due today. Please proceed immediately";
                action_required_th = "งาน \"" + action_detail + "\" มีกำหนดเสร็จวันนี้ กรุณาดำเนินการโดยด่วน";
                link_to_system = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 11)
            {
                action_required = "Overdue! Action \"" + action_detail + "\". Please proceed immediately";
                action_required_th = "งาน \"" + action_detail + "\" เลยเสร็จวันนี้ กรุณาดำเนินการโดยด่วน";
                link_to_system = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/Sotform.aspx?pagetype=view&id=" + sot_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else
            {
                //ret = "";
            }

            ret += this.setFooterEmailTemplate();

            //return System.Web.HttpUtility.UrlEncode(ret);
            return ret;
        }





        public String setHealthTemplate(int type, int health_id, String receiver, String health_code, String company_name,String function_name, String department_name,String division_name,string action_detail,String assign_action_by,  String country, bool role_action)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var query = from c in dbConnect.healths
                        where c.id == Convert.ToInt32(health_id)
                        select new
                        {
                            c.employee_id,
                            c.health_employee_id

                        };
            string reporter_name_en = "";
            string reporter_name_th = "";
            string health_name_en = "";
            string health_name_th = "";
            string employee_id = "";
            string health_employee_id = "";
  
            foreach (var rc in query)
            {
                employee_id = rc.employee_id;
                health_employee_id = rc.health_employee_id;
 

            }



                var e = from c in dbConnect.employees
                        where c.employee_id == employee_id
                        select new
                        {
                            report_name_en = c.first_name_en + " " + c.last_name_en,
                            report_name_th = c.first_name_th + " " + c.last_name_th,
                        };

                foreach (var o in e)
                {
                    reporter_name_en = o.report_name_en;
                    reporter_name_th = o.report_name_th;
                }
                


                 var h = from c in dbConnect.employees
                        where c.employee_id == health_employee_id
                        select new
                        {
                            health_name_en = c.first_name_en + " " + c.last_name_en,
                            health_name_th = c.first_name_th + " " + c.last_name_th,
                        };

                foreach (var x in h)
                {
                    health_name_en = x.health_name_en;
                    health_name_th = x.health_name_th;
   
                }



            var risks = from c in dbConnect.risk_factor_relate_work_actions
                        join r in dbConnect.risk_factor_relate_works on c.risk_factor_relate_work_id equals r.id
                        where c.health_id == Convert.ToInt32(health_id)
                        select new
                        {
                            risk_name_en = r.name_en,
                            risk_name_th = r.name_th

                        };
        
            string risk_factor_th = "";
            string risk_factor_en = "";
  
            foreach (var r in risks)
            {
                  if(risk_factor_th!="")
                  {
                      risk_factor_th = risk_factor_th + ", " + r.risk_name_th;

                  }else{

                      risk_factor_th = r.risk_name_th;
                  }


                    if(risk_factor_en!="")
                  {
                      risk_factor_en = risk_factor_en + ", " + r.risk_name_en;

                  }else{

                      risk_factor_en = r.risk_name_en;
                  }

            }




             var occs = from c in dbConnect.occupational_health_report_actions
                        join r in dbConnect.occupational_health_reports on c.occupational_health_report_id equals r.id
                        where c.health_id == Convert.ToInt32(health_id)
                        select new
                        {
                            occupational_name_en = r.name_en,
                            occupational_name_th = r.name_th

                        };
        
            string occupational_th = "";
            string occupational_en = "";
  
            foreach (var oc in occs)
            {
                  if(occupational_th!="")
                  {
                      occupational_th = occupational_th + ", " + oc.occupational_name_th;

                  }else{

                      occupational_th = oc.occupational_name_th;
                  }


                    if(occupational_en!="")
                  {
                      occupational_en = occupational_en + ", " + oc.occupational_name_en;

                  }else{

                      occupational_en = oc.occupational_name_en;
                  }

            }


            String ret = "";
            String domain_name = System.Configuration.ConfigurationManager.AppSettings["pathhost"];

            ret += this.setHeaderEmailTemplate("OH&amp;S Reporting Health Rehabilitation Notification");

            String dear = "Dear " + receiver;
            String dear_th = "เรียน " + receiver;
            String health_report = "Health Rehabilitation report: " + health_code;
            String health_report_th = "รายงานแผนฟื้นฟูสุขภาพพนักงาน: " + health_code;
            String action_required = "";
            String action_required_th = "";
            String action_detail_email = "";
            String action_detail_email_th = "";
            String link_to_system = "";
            String link_to_system_th = "";
            String due_date_en = "";
            String due_date_th = "";

            ret += this.addBodyEmailTemplate("Dear ", receiver, "เรียน ", receiver);
            ret += this.addBodyEmailTemplate("Health Rehabilitation report:: ", health_code, "รายงานแผนฟื้นฟูสุขภาพพนักงาน: ", health_code);
            
            ret += this.addBodyEmailTemplate("Employee Name: ", health_name_en, "ชื่อพนักงาน: ", health_name_th);
            ret += this.addBodyEmailTemplate("Company: ", company_name, "บริษัท: ", company_name);
            ret += this.addBodyEmailTemplate("Function: ", function_name, "สายงาน: ", function_name);
            ret += this.addBodyEmailTemplate("Department: ", department_name, "ฝ่าย: ", department_name);
            ret += this.addBodyEmailTemplate("Division: ", division_name, "ส่วน: ", division_name);

            ret += this.addBodyEmailTemplate("Risk Factor Related to Work: ", risk_factor_en, "ปัจจัยเสี่ยงการทำงาน: ", risk_factor_th);
            ret += this.addBodyEmailTemplate("Occupational Health Report: ", occupational_en, "ผลตรวจสุขภาพตามปัจจัยเสี่ยงที่พบความผิดปกติ: ", occupational_th);



            if (type == 1)
            {
                action_required = "";
                action_required_th = "";
                link_to_system = "<a href=\"" + domain_name + "/healthform.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/healthform.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

               // ret += this.addBodyEmailTemplate("Name of reporter: ", reporter_name_en, "ผู้รายงานแหล่งอันตราย: ", reporter_name_th);
          

                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
             }
            else if (type == 2)
            {
                action_required = "Please close the health rehabilitation report";
                action_required_th = "กรุณาปิดรายงานแผนฟื้นฟูสุขภาพพนักงาน";
                link_to_system = "<a href=\"" + domain_name + "/healthform3.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/healthform3.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
              
            }
            else if (type == 3)
            {
                action_required = "Your request has been rejected, please verify and resubmit";
                action_required_th = "การร้องขอปิดรายงานแผนฟื้นฟูสุขภาพพนักงานของคุณถูกปฏิเสธ กรุณาตรวจสอบและขอปิดรายงานอีกครั้ง";
                link_to_system = "<a href=\"" + domain_name + "/healthform.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/healthform.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }

                var query_rej = (from c in dbConnect.log_request_close_healths
                                 where c.health_id == Convert.ToInt32(health_id)
                                 select new
                                 {
                                     remark = c.remark,
                                     created_at = c.created_at
                                 }).OrderByDescending(x => x.created_at).Take(1);
                string remark = "";

                foreach (var rcj in query_rej)
                {
                    remark = rcj.remark;
                }

                ret += this.addBodyEmailTemplate("Reject reason : ", remark, "เหตุผล : ", remark);
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

            }
            else if (type == 4)
            {
                action_required = "Overdue! Please close health rehabilitation report immediately.";
                action_required_th = "Overdue! Please close health rehabilitation report immediately.";
                link_to_system = "<a href=\"" + domain_name + "/healthform2.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/healthform2.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");

            }else if (type == 5)
            {
                action_required = "";
                action_required_th = "";
                link_to_system = "<a href=\"" + domain_name + "/healthform2.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/healthform2.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                // ret += this.addBodyEmailTemplate("Name of reporter: ", reporter_name_en, "ผู้รายงานแหล่งอันตราย: ", reporter_name_th);


                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else if (type == 6)
            {
                action_required = "Action \"" + action_detail + "\" is due today.  Please proceed immediately.";
                action_required_th = "งาน \"" + action_detail + "\" กำหนดเสร็จวันนี้. กรุณาดำเนินการทันที";
                //link_to_system = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>Link to the system</a>";
                //link_to_system_th = "<a href=\"" + domain_name + "/incidentform3.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }

            }
            else if (type == 7)
            {
                action_required = "Overdue! Action \"" + action_detail + "\". Please proceed immediately";
                action_required_th = "งาน \"" + action_detail + "\" เลยเสร็จวันนี้ กรุณาดำเนินการโดยด่วน";
                link_to_system = "<a href=\"" + domain_name + "/helathform2.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>Link to the system</a>";
                link_to_system_th = "<a href=\"" + domain_name + "/healthform2.aspx?pagetype=view&id=" + health_id + "\" target='_blank'>เข้าสู่ระบบ</a>";

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);
                }
                ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            
            }
            else if (type == 8)
            {
                action_required = "Please complete your action";
                action_required_th = "กรุณาดำเนินการตามมาตรการการฟื้นฟูสุขภาพ";

              //  link_to_system = "<a href=\"" + domain_name + "/myactionhazard.aspx\" target='_blank'>Link to the system</a>";
              //  link_to_system_th = "<a href=\"" + domain_name + "/myactionhazard.aspx\" target='_blank'>เข้าสู่ระบบ</a>";

                ret += this.addBodyEmailTemplate("Action assign by: ", assign_action_by, "สร้างงานโดย: ", assign_action_by);

                ret += this.addBodyEmailTemplate("Action required: ", action_required, "งานในระบบที่ต้องทำ: ", action_required_th);

                if (role_action)//ถ้าเป็นคนทำต้องใส่ action required
                {
                    ret += this.addBodyEmailTemplate("Action detail: ", action_detail, "รายละเอียดของงาน: ", action_detail);
                }
               // ret += this.addBodyEmailTemplate(link_to_system, "", link_to_system_th, "");
            }
            else
            {
                //ret = "";
            }

            ret += this.setFooterEmailTemplate();

            //return System.Web.HttpUtility.UrlEncode(ret);
            return ret;
        }

        public String getSubjectName(int type, String incident_code, String title)
        {
            String ret = "";
            if(type ==1)
            {
                ret = "New! Incident report |" + incident_code + " : " + title;
            }
            else if(type == 2)
            {
                ret = "Incident report |" + incident_code + " :Request to verify Incident  form 1&2";
            }
            else if (type == 3)
            {
                ret = "Incident report |" + incident_code + " : " + title;
            }
            else if (type == 4 || type == 5)
            {
                ret = "Incident report |" + incident_code + " : New Action!";
            }
            else if (type == 6)
            {
                ret = "Incident report |" + incident_code + " : Request to close action";
            }
            else if (type == 7)
            {
                ret = "Incident report |" + incident_code + " : Action closed";
            }
            else if (type == 8)
            {
                ret = "Incident report |" + incident_code + " : Action not close";
            }
            else if (type == 9)
            {
                ret = "Incident report |" + incident_code + " : Action is due";
            }
            else if (type == 10)
            {
                ret = "Incident report |" + incident_code + " : Action is overdue";
            }
            else if (type == 11)
            {
                ret = "Incident report |" + incident_code + " : Request to close the incident report";
            }
            else if (type == 12)
            {
                ret = "Incident report |" + incident_code + " : Reject to close incident report";
            }
            else if (type == 13)
            {
                ret = "Incident report |" + incident_code + " : Close completed";
            }
            else if (type == 14)
            {
                ret = "Incident report |" + incident_code + " : " + title; ;
            }
            else if (type == 15)
            {
                ret = "Incident report |" + incident_code + " : " + title; ;
            }
            else if (type == 16)
            {
                ret = "Incident report |" + incident_code + " : Serious incident! " + title; ;
            }
            else if (type == 17)
            {
                ret = "Incident report |" + incident_code + " : " + title;
            }
            else if (type == 18)
            {
                ret = "Incident report |" + incident_code + " : " + title;
            }
            else if (type == 19)
            {
                ret = "Incident report |" + incident_code + " : Reject incident report";
            }
            else if (type == 20)
            {
                ret = "Incident report |" + incident_code + " : " + title;
            }
            else if (type == 21)
            {
                ret = "Incident report |" + incident_code + " : " + title;
            }
            else if (type == 22)
            {
                ret = "Incident report |" + incident_code + " : " + title + " (Delay of Verification)";
            }
            else if (type == 23)
            {
                ret = "Incident report |" + incident_code + " : " + title + " (Delay of Investigation and Action)";
            }
            else if (type == 24)
            {
                ret = "Incident report |" + incident_code + " : Exemption incident report";
            }
            else if (type == 25)
            {
                ret = "Incident report |" + incident_code + " : Corrective Action is overdue";
            }
            else if (type == 26)
            {
                ret = "Incident report |" + incident_code + " : Preventive Action is overdue";
            }
            else if (type == 27)
            {
                ret = "Incident report |" + incident_code + " : Consequence Management is overdue";
            }

            return ret;
        }

        public String getHazardSubjectName(int type, String incident_code, String title)
        {
            String ret = "";
            if (type == 1)
            {
                ret = "New! Hazard report |" + incident_code + " : " + title;
            }
            else if (type == 2)
            {
                ret = "Hazard report |" + incident_code + " :Request to create action";
            }
            else if (type == 3 || type == 4)
            {
                ret = "Hazard report |" + incident_code + " : New Action!";
            }
            else if (type == 5)
            {
                ret = "Hazard report |" + incident_code + " : Request to close action";
            }
            else if (type == 6)
            {
                ret = "Hazard report |" + incident_code + " : Action closed";
            }
            else if (type == 7)
            {
                ret = "Hazard report |" + incident_code + " : Action not close";
            }
            else if (type == 10)
            {
                ret = "Hazard report |" + incident_code + " : Action is due today";
            }
            else if (type == 11)
            {
                ret = "Hazard report |" + incident_code + " : Action is overdue";
            }
            else if (type == 12)
            {
                ret = "Hazard report |" + incident_code + " : Request to close";
            }
            else if (type == 13)
            {
                ret = "Hazard report |" + incident_code + " : Reject to close";
            }
            else if (type == 14)
            {
                ret = "Hazard report |" + incident_code + " : Close completed";
            }
            else if (type == 15)
            {
                ret = "Hazard report |" + incident_code + " : " + title;
            }
            else if (type == 16)
            {
                ret = "Hazard report |" + incident_code + " : Reject hazard report";
            }
            else if (type == 17)
            {
                ret = "Hazard report |" + incident_code + " : " + title;
            }
            else if (type == 18)
            {
                ret = "Hazard report |" + incident_code + " : " + title;
            }
            else if (type == 19)
            {
                ret = "Hazard report |" + incident_code + " : " + title + " (Delay of Verification)";
            }
            else if (type == 20)
            {
                ret = "Hazard report |" + incident_code + " : " + title + " (Delay of Process of Action)";
            }
            else if (type == 21)
            {
                ret = "Hazard report |" + incident_code + " has been closed";
            }
            else if (type == 22)
            {
                ret = "Hazard report |" + incident_code + " : Action is overdue";
            }

            return ret;
        }


        public String getSotSubjectName(int type, String sot_code)
        {
            String ret = "";
            if (type == 1)
            {
                ret = "New! SOT report |" + sot_code;
            }
            else if (type == 2)
            {
                ret = "SOT report |" + sot_code + " : Request to create action";
            }
            else if (type == 3 || type == 4)
            {
                ret = "SOT report |" + sot_code + " : New Action!";
            }
            else if (type == 5)
            {
                ret = "SOT report |" + sot_code + " : Request to close action";
            }
            else if (type == 6)
            {
                ret = "SOT report |" + sot_code + " : Action closed";
            }
            else if (type == 7)
            {
                ret = "SOT report |" + sot_code + " : Action not close";
            }
            else if (type == 10)
            {
                ret = "SOT report |" + sot_code + " : Action is due today";
            }
            else if (type == 11)
            {
                ret = "SOT report |" + sot_code + " : Action is overdue";
            }
         

            return ret;
        }




        public String getHealthSubjectName(int type, String health_code, String title)
        {
            String ret = "";
            if (type == 1)
            {
                ret = "New! Health Rehabilitation report |" + health_code + " - Private and Confidential";
            }
            else if (type == 2)
            {
                ret = "Health Rehabilitation report |" + health_code + " :Request to close report" + " - Private and Confidential";
            }
            else if (type == 3)
            {
                ret = "Health Rehabilitation report |" + health_code + " :Reject to close report!" + " - Private and Confidential";
            }
            else if (type == 4)
            {
                ret = "Health Rehabilitation report |" + health_code + " - Private and Confidential";
            }
            else if (type == 5)
            {
                ret = "Health Rehabilitation report |" + health_code + " - Private and Confidential";
            }
            else if (type == 6)
            {
                ret = "Health Rehabilitation report |" + health_code + " : Action is due today" + " - Private and Confidential";
            }
            else if (type == 7)
            {
                ret = "Health Rehabilitation report |" + health_code + " : Action is overdue" + " - Private and Confidential";
            }
            else if (type == 8)
            {
                ret = "Health Rehabilitation report |" + health_code + " : New Action!" + " - Private and Confidential";
            }
          
            return ret;
        }

        public void insertIncidentNotify(int type, int incident_id, String employee_id, String incedent_code, String title, String occur_date, String description, String action_detail, DateTime? action_due_date, String function_name, String department_name,String timezone,bool role_action)
        {
            String ret_en = "";
            String ret_th = "";
            String new_line = "<br />";

            if (type == 1 || type == 50)
            {
                ret_en = "New! Incident report |" + incedent_code + new_line;
                ret_en += title;
                ret_th = "รายงานอุบัติการณ์ใหม่ |" + incedent_code + new_line;
                ret_th += title;
            }
            else if (type == 2)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Request to verify Incident form 1&2";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "ร้องขอให้ตรวจสอบฟอร์ม 1 และ 2";
            }
            else if (type == 3)
            {
                ret_en = "Serious Incident report |" + incedent_code + new_line;
                ret_en += title;
                ret_th = "Serious Incident report |" + incedent_code + new_line;
                ret_th += title;
            }
            else if (type == 4)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "New action!" + new_line;
                ret_en += action_detail;
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "งานใหม่!" + new_line;
                ret_th += action_detail;
            }
            else if (type == 5)
            {
                
            }
            else if (type == 6)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Request to close action" + new_line;
                ret_en += action_detail;
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "ร้องขอปิดงาน" + new_line;
                ret_th += action_detail;
            }
            else if (type == 7)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Action closed" + new_line;
                ret_en += action_detail;
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "ปิดงานสำเร็จ" + new_line;
                ret_th += action_detail;
            }
            else if (type == 8)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Action not close" + new_line;
                ret_en += action_detail;
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "ปิดงานไม่สำเร็จ" + new_line;
                ret_th += action_detail;
            }
            else if (type == 9)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Action is due today" + new_line;
                ret_en += action_detail;
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "ถึงกำหนดปิดงาน" + new_line;
                ret_th += action_detail;
            }
            else if (type == 10)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Action is overdue" + new_line;
                ret_en += action_detail;
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "เลยกำหนดปิดงาน" + new_line;
                ret_th += action_detail;
            }
            else if (type == 11)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Request to close the incident report";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "ร้องขอปิดรายงานอุบัติการณ์";
                
            }
            else if (type == 12)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Reject to close incident report";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "ปฏิเสธการปิดรายงานอุบัติการ";
            }
            else if (type == 13)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Close completed";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "ปิดรายงานอุบัติการสำเร็จ";
            }
            else if (type == 14)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Please verify and confirm";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "Please verify and confirm";
            }
            else if (type == 15)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Please verify and confirm";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "Please verify and confirm";
            }
            else if (type == 16)
            {
                ret_en = "Incident report | Serious incident! " + incedent_code + new_line;
                ret_en += "NA";
                ret_th = "อุบัติการณ์ | Serious incident! " + incedent_code + new_line;
                ret_th += "NA";
            }
            else if (type == 17)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Please investigate and create corrective and preventive action";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "Please investigate and create corrective and preventive action";
            }
            else if (type == 18)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Overdue! Please verify the incident report immediately";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "Overdue! Please verify the incident report immediately";
            }
            else if (type == 19)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Reject incident report";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "ปฏิเสธรายงานอุบัติการณ์";
            }
            else if (type == 20)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Overdue! Please investigate and create corrective and preventive action immediately";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "Overdue! Please investigate and create corrective and preventive action immediately";
            }
            else if (type == 21)
            {
                ret_en = "Incident report |" + incedent_code + new_line;
                ret_en += "Overdue! Please close the incident report immediately";
                ret_th = "อุบัติการณ์ |" + incedent_code + new_line;
                ret_th += "Overdue! Please close the incident report immediately";
            }
            else
            {
                ret_en = "";
                ret_th = "";
            }

            if(ret_en != "")
            {
                safetys4dbDataContext dbConnect = new safetys4dbDataContext();
                notification InsertObj = new notification();
                InsertObj.employee_id = employee_id;
                InsertObj.body_en = ret_en;
                InsertObj.body_th = ret_th;
                InsertObj.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                InsertObj.flow = type;
                InsertObj.incident_hazard_id = incident_id;
                InsertObj.type = "Incident";
				InsertObj.read_flag = 0;
                dbConnect.notifications.InsertOnSubmit(InsertObj);
                dbConnect.SubmitChanges();

            }

            
            //return System.Web.HttpUtility.UrlEncode(ret);
            
        }

        public void insertHazardNotify(int type, int incident_id, String employee_id, String incedent_code, String title, String occur_date, String description, String action_detail, DateTime? action_due_date, String function_name, String department_name,String timezone,bool role_action)
        {
            String ret_en = "";
            String ret_th = "";
            String new_line = "<br />";

            if (type == 1 || type == 50)
            {
                ret_en = "New! Hazard report " + incedent_code + new_line;
                ret_en += title;
                ret_th = "รายงานแหล่งอันตรายใหม่! " + incedent_code + new_line;
                ret_th += title;
            }
            else if (type == 2)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Request to create action";
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "ให้ดำเนินการสร้างมาตรการแก้ไขและป้องกัน";
            }
            else if (type == 3)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "New action!" + new_line;
                ret_en += action_detail;
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "งานใหม่!" + new_line;
                ret_th += action_detail;
            }
            else if (type == 4)
            {
            }
            else if (type == 5)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Request to close action" + new_line;
                ret_en += action_detail;
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "ร้องขอปิดงาน" + new_line;
                ret_th += action_detail;
            }
            else if (type == 6)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Action closed" + new_line;
                ret_en += action_detail;
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "ปิดงานสำเร็จ" + new_line;
                ret_th += action_detail;
            }
            else if (type == 7)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Action not close" + new_line;
                ret_en += action_detail;
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "ปิดงานไม่สำเร็จ" + new_line;
                ret_th += action_detail;
            }
            else if (type == 10)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Action is due today" + new_line;
                ret_en += action_detail;
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "ถึงกำหนดปิดงาน" + new_line;
                ret_th += action_detail;
            }
            else if (type == 11)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Action is overdue today" + new_line;
                ret_en += action_detail;
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "เลยกำหนดปิดงาน" + new_line;
                ret_th += action_detail;
            }
            else if (type == 12)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Request to close the hazard report";
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "ร้องขอปิดรายงานแหล่งอันตราย";
            }
            else if (type == 13)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Reject to close the hazard report";
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "ปฏิเสธการปิดรายงานแหล่งอันตราย";
            }
            else if (type == 14)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Close completed";
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "ปิดรายงานแหล่งอันตราย";
            }
            else if (type == 15)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Overdue!";
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "Overdue!";
            }
            else if (type == 16)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Reject hazard report";
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "ปฏิเสธรายงานแหล่งอันตราย";
            }
            else if (type == 17)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Overdue! Please create action immediately";
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "Overdue! Please create action immediately";
            }
            else if (type == 18)
            {
                ret_en = "Hazard report " + incedent_code + new_line;
                ret_en += "Overdue! Please close hazard report immediately";
                ret_th = "แหล่งอันตราย " + incedent_code + new_line;
                ret_th += "Overdue! Please close hazard report immediately";
            }
            else
            {
                ret_en = "";
                ret_th = "";
            }

            if (ret_en != "")
            {
                safetys4dbDataContext dbConnect = new safetys4dbDataContext();
                notification InsertObj = new notification();
                InsertObj.employee_id = employee_id;
                InsertObj.body_en = ret_en;
                InsertObj.body_th = ret_th;
                InsertObj.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                InsertObj.flow = type;
                InsertObj.incident_hazard_id = incident_id;
				InsertObj.read_flag = 0;
                InsertObj.type = "Hazard";

                dbConnect.notifications.InsertOnSubmit(InsertObj);
                dbConnect.SubmitChanges();

            }

            //return System.Web.HttpUtility.UrlEncode(ret);
            //return ret;
        }






        public void insertSotNotify(int type, int sot_id, String employee_id, String sot_code, String occur_date,String occur_date_end, String action_detail, DateTime? action_due_date, String function_name, String department_name, String timezone,bool role_action)
        {
            String ret_en = "";
            String ret_th = "";
            String new_line = "<br />";

            if (type == 1 || type == 50)
            {
                ret_en = "New! SOT report " + sot_code + new_line;
               // ret_en += title;
                ret_th = "รายงาน SOT ใหม่! " + sot_code + new_line;
              //  ret_th += title;
            }
            else if (type == 2)
            {
                ret_en = "SOT report " + sot_code + new_line;
                ret_en += "Request to create action";
                ret_th = "SOT " + sot_code + new_line;
                ret_th += "ให้ดำเนินการสร้างมาตรการแก้ไขและป้องกัน";
            }
            else if (type == 3)
            {
                ret_en = "SOT report " + sot_code + new_line;
                ret_en += "New action!" + new_line;
                ret_en += action_detail;
                ret_th = "SOT " + sot_code + new_line;
                ret_th += "งานใหม่!" + new_line;
                ret_th += action_detail;
            }
            else if (type == 4)
            {
            }
            else if (type == 5)
            {
                ret_en = "SOT report " + sot_code + new_line;
                ret_en += "Request to close action" + new_line;
                ret_en += action_detail;
                ret_th = "SOT " + sot_code + new_line;
                ret_th += "ร้องขอปิดงาน" + new_line;
                ret_th += action_detail;
            }
            else if (type == 6)
            {
                ret_en = "SOT report " + sot_code + new_line;
                ret_en += "Action closed" + new_line;
                ret_en += action_detail;
                ret_th = "SOT " + sot_code + new_line;
                ret_th += "ปิดงานสำเร็จ" + new_line;
                ret_th += action_detail;
            }
            else if (type == 7)
            {
                ret_en = "SOT report " + sot_code + new_line;
                ret_en += "Action not close" + new_line;
                ret_en += action_detail;
                ret_th = "SOT " + sot_code + new_line;
                ret_th += "ปิดงานไม่สำเร็จ" + new_line;
                ret_th += action_detail;
            }
            else if (type == 10)
            {
                ret_en = "SOT report " + sot_code + new_line;
                ret_en += "Action is due today" + new_line;
                ret_en += action_detail;
                ret_th = "SOT " + sot_code + new_line;
                ret_th += "ถึงกำหนดปิดงาน" + new_line;
                ret_th += action_detail;
            }
            else if (type == 11)
            {
                ret_en = "SOT report " + sot_code + new_line;
                ret_en += "Action is overdue today" + new_line;
                ret_en += action_detail;
                ret_th = "SOT " + sot_code + new_line;
                ret_th += "เลยกำหนดปิดงาน" + new_line;
                ret_th += action_detail;
            }
            
            else if (type == 15)
            {
                ret_en = "SOT report " + sot_code + new_line;
                ret_en += "Overdue!";
                ret_th = "SOT " + sot_code + new_line;
                ret_th += "Overdue!";
            }
            else
            {
                ret_en = "";
                ret_th = "";
            }

            if (ret_en != "")
            {
                safetys4dbDataContext dbConnect = new safetys4dbDataContext();
                notification InsertObj = new notification();
                InsertObj.employee_id = employee_id;
                InsertObj.body_en = ret_en;
                InsertObj.body_th = ret_th;
                InsertObj.created_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                InsertObj.flow = type;
                InsertObj.incident_hazard_id = sot_id;
                InsertObj.read_flag = 0;
                InsertObj.type = "SOT";

                dbConnect.notifications.InsertOnSubmit(InsertObj);
                dbConnect.SubmitChanges();

            }

            //return System.Web.HttpUtility.UrlEncode(ret);
            //return ret;
        }




       
    
    
        private String setHeaderEmailTemplate(String title)
        {
            String ret = "";
            ret += "<table class=\"\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"800\" style=\"width:800px\">";
            ret += "<tbody>";
            ret += "<tr>";
            ret += "<td colspan=\"2\" style=\"background:red;padding:2.5pt 2.5pt 2.5pt 2.5pt\">";
            ret += "<p style=\"margin:0in 0in 0.0001pt;font-size:14pt;font-family:'Angsana New',serif\">";
            ret += "<span><span style=\"font-size:8.0pt;font-family:'Arial',sans-serif\">&nbsp;</span></span><span style=\"font-size:12.0pt\"></span></p>";
            ret += "</td>";
            ret += "</tr>";
            ret += "<tr>";
            ret += "<td width=\"33%\" style=\"width:33.86%;background:white;padding:7.5pt 7.5pt 7.5pt 7.5pt\">";
            ret += "<p style=\"margin:0in 0in 0.0001pt;font-size:14pt;font-family:'Angsana New',serif\">";
            ret += "<img width=\"175\" height=\"57\" id=\"\" style=\"width:1.8229in;height:0.5937in\" aria-expanded=\"false\" src=\"https://ohsreport.siamcitycement.com/template/img/Logo_Eng.png\" class=\"\"></p>";
            ret += "</td>";
            ret += "<td width=\"66%\" style=\"width:66.14%;background:white;padding:7.5pt 7.5pt 7.5pt 7.5pt\">";
            ret += "<p style=\"margin-right:0in;margin-left:0in;font-size:12pt;font-family:'Times New Roman',serif\">";
            ret += "<span><b><span style=\"font-size:14.0pt;font-family:'Arial',sans-serif;color:black\">" + title + "</span></b></span><b></b></p>";
            ret += "</td>";
            ret += "</tr>";
            ret += "<tr>";
            ret += "<td colspan=\"2\" style=\"background:white;padding:15.0pt 15.0pt 15.0pt 15.0pt\">";
            ret += "<table class=\"\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-collapse:collapse;border:none;width:800px\">";
            ret += "<tbody>";

            return ret;
        }

        private String addBodyEmailTemplate(String field_en, String value_en, String field_th, String value_th)
        {
            String ret = "";
            ret += "<tr>";
            ret += "<td width=\"100%\" valign=\"top\" style=\"width:100%;padding:0in 5.4pt 0in 5.4pt; overflow-wrap: break-word;\">";
            ret += "<p style=\"margin:0in 0in 0.0001pt;font-size:14pt;font-family:'Angsana New',serif\">";
            ret += "<span><b><span style=\"font-family:'Browallia New',sans-serif;color:black\">" + field_en + " </span></b></span>";
            ret += "<span><span style=\"font-family:'Browallia New',sans-serif;color:black\">";
            ret += value_en;
            ret += "</span></span></p>";
            ret += "</td>";
            /*ret += "<td width=\"50%\" valign=\"top\" style=\"width:50%;padding:0in 5.4pt 0in 5.4pt; overflow-wrap: break-word;\">";
            ret += "<p style=\"margin:0in 0in 0.0001pt;font-size:14pt;font-family:'Angsana New',serif\">";
            ret += "<span><b><span style=\"font-family:'Browallia New',sans-serif;color:black\">" + field_th + " </span></b></span>";
            ret += "<span><span style=\"font-family:'Browallia New',sans-serif;color:black\">";
            ret += value_th;
            ret += "</span></span></p>";
            ret += "</td>";*/
            ret += "</tr>";
            return ret;
        }

        private String setFooterEmailTemplate()
        {
            String ret = "";
            ret += "</tbody>";
            ret += "</table>";
            ret += "<p style=\"margin:0in 0in 0.0001pt;font-size:14pt;font-family:'Angsana New',serif\">";
            ret += "<span><b><span style=\"font-size:10.0pt;font-family:'Arial',sans-serif;color:black\">&nbsp;</span></b></span></p>";
            ret += "</td>";
            ret += "</tr>";
            ret += "<tr>";
            ret += "<td colspan=\"2\" style=\"background:red;padding:7.5pt 7.5pt 7.5pt 7.5pt\">";
            ret += "<p style=\"margin:0in 0in 0.0001pt;font-size:14pt;font-family:'Angsana New',serif\">";
            ret += "<span style=\"font-size:10.0pt;font-family:'Arial',sans-serif;color:white\">Do not reply to this mail</span></p>";
            ret += "<p style=\"margin:0in 0in 0.0001pt;font-size:14pt;font-family:'Angsana New',serif\">";
            ret += "<span style=\"font-size:10.0pt;font-family:'Arial',sans-serif;color:white\">For more information please contact your OHS representatives</span><span style=\"font-size:8.0pt;font-family:'Calibri',sans-serif;color:white\"></span></p>";
            ret += "</td>";
            ret += "</tr>";
            ret += "</tbody>";
            ret += "</table>";
            return ret;
        }


    
    }

    
}