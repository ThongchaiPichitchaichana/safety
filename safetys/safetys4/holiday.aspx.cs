using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using safetys4.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class holiday : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_id"] != null && Session["lang"] != null && Session["permission"] != null)
            {
                if (!IsPostBack)
                {
                    bool result = Permission.checkPermision("holiday", Session["permission"] as ArrayList);
                    if (!result)
                    {
                        Response.Redirect("MainMenu.aspx?msg_err=permision");
                    }
                    setPermissionMenuSide();
                    setShowHoliday();
                   
                }

               
            }
            else
            {
                string original_url = Server.UrlEncode(Context.Request.RawUrl);
                Response.Redirect("login.aspx?returnUrl=" + original_url);
            }
        }



        protected void setShowHoliday()
        {
            string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathfile"];
            //string pathfolder = string.Format("{0}\\upload\\holiday\\", Server.MapPath(@"\"));
            string pathfolder = string.Format("{0}" + pathupload + "holiday\\" + Session["country"].ToString() + "\\", Server.MapPath(@"\"));
            string path_file = "upload/holiday/" + Session["country"].ToString()+"/";
            string[] files = Directory.GetFiles(pathfolder, "*")
                                        .Select(Path.GetFileName).OrderByDescending(Path.GetFileName).Take(1)
                                        .ToArray();

            string name_current = "";
            foreach (var d in files)
            {
                name_current = d;
            }

            //imgShowfile.ImageUrl = "~/template/img/excel_icon.png";
            //imgShowfile.ImageUrl = pathfolder + name_current;
            lbname.Text = name_current;

            if(name_current!="")
            {
                lbshowfile.Text = "<a href='" + path_file + name_current + "'><img src='template/img/excel_icon.png'  style='width:110px;height:120px;padding-left:5px;padding-right:5px;'></a>";

            }
        }

        protected void setPermissionMenuSide()
        {
            Panel pn;
            pn = (Panel)Master.FindControl("menu_sidebar_admin");
            pn.Visible = true;

            ArrayList per = new ArrayList();

            per = Session["permission"] as ArrayList;

            if (per.IndexOf("super admin") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btSuperAdminSide");
                link.Visible = true;

            }

            if (per.IndexOf("holiday") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btHolidaySide");
                link.Visible = true;
                link.Attributes.CssStyle.Add("background-color", "#e6e6e8");

            }

            if (per.IndexOf("area management") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btAreaManagementSide");
                link.Visible = true;

            }

            if (per.IndexOf("setting") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btSettingSide");
                link.Visible = true;

            }

            if (per.IndexOf("notify group") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btNotifyGroupSide");
                link.Visible = true;

            }


            if (per.IndexOf("target") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btTargetSide");
                link.Visible = true;

            }

            if (per.IndexOf("work hour") > -1)
            {
                LinkButton link = (LinkButton)Master.FindControl("btWorkHourSide");
                link.Visible = true;

            }



        }

        protected void btSearch_Click(object sender, EventArgs e)
        {

            HttpFileCollection httpFileCollection =  Request.Files;
            string pathupload = System.Configuration.ConfigurationManager.AppSettings["pathfile"];
           
           // string pathfolder = string.Format("{0}\\upload\\holiday\\", Server.MapPath(@"\"));
            string pathfolder = string.Format("{0}" + pathupload + "holiday\\" + Session["country"].ToString()+"\\", Server.MapPath(@"\"));
            string file_name = "";
            string year_select = Request.Form[ddyear.UniqueID];
            for (int i = 0; i < httpFileCollection.Count; i++)
            {
                HttpPostedFile files = httpFileCollection[i];

                if (files != null && files.ContentLength > 0)
                {

                    if (!Directory.Exists(pathfolder))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(pathfolder);
                    }

                    var originalDirectory = new DirectoryInfo(pathfolder);

                    string pathString = System.IO.Path.Combine(originalDirectory.ToString());
                    bool isExists = System.IO.Directory.Exists(pathString);

                    if (!isExists)
                        System.IO.Directory.CreateDirectory(pathString);

                  
                    string type_file = Path.GetExtension(files.FileName);
                    var path = string.Format("{0}\\{1}", pathString,  year_select + type_file);
                    files.SaveAs(path);
                    file_name = year_select + type_file;
                }

            }

            FileStream file = new FileStream(pathfolder+file_name, FileMode.Open, FileAccess.Read);



            XSSFWorkbook workbook = new XSSFWorkbook(file);

            ISheet sheet1 = workbook.GetSheet("Sheet1");

            //sheet1.GetRow(1).GetCell(1).SetCellValue(1);
            //ICell cell = sheet1.GetRow(0).GetCell(0);
           
           safetys4dbDataContext dbConnect = new safetys4dbDataContext();

           ///////////////////////////////////////////ลบอันเก่า ถ้ามีอยู่ก่อน//////////////////////////////////////
           var q = from c in dbConnect.holidays
                   where Convert.ToDateTime(c.holiday_date).Year == Convert.ToInt16(year_select)
                   select c;
            foreach (var a in q)
            {
                dbConnect.holidays.DeleteOnSubmit(a);
            }
            dbConnect.SubmitChanges();
           /////////////////////////////////////////////////////////////////////////////////////////////////

            for (int row = 1; row <= sheet1.LastRowNum; row++)
            {
                if (sheet1.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    ICell cell1 = sheet1.GetRow(row).GetCell(0);
                    ICell cell2 = sheet1.GetRow(row).GetCell(1);

                    string data_date = cell1.StringCellValue;
                    string data_holiday = cell2.StringCellValue;

                    if(data_date!="" &&data_holiday!="")
                    {
                        string rp_date = data_date.Replace(".", "/");

                     
                        holiday objInsert = new holiday();
                        objInsert.holiday_date = FormatDates.changeDateTimeDB(rp_date, "en");
                        objInsert.holiday_name = data_holiday;
                        objInsert.country = Session["country"].ToString();
                        objInsert.created = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));

                        dbConnect.holidays.InsertOnSubmit(objInsert);

                        dbConnect.SubmitChanges();
                    }
                    else
                    {
                        break;
                    }
                   

                  
                }
            }//end for


            // ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + Resources.Main.success + "'); window.location='" + Request.ApplicationPath + "/incidentform3.aspx?pagetype=view&id=" + id + "';", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + Resources.Main.success + "');window.location.href ='holiday.aspx' ", true);
        }
    }
}