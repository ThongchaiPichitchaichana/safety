using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using safetys4.App_Code;

namespace safetys4
{
    public partial class incidentform4 : System.Web.UI.Page
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
                    bool result = Permission.checkPermision("report incident4 view", Session["permission"] as ArrayList);
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



        protected void setEdit()
        {

            btIncidentedit.Style["visibility"] = "hidden";

        }


        protected void setView()
        {

            request_close1.Disabled = true;
            request_close2.Disabled = true;
            txtreason.Disabled = true;
            btUpdate.Style["visibility"] = "hidden";
          

        }


        protected void setPermissionByStatus()
        {
            PageType = Request.QueryString["PageType"];
            id = Request.QueryString["id"];

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
           
            int status = 1;
            byte step = 0;
            var query = from c in dbConnect.incidents
                        where c.id == Convert.ToInt32(id)
                        select c;

            foreach (incident rc in query)
            {
               status= rc.process_status;
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

            if (step < 4)
            {
                Response.Redirect("incidentform" + step + ".aspx?pagetype=view&id=" + id);
            }

        }

        protected void step1_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform.aspx?pagetype=view&id=" + id);
        }

        protected void step2_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform2.aspx?pagetype=view&id=" + id);
        }

        protected void step3_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform3.aspx?pagetype=view&id=" + id);
        }

        protected void btIncidentedit_Click(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];
            Response.Redirect("incidentform4.aspx?pagetype=edit&id=" + id);
        }



    }
}