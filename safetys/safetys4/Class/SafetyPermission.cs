using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace safetys4.Class
{
    public class SafetyPermission
    {
        //private const int SUPER_ADMIN = 2;
        //private const int DELEGAT_SUPER_ADMIN = 3;
        //private const int OHS_ADMIN = 4;
        //private const int DELEGATE_OHS_ADMIN = 5;
        //private const int GROUP_COMMINICATION_VP = 6;
        //private const int LEGAL_DEPARTMENT = 7;
        //private const int GROUP_OHS = 8;
        //private const int AREA_OHS = 9;
        //private const int AREA_MANAGER = 10; 
        //private const int AREA_SUPERVISOR = 11; 
        //private const int GROUP_HOS_HAZARD = 16;
        public static bool checkPermisionAction(string permission, string report_id, string type_report, int group_value)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                bool result = false;

                if (report_id != "")
                {

                    if (type_report == "incident")
                    {
                        var g = from c in dbConnect.incidents
                                where c.id == Convert.ToInt32(report_id)
                                select new
                                {
                                    c.verify_report_form1,
                                    c.reject_report_form1,
                                    c.edit_form1,
                                    c.reject_report_form2,
                                    c.submit_report_form2,
                                    c.confirm_investigate_form2,
                                    c.confirm_by_groupohs_form2,
                                    c.edit_form2,
                                    c.request_close_form3,
                                    c.create_action_form3,
                                    c.edit_action_form3,
                                    c.close_action_form3,
                                    c.reject_action_form3,
                                    c.edit_form3,
                                    c.edit_form4,
                                    c.process_status,
                                    c.step_form

                                };
                        int verify_report_form1 = 0;
                        int reject_report_form1 = 0;
                        int edit_form1 = 0;
                        int reject_report_form2 = 0;
                        int submit_report_form2 = 0;
                        int confirm_investigate_form2 = 0;
                        int confirm_by_groupohs_form2 = 0;
                        int edit_form2 = 0;
                        int request_close_form3 = 0;
                        int create_action_form3 = 0;
                        int edit_action_form3 = 0;
                        int close_action_form3 = 0;
                        int reject_action_form3 = 0;
                        int edit_form3 = 0;
                        int edit_form4 = 0;
                        int status = 0;
                        int step_form = 0;

                        foreach (var v in g)
                        {
                            verify_report_form1 = v.verify_report_form1 != null ? Convert.ToInt32(v.verify_report_form1) : 0;
                            reject_report_form1 = v.reject_report_form1 != null ? Convert.ToInt32(v.reject_report_form1) : 0;
                            edit_form1 = v.edit_form1 != null ? Convert.ToInt32(v.edit_form1) : 0;
                            reject_report_form2 = v.reject_report_form2 != null ? Convert.ToInt32(v.reject_report_form2) : 0;
                            submit_report_form2 = v.submit_report_form2 != null ? Convert.ToInt32(v.submit_report_form2) : 0;
                            confirm_investigate_form2 = v.confirm_investigate_form2 != null ? Convert.ToInt32(v.confirm_investigate_form2) : 0;
                            confirm_by_groupohs_form2 = v.confirm_by_groupohs_form2 != null ? Convert.ToInt32(v.confirm_by_groupohs_form2) : 0;
                            edit_form2 = v.edit_form2 != null ? Convert.ToInt32(v.edit_form2) : 0;
                            request_close_form3 = v.request_close_form3 != null ? Convert.ToInt32(v.request_close_form3) : 0;
                            create_action_form3 = v.create_action_form3 != null ? Convert.ToInt32(v.create_action_form3) : 0;
                            edit_action_form3 = v.edit_action_form3 != null ? Convert.ToInt32(v.edit_action_form3) : 0;
                            close_action_form3 = v.close_action_form3 != null ? Convert.ToInt32(v.close_action_form3) : 0;
                            reject_action_form3 = v.reject_action_form3 != null ? Convert.ToInt32(v.reject_action_form3) : 0;
                            edit_form3 = v.edit_form3 != null ? Convert.ToInt32(v.edit_form3) : 0;
                            edit_form4 = v.edit_form4 != null ? Convert.ToInt32(v.edit_form4) : 0;
                            status = v.process_status;
                            step_form = Convert.ToInt16(v.step_form);

                        }


                        if (status != 2 && status != 3 && status != 4)//close and reject and exemption
                        {

                            if (permission == "report incident1 verify")
                            {
                                int v_1 = safetys4.Class.SafetyPermission.getGroupValue(verify_report_form1);
                                if (group_value < v_1 || v_1 == 0)//group ที่ login เข้ามาสิทธิ์ใหญ่กว่าที่ทำล่าสุดใน table เพราะฉะนั้นก็ต้องเห็นสิทธิ์นี้
                                {
                                    result = true;
                                }
                            }

                            if (permission == "report incident1 reject")
                            {

                                int v_2 = safetys4.Class.SafetyPermission.getGroupValue(reject_report_form1);
                                if (group_value < v_2 || v_2 == 0)
                                {
                                    result = true;
                                }
                            }

                            if (permission == "report incident1 edit")
                            {
                                //int v_1 = safetys4.Class.SafetyPermission.getGroupValue(verify_report_form1);
                                //int v_3 = safetys4.Class.SafetyPermission.getGroupValue(edit_form1);
                                ////group_value < v_1 อันนี้กรณีแบบ มีการกดปุ่มไป step 2 เช่่นโดย supervisor ,supervisor กดอีกไมได้แต่ area oh&s ยังต้อง edit ได้เพราะยังไมได้แก้ไง
                                //if (group_value < v_3 || v_3 == 0 || step_form == 1)//|| group_value < v_1//step form เช็คกรณีไป form2 แล้วแก้ form1 ไปแล้วจะกลับมาแก้ไมได้ต้องรอคนสิทธิ์ใหญ่กว่ามาแก้
                                //{
                                    result = true;
                                //}
                            }

                            if (permission == "report incident2 reject")
                            {
                                int v_4 = safetys4.Class.SafetyPermission.getGroupValue(reject_report_form2);
                                if (group_value < v_4 || v_4 == 0)//==0 คือยังไม่มีใครกด
                                {
                                    result = true;
                                }

                            }


                            if (permission == "report incident3 except")
                            {
                                 result = true;                                

                            }

                            if (permission == "report incident2 confirm")
                            {

                                int v_5 = safetys4.Class.SafetyPermission.getGroupValue(confirm_investigate_form2);
                                if (group_value < v_5 || v_5 == 0)
                                {
                                    result = true;
                                }
                            }

                            if (permission == "report incident2 approve")
                            {
                                int v_6 = safetys4.Class.SafetyPermission.getGroupValue(submit_report_form2);
                                if (group_value < v_6 || v_6 == 0)
                                {
                                    result = true;
                                }

                            }

                            if (permission == "report incident2 edit")
                            {
                                //int v_5 = safetys4.Class.SafetyPermission.getGroupValue(confirm_investigate_form2);
                                //int v_7 = safetys4.Class.SafetyPermission.getGroupValue(edit_form2);
                                //if (group_value < v_7 || v_7 == 0 || step_form == 2)//|| group_value < v_5 คิดว่าไม่จำเป็นแล้วอ่ะ
                                //{
                                    result = true;
                               // }

                            }

                            if (permission == "report incident3 request close")
                            {
                                int v_8 = safetys4.Class.SafetyPermission.getGroupValue(request_close_form3);
                                if (group_value < v_8 || v_8 == 0)
                                {
                                    result = true;
                                }

                            }

                            if (permission == "report incident2 confirm groupohs")
                            {
                                int v_9 = safetys4.Class.SafetyPermission.getGroupValue(confirm_by_groupohs_form2);
                                if (group_value < v_9 || v_9 == 0)
                                {
                                    result = true;
                                }

                            }

                            //if (permission == "report incident3 action add")
                            //{
                            //    int v_9 = safetys4.Class.SafetyPermission.getGroupValue(create_action_form3);
                            //    if (group_value < v_9 || v_9 == 0)
                            //    {
                            //        result = true;
                            //    }

                            //}

                            //if (permission == "report incident3 action edit")
                            //{
                            //    int v_10 = safetys4.Class.SafetyPermission.getGroupValue(edit_action_form3);
                            //    if (group_value < v_10 || v_10 == 0)
                            //    {
                            //        result = true;
                            //    }

                            //}

                            //if (permission == "report incident3 action close")
                            //{
                            //    int v_11 = safetys4.Class.SafetyPermission.getGroupValue(close_action_form3);
                            //    if (group_value < v_11 || v_11 == 0)
                            //    {
                            //        result = true;
                            //    }

                            //}

                            //if (permission == "report incident3 action reject")
                            //{
                            //    int v_12 = safetys4.Class.SafetyPermission.getGroupValue(reject_action_form3);
                            //    if (group_value < v_12 || v_12 == 0)
                            //    {
                            //        result = true;
                            //    }

                            //}

                            if (permission == "report incident3 edit")
                            {
                                //int v_8 = safetys4.Class.SafetyPermission.getGroupValue(request_close_form3);
                                //int v_13 = safetys4.Class.SafetyPermission.getGroupValue(edit_form3);
                                //if (group_value < v_13 || v_13 == 0 || step_form == 3)//|| group_value < v_8
                                //{
                                    result = true;
                                //}
                            }


                            if (permission == "report incident4 edit")
                            {
                                //int v_14 = safetys4.Class.SafetyPermission.getGroupValue(edit_form4);
                                //if (group_value < v_14 || v_14 == 0 || step_form == 4)
                                //{
                                    result = true;
                                //}

                            }

                        }
                        else
                        {
                            if (group_value == 2)//super admin ,group,.... close incident edit form1-3 ,view all
                            {
                                if (permission == "report incident1 edit")
                                {
                                    result = true;
                                }


                                if (permission == "report incident2 edit")
                                {
                                    result = true;
                                }

                                if (permission == "report incident3 edit")
                                {
                                    result = true;

                                }

                                if (permission == "report incident3 action add")
                                {
                                    result = true;
                                }

                                if (permission == "report incident3 action edit")
                                {
                                    result = true;
                                }

                                if (permission == "report incident3 action close")
                                {
                                    result = true;
                                }

                                if (permission == "report incident3 action reject")
                                {
                                    result = true;
                                }

                            }


                        }//end check status close or onprocess


                    }
                    else
                    {


                        var h = from c in dbConnect.hazards
                                where c.id == Convert.ToInt32(report_id)
                                select new
                                {
                                    c.verify_report_form1,
                                    c.reject_report_form1,
                                    c.edit_form1,
                                    c.process_action_form2,
                                    c.reject_report_form2,
                                    c.submit_report_form2,
                                    c.edit_form2,
                                    c.request_close_form3,
                                    c.create_action_form3,
                                    c.edit_action_form3,
                                    c.close_action_form3,
                                    c.reject_action_form3,
                                    c.edit_form3,
                                    c.edit_form4,
                                    c.process_status,
                                    c.step_form

                                };
                        int verify_report_form1 = 0;
                        int reject_report_form1 = 0;
                        int edit_form1 = 0;
                        int reject_report_form2 = 0;
                        int process_action_form2 = 0;
                        int submit_report_form2 = 0;
                        int edit_form2 = 0;
                        int request_close_form3 = 0;
                        int create_action_form3 = 0;
                        int edit_action_form3 = 0;
                        int close_action_form3 = 0;
                        int reject_action_form3 = 0;
                        int edit_form3 = 0;
                        int edit_form4 = 0;
                        int status = 0;
                        int step_form = 0;

                        foreach (var v in h)
                        {
                            verify_report_form1 = v.verify_report_form1 != null ? Convert.ToInt32(v.verify_report_form1) : 0;
                            reject_report_form1 = v.reject_report_form1 != null ? Convert.ToInt32(v.reject_report_form1) : 0;
                            edit_form1 = v.edit_form1 != null ? Convert.ToInt32(v.edit_form1) : 0;
                            reject_report_form2 = v.reject_report_form2 != null ? Convert.ToInt32(v.reject_report_form2) : 0;
                            submit_report_form2 = v.submit_report_form2 != null ? Convert.ToInt32(v.submit_report_form2) : 0;
                            process_action_form2 = v.process_action_form2 != null ? Convert.ToInt32(v.process_action_form2) : 0;
                            edit_form2 = v.edit_form2 != null ? Convert.ToInt32(v.edit_form2) : 0;
                            request_close_form3 = v.request_close_form3 != null ? Convert.ToInt32(v.request_close_form3) : 0;
                            create_action_form3 = v.create_action_form3 != null ? Convert.ToInt32(v.create_action_form3) : 0;
                            edit_action_form3 = v.edit_action_form3 != null ? Convert.ToInt32(v.edit_action_form3) : 0;
                            close_action_form3 = v.close_action_form3 != null ? Convert.ToInt32(v.close_action_form3) : 0;
                            reject_action_form3 = v.reject_action_form3 != null ? Convert.ToInt32(v.reject_action_form3) : 0;
                            edit_form3 = v.edit_form3 != null ? Convert.ToInt32(v.edit_form3) : 0;
                            edit_form4 = v.edit_form4 != null ? Convert.ToInt32(v.edit_form4) : 0;
                            status = v.process_status;
                            step_form = Convert.ToInt16(v.step_form);
                        }


                        if (status != 2 && status != 3)//close and reject
                        {
                            if (permission == "report hazard1 verify")
                            {
                                int v_1 = safetys4.Class.SafetyPermission.getGroupValue(verify_report_form1);
                                if (group_value < v_1 || v_1 == 0)//group ที่ login เข้ามาสิทธิ์ใหญ่กว่าที่ทำล่าสุดใน table เพราะฉะนั้นก็ต้องเห็นสิทธิ์นี้
                                {
                                    result = true;
                                }
                            }

                            if (permission == "report hazard1 reject")
                            {

                                int v_2 = safetys4.Class.SafetyPermission.getGroupValue(reject_report_form1);
                                if (group_value < v_2 || v_2 == 0)
                                {
                                    result = true;
                                }
                            }

                            if (permission == "report hazard1 edit")
                            {
                                //int v_1 = safetys4.Class.SafetyPermission.getGroupValue(verify_report_form1);
                                //int v_3 = safetys4.Class.SafetyPermission.getGroupValue(edit_form1);
                                //if (group_value < v_3 || v_3 == 0 || step_form == 1)// || group_value < v_1
                                //{
                                    result = true;
                               // }
                            }

                            if (permission == "report hazard2 reject")
                            {
                                int v_4 = safetys4.Class.SafetyPermission.getGroupValue(reject_report_form2);
                                if (group_value < v_4 || v_4 == 0)
                                {
                                    result = true;
                                }

                            }

                            if (permission == "report hazard2 process")
                            {

                                int v_5 = safetys4.Class.SafetyPermission.getGroupValue(process_action_form2);
                                if (group_value < v_5 || v_5 == 0)
                                {
                                    result = true;
                                }
                            }

                            if (permission == "report hazard2 submit")
                            {
                                int v_6 = safetys4.Class.SafetyPermission.getGroupValue(submit_report_form2);
                                if (group_value < v_6 || v_6 == 0)
                                {
                                    result = true;
                                }

                            }

                            if (permission == "report hazard2 edit")
                            {
                                //int v_5 = safetys4.Class.SafetyPermission.getGroupValue(process_action_form2);
                                //int v_7 = safetys4.Class.SafetyPermission.getGroupValue(edit_form2);
                                //if (group_value < v_7 || v_7 == 0 || step_form == 2)//|| group_value < v_5
                                //{
                                    result = true;
                               // }

                            }



                            //if (permission == "report hazard3 action add")
                            //{
                            //    int v_8 = safetys4.Class.SafetyPermission.getGroupValue(create_action_form3);
                            //    if (group_value < v_8 || v_8 == 0)
                            //    {
                            //        result = true;
                            //    }

                            //}

                            //if (permission == "report hazard3 action edit")
                            //{
                            //    int v_9 = safetys4.Class.SafetyPermission.getGroupValue(edit_action_form3);
                            //    if (group_value < v_9 || v_9 == 0)
                            //    {
                            //        result = true;
                            //    }

                            //}

                            //if (permission == "report hazard3 action close")
                            //{
                            //    int v_10 = safetys4.Class.SafetyPermission.getGroupValue(close_action_form3);
                            //    if (group_value < v_10 || v_10 == 0)
                            //    {
                            //        result = true;
                            //    }

                            //}

                            //if (permission == "report hazard3 action reject")
                            //{
                            //    int v_11 = safetys4.Class.SafetyPermission.getGroupValue(reject_action_form3);
                            //    if (group_value < v_11 || v_11 == 0)
                            //    {
                            //        result = true;
                            //    }

                            //}

                            if (permission == "report hazard3 edit")
                            {
                                //int v_14 = safetys4.Class.SafetyPermission.getGroupValue(request_close_form3);
                                //int v_12 = safetys4.Class.SafetyPermission.getGroupValue(edit_form3);
                                //if (group_value < v_12 || v_12 == 0 || step_form == 3)//|| group_value < v_14
                                //{
                                    result = true;
                                //}
                            }


                            if (permission == "report hazard4 edit")
                            {
                                //int v_13 = safetys4.Class.SafetyPermission.getGroupValue(edit_form4);
                                //if (group_value < v_13 || v_13 == 0 || step_form == 4)
                                //{
                                    result = true;
                                //}

                            }

                            if (permission == "report hazard3 request close")
                            {

                                int v_14 = safetys4.Class.SafetyPermission.getGroupValue(request_close_form3);
                                if (group_value < v_14 || v_14 == 0)
                                {
                                    result = true;
                                }
                            }






                        }
                        else
                        {
                            if (group_value == 2)//super admin ,group,.... close incident edit form1-3 ,view all
                            {
                                if (permission == "report hazard1 edit")
                                {
                                    result = true;
                                }


                                if (permission == "report hazard2 edit")
                                {
                                    result = true;
                                }

                                if (permission == "report hazard3 edit")
                                {
                                    result = true;

                                }

                                if (permission == "report hazard3 action add")
                                {
                                    result = true;
                                }

                                if (permission == "report hazard3 action edit")
                                {
                                    result = true;
                                }

                                if (permission == "report hazard3 action close")
                                {
                                    result = true;
                                }

                                if (permission == "report hazard3 action reject")
                                {
                                    result = true;
                                }

                            }

                        }//end check status

                    }//end else




                }





                return result;
            }
        }


        protected static int getGroupValue(int group_id)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {

                var g = from c in dbConnect.groups
                        where c.id == group_id
                        select new
                        {
                            c.value

                        };


                int group_value = 0;
                foreach (var v in g)
                {
                    group_value = v.value != null ? Convert.ToInt32(v.value) : 0;
                }

                return group_value;
            }
        }



        public static bool checkPermisionInArea(string report_id, string type_report)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                bool result = false;

                //string function_id = HttpContext.Current.Session["function_id"].ToString();
                //string department_id = HttpContext.Current.Session["department_id"].ToString();
                //string division_id = HttpContext.Current.Session["division_id"].ToString();
                //string section_id = HttpContext.Current.Session["section_id"].ToString();

                ArrayList functions = HttpContext.Current.Session["area_function"] as ArrayList;
                ArrayList departments = HttpContext.Current.Session["area_department"] as ArrayList;
                ArrayList department_functional = HttpContext.Current.Session["area_department_functional"] as ArrayList;
                ArrayList divisions = HttpContext.Current.Session["area_division"] as ArrayList;
                ArrayList sections = HttpContext.Current.Session["area_section"] as ArrayList;

                ArrayList group = HttpContext.Current.Session["group"] as ArrayList;



                if (report_id != "")
                {
                    if (type_report == "incident")
                    {
                        var incidents = from c in dbConnect.incidents
                                        where c.id == Convert.ToInt32(report_id)
                                        select new
                                        {
                                            function_id = c.function_id,
                                            department_id = c.department_id,
                                            division_id = c.division_id,
                                            section_id = c.section_id,
                                            activity_company_id = c.activity_company_id,     
                                            activity_function_id = c.activity_function_id,
                                            activity_department_id = c.activity_department_id,
                                            activity_division_id = c.activity_division_id,
                                            activity_section_id = c.activity_section_id,
                                            c.responsible_area,
                                            c.owner_activity,

                                            incident_id = c.id,

                                        };

                        String function_id2 = "";
                        String department_id2 = "";
                        String division_id2 = "";
                        String section_id2 = "";
                        String activity_function_id2 = "";
                        String activity_department_id2 = "";
                        String activity_division_id2 = "";
                        String activity_section_id2 = "";
                        String responsible_area = "";
                        String owner_activity = "";


                        foreach (var v in incidents)
                        {
                            function_id2 = v.function_id;
                            department_id2 = v.department_id;
                            division_id2 = v.division_id;
                            section_id2 = v.section_id;

                            activity_function_id2 = v.activity_function_id;
                            activity_department_id2 = v.activity_department_id;
                            activity_division_id2 = v.activity_division_id;
                            activity_section_id2 = v.activity_section_id;

                            responsible_area = v.responsible_area;
                            owner_activity = v.owner_activity;

                        }



                        if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S") > -1)
                        {

                            result = true;
                        }
                        else
                        {


                            if (owner_activity == "KNOWN")
                            {
                                if (group.IndexOf("Area Supervisor") > -1)
                                {
                                    if (sections.IndexOf(activity_section_id2) > -1)
                                    {
                                        result = true;
                                    }

                                }

                                if (group.IndexOf("Area Manager") > -1)
                                {
                                    if (divisions.IndexOf(activity_division_id2) > -1)
                                    {
                                        result = true;
                                    }

                                }


                                if (group.IndexOf("Area OH&S") > -1)
                                {
                                    if (departments.IndexOf(activity_department_id2) > -1)
                                    {
                                        result = true;
                                    }

                                }



                                if (group.IndexOf("Delegate OH&S Admin") > -1)
                                {
                                    if (functions.IndexOf(activity_function_id2) > -1)
                                    {
                                        result = true;
                                    }

                                }

                                if (group.IndexOf("OH&S Admin") > -1)
                                {
                                    if (functions.IndexOf(activity_function_id2) > -1)
                                    {
                                        result = true;
                                    }

                                }

                            }



                            if (owner_activity != "KNOWN" || responsible_area == "IN")
                             {

                                 if (group.IndexOf("Area Supervisor") > -1)
                                 {
                                     if (sections.IndexOf(section_id2) > -1)
                                     {
                                         result = true;
                                     }

                                 }

                                 if (group.IndexOf("Area Manager") > -1)
                                 {
                                     if (divisions.IndexOf(division_id2) > -1)
                                     {
                                         result = true;
                                     }

                                 }


                                 if (group.IndexOf("Area OH&S") > -1)
                                 {
                                     if (departments.IndexOf(department_id2) > -1)
                                     {
                                         result = true;
                                     }

                                 }



                                 if (group.IndexOf("Delegate OH&S Admin") > -1)
                                 {
                                     if (functions.IndexOf(function_id2) > -1)
                                     {
                                         result = true;
                                     }

                                 }

                                 if (group.IndexOf("OH&S Admin") > -1)
                                 {
                                     if (functions.IndexOf(function_id2) > -1)
                                     {
                                         result = true;
                                     }

                                 }

                             }

                            




                        }



                    }
                    else if (type_report == "health")
                    {
                        var incidents = from c in dbConnect.healths
                                        where c.id == Convert.ToInt32(report_id)
                                        select new
                                        {
                                            function_id = c.function_id,
                                            department_id = c.department_id,
                                            division_id = c.division_id,
                                            section_id = c.section_id,
                                            incident_id = c.id,

                                        };

                        String function_id2 = "";
                        String department_id2 = "";
                        String division_id2 = "";
                        String section_id2 = "";

                        foreach (var v in incidents)
                        {
                            function_id2 = v.function_id;
                            department_id2 = v.department_id;
                            division_id2 = v.division_id;
                            section_id2 = v.section_id;

                        }



                        if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S Health") > -1)
                        {

                            result = true;
                        }
                        else
                        {
                          


                            if (group.IndexOf("Area OH&S") > -1)
                            {
                                if (departments.IndexOf(department_id2) > -1)
                                {
                                    result = true;
                                }

                            }


                            if (group.IndexOf("Area Functional Manager") > -1)
                            {
                                if (department_functional.IndexOf(department_id2) > -1)
                                {
                                    result = true;
                                }

                            }



                            if (group.IndexOf("Delegate OH&S Admin") > -1)
                            {
                                if (functions.IndexOf(function_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                            if (group.IndexOf("OH&S Admin") > -1)
                            {
                                if (functions.IndexOf(function_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                        }



                    }
                    else
                    {//endchek type

                        var hazards = from c in dbConnect.hazards
                                      where c.id == Convert.ToInt32(report_id)
                                      select new
                                      {
                                          function_id = c.function_id,
                                          department_id = c.department_id,
                                          division_id = c.division_id,
                                          section_id = c.section_id,
                                          incident_id = c.id,

                                      };

                        String function_id2 = "";
                        String department_id2 = "";
                        String division_id2 = "";
                        String section_id2 = "";

                        foreach (var v in hazards)
                        {
                            function_id2 = v.function_id;
                            department_id2 = v.department_id;
                            division_id2 = v.division_id;
                            section_id2 = v.section_id;

                        }


                        if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S") > -1)
                        {

                            result = true;
                        }
                        else
                        {
                            if (group.IndexOf("Area Supervisor") > -1)
                            {
                                if (sections.IndexOf(section_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                            if (group.IndexOf("Area Manager") > -1)
                            {
                                if (divisions.IndexOf(division_id2) > -1)
                                {
                                    result = true;
                                }

                            }


                            if (group.IndexOf("Area OH&S") > -1)
                            {
                                if (departments.IndexOf(department_id2) > -1)
                                {
                                    result = true;
                                }

                            }



                            if (group.IndexOf("Delegate OH&S Admin") > -1)
                            {
                                if (functions.IndexOf(function_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                            if (group.IndexOf("OH&S Admin") > -1)
                            {
                                if (functions.IndexOf(function_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                        }



                    }//end else
                }



                return result;
            }
        }


        public static bool checkPermissionHealthCreator(string report_id, string creator_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                bool result = false;

                ArrayList group = HttpContext.Current.Session["group"] as ArrayList;


                if (report_id != "")
                {
                    var healths = from c in dbConnect.healths
                                    where c.id == Convert.ToInt32(report_id)
                                    select new
                                    {
                                        function_id = c.function_id,
                                        department_id = c.department_id,
                                        division_id = c.division_id,
                                        section_id = c.section_id,
                                        health_id = c.id,
                                        c.employee_id,
                                        c.process_status

                                    };

                    String function_id2 = "";
                    String department_id2 = "";
                    String division_id2 = "";
                    String section_id2 = "";
                    String employee_id = "";
                    int status  = 0;

                    foreach (var v in healths)
                    {
                        function_id2 = v.function_id;
                        department_id2 = v.department_id;
                        division_id2 = v.division_id;
                        section_id2 = v.section_id;
                        employee_id = v.employee_id;
                        status = v.process_status;

                    }

                    if (status != 3)//reject แก้ไม่ได้
                    {

                        if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S Health") > -1)
                        {

                            result = true;
                        }
                        else
                        {


                            if (group.IndexOf("OH&S Admin") > -1 || group.IndexOf("Delegate OH&S Admin") > -1)
                            {
                                result = true;

                            }
                            else
                            {

                                if (group.IndexOf("Area Functional Manager") > -1)
                                {
                                    result = true;
                                }


                                if (group.IndexOf("Area OH&S") > -1)
                                {
                                    //พี่ออยสั่งแก้ เพราะมีเคส area oh&s แก้ไม่ได้
                                    //if (employee_id == creator_id)///เป็นคน create form
                                    //{
                                    result = true;
                                    //}
                                    //else
                                    //{
                                    //    result = false;
                                    //}


                                }

                            }


                        }

                    }

                    
                }



                return result;

            }

           
        }




        public static bool checkPermissionHealthForm3(string report_id)
        {
            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                bool result = true;

                ArrayList group = HttpContext.Current.Session["group"] as ArrayList;


                if (report_id != "")
                {
                    var healths = from c in dbConnect.healths
                                  where c.id == Convert.ToInt32(report_id)
                                  select new
                                  {
                                      c.process_status,
                                      c.step_form

                                  };

                    int status = 0;
                    int step_form = 0;

                    foreach (var v in healths)
                    {
                    
                        status = v.process_status;
                        step_form = Convert.ToInt16(v.step_form);

                    }

                    if (status == 1)//onprocess
                    {
                        if(step_form==3)
                        {
                            if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S Health") > -1)
                            {
                                result = true;
                            }
                            else
                            {
                                var form3 = (from c in dbConnect.log_request_close_healths
                                             where c.health_id == Convert.ToInt32(report_id)
                                             orderby c.id descending
                                             select c).Take(1);

                                foreach (var g in form3)
                                {
                                    if(g.status_process=="P")//ถ้ายังไม่ปิดเคส แล้วอยู่ฟอร์ม 3 แล้ว ถ้าไม่ใช่ not approve ฟอร์ม 1 กับ 2 จะแก้ไม่ได้
                                   {
                                        result = false;
                                   }

                                }

                            }

                        }


                    }
                    else
                    {

                        if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S Health") > -1)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;

                        }

                    }


                }



                return result;

            }


        }


        public static bool checkPermisionInAreaMobile(string report_id, string type_report, ArrayList functions, ArrayList departments, ArrayList divisions, ArrayList sections,ArrayList group)
        {

            using (safetys4dbDataContext dbConnect = new safetys4dbDataContext())
            {
                bool result = false;


                if (report_id != "")
                {
                    if (type_report == "incident")
                    {
                        var incidents = from c in dbConnect.incidents
                                        where c.id == Convert.ToInt32(report_id)
                                        select new
                                        {
                                            function_id = c.function_id,
                                            department_id = c.department_id,
                                            division_id = c.division_id,
                                            section_id = c.section_id,
                                            incident_id = c.id,

                                        };

                        String function_id2 = "";
                        String department_id2 = "";
                        String division_id2 = "";
                        String section_id2 = "";

                        foreach (var v in incidents)
                        {
                            function_id2 = v.function_id;
                            department_id2 = v.department_id;
                            division_id2 = v.division_id;
                            section_id2 = v.section_id;

                        }



                        if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S") > -1)
                        {

                            result = true;
                        }
                        else
                        {
                            if (group.IndexOf("Area Supervisor") > -1)
                            {
                                if (sections.IndexOf(section_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                            if (group.IndexOf("Area Manager") > -1)
                            {
                                if (divisions.IndexOf(division_id2) > -1)
                                {
                                    result = true;
                                }

                            }


                            if (group.IndexOf("Area OH&S") > -1)
                            {
                                if (departments.IndexOf(department_id2) > -1)
                                {
                                    result = true;
                                }

                            }



                            if (group.IndexOf("Delegate OH&S Admin") > -1)
                            {
                                if (functions.IndexOf(function_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                            if (group.IndexOf("OH&S Admin") > -1)
                            {
                                if (functions.IndexOf(function_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                        }



                    }
                    else
                    {//endchek type

                        var hazards = from c in dbConnect.hazards
                                      where c.id == Convert.ToInt32(report_id)
                                      select new
                                      {
                                          function_id = c.function_id,
                                          department_id = c.department_id,
                                          division_id = c.division_id,
                                          section_id = c.section_id,
                                          incident_id = c.id,

                                      };

                        String function_id2 = "";
                        String department_id2 = "";
                        String division_id2 = "";
                        String section_id2 = "";

                        foreach (var v in hazards)
                        {
                            function_id2 = v.function_id;
                            department_id2 = v.department_id;
                            division_id2 = v.division_id;
                            section_id2 = v.section_id;

                        }


                        if (group.IndexOf("Super Admin") > -1 || group.IndexOf("Delegate Super Admin") > -1 || group.IndexOf("Group OH&S") > -1)
                        {

                            result = true;
                        }
                        else
                        {
                            if (group.IndexOf("Area Supervisor") > -1)
                            {
                                if (sections.IndexOf(section_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                            if (group.IndexOf("Area Manager") > -1)
                            {
                                if (divisions.IndexOf(division_id2) > -1)
                                {
                                    result = true;
                                }

                            }


                            if (group.IndexOf("Area OH&S") > -1)
                            {
                                if (departments.IndexOf(department_id2) > -1)
                                {
                                    result = true;
                                }

                            }



                            if (group.IndexOf("Delegate OH&S Admin") > -1)
                            {
                                if (functions.IndexOf(function_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                            if (group.IndexOf("OH&S Admin") > -1)
                            {
                                if (functions.IndexOf(function_id2) > -1)
                                {
                                    result = true;
                                }

                            }

                        }



                    }//end else
                }



                return result;
            }
        }




    }





}