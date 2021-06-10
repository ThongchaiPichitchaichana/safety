<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="myactionhazard.aspx.cs" Inherits="safetys4.myactionhazard" %>
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
         var dataTableWork; //reference to your dataTable
         var dialog;
         var dialogwork;
         var action_id = 0;
         var hazard_id = 0;
         var filename_action = "";
         var dropzoneAction;

         var function_id = "";
         var company_id = "";
         var department_id = "";
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

             $("input[type=radio][name='ctl00$MainContent$request_approve']").change(function () {
                 if (this.value == 'P') {
                     $("#hd_boxreason").hide();


                 }
                 else if (this.value == 'NP') {
                     $("#hd_boxreason").show();

                 }
             });



             setCompany(company_id, function_id, department_id);
             setDropzoneAction();


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

             dataTable.ajax.url('Datatablelist.asmx/getListMyActionHazard?employee_id='
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
                 //"serverSide": true,
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
                 },
                 "drawCallback": function (settings) {
                     closeLoading();
                 },

             });

             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();

             dataTableWork.ajax.url('Datatablelist.asmx/getAllWorkInMyActionHazard?employee_id='
                                    + user_login_id +
                                    '&company_id=' + ddl_company_id +
                                    '&function_id=' + ddl_function_id +
                                    '&department_id=' + ddl_department_id +
                                    '&doc_no=' + doc_no +
                                    "&lang=" + lang);




         }

         function redirecthazard(id)
         {
                 var url = "hazardform.aspx?pagetype=view&id=" + id;
                 window.location.href = url;

         }


         function callDatatable()
         {
             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var doc_no = $('#MainContent_txtdoc_no').val();

             dataTable.ajax.url('Datatablelist.asmx/getListMyActionHazard?employee_id='
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

             dataTableWork.ajax.url('Datatablelist.asmx/getAllWorkInMyActionHazard?employee_id='
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


             dataTableWork.ajax.url('Datatablelist.asmx/getAllWorkInMyActionHazard?employee_id='
                                    + user_login_id +
                                    '&company_id=' + ddl_company_id +
                                    '&function_id=' + ddl_function_id +
                                    '&department_id=' + ddl_department_id +
                                    '&doc_no=' + doc_no +
                                    "&lang=" + lang).load();

             dataTable.ajax.url('Datatablelist.asmx/getListMyActionHazard?employee_id='
                                + user_login_id +
                                '&company_id=' + ddl_company_id +
                                '&function_id=' + ddl_function_id +
                                '&department_id=' + ddl_department_id +
                                '&doc_no=' + doc_no +
                                "&lang=" + lang).load();




             return false;
         }


         function setDropzoneAction()
         {
             dropzoneAction =  Dropzone.options.dropzoneAction = {
                 maxFiles: 1,
                 maxFilesize: 5,
                 url: "dropzoneuploadaction.aspx",
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
                                 data: "folder=" + file.folderimage + "&type=hazard&name=" + file.newname,
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

                         document.querySelector("button#MainContent_btCloseAction").addEventListener("click", function () {

                             _this.removeAllFiles();

                         });

                         document.querySelector("#MainContent_btUpdateAction").addEventListener("click", function () {

                             _this.removeAllFiles();

                         });

                         // Add the button to the file preview element.
                         file.previewElement.appendChild(removeButton);
                     });

                     this.on("processing", function (file) {
                         this.options.url = "dropzoneuploadaction.aspx?id=" + hazard_id;
                     });
                    

                     this.on("success", function (file, response) {
                         var obj = $.parseJSON(response);

                         filename_action = obj.name;

                         file.newname = obj.name;
                         file.folderimage = obj.folder;
                         //console.log(file.name);

                         setTimeout(function () {

                             var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/hazard/step3/" + "<%= Session["country"].ToString()%>" + "/" + obj.folder + "/" + obj.name;

                             $("img[alt='" + file.name + "']").wrap("<a class='fancybox' href='" + image_name + "'/>");

                             $('.fancybox').fancybox();

                         }, 1000);
                     });

                 }

             };

         }

         function closeAll()
         {
             closeLoading();
             CloseDialog(); 
         }
      

         function updateProcessAction()
         {          
                 showLoading();
                 setTimeout(closeAll(), 3000);

         }


         function updateAttachFile()
         {

             $.ajax({
                 type: "POST",
                 data: {
                     attachment_file: filename_action,
                     id: action_id,

                 },
                 url: 'Actionevent.asmx/updateMyactionAttachFileAction',
                 dataType: 'json',
                 success: function (result) {
                     filename_action = "";
                     closeLoading();
                     //CloseDialog();
                     callDatatable();

                 }
             });

         }

             function closeAction(action_id)
         {
                 if (filename_action != "")
                 {
                     var message_confirm_close = '<%= Resources.Main.confirm_request_close_action %>';
                     if (confirm(message_confirm_close))
                     {
                         showLoading();
                         $.ajax({
                             type: "POST",
                             data: { id: action_id, type: "request close", remark: "" },
                             url: 'Actionevent.asmx/requestActionHazard',
                             dataType: 'json',
                             success: function (json) {
                                 updateAttachFile();
                                // closeLoading();
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


             function showAttachfile(id,hazardid)
             {
                 action_id = id;
                 hazard_id = hazardid;
                 dialog.dialog("open");

             }

             function showCloseWork(hazardid)
             {

                 hazard_id = hazardid;
                 $("#MainContent_request_approve1").prop('checked', true);//set default
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


             function updateHazard()
             {

                 var request_approve = $("input:radio[name='ctl00$MainContent$request_approve']:checked").val();
                 if (request_approve != undefined) {
                     showLoading();
                     // request_approve = "";

                     var reason = $("#MainContent_txtreason").val();

                     var data_post = JSON.stringify({
                         request_approve: request_approve,
                         reason: reason,
                         employee_id: user_login_id,
                         hazardid: hazard_id,
                         stepform: 4,
                         typelogin: type_login,
                         lang: lang,
                         group_id: user_group_id,
                     });

                     $.ajax({
                         type: "POST",
                         data: data_post,
                         url: 'Actionevent.asmx/createLogRequestCloseHazard',
                         contentType: "application/json; charset=utf-8",
                         dataType: "text",
                         success: function (result) {
                             var v = result.split("{");
                             if (v[0] != "") {
                                 closeLoading();
                                 alert(v[0]);

                             } else {

                                 callDatatableWork();

                                 CloseDialogWork();

                                 //window.location.href = "hazardform4.aspx?pagetype=view&id=" + id;
                             }

                         },
                         error: function (xhr) {
                             //  alert('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
                         }
                     });

                 } else {

                     var message = '<%= Resources.Hazard.consider_approval %>';
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
      <div id="approve-form" title="<%= Resources.Hazard.hazardstep4 %>">     
        <div  class="row" style="padding-bottom:10px;"> 
                  
                                <div class="col-md-12">
                                    <label class="control-label"><%= Resources.Hazard.request_approve %></label>
                                     <div  class="form-group">
                                        
                                         <div class="col-sm-6">
                                            <label> <input  value="P" id="request_approve1" name="request_approve" type="radio" runat="server">
                                            <%= Resources.Hazard.approve %> </label>
                                         </div>
                                         <div class="col-sm-6">
                                              <label> <input value="NP" id="request_approve2" name="request_approve" type="radio" runat="server">
                                            <%= Resources.Hazard.notapprove %> </label>
                                         </div>
                                        
                                    </div>

                                </div>

                    </div>

                
                    <div id="hd_boxreason" class="row">
                              
                        <div class="col-md-12">
                            <div class="form-group">
                            <label class="control-label"><%= Resources.Hazard.reason_not_approve %></label>
                                
                                <textarea class="form-control" rows="5" id="txtreason" runat="server"></textarea>      
                            </div>
                        </div>

                              				
                    </div>

            <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                         <asp:Button ID="btUpdate" runat="server" Text="<%$ Resources:Main, btSubmit%>"  CssClass="btn btn-primary" OnClientClick="return updateHazard();" />   
                        <button type="button" id="btCloseWork" class="btn btn-default" runat="server" onclick="CloseDialogWork();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>
      
    </div>



    <div id="attachfile-form" title="<%= Resources.Hazard.attachment_header %>">     
              
      <div class="row">
                  <div class="col-sm-12">
                      
                       <div class="form-group">
                                                    
                              <div  class="dropzone" id="dropzoneAction" style="margin-top:8px;">
                                    <div class="fallback">
                                        <input name="fileaction" type="file" />
                                       <input type="submit" value="Upload" />
                                    </div>
                                </div>
                          
                        </div>
                        </div>
                     </div>

          

             <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                        <asp:Button ID="btUpdateAction" runat="server" ValidationGroup="action"  Text="<%$ Resources:Main, btsave %>" OnClientClick="updateProcessAction();" CssClass="btn btn-primary"/>
                        <button type="button" id="btCloseAction" class="btn btn-default" runat="server" onclick="CloseDialog();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>

      
    </div>
    <strong><h3><%= Resources.Hazard.hazard %></h3></strong>
    <br />
    <br />

    
                       <div class="row" id="filter">
                                <div class="col-md-2">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Hazard.lbCompany %></label>                         
                                        <br />
                                       <select id="ddcompany" name="ddcompany" class="form-control" onchange="changCompany();" runat="server">
                       
                                        </select>
                                     
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Hazard.lbfucntion %></label>
                                    
                                     <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server">
                       
                                        </select>
                                  
                                    </div>
                                </div>

                                       <div class="col-md-3">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Hazard.lbdepartment %></label>                                              
                                    
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
                        <th> <%= Resources.Hazard.doc_no %></th>
                        <th> <%= Resources.Hazard.action %></th>
                        <th> <%= Resources.Hazard.due_date %></th>
                        <th> <%= Resources.Hazard.status %></th>
                        <th> <%= Resources.Hazard.date_complete %></th>
                        <th> <%= Resources.Hazard.attachment %></th>
                        <th> <%= Resources.Hazard.notify_contractor %></th>
                        <th> <%= Resources.Hazard.close_action %></th>
                        <th> <%= Resources.Hazard.remark %></th>
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
                    <th><%= Resources.Hazard.lbhazardno %></th>
                    <th><%= Resources.Hazard.lbhazardtitle %></th>
                    <th><%= Resources.Hazard.lbhazarddetail %></th>
                    <th><%= Resources.Hazard.lbhazarddate %></th>
                    <th><%= Resources.Hazard.lbstatus %></th>
                     <th></th>
                    
                    
                    </tr>
                </thead>
         
        </table>


</asp:Content>
