using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class TotalinjuryFPEReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Panel secondPanel;
            secondPanel = (Panel)Master.FindControl("menu_sidebar_report");
            secondPanel.Visible = true;

            LinkButton link = (LinkButton)Master.FindControl("btTotalInjuryReport");
            link.Attributes.CssStyle.Add("background-color", "#e6e6e8");


        }
    }
}