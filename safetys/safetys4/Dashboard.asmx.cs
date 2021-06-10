using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using safetys4.App_Code;
using System.Web.Script.Services;
using System.Collections;
using System.Web.Script.Serialization;
using System.Threading;

namespace safetys4
{
    /// <summary>
    /// Summary description for Dashboard
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Dashboard : System.Web.Services.WebService
    {
         

        [WebMethod(EnableSession =true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncident1(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
           
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string company_id = "";
            string function_id = "";
            string department_id = "";
            string division_id = "";
            string redirect = "";

            int REJECT = 3;
            int EXEMPTION = 4;

            if(area_id!="")
            {
                var companys = from c in dbConnect.companies
                               where c.company_id == area_id && c.country == Session["country"].ToString()
                               select c;

                if(companys.Count()>0)
                {
                    type = "company";
                    company_id = area_id;

                }

                var functions = from f in dbConnect.functions
                                where f.function_id == area_id && f.country == Session["country"].ToString()
                    select f;
               if(functions.Count()>0)
               {
                   type = "function";
                   function_id = area_id;
               }


               var departments = from f in dbConnect.departments
                                 where f.department_id == area_id && f.country == Session["country"].ToString()
                               select f;
               if (departments.Count() > 0)
               {
                   type = "department";
                   department_id = area_id;
               }


               var divisions = from f in dbConnect.divisions
                               where f.division_id == area_id && f.country == Session["country"].ToString()
                               select f;
               if (divisions.Count() > 0)
               {
                   type = "division";
                   division_id = area_id;
               }

            }
            else
            {
                type = "all";

            }

           

            ArrayList dataJson = new ArrayList();



            string label_all = "";
            if (type == "all")
            {

                var n = from c in dbConnect.incidents
                        where c.country == Session["country"].ToString()
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_all = n.Count();
                label_all = chageDataLanguage("INSEE Group", "INSEE Group", lang);


                var v = from c in dbConnect.incidents
                        where c.work_relate == "Y" && c.country == Session["country"].ToString() &&
                        (c.culpability == "G" || c.culpability == "P")
                        && c.process_status != REJECT
                        && c.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            c.incident_date

                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_condition = v.Count();

                var result = new
                {
                    label = label_all,
                    all = count_incident_all,
                    condition = count_incident_condition,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////


                DataSet ds = new DataSet();
                string sql = "select c.company_id,c.company_th,c.company_en,ISNULL(n.count_select,0) as count_select from (select i.company_id,ISNULL(count(i.company_id),0) as count_select from incident i ";
                sql = sql + " where 1=1 ";
                // sql = sql + "where i.country ='" + Session["country"].ToString()+"' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.company_id) as n ";
                sql = sql + "right join company c on c.company_id = n.company_id";
                sql = sql + " where c.country ='" + Session["country"].ToString() + "' ";
               
                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_company = chageDataLanguage(ds.Tables[0].Rows[i]["company_th"].ToString(), ds.Tables[0].Rows[i]["company_en"].ToString(), lang);
                    int count_company = Convert.ToInt32(ds.Tables[0].Rows[i]["count_select"]);

                    DataSet ds2 = new DataSet();
                    string sql2 = "select i.company_id from incident i " +
                                  "join [function] f on i.form2_function_id = f.function_id "+
                                  "where f.company_id = '" + ds.Tables[0].Rows[i]["company_id"] + "' and i.work_relate = 'Y' and (i.culpability = 'G' or i.culpability = 'P') and i.process_status != 3 and i.process_status != 4 ";


                    if (date_start != "")
                    {
                        string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                        sql2 = sql2 + " and i.incident_date >='" + d_start + "'";

                    }

                    if (date_end != "")
                    {
                        string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                        sql2 = sql2 + " and i.incident_date <='" + d_end + "'";
                    }


                    ds2 = DatabaseConnector.GetData(sql2);


                    var result2 = new
                    {
                        label = label_company,
                        all = count_company,
                        condition = ds2.Tables[0].Rows.Count,
                        area_id = ds.Tables[0].Rows[i]["company_id"]
                    };


                    dataJson.Add(result2);

                }

            }
            else if (type == "company")
            {
                var cu = from c in dbConnect.companies
                         where c.company_id == company_id
                         select c;
                foreach (var u in cu)
                {
                    label_all = chageDataLanguage(u.company_th, u.company_en, lang);
                }


                var n = from c in dbConnect.incidents
                        where c.company_id == company_id
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_all = n.Count();


                var v = from c in dbConnect.incidents
                        join f in dbConnect.functions on c.form2_function_id equals f.function_id
                        where f.company_id == company_id && c.work_relate == "Y" && 
                        (c.culpability == "G" || c.culpability == "P")
                        && c.process_status != REJECT
                        && c.process_status != EXEMPTION

                        select new
                        {
                            c.id,
                            c.incident_date

                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }

                int count_incident_condition = v.Count();
                var result3 = new
                {
                    label = label_all,
                    all = count_incident_all,
                    condition = count_incident_condition,
                    area_id = ""
                };


                dataJson.Add(result3);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////

                DataSet ds = new DataSet();
                string sql = "select f.function_id,f.function_th,f.function_en,ISNULL(n.count_select,0) as count_select from (select i.function_id,ISNULL(count(i.function_id),0) as count_select from incident i ";
                sql = sql + "where 1=1 and i.company_id='" + company_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + "and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.function_id) as n ";
                sql = sql + "right join [function] f on f.function_id = n.function_id where  f.function_en !='-' and f.company_id='" + company_id + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_function = chageDataLanguage(ds.Tables[0].Rows[i]["function_th"].ToString(), ds.Tables[0].Rows[i]["function_en"].ToString(), lang);
                    int count_function = Convert.ToInt32(ds.Tables[0].Rows[i]["count_select"]);

                    DataSet ds2 = new DataSet();
                    string sql2 = "select i.function_id from incident i " +
                                  "where i.form2_function_id = '" + ds.Tables[0].Rows[i]["function_id"] + "' and i.work_relate = 'Y' and (i.culpability = 'G' or i.culpability = 'P') and i.process_status != 3  and i.process_status != 4 ";


                    if (date_start != "")
                    {
                        string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                        sql2 = sql2 + "and i.incident_date >='" + d_start + "'";

                    }

                    if (date_end != "")
                    {
                        string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                        sql2 = sql2 + " and i.incident_date <='" + d_end + "'";
                    }


                    ds2 = DatabaseConnector.GetData(sql2);


                    var result2 = new
                    {
                        label = label_function,
                        all = count_function,
                        condition = ds2.Tables[0].Rows.Count,
                        area_id = ds.Tables[0].Rows[i]["function_id"]
                    };


                    dataJson.Add(result2);

                }



            }
            else if (type == "function")
            {

                var fu = from f in dbConnect.functions
                         where f.function_id == function_id
                         select f;
                foreach (var u in fu)
                {
                    label_all = chageDataLanguage(u.function_th, u.function_en, lang);
                }


                var n = from c in dbConnect.incidents
                        where c.function_id == function_id
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_all = n.Count();


                var v = from c in dbConnect.incidents
                        where c.form2_function_id == function_id && c.work_relate == "Y" && 
                        (c.culpability == "G" || c.culpability == "P")
                        && c.process_status != REJECT
                        && c.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            c.incident_date

                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }

                int count_incident_condition = v.Count();
                var result3 = new
                {
                    label = label_all,
                    all = count_incident_all,
                    condition = count_incident_condition,
                    area_id = ""
                };


                dataJson.Add(result3);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select n.department_id,d.department_th,d.department_en,n.count_select from (select i.department_id,ISNULL(count(i.department_id),0) as count_select from incident i ";
                sql = sql + "where 1=1 and i.function_id='" + function_id + "' ";


                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + "group by i.department_id) as n ";
                sql = sql + "join department d on d.department_id = n.department_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_department = chageDataLanguage(ds.Tables[0].Rows[i]["department_th"].ToString(), ds.Tables[0].Rows[i]["department_en"].ToString(), lang);
                    int count_department = Convert.ToInt32(ds.Tables[0].Rows[i]["count_select"]);

                    DataSet ds2 = new DataSet();
                    string sql2 = "select i.department_id from incident i " +
                                  "where i.form3_department_id = '" + ds.Tables[0].Rows[i]["department_id"] + "' and i.work_relate = 'Y' and (i.culpability = 'G' or i.culpability = 'P') and i.process_status != 3 and i.process_status != 4 ";


                    if (date_start != "")
                    {
                        string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                        sql2 = sql2 + "and i.incident_date >='" + d_start + "'";

                    }

                    if (date_end != "")
                    {
                        string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                        sql2 = sql2 + " and i.incident_date <='" + d_end + "'";
                    }

                    ds2 = DatabaseConnector.GetData(sql2);


                    var result4 = new
                    {
                        label = label_department,
                        all = count_department,
                        condition = ds2.Tables[0].Rows.Count,
                        area_id = ds.Tables[0].Rows[i]["department_id"]
                    };


                    dataJson.Add(result4);
                }


            }
            else if (type == "department")
            {

                var de = from d in dbConnect.departments
                         where d.department_id == department_id
                         select d;
                foreach (var u in de)
                {
                    label_all = chageDataLanguage(u.department_th, u.department_en, lang);
                }




                var n = from c in dbConnect.incidents
                        where c.department_id == department_id
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_all = n.Count();


                var v = from c in dbConnect.incidents
                        where c.form3_department_id == department_id &&
                        c.work_relate == "Y" && (c.culpability == "G" || c.culpability == "P")
                        && c.process_status != REJECT
                        && c.process_status != EXEMPTION

                        select new
                        {
                            c.id,
                            c.incident_date

                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_condition = v.Count();
                var result5 = new
                {
                    label = label_all,
                    all = count_incident_all,
                    condition = count_incident_condition,
                    area_id = ""
                };


                dataJson.Add(result5);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select n.division_id,d.division_th,d.division_en,n.count_select from (select i.division_id,ISNULL(count(i.department_id),0) as count_select from incident i ";
                sql = sql + " where 1=1 and i.department_id='" + department_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + "group by i.division_id ) as n ";
                sql = sql + "join division d on d.division_id = n.division_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_division = chageDataLanguage(ds.Tables[0].Rows[i]["division_th"].ToString(), ds.Tables[0].Rows[i]["division_en"].ToString(), lang);
                    int count_division = Convert.ToInt32(ds.Tables[0].Rows[i]["count_select"]);

                    //DataSet ds2 = new DataSet();
                    //string sql2 = "select i.division_id from incident i " +
                    //              "where i.division_id = '" + ds.Tables[0].Rows[i]["division_id"] + "' and i.work_relate = 'Y' and (i.culpability = 'G' or i.culpability = 'P') and i.process_status != 3 ";


                    //if (date_start != "")
                    //{
                    //    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    //    sql2 = sql2 + "and i.incident_date >='" + d_start + "'";

                    //}

                    //if (date_end != "")
                    //{
                    //    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    //    sql2 = sql2 + " and i.incident_date <='" + d_end + "'";
                    //}

                    //ds2 = DatabaseConnector.GetData(sql2);



                    var result6 = new
                    {
                        label = label_division,
                        all = count_division,
                        condition = 0,// ds2.Tables[0].Rows.Count,
                        area_id = ds.Tables[0].Rows[i]["division_id"]
                    };


                    dataJson.Add(result6);

                }

            }
            else if (type == "division")
            {
                string department_id2 = "";
                string function_id2 = "";
                string company_id2 = "";

                var divisions2 = from f in dbConnect.divisions
                                 where f.division_id == division_id
                                 select f;

                foreach (var di in divisions2)
                {
                    department_id2 = di.department_id;

                    var departments2 = from f in dbConnect.departments
                                       where f.department_id == department_id2
                                       select f;

                    foreach (var de in departments2)
                    {
                        function_id2 = de.function_id;


                        var functions2 = from f in dbConnect.functions
                                         where f.function_id == function_id2
                                         select f;

                        foreach (var fun in functions2)
                        {
                            company_id2 = fun.company_id;

                        }

                    }
                }

                redirect = "allincident.aspx?company_id=" + company_id2 + "&function_id=" + function_id2 + "&department_id=" + department_id2 + "&division_id=" + division_id;





            }

            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }









        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncident2(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();


            string type = "";
            string company_id = "";
            string function_id = "";
            string department_id = "";
            string division_id = "";
            string redirect = "";
            int REJECT = 3;
            int EXEMPTION = 4;

            if (area_id != "")
            {
                var companys = from c in dbConnect.companies
                               where c.company_id == area_id && c.country == Session["country"].ToString()
                               select c;

                if (companys.Count() > 0)
                {
                    type = "company";
                    company_id = area_id;

                }


                var functions = from f in dbConnect.functions
                                where f.function_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (functions.Count() > 0)
                {
                    type = "function";
                    function_id = area_id;
                }


                var departments = from f in dbConnect.departments
                                  where f.department_id == area_id && f.country == Session["country"].ToString()
                                  select f;
                if (departments.Count() > 0)
                {
                    type = "department";
                    department_id = area_id;
                }


                var divisions = from f in dbConnect.divisions
                                where f.division_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (divisions.Count() > 0)
                {
                    type = "division";
                    division_id = area_id;
                }

            }
            else
            {
                type = "all";

            }





            ArrayList dataJson = new ArrayList();

            int F = 1;
            int PD = 2;
            int LTI = 3;
            int MTI = 4;
            int MI = 5;
            int RWC = 6;

            if (Session["country"].ToString() == "thailand")
            {
                F = 1;
                PD = 2;
                LTI = 3;
                MTI = 4;
                MI = 5;
                RWC = 6;
            }
            else if (Session["country"].ToString() == "srilanka")
            {
                F = 7;
                PD = 8;
                LTI = 9;
                MTI = 10;
                MI = 11;
                RWC = 12;
            }
               
            string label_all = "";
            if (type == "all")
            {

                var n = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == F
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && i.country == Session["country"].ToString()
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_fatality_all = n.Count();
                label_all = chageDataLanguage("INSEE Group", "INSEE Group", lang);

                ////////////////////////////////////////end bar1///////////////////////////////////////////////////////

                var v = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == PD
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && i.country == Session["country"].ToString()
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_pd_all = v.Count();
                ////////////////////////////////////////////////////////end bar2/////////////////////////////////////////////

                var l = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == LTI
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && i.country == Session["country"].ToString()
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    l = l.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    l = l.Where(c => c.incident_date <= d_end);
                }


                int count_lti_all = l.Count();

                //////////////////////////////////////////////////////end bar3///////////////////////////////////////////////

                var m = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == MTI
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && i.country == Session["country"].ToString()
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    m = m.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    m = m.Where(c => c.incident_date <= d_end);
                }


                int count_mti_all = m.Count();

                //////////////////////////////////////////////////////end bar4///////////////////////////////////////////////

                var mi = from c in dbConnect.injury_persons
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && c.severity_injury_id == MI
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                         && i.country == Session["country"].ToString()
                         && c.status == "A"
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    mi = mi.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    mi = mi.Where(c => c.incident_date <= d_end);
                }


                int count_mi_all = mi.Count();

                //////////////////////////////////////////////////////end bar5///////////////////////////////////////////////

                var da = from c in dbConnect.damage_lists
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                         && i.country == Session["country"].ToString()
                         && c.status == "A"
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    da = da.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    da = da.Where(c => c.incident_date <= d_end);
                }


                int count_damage_list_all = da.Count();

                //////////////////////////////////////////////////////end bar6///////////////////////////////////////////////


                var rw = from c in dbConnect.injury_persons
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && c.severity_injury_id == RWC
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                         && i.country == Session["country"].ToString()
                         && c.status == "A"
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                   rw = rw.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                   rw = rw.Where(c => c.incident_date <= d_end);
                }


                int count_rw_all = rw.Count();

                //////////////////////////////////////////////////////end bar7///////////////////////////////////////////////


                var nm = from i in dbConnect.incidents 
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && i.impact == "N"
                         && i.country == Session["country"].ToString()
                         select new
                         {
                             i.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    nm = nm.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    nm = nm.Where(c => c.incident_date <= d_end);
                }


                int count_nm_all = nm.Count();


                ///////////////////////////////////////end bar8//////////////////////////////////////////


                var result = new
                {
                    label = label_all,
                    fatality = count_fatality_all,
                    pd = count_pd_all,
                    lti = count_lti_all,
                    mti = count_mti_all,
                    mi = count_mi_all,
                    damage = count_damage_list_all,
                    rwc = count_rw_all,
                    nm = count_nm_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select c.company_id,c.company_th,c.company_en,ISNULL(n.count_select,0) as count_select from (select i.company_id,ISNULL(count(i.company_id),0) as count_select from incident i ";
                sql = sql + " where 1=1 ";
                // sql = sql + "where i.country ='" + Session["country"].ToString() + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.company_id) as n ";
                sql = sql + "right join company c on c.company_id = n.company_id";
                sql = sql + " where c.country ='" + Session["country"].ToString() + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_company = chageDataLanguage(ds.Tables[0].Rows[i]["company_th"].ToString(), ds.Tables[0].Rows[i]["company_en"].ToString(), lang);


                    var f = from c in dbConnect.injury_persons
                            join fu in dbConnect.functions on c.function_id equals fu.function_id
                            join nc in dbConnect.incidents on c.incident_id equals nc.id
                            where nc.work_relate == "Y" && (nc.culpability == "G" || nc.culpability == "P")
                            && c.severity_injury_id == F
                            && fu.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                            && nc.process_status != REJECT
                            && nc.process_status != EXEMPTION
                            && c.status == "A"
                            select new
                            {
                                c.id,
                                nc.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        f = f.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        f = f.Where(c => c.incident_date<= d_end);
                    }
                    int count_fatality_fun = f.Count();

                    /////////////////////////////////////////end bar1////////////////////////////////////////////////////


                    var p = from c in dbConnect.injury_persons
                            join fu in dbConnect.functions on c.function_id equals fu.function_id
                            join t in dbConnect.incidents on c.incident_id equals t.id
                            where t.work_relate == "Y" && (t.culpability == "G" || t.culpability == "P")
                            && c.severity_injury_id == PD
                            && fu.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                            && t.process_status != REJECT
                            && t.process_status != EXEMPTION
                            && c.status == "A"
                            select new
                            {
                                c.id,
                                t.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        p = p.Where(c => c.incident_date>= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        p = p.Where(c => c.incident_date <= d_end);
                    }


                    int count_pd_fun = p.Count();


                    /////////////////////////////////////////////bar2/////////////////////////////////////////////////

                    var li = from c in dbConnect.injury_persons
                             join fu in dbConnect.functions on c.function_id equals fu.function_id
                             join g in dbConnect.incidents on c.incident_id equals g.id
                             where g.work_relate == "Y" && (g.culpability == "G" || g.culpability == "P")
                             && c.severity_injury_id == LTI
                             && fu.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && g.process_status != REJECT
                             && g.process_status != EXEMPTION
                             && c.status == "A"
                             select new
                             {
                                 c.id,
                                 g.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        li = li.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        li = li.Where(c => c.incident_date <= d_end);
                    }


                    int count_lti_fun = li.Count();

                    /////////////////////////////////////////////bar3/////////////////////////////////////////////////

                    var j = from c in dbConnect.injury_persons
                            join fu in dbConnect.functions on c.function_id equals fu.function_id
                            join ii in dbConnect.incidents on c.incident_id equals ii.id
                            where ii.work_relate == "Y" && (ii.culpability == "G" || ii.culpability == "P")
                            && c.severity_injury_id == MTI
                            && fu.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                            && ii.process_status != REJECT
                            && ii.process_status != EXEMPTION
                            && c.status == "A"
                           // && ii.incident_date >= Convert.ToDateTime("2017-01-01 00:00") && ii.incident_date <= Convert.ToDateTime("2017-12-31 23:59")
                            select new
                            {
                                c.id,
                                ii.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        j = j.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        j = j.Where(c => c.incident_date <= d_end);
                    }

               


                    int count_mti_fun = j.Count();

                    /////////////////////////////////////////////bar4/////////////////////////////////////////////////

                    var m2 = from c in dbConnect.injury_persons
                             join fu in dbConnect.functions on c.function_id equals fu.function_id
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.severity_injury_id == MI
                             && fu.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && i2.process_status != REJECT
                             && i2.process_status != EXEMPTION
                             && c.status == "A"
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        m2 = m2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        m2 = m2.Where(c => c.incident_date <= d_end);
                    }


                    int count_mi_fun = m2.Count();

                    //////////////////////////////////////////////////////end bar5///////////////////////////////////////////////

                    var d2 = from c in dbConnect.damage_lists
                             join i3 in dbConnect.incidents on c.incident_id equals i3.id
                             join fu in dbConnect.functions on i3.form2_function_id equals fu.function_id
                             where i3.work_relate == "Y" && (i3.culpability == "G" || i3.culpability == "P")
                             && fu.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && i3.process_status != REJECT
                             && i3.process_status != EXEMPTION
                              && c.status == "A"
                             select new
                             {
                                 c.id,
                                 i3.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        d2 = d2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        d2 = d2.Where(c => c.incident_date <= d_end);
                    }


                    int count_damage_list_fun = d2.Count();

                    //////////////////////////////////////////////////////end bar6///////////////////////////////////////////////

                    var rw2 = from c in dbConnect.injury_persons
                             join fu in dbConnect.functions on c.function_id equals fu.function_id
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.severity_injury_id == RWC
                             && fu.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && i2.process_status != REJECT
                             && i2.process_status != EXEMPTION
                             && c.status == "A"
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        rw2 = rw2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        rw2 = rw2.Where(c => c.incident_date <= d_end);
                    }


                    int count_rw_fun = rw2.Count();

                    //////////////////////////////////////////////////////end bar7///////////////////////////////////////////////


                    var nm2 = from c in dbConnect.incidents
                             join fu in dbConnect.functions on c.form2_function_id equals fu.function_id
                             where c.work_relate == "Y" && (c.culpability == "G" || c.culpability == "P")
                             && c.impact == "N"
                             && c.country == Session["country"].ToString()
                             && fu.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        nm2 = nm2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        nm2 = nm2.Where(c => c.incident_date <= d_end);
                    }


                    int count_nm_fun = nm2.Count();

                    ///////////////////////////////////////////////////////end bar8////////////////////////////////////////////////


                    var result2 = new
                    {
                        label = label_company,
                        fatality = count_fatality_fun,
                        pd = count_pd_fun,
                        lti = count_lti_fun,
                        mti = count_mti_fun,
                        mi = count_mi_fun,
                        damage = count_damage_list_fun,
                        rwc = count_rw_fun,
                        nm = count_nm_fun,
                        area_id = ds.Tables[0].Rows[i]["company_id"]
                    };

                    dataJson.Add(result2);

                }



            }
            else if (type == "company")
            {

                string label_company = "";
                var cu = from c in dbConnect.companies
                         where c.company_id == company_id
                         select c;

                foreach (var c in cu)
                {
                    label_company = chageDataLanguage(c.company_th, c.company_en, lang);
                }

                var n = from c in dbConnect.injury_persons
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == F
                        && fu.company_id == company_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_fatality_company = n.Count();


                ////////////////////////////////////////end bar1///////////////////////////////////////////////////////

                var v = from c in dbConnect.injury_persons
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == PD
                        && fu.company_id == company_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_pd_company = v.Count();
                ////////////////////////////////////////////////////////end bar2/////////////////////////////////////////////

                var l = from c in dbConnect.injury_persons
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == LTI
                        && fu.company_id == company_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    l = l.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    l = l.Where(c => c.incident_date <= d_end);
                }


                int count_lti_company = l.Count();

                //////////////////////////////////////////////////////end bar3///////////////////////////////////////////////

                var m = from c in dbConnect.injury_persons
                        join fu in dbConnect.functions on c.function_id equals fu.function_id
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == MTI
                        && fu.company_id == company_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    m = m.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    m = m.Where(c => c.incident_date <= d_end);
                }


                int count_mti_company = m.Count();

                //////////////////////////////////////////////////////end bar4///////////////////////////////////////////////

                var mi = from c in dbConnect.injury_persons
                         join fu in dbConnect.functions on c.function_id equals fu.function_id
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && c.severity_injury_id == MI
                         && fu.company_id == company_id
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                         && c.status == "A"
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    mi = mi.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    mi = mi.Where(c => c.incident_date <= d_end);
                }


                int count_mi_company = mi.Count();

                //////////////////////////////////////////////////////end bar5///////////////////////////////////////////////

                var da = from c in dbConnect.damage_lists
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         join fu in dbConnect.functions on i.form2_function_id equals fu.function_id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && fu.company_id == company_id
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                          && c.status == "A"
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    da = da.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    da = da.Where(c => c.incident_date <= d_end);
                }


                int count_damage_list_company = da.Count();

                //////////////////////////////////////////////////////end bar6///////////////////////////////////////////////


                var rw = from c in dbConnect.injury_persons
                         join fu in dbConnect.functions on c.function_id equals fu.function_id
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && c.severity_injury_id == RWC
                         && fu.company_id == company_id
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                         && c.status == "A"
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    rw = rw.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    rw = rw.Where(c => c.incident_date <= d_end);
                }


                int count_rw_company = mi.Count();
                ////////////////////////////////////////////////end bar 7////////////////////////////////////////////

                var nm = from c in dbConnect.incidents
                          join fu in dbConnect.functions on c.form2_function_id equals fu.function_id
                          where c.work_relate == "Y" && (c.culpability == "G" || c.culpability == "P")
                          && c.impact == "N"
                          && fu.company_id == company_id
                          select new
                          {
                              c.id,
                              c.incident_date
                          };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    nm = nm.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    nm = nm.Where(c => c.incident_date <= d_end);
                }


                int count_nm_company = nm.Count();

                ///////////////////////////////////////////////////////end bar8////////////////////////////////////////////////


                var result = new
                {
                    label = label_company,
                    fatality = count_fatality_company,
                    pd = count_pd_company,
                    lti = count_lti_company,
                    mti = count_mti_company,
                    mi = count_mi_company,
                    damage = count_damage_list_company,
                    rwc = count_rw_company,
                    nm = count_nm_company,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select f.function_id,f.function_th,f.function_en,ISNULL(n.count_select,0) as count_select from (select i.function_id,ISNULL(count(i.function_id),0) as count_select from incident i ";
                sql = sql + "where 1=1 and i.company_id='" + company_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.function_id) as n ";
                sql = sql + "right join [function] f on f.function_id = n.function_id where f.function_en !='-' and f.company_id='" + company_id + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_function = chageDataLanguage(ds.Tables[0].Rows[i]["function_th"].ToString(), ds.Tables[0].Rows[i]["function_en"].ToString(), lang);


                    var f = from c in dbConnect.injury_persons
                            join nc in dbConnect.incidents on c.incident_id equals nc.id
                            where nc.work_relate == "Y" && (nc.culpability == "G" || nc.culpability == "P")
                            && c.severity_injury_id == F
                            && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                            && nc.process_status != REJECT
                            && nc.process_status != EXEMPTION
                            && c.status == "A"
                            select new
                            {
                                c.id,
                                nc.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        f = f.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        f = f.Where(c => c.incident_date <= d_end);
                    }
                    int count_fatality_fun = f.Count();

                    /////////////////////////////////////////end bar1////////////////////////////////////////////////////


                    var p = from c in dbConnect.injury_persons
                            join t in dbConnect.incidents on c.incident_id equals t.id
                            where t.work_relate == "Y" && (t.culpability == "G" || t.culpability == "P")
                            && c.severity_injury_id == PD
                            && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                            && t.process_status != REJECT
                            && t.process_status != EXEMPTION
                            && c.status == "A"
                            select new
                            {
                                c.id,
                                t.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        p = v.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        p = v.Where(c => c.incident_date <= d_end);
                    }


                    int count_pd_fun = p.Count();


                    /////////////////////////////////////////////bar2/////////////////////////////////////////////////

                    var li = from c in dbConnect.injury_persons
                             join g in dbConnect.incidents on c.incident_id equals g.id
                             where g.work_relate == "Y" && (g.culpability == "G" || g.culpability == "P")
                             && c.severity_injury_id == LTI
                             && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && g.process_status != REJECT
                             && g.process_status != EXEMPTION
                             && c.status == "A"
                             select new
                             {
                                 c.id,
                                 g.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        li = li.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        li = li.Where(c => c.incident_date <= d_end);
                    }


                    int count_lti_fun = li.Count();

                    /////////////////////////////////////////////bar3/////////////////////////////////////////////////

                    var j = from c in dbConnect.injury_persons
                            join ii in dbConnect.incidents on c.incident_id equals ii.id
                            where ii.work_relate == "Y" && (ii.culpability == "G" || ii.culpability == "P")
                            && c.severity_injury_id == MTI
                            && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                            && ii.process_status != REJECT
                            && ii.process_status != EXEMPTION
                            && c.status == "A"
                            select new
                            {
                                c.id,
                                ii.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        j = j.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        j = j.Where(c => c.incident_date <= d_end);
                    }


                    int count_mti_fun = j.Count();

                    /////////////////////////////////////////////bar4/////////////////////////////////////////////////

                    var m2 = from c in dbConnect.injury_persons
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.severity_injury_id == MI
                             && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && i2.process_status != REJECT
                             && i2.process_status != EXEMPTION
                             && c.status == "A"
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        m2 = m2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        m2 = m2.Where(c => c.incident_date <= d_end);
                    }


                    int count_mi_fun = m2.Count();

                    //////////////////////////////////////////////////////end bar5///////////////////////////////////////////////

                    var d2 = from c in dbConnect.damage_lists
                             join i3 in dbConnect.incidents on c.incident_id equals i3.id
                             join fu in dbConnect.functions on i3.form2_function_id equals fu.function_id
                             where i3.work_relate == "Y" && (i3.culpability == "G" || i3.culpability == "P")
                             && fu.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && i3.process_status != REJECT
                             && i3.process_status != EXEMPTION
                             && c.status == "A"
                             select new
                             {
                                 c.id,
                                 i3.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        d2 = d2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        d2 = d2.Where(c => c.incident_date <= d_end);
                    }


                    int count_damage_list_fun = d2.Count();

                    //////////////////////////////////////////////////////end bar6///////////////////////////////////////////////

                    var rw2 = from c in dbConnect.injury_persons
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.severity_injury_id == RWC
                             && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && i2.process_status != REJECT
                             && i2.process_status != EXEMPTION
                             && c.status == "A"
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        rw2 = rw2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        rw2 = rw2.Where(c => c.incident_date <= d_end);
                    }


                    int count_rw_fun = rw2.Count();

                    //////////////////////////////////////////////////////end bar7///////////////////////////////////////////////


                    var nm2 = from c in dbConnect.incidents
                              join fu in dbConnect.functions on c.form2_function_id equals fu.function_id
                             where c.work_relate == "Y" && (c.culpability == "G" || c.culpability == "P")
                             && c.impact == "N"
                             && fu.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        nm2 = nm2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        nm2 = nm2.Where(c => c.incident_date <= d_end);
                    }


                    int count_nm_fun = nm2.Count();

                    ///////////////////////////////////////////////////////end bar8////////////////////////////////////////////////



                    var result2 = new
                    {
                        label = label_function,
                        fatality = count_fatality_fun,
                        pd = count_pd_fun,
                        lti = count_lti_fun,
                        mti = count_mti_fun,
                        mi = count_mi_fun,
                        damage = count_damage_list_fun,
                        rwc = count_rw_fun,
                        nm = count_nm_fun,
                        area_id = ds.Tables[0].Rows[i]["function_id"]
                    };

                    dataJson.Add(result2);

                }




            }
            else if (type == "function")
            {

                string label_function = "";
                var fu = from c in dbConnect.functions
                         where c.function_id == function_id
                         select c;

                foreach (var f in fu)
                {
                    label_function = chageDataLanguage(f.function_th, f.function_en, lang);
                }

                var n = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == F
                        && c.function_id == function_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_fatality_function = n.Count();


                ////////////////////////////////////////end bar1///////////////////////////////////////////////////////

                var v = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == PD
                        && c.function_id == function_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_pd_function = v.Count();
                ////////////////////////////////////////////////////////end bar2/////////////////////////////////////////////

                var l = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == LTI
                        && c.function_id == function_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    l = l.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    l = l.Where(c => c.incident_date <= d_end);
                }


                int count_lti_function = l.Count();

                //////////////////////////////////////////////////////end bar3///////////////////////////////////////////////

                var m = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == MTI
                        && c.function_id == function_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        && c.status == "A"
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    m = m.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    m = m.Where(c => c.incident_date <= d_end);
                }


                int count_mti_function = m.Count();

                //////////////////////////////////////////////////////end bar4///////////////////////////////////////////////

                var mi = from c in dbConnect.injury_persons
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && c.severity_injury_id == MI
                         && c.function_id == function_id
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                         && c.status == "A"
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    mi = mi.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    mi = mi.Where(c => c.incident_date <= d_end);
                }


                int count_mi_function = mi.Count();

                //////////////////////////////////////////////////////end bar5///////////////////////////////////////////////

                var da = from c in dbConnect.damage_lists
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         join f in dbConnect.functions on i.form2_function_id equals f.function_id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && f.function_id == function_id
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                          && c.status == "A"
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    da = da.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    da = da.Where(c => c.incident_date <= d_end);
                }


                int count_damage_list_function = da.Count();

                //////////////////////////////////////////////////////end bar6///////////////////////////////////////////////
                var rw = from c in dbConnect.injury_persons
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && c.severity_injury_id == RWC
                         && c.function_id == function_id
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                         && c.status == "A"
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    rw = rw.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    rw = rw.Where(c => c.incident_date <= d_end);
                }


                int count_rw_function = rw.Count();

                //////////////////////////////////////////////////////end bar7///////////////////////////////////////////////


                var nm2 = from c in dbConnect.incidents
                          join f in dbConnect.functions on c.form2_function_id equals f.function_id
                          where c.work_relate == "Y" && (c.culpability == "G" || c.culpability == "P")
                          && c.impact == "N"
                          && f.function_id == function_id
                          select new
                          {
                              c.id,
                              c.incident_date
                          };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    nm2 = nm2.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    nm2 = nm2.Where(c => c.incident_date <= d_end);
                }


                int count_nm_function = nm2.Count();

                ///////////////////////////////////////////////////////end bar8////////////////////////////////////////////////


                var result = new
                {
                    label = label_function,
                    fatality = count_fatality_function,
                    pd = count_pd_function,
                    lti = count_lti_function,
                    mti = count_mti_function,
                    mi = count_mi_function,
                    damage = count_damage_list_function,
                    rwc = count_rw_function,
                    nm = count_nm_function,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select n.department_id,d.department_th,d.department_en,n.count_select from (select i.department_id,ISNULL(count(i.department_id),0) as count_select from incident i ";
                sql = sql + "where 1=1 and i.country ='" + Session["country"].ToString() + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.department_id) as n ";
                sql = sql + "join department d on d.department_id = n.department_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_department = chageDataLanguage(ds.Tables[0].Rows[i]["department_th"].ToString(), ds.Tables[0].Rows[i]["department_en"].ToString(), lang);


                    var f = from c in dbConnect.injury_persons
                            join nc in dbConnect.incidents on c.incident_id equals nc.id
                            where nc.work_relate == "Y" && (nc.culpability == "G" || nc.culpability == "P")
                            && c.severity_injury_id == F
                            && nc.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                            && nc.process_status != REJECT
                            && nc.process_status != EXEMPTION
                            && c.status == "A"
                            select new
                            {
                                c.id,
                                nc.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        f = f.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        f = f.Where(c => c.incident_date <= d_end);
                    }
                    int count_fatality_department = f.Count();

                    /////////////////////////////////////////end bar1////////////////////////////////////////////////////


                    var p = from c in dbConnect.injury_persons
                            join t in dbConnect.incidents on c.incident_id equals t.id
                            where t.work_relate == "Y" && (t.culpability == "G" || t.culpability == "P")
                            && c.severity_injury_id == PD
                            && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                            && t.process_status != REJECT
                            && t.process_status != EXEMPTION
                            && c.status == "A"
                            select new
                            {
                                c.id,
                                t.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        p = v.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        p = v.Where(c => c.incident_date <= d_end);
                    }


                    int count_pd_department = p.Count();


                    /////////////////////////////////////////////bar2/////////////////////////////////////////////////

                    var li = from c in dbConnect.injury_persons
                             join g in dbConnect.incidents on c.incident_id equals g.id
                             where g.work_relate == "Y" && (g.culpability == "G" || g.culpability == "P")
                             && c.severity_injury_id == LTI
                             && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && g.process_status != REJECT
                             && g.process_status != EXEMPTION
                             && c.status == "A"
                             select new
                             {
                                 c.id,
                                 g.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        li = li.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        li = li.Where(c => c.incident_date <= d_end);
                    }


                    int count_lti_department = li.Count();

                    /////////////////////////////////////////////bar3/////////////////////////////////////////////////

                    var j = from c in dbConnect.injury_persons
                            join ii in dbConnect.incidents on c.incident_id equals ii.id
                            where ii.work_relate == "Y" && (ii.culpability == "G" || ii.culpability == "P")
                            && c.severity_injury_id == MTI
                            && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                            && ii.process_status != REJECT
                            && ii.process_status != EXEMPTION
                            && c.status == "A"
                            select new
                            {
                                c.id,
                                ii.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        j = j.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        j = j.Where(c => c.incident_date <= d_end);
                    }


                    int count_mti_department = j.Count();

                    /////////////////////////////////////////////bar4/////////////////////////////////////////////////

                    var m2 = from c in dbConnect.injury_persons
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.severity_injury_id == MI
                             && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && i2.process_status != REJECT
                             && i2.process_status != EXEMPTION
                             && c.status == "A"
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        m2 = m2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        m2 = m2.Where(c => c.incident_date <= d_end);
                    }


                    int count_mi_department = m2.Count();

                    //////////////////////////////////////////////////////end bar5///////////////////////////////////////////////

                    var d2 = from c in dbConnect.damage_lists
                             join i3 in dbConnect.incidents on c.incident_id equals i3.id
                             join d in dbConnect.departments on i3.form3_department_id equals d.department_id
                             where i3.work_relate == "Y" && (i3.culpability == "G" || i3.culpability == "P")
                             && d.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && i3.process_status != REJECT
                             && i3.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i3.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        d2 = d2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        d2 = d2.Where(c => c.incident_date <= d_end);
                    }


                    int count_damage_list_department = d2.Count();

                    //////////////////////////////////////////////////////end bar6///////////////////////////////////////////////



                    var rw2 = from c in dbConnect.injury_persons
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.severity_injury_id == RWC
                             && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && i2.process_status != REJECT
                             && i2.process_status != EXEMPTION
                             && c.status == "A"
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        rw2 = rw2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        rw2 = rw2.Where(c => c.incident_date <= d_end);
                    }


                    int count_rw_department = rw2.Count();

                    //////////////////////////////////////////////////////end bar7///////////////////////////////////////////////


                    var nm = from c in dbConnect.incidents
                             join d in dbConnect.departments on c.form3_department_id equals d.department_id
                             where c.work_relate == "Y" && (c.culpability == "G" || c.culpability == "P")
                             && c.impact == "N"
                             && d.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        nm = nm.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        nm = nm.Where(c => c.incident_date <= d_end);
                    }


                    int count_nm_department = nm.Count();

                    ///////////////////////////////////////////////////////end bar8////////////////////////////////////////////////


                    var result3 = new
                    {
                        label = label_department,
                        fatality = count_fatality_department,
                        pd = count_pd_department,
                        lti = count_lti_department,
                        mti = count_mti_department,
                        mi = count_mi_department,
                        damage = count_damage_list_department,
                        rwc = count_rw_department,
                        nm = count_nm_department,
                        area_id = ds.Tables[0].Rows[i]["department_id"].ToString()
                    };

                    dataJson.Add(result3);

                }




            }
            else if (type == "department")
            {
               // redirect = "none";

                string label_department = "";
                var de = from d in dbConnect.departments
                         where d.department_id == department_id
                         select d;
                foreach (var u in de)
                {
                    label_department = chageDataLanguage(u.department_th, u.department_en, lang);
                }



                var n = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == F
                        && c.department_id == department_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_fatality_department = n.Count();


                ////////////////////////////////////////end bar1///////////////////////////////////////////////////////

                var v = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == PD
                        && c.department_id == department_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_pd_department = v.Count();
                ////////////////////////////////////////////////////////end bar2/////////////////////////////////////////////

                var l = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == LTI
                        && c.department_id == department_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    l = l.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    l = l.Where(c => c.incident_date <= d_end);
                }


                int count_lti_department = l.Count();

                //////////////////////////////////////////////////////end bar3///////////////////////////////////////////////

                var m = from c in dbConnect.injury_persons
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.severity_injury_id == MTI
                        && c.department_id == department_id
                        && i.process_status != REJECT
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    m = m.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    m = m.Where(c => c.incident_date <= d_end);
                }


                int count_mti_department = m.Count();

                //////////////////////////////////////////////////////end bar4///////////////////////////////////////////////

                var mi = from c in dbConnect.injury_persons
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && c.severity_injury_id == MI
                         && c.department_id == department_id
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    mi = mi.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    mi = mi.Where(c => c.incident_date <= d_end);
                }


                int count_mi_department = mi.Count();

                //////////////////////////////////////////////////////end bar5///////////////////////////////////////////////

                var da = from c in dbConnect.damage_lists
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         join d in dbConnect.departments on i.form3_department_id equals d.department_id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && d.department_id == department_id
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    da = da.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    da = da.Where(c => c.incident_date <= d_end);
                }


                int count_damage_list_department = da.Count();

                //////////////////////////////////////////////////////end bar6///////////////////////////////////////////////




                var rw = from c in dbConnect.injury_persons
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && c.severity_injury_id == RWC
                         && c.department_id == department_id
                         && i.process_status != REJECT
                         && i.process_status != EXEMPTION
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    rw = rw.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    rw = rw.Where(c => c.incident_date <= d_end);
                }


                int count_rw_department = rw.Count();
                /////////////////////////////////////////////////end bar7//////////////////////////////////////

                var nm = from c in dbConnect.incidents
                         join d in dbConnect.departments on c.form3_department_id equals d.department_id
                         where c.work_relate == "Y" && (c.culpability == "G" || c.culpability == "P")
                         && c.impact == "N"
                         && d.department_id == department_id
                         select new
                         {
                             c.id,
                             c.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    nm = nm.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    nm = nm.Where(c => c.incident_date <= d_end);
                }


                int count_nm_department = nm.Count();

                ///////////////////////////////////////////////////////end bar8////////////////////////////////////////////////

                var result = new
                {
                    label = label_department,
                    fatality = count_fatality_department,
                    pd = count_pd_department,
                    lti = count_lti_department,
                    mti = count_mti_department,
                    mi = count_mi_department,
                    damage = count_damage_list_department,
                    rwc = count_rw_department,
                    nm = count_nm_department,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select n.division_id,d.division_th,d.division_en,n.count_select from (select i.division_id,ISNULL(count(i.division_id),0) as count_select from incident i ";
                sql = sql + "where 1=1 and i.department_id='" + department_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.division_id) as n ";
                sql = sql + "join division d on d.division_id = n.division_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_division = chageDataLanguage(ds.Tables[0].Rows[i]["division_th"].ToString(), ds.Tables[0].Rows[i]["division_en"].ToString(), lang);


                    var f = from c in dbConnect.injury_persons
                            join nc in dbConnect.incidents on c.incident_id equals nc.id
                            where nc.work_relate == "Y" && (nc.culpability == "G" || nc.culpability == "P")
                            && c.severity_injury_id == F
                            && nc.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                            && nc.process_status != REJECT
                            && nc.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                nc.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        f = f.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        f = f.Where(c => c.incident_date <= d_end);
                    }
                    int count_fatality_div = f.Count();

                    /////////////////////////////////////////end bar1////////////////////////////////////////////////////


                    var p = from c in dbConnect.injury_persons
                            join t in dbConnect.incidents on c.incident_id equals t.id
                            where t.work_relate == "Y" && (t.culpability == "G" || t.culpability == "P")
                            && c.severity_injury_id == PD
                            && t.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                            && t.process_status != REJECT
                            && t.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                t.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        p = v.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        p = v.Where(c => c.incident_date <= d_end);
                    }


                    int count_pd_div = p.Count();


                    /////////////////////////////////////////////bar2/////////////////////////////////////////////////

                    var li = from c in dbConnect.injury_persons
                             join g in dbConnect.incidents on c.incident_id equals g.id
                             where g.work_relate == "Y" && (g.culpability == "G" || g.culpability == "P")
                             && c.severity_injury_id == LTI
                             && g.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             && g.process_status != REJECT
                             && g.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 g.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        li = li.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        li = li.Where(c => c.incident_date <= d_end);
                    }


                    int count_lti_div = li.Count();

                    /////////////////////////////////////////////bar3/////////////////////////////////////////////////

                    var j = from c in dbConnect.injury_persons
                            join ii in dbConnect.incidents on c.incident_id equals ii.id
                            where ii.work_relate == "Y" && (ii.culpability == "G" || ii.culpability == "P")
                            && c.severity_injury_id == MTI
                            && ii.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                            && ii.process_status != REJECT
                            && ii.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                ii.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        j = j.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        j = j.Where(c => c.incident_date <= d_end);
                    }


                    int count_mti_div = j.Count();

                    /////////////////////////////////////////////bar4/////////////////////////////////////////////////

                    var m2 = from c in dbConnect.injury_persons
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.severity_injury_id == MI
                             && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             && i2.process_status != REJECT
                             && i2.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        m2 = m2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        m2 = m2.Where(c => c.incident_date <= d_end);
                    }


                    int count_mi_div = m2.Count();

                    //////////////////////////////////////////////////////end bar5///////////////////////////////////////////////

                    //var d2 = from c in dbConnect.damage_lists
                    //         join i3 in dbConnect.incidents on c.incident_id equals i3.id
                    //         where i3.work_relate == "Y" && (i3.culpability == "G" || i3.culpability == "P")
                    //         && i3.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                    //         && i3.process_status != REJECT
                    //         select new
                    //         {
                    //             c.id,
                    //             i3.incident_date
                    //         };

                    //if (date_start != "")
                    //{
                    //    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    //    d2 = d2.Where(c => c.incident_date >= d_start);
                    //}

                    //if (date_end != "")
                    //{
                    //    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    //    d2 = d2.Where(c => c.incident_date <= d_end);
                    //}


                    int count_damage_list_div = 0;//d2.Count();

                    //////////////////////////////////////////////////////end bar6///////////////////////////////////////////////

                    var rw2 = from c in dbConnect.injury_persons
                              join i2 in dbConnect.incidents on c.incident_id equals i2.id
                              where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                              && c.severity_injury_id == RWC
                              && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                              && i2.process_status != REJECT
                              && i2.process_status != EXEMPTION
                              select new
                              {
                                  c.id,
                                  i2.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        rw2 = rw2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        rw2 = rw2.Where(c => c.incident_date <= d_end);
                    }


                    int count_rw_div = rw2.Count();

                    //////////////////////////////////////////////////////end bar7///////////////////////////////////////////////


                    //var nm2 = from c in dbConnect.incidents
                    //          where c.work_relate == "Y" && (c.culpability == "G" || c.culpability == "P")
                    //          && c.impact == "N"
                    //          && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                    //          select new
                    //          {
                    //              c.id,
                    //              c.incident_date
                    //          };

                    //if (date_start != "")
                    //{
                    //    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    //    nm2 = nm2.Where(c => c.incident_date >= d_start);
                    //}

                    //if (date_end != "")
                    //{
                    //    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    //    nm2 = nm2.Where(c => c.incident_date <= d_end);
                    //}


                    int count_nm_div = 0;//nm2.Count();

                    ///////////////////////////////////////////////////////end bar8////////////////////////////////////////////////



                    var result3 = new
                    {
                        label = label_division,
                        fatality = count_fatality_div,
                        pd = count_pd_div,
                        lti = count_lti_div,
                        mti = count_mti_div,
                        mi = count_mi_div,
                        damage = count_damage_list_div,
                        rwc = count_rw_div,
                        nm = count_nm_div,
                        area_id = ds.Tables[0].Rows[i]["division_id"].ToString()
                    };

                    dataJson.Add(result3);

                }


            }
            else if (type == "division")
            {
                string department_id2 = "";
                string function_id2 = "";
                string company_id2 = "";

                var divisions2 = from f in dbConnect.divisions
                                 where f.division_id == division_id
                                 select f;

                foreach (var di in divisions2)
                {
                    department_id2 = di.department_id;

                    var departments2 = from f in dbConnect.departments
                                       where f.department_id == department_id2
                                       select f;

                    foreach (var de in departments2)
                    {
                        function_id2 = de.function_id;


                        var functions2 = from f in dbConnect.functions
                                         where f.function_id == function_id2
                                         select f;

                        foreach (var fun in functions2)
                        {
                            company_id2 = fun.company_id;

                        }

                    }
                }

                redirect = "allincident.aspx?company_id=" + company_id2 + "&function_id=" + function_id2 + "&department_id=" + department_id2 + "&division_id=" + division_id;





            }

            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };



            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }





        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncident3(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            int ONPROCESS = 1;
            int REJECT = 6;
            int REQUEST_CLOSE = 2;
            int CLOSE = 4;
            int CANCEL = 5;

            int REJECT_CASE = 3;
            int EXEMPTION = 4;



            string type = "";
            string company_id = "";
            string function_id = "";
            string department_id = "";
            string division_id = "";
            string redirect = "";

            if (area_id != "")
            {
                var companys = from c in dbConnect.companies
                               where c.company_id == area_id && c.country ==  Session["country"].ToString()
                               select c;

                if (companys.Count() > 0)
                {
                    type = "company";
                    company_id = area_id;

                }


                var functions = from f in dbConnect.functions
                                where f.function_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (functions.Count() > 0)
                {
                    type = "function";
                    function_id = area_id;
                }


                var departments = from f in dbConnect.departments
                                  where f.department_id == area_id && f.country == Session["country"].ToString()
                                  select f;
                if (departments.Count() > 0)
                {
                    type = "department";
                    department_id = area_id;
                }


                var divisions = from f in dbConnect.divisions
                                where f.division_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (divisions.Count() > 0)
                {
                    type = "division";
                    division_id = area_id;
                }

            }
            else
            {
                type = "all";

            }



            ArrayList dataJson = new ArrayList();



            string label_all = "";
            if (type == "all")
            {
                var n_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && (c.action_status_id == ONPROCESS//1 is on process
                           || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                            && i.country == Session["country"].ToString()
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_con = n_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_con = n_con.Where(c => c.incident_date <= d_end);
                }

                var n_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && (c.action_status_id == ONPROCESS//1 is on process
                           || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                            && i.country == Session["country"].ToString()
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_pre = n_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_pre = n_pre.Where(c => c.incident_date <= d_end);
                }



                var n = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && (c.action_status_id == ONPROCESS//1 is on process
                       || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) : Convert.ToDateTime(c.date_complete).Date) 
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && i.country == Session["country"].ToString()
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date<= d_end);
                }

                int count_incident_onprocess_all = n.Count() + n_pre.Count() + n_con.Count();
                label_all = chageDataLanguage("INSEE Group", "INSEE Group", lang);




                var v_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.country == Session["country"].ToString()
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_con = v_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_con = v_con.Where(c => c.incident_date <= d_end);
                }


                var v_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.country == Session["country"].ToString()
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_pre = v_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_pre = v_pre.Where(c => c.incident_date <= d_end);
                }



                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.action_status_id == CLOSE//1 is on process
                        && i.country == Session["country"].ToString()
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count() + v_pre.Count() + v_con.Count();





                var cc_con = from c in dbConnect.consequence_management_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                             (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) : Convert.ToDateTime(c.date_complete).Date)
                             > Convert.ToDateTime(c.due_date).Date)//delay
                             && i.country == Session["country"].ToString()
                             && i.process_status != REJECT_CASE
                             && i.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_con = cc_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_con = cc_con.Where(c => c.incident_date <= d_end);
                }




                var cc_pre = from c in dbConnect.preventive_action_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                             (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) : Convert.ToDateTime(c.date_complete).Date)
                             > Convert.ToDateTime(c.due_date).Date)//delay
                             && i.country == Session["country"].ToString()
                             && i.process_status != REJECT_CASE
                             && i.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_pre = cc_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_pre = cc_pre.Where(c => c.incident_date <= d_end);
                }







                 var cc = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                        (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) : Convert.ToDateTime(c.date_complete).Date) 
                        > Convert.ToDateTime(c.due_date).Date)//delay
                        && i.country == Session["country"].ToString()
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_delay_all = cc.Count() + cc_pre.Count() + cc_con.Count();

                var result = new
                {
                    label = label_all,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////


                DataSet ds = new DataSet();
                string sql = "select c.company_id,c.company_th,c.company_en,ISNULL(n.count_select,0) as count_select from (select i.company_id,ISNULL(count(i.company_id),0) as count_select from incident i ";
                sql = sql + " where 1=1 ";
                //  sql = sql + "where 1=1 and i.country ='" + Session["country"].ToString() + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.company_id) as n ";
                sql = sql + "right join company c on c.company_id = n.company_id";
                sql = sql + " where c.country ='" + Session["country"].ToString() + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_company = chageDataLanguage(ds.Tables[0].Rows[i]["company_th"].ToString(), ds.Tables[0].Rows[i]["company_en"].ToString(), lang);



                    var n2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && (c.action_status_id == ONPROCESS//1 is on process
                                 || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                 && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                 <= Convert.ToDateTime(c.due_date).Date
                                 && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_con = n2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_con = n2_con.Where(c => c.incident_date <= d_end);
                    }






                    var n2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && (c.action_status_id == ONPROCESS//1 is on process
                                 || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                 && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                 <= Convert.ToDateTime(c.due_date).Date
                                 && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_pre = n2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_pre = n2_pre.Where(c => c.incident_date <= d_end);
                    }




                    var n2 = from c in dbConnect.corrective_prevention_action_incidents
                            join i2 in dbConnect.incidents on c.incident_id equals i2.id
                            where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                            && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date
                            && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                            && i2.process_status != REJECT_CASE
                            && i2.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i2.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_company = n2.Count() + n2_pre.Count() + n2_con.Count();




                    var v2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_con = v2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_con = v2_con.Where(c => c.incident_date <= d_end);
                    }



                    var v2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_pre = v2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_pre = v2_pre.Where(c => c.incident_date <= d_end);
                    }







                    var v2 = from c in dbConnect.corrective_prevention_action_incidents
                            join i2 in dbConnect.incidents on c.incident_id equals i2.id
                            where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                            && i2.process_status != REJECT_CASE
                            && i2.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i2.incident_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date>= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_company = v2.Count() + v2_pre.Count() + v2_con.Count();






                    var cc2_con = from c in dbConnect.consequence_management_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && (c.action_status_id == ONPROCESS//1 is on process
                                  || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                  && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                  > Convert.ToDateTime(c.due_date).Date
                                  && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  select new
                                  {
                                      c.id,
                                      i2.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_con = cc2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_con = cc2_con.Where(c => c.incident_date <= d_end);
                    }



                    var cc2_pre = from c in dbConnect.preventive_action_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && (c.action_status_id == ONPROCESS//1 is on process
                                  || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                  && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                  > Convert.ToDateTime(c.due_date).Date
                                  && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  select new
                                  {
                                      c.id,
                                      i2.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_pre = cc2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_pre = cc2_pre.Where(c => c.incident_date <= d_end);
                    }





                    var cc2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                              && (c.action_status_id == ONPROCESS//1 is on process
                             || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                             && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                             > Convert.ToDateTime(c.due_date).Date
                             && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             && i2.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_delay_company = cc2.Count() + cc2_pre.Count() + cc2_con.Count() ;

                    var result2 = new
                    {
                        label = label_company,
                        onprocess = count_onprocess_company,
                        close = count_close_company,
                        delay = count_delay_company,
                        area_id = ds.Tables[0].Rows[i]["company_id"]
                    };


                    dataJson.Add(result2);

                }




            }
            else if (type == "company")
            {
                string label_company = "";
                var cu = from c in dbConnect.companies
                         where c.company_id == company_id
                         select c;

                foreach (var c in cu)
                {
                    label_company = chageDataLanguage(c.company_th, c.company_en, lang);
                }



                var n_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                            && i.company_id == company_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_con = n_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_con = n_con.Where(c => c.incident_date <= d_end);
                }


                var n_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                            && i.company_id == company_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_pre = n_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_pre = n_pre.Where(c => c.incident_date <= d_end);
                }


                var n = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && (c.action_status_id == ONPROCESS//1 is on process
                        || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                        && i.company_id == company_id
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count() + n_pre.Count() + n_con.Count();




                var v_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.company_id == company_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_con = v_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_con = v_con.Where(c => c.incident_date <= d_end);
                }




                var v_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.company_id == company_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_pre = v_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_pre = v_pre.Where(c => c.incident_date <= d_end);
                }


                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.action_status_id == CLOSE//1 is on process
                        && i.company_id == company_id
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count() + v_pre.Count() + v_con.Count();


                var cc_con = from c in dbConnect.consequence_management_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                             || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                             && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                             > Convert.ToDateTime(c.due_date).Date
                             && i.company_id == company_id
                             && i.process_status != REJECT_CASE
                             && i.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_con = cc_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_con = cc_con.Where(c => c.incident_date <= d_end);
                }




                var cc_pre = from c in dbConnect.preventive_action_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                             || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                             && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                             > Convert.ToDateTime(c.due_date).Date
                             && i.company_id == company_id
                             && i.process_status != REJECT_CASE
                             && i.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_pre = cc_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_pre = cc_pre.Where(c => c.incident_date <= d_end);
                }



                var cc = from c in dbConnect.corrective_prevention_action_incidents
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && (c.action_status_id == ONPROCESS//1 is on process
                         || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                         && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                         > Convert.ToDateTime(c.due_date).Date
                         && i.company_id == company_id
                         && i.process_status != REJECT_CASE
                         && i.process_status != EXEMPTION
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_delay_all = cc.Count() + cc_pre.Count() + cc_con.Count();

                var result = new
                {
                    label = label_company,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////

                DataSet ds = new DataSet();
                string sql = "select f.function_id,f.function_th,f.function_en,ISNULL(n.count_select,0) as count_select from (select i.function_id,ISNULL(count(i.function_id),0) as count_select from incident i ";
                sql = sql + "where 1=1 and i.company_id='" + company_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.function_id) as n ";
                sql = sql + "right join [function] f on f.function_id = n.function_id where  f.function_en !='-' and f.company_id='" + company_id + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_function = chageDataLanguage(ds.Tables[0].Rows[i]["function_th"].ToString(), ds.Tables[0].Rows[i]["function_en"].ToString(), lang);


                    var n2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && (c.action_status_id == ONPROCESS//1 is on process
                                 || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                 && (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date <= Convert.ToDateTime(c.due_date).Date)//ไม่นับตัว delay
                                 && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_con = n2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_con = n2_con.Where(c => c.incident_date <= d_end);
                    }

                    var n2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && (c.action_status_id == ONPROCESS//1 is on process
                                 || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                 && (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date <= Convert.ToDateTime(c.due_date).Date)//ไม่นับตัว delay
                                 && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                        if (date_start != "")
                        {
                            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                            n2_pre = n2_pre.Where(c => c.incident_date >= d_start);
                        }

                        if (date_end != "")
                        {
                            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                            n2_pre = n2_pre.Where(c => c.incident_date <= d_end);
                        }



                    var n2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                             || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                             && (DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date <= Convert.ToDateTime(c.due_date).Date)//ไม่นับตัว delay
                             && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             && i2.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_function = n2.Count() + n2_pre.Count() + n2_con.Count();




                    var v2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_con = v2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_con = v2_con.Where(c => c.incident_date <= d_end);
                    }





                    var v2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                        if (date_start != "")
                        {
                            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                            v2_pre = v2_pre.Where(c => c.incident_date >= d_start);
                        }

                        if (date_end != "")
                        {
                            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                            v2_pre = v2_pre.Where(c => c.incident_date <= d_end);
                        }



                    var v2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.action_status_id == CLOSE//1 is on process
                             && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             && i2.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_function = v2.Count() + v2_pre.Count() + v2_con.Count();






                    var cc2_con = from c in dbConnect.consequence_management_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                  && (c.action_status_id == ONPROCESS//1 is on process
                                 || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                 && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                 > Convert.ToDateTime(c.due_date).Date
                                  && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  select new
                                  {
                                      c.id,
                                      i2.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_con = cc2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_con = cc2_con.Where(c => c.incident_date <= d_end);
                    }




                    var cc2_pre = from c in dbConnect.preventive_action_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                  && (c.action_status_id == ONPROCESS//1 is on process
                                 || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                 && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                 > Convert.ToDateTime(c.due_date).Date
                                  && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  select new
                                  {
                                      c.id,
                                      i2.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_pre = cc2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_pre = cc2_pre.Where(c => c.incident_date <= d_end);
                    }


                    var cc2 = from c in dbConnect.corrective_prevention_action_incidents
                              join i2 in dbConnect.incidents on c.incident_id equals i2.id
                              where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                              && (c.action_status_id == ONPROCESS//1 is on process
                             || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                             && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                             > Convert.ToDateTime(c.due_date).Date
                              && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                              && i2.process_status != REJECT_CASE
                              && i2.process_status != EXEMPTION
                              select new
                              {
                                  c.id,
                                  i2.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_delay_function = cc2.Count() + cc2_pre.Count() + cc2_con.Count();

                    var result2 = new
                    {
                        label = label_function,
                        onprocess = count_onprocess_function,
                        close = count_close_function,
                        delay = count_delay_function,
                        area_id = ds.Tables[0].Rows[i]["function_id"]
                    };


                    dataJson.Add(result2);

                }

            }
            else if (type == "function")
            {
                string label_function = "";
                var fu = from c in dbConnect.functions
                         where c.function_id == function_id
                         select c;

                foreach (var f in fu)
                {
                    label_function = chageDataLanguage(f.function_th, f.function_en, lang);
                }


                var n_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                            && i.function_id == function_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_con = n_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_con = n_con.Where(c => c.incident_date <= d_end);
                }




                var n_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                            && i.function_id == function_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_pre = n_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_pre = n_pre.Where(c => c.incident_date <= d_end);
                }



                var n = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && (c.action_status_id == ONPROCESS//1 is on process
                        || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                        && i.function_id == function_id
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date>= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count() + n_pre.Count() + n_con.Count();


                var v_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.function_id == function_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_con = v_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_con = v_con.Where(c => c.incident_date <= d_end);
                }


                var v_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.function_id == function_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_pre = v_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_pre = v_pre.Where(c => c.incident_date <= d_end);
                }
              

                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.action_status_id == CLOSE//1 is on process
                        && i.function_id == function_id
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count() + v_pre.Count() + v_con.Count();



                var cc_con = from c in dbConnect.consequence_management_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            > Convert.ToDateTime(c.due_date).Date
                             && i.function_id == function_id
                             && i.process_status != REJECT_CASE
                             && i.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_con = cc_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_con = cc_con.Where(c => c.incident_date <= d_end);
                }


                var cc_pre = from c in dbConnect.preventive_action_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            > Convert.ToDateTime(c.due_date).Date
                             && i.function_id == function_id
                             && i.process_status != REJECT_CASE
                             && i.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_pre = cc_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_pre = cc_pre.Where(c => c.incident_date <= d_end);
                }


                var cc = from c in dbConnect.corrective_prevention_action_incidents
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && (c.action_status_id == ONPROCESS//1 is on process
                        || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        > Convert.ToDateTime(c.due_date).Date
                         && i.function_id == function_id
                         && i.process_status != REJECT_CASE
                         && i.process_status != EXEMPTION
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_delay_all = cc.Count() + cc_pre.Count() + cc_con.Count();

                var result = new
                {
                    label = label_function,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select n.department_id,d.department_th,d.department_en,n.count_select from (select i.department_id,ISNULL(count(i.department_id),0) as count_select from incident i ";
                sql = sql + "where 1=1  and i.function_id='" + function_id + "' ";


                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + "group by i.department_id) as n ";
                sql = sql + "join department d on d.department_id = n.department_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                   
                    string label_department = chageDataLanguage(ds.Tables[0].Rows[i]["department_th"].ToString(), ds.Tables[0].Rows[i]["department_en"].ToString(), lang);


                    var n2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && (c.action_status_id == ONPROCESS//1 is on process
                                || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                                 && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_con = n2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_con = n2_con.Where(c => c.incident_date <= d_end);
                    }



                    var n2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && (c.action_status_id == ONPROCESS//1 is on process
                                || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                                 && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_pre = n2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_pre = n2_pre.Where(c => c.incident_date <= d_end);
                    }




                    var n2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                               && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                             && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             && i2.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_department = n2.Count() + n2_pre.Count() + n2_con.Count();



                    var v2_con = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_con = v2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_con = v2_con.Where(c => c.incident_date <= d_end);
                    }



                    var v2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_pre = v2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_pre = v2_pre.Where(c => c.incident_date <= d_end);
                    }






                    var v2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.action_status_id == CLOSE//1 is on process
                             && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             && i2.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i2.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_department = v2.Count() + v2_pre.Count() + v2_con.Count();


                    var cc2_con = from c in dbConnect.consequence_management_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && (c.action_status_id == ONPROCESS//1 is on process
                                    || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                    && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    > Convert.ToDateTime(c.due_date).Date
                                  && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  select new
                                  {
                                      c.id,
                                      i2.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_con = cc2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_con = cc2_con.Where(c => c.incident_date <= d_end);
                    }



                    var cc2_pre = from c in dbConnect.preventive_action_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && (c.action_status_id == ONPROCESS//1 is on process
                                    || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                    && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    > Convert.ToDateTime(c.due_date).Date
                                  && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  select new
                                  {
                                      c.id,
                                      i2.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_pre = cc2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_pre = cc2_pre.Where(c => c.incident_date <= d_end);
                    }



                    var cc2 = from c in dbConnect.corrective_prevention_action_incidents
                              join i2 in dbConnect.incidents on c.incident_id equals i2.id
                              where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                               && (c.action_status_id == ONPROCESS//1 is on process
                                || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                > Convert.ToDateTime(c.due_date).Date
                              && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                              && i2.process_status != REJECT_CASE
                              && i2.process_status != EXEMPTION
                              select new
                              {
                                  c.id,
                                  i2.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_delay_department = cc2.Count() + cc2_pre.Count() + cc2_con.Count();

                    var result2 = new
                    {
                        label = label_department,
                        onprocess = count_onprocess_department,
                        close = count_close_department,
                        delay = count_delay_department,
                        area_id = ds.Tables[0].Rows[i]["department_id"]
                    };


                    dataJson.Add(result2);
                }


            }
            else if (type == "department")
            {

                var de = from d in dbConnect.departments
                         where d.department_id == department_id
                         select d;
                foreach (var u in de)
                {
                    label_all = chageDataLanguage(u.department_th, u.department_en, lang);
                }




                var n_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                            && i.department_id == department_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_con = n_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_con = n_con.Where(c => c.incident_date <= d_end);
                }


                var n_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                            && i.department_id == department_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_pre = n_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_pre = n_pre.Where(c => c.incident_date <= d_end);
                }





                var n = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && (c.action_status_id == ONPROCESS//1 is on process
                        || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                        && i.department_id == department_id
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count() + n_pre.Count() + n_con.Count();



                var v_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.department_id == department_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_con = v_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_con = v_con.Where(c => c.incident_date <= d_end);
                }



                var v_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.department_id == department_id
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            select new
                            {
                                c.id,
                                i.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_pre = v_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_pre = v_pre.Where(c => c.incident_date <= d_end);
                }


                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.action_status_id == CLOSE//1 is on process
                        && i.department_id == department_id
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        select new
                        {
                            c.id,
                            i.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count() + v_pre.Count() + v_con.Count();



                var cc_con = from c in dbConnect.consequence_management_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                             || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                             && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                             > Convert.ToDateTime(c.due_date).Date
                             && i.department_id == department_id
                             && i.process_status != REJECT_CASE
                             && i.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_con = cc_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_con = cc_con.Where(c => c.incident_date <= d_end);
                }





                var cc_pre = from c in dbConnect.preventive_action_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                             || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                             && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                             > Convert.ToDateTime(c.due_date).Date
                             && i.department_id == department_id
                             && i.process_status != REJECT_CASE
                             && i.process_status != EXEMPTION
                             select new
                             {
                                 c.id,
                                 i.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_pre = cc_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_pre = cc_pre.Where(c => c.incident_date <= d_end);
                }


                var cc = from c in dbConnect.corrective_prevention_action_incidents
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && (c.action_status_id == ONPROCESS//1 is on process
                         || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                         && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                         > Convert.ToDateTime(c.due_date).Date
                         && i.department_id == department_id
                         && i.process_status != REJECT_CASE
                         && i.process_status != EXEMPTION
                         select new
                         {
                             c.id,
                             i.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_delay_all = cc.Count() + cc_pre.Count() + cc_con.Count();

                var result = new
                {
                    label = label_all,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select n.division_id,d.division_th,d.division_en,n.count_select from (select i.division_id,ISNULL(count(i.department_id),0) as count_select from incident i ";
                sql = sql + " where 1=1 and i.department_id='" + department_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }
                sql = sql + "group by i.division_id ) as n ";
                sql = sql + "join division d on d.division_id = n.division_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_division = chageDataLanguage(ds.Tables[0].Rows[i]["division_th"].ToString(), ds.Tables[0].Rows[i]["division_en"].ToString(), lang);


                    var n2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && (c.action_status_id == ONPROCESS//1 is on process
                                || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                                 && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_con = n2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_con = n2_con.Where(c => c.incident_date <= d_end);
                    }



                    var n2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && (c.action_status_id == ONPROCESS//1 is on process
                                || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                                 && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_pre = n2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_pre = n2_pre.Where(c => c.incident_date <= d_end);
                    }
                    
                    
                    
                    
                    var n2 = from c in dbConnect.corrective_prevention_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && (c.action_status_id == ONPROCESS//1 is on process
                                || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date //ไม่นัยตัว delay
                                 && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                        if (date_start != "")
                        {
                            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                            n2 = n2.Where(c => c.incident_date >= d_start);
                        }

                        if (date_end != "")
                        {
                            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                            n2 = n2.Where(c => c.incident_date <= d_end);
                        }

                        int count_onprocess_department = n2.Count() + n2_pre.Count() + n2_con.Count();




                        var v2_con = from c in dbConnect.consequence_management_incidents
                                     join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                     where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                     && c.action_status_id == CLOSE//1 is on process
                                     && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                     && i2.process_status != REJECT_CASE
                                     && i2.process_status != EXEMPTION
                                     select new
                                     {
                                         c.id,
                                         i2.incident_date
                                     };

                        if (date_start != "")
                        {
                            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                            v2_con = v2_con.Where(c => c.incident_date >= d_start);
                        }

                        if (date_end != "")
                        {
                            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                            v2_con = v2_con.Where(c => c.incident_date <= d_end);
                        }


                        

                        var v2_pre = from c in dbConnect.preventive_action_incidents
                                     join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                     where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                     && c.action_status_id == CLOSE//1 is on process
                                     && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                     && i2.process_status != REJECT_CASE
                                     && i2.process_status != EXEMPTION
                                     select new
                                     {
                                         c.id,
                                         i2.incident_date
                                     };

                        if (date_start != "")
                        {
                            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                            v2_pre = v2_pre.Where(c => c.incident_date >= d_start);
                        }

                        if (date_end != "")
                        {
                            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                            v2_pre = v2_pre.Where(c => c.incident_date <= d_end);
                        }


                        var v2 = from c in dbConnect.corrective_prevention_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 select new
                                 {
                                     c.id,
                                     i2.incident_date
                                 };

                        if (date_start != "")
                        {
                            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                            v2 = v2.Where(c => c.incident_date >= d_start);
                        }

                        if (date_end != "")
                        {
                            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                            v2 = v2.Where(c => c.incident_date <= d_end);
                        }


                        int count_close_department = v2.Count() + v2_pre.Count() + v2_con.Count();




                        var cc2_con = from c in dbConnect.consequence_management_incidents
                                      join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                      where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                      && (c.action_status_id == ONPROCESS//1 is on process
                                      || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                      && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                      > Convert.ToDateTime(c.due_date).Date
                                      && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                      && i2.process_status != REJECT_CASE
                                      && i2.process_status != EXEMPTION
                                      select new
                                      {
                                          c.id,
                                          i2.incident_date
                                      };

                        if (date_start != "")
                        {
                            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                            cc2_con = cc2_con.Where(c => c.incident_date >= d_start);
                        }

                        if (date_end != "")
                        {
                            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                            cc2_con = cc2_con.Where(c => c.incident_date <= d_end);
                        }


                        var cc2_pre = from c in dbConnect.preventive_action_incidents
                                      join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                      where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                      && (c.action_status_id == ONPROCESS//1 is on process
                                      || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                      && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                      > Convert.ToDateTime(c.due_date).Date
                                      && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                      && i2.process_status != REJECT_CASE
                                      && i2.process_status != EXEMPTION
                                      select new
                                      {
                                          c.id,
                                          i2.incident_date
                                      };

                        if (date_start != "")
                        {
                            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                            cc2_pre = cc2_pre.Where(c => c.incident_date >= d_start);
                        }

                        if (date_end != "")
                        {
                            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                            cc2_pre = cc2_pre.Where(c => c.incident_date <= d_end);
                        }


                        var cc2 = from c in dbConnect.corrective_prevention_action_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                  && (c.action_status_id == ONPROCESS//1 is on process
                                  || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                  && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                  > Convert.ToDateTime(c.due_date).Date
                                  && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  select new
                                  {
                                      c.id,
                                      i2.incident_date
                                  };

                        if (date_start != "")
                        {
                            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                            cc2 = cc2.Where(c => c.incident_date >= d_start);
                        }

                        if (date_end != "")
                        {
                            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                            cc2 = cc2.Where(c => c.incident_date <= d_end);
                        }


                        int count_delay_department = cc2.Count() + cc2_pre.Count() + cc2_con.Count();

                        var result2 = new
                        {
                            label = label_division,
                            onprocess = count_onprocess_department,
                            close = count_close_department,
                            delay = count_delay_department,
                            area_id = ds.Tables[0].Rows[i]["division_id"]
                        };


                        dataJson.Add(result2);
                

                }

            }
            else if (type == "division")
            {
                string department_id2 = "";
                string function_id2 = "";
                string company_id2 = "";

                var divisions2 = from f in dbConnect.divisions
                                 where f.division_id == division_id
                                 select f;

                foreach (var di in divisions2)
                {
                    department_id2 = di.department_id;

                    var departments2 = from f in dbConnect.departments
                                       where f.department_id == department_id2
                                       select f;

                    foreach (var de in departments2)
                    {
                        function_id2 = de.function_id;


                        var functions2 = from f in dbConnect.functions
                                         where f.function_id == function_id2
                                         select f;

                        foreach (var fun in functions2)
                        {
                            company_id2 = fun.company_id;

                        }

                    }
                }

                redirect = "allincident.aspx?company_id=" + company_id2 + "&function_id=" + function_id2 + "&department_id=" + department_id2 + "&division_id=" + division_id;





            }

            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }




        [WebMethod(EnableSession=true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncident4(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            int ONPROCESS = 1;
            int REJECT = 6;
            int REQUEST_CLOSE = 2;
            int CLOSE = 4;
            int CANCEL = 5;

            int REJECT_CASE = 3;
            int EXEMPTION = 4;


            int F = 1;
            int PD = 2;
            int LTI = 3;
            int MTI = 4;
            int MI = 5;
            int RWC = 6;


            if (Session["country"].ToString() == "thailand")
            {
                F = 1;
                PD = 2;
                LTI = 3;
                MTI = 4;
                MI = 5;
                RWC = 6;
            }
            else if (Session["country"].ToString() == "srilanka")
            {
                F = 7;
                PD = 8;
                LTI = 9;
                MTI = 10;
                MI = 11;
                RWC = 12;
            }

            string type = "";
            string company_id = "";
            string function_id = "";
            string department_id = "";
            string division_id = "";
            string redirect = "";

            if (area_id != "")
            {

                var companys = from c in dbConnect.companies
                               where c.company_id == area_id && c.country == Session["country"].ToString()
                               select c;

                if (companys.Count() > 0)
                {
                    type = "company";
                    company_id = area_id;

                }

                var functions = from f in dbConnect.functions
                                where f.function_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (functions.Count() > 0)
                {
                    type = "function";
                    function_id = area_id;
                }


                var departments = from f in dbConnect.departments
                                  where f.department_id == area_id && f.country == Session["country"].ToString()
                                  select f;
                if (departments.Count() > 0)
                {
                    type = "department";
                    department_id = area_id;
                }


                var divisions = from f in dbConnect.divisions
                                where f.division_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (divisions.Count() > 0)
                {
                    type = "division";
                    division_id = area_id;
                }

            }
            else
            {
                type = "all";

            }



            ArrayList dataJson = new ArrayList();



            string label_all = "";
            if (type == "all")
            {
                var n_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                           || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                            || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.country == Session["country"].ToString()
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_con = n_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_con = n_con.Where(c => c.incident_date <= d_end);
                }



                var n_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                           || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                            || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.country == Session["country"].ToString()
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_pre = n_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_pre = n_pre.Where(c => c.incident_date <= d_end);
                }


                var n = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && (c.action_status_id == ONPROCESS//1 is on process
                       || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                        || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.country == Session["country"].ToString()
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                        group c by new{
                            c.id,
                            i.incident_date
                        } into g
                        select new
                        {
                            g.Key.id,
                            g.Key.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count() + n_pre.Count() + n_con.Count();
                label_all = chageDataLanguage("INSEE Group", "INSEE Group", lang);

                var v_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                            || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.country == Session["country"].ToString()
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_con = v_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_con = v_con.Where(c => c.incident_date <= d_end);
                }



                var v_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                            || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.country == Session["country"].ToString()
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_pre = v_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_pre = v_pre.Where(c => c.incident_date <= d_end);
                }


                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.action_status_id == CLOSE//
                        && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                        || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.country == Session["country"].ToString()
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                        group c by new
                        {
                            c.id,
                            i.incident_date
                        } into g
                        select new
                        {
                            g.Key.id,
                            g.Key.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count() + v_pre.Count() + v_con.Count();




                var cc_con = from c in dbConnect.consequence_management_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                              && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                            (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            > Convert.ToDateTime(c.due_date).Date)//delay
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                            || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.country == Session["country"].ToString()
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_con = cc_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_con = cc_con.Where(c => c.incident_date <= d_end);
                }



                var cc_pre = from c in dbConnect.preventive_action_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                              && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                            (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            > Convert.ToDateTime(c.due_date).Date)//delay
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                            || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.country == Session["country"].ToString()
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_pre = cc_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_pre = cc_pre.Where(c => c.incident_date <= d_end);
                }


                var cc = from c in dbConnect.corrective_prevention_action_incidents
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                          && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                        (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        > Convert.ToDateTime(c.due_date).Date)//delay
                         && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                        || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.country == Session["country"].ToString()
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                         group c by new
                         {
                             c.id,
                             i.incident_date
                         } into g
                         select new
                         {
                             g.Key.id,
                             g.Key.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_delay_all = cc.Count() + cc_pre.Count() + cc_con.Count();

                var result = new
                {
                    label = label_all,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select c.company_id,c.company_th,c.company_en,ISNULL(n.count_select,0) as count_select from (select i.company_id,ISNULL(count(i.company_id),0) as count_select from incident i  ";
                sql = sql + " where 1=1 ";
                //sql = sql + "where 1=1 and i.country ='" + Session["country"].ToString() + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.company_id) as n ";
                sql = sql + "right join company c on c.company_id = n.company_id";
                sql = sql + " where c.country ='" + Session["country"].ToString() + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_company = chageDataLanguage(ds.Tables[0].Rows[i]["company_th"].ToString(), ds.Tables[0].Rows[i]["company_en"].ToString(), lang);


                    var n2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                  && (c.action_status_id == ONPROCESS//1 is on process
                                   || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                    && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                                 && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                 || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_con = n2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_con = n2_con.Where(c => c.incident_date <= d_end);
                    }




                    var n2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                  && (c.action_status_id == ONPROCESS//1 is on process
                                   || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                    && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                                 && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                 || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_pre = n2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_pre = n2_pre.Where(c => c.incident_date <= d_end);
                    }


                    var n2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                              && (c.action_status_id == ONPROCESS//1 is on process
                               || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                             && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                             || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                             && i2.process_status != REJECT_CASE
                             && i2.process_status != EXEMPTION
                             && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i2.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_function = n2.Count() + n2_pre.Count() + n2_con.Count();




                    var v2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE
                                 && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_con = v2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_con = v2_con.Where(c => c.incident_date <= d_end);
                    }




                    var v2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE
                                 && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_pre = v2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_pre = v2_pre.Where(c => c.incident_date <= d_end);
                    }




                    var v2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.action_status_id == CLOSE
                             && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                            || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i2.process_status != REJECT_CASE
                            && i2.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i2.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_function = v2.Count() + v2_pre.Count() + v2_con.Count();





                    var cc2_con = from c in dbConnect.consequence_management_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                    (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    > Convert.ToDateTime(c.due_date).Date)//delay
                                  && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                  && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                    && i2.process_status != REJECT_CASE
                                    && i2.process_status != EXEMPTION
                                    && ju.status == "A"
                                  group c by new
                                  {
                                      c.id,
                                      i2.incident_date
                                  } into g
                                  select new
                                  {
                                      g.Key.id,
                                      g.Key.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_con = cc2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_con = cc2_con.Where(c => c.incident_date <= d_end);
                    }


                    var cc2_pre = from c in dbConnect.preventive_action_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                    (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    > Convert.ToDateTime(c.due_date).Date)//delay
                                  && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                  && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                    && i2.process_status != REJECT_CASE
                                    && i2.process_status != EXEMPTION
                                    && ju.status == "A"
                                  group c by new
                                  {
                                      c.id,
                                      i2.incident_date
                                  } into g
                                  select new
                                  {
                                      g.Key.id,
                                      g.Key.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_pre = cc2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_pre = cc2_pre.Where(c => c.incident_date <= d_end);
                    }


                    var cc2 = from c in dbConnect.corrective_prevention_action_incidents
                              join i2 in dbConnect.incidents on c.incident_id equals i2.id
                              join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                              where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                               && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                > Convert.ToDateTime(c.due_date).Date)//delay
                              && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                              && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                              group c by new
                              {
                                  c.id,
                                  i2.incident_date
                              } into g
                              select new
                              {
                                  g.Key.id,
                                  g.Key.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_delay_function = cc2.Count() + cc2_pre.Count() + cc2_con.Count();

                    var result2 = new
                    {
                        label = label_company,
                        onprocess = count_onprocess_function,
                        close = count_close_function,
                        delay = count_delay_function,
                        area_id = ds.Tables[0].Rows[i]["company_id"]
                    };


                    dataJson.Add(result2);

                }




            }
            else if (type == "company")
            {

                string label_company = "";
                var cu = from c in dbConnect.companies
                         where c.company_id == company_id
                         select c;

                foreach (var c in cu)
                {
                    label_company = chageDataLanguage(c.company_th, c.company_en, lang);
                }


                var n_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                           || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                            && i.company_id == company_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_con = n_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_con = n_con.Where(c => c.incident_date <= d_end);
                }




                var n_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                           || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                            && i.company_id == company_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_pre = n_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_pre = n_pre.Where(c => c.incident_date <= d_end);
                }





                var n = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && (c.action_status_id == ONPROCESS//1 is on process
                       || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && i.company_id == company_id
                        && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                        group c by new
                        {
                            c.id,
                            i.incident_date
                        } into g
                        select new
                        {
                            g.Key.id,
                            g.Key.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count() + n_pre.Count() + n_con.Count();




                var v_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.company_id == company_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_con = v_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_con = v_con.Where(c => c.incident_date <= d_end);
                }



                var v_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.company_id == company_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_pre = v_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_pre = v_pre.Where(c => c.incident_date <= d_end);
                }




                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.action_status_id == CLOSE//1 is on process
                        && i.company_id == company_id
                        && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                        group c by new
                        {
                            c.id,
                            i.incident_date
                        } into g
                        select new
                        {
                            g.Key.id,
                            g.Key.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count() + v_pre.Count() + v_con.Count();


                var cc_con = from c in dbConnect.consequence_management_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                            (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            > Convert.ToDateTime(c.due_date).Date)//delay
                             && i.company_id == company_id
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_con = cc_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_con = cc_con.Where(c => c.incident_date <= d_end);
                }



                var cc_pre = from c in dbConnect.preventive_action_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                            (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            > Convert.ToDateTime(c.due_date).Date)//delay
                             && i.company_id == company_id
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_pre = cc_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_pre = cc_pre.Where(c => c.incident_date <= d_end);
                }




                var cc = from c in dbConnect.corrective_prevention_action_incidents
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                        (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        > Convert.ToDateTime(c.due_date).Date)//delay
                         && i.company_id == company_id
                         && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                         group c by new
                         {
                             c.id,
                             i.incident_date
                         } into g
                         select new
                         {
                             g.Key.id,
                             g.Key.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_delay_all = cc.Count() + cc_pre.Count() + cc_con.Count();

                var result = new
                {
                    label = label_company,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select f.function_id,f.function_th,f.function_en,ISNULL(n.count_select,0) as count_select from (select i.function_id,ISNULL(count(i.function_id),0) as count_select from incident i ";
                sql = sql + "where 1=1 and i.company_id='" + company_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.function_id) as n ";
                sql = sql + "right join [function] f on f.function_id = n.function_id where  f.function_en !='-' and f.company_id='" + company_id + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_function = chageDataLanguage(ds.Tables[0].Rows[i]["function_th"].ToString(), ds.Tables[0].Rows[i]["function_en"].ToString(), lang);



                    var n2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                  && (c.action_status_id == ONPROCESS//1 is on process
                                   || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                    && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                                 && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                 || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_con = n2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_con = n2_con.Where(c => c.incident_date <= d_end);
                    }


                    var n2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                  && (c.action_status_id == ONPROCESS//1 is on process
                                   || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                    && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                                 && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                 || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_pre = n2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_pre = n2_pre.Where(c => c.incident_date <= d_end);
                    }



                    var n2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                              && (c.action_status_id == ONPROCESS//1 is on process
                               || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                             && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                             || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                             && i2.process_status != REJECT_CASE
                             && i2.process_status != EXEMPTION
                             && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i2.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_function = n2.Count() + n2_pre.Count() + n2_con.Count();



                    var v2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE
                                 && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_con = v2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_con = v2_con.Where(c => c.incident_date <= d_end);
                    }




                    var v2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE
                                 && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_pre = v2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_pre = v2_pre.Where(c => c.incident_date <= d_end);
                    }





                    var v2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.action_status_id == CLOSE
                             && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                            || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i2.process_status != REJECT_CASE
                            && i2.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i2.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_function = v2.Count() + v2_pre.Count() + v2_con.Count();




                    var cc2_con = from c in dbConnect.consequence_management_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                    (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    > Convert.ToDateTime(c.due_date).Date)//delay
                                  && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                  && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  && ju.status == "A"
                                  group c by new
                                  {
                                      c.id,
                                      i2.incident_date
                                  } into g
                                  select new
                                  {
                                      g.Key.id,
                                      g.Key.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_con = cc2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_con = cc2_con.Where(c => c.incident_date <= d_end);
                    }


                    var cc2_pre = from c in dbConnect.preventive_action_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                    (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    > Convert.ToDateTime(c.due_date).Date)//delay
                                  && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                  && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  && ju.status == "A"
                                  group c by new
                                  {
                                      c.id,
                                      i2.incident_date
                                  } into g
                                  select new
                                  {
                                      g.Key.id,
                                      g.Key.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_pre = cc2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_pre = cc2_pre.Where(c => c.incident_date <= d_end);
                    }


                    var cc2 = from c in dbConnect.corrective_prevention_action_incidents
                              join i2 in dbConnect.incidents on c.incident_id equals i2.id
                              join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                              where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                               && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                > Convert.ToDateTime(c.due_date).Date)//delay
                              && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                              && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                              && i2.process_status != REJECT_CASE
                              && i2.process_status != EXEMPTION
                              && ju.status == "A"
                              group c by new
                              {
                                  c.id,
                                  i2.incident_date
                              } into g
                              select new
                              {
                                  g.Key.id,
                                  g.Key.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_delay_function = cc2.Count() + cc2_pre.Count() + cc2_con.Count();

                    var result2 = new
                    {
                        label = label_function,
                        onprocess = count_onprocess_function,
                        close = count_close_function,
                        delay = count_delay_function,
                        area_id = ds.Tables[0].Rows[i]["function_id"]
                    };


                    dataJson.Add(result2);

                }



            }
            else if (type == "function")
            {
                string label_function = "";
                var fu = from c in dbConnect.functions
                         where c.function_id == function_id
                         select c;

                foreach (var f in fu)
                {
                    label_function = chageDataLanguage(f.function_th, f.function_en, lang);
                }

                var n_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                           || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                            && i.function_id == function_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_con = n_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_con = n_con.Where(c => c.incident_date <= d_end);
                }


                var n_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                           || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                            && i.function_id == function_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_pre = n_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_pre = n_pre.Where(c => c.incident_date <= d_end);
                }


                var n = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && (c.action_status_id == ONPROCESS//1 is on process
                       || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && i.function_id == function_id
                        && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                        group c by new
                        {
                            c.id,
                            i.incident_date
                        } into g
                        select new
                        {
                            g.Key.id,
                            g.Key.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count() + n_pre.Count() + n_con.Count();




                var v_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.function_id == function_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_con = v_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_con = v_con.Where(c => c.incident_date <= d_end);
                }




                var v_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.function_id == function_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_pre = v_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_pre = v_pre.Where(c => c.incident_date <= d_end);
                }

                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.action_status_id == CLOSE//1 is on process
                        && i.function_id == function_id
                        && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                        group c by new
                        {
                            c.id,
                            i.incident_date
                        } into g
                        select new
                        {
                            g.Key.id,
                            g.Key.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count() + v_pre.Count() + v_con.Count();





                var cc_con = from c in dbConnect.consequence_management_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                            (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            > Convert.ToDateTime(c.due_date).Date)//delay
                             && i.function_id == function_id
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_con = cc_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_con = cc_con.Where(c => c.incident_date <= d_end);
                }


                var cc_pre = from c in dbConnect.preventive_action_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                             && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                            (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            > Convert.ToDateTime(c.due_date).Date)//delay
                             && i.function_id == function_id
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_pre = cc_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_pre = cc_pre.Where(c => c.incident_date <= d_end);
                }

                var cc = from c in dbConnect.corrective_prevention_action_incidents
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                         && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                        (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        > Convert.ToDateTime(c.due_date).Date)//delay
                         && i.function_id == function_id
                         && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                         group c by new
                         {
                             c.id,
                             i.incident_date
                         } into g
                         select new
                         {
                             g.Key.id,
                             g.Key.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_delay_all = cc.Count() + cc_pre.Count() + cc_con.Count();

                var result = new
                {
                    label = label_function,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select n.department_id,d.department_th,d.department_en,n.count_select from (select i.department_id,ISNULL(count(i.department_id),0) as count_select from incident i ";
                sql = sql + "where 1=1  and i.function_id='" + function_id + "' ";


                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + "group by i.department_id) as n ";
                sql = sql + "join department d on d.department_id = n.department_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_department = chageDataLanguage(ds.Tables[0].Rows[i]["department_th"].ToString(), ds.Tables[0].Rows[i]["department_en"].ToString(), lang);

                    var n2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && (c.action_status_id == ONPROCESS//1 is on process
                                   || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                    && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                                 && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_con = n2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_con = n2_con.Where(c => c.incident_date <= d_end);
                    }


                    var n2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && (c.action_status_id == ONPROCESS//1 is on process
                                   || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                    && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                                 && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_pre = n2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_pre = n2_pre.Where(c => c.incident_date <= d_end);
                    }


                    var n2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                               || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                             && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i2.process_status != REJECT_CASE
                            && i2.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i2.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_department = n2.Count() + n2_pre.Count() + n2_con.Count();

                    var v2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_con = v2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_con = v2_con.Where(c => c.incident_date <= d_end);
                    }


                    var v2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_pre = v2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_pre = v2_pre.Where(c => c.incident_date <= d_end);
                    }



                    var v2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.action_status_id == CLOSE//1 is on process
                             && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i2.process_status != REJECT_CASE
                            && i2.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i2.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_department = v2.Count() + v2_pre.Count() + v2_con.Count();



                    var cc2_con = from c in dbConnect.consequence_management_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                    && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                    (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    > Convert.ToDateTime(c.due_date).Date)//delay
                                  && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                  && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  && ju.status == "A"
                                  group c by new
                                  {
                                      c.id,
                                      i2.incident_date
                                  } into g
                                  select new
                                  {
                                      g.Key.id,
                                      g.Key.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_con = cc2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_con = cc2_con.Where(c => c.incident_date <= d_end);
                    }


                    var cc2_pre = from c in dbConnect.preventive_action_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                    && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                    (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    > Convert.ToDateTime(c.due_date).Date)//delay
                                  && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                  && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                  && i2.process_status != REJECT_CASE
                                  && i2.process_status != EXEMPTION
                                  && ju.status == "A"
                                  group c by new
                                  {
                                      c.id,
                                      i2.incident_date
                                  } into g
                                  select new
                                  {
                                      g.Key.id,
                                      g.Key.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_pre = cc2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_pre = cc2_pre.Where(c => c.incident_date <= d_end);
                    }


                    var cc2 = from c in dbConnect.corrective_prevention_action_incidents
                              join i2 in dbConnect.incidents on c.incident_id equals i2.id
                              join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                              where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                > Convert.ToDateTime(c.due_date).Date)//delay
                              && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                              && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                              && i2.process_status != REJECT_CASE
                              && i2.process_status != EXEMPTION
                              && ju.status == "A"
                              group c by new
                              {
                                  c.id,
                                  i2.incident_date
                              } into g
                              select new
                              {
                                  g.Key.id,
                                  g.Key.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_delay_department = cc2.Count() + cc2_pre.Count() + cc2_con.Count();

                    var result2 = new
                    {
                        label = label_department,
                        onprocess = count_onprocess_department,
                        close = count_close_department,
                        delay = count_delay_department,
                        area_id = ds.Tables[0].Rows[i]["department_id"]
                    };


                    dataJson.Add(result2);
                }


            }
            else if (type == "department")
            {

                var de = from d in dbConnect.departments
                         where d.department_id == department_id
                         select d;
                foreach (var u in de)
                {
                    label_all = chageDataLanguage(u.department_th, u.department_en, lang);
                }



                var n_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                              && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                            && i.department_id == department_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_con = n_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_con = n_con.Where(c => c.incident_date <= d_end);
                }




                var n_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                              && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                            && i.department_id == department_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n_pre = n_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n_pre = n_pre.Where(c => c.incident_date <= d_end);
                }




                var n = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                          && (c.action_status_id == ONPROCESS//1 is on process
                        || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && i.department_id == department_id
                        && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                        group c by new
                        {
                            c.id,
                            i.incident_date
                        } into g
                        select new
                        {
                            g.Key.id,
                            g.Key.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count() + n_pre.Count() + n_con.Count();



                var v_con = from c in dbConnect.consequence_management_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.department_id == department_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_con = v_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_con = v_con.Where(c => c.incident_date <= d_end);
                }


                var v_pre = from c in dbConnect.preventive_action_incidents
                            join i in dbConnect.incidents on c.incident_id equals i.id
                            join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                            where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                            && c.action_status_id == CLOSE//1 is on process
                            && i.department_id == department_id
                            && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                            group c by new
                            {
                                c.id,
                                i.incident_date
                            } into g
                            select new
                            {
                                g.Key.id,
                                g.Key.incident_date
                            };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v_pre = v_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v_pre = v_pre.Where(c => c.incident_date <= d_end);
                }



                var v = from c in dbConnect.corrective_prevention_action_incidents
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && c.action_status_id == CLOSE//1 is on process
                        && i.department_id == department_id
                        && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                        group c by new
                        {
                            c.id,
                            i.incident_date
                        } into g
                        select new
                        {
                            g.Key.id,
                            g.Key.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count() + v_pre.Count() + v_con.Count();


                var cc_con = from c in dbConnect.consequence_management_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                              && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                            (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            > Convert.ToDateTime(c.due_date).Date)//delay
                             && i.department_id == department_id
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_con = cc_con.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_con = cc_con.Where(c => c.incident_date <= d_end);
                }


                var cc_pre = from c in dbConnect.preventive_action_incidents
                             join i in dbConnect.incidents on c.incident_id equals i.id
                             join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                             where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                              && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                            (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            > Convert.ToDateTime(c.due_date).Date)//delay
                             && i.department_id == department_id
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i.process_status != REJECT_CASE
                            && i.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc_pre = cc_pre.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc_pre = cc_pre.Where(c => c.incident_date <= d_end);
                }


                var cc = from c in dbConnect.corrective_prevention_action_incidents
                         join i in dbConnect.incidents on c.incident_id equals i.id
                         join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                         where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                          && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                        (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        > Convert.ToDateTime(c.due_date).Date)//delay
                         && i.department_id == department_id
                         && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                        && i.process_status != REJECT_CASE
                        && i.process_status != EXEMPTION
                        && ju.status == "A"
                         group c by new
                         {
                             c.id,
                             i.incident_date
                         } into g
                         select new
                         {
                             g.Key.id,
                             g.Key.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_delay_all = cc.Count() + cc_pre.Count() + cc_con.Count();

                var result = new
                {
                    label = label_all,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select n.division_id,d.division_th,d.division_en,n.count_select from (select i.division_id,ISNULL(count(i.department_id),0) as count_select from incident i ";
                sql = sql + " where 1=1 and i.department_id='" + department_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }
                sql = sql + "group by i.division_id ) as n ";
                sql = sql + "join division d on d.division_id = n.division_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_division = chageDataLanguage(ds.Tables[0].Rows[i]["division_th"].ToString(), ds.Tables[0].Rows[i]["division_en"].ToString(), lang);



                    var n2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && (c.action_status_id == ONPROCESS//1 is on process
                                || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                                 && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_con = n2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_con = n2_con.Where(c => c.incident_date <= d_end);
                    }



                    var n2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && (c.action_status_id == ONPROCESS//1 is on process
                                || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                                 && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                && i2.process_status != REJECT_CASE
                                && i2.process_status != EXEMPTION
                                && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2_pre = n2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2_pre = n2_pre.Where(c => c.incident_date <= d_end);
                    }

                    
                    
                    
                    
                    
                    var n2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && (c.action_status_id == ONPROCESS//1 is on process
                            || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                            && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                            <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                             && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                            && i2.process_status != REJECT_CASE
                            && i2.process_status != EXEMPTION
                            && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i2.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_department = n2.Count() + n2_pre.Count() + n2_con.Count();



                    var v2_con = from c in dbConnect.consequence_management_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_con = v2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_con = v2_con.Where(c => c.incident_date <= d_end);
                    }



                    var v2_pre = from c in dbConnect.preventive_action_incidents
                                 join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                 join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                 where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                 && c.action_status_id == CLOSE//1 is on process
                                 && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 && ju.status == "A"
                                 group c by new
                                 {
                                     c.id,
                                     i2.incident_date
                                 } into g
                                 select new
                                 {
                                     g.Key.id,
                                     g.Key.incident_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2_pre = v2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2_pre = v2_pre.Where(c => c.incident_date <= d_end);
                    }



                    var v2 = from c in dbConnect.corrective_prevention_action_incidents
                             join i2 in dbConnect.incidents on c.incident_id equals i2.id
                             join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                             where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                             && c.action_status_id == CLOSE//1 is on process
                             && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                             && i2.process_status != REJECT_CASE
                             && i2.process_status != EXEMPTION
                             && ju.status == "A"
                             group c by new
                             {
                                 c.id,
                                 i2.incident_date
                             } into g
                             select new
                             {
                                 g.Key.id,
                                 g.Key.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_department = v2.Count() + v2_pre.Count() + v2_con.Count();




                    var cc2_con = from c in dbConnect.consequence_management_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                    (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    > Convert.ToDateTime(c.due_date).Date)//delay
                                  && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                  && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 && ju.status == "A"
                                  group c by new
                                  {
                                      c.id,
                                      i2.incident_date
                                  } into g
                                  select new
                                  {
                                      g.Key.id,
                                      g.Key.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_con = cc2_con.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_con = cc2_con.Where(c => c.incident_date <= d_end);
                    }


                    var cc2_pre = from c in dbConnect.preventive_action_incidents
                                  join i2 in dbConnect.incidents on c.incident_id equals i2.id
                                  join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                                  where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                                   && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                    (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                    > Convert.ToDateTime(c.due_date).Date)//delay
                                  && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                  && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                                 && i2.process_status != REJECT_CASE
                                 && i2.process_status != EXEMPTION
                                 && ju.status == "A"
                                  group c by new
                                  {
                                      c.id,
                                      i2.incident_date
                                  } into g
                                  select new
                                  {
                                      g.Key.id,
                                      g.Key.incident_date
                                  };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2_pre = cc2_pre.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2_pre = cc2_pre.Where(c => c.incident_date <= d_end);
                    }




                    var cc2 = from c in dbConnect.corrective_prevention_action_incidents
                              join i2 in dbConnect.incidents on c.incident_id equals i2.id
                              join ju in dbConnect.injury_persons on i2.id equals ju.incident_id
                              where i2.work_relate == "Y" && (i2.culpability == "G" || i2.culpability == "P")
                               && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                > Convert.ToDateTime(c.due_date).Date)//delay
                              && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                              && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                                || ju.severity_injury_id == MTI || ju.severity_injury_id == MI || ju.severity_injury_id == RWC)
                             && i2.process_status != REJECT_CASE
                             && i2.process_status != EXEMPTION
                             && ju.status == "A"
                              group c by new
                              {
                                  c.id,
                                  i2.incident_date
                              } into g
                              select new
                              {
                                  g.Key.id,
                                  g.Key.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_delay_department = cc2.Count() + cc2_pre.Count() + cc2_con.Count();

                    var result2 = new
                    {
                        label = label_division,
                        onprocess = count_onprocess_department,
                        close = count_close_department,
                        delay = count_delay_department,
                        area_id = ds.Tables[0].Rows[i]["division_id"]
                    };


                    dataJson.Add(result2);


                }

            }
            else if (type == "division")
            {
                string department_id2 = "";
                string function_id2 = "";
                string company_id2 = "";

                var divisions2 = from f in dbConnect.divisions
                                 where f.division_id == division_id
                                 select f;

                foreach (var di in divisions2)
                {
                    department_id2 = di.department_id;

                    var departments2 = from f in dbConnect.departments
                                       where f.department_id == department_id2
                                       select f;

                    foreach (var de in departments2)
                    {
                        function_id2 = de.function_id;


                        var functions2 = from f in dbConnect.functions
                                         where f.function_id == function_id2
                                         select f;

                        foreach (var fun in functions2)
                        {
                            company_id2 = fun.company_id;

                        }

                    }
                }

                redirect = "allincident.aspx?company_id=" + company_id2 + "&function_id=" + function_id2 + "&department_id=" + department_id2 + "&division_id=" + division_id;





            }

            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }























        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardHazard1(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string company_id = "";
            string function_id = "";
            string department_id = "";
            string division_id = "";
            string redirect = "";

            int REJECT = 3;

            if (area_id != "")
            {
                var companys = from c in dbConnect.companies
                               where c.company_id == area_id && c.country == Session["country"].ToString()
                               select c;

                if (companys.Count() > 0)
                {
                    type = "company";
                    company_id = area_id;

                }


                var functions = from f in dbConnect.functions
                                where f.function_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (functions.Count() > 0)
                {
                    type = "function";
                    function_id = area_id;
                }


                var departments = from f in dbConnect.departments
                                  where f.department_id == area_id && f.country == Session["country"].ToString()
                                  select f;
                if (departments.Count() > 0)
                {
                    type = "department";
                    department_id = area_id;
                }


                var divisions = from f in dbConnect.divisions
                                where f.division_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (divisions.Count() > 0)
                {
                    type = "division";
                    division_id = area_id;
                }

            }
            else
            {
                type = "all";

            }



            ArrayList dataJson = new ArrayList();



            string label_all = "";
            if (type == "all")
            {

                var n = from c in dbConnect.hazards
                        where c.country == Session["country"].ToString()
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_all = n.Count();
                label_all = chageDataLanguage("INSEE Group", "INSEE Group", lang);



                var v = from c in dbConnect.hazards
                        where c.country == Session["country"].ToString()
                        && c.process_status != REJECT
                        select new
                        {
                            c.id,
                            c.hazard_date

                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.hazard_date <= d_end);
                }


                int count_hazard_condition = v.Count();

             

                var result = new
                {
                    label = label_all,
                    count = count_hazard_all,
                    condition = count_hazard_condition,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select c.company_id,c.company_th,c.company_en,ISNULL(n.count_select,0) as count_select from (select i.company_id,ISNULL(count(i.company_id),0) as count_select from hazard i ";
                sql = sql + " where 1=1 ";
                // sql = sql + "where 1=1 and i.country ='" + Session["country"].ToString() + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + " group by i.company_id) as n ";
                sql = sql + "right join company c on c.company_id = n.company_id";
                sql = sql + " where c.country ='" + Session["country"].ToString() + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_company = chageDataLanguage(ds.Tables[0].Rows[i]["company_th"].ToString(), ds.Tables[0].Rows[i]["company_en"].ToString(), lang);
                   
                    var n1 = from c in dbConnect.hazards
                             where c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                            select new
                            {
                                c.id,
                                c.hazard_date
                            };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n1 = n1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n1 = n1.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_company = n1.Count();


                    var n2 = from c in dbConnect.hazards
                             where c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && c.process_status != REJECT
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_condition_company = n2.Count();

                    var result2 = new
                    {
                        label = label_company,
                        count = count_hazard_company,
                        condition = count_hazard_condition_company,
                        area_id = ds.Tables[0].Rows[i]["company_id"]
                    };


                    dataJson.Add(result2);

                }




            }
            else if (type == "company")
            {
                var cu = from c in dbConnect.companies
                         where c.company_id == company_id
                         select c;
                foreach (var u in cu)
                {
                    label_all = chageDataLanguage(u.company_th, u.company_en, lang);
                }



                var n = from c in dbConnect.hazards
                        where c.company_id ==company_id
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_all = n.Count();



                var nc = from c in dbConnect.hazards
                        where c.company_id == company_id
                        && c.process_status != REJECT
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    nc = nc.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    nc = nc.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_condition = nc.Count();


                var result3 = new
                {
                    label = label_all,
                    count = count_hazard_all,
                    condition = count_hazard_condition,
                    area_id = ""
                };


                dataJson.Add(result3);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////

                DataSet ds = new DataSet();
                string sql = "select f.function_id,f.function_th,f.function_en,ISNULL(n.count_select,0) as count_select from (select i.function_id,ISNULL(count(i.function_id),0) as count_select from hazard i ";
                sql = sql + "where 1=1 and i.company_id='" + company_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + " group by i.function_id) as n ";
                sql = sql + "right join [function] f on f.function_id = n.function_id where  f.function_en !='-' and f.company_id='" + company_id + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_function = chageDataLanguage(ds.Tables[0].Rows[i]["function_th"].ToString(), ds.Tables[0].Rows[i]["function_en"].ToString(), lang);

                    var n1 = from c in dbConnect.hazards
                             where c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n1 = n1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n1 = n1.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_function = n1.Count();


                    var n2 = from c in dbConnect.hazards
                             where c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && c.process_status != REJECT
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_function_condition = n2.Count();

                    var result2 = new
                    {
                        label = label_function,
                        count = count_hazard_function,
                        condition = count_hazard_function_condition,
                        area_id = ds.Tables[0].Rows[i]["function_id"]
                    };


                    dataJson.Add(result2);

                }





            }
            else if (type == "function")
            {

                var fu = from f in dbConnect.functions
                         where f.function_id == function_id
                         select f;
                foreach (var u in fu)
                {
                    label_all = chageDataLanguage(u.function_th, u.function_en, lang);
                }


                var n = from c in dbConnect.hazards
                        where c.function_id == function_id
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_all = n.Count();



                var nc = from c in dbConnect.hazards
                        where c.function_id == function_id
                        && c.process_status != REJECT
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    nc = nc.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    nc = nc.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_condition = nc.Count();


                var result3 = new
                {
                    label = label_all,
                    count = count_hazard_all,
                    condition = count_hazard_condition,
                    area_id = ""
                };


                dataJson.Add(result3);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select n.department_id,d.department_th,d.department_en,n.count_select from (select i.department_id,ISNULL(count(i.department_id),0) as count_select from hazard i ";
                sql = sql + "where 1=1  and i.function_id='" + function_id + "' ";


                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + "group by i.department_id) as n ";
                sql = sql + "join department d on d.department_id = n.department_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_department = chageDataLanguage(ds.Tables[0].Rows[i]["department_th"].ToString(), ds.Tables[0].Rows[i]["department_en"].ToString(), lang);

                    var n1 = from c in dbConnect.hazards
                             where c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n1 = n1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n1 = n1.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_function = n1.Count();


                    var n2 = from c in dbConnect.hazards
                             where c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && c.process_status != REJECT
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_function_condition = n2.Count();

                    var result2 = new
                    {
                        label = label_department,
                        count = count_hazard_function,
                        condition = count_hazard_function_condition,
                        area_id = ds.Tables[0].Rows[i]["department_id"]
                    };


                    dataJson.Add(result2);
                }


            }
            else if (type == "department")
            {

                var de = from d in dbConnect.departments
                         where d.department_id == department_id
                         select d;
                foreach (var u in de)
                {
                    label_all = chageDataLanguage(u.department_th, u.department_en, lang);
                }




                var n = from c in dbConnect.hazards
                        where c.department_id == department_id
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_all = n.Count();



                var nc = from c in dbConnect.hazards
                        where c.department_id == department_id
                        && c.process_status != REJECT
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    nc = nc.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    nc = nc.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_condition = nc.Count();


                var result5 = new
                {
                    label = label_all,
                    count = count_hazard_all,
                    condition = count_hazard_condition,
                    area_id = ""
                };


                dataJson.Add(result5);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select n.division_id,d.division_th,d.division_en,n.count_select from (select i.division_id,ISNULL(count(i.department_id),0) as count_select from hazard i ";
                sql = sql + " where 1=1 and i.department_id='" + department_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }
                sql = sql + "group by i.division_id ) as n ";
                sql = sql + "join division d on d.division_id = n.division_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_division = chageDataLanguage(ds.Tables[0].Rows[i]["division_th"].ToString(), ds.Tables[0].Rows[i]["division_en"].ToString(), lang);

                    var n1 = from c in dbConnect.hazards
                             where c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n1 = n1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n1 = n1.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_department = n1.Count();


                    var n2 = from c in dbConnect.hazards
                             where c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             && c.process_status != REJECT
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_department_condition = n2.Count();

                    var result2 = new
                    {
                        label = label_division,
                        count = count_hazard_department,
                        condition = count_hazard_department_condition,
                        area_id = ds.Tables[0].Rows[i]["division_id"]
                    };


                    dataJson.Add(result2);

                }

            }
            else if (type == "division")
            {
                string department_id2 = "";
                string function_id2 = "";
                string company_id2 = "";

                var divisions2 = from f in dbConnect.divisions
                                 where f.division_id == division_id
                                 select f;

                foreach (var di in divisions2)
                {
                    department_id2 = di.department_id;

                    var departments2 = from f in dbConnect.departments
                                       where f.department_id == department_id2
                                       select f;

                    foreach (var de in departments2)
                    {
                        function_id2 = de.function_id;


                        var functions2 = from f in dbConnect.functions
                                         where f.function_id == function_id2
                                         select f;

                        foreach (var fun in functions2)
                        {
                            company_id2 = fun.company_id;

                        }

                    }
                }

                redirect = "allhazard.aspx?company_id=" + company_id2 + "&function_id=" + function_id2 + "&department_id=" + department_id2 + "&division_id=" + division_id;





            }

            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }








        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardHazard2(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string company_id = "";
            string function_id = "";
            string department_id = "";
            string division_id = "";
            string redirect = "";

            int REJECT = 3;

            if (area_id != "")
            {
                var companys = from c in dbConnect.companies
                               where c.company_id == area_id && c.country == Session["country"].ToString()
                               select c;

                if (companys.Count() > 0)
                {
                    type = "company";
                    company_id = area_id;

                }


                var functions = from f in dbConnect.functions
                                where f.function_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (functions.Count() > 0)
                {
                    type = "function";
                    function_id = area_id;
                }


                var departments = from f in dbConnect.departments
                                  where f.department_id == area_id && f.country == Session["country"].ToString()
                                  select f;
                if (departments.Count() > 0)
                {
                    type = "department";
                    department_id = area_id;
                }


                var divisions = from f in dbConnect.divisions
                                where f.division_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (divisions.Count() > 0)
                {
                    type = "division";
                    division_id = area_id;
                }

            }
            else
            {
                type = "all";

            }



            ArrayList dataJson = new ArrayList();



            string label_all = "";
            if (type == "all")
            {

                var n = from c in dbConnect.hazards
                        where (c.level_hazard == "H" || c.level_hazard == "M")
                        && c.country == Session["country"].ToString()
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_all = n.Count();
                label_all = chageDataLanguage("INSEE Group", "INSEE Group", lang);


                var result = new
                {
                    label = label_all,
                    count = count_hazard_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select c.company_id,c.company_th,c.company_en,ISNULL(n.count_select,0) as count_select from (select i.company_id,ISNULL(count(i.company_id),0) as count_select from hazard i ";
                sql = sql + " where 1=1 ";
                // sql = sql + "where 1=1 and i.country ='" + Session["country"].ToString() + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + " group by i.company_id) as n ";
                sql = sql + "right join company c on c.company_id = n.company_id";
                sql = sql + " where c.country ='" + Session["country"].ToString() + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_company = chageDataLanguage(ds.Tables[0].Rows[i]["company_th"].ToString(), ds.Tables[0].Rows[i]["company_en"].ToString(), lang);

                    var n1 = from c in dbConnect.hazards
                             where c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && (c.level_hazard == "H" || c.level_hazard == "M")
                             && c.process_status != REJECT
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n1 = n1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n1 = n1.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_company = n1.Count();

                    var result2 = new
                    {
                        label = label_company,
                        count = count_hazard_company,
                        area_id = ds.Tables[0].Rows[i]["company_id"]
                    };


                    dataJson.Add(result2);

                }




            }
            else if (type == "company")
            {
                var cu = from c in dbConnect.companies
                         where c.company_id == company_id
                         select c;
                foreach (var u in cu)
                {
                    label_all = chageDataLanguage(u.company_th, u.company_en, lang);
                }



                var n = from c in dbConnect.hazards
                        where c.company_id == company_id
                        && (c.level_hazard == "H" || c.level_hazard == "M")
                        && c.process_status != REJECT
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_all = n.Count();


                var result3 = new
                {
                    label = label_all,
                    count = count_hazard_all,
                    area_id = ""
                };


                dataJson.Add(result3);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////


                DataSet ds = new DataSet();
                string sql = "select f.function_id,f.function_th,f.function_en,ISNULL(n.count_select,0) as count_select from (select i.function_id,ISNULL(count(i.function_id),0) as count_select from hazard i ";
                sql = sql + "where 1=1 and i.company_id='" + company_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + " group by i.function_id) as n ";
                sql = sql + "right join [function] f on f.function_id = n.function_id where  f.function_en !='-' and f.company_id='" + company_id + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_function = chageDataLanguage(ds.Tables[0].Rows[i]["function_th"].ToString(), ds.Tables[0].Rows[i]["function_en"].ToString(), lang);

                    var n1 = from c in dbConnect.hazards
                             where c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && (c.level_hazard == "H" || c.level_hazard == "M")
                             && c.process_status != REJECT
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n1 = n1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n1 = n1.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_function = n1.Count();

                    var result2 = new
                    {
                        label = label_function,
                        count = count_hazard_function,
                        area_id = ds.Tables[0].Rows[i]["function_id"]
                    };


                    dataJson.Add(result2);

                }



            }
            else if (type == "function")
            {

                var fu = from f in dbConnect.functions
                         where f.function_id == function_id
                         select f;
                foreach (var u in fu)
                {
                    label_all = chageDataLanguage(u.function_th, u.function_en, lang);
                }


                var n = from c in dbConnect.hazards
                        where c.function_id == function_id
                        && (c.level_hazard == "H" || c.level_hazard == "M")
                        && c.process_status != REJECT
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_all = n.Count();


                var result3 = new
                {
                    label = label_all,
                    count = count_hazard_all,
                    area_id = ""
                };


                dataJson.Add(result3);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select n.department_id,d.department_th,d.department_en,n.count_select from (select i.department_id,ISNULL(count(i.department_id),0) as count_select from hazard i ";
                sql = sql + "where 1=1  and i.function_id='" + function_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + "group by i.department_id) as n ";
                sql = sql + "join department d on d.department_id = n.department_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_department = chageDataLanguage(ds.Tables[0].Rows[i]["department_th"].ToString(), ds.Tables[0].Rows[i]["department_en"].ToString(), lang);

                    var n1 = from c in dbConnect.hazards
                             where c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && (c.level_hazard == "H" || c.level_hazard == "M")
                             && c.process_status != REJECT
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n1 = n1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n1 = n1.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_function = n1.Count();

                    var result2 = new
                    {
                        label = label_department,
                        count = count_hazard_function,
                        area_id = ds.Tables[0].Rows[i]["department_id"]
                    };


                    dataJson.Add(result2);
                }


            }
            else if (type == "department")
            {

                var de = from d in dbConnect.departments
                         where d.department_id == department_id
                         select d;
                foreach (var u in de)
                {
                    label_all = chageDataLanguage(u.department_th, u.department_en, lang);
                }




                var n = from c in dbConnect.hazards
                        where c.department_id == department_id
                        && (c.level_hazard == "H" || c.level_hazard == "M")
                        && c.process_status != REJECT
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_hazard_all = n.Count();


                var result5 = new
                {
                    label = label_all,
                    count = count_hazard_all,
                    area_id = ""
                };


                dataJson.Add(result5);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select n.division_id,d.division_th,d.division_en,n.count_select from (select i.division_id,ISNULL(count(i.department_id),0) as count_select from hazard i ";
                sql = sql + " where 1=1 and i.department_id='" + department_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }
                sql = sql + "group by i.division_id ) as n ";
                sql = sql + "join division d on d.division_id = n.division_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_division = chageDataLanguage(ds.Tables[0].Rows[i]["division_th"].ToString(), ds.Tables[0].Rows[i]["division_en"].ToString(), lang);

                    var n1 = from c in dbConnect.hazards
                             where c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             && (c.level_hazard == "H" || c.level_hazard == "M")
                             && c.process_status != REJECT
                             select new
                             {
                                 c.id,
                                 c.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n1 = n1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n1 = n1.Where(c => c.hazard_date <= d_end);
                    }

                    int count_hazard_department = n1.Count();

                    var result2 = new
                    {
                        label = label_division,
                        count = count_hazard_department,
                        area_id = ds.Tables[0].Rows[i]["division_id"]
                    };


                    dataJson.Add(result2);

                }

            }
            else if (type == "division")
            {
                string department_id2 = "";
                string function_id2 = "";
                string company_id2 = "";

                var divisions2 = from f in dbConnect.divisions
                                 where f.division_id == division_id
                                 select f;

                foreach (var di in divisions2)
                {
                    department_id2 = di.department_id;

                    var departments2 = from f in dbConnect.departments
                                       where f.department_id == department_id2
                                       select f;

                    foreach (var de in departments2)
                    {
                        function_id2 = de.function_id;


                        var functions2 = from f in dbConnect.functions
                                         where f.function_id == function_id2
                                         select f;

                        foreach (var fun in functions2)
                        {
                            company_id2 = fun.company_id;

                        }

                    }
                }

                redirect = "allhazard.aspx?company_id=" + company_id2 + "&function_id=" + function_id2 + "&department_id=" + department_id2 + "&division_id=" + division_id;





            }

            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }






        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardHazard3(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            int ONPROCESS = 1;
            int REJECT = 6;
            int REQUEST_CLOSE = 2;
            int CLOSE = 4;
            int CANCEL = 5;

            int REJECT_CASE = 3;

            string type = "";
            string company_id = "";
            string function_id = "";
            string department_id = "";
            string division_id = "";
            string redirect = "";

            if (area_id != "")
            {

                var companys = from c in dbConnect.companies
                               where c.company_id == area_id && c.country == Session["country"].ToString()
                               select c;

                if (companys.Count() > 0)
                {
                    type = "company";
                    company_id = area_id;

                }


                var functions = from f in dbConnect.functions
                                where f.function_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (functions.Count() > 0)
                {
                    type = "function";
                    function_id = area_id;
                }


                var departments = from f in dbConnect.departments
                                  where f.department_id == area_id && f.country == Session["country"].ToString()
                                  select f;
                if (departments.Count() > 0)
                {
                    type = "department";
                    department_id = area_id;
                }


                var divisions = from f in dbConnect.divisions
                                where f.division_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (divisions.Count() > 0)
                {
                    type = "division";
                    division_id = area_id;
                }

            }
            else
            {
                type = "all";

            }



            ArrayList dataJson = new ArrayList();



            string label_all = "";
            if (type == "all")
            {

                var n = from c in dbConnect.process_actions
                        join i in dbConnect.hazards on c.hazard_id equals i.id
                        where (c.action_status_id == ONPROCESS//1 is on process
                       || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date) 
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && i.country == Session["country"].ToString()
                        && i.process_status != REJECT_CASE
                        select new
                        {
                            c.id,
                            i.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count();
                label_all = chageDataLanguage("INSEE Group", "INSEE Group", lang);


                var v = from c in dbConnect.process_actions
                        join i in dbConnect.hazards on c.hazard_id equals i.id
                        where c.action_status_id == CLOSE//1 is on process
                        && i.country == Session["country"].ToString()
                        && i.process_status != REJECT_CASE
                        select new
                        {
                            c.id,
                            i.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.hazard_date <= d_end);
                }


                int count_incident_close_all = v.Count();


                var cc = from c in dbConnect.process_actions
                         join i in dbConnect.hazards on c.hazard_id equals i.id
                         where ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                        (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        > Convert.ToDateTime(c.due_date).Date)//delay
                        && i.country == Session["country"].ToString()
                        && i.process_status != REJECT_CASE
                         select new
                         {
                             c.id,
                             i.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.hazard_date <= d_end);
                }


                int count_incident_delay_all = cc.Count();

                var result = new
                {
                    label = label_all,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select c.company_id,c.company_th,c.company_en,ISNULL(n.count_select,0) as count_select from (select i.company_id,ISNULL(count(i.company_id),0) as count_select from hazard i ";
                sql = sql + " where 1=1 ";
                // sql = sql + "where 1=1 and i.country ='" + Session["country"].ToString() + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + " group by i.company_id) as n ";
                sql = sql + "right join company c on c.company_id = n.company_id";
                sql = sql + " where c.country ='" + Session["country"].ToString() + "' ";


                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_company = chageDataLanguage(ds.Tables[0].Rows[i]["company_th"].ToString(), ds.Tables[0].Rows[i]["company_en"].ToString(), lang);


                    var n2 = from c in dbConnect.process_actions
                             join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                             where (c.action_status_id == ONPROCESS//1 is on process
                               || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                             && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             select new
                             {
                                 c.id,
                                 i2.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.hazard_date <= d_end);
                    }

                    int count_onprocess_company = n2.Count();



                    var v2 = from c in dbConnect.process_actions
                             join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                             where c.action_status_id == CLOSE//1 is on process
                             && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             select new
                             {
                                 c.id,
                                 i2.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.hazard_date <= d_end);
                    }


                    int count_close_company = v2.Count();


                    var cc2 = from c in dbConnect.process_actions
                              join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                              where ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                > Convert.ToDateTime(c.due_date).Date)//delay
                              && i2.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                              && i2.process_status != REJECT_CASE
                              select new
                              {
                                  c.id,
                                  i2.hazard_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.hazard_date <= d_end);
                    }


                    int count_delay_company = cc2.Count();

                    var result2 = new
                    {
                        label = label_company,
                        onprocess = count_onprocess_company,
                        close = count_close_company,
                        delay = count_delay_company,
                        area_id = ds.Tables[0].Rows[i]["company_id"]
                    };


                    dataJson.Add(result2);

                }




            }
            else if (type == "company")
            {
                string label_company = "";
                var cu = from c in dbConnect.companies
                         where c.company_id == company_id
                         select c;

                foreach (var c in cu)
                {
                    label_company = chageDataLanguage(c.company_th, c.company_en, lang);
                }


                var n = from c in dbConnect.process_actions
                        join i in dbConnect.hazards on c.hazard_id equals i.id
                        where (c.action_status_id == ONPROCESS//1 is on process
                       || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && i.company_id == company_id
                        && i.process_status != REJECT_CASE
                        select new
                        {
                            c.id,
                            i.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count();


                var v = from c in dbConnect.process_actions
                        join i in dbConnect.hazards on c.hazard_id equals i.id
                        where c.action_status_id == CLOSE//1 is on process
                        && i.company_id == company_id
                        && i.process_status != REJECT_CASE
                        select new
                        {
                            c.id,
                            i.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.hazard_date <= d_end);
                }


                int count_incident_close_all = v.Count();


                var cc = from c in dbConnect.process_actions
                         join i in dbConnect.hazards on c.hazard_id equals i.id
                         where ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                        (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        > Convert.ToDateTime(c.due_date).Date)//delay
                         && i.company_id == company_id
                         && i.process_status != REJECT_CASE
                         select new
                         {
                             c.id,
                             i.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.hazard_date <= d_end);
                }


                int count_incident_delay_all = cc.Count();

                var result = new
                {
                    label = label_company,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select f.function_id,f.function_th,f.function_en,ISNULL(n.count_select,0) as count_select from (select i.function_id,ISNULL(count(i.function_id),0) as count_select from hazard i ";
                sql = sql + "where 1=1 and i.company_id='" + company_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + " group by i.function_id) as n ";
                sql = sql + "right join [function] f on f.function_id = n.function_id where  f.function_en !='-' and f.company_id='" + company_id + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_function = chageDataLanguage(ds.Tables[0].Rows[i]["function_th"].ToString(), ds.Tables[0].Rows[i]["function_en"].ToString(), lang);


                    var n2 = from c in dbConnect.process_actions
                             join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                             where (c.action_status_id == ONPROCESS//1 is on process
                               || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                             && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             select new
                             {
                                 c.id,
                                 i2.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.hazard_date <= d_end);
                    }

                    int count_onprocess_function = n2.Count();



                    var v2 = from c in dbConnect.process_actions
                             join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                             where c.action_status_id == CLOSE//1 is on process
                             && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             select new
                             {
                                 c.id,
                                 i2.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.hazard_date <= d_end);
                    }


                    int count_close_function = v2.Count();


                    var cc2 = from c in dbConnect.process_actions
                              join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                              where ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                > Convert.ToDateTime(c.due_date).Date)//delay
                              && i2.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                              && i2.process_status != REJECT_CASE
                              select new
                              {
                                  c.id,
                                  i2.hazard_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.hazard_date <= d_end);
                    }


                    int count_delay_function = cc2.Count();

                    var result2 = new
                    {
                        label = label_function,
                        onprocess = count_onprocess_function,
                        close = count_close_function,
                        delay = count_delay_function,
                        area_id = ds.Tables[0].Rows[i]["function_id"]
                    };


                    dataJson.Add(result2);

                }

            }
            else if (type == "function")
            {
                string label_function = "";
                var fu = from c in dbConnect.functions
                         where c.function_id == function_id
                         select c;

                foreach (var f in fu)
                {
                    label_function = chageDataLanguage(f.function_th, f.function_en, lang);
                }


                var n = from c in dbConnect.process_actions
                        join i in dbConnect.hazards on c.hazard_id equals i.id
                        where (c.action_status_id == ONPROCESS//1 is on process
                       || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && i.function_id == function_id
                        && i.process_status != REJECT_CASE
                        select new
                        {
                            c.id,
                            i.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count();


                var v = from c in dbConnect.process_actions
                        join i in dbConnect.hazards on c.hazard_id equals i.id
                        where c.action_status_id == CLOSE//1 is on process
                        && i.function_id == function_id
                        && i.process_status != REJECT_CASE
                        select new
                        {
                            c.id,
                            i.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.hazard_date <= d_end);
                }


                int count_incident_close_all = v.Count();


                var cc = from c in dbConnect.process_actions
                         join i in dbConnect.hazards on c.hazard_id equals i.id
                         where ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                        (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        > Convert.ToDateTime(c.due_date).Date)//delay
                         && i.function_id == function_id
                         && i.process_status != REJECT_CASE
                         select new
                         {
                             c.id,
                             i.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.hazard_date <= d_end);
                }


                int count_incident_delay_all = cc.Count();

                var result = new
                {
                    label = label_function,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select n.department_id,d.department_th,d.department_en,n.count_select from (select i.department_id,ISNULL(count(i.department_id),0) as count_select from hazard i ";
                sql = sql + " where 1=1  and i.function_id='" + function_id + "' ";


                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + "group by i.department_id) as n ";
                sql = sql + "join department d on d.department_id = n.department_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_department = chageDataLanguage(ds.Tables[0].Rows[i]["department_th"].ToString(), ds.Tables[0].Rows[i]["department_en"].ToString(), lang);

                    var n2 = from c in dbConnect.process_actions
                             join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                             where (c.action_status_id == ONPROCESS//1 is on process
                               || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                                && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                             && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             select new
                             {
                                 c.id,
                                 i2.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.hazard_date <= d_end);
                    }

                    int count_onprocess_department = n2.Count();



                    var v2 = from c in dbConnect.process_actions
                             join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                             where c.action_status_id == CLOSE//1 is on process
                             && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             select new
                             {
                                 c.id,
                                 i2.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.hazard_date <= d_end);
                    }


                    int count_close_department = v2.Count();


                    var cc2 = from c in dbConnect.process_actions
                              join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                              where ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                > Convert.ToDateTime(c.due_date).Date)//delay
                              && i2.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                              && i2.process_status != REJECT_CASE
                              select new
                              {
                                  c.id,
                                  i2.hazard_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.hazard_date <= d_end);
                    }


                    int count_delay_department = cc2.Count();

                    var result2 = new
                    {
                        label = label_department,
                        onprocess = count_onprocess_department,
                        close = count_close_department,
                        delay = count_delay_department,
                        area_id = ds.Tables[0].Rows[i]["department_id"]
                    };


                    dataJson.Add(result2);
                }


            }
            else if (type == "department")
            {

                var de = from d in dbConnect.departments
                         where d.department_id == department_id
                         select d;
                foreach (var u in de)
                {
                    label_all = chageDataLanguage(u.department_th, u.department_en, lang);
                }




                var n = from c in dbConnect.process_actions
                        join i in dbConnect.hazards on c.hazard_id equals i.id
                        where (c.action_status_id == ONPROCESS//1 is on process
                       || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && i.department_id == department_id
                        && i.process_status != REJECT_CASE
                        select new
                        {
                            c.id,
                            i.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count();


                var v = from c in dbConnect.process_actions
                        join i in dbConnect.hazards on c.hazard_id equals i.id
                        where c.action_status_id == CLOSE//1 is on process
                        && i.department_id == department_id
                        && i.process_status != REJECT_CASE
                        select new
                        {
                            c.id,
                            i.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.hazard_date <= d_end);
                }


                int count_incident_close_all = v.Count();


                var cc = from c in dbConnect.process_actions
                         join i in dbConnect.hazards on c.hazard_id equals i.id
                         where ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                        (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                        > Convert.ToDateTime(c.due_date).Date)//delay
                         && i.department_id == department_id
                         && i.process_status != REJECT_CASE
                         select new
                         {
                             c.id,
                             i.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.hazard_date <= d_end);
                }


                int count_incident_delay_all = cc.Count();

                var result = new
                {
                    label = label_all,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    delay = count_incident_delay_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select n.division_id,d.division_th,d.division_en,n.count_select from (select i.division_id,ISNULL(count(i.department_id),0) as count_select from hazard i ";
                sql = sql + " where 1=1 and i.department_id='" + department_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + "group by i.division_id ) as n ";
                sql = sql + "join division d on d.division_id = n.division_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_division = chageDataLanguage(ds.Tables[0].Rows[i]["division_th"].ToString(), ds.Tables[0].Rows[i]["division_en"].ToString(), lang);

                    var n2 = from c in dbConnect.process_actions
                             join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                             where (c.action_status_id == ONPROCESS//1 is on process
                             || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                             && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                             <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                             && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             select new
                             {
                                 c.id,
                                 i2.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.hazard_date <= d_end);
                    }

                    int count_onprocess_department = n2.Count();



                    var v2 = from c in dbConnect.process_actions
                             join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                             where c.action_status_id == CLOSE
                             && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             && i2.process_status != REJECT_CASE
                             select new
                             {
                                 c.id,
                                 i2.hazard_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.hazard_date <= d_end);
                    }


                    int count_close_department = v2.Count();


                    var cc2 = from c in dbConnect.process_actions
                              join i2 in dbConnect.hazards on c.hazard_id equals i2.id
                              where ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                                (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Date : Convert.ToDateTime(c.date_complete).Date)
                                > Convert.ToDateTime(c.due_date).Date)//delay
                              && i2.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                              && i2.process_status != REJECT_CASE
                              select new
                              {
                                  c.id,
                                  i2.hazard_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.hazard_date <= d_end);
                    }


                    int count_delay_department = cc2.Count();

                    var result2 = new
                    {
                        label = label_division,
                        onprocess = count_onprocess_department,
                        close = count_close_department,
                        delay = count_delay_department,
                        area_id = ds.Tables[0].Rows[i]["division_id"]
                    };


                    dataJson.Add(result2);


                }

            }
            else if (type == "division")
            {
                string department_id2 = "";
                string function_id2 = "";
                string company_id2 = "";

                var divisions2 = from f in dbConnect.divisions
                                 where f.division_id == division_id
                                 select f;

                foreach (var di in divisions2)
                {
                    department_id2 = di.department_id;

                    var departments2 = from f in dbConnect.departments
                                       where f.department_id == department_id2
                                       select f;

                    foreach (var de in departments2)
                    {
                        function_id2 = de.function_id;


                        var functions2 = from f in dbConnect.functions
                                         where f.function_id == function_id2
                                         select f;

                        foreach (var fun in functions2)
                        {
                            company_id2 = fun.company_id;

                        }

                    }
                }

                redirect = "allhazard.aspx?company_id=" + company_id2 + "&function_id=" + function_id2 + "&department_id=" + department_id2 + "&division_id=" + division_id;





            }

            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }








        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardHazard4(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string company_id = "";
            string function_id = "";
            string department_id = "";
            string division_id = "";
            string redirect = "";

            int vehicle_traffic = 1;
            int fall_gravity = 4;
            int electricity = 8;
            int machine = 2;
            int chemical = 9;
            int fire_heat = 7;
            int slip_fall = 6;
            int animal = 32;//อันเดิมไม่มีเพิ่มให้ไปละ 


            if (Session["country"].ToString() == "thailand")
            {
                vehicle_traffic = 1;
                fall_gravity = 4;
                electricity = 8;
                machine = 2;
                chemical = 9;
                fire_heat = 7;
                slip_fall = 6;
                animal = 32;//อันเดิมไม่มีเพิ่มให้ไปละ 
            }
            else if (Session["country"].ToString() == "srilanka")
            {
                vehicle_traffic = 34;
                fall_gravity = 37;
                electricity = 41;
                machine = 35;
                chemical = 42;
                fire_heat = 40;
                slip_fall = 39;
                animal = 65;//อันเดิมไม่มีเพิ่มให้ไปละ 
            }


            if (area_id != "")
            {

                var companys = from c in dbConnect.companies
                               where c.company_id == area_id && c.country == Session["country"].ToString()
                               select c;

                if (companys.Count() > 0)
                {
                    type = "company";
                    company_id = area_id;

                }


                var functions = from f in dbConnect.functions
                                where f.function_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (functions.Count() > 0)
                {
                    type = "function";
                    function_id = area_id;
                }


                var departments = from f in dbConnect.departments
                                  where f.department_id == area_id && f.country == Session["country"].ToString()
                                  select f;
                if (departments.Count() > 0)
                {
                    type = "department";
                    department_id = area_id;
                }


                var divisions = from f in dbConnect.divisions
                                where f.division_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (divisions.Count() > 0)
                {
                    type = "division";
                    division_id = area_id;
                }

            }
            else
            {
                type = "all";

            }



            ArrayList dataJson = new ArrayList();



            string label_all = "";
            if (type == "all")
            {

                var n = from c in dbConnect.hazards
                        where c.source_hazard == vehicle_traffic
                        && c.country == Session["country"].ToString()
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int vehicle_tracffic_all = n.Count();
                label_all = chageDataLanguage("INSEE Group", "INSEE Group", lang);
                ///////////////////////////////////////////////////1///////////////////////////////////////////////////




                var n2 = from c in dbConnect.hazards
                        where c.source_hazard == electricity
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n2 = n2.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n2 = n2.Where(c => c.hazard_date <= d_end);
                }

                int electricity_all = n2.Count();

                ////////////////////////////////////////////////////2/////////////////////////////////////////////////////


                var n3 = from c in dbConnect.hazards
                         where c.source_hazard == machine
                         && c.country == Session["country"].ToString()
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n3 = n3.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n3 = n3.Where(c => c.hazard_date <= d_end);
                }

                int machine_all = n3.Count();

                ////////////////////////////////////////////////////3/////////////////////////////////////////////////////



                var n4 = from c in dbConnect.hazards
                         where c.source_hazard == chemical
                          && c.country == Session["country"].ToString()
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n4 = n4.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n4 = n4.Where(c => c.hazard_date <= d_end);
                }

                int chemical_all = n4.Count();

                ////////////////////////////////////////////////////4/////////////////////////////////////////////////////


                var n5 = from c in dbConnect.hazards
                         where c.source_hazard == fire_heat
                          && c.country == Session["country"].ToString()
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n5 = n5.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n5 = n5.Where(c => c.hazard_date <= d_end);
                }

                int fire_heat_all = n5.Count();

                ////////////////////////////////////////////////////5/////////////////////////////////////////////////////

                var n6 = from c in dbConnect.hazards
                         where c.source_hazard == slip_fall
                          && c.country == Session["country"].ToString()
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n6 = n6.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n6 = n6.Where(c => c.hazard_date <= d_end);
                }

                int slip_fall_all = n6.Count();

                ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                var n7 = from c in dbConnect.hazards
                         where c.source_hazard == animal
                          && c.country == Session["country"].ToString()
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n7 = n7.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n7 = n7.Where(c => c.hazard_date <= d_end);
                }

                int animal_all = n7.Count();

                ////////////////////////////////////////////////////7/////////////////////////////////////////////////////



                var n8 = from c in dbConnect.hazards
                         where c.source_hazard == fall_gravity
                          && c.country == Session["country"].ToString()
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n8 = n8.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n8 = n8.Where(c => c.hazard_date <= d_end);
                }

                int fall_gravity_all = n8.Count();

                ////////////////////////////////////////////////////8/////////////////////////////////////////////////////


                var result = new
                {
                    label = label_all,
                    vehicle_tracffic = vehicle_tracffic_all,
                    fall_gravity = fall_gravity_all,
                    electricity = electricity_all,
                    machine = machine_all,
                    chemical = chemical_all,
                    fire_heat = fire_heat_all,
                    slip_fall = slip_fall_all,
                    animal = animal_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////





                DataSet ds = new DataSet();
                string sql = "select c.company_id,c.company_th,c.company_en,ISNULL(n.count_select,0) as count_select from (select i.company_id,ISNULL(count(i.company_id),0) as count_select from hazard i ";
                sql = sql + " where 1=1 ";
                // sql = sql + "where 1=1 and i.country ='" + Session["country"].ToString() + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + " group by i.company_id) as n ";
                sql = sql + "right join company c on c.company_id = n.company_id";
                sql = sql + " where c.country ='" + Session["country"].ToString() + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_company = chageDataLanguage(ds.Tables[0].Rows[i]["company_th"].ToString(), ds.Tables[0].Rows[i]["company_en"].ToString(), lang);

                    var n_fun1 = from c in dbConnect.hazards
                                 where c.source_hazard == vehicle_traffic
                                 && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun1 = n_fun1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun1 = n_fun1.Where(c => c.hazard_date <= d_end);
                    }

                    int vehicle_tracffic_company = n_fun1.Count();

                    ///////////////////////////////////////////////////1///////////////////////////////////////////////////




                    var n_fun2 = from c in dbConnect.hazards
                                 where c.source_hazard == electricity
                                  && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun2 = n_fun2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun2 = n_fun2.Where(c => c.hazard_date <= d_end);
                    }

                    int electricity_company = n_fun2.Count();

                    ////////////////////////////////////////////////////2/////////////////////////////////////////////////////


                    var n_fun3 = from c in dbConnect.hazards
                                 where c.source_hazard == machine
                                  && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun3 = n_fun3.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun3 = n_fun3.Where(c => c.hazard_date <= d_end);
                    }

                    int machine_company = n_fun3.Count();

                    ////////////////////////////////////////////////////3/////////////////////////////////////////////////////



                    var n_fun4 = from c in dbConnect.hazards
                                 where c.source_hazard == chemical
                                  && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun4 = n_fun4.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun4 = n_fun4.Where(c => c.hazard_date <= d_end);
                    }

                    int chemical_company = n_fun4.Count();

                    ////////////////////////////////////////////////////4/////////////////////////////////////////////////////


                    var n_fun5 = from c in dbConnect.hazards
                                 where c.source_hazard == fire_heat
                                  && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun5 = n_fun5.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun5 = n_fun5.Where(c => c.hazard_date <= d_end);
                    }

                    int fire_heat_company = n_fun5.Count();

                    ////////////////////////////////////////////////////5/////////////////////////////////////////////////////

                    var n_fun6 = from c in dbConnect.hazards
                                 where c.source_hazard == slip_fall
                                  && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun6 = n_fun6.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun6 = n_fun6.Where(c => c.hazard_date <= d_end);
                    }

                    int slip_fall_company = n_fun6.Count();

                    ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                    var n_fun7 = from c in dbConnect.hazards
                                 where c.source_hazard == animal
                                  && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun7 = n_fun7.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun7 = n_fun7.Where(c => c.hazard_date <= d_end);
                    }

                    int animal_company = n_fun7.Count();

                    ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                    var n_fun8 = from c in dbConnect.hazards
                                 where c.source_hazard == fall_gravity
                                  && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun8 = n_fun8.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun8 = n_fun8.Where(c => c.hazard_date <= d_end);
                    }

                    int fall_gravity_company = n_fun8.Count();

                    ////////////////////////////////////////////////////7/////////////////////////////////////////////////////

                    var result2 = new
                    {
                        label = label_company,
                        vehicle_tracffic = vehicle_tracffic_company,
                        fall_gravity = fall_gravity_company,
                        electricity = electricity_company,
                        machine = machine_company,
                        chemical = chemical_company,
                        fire_heat = fire_heat_company,
                        slip_fall = slip_fall_company,
                        animal = animal_company,
                        area_id = ds.Tables[0].Rows[i]["company_id"].ToString()
                    };


                    dataJson.Add(result2);

                }





            }
            else if (type == "company")
            {


                string label_company = "";
                var cu = from c in dbConnect.companies
                         where c.company_id == company_id
                         select c;

                foreach (var c in cu)
                {
                    label_company = chageDataLanguage(c.company_th, c.company_en, lang);
                }



                var n = from c in dbConnect.hazards
                        where c.source_hazard == vehicle_traffic
                        && c.company_id == company_id
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int vehicle_tracffic_all = n.Count();

                ///////////////////////////////////////////////////1///////////////////////////////////////////////////




                var n2 = from c in dbConnect.hazards
                         where c.source_hazard == electricity
                         && c.company_id == company_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n2 = n2.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n2 = n2.Where(c => c.hazard_date <= d_end);
                }

                int electricity_all = n2.Count();

                ////////////////////////////////////////////////////2/////////////////////////////////////////////////////


                var n3 = from c in dbConnect.hazards
                         where c.source_hazard == machine
                         && c.company_id == company_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n3 = n3.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n3 = n3.Where(c => c.hazard_date <= d_end);
                }

                int machine_all = n3.Count();

                ////////////////////////////////////////////////////3/////////////////////////////////////////////////////



                var n4 = from c in dbConnect.hazards
                         where c.source_hazard == chemical
                         && c.company_id == company_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n4 = n4.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n4 = n4.Where(c => c.hazard_date <= d_end);
                }

                int chemical_all = n4.Count();

                ////////////////////////////////////////////////////4/////////////////////////////////////////////////////


                var n5 = from c in dbConnect.hazards
                         where c.source_hazard == fire_heat
                         && c.company_id == company_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n5 = n5.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n5 = n5.Where(c => c.hazard_date <= d_end);
                }

                int fire_heat_all = n5.Count();

                ////////////////////////////////////////////////////5/////////////////////////////////////////////////////

                var n6 = from c in dbConnect.hazards
                         where c.source_hazard == slip_fall
                         && c.company_id == company_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n6 = n6.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n6 = n6.Where(c => c.hazard_date <= d_end);
                }

                int slip_fall_all = n6.Count();

                ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                var n7 = from c in dbConnect.hazards
                         where c.source_hazard == animal
                         && c.company_id == company_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n7 = n7.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n7 = n7.Where(c => c.hazard_date <= d_end);
                }

                int animal_all = n7.Count();

                ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                var n8 = from c in dbConnect.hazards
                         where c.source_hazard == fall_gravity
                          && c.company_id == company_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n8 = n8.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n8 = n8.Where(c => c.hazard_date <= d_end);
                }

                int fall_gravity_all = n8.Count();

                ////////////////////////////////////////////////////8/////////////////////////////////////////////////////


                var result = new
                {
                    label = label_company,
                    vehicle_tracffic = vehicle_tracffic_all,
                    fall_gravity = fall_gravity_all,
                    electricity = electricity_all,
                    machine = machine_all,
                    chemical = chemical_all,
                    fire_heat = fire_heat_all,
                    slip_fall = slip_fall_all,
                    animal = animal_all,
                    area_id = ""
                };


                dataJson.Add(result);


                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select f.function_id,f.function_th,f.function_en,ISNULL(n.count_select,0) as count_select from (select i.function_id,ISNULL(count(i.function_id),0) as count_select from hazard i ";
                sql = sql + " where 1=1 and i.company_id='" + company_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + " group by i.function_id) as n ";
                sql = sql + "right join [function] f on f.function_id = n.function_id where  f.function_en !='-' and f.company_id='" + company_id + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_function = chageDataLanguage(ds.Tables[0].Rows[i]["function_th"].ToString(), ds.Tables[0].Rows[i]["function_en"].ToString(), lang);

                    var n_fun1 = from c in dbConnect.hazards
                                 where c.source_hazard == vehicle_traffic
                                 && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun1 = n_fun1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun1 = n_fun1.Where(c => c.hazard_date <= d_end);
                    }

                    int vehicle_tracffic_func = n_fun1.Count();

                    ///////////////////////////////////////////////////1///////////////////////////////////////////////////




                    var n_fun2 = from c in dbConnect.hazards
                                 where c.source_hazard == electricity
                                  && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun2 = n_fun2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun2 = n_fun2.Where(c => c.hazard_date <= d_end);
                    }

                    int electricity_func = n_fun2.Count();

                    ////////////////////////////////////////////////////2/////////////////////////////////////////////////////


                    var n_fun3 = from c in dbConnect.hazards
                                 where c.source_hazard == machine
                                  && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun3 = n_fun3.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun3 = n_fun3.Where(c => c.hazard_date <= d_end);
                    }

                    int machine_func = n_fun3.Count();

                    ////////////////////////////////////////////////////3/////////////////////////////////////////////////////



                    var n_fun4 = from c in dbConnect.hazards
                                 where c.source_hazard == chemical
                                  && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun4 = n_fun4.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun4 = n_fun4.Where(c => c.hazard_date <= d_end);
                    }

                    int chemical_func = n_fun4.Count();

                    ////////////////////////////////////////////////////4/////////////////////////////////////////////////////


                    var n_fun5 = from c in dbConnect.hazards
                                 where c.source_hazard == fire_heat
                                  && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun5 = n_fun5.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun5 = n_fun5.Where(c => c.hazard_date <= d_end);
                    }

                    int fire_heat_func = n_fun5.Count();

                    ////////////////////////////////////////////////////5/////////////////////////////////////////////////////

                    var n_fun6 = from c in dbConnect.hazards
                                 where c.source_hazard == slip_fall
                                  && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun6 = n_fun6.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun6 = n_fun6.Where(c => c.hazard_date <= d_end);
                    }

                    int slip_fall_func = n_fun6.Count();

                    ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                    var n_fun7 = from c in dbConnect.hazards
                                 where c.source_hazard == animal
                                  && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun7 = n_fun7.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun7 = n_fun7.Where(c => c.hazard_date <= d_end);
                    }

                    int animal_func = n_fun7.Count();

                    ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                    var n_fun8 = from c in dbConnect.hazards
                                 where c.source_hazard == fall_gravity
                                  && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun8 = n_fun8.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun8 = n_fun8.Where(c => c.hazard_date <= d_end);
                    }

                    int fall_gravity_func = n_fun8.Count();

                    ////////////////////////////////////////////////////7/////////////////////////////////////////////////////

                    var result2 = new
                    {
                        label = label_function,
                        vehicle_tracffic = vehicle_tracffic_func,
                        fall_gravity = fall_gravity_func,
                        electricity = electricity_func,
                        machine = machine_func,
                        chemical = chemical_func,
                        fire_heat = fire_heat_func,
                        slip_fall = slip_fall_func,
                        animal = animal_func,
                        area_id = ds.Tables[0].Rows[i]["function_id"].ToString()
                    };


                    dataJson.Add(result2);

                }




            }
            else if (type == "function")
            {

                var fu = from f in dbConnect.functions
                         where f.function_id == function_id
                         select f;
                foreach (var u in fu)
                {
                    label_all = chageDataLanguage(u.function_th, u.function_en, lang);
                }


                var n = from c in dbConnect.hazards
                        where c.source_hazard == vehicle_traffic
                        && c.function_id == function_id
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int vehicle_tracffic_all = n.Count();

                ///////////////////////////////////////////////////1///////////////////////////////////////////////////




                var n2 = from c in dbConnect.hazards
                         where c.source_hazard == electricity
                         && c.function_id == function_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n2 = n2.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n2 = n2.Where(c => c.hazard_date <= d_end);
                }

                int electricity_all = n2.Count();

                ////////////////////////////////////////////////////2/////////////////////////////////////////////////////


                var n3 = from c in dbConnect.hazards
                         where c.source_hazard == machine
                         && c.function_id == function_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n3 = n3.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n3 = n3.Where(c => c.hazard_date <= d_end);
                }

                int machine_all = n3.Count();

                ////////////////////////////////////////////////////3/////////////////////////////////////////////////////



                var n4 = from c in dbConnect.hazards
                         where c.source_hazard == chemical
                         && c.function_id == function_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n4 = n4.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n4 = n4.Where(c => c.hazard_date <= d_end);
                }

                int chemical_all = n4.Count();

                ////////////////////////////////////////////////////4/////////////////////////////////////////////////////


                var n5 = from c in dbConnect.hazards
                         where c.source_hazard == fire_heat
                         && c.function_id == function_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n5 = n5.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n5 = n5.Where(c => c.hazard_date <= d_end);
                }

                int fire_heat_all = n5.Count();

                ////////////////////////////////////////////////////5/////////////////////////////////////////////////////

                var n6 = from c in dbConnect.hazards
                         where c.source_hazard == slip_fall
                         && c.function_id == function_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n6 = n6.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n6 = n6.Where(c => c.hazard_date <= d_end);
                }

                int slip_fall_all = n6.Count();

                ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                var n7 = from c in dbConnect.hazards
                         where c.source_hazard == animal
                         && c.function_id == function_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n7 = n7.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n7 = n7.Where(c => c.hazard_date <= d_end);
                }

                int animal_all = n7.Count();

                ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                var n8 = from c in dbConnect.hazards
                         where c.source_hazard == fall_gravity
                         && c.function_id == function_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n8 = n8.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n8 = n8.Where(c => c.hazard_date <= d_end);
                }

                int fall_gravity_all = n8.Count();

                ////////////////////////////////////////////////////8/////////////////////////////////////////////////////


                var result = new
                {
                    label = label_all,
                    vehicle_tracffic = vehicle_tracffic_all,
                    fall_gravity = fall_gravity_all,
                    electricity = electricity_all,
                    machine = machine_all,
                    chemical = chemical_all,
                    fire_heat = fire_heat_all,
                    slip_fall = slip_fall_all,
                    animal = animal_all,
                    area_id = ""
                };


                dataJson.Add(result);


                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select n.department_id,d.department_th,d.department_en,n.count_select from (select i.department_id,ISNULL(count(i.department_id),0) as count_select from hazard i ";
                sql = sql + "where 1=1  and i.function_id='" + function_id + "' ";


                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }

                sql = sql + "group by i.department_id) as n ";
                sql = sql + "join department d on d.department_id = n.department_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_department = chageDataLanguage(ds.Tables[0].Rows[i]["department_th"].ToString(), ds.Tables[0].Rows[i]["department_en"].ToString(), lang);

                    var n_fun1 = from c in dbConnect.hazards
                                 where c.source_hazard == vehicle_traffic
                                 && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun1 = n_fun1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun1 = n_fun1.Where(c => c.hazard_date <= d_end);
                    }

                    int vehicle_tracffic_de = n_fun1.Count();

                    ///////////////////////////////////////////////////1///////////////////////////////////////////////////




                    var n_fun2 = from c in dbConnect.hazards
                                 where c.source_hazard == electricity
                                  && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun2 = n_fun2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun2 = n_fun2.Where(c => c.hazard_date <= d_end);
                    }

                    int electricity_de = n_fun2.Count();

                    ////////////////////////////////////////////////////2/////////////////////////////////////////////////////


                    var n_fun3 = from c in dbConnect.hazards
                                 where c.source_hazard == machine
                                 && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun3 = n_fun3.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun3 = n_fun3.Where(c => c.hazard_date <= d_end);
                    }

                    int machine_de = n_fun3.Count();

                    ////////////////////////////////////////////////////3/////////////////////////////////////////////////////



                    var n_fun4 = from c in dbConnect.hazards
                                 where c.source_hazard == chemical
                                 && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun4 = n_fun4.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun4 = n_fun4.Where(c => c.hazard_date <= d_end);
                    }

                    int chemical_de = n_fun4.Count();

                    ////////////////////////////////////////////////////4/////////////////////////////////////////////////////


                    var n_fun5 = from c in dbConnect.hazards
                                 where c.source_hazard == fire_heat
                                 && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun5 = n_fun5.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun5 = n_fun5.Where(c => c.hazard_date <= d_end);
                    }

                    int fire_heat_de = n_fun5.Count();

                    ////////////////////////////////////////////////////5/////////////////////////////////////////////////////

                    var n_fun6 = from c in dbConnect.hazards
                                 where c.source_hazard == slip_fall
                                 && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun6 = n_fun6.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun6 = n_fun6.Where(c => c.hazard_date <= d_end);
                    }

                    int slip_fall_de = n_fun6.Count();

                    ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                    var n_fun7 = from c in dbConnect.hazards
                                 where c.source_hazard == animal
                                 && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun7 = n_fun7.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun7 = n_fun7.Where(c => c.hazard_date <= d_end);
                    }

                    int animal_de = n_fun7.Count();

                    ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                    var n_fun8 = from c in dbConnect.hazards
                                 where c.source_hazard == fall_gravity
                                 && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun8 = n_fun8.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun8 = n_fun8.Where(c => c.hazard_date <= d_end);
                    }

                    int fall_gravity_de = n_fun8.Count();

                    ////////////////////////////////////////////////////7/////////////////////////////////////////////////////

                    var result2 = new
                    {
                        label = label_department,
                        vehicle_tracffic = vehicle_tracffic_de,
                        fall_gravity = fall_gravity_de,
                        electricity = electricity_de,
                        machine = machine_de,
                        chemical = chemical_de,
                        fire_heat = fire_heat_de,
                        slip_fall = slip_fall_de,
                        animal = animal_de,
                        area_id = ds.Tables[0].Rows[i]["department_id"].ToString()
                    };


                    dataJson.Add(result2);
                }


            }
            else if (type == "department")
            {

                var de = from d in dbConnect.departments
                         where d.department_id == department_id
                         select d;
                foreach (var u in de)
                {
                    label_all = chageDataLanguage(u.department_th, u.department_en, lang);
                }


                var n = from c in dbConnect.hazards
                        where c.source_hazard == vehicle_traffic
                        && c.department_id == department_id
                        select new
                        {
                            c.id,
                            c.hazard_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }

                int vehicle_tracffic_all = n.Count();

                ///////////////////////////////////////////////////1///////////////////////////////////////////////////




                var n2 = from c in dbConnect.hazards
                         where c.source_hazard == electricity
                         && c.department_id == department_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n2 = n2.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n2 = n2.Where(c => c.hazard_date <= d_end);
                }

                int electricity_all = n2.Count();

                ////////////////////////////////////////////////////2/////////////////////////////////////////////////////


                var n3 = from c in dbConnect.hazards
                         where c.source_hazard == machine
                         && c.department_id == department_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n3 = n3.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n3 = n3.Where(c => c.hazard_date <= d_end);
                }

                int machine_all = n3.Count();

                ////////////////////////////////////////////////////3/////////////////////////////////////////////////////



                var n4 = from c in dbConnect.hazards
                         where c.source_hazard == chemical
                         && c.department_id == department_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n4 = n4.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n4 = n4.Where(c => c.hazard_date <= d_end);
                }

                int chemical_all = n4.Count();

                ////////////////////////////////////////////////////4/////////////////////////////////////////////////////


                var n5 = from c in dbConnect.hazards
                         where c.source_hazard == fire_heat
                        && c.department_id == department_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n5 = n5.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n5 = n5.Where(c => c.hazard_date <= d_end);
                }

                int fire_heat_all = n5.Count();

                ////////////////////////////////////////////////////5/////////////////////////////////////////////////////

                var n6 = from c in dbConnect.hazards
                         where c.source_hazard == slip_fall
                         && c.department_id == department_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n6 = n6.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n6 = n6.Where(c => c.hazard_date <= d_end);
                }

                int slip_fall_all = n6.Count();

                ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                var n7 = from c in dbConnect.hazards
                         where c.source_hazard == animal
                         && c.department_id == department_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n7 = n7.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n7 = n7.Where(c => c.hazard_date <= d_end);
                }

                int animal_all = n7.Count();

                ////////////////////////////////////////////////////6/////////////////////////////////////////////////////



                var n8 = from c in dbConnect.hazards
                         where c.source_hazard == fall_gravity
                         && c.department_id == department_id
                         select new
                         {
                             c.id,
                             c.hazard_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n8 = n8.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n8 = n8.Where(c => c.hazard_date <= d_end);
                }

                int fall_gravity_all = n8.Count();

                ////////////////////////////////////////////////////8/////////////////////////////////////////////////////


                var result = new
                {
                    label = label_all,
                    vehicle_tracffic = vehicle_tracffic_all,
                    fall_gravity = fall_gravity_all,
                    electricity = electricity_all,
                    machine = machine_all,
                    chemical = chemical_all,
                    fire_heat = fire_heat_all,
                    slip_fall = slip_fall_all,
                    animal = animal_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select n.division_id,d.division_th,d.division_en,n.count_select from (select i.division_id,ISNULL(count(i.department_id),0) as count_select from hazard i ";
                sql = sql + " where 1=1 and i.department_id='" + department_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.hazard_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.hazard_date <='" + d_end + "'";
                }
                sql = sql + "group by i.division_id ) as n ";
                sql = sql + "join division d on d.division_id = n.division_id";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_division = chageDataLanguage(ds.Tables[0].Rows[i]["division_th"].ToString(), ds.Tables[0].Rows[i]["division_en"].ToString(), lang);

                    var n_fun1 = from c in dbConnect.hazards
                                 where c.source_hazard == vehicle_traffic
                                 && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun1 = n_fun1.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun1 = n_fun1.Where(c => c.hazard_date <= d_end);
                    }

                    int vehicle_tracffic_div = n_fun1.Count();

                    ///////////////////////////////////////////////////1///////////////////////////////////////////////////




                    var n_fun2 = from c in dbConnect.hazards
                                 where c.source_hazard == electricity
                                 && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun2 = n_fun2.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun2 = n_fun2.Where(c => c.hazard_date <= d_end);
                    }

                    int electricity_div = n_fun2.Count();

                    ////////////////////////////////////////////////////2/////////////////////////////////////////////////////


                    var n_fun3 = from c in dbConnect.hazards
                                 where c.source_hazard == machine
                                 && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun3 = n_fun3.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun3 = n_fun3.Where(c => c.hazard_date <= d_end);
                    }

                    int machine_div = n_fun3.Count();

                    ////////////////////////////////////////////////////3/////////////////////////////////////////////////////



                    var n_fun4 = from c in dbConnect.hazards
                                 where c.source_hazard == chemical
                                 && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun4 = n_fun4.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun4 = n_fun4.Where(c => c.hazard_date <= d_end);
                    }

                    int chemical_div = n_fun4.Count();

                    ////////////////////////////////////////////////////4/////////////////////////////////////////////////////


                    var n_fun5 = from c in dbConnect.hazards
                                 where c.source_hazard == fire_heat
                                 && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun5 = n_fun5.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun5 = n_fun5.Where(c => c.hazard_date <= d_end);
                    }

                    int fire_heat_div = n_fun5.Count();

                    ////////////////////////////////////////////////////5/////////////////////////////////////////////////////

                    var n_fun6 = from c in dbConnect.hazards
                                 where c.source_hazard == slip_fall
                                 && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun6 = n_fun6.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun6 = n_fun6.Where(c => c.hazard_date <= d_end);
                    }

                    int slip_fall_div = n_fun6.Count();

                    ////////////////////////////////////////////////////6/////////////////////////////////////////////////////


                    var n_fun7 = from c in dbConnect.hazards
                                 where c.source_hazard == animal
                                 && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun7 = n_fun7.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun7 = n_fun7.Where(c => c.hazard_date <= d_end);
                    }

                    int animal_div = n_fun7.Count();

                    ////////////////////////////////////////////////////6/////////////////////////////////////////////////////

                    var n_fun8 = from c in dbConnect.hazards
                                 where c.source_hazard == fall_gravity
                                 && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                                 select new
                                 {
                                     c.id,
                                     c.hazard_date
                                 };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n_fun8 = n_fun8.Where(c => c.hazard_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n_fun8 = n_fun8.Where(c => c.hazard_date <= d_end);
                    }

                    int fall_gravity_div = n_fun8.Count();

                    ////////////////////////////////////////////////////7/////////////////////////////////////////////////////


                    var result2 = new
                    {
                        label = label_division,
                        vehicle_tracffic = vehicle_tracffic_div,
                        fall_gravity = fall_gravity_div,
                        electricity = electricity_div,
                        machine = machine_div,
                        chemical = chemical_div,
                        fire_heat = fire_heat_div,
                        slip_fall = slip_fall_div,
                        animal = animal_div,
                        area_id = ds.Tables[0].Rows[i]["division_id"].ToString()
                    };


                    dataJson.Add(result2);

                }

            }
            else if (type == "division")
            {
                string department_id2 = "";
                string function_id2 = "";
                string company_id2 = "";

                var divisions2 = from f in dbConnect.divisions
                                 where f.division_id == division_id
                                 select f;

                foreach (var di in divisions2)
                {
                    department_id2 = di.department_id;

                    var departments2 = from f in dbConnect.departments
                                       where f.department_id == department_id2
                                       select f;

                    foreach (var de in departments2)
                    {
                        function_id2 = de.function_id;


                        var functions2 = from f in dbConnect.functions
                                         where f.function_id == function_id2
                                         select f;

                        foreach (var fun in functions2)
                        {
                            company_id2 = fun.company_id;

                        }

                    }
                }

                redirect = "allhazard.aspx?company_id=" + company_id2 + "&function_id=" + function_id2 + "&department_id=" + department_id2 + "&division_id=" + division_id;





            }

            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }






        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardHazard5(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string redirect = "";

            ArrayList dataJson = new ArrayList();

            if (area_id != "")
            {
                type = "department";
            }
            else
            {

                type = "all";
            }



            if (type == "all")
            {
                List<string> ls_sub = new List<string>();

                var n = from c in dbConnect.hazards
                        where c.country == Session["country"].ToString()

                        select new
                        {
                            c.id,
                            c.hazard_date,
                            c.section_id,
                            c.division_id,
                            c.department_id,
                            c.function_id
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.hazard_date <= d_end);
                }


               
                foreach (var s in n)
                {

                    var v = from c in dbConnect.organizations
                            where c.country == Session["country"].ToString()
                            && c.org_unit_id == s.section_id
                            orderby c.personnel_subarea ascending
                            select new
                            {
                                id = c.personnel_subarea,
                                name = c.personnel_subarea_description

                            };

                    if (v.Count() > 0)
                    {
                        foreach (var rc in v)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v1 = from c in dbConnect.organizations
                                where c.country == Session["country"].ToString()
                                && c.org_unit_id == s.division_id
                                orderby c.personnel_subarea ascending
                                select new
                                {
                                    id = c.personnel_subarea,
                                    name = c.personnel_subarea_description

                                };
                        if (v1.Count() > 0)
                        {
                            foreach (var rc in v1)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {
                            var v2 = from c in dbConnect.organizations
                                     where c.country == Session["country"].ToString()
                                     && c.org_unit_id == s.department_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v2.Count() > 0)
                            {
                                foreach (var rc in v2)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }
                            else
                            {

                                var v3 = from c in dbConnect.organizations
                                         where c.country == Session["country"].ToString()
                                         && c.org_unit_id == s.function_id
                                         orderby c.personnel_subarea ascending
                                         select new
                                         {
                                             id = c.personnel_subarea,
                                             name = c.personnel_subarea_description

                                         };
                                if (v3.Count() > 0)
                                {
                                    foreach (var rc in v3)
                                    {
                                        ls_sub.Add(rc.id);
                                    }
                                }
                                
                            }

                        }

                    }

                }// end foreach




                var ve = (from c in dbConnect.organizations
                         where c.country == Session["country"].ToString()
                         orderby c.personnel_subarea ascending
                         select new
                         {
                             id = c.personnel_subarea,
                             name = c.personnel_subarea_description

                         }).Distinct();


                foreach (var rc in ve)
                {

                    var e = ls_sub.Where(s => s == rc.id);


                    int count_all = e.Count();

                    string name_personnel = "";
                    var na = from a in dbConnect.organizations
                             where a.personnel_subarea == rc.id
                             select new
                             {
                                 name = a.personnel_subarea_description
                             };

                    foreach (var q in na)
                    {
                        name_personnel = q.name;
                    }

                    var result = new
                    {
                        label = name_personnel,
                        count = count_all,
                        area_id = ""// rc.id
                    };


                    dataJson.Add(result);


                }

            }
            else if (type == "department")
            {
                //var s1 = from c in dbConnect.organizations
                //        where c.personnel_subarea == area_id
                //        && c.division_id != "00000000"
                //        select new
                //        {
                //            c.division_id
                //        };

                //List<string> ls = new List<string>();

                //foreach (var r1 in s1)
                //{
                //    ls.Add(r1.division_id);
                //}


                //var n1 = from c in dbConnect.hazards
                //        where c.country == Session["country"].ToString()
                //        && ls.Contains(c.section_id)//srilanka is division
                //        select new
                //        {
                //            c.id,
                //            c.hazard_date
                //        };

                //if (date_start != "")
                //{
                //    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //    n1 = n1.Where(c => c.hazard_date >= d_start);
                //}

                //if (date_end != "")
                //{
                //    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //    n1 = n1.Where(c => c.hazard_date <= d_end);
                //}

                //int count_all = n1.Count();

                //string name_personnel = "";
                //var na = from a in dbConnect.organizations
                //         where a.personnel_subarea == area_id
                //         select new
                //         {
                //             name = a.personnel_subarea_description
                //         };
                //foreach (var q in na)
                //{
                //    name_personnel = q.name;
                //}

                //var result = new
                //{
                //    label = name_personnel,
                //    count = count_all,
                //    area_id = ""
                //};


                //dataJson.Add(result);


                //var s = from c in dbConnect.divisions
                //         where c.country == Session["country"].ToString()
                //         select new
                //        {
                //            c.division_id,
                //            name = chageDataLanguage(c.division_th,c.division_en,lang)
                //        };


                //foreach (var r in s)
                //{
                //    var n = from c in dbConnect.hazards
                //            join e in dbConnect.employees on c.employee_id equals e.employee_id
                //            join o in dbConnect.organizations on e.unit_id equals o.org_unit_id
                //            where c.country == Session["country"].ToString()
                //            && o.division_id == r.division_id
                //            select new
                //            {
                //                c.id,
                //                c.hazard_date
                //            };

                //    if (date_start != "")
                //    {
                //        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //        n = n.Where(c => c.hazard_date >= d_start);
                //    }

                //    if (date_end != "")
                //    {
                //        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //        n = n.Where(c => c.hazard_date <= d_end);
                //    }

                //    int count = n.Count();

                //    var result2 = new
                //    {
                //        label = r.name,
                //        count = count,
                //        area_id = ""
                //    };


                //    dataJson.Add(result2);

                //}

      


            }


           

            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardHazard6(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string redirect = "";

            ArrayList dataJson = new ArrayList();

            if (area_id != "")
            {
                type = "department";
            }
            else
            {

                type = "all";
            }



            if (type == "all")
            {
                List<string> ls_sub_onprocess = new List<string>();
                List<string> ls_sub_close = new List<string>();
                List<string> ls_sub_reject = new List<string>();

                ls_sub_onprocess = getProcessStatusBySite(date_start, date_end, 1, Session["country"].ToString(), lang);
                ls_sub_close = getProcessStatusBySite(date_start, date_end, 2, Session["country"].ToString(), lang);
                ls_sub_reject = getProcessStatusBySite(date_start, date_end, 3, Session["country"].ToString(), lang);

                


                var ve = (from c in dbConnect.organizations
                          where c.country == Session["country"].ToString()
                          orderby c.personnel_subarea ascending
                          select new
                          {
                              id = c.personnel_subarea,
                              name = c.personnel_subarea_description

                          }).Distinct();


                foreach (var rc in ve)
                {

                    var o = ls_sub_onprocess.Where(s => s == rc.id);
                    var c = ls_sub_close.Where(s => s == rc.id);
                    var j = ls_sub_reject.Where(s => s == rc.id);


                    int count_onprocess = o.Count();
                    int count_close = c.Count();
                    int count_reject = j.Count();

                    string name_personnel = "";
                    var na = from a in dbConnect.organizations
                             where a.personnel_subarea == rc.id
                             select new
                             {
                                 name = a.personnel_subarea_description
                             };

                    foreach (var q in na)
                    {
                        name_personnel = q.name;
                    }

                    var result = new
                    {
                        label = name_personnel,
                        onprocess = count_onprocess,
                        close = count_close,
                        reject = count_reject,
                        area_id = ""// rc.id
                    };


                    dataJson.Add(result);


                }

            }
            else if (type == "department")
            {
              


            }




            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardHazard7(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string redirect = "";

            ArrayList dataJson = new ArrayList();

            if (area_id != "")
            {
                type = "department";
            }
            else
            {

                type = "all";
            }



            if (type == "all")
            {
                List<string> ls_sub_onprocess = new List<string>();
                List<string> ls_sub_close = new List<string>();
                List<string> ls_sub_delay = new List<string>();

                ls_sub_onprocess = getActionStatusOnprocessBySite(date_start, date_end, Session["country"].ToString(), Session["timezone"].ToString(), lang);
                ls_sub_close = getActionStatusCloseBySite(date_start, date_end, Session["country"].ToString(), Session["timezone"].ToString(), lang);
                ls_sub_delay = getActionStatusDelayBySite(date_start, date_end, Session["country"].ToString(), Session["timezone"].ToString(), lang);




                var ve = (from c in dbConnect.organizations
                          where c.country == Session["country"].ToString()
                          orderby c.personnel_subarea ascending
                          select new
                          {
                              id = c.personnel_subarea,
                              name = c.personnel_subarea_description

                          }).Distinct();


                foreach (var rc in ve)
                {

                    var o = ls_sub_onprocess.Where(s => s == rc.id);
                    var c = ls_sub_close.Where(s => s == rc.id);
                    var j = ls_sub_delay.Where(s => s == rc.id);


                    int count_onprocess = o.Count();
                    int count_close = c.Count();
                    int count_delay = j.Count();

                    string name_personnel = "";
                    var na = from a in dbConnect.organizations
                             where a.personnel_subarea == rc.id
                             select new
                             {
                                 name = a.personnel_subarea_description
                             };

                    foreach (var q in na)
                    {
                        name_personnel = q.name;
                    }

                    var result = new
                    {
                        label = name_personnel,
                        onprocess = count_onprocess,
                        close = count_close,
                        delay = count_delay,
                        area_id = ""// rc.id
                    };


                    dataJson.Add(result);


                }

            }
            else if (type == "department")
            {



            }




            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardHazard8(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string redirect = "";

            ArrayList dataJson = new ArrayList();

            if (area_id != "")
            {
                type = "department";
            }
            else
            {

                type = "all";
            }



            if (type == "all")
            {
                List<string> ls_count = new List<string>();

                ls_count = getAllHazardwithreporterBySite(date_start, date_end, Session["country"].ToString(), lang);
               

                var ve = (from c in dbConnect.organizations
                          where c.country == Session["country"].ToString()
                          orderby c.personnel_subarea ascending
                          select new
                          {
                              id = c.personnel_subarea,
                              name = c.personnel_subarea_description

                          }).Distinct();


                foreach (var rc in ve)
                {

                    var o = ls_count.Where(s => s == rc.id);
                   

                    int count_hazard = o.Count();
                   
                    string name_personnel = "";
                    var na = from a in dbConnect.organizations
                             where a.personnel_subarea == rc.id
                             select new
                             {
                                 name = a.personnel_subarea_description
                             };

                    foreach (var q in na)
                    {
                        name_personnel = q.name;
                    }

                    var result = new
                    {
                        label = name_personnel,
                        count = count_hazard,                     
                        area_id = ""// rc.id
                    };


                    dataJson.Add(result);


                }

            }
            else if (type == "department")
            {



            }




            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncident5(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string redirect = "";

            ArrayList dataJson = new ArrayList();

            if (area_id != "")
            {
                type = "department";
            }
            else
            {

                type = "all";
            }



            if (type == "all")
            {
                List<string> ls_sub = new List<string>();

                var n = from c in dbConnect.incidents
                        where c.country == Session["country"].ToString()

                        select new
                        {
                            c.id,
                            c.incident_date,
                            c.section_id,
                            c.division_id,
                            c.department_id,
                            c.function_id
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }



                foreach (var s in n)
                {

                    var v = from c in dbConnect.organizations
                            where c.country == Session["country"].ToString()
                            && c.org_unit_id == s.section_id
                            orderby c.personnel_subarea ascending
                            select new
                            {
                                id = c.personnel_subarea,
                                name = c.personnel_subarea_description

                            };

                    if (v.Count() > 0)
                    {
                        foreach (var rc in v)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v1 = from c in dbConnect.organizations
                                 where c.country == Session["country"].ToString()
                                 && c.org_unit_id == s.division_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v1.Count() > 0)
                        {
                            foreach (var rc in v1)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {
                            var v2 = from c in dbConnect.organizations
                                     where c.country == Session["country"].ToString()
                                     && c.org_unit_id == s.department_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v2.Count() > 0)
                            {
                                foreach (var rc in v2)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }
                            else
                            {

                                var v3 = from c in dbConnect.organizations
                                         where c.country == Session["country"].ToString()
                                         && c.org_unit_id == s.function_id
                                         orderby c.personnel_subarea ascending
                                         select new
                                         {
                                             id = c.personnel_subarea,
                                             name = c.personnel_subarea_description

                                         };
                                if (v3.Count() > 0)
                                {
                                    foreach (var rc in v3)
                                    {
                                        ls_sub.Add(rc.id);
                                    }
                                }

                            }

                        }

                    }

                }// end foreach

                ////////////////////////////////////////end จำนวน incident//////////////////////////////////////////
                int REJECT = 3;

                List<string> ls_sub2 = new List<string>();

                var n2 = from c in dbConnect.incidents
                         where c.work_relate == "Y" && c.country == Session["country"].ToString() &&
                        (c.culpability == "G" || c.culpability == "P")
                        && c.process_status != REJECT

                        select new
                        {
                            c.id,
                            c.incident_date,
                            c.section_id,
                            c.division_id,
                            c.department_id,
                            c.function_id
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n2 = n2.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n2 = n2.Where(c => c.incident_date <= d_end);
                }



                foreach (var s in n2)
                {

                    var v = from c in dbConnect.organizations
                            where c.country == Session["country"].ToString()
                            && c.org_unit_id == s.section_id
                            orderby c.personnel_subarea ascending
                            select new
                            {
                                id = c.personnel_subarea,
                                name = c.personnel_subarea_description

                            };

                    if (v.Count() > 0)
                    {
                        foreach (var rc in v)
                        {
                            ls_sub2.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v1 = from c in dbConnect.organizations
                                 where c.country == Session["country"].ToString()
                                 && c.org_unit_id == s.division_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v1.Count() > 0)
                        {
                            foreach (var rc in v1)
                            {
                                ls_sub2.Add(rc.id);
                            }
                        }
                        else
                        {
                            var v2 = from c in dbConnect.organizations
                                     where c.country == Session["country"].ToString()
                                     && c.org_unit_id == s.department_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v2.Count() > 0)
                            {
                                foreach (var rc in v2)
                                {
                                    ls_sub2.Add(rc.id);
                                }
                            }
                            else
                            {

                                var v3 = from c in dbConnect.organizations
                                         where c.country == Session["country"].ToString()
                                         && c.org_unit_id == s.function_id
                                         orderby c.personnel_subarea ascending
                                         select new
                                         {
                                             id = c.personnel_subarea,
                                             name = c.personnel_subarea_description

                                         };
                                if (v3.Count() > 0)
                                {
                                    foreach (var rc in v3)
                                    {
                                        ls_sub2.Add(rc.id);
                                    }
                                }

                            }

                        }

                    }

                }// end foreach


                /////////////////////////////////////////end record incident////////////////////////////////////////


                var ve = (from c in dbConnect.organizations
                          where c.country == Session["country"].ToString()
                          orderby c.personnel_subarea ascending
                          select new
                          {
                              id = c.personnel_subarea,
                              name = c.personnel_subarea_description

                          }).Distinct();


                foreach (var rc in ve)
                {

                    var e = ls_sub.Where(s => s == rc.id);
                    var r = ls_sub2.Where(s => s == rc.id);


                    int count_all = e.Count();
                    int count_condition = r.Count();

                    string name_personnel = "";
                    var na = from a in dbConnect.organizations
                             where a.personnel_subarea == rc.id
                             select new
                             {
                                 name = a.personnel_subarea_description
                             };

                    foreach (var q in na)
                    {
                        name_personnel = q.name;
                    }

                    var result = new
                    {
                        label = name_personnel,
                        all = count_all,
                        condition = count_condition,
                        area_id = ""// rc.id
                    };


                    dataJson.Add(result);


                }

            }
            else if (type == "department")
            {
          


            }




            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }




        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncident6(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string redirect = "";


            int F = 7;
            int PD = 8;
            int LTI = 9;
            int MTI = 10;
            int MI = 11;
            int RWC = 12;

           

            ArrayList dataJson = new ArrayList();

            if (area_id != "")
            {
                type = "department";
            }
            else
            {

                type = "all";
            }



            if (type == "all")
            {
                List<string> ls_f = new List<string>();
                List<string> ls_pd = new List<string>();
                List<string> ls_lti = new List<string>();
                List<string> ls_mti = new List<string>();
                List<string> ls_mi = new List<string>();
                List<string> ls_rwc = new List<string>();
                List<string> ls_damage = new List<string>();
                List<string> ls_nm = new List<string>();
             

                ls_f = getCategoriesBySite(date_start, date_end, F, Session["country"].ToString(), lang);
                ls_pd = getCategoriesBySite(date_start, date_end, PD, Session["country"].ToString(), lang);
                ls_lti = getCategoriesBySite(date_start, date_end, LTI, Session["country"].ToString(), lang);
                ls_mti = getCategoriesBySite(date_start, date_end, MTI, Session["country"].ToString(), lang);
                ls_mi = getCategoriesBySite(date_start, date_end, MI, Session["country"].ToString(), lang);
                ls_rwc = getCategoriesBySite(date_start, date_end, RWC, Session["country"].ToString(), lang);
                ls_nm = getNearmissBySite(date_start, date_end, Session["country"].ToString(), lang);

                //////////////////////////////////end injury////////////////////////////////////////////////


                int REJECT = 3;

                var n = from c in dbConnect.damage_lists
                        join i in dbConnect.incidents on c.incident_id equals i.id
                        where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                        && i.process_status != REJECT
                        && i.country == Session["country"].ToString()
                         && c.status == "A"
                        select new
                        {
                            i.id,
                            i.incident_date,
                            i.section_id,
                            i.division_id,
                            i.department_id,
                            i.function_id
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }



                foreach (var s in n)
                {

                    var v = from c in dbConnect.organizations
                            where c.country == Session["country"].ToString()
                            && c.org_unit_id == s.section_id
                            orderby c.personnel_subarea ascending
                            select new
                            {
                                id = c.personnel_subarea,
                                name = c.personnel_subarea_description

                            };

                    if (v.Count() > 0)
                    {
                        foreach (var rc in v)
                        {
                            ls_damage.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v1 = from c in dbConnect.organizations
                                 where c.country == Session["country"].ToString()
                                 && c.org_unit_id == s.division_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v1.Count() > 0)
                        {
                            foreach (var rc in v1)
                            {
                                ls_damage.Add(rc.id);
                            }
                        }
                        else
                        {
                            var v2 = from c in dbConnect.organizations
                                     where c.country == Session["country"].ToString()
                                     && c.org_unit_id == s.department_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v2.Count() > 0)
                            {
                                foreach (var rc in v2)
                                {
                                    ls_damage.Add(rc.id);
                                }
                            }
                            else
                            {

                                var v3 = from c in dbConnect.organizations
                                         where c.country == Session["country"].ToString()
                                         && c.org_unit_id == s.function_id
                                         orderby c.personnel_subarea ascending
                                         select new
                                         {
                                             id = c.personnel_subarea,
                                             name = c.personnel_subarea_description

                                         };
                                if (v3.Count() > 0)
                                {
                                    foreach (var rc in v3)
                                    {
                                        ls_damage.Add(rc.id);
                                    }
                                }

                            }

                        }

                    }

                }// end foreach







                ///////////////////////////////////end damage list///////////////////////////////////////////////


                var ve = (from c in dbConnect.organizations
                          where c.country == Session["country"].ToString()
                          orderby c.personnel_subarea ascending
                          select new
                          {
                              id = c.personnel_subarea,
                              name = c.personnel_subarea_description

                          }).Distinct();


                foreach (var rc in ve)
                {

                    var f = ls_f.Where(s => s == rc.id);
                    var pd = ls_pd.Where(s => s == rc.id);
                    var lti = ls_lti.Where(s => s == rc.id);
                    var mti = ls_mti.Where(s => s == rc.id);
                    var mi = ls_mi.Where(s => s == rc.id);
                    var rwc = ls_rwc.Where(s => s == rc.id);
                    var damage = ls_damage.Where(s => s == rc.id);
                    var nm = ls_nm.Where(s => s == rc.id);


                    int count_f = f.Count();
                    int count_pd = pd.Count();
                    int count_lti = lti.Count();
                    int count_mti = mti.Count();
                    int count_mi = mi.Count();
                    int count_rwc = rwc.Count();
                    int count_damage = damage.Count();
                    int count_nm = nm.Count();

                    string name_personnel = "";
                    var na = from a in dbConnect.organizations
                             where a.personnel_subarea == rc.id
                             select new
                             {
                                 name = a.personnel_subarea_description
                             };

                    foreach (var q in na)
                    {
                        name_personnel = q.name;
                    }

                    var result = new
                    {
                        label = name_personnel,
                        fatality = count_f,
                        pd = count_pd,
                        lti = count_lti,
                        mti = count_mti,
                        mi = count_mi,
                        damage = count_damage,
                        rwc = count_rwc,
                        nm = count_nm,
                        area_id = ""// rc.id
                    };


                    dataJson.Add(result);


                }

            }
            else if (type == "department")
            {



            }




            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncident7(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string redirect = "";

            ArrayList dataJson = new ArrayList();

            if (area_id != "")
            {
                type = "department";
            }
            else
            {

                type = "all";
            }



            if (type == "all")
            {
                List<string> ls_sub_onprocess = new List<string>();
                List<string> ls_sub_close = new List<string>();
                List<string> ls_sub_delay = new List<string>();

                ls_sub_onprocess = getIncidentActionStatusOnprocessBySite(date_start, date_end, Session["country"].ToString(), Session["timezone"].ToString(), lang);
                ls_sub_close = getIncidentActionStatusCloseBySite(date_start, date_end, Session["country"].ToString(), Session["timezone"].ToString(), lang);
                ls_sub_delay = getIncidentActionStatusDelayBySite(date_start, date_end, Session["country"].ToString(), Session["timezone"].ToString(), lang);




                var ve = (from c in dbConnect.organizations
                          where c.country == Session["country"].ToString()
                          orderby c.personnel_subarea ascending
                          select new
                          {
                              id = c.personnel_subarea,
                              name = c.personnel_subarea_description

                          }).Distinct();


                foreach (var rc in ve)
                {

                    var o = ls_sub_onprocess.Where(s => s == rc.id);
                    var c = ls_sub_close.Where(s => s == rc.id);
                    var j = ls_sub_delay.Where(s => s == rc.id);


                    int count_onprocess = o.Count();
                    int count_close = c.Count();
                    int count_delay = j.Count();

                    string name_personnel = "";
                    var na = from a in dbConnect.organizations
                             where a.personnel_subarea == rc.id
                             select new
                             {
                                 name = a.personnel_subarea_description
                             };

                    foreach (var q in na)
                    {
                        name_personnel = q.name;
                    }

                    var result = new
                    {
                        label = name_personnel,
                        onprocess = count_onprocess,
                        close = count_close,
                        delay = count_delay,
                        area_id = ""// rc.id
                    };


                    dataJson.Add(result);


                }

            }
            else if (type == "department")
            {



            }




            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncident8(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string redirect = "";

            ArrayList dataJson = new ArrayList();

            if (area_id != "")
            {
                type = "department";
            }
            else
            {

                type = "all";
            }



            if (type == "all")
            {
                List<string> ls_sub_onprocess = new List<string>();
                List<string> ls_sub_close = new List<string>();
                List<string> ls_sub_delay = new List<string>();

                ls_sub_onprocess = getIncidentInjuryActionStatusOnprocessBySite(date_start, date_end, Session["country"].ToString(), Session["timezone"].ToString(), lang);
                ls_sub_close = getIncidentInjuryActionStatusCloseBySite(date_start, date_end, Session["country"].ToString(), Session["timezone"].ToString(), lang);
                ls_sub_delay = getIncidentInjuryActionStatusDelayBySite(date_start, date_end, Session["country"].ToString(), Session["timezone"].ToString(), lang);




                var ve = (from c in dbConnect.organizations
                          where c.country == Session["country"].ToString()
                          orderby c.personnel_subarea ascending
                          select new
                          {
                              id = c.personnel_subarea,
                              name = c.personnel_subarea_description

                          }).Distinct();


                foreach (var rc in ve)
                {

                    var o = ls_sub_onprocess.Where(s => s == rc.id);
                    var c = ls_sub_close.Where(s => s == rc.id);
                    var j = ls_sub_delay.Where(s => s == rc.id);


                    int count_onprocess = o.Count();
                    int count_close = c.Count();
                    int count_delay = j.Count();

                    string name_personnel = "";
                    var na = from a in dbConnect.organizations
                             where a.personnel_subarea == rc.id
                             select new
                             {
                                 name = a.personnel_subarea_description
                             };

                    foreach (var q in na)
                    {
                        name_personnel = q.name;
                    }

                    var result = new
                    {
                        label = name_personnel,
                        onprocess = count_onprocess,
                        close = count_close,
                        delay = count_delay,
                        area_id = ""// rc.id
                    };


                    dataJson.Add(result);


                }

            }
            else if (type == "department")
            {



            }




            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncident9(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string redirect = "";

            ArrayList dataJson = new ArrayList();

            if (area_id != "")
            {
                type = "department";
            }
            else
            {

                type = "all";
            }



            if (type == "all")
            {
                List<string> ls_sub_onprocess = new List<string>();
                List<string> ls_sub_close = new List<string>();
                List<string> ls_sub_reject = new List<string>();

                ls_sub_onprocess = getIncidentProcessStatusBySite(date_start, date_end, 1, Session["country"].ToString(), lang);
                ls_sub_close = getIncidentProcessStatusBySite(date_start, date_end, 2, Session["country"].ToString(), lang);
                ls_sub_reject = getIncidentProcessStatusBySite(date_start, date_end, 3, Session["country"].ToString(), lang);




                var ve = (from c in dbConnect.organizations
                          where c.country == Session["country"].ToString()
                          orderby c.personnel_subarea ascending
                          select new
                          {
                              id = c.personnel_subarea,
                              name = c.personnel_subarea_description

                          }).Distinct();


                foreach (var rc in ve)
                {

                    var o = ls_sub_onprocess.Where(s => s == rc.id);
                    var c = ls_sub_close.Where(s => s == rc.id);
                    var j = ls_sub_reject.Where(s => s == rc.id);


                    int count_onprocess = o.Count();
                    int count_close = c.Count();
                    int count_reject = j.Count();

                    string name_personnel = "";
                    var na = from a in dbConnect.organizations
                             where a.personnel_subarea == rc.id
                             select new
                             {
                                 name = a.personnel_subarea_description
                             };

                    foreach (var q in na)
                    {
                        name_personnel = q.name;
                    }

                    var result = new
                    {
                        label = name_personnel,
                        onprocess = count_onprocess,
                        close = count_close,
                        reject = count_reject,
                        area_id = ""// rc.id
                    };


                    dataJson.Add(result);


                }

            }
            else if (type == "department")
            {



            }




            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void  getDashboardIncidentFormOneToTwo(string area_id,
                                                      string date_start,
                                                      string date_end,
                                                      string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string redirect = "";

            ArrayList dataJson = new ArrayList();


            if (area_id == "")
            {
                var v = from c in dbConnect.incidents
                        where c.process_status == 1 && c.step_form == 1
                        && c.country == Session["country"].ToString()
                        //&& DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.report_date.AddDays(1)
                       // && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.report_date.AddDays(2)
                        select new
                        {
                            c.incident_date,
                            c.report_date
                        };


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int day1 = 0;
                int day2 = 0;
                int day3 = 0;
                int day_more_3 = 0;

               

                    foreach (var rc in v)
                    {
                        DateTime dtnew = rc.report_date;
                        DateTime current = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        TimeSpan span = current.Subtract(dtnew);
                        int cday = Convert.ToInt32(span.TotalDays);
                        int day = 0;


                        if (cday < 15)
                            {
                                while (dtnew <= current)
                                {
                                        dtnew = dtnew.AddDays(1);

                                        if (dtnew.DayOfWeek.ToString() != "Saturday" && dtnew.DayOfWeek.ToString() != "Sunday")
                                        {
                                            var holidays = from c in dbConnect.holidays
                                                           where c.holiday_date == dtnew.Date
                                                           select new
                                                           {
                                                               holiday_date = c.holiday_date,

                                                           };

                                            bool is_holiday = false;
                                            foreach (var r in holidays)
                                            {
                                                is_holiday = true;
                                            }

                                            if (!is_holiday)
                                            {
                                                day++;

                                            }

                                        }//end if

                                }//end while


                                ////////////////////////////////////////////////////////////////////////////////////////////
                                if (day == 1)
                                {
                                    day1++;
                                }
                                else if (day == 2)
                                {
                                    day2++;

                                }
                                else if (day == 3)
                                {

                                    day3++;

                                }
                                else if(day >3)
                                {

                                    day_more_3++;
                                }


                            }
                            else
                            {

                                day_more_3++;


                            }


                    }//end foreach




                    var result1 = new
                    {
                        label = chageDataLanguage("1 วัน", "1 day", lang),
                        work = day1,
                        area_id = 1
                    };

                    dataJson.Add(result1);

                    var result2 = new
                    {
                        label = chageDataLanguage("2 วัน", "2 days", lang),
                        work = day2,
                        area_id = 2
                    };

                    dataJson.Add(result2);


                    var result3 = new
                    {
                        label = chageDataLanguage("3 วัน", "3 days", lang),
                        work = day3,
                        area_id = 3
                    };

                    dataJson.Add(result3);

                    var result4 = new
                    {
                        label = chageDataLanguage("มากกว่า 3 วัน", "More than 3 days", lang),
                        work = day_more_3,
                        area_id = 4
                    };
                    dataJson.Add(result4);


                #region oldcode

                //for (int i = 1; i <= 4; i++)
                //{
                //    int day = 0;
                //    int amount_day = 0;
                //    string lb = "";

                //    if (i == 1)
                //    {
                //        var v = from c in dbConnect.incidents
                //                where c.process_status == 1 && c.step_form == 1
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.report_date.AddDays(1)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.report_date.AddDays(2)
                //                select new
                //                {
                //                    c.incident_date
                //                };
                                

                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.incident_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.incident_date <= d_end);
                //        }


                //        amount_day = v.Count();
                //        lb = chageDataLanguage("1 วัน", "1 day", lang);
                //        day = 1;

                //    }
                //    else if (i == 2)
                //    {
                //        var v = from c in dbConnect.incidents
                //                where c.process_status == 1 && c.step_form == 1
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.report_date.AddDays(2)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.report_date.AddDays(3)
                //                select new
                //                {
                //                    c.incident_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.incident_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.incident_date <= d_end);
                //        }

                //        amount_day = v.Count();
                //        lb = chageDataLanguage("2 วัน", "2 days", lang);
                //        day = 2;


                //    }
                //    else if (i == 3)
                //    {
                //        var v = from c in dbConnect.incidents
                //                where c.process_status == 1 && c.step_form == 1
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.report_date.AddDays(3)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.report_date.AddDays(4)
                //                select new
                //                {
                //                    c.incident_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.incident_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.incident_date <= d_end);
                //        }

                //        amount_day = v.Count();
                //        lb = chageDataLanguage("3 วัน", "3 days", lang);
                //        day = 3;


                //    }
                //    else if (i >3 )
                //    {
                //        var v = from c in dbConnect.incidents
                //                where c.process_status == 1 && c.step_form == 1
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) > c.report_date.AddDays(4)
                //                select new
                //                {
                //                    c.incident_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.incident_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.incident_date <= d_end);
                //        }

                //        amount_day = v.Count();
                //        lb = chageDataLanguage("มากกว่า 3 วัน", "More than 3 days", lang);
                //        day = 4;


                //    }
                   

                //    var result = new
                //    {
                //        label = lb,
                //        work = amount_day,
                //        area_id = day
                //    };

                    


                //    dataJson.Add(result);

                //}
                #endregion
            }
            else
            {

                redirect = "allincident.aspx?form=1to2&day="+area_id;

            }
           
               

            
           

            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncidentFormTwoToThree(string area_id,
                                                      string date_start,
                                                      string date_end,
                                                      string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string redirect = "";

            ArrayList dataJson = new ArrayList();


            if (area_id == "")
            {

                var v = from c in dbConnect.incidents
                        where c.process_status == 1 && c.step_form == 3
                        && c.confirm_form_two_to_three_at != null
                        && c.action_form_three_at == null
                        && c.country == Session["country"].ToString()
                        //&& DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.confirm_form_two_to_three_at.Value.AddDays(5)
                        //&& DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.confirm_form_two_to_three_at.Value.AddDays(8)
                        select new
                        {
                            c.incident_date,
                            c.id,
                            c.confirm_form_two_to_three_at

                        };


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


            
                int day5_7 = 0;
                int day8_10 = 0;
                int day11_13 = 0;
                int day14_15 = 0;
                int day_more_15 = 0;



                foreach (var rc in v)
                {

                    var count_row = dbConnect.corrective_prevention_action_incidents.Where(x => x.incident_id == rc.id).Count();
                    var count_row_fact = dbConnect.fact_findings.Where(x => x.incident_id == rc.id).Count();

                    if (count_row == 0 && count_row_fact == 0)
                    {
                        DateTime dtnew = rc.confirm_form_two_to_three_at.Value;
                        DateTime current = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        TimeSpan span = current.Subtract(dtnew);
                        int cday = Convert.ToInt16(span.TotalDays);
                        int day = 0;


                        if (cday < 30)
                        {
                            while (dtnew <= current)
                            {
                                dtnew = dtnew.AddDays(1);

                                if (dtnew.DayOfWeek.ToString() != "Saturday" && dtnew.DayOfWeek.ToString() != "Sunday")
                                {
                                    var holidays = from c in dbConnect.holidays
                                                   where c.holiday_date == dtnew.Date
                                                   select new
                                                   {
                                                       holiday_date = c.holiday_date,

                                                   };

                                    bool is_holiday = false;
                                    foreach (var r in holidays)
                                    {
                                        is_holiday = true;
                                    }

                                    if (!is_holiday)
                                    {
                                        day++;

                                    }

                                }//end if

                            }//end while


                            ////////////////////////////////////////////////////////////////////////////////////////////
                            if (day>=5 & day <=7)
                            {
                                day5_7++;
                            }
                            else if (day >= 8 & day <= 10)
                            {
                                day8_10++;

                            }
                            else if (day >= 11 & day <= 13)
                            {

                                day11_13++;

                            }
                            else if (day >= 14 & day <= 15)
                            {

                                day14_15++;

                            }
                            else if(day > 15)
                            {

                                day_more_15++;
                            }


                        }
                        else
                        {

                            day_more_15++;


                        }

                    }


                }//end foreach




                var result1 = new
                {
                    label = chageDataLanguage("5-7 วัน", "5-7 day", lang),
                    work = day5_7,
                    area_id = 5
                };

                dataJson.Add(result1);

                var result2 = new
                {
                    label = chageDataLanguage("8-10 วัน", "8-10 days", lang),
                    work = day8_10,
                    area_id = 8
                };

                dataJson.Add(result2);


                var result3 = new
                {
                    label = chageDataLanguage("11-13 วัน", "11-13 days", lang),
                    work = day11_13,
                    area_id = 11
                };

                dataJson.Add(result3);

                var result4 = new
                {
                    label = chageDataLanguage("14-15 วัน", "14-15 days", lang),
                    work = day14_15,
                    area_id = 14
                };
                dataJson.Add(result4);


                var result5 = new
                {
                    label = chageDataLanguage("มากกว่า 15 วัน", "More than 15 days", lang),
                    work = day_more_15,
                    area_id = 16
                };
                dataJson.Add(result5);



                #region oldcode
                //for (int i = 5; i <= 16; i++)
                //{
                //    int day = 0;
                //    int amount_day = 0;
                //    string lb = "";

                //    if (i == 5)
                //    {
                //        var v = from c in dbConnect.incidents
                //                where c.process_status == 1 && c.step_form == 3
                //                && c.confirm_form_two_to_three_at != null
                //                && c.action_form_three_at == null
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.confirm_form_two_to_three_at.Value.AddDays(5)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.confirm_form_two_to_three_at.Value.AddDays(8)
                //                select new
                //                {
                //                    c.incident_date,
                //                    c.id
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.incident_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.incident_date <= d_end);
                //        }


                //        int count = 0;
                //        foreach (var rc in v)
                //        {
                //            var count_row = dbConnect.corrective_prevention_action_incidents.Where(x => x.incident_id == rc.id).Count();
                //            var count_row_fact = dbConnect.fact_findings.Where(x => x.incident_id == rc.id).Count();
                //            if (count_row == 0 && count_row_fact==0)
                //            {
                //                count++;

                //            }
                //        }

                //        amount_day = count;
                //        lb = chageDataLanguage("5-7 วัน", "5-7 day", lang);
                //        day = 5;

                //    }
                //    else if (i == 8)
                //    {
                //        var v = from c in dbConnect.incidents
                //                where c.process_status == 1 && c.step_form == 3
                //               && c.confirm_form_two_to_three_at != null
                //               && c.action_form_three_at == null
                //               && c.country == Session["country"].ToString()
                //               && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.confirm_form_two_to_three_at.Value.AddDays(8)
                //               && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.confirm_form_two_to_three_at.Value.AddDays(11)
                //                select new
                //                {
                //                    c.incident_date,
                //                    c.id
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.incident_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.incident_date <= d_end);
                //        }

                //        int count = 0;
                //        foreach (var rc in v)
                //        {
                //            var count_row = dbConnect.corrective_prevention_action_incidents.Where(x => x.incident_id == rc.id).Count();
                //            var count_row_fact = dbConnect.fact_findings.Where(x => x.incident_id == rc.id).Count();
                //            if (count_row == 0 && count_row_fact == 0)
                //            {
                //                count++;

                //            }
                //        }

                //        amount_day = count;
                //        lb = chageDataLanguage("8-10 วัน", "8-10 days", lang);
                //        day = 8;


                //    }
                //    else if (i == 11)
                //    {
                //        var v = from c in dbConnect.incidents
                //                where c.process_status == 1 && c.step_form == 3
                //                && c.confirm_form_two_to_three_at != null
                //                && c.action_form_three_at == null
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.confirm_form_two_to_three_at.Value.AddDays(11)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.confirm_form_two_to_three_at.Value.AddDays(14)
                //                select new
                //                {
                //                    c.incident_date,
                //                    c.id
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.incident_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.incident_date <= d_end);
                //        }

                //        int count = 0;
                //        foreach (var rc in v)
                //        {
                //            var count_row = dbConnect.corrective_prevention_action_incidents.Where(x => x.incident_id == rc.id).Count();
                //            var count_row_fact = dbConnect.fact_findings.Where(x => x.incident_id == rc.id).Count();
                //            if (count_row == 0 && count_row_fact == 0)
                //            {
                //                count++;

                //            }
                //        }

                //        amount_day = count;
                //        lb = chageDataLanguage("11-13 วัน", "11-13 days", lang);
                //        day = 11;


                //    }
                //    else if (i == 14)
                //    {
                //        var v = from c in dbConnect.incidents
                //                where c.process_status == 1 && c.step_form == 3
                //                && c.confirm_form_two_to_three_at != null
                //                && c.action_form_three_at == null
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.confirm_form_two_to_three_at.Value.AddDays(14)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.confirm_form_two_to_three_at.Value.AddDays(16)
                //                select new
                //                {
                //                    c.incident_date,
                //                    c.id
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.incident_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.incident_date <= d_end);
                //        }

                //        int count = 0;
                //        foreach (var rc in v)
                //        {
                //            var count_row = dbConnect.corrective_prevention_action_incidents.Where(x => x.incident_id == rc.id).Count();
                //            var count_row_fact = dbConnect.fact_findings.Where(x => x.incident_id == rc.id).Count();
                //            if (count_row == 0 && count_row_fact == 0)
                //            {
                //                count++;

                //            }
                //        }

                //        amount_day = count;
                //        lb = chageDataLanguage("14-15 วัน", "14-15 days", lang);
                //        day = 14;


                //    }
                //    else if (i > 15)
                //    {
                //        var v = from c in dbConnect.incidents
                //                where c.process_status == 1 && c.step_form == 3
                //                 && c.confirm_form_two_to_three_at != null
                //                 && c.action_form_three_at == null
                //                 && c.country == Session["country"].ToString()
                //                 && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) > c.confirm_form_two_to_three_at.Value.AddDays(16)
                //                select new
                //                {
                //                    c.incident_date,
                //                    c.id
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.incident_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.incident_date <= d_end);
                //        }

                //        int count = 0;
                //        foreach (var rc in v)
                //        {
                //            var count_row = dbConnect.corrective_prevention_action_incidents.Where(x => x.incident_id == rc.id).Count();
                //            var count_row_fact = dbConnect.fact_findings.Where(x => x.incident_id == rc.id).Count();
                //            if (count_row == 0 && count_row_fact == 0)
                //            {
                //                count++;

                //            }
                //        }

                //        amount_day = count;
                //        lb = chageDataLanguage("มากกว่า 15 วัน", "More than 15 days", lang);
                //        day = 16;


                //    }

                //    if (i == 5 || i == 8 || i == 11 || i == 14 ||  i == 16)
                //    {
                //        var result = new
                //        {
                //            label = lb,
                //            work = amount_day,
                //            area_id = day
                //        };




                //        dataJson.Add(result);
                //    }


                //}

                #endregion
            }
            else
            {

                redirect = "allincident.aspx?form=2to3&day=" + area_id;

            }






            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }


















        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardHazardFormOneToTwo(string area_id,
                                                      string date_start,
                                                      string date_end,
                                                      string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string redirect = "";

            ArrayList dataJson = new ArrayList();


            if (area_id == "")
            {
                var v = from c in dbConnect.hazards
                        where c.process_status == 1 && c.step_form == 1
                        && c.country == Session["country"].ToString()
                        //&& DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.report_date.AddDays(6)
                        //&& DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.report_date.AddDays(7)
                        select new
                        {
                            c.hazard_date,
                            c.report_date
                        };


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.hazard_date <= d_end);
                }

                int day6 = 0;
                int day7 = 0;
                int day8_10 = 0;
                int day11_15 = 0;
                int day_more_15 = 0;



                foreach (var rc in v)
                {
                    DateTime dtnew = rc.report_date;
                    DateTime current = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                    TimeSpan span = current.Subtract(dtnew);
                    int cday = Convert.ToInt16(span.TotalDays);
                    int day = 0;


                    if (cday < 30)
                    {
                        while (dtnew <= current)
                        {
                            dtnew = dtnew.AddDays(1);

                            if (dtnew.DayOfWeek.ToString() != "Saturday" && dtnew.DayOfWeek.ToString() != "Sunday")
                            {
                                var holidays = from c in dbConnect.holidays
                                               where c.holiday_date == dtnew.Date
                                               select new
                                               {
                                                   holiday_date = c.holiday_date,

                                               };

                                bool is_holiday = false;
                                foreach (var r in holidays)
                                {
                                    is_holiday = true;
                                }

                                if (!is_holiday)
                                {
                                    day++;

                                }

                            }//end if

                        }//end while


                        ////////////////////////////////////////////////////////////////////////////////////////////
                        if (day == 6)
                        {
                            day6++;
                        }
                        else if (day == 7)
                        {
                            day7++;

                        }
                        else if (day >=8 & day <=10)
                        {

                            day8_10++;

                        }
                        else if (day >= 11 & day <= 15)
                        {

                            day11_15++;

                        }
                        else if(day > 15)
                        {

                            day_more_15++;
                        }


                    }
                    else
                    {

                        day_more_15++;


                    }


                }//end foreach




                var result1 = new
                {
                    label = chageDataLanguage("6 วัน", "6 day", lang),
                    work = day6,
                    area_id = 6
                };

                dataJson.Add(result1);

                var result2 = new
                {
                    label = chageDataLanguage("7 วัน", "7 days", lang),
                    work = day7,
                    area_id = 7
                };

                dataJson.Add(result2);


                var result3 = new
                {
                    label = chageDataLanguage("8-10 วัน", "8-10 days", lang),
                    work = day8_10,
                    area_id = 8
                };

                dataJson.Add(result3);

                var result4 = new
                {
                    label = chageDataLanguage("11-15 วัน", "11-15 days", lang),
                    work = day11_15,
                    area_id =11
                };
                dataJson.Add(result4);

                var result5 = new
                {
                    label = chageDataLanguage("มากกว่า 15 วัน", "More than 15 days", lang),
                    work = day_more_15,
                    area_id = 16
                };
                dataJson.Add(result5);



                #region oldcode

                //for (int i = 6; i <= 16; i++)
                //{
                //    int day = 0;
                //    int amount_day = 0;
                //    string lb = "";

                //    if (i == 6)
                //    {
                //        var v = from c in dbConnect.hazards
                //                where c.process_status == 1 && c.step_form == 1
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.report_date.AddDays(6)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.report_date.AddDays(7)
                //                select new
                //                {
                //                    c.hazard_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.hazard_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.hazard_date <= d_end);
                //        }


                //        amount_day = v.Count();
                //        lb = chageDataLanguage("6 วัน", "6 day", lang);
                //        day = 6;

                //    }
                //    else if (i == 7)
                //    {
                //        var v = from c in dbConnect.hazards
                //                where c.process_status == 1 && c.step_form == 1
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.report_date.AddDays(7)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.report_date.AddDays(8)
                //                select new
                //                {
                //                    c.hazard_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.hazard_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.hazard_date <= d_end);
                //        }

                //        amount_day = v.Count();
                //        lb = chageDataLanguage("7 วัน", "7 days", lang);
                //        day = 7;


                //    }
                //    else if (i == 8)
                //    {
                //        var v = from c in dbConnect.hazards
                //                where c.process_status == 1 && c.step_form == 1
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.report_date.AddDays(8)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.report_date.AddDays(11)
                //                select new
                //                {
                //                    c.hazard_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.hazard_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.hazard_date <= d_end);
                //        }

                //        amount_day = v.Count();
                //        lb = chageDataLanguage("8-10 วัน", "8-10 days", lang);
                //        day = 8;


                //    }
                //    else if (i == 11)
                //    {
                //        var v = from c in dbConnect.hazards
                //                where c.process_status == 1 && c.step_form == 1
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.report_date.AddDays(11)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.report_date.AddDays(16)
                //                select new
                //                {
                //                    c.hazard_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.hazard_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.hazard_date <= d_end);
                //        }

                //        amount_day = v.Count();
                //        lb = chageDataLanguage("11-15 วัน", "11-15 days", lang);
                //        day = 11;


                //    }
                //    else if (i > 15)
                //    {
                //        var v = from c in dbConnect.hazards
                //                where c.process_status == 1 && c.step_form == 1
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) > c.report_date.AddDays(16)
                //                select new
                //                {
                //                    c.hazard_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.hazard_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.hazard_date <= d_end);
                //        }

                //        amount_day = v.Count();
                //        lb = chageDataLanguage("มากกว่า 15 วัน", "More than 15 days", lang);
                //        day = 16;


                //    }


                //    if (i == 6 || i == 7 || i == 8 || i == 11 || i == 16)
                //    {
                //        var result = new
                //        {
                //            label = lb,
                //            work = amount_day,
                //            area_id = day
                //        };




                //        dataJson.Add(result);
                //    }


                //}//end for loop

                #endregion


            }
            else
            {

                redirect = "allhazard.aspx?form=1to2&day=" + area_id;

            }






            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardHazardFormTwoToThree(string area_id,
                                                      string date_start,
                                                      string date_end,
                                                      string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string redirect = "";

            ArrayList dataJson = new ArrayList();


            if (area_id == "")
            {


                var v = from c in dbConnect.hazards
                        where c.process_status == 1 && c.step_form == 3
                        && c.confirm_form_two_to_three_at != null
                        && c.country == Session["country"].ToString()
                        //&& DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.confirm_form_two_to_three_at.Value.AddDays(6)
                        //&& DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.confirm_form_two_to_three_at.Value.AddDays(7)
                        select new
                        {
                            c.id,
                            c.hazard_date,
                            c.confirm_form_two_to_three_at
                        };


                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.hazard_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.hazard_date <= d_end);
                }

                int day6 = 0;
                int day7 = 0;
                int day8_10 = 0;
                int day11_15 = 0;
                int day_more_15 = 0;



                foreach (var rc in v)
                {
                    var count_row = dbConnect.process_actions.Where(x => x.hazard_id == rc.id).Count();
                    if (count_row == 0)
                    {
                        DateTime dtnew = rc.confirm_form_two_to_three_at.Value;
                        DateTime current = DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString()));
                        TimeSpan span = current.Subtract(dtnew);
                        int cday = Convert.ToInt16(span.TotalDays);
                        int day = 0;


                        if (cday < 30)
                        {
                            while (dtnew <= current)
                            {
                                dtnew = dtnew.AddDays(1);

                                if (dtnew.DayOfWeek.ToString() != "Saturday" && dtnew.DayOfWeek.ToString() != "Sunday")
                                {
                                    var holidays = from c in dbConnect.holidays
                                                   where c.holiday_date == dtnew.Date
                                                   select new
                                                   {
                                                       holiday_date = c.holiday_date,

                                                   };

                                    bool is_holiday = false;
                                    foreach (var r in holidays)
                                    {
                                        is_holiday = true;
                                    }

                                    if (!is_holiday)
                                    {
                                        day++;

                                    }

                                }//end if

                            }//end while


                            ////////////////////////////////////////////////////////////////////////////////////////////
                            if (day == 6)
                            {
                                day6++;
                            }
                            else if (day == 7)
                            {
                                day7++;

                            }
                            else if (day >= 8 & day <= 10)
                            {

                                day8_10++;

                            }
                            else if (day >= 11 & day <= 15)
                            {

                                day11_15++;

                            }
                            else if(day > 15)
                            {

                                day_more_15++;
                            }


                        }
                        else
                        {

                            day_more_15++;


                        }

                    }//end if


                }//end foreach




                var result1 = new
                {
                    label = chageDataLanguage("6 วัน", "6 day", lang),
                    work = day6,
                    area_id = 6
                };

                dataJson.Add(result1);

                var result2 = new
                {
                    label = chageDataLanguage("7 วัน", "7 days", lang),
                    work = day7,
                    area_id = 7
                };

                dataJson.Add(result2);


                var result3 = new
                {
                    label = chageDataLanguage("8-10 วัน", "8-10 days", lang),
                    work = day8_10,
                    area_id = 8
                };

                dataJson.Add(result3);

                var result4 = new
                {
                    label = chageDataLanguage("11-15 วัน", "11-15 days", lang),
                    work = day11_15,
                    area_id = 11
                };
                dataJson.Add(result4);

                var result5 = new
                {
                    label = chageDataLanguage("มากกว่า 15 วัน", "More than 15 days", lang),
                    work = day_more_15,
                    area_id = 16
                };
                dataJson.Add(result5);


                #region oldcode

                //for (int i = 6; i <= 16; i++)
                //{
                //    int day = 0;
                //    int amount_day = 0;
                //    string lb = "";

                //    if (i == 6)
                //    {
                //        var v = from c in dbConnect.hazards
                //                where c.process_status == 1 && c.step_form == 3
                //                && c.confirm_form_two_to_three_at != null
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.confirm_form_two_to_three_at.Value.AddDays(6)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.confirm_form_two_to_three_at.Value.AddDays(7)
                //                select new
                //                {
                //                    c.id,
                //                    c.hazard_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.hazard_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.hazard_date <= d_end);
                //        }

                //        int count = 0;
                //        foreach (var rc in v)
                //        {
                //            var count_row = dbConnect.process_actions.Where(x => x.hazard_id == rc.id).Count();
                //            if (count_row == 0)
                //            {
                //                count++;

                //            }
                //        }

                //        amount_day = count;
                //        lb = chageDataLanguage("6 วัน", "6 day", lang);
                //        day = 6;

                //    }
                //    else if (i == 7)
                //    {
                //        var v = from c in dbConnect.hazards
                //                where c.process_status == 1 && c.step_form == 3
                //               && c.confirm_form_two_to_three_at != null
                //               && c.country == Session["country"].ToString()
                //               && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.confirm_form_two_to_three_at.Value.AddDays(7)
                //               && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.confirm_form_two_to_three_at.Value.AddDays(8)
                //                select new
                //                {
                //                    c.id,
                //                    c.hazard_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.hazard_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.hazard_date <= d_end);
                //        }

                //        int count = 0;
                //        foreach (var rc in v)
                //        {
                //            var count_row = dbConnect.process_actions.Where(x => x.hazard_id == rc.id).Count();
                //            if (count_row == 0)
                //            {
                //                count++;

                //            }
                //        }

                //        amount_day = count;
                //        lb = chageDataLanguage("7 วัน", "7 days", lang);
                //        day = 7;


                //    }
                //    else if (i == 8)
                //    {
                //        var v = from c in dbConnect.hazards
                //                where c.process_status == 1 && c.step_form == 3
                //                && c.confirm_form_two_to_three_at != null
                //                && c.country == Session["country"].ToString()
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.confirm_form_two_to_three_at.Value.AddDays(8)
                //                && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.confirm_form_two_to_three_at.Value.AddDays(11)
                //                select new
                //                {
                //                    c.id,
                //                    c.hazard_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.hazard_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.hazard_date <= d_end);
                //        }

                //        int count = 0;
                //        foreach (var rc in v)
                //        {
                //            var count_row = dbConnect.process_actions.Where(x => x.hazard_id == rc.id).Count();
                //            if (count_row == 0)
                //            {
                //                count++;

                //            }
                //        }

                //        amount_day = count;
                //        lb = chageDataLanguage("8-10 วัน", "8-10 days", lang);
                //        day = 8;


                //    }
                   
                //    else if (i == 11)
                //    {
                //        var v = from c in dbConnect.hazards
                //                where c.process_status == 1 && c.step_form == 3
                //                 && c.confirm_form_two_to_three_at != null
                //                 && c.country == Session["country"].ToString()
                //                 && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) >= c.confirm_form_two_to_three_at.Value.AddDays(11)
                //                 && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) < c.confirm_form_two_to_three_at.Value.AddDays(16)
                //                select new
                //                {
                //                    c.id,
                //                    c.hazard_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.hazard_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.hazard_date <= d_end);
                //        }

                //        int count = 0;
                //        foreach (var rc in v)
                //        {
                //            var count_row = dbConnect.process_actions.Where(x => x.hazard_id == rc.id).Count();
                //            if (count_row == 0)
                //            {
                //                count++;

                //            }
                //        }

                //        amount_day = count;
                //        lb = chageDataLanguage("11-15 วัน", "11-15 days", lang);
                //        day = 11;


                //    }
                //    else if (i > 15)
                //    {
                //        var v = from c in dbConnect.hazards
                //                where c.process_status == 1 && c.step_form == 3
                //                 && c.confirm_form_two_to_three_at != null
                //                 && c.country == Session["country"].ToString()
                //                 && DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())) > c.confirm_form_two_to_three_at.Value.AddDays(16)
                //                select new
                //                {
                //                    c.id,
                //                    c.hazard_date
                //                };


                //        if (date_start != "")
                //        {
                //            DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                //            v = v.Where(c => c.hazard_date >= d_start);
                //        }

                //        if (date_end != "")
                //        {
                //            DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                //            v = v.Where(c => c.hazard_date <= d_end);
                //        }

                //        int count = 0;
                //        foreach (var rc in v)
                //        {
                //            var count_row = dbConnect.process_actions.Where(x => x.hazard_id == rc.id).Count();
                //            if (count_row == 0)
                //            {
                //                count++;

                //            }
                //        }

                //        amount_day = count;
                //        lb = chageDataLanguage("มากกว่า 15 วัน", "More than 15 days", lang);
                //        day = 16;


                //    }

                //    if (i == 6 || i == 7 || i == 8 || i == 11 || i == 16)
                //    {
                //        var result = new
                //        {
                //            label = lb,
                //            work = amount_day,
                //            area_id = day
                //        };




                //        dataJson.Add(result);
                //    }

                //}
                #endregion
            }
            else
            {

                redirect = "allhazard.aspx?form=2to3&day=" + area_id;

            }






            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardIncident10(string area_id,
                                          string date_start,
                                          string date_end,
                                          string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            int ONPROCESS = 1;
            int REJECT = 3;
            int CLOSE = 2;


            string type = "";
            string company_id = "";
            string function_id = "";
            string department_id = "";
            string division_id = "";
            string redirect = "";

            if (area_id != "")
            {
                var companys = from c in dbConnect.companies
                               where c.company_id == area_id && c.country == Session["country"].ToString()
                               select c;

                if (companys.Count() > 0)
                {
                    type = "company";
                    company_id = area_id;

                }


                var functions = from f in dbConnect.functions
                                where f.function_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (functions.Count() > 0)
                {
                    type = "function";
                    function_id = area_id;
                }


                var departments = from f in dbConnect.departments
                                  where f.department_id == area_id && f.country == Session["country"].ToString()
                                  select f;
                if (departments.Count() > 0)
                {
                    type = "department";
                    department_id = area_id;
                }


                var divisions = from f in dbConnect.divisions
                                where f.division_id == area_id && f.country == Session["country"].ToString()
                                select f;
                if (divisions.Count() > 0)
                {
                    type = "division";
                    division_id = area_id;
                }

            }
            else
            {
                type = "all";

            }



            ArrayList dataJson = new ArrayList();



            string label_all = "";
            if (type == "all")
            {

                var n = from c in dbConnect.incidents 
                        where  c.country == Session["country"].ToString()
                        && c.process_status == ONPROCESS
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count();
                label_all = chageDataLanguage("INSEE Lanka", "INSEE Lanka", lang);


                var v = from c in dbConnect.incidents
                        where c.country == Session["country"].ToString()
                        && c.process_status == CLOSE
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count();


                var cc = from c in dbConnect.incidents
                         where c.country == Session["country"].ToString()
                         && c.process_status == REJECT
                         select new
                         {
                             c.id,
                             c.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_reject_all = cc.Count();

                var result = new
                {
                    label = label_all,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    reject = count_incident_reject_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////


                DataSet ds = new DataSet();
                string sql = "select c.company_id,c.company_th,c.company_en,ISNULL(n.count_select,0) as count_select from (select i.company_id,ISNULL(count(i.company_id),0) as count_select from incident i ";
                sql = sql + " where 1=1 ";
                //  sql = sql + "where 1=1 and i.country ='" + Session["country"].ToString() + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.company_id) as n ";
                sql = sql + "right join company c on c.company_id = n.company_id";
                sql = sql + " where c.country ='" + Session["country"].ToString() + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_company = chageDataLanguage(ds.Tables[0].Rows[i]["company_th"].ToString(), ds.Tables[0].Rows[i]["company_en"].ToString(), lang);


                    var n2 = from c in dbConnect.incidents
                             where c.country == Session["country"].ToString()
                             && c.process_status == ONPROCESS
                             && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_company = n2.Count();



                    var v2 = from c in dbConnect.incidents
                             where c.country == Session["country"].ToString()
                             && c.process_status == CLOSE
                             && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_company = v2.Count();


                    var cc2 = from c in dbConnect.incidents
                              where c.country == Session["country"].ToString()
                              && c.process_status == REJECT
                              && c.company_id == ds.Tables[0].Rows[i]["company_id"].ToString()
                              select new
                              {
                                  c.id,
                                  c.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_reject_company = cc2.Count();

                    var result2 = new
                    {
                        label = label_company,
                        onprocess = count_onprocess_company,
                        close = count_close_company,
                        reject = count_reject_company,
                        area_id = ds.Tables[0].Rows[i]["company_id"]
                    };


                    dataJson.Add(result2);

                }




            }
            else if (type == "company")
            {
                string label_company = "";
                var cu = from c in dbConnect.companies
                         where c.company_id == company_id
                         select c;

                foreach (var c in cu)
                {
                    label_company = chageDataLanguage(c.company_th, c.company_en, lang);
                }


                var n = from c in dbConnect.incidents
                        where c.country == Session["country"].ToString()
                        && c.process_status == ONPROCESS
                        && c.company_id == company_id
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count();


                var v = from c in dbConnect.incidents
                        where c.country == Session["country"].ToString()
                        && c.process_status == CLOSE
                        && c.company_id == company_id
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count();


                var cc = from c in dbConnect.incidents
                         where c.country == Session["country"].ToString()
                         && c.process_status == REJECT
                         && c.company_id == company_id
                         select new
                         {
                             c.id,
                             c.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_reject_all = cc.Count();

                var result = new
                {
                    label = label_company,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    reject = count_incident_reject_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////

                DataSet ds = new DataSet();
                string sql = "select f.function_id,f.function_th,f.function_en,ISNULL(n.count_select,0) as count_select from (select i.function_id,ISNULL(count(i.function_id),0) as count_select from incident i ";
                sql = sql + "where 1=1 and i.company_id='" + company_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + " group by i.function_id) as n ";
                sql = sql + "right join [function] f on f.function_id = n.function_id where  f.function_en !='-' and f.company_id='" + company_id + "' ";

               
                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_function = chageDataLanguage(ds.Tables[0].Rows[i]["function_th"].ToString(), ds.Tables[0].Rows[i]["function_en"].ToString(), lang);


                    var n2 = from c in dbConnect.incidents
                             where c.country == Session["country"].ToString()
                             && c.process_status == ONPROCESS 
                             && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_function = n2.Count();



                    var v2 = from c in dbConnect.incidents
                             where c.country == Session["country"].ToString()
                             && c.process_status == CLOSE
                             && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_function = v2.Count();


                    var cc2 = from c in dbConnect.incidents
                              where c.country == Session["country"].ToString()
                              && c.process_status == REJECT
                              && c.function_id == ds.Tables[0].Rows[i]["function_id"].ToString()
                              select new
                              {
                                  c.id,
                                  c.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_reject_function = cc2.Count();

                    var result2 = new
                    {
                        label = label_function,
                        onprocess = count_onprocess_function,
                        close = count_close_function,
                        reject = count_reject_function,
                        area_id = ds.Tables[0].Rows[i]["function_id"]
                    };


                    dataJson.Add(result2);

                }

            }
            else if (type == "function")
            {
                string label_function = "";
                var fu = from c in dbConnect.functions
                         where c.function_id == function_id
                         select c;

                foreach (var f in fu)
                {
                    label_function = chageDataLanguage(f.function_th, f.function_en, lang);
                }


                var n = from c in dbConnect.incidents
                        where c.country == Session["country"].ToString()
                        && c.process_status == ONPROCESS
                        && c.function_id == function_id
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count();


                var v = from c in dbConnect.incidents
                        where c.country == Session["country"].ToString()
                        && c.process_status == CLOSE
                        && c.function_id == function_id
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count();


                var cc = from c in dbConnect.incidents
                         where c.country == Session["country"].ToString()
                         && c.process_status == REJECT
                         && c.function_id == function_id
                         select new
                         {
                             c.id,
                             c.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_reject_all = cc.Count();

                var result = new
                {
                    label = label_function,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    reject = count_incident_reject_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////




                DataSet ds = new DataSet();
                string sql = "select n.department_id,d.department_th,d.department_en,n.count_select from (select i.department_id,ISNULL(count(i.department_id),0) as count_select from incident i ";
                sql = sql + "where 1=1  and i.function_id='" + function_id + "' ";


                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }

                sql = sql + "group by i.department_id) as n ";
                sql = sql + "right join department d on d.department_id = n.department_id ";
                sql = sql + " where d.function_id ='" + function_id + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string label_department = chageDataLanguage(ds.Tables[0].Rows[i]["department_th"].ToString(), ds.Tables[0].Rows[i]["department_en"].ToString(), lang);

                    var n2 = from c in dbConnect.incidents
                             where c.country == Session["country"].ToString()
                             && c.process_status == ONPROCESS
                             && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_department = n2.Count();



                    var v2 = from c in dbConnect.incidents
                             where c.country == Session["country"].ToString()
                             && c.process_status == CLOSE
                             && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_department = v2.Count();


                    var cc2 = from c in dbConnect.incidents
                              where c.country == Session["country"].ToString()
                              && c.process_status == REJECT
                              && c.department_id == ds.Tables[0].Rows[i]["department_id"].ToString()
                              select new
                              {
                                  c.id,
                                  c.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_reject_department = cc2.Count();

                    var result2 = new
                    {
                        label = label_department,
                        onprocess = count_onprocess_department,
                        close = count_close_department,
                        reject = count_reject_department,
                        area_id = ds.Tables[0].Rows[i]["department_id"]
                    };


                    dataJson.Add(result2);
                }


            }
            else if (type == "department")
            {

                var de = from d in dbConnect.departments
                         where d.department_id == department_id
                         select d;
                foreach (var u in de)
                {
                    label_all = chageDataLanguage(u.department_th, u.department_en, lang);
                }




                var n = from c in dbConnect.incidents
                        where c.country == Session["country"].ToString()
                        && c.process_status == ONPROCESS
                        && c.department_id == department_id
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    n = n.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    n = n.Where(c => c.incident_date <= d_end);
                }

                int count_incident_onprocess_all = n.Count();


                var v = from c in dbConnect.incidents
                        where c.country == Session["country"].ToString()
                        && c.process_status == CLOSE
                        && c.department_id == department_id
                        select new
                        {
                            c.id,
                            c.incident_date
                        };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    v = v.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    v = v.Where(c => c.incident_date <= d_end);
                }


                int count_incident_close_all = v.Count();


                var cc = from c in dbConnect.incidents
                         where c.country == Session["country"].ToString()
                         && c.process_status == REJECT
                         && c.department_id == department_id
                         select new
                         {
                             c.id,
                             c.incident_date
                         };

                if (date_start != "")
                {
                    DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                    cc = cc.Where(c => c.incident_date >= d_start);
                }

                if (date_end != "")
                {
                    DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                    cc = cc.Where(c => c.incident_date <= d_end);
                }


                int count_incident_reject_all = cc.Count();

                var result = new
                {
                    label = label_all,
                    onprocess = count_incident_onprocess_all,
                    close = count_incident_close_all,
                    reject = count_incident_reject_all,
                    area_id = ""
                };


                dataJson.Add(result);

                ///////////////////////////////////////////end first row/////////////////////////////////////////////////////



                DataSet ds = new DataSet();
                string sql = "select n.division_id,d.division_th,d.division_en,n.count_select from (select i.division_id,ISNULL(count(i.department_id),0) as count_select from incident i ";
                sql = sql + " where 1=1 and i.department_id='" + department_id + "' ";

                if (date_start != "")
                {
                    string d_start = FormatDates.changeDateTimeReport(date_start + " " + "00:00", lang);

                    sql = sql + " and i.incident_date >='" + d_start + "'";

                }

                if (date_end != "")
                {
                    string d_end = FormatDates.changeDateTimeReport(date_end + " " + "23:59", lang);
                    sql = sql + " and i.incident_date <='" + d_end + "'";
                }
                sql = sql + "group by i.division_id ) as n ";
                sql = sql + "right join division d on d.division_id = n.division_id";
                sql = sql + " where d.department_id ='" + department_id + "' ";

                ds = DatabaseConnector.GetData(sql);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //ds.Tables[0].Rows[i]["typeparty"].ToString();
                    string label_division = chageDataLanguage(ds.Tables[0].Rows[i]["division_th"].ToString(), ds.Tables[0].Rows[i]["division_en"].ToString(), lang);
                    var n2 = from c in dbConnect.incidents
                             where c.country == Session["country"].ToString()
                             && c.process_status == ONPROCESS
                             && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        n2 = n2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        n2 = n2.Where(c => c.incident_date <= d_end);
                    }

                    int count_onprocess_department = n2.Count();



                    var v2 = from c in dbConnect.incidents
                             where c.country == Session["country"].ToString()
                             && c.process_status == CLOSE
                             && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                             select new
                             {
                                 c.id,
                                 c.incident_date
                             };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        v2 = v2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        v2 = v2.Where(c => c.incident_date <= d_end);
                    }


                    int count_close_department = v2.Count();


                    var cc2 = from c in dbConnect.incidents
                              where c.country == Session["country"].ToString()
                              && c.process_status == REJECT
                              && c.division_id == ds.Tables[0].Rows[i]["division_id"].ToString()
                              select new
                              {
                                  c.id,
                                  c.incident_date
                              };

                    if (date_start != "")
                    {
                        DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                        cc2 = cc2.Where(c => c.incident_date >= d_start);
                    }

                    if (date_end != "")
                    {
                        DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                        cc2 = cc2.Where(c => c.incident_date <= d_end);
                    }


                    int count_reject_department = cc2.Count();

                    var result2 = new
                    {
                        label = label_division,
                        onprocess = count_onprocess_department,
                        close = count_close_department,
                        reject = count_reject_department,
                        area_id = ds.Tables[0].Rows[i]["division_id"]
                    };


                    dataJson.Add(result2);


                }

            }
            else if (type == "division")
            {
                string department_id2 = "";
                string function_id2 = "";
                string company_id2 = "";

                var divisions2 = from f in dbConnect.divisions
                                 where f.division_id == division_id
                                 select f;

                foreach (var di in divisions2)
                {
                    department_id2 = di.department_id;

                    var departments2 = from f in dbConnect.departments
                                       where f.department_id == department_id2
                                       select f;

                    foreach (var de in departments2)
                    {
                        function_id2 = de.function_id;


                        var functions2 = from f in dbConnect.functions
                                         where f.function_id == function_id2
                                         select f;

                        foreach (var fun in functions2)
                        {
                            company_id2 = fun.company_id;

                        }

                    }
                }

                redirect = "allincident.aspx?company_id=" + company_id2 + "&function_id=" + function_id2 + "&department_id=" + department_id2 + "&division_id=" + division_id;





            }

            var returnv = new
            {
                result = dataJson,
                redirect = redirect


            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }




        public List<string> getCategoriesBySite(string date_start, string date_end, int categorie, string country, string lang)
        {


            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();
            int REJECT = 3;

            var n = from c in dbConnect.injury_persons
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                    && c.severity_injury_id == categorie
                    && i.process_status != REJECT
                    && i.country == country
                    && c.status == "A"
                    select new
                    {
                        i.id,
                        i.incident_date,
                        i.section_id,
                        i.division_id,
                        i.department_id,
                        i.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.incident_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }



        public List<string> getNearmissBySite(string date_start, string date_end, string country, string lang)
        {


            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();
            int REJECT = 3;

            var n = from c in dbConnect.incidents
                    where c.work_relate == "Y" && (c.culpability == "G" || c.culpability == "P")
                    && c.impact == "N"
                    && c.country == country
                    select new
                    {
                        c.id,
                        c.incident_date,
                        c.section_id,
                        c.division_id,
                        c.department_id,
                        c.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.incident_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }

        public List<string> getProcessStatusBySite(string date_start,string date_end,int process_status,string country,string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();
            var n = from c in dbConnect.hazards
                    where c.country == country
                    && c.process_status == process_status
                    select new
                    {
                        c.id,
                        c.hazard_date,
                        c.section_id,
                        c.division_id,
                        c.department_id,
                        c.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.hazard_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.hazard_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }



        public List<string> getIncidentProcessStatusBySite(string date_start, string date_end, int process_status, string country, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();
            var n = from c in dbConnect.incidents
                    where c.country == country
                    && c.process_status == process_status
                    select new
                    {
                        c.id,
                        c.incident_date,
                        c.section_id,
                        c.division_id,
                        c.department_id,
                        c.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.incident_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }




        public List<string> getActionStatusOnprocessBySite(string date_start, string date_end,string country,string timezone, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();

            int ONPROCESS = 1;
            int REJECT = 6;
            int REQUEST_CLOSE = 2;
            int CLOSE = 4;
            int CANCEL = 5;

            var n = from c in dbConnect.process_actions
                    join h in dbConnect.hazards on c.hazard_id equals h.id
                    where (c.action_status_id == ONPROCESS//1 is on process
                    || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && h.country == country
                    select new
                    {
                        c.id,
                        h.hazard_date,
                        h.section_id,
                        h.division_id,
                        h.department_id,
                        h.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.hazard_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.hazard_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }

        public List<string> getAllHazardwithreporterBySite(string date_start, string date_end, string country, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();


            var n = from c in dbConnect.hazards
                   // join e in dbConnect.employees on c.employee_id equals e.employee_id
                    //join o in dbConnect.organizations on e.unit_id equals o.org_unit_id into joinO

                   // from o in joinO.DefaultIfEmpty()
                    where  c.country == country
                    select new
                    {
                        c.id,
                        c.hazard_date,                   
                        c.reporter_company_id,
                        c.reporter_function_id,
                        c.reporter_department_id,
                        c.reporter_division_id,
                        c.reporter_section_id,
          
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.hazard_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.hazard_date <= d_end);
            }



            foreach (var s in n)
            {

                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.reporter_division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.reporter_department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                                var v4 = from c in dbConnect.organizations
                                         where c.country == country
                                         && c.org_unit_id == s.reporter_function_id
                                         orderby c.personnel_subarea ascending
                                         select new
                                         {
                                             id = c.personnel_subarea,
                                             name = c.personnel_subarea_description

                                         };
                                if (v4.Count() > 0)
                                {
                                    foreach (var rc in v4)
                                    {
                                        ls_sub.Add(rc.id);
                                    }
                                }


                            

                        }

                    }

             

            }// end foreach


            return ls_sub;

        }
        public List<string> getIncidentActionStatusOnprocessBySite(string date_start, string date_end, string country, string timezone, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();

            int ONPROCESS = 1;
            int REJECT = 6;
            int REQUEST_CLOSE = 2;
            int CLOSE = 4;
            int CANCEL = 5;

            var n = from c in dbConnect.corrective_prevention_action_incidents
                    join h in dbConnect.incidents on c.incident_id equals h.id
                    where (c.action_status_id == ONPROCESS//1 is on process
                    || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                        && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date : Convert.ToDateTime(c.date_complete).Date)
                        <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                        && h.country == country
                    select new
                    {
                        c.id,
                        h.incident_date,
                        h.section_id,
                        h.division_id,
                        h.department_id,
                        h.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.incident_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }


        public List<string> getIncidentInjuryActionStatusOnprocessBySite(string date_start, string date_end, string country, string timezone, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();

            int ONPROCESS = 1;
            int REJECT = 6;
            int REQUEST_CLOSE = 2;
            int CLOSE = 4;
            int CANCEL = 5;

            int F = 7;
            int PD = 8;
            int LTI = 9;
            int MTI = 10;
            int MI = 11;

            var n = from c in dbConnect.corrective_prevention_action_incidents
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                    where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                     && (c.action_status_id == ONPROCESS//1 is on process
                   || c.action_status_id == REQUEST_CLOSE || c.action_status_id == REJECT)
                    && (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date : Convert.ToDateTime(c.date_complete).Date)
                    <= Convert.ToDateTime(c.due_date).Date//ไม่นับตัว delay
                    && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI)
                    && i.country == country
                    && ju.status == "A"
                    group c by new
                    {
                        c.id,
                        i.incident_date,
                        i.section_id,
                        i.division_id,
                        i.department_id,
                        i.function_id
                    } into g
                    select new
                    {
                        g.Key.id,
                        g.Key.incident_date,
                        g.Key.section_id,
                        g.Key.division_id,
                        g.Key.department_id,
                        g.Key.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.incident_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }




        public List<string> getActionStatusCloseBySite(string date_start, string date_end, string country, string timezone, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();


            int CLOSE = 4;

            var n = from c in dbConnect.process_actions
                    join h in dbConnect.hazards on c.hazard_id equals h.id
                    where c.action_status_id == CLOSE
                        && h.country == country
                    select new
                    {
                        c.id,
                        h.hazard_date,
                        h.section_id,
                        h.division_id,
                        h.department_id,
                        h.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.hazard_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.hazard_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }


        public List<string> getIncidentActionStatusCloseBySite(string date_start, string date_end, string country, string timezone, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();


            int CLOSE = 4;

            var n = from c in dbConnect.corrective_prevention_action_incidents
                    join h in dbConnect.incidents on c.incident_id equals h.id
                    where c.action_status_id == CLOSE
                        && h.country == country
                    select new
                    {
                        c.id,
                        h.incident_date,
                        h.section_id,
                        h.division_id,
                        h.department_id,
                        h.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.incident_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }



        public List<string> getIncidentInjuryActionStatusCloseBySite(string date_start, string date_end, string country, string timezone, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();


            int ONPROCESS = 1;
            int REJECT = 6;
            int REQUEST_CLOSE = 2;
            int CLOSE = 4;
            int CANCEL = 5;

            int F = 7;
            int PD = 8;
            int LTI = 9;
            int MTI = 10;
            int MI = 11;


            var n = from c in dbConnect.corrective_prevention_action_incidents
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                    where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                    && c.action_status_id == CLOSE//
                    && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                    || ju.severity_injury_id == MTI || ju.severity_injury_id == MI)
                    && i.country == country
                    && ju.status == "A"
                    group c by new
                    {
                        c.id,
                        i.incident_date,
                        i.section_id,
                        i.division_id,
                        i.department_id,
                        i.function_id
                    } into g
                    select new
                    {
                        g.Key.id,
                        g.Key.incident_date,
                        g.Key.section_id,
                        g.Key.division_id,
                        g.Key.department_id,
                        g.Key.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.incident_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }



        public List<string> getActionStatusDelayBySite(string date_start, string date_end, string country, string timezone, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();


            int ONPROCESS = 1;
            int REJECT = 6;
            int REQUEST_CLOSE = 2;
            int CLOSE = 4;
            int CANCEL = 5;

            var n = from c in dbConnect.process_actions
                    join h in dbConnect.hazards on c.hazard_id equals h.id
                    where ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                       (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date : Convert.ToDateTime(c.date_complete).Date)
                       > Convert.ToDateTime(c.due_date).Date)//delay
                       && h.country == country
                    select new
                    {
                        c.id,
                        h.hazard_date,
                        h.section_id,
                        h.division_id,
                        h.department_id,
                        h.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.hazard_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.hazard_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }



        public List<string> getIncidentActionStatusDelayBySite(string date_start, string date_end, string country, string timezone, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();


            int ONPROCESS = 1;
            int REJECT = 6;
            int REQUEST_CLOSE = 2;
            int CLOSE = 4;
            int CANCEL = 5;

            var n = from c in dbConnect.corrective_prevention_action_incidents
                    join h in dbConnect.incidents on c.incident_id equals h.id
                    where ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                       (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date : Convert.ToDateTime(c.date_complete).Date)
                       > Convert.ToDateTime(c.due_date).Date)//delay
                       && h.country == country
                    select new
                    {
                        c.id,
                        h.incident_date,
                        h.section_id,
                        h.division_id,
                        h.department_id,
                        h.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.incident_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }


        public List<string> getIncidentInjuryActionStatusDelayBySite(string date_start, string date_end, string country, string timezone, string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            List<string> ls_sub = new List<string>();

            int ONPROCESS = 1;
            int REJECT = 6;
            int REQUEST_CLOSE = 2;
            int CLOSE = 4;
            int CANCEL = 5;

            int F = 7;
            int PD = 8;
            int LTI = 9;
            int MTI = 10;
            int MI = 11;


            var n = from c in dbConnect.corrective_prevention_action_incidents
                    join i in dbConnect.incidents on c.incident_id equals i.id
                    join ju in dbConnect.injury_persons on i.id equals ju.incident_id
                    where i.work_relate == "Y" && (i.culpability == "G" || i.culpability == "P")
                     && ((c.action_status_id != CLOSE && c.action_status_id != CANCEL) &&
                   (c.date_complete == null ? DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date : Convert.ToDateTime(c.date_complete).Date)
                   > Convert.ToDateTime(c.due_date).Date)//delay
                    && (ju.severity_injury_id == F || ju.severity_injury_id == PD || ju.severity_injury_id == LTI
                   || ju.severity_injury_id == MTI || ju.severity_injury_id == MI)
                   && i.country == country
                   && ju.status == "A"
                    group c by new
                    {
                        c.id,
                        i.incident_date,
                        i.section_id,
                        i.division_id,
                        i.department_id,
                        i.function_id
                    } into g
                    select new
                    {
                        g.Key.id,
                        g.Key.incident_date,
                        g.Key.section_id,
                        g.Key.division_id,
                        g.Key.department_id,
                        g.Key.function_id
                    };

            if (date_start != "")
            {
                DateTime d_start = FormatDates.changeDateTimeDB(date_start + " " + "00:00", lang);

                n = n.Where(c => c.incident_date >= d_start);
            }

            if (date_end != "")
            {
                DateTime d_end = FormatDates.changeDateTimeDB(date_end + " " + "23:59", lang);
                n = n.Where(c => c.incident_date <= d_end);
            }



            foreach (var s in n)
            {

                var v = from c in dbConnect.organizations
                        where c.country == country
                        && c.org_unit_id == s.section_id
                        orderby c.personnel_subarea ascending
                        select new
                        {
                            id = c.personnel_subarea,
                            name = c.personnel_subarea_description

                        };

                if (v.Count() > 0)
                {
                    foreach (var rc in v)
                    {
                        ls_sub.Add(rc.id);
                    }
                }
                else
                {
                    var v1 = from c in dbConnect.organizations
                             where c.country == country
                             && c.org_unit_id == s.division_id
                             orderby c.personnel_subarea ascending
                             select new
                             {
                                 id = c.personnel_subarea,
                                 name = c.personnel_subarea_description

                             };
                    if (v1.Count() > 0)
                    {
                        foreach (var rc in v1)
                        {
                            ls_sub.Add(rc.id);
                        }
                    }
                    else
                    {
                        var v2 = from c in dbConnect.organizations
                                 where c.country == country
                                 && c.org_unit_id == s.department_id
                                 orderby c.personnel_subarea ascending
                                 select new
                                 {
                                     id = c.personnel_subarea,
                                     name = c.personnel_subarea_description

                                 };
                        if (v2.Count() > 0)
                        {
                            foreach (var rc in v2)
                            {
                                ls_sub.Add(rc.id);
                            }
                        }
                        else
                        {

                            var v3 = from c in dbConnect.organizations
                                     where c.country == country
                                     && c.org_unit_id == s.function_id
                                     orderby c.personnel_subarea ascending
                                     select new
                                     {
                                         id = c.personnel_subarea,
                                         name = c.personnel_subarea_description

                                     };
                            if (v3.Count() > 0)
                            {
                                foreach (var rc in v3)
                                {
                                    ls_sub.Add(rc.id);
                                }
                            }

                        }

                    }

                }

            }// end foreach


            return ls_sub;

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void getDashboardSOT1(string company_id,
                                    string function_id,
                                    string department_id,
                                    string division_id,
                                    string area_id,
                                    string mnglevel,
                                    string date_start,
                                    string date_end,
                                    string lang)
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();

            string type = "";
            string redirect = "";

            ArrayList dataJson = new ArrayList();

            if (area_id != "")
            {
                type = "department";
            }
            else
            {

                type = "all";
            }



            if (type == "all")
            {
               

                var ve = from c in dbConnect.employees
                         join o in dbConnect.organizations on c.unit_id equals o.org_unit_id into joinO
                         from o in joinO.DefaultIfEmpty()
                         where c.country == Session["country"].ToString()
                         && c.mngt_level == mnglevel
                         && c.status == "Active"
                         orderby c.first_name_en ascending
                         select new
                         {
                             name = c.first_name_en + " " + c.last_name_en,
                             c.employee_id,
                             o.company_id,
                             o.function_id,
                             o.sub_function_id,
                             o.department_id

                         };

                if (company_id != "")
                {
                    ve = ve.Where(c => c.company_id == company_id);

                }

                if (function_id != "")
                {
                    ve = ve.Where(c => c.function_id == function_id);

                }

                if (department_id != "")
                {
                    ve = ve.Where(c => c.sub_function_id == department_id);

                }


                if (division_id != "")
                {
                    ve = ve.Where(c => c.department_id == division_id);

                }

                foreach (var rc in ve)
                {

                    var na = from a in dbConnect.employee_has_sots
                             where a.employee_id == rc.employee_id
                             select new
                             {
                                 a.sot_id
                             };

                    int count = na.Count();

                    var result = new
                    {
                        label = rc.name,
                        sot = count,
                        area_id = ""// rc.id
                    };


                    dataJson.Add(result);


                }

            }
            else if (type == "department")
            {



            }




            var returnv = new
            {
                result = dataJson,
                redirect = redirect
            };




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(returnv));

        }


        public string chageDataLanguage(string vTH, string vEN, string lang)
        {
            string vReturn = "";

            if (lang == "th")
            {

                vReturn = vTH;

            }
            else if (lang == "en")
            {

                vReturn = vEN;

            }else if (lang == "si")
            {

                vReturn = vEN;

            }


            return vReturn;
        }
    }
}
