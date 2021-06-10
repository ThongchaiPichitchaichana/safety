using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using safetys4.App_Code;
using System.Collections;

namespace safetys4
{
    public partial class contractoredit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

             if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
            {
                if (!IsPostBack)
                {
                    bool result = Permission.checkPermision("contractor", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
                    }

                }
            }
             else
             {
                 string original_url = Server.UrlEncode(Context.Request.RawUrl);
                 Response.Redirect("login.aspx?returnUrl=" + original_url);
             }


        }

        protected void btCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("contractor.aspx");
        }
    }
}