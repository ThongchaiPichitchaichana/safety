using safetys4.App_Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class dropzoneuploadsot : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] != null && Session["lang"] != null)
            {
                if (!IsPostBack)
                {
                    SaveUploadedFile(Request.Files);
                }

            }
            else
            {

                Response.Redirect("login.aspx");
            }
        }



        public void SaveUploadedFile(HttpFileCollection httpFileCollection)
        {

            string user_id = Request.QueryString["user_id"];
            string reportdate = Request.QueryString["reportdate"];
            string id = Request.QueryString["id"];

            if (reportdate.Trim() == "")
            {
                safetys4dbDataContext dbConnect = new safetys4dbDataContext();

                var q = from c in dbConnect.sots
                        where c.id == Convert.ToInt32(id)
                        select new
                        {
                            reportdate = FormatDates.getDatetimeShow(Convert.ToDateTime(c.report_date), "en"),
                            user_id = c.employee_id
                        };



                foreach (var v in q)
                {
                    reportdate = v.reportdate;
                    user_id = v.user_id;
                }

            }
            else
            {

                reportdate = FormatDates.changeDateTimeUpload(reportdate, Session["lang"].ToString());

            }

            string name_folder = user_id + "_" + FormatDates.getDateTimeNoDash(reportdate.Trim());
            string file_name = FormatDates.getDateTimeMicro(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())));
            //bool isSavedSuccessfully = false;
            string fName = "";
            string typefile = "";

            foreach (string fileName in httpFileCollection)
            {
                HttpPostedFile file = httpFileCollection.Get(fileName);
                //Save file content goes here
                fName = file.FileName;
                typefile = Path.GetExtension(file.FileName);

                if (file != null && file.ContentLength > 0)
                {
                    string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathfile"];
                    //string pathfolder = string.Format("{0}\\upload\\hazard\\"+name_folder, Server.MapPath(@"\"));
                    string pathfolder = string.Format("{0}" + pathupload + "sot\\" + Session["country"].ToString() +"\\"+ name_folder, Server.MapPath(@"\"));
                    if (!Directory.Exists(pathfolder))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(pathfolder);
                    }



                    var originalDirectory = new DirectoryInfo(pathfolder);

                    string pathString = System.IO.Path.Combine(originalDirectory.ToString());

                    // var fileName1 = Path.GetFileName(file.FileName);


                    bool isExists = System.IO.Directory.Exists(pathString);

                    if (!isExists)
                    {
                        System.IO.Directory.CreateDirectory(pathString);

                    }
                      

                    file_name = file_name + typefile;
                    // var path = string.Format("{0}\\{1}", pathString, file.FileName);
                    var path = string.Format("{0}\\{1}", pathString, file_name);
                    file.SaveAs(path);
                    //isSavedSuccessfully = true;

                }

            }
            var result = new
            {
                folder = name_folder,
                name = file_name
            };



            JavaScriptSerializer js = new JavaScriptSerializer();
            Response.Write(js.Serialize(result));




        }
    }
}