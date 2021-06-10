using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class dropzoneremove : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string name = Context.Request["name"].ToString();
                string folder = Context.Request["folder"].ToString();
                string type = Context.Request["type"].ToString();

                string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathfile"];

                //string pathfile = string.Format("{0}\\upload\\incident\\" + folder, Server.MapPath(@"\"));
                string pathfile =  string.Format("{0}"+pathupload+type+"\\" + folder+"\\"+name, Server.MapPath(@"\"));
                FileInfo file = new FileInfo(pathfile);
                if (file.Exists)
                {
                    file.Delete();
                }
               
            }
            catch (Exception ex)
            {
                using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
                {
                    action_log insertLog = new action_log();
                    insertLog.function_name = "page_load";
                    insertLog.file_name = "dropzoneremove";
                    insertLog.created = DateTime.Now;
                    insertLog.error_message = ex.Message;
                    dbConnect.action_logs.InsertOnSubmit(insertLog);
                    dbConnect.SubmitChanges();
                }

                Response.Write(ex.Message);

            }
        }
    }
}