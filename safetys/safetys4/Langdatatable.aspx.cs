using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class Langdatatable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("{");
            sb.AppendLine("\"sEmptyTable\": \"" + Resources.Main.sEmptyTable + "\",");
            sb.AppendLine("\"sInfo\": \"" + Resources.Main.sInfo + "\",");
            sb.AppendLine("\"sInfoEmpty\": \"" + Resources.Main.sInfoEmpty + "\",");
            sb.AppendLine("\"sInfoFiltered\": \"" + Resources.Main.sInfoFiltered +"\",");
            sb.AppendLine("\"sLoadingRecords\": \"" + Resources.Main.sLoadingRecords + "\",");
            sb.AppendLine("\"sProcessing\": \"" + Resources.Main.sProcessing + "\",");
            sb.AppendLine("\"sSearch\": \"" + Resources.Main.sSearch + "\",");
            sb.AppendLine("\"sZeroRecords\": \"" + Resources.Main.sZeroRecords + "\",");
            sb.AppendLine("\"sLengthMenu\": \"" + Resources.Main.sLengthMenu + "\",");
            sb.AppendLine("\"oPaginate\": ");
            sb.AppendLine("{");
            sb.AppendLine("\"sFirst\": \"" + Resources.Main.sFirst + "\",");
            sb.AppendLine("\"sPrevious\": \"" + Resources.Main.sPrevious + "\",");
            sb.AppendLine("\"sNext\": \"" + Resources.Main.sNext + "\",");
            sb.AppendLine("\"sLast\": \"" + Resources.Main.sLast + "\"");
            sb.AppendLine("}");
            sb.AppendLine("}");

           Response.Write(sb.ToString());
        }




    }
}