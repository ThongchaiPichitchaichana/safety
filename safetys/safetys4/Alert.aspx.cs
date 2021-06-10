using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace safetys4
{
    public partial class Alert : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            String alert_type = Request.QueryString["alert_type"];

            if (DateTime.Now.DayOfWeek.ToString() == "Saturday" || DateTime.Now.DayOfWeek.ToString() == "Sunday")
            {
                Response.End();
            }

            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            var holidays = from c in dbConnect.holidays
                           where c.holiday_date == DateTime.Now.Date
                           select new
                           {
                               holiday_date = c.holiday_date,

                           };

            bool is_holiday = false;
            foreach (var v in holidays)
            {
                is_holiday = true;
            }

            if (is_holiday)
            {
                Response.End();
            }

            this.incidentNoActive();
            this.incidentBossNoActive();
            this.hazardNoActive();
            this.hazardBossNoActive();
            this.incidentActionDueToday();
            this.incidentActionOverDueToday();
            this.hazardActionDueToday();
            this.hazardActionOverDueToday();
            this.incidentFormTwoToThreeNoActive();

            this.incidentFormTwoToThreeBossNoActive();

            this.incidentFormThreeToFourNoActive();
            this.incidentFormThreeToFourStepNoActive();
            this.hazardFormTwoToThreeNoActive();

            this.hazardFormTwoToThreeBossNoActive();

            this.hazardFormThreeToFourNoActive();
            this.hazardFormThreeToFourStepNoActive();
            this.healthFormTwoToThreeNoActive();
            this.healthFormTwoToThreeStepNoActive();
            this.healthNoActive();
            this.healthActionDueToday();
            this.healthActionOverDueToday();

        }

        private string getTimezone(string country)
        {
            string timezone = "";

            if (country == "srilanka")
            {
                timezone = "+5.5";
            }
            else if (country == "thailand")
            {
                timezone = "+7";
            }

            return timezone;
        }

        private void incidentNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var incidents = from c in dbConnect.incidents
                            where c.process_status == 1 && c.step_form == 1 /*&& DateTime.Now > c.report_date.AddHours(24) && DateTime.Now > c.last_alerted_at.AddHours(24)*/

                            select new
                            {
                                function_id = c.function_id,
                                department_id = c.department_id,
                                division_id = c.division_id,
                                section_id = c.section_id,
                                incident_id = c.id,
                                incident_code = c.doc_no,
                                title = c.incident_name,
                                description = c.incident_detail,
                                occur_date = c.incident_date,
                                is_alert_over_due = c.is_alert_over_due,
                                report_date = c.report_date,
                                last_alerted_at = c.last_alerted_at,
                                country = c.country
                            };

            foreach (var v in incidents)
            {
                string timezone = this.getTimezone(v.country);
                bool rs = addday(Convert.ToDateTime(v.report_date, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.report_date.AddHours(24)
                bool rs2 = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.last_alerted_at.AddHours(24)

                if (rs == true && rs2 == true)
                {
                    safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();
                    //sn.InsertNotification()
                    string[] alert_to_groups = { "AdminOH&S", "AreaSuperervisor", "AreaManager", "AreaOH&S" };

                    if (v.country == "thailand")
                    {
                        sn.InsertNotification(18, v.incident_id, alert_to_groups, timezone, "AreaSuperervisor");
                    }
                    else if (v.country == "srilanka")
                    {
                        sn.InsertNotification(18, v.incident_id, alert_to_groups, timezone, "AreaSuperervisor");
                    }


                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    incident updateObj = dbConnectU.incidents.Single(n => n.id == v.incident_id);
                    updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();
                }

            }
        }




        private void incidentBossNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var incidents = from c in dbConnect.incidents
                            where c.process_status == 1 && c.step_form == 1
                            select new
                            {
                                function_id = c.function_id,
                                department_id = c.department_id,
                                division_id = c.division_id,
                                section_id = c.section_id,
                                incident_id = c.id,
                                incident_code = c.doc_no,
                                title = c.incident_name,
                                description = c.incident_detail,
                                occur_date = c.incident_date,
                                is_alert_over_due = c.is_alert_over_due,
                                report_date = c.report_date,
                                c.last_alerted_boss_at,
                                country = c.country
                            };

            foreach (var v in incidents)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.report_date, CultureInfo.InvariantCulture), 3, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.report_date.AddDays(3)
                bool rs2 = addday(Convert.ToDateTime(v.last_alerted_boss_at, CultureInfo.InvariantCulture), 3, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.last_alerted_boss_at.Value.AddDays(3)

                if (rs == true && rs2 == true)
                {
                    safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();
                    //sn.InsertNotification()
                    string[] alert_to_groups = {"AreaSuperervisor"};

                    if (v.country == "thailand")
                    {
                        sn.InsertNotification(22, v.incident_id, alert_to_groups, timezone, "AreaSuperervisor");
                    }



                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    incident updateObj = dbConnectU.incidents.Single(n => n.id == v.incident_id);
                    updateObj.last_alerted_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();
                }

            }
        }

        private void hazardNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var incidents = from c in dbConnect.hazards
                            where c.process_status == 1 && c.step_form == 1 //&& DateTime.Now > c.report_date.AddDays(7) && DateTime.Now > c.last_alerted_at.AddHours(24)
                            select new
                            {
                                function_id = c.function_id,
                                department_id = c.department_id,
                                division_id = c.division_id,
                                section_id = c.section_id,
                                incident_id = c.id,
                                incident_code = c.doc_no,
                                c.report_date,
                                c.last_alerted_at,
                                c.country

                            };

            foreach (var v in incidents)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.report_date, CultureInfo.InvariantCulture), 7, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.report_date.AddDays(7)
                bool rs2 = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.last_alerted_at.AddHours(24)

                if (rs == true && rs2 == true)
                {
                    safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();
                    //sn.InsertNotification()


                    if (v.country == "thailand")
                    {
                        string[] alert_to_groups = { "AreaSuperervisor", "AreaOH&S", "AdminOH&S" };
                        sn.InsertHazardNotification(15, v.incident_id, alert_to_groups, timezone, "AreaOH&S");

                    }
                    else if (v.country == "srilanka")
                    {
                        string[] alert_to_groups = { "AreaSuperervisor", "AreaOH&S" };
                        sn.InsertHazardNotification(15, v.incident_id, alert_to_groups, timezone, "AreaSuperervisor");

                    }

                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    hazard updateObj = dbConnectU.hazards.Single(n => n.id == v.incident_id);
                    updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();
                }

            }
        }


        private void hazardBossNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var incidents = from c in dbConnect.hazards
                            where c.process_status == 1 && c.step_form == 1
                            select new
                            {
                                function_id = c.function_id,
                                department_id = c.department_id,
                                division_id = c.division_id,
                                section_id = c.section_id,
                                incident_id = c.id,
                                incident_code = c.doc_no,
                                c.report_date,
                                c.last_alerted_boss_at,
                                c.country

                            };

            foreach (var v in incidents)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.report_date, CultureInfo.InvariantCulture), 7, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.report_date.AddDays(7)
                bool rs2 = addday(Convert.ToDateTime(v.last_alerted_boss_at, CultureInfo.InvariantCulture), 7, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.last_alerted_boss_at.Value.AddDays(7)

                if (rs == true && rs2 == true)
                {
                    safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();
                    //sn.InsertNotification()


                    if (v.country == "thailand")
                    {
                        string[] alert_to_groups = { "AreaOH&S" };
                        sn.InsertHazardNotification(19, v.incident_id, alert_to_groups, timezone, "AreaOH&S");

                    }


                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    hazard updateObj = dbConnectU.hazards.Single(n => n.id == v.incident_id);
                    updateObj.last_alerted_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();
                }

            }
        }


        private void incidentActionDueToday()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();

            var actions = from c in dbConnect.corrective_prevention_action_incidents
                          join d in dbConnect.incidents on c.incident_id equals d.id
                          where c.date_complete == null//DateTime.Now.Date == c.due_date && c.date_complete == null && DateTime.Now.Date > c.last_alerted_at.Date
                          && d.process_status == 1//onprocess
                         // && c.incident_id== 4180 // for test case
                          select new
                          {
                              action_id = c.id,
                              incident_id = c.incident_id,
                              process_status = d.process_status,
                              action_status_id = c.action_status_id,
                              c.due_date,
                              c.date_complete,
                              c.last_alerted_at,
                              d.country
                          };


            foreach (var v in actions)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.last_alerted_at.Date

               // rs = true;
                if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date == v.due_date && rs == true)
                {
                    if (v.process_status == 1)
                    {
                        if (v.action_status_id == 1 || v.action_status_id == 2 || v.action_status_id == 6)
                        {
                            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();

                            if (v.country == "thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S" };
                                sn.InsertNotification(9, v.incident_id, alert_to_groups, timezone, "AreaOH&S", v.action_id, "corrective");
                            }
                            else if (v.country == "srilanka")
                            {
                                string[] alert_to_groups = { "AreaManager" };
                                sn.InsertNotification(9, v.incident_id, alert_to_groups, timezone, "AreaManager", v.action_id, "corrective");
                            }

                            //if (v.country == "thailand")
                            //{
                            //    string[] alert_to_groups = { "AreaOH&S" };
                            //    sn.InsertNotification(9, v.incident_id, alert_to_groups, timezone, "AreaOH&S", v.action_id);
                            //}
                            //else if (v.country == "srilanka")
                            //{
                            //    string[] alert_to_groups = { "AreaManager" };
                            //    sn.InsertNotification(9, v.incident_id, alert_to_groups, timezone, "AreaManager", v.action_id, "consequence");
                            //}


                            safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                            corrective_prevention_action_incident updateObj = dbConnectU.corrective_prevention_action_incidents.Single(n => n.id == v.action_id);
                            updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            dbConnectU.SubmitChanges();
                        }
                    }
                }

            }



            var actions2 = from c in dbConnect.preventive_action_incidents
                           join d in dbConnect.incidents on c.incident_id equals d.id
                           where c.date_complete == null
                           && d.process_status == 1//onprocess
                           select new
                           {
                               action_id = c.id,
                               incident_id = c.incident_id,
                               process_status = d.process_status,
                               action_status_id = c.action_status_id,
                               c.due_date,
                               c.date_complete,
                               c.last_alerted_at,
                               d.country
                           };


            foreach (var v in actions2)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 1, timezone);

                if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date == v.due_date && rs == true)
                {
                    if (v.process_status == 1)
                    {
                        if (v.action_status_id == 1 || v.action_status_id == 2 || v.action_status_id == 6)
                        {
                            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();


                            if (v.country == "thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S" };
                                //sn.InsertNotification(9, v.incident_id, alert_to_groups, timezone, "AreaOH&S", v.action_id);
                                sn.InsertNotification(9, v.incident_id, alert_to_groups, timezone, "AreaOH&S", v.action_id, "preventive");
                            }


                            safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                            preventive_action_incident updateObj = dbConnectU.preventive_action_incidents.Single(n => n.id == v.action_id);
                            updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            dbConnectU.SubmitChanges();
                        }
                    }
                }

            }




            var actions3 = from c in dbConnect.consequence_management_incidents
                           join d in dbConnect.incidents on c.incident_id equals d.id
                           where c.date_complete == null
                           && d.process_status == 1//onprocess
                           select new
                           {
                               action_id = c.id,
                               incident_id = c.incident_id,
                               process_status = d.process_status,
                               action_status_id = c.action_status_id,
                               c.due_date,
                               c.date_complete,
                               c.last_alerted_at,
                               d.country
                           };


            foreach (var v in actions3)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 1, timezone);

                if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date == v.due_date && rs == true)
                {
                    if (v.process_status == 1)
                    {
                        if (v.action_status_id == 1 || v.action_status_id == 2 || v.action_status_id == 6)
                        {
                            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();


                            if (v.country == "thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S" };
                                sn.InsertNotification(9, v.incident_id, alert_to_groups, timezone, "AreaOH&S", v.action_id, "consequence");
                            }


                            safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                            consequence_management_incident updateObj = dbConnectU.consequence_management_incidents.Single(n => n.id == v.action_id);
                            updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            dbConnectU.SubmitChanges();
                        }
                    }
                }

            }


        }

        private void incidentActionOverDueToday()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();

            var actions = from c in dbConnect.corrective_prevention_action_incidents
                          join d in dbConnect.incidents on c.incident_id equals d.id
                          where c.date_complete == null//DateTime.Now > c.due_date.Value.AddDays(1) && c.date_complete == null && DateTime.Now.Date > c.last_alerted_at.Date
                          && d.process_status == 1//onprocess
                          //&& c.incident_id == 4180
                          select new
                          {
                              action_id = c.id,
                              incident_id = c.incident_id,
                              process_status = d.process_status,
                              action_status_id = c.action_status_id,
                              c.due_date,
                              c.last_alerted_at,
                              d.country
                          };

            foreach (var v in actions)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.due_date, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.due_date.Value.AddDays(1)
               bool rs2 = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.last_alerted_at.Date
                //rs = true;
                //rs2 = true;
                if (rs == true && rs2 == true)
                {
                    if (v.process_status == 1)
                    {
                        if (v.action_status_id == 1 || v.action_status_id == 2 || v.action_status_id == 6)
                        {
                            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();


                            if (v.country == "thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S", "AdminOH&S", "AreaSuperervisor" };
                                sn.InsertNotification(10, v.incident_id, alert_to_groups, timezone, "AreaOH&S", v.action_id, "corrective");

                                string[] alert_to_groups_super = { };
                                sn.InsertNotification(25, v.incident_id, alert_to_groups_super, timezone, "", v.action_id, "corrective");
                            }
                            else if (v.country == "srilanka")
                            {
                                string[] alert_to_groups = { "AreaManager" };
                                sn.InsertNotification(10, v.incident_id, alert_to_groups, timezone, "AreaManager", v.action_id, "corrective");
                            }


                            safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                            corrective_prevention_action_incident updateObj = dbConnectU.corrective_prevention_action_incidents.Single(n => n.id == v.action_id);
                            updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            dbConnectU.SubmitChanges();
                        }
                    }
                }

            }




            var actions2 = from c in dbConnect.preventive_action_incidents
                           join d in dbConnect.incidents on c.incident_id equals d.id
                           where c.date_complete == null//
                           && d.process_status == 1//onprocess
                         // && c.incident_id == 5168
                           select new
                           {
                               action_id = c.id,
                               incident_id = c.incident_id,
                               process_status = d.process_status,
                               action_status_id = c.action_status_id,
                               c.due_date,
                               c.last_alerted_at,
                               d.country
                           };

            foreach (var v in actions2)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.due_date, CultureInfo.InvariantCulture), 1, timezone);
                bool rs2 = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 1, timezone);

                if (rs == true && rs2 == true)
                {
                    if (v.process_status == 1)
                    {
                        if (v.action_status_id == 1 || v.action_status_id == 2 || v.action_status_id == 6)
                        {
                            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();


                            if (v.country == "thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S", "AdminOH&S", "AreaSuperervisor" };
                                sn.InsertNotification(10, v.incident_id, alert_to_groups, timezone, "AreaOH&S", v.action_id, "preventive");

                                string[] alert_to_groups_super = { };
                                sn.InsertNotification(26, v.incident_id, alert_to_groups_super, timezone, "", v.action_id, "preventive");
                            }


                            safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                            preventive_action_incident updateObj = dbConnectU.preventive_action_incidents.Single(n => n.id == v.action_id);
                            updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            dbConnectU.SubmitChanges();
                        }
                    }
                }

            }



            var actions3 = from c in dbConnect.consequence_management_incidents
                           join d in dbConnect.incidents on c.incident_id equals d.id
                           where c.date_complete == null//
                           && d.process_status == 1//onprocess
                           //&& c.incident_id == 5168
                           select new
                           {
                               action_id = c.id,
                               incident_id = c.incident_id,
                               process_status = d.process_status,
                               action_status_id = c.action_status_id,
                               c.due_date,
                               c.last_alerted_at,
                               d.country
                           };

            foreach (var v in actions3)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.due_date, CultureInfo.InvariantCulture), 1, timezone);
                bool rs2 = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 1, timezone);

                if (rs == true && rs2 == true)
                {
                    if (v.process_status == 1)
                    {
                        if (v.action_status_id == 1 || v.action_status_id == 2 || v.action_status_id == 6)
                        {
                            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();


                            if (v.country == "thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S", "AdminOH&S", "AreaSuperervisor" };
                                sn.InsertNotification(10, v.incident_id, alert_to_groups, timezone, "AreaOH&S", v.action_id, "consequence");

                                string[] alert_to_groups_super = { };
                                sn.InsertNotification(27, v.incident_id, alert_to_groups_super, timezone, "", v.action_id, "consequence");
                            }


                            safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                            consequence_management_incident updateObj = dbConnectU.consequence_management_incidents.Single(n => n.id == v.action_id);
                            updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            dbConnectU.SubmitChanges();
                        }
                    }
                }

            }





        }

        private void hazardActionDueToday()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();

            var actions = from c in dbConnect.process_actions
                          join d in dbConnect.hazards on c.hazard_id equals d.id
                          where c.date_complete == null//DateTime.Now.Date == c.due_date && c.date_complete == null && DateTime.Now.Date > c.last_alerted_at.Date
                          && d.process_status == 1//onprocess
                          select new
                          {
                              action_id = c.id,
                              incident_id = c.hazard_id,
                              process_status = d.process_status,
                              action_status_id = c.action_status_id,
                              c.due_date,
                              c.last_alerted_at,
                              d.country
                          };

            foreach (var v in actions)
            {
                string timezone = this.getTimezone(v.country);

                bool rs2 = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.last_alerted_at.Date

                if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date == v.due_date && rs2 == true)
                {
                    if (v.process_status == 1)
                    {
                        if (v.action_status_id == 1 || v.action_status_id == 2 || v.action_status_id == 6)
                        {
                            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();



                            if (v.country == "thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S" };
                                sn.InsertHazardNotification(10, v.incident_id, alert_to_groups, timezone, "AreaOH&S", v.action_id);
                            }
                            else if (v.country == "srilanka")
                            {
                                string[] alert_to_groups = { "AreaOH&S" };
                                sn.InsertHazardNotification(10, v.incident_id, alert_to_groups, timezone, "AreaSuperervisor", v.action_id);
                            }


                            safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                            process_action updateObj = dbConnectU.process_actions.Single(n => n.id == v.action_id);
                            updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            dbConnectU.SubmitChanges();
                        }
                    }

                }

            }
        }

        private void hazardActionOverDueToday()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();

            var actions = from c in dbConnect.process_actions
                          join d in dbConnect.hazards on c.hazard_id equals d.id
                          where c.date_complete == null//DateTime.Now > c.due_date.Value.AddDays(1) && c.date_complete == null && DateTime.Now.Date > c.last_alerted_at.Date
                          && d.process_status == 1//onprocess
                          
                          select new
                          {
                              action_id = c.id,
                              incident_id = c.hazard_id,
                              process_status = d.process_status,
                              action_status_id = c.action_status_id,
                              c.due_date,
                              c.last_alerted_at,
                              d.country
                          };

            foreach (var v in actions)
            {
                string timezone = this.getTimezone(v.country);
                bool rs = addday(Convert.ToDateTime(v.due_date, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.due_date.Value.AddDays(1)
                bool rs2 = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.last_alerted_at.Date
                
                if (rs == true && rs2 == true)
                {
                    if (v.process_status == 1)
                    {
                        if (v.action_status_id == 1 || v.action_status_id == 2 || v.action_status_id == 6)
                        {
                            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();


                            if (v.country == "thailand")
                            {
                                string[] alert_to_groups = { "AreaOH&S", "AdminOH&S" };
                                sn.InsertHazardNotification(11, v.incident_id, alert_to_groups, timezone, "AreaOH&S", v.action_id);

                                string[] alert_to_groups_super = { };
                                sn.InsertHazardNotification(22, v.incident_id, alert_to_groups, timezone, "", v.action_id);

                            }
                            else if (v.country == "srilanka")
                            {
                                string[] alert_to_groups = { "AreaSuperervisor" };
                                sn.InsertHazardNotification(11, v.incident_id, alert_to_groups, timezone, "AreaSuperervisor", v.action_id);

                            }

                            safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                            process_action updateObj = dbConnectU.process_actions.Single(n => n.id == v.action_id);
                            updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            dbConnectU.SubmitChanges();
                        }
                    }
                }

            }
        }




        private void incidentFormTwoToThreeNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var incidents = from c in dbConnect.incidents
                            where c.process_status == 1 && c.step_form == 3
                            && c.confirm_form_two_to_three_at != null
                            && c.action_form_three_at == null
                            select new
                            {
                                function_id = c.function_id,
                                department_id = c.department_id,
                                division_id = c.division_id,
                                section_id = c.section_id,
                                incident_id = c.id,
                                incident_code = c.doc_no,
                                title = c.incident_name,
                                description = c.incident_detail,
                                occur_date = c.incident_date,
                                is_alert_over_due = c.is_alert_over_due,
                                report_date = c.report_date,
                                c.alert_form_two_to_three_at,
                                confirm_form_two_to_three_at = c.confirm_form_two_to_three_at,
                                country = c.country
                            };

            foreach (var v in incidents)
            {
                string timezone = this.getTimezone(v.country);
                bool rs = addday(Convert.ToDateTime(v.confirm_form_two_to_three_at, CultureInfo.InvariantCulture), 7, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.confirm_form_two_to_three_at.Value.AddDays(7)
                bool rs2 = addday(Convert.ToDateTime(v.alert_form_two_to_three_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.alert_form_two_to_three_at.Value.Date

                var count_row = dbConnect.corrective_prevention_action_incidents.Where(x => x.incident_id == v.incident_id).Count();
                var count_row_fact = dbConnect.fact_findings.Where(x => x.incident_id == v.incident_id).Count();

                if ((rs == true) && (rs2 == true) && count_row == 0 && count_row_fact == 0)
                {
                    safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();


                    if (v.country == "thailand")
                    {
                        string[] alert_to_groups = { "AdminOH&S", "AreaOH&S" };
                        sn.InsertNotification(20, v.incident_id, alert_to_groups, timezone, "AreaOH&S");
                    }
                    else if (v.country == "srilanka")
                    {
                        string[] alert_to_groups = { "AreaManager" };
                        sn.InsertNotification(20, v.incident_id, alert_to_groups, timezone, "AreaManager");
                    }


                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    incident updateObj = dbConnectU.incidents.Single(n => n.id == v.incident_id);
                    updateObj.alert_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();


                }//end if


            }
        }//end function

        //Checking for Incident manger loping for Incident RK
        private void incidentFormTwoToThreeBossNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var incidents = from c in dbConnect.incidents
                            where c.process_status == 1 && c.step_form == 3
                            && c.confirm_form_two_to_three_at != null
                            && c.action_form_three_at == null
                           // && c.doc_no== "I2020-00080"  // for manger loping for Incident RK
                            select new
                            {
                                function_id = c.function_id,
                                department_id = c.department_id,
                                division_id = c.division_id,
                                section_id = c.section_id,
                                incident_id = c.id,
                                incident_code = c.doc_no,
                                title = c.incident_name,
                                description = c.incident_detail,
                                occur_date = c.incident_date,
                                is_alert_over_due = c.is_alert_over_due,
                                report_date = c.report_date,
                                c.alert_form_two_to_three_boss_at,
                                confirm_form_two_to_three_at = c.confirm_form_two_to_three_at,
                                country = c.country
                            };

            foreach (var v in incidents)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.confirm_form_two_to_three_at, CultureInfo.InvariantCulture), 14, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.confirm_form_two_to_three_at.Value.AddDays(14)
                bool rs2 = addday(Convert.ToDateTime(v.alert_form_two_to_three_boss_at, CultureInfo.InvariantCulture), 7, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.alert_form_two_to_three_boss_at.Value.AddDays(7)

                var count_row = dbConnect.corrective_prevention_action_incidents.Where(x => x.incident_id == v.incident_id).Count();
                var count_row_fact = dbConnect.fact_findings.Where(x => x.incident_id == v.incident_id).Count();

                if ((rs == true) && (rs2 == true) && count_row == 0 && count_row_fact == 0)
                {
                    safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();


                    if (v.country == "thailand")
                    {
                        string[] alert_to_groups = { "AreaOH&S" };
                        sn.InsertNotification(23, v.incident_id, alert_to_groups, timezone, "AreaOH&S");
                    }


                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    incident updateObj = dbConnectU.incidents.Single(n => n.id == v.incident_id);
                    updateObj.alert_form_two_to_three_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();


                }//end if


            }
        }//end function



        private void incidentFormThreeToFourNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var incidents = from c in dbConnect.incidents
                            where c.process_status == 1 && c.step_form == 4
                            && c.confirm_form_three_to_four_at != null
                            select new
                            {
                                function_id = c.function_id,
                                department_id = c.department_id,
                                division_id = c.division_id,
                                section_id = c.section_id,
                                incident_id = c.id,
                                incident_code = c.doc_no,
                                title = c.incident_name,
                                description = c.incident_detail,
                                occur_date = c.incident_date,
                                is_alert_over_due = c.is_alert_over_due,
                                report_date = c.report_date,
                                c.alert_form_three_to_four_at,
                                c.confirm_form_three_to_four_at,
                                country = c.country
                            };

            foreach (var v in incidents)
            {
                string timezone = this.getTimezone(v.country);
                bool rs = addday(Convert.ToDateTime(v.confirm_form_three_to_four_at, CultureInfo.InvariantCulture), 3, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.confirm_form_three_to_four_at.Value.AddDays(3)
                bool rs2 = addday(Convert.ToDateTime(v.alert_form_three_to_four_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.alert_form_three_to_four_at.Value.Date

                var count_row = dbConnect.log_request_close_incidents.Where(i => i.incident_id == v.incident_id && i.status == "A").Count();
                if (rs == true && count_row == 0 && (rs2 == true))
                {
                    var s = from c in dbConnect.close_step_incidents
                            where c.country == v.country
                            && c.step == 1
                            orderby c.step descending
                            select c;

                    foreach (var r in s)
                    {
                        setGroupEmailStepClose(Convert.ToInt32(r.group_id), 21, v.incident_id, timezone, "incident");

                    }

                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    incident updateObj = dbConnectU.incidents.Single(n => n.id == v.incident_id);
                    updateObj.alert_form_three_to_four_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();

                }//end if


            }

        }//end function




        private void incidentFormThreeToFourStepNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();



            var incidents = from c in dbConnect.incidents
                            where c.process_status == 1 && c.step_form == 4
                            select new
                            {
                                function_id = c.function_id,
                                department_id = c.department_id,
                                division_id = c.division_id,
                                section_id = c.section_id,
                                incident_id = c.id,
                                incident_code = c.doc_no,
                                c.report_date,
                                c.alert_form_three_to_four_at,
                                c.country,
                                c.confirm_form_three_to_four_at,
                                c.confirm_form_two_to_three_at

                            };

            foreach (var v in incidents)
            {
                string timezone = this.getTimezone(v.country);


                var g = from c in dbConnect.close_step_incidents
                        where c.country == v.country
                        orderby c.step ascending
                        select c;

                foreach (var rc in g)
                {
                    var w = from c in dbConnect.log_request_close_incidents
                            where c.incident_id == v.incident_id && c.status == "A"
                            && c.group_id == rc.group_id
                            select c;

                    foreach (var rc2 in w)
                    {
                        var count_row = dbConnect.log_request_close_incidents.Where(i => i.incident_id == v.incident_id && i.status == "A").Count();
                        bool rs = addday(Convert.ToDateTime(rc2.created_at, CultureInfo.InvariantCulture), 3, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > rc2.created_at.Value.AddDays(3)
                        bool rs2 = addday(Convert.ToDateTime(v.alert_form_three_to_four_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.alert_form_three_to_four_at.Value.Date

                        if ((rs == true) && (count_row == rc.step) && (rs2 == true))
                        {

                            var d = dbConnect.close_step_incidents.FirstOrDefault(i => i.step == (rc.step + 1) && i.country == v.country);

                            if (d != null)
                            {
                                setGroupEmailStepClose(Convert.ToInt32(d.group_id), 21, v.incident_id, timezone, "incident");

                                safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                                incident updateObj = dbConnectU.incidents.Single(n => n.id == v.incident_id);
                                updateObj.alert_form_three_to_four_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                                dbConnectU.SubmitChanges();

                            }



                        }



                    }//end foreach inside




                }
            }
        }//end function




        private void hazardFormTwoToThreeNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var hazards = from c in dbConnect.hazards
                          where c.process_status == 1 && c.step_form == 3
                          && c.confirm_form_two_to_three_at != null
                          select new
                          {
                              function_id = c.function_id,
                              department_id = c.department_id,
                              division_id = c.division_id,
                              section_id = c.section_id,
                              hazard_id = c.id,
                              hazard_code = c.doc_no,
                              c.report_date,
                              c.alert_form_two_to_three_at,
                              c.country,
                              c.confirm_form_two_to_three_at

                          };

            foreach (var v in hazards)
            {
                string timezone = this.getTimezone(v.country);


                var count_row = dbConnect.process_actions.Where(i => i.hazard_id == v.hazard_id).Count();

                bool rs = addday(Convert.ToDateTime(v.confirm_form_two_to_three_at, CultureInfo.InvariantCulture), 7, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.confirm_form_two_to_three_at.Value.AddDays(7)
                bool rs2 = addday(Convert.ToDateTime(v.alert_form_two_to_three_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.alert_form_two_to_three_at.Value.Date

                if (rs == true && count_row == 0 && (rs2 == true))
                {

                    safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();

                    if (v.country == "thailand")
                    {
                        string[] alert_to_groups = { "AdminOH&S", "AreaOH&S", "AreaSuperervisor" };
                        sn.InsertHazardNotification(17, v.hazard_id, alert_to_groups, timezone, "AreaSuperervisor");

                    }
                    else if (v.country == "srilanka")
                    {
                        string[] alert_to_groups = { "AreaSuperervisor" };
                        sn.InsertHazardNotification(17, v.hazard_id, alert_to_groups, timezone, "AreaSuperervisor");

                    }


                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    hazard updateObj = dbConnectU.hazards.Single(n => n.id == v.hazard_id);
                    updateObj.alert_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();


                }




            }
        }//end function


        //Checking for Hazards  manger loping for Incident RK
        private void hazardFormTwoToThreeBossNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var hazards = from c in dbConnect.hazards
                          where c.process_status == 1 && c.step_form == 3
                          && c.confirm_form_two_to_three_at != null
                         // && c.id == 10281 //for H2020-00098 testing The system not send 2nd notification email to supervisor

                          select new
                          {
                              function_id = c.function_id,
                              department_id = c.department_id,
                              division_id = c.division_id,
                              section_id = c.section_id,
                              hazard_id = c.id,
                              hazard_code = c.doc_no,
                              c.report_date,
                              c.alert_form_two_to_three_boss_at,
                              c.country,
                              c.confirm_form_two_to_three_at

                          };

            foreach (var v in hazards)
            {
                string timezone = this.getTimezone(v.country);


                var count_row = dbConnect.process_actions.Where(i => i.hazard_id == v.hazard_id).Count();

                bool rs = addday(Convert.ToDateTime(v.confirm_form_two_to_three_at, CultureInfo.InvariantCulture), 7, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.confirm_form_two_to_three_at.Value.AddDays(7)
                bool rs2 = addday(Convert.ToDateTime(v.alert_form_two_to_three_boss_at, CultureInfo.InvariantCulture), 7, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.alert_form_two_to_three_boss_at.Value.AddDays(7)

                if (rs == true && count_row == 0 && (rs2 == true))
                {

                    safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();

                    if (v.country == "thailand")
                    {
                        string[] alert_to_groups = { "AreaSuperervisor" };
                        sn.InsertHazardNotification(20, v.hazard_id, alert_to_groups, timezone, "AreaSuperervisor");

                    }

                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    hazard updateObj = dbConnectU.hazards.Single(n => n.id == v.hazard_id);
                    updateObj.alert_form_two_to_three_boss_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();


                }




            }
        }//end function




        private void hazardFormThreeToFourNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var hazards = from c in dbConnect.hazards
                          where c.process_status == 1 && c.step_form == 4
                          && c.confirm_form_three_to_four_at != null
                          select new
                          {
                              function_id = c.function_id,
                              department_id = c.department_id,
                              division_id = c.division_id,
                              section_id = c.section_id,
                              hazard_id = c.id,
                              hazard_code = c.doc_no,
                              c.report_date,
                              c.alert_form_three_to_four_at,
                              c.country,
                              c.confirm_form_three_to_four_at,
                              c.confirm_form_two_to_three_at

                          };

            foreach (var v in hazards)
            {
                string timezone = this.getTimezone(v.country);

                var count_row = dbConnect.log_request_close_hazards.Where(i => i.hazard_id == v.hazard_id && i.status == "A").Count();

                bool rs = addday(Convert.ToDateTime(v.confirm_form_three_to_four_at, CultureInfo.InvariantCulture), 3, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.confirm_form_three_to_four_at.Value.AddDays(3)
                bool rs2 = addday(Convert.ToDateTime(v.alert_form_three_to_four_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.alert_form_three_to_four_at.Value.Date

                if (rs == true && count_row == 0 && (rs2 == true))
                {
                    var s = from c in dbConnect.close_step_hazards
                            where c.country == v.country
                            && c.step == 1
                            orderby c.step descending
                            select c;

                    foreach (var r in s)
                    {
                        setGroupEmailStepClose(Convert.ToInt32(r.group_id), 18, v.hazard_id, timezone, "hazard");

                    }



                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    hazard updateObj = dbConnectU.hazards.Single(n => n.id == v.hazard_id);
                    updateObj.alert_form_three_to_four_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();

                }



            }
        }//end function


        private void hazardFormThreeToFourStepNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();



            var hazards = from c in dbConnect.hazards
                          where c.process_status == 1 && c.step_form == 4
                          select new
                          {
                              function_id = c.function_id,
                              department_id = c.department_id,
                              division_id = c.division_id,
                              section_id = c.section_id,
                              hazard_id = c.id,
                              hazard_code = c.doc_no,
                              c.report_date,
                              c.alert_form_three_to_four_at,
                              c.country,
                              c.confirm_form_three_to_four_at,
                              c.confirm_form_two_to_three_at

                          };

            foreach (var v in hazards)
            {
                string timezone = this.getTimezone(v.country);


                var g = from c in dbConnect.close_step_hazards
                        where c.country == v.country
                        orderby c.step ascending
                        select c;

                foreach (var rc in g)
                {
                    var w = from c in dbConnect.log_request_close_hazards
                            where c.hazard_id == v.hazard_id && c.status == "A"
                            && c.group_id == rc.group_id
                            select c;

                    foreach (var rc2 in w)
                    {
                        var count_row = dbConnect.log_request_close_hazards.Where(i => i.hazard_id == v.hazard_id && i.status == "A").Count();

                        bool rs = addday(Convert.ToDateTime(rc2.created_at, CultureInfo.InvariantCulture), 3, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > rc2.created_at.Value.AddDays(3)
                        bool rs2 = addday(Convert.ToDateTime(v.alert_form_three_to_four_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.alert_form_three_to_four_at.Value.Date

                        if ((rs == true) && (count_row == rc.step) && (rs2 == true))
                        {

                            var d = dbConnect.close_step_hazards.FirstOrDefault(i => i.step == (rc.step + 1) && i.country == v.country);

                            if (d != null)
                            {
                                setGroupEmailStepClose(Convert.ToInt32(d.group_id), 18, v.hazard_id, timezone, "hazard");


                                safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                                hazard updateObj = dbConnectU.hazards.Single(n => n.id == v.hazard_id);
                                updateObj.alert_form_three_to_four_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                                dbConnectU.SubmitChanges();


                            }



                        }



                    }//end foreach inside




                }
            }
        }//end function



        private void healthFormTwoToThreeStepNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();



            var healths = from c in dbConnect.healths
                          where c.process_status == 1 && c.step_form == 3
                          select new
                          {
                              function_id = c.function_id,
                              department_id = c.department_id,
                              division_id = c.division_id,
                              section_id = c.section_id,
                              health_id = c.id,
                              health_code = c.doc_no,
                              c.report_date,
                              c.country,
                              c.confirm_form_two_to_three_at,
                              c.alert_form_two_to_three_at

                          };

            foreach (var v in healths)
            {
                string timezone = this.getTimezone(v.country);


                var g = from c in dbConnect.close_step_healths
                        where c.country == v.country
                        orderby c.step ascending
                        select c;

                foreach (var rc in g)
                {
                    var w = from c in dbConnect.log_request_close_healths
                            where c.health_id == v.health_id && c.status == "A"
                            && c.group_id == rc.group_id
                            select c;

                    foreach (var rc2 in w)
                    {
                        var count_row = dbConnect.log_request_close_healths.Where(i => i.health_id == v.health_id && i.status == "A").Count();

                        bool rs = addday(Convert.ToDateTime(rc2.created_at, CultureInfo.InvariantCulture), 3, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > rc2.created_at.Value.AddDays(3)
                        bool rs2 = addday(Convert.ToDateTime(v.alert_form_two_to_three_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.alert_form_three_to_four_at.Value.Date

                        if ((rs == true) && (count_row == rc.step) && (rs2 == true))
                        {

                            var d = dbConnect.close_step_healths.FirstOrDefault(i => i.step == (rc.step + 1) && i.country == v.country);

                            if (d != null)
                            {
                                setGroupEmailStepClose(Convert.ToInt32(d.group_id), 4, v.health_id, timezone, "health");


                                safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                                health updateObj = dbConnectU.healths.Single(n => n.id == v.health_id);
                                updateObj.alert_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                                dbConnectU.SubmitChanges();


                            }



                        }



                    }//end foreach inside




                }
            }
        }//end function



        private void healthFormTwoToThreeNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var hazards = from c in dbConnect.healths
                          where c.process_status == 1 && c.step_form == 3
                          && c.confirm_form_two_to_three_at != null
                          select new
                          {
                              function_id = c.function_id,
                              department_id = c.department_id,
                              division_id = c.division_id,
                              section_id = c.section_id,
                              health_id = c.id,
                              health_code = c.doc_no,
                              c.report_date,
                              c.alert_form_two_to_three_at,
                              c.country,
                              c.confirm_form_two_to_three_at


                          };

            foreach (var v in hazards)
            {
                string timezone = this.getTimezone(v.country);

                var count_row = dbConnect.log_request_close_healths.Where(i => i.health_id == v.health_id && i.status == "A").Count();

                bool rs = addday(Convert.ToDateTime(v.confirm_form_two_to_three_at, CultureInfo.InvariantCulture), 3, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)) > v.confirm_form_three_to_four_at.Value.AddDays(3)
                bool rs2 = addday(Convert.ToDateTime(v.alert_form_two_to_three_at, CultureInfo.InvariantCulture), 1, timezone);//DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date > v.alert_form_three_to_four_at.Value.Date

                if (rs == true && count_row == 0 && (rs2 == true))
                {
                    var s = from c in dbConnect.close_step_healths
                            where c.country == v.country
                            && c.step == 1
                            orderby c.step descending
                            select c;

                    foreach (var r in s)
                    {
                        setGroupEmailStepClose(Convert.ToInt32(r.group_id), 4, v.health_id, timezone, "health");

                    }



                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    health updateObj = dbConnectU.healths.Single(n => n.id == v.health_id);
                    updateObj.alert_form_two_to_three_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();

                }



            }
        }//end function


        private void healthNoActive()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();
            var healths = from c in dbConnect.healths
                          where c.process_status == 1 && c.step_form == 2
                          select new
                          {
                              function_id = c.function_id,
                              department_id = c.department_id,
                              division_id = c.division_id,
                              section_id = c.section_id,
                              health_id = c.id,
                              health_code = c.doc_no,
                              c.report_date,
                              c.last_alerted_at,
                              c.country

                          };

            foreach (var v in healths)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.report_date, CultureInfo.InvariantCulture), 7, timezone);
                bool rs2 = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 7, timezone);

                var count_row = dbConnect.process_action_healths.Where(i => i.health_id == v.health_id).Count();

                if (rs == true && rs2 == true && count_row == 0)
                {
                    safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();

                    string[] alert_to_groups = { "AreaOH&S" };
                    sn.InsertHealthNotification(4, v.health_id, alert_to_groups, timezone, "AreaOH&S");


                    safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                    health updateObj = dbConnectU.healths.Single(n => n.id == v.health_id);
                    updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                    dbConnectU.SubmitChanges();
                }

            }
        }




        private void healthActionDueToday()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();

            var actions = from c in dbConnect.process_action_healths
                          join d in dbConnect.healths on c.health_id equals d.id
                          where c.date_complete == null
                          && d.process_status == 1//onprocess
                          select new
                          {
                              action_id = c.id,
                              health_id = c.health_id,
                              process_status = d.process_status,
                              action_status_id = c.action_status_id,
                              c.due_date,
                              c.date_complete,
                              c.last_alerted_at,
                              d.country
                          };


            foreach (var v in actions)
            {
                string timezone = this.getTimezone(v.country);

                bool rs = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 7, timezone);

                if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Date == v.due_date && rs == true)
                {
                    if (v.process_status == 1)
                    {
                        if (v.action_status_id == 1)//on process
                        {
                            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();


                            string[] alert_to_groups = { "AreaOH&S", "AdminOH&S" };
                            sn.InsertHealthNotification(6, v.health_id, alert_to_groups, timezone, "", v.action_id);


                            safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                            process_action_health updateObj = dbConnectU.process_action_healths.Single(n => n.id == v.action_id);
                            updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            dbConnectU.SubmitChanges();
                        }
                    }
                }

            }


        }




        private void healthActionOverDueToday()
        {
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();
            safetys4.App_Code.SafetyEmail se = new App_Code.SafetyEmail();

            var actions = from c in dbConnect.process_action_healths
                          join d in dbConnect.healths on c.health_id equals d.id
                          where c.date_complete == null
                          && d.process_status == 1//onprocess
                          
                          select new
                          {
                              action_id = c.id,
                              health_id = c.health_id,
                              process_status = d.process_status,
                              action_status_id = c.action_status_id,
                              c.due_date,
                              c.last_alerted_at,
                              d.country
                          };

            foreach (var v in actions)
            {
                string timezone = this.getTimezone(v.country);
                bool rs = addday(Convert.ToDateTime(v.due_date, CultureInfo.InvariantCulture), 7, timezone);
                bool rs2 = addday(Convert.ToDateTime(v.last_alerted_at, CultureInfo.InvariantCulture), 7, timezone);

                if (rs == true && rs2 == true)
                {
                    if (v.process_status == 1)
                    {
                        if (v.action_status_id == 1)
                        {
                            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();

                            string[] alert_to_groups = { "AreaOH&S", "AdminOH&S" };
                            sn.InsertHealthNotification(7, v.health_id, alert_to_groups, timezone, "", v.action_id);



                            safetys4dbDataContext dbConnectU = new safetys4dbDataContext();
                            process_action_health updateObj = dbConnectU.process_action_healths.Single(n => n.id == v.action_id);
                            updateObj.last_alerted_at = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone));
                            dbConnectU.SubmitChanges();
                        }
                    }
                }

            }
        }




        public void setGroupEmailStepClose(int group_id, int template, int id, string timezone, string type_report)
        {

            safetys4.Class.SafetyNotification sn = new safetys4.Class.SafetyNotification();

            string[] alert_to_groups = new string[1];
            string role_action = "";

            if (group_id == 4 || group_id == 5)
            {
                alert_to_groups[0] = "AdminOH&S";
                role_action = "AdminOH&S";
            }
            else if (group_id == 8)
            {
                alert_to_groups[0] = "GroupOH&S";
                role_action = "GroupOH&S";
            }
            else if (group_id == 9)
            {
                alert_to_groups[0] = "AreaOH&S";
                role_action = "AreaOH&S";
            }
            else if (group_id == 10)
            {
                alert_to_groups[0] = "AreaManager";
                role_action = "AreaManager";
            }
            else if (group_id == 11)
            {
                alert_to_groups[0] = "AreaSuperervisor";
                role_action = "AreaSuperervisor";
            }
            else if (group_id == 16)
            {
                alert_to_groups[0] = "GroupOH&SHazard";
                role_action = "GroupOH&SHazard";
            }
            else if (group_id == 17)
            {
                alert_to_groups[0] = "GroupOH&SHealth";
                role_action = "&&SHealth";
            }
            else if (group_id == 18)
            {
                alert_to_groups[0] = "Functional";
                role_action = "Functional";
            }

            if (type_report == "incident")
            {
                sn.InsertNotification(template, id, alert_to_groups, timezone, role_action);

            }
            else if (type_report == "hazard")
            {
                sn.InsertHazardNotification(template, id, alert_to_groups, timezone, role_action);


            }
            else if (type_report == "health")
            {
                sn.InsertHealthNotification(template, id, alert_to_groups, timezone, role_action);


            }



        }






        public bool addday(DateTime dt, int day, string timezone)
        {
            bool result = false;
            int count = 0;
            safetys4dbDataContext dbConnect = new safetys4dbDataContext();


            DateTime dtnew = dt;

            TimeSpan span = DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).Subtract(dtnew);
            int cday = Convert.ToInt32(span.TotalDays);//ถ้าเกินระดับ 35 วันก็ไม่ต้องมาวนนับละ มันเกินแน่ๆ


            if (cday < 35)
            {
                while (dtnew <= DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)))
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
                        foreach (var v in holidays)
                        {
                            is_holiday = true;
                        }
                        if (!is_holiday)
                        {
                            count++;

                        }
                        //count++;
                    }//end if


                }//end for



                if (DateTime.UtcNow.AddHours(Convert.ToDouble(timezone)).DayOfWeek.ToString() != "Friday")//มันมีกรณีวันศุกร์ไม่ส่งอ่ะ
                {
                    if (count > day)
                    {
                        result = true;
                    }
                }
                else
                {

                    if (count >= day)
                    {
                        result = true;
                    }


                }



            }
            else
            {

                result = true;//ถ้าเกินระดับ 35 วัน ยังไงก็ต้องเกินทุกเงื่อนไขจะไม่มาวนคิดให้เสียเวลา
            }

            
            return result;
        }






    }
}