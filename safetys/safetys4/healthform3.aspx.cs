using safetys4.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class healthform3 : System.Web.UI.Page
    {
        string PageType = "add";
        string id = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
            {
                PageType = Request.QueryString["PageType"];

                if (!IsPostBack)
                {
                    bool result = Permission.checkPermision("report health3 view", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
                    }


                    setPermissionByStatus();

                }

            }
            else
            {
                string original_url = Server.UrlEncode(Context.Request.RawUrl);
                Response.Redirect("login.aspx?returnUrl=" + original_url);
            }
        }

        protected void btHealthedit_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("healthform3.aspx?pagetype=edit&id=" + id);
        }


        protected void setEdit()
        {

            btHealthedit.Style["visibility"] = "hidden";

        }


        protected void setView()
        {

            request_approve1.Disabled = true;
            request_approve2.Disabled = true;
            txtreason.Disabled = true;
            btUpdate.Style["visibility"] = "hidden";


        }


        protected void setPermissionByStatus()
        {
            PageType = Request.QueryString["PageType"];
            id = Request.QueryString["id"];

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            byte step = 0;
            int status = 1;
            var query = from c in dbConnect.healths
                        where c.id == Convert.ToInt32(id)
                        select c;

            foreach (health rc in query)
            {
                status = rc.process_status;
                step = Convert.ToByte(rc.step_form);
            }

            ArrayList gr = new ArrayList();

            gr = Session["group"] as ArrayList;


            if (PageType.Equals("edit"))
            {
                id = Request.QueryString["id"];
                setEdit();
            }
            else if (PageType.Equals("view"))
            {
                id = Request.QueryString["id"];
                setView();

            }

            if (step < 3)
            {
                Response.Redirect("healthform" + step + ".aspx?pagetype=view&id=" + id);
            }


        }

        protected void step1_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("healthform.aspx?pagetype=view&id=" + id);
        }

        protected void step2_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("healthform2.aspx?pagetype=view&id=" + id);
        }





    }
}