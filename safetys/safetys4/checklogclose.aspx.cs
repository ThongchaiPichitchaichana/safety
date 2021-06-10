using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class checklogclose : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var v = from c in dbConnect.action_logs                   
                    select c;


            DataSet ds = new DataSet();
            DataTable ds2 = new DataTable();
            ds2 = ChangeToDatatable.LINQToDataTable(v);
            ds.Tables.Add(ds2);

            GridView1.DataSource = ds2;
            GridView1.DataBind();

        }
    }
}