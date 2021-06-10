using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace safetys4.App_Code
{
    public class FormatDates
    {

        public static string getDateNow(string lang,string timezone)
        {
            string datetime = "";
            if (lang == "th")
            {
                datetime = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("th-TH"));
            }
            else if (lang == "en")
            {
                datetime = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB"));


            }
            else if (lang == "si")
            {
                datetime = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB"));


            }


            return datetime;
        }



        public static string getDatetimeShow(DateTime dt,string lang)
        {
            string datetime = "";
            if (lang == "th")
            {
              // datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("th-TH")).ToString("dd/MM/yyyy HH:mm:ss");
                datetime = dt.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("th-TH"));
            }
            else if (lang == "en")
            {
                //datetime = Convert.ToDateTime(dt, lang).ToString("dd/MM/yyyy HH:mm:ss");
                datetime = dt.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB"));

            }
            else if (lang == "si")
            {
                //datetime = Convert.ToDateTime(dt, lang).ToString("dd/MM/yyyy HH:mm:ss");
                datetime = dt.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB"));

            }


            return datetime;
        }

        public static string getDatetimeShowShot(DateTime dt, string lang)
        {
            string datetime = "";
            if (lang == "th")
            {
                // datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("th-TH")).ToString("dd/MM/yyyy HH:mm:ss");
                datetime = dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("th-TH"));
            }
            else if (lang == "en")
            {
                //datetime = Convert.ToDateTime(dt, lang).ToString("dd/MM/yyyy HH:mm:ss");
                datetime = dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB"));

            }
            else if (lang == "si")
            {
                //datetime = Convert.ToDateTime(dt, lang).ToString("dd/MM/yyyy HH:mm:ss");
                datetime = dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.CreateSpecificCulture("en-GB"));

            }


            return datetime;
        }

        public static string getDateShow(string dt, string lang)
        {
            string datetime = "";
            if (lang == "th")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("th-TH")).ToString("dd/MM/yyyy");

            }
            else if (lang == "en")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB")).ToString("dd/MM/yyyy");


            }
            else if (lang == "si")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB")).ToString("dd/MM/yyyy");


            }


            return datetime;
        }




        public static string getDateShowFromDate(DateTime dt, string lang)
        {
            string datetime = "";
            if (lang == "th")
            {
                datetime = dt.ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("th-TH"));

            }
            else if (lang == "en")
            {
                datetime = dt.ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-GB"));


            }
            else if (lang == "si")
            {
                datetime = dt.ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-GB"));


            }


            return datetime;
        }


        public static string getTimeShowFromDate(DateTime dt, string lang)
        {
            string datetime = "";

            if (lang == "th")
            {
                datetime = dt.ToString("HH:mm", CultureInfo.CreateSpecificCulture("th-TH"));

            }
            else if (lang == "en")
            {
                datetime = dt.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB"));


            }
            else if (lang == "si")
            {
                datetime = dt.ToString("HH:mm", CultureInfo.CreateSpecificCulture("en-GB"));


            }
           


            return datetime;
        }

  
        public static int getYear(int year,string lang)
        {
            int cyear = year;

            if (lang == "th")
            {
                cyear = cyear + 543;

            }




            return cyear;
        }

        public static string getTimeShow(string dt, string lang)
        {
            string datetime = "";
            if (lang == "th")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("th-TH")).ToString("HH:mm");

            }
            else if (lang == "en")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB")).ToString("HH:mm");


            }
            else if (lang == "si")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB")).ToString("HH:mm");


            }


            return datetime;
        }


        public static string getDateTimeNoDash(string dt)
        {
            string datetime = "";
            CultureInfo en = new CultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = en;
            datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyyMMddHHmmss");
           
            return datetime;
        }

      

        public static string getDateTimeMicro(DateTime dt)
        {
            string datetime = "";

            datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyyMMddHHmmssffffff");
          
            return datetime;
        }


        public static DateTime changeDateTimeDB(string dt, string lang)
        {
            DateTime datetime = new DateTime();
            if (lang == "th")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("th-TH"));

            }
            else if (lang == "en")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB"));


            }
            else if (lang == "si")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB"));


            }


            return datetime;
        }


        public static string changeDateTimeUpload(string dt, string lang)
        {
            string datetime = "";
            if (lang == "th")
            {
                datetime = Convert.ToDateTime(dt, new CultureInfo("th-TH")).ToString(new CultureInfo("en-GB"));

            }
            else if (lang == "en")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB")).ToString("dd/MM/yyyy HH:mm:ss");


            }
            else if (lang == "si")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB")).ToString("dd/MM/yyyy HH:mm:ss");


            }


            return datetime;
        }


         public static string changeDateTimeReport(string dt, string lang)
        {
            string datetime = "";
            if (lang == "th")
            {
                datetime = Convert.ToDateTime(dt, new CultureInfo("th-TH")).ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-GB"));

            }
            else if (lang == "en")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy-MM-dd HH:mm:ss");


            }
            else if (lang == "si")
            {
                datetime = Convert.ToDateTime(dt, CultureInfo.CreateSpecificCulture("en-GB")).ToString("yyyy-MM-dd HH:mm:ss");


            }


            return datetime;
        }









    }



    
}