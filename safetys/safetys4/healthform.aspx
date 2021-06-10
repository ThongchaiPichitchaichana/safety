<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="healthform.aspx.cs" Inherits="safetys4.healthform" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="template/css/plugins/dropzone/dropzone.css" rel="stylesheet" />
<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet"> 
<link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
<link href="template/css/plugins/select2/select2.min.css" rel="stylesheet">


<link rel="stylesheet" href="template/js/plugins/fancybox/jquery.fancybox.css" type="text/css" />
   
<script type="text/javascript" src="template/js/plugins/dropzone/dropzone.min.js"></script>
<script type="text/javascript" src="template/js/plugins/select2/select2.min.js"></script>
<script type="text/javascript" src="template/js/plugins/jquery.number/jquery.number.min.js"></script>

 <%
                
if (Session["lang"] != null)         
{
    if (Session["lang"] =="th")
    {                  
 %>
<script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker-custom.js"></script>

 <%                      
    }
else { 
       
%>

    <script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker.js"></script>

<%                      
   }  
 
 }    
%>


<script type="text/javascript" src="template/js/plugins/datepicker/locales/bootstrap-datepicker.th.js"></script>
<script type="text/javascript" src="template/js/plugins/fancybox/jquery.fancybox.pack.js"></script>
<script src="template/js/plugins/dataTables/jquery.dataTables.min.js"></script>  


<style>
.stepwizard-step p {
    margin-top: 10px;
}
.stepwizard-row {
    display: table-row;
}
.stepwizard {
    display: table;
    width: 100%;
    position: relative;
}
.stepwizard-step button[disabled] {
    opacity: 1 !important;
    filter: alpha(opacity=100) !important;
}
.stepwizard-row:before {
    top: 14px;
    bottom: 0;
    position: absolute;
    content: " ";
    width: 100%;
    height: 1px;
    background-color: #ccc;

}
.stepwizard-step {
    display: table-cell;
    text-align: center;
    position: relative;
}
.btn-circle {
    width: 30px;
    height: 30px;
    text-align: center;
    padding: 6px 0;
    font-size: 12px;
    line-height: 1.428571429;
    border-radius: 15px;
}


hr {
   
    border-top-color: #DE2F13;
    border-top-width: 3px;
    margin-top: 5px !important;
    margin-bottom: 5px !important;

}

 strong{
    color: #DE2F13;
 }


.dz-max-files-reached {
        background-color: red;
    }


.ui-dialog-titlebar-close {
    visibility: hidden;
  }

a {
        color:black !important;
    }

.a-step{
    color:white !important;
}


  .wrapper-content {
    margin-left: 0px !important;
} 

.dataTable  tr {
   height: 1px !important; /* or whatever height you need to make them all consistent */
}

</style>

  

 <script type="text/javascript">
    var store_employee = [];
    var id = "";
    var pagetype = "";
   

    var myDropzone;

    var dialogRiskFactor;
    var dataTableRiskFactor; //reference to your dataTable
    var risk_factor_relate_work_action_id = 0;

    var dialogOccupationalHealth;
    var dataTableOccupationalHealth; //reference to your dataTable
    var occupational_health_action_id = 0;

  //  var dialogProcessAction;
   // var dataTableProcessAction; //reference to your dataTable

    var process_action_id = 0;
    var filename_opinion_doctor = "";
    var filename_recovery_plan = "";
  //  var filename_process_action = "";

    var risk_factor_relate_work_id = 0;
    var filename_risk_factor = "";
    var file_risk_factor = [];

    var occupational_health_id = 0;
    var filename_result_health = "";
    var filename_repeat_result_health = "";
    var file_result_health = [];
    var file_repeat_result_health = [];

    var other_risk_factor = false;

    var dropzone_risk_factor;
    var dropzone_occupational_health;
    var dropzone_repeat_occupational_health;
   // var dropzone_opinion_doctor;
    //var dropzone_recovery_health;
    //var dropzone_process_action_health;

    var folder_image = "";
    var dialogReason;

    $(document).ready(function () {


    <%
                
    if (Session["lang"] != null)         
    {
        if (Session["lang"] =="th")
        {                  
            %>

        $('#due_date .input-group.date').datepicker({
            todayBtn: "linked",
            keyboardNavigation: false,
            forceParse: false,
            autoclose: true,
            format: "dd/mm/yyyy",
            language: 'th',
            thaiyear: true
        });
        <%                      
        }
        else { 
       
        %>
        $('#due_date .input-group.date').datepicker({
            todayBtn: "linked",
            keyboardNavigation: false,
            forceParse: false,
            autoclose: true,
            format: "dd/mm/yyyy",
            language: 'en',
            thaiyear: false
        });
        <%     
     
                }                 
            }
               
    %>



     <%
                
    if (Session["lang"] != null)         
    {
        if (Session["lang"] =="th")
        {                  
                %>
        $('#data_birth_date .input-group.date').datepicker({
            todayBtn: "linked",
            keyboardNavigation: false,
            forceParse: false,
            autoclose: true,
            format: "dd/mm/yyyy",
            language: 'th',
            thaiyear: true
        }).on('changeDate', function (e) {
            calculateage();
        });


        $('#data_hiring_date .input-group.date').datepicker({
            todayBtn: "linked",
            keyboardNavigation: false,
            forceParse: false,
            autoclose: true,
            format: "dd/mm/yyyy",
            language: 'th',
            thaiyear: true
        }).on('changeDate', function (e) {
            calculateserviceyear();
        });

        <%                      
        }
        else { 
       
        %>

        $('#data_birth_date .input-group.date').datepicker({
            todayBtn: "linked",
            keyboardNavigation: false,
            forceParse: false,
            autoclose: true,
            format: "dd/mm/yyyy",
            language: 'en',
            thaiyear: false
        }).on('changeDate', function (e) {
            calculateage();
        });


        $('#data_hiring_date .input-group.date').datepicker({
            todayBtn: "linked",
            keyboardNavigation: false,
            forceParse: false,
            autoclose: true,
            format: "dd/mm/yyyy",
            language: 'en',
            thaiyear: false
        }).on('changeDate', function (e) {
            calculateserviceyear();
        });

        <%     
     
                }                 
            }
               
    %>


        var url = window.location.href;
        var urlarr = url.split("=");

        if (urlarr.length > 2)
        {
            id = urlarr[2];
            pagetype_arr = urlarr[1].split("&");
            pagetype = pagetype_arr[0];

        } else {
            pagetype = urlarr[1];
            

        }
     
        if (pagetype == "view")
        {
            setShowedit();
            //$("#dropzoneForm").hide();
            //$("#infouploadimage").hide();
           

        } else if (pagetype == "edit") {
          
            setShowedit();
           // setDropzone();

          

        } else {
           
            setAreaSelect("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            // setDropzone();
            setYear("");
            getEmployee("");
            setHospital("");
           
        }

        setReasonReject();

        dialogReason = $("#create_reason_reject").dialog({
            autoOpen: false,
            height: 380,
            width: 400,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {
      
                $("#create_reason_reject").css('overflow-x', 'hidden');
            },
            modal: true,
        });

      
        setRiskFactorYear();
        setRiskFactorYear();
        setRiskFactorRelateWork();
        setOccupationalHealth();
       // setTypecontrol();
        setActionHealthStatus();
        setDurationRiskFactor();

        setDropzoneRiskFactor();
        setDropzoneOccupationalHealth();
        setDropzoneRepeatOccupationalHealth();
       // setDropzoneOpinionDoctor();
       // setDropzoneRecoveryPlan();
       // setDropzoneProcessActionHealth();

        //setDatatableProcessAction();
        setDatatableRiskFactor();
        setDatatableOccupationalHealth();
        setMonitoringResults();
        setAbnormalAudiogram();
        setAbnormalPulmonaryFunction();
        
      
       
       
        $('#MainContent_txtservice_year_current').number(true, 2);
        $('#MainContent_txtcigarettes_per_day').number(true);
        $('#MainContent_txtyears').number(true);
        $('#MainContent_txtmonths').number(true);

        
  

        //$("#MainContent_txtemployee_id").autocomplete({
        //    source: "Masterdata.asmx/getEmployeeIDAutocomplete",
        //    select: function (event, ui) {
               
        //        $("#MainContent_txtemployee_name").val(ui.item.first_name + " " + ui.item.last_name);
        //        setAreaSelect(ui.item.company_id, ui.item.function_id, ui.item.department_id, ui.item.division_id, ui.item.section_id, "", "", "", "", "", "", "", "", "", "");
        //    }
        //});





     


        dialogRiskFactor = $("#risk_factor_relate_work_form").dialog({
            autoOpen: false,
            height: 600,
            width: 800,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {
                //clearValidationErrors();
                // setRootCauseAction();
                $("#risk_factor_relate_work_form").css('overflow-x', 'hidden');
            },
            modal: true,
        });




        dialogOccupationalHealth = $("#occupational_health_form").dialog({
            autoOpen: false,
            height: 650,
            width: 650,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {
                //clearValidationErrors();
                // setRootCauseAction();
                $("#occupational_health_form").css('overflow-x', 'hidden');
            },
            modal: true,
        });




        //dialogProcessAction = $("#process_action_form").dialog({
        //    autoOpen: false,
        //    height: 650,
        //    width: 650,
        //    modal: true,

        //    close: function () {



        //    },
        //    open: function (event, ui) {
        //        //clearValidationErrors();
        //        // setRootCauseAction();
        //        $("#process_action_form").css('overflow-x', 'hidden');
        //    },
        //    modal: true,
        //});


        $("input[type=radio][name='ctl00$MainContent$repeathealthcheck']").change(function ()
        {
            if (this.value == 'N') {
                $("#show_upload_repeat_health_check").hide();
            }
            else if (this.value == 'Y') {
                $("#show_upload_repeat_health_check").show();

            }
        });


        $("input[type=radio][name='ctl00$MainContent$smoked_cigarettes']").change(function () {
            if (this.value == 'NO')
            {
                $("#MainContent_txtcigarettes_per_day").prop('disabled', true);
                $("#MainContent_txtyears").prop('disabled', true);
                $("#MainContent_txtmonths").prop('disabled', true);
                $("#MainContent_txtsmoked_other").prop('disabled', true);
            }
            else if (this.value == 'YES_SMOKING') {
                
                $("#MainContent_txtcigarettes_per_day").prop('disabled', false);
                $("#MainContent_txtyears").prop('disabled', true);
                $("#MainContent_txtmonths").prop('disabled', true);
                $("#MainContent_txtsmoked_other").prop('disabled', true);

            } else if (this.value == 'YES_SMOKED') {

                $("#MainContent_txtcigarettes_per_day").prop('disabled', true);
                $("#MainContent_txtyears").prop('disabled', false);
                $("#MainContent_txtmonths").prop('disabled', false);
                $("#MainContent_txtsmoked_other").prop('disabled', true);

            } else if (this.value == 'SMOKED_OTHER') {

                $("#MainContent_txtcigarettes_per_day").prop('disabled', true);
                $("#MainContent_txtyears").prop('disabled', true);
                $("#MainContent_txtmonths").prop('disabled', true);
                $("#MainContent_txtsmoked_other").prop('disabled', false);
            }
        });

        

    
        $('.select2').select2();


        $("input[type=radio][name='ctl00$MainContent$monitoringenvironment']").change(function () {
            if (this.value == 'N') {
                $("#year_risk_factor").hide();
                $("#year_risk_factor2").hide();
                $("#year_risk_factor3").hide();
                $("#year_risk_factor4").hide();
                $("#MainContent_ddlYearriskfactor").val("");
            }
            else if (this.value == 'Y') {
                $("#year_risk_factor").show();
                $("#year_risk_factor2").show();
                $("#year_risk_factor3").show();
                $("#year_risk_factor4").show();

            }
        });

     
        $("input[type=radio][name='ctl00$MainContent$chronic_diseases_ear']").change(function () {
            if (this.value == 'N') {
                $("#specify_abnormalaudiogram").hide();
            }
            else if (this.value == 'Y') {
                $("#specify_abnormalaudiogram").show();

            }
        });




        $("#MainContent_ddlOccupationalhealth").change(function () {
            var ddlOccupationalhealth = this.value;

            if (ddlOccupationalhealth == 3) {
                $("#abnormalaudiogram").show();
                $("#abnormalaudiogram2").show();

                $("#abnormal_pulmonary_function").hide();
                $("#abnormal_pulmonary_function2").hide();

                $("#MainContent_ddabnormal_pulmonary_function").val("");
                $('input[name="ctl00$MainContent$smoked_cigarettes"]').attr('checked', false);
                $("#MainContent_txtcigarettes_per_day").val("");
                $("#MainContent_txtyears").val("");
                $("#MainContent_txtmonths").val("");
                $("#MainContent_txtsmoked_other").val("");

            }else if (ddlOccupationalhealth == 2) {
                $("#abnormal_pulmonary_function").show();
                $("#abnormal_pulmonary_function2").show();

                $("#abnormalaudiogram").hide();
                $("#abnormalaudiogram2").hide();

                $("#MainContent_ddabnormalaudiogram").val("");
                $("#MainContent_txthearing_threshold_level").val("");
                $('input[name="ctl00$MainContent$chronic_diseases_ear"]').attr('checked', false);
                $("#MainContent_txtspecify_chronic_diseases_ear").val("");

            } else {

                $("#abnormalaudiogram").hide();
                $("#abnormalaudiogram2").hide();

                $("#abnormal_pulmonary_function").hide();
                $("#abnormal_pulmonary_function2").hide();


                $("#MainContent_ddabnormalaudiogram").val("");
                $("#MainContent_txthearing_threshold_level").val("");
                $('input[name="ctl00$MainContent$chronic_diseases_ear"]').attr('checked', false);
                $("#MainContent_txtspecify_chronic_diseases_ear").val("");

                $("#MainContent_ddabnormal_pulmonary_function").val("");
                $('input[name="ctl00$MainContent$smoked_cigarettes"]').attr('checked', false);
                $("#MainContent_txtcigarettes_per_day").val("");
                $("#MainContent_txtyears").val("");
                $("#MainContent_txtmonths").val("");
                $("#MainContent_txtsmoked_other").val("");
            }
        });


    });




     function calculateage()
     {
         var date_birth = $("#MainContent_txtdateofbirth").val();
         $.ajax({
             type: "GET",
             data: { birthdate: date_birth, lang: lang },
             url: 'Actionevent.asmx/getAge',
             //dataType: 'json',
             async: false,
             cache: false,
             success: function (result) {

                 $("#MainContent_txtage").val(result);

             },
             error: function (e) {

                 console.log(e);
             }
         });


     }



     function calculateserviceyear()
     {
         var date_hiring = $("#MainContent_txthiring_date").val();
         $.ajax({
             type: "GET",
             data: { hiringdate: date_hiring, lang: lang },
             url: 'Actionevent.asmx/getServiceyear',
             //dataType: 'json',
             async: false,
             cache: false,
             success: function (result) {

                 $("#MainContent_txtservice_year").val(result);

             },
             error: function (e) {

                 console.log(e);
             }
         });


     }


    




   





    function addRiskFactor()
    {
        var monitoringenvironment = $("input:radio[name='ctl00$MainContent$monitoringenvironment']:checked").val();

        if (Page_ClientValidate("riskfactor") && monitoringenvironment != undefined)
        {
           
            var risk_factor_relate_work = $("#MainContent_ddlRiskfactorrelatework").val();
            var other_risk_factor = $("#MainContent_txtotherriskfactor").val();
            var year_factor = $("#MainContent_ddlYearriskfactor").val();

            if (year_factor == undefined)
            {
                year_factor = "";
            }


            var result_risk_factor = $("#MainContent_txtresultriskfactor").val();
            var duration_risk_factor = $("#MainContent_ddlDurationriskfactor").val();
            var monitoring_results = $("#MainContent_ddmonitoringresults").val();

            if ((monitoringenvironment == "Y" && file_risk_factor.length > 0) || monitoringenvironment == "N")
            {
                showLoading();
                for (i in file_risk_factor)
                {
                    if (filename_risk_factor != "") {
                        filename_risk_factor = filename_risk_factor + "," + file_risk_factor[i];

                    } else {
                        filename_risk_factor = file_risk_factor[i];
                    }
                }

                $.ajax({
                    type: "POST",
                    data: {
                        risk_factor_relate_work: risk_factor_relate_work,
                        other_risk_factor: other_risk_factor,
                        year_factor: year_factor,
                        duration_risk_factor: duration_risk_factor,
                        file_risk_factor: filename_risk_factor,
                        result_risk_factor: result_risk_factor,
                        user_id: user_login_id,
                        health_id: id,
                        page_type: pagetype,
                        monitoring_environment: monitoringenvironment,
                        monitoring_results: monitoring_results

                    },
                    url: 'Actionevent.asmx/createRiskFactor',
                    dataType: 'json',
                    success: function (result) {
                        closeLoading();
                        closeRiskFactor();
                        clearRiskFactor();
                        callRiskFactorRelate();

                        dropzone_risk_factor.removeAllFiles();
                    }
                });



            } else {

                var require_file_risk_factor = '<%= Resources.Health.rqfileriskfactor %>';
                alert(require_file_risk_factor);
              

                return false;

            }

           

        }
        else {

            if (monitoringenvironment == undefined) {
                var require_monitoringenvironment = '<%= Resources.Health.rqmonitoringenvironment %>';
                $("#rqmonitoringenvironment").text(require_monitoringenvironment);
            }
            return false;
        }


    }

    

    function  addOccupationalHealth()
    {
 
        var repeat_health = $("input:radio[name='ctl00$MainContent$repeathealthcheck']:checked").val();
      

        if (Page_ClientValidate("occupationalhealth") && repeat_health != undefined)
        {
            if ((repeat_health == "Y"))
            {

                if (file_repeat_result_health.length > 0 && file_result_health.length >0)
                {

                    $.when(checkValidateOccupational()).done(function (value) {
                        if (value)
                        {
                            sentAddOccupationalHealth();
                        } else {
                            return false;
                        }
                    });
                   

                } else {

                    if (file_repeat_result_health.length==0)
                    {
                         var require_file_repeate_health = '<%= Resources.Health.rqfilerepeatehealthcheck %>';
                         alert(require_file_repeate_health);
                    }


                    if (file_result_health.length == 0)
                    {
                        var require_result_health = '<%= Resources.Health.rqfileresulthealthcheck %>';
                        alert(require_result_health);
                    }


                   
                    return false;
                }
                
               

            } else {


                if (file_result_health.length > 0)
                {
                    $.when(checkValidateOccupational()).done(function (value) {
                        if (value) {
                            sentAddOccupationalHealth();
                        } else {
                            return false;
                        }
                    });
                } else {
                    var require_result_health = '<%= Resources.Health.rqfileresulthealthcheck %>';
                    alert(require_result_health);
                }
               
            }
           


        }
        else {
           
            if (repeat_health == undefined)
            {
                var require_repeate_health = '<%= Resources.Health.rqrepeatehealthcheck %>';
                $("#rqrepeathealthcheck").text(require_repeate_health);

            }
          

            return false;
        }


    }


     function checkValidateOccupational()
     {
         var value = true;
         var chronic_diseases_ear = $("input:radio[name='ctl00$MainContent$chronic_diseases_ear']:checked").val();
         var ddlOccupationalhealth = $("#MainContent_ddlOccupationalhealth").val();

         var smoked_cigarettes = $("input:radio[name='ctl00$MainContent$smoked_cigarettes']:checked").val();
       
         if (parseInt(ddlOccupationalhealth) == 3)//ผิดปกติจากหู
         {
             if(chronic_diseases_ear== undefined)
             {
                 value = false;            
                 var rqchronic_diseases_ear = '<%= Resources.Health.rqchronic_diseases_ear %>';
                 $("#rqchronic_diseases_ear").text(rqchronic_diseases_ear);              

             }
         }


         if (parseInt(ddlOccupationalhealth) == 1)//ปอดผิดปกติ
         {
             if (smoked_cigarettes == undefined) {
                 value = false;
                 var rqsmoked_cigarettes = '<%= Resources.Health.rqsmoked_cigarettes %>';
                 $("#rqsmoked_cigarettes").text(rqsmoked_cigarettes);

             }
         }

         return value;
     }



     function sentAddOccupationalHealth()
     {

         showLoading();
         var occupational_health = $("#MainContent_ddlOccupationalhealth").val();
         var repeat_health = $("input:radio[name='ctl00$MainContent$repeathealthcheck']:checked").val();
         var abnormalaudiogram = $("#MainContent_ddabnormalaudiogram").val();
         var hearing_threshold_level = $("#MainContent_txthearing_threshold_level").val();
         var chronic_diseases_ear = $("input:radio[name='ctl00$MainContent$chronic_diseases_ear']:checked").val();
         var specify_chronic_diseases_ear = $("#MainContent_txtspecify_chronic_diseases_ear").val();

         var abnormal_pulmonary_function = $("#MainContent_ddabnormal_pulmonary_function").val();
         var smoked_cigarettes = $("input:radio[name='ctl00$MainContent$smoked_cigarettes']:checked").val();
         var cigarettes_per_day = $("#MainContent_txtcigarettes_per_day").val();
         var years = $("#MainContent_txtyears").val();
         var months = $("#MainContent_txtmonths").val();
         var smoked_other = $("#MainContent_txtsmoked_other").val();

         if (abnormalaudiogram == undefined) abnormalaudiogram = "";
         if (hearing_threshold_level == undefined) hearing_threshold_level = "";
         if (chronic_diseases_ear == undefined) chronic_diseases_ear = "";
         if (specify_chronic_diseases_ear == undefined) specify_chronic_diseases_ear = "";

         if (abnormal_pulmonary_function == undefined) abnormal_pulmonary_function = "";
         if (smoked_cigarettes == undefined) smoked_cigarettes = "";
         if (cigarettes_per_day == undefined) cigarettes_per_day = "";
         if (years == undefined) years = "";
         if (months == undefined) months = "";
         if (smoked_other == undefined) smoked_other = "";

         for (i in file_result_health)
         {
             if (filename_result_health != "")
             {
                 filename_result_health = filename_result_health + "," + file_result_health[i];

             } else {
                 filename_result_health = file_result_health[i];
             }
         }


         for (j in file_repeat_result_health)
         {
             if (filename_repeat_result_health != "")
             {
                 filename_repeat_result_health = filename_repeat_result_health + "," + file_repeat_result_health[j];

             } else {
                 filename_repeat_result_health = file_repeat_result_health[j];
             }
         }

         $.ajax({
             type: "POST",
             data: {
                 occupational_health: occupational_health,
                 repeat_health_check:repeat_health,
                 filename_result_health: filename_result_health,
                 filename_repeat_result_health: filename_repeat_result_health,
                 abnormal_audiogram: abnormalaudiogram,
                 hearing_threshold_level: hearing_threshold_level,
                 chronic_diseases_ear: chronic_diseases_ear,
                 specify_chronic_diseases_ear: specify_chronic_diseases_ear,
                 abnormal_pulmonary_function: abnormal_pulmonary_function,
                 smoked_cigarettes: smoked_cigarettes,
                 cigarettes_per_day: cigarettes_per_day,
                 years: years,
                 months: months,
                 smoked_other: smoked_other,
                 health_id: id,
                 page_type: pagetype

             },
             url: 'Actionevent.asmx/createOccupationalHealth',
             dataType: 'json',
             success: function (result) {
                 closeLoading();
                 closeOccupationalHealth();
                 clearOccupationalHealth();
                 callOccupationalHealth();
                 dropzone_occupational_health.removeAllFiles();
                 dropzone_repeat_occupational_health.removeAllFiles();
             }
         });
     }




     function sentUpdateOccupationalHealth()
     {

         showLoading();
         var occupational_health = $("#MainContent_ddlOccupationalhealth").val();
         var repeat_health = $("input:radio[name='ctl00$MainContent$repeathealthcheck']:checked").val();
         var abnormalaudiogram = $("#MainContent_ddabnormalaudiogram").val();
         var hearing_threshold_level = $("#MainContent_txthearing_threshold_level").val();
         var chronic_diseases_ear = $("input:radio[name='ctl00$MainContent$chronic_diseases_ear']:checked").val();
         var specify_chronic_diseases_ear = $("#MainContent_txtspecify_chronic_diseases_ear").val();

         var abnormal_pulmonary_function = $("#MainContent_ddabnormal_pulmonary_function").val();
         var smoked_cigarettes = $("input:radio[name='ctl00$MainContent$smoked_cigarettes']:checked").val();
         var cigarettes_per_day = $("#MainContent_txtcigarettes_per_day").val();
         var years = $("#MainContent_txtyears").val();
         var months = $("#MainContent_txtmonths").val();
         var smoked_other = $("#MainContent_txtsmoked_other").val();

         if (abnormalaudiogram == undefined) abnormalaudiogram = "";
         if (hearing_threshold_level == undefined) hearing_threshold_level = "";
         if (chronic_diseases_ear == undefined) chronic_diseases_ear = "";
         if (specify_chronic_diseases_ear == undefined) specify_chronic_diseases_ear = "";

         if (abnormal_pulmonary_function == undefined) abnormal_pulmonary_function = "";
         if (smoked_cigarettes == undefined) smoked_cigarettes = "";
         if (cigarettes_per_day == undefined) cigarettes_per_day = "";
         if (years == undefined) years = "";
         if (months == undefined) months = "";
         if (smoked_other == undefined) smoked_other = "";

         for (i in file_result_health)
         {
             if (filename_result_health != "") {
                 filename_result_health = filename_result_health + "," + file_result_health[i];

             } else {
                 filename_result_health = file_result_health[i];
             }
         }


         for (j in file_repeat_result_health)
         {
             if (filename_repeat_result_health != "")
             {
                 filename_repeat_result_health = filename_repeat_result_health + "," + file_repeat_result_health[j];

             } else {
                 filename_repeat_result_health = file_repeat_result_health[j];
             }
         }

         $.ajax({
             type: "POST",
             data: {
                 occupational_health: occupational_health,
                 repeat_health_check: repeat_health,
                 filename_result_health: filename_result_health,
                 filename_repeat_result_health: filename_repeat_result_health,
                 abnormal_audiogram: abnormalaudiogram,
                 hearing_threshold_level: hearing_threshold_level,
                 chronic_diseases_ear: chronic_diseases_ear,
                 specify_chronic_diseases_ear: specify_chronic_diseases_ear,
                 abnormal_pulmonary_function: abnormal_pulmonary_function,
                 smoked_cigarettes: smoked_cigarettes,
                 cigarettes_per_day: cigarettes_per_day,
                 years: years,
                 months: months,
                 smoked_other: smoked_other,
                 health_id: id,
                 id: occupational_health_id,
                 page_type: pagetype

             },
             url: 'Actionevent.asmx/updateOccupationalHealth',
             dataType: 'json',
             success: function (result) {
                 closeLoading();
                 closeOccupationalHealth();
                 clearOccupationalHealth();
                 callOccupationalHealth();
                 dropzone_occupational_health.removeAllFiles();
                 dropzone_repeat_occupational_health.removeAllFiles();

             }
         });
     }




















    function updateRiskFactor()
    {

        var monitoringenvironment = $("input:radio[name='ctl00$MainContent$monitoringenvironment']:checked").val();

        if (Page_ClientValidate("riskfactor") && monitoringenvironment != undefined)
        {
            
            var risk_factor_relate_work = $("#MainContent_ddlRiskfactorrelatework").val();
            var other_risk_factor = $("#MainContent_txtotherriskfactor").val();
            var year_factor = $("#MainContent_ddlYearriskfactor").val();

            if (year_factor == undefined) {
                year_factor = "";
            }
            
            var result_risk_factor = $("#MainContent_txtresultriskfactor").val();
            var duration_risk_factor = $("#MainContent_ddlDurationriskfactor").val();
            var monitoring_results = $("#MainContent_ddmonitoringresults").val();


            if ((monitoringenvironment == "Y" && file_risk_factor.length > 0) || monitoringenvironment == "N")
            {
                showLoading();

                for (i in file_risk_factor) {
                    if (filename_risk_factor != "") {
                        filename_risk_factor = filename_risk_factor + "," + file_risk_factor[i];

                    } else {
                        filename_risk_factor = file_risk_factor[i];
                    }
                }


                for (i in file_risk_factor) {
                    if (filename_risk_factor != "") {
                        filename_risk_factor = filename_risk_factor + "," + file_risk_factor[i];

                    } else {
                        filename_risk_factor = file_risk_factor[i];
                    }
                }

                $.ajax({
                    type: "POST",
                    data: {
                        risk_factor_relate_work: risk_factor_relate_work,
                        other_risk_factor: other_risk_factor,
                        year_factor: year_factor,
                        duration_risk_factor: duration_risk_factor,
                        file_risk_factor: filename_risk_factor,
                        result_risk_factor: result_risk_factor,
                        user_id: user_login_id,
                        health_id: id,
                        page_type: pagetype,
                        monitoring_environment: monitoringenvironment,
                        monitoring_results: monitoring_results,
                        id: risk_factor_relate_work_id,

                    },
                    url: 'Actionevent.asmx/updateRiskFactor',
                    dataType: 'json',
                    success: function (result) {
                        closeLoading();
                        closeRiskFactor();
                        clearRiskFactor();
                        callRiskFactorRelate();
                        dropzone_risk_factor.removeAllFiles();
                    }
                });

            } else {

                var require_file_risk_factor = '<%= Resources.Health.rqfileriskfactor %>';
                alert(require_file_risk_factor);


                return false;

            }


        }
        else {

            if (monitoringenvironment == undefined) {
                var require_monitoringenvironment = '<%= Resources.Health.rqmonitoringenvironment %>';
                $("#rqmonitoringenvironment").text(require_monitoringenvironment);
            }
            return false;
        }


    }





    function updateOccupationalHealth()
    {

        var repeat_health = $("input:radio[name='ctl00$MainContent$repeathealthcheck']:checked").val();


        if (Page_ClientValidate("occupationalhealth") && repeat_health != undefined) {
            if ((repeat_health == "Y")) {

                if (file_repeat_result_health.length > 0 && file_result_health.length > 0) {

                    $.when(checkValidateOccupational()).done(function (value) {
                        if (value) {
                            sentUpdateOccupationalHealth();
                        } else {
                            return false;
                        }
                    });

                } else {

                    if (file_repeat_result_health.length == 0) {
                        var require_file_repeate_health = '<%= Resources.Health.rqfilerepeatehealthcheck %>';
                        alert(require_file_repeate_health);
                    }


                    if (file_result_health.length == 0) {
                        var require_result_health = '<%= Resources.Health.rqfileresulthealthcheck %>';
                        alert(require_result_health);
                    }



                    return false;
                }



            } else {

                if (file_result_health.length > 0) {
                    $.when(checkValidateOccupational()).done(function (value) {
                        if (value) {
                            sentUpdateOccupationalHealth();
                        } else {
                            return false;
                        }
                    });
                } else {

                        var require_result_health = '<%= Resources.Health.rqfileresulthealthcheck %>';
                        alert(require_result_health);
                    
                }

            }



        }
        else {

            if (repeat_health == undefined) {
                var require_repeate_health = '<%= Resources.Health.rqrepeatehealthcheck %>';
                $("#rqrepeathealthcheck").text(require_repeate_health);

            }


            return false;
        }


    }









    


    function ShowEditRiskFactor(risk_factor_id)
    {
        $("#MainContent_btCreateriskfactor").hide();
        $("#MainContent_btUpdateriskfactor").show();


        risk_factor_relate_work_id = risk_factor_id;
        dialogRiskFactor.dialog("open");
        $.ajax({
            type: "POST",
            data: { id: risk_factor_relate_work_id,page_type:pagetype, lang: lang },
            url: 'Actionevent.asmx/getRiskFactorByID',
            dataType: 'json',
            success: function (json) {

                $.each(json, function (value, key) {
    
                    $("#MainContent_ddlRiskfactorrelatework").val(key.risk_factor_relate_work_id);
                    $("#MainContent_txtotherriskfactor").val(key.other);
                    $("#MainContent_ddlYearriskfactor").val(key.year);
                    $("#MainContent_txtresultriskfactor").val(key.result);
                    $("#MainContent_ddlDurationriskfactor").val(key.duration_risk_factor_id);
                    dropzone_risk_factor.removeAllFiles();
                    setOther(key.risk_factor_relate_work_id);
                    if (key.file_risk_factor != "")
                    {
                        file_risk_factor = key.file_risk_factor.split(",");
                        setShowEidtImage(key.file_risk_factor, dropzone_risk_factor);
                       // console.log(file_risk_factor);
                       // filename_risk_factor = key.file_risk_factor;
                    }

                    if (key.monitoring_environment != null) {
                        $("input[name='ctl00$MainContent$monitoringenvironment'][value='" + key.monitoring_environment + "']").prop('checked', true);

                        if (key.monitoring_environment == "Y") {
                            $("#year_risk_factor").show();
                            $("#year_risk_factor2").show();
                            $("#year_risk_factor3").show();
                            $("#year_risk_factor4").show();

                        } else if (key.monitoring_environment == "N") {
                            $("#year_risk_factor").hide();
                            $("#year_risk_factor2").hide();
                            $("#year_risk_factor3").hide();
                            $("#year_risk_factor4").hide();
                        }
                    }

                    $("#MainContent_ddmonitoringresults").val(key.monitoring_results);


                });



            }
        });



    }



    function ShowEditOccupationalHealth(occupational_health_report_id)
    {
        $("#MainContent_btCreatoccupationhealth").hide();
        $("#MainContent_btUpdateoccupationhealth").show();


        occupational_health_id = occupational_health_report_id;
        dialogOccupationalHealth.dialog("open");
        $.ajax({
            type: "POST",
            data: { id: occupational_health_id, page_type: pagetype, lang: lang },
            url: 'Actionevent.asmx/getOccupationalHealthByID',
            dataType: 'json',
            success: function (json) {

                $.each(json, function (value, key) {
                    $("#MainContent_ddlOccupationalhealth").val(key.occupational_health_report_id);
          
                    if (key.occupational_health_report_id == 3) {
                        $("#abnormalaudiogram").show();
                        $("#abnormalaudiogram2").show();

                        $("#abnormal_pulmonary_function").hide();
                        $("#abnormal_pulmonary_function2").hide();

                    }else if (key.occupational_health_report_id == 2) {

                        $("#abnormal_pulmonary_function").show();
                        $("#abnormal_pulmonary_function2").show();

                        $("#abnormalaudiogram").hide();
                        $("#abnormalaudiogram2").hide();

                    } else {

                        $("#abnormalaudiogram").hide();
                        $("#abnormalaudiogram2").hide();

                        $("#abnormal_pulmonary_function").hide();
                        $("#abnormal_pulmonary_function2").hide();
                    }
                    

                    if (key.repeat_health_check != null)
                    {
                        if (key.repeat_health_check == 'N') {
                            $("#show_upload_repeat_health_check").hide();
                        }
                        else if (key.repeat_health_check == 'Y') {
                            $("#show_upload_repeat_health_check").show();

                        }
                        $("input[name='ctl00$MainContent$repeathealthcheck'][value='" + key.repeat_health_check + "']").prop('checked', true);

                    }

                    if (key.file_health_check!="")
                    {
                        file_result_health = key.file_health_check.split(",");
                        setShowEidtImage(key.file_health_check, dropzone_occupational_health);
                       // filename_result_health = key.file_health_check;
                        
                    }
                    
                    if (key.flie_repeat_health_check!="")
                    {
                        file_repeat_result_health = key.flie_repeat_health_check.split(",");
                        setShowEidtImage(key.flie_repeat_health_check, dropzone_repeat_occupational_health);
                       // filename_repeat_result_health = key.flie_repeat_health_check;
                    }


                    if (key.chronic_diseases_ear != null) {
                        if (key.chronic_diseases_ear == 'N') {
                            $("#specify_abnormalaudiogram").hide();
                        }
                        else if (key.chronic_diseases_ear == 'Y') {
                            $("#specify_abnormalaudiogram").show();

                        }
                        $("input[name='ctl00$MainContent$chronic_diseases_ear'][value='" + key.chronic_diseases_ear + "']").prop('checked', true);

                    }

                    $("#MainContent_ddabnormalaudiogram").val(key.abnormal_audiogram);
                    $("#MainContent_txthearing_threshold_level").val(key.hearing_threshold_level);
                    $("#MainContent_txtspecify_chronic_diseases_ear").val(key.specify_chronic_diseases_ear);


                    if (key.smoked_cigarettes != null) {
                       
                        $("input[name='ctl00$MainContent$smoked_cigarettes'][value='" + key.smoked_cigarettes + "']").prop('checked', true);

                       
                        if (key.smoked_cigarettes == 'NO') {
                            $("#MainContent_txtcigarettes_per_day").prop('disabled', true);
                            $("#MainContent_txtyears").prop('disabled', true);
                            $("#MainContent_txtmonths").prop('disabled', true);
                            $("#MainContent_txtsmoked_other").prop('disabled', true);
                        }
                        else if (key.smoked_cigarettes == 'YES_SMOKING') {

                            $("#MainContent_txtcigarettes_per_day").prop('disabled', false);
                            $("#MainContent_txtyears").prop('disabled', true);
                            $("#MainContent_txtmonths").prop('disabled', true);
                            $("#MainContent_txtsmoked_other").prop('disabled', true);

                        } else if (key.smoked_cigarettes == 'YES_SMOKED') {

                            $("#MainContent_txtcigarettes_per_day").prop('disabled', true);
                            $("#MainContent_txtyears").prop('disabled', false);
                            $("#MainContent_txtmonths").prop('disabled', false);
                            $("#MainContent_txtsmoked_other").prop('disabled', true);

                        } else if (key.smoked_cigarettes == 'SMOKED_OTHER') {

                            $("#MainContent_txtcigarettes_per_day").prop('disabled', true);
                            $("#MainContent_txtyears").prop('disabled', true);
                            $("#MainContent_txtmonths").prop('disabled', true);
                            $("#MainContent_txtsmoked_other").prop('disabled', false);
                        }
                     

                    }

                    $("#MainContent_ddabnormal_pulmonary_function").val(key.abnormal_pulmonary_function);
                    $("#MainContent_txtcigarettes_per_day").val(key.cigarette_per_day);
                    $("#MainContent_txtyears").val(key.smoked_years);
                    $("#MainContent_txtmonths").val(key.smoked_months);
                    $("#MainContent_txtsmoked_other").val(key.smoked_cigarettes_other);
                    
                });



            }
        });



    }

  
    function callRiskFactorRelate()
    {

       dataTableRiskFactor.ajax.url('Datatablelist.asmx/getListRiskFactorRelate?health_id=' + id +"&folder_name="+folder_image+ "&lang=" + lang + '&pagetype=' + pagetype).load();

     }


     function callOccupationalHealth()
     {

        dataTableOccupationalHealth.ajax.url('Datatablelist.asmx/getListOccupationalHealth?health_id=' + id + "&folder_name=" + folder_image + "&lang=" + lang + '&pagetype=' + pagetype).load();

     }






     function showCreateRiskFactor()
     {
         $("#MainContent_btCreateriskfactor").show();
         $("#MainContent_btUpdateriskfactor").hide();
         $("#risk_factor_other").hide();
         dialogRiskFactor.dialog("open");

         return false;

     }


     function showCreateOccupationalHealth()
     {
         $("#MainContent_btCreatoccupationhealth").show();
         $("#MainContent_btUpdateoccupationhealth").hide();
         $("#show_upload_repeat_health_check").hide();
         $("#abnormalaudiogram").hide();
         $("#abnormalaudiogram2").hide();
         $("#specify_abnormalaudiogram").hide();

         $("#abnormal_pulmonary_function").hide();
         $("#abnormal_pulmonary_function2").hide();

       
         dialogOccupationalHealth.dialog("open");

         return false;

     }




     function closeRiskFactor()
     {
         dialogRiskFactor.dialog("close");
         clearValidationErrors();
         clearRiskFactor();
     }


     function closeOccupationalHealth()
     {
         dialogOccupationalHealth.dialog("close");
         clearValidationErrors();
         clearOccupationalHealth();
     }


     function clearValidationErrors(group)
     {
         var i;
         for (i = 0; i < Page_Validators.length; i++) 
         {            
             Page_Validators[i].style.display = "none";
 
         }


         $("#rqrepeathealthcheck").text("");
     }




   



     function clearRiskFactor()
     {
         $("#MainContent_ddlRiskfactorrelatework").val("");
         $("#MainContent_txtotherriskfactor").val("");
         $("#MainContent_ddlYearriskfactor").val("");
         $("#MainContent_ddlDurationriskfactor").val("");
         $("#MainContent_txtresultriskfactor").val("");
         $("#MainContent_ddmonitoringresults").val("");

         $("#rqmonitoringenvironment").text("");
         $("input[name='ctl00$MainContent$monitoringenvironment']").attr('checked', false);
         $("#year_risk_factor").show();
         $("#year_risk_factor2").show();
         $("#year_risk_factor3").show();
         $("#year_risk_factor4").show();

         risk_factor_relate_work_id = 0;
         filename_risk_factor = "";
         file_risk_factor = [];
       
     }


     function clearOccupationalHealth()
     {
         $("#MainContent_ddlOccupationalhealth").val("");
         $('input[name="ctl00$MainContent$repeathealthcheck"]').attr('checked', false);

         $("#MainContent_ddabnormalaudiogram").val("");
         $("#MainContent_txthearing_threshold_level").val("");
         $('input[name="ctl00$MainContent$chronic_diseases_ear"]').attr('checked', false);
         $("#MainContent_txtspecify_chronic_diseases_ear").val("");

         $("#MainContent_ddabnormal_pulmonary_function").val("");
         $('input[name="ctl00$MainContent$smoked_cigarettes"]').attr('checked', false);
         $("#MainContent_txtcigarettes_per_day").val("");
         $("#MainContent_txtyears").val("");
         $("#MainContent_txtmonths").val("");
         $("#MainContent_txtsmoked_other").val("");

         $("#rqsmoked_cigarettess").text("");
         $("#rqchronic_diseases_ear").text("");

         occupational_health_id = 0;
         filename_result_health = "";
         filename_repeat_result_health = "";
         file_result_health = [];
         file_repeat_result_health = [];
         
     }







    function setDatatableRiskFactor()
    {

        dataTableRiskFactor = $("#tbRisk_factor_relate_work").DataTable({
            "bProcessing": true,
            "sProcessing": true,

            "bPaginate": false,
            "bInfo": false,
            "bFilter": false,
            "ordering": false,
            // "stateSave": true,
            "responsive": true,
            "scrollX": true,
            //"pageLength": 25,
            "lengthChange": false,
            "order": [],
            "language": {
                "url": 'Langdatatable.aspx',
                "decimal": ","
            },
            "columnDefs": [
               {
                   "targets": [0],
                   "visible": false,
               }
            ]
        });


        dataTableRiskFactor.ajax.url('Datatablelist.asmx/getListRiskFactorRelate?health_id=' + id + "&folder_name=" + folder_image + "&lang=" + lang + '&pagetype=' + pagetype);



    }



    function setDatatableOccupationalHealth()
    {

        dataTableOccupationalHealth = $("#tbOccupational_health").DataTable({
            "bProcessing": true,
            "sProcessing": true,

            "bPaginate": false,
            "bInfo": false,
            "bFilter": false,
            "ordering": false,
            "scrollX": true,
            // "stateSave": true,
            "responsive": true,
            //"pageLength": 25,
            "lengthChange": false,
            "order": [],
            "language": {
                "url": 'Langdatatable.aspx',
                "decimal": ","
            },
            "columnDefs": [
               {
                   "targets": [0],
                   "visible": false,
               }
            ]
        });


        dataTableOccupationalHealth.ajax.url('Datatablelist.asmx/getListOccupationalHealth?health_id=' + id + "&folder_name=" + folder_image + "&lang=" + lang + '&pagetype=' + pagetype);



    }


    function setDropzoneRiskFactor()
    {
        var reportdate = $("#MainContent_txtreport_date").val();
        //alert(reportdate);
        //File Upload response from the server
        dropzone_risk_factor = Dropzone.options.dropzoneRiskFactor = {
            maxFiles: 3,
            maxFilesize: 10,
            url: "dropzoneuploadhealth.aspx?user_id=" + user_login_id + "&reportdate=" + reportdate + "&id=" + id + "&type=risk_factor",
            acceptedFiles: ".pdf,image/jpeg,image/png",
            init: function () {
                dropzone_risk_factor = this;
                this.on("maxfilesexceeded", function (data) {
                    // var res = eval('(' + data.xhr.responseText + ')');

                });
                this.on("addedfile", function (file) {

                    // Create the remove button
                    var removeButton = Dropzone.createElement("<button>Remove file</button>");


                    // Capture the Dropzone instance as closure.
                    var _this = this;

                    // Listen to the click event
                    removeButton.addEventListener("click", function (e) {
                        // Make sure the button click doesn't submit the form:
                        $.ajax({
                            type: "POST",
                            url: "dropzoneremove.aspx",
                            data: "folder=" + file.folderimage + "&type=health&name=" + file.newname,
                            success: function (msg) {

                                var index_remove = file_risk_factor.indexOf(file.newname);
                                if (index_remove != -1)
                                {
                                    file_risk_factor.splice(index_remove, 1);

                                }
                            },
                            error: function (e) {

                                //alert(e);


                            }
                        });
                        e.preventDefault();
                        e.stopPropagation();
                       _this.removeFile(file);
                 // Remove the file preview.
                        // If you want to the delete the file on the server as well,
                        // you can do the AJAX request here.
                    });




                    document.querySelector("#MainContent_btCloseRiskFactor").addEventListener("click", function () {

                        _this.removeAllFiles();

                    });

                    document.querySelector("#MainContent_btShowCreateRiskFactor").addEventListener("click", function () {

                        _this.removeAllFiles();

                    });



                    //document.querySelector("#MainContent_btUpdateoccupationhealth").addEventListener("click", function () {

                    //    _this.removeAllFiles();

                    //});

                    // Add the button to the file preview element.
                    file.previewElement.appendChild(removeButton);

                    this.on("success", function (file, response) {
                        var obj = $.parseJSON(response);

                        if (file_risk_factor.indexOf(obj.name) == -1)
                        {
                            file_risk_factor.push(obj.name);

                        }

                       // filename_risk_factor = obj.name;
                        folder_image = obj.folder;
                        file.newname = obj.name;
                        file.folderimage = obj.folder;
                        //console.log(file.name);

                        setTimeout(function () {

                            var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/health/" + obj.folder + "/" + obj.name;

                            $("img[alt='" + file.name + "']").wrap("<a class='fancybox' href='" + image_name + "'/>");

                            $('.fancybox').fancybox();

                        }, 1500);


                    });
                });
            }
        };

    }



    function setDropzoneOccupationalHealth()
    {
       var reportdate = $("#MainContent_txtreport_date").val();
        //alert(reportdate);
        //File Upload response from the server
       dropzone_occupational_health = Dropzone.options.dropzoneOccupationalHealth = {
            maxFiles: 2,
            maxFilesize: 10,
            url: "dropzoneuploadhealth.aspx?user_id=" + user_login_id + "&reportdate=" + reportdate + "&id=" + id+"&type=occupational_health",
            acceptedFiles: ".pdf,image/jpeg,image/png",
            init: function () {
                dropzone_occupational_health = this;
                this.on("maxfilesexceeded", function (data) {
                   // var res = eval('(' + data.xhr.responseText + ')');

                });
                this.on("addedfile", function (file) {

                    // Create the remove button
                    var removeButton = Dropzone.createElement("<button>Remove file</button>");


                    // Capture the Dropzone instance as closure.
                    var _this = this;

                    // Listen to the click event
                    removeButton.addEventListener("click", function (e) {
                        // Make sure the button click doesn't submit the form:
                        console.log(file.newname);
                        $.ajax({
                            type: "POST",
                            url: "dropzoneremove.aspx",
                            data: "folder=" + file.folderimage + "&type=health&name=" + file.newname,
                            success: function (msg) {

                                var index_remove = file_result_health.indexOf(file.newname);
                                if (index_remove != -1)
                                {
                                    file_result_health.splice(index_remove, 1);

                                }
                            },
                            error: function (e) {

                                //alert(e);


                            }
                        });
                        e.preventDefault();
                        e.stopPropagation();
                        // Remove the file preview.
                        _this.removeFile(file);
                        // If you want to the delete the file on the server as well,
                        // you can do the AJAX request here.
                    });




                    document.querySelector("#MainContent_btCloseoccupationhealth").addEventListener("click", function () {

                        _this.removeAllFiles();

                    });

                    document.querySelector("#MainContent_btCreateOccupationalHealth").addEventListener("click", function () {

                        _this.removeAllFiles();

                    });


                    //document.querySelector("#MainContent_btCreateOccupationalHealth").addEventListener("click", function () {

                    //    _this.removeAllFiles();

                    //});

                    //document.querySelector("#MainContent_btUpdateoccupationhealth").addEventListener("click", function () {

                    //    _this.removeAllFiles();

                    //});

                    // Add the button to the file preview element.
                    file.previewElement.appendChild(removeButton);

                    this.on("success", function (file, response) {
                        var obj = $.parseJSON(response);

                        if (file_result_health.indexOf(obj.name) == -1)
                        {
                            file_result_health.push(obj.name);

                        }
                      
                        //if (filename_result_health != "")
                        //{
                        //    filename_result_health = filename_result_health + "," + obj.name;

                        //} else {
                        //    filename_result_health = obj.name;
                        //}

                        folder_image = obj.folder;
                        
                        file.newname = obj.name;
                        file.folderimage = obj.folder;
                        //console.log(file.name);

                        setTimeout(function () {

                            var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/health/" + obj.folder + "/" + obj.name;

                            $("img[alt='" + file.name + "']").wrap("<a class='fancybox' href='" + image_name + "'/>");

                            $('.fancybox').fancybox();

                        }, 1500);


                    });
                });
            }
        };

    }





     function setDropzoneRepeatOccupationalHealth()
     {
         var reportdate = $("#MainContent_txtreport_date").val();
         //alert(reportdate);
         //File Upload response from the server
         dropzone_repeat_occupational_health = Dropzone.options.dropzonerRepeatOccupationalHealth = {
             maxFiles: 2,
             maxFilesize: 10,
             url: "dropzoneuploadhealth.aspx?user_id=" + user_login_id + "&reportdate=" + reportdate + "&id=" + id + "&type=repeat_occupational_health",
             acceptedFiles: ".pdf,image/jpeg,image/png",
             init: function () {
                 dropzone_repeat_occupational_health = this;
                 this.on("maxfilesexceeded", function (data) {
                     // var res = eval('(' + data.xhr.responseText + ')');

                 });
                 this.on("addedfile", function (file) {

                     // Create the remove button
                     var removeButton = Dropzone.createElement("<button>Remove file</button>");


                     // Capture the Dropzone instance as closure.
                     var _this = this;

                     // Listen to the click event
                     removeButton.addEventListener("click", function (e) {
                         // Make sure the button click doesn't submit the form:
                         $.ajax({
                             type: "POST",
                             url: "dropzoneremove.aspx",
                             data: "folder=" + file.folderimage + "&type=health&name=" + file.newname,
                             success: function (msg) {

                                 var index_remove = file_repeat_result_health.indexOf(file.newname);
                                 if (index_remove != -1)
                                 {
                                     file_repeat_result_health.splice(index_remove, 1);

                                 }
                                
                             },
                             error: function (e) {

                                 //alert(e);


                             }
                         });
                         e.preventDefault();
                         e.stopPropagation();
                         // Remove the file preview.
                         _this.removeFile(file);
                         // If you want to the delete the file on the server as well,
                         // you can do the AJAX request here.
                     });



                     document.querySelector("#MainContent_btCloseoccupationhealth").addEventListener("click", function () {

                         _this.removeAllFiles();

                     });

                     document.querySelector("#MainContent_btCreateOccupationalHealth").addEventListener("click", function () {

                         _this.removeAllFiles();

                     });

                     //document.querySelector("#MainContent_btUpdateoccupationhealth").addEventListener("click", function () {

                     //    _this.removeAllFiles();

                     //});

                     // Add the button to the file preview element.
                     file.previewElement.appendChild(removeButton);

                     this.on("success", function (file, response) {
                         var obj = $.parseJSON(response);
                         
                         if (file_repeat_result_health.indexOf(obj.name) == -1)
                         {
                             file_repeat_result_health.push(obj.name);

                         }

                         //if (filename_repeat_result_health != "")
                         //{
                         //    filename_repeat_result_health = filename_repeat_result_health + "," + obj.name;
                         //} else {
                         //    filename_repeat_result_health = obj.name;
                         //}

                         folder_image = obj.folder;
                        
                         file.newname = obj.name;
                         file.folderimage = obj.folder;
                         //console.log(file.name);

                         setTimeout(function () {

                             var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/health/" + obj.folder + "/" + obj.name;

                             $("img[alt='" + file.name + "']").wrap("<a class='fancybox' href='" + image_name + "'/>");

                             $('.fancybox').fancybox();

                         }, 1500);


                     });
                 });
             }
         };

     }




     function CheckMonitoringEnvironment(oSrc, args)
     {

         var monitoringenvironment = $("input:radio[name='ctl00$MainContent$monitoringenvironment']:checked").val();

         if (monitoringenvironment == "Y") {
             if (args.Value.length == 0) {

                 args.IsValid = false;
             } else {

                 args.IsValid = true;
             }


         } else {
             args.IsValid = true;

         }


     }


     function CheckResultRiskfactor(oSrc, args)
     {

         var monitoringenvironment = $("input:radio[name='ctl00$MainContent$monitoringenvironment']:checked").val();

         if (monitoringenvironment == "Y") {
             if (args.Value.length == 0) {

                 args.IsValid = false;
             } else {

                 args.IsValid = true;
             }


         } else {
             args.IsValid = true;

         }


     }



     function CheckMonitoringResults(oSrc, args)
     {

         var monitoringenvironment = $("input:radio[name='ctl00$MainContent$monitoringenvironment']:checked").val();

         if (monitoringenvironment == "Y") {
             if (args.Value.length == 0) {

                 args.IsValid = false;
             } else {

                 args.IsValid = true;
             }


         } else {
             args.IsValid = true;

         }


     }


    


    function setShowEidtImage(file_name,mydropzone)
    {
       
        var reportdate = $("#MainContent_txtreport_date").val();
        $.ajax({
            type: "POST",
            data: { report_date: reportdate, file_name: file_name, user_id: user_login_id,id: id ,lang:lang},
            url: 'Actionevent.asmx/getImageHealth',
            dataType: 'json',
            success: function (json) {
                //console.log(json);
                //var existingFileCount = 0;
                $.each(json, function (value, key) {
                    //console.log(key.name);
                    // Create the mock file:
                    var mockFile = { name: key.name, size: key.size, accepted: true, status: "success", newname: key.name, folderimage: key.folder };

                    // Call the default addedfile event handler
                    mydropzone.emit("addedfile", mockFile);

                    // And optionally show the thumbnail of the file:
                    mydropzone.emit("thumbnail", mockFile, key.path);
                    // myDropzone.createThumbnailFromUrl(file, "upload/Health/15100028_20170607110927/IMG_20141205_154840.jpg", callback, crossOrigin);
                    mydropzone.emit("complete", mockFile);

                    // If you use the maxFiles option, make sure you adjust it to the
                    // correct amount:
                    // The number of files already uploaded
                    // existingFileCount = existingFileCount + 1;
                    mydropzone.files.push(mockFile);


                    var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/health/" + key.folder + "/" + key.name;

                    $("img[alt='" + key.name + "']").wrap("<a class='fancybox' href='" + image_name + "'/>");



                });

                // myDropzone.options.maxFiles = myDropzone.options.maxFiles - existingFileCount;



                $('.fancybox').fancybox();

            }
        });

    }


    function setShowedit()
    {

        // console.log(id);
        $.ajax({
            type: "POST",
            data: { id: id,user_id: user_login_id, lang: lang },
            url: 'Actionevent.asmx/getHealthbyid',
            dataType: 'json',
            success: function (json) {
               
                $.each(json, function (value, key) {
                    //setCompany(key.company_id);
                    setAreaSelect(key.company_id, key.function_id, key.department_id, key.division_id, key.section_id,
                               key.location_company_name_th, key.location_function_name_th, key.location_department_name_th, key.location_division_name_th, key.location_section_name_th,
                               key.location_company_name_en, key.location_function_name_en, key.location_department_name_en, key.location_division_name_en, key.location_section_name_en);
                    setYear(key.year_health);
                    getEmployee(key.health_employee_id);
                    //$("#MainContent_ddemployee").val(key.health_employee_id).trigger('change');
                  
                    // $("#MainContent_ddyear").val(key.year_health);
                    //console.log(key.hospital_id);
                    setHospital(key.hospital_id);
                    $("#MainContent_txtreport_date").val(key.health_report);
                    $("#MainContent_txtemployee_name").val(key.first_name + " " + key.last_name);
              
                    //$("#MainContent_txtdateofbirth").val(key.birth_date);
                    //$("#MainContent_txthiring_date").val(key.hiring_date);
                    $("#MainContent_txtdateofbirth").datepicker({
                        format: "dd/mm/yyyy"
                    }).datepicker("setDate", key.birth_date);
                    $("#MainContent_txthiring_date").datepicker({
                        format: "dd/mm/yyyy"
                    }).datepicker("setDate", key.hiring_date);

                    $("#MainContent_txtage").val(key.age);
                    $("#MainContent_txtservice_year").val(key.service_year);
                    $("#MainContent_txtservice_year_current").val(key.service_year_current);
                    $("#MainContent_txtjob_type").val(key.job_type_machine_type);

                    if (key.significant_insignificant != null)
                    {
                        $("input[name='ctl00$MainContent$significantorinsignificant'][value='" + key.significant_insignificant + "']").prop('checked', true);
                    }
                    $("#MainContent_lbEmployee").text(key.name_modify);
                    $("#MainContent_lbUpdate").text(key.datetime_modify);

                 

                    $("#show_doc_status").html(key.doc_no + ' ' + key.status);

                
                   
                  
                });

             


            }
        });

    }



    function setAreaSelect(company_id, function_id, department_id, division_id, section_id,
                           location_company_name_th, location_function_name_th, location_department_name_th, location_division_name_th, location_section_name_th,
                           location_company_name_en, location_function_name_en, location_department_name_en, location_division_name_en,location_section_name_en)
    {
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getCompanyForm',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddcompany");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


             

                    if (pagetype == "view")
                    {
                       // alert(location_company_name_en);
                        if (lang == "th")
                        {
                            $('#MainContent_ddcompany').append($('<option></option>').val("old").html(location_company_name_th));
                        } else {
                            $('#MainContent_ddcompany').append($('<option></option>').val("old").html(location_company_name_en));
                        }

                        $('#MainContent_ddcompany').val("old");
                        setFunction(company_id, function_id, department_id, division_id, section_id,
                                   location_function_name_th, location_department_name_th, location_division_name_th, location_section_name_th,
                                   location_function_name_en, location_department_name_en, location_division_name_en, location_section_name_en);



                    } else {

                        $('#MainContent_ddcompany').val(company_id);
                        setFunction(company_id, function_id, department_id, division_id, section_id,
                                    location_function_name_th, location_department_name_th, location_division_name_th, location_section_name_th,
                                    location_function_name_en, location_department_name_en, location_division_name_en, location_section_name_en);



                    }


                
            }
        });


       

    }





     function setFunction(company_id, function_id, department_id, division_id, section_id,
                          location_function_name_th, location_department_name_th, location_division_name_th, location_section_name_th,
                          location_function_name_en, location_department_name_en, location_division_name_en, location_section_name_en)
    {
        $.ajax({
            type: "POST",
            data: { company: company_id, lang: lang },
            url: 'Masterdata.asmx/getFuctionFormByCompany',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddfunction");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                if (pagetype == "view")
                {
                    if (lang == "th") {
                        $('#MainContent_ddfunction').append($('<option></option>').val("old").html(location_function_name_th));
                    } else {
                        $('#MainContent_ddfunction').append($('<option></option>').val("old").html(location_function_name_en));
                    }

                    $('#MainContent_ddfunction').val("old");

                } else {
                    $('#MainContent_ddfunction').val(function_id);
                }

               
                setDepartment(function_id, department_id, division_id, section_id,
                              location_department_name_th, location_division_name_th, location_section_name_th,
                              location_department_name_en, location_division_name_en, location_section_name_en);
            }
        });



    }



     function setDepartment(function_id, department_id, division_id, section_id,
                            location_department_name_th, location_division_name_th, location_section_name_th,
                            location_department_name_en, location_division_name_en, location_section_name_en)
    {
      
        $.ajax({
            type: "POST",
            data: { function: function_id, lang: lang },
            url: 'Masterdata.asmx/getDepartmentFormbyFunction',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_dddepartment");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


                if (pagetype == "view")
                {

                    if (lang == "th")
                    {
                        $('#MainContent_dddepartment').append($('<option></option>').val("old").html(location_department_name_th));
                    } else {
                        $('#MainContent_dddepartment').append($('<option></option>').val("old").html(location_department_name_en));
                    }

                    $('#MainContent_dddepartment').val("old");

                    setDivision(department_id, division_id, section_id,
                                location_division_name_th, location_section_name_th,
                                location_division_name_en, location_section_name_en);

                } else {

                    if (department_id == "" || department_id == "00000000") {
                        $('#MainContent_dddepartment').val(function_id + "F");
                        setDivision(function_id + "F", division_id, section_id, "", "", "", "");

                    } else {
                        $('#MainContent_dddepartment').val(department_id);
                        setDivision(department_id, division_id, section_id,
                                     location_division_name_th, location_section_name_th,
                                     location_division_name_en, location_section_name_en);
                    }



                }
                
            }
        });



    }



    function setDivision(department_id, division_id, section_id,
                        location_division_name_th, location_section_name_th,
                        location_division_name_en, location_section_name_en)
    {
        //alert(division_id);
        $.ajax({
            type: "POST",
            data: { department: department_id, lang: lang },
            url: 'Masterdata.asmx/getDivisionFormbyDepartment',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_dddivision");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                if (pagetype == "view") {

                    if (lang == "th") {
                        $('#MainContent_dddivision').append($('<option></option>').val("old").html(location_division_name_th));
                    } else {
                        $('#MainContent_dddivision').append($('<option></option>').val("old").html(location_division_name_en));
                    }

                    $('#MainContent_dddivision').val("old");
                    setSection(division_id, section_id, location_section_name_th, location_section_name_en);

                } else {

                    if (division_id == "" || division_id == "00000000") {
                       // if (pagetype != "view" && pagetype != "edit") {
                            $('#MainContent_dddivision').val(department_id + "D");
                            setSection(department_id + "D", section_id, "", "");
                       // }

                    } else {

                        $('#MainContent_dddivision').val(division_id);
                        setSection(division_id, section_id, location_section_name_th, location_section_name_en);
                    }

                }


                
            }
        });



    }



    function setSection(division_id, section_id, location_section_name_th, location_section_name_en)
    {
      
        $.ajax({
            type: "POST",
            data: { division: division_id, lang: lang },
            url: 'Masterdata.asmx/getSectionFormbyDivision',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddsection");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


                if (pagetype == "view")
                {
                    if (lang == "th")
                    {
                        $('#MainContent_ddsection').append($('<option></option>').val("old").html(location_section_name_th));
                    } else {
                       
                        $('#MainContent_ddsection').append($('<option></option>').val("old").html(location_section_name_en));
                    }

                    $('#MainContent_ddsection').val("old");

                } else {

                    if (section_id == "" || section_id == "00000000")
                    {
                       // if (pagetype != "view" && pagetype != "edit")
                       // {
                            $('#MainContent_ddsection').val(division_id + "D");
                       // }
                    } else {
                        $('#MainContent_ddsection').val(section_id);
                    }



                }

                
            }
        });



    }












    function changCompany()
    {
        var ddl_company_id = $('#MainContent_ddcompany').val();

        $.ajax({
            type: "POST",
            data: { company: ddl_company_id, lang: lang },
            url: 'Masterdata.asmx/getFuctionFormByCompany',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddfunction");
                $el.empty(); // remove old options
               
                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


            }
        });



    }







    function changFunction()
    {
        var ddl_function_id = $('#MainContent_ddfunction').val();

        $.ajax({
            type: "POST",
            data: { function: ddl_function_id, lang: lang },
            url: 'Masterdata.asmx/getDepartmentFormbyFunction',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_dddepartment");
                $el.empty(); // remove old options
               
                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


            }
        });



    }


    function changDepartment()
    {
        var ddl_department_id = $('#MainContent_dddepartment').val();

        $.ajax({
            type: "POST",
            data: { department: ddl_department_id, lang: lang },
            url: 'Masterdata.asmx/getDivisionFormbyDepartment',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_dddivision");
                $el.empty(); // remove old options
                
                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                $('#MainContent_dddivision').val(ddl_department_id + 'D');
                changDivision();
            }
        });



    }



    function changDivision()
    {
        var ddl_division_id = $('#MainContent_dddivision').val();

        $.ajax({
            type: "POST",
            data: { division: ddl_division_id, lang: lang },
            url: 'Masterdata.asmx/getSectionFormbyDivision',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddsection");
                $el.empty(); // remove old options
               
                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                $("#MainContent_ddsection").val(ddl_division_id + "D");
            }
        });



    }



    function saveHealth()
    {
        if (Page_ClientValidate("health"))
        {
            var tr_risk = dataTableRiskFactor.data().length;
            var tr_occu = dataTableOccupationalHealth.data().length;

            if (tr_risk > 0 && tr_occu > 0)
            {       
                
                    var significantorinsignificant = $("input:radio[name='ctl00$MainContent$significantorinsignificant']:checked").val();

                    if (significantorinsignificant != undefined)
                    {
                        showLoading();
                        var hospital = $("#MainContent_ddhospital").val();
                        var employee_id = $("#MainContent_ddemployee").val();
                        var year = $("#MainContent_ddyear").val();
                        var report_date = $("#MainContent_txtreport_date").val();
                        var company_id = $("#MainContent_ddcompany").val();
                        var function_id = $("#MainContent_ddfunction").val();
                        var department_id = $("#MainContent_dddepartment").val();
                        var division_id = $("#MainContent_dddivision").val();
                        var section_id = $("#MainContent_ddsection").val();

                        var age = $("#MainContent_txtage").val();
                        var service_year = $("#MainContent_txtservice_year").val();
                        var service_year_current = $("#MainContent_txtservice_year_current").val();
                        var job_type = $("#MainContent_txtjob_type").val();

                        var date_birth = $("#MainContent_txtdateofbirth").val();
                        var date_hiring = $("#MainContent_txthiring_date").val();


                        $.ajax({
                            type: "POST",
                            data: {
                                health_employee_id: employee_id,
                                year_health: year,
                                report_date: report_date,
                                company_id: company_id,
                                function_id: function_id,
                                department_id: department_id,
                                division_id: division_id,
                                section_id: section_id,
                                birth_date: date_birth,
                                hiring_date: date_hiring,
                                age: age,
                                service_year: service_year,
                                service_year_current: service_year_current,
                                job_type_machine_type: job_type,
                                significant_insignificant :significantorinsignificant,
                                userid: user_login_id,
                                typelogin: type_login,
                                hospital : hospital,

                            },
                            url: 'Actionevent.asmx/createHealth',
                            dataType: 'json',
                            success: function (id) {

                                closeLoading();
                                var result_save = '<%= Resources.Main.success %>';
                                alert(result_save);

                                window.location.href = "healthform.aspx?pagetype=view&id=" + id;



                            }
                        });


                    }else{

                        if (significantorinsignificant == undefined)
                        {
                            var require_significantorinsignificant = '<%= Resources.Health.rqsignificantorinsignificant %>';
                            $("#rqsignificantorinsignificant").text(require_significantorinsignificant);

                        }


                  }

             
                           
                }else {       
            
                    var result_table = '<%= Resources.Main.lbrqtalberiskandoccu %>';
                    alert(result_table);

                            
                }     
            

            return false;

        }
        else {
            return false;
        }

      
    }

    function updateHealth()
    {
        if (Page_ClientValidate("health")) {

                 var tr_risk = dataTableRiskFactor.data().length;
                 var tr_occu = dataTableOccupationalHealth.data().length;

                if (tr_risk > 0 && tr_occu > 0)
                {

                    var significantorinsignificant = $("input:radio[name='ctl00$MainContent$significantorinsignificant']:checked").val();

                    if (significantorinsignificant != undefined)
                    {

                        showLoading();
                        var hospital = $("#MainContent_ddhospital").val();
                        var employee_id = $("#MainContent_ddemployee").val();
                        var year = $("#MainContent_ddyear").val();
                        var report_date = $("#MainContent_txtreport_date").val();
                        var company_id = $("#MainContent_ddcompany").val();
                        var function_id = $("#MainContent_ddfunction").val();
                        var department_id = $("#MainContent_dddepartment").val();
                        var division_id = $("#MainContent_dddivision").val();
                        var section_id = $("#MainContent_ddsection").val();

                        var age = $("#MainContent_txtage").val();
                        var service_year = $("#MainContent_txtservice_year").val();
                        var service_year_current = $("#MainContent_txtservice_year_current").val();
                        var job_type = $("#MainContent_txtjob_type").val();

                        var date_birth = $("#MainContent_txtdateofbirth").val();
                        var date_hiring = $("#MainContent_txthiring_date").val();

                        $.ajax({
                            type: "POST",
                            data: {
                                health_employee_id: employee_id,
                                year_health: year,
                                report_date: report_date,
                                company_id: company_id,
                                function_id: function_id,
                                department_id: department_id,
                                division_id: division_id,
                                section_id: section_id,
                                birth_date: date_birth,
                                hiring_date:date_hiring,
                                age: age,
                                service_year: service_year,
                                service_year_current: service_year_current,
                                job_type_machine_type: job_type,
                                significant_insignificant: significantorinsignificant,
                                userid: user_login_id,
                                typelogin: type_login,
                                health_id: id,
                                hospital: hospital,

                            },
                            url: 'Actionevent.asmx/updateHealth',
                            dataType: 'json',
                            success: function (id) {

                                closeLoading();
                                var result_save = '<%= Resources.Main.success %>';
                                alert(result_save);

                                window.location.href = "healthform.aspx?pagetype=view&id=" + id;



                            }
                        });

                    }else {

                        if (significantorinsignificant == undefined)
                        {
                            var require_significantorinsignificant = '<%= Resources.Health.rqsignificantorinsignificant %>';
                            $("#rqsignificantorinsignificant").text(require_significantorinsignificant);

                        }


                    }
           
                            
                }else {       
            
                    var result_table = '<%= Resources.Main.lbrqtalberiskandoccu %>';
                    alert(result_table);

                           
                }     
             
            return false;

        }
        else {
            return false;
        }


    }



    function setYear(year)
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getHealthYear',
            dataType: 'json',
            success: function (json) {

                // var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_ddyear");
                $el.empty(); // remove old options
                $el.append($("<option></option>")
                         .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.year));
                });

                $("#MainContent_ddyear").val(year);

            }
        });



    }


     function setRiskFactorYear()
     {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getRiskfactoryear',
             dataType: 'json',
             success: function (json) {

                 // var all = '<%= Resources.Main.all %>';
                 var $el = $("#MainContent_ddlRiskfactorrelatework");
                 $el.empty(); // remove old options
                 $el.append($("<option></option>")
                          .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.year));
                 });

             }
         });



     }




     function setRiskFactorYear()
     {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getRiskfactoryear',
             dataType: 'json',
             success: function (json) {

                 // var all = '<%= Resources.Main.all %>';
                 var $el = $("#MainContent_ddlYearriskfactor");
                 $el.empty(); // remove old options
                 $el.append($("<option></option>")
                          .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.year));
                 });

             }
         });



     }



     function setRiskFactorRelateWork()
     {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getRiskFactorRelateWork',
             dataType: 'json',
             success: function (json) {

                 // var all = '<%= Resources.Main.all %>';
                 var $el = $("#MainContent_ddlRiskfactorrelatework");
                 $el.empty(); // remove old options
                 $el.append($("<option></option>")
                          .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });

             }
         });



     }



     function setOccupationalHealth()
     {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getOccupationalHealthReport',
             dataType: 'json',
             success: function (json) {

                 // var all = '<%= Resources.Main.all %>';
                 var $el = $("#MainContent_ddlOccupationalhealth");
                 $el.empty(); // remove old options
                 $el.append($("<option></option>")
                          .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });

             }
         });



     }



     function setTypecontrol()
     {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getTypecontrolHealth',
             dataType: 'json',
             success: function (json) {

                 // var all = '<%= Resources.Main.all %>';
                 var $el = $("#MainContent_ddtypecontrol");
                 $el.empty(); // remove old options
                 $el.append($("<option></option>")
                          .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });

             }
         });



     }



     function setActionHealthStatus()
     {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getActionHealthStatus',
             dataType: 'json',
             success: function (json) {

                 // var all = '<%= Resources.Main.all %>';
                 var $el = $("#MainContent_ddlStatus");
                 $el.empty(); // remove old options
                 $el.append($("<option></option>")
                          .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });

             }
         });



     }




     function setDurationRiskFactor()
     {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getDurationRiskFactor',
             dataType: 'json',
             success: function (json) {

                 // var all = '<%= Resources.Main.all %>';
                 var $el = $("#MainContent_ddlDurationriskfactor");
                 $el.empty(); // remove old options
                 $el.append($("<option></option>")
                          .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });

             }
         });



     }


     function setDurationRiskFactor() {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getDurationRiskFactor',
             dataType: 'json',
             success: function (json) {

                 // var all = '<%= Resources.Main.all %>';
                 var $el = $("#MainContent_ddlDurationriskfactor");
                 $el.empty(); // remove old options
                 $el.append($("<option></option>")
                          .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });

             }
         });



     }


     function setMonitoringResults()
     {
        var $el = $("#MainContent_ddmonitoringresults");
        $el.empty(); // remove old options
        $el.append($("<option></option>")
                .attr("value", "").text(""));
        
        $el.append($("<option></option>")
                    .attr("value", "comply").text('<%=Resources.Health.comply %>'));
        $el.append($("<option></option>")
                  .attr("value", "not_comply").text('<%= Resources.Health.not_comply %>'));
      
     }


     function setAbnormalPulmonaryFunction()
     {
         var $el = $("#MainContent_ddabnormal_pulmonary_function");
         $el.empty(); // remove old options
         $el.append($("<option></option>")
                 .attr("value", "").text(""));

         $el.append($("<option></option>")
                     .attr("value", "obstructive").text('<%=Resources.Health.lbobstructive %>'));
         $el.append($("<option></option>")
                   .attr("value", "restrictive").text('<%= Resources.Health.lbrestrictive %>'));

         $el.append($("<option></option>")
                  .attr("value", "obstructive_restrictive").text('<%= Resources.Health.lbobstructive_restrictive %>'));

     }


     function setAbnormalAudiogram()
     {
         var $el = $("#MainContent_ddabnormalaudiogram");
         $el.empty(); // remove old options
         $el.append($("<option></option>")
                 .attr("value", "").text(""));

         $el.append($("<option></option>")
                     .attr("value", "left").text('<%=Resources.Health.lbleft_ear %>'));
         $el.append($("<option></option>")
                   .attr("value", "right").text('<%= Resources.Health.lbright_ear %>'));

         $el.append($("<option></option>")
                  .attr("value", "both").text('<%= Resources.Health.lbboth_ear %>'));

     }



     function checkOther()
     {

         var ddl_risk_factor_id = $('#MainContent_ddlRiskfactorrelatework').val();

         $.ajax({
             type: "POST",
             data: { id: ddl_risk_factor_id},
             url: 'Masterdata.asmx/checkOtherRiskFactor',
             dataType: 'json',
             success: function (json) {


                 $.each(json, function (value, key) {

                     if (key.result == true)
                     {
                         $("#risk_factor_other").show();
                         other_risk_factor = true;

                     } else {

                         $("#risk_factor_other").hide();
                         other_risk_factor = false;
                     }

                  
                 });


             }
         });

        

     }



     function setOther(dd_risk_factor_id)
     {

      
         $.ajax({
             type: "POST",
             data: { id: dd_risk_factor_id },
             url: 'Masterdata.asmx/checkOtherRiskFactor',
             dataType: 'json',
             success: function (json) {


                 $.each(json, function (value, key) {

                     if (key.result == true) {
                         $("#risk_factor_other").show();
                        
                     } else {

                         $("#risk_factor_other").hide();
                        
                     }


                 });


             }
         });



     }


     function checkValidateOther(oSrc, args)
     {
        
         if (other_risk_factor == true)
         {
             if (args.Value != "") {

                 args.IsValid = true;

             } else {

                 args.IsValid = false;

             }

         } else {

             args.IsValid = true;
         }
        


     }




     function checkRepeatHealth(oSrc, args)
     {

         var repeat_health = $("input:radio[name='ctl00$MainContent$repeathealthcheck']:checked").val();

         if (repeat_health == undefined)
         {
             args.IsValid = false;

         } else {

             args.IsValid = true;
         }
         
     }



     function checkValidateStatus(oSrc, args)
     {

         var ddl_status = $("#MainContent_ddlStatus").val();


         if (parseInt(process_action_id) != 0)//edit
         {
             if (ddl_status != "")//
             {
                 args.IsValid = true;

             } else {

                 args.IsValid = false;
             }

         } else {

             args.IsValid = true;

         }
        

     }



     function CheckAbnormalAudiogram(oSrc, args)
     {
         var ddlOccupationalhealth = $("#MainContent_ddlOccupationalhealth").val();
         var ddabnormalaudiogram = $("#MainContent_ddabnormalaudiogram").val();


         if (parseInt(ddlOccupationalhealth) == 3)//ผิดปกติจากหู
         {
             if (ddabnormalaudiogram != "")
             {
                 args.IsValid = true;

             } else {

                 args.IsValid = false;
             }

         } else {

             args.IsValid = true;

         }


     }

     function CheckAbnormalPulmonaryFunction(oSrc, args)
     {
         var ddlOccupationalhealth = $("#MainContent_ddlOccupationalhealth").val();
         var ddabnormal_pulmonary_function = $("#MainContent_ddabnormal_pulmonary_function").val();


         if (parseInt(ddlOccupationalhealth) == 1)//ผิดปกติจากหู
         {
             if (ddabnormal_pulmonary_function != "") {
                 args.IsValid = true;

             } else {

                 args.IsValid = false;
             }

         } else {

             args.IsValid = true;

         }


     }

     





     function CheckThearingThresholdLevel(oSrc, args)
     {
         var ddlOccupationalhealth = $("#MainContent_ddlOccupationalhealth").val();
         var thearing_threshold_level = $("#MainContent_txthearing_threshold_level").val();
        

         if (parseInt(ddlOccupationalhealth) == 3)//ผิดปกติจากหู
         {
             if (thearing_threshold_level != "")//
             {
                 args.IsValid = true;

             } else {

                 args.IsValid = false;
             }

         } else {

             args.IsValid = true;

         }


     }



     function CheckSpecifyChronicDiseasesEar(oSrc, args)
     {
         var ddlOccupationalhealth = $("#MainContent_ddlOccupationalhealth").val();
         var chronic_diseases_ear = $("input:radio[name='ctl00$MainContent$chronic_diseases_ear']:checked").val();
         var specify_chronic_diseases_ear = $("#MainContent_txtspecify_chronic_diseases_ear").val();

         if (parseInt(ddlOccupationalhealth) == 3)//ผิดปกติจากหู
         {
             if (chronic_diseases_ear == "Y")
             {
                 if (specify_chronic_diseases_ear != "") {
                     args.IsValid = true;

                 } else {

                     args.IsValid = false;
                 }

             } else {

                 args.IsValid = true;
             }
           

         } else {

             args.IsValid = true;

         }


     }



     function DeleteRiskFactor(id)
     {
         var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
         if (confirm(message_confirm_delete)) {
             showLoading();
             $.ajax({
                 type: "POST",
                 data: { id: id,page_type:pagetype },
                 url: 'Actionevent.asmx/deleteRiskFactor',
                 success: function (json) {
           
                     closeLoading();
                     callRiskFactorRelate();

                 }
             });

         }


     }


     function DeleteOccupationalHealth(id)
     {
         var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
         if (confirm(message_confirm_delete)) {
             showLoading();
             $.ajax({
                 type: "POST",
                 data: { id: id, page_type: pagetype },
                 url: 'Actionevent.asmx/deleteOccupationalHealth',
                 success: function (json) {

                     closeLoading();
                     callOccupationalHealth();

                 }
             });

         }


     }
    



     function validateDuedate(oSrc, args)
     {

         var due_date = $("#MainContent_txtdue_date").val();
         $.ajax({
             type: "POST",
             data: { duedate: due_date, lang: lang },
             url: 'Actionevent.asmx/checkDuedate',
             dataType: 'json',
             async: false,
             cache: false,
             success: function (result) {

                 if (result == true) {//วันที่ duedate < datenow

                     args.IsValid = false;


                 } else {

                     args.IsValid = true;;

                 }

             }
         });


     }



     function CheckJobTypeLength(oSrc, args)
     {

         if (args.Value.length > 4000) {

             args.IsValid = false;
         } else {

             args.IsValid = true;
         }


     }





  



     function getEmployee(employee_id)
     {

         $.ajax({
             type: "POST",
             data: { lang: lang, employee_id: employee_id ,pagetype:pagetype },
             url: 'Masterdata.asmx/getEmployeeDropdown',
             dataType: 'json',
             success: function (json) {

                 var $el = $("#MainContent_ddemployee");
                 $el.empty(); // remove old options

                 $el.append($("<option></option>")
                            .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.employee_id).text(key.employee_id));

                     store_employee[key.employee_id]= {name:key.fullname,company_id:key.company_id,function_id:key.function_id,department_id:key.department_id,division_id : key.division_id, section_id:key.section_id,birth_date:key.birth_date,hiring_date:key.hiring_date}
                 });

                 if (employee_id != "")
                 {
                     $("#MainContent_ddemployee").val(employee_id).select2();
                 }
                 
             }
         });



     }


     function setOrg()
     {
         var employee_id = $('#MainContent_ddemployee').val();
         if (store_employee.length > 0)
         {
             if (employee_id != "")
             {
                 var fullname = store_employee[employee_id].name;

                 $("#MainContent_txtdateofbirth").datepicker({
                     format: "dd/mm/yyyy"
                 }).datepicker("setDate", store_employee[employee_id].birth_date);
                 $("#MainContent_txthiring_date").datepicker({
                     format: "dd/mm/yyyy"
                 }).datepicker("setDate", store_employee[employee_id].hiring_date);

                 $("#MainContent_txtemployee_name").val(fullname);
                 setAreaSelect(store_employee[employee_id].company_id, store_employee[employee_id].function_id, store_employee[employee_id].department_id, store_employee[employee_id].division_id, store_employee[employee_id].section_id, "", "", "", "", "", "", "", "", "", "");

             }
           
         }

     }


     function showUpdateReasonReject() {


         dialogReason.dialog("open");

         return false;

     }


     function UpdateReason()
     {

         var reason_type = $("#ddReasonreject").val();
         var reason = $("#MainContent_txtreasonreject").val();

         if (reason_type != "") {
             showLoading();
             $.ajax({
                 type: "POST",
                 data: {
                     healthid: id,
                     reason_reject_type: reason_type,
                     reasonreject: reason,
                     userid: user_login_id,
                     typelogin: type_login,
                     step_form: 1,
                     group_id: user_group_id,

                 },
                 url: 'Actionevent.asmx/updateReasonRejectHealth',
                 dataType: 'json',
                 success: function (result) {

                     closeLoading();
                     dialogReason.dialog("close");
                     $("#MainContent_txtreasonreject").val("");
                     $("#rqreason").text("");
                     //setShowedit();//update status
                     window.location.href = "healthform.aspx?pagetype=view&id=" + id;
                 }


             });
         } else {

             var require_reason = '<%= Resources.Incident.rqreasonreject %>';
             $("#rqreason").text(require_reason);
         }



     }


     function CloseReasonReject()
     {
         dialogReason.dialog("close");
         $("#ddReasonreject").val("");
         $("#MainContent_txtreasonreject").val("");


     }


     function setReasonReject()
     {
         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getReasonRejectHealth',
             dataType: 'json',
             success: function (json) {

                 var $el = $("#ddReasonreject");
                 $el.empty(); // remove old options

                 $el.append($("<option></option>")
                            .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });


             }
         });



     }


     function setHospital(hospital_id)
     {
         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getHospital',
             dataType: 'json',
             success: function (json) {

                 var $el = $("#MainContent_ddhospital");
                 $el.empty(); // remove old options

                 $el.append($("<option></option>")
                            .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });

                 if(hospital_id!="")
                 {
                     $("#MainContent_ddhospital").val(hospital_id).select2();
                 }

             }
         });



     }

 </script>


     <div id="create_reason_reject">
         <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.lbreasonreject %></label><div class="lbrequire"> *</div>
                      <select id="ddReasonreject" class="form-control">
                         
                        </select>                        
                    <label id="rqreason" class="text-danger"></label>  
                     
                </div>
                </div>

                              				
            </div>
       <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.detailreject %></label>
                           
                    <textarea class="form-control" rows="4" id="txtreasonreject" runat="server"></textarea>
                    
                     
                </div>
                </div>

                              				
            </div>
       <div class="row">
             
            <div class="form-group">
                <div class="pull-right">
                 <button id="btConfirm"class="btn btn-sm btn-primary" onclick="UpdateReason()"><%= Resources.Main.btconfirm %></button>
                 <button class="btn btn-sm btn-default" onclick="CloseReasonReject();"><%= Resources.Main.btCancel %></button>
                 </div>
             </div>  
        </div>
        
    </div>


     <div id="risk_factor_relate_work_form" title="<%= Resources.Health.risk_factor_relate_work_form %>">     
         
          	<div class="row">

				<div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"><%= Resources.Health.risk_factor_relate_work %></label><div class="lbrequire"> *</div>
						<select id="ddlRiskfactorrelatework" class="form-control" runat="server" onchange="checkOther();">
                            
                        </select>
                        
                         <asp:RequiredFieldValidator ID="rqriskfactorrelatework" runat="server" ControlToValidate ="ddlRiskfactorrelatework" ErrorMessage="<%$ Resources:Health, rqriskfactorrelatework %>" 
                             ValidationGroup="riskfactor" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
                  
		   </div>


         <div class="row">

				<div class="col-sm-12">
					<div class="form-group" id="risk_factor_other">
						<label class="control-label"><%= Resources.Health.risk_factor_relate_work_other %></label><div class="lbrequire"> *</div>
						 <input id="txtotherriskfactor" name="txtotherriskfactor"  type="text" class="form-control" runat="server">
                         <asp:CustomValidator id="rqcheckotherriskfactor" runat="server" ClientValidationFunction="checkValidateOther" ValidateEmptyText="true" ValidationGroup="riskfactor" ControlToValidate="txtotherriskfactor" Display="Dynamic" ErrorMessage="<%$ Resources:Health, rqcheckotherriskfactor %>" CssClass="text-danger"></asp:CustomValidator>

					</div>
				</div>
                  
		   </div>

          <div class="row">
                <div class="col-sm-12">
                     <label class="control-label"><%= Resources.Health.monitoringenvironment %></label><div class="lbrequire"> *</div>    
                        <div class="form-group">		
		                <div class="col-sm-6">
				
				            <input  type="radio" id="monitoringenvironment1" value="Y" name="monitoringenvironment" runat="server" style="padding-right:10px;"><%= Resources.Health.lbyes %>
			            </div>
		 
                        <div class="col-sm-6">							
					        <input  type="radio" id="monitoringenvironment2" value="N" name="monitoringenvironment" runat="server" style="padding-right:10px;"><%= Resources.Health.lbno %>
				        </div>
                            <label id="rqmonitoringenvironment" class="text-danger"></label> 
                 
		                </div>
                    </div>
	        </div>


          <div class="row">
                <div class="col-sm-6">
					<div class="form-group" id="year_risk_factor">
						<label class="control-label"> <%= Resources.Health.year_risk_factor_relate_work %></label><div class="lbrequire"> *</div>
					    <select id="ddlYearriskfactor" class="form-control" runat="server">
                            
                        </select>
                       
                        <asp:CustomValidator id="rqYearriskfactor" runat="server" ClientValidationFunction="CheckMonitoringEnvironment" ValidationGroup="riskfactor" Display="Dynamic" ControlToValidate="ddlYearriskfactor" ValidateEmptyText="True" ErrorMessage="<%$ Resources:Health, rqyearriskfactor %>" CssClass="text-danger"></asp:CustomValidator>
					</div>
				</div>
				
				<div class="col-sm-6">
					<div class="form-group">
						<label class="control-label"><%= Resources.Health.duration_risk_factor %></label><div class="lbrequire"> *</div>
						 <select id="ddlDurationriskfactor" class="form-control" runat="server">
                            
                        </select>
                        <asp:RequiredFieldValidator ID="rqdurationriskfactor" runat="server" ValidationGroup="riskfactor" ControlToValidate ="ddlDurationriskfactor" ErrorMessage="<%$ Resources:Health, rqdurationriskfactor %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>
                   	
                    </div>
				</div>


                 
		   </div>

         

        


           <div class="row">
                <div class="col-sm-12">
					<div class="form-group" id="year_risk_factor2">
						<label class="control-label"> <%= Resources.Health.result_risk_factor %></label><div class="lbrequire"> *</div>
					     <textarea class="form-control" rows="3" id="txtresultriskfactor" runat="server"></textarea>

                         <asp:CustomValidator id="rqresultriskfactor" runat="server" ClientValidationFunction="CheckResultRiskfactor" ValidationGroup="riskfactor" Display="Dynamic" ControlToValidate="txtresultriskfactor" ValidateEmptyText="True" ErrorMessage="<%$ Resources:Health, rqresultriskfactor %>" CssClass="text-danger"></asp:CustomValidator>	
					</div>
				</div>
				
                 
		   </div>

         	<div class="row">

				<div class="col-sm-12">
					<div class="form-group" id="year_risk_factor3">
						<label class="control-label"><%= Resources.Health.monitoring_results %></label><div class="lbrequire"> *</div>
						<select id="ddmonitoringresults" class="form-control" runat="server">
                            
                        </select>
                        
                         <asp:CustomValidator id="rqmonitoringresults" runat="server" ClientValidationFunction="CheckMonitoringResults" ValidationGroup="riskfactor" Display="Dynamic" ControlToValidate="ddmonitoringresults" ValidateEmptyText="True" ErrorMessage="<%$ Resources:Health, rqmonitoringresults %>" CssClass="text-danger"></asp:CustomValidator>	
					</div>
				</div>
                  
		   </div>

               
            <div class="row">
            <div class="col-sm-12">                
                <div class="form-group" id="year_risk_factor4">
                         <label class="control-label"><%= Resources.Health.file_risk_factor %><div class="lbrequire"> <%= Resources.Health.inforiskimage %></div></label>                                   
                        <div  class="dropzone" id="dropzoneRiskFactor" style="margin-top:8px;">
                            <div class="fallback">
                                <input name="fileriskfactor" type="file"/>
                                <input type="submit" value="Upload" />
                            </div>
                        </div>
                          
                </div>
            </div>
          </div>

			
           
          

             <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                        <asp:Button ID="btCreateriskfactor" runat="server" ValidationGroup="riskfactor"  Text="<%$ Resources:Main, btadd %>" OnClientClick="addRiskFactor();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btUpdateriskfactor" runat="server" ValidationGroup="riskfactor" Text="<%$ Resources:Main, btsave %>" OnClientClick="updateRiskFactor();" CssClass="btn btn-primary"/>
                        <button type="button" id="btCloseRiskFactor" class="btn btn-default" runat="server" onclick="closeRiskFactor();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>

      
    </div>



     <div id="occupational_health_form" title="<%= Resources.Health.occupational_health_form %>">     
         
          	<div class="row">
				
				<div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"><%= Resources.Health.occupational_health_report %></label><div class="lbrequire"> *</div>
						<select id="ddlOccupationalhealth" class="form-control" runat="server">
                            
                        </select>
                        
                         <asp:RequiredFieldValidator ID="rqoccupationalhealth" runat="server" ControlToValidate ="ddlOccupationalhealth" ErrorMessage="<%$ Resources:Health, rqoccupationalhealth %>" 
                             ValidationGroup="occupationalhealth" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
                  
		   </div>

         	<div class="row" id="abnormalaudiogram">

				<div class="col-sm-6">
					<div class="form-group">
						<label class="control-label"><%= Resources.Health.lbabnormalaudiogram %></label><div class="lbrequire"> *</div>
						<select id="ddabnormalaudiogram" class="form-control" runat="server">
                            
                        </select>
                        
                          <asp:CustomValidator id="rqabnormalaudiogram" runat="server" ClientValidationFunction="CheckAbnormalAudiogram" Display="Dynamic" ControlToValidate="ddabnormalaudiogram"  ErrorMessage="<%$ Resources:Health, rqabnormalaudiogram %>" ValidationGroup="occupationalhealth" ValidateEmptyText="True" CssClass="text-danger" SetFocusOnError="true" ></asp:CustomValidator>
					  
					</div>
				</div>
				
				    <div class="col-md-6">
						<div class="form-group">
						<label class="control-label"><%= Resources.Health.lbhearing_threshold_level %></label><div class="lbrequire"> *</div>
			   
						<input class="form-control" value="" type="text" id="txthearing_threshold_level" runat="server">
						 <asp:CustomValidator id="rqthearing_threshold_level" runat="server" ClientValidationFunction="CheckThearingThresholdLevel" Display="Dynamic" ControlToValidate="txthearing_threshold_level" ValidateEmptyText="True"  ErrorMessage="<%$ Resources:Health, rqthearing_threshold_level %>" CssClass="text-danger" SetFocusOnError="true" ValidationGroup="occupationalhealth"></asp:CustomValidator>
					  
					</div>
					</div>
                  
		   </div>

         	<div class="row" id="abnormal_pulmonary_function">

				<div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"><%= Resources.Health.lbabnormal_pulmonary_function %></label><div class="lbrequire"> *</div>
						<select id="ddabnormal_pulmonary_function" class="form-control" runat="server">
                            
                        </select>
                        
                          <asp:CustomValidator id="rqabnormal_pulmonary_function" runat="server" ClientValidationFunction="CheckAbnormalPulmonaryFunction" Display="Dynamic" ControlToValidate="ddabnormal_pulmonary_function"  ErrorMessage="<%$ Resources:Health, rqabnormal_pulmonary_function %>" ValidationGroup="occupationalhealth" ValidateEmptyText="True" CssClass="text-danger" SetFocusOnError="true" ></asp:CustomValidator>
					  
					</div>
				</div>

		   </div>

         <div class="row">
        <div class="col-sm-12">
                      
            <div class="form-group">
                      <label class="control-label"><%= Resources.Health.file_result_health %> <div class="lbrequire"><%= Resources.Health.infooccuimage %></div></label>                                  
                    <div  class="dropzone" id="dropzoneOccupationalHealth" style="margin-top:8px;">
                        <div class="fallback">
                            <input name="filehealthcheck" type="file"/>
                            <input type="submit" value="Upload" />
                        </div>
                    </div>
                          
            </div>
            </div>
      </div>


       <div class="row">
           <div class="col-sm-12">
                 <div class="form-group">		
		            <div class="col-sm-6">
				        				
					    <input  type="radio" id="rdNorepeathealthcheck" value="N" name="repeathealthcheck" runat="server" style="padding-right:10px;"><%= Resources.Health.no_repeat_health_check %>

				    </div>
		 
                    <div class="col-sm-6">	
                         <input  type="radio" id="rdRepeathealthcheck" value="Y" name="repeathealthcheck" runat="server" style="padding-right:10px;"><%= Resources.Health.repeat_health_check %>
			       						    </div>
                      <label id="rqrepeathealthcheck" class="text-danger"></label> 
                 
		          </div>
               </div>
	    </div>


      <div class="row">
        <div class="col-sm-12">
                      
            <div class="form-group" id="show_upload_repeat_health_check">
                      <label class="control-label"><%= Resources.Health.file_repeat_result_health %><div class="lbrequire"><%= Resources.Health.infooccuimage %></div></label>                                  
                    <div  class="dropzone" id="dropzonerRepeatOccupationalHealth" style="margin-top:8px;">
                        <div class="fallback">
                            <input name="filerepeathealthcheck" type="file"/>
                            <input type="submit" value="Upload" />
                        </div>
                    </div>
                          
            </div>
            </div>
      </div>

         
        <div class="row"  id="abnormalaudiogram2">
         <div class="col-sm-12">
                <label class="control-label"><%= Resources.Health.lbchronic_diseases_ear  %></label><div class="lbrequire"> *</div>    
                <div class="form-group">		
		            <div class="col-sm-2">
				           <input  type="radio" id="chronic_diseases_ear2" value="N" name="chronic_diseases_ear" runat="server" style="padding-right:10px;"><%= Resources.Health.lbno %>			      
			        </div>
		 
                    <div class="col-sm-2">	
                          <input  type="radio" id="chronic_diseases_ear1" value="Y" name="chronic_diseases_ear" runat="server" style="padding-right:10px;"><%= Resources.Health.lbyes %>										 
				    </div>
                     
                     <div class="col-sm-8">
                     <div class="form-group" id="specify_abnormalaudiogram">
                         <div class="col-sm-5">
                             <label class="control-label"><%= Resources.Health.lbspecify_chronic_diseases_ear  %></label><div class="lbrequire"> *</div>
                        </div>
                        <div class="col-sm-7">
                            <input class="form-control" value="" type="text" id="txtspecify_chronic_diseases_ear" runat="server">
				        <asp:CustomValidator id="rqspecify_chronic_diseases_ear" runat="server" ClientValidationFunction="CheckSpecifyChronicDiseasesEar" ValidateEmptyText="True" Display="Dynamic" ControlToValidate="txtspecify_chronic_diseases_ear"  ErrorMessage="<%$ Resources:Health, rqspecify_chronic_diseases_ear %>" CssClass="text-danger" SetFocusOnError="true" ValidationGroup="occupationalhealth"></asp:CustomValidator>
			          </div>
                  </div>  
                 </div>  
            
		        </div>
                <label id="rqchronic_diseases_ear" class="text-danger"></label> 
                
            </div>
	    </div>

         <div class="row"  id="abnormal_pulmonary_function2">
         <div class="col-sm-12">
            <label class="control-label"><%= Resources.Health.lbsmoked_cigarettes  %></label><div class="lbrequire"> *</div>    
            <div class="radio">
                <label>
                    <input  type="radio" id="smoked_cigarettes1" value="NO" name="smoked_cigarettes" runat="server" style="padding-right:10px;"><%= Resources.Health.lbno %>		
                </label>
            </div>
            <div class="radio">
                <label>
                   <div class="controls form-inline"><input  type="radio" id="smoked_cigarettes2" value="YES_SMOKING" name="smoked_cigarettes" runat="server" style="padding-right:10px;"><%= Resources.Health.lbyes_smoking %>
                    <input class="form-control" value="" type="text" id="txtcigarettes_per_day" runat="server" style="margin:0 10px 0 10px;width:70px;"><%= Resources.Health.lbcigarettes_per_day %></div>
                    
                </label>
            </div>
            <div class="radio">
                <label>
                     <div class="controls form-inline"><input  type="radio" id="smoked_cigarettes3" value="YES_SMOKED" name="smoked_cigarettes" runat="server" style="padding-right:10px;"><%= Resources.Health.lbyes_smoked %>
                    <input class="form-control" value="" type="text" id="txtyears" runat="server" style="margin:0 10px 0 10px;width:70px;"><%= Resources.Health.lbyears %>	
                    <input class="form-control" value="" type="text" id="txtmonths" runat="server" style="margin:0 10px 0 10px;width:70px;"><%= Resources.Health.lbmonths %>	</div>
                </label>
            </div>
             <div class="radio">
                <label>
                    <div class="controls form-inline"><input  type="radio" id="smoked_cigarettes4" value="SMOKED_OTHER" name="smoked_cigarettes" runat="server" style="padding-right:10px;"><%= Resources.Health.lbsmoked_other %>
                   <input class="form-control" value="" type="text" id="txtsmoked_other" runat="server" style="margin:0 10px 0 10px;"></div>					
                </label>
            </div>
           <label id="rqsmoked_cigarettes" class="text-danger"></label> 
                
            </div>
	    </div>
    
         
        <div class="row">
           <div class="col-sm-12">
                 <div class="form-group">		
		           <br />
		          </div>
               </div>
	    </div>
          

             <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                        <asp:Button ID="btCreatoccupationhealth" runat="server" ValidationGroup="occupationalhealth"  Text="<%$ Resources:Main, btadd %>" OnClientClick="addOccupationalHealth();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btUpdateoccupationhealth" runat="server" ValidationGroup="occupationalhealth"  Text="<%$ Resources:Main, btsave %>" OnClientClick="updateOccupationalHealth();" CssClass="btn btn-primary"/>
                        <button type="button" id="btCloseoccupationhealth" class="btn btn-default" runat="server" onclick="closeOccupationalHealth();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>

      
    </div>










 
<div class="ibox float-e-margins">
                
 <div class="ibox-content" style="display: block;">

               
<div class="stepwizard">
      <div class="stepwizard-row setup-panel">
        <div class="stepwizard-step">
            <asp:LinkButton ID="step1" runat="server" CssClass="btn btn-primary btn-circle a-step" CausesValidation="False" OnClick="step1_Click">1</asp:LinkButton>
        <p><%= Resources.Health.healthstep1 %></p>
        </div>
       
        <div class="stepwizard-step">
            <asp:LinkButton ID="step2" runat="server" CssClass="btn btn-default btn-circle" CausesValidation="False" OnClick="step2_Click">2</asp:LinkButton>
        <p><%= Resources.Health.healthstep2 %></p>
        </div>

         <div class="stepwizard-step">
            <asp:LinkButton ID="step3" runat="server" CssClass="btn btn-default btn-circle" CausesValidation="False" OnClick="step3_Click" >3</asp:LinkButton>             
        <p><%= Resources.Health.healthstep3 %></p>
        </div>
    </div>
</div>

    <div class="row setup-content" id="step-1">
        <div class="col-xs-12">
            <div class="col-lg-12">
                <div class="row">
                    <div class="col-md-12">
                    <div style="font-weight:bold;" id="show_doc_status"></div>
                    <div class="pull-right">

                                             
                       
                    </div>
                    </div>
                  </div>
                <hr>
                 <div class="row">
                    <div class="col-md-12">
                    <div class="pull-right">

                        
                        <%
                            string PageType = Request.QueryString["PageType"];
                            string user_id= Session["user_id"].ToString();
                            
                            string id = "";

                            if (PageType == "edit" || PageType == "view")
                            {
                                id = Request.QueryString["id"];
                            }
                            ArrayList per = Session["permission"] as ArrayList;
                           
                           // bool pa2 = safetys4.Class.SafetyPermission.checkPermisionAction("report health1 edit", id, "Health", Convert.ToInt32(Session["group_value"]));
                           bool ca = safetys4.Class.SafetyPermission.checkPermissionHealthCreator(id, user_id);  
                           bool area = safetys4.Class.SafetyPermission.checkPermisionInArea(id, "health");
                           bool form3 = safetys4.Class.SafetyPermission.checkPermissionHealthForm3(id);

                            if (per.IndexOf("report health1 edit") > -1 && area == true && ca == true && form3==true)         
                           {                            
       
                          %>
                              <asp:Button ID="btHealthedit" runat="server" Text="<%$ Resources:Health, btHealthedit %>" CssClass="btn btn-primary"  CausesValidation="False" OnClick="btHealthedit_Click"/>

                          <%                      
                            }
       
                          %>



                           <%
                            string PageType2 = Request.QueryString["PageType"];
                            string id2 = "";

                            if (PageType2 == "edit" || PageType2 == "view")
                            {
                                id2 = Request.QueryString["id"];
                            }
                            ArrayList per2= Session["permission"] as ArrayList;

                            bool ca2 = safetys4.Class.SafetyPermission.checkPermissionHealthCreator(id, user_id);
                            bool area2 = safetys4.Class.SafetyPermission.checkPermisionInArea(id2, "health");


                            if (per2.IndexOf("report health1 reject") > -1 && ca2 == true && area2 == true)         
                           {                            
       
                          %>
                             <asp:Button ID="btHealthreject" runat="server" Text="<%$ Resources:Incident, btIncidentreject %>" CssClass="btn btn-primary" CausesValidation="False" OnClientClick="return showUpdateReasonReject();"/>

                          <%                      
                            }
       
                          %>








                         </div>
                    </div>
                  </div>
                           
                
                
                
                 <div class="row" style="padding-bottom:10px;"> <div class="col-md-4"><strong>
                         <h3><%= Resources.Health.lbhealth_rehab %></h3></strong></div></div>

                			<div class="row">
                                <div class="col-md-3">
                                     <div class="form-group">
                                        <label class="control-label"><%= Resources.Health.lbemployeeid %></label><div class="lbrequire"> *</div>
                                          
                                           <%-- <input class="form-control" value="" type="text" id="txtemployee_id" runat="server">--%>
                                            <select id="ddemployee" class="form-control select2" onchange="setOrg()" runat="server">
                       
                                            </select>
                                      
                                           <asp:RequiredFieldValidator ID="rqemployeeid" runat="server" ControlToValidate ="ddemployee" ValidationGroup="health" ErrorMessage="<%$ Resources:Health, rqemployeeid %>" CssClass="text-danger" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                          
                                    </div>

                                </div>
                                <div class="col-md-2">
                                     <div class="form-group">
                                        <label class="control-label"><%= Resources.Health.lbyear%></label><div class="lbrequire"> *</div>
                                     
                                             <select id="ddyear" class="form-control"  runat="server">
                       
                                             </select>
                                          <asp:RequiredFieldValidator ID="rqyear" runat="server" ControlToValidate ="ddyear" ErrorMessage="<%$ Resources:Health, rqyear %>" ValidationGroup="health" CssClass="text-danger" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                     </div>
                                </div>
                                <div class="col-md-4">
                                     <div class="form-group">
                                        <label class="control-label"><%= Resources.Health.lbhospital %></label><div class="lbrequire"> *</div>
                                          
                                            <select id="ddhospital" class="form-control select2" runat="server">
                       
                                            </select>
                                      
                                           <asp:RequiredFieldValidator ID="rqhospital" runat="server" ControlToValidate ="ddhospital" ValidationGroup="health" ErrorMessage="<%$ Resources:Health, rqhospital %>" CssClass="text-danger" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                          
                                    </div>

                                </div>
                                <div class="col-md-3">
                                     <div id="data_report_date" class="form-group">
                                       <label class="control-label"><%= Resources.Health.report_date %></label>
                                 
                                         <input class="form-control" value="" type="text" id="txtreport_date" runat="server">
                                        
                                     </div>
                                </div>
                          
                            </div>


                          <div class="row">
                         <div class="col-md-12">
                               <div class="form-group">
                                    <label class="control-label"><%= Resources.Health.lbemployee_name %></label>
                           
                                    <input class="form-control" value="" type="text" id="txtemployee_name" runat="server" disabled="disabled">
                                  
                                </div>
                                </div>

                              				
                           </div>





                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Health.lbCompany %></label>                           

                                       <select id="ddcompany" name="ddcompany" class="form-control" onchange="changCompany();" runat="server" disabled="disabled">
                       
                                        </select>
                                        
                                    
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Health.lbfucntion %></label>
                                    
                                     <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server" disabled="disabled">
                       
                                        </select>
                                  
                                       
                                    </div>
                                </div>
                              
                            </div>



                          <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Health.lbdepartment %></label>                                             
                                    
                                        <select id="dddepartment" class="form-control" onchange="changDepartment();" runat="server" disabled="disabled">
                       
                                        </select>
                                          
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Health.lbdivision %></label>
                                        
                                         <select id="dddivision" class="form-control" onchange="changDivision();" runat="server" disabled="disabled">
                       
                                        </select>
                                   </div>
                                </div>

                               <div class="col-md-4">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Health.lbsection %></label>
                                    
                                    
                                     <select id="ddsection" class="form-control"  runat="server" disabled="disabled">
                       
                                        </select>
                                    </div>
                                </div>

                              	
                            </div>


                  <div class="row">
                             
                        <div class="col-md-6">
                            <div id="data_birth_date" class="form-group">
                            <label class="control-label"><%= Resources.Health.lbdateofbirth  %></label><div class="lbrequire"> *</div>
                                <div class="input-group date">
                                <input id="txtdateofbirth" name="txtdateofbirth"  type="text" class="form-control" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                                </div>
                                <asp:RequiredFieldValidator ID="rqdateofbirth" runat="server" ControlToValidate ="txtdateofbirth" ValidationGroup="health" ErrorMessage="<%$ Resources:Health, rqdateofbirth %>" CssClass="text-danger" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                              
                            </div>

                        </div>


                         <div class="col-md-6">
                            <div id="data_hiring_date" class="form-group">
                              <label class="control-label"><%= Resources.Health.lbhiringdate %></label><div class="lbrequire"> *</div>
                                <div class="input-group date">
                                <input id="txthiring_date" name="txthiring_date"  type="text" class="form-control" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                                </div>
                                <asp:RequiredFieldValidator ID="rqhiringdate" runat="server" ControlToValidate ="txthiring_date" ValidationGroup="health" ErrorMessage="<%$ Resources:Health, rqhiringdate %>" CssClass="text-danger" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                              
                            </div>

                        </div>

                    
                   </div>






                   <div class="row">
                              

                        <div class="col-md-4">
                            <div class="form-group">
                            <label class="control-label"><%= Resources.Health.lbage %></label>
                            <input id="txtage" name="txtage"  type="text" class="form-control" runat="server" disabled="disabled">
                               <%-- <asp:RequiredFieldValidator ID="rqage" runat="server" ControlToValidate ="txtage" ValidationGroup="health" ErrorMessage="<%$ Resources:Health, rqage %>" CssClass="text-danger" Display="Dynamic">
                                </asp:RequiredFieldValidator>--%>

                              <%--  <asp:RegularExpressionValidator ID="rqcheckage" runat="server" ControlToValidate="txtage" SetFocusOnError="true" ValidationGroup="health" ErrorMessage="<%$ Resources:Health, rqcheckage %>" ValidationExpression="^\d+$" Display="Dynamic" CssClass="text-danger">
                                </asp:RegularExpressionValidator>--%>	
                              
                            </div>
                        </div>

                        <div class="col-md-4">
                            <div class="form-group">
                            <label class="control-label"><%= Resources.Health.lbserviceyear %></label>
                            <input id="txtservice_year" name="txtservice_year"  type="text" class="form-control" runat="server" disabled="disabled">
                               <%-- <asp:RequiredFieldValidator ID="rqserviceyear" runat="server" ControlToValidate ="txtservice_year" ValidationGroup="health" ErrorMessage="<%$ Resources:Health, rqserviceyear %>" CssClass="text-danger" Display="Dynamic">
                                </asp:RequiredFieldValidator>--%>

                               <%-- <asp:RegularExpressionValidator ID="rqcheckservice_year" runat="server" ControlToValidate="txtservice_year" SetFocusOnError="true" ValidationGroup="health" ErrorMessage="<%$ Resources:Health, rqcheckservice_year %>" ValidationExpression="^[0-9]*(?:\.[0-9]*)?$" Display="Dynamic" CssClass="text-danger">
                                </asp:RegularExpressionValidator>--%>
                              
                            </div>
                        </div>

                           <div class="col-md-4">
                            <div class="form-group">
                            <label class="control-label"><%= Resources.Health.lbserviceyear_current %></label><div class="lbrequire"> *</div>
                                                                        
                                <input id="txtservice_year_current" name="txtservice_year_current"  type="text" class="form-control" runat="server" placeholder="2.4">

                                <asp:RequiredFieldValidator ID="rqserviceyearcurrent" runat="server" ControlToValidate ="txtservice_year_current" ValidationGroup="health" ErrorMessage="<%$ Resources:Health, rqserviceyearcurrent %>" CssClass="text-danger" Display="Dynamic">
                                </asp:RequiredFieldValidator>

                                 <asp:RegularExpressionValidator ID="rqcheckservice_year_current" runat="server" ControlToValidate="txtservice_year_current" ValidationGroup="health" SetFocusOnError="true" ErrorMessage="<%$ Resources:Health, rqcheckservice_year_current %>" ValidationExpression="^[0-9]*(?:\.[0-9]*)?$" Display="Dynamic" CssClass="text-danger">
                                </asp:RegularExpressionValidator>
                            </div>
                         </div>

                              				
                   </div>










                   <div class="row">
                    <div class="col-md-12">
                          <strong><h3><%= Resources.Health.risk_factor_relate_work_form %><div class="lbrequire"> *</div></h3></strong>
                       </div>
                              
                   </div>
     

                 <div  class="row">
                    <div class="col-md-12">
                          <table id="tbRisk_factor_relate_work" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <%--<th rowspan="2"> <%= Resources.Health.no %></th>--%>
                                    <th></th>
                                    <th> <%= Resources.Health.risk_factor_relate_work %></th>
                                    <th> <%= Resources.Health.monitoringenvironment %></th>
                                    <th> <%= Resources.Health.year_risk_factor_relate_work %></th>
                                    <th> <%= Resources.Health.result_risk_factor %></th>
                                    <th> <%= Resources.Health.duration_risk_factor %></th>
                                    <th> <%= Resources.Health.monitoring_results %></th>
                                    <th> <%= Resources.Health.file_risk_factor %></th>
                                    <th> <%= Resources.Health.manage %></th>

                                 </tr>
                            </thead>
                            
                            </table>




                       </div>
                              
                   </div>

                <div  class="row">
                    <div class="col-md-12">
                       <button type="button" id="btShowCreateRiskFactor" class="btn btn-primary" runat="server" onclick="showCreateRiskFactor();"><i class="fa fa-plus"></i></button>  <%= Resources.Health.add_risk_factor_relate_work %>
                       
                       </div>
                              
                   </div>

                <div  class=="row">
                    <div class="col-md-12">
                        <br />
                    </div>
                              
                 </div>


                    <div class="row">
                     <div class="col-md-12">
                           <div class="form-group">
                                <label class="control-label"><%= Resources.Health.lbjobtype %> (4000 <%= Resources.Incident.rqCharacters %>)</label><div class="lbrequire"> *</div>
                           
                                <textarea class="form-control" rows="5" id="txtjob_type" runat="server"></textarea>
                                 <asp:RequiredFieldValidator ID="rqjobtype" runat="server" ControlToValidate ="txtjob_type" ErrorMessage="<%$ Resources:Health, rqjobtype %>" ValidationGroup="health" CssClass="text-danger" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                               <asp:CustomValidator id="rqJobTypeLength" runat="server" ClientValidationFunction="CheckJobTypeLength" Display="Dynamic" ControlToValidate="txtjob_type"  ErrorMessage="<%$ Resources:Health, rqJobTypeLength %>" CssClass="text-danger" SetFocusOnError="true"></asp:CustomValidator>
                            </div>
                            </div>

                              				
                       </div>

                
                 <div class="row">
                    <div class="col-md-12">
                          <strong><h3><%= Resources.Health.occupational_health_form %><div class="lbrequire"> *</div></h3></strong>
                       </div>
                              
                   </div>
     

                 <div  class="row">
                    <div class="col-md-12">
                          <table id="tbOccupational_health" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <%--<th> <%= Resources.Health.no %></th>--%>
                                    <th></th>
                                    <th> <%= Resources.Health.occupational_health_report %></th>
                                    <th> <%= Resources.Health.lbabnormalaudiogram %></th>
                                    <th> <%= Resources.Health.lbabnormal_pulmonary_function %></th>
                                    <th> <%= Resources.Health.lbhearing_threshold_level %></th>
                                    <th> <%= Resources.Health.file_health_check %></th>
                                    <th> <%= Resources.Health.have_repeat_health_check %></th>
                                    <th> <%= Resources.Health.not_repeat_health_check %></th>
                                    <th> <%= Resources.Health.flie_repeat_health_check %></th>
                                    <th> <%= Resources.Health.lbchronic_diseases_ear %></th>
                                    <th> <%= Resources.Health.lbsmoked_cigarettes %></th>
                                    <th> <%= Resources.Health.manage %></th>
                    
                                </tr>
                            </thead>
                            
                            </table>




                       </div>
                              
                   </div>

                <div  class="row">
                    <div class="col-md-12">
                       <button type="button" id="btCreateOccupationalHealth" class="btn btn-primary" runat="server" onclick="showCreateOccupationalHealth();"><i class="fa fa-plus"></i></button>  <%= Resources.Health.add_occupational_health %>
                       
                       </div>
                              
                   </div>

                <div  class=="row">
                    <div class="col-md-12">
                        <br />
                    </div>
                              
                 </div>


                     <div class="row">
                       <div class="col-sm-12">
                             <div class="form-group">		
		                        <div class="col-sm-6">
				
				                   <input  type="radio" id="rdSignificant" value="sign" name="significantorinsignificant" runat="server" style="padding-right:10px;"><%= Resources.Health.significant %>
			                    </div>
		 
                                <div class="col-sm-6">							
					                <input  type="radio" id="rdInsignificant" value="insign" name="significantorinsignificant" runat="server" style="padding-right:10px;"><%= Resources.Health.insignificant %>
				                </div>
                                  <label id="rqsignificantorinsignificant" class="text-danger"></label> 
                 
		                      </div>
                           </div>
	                </div>


                  


                <div  class=="row">
                    <div class="col-md-12">
                        <br />
                    </div>
                              
                 </div>


                <div  class=="row">
                    <div class="col-md-12">
                        <div class="lbrequire"><%= Resources.Health.message_form1 %></div>
                    </div>
                              
                 </div>
                
           

                              				
                   </div>

                </div>
            

            

        
            

             <div class="row">                             
                <div class="col-md-12">
                  <span class="pull-right">
                            <asp:Label ID="lbModify" runat="server" Text="<%$ Resources:Contractor, lbModify %>"></asp:Label>
                            <asp:Label ID="lbEmployee" runat="server" Text=""></asp:Label>
                            <asp:Label ID="lbDate" runat="server" Text="<%$ Resources:Contractor, lbUpdate %>"></asp:Label>
                            <asp:Label ID="lbUpdate" runat="server" Text=""></asp:Label>
                   </span>
               </div>
            </div>

            <div class="row">                             
                <div class="col-md-4">
                <asp:Button ID="btSubmit" runat="server" Text="<%$ Resources:Main, btSubmit %>"  CssClass="btn btn-primary" OnClientClick="return saveHealth();" />  
                <asp:Button ID="btUpdate" runat="server" Text="<%$ Resources:Main, btSubmit %>"  CssClass="btn btn-primary" OnClientClick="return updateHealth();" />    
                 

               </div>
            </div>
              

            </div>
        </div>
    </div>
 







</asp:Content>
