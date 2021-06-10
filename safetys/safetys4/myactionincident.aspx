<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="myactionincident.aspx.cs" Inherits="safetys4.myactionincident" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
    <link href="template/css/plugins/dropzone/dropzone.css" rel="stylesheet" />
    <link rel="stylesheet" href="template/js/plugins/fancybox/jquery.fancybox.css" type="text/css" />
    <script src="template/js/plugins/dataTables/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="template/js/plugins/dropzone/dropzone.min.js"></script>
    <script type="text/javascript" src="template/js/plugins/fancybox/jquery.fancybox.pack.js"></script>
  
    <style>
#tbAction tbody tr {
    cursor: pointer;
}

.row-gray{
    background-color:#e7e7e7 !important;
}

    a {
color:black ;
}


         
.ui-dialog-titlebar-close {
    visibility: hidden;
  }


    </style>

     <script>
         var dataTable; //reference to your dataTable
         var dataTablePreventive; //reference to your dataTable
         var dataTableConsequence; //reference to your dataTable
         var dataTableWork; //reference to your dataTable
         var dialog;
         var dialogwork;
         var action_id = 0;
         var incident_id = 0;
         var filename_corrective = "";
         var dropzoneAction;

         var function_id = "";
         var company_id = "";
         var department_id = "";
         var type_action = "";

         $(document).ready(function () {
 

             dialog = $("#attachfile-form").dialog({
                 autoOpen: false,
                 height: 370,
                 width: 700,
                 modal: true,
               
                 close: function () {
                     
                   
                     
                 },
                 open: function (event, ui) {
                   
                     $("#attachfile-form").css('overflow-x', 'hidden');
                 },
                 modal: true,
             });



             dialogwork = $("#approve-form").dialog({
                 autoOpen: false,
                 height: 370,
                 width: 560,
                 modal: true,

                 close: function () {



                 },
                 open: function (event, ui) {

                     $("#approve-form").css('overflow-x', 'hidden');
                 },
                 modal: true,
             });


             $("#hd_boxreason").hide();

             $("input[type=radio][name='ctl00$MainContent$request_close']").change(function () {
                 if (this.value == 'C') {
                     $("#hd_boxreason").hide();


                 }
                 else if (this.value == 'NC') {
                     $("#hd_boxreason").show();

                 }
             });

             setCompany(company_id, function_id, department_id);
             setDropzoneCorrective();
         });


    

         function setDatatableList()
         {


             dataTable = $("#tbAction").DataTable({
                 "bProcessing": true,
                 "sProcessing": true,
        
                 "bPaginate": true,
                 "bInfo": false,
                 "bFilter": false,
                 "ordering": false,
                 // "stateSave": true,
                 "responsive": true,
                 "pageLength": 20,
                 "lengthChange": false,
                 "order": [],
                 "language": {
                     "url": 'Langdatatable.aspx'
                 },
                 "columnDefs": [
                    {
                        "targets": [0],
                        "visible": false,
                    },
                     {
                         "targets": [1],
                         "visible": false,
                     },
                      {
                          "targets": [2],
                          "visible": false,
                      }
                 ],

                 "createdRow": function (row, data, index) {
                     
                     if (data[1] == "3" || data[1] == "5")//complete,cancel
                     {
                         //alert(data[1]);
                         $(row).addClass('row-gray');
                     }
                 }

             });


             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();

             dataTable.ajax.url('Datatablelist.asmx/getListMyActionIncident?employee_id='
                                + user_login_id +
                                '&company_id=' + ddl_company_id +
                                '&function_id=' + ddl_function_id +
                                '&department_id=' + ddl_department_id +
                                '&doc_no=' + doc_no +
                                "&lang=" + lang);


         }


         function setDatatablePreventiveList()
         {


             dataTablePreventive = $("#tbPreventiveAction").DataTable({
                 "bProcessing": true,
                 "sProcessing": true,

                 "bPaginate": true,
                 "bInfo": false,
                 "bFilter": false,
                 "ordering": false,
                 // "stateSave": true,
                 "responsive": true,
                 "pageLength": 20,
                 "lengthChange": false,
                 "order": [],
                 "language": {
                     "url": 'Langdatatable.aspx'
                 },
                 "columnDefs": [
                    {
                        "targets": [0],
                        "visible": false,
                    },
                     {
                         "targets": [1],
                         "visible": false,
                     },
                      {
                          "targets": [2],
                          "visible": false,
                      }
                 ],

                 "createdRow": function (row, data, index) {

                     if (data[1] == "3" || data[1] == "5")//complete,cancel
                     {
                         //alert(data[1]);
                         $(row).addClass('row-gray');
                     }
                 }

             });


             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();

             dataTablePreventive.ajax.url('Datatablelist.asmx/getListMyActionPreventiveIncident?employee_id='
                                + user_login_id +
                                '&company_id=' + ddl_company_id +
                                '&function_id=' + ddl_function_id +
                                '&department_id=' + ddl_department_id +
                                '&doc_no=' + doc_no +
                                "&lang=" + lang);


         }


         function setDatatableConsequenceList() {


             dataTableConsequence = $("#tbConsequenceAction").DataTable({
                 "bProcessing": true,
                 "sProcessing": true,

                 "bPaginate": true,
                 "bInfo": false,
                 "bFilter": false,
                 "ordering": false,
                 // "stateSave": true,
                 "responsive": true,
                 "pageLength": 20,
                 "lengthChange": false,
                 "order": [],
                 "language": {
                     "url": 'Langdatatable.aspx'
                 },
                 "columnDefs": [
                    {
                        "targets": [0],
                        "visible": false,
                    },
                     {
                         "targets": [1],
                         "visible": false,
                     },
                      {
                          "targets": [2],
                          "visible": false,
                      }
                 ],

                 "createdRow": function (row, data, index) {

                     if (data[1] == "3" || data[1] == "5")//complete,cancel
                     {
                         //alert(data[1]);
                         $(row).addClass('row-gray');
                     }
                 }

             });


             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();

             dataTableConsequence.ajax.url('Datatablelist.asmx/getListMyActionConsequenceIncident?employee_id='
                                + user_login_id +
                                '&company_id=' + ddl_company_id +
                                '&function_id=' + ddl_function_id +
                                '&department_id=' + ddl_department_id +
                                '&doc_no=' + doc_no +
                                "&lang=" + lang);


         }

         function setDatatableWork()
         {


             dataTableWork = $("#tbListwork").DataTable({
                 "bProcessing": true,
                 "sProcessing": true,

                 "bPaginate": true,
                 "bInfo": true,
                 "bFilter": false,
                 "ordering": false,
                 // "stateSave": true,
                 "responsive": true,
                 "pageLength": 20,
                 "lengthChange": false,
                 "order": [],
                 "language": {
                     "url": 'Langdatatable.aspx'
                 },
                 "columnDefs": [
                    {
                        "targets": [0],
                        "visible": false,
                    },
                     //{
                     //    "targets": [1],
                     //    "visible": false,
                     //},
                     // {
                     //     "targets": [2],
                     //     "visible": false,
                     // }
                 ],

                 "createdRow": function (row, data, index) {

                     //if (data[1] == "3" || data[1] == "5")//complete,cancel
                     //{
                     //    //alert(data[1]);
                     //    $(row).addClass('row-gray');
                     //}
                 }
                 , "drawCallback": function (settings) {
                     closeLoading();
                 },

             });

             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();

             dataTableWork.ajax.url('Datatablelist.asmx/getAllWorkInMyActionIncident?employee_id='
                                  + user_login_id +
                                  '&company_id=' + ddl_company_id +
                                  '&function_id=' + ddl_function_id +
                                  '&department_id=' + ddl_department_id +
                                  '&doc_no=' + doc_no +
                                  "&lang=" + lang);



         }


         function redirectincident(id)
         {
                 var url = "incidentform.aspx?pagetype=view&id=" + id;
                 window.location.href = url;

         }


         function callDatatable()
         {
             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();

             dataTable.ajax.url('Datatablelist.asmx/getListMyActionIncident?employee_id='
                                + user_login_id +
                                '&company_id=' + ddl_company_id +
                                '&function_id=' + ddl_function_id +
                                '&department_id=' + ddl_department_id +
                                '&doc_no=' + doc_no +
                                "&lang=" + lang).load();

         }


         function callPreventiveDatatable()
         {
             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();

             dataTablePreventive.ajax.url('Datatablelist.asmx/getListMyActionPreventiveIncident?employee_id='
                                + user_login_id +
                                '&company_id=' + ddl_company_id +
                                '&function_id=' + ddl_function_id +
                                '&department_id=' + ddl_department_id +
                                '&doc_no=' + doc_no +
                                "&lang=" + lang).load();

         }


         function callConsequenceDatatable()
         {
             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();

             dataTableConsequence.ajax.url('Datatablelist.asmx/getListMyActionConsequenceIncident?employee_id='
                                + user_login_id +
                                '&company_id=' + ddl_company_id +
                                '&function_id=' + ddl_function_id +
                                '&department_id=' + ddl_department_id +
                                '&doc_no=' + doc_no +
                                "&lang=" + lang).load();

         }

         function callDatatableWork()
         {
             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();

             dataTableWork.ajax.url('Datatablelist.asmx/getAllWorkInMyActionIncident?employee_id='
                                   + user_login_id +
                                   '&company_id=' + ddl_company_id +
                                   '&function_id=' + ddl_function_id +
                                   '&department_id=' + ddl_department_id +
                                   '&doc_no=' + doc_no +
                                   "&lang=" + lang).load();

         }



         function filterSearch()
         {
             showLoading();

             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();


             dataTableWork.ajax.url('Datatablelist.asmx/getAllWorkInMyActionIncident?employee_id='
                                    + user_login_id +
                                    '&company_id=' + ddl_company_id +
                                    '&function_id=' + ddl_function_id +
                                    '&department_id=' + ddl_department_id +
                                    '&doc_no=' + doc_no +
                                    "&lang=" + lang).load();

             dataTable.ajax.url('Datatablelist.asmx/getListMyActionIncident?employee_id='
                                + user_login_id +
                                '&company_id=' + ddl_company_id +
                                '&function_id=' + ddl_function_id +
                                '&department_id=' + ddl_department_id +
                                '&doc_no=' + doc_no +
                                "&lang=" + lang).load();


             dataTablePreventive.ajax.url('Datatablelist.asmx/getListMyActionPreventiveIncident?employee_id='
                               + user_login_id +
                               '&company_id=' + ddl_company_id +
                               '&function_id=' + ddl_function_id +
                               '&department_id=' + ddl_department_id +
                               '&doc_no=' + doc_no +
                               "&lang=" + lang).load();


             dataTableConsequence.ajax.url('Datatablelist.asmx/getListMyActionConsequenceIncident?employee_id='
                               + user_login_id +
                               '&company_id=' + ddl_company_id +
                               '&function_id=' + ddl_function_id +
                               '&department_id=' + ddl_department_id +
                               '&doc_no=' + doc_no +
                               "&lang=" + lang).load();




             return false;
         }


         function setDropzoneCorrective()
         {
             dropzoneAction =  Dropzone.options.dropzoneCorrective = {
                 maxFiles: 1,
                 maxFilesize: 5,
                 url: "dropzoneuploadcorrective.aspx",
                 acceptedFiles: "image/jpeg,image/png,.pdf,.docx",
                 init: function () {
                     this.on("maxfilesexceeded", function (data) {
                         var res = eval('(' + data.xhr.responseText + ')');

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
                                 url: "dropzoneremoveaction.aspx",
                                 data: "folder=" + file.folderimage + "&type=incident&name=" + file.newname,
                                 success: function (msg) {


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

                         document.querySelector("button#MainContent_btCloseCorrective").addEventListener("click", function () {

                             _this.removeAllFiles();

                         });

                         document.querySelector("#MainContent_btUpdateCorrective").addEventListener("click", function () {

                             _this.removeAllFiles();

                         });

                         // Add the button to the file preview element.
                         file.previewElement.appendChild(removeButton);
                     });

                     this.on("processing", function (file) {
                         this.options.url = "dropzoneuploadcorrective.aspx?id=" + incident_id +"&type_action=" + type_action;
                     });
                    

                     this.on("success", function (file, response) {
                         var obj = $.parseJSON(response);

                         filename_corrective = obj.name;

                         file.newname = obj.name;
                         file.folderimage = obj.folder;
                         //console.log(file.name);

                         setTimeout(function () {

                             var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/incident/step3/"+"<%= Session["country"].ToString()%>"+"/" + obj.folder + "/" + obj.name;

                             $("img[alt='" + file.name + "']").wrap("<a class='fancybox' href='" + image_name + "'/>");

                             $('.fancybox').fancybox();

                         }, 1000);


                         //alert(response);
                     
                     });

                 }

             };

         }
      

         function closeAll()
         {
             closeLoading();
             CloseDialog();
         }

         function updateCorrectivePreventive()
         {          
            showLoading();
            setTimeout(closeAll(), 3000);
               

         }


         function updateAttachFile()
         {

             $.ajax({
                 type: "POST",
                 data: {
                     attachment_file: filename_corrective,
                     id: action_id,
                     type_action:type_action,

                 },
                 url: 'Actionevent.asmx/updateMyactionAttachFile',
                 dataType: 'json',
                 success: function (result) {
                     filename_corrective = "";
                     closeLoading();
                     // CloseDialog();

                     if(type_action=="corrective")
                     {
                         callDatatable();

                     }else if(type_action=="preventive"){

                         callPreventiveDatatable();

                     } else if (type_action == "consequence") {

                         callConsequenceDatatable();

                     }
                    

                 }
             });


         }


         function closeAction(action_id)
         {

             if (filename_corrective != "")
             {
                  
                 var message_confirm_close = '<%= Resources.Main.confirm_request_close_action %>';
                 if (confirm(message_confirm_close))
                 {
                     showLoading();
                     $.ajax({
                         type: "POST",
                         data: { id: action_id, type: "request close", remark: "" ,type_action:type_action},
                         url: 'Actionevent.asmx/requestActionIncident',
                         dataType: 'json',
                         success: function (json) {
                             updateAttachFile();
                             //closeLoading();
                             //callDatatable();
                         }
                     });
                 }

             } else {

                 var message_confirm_upload = '<%= Resources.Main.confirm_upload_image_action %>';
                 alert(message_confirm_upload)

             }

             return false;
         }


         function showAttachfile(id,incidentid,type)
         {
             //alert(type_action);
             action_id = id;
             incident_id = incidentid;
             type_action = type;
             dialog.dialog("open");

         }

         function showCloseWork(incidentid)
         {
             incident_id = incidentid;
             $("#MainContent_request_close1").prop('checked', true);//set default
             dialogwork.dialog("open");

         }



         function CloseDialog()
         {
             dialog.dialog("close");
            
         }


         function CloseDialogWork()
         {
             dialogwork.dialog("close");

         }



         function updateIncident()
         {

             var request_close = $("input:radio[name='ctl00$MainContent$request_close']:checked").val();
             if (request_close != undefined) {
                 showLoading();
                 // request_close = "";


                 var reason = $("#MainContent_txtreason").val();

                 var data_post = JSON.stringify({
                     request_close: request_close,
                     reason: reason,
                     employee_id: user_login_id,
                     incidentid: incident_id,
                     stepform: 4,
                     typelogin: type_login,
                     lang: lang,
                     group_id: user_group_id,
                 });

                 $.ajax({
                     type: "POST",
                     data: data_post,
                     url: 'Actionevent.asmx/createLogRequestCloseIncident',
                     contentType: "application/json; charset=utf-8",
                     dataType: "text",
                     success: function (result) {
                         var v = result.split("{");
                         if (v[0] != "") {
                             closeLoading();
                             alert(v[0]);

                         } else {
                             //closeLoading();
                             CloseDialogWork();
                             callDatatableWork();
                         }

                     },
                     error: function (xhr) {
                         //  alert('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
                     }
                 });

             } else {

                 var message = '<%= Resources.Incident.request_close_form4 %>';
                 alert(message);
             }

             return false;


         }




         function setCompany(company_id, function_id, department_id)
         {

             $.ajax({
                 type: "POST",
                 data: { lang: lang },
                 url: 'Masterdata.asmx/getCompany',
                 dataType: 'json',
                 success: function (json) {

                     var all = '<%= Resources.Main.all %>';
                     var $el = $("#MainContent_ddcompany");
                     $el.empty(); // remove old options

                     $el.append($("<option></option>")
                                .attr("value", "").text(all));
                     $.each(json, function (value, key) {

                         $el.append($("<option></option>")
                                 .attr("value", key.id).text(key.name));
                     });

                     //$('#MainContent_ddcompany').val(company_id);
                     setFunction(company_id, function_id, department_id);

                 }
             });

         }



         function setFunction(company_id, function_id, department_id, division_id, section_id)
         {
             $.ajax({
                 type: "POST",
                 data: { company: company_id, lang: lang },
                 url: 'Masterdata.asmx/getFuctionByCompany',
                 dataType: 'json',
                 success: function (json) {

                     var all = '<%= Resources.Main.all %>';
                     var $el = $("#MainContent_ddfunction");
                     $el.empty(); // remove old options

                     $el.append($("<option></option>")
                                .attr("value", "").text(all));
                     $.each(json, function (value, key) {

                         $el.append($("<option></option>")
                                 .attr("value", key.id).text(key.name));
                     });

                     // $('#MainContent_ddfunction').val(function_id);
                     setDepartment(function_id, department_id);
                 }
             });



         }



         function setDepartment(function_id, department_id)
         {

             $.ajax({
                 type: "POST",
                 data: { function: function_id, lang: lang },
                 url: 'Masterdata.asmx/getDepartmentbyFunction',
                 dataType: 'json',
                 success: function (json) {

                     var all = '<%= Resources.Main.all %>';
                     var $el = $("#MainContent_dddepartment");
                     $el.empty(); // remove old options

                     $el.append($("<option></option>")
                                .attr("value", "").text(all));
                     $.each(json, function (value, key) {

                         $el.append($("<option></option>")
                                 .attr("value", key.id).text(key.name));
                     });


                     if (department_id != "") {
                         // $("#MainContent_dddepartment").val(department_id);

                     }

                     setDatatableList();
                     setDatatablePreventiveList();
                     setDatatableConsequenceList();
                     setDatatableWork();
                    

                 }
             });



         }




         function changCompany()
         {
             var ddl_company_id = $('#MainContent_ddcompany').val();

             $.ajax({
                 type: "POST",
                 data: { company: ddl_company_id, lang: lang },
                 url: 'Masterdata.asmx/getFuctionByCompany',
                 dataType: 'json',
                 success: function (json) {

                     var all = '<%= Resources.Main.all %>';
                     var $el = $("#MainContent_ddfunction");
                     $el.empty(); // remove old options

                     $el.append($("<option></option>")
                                .attr("value", "").text(all));
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
                 url: 'Masterdata.asmx/getDepartmentbyFunction',
                 dataType: 'json',
                 success: function (json) {

                     var all = '<%= Resources.Main.all %>';
                     var $el = $("#MainContent_dddepartment");
                     $el.empty(); // remove old options

                     $el.append($("<option></option>")
                                .attr("value", "").text(all));
                     $.each(json, function (value, key) {

                         $el.append($("<option></option>")
                                 .attr("value", key.id).text(key.name));
                     });


                 }
             });



         }


    </script>


     
    <div id="approve-form" title="<%= Resources.Incident.incidentstep4 %>">     
         <div  class="row" style="padding-bottom:10px;"> 
           
                                <div class="col-md-12">
                                    <label class="control-label"><%= Resources.Incident.request_close %></label>
                                     <div  class="form-group">
                                        
                                         <div class="col-sm-6">
                                            <label> <input  value="C" id="request_close1" name="request_close" type="radio" runat="server">
                                            <%= Resources.Incident.close %> </label>
                                         </div>
                                         <div class="col-sm-6">
                                              <label> <input value="NC" id="request_close2" name="request_close" type="radio" runat="server">
                                            <%= Resources.Incident.notclose %> </label>
                                         </div>
                                        
                                    </div>

                                </div>
  
                </div>

                
            <div id="hd_boxreason" class="row">
                              
                <div class="col-md-12">
                    <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.reason_not_close %></label>
                                
                        <textarea class="form-control" rows="5" id="txtreason" runat="server"></textarea>      
                    </div>
                </div>

                              				
            </div>

            <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                         <asp:Button ID="btUpdate" runat="server" Text="<%$ Resources:Main, btSubmit%>"  CssClass="btn btn-primary" OnClientClick="return updateIncident();" />   
                        <button type="button" id="btCloseWork" class="btn btn-default" runat="server" onclick="CloseDialogWork();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>
      
    </div>

    <div id="attachfile-form" title="<%= Resources.Incident.attachment_header %>">     
              
      <div class="row">
                  <div class="col-sm-12">
                      
                       <div class="form-group">
                                                    
                              <div  class="dropzone" id="dropzoneCorrective" style="margin-top:8px;">
                                    <div class="fallback">
                                        <input name="filecorrective" type="file" />
                                       <input type="submit" value="Upload" />
                                    </div>
                                </div>
                          
                        </div>
                        </div>
                     </div>

          

             <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                        <asp:Button ID="btUpdateCorrective" runat="server" ValidationGroup="corrective"  Text="<%$ Resources:Main, btsave %>" OnClientClick="updateCorrectivePreventive();" CssClass="btn btn-primary"/>
                        <button type="button" id="btCloseCorrective" class="btn btn-default" runat="server" onclick="CloseDialog();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>

      
    </div>
    <strong><h3><%= Resources.Incident.incident %></h3></strong>
    <br />
    <br />

    
    
                              <div class="row" id="filter">
                                <div class="col-md-2">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbCompany %></label>                         
                                        <br />
                                       <select id="ddcompany" name="ddcompany" class="form-control" onchange="changCompany();" runat="server">
                       
                                        </select>
                                     
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbfucntion %></label>
                                    
                                     <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server">
                       
                                        </select>
                                  
                                    </div>
                                </div>

                                       <div class="col-md-3">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbdepartment %></label>                                              
                                    
                                        <select id="dddepartment" class="form-control" onchange="changDepartment();" runat="server">
                       
                                        </select>
                                        
                                    </div>
                                </div>

                                   <div class="col-md-2">
                                    <div class="form-group">
                                         <label class="control-label"><%= Resources.Main.doc_no %></label>
                                        <div class="form-group">
                        
                                                <input class="form-control" value="" type="text" id="txtdoc_no" runat="server">                                   
                        
                                            </div>
                                          
                                            </div>
                                        </div>
                              


                                  <div class="col-md-2">
                                    <div class="form-group">
                                     <label class="control-label"></label>
                                        <div class="form-inline">
                                   
                                         <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterSearch();" CssClass="btn btn-primary"/>
                                       
                                            
                                        </div>
                                    </div>
                                </div>


                              
                            </div>
    <br />
    <br />
      <table id="tbAction" class="table table-bordered table-hover" >
                 <thead>
                    <tr>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th> <%= Resources.Incident.doc_no %></th>
                        <th> <%= Resources.Incident.tb_corrective_preventive %></th>
                        <th> <%= Resources.Incident.due_date %></th>
                        <th> <%= Resources.Incident.status %></th>
                        <th> <%= Resources.Incident.date_complete %></th>
                        <th> <%= Resources.Incident.attachment %></th>
                        <th> <%= Resources.Incident.notify_contractor %></th>
                        <th> <%= Resources.Incident.close_action %></th>
                        <th> <%= Resources.Incident.remark %></th>
                        <th></th>
                    
                    
                    </tr>
                </thead>
         
        </table>

  <br />
    <br />

    <table id="tbPreventiveAction" class="table table-bordered table-hover" >
                 <thead>
                    <tr>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th> <%= Resources.Incident.doc_no %></th>
                        <th> <%= Resources.Incident.tb_preventive %></th>
                        <th> <%= Resources.Incident.due_date %></th>
                        <th> <%= Resources.Incident.status %></th>
                        <th> <%= Resources.Incident.date_complete %></th>
                        <th> <%= Resources.Incident.attachment %></th>
                        <th> <%= Resources.Incident.notify_contractor %></th>
                        <th> <%= Resources.Incident.close_action %></th>
                        <th> <%= Resources.Incident.remark %></th>
                        <th></th>
                    
                    
                    </tr>
                </thead>
         
        </table>

  <br />
    <br />

    <table id="tbConsequenceAction" class="table table-bordered table-hover" >
                 <thead>
                    <tr>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th> <%= Resources.Incident.doc_no %></th>
                        <th> <%= Resources.Incident.tb_consequence %></th>
                        <th> <%= Resources.Incident.due_date %></th>
                        <th> <%= Resources.Incident.status %></th>
                        <th> <%= Resources.Incident.date_complete %></th>
                        <th> <%= Resources.Incident.attachment %></th>
                        <th> <%= Resources.Incident.notify_contractor %></th>
                        <th> <%= Resources.Incident.close_action %></th>
                        <th> <%= Resources.Incident.remark %></th>
                        <th></th>
                    
                    
                    </tr>
                </thead>
         
        </table>

  <br />
    <br />
      <table id="tbListwork" class="table table-bordered table-hover" >
                 <thead>
                    <tr>

                    <th></th>
                    <th><%= Resources.Incident.lbincidentno %></th>
                    <th><%= Resources.Incident.lbincidenttitle %></th>
                    <th><%= Resources.Incident.lbincidentdetail %></th>
                    <th><%= Resources.Incident.lbincidentdate %></th>
                    <th><%= Resources.Incident.lbstatus %></th>
                    <th></th>
                    
                    
                    </tr>
                </thead>
         
        </table>




</asp:Content>
