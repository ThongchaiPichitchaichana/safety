using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using safetys4.App_Code;
using System.Collections;
using System.Web.Services;
using System.Web.Providers.Entities;

namespace safetys4
{
    public partial class allhazard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Cache.SetCacheability(HttpCacheability.NoCache);          
            Response.Cache.SetNoStore();
            //ใส่เพื่อให้ตอน back กลับไม่ต้องจดจำ fiter search

            try
            {

                if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
                {

                    bool result = Permission.checkPermision("all hazards", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
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
                    objInsert.error_message = ex.Message;
                    objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));


                    dbConnect.action_logs.InsertOnSubmit(objInsert);

                    dbConnect.SubmitChanges();

                }

            }

        }

    





    }
}