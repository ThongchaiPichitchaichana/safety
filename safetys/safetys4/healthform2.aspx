<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="healthform2.aspx.cs" Inherits="safetys4.healthform2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


  <link href="template/css/plugins/dropzone/dropzone.css" rel="stylesheet" />
<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet"> 
<link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">


<link rel="stylesheet" href="template/js/plugins/fancybox/jquery.fancybox.css" type="text/css" />
   
<script type="text/javascript" src="template/js/plugins/dropzone/dropzone.min.js"></script>

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
        height: 46px;
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

     var id = "";
     var pagetype = "";


     var process_action_id = 0;
     var filename_opinion_doctor = "";
     var filename_recovery_plan = "";
     var filename_process_action = "";

     var dropzone_opinion_doctor;
     var dropzone_recovery_health;
     var dropzone_process_action_health;

     var folder_image = "";

     $(document).ready(function () {

         var url = window.location.href;
         var urlarr = url.split("=");

         if (urlarr.length > 2) {
             id = urlarr[2];
             pagetype_arr = urlarr[1].split("&");
             pagetype = pagetype_arr[0];

         } else {
             pagetype = urlarr[1];


         }

         if (pagetype == "view") {
             setShowedit();
         

         } else if (pagetype == "edit") {

             setShowedit();

         } 


       
         setTypecontrol();
         setActionHealthStatus();
        
         setDropzoneOpinionDoctor();
         setDropzoneRecoveryPlan();
         setDropzoneProcessActionHealth();

         setDatatableProcessAction();
      



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


      


        dialogProcessAction = $("#process_action_form").dialog({
            autoOpen: false,
            height: 650,
            width: 650,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {
                //clearValidationErrors();
                // setRootCauseAction();
                $("#process_action_form").css('overflow-x', 'hidden');
            },
            modal: true,
        });



        $("#MainContent_txtresponsible_person").autocomplete({
            source: "Masterdata.asmx/getEmployeeautocompleteofactionhealth",
            select: function (event, ui) {
                $("#MainContent_employee_id").val(ui.item.employee_id);

            }
        });


        $("#MainContent_ddtypecontrol").change(function () {
            var type_control_id = $("#MainContent_ddtypecontrol").val();
            $.ajax({
                type: "POST",
                data: { id: type_control_id },
                url: 'Masterdata.asmx/checkTypecontrolrecoveryplan',
                dataType: 'json',
                success: function (json) {


                    $.each(json, function (value, key) {

                        if (key.result == true) {
                            $("#show_opinion_doctor").show();
                            $("#show_recovery_plan").show();
                        } else {
                            $("#show_opinion_doctor").hide();
                            $("#show_recovery_plan").hide();

                        }


                    });


                }
            });

        });



    });






     function addProcessAction()
     {
        var type_control = $("#MainContent_ddtypecontrol").val();

        if (Page_ClientValidate("process")) {

            var status = $("#MainContent_ddlStatus").val();

            if (status != "2")//close
            {

                checkTypeControlRecoveryPlan(type_control);


            } else {

                if (filename_process_action != "") {
                    checkTypeControlRecoveryPlan(type_control);

                } else {

                    var require_file_for_close = '<%= Resources.Health.rqfile_action_close %>';
                alert(require_file_for_close);
                return false;


            }

        }





    }
    else {
        return false;
    }


}



     function sendAddProcessAction()
     {
         var type_control = $("#MainContent_ddtypecontrol").val();
         var action = $("#MainContent_txtaction").val();
         var responsible_person = $("#MainContent_txtresponsible_person").val();
         var due_date = $("#MainContent_txtdue_date").val();
         var status = $("#MainContent_ddlStatus").val();
         var remark = $("#MainContent_txtremark").val();
         var employee_id = $("#MainContent_employee_id").val();


         $.ajax({
             type: "POST",
             data: {
                 type_control: type_control,
                 action: action,
                 responsible_person: responsible_person,
                 employee_id: employee_id,
                 due_date: due_date,
                 remark: remark,
                 status: status,
                 filename_opinion_doctor: filename_opinion_doctor,
                 filename_recovery_plan: filename_recovery_plan,
                 filename_process_action: filename_process_action,
                 user_id: user_login_id,
                 health_id: id,
                 page_type: pagetype

             },
             url: 'Actionevent.asmx/createProcessActionHealth',
             dataType: 'json',
             success: function (result) {
                 closeLoading();
                 closeProcessAction();
                 clearProcessAction();
                 callProcessAction();


                 dropzone_opinion_doctor.removeAllFiles();
                 dropzone_recovery_health.removeAllFiles();
                 dropzone_process_action_health.removeAllFiles();

             }
         });


     }

     function checkTypeControlRecoveryPlanUpdate(id)
     {

         $.ajax({
             type: "POST",
             data: { id: id },
             url: 'Masterdata.asmx/checkTypecontrolrecoveryplan',
             dataType: 'json',
             success: function (json) {


                 $.each(json, function (value, key) {

                     if (key.result == true) {
                         if (filename_recovery_plan != "" && filename_opinion_doctor != "") {
                             sendUpdateProcessAction();

                         } else {
                             var txt_alert = "";

                             if (filename_recovery_plan == "") {
                                 var require_file_recovery = '<%= Resources.Health.rqfile_recovery_plan %>';
                                 txt_alert = txt_alert + require_file_recovery;
                             }


                             if (filename_opinion_doctor == "") {
                                 var require_file_opinion = '<%= Resources.Health.rqfile_opinion_doctor %>';
                                 txt_alert = txt_alert + "\n" + require_file_opinion;
                             }

                             alert(txt_alert);
                             return false;
                         }


                     } else {
                         sendUpdateProcessAction();

                     }


                 });


             }
         });


     }




     function checkTypeControlRecoveryPlan(id)
     {

         $.ajax({
             type: "POST",
             data: { id: id },
             url: 'Masterdata.asmx/checkTypecontrolrecoveryplan',
             dataType: 'json',
             success: function (json) {


                 $.each(json, function (value, key) {

                     if (key.result == true) {
                         if (filename_recovery_plan != "" && filename_opinion_doctor != "") {
                             sendAddProcessAction();

                         } else {
                             var txt_alert = "";

                             if (filename_recovery_plan == "") {
                                 var require_file_recovery = '<%= Resources.Health.rqfile_recovery_plan %>';
                                 txt_alert = txt_alert + require_file_recovery;
                             }


                             if (filename_opinion_doctor == "") {
                                 var require_file_opinion = '<%= Resources.Health.rqfile_opinion_doctor %>';
                                 txt_alert = txt_alert + "\n" + require_file_opinion;
                             }

                             alert(txt_alert);
                             return false;
                         }


                     } else {
                         sendAddProcessAction();

                     }


                 });


             }
         });


     }






     function updateProcessAction()
     {
        var type_control = $("#MainContent_ddtypecontrol").val();
       // console.log(Page_ClientValidate("process"));

        if (Page_ClientValidate("process")) {

            var status = $("#MainContent_ddlStatus").val();

            if (status != "2")//close
            {

                checkTypeControlRecoveryPlanUpdate(type_control);


            } else {

                if (filename_process_action != "") {
                    checkTypeControlRecoveryPlanUpdate(type_control);

                } else {

                    var require_file_for_close = '<%= Resources.Health.rqfile_action_close %>';
                     alert(require_file_for_close);
                     return false;


                 }

             }





         }
         else {
             return false;
         }


     }


     function sendUpdateProcessAction()
     {
         showLoading();
         var type_control = $("#MainContent_ddtypecontrol").val();
         var action = $("#MainContent_txtaction").val();
         var responsible_person = $("#MainContent_txtresponsible_person").val();
         var due_date = $("#MainContent_txtdue_date").val();
         var status = $("#MainContent_ddlStatus").val();

         var remark = $("#MainContent_txtremark").val();
         var employee_id = $("#MainContent_employee_id").val();
         $.ajax({
             type: "POST",
             data: {
                 type_control: type_control,
                 action: action,
                 responsible_person: responsible_person,
                 employee_id: employee_id,
                 due_date: due_date,
                 status: status,
                 remark: remark,
                 filename_opinion_doctor: filename_opinion_doctor,
                 filename_recovery_plan: filename_recovery_plan,
                 filename_process_action: filename_process_action,
                 health_id: id,
                 id: process_action_id,
                 page_type: pagetype

             },
             url: 'Actionevent.asmx/updateProcessActionHealth',
             dataType: 'json',
             success: function (result) {
                 closeLoading();
                 closeProcessAction();
                 clearProcessAction();
                 callProcessAction();

                 dropzone_opinion_doctor.removeAllFiles();
                 dropzone_recovery_health.removeAllFiles();
                 dropzone_process_action_health.removeAllFiles();

             }
         });


     }








     function ShowEditProcessAction(processaction_id)
     {
        $("#MainContent_btCreateProcess").hide();
        $("#MainContent_btUpdateProcess").show();
       // $("#process_status").show();


        process_action_id = processaction_id;
        dialogProcessAction.dialog("open");
        $.ajax({
            type: "POST",
            data: { id: process_action_id, page_type: pagetype, lang: lang },
            url: 'Actionevent.asmx/getProcessActionHealthByID',
            dataType: 'json',
            success: function (json) {

                $.each(json, function (value, key) {
                    $("#MainContent_ddtypecontrol").val(key.type_control_id);
                    $("#MainContent_txtaction").val(key.action);
                    $("#MainContent_txtresponsible_person").val(key.responsible_person);

                    $("#MainContent_employee_id").val(key.employee_id);
                    $("#MainContent_txtdue_date").val(key.due_date);
                    $("#MainContent_txtremark").val(key.remark);
                    $("#MainContent_ddlStatus").val(key.action_status_id);

                    setTypeControlDropZone(key.type_control_id);

                    if (key.doctor_opinion_file != "") {
                        setShowEidtImage(key.doctor_opinion_file, dropzone_opinion_doctor);
                        filename_opinion_doctor = key.doctor_opinion_file;
                    }

                    if (key.recovery_plan_file != "") {
                        setShowEidtImage(key.recovery_plan_file, dropzone_recovery_health);
                        filename_recovery_plan = key.recovery_plan_file;
                    }

                    if (key.attachment_file != "") {
                        setShowEidtImage(key.attachment_file, dropzone_process_action_health);
                        filename_process_action = key.attachment_file;
                    }


                });



            }
        });



    }


   
     function setTypeControlDropZone(type_control_id)
     {
         $.ajax({
             type: "POST",
             data: { id: type_control_id },
             url: 'Masterdata.asmx/checkTypecontrolrecoveryplan',
             dataType: 'json',
             success: function (json) {


                 $.each(json, function (value, key) {

                     if (key.result == true) {
                         $("#show_opinion_doctor").show();
                         $("#show_recovery_plan").show();
                     } else {
                         $("#show_opinion_doctor").hide();
                         $("#show_recovery_plan").hide();

                     }


                 });


             }
         });
     }



    function callProcessAction() {

        dataTableProcessAction.ajax.url('Datatablelist.asmx/getListProcessActionHealth?health_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype).load();

    }





    function showCreateProcessAction()
    {
        $("#MainContent_btCreateProcess").show();
        $("#MainContent_btUpdateProcess").hide();
        //$("#process_status").hide();
        dialogProcessAction.dialog("open");

        return false;

    }

   


    function closeProcessAction()
    {
        dialogProcessAction.dialog("close");
        clearValidationErrors();
        clearProcessAction();
    }




    function clearValidationErrors(group)
    {
        var i;
        for (i = 0; i < Page_Validators.length; i++) {
            Page_Validators[i].style.display = "none";

        }

    }




    function clearProcessAction() {
        $("#MainContent_ddtypecontrol").val("");
        $("#MainContent_txtaction").val("");
        $("#MainContent_txtresponsible_person").val("");
        $("#MainContent_txtdue_date").val("");
        $("#MainContent_ddlStatus").val("");
        $("#MainContent_txtremark").val("");

        $("#MainContent_employee_id").val("");


        process_action_id = 0;
        filename_opinion_doctor = "";
        filename_recovery_plan = "";
        filename_process_action = "";
    }



   


    function setDatatableProcessAction()
    {

        dataTableProcessAction = $("#tbProcess_action").DataTable({
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


        dataTableProcessAction.ajax.url('Datatablelist.asmx/getListProcessActionHealth?health_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype);



    }



   



    function setDropzoneOpinionDoctor()
    {
             var reportdate = "";
             //alert(reportdate);
             //File Upload response from the server
             dropzone_opinion_doctor = Dropzone.options.dropzoneOpinionDoctor = {
                 maxFiles: 1,
                 maxFilesize: 10,
                 url: "dropzoneuploadhealth.aspx?user_id=" + user_login_id + "&reportdate=" + reportdate + "&id=" + id + "&type=opinion_doctor",
                 acceptedFiles: ".pdf,image/jpeg,image/png",
                 init: function () {
                     dropzone_opinion_doctor = this;
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

                                     filename_opinion_doctor = "";
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



                         document.querySelector("#MainContent_btCloseProcess").addEventListener("click", function () {

                             _this.removeAllFiles();

                         });

                         document.querySelector("#MainContent_btCreateProcessAction").addEventListener("click", function () {

                             _this.removeAllFiles();

                         });

                         //document.querySelector("#MainContent_btUpdateProcess").addEventListener("click", function () {

                         //    _this.removeAllFiles();

                         //});

                         // Add the button to the file preview element.
                         file.previewElement.appendChild(removeButton);

                         this.on("success", function (file, response) {
                             var obj = $.parseJSON(response);

                             filename_opinion_doctor = obj.name;
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













     function setDropzoneRecoveryPlan()
     {
            var reportdate = "";
             //alert(reportdate);
             //File Upload response from the server
             dropzone_recovery_health = Dropzone.options.dropzoneRecoveryPlan = {
                 maxFiles: 1,
                 maxFilesize: 10,
                 url: "dropzoneuploadhealth.aspx?user_id=" + user_login_id + "&reportdate=" + reportdate + "&id=" + id + "&type=recovery_plan",
                 acceptedFiles: ".pdf,image/jpeg,image/png",
                 init: function () {
                     dropzone_recovery_health = this;
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
                                     filename_recovery_plan = "";

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


                         document.querySelector("#MainContent_btCloseProcess").addEventListener("click", function () {

                             _this.removeAllFiles();

                         });

                         document.querySelector("#MainContent_btCreateProcessAction").addEventListener("click", function () {

                             _this.removeAllFiles();

                         });

                         //document.querySelector("#MainContent_btUpdateProcess").addEventListener("click", function () {

                         //    _this.removeAllFiles();

                         //});


                         // Add the button to the file preview element.
                         file.previewElement.appendChild(removeButton);

                         this.on("success", function (file, response) {
                             var obj = $.parseJSON(response);

                             folder_image = obj.folder;
                             filename_recovery_plan = obj.name;
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




     function setDropzoneProcessActionHealth()
     {
            var reportdate = "";
             //alert(reportdate);
             //File Upload response from the server
             dropzone_process_action_health = Dropzone.options.dropzoneProcessActionHealth = {
                 maxFiles: 1,
                 maxFilesize: 10,
                 url: "dropzoneuploadhealth.aspx?user_id=" + user_login_id + "&reportdate=" + reportdate + "&id=" + id + "&type=process_action_health",
                 acceptedFiles: ".pdf,image/jpeg,image/png",
                 init: function () {
                     dropzone_process_action_health = this;
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
                                     filename_process_action = "";

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


                         document.querySelector("#MainContent_btCloseProcess").addEventListener("click", function () {

                             _this.removeAllFiles();

                         });

                         document.querySelector("#MainContent_btCreateProcessAction").addEventListener("click", function () {

                             _this.removeAllFiles();

                         });

                         //document.querySelector("#MainContent_btUpdateProcess").addEventListener("click", function () {

                         //    _this.removeAllFiles();

                         //});

                         // Add the button to the file preview element.
                         file.previewElement.appendChild(removeButton);

                         this.on("success", function (file, response) {
                             var obj = $.parseJSON(response);

                             filename_process_action = obj.name;
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






     function setShowEidtImage(file_name, mydropzone)
     {

             var reportdate = "";
             $.ajax({
                 type: "POST",
                 data: { report_date: reportdate, file_name: file_name, user_id: user_login_id, id: id, lang: lang },
                 url: 'Actionevent.asmx/getImageHealth',
                 dataType: 'json',
                 success: function (json) {

                     //var existingFileCount = 0;
                     $.each(json, function (value, key) {

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
            data: { id: id, user_id: user_login_id, lang: lang },
            url: 'Actionevent.asmx/getHealthbyid',
            dataType: 'json',
            success: function (json) {

                $.each(json, function (value, key) {
                    //setCompany(key.company_id);
                  
                    $("#MainContent_txtemployee_id").val(key.health_employee_id);
                 
                    $("#MainContent_lbEmployee").text(key.name_modify);
                    $("#MainContent_lbUpdate").text(key.datetime_modify);



                    $("#show_doc_status").html(key.doc_no + ' ' + key.status);




                });




            }
        });

    }





     function updateHealth()
     {
    
            showLoading();
           
            $.ajax({
                type: "POST",
                data: {
                    userid: user_login_id,
                    typelogin: type_login,
                    health_id: id,
                    group_id: user_group_id,

                },
                url: 'Actionevent.asmx/updateHealth2',
                dataType: 'json',
                success: function (id) {

                    closeLoading();
                    var result_save = '<%= Resources.Main.success %>';
                    alert(result_save);

                    window.location.href = "healthform2.aspx?pagetype=view&id=" + id;



                }
            });
            return false;

  


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



     function setActionHealthStatus() {

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



 </script>


     <input type="hidden" id="employee_id" name="employee_id" runat="server">

       <div id="process_action_form" title="<%= Resources.Health.process_action_form %>">     
         
          	<div class="row">

                  <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"><%= Resources.Health.typecontrol %></label><div class="lbrequire"> *</div>
						 <select id="ddtypecontrol" class="form-control" runat="server">
                            
                        </select>
                        
                         <asp:RequiredFieldValidator ID="rqtypecontrl" runat="server" ControlToValidate ="ddtypecontrol" ErrorMessage="<%$ Resources:Health, rqtypecontrol %>" 
                             ValidationGroup="process" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				
                  
		   </div>

           <div class="row">
				<div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"><%= Resources.Health.action %></label><div class="lbrequire"> *</div>
						
                         <textarea class="form-control" rows="3" id="txtaction" runat="server"></textarea>
                        
                         <asp:RequiredFieldValidator ID="rqaction" runat="server" ControlToValidate ="txtaction" ErrorMessage="<%$ Resources:Health, rqaction %>" 
                             ValidationGroup="process" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
                  
		   </div>

          <div class="row">
                <div class="col-sm-8">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Health.responsible_person %></label><div class="lbrequire"> *</div>
					    <input id="txtresponsible_person" name="txtresponsible_person"  type="text" class="form-control" runat="server">
                        
                         <asp:RequiredFieldValidator ID="rqresponsible_person" runat="server" ControlToValidate ="txtresponsible_person" ErrorMessage="<%$ Resources:Health, rqresponsible_person %>" 
                             ValidationGroup="process" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				
				<div class="col-sm-4">
					<div id="due_date" class="form-group">
						<label class="control-label"><%= Resources.Health.due_date %></label><div class="lbrequire"> *</div>
						 <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtdue_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                        </div>
                        <asp:RequiredFieldValidator ID="rqduedate" runat="server" ValidationGroup="process" ControlToValidate ="txtdue_date" ErrorMessage="<%$ Resources:Health, rqduedate %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>	
                        
                         <asp:CustomValidator id="rqduedate2" runat="server"  ValidationGroup="process" ControlToValidate = "txtdue_date" ErrorMessage = "<%$ Resources:Health, rqduedate2 %>"  CssClass="text-danger"  Display="Dynamic"  ClientValidationFunction="validateDuedate" ValidateEmptyText="true" >
                        </asp:CustomValidator>			
                    </div>
				</div>

                 
		   </div>

             <div class="row">
				<div class="col-sm-12">
					<div class="form-group" id="process_status">
						<label class="control-label"><%= Resources.Health.status %></label><div class="lbrequire"> *</div>
						<select id="ddlStatus" class="form-control" runat="server">
                            
                        </select>
                        
                         <asp:CustomValidator id="rqprocessstatus" runat="server" ClientValidationFunction="checkValidateStatus" ValidateEmptyText="true" ValidationGroup="process" ControlToValidate="ddlStatus" Display="Dynamic" ErrorMessage="<%$ Resources:Health, rqprocessstatus %>" CssClass="text-danger"></asp:CustomValidator>

                    
					</div>
				</div>
                  
		   </div>


      

       <div class="row">
        <div class="col-sm-12">                
            <div class="form-group" id="show_opinion_doctor">
                    <label class="control-label"><%= Resources.Health.file_opinion_doctor %></label>                                
                    <div  class="dropzone" id="dropzoneOpinionDoctor" style="margin-top:8px;">
                        <div class="fallback">
                            <input name="filedoctoropinion" type="file"/>
                            <input type="submit" value="Upload" />
                        </div>
                    </div>
                          
            </div>
        </div>
      </div>

       <div class="row">
        <div class="col-sm-12">                
            <div class="form-group" id="show_recovery_plan">
                    <label class="control-label"><%= Resources.Health.file_recovery_plan %></label>                                
                    <div  class="dropzone" id="dropzoneRecoveryPlan" style="margin-top:8px;">
                        <div class="fallback">
                            <input name="filerecoveryplan" type="file"/>
                            <input type="submit" value="Upload" />
                        </div>
                    </div>
                          
            </div>
        </div>
      </div>


        <div class="row">
        <div class="col-sm-12">                
            <div class="form-group">
                     <label class="control-label"><%= Resources.Health.file_action_close %></label>                                   
                    <div  class="dropzone" id="dropzoneProcessActionHealth" style="margin-top:8px;">
                        <div class="fallback">
                            <input name="fileprocessactionhealth" type="file"/>
                            <input type="submit" value="Upload" />
                        </div>
                    </div>
                          
            </div>
        </div>
      </div>

 
        

          <div class="row">
                <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Health.remark %></label>
					     <textarea class="form-control" rows="3" id="txtremark" runat="server"></textarea>

					</div>
				</div>
				
                 
		   </div>



          

             <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                        <asp:Button ID="btCreateProcess" runat="server" ValidationGroup="process"  Text="<%$ Resources:Main, btadd %>" OnClientClick="addProcessAction();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btUpdateProcess" runat="server" ValidationGroup="process"  Text="<%$ Resources:Main, btsave %>" OnClientClick="updateProcessAction();" CssClass="btn btn-primary"/>
                        <button type="button" id="btCloseProcess" class="btn btn-default" runat="server" onclick="closeProcessAction();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>

      
    </div>









 
<div class="ibox float-e-margins">
                
 <div class="ibox-content" style="display: block;">

               
<div class="stepwizard">
      <div class="stepwizard-row setup-panel">
        <div class="stepwizard-step">
            <asp:LinkButton ID="step1" runat="server" CssClass="btn btn-default btn-circle" CausesValidation="False" OnClick="step1_Click">1</asp:LinkButton>
        <p><%= Resources.Health.healthstep1 %></p>
        </div>

          <div class="stepwizard-step">
            <asp:LinkButton ID="step2" runat="server" CssClass="btn btn-primary btn-circle a-step" CausesValidation="False" OnClick="step2_Click">2</asp:LinkButton>
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

                               
                          <%
                              string id = Request.QueryString["id"];
                              string user_id = Session["user_id"].ToString();

                              ArrayList per = Session["permission"] as ArrayList;
                             
                            //  bool pa = safetys4.Class.SafetyPermission.checkPermisionAction("report health1 request close", id, "health", Convert.ToInt32(Session["group_value"]));
                              bool ca = safetys4.Class.SafetyPermission.checkPermissionHealthCreator(id, user_id);  
                              bool area = safetys4.Class.SafetyPermission.checkPermisionInArea( id, "health");

                              if (per.IndexOf("report health2 request close") > -1 && area == true && ca==true)         
                           {                            
       
                          %>
                                  <asp:Button ID="btrequestclose" runat="server" Text="<%$ Resources:Health, btRequestclose %>" CssClass="btn btn-primary" CausesValidation="False" OnClick="btrequestclose_Click"/>
               
                          <%                      
                            }
       
                          %>
                         
                                             
                       
                    </div>
                    </div>
                  </div>
                <hr>
                 <div class="row">
                    <div class="col-md-12">
                    <div class="pull-right">

                        
                        <%
                            string PageType = Request.QueryString["PageType"];
                            string user_id2= Session["user_id"].ToString();
                            
                            string id2 = "";

                            if (PageType == "edit" || PageType == "view")
                            {
                                id2 = Request.QueryString["id"];
                            }
                            ArrayList per3 = Session["permission"] as ArrayList;
                           
                           // bool pa2 = safetys4.Class.SafetyPermission.checkPermisionAction("report health1 edit", id, "Health", Convert.ToInt32(Session["group_value"]));
                           bool ca2 = safetys4.Class.SafetyPermission.checkPermissionHealthCreator(id2, user_id2);  
                           bool area2 = safetys4.Class.SafetyPermission.checkPermisionInArea(id2, "health");
                           bool form3 = safetys4.Class.SafetyPermission.checkPermissionHealthForm3(id2);

                            if (per3.IndexOf("report health2 edit") > -1 && area2 == true && ca2 == true && form3 == true)         
                           {                            
       
                          %>
                              <asp:Button ID="btHealthedit" runat="server" Text="<%$ Resources:Health, btHealthedit %>" CssClass="btn btn-primary"  CausesValidation="False" OnClick="btHealthedit_Click"/>

                          <%                      
                            }
       
                          %>
                         </div>
                    </div>
                  </div>
                           
                
                

                 <div class="row">
                    <div class="col-md-12">
                          <strong><h3><%= Resources.Health.process_action_form %></h3></strong>
                       </div>
                              
                   </div>
     

                 <div  class="row">
                    <div class="col-md-12">
                          <table id="tbProcess_action" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                   <%-- <th> <%= Resources.Health.no %></th>--%>
                                    <th></th>
                                    <th> <%= Resources.Health.typecontrol %></th>
                                    <th> <%= Resources.Health.action %></th>
                                    <th> <%= Resources.Health.responsible_person %></th>
                                    <th> <%= Resources.Health.due_date %></th>
                                    <th> <%= Resources.Health.status %></th>
                                    <th> <%= Resources.Health.date_complete %></th>   
                                    <th> <%= Resources.Health.file_recovery_plan %></th>                           
                                    <th> <%= Resources.Health.file_opinion_doctor  %></th>                             
                                    <th> <%= Resources.Health.file_action_close %></th>
                                    <th> <%= Resources.Health.remark %></th>
                         
                                    <th> <%= Resources.Health.manage %></th>
                    
                                </tr>
                            </thead>
                            
                            </table>




                       </div>
                              
                   </div>

                <div  class="row">
                    <div class="col-md-12">
                       <button type="button" id="btCreateProcessAction" class="btn btn-primary" runat="server" onclick="showCreateProcessAction();"><i class="fa fa-plus"></i></button>  <%= Resources.Health.add_process_action %>
                       
                       </div>
                              
                   </div>

                <div  class=="row">
                    <div class="col-md-12">
                        <br />
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
                <asp:Button ID="btUpdate" runat="server" Text="<%$ Resources:Main, btSubmit %>"  CssClass="btn btn-primary" OnClientClick="return updateHealth();" />    
                 

               </div>
            </div>
              

            </div>
        </div>
    </div>
 








</asp:Content>
