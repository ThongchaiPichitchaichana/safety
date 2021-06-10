<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="myactionsot.aspx.cs" Inherits="safetys4.myactionsot" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="template/js/plugins/fancybox/jquery.fancybox.css" type="text/css" />
    <link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
    <link href="template/css/plugins/dropzone/dropzone.css" rel="stylesheet" />
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
         var dialog;
         var action_id = 0;
         var Sot_id = 0;
         var filename_action = "";
         var dropzoneAction;
         $(document).ready(function () {
           
           

             setDatatableList();
             setDropzoneAction();

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


             dataTable.ajax.url('Datatablelist.asmx/getListMyActionSot?employee_id=' + user_login_id + "&lang=" + lang);




         }

         function redirectSot(id)
         {
                 var url = "sotform.aspx?pagetype=view&id=" + id;
                 window.location.href = url;

         }


         function callDatatable()
         {
             dataTable.ajax.url('Datatablelist.asmx/getListMyActionSot?employee_id=' + user_login_id + "&lang=" + lang).load();


         }



         function setDropzoneAction()
         {
             dropzoneAction = Dropzone.options.dropzoneAction = {
                 maxFiles: 1,
                 maxFilesize: 5,
                 url: "dropzoneuploadactionsot.aspx",
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
                                 url: "dropzoneremoveactionsot.aspx",
                                 data: "folder=" + file.folderimage + "&name=" + file.newname,
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
                         this.options.url = "dropzoneuploadactionsot.aspx?id=" + Sot_id;
                     });


                     this.on("success", function (file, response) {
                         var obj = $.parseJSON(response);

                         filename_action = obj.name;

                         file.newname = obj.name;
                         file.folderimage = obj.folder;
                         //console.log(file.name);

                         setTimeout(function () {

                             var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/sot/" + "<%= Session["country"].ToString() %>" + "/action/" + obj.folder + "/" + obj.name;

                             $("img[alt='" + file.name + "']").wrap("<a class='fancybox' href='" + image_name + "'/>");

                             $('.fancybox').fancybox();

                         }, 1500);

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
                 url: 'Actionevent.asmx/updateMyactionAttachFileActionSot',
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
                             url: 'Actionevent.asmx/requestActionSot',
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


             function showAttachfile(id,Sotid)
             {
                 action_id = id;
                 Sot_id = Sotid;
                 dialog.dialog("open");

             }


             function CloseDialog()
             {
                 dialog.dialog("close");
            
             }




    </script>

    <div id="attachfile-form" title="<%= Resources.Sot.attachment_header %>">     
              
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
    <strong><h3><%= Resources.Sot.sot %></h3></strong>
    <br />
    <br />
      <table id="tbAction" class="table table-bordered table-hover" >
                 <thead>
                    <tr>
                        <th></th>
                        <th></th>
                        <th></th>
                        <th> <%= Resources.Sot.doc_no %></th>
                        <th> <%= Resources.Sot.action %></th>
                        <th> <%= Resources.Sot.due_date %></th>
                        <th> <%= Resources.Sot.status %></th>
                        <th> <%= Resources.Sot.date_complete %></th>
                        <th> <%= Resources.Sot.attachment %></th>
                        <th> <%= Resources.Sot.notify_contractor %></th>
                        <th> <%= Resources.Sot.close_action %></th>
                        <th> <%= Resources.Sot.remark %></th>
                        <th></th>
                    
                    
                    </tr>
                </thead>
         
        </table>


</asp:Content>
