using safetys4.App_Code;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class testemail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string datetime = "";
            //var calendar = new ThaiBuddhistCalendar();
            //string httpTime = "Feb 29 2020 12:00AM";
            //DateTime time = Convert.ToDateTime(httpTime);
            var dt = DateTime.ParseExact("Feb 29 2020 12:00AM", "MMM dd yyyy hh:mmtt", CultureInfo.InvariantCulture);
            datetime = dt.ToString("dd/MM/yyyy",  CultureInfo.CreateSpecificCulture("th-TH"));
            Response.Write(datetime);
            //var culture = new System.Globalization.CultureInfo("th-TH", true);
            ////culture.DateTimeFormat.Calendar = calendar;
            // datetime = dt.ToString("dd/MM/yyyy",  CultureInfo.CreateSpecificCulture("th-TH"));
           // CultureInfo thCulture = new CultureInfo("th-TH");
            //DateTime oDate = DateTime.ParseExact("2019-02-28");
           // Response.Write(oDate.ToString("dd/MM/yyyy", culture));
            // string duedate = FormatDates.getDateShowFromDate(Convert.ToDateTime("Feb 29 2020 12:00AM"), "th");
             //datetime = dt.ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("th-TH"));
             //Response.Write(datetime);
            //DateTime datetime = DateTime.Parse(dt, new System.Globalization.CultureInfo("th-TH", true));
            // ThreadPool.QueueUserWorkItem(delegate
            //{
            //    safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            //    try
            //    {

            //        MailMessage mail = new MailMessage();
            //        SmtpClient client = new SmtpClient();
            //        client.Port = 25;
            //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //        client.UseDefaultCredentials = false;
            //        //client.Credentials = new System.Net.NetworkCredential("TERMINUS.REPORT@sccc.co.th", "");
            //        mail.From = new MailAddress("noreply-ohsreport@siamcitycement.com");
            //        mail.To.Add("kpnmtu@gmail.com");
            //        //client.EnableSsl = true;
            //        client.Host = "10.254.1.244";
            //        mail.Subject = "test";
            //        mail.Body = "test";
            //        mail.IsBodyHtml = true;
            //        client.Send(mail);

            //    }
            //    catch (Exception ex)
            //    {
            //        /////////////////////////////////log////////////////////////////////////////////////////


            //        action_log objInsert = new action_log();
            //        objInsert.function_name = "sendemail";
            //        objInsert.file_name = "safetyemail";
            //        //objInsert.send_to = "test";
            //        //objInsert.subject_email = "test";
            //        //objInsert.error_message = ex.ToString();

            //        objInsert.created = DateTime.Now;

            //        dbConnect.action_logs.InsertOnSubmit(objInsert);

            //        dbConnect.SubmitChanges();

            //        //////////////////////////////////end log////////////////////////////////////////////
            //        //Response.Write("error:"+ex.ToString());
            //    }
            //});

            
            //Response.Write(DateTime.UtcNow);
            //Response.Write(DateTime.UtcNow.AddHours(Convert.ToDouble("+7")));
            //Response.Write(DateTime.UtcNow.AddHours(Convert.ToDouble("-7")));
        }
    }
}