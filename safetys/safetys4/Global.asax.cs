using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace safetys4
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
        }



        void Application_BeginRequest(Object sender, EventArgs e) 
        {     
             // Code that runs on application startup                                                            
            HttpCookie cookie = HttpContext.Current.Request.Cookies["lang"];

            if (cookie != null && cookie.Value != null)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");

            }
        }


        //void Session_Start(object sender, EventArgs e)
        //{
        //    if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
        //    {

              
        //    }
        //    else
        //    {

        //        Response.Redirect("login.aspx");
        //    }
        //}





    }
}