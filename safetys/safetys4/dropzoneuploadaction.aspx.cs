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
    public partial class dropzoneuploadaction : System.Web.UI.Page
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

            string id = Request.QueryString["id"];
            string doc_no = "";
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            var q = from c in dbConnect.hazards
                    where c.id == Convert.ToInt32(id)
                    select new
                    {
                        doc_no = c.doc_no
                    };



            foreach (var v in q)
            {
                doc_no = v.doc_no;
            }


            string name_folder = doc_no;
            string file_name = "action_" + FormatDates.getDateTimeMicro(DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())));
            string country = Session["country"].ToString();
            bool isSavedSuccessfully = false;
            string fName = "";
            string typefile = "";

            foreach (string fileName in httpFileCollection)
            {
                HttpPostedFile file = httpFileCollection.Get(fileName);
                //Save file content goes here
                fName = file.FileName;
                typefile = Path.GetExtension(file.FileName);
                //string[] arr_name = fName.Split('.');
               // typefile = arr_name[1];

                if (file != null && file.ContentLength > 0)
                {
                    string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathuploadhazard"];
                   // string pathfolder = string.Format("{0}\\upload\\hazard\\step3\\" + name_folder, Server.MapPath(@"\"));
                    string pathfolder = string.Format("{0}" + pathupload + "step3\\" + country +"\\"+ name_folder, Server.MapPath(@"\"));
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


                    file_name = file_name  + typefile;
                    // var path = string.Format("{0}\\{1}", pathString, file.FileName);
                    var path = string.Format("{0}\\{1}", pathString, file_name);
                    file.SaveAs(path);
                    isSavedSuccessfully = true;
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