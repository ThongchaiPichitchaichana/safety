using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Globalization;
using safetys4.App_Code;

namespace safetys4
{
    public partial class Notificationservice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			String emp_id = (String)Request.Form["emp_id"];
			String nid = (String)Request.Form["nid"];
            string pathhost = System.Configuration.ConfigurationManager.AppSettings["pathhost"];

            if (!String.IsNullOrEmpty(nid))
            {
                safetys4dbDataContext dbConnect = new safetys4dbDataContext();
                notification InsertObj = dbConnect.notifications.Single(n => n.id == Convert.ToInt32(nid));
                InsertObj.read_flag = 1;
                dbConnect.SubmitChanges();

                //Response.Write("{\"result\":\"Success\"}");
            }

			if(!String.IsNullOrEmpty(emp_id))
			{
				safetys4dbDataContext dbConnect = new safetys4dbDataContext();
				var n = (from c in dbConnect.notifications
						 where (c.employee_id == emp_id)
						 select new
						 {
							 created_at = string.Format("{0:dd/MM/yyyy HH:mm:ss}", c.created_at),
							 body_en = c.body_en,
							 body_th = c.body_th,
							 id = c.created_at,
							 notify_id = c.id,
							 incident_hazard_id = c.incident_hazard_id,
							 read_flag = c.read_flag,
							 type = c.type
						 }).OrderByDescending(c => c.id).Take(10);

				String ret = "";
				int count = 0;
				foreach(var v in n)
				{
					
					string link = "";
					if(v.type == "Incident")
					{
						link = pathhost+"/incidentform.aspx?pagetype=view&id=" + v.incident_hazard_id;
					}
					else
					{
						link = pathhost+"/hazardform.aspx?pagetype=view&id=" + v.incident_hazard_id;
					}
					
					if (ret != "") ret += ",";
					ret += "{\"id\":" + v.notify_id + ",\"body_en\":\"" + v.body_en + "\",\"body_th\":\"" + v.body_th + "\",\"created_at\":\"" + v.created_at + "\",\"read_flag\":" + v.read_flag + ",\"link\":\"" + link + "\" }";
					
					if(v.read_flag == 0) 
					{
						count++;
					}
				}

				Response.Write("{\"new\":" + count.ToString() + ", \"notifications\":[" + ret + "]}");
				Response.End();
			}
			
			
			Response.End();
        }
    }
}