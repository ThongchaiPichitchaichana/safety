<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="Sotform.aspx.cs" Inherits="safetys4.Sotform" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
<link href="template/css/plugins/dropzone/dropzone.css" rel="stylesheet" />
<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">
<script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker-custom.js"></script>
<script type="text/javascript" src="template/js/plugins/datepicker/locales/bootstrap-datepicker.th.js"></script>
<link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
<link href="template/css/jquery-ui.css" rel="stylesheet">
<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">
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
<script type="text/javascript" src="template/js/jquery.mcautocomplete.js"></script>
<script type="text/javascript" src="template/js/plugins/multiautocomplete/jquery.tokeninput.js"></script>

<link rel="stylesheet" href="template/css/plugins/multiautocomplete/token-input.css" type="text/css" />
<script type="text/javascript" src="template/js/plugins/fancybox/jquery.fancybox.pack.js"></script>
<style>


hr {
   
    border-top-color: #DE2F13;
    border-top-width: 3px;
    margin-top: 5px !important;
    margin-bottom: 5px !important;

}

 hr2 {
    -moz-border-bottom-colors: none;
    -moz-border-left-colors: none;
    -moz-border-right-colors: none;
    -moz-border-top-colors: none;
    border-bottom-color: -moz-use-text-color;
    border-bottom-style: none;
    border-bottom-width: 0;
    border-image-outset: 0 0 0 0;
    border-image-repeat: stretch stretch;
    border-image-slice: 100% 100% 100% 100%;
    border-image-source: none;
    border-image-width: 1 1 1 1;
    border-left-color: -moz-use-text-color;
    border-left-style: none;
    border-left-width: 0;
    border-right-color: -moz-use-text-color;
    border-right-style: none;
    border-right-width: 0;
    border-top-color: #eeeeee;
    border-top-style: solid;
    border-top-width: 2px;
    margin-bottom: 20px;
    margin-top: 20px;
}
hr2 {
    box-sizing: content-box;
    height: 0;
}

hr2 {
    -moz-float-edge: margin-box;
    color: #808080;
    display: block;
    margin-block-end: 0.5em;
    margin-block-start: 0.5em;
    margin-inline-end: auto;
    margin-inline-start: auto;
}


.dz-max-files-reached {
        background-color: red;
    }



  .wrapper-content {
    margin-left: 0px !important;
} 


.ui-dialog-titlebar-close {
    visibility: hidden;
  }


.label-font{
   font-weight:normal;
}

strong{
    color: #DE2F13;
 }

</style>

 <script type="text/javascript">

     var id = "";
     var pagetype = "";

     var dialogProcessAction;

     var dataTableProcessAction; //reference to your dataTable

     var process_action_id = 0;

     var dialogReason;

     var action_reason_id = 0;


     var default_company_id = '<%=Session["company_id"]%>';
     var default_function_id = '<%=Session["function_id"]%>';
     var default_department_id = '<%=Session["department_id"]%>';
     var default_division_id = '<%=Session["division_id"]%>';
     var default_section_id = '<%=Session["section_id"]%>';

     var country = '<%=Session["country"]%>';

     var sotteam_employee = [];

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
            $("#dropzoneForm").hide();
            $("#infouploadimage").hide();

        } else if (pagetype == "edit") {


            setShowedit();
            setDropzone();
          

        } else {

            setAreaSelect("", "", "", "", "", "", "", "", "", "", "", "");
            setSotteamDefault();
            setDropzone();
            setTypeEmployment();
           // setSite("");
           // setCountry(country);

        
        }

               <%
                
                if (Session["lang"] != null)         
                {
                    if (Session["lang"] =="th")
                    {                  
                       %>
                        $('#data_sot_date .input-group.date').datepicker({
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

                            $('#data_sot_date .input-group.date').datepicker({
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
            height: 550,
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

        dialogReason = $("#create_reason_reject").dialog({
            autoOpen: false,
            height: 280,
            width: 400,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {

                $("#create_reason_reject").css('overflow-x', 'hidden');
            },
            modal: true,
        });


        $("#MainContent_txtresponsible_person").autocomplete({
            source: "Masterdata.asmx/getEmployeeautocompleteofaction",
            select: function (event, ui) {
                $("#MainContent_employee_id").val(ui.item.employee_id);

            }
        });

      

        $("#MainContent_txtnotyfy_contractor").autocomplete({
            source: "Masterdata.asmx/getContractorautocompletesot",
            select: function (event, ui) {
                $("#MainContent_contractor_id").val(ui.item.id);

            }
        });

        function customRenderItem(ul, item)
        {
            var t = '<span class="mcacCol1">' + item[0] + '</span><span class="mcacCol2">' + item[1] + '</span>',
				result = $('<li class="ui-menu-item" role="menuitem"></li>')
				.data('item.autocomplete', item)
				.append('<a class="ui-corner-all" style="position:relative;" tabindex="-1">' + t + '</a>')
				.appendTo(ul);

            return result;
        }

        // Sets up the multicolumn autocomplete widget.
        $("#MainContent_txtlocation").mcautocomplete({
            // These next two options are what this plugin adds to the autocomplete widget.
            showHeader: true,
            columns: [{
                name: '<%= Resources.Main.lbNameThArea %>',
                minWidth: '20%',
                valueField: 'area_th'
            },
            {
                name: '<%= Resources.Main.lbNameEnArea %>',
                minWidth: '20%',
                valueField: 'area_en'
            },
            {
                name: '<%= Resources.Incident.lbfucntion %>',
                minWidth: '30%',
                valueField: 'function'
            }],

            // Event handler for when a list item is selected.
            select: function (event, ui) {
                var area = $("#MainContent_txtlocation").val();

                if ((ui.item.area_th).indexOf(area) > -1) {
                    this.value = (ui.item ? ui.item.area_th : '');
                } else {

                    this.value = (ui.item ? ui.item.area_en : '');
                }


                setAreaSelect(ui.item.company_id, ui.item.function_id, ui.item.department_id,ui.item.division_id,"","","","","","","","");

                //$('#results').text(ui.item ? 'Selected: ' + ui.item.name + ', ' + ui.item.adminName1 + ', ' + ui.item.countryName : 'Nothing selected, input was ' + this.value);
                return false;
            },

            // The rest of the options are for configuring the ajax webservice call.
            minLength: 1,
            source: function (request, response) {
                $.ajax({
                    url: 'Masterdata.asmx/getArealist',
                    dataType: 'json',
                    data: {
                        area: request.term,
                        company_id: $("#MainContent_ddcompany").val(),
                        function_id: $("#MainContent_ddfunction").val(),
                        lang: lang
                    },
                    // The success event handler will display "No match found" if no items are returned.
                    success: function (data) {

                        var result;
                        if (data.length === 0) {
                            result = [{
                                label: 'No match found.'
                            }];
                        } else {
                            result = data;
                        }
                        response(data);
                    }

                });
            }
        });





        setDatatableProcessAction();
        setTypeControl();
       


    });



     function setDropzone()
     {
         var reportdate = $("#MainContent_txtreport_date").val();
         //alert(reportdate);
         //File Upload response from the server
         Dropzone.options.dropzoneForm = {
             maxFiles: 1,
             maxFilesize: 5,
             url: "dropzoneuploadsot.aspx?user_id=" + user_login_id + "&reportdate=" + reportdate + "&id=" + id,
           //  acceptedFiles: "image/jpeg,image/png",
             init: function () {
                 myDropzone = this;
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
                             url: "dropzoneremovesot.aspx",
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

                     // Add the button to the file preview element.
                     file.previewElement.appendChild(removeButton);

                     this.on("success", function (file, response) {
                         var obj = $.parseJSON(response);

                         file.newname = obj.name;
                         file.folderimage = obj.folder;
                         //console.log(file.name);

                         setTimeout(function () {

                             var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/sot/" + "<%=Session["country"].ToString()  %>"+"/" + obj.folder + "/" + obj.name;

                             $("img[alt='" + file.name + "']").wrap("<a class='fancybox' href='" + image_name + "'/>");

                             $('.fancybox').fancybox();

                         }, 1500);


                     });

                 });
             }
         };

     }




     function setShowImage(sot_id)
     {
         
         $.ajax({
             type: "POST",
             data: { id: sot_id },
             url: 'Actionevent.asmx/getImageSot',
             dataType: 'json',
             success: function (json) {

                 var html = "";
                 $.each(json, function (value, key) {

                     html = html + '<a href="' + key.path + '" class="fancybox" rel="gallery"><img src="' + key.path + '"  style="width:150px;height:120px;padding-left:5px;padding-right:5px;" runat="server"></a>';

                 });

                 $("#showimage").html(html);

                 $('.fancybox').fancybox();
             }
         });

     }


     function setShowEidtImage(sot_id)
     {

         $.ajax({
             type: "POST",
             data: { id: sot_id },
             url: 'Actionevent.asmx/getImageSot',
             dataType: 'json',
             success: function (json) {

                 //var existingFileCount = 0;
                 $.each(json, function (value, key) {

                     // Create the mock file:
                     var mockFile = { name: key.name, size: key.size, accepted: true, status: "success", newname: key.name, folderimage: key.folder };

                     // Call the default addedfile event handler
                     myDropzone.emit("addedfile", mockFile);

                     // And optionally show the thumbnail of the file:
                     myDropzone.emit("thumbnail", mockFile, key.path);
                     // myDropzone.createThumbnailFromUrl(file, "upload/hazard/15100028_20170607110927/IMG_20141205_154840.jpg", callback, crossOrigin);
                     myDropzone.emit("complete", mockFile);

                     // If you use the maxFiles option, make sure you adjust it to the
                     // correct amount:
                     // The number of files already uploaded
                     // existingFileCount = existingFileCount + 1;
                     myDropzone.files.push(mockFile);


                     var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/sot/" + "<%= Session["country"].ToString()  %>"+ "/" + key.folder + "/" + key.name;

                     $("img[alt='" + key.name + "']").wrap("<a class='fancybox' href='" + image_name + "'/>");



                 });

                 // myDropzone.options.maxFiles = myDropzone.options.maxFiles - existingFileCount;



                 $('.fancybox').fancybox();

             }
         });

     }




     function manageAction()
     {
         if (pagetype == "add") {
             
             saveSot();

         } else if (pagetype == "edit") {

             updateSot('');

         }

         return false;
     }

     function setAreaSelect(company_id, function_id, department_id, division_id,
                            location_company_name_th, location_function_name_th, location_department_name_th, location_division_name_th,
                            location_company_name_en, location_function_name_en, location_department_name_en, location_division_name_en)
     {


         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getCompany',
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

            

                 if (company_id == "")
                 {
                     $('#MainContent_ddcompany').val(default_company_id);

                     setFunction(default_company_id, default_function_id, default_department_id,"",
                                location_function_name_th, location_department_name_th, location_division_name_th,
                                location_function_name_en, location_department_name_en, location_division_name_en);


                 } else {


                     if (pagetype == "view")
                     {
                         // alert(location_company_name_en);
                         if (lang == "th") {
                             $('#MainContent_ddcompany').append($('<option></option>').val("old").html(location_company_name_th));
                         } else {
                             $('#MainContent_ddcompany').append($('<option></option>').val("old").html(location_company_name_en));
                         }

                         $('#MainContent_ddcompany').val("old");
                        
                     } else {
                         
                         $('#MainContent_ddcompany').val(company_id);

                     }
                  
                     setFunction(company_id, function_id, department_id, division_id,
                                   location_function_name_th, location_department_name_th, location_division_name_th,
                                   location_function_name_en, location_department_name_en, location_division_name_en);



                 }

             }
         });




     }

     function setFunction(company_id, function_id, department_id, division_id,
                          location_function_name_th, location_department_name_th, location_division_name_th,
                          location_function_name_en, location_department_name_en, location_division_name_en)
     {
         $.ajax({
             type: "POST",
             data: { company: company_id, lang: lang },
             url: 'Masterdata.asmx/getFuctionByCompany',
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

                 if (pagetype == "view") {
                     if (lang == "th") {
                         $('#MainContent_ddfunction').append($('<option></option>').val("old").html(location_function_name_th));
                     } else {
                         $('#MainContent_ddfunction').append($('<option></option>').val("old").html(location_function_name_en));
                     }

                     $('#MainContent_ddfunction').val("old");

                 } else {
                     $('#MainContent_ddfunction').val(function_id);
                 }


                 setDepartment(function_id, department_id, division_id,
                               location_department_name_th, location_division_name_th,
                               location_department_name_en, location_division_name_en);

             }
         });



     }



     function setDepartment(function_id, department_id, division_id,
                            location_department_name_th, location_division_name_th,
                            location_department_name_en, location_division_name_en)
     {

         $.ajax({
             type: "POST",
             data: { function: function_id, lang: lang },
             url: 'Masterdata.asmx/getDepartmentbyFunction',
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

                     setDivision(department_id, division_id,location_division_name_th,location_division_name_en);

                 } else {

                     if (department_id == "" || department_id == "00000000") {
                         $('#MainContent_dddepartment').val(function_id + "F");
                         setDivision(function_id + "F", division_id,"","");

                     } else {
                         $('#MainContent_dddepartment').val(department_id);
                         setDivision(department_id, division_id,location_division_name_th,location_division_name_en);
                     }


                 }

               

             }
         });



     }

     function setSite(site_id)
     {

         $.ajax({
             type: "POST",
             data: {  lang: lang },
             url: 'Masterdata.asmx/getSitelist',
             dataType: 'json',
             success: function (json) {

                 var $el = $("#MainContent_ddsite");
                 $el.empty(); // remove old options

                 $el.append($("<option></option>")
                            .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });

                

                 $('#MainContent_ddsite').val(site_id);
                   
                 
             }
         });



     }


     function setCountry(country_id)
     {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getCountry',
             dataType: 'json',
             success: function (json) {

                 var $el = $("#MainContent_ddcountry");
                 $el.empty(); // remove old options

                 $el.append($("<option></option>")
                            .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });



                 $('#MainContent_ddcountry').val(country_id);


             }
         });



     }



     function setDivision(department_id, division_id,location_division_name_th,location_division_name_en)
     {
         //alert(division_id);
         $.ajax({
             type: "POST",
             data: { department: department_id, lang: lang },
             url: 'Masterdata.asmx/getDivisionbyDepartment',
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

                 
                 if (pagetype == "view")
                 {

                     if (lang == "th")
                     {
                         $('#MainContent_dddivision').append($('<option></option>').val("old").html(location_division_name_th));

                     } else {
                         $('#MainContent_dddivision').append($('<option></option>').val("old").html(location_division_name_en));
                     }

                     $('#MainContent_dddivision').val("old");
                   

                 } else {

                     $('#MainContent_dddivision').val(division_id);

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
             url: 'Masterdata.asmx/getFuctionByCompany',
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
             url: 'Masterdata.asmx/getDepartmentbyFunction',
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

     function closeProcessAction()
     {
         dialogProcessAction.dialog("close");
         clearValidationErrors();
         clearProcessAction();
     }

     function clearProcessAction()
     {
         $("#MainContent_ddtypecontrol").val("");
         $("#MainContent_txtaction").val("");
         $("#MainContent_txtresponsible_person").val("");
         $("#MainContent_txtdue_date").val("");
         // $("#MainContent_txtdate_complete").val("");
         $("#MainContent_txtnotyfy_contractor").val("");
         $("#MainContent_txtremark").val("");

         $("#MainContent_employee_id").val("");
         $("#MainContent_contractor_id").val("");

         process_action_id = 0;

     }



     function clearValidationErrors()
     {

         var i;
         for (i = 0; i < Page_Validators.length; i++) {

             Page_Validators[i].style.display = "none";

         }
     }



     function showCreateProcessAction()
     {
         $("#MainContent_btCreateProcess").show();
         $("#MainContent_btUpdateProcess").hide();
         dialogProcessAction.dialog("open");

         return false;

     }

     function addProcessAction()
     {

         if (Page_ClientValidate("process"))
         {
             showLoading();
             var type_control = $("#MainContent_ddtypecontrol").val();
             var action = $("#MainContent_txtaction").val();
             var responsible_person = $("#MainContent_txtresponsible_person").val();
             var due_date = $("#MainContent_txtdue_date").val();
             // var date_complete = $("#MainContent_txtdate_complete").val();
             var notyfy_contractor = $("#MainContent_txtnotyfy_contractor").val();
             var remark = $("#MainContent_txtremark").val();

             var employee_id = $("#MainContent_employee_id").val();
             var contractor_id = $("#MainContent_contractor_id").val();
             //  var root_cause_action = $("#MainContent_txtRootcauseaction").val();
             $.ajax({
                 type: "POST",
                 data: {
                     type_control: type_control,
                     action: action,
                     responsible_person: responsible_person,
                     due_date: due_date,
                     date_complete: "",
                     notify_contractor: notyfy_contractor,
                     remark: remark,
                     attachment_file: "",
                     employee_id: employee_id,
                     contractor_id: contractor_id,
                     // root_cause_action: root_cause_action,
                     user_id: user_login_id,
                     sot_id: id,
                     page_type:pagetype

                 },
                 url: 'Actionevent.asmx/createProcessActionSot',
                 dataType: 'json',
                 success: function (result) {
                     console.log(result);
                     closeLoading();
                     closeProcessAction();
                     clearProcessAction();
                     callProcessAction();

                 },
                 error: function (xhr, ajaxOptions, thrownError) {
                     alert(xhr.status);
                     alert(thrownError);
                 }
             });


         }
         else {
             return false;
         }


     }




     function updateProcessAction()
     {

         if (Page_ClientValidate("process"))
         {
             showLoading();
             var type_control = $("#MainContent_ddtypecontrol").val();
             var action = $("#MainContent_txtaction").val();
             var responsible_person = $("#MainContent_txtresponsible_person").val();
             var due_date = $("#MainContent_txtdue_date").val();
             // var date_complete = $("#MainContent_txtdate_complete").val();
             var notyfy_contractor = $("#MainContent_txtnotyfy_contractor").val();
             var remark = $("#MainContent_txtremark").val();

             var employee_id = $("#MainContent_employee_id").val();
             var contractor_id = $("#MainContent_contractor_id").val();
             //var root_cause_action = $("#MainContent_txtRootcauseaction").val();
             $.ajax({
                 type: "POST",
                 data: {
                     type_control: type_control,
                     action: action,
                     responsible_person: responsible_person,
                     due_date: due_date,
                     date_complete: "",
                     notify_contractor: notyfy_contractor,
                     remark: remark,
                     attachment_file: "",
                     employee_id: employee_id,
                     contractor_id: contractor_id,
                     //  root_cause_action: root_cause_action,
                     sot_id: id,
                     id: process_action_id,
                     page_type: pagetype,

                 },
                 url: 'Actionevent.asmx/updateProcessActionSot',
                 dataType: 'json',
                 success: function (result) {
                     closeLoading();
                     closeProcessAction();
                     clearProcessAction();
                     callProcessAction();

                 }
             });


         }
         else {
             return false;
         }


     }




     function ShowEditProcessAction(processaction_id)
     {
         $("#MainContent_btCreateProcess").hide();
         $("#MainContent_btUpdateProcess").show();


         <%
                
              
     if (Session["group_id"].ToString() == "2" || Session["group_id"].ToString() == "3" || Session["group_id"].ToString() == "8")//super admin and delegate and group
        {                  
            %>
         $("#due_date").show();

         <%                      
            }
            else { 
       
            %>
         $("#due_date").hide();
         <%     
     
            }     

            %>
         process_action_id = processaction_id;
         dialogProcessAction.dialog("open");
         $.ajax({
             type: "POST",
             data: { id: process_action_id,page_type: pagetype, lang: lang },
             url: 'Actionevent.asmx/getProcessActionSotByID',
             dataType: 'json',
             success: function (json) {

                 $.each(json, function (value, key) {
                     $("#MainContent_ddtypecontrol").val(key.type_control);
                     $("#MainContent_txtaction").val(key.action);
                     $("#MainContent_txtresponsible_person").val(key.responsible_person);


                     $("#MainContent_txtdue_date").val(key.due_date);
                     // $("#MainContent_txtdate_complete").val(key.date_complete);
                     $("#MainContent_txtnotyfy_contractor").val(key.notify_contractor);
                     $("#MainContent_txtremark").val(key.remark);

                     $("#MainContent_employee_id").val(key.employee_id);
                     $("#MainContent_contractor_id").val(key.contractor_id);
                     $("#MainContent_txtRootcauseaction").val(key.root_cause_action);

                 });



             }
         });



     }

     function callProcessAction()
     {

         dataTableProcessAction.ajax.url('Datatablelist.asmx/getListProcessActionSot?sot_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype).load();

     }


     function setDatatableProcessAction()
     {

         dataTableProcessAction = $("#tbProcess_action").DataTable({
             "bProcessing": true,
             "sProcessing": true,

             "bPaginate": false,
             "bInfo": false,
             "bFilter": false,
             "ordering": true,
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


         dataTableProcessAction.ajax.url('Datatablelist.asmx/getListProcessActionSot?sot_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype);



     }


     function closeAction(action_id)
     {
         var message_confirm_close = '<%= Resources.Main.confirm_close_action %>';

         $("#dialog-confirm").text(message_confirm_close);
         $("#dialog-confirm").dialog({
             resizable: false,
             height: "auto",
             width: 400,
             modal: true,
             buttons: {
                 '<%= Resources.Main.btOK2 %>': function () {

                     showLoading();
                     $.ajax({
                         type: "POST",
                         data: { id: action_id, type: "close", remark: "" },
                         url: 'Actionevent.asmx/requestActionSot',
                         dataType: 'json',
                         success: function (json) {
                             closeLoading();
                             callProcessAction();
                         }
                     });

                     $(this).dialog("close");
                 },
                 '<%= Resources.Main.btCancel %>': function () {
                     $(this).dialog("close");
                 }
             }
         });

         return false;
     }


     function rejectAction(action_id)
     {

         action_reason_id = action_id;
         dialogReason.dialog("open");

         return false;
     }


     function cancelAction(action_id)
     {
         var message_confirm_cancel = '<%= Resources.Main.confirm_cancel_action %>';
         $("#dialog-confirm").text(message_confirm_cancel);
         $("#dialog-confirm").dialog({
             resizable: false,
             height: "auto",
             width: 400,
             modal: true,
             buttons: {
                 '<%= Resources.Main.btOK2 %>': function () {

                     showLoading();
                     $.ajax({
                         type: "POST",
                         data: { id: action_id, type: "cancel", remark: "" },
                         url: 'Actionevent.asmx/requestActionSot',
                         dataType: 'json',
                         success: function (json) {
                             closeLoading();
                             dialogReason.dialog("close");
                             callProcessAction();
                         }
                     });

                     $(this).dialog("close");
                 },
                 '<%= Resources.Main.btCancel %>': function () {
                     $(this).dialog("close");
                 }
             }
         });


         return false;

     }


     function UpdateReason()
     {
         var reason = $("#MainContent_txtreasonreject").val();
         showLoading();
         $.ajax({
             type: "POST",
             data: { id: action_reason_id, type: "reject", remark: reason },
             url: 'Actionevent.asmx/requestActionSot',
             dataType: 'json',
             success: function (json) {
                 closeLoading();
                 action_reason_id = 0;
                 callProcessAction();
                 $("#MainContent_txtreasonreject").val("");
                 dialogReason.dialog("close");
             }
         });

     }


     function CloseReasonReject()
     {
         dialogReason.dialog("close");
         $("#MainContent_txtreasonreject").val("");

     }



     function setTypeControl()
     {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getTypecontrol',
             dataType: 'json',
             success: function (json) {

                 var $el = $("#MainContent_ddtypecontrol");
                 $el.empty(); // remove old options

                 $el.append($("<option></option>")
                            .attr("value", "").text(""));
                 $.each(json, function (value, key) {

                     $el.append($("<option></option>")
                             .attr("value", key.id).text(key.name));
                 });

                 //$("#MainContent_ddfatality").val(type_control_id);
             }
         });



     }


     function clearProcessAction()
     {
         $("#MainContent_ddtypecontrol").val("");
         $("#MainContent_txtaction").val("");
         $("#MainContent_txtresponsible_person").val("");
         $("#MainContent_txtdue_date").val("");
         // $("#MainContent_txtdate_complete").val("");
         $("#MainContent_txtnotyfy_contractor").val("");
         $("#MainContent_txtremark").val("");

         $("#MainContent_employee_id").val("");
         $("#MainContent_contractor_id").val("");

         corrective_id = 0;
         filename_corrective = "";
     }



  
    function setShowedit()
    {
        setTypeEmployment();
        $.ajax({
            type: "POST",
            data: { id: id, user_id: user_login_id,pagetype:pagetype, lang: lang },
            url: 'Actionevent.asmx/getSotbyid',
            dataType: 'json',
            success: function (json) {
              //  alert("tes");
                var sotteam = [];
                //console.log(json);
                $.each(json, function (value, key) {
                    //setCompany(key.company_id);
                    //setAreaSelect(key.company_id, key.function_id, key.department_id, key.division_id);
                    setAreaSelect(key.company_id, key.function_id, key.department_id, key.division_id,
                                  key.location_company_name_th, key.location_function_name_th, key.location_department_name_th, key.location_division_name_th,
                                  key.location_company_name_en, key.location_function_name_en, key.location_department_name_en, key.location_division_name_en);
                    
                    if (key.process_status == "2")
                    {
                        $("#MainContent_btSubmit").hide();
                        $("#MainContent_btSotformedit").hide();

                    }
                  //  setSite(key.site_id);
                   // setCountry(key.country_id);
                    
                    $("#MainContent_txtsot_date").val(key.sot_date);

                    $("#MainContent_txtreport_date").val(key.report_date);

                    $("#MainContent_ddtimehour").val(key.sot_time);                 
                   // $("#MainContent_ddtimeminute").val(key.sot_minute);

                    $("#MainContent_ddtimehour2").val(key.sot_time_end);
                    //$("#MainContent_ddtimeminute2").val(key.sot_minute_end);

                    $("#MainContent_txtlocation").val(key.location);
                    $("#MainContent_txttypework").val(key.type_work);
                    $("#MainContent_txtcomment").val(key.comment);

                    $("#MainContent_ddlTypeemployment").val(key.type_employment_id);
                    
                    $("#MainContent_lbEmployee").text(key.name_modify);
                    $("#MainContent_lbUpdate").text(key.datetime_modify);

                  


                    if (key.changinge_position != null) {
                        $("input[name='ctl00$MainContent$changing_position'][value='" + key.changinge_position + "']").attr('checked', 'checked');

                    }

                    if (key.stopping_work != null) {
                        $("input[name='ctl00$MainContent$stopping_work'][value='" + key.stopping_work + "']").attr('checked', 'checked');

                    }

                    if (key.rearranging_job != null) {
                        $("input[name='ctl00$MainContent$rearranging_job'][value='" + key.rearranging_job + "']").attr('checked', 'checked');

                    }

                    if (key.hiding_dodging != null) {
                        $("input[name='ctl00$MainContent$hiding_dodging'][value='" + key.hiding_dodging + "']").attr('checked', 'checked');

                    }

                    if (key.changing_tools != null) {
                        $("input[name='ctl00$MainContent$changing_tools'][value='" + key.changing_tools + "']").attr('checked', 'checked');

                    }

                    if (key.applying_lockout != null) {
                        $("input[name='ctl00$MainContent$applying_lockout'][value='" + key.applying_lockout + "']").attr('checked', 'checked');

                    }

                    if (key.adjusting_ppe != null) {
                        $("input[name='ctl00$MainContent$adjusting_ppe'][value='" + key.adjusting_ppe + "']").attr('checked', 'checked');

                    }








                    if (key.striking_against != null) {
                        $("input[name='ctl00$MainContent$striking_against'][value='" + key.striking_against + "']").attr('checked', 'checked');

                    }

                    if (key.caught_between != null) {
                        $("input[name='ctl00$MainContent$caught_between'][value='" + key.caught_between + "']").attr('checked', 'checked');

                    }

                    if (key.inhaling != null) {
                        $("input[name='ctl00$MainContent$inhaling'][value='" + key.inhaling + "']").attr('checked', 'checked');

                    }

                    if (key.absorbing != null) {
                        $("input[name='ctl00$MainContent$absorbing'][value='" + key.absorbing + "']").attr('checked', 'checked');

                    }

                    if (key.electricity != null) {
                        $("input[name='ctl00$MainContent$electricity'][value='" + key.electricity + "']").attr('checked', 'checked');

                    }

                    if (key.falling != null) {
                        $("input[name='ctl00$MainContent$falling'][value='" + key.falling + "']").attr('checked', 'checked');

                    }

                    if (key.struck_by != null) {
                        $("input[name='ctl00$MainContent$struck_by'][value='" + key.struck_by + "']").attr('checked', 'checked');

                    }


                    if (key.line_fire != null) {
                        $("input[name='ctl00$MainContent$line_fire'][value='" + key.line_fire + "']").attr('checked', 'checked');

                    }

                    if (key.eyes_tasks != null) {
                        $("input[name='ctl00$MainContent$eyes_tasks'][value='" + key.eyes_tasks + "']").attr('checked', 'checked');

                    }

                    if (key.lifting_lowering != null) {
                        $("input[name='ctl00$MainContent$lifting_lowering'][value='" + key.lifting_lowering + "']").attr('checked', 'checked');

                    }

                    if (key.posture != null) {
                        $("input[name='ctl00$MainContent$posture'][value='" + key.posture + "']").attr('checked', 'checked');

                    }






                    if (key.head != null) {
                        $("input[name='ctl00$MainContent$head'][value='" + key.head + "']").attr('checked', 'checked');

                    }

                    if (key.ears_eyes != null) {
                        $("input[name='ctl00$MainContent$ears_eyes'][value='" + key.ears_eyes + "']").attr('checked', 'checked');

                    }

                    if (key.face_respiratory != null) {
                        $("input[name='ctl00$MainContent$face_respiratory'][value='" + key.face_respiratory + "']").attr('checked', 'checked');

                    }

                    if (key.hand_arms != null) {
                        $("input[name='ctl00$MainContent$hands_arms'][value='" + key.hand_arms + "']").attr('checked', 'checked');

                    }

                    if (key.feet_legs != null) {
                        $("input[name='ctl00$MainContent$feet_legs'][value='" + key.feet_legs + "']").attr('checked', 'checked');

                    }







                    if (key.right_job != null) {
                        $("input[name='ctl00$MainContent$right_job'][value='" + key.right_job + "']").attr('checked', 'checked');

                    }

                    if (key.used_correctly != null) {
                        $("input[name='ctl00$MainContent$used_correctly'][value='" + key.used_correctly + "']").attr('checked', 'checked');

                    }

                    if (key.safe_conditions != null) {
                        $("input[name='ctl00$MainContent$in_safe_conditions'][value='" + key.safe_conditions + "']").attr('checked', 'checked');

                    }

                    if (key.hamesses != null) {
                        $("input[name='ctl00$MainContent$hamesses'][value='" + key.hamesses + "']").attr('checked', 'checked');

                    }

                    if (key.barricades_warning_lights != null) {
                        $("input[name='ctl00$MainContent$barricade_warning_lights'][value='" + key.barricades_warning_lights + "']").attr('checked', 'checked');

                    }

                    if (key.chock_restraints != null) {
                        $("input[name='ctl00$MainContent$chocks_restraints'][value='" + key.chock_restraints + "']").attr('checked', 'checked');

                    }

                    if (key.prejob_safety_checks != null) {
                        $("input[name='ctl00$MainContent$pre_job_safe_checks'][value='" + key.prejob_safety_checks + "']").attr('checked', 'checked');

                    }







                    if (key.standard_adequate_job != null) {
                        $("input[name='ctl00$MainContent$standard_adequate_job'][value='" + key.standard_adequate_job + "']").attr('checked', 'checked');

                    }

                    if (key.standard_established != null) {
                        $("input[name='ctl00$MainContent$standard_established'][value='" + key.standard_established + "']").attr('checked', 'checked');

                    }

                    if (key.standard_maintained != null) {
                        $("input[name='ctl00$MainContent$standard_being_maintained'][value='" + key.standard_maintained + "']").attr('checked', 'checked');

                    }

                    if (key.isolation_lockout != null) {
                        $("input[name='ctl00$MainContent$isolation_lockout'][value='" + key.isolation_lockout + "']").attr('checked', 'checked');

                    }

                    if (key.hot_work_permit != null) {
                        $("input[name='ctl00$MainContent$hot_work_permit'][value='" + key.hot_work_permit + "']").attr('checked', 'checked');

                    }

                    if (key.confined_space_permit != null) {
                        $("input[name='ctl00$MainContent$confined_space_permit'][value='" + key.confined_space_permit + "']").attr('checked', 'checked');

                    }

                    if (key.electrical_permit != null) {
                        $("input[name='ctl00$MainContent$electrical_permit'][value='" + key.electrical_permit + "']").attr('checked', 'checked');

                    }

                    if (key.work_height_permit != null) {
                        $("input[name='ctl00$MainContent$work_height_permit'][value='" + key.work_height_permit + "']").attr('checked', 'checked');

                    }

                    if (key.rescue_plan_place != null) {
                        $("input[name='ctl00$MainContent$rescue_plan_place'][value='" + key.rescue_plan_place + "']").attr('checked', 'checked');

                    }





                    if (key.standards_established_understood != null) {
                        $("input[name='ctl00$MainContent$standards_established_understood'][value='" + key.standards_established_understood + "']").attr('checked', 'checked');

                    }

                    if (key.walkway_passageways != null) {
                        $("input[name='ctl00$MainContent$walkway_passageways'][value='" + key.walkway_passageways + "']").attr('checked', 'checked');

                    }

                    if (key.disorganized_tools_bench != null) {
                        $("input[name='ctl00$MainContent$disorganized_tools_bench'][value='" + key.disorganized_tools_bench + "']").attr('checked', 'checked');

                    }

                    if (key.materials_storage != null) {
                        $("input[name='ctl00$MainContent$materials_storage'][value='" + key.materials_storage + "']").attr('checked', 'checked');

                    }

                    if (key.obstructions_leaning_items != null) {
                        $("input[name='ctl00$MainContent$obstruction_leaning_items'][value='" + key.obstructions_leaning_items + "']").attr('checked', 'checked');

                    }

                    if (key.stairs_platforms != null) {
                        $("input[name='ctl00$MainContent$stairs_platforms'][value='" + key.stairs_platforms + "']").attr('checked', 'checked');

                    }






                    if (key.housekeeping != null) {
                        $("input[name='ctl00$MainContent$housekeeping'][value='" + key.housekeeping + "']").attr('checked', 'checked');

                    }

                    if (key.chemical_storage != null) {
                        $("input[name='ctl00$MainContent$chemical_storage'][value='" + key.chemical_storage + "']").attr('checked', 'checked');

                    }

                    if (key.waste_diposal != null) {
                        $("input[name='ctl00$MainContent$waste_diposal'][value='" + key.waste_diposal + "']").attr('checked', 'checked');

                    }

                    if (key.walking_working_surface != null) {
                        $("input[name='ctl00$MainContent$walking_working_surface'][value='" + key.walking_working_surface + "']").attr('checked', 'checked');

                    }


                    $("#MainContent_txtreactions_people").val(key.reactions_people);
                    $("#MainContent_txtpostion_people").val(key.postion_people);
                    $("#MainContent_txtpersonal_protection_equipment").val(key.personal_protection_equipment);
                    $("#MainContent_txttools_equipment").val(key.tools_equipment);
                    $("#MainContent_txtprocedures").val(key.procedures);
                    $("#MainContent_txtorderliness_tidiness").val(key.orderliness_tidiness);
                    $("#MainContent_txtenvironment").val(key.environment);


                    $("#show_doc_status").html(key.doc_no + ' ' + key.status);

                    var json_sotteam = JSON.parse(key.sotteam);
                    //console.log(json_sotteam);
                    $("#MainContent_txtsotteam").tokenInput("Masterdata.asmx/getEmployeeautocompletesot", {
                        prePopulate: json_sotteam,
                        preventDuplicates: true
                    });
                });


                if (pagetype == "view") {
                    setShowImage(id);

                } else if (pagetype == "edit") {

                    setShowEidtImage(id);
                }


            }
        });

    }


    function setSotteamDefault()
    {

        $.ajax({
            type: "POST",
            data: { user_id: user_login_id,lang: lang },
            url: 'Actionevent.asmx/getEmployeeDefault',
            dataType: 'json',
            success: function (json) {

                $("#MainContent_txtsotteam").tokenInput("Masterdata.asmx/getEmployeeautocompletesot", {
                    prePopulate: json,
                    preventDuplicates: true
                });

                
            }
        });

    }






    function updateSot(type_save)
    {
        

        if (Page_ClientValidate("main"))
        {
           // alert("tdd");
            showLoading();
            var sot_date = $("#MainContent_txtsot_date").val();

            var location = $("#MainContent_txtlocation").val();
            var typework = $("#MainContent_txttypework").val();
            var comment = $("#MainContent_txtcomment").val();

            var hour = $("#MainContent_ddtimehour").val();
            var minute = $("#MainContent_ddtimeminute").val();
            var sot_time = hour;

            var hour2 = $("#MainContent_ddtimehour2").val();
            var minute2 = $("#MainContent_ddtimeminute2").val();
            var sot_time_end = hour2;

            var country_id = country;//$("#MainContent_ddcountry").val();
            var company_id = $("#MainContent_ddcompany").val();
            var function_id = $("#MainContent_ddfunction").val();
            var department_id = $("#MainContent_dddepartment").val();
            var division_id = $("#MainContent_dddivision").val();
            var site_id = "";// $("#MainContent_ddsite").val();

            var type_employment = $("#MainContent_ddlTypeemployment").val();

            var sotteam = $('#MainContent_txtsotteam').tokenInput("get");
            sotteam_employee = [];
            for (var key in sotteam) {
                // console.log(dd[key].id);
                sotteam_employee.push(sotteam[key].id);
            }





            var changing_position = $("input:radio[name='ctl00$MainContent$changing_position']:checked").val();
            if (changing_position == undefined) {
                changing_position = "";
            }

            var stopping_work = $("input:radio[name='ctl00$MainContent$stopping_work']:checked").val();
            if (stopping_work == undefined) {
                stopping_work = "";
            }

            var rearranging_job = $("input:radio[name='ctl00$MainContent$rearranging_job']:checked").val();
            if (rearranging_job == undefined) {
                rearranging_job = "";
            }

            var hiding_dodging = $("input:radio[name='ctl00$MainContent$hiding_dodging']:checked").val();
            if (hiding_dodging == undefined) {
                hiding_dodging = "";
            }

            var changing_tools = $("input:radio[name='ctl00$MainContent$changing_tools']:checked").val();
            if (changing_tools == undefined) {
                changing_tools = "";
            }

            var applying_lockout = $("input:radio[name='ctl00$MainContent$applying_lockout']:checked").val();
            if (applying_lockout == undefined) {
                applying_lockout = "";
            }

            var adjusting_ppe = $("input:radio[name='ctl00$MainContent$adjusting_ppe']:checked").val();
            if (adjusting_ppe == undefined) {
                adjusting_ppe = "";
            }

            var applying_lockout = $("input:radio[name='ctl00$MainContent$applying_lockout']:checked").val();
            if (applying_lockout == undefined) {
                applying_lockout = "";
            }




            var striking_against = $("input:radio[name='ctl00$MainContent$striking_against']:checked").val();
            if (striking_against == undefined) {
                striking_against = "";
            }

            var caught_between = $("input:radio[name='ctl00$MainContent$caught_between']:checked").val();
            if (caught_between == undefined) {
                caught_between = "";
            }

            var inhaling = $("input:radio[name='ctl00$MainContent$inhaling']:checked").val();
            if (inhaling == undefined) {
                inhaling = "";
            }

            var absorbing = $("input:radio[name='ctl00$MainContent$absorbing']:checked").val();
            if (absorbing == undefined) {
                absorbing = "";
            }

            var electricity = $("input:radio[name='ctl00$MainContent$electricity']:checked").val();
            if (electricity == undefined) {
                electricity = "";
            }

            var falling = $("input:radio[name='ctl00$MainContent$falling']:checked").val();
            if (falling == undefined) {
                falling = "";
            }

            var struck_by = $("input:radio[name='ctl00$MainContent$struck_by']:checked").val();
            if (struck_by == undefined) {
                struck_by = "";
            }

            var line_fire = $("input:radio[name='ctl00$MainContent$line_fire']:checked").val();
            if (line_fire == undefined) {
                line_fire = "";
            }

            var eyes_tasks = $("input:radio[name='ctl00$MainContent$eyes_tasks']:checked").val();
            if (eyes_tasks == undefined) {
                eyes_tasks = "";
            }

            var lifting_lowering = $("input:radio[name='ctl00$MainContent$lifting_lowering']:checked").val();
            if (lifting_lowering == undefined) {
                lifting_lowering = "";
            }

            var posture = $("input:radio[name='ctl00$MainContent$posture']:checked").val();
            if (posture == undefined) {
                posture = "";
            }






            var head = $("input:radio[name='ctl00$MainContent$head']:checked").val();
            if (head == undefined) {
                head = "";
            }

            var ears_eyes = $("input:radio[name='ctl00$MainContent$ears_eyes']:checked").val();
            if (ears_eyes == undefined) {
                ears_eyes = "";
            }

            var face_respiratory = $("input:radio[name='ctl00$MainContent$face_respiratory']:checked").val();
            if (face_respiratory == undefined) {
                face_respiratory = "";
            }

            var hands_arms = $("input:radio[name='ctl00$MainContent$hands_arms']:checked").val();
            if (hands_arms == undefined) {
                hands_arms = "";
            }

            var feet_legs = $("input:radio[name='ctl00$MainContent$feet_legs']:checked").val();
            if (feet_legs == undefined) {
                feet_legs = "";
            }



            var right_job = $("input:radio[name='ctl00$MainContent$right_job']:checked").val();
            if (right_job == undefined) {
                right_job = "";
            }

            var used_correctly = $("input:radio[name='ctl00$MainContent$used_correctly']:checked").val();
            if (used_correctly == undefined) {
                used_correctly = "";
            }

            var in_safe_conditions = $("input:radio[name='ctl00$MainContent$in_safe_conditions']:checked").val();
            if (in_safe_conditions == undefined) {
                in_safe_conditions = "";
            }

            var hamesses = $("input:radio[name='ctl00$MainContent$hamesses']:checked").val();
            if (hamesses == undefined) {
                hamesses = "";
            }

            var barricade_warning_lights = $("input:radio[name='ctl00$MainContent$barricade_warning_lights']:checked").val();
            if (barricade_warning_lights == undefined) {
                barricade_warning_lights = "";
            }

            var chocks_restraints = $("input:radio[name='ctl00$MainContent$chocks_restraints']:checked").val();
            if (chocks_restraints == undefined) {
                chocks_restraints = "";
            }

            var pre_job_safe_checks = $("input:radio[name='ctl00$MainContent$pre_job_safe_checks']:checked").val();
            if (pre_job_safe_checks == undefined) {
                pre_job_safe_checks = "";
            }




            var standard_adequate_job = $("input:radio[name='ctl00$MainContent$standard_adequate_job']:checked").val();
            if (standard_adequate_job == undefined) {
                standard_adequate_job = "";
            }

            var standard_established = $("input:radio[name='ctl00$MainContent$standard_established']:checked").val();
            if (standard_established == undefined) {
                standard_established = "";
            }

            var standard_being_maintained = $("input:radio[name='ctl00$MainContent$standard_being_maintained']:checked").val();
            if (standard_being_maintained == undefined) {
                standard_being_maintained = "";
            }

            var isolation_lockout = $("input:radio[name='ctl00$MainContent$isolation_lockout']:checked").val();
            if (isolation_lockout == undefined) {
                isolation_lockout = "";
            }

            var hot_work_permit = $("input:radio[name='ctl00$MainContent$hot_work_permit']:checked").val();
            if (hot_work_permit == undefined) {
                hot_work_permit = "";
            }

            var confined_space_permit = $("input:radio[name='ctl00$MainContent$confined_space_permit']:checked").val();
            if (confined_space_permit == undefined) {
                confined_space_permit = "";
            }

            var electrical_permit = $("input:radio[name='ctl00$MainContent$electrical_permit']:checked").val();
            if (electrical_permit == undefined) {
                electrical_permit = "";
            }

            var work_height_permit = $("input:radio[name='ctl00$MainContent$work_height_permit']:checked").val();
            if (work_height_permit == undefined) {
                work_height_permit = "";
            }

            var rescue_plan_place = $("input:radio[name='ctl00$MainContent$rescue_plan_place']:checked").val();
            if (rescue_plan_place == undefined) {
                rescue_plan_place = "";
            }



            var standards_established_understood = $("input:radio[name='ctl00$MainContent$standards_established_understood']:checked").val();
            if (standards_established_understood == undefined) {
                standards_established_understood = "";
            }

            var walkway_passageways = $("input:radio[name='ctl00$MainContent$walkway_passageways']:checked").val();
            if (walkway_passageways == undefined) {
                walkway_passageways = "";
            }

            var disorganized_tools_bench = $("input:radio[name='ctl00$MainContent$disorganized_tools_bench']:checked").val();
            if (disorganized_tools_bench == undefined) {
                disorganized_tools_bench = "";
            }

            var materials_storage = $("input:radio[name='ctl00$MainContent$materials_storage']:checked").val();
            if (materials_storage == undefined) {
                materials_storage = "";
            }

            var obstruction_leaning_items = $("input:radio[name='ctl00$MainContent$obstruction_leaning_items']:checked").val();
            if (obstruction_leaning_items == undefined) {
                obstruction_leaning_items = "";
            }

            var stairs_platforms = $("input:radio[name='ctl00$MainContent$stairs_platforms']:checked").val();
            if (stairs_platforms == undefined) {
                stairs_platforms = "";
            }





            var housekeeping = $("input:radio[name='ctl00$MainContent$housekeeping']:checked").val();
            if (housekeeping == undefined) {
                housekeeping = "";
            }

            var chemical_storage = $("input:radio[name='ctl00$MainContent$chemical_storage']:checked").val();
            if (chemical_storage == undefined) {
                chemical_storage = "";
            }

            var waste_diposal = $("input:radio[name='ctl00$MainContent$waste_diposal']:checked").val();
            if (waste_diposal == undefined) {
                waste_diposal = "";
            }

            var walking_working_surface = $("input:radio[name='ctl00$MainContent$walking_working_surface']:checked").val();
            if (walking_working_surface == undefined) {
                walking_working_surface = "";
            }

            var reactions_people = $("#MainContent_txtreactions_people").val();
            var postion_people = $("#MainContent_txtpostion_people").val();
            var personal_protection_equipment = $("#MainContent_txtpersonal_protection_equipment").val();
            var tools_equipment = $("#MainContent_txttools_equipment").val();
            var procedures = $("#MainContent_txtprocedures").val();
            var orderliness_tidiness = $("#MainContent_txtorderliness_tidiness").val();
            var environment = $("#MainContent_txtenvironment").val();
           
            var data_post = JSON.stringify({
                country_id: country_id,
                company_id: company_id,
                function_id: function_id,
                department_id: department_id,
                division_id: division_id,
                site_id: site_id,
                sot_date: sot_date,
                sot_time: sot_time,
                sot_time_end: sot_time_end,
                location: location,
                typework: typework,
                comment: comment,
                sotteam: sotteam_employee,
                user_id: user_login_id,
                typelogin: type_login,
                sotid: id,
                group_id: user_group_id,
                type: type_save,
                type_employment: type_employment,

                changing_position: changing_position,
                stopping_work: stopping_work,
                rearranging_job: rearranging_job,
                hiding_dodging: hiding_dodging,
                changing_tools: changing_tools,
                applying_lockout: applying_lockout,
                adjusting_ppe: adjusting_ppe,

                striking_against: striking_against,
                caught_between: caught_between,
                inhaling: inhaling,
                absorbing: absorbing,
                electricity: electricity,
                falling: falling,
                struck_by: struck_by,
                line_fire: line_fire,
                eyes_tasks: eyes_tasks,
                lifting_lowering: lifting_lowering,
                posture: posture,

                head: head,
                ears_eyes: ears_eyes,
                face_respiratory: face_respiratory,
                hands_arms: hands_arms,
                feet_legs: feet_legs,

                right_job: right_job,
                used_correctly: used_correctly,
                in_safe_conditions: in_safe_conditions,
                hamesses: hamesses,
                barricade_warning_lights: barricade_warning_lights,
                chocks_restraints: chocks_restraints,
                pre_job_safe_checks: pre_job_safe_checks,

                standard_adequate_job: standard_adequate_job,
                standard_established: standard_established,
                standard_being_maintained: standard_being_maintained,
                isolation_lockout: isolation_lockout,
                hot_work_permit: hot_work_permit,
                confined_space_permit: confined_space_permit,
                electrical_permit: electrical_permit,
                work_height_permit: work_height_permit,
                rescue_plan_place: rescue_plan_place,

                standards_established_understood: standards_established_understood,
                walkway_passageways: walkway_passageways,
                disorganized_tools_bench: disorganized_tools_bench,
                materials_storage: materials_storage,
                obstruction_leaning_items: obstruction_leaning_items,
                stairs_platforms: stairs_platforms,

                housekeeping: housekeeping,
                chemical_storage: chemical_storage,
                waste_diposal: waste_diposal,
                walking_working_surface: walking_working_surface,

                reactions_people: reactions_people,
                postion_people: postion_people,
                personal_protection_equipment: personal_protection_equipment,
                tools_equipment: tools_equipment,
                procedures: procedures,
                orderliness_tidiness: orderliness_tidiness,
                environment: environment,


            });


                $.ajax({
                    type: "POST",
                    data: data_post,
                    url: 'Actionevent.asmx/updateSot',
                    contentType: "application/json; charset=utf-8",
                    success: function (id) {
                      
                        window.location.href = "sotform.aspx?pagetype=view&id=" + id;
                    }
                });

                return false;

            } else {

            
                return false;

            }

    }

 
     function saveSot()
     {
        
         if (Page_ClientValidate("main"))
         {
             showLoading();
             var sot_date = $("#MainContent_txtsot_date").val();
             var report_date = $("#MainContent_txtreport_date").val();

             var location = $("#MainContent_txtlocation").val();
             var typework = $("#MainContent_txttypework").val();
             var comment =  $("#MainContent_txtcomment").val();

             var hour = $("#MainContent_ddtimehour").val();
             var minute = $("#MainContent_ddtimeminute").val();
             var sot_time = hour;

             var hour2 = $("#MainContent_ddtimehour2").val();
             var minute2 = $("#MainContent_ddtimeminute2").val();
             var sot_time_end = hour2;

             var country_id = country;//$("#MainContent_ddcountry").val();
             var company_id = $("#MainContent_ddcompany").val();
             var function_id = $("#MainContent_ddfunction").val();
             var department_id = $("#MainContent_dddepartment").val();
             var division_id = $("#MainContent_dddivision").val();
             var site_id = "";//$("#MainContent_ddsite").val();

             var type_employment = $("#MainContent_ddlTypeemployment").val();
            // var createdate = $("#MainContent_create_at").val();

             //alert(type_employment);

             var sotteam = $('#MainContent_txtsotteam').tokenInput("get");
             sotteam_employee = [];
             for (var key in sotteam)
             {
                 // console.log(dd[key].id);
                 sotteam_employee.push(sotteam[key].id);
             }




             var changing_position = $("input:radio[name='ctl00$MainContent$changing_position']:checked").val();
             if (changing_position == undefined)
             {
                 changing_position = "";
             }

             var stopping_work = $("input:radio[name='ctl00$MainContent$stopping_work']:checked").val();
             if (stopping_work == undefined) {
                 stopping_work = "";
             }

             var rearranging_job = $("input:radio[name='ctl00$MainContent$rearranging_job']:checked").val();
             if (rearranging_job == undefined) {
                 rearranging_job = "";
             }

             var hiding_dodging = $("input:radio[name='ctl00$MainContent$hiding_dodging']:checked").val();
             if (hiding_dodging == undefined) {
                 hiding_dodging = "";
             }

             var changing_tools = $("input:radio[name='ctl00$MainContent$changing_tools']:checked").val();
             if (changing_tools == undefined) {
                 changing_tools = "";
             }

             var applying_lockout = $("input:radio[name='ctl00$MainContent$applying_lockout']:checked").val();
             if (applying_lockout == undefined) {
                 applying_lockout = "";
             }

             var adjusting_ppe = $("input:radio[name='ctl00$MainContent$adjusting_ppe']:checked").val();
             if (adjusting_ppe == undefined) {
                 adjusting_ppe = "";
             }

             var applying_lockout = $("input:radio[name='ctl00$MainContent$applying_lockout']:checked").val();
             if (applying_lockout == undefined) {
                 applying_lockout = "";
             }




             var striking_against = $("input:radio[name='ctl00$MainContent$striking_against']:checked").val();
             if (striking_against == undefined) {
                 striking_against = "";
             }

             var caught_between = $("input:radio[name='ctl00$MainContent$caught_between']:checked").val();
             if (caught_between == undefined) {
                 caught_between = "";
             }

             var inhaling = $("input:radio[name='ctl00$MainContent$inhaling']:checked").val();
             if (inhaling == undefined) {
                 inhaling = "";
             }

             var absorbing = $("input:radio[name='ctl00$MainContent$absorbing']:checked").val();
             if (absorbing == undefined) {
                 absorbing = "";
             }

             var electricity = $("input:radio[name='ctl00$MainContent$electricity']:checked").val();
             if (electricity == undefined) {
                 electricity = "";
             }

             var falling = $("input:radio[name='ctl00$MainContent$falling']:checked").val();
             if (falling == undefined) {
                 falling = "";
             }

             var struck_by = $("input:radio[name='ctl00$MainContent$struck_by']:checked").val();
             if (struck_by == undefined) {
                 struck_by = "";
             }

             var line_fire = $("input:radio[name='ctl00$MainContent$line_fire']:checked").val();
             if (line_fire == undefined) {
                 line_fire = "";
             }

             var eyes_tasks = $("input:radio[name='ctl00$MainContent$eyes_tasks']:checked").val();
             if (eyes_tasks == undefined) {
                 eyes_tasks = "";
             }

             var lifting_lowering = $("input:radio[name='ctl00$MainContent$lifting_lowering']:checked").val();
             if (lifting_lowering == undefined) {
                 lifting_lowering = "";
             }

             var posture = $("input:radio[name='ctl00$MainContent$posture']:checked").val();
             if (posture == undefined) {
                 posture = "";
             }






             var head = $("input:radio[name='ctl00$MainContent$head']:checked").val();
             if (head == undefined) {
                 head = "";
             }

             var ears_eyes = $("input:radio[name='ctl00$MainContent$ears_eyes']:checked").val();
             if (ears_eyes == undefined) {
                 ears_eyes = "";
             }

             var face_respiratory = $("input:radio[name='ctl00$MainContent$face_respiratory']:checked").val();
             if (face_respiratory == undefined) {
                 face_respiratory = "";
             }

             var hands_arms = $("input:radio[name='ctl00$MainContent$hands_arms']:checked").val();
             if (hands_arms == undefined) {
                 hands_arms = "";
             }

             var feet_legs = $("input:radio[name='ctl00$MainContent$feet_legs']:checked").val();
             if (feet_legs == undefined) {
                 feet_legs = "";
             }



             var right_job = $("input:radio[name='ctl00$MainContent$right_job']:checked").val();
             if (right_job == undefined) {
                 right_job = "";
             }

             var used_correctly = $("input:radio[name='ctl00$MainContent$used_correctly']:checked").val();
             if (used_correctly == undefined) {
                 used_correctly = "";
             }

             var in_safe_conditions = $("input:radio[name='ctl00$MainContent$in_safe_conditions']:checked").val();
             if (in_safe_conditions == undefined) {
                 in_safe_conditions = "";
             }

             var hamesses = $("input:radio[name='ctl00$MainContent$hamesses']:checked").val();
             if (hamesses == undefined) {
                 hamesses = "";
             }

             var barricade_warning_lights = $("input:radio[name='ctl00$MainContent$barricade_warning_lights']:checked").val();
             if (barricade_warning_lights == undefined) {
                 barricade_warning_lights = "";
             }

             var chocks_restraints = $("input:radio[name='ctl00$MainContent$chocks_restraints']:checked").val();
             if (chocks_restraints == undefined) {
                 chocks_restraints = "";
             }

             var pre_job_safe_checks = $("input:radio[name='ctl00$MainContent$pre_job_safe_checks']:checked").val();
             if (pre_job_safe_checks == undefined) {
                 pre_job_safe_checks = "";
             }




             var standard_adequate_job = $("input:radio[name='ctl00$MainContent$standard_adequate_job']:checked").val();
             if (standard_adequate_job == undefined) {
                 standard_adequate_job = "";
             }

             var standard_established = $("input:radio[name='ctl00$MainContent$standard_established']:checked").val();
             if (standard_established == undefined) {
                 standard_established = "";
             }

             var standard_being_maintained = $("input:radio[name='ctl00$MainContent$standard_being_maintained']:checked").val();
             if (standard_being_maintained == undefined) {
                 standard_being_maintained = "";
             }

             var isolation_lockout = $("input:radio[name='ctl00$MainContent$isolation_lockout']:checked").val();
             if (isolation_lockout == undefined) {
                 isolation_lockout = "";
             }

             var hot_work_permit = $("input:radio[name='ctl00$MainContent$hot_work_permit']:checked").val();
             if (hot_work_permit == undefined) {
                 hot_work_permit = "";
             }

             var confined_space_permit = $("input:radio[name='ctl00$MainContent$confined_space_permit']:checked").val();
             if (confined_space_permit == undefined) {
                 confined_space_permit = "";
             }

             var electrical_permit = $("input:radio[name='ctl00$MainContent$electrical_permit']:checked").val();
             if (electrical_permit == undefined) {
                 electrical_permit = "";
             }

             var work_height_permit = $("input:radio[name='ctl00$MainContent$work_height_permit']:checked").val();
             if (work_height_permit == undefined) {
                 work_height_permit = "";
             }

             var rescue_plan_place = $("input:radio[name='ctl00$MainContent$rescue_plan_place']:checked").val();
             if (rescue_plan_place == undefined) {
                 rescue_plan_place = "";
             }



             var standards_established_understood = $("input:radio[name='ctl00$MainContent$standards_established_understood']:checked").val();
             if (standards_established_understood == undefined) {
                 standards_established_understood = "";
             }

             var walkway_passageways = $("input:radio[name='ctl00$MainContent$walkway_passageways']:checked").val();
             if (walkway_passageways == undefined) {
                 walkway_passageways = "";
             }

             var disorganized_tools_bench = $("input:radio[name='ctl00$MainContent$disorganized_tools_bench']:checked").val();
             if (disorganized_tools_bench == undefined) {
                 disorganized_tools_bench = "";
             }

             var materials_storage = $("input:radio[name='ctl00$MainContent$materials_storage']:checked").val();
             if (materials_storage == undefined) {
                 materials_storage = "";
             }

             var obstruction_leaning_items = $("input:radio[name='ctl00$MainContent$obstruction_leaning_items']:checked").val();
             if (obstruction_leaning_items == undefined) {
                 obstruction_leaning_items = "";
             }

             var stairs_platforms = $("input:radio[name='ctl00$MainContent$stairs_platforms']:checked").val();
             if (stairs_platforms == undefined) {
                 stairs_platforms = "";
             }








             var housekeeping = $("input:radio[name='ctl00$MainContent$housekeeping']:checked").val();
             if (housekeeping == undefined) {
                 housekeeping = "";
             }

             var chemical_storage = $("input:radio[name='ctl00$MainContent$chemical_storage']:checked").val();
             if (chemical_storage == undefined) {
                 chemical_storage = "";
             }

             var waste_diposal = $("input:radio[name='ctl00$MainContent$waste_diposal']:checked").val();
             if (waste_diposal == undefined) {
                 waste_diposal = "";
             }

             var walking_working_surface = $("input:radio[name='ctl00$MainContent$walking_working_surface']:checked").val();
             if (walking_working_surface == undefined) {
                 walking_working_surface = "";
             }



             var reactions_people = $("#MainContent_txtreactions_people").val();
             var postion_people = $("#MainContent_txtpostion_people").val();
             var personal_protection_equipment = $("#MainContent_txtpersonal_protection_equipment").val();
             var tools_equipment = $("#MainContent_txttools_equipment").val();
             var procedures = $("#MainContent_txtprocedures").val();
             var orderliness_tidiness = $("#MainContent_txtorderliness_tidiness").val();
             var environment = $("#MainContent_txtenvironment").val();
             


             var data_post = JSON.stringify({
                 country_id: country_id,
                 company_id: company_id,
                 function_id: function_id,
                 department_id: department_id,
                 division_id:division_id,
                 site_id: site_id,
                 sot_date: sot_date,
                 reportdate: report_date,
                 sot_time: sot_time,
                 sot_time_end:sot_time_end,
                 location: location,
                 typework: typework,
                 comment: comment,
                 sotteam: sotteam_employee,
                 user_id: user_login_id,
                 typelogin: type_login,
                 type_employment: type_employment,

                 changing_position:changing_position,
                 stopping_work: stopping_work,
                 rearranging_job: rearranging_job,
                 hiding_dodging:hiding_dodging,
                 changing_tools: changing_tools,
                 applying_lockout: applying_lockout,
                 adjusting_ppe: adjusting_ppe,

                 striking_against: striking_against,
                 caught_between: caught_between,
                 inhaling: inhaling,
                 absorbing: absorbing,
                 electricity: electricity,
                 falling: falling,
                 struck_by: struck_by,
                 line_fire: line_fire,
                 eyes_tasks: eyes_tasks,
                 lifting_lowering: lifting_lowering,
                 posture:posture,

                 head: head,
                 ears_eyes: ears_eyes,
                 face_respiratory: face_respiratory,
                 hands_arms: hands_arms,
                 feet_legs: feet_legs,

                 right_job: right_job,
                 used_correctly: used_correctly,
                 in_safe_conditions: in_safe_conditions,
                 hamesses: hamesses,
                 barricade_warning_lights: barricade_warning_lights,
                 chocks_restraints: chocks_restraints,
                 pre_job_safe_checks: pre_job_safe_checks,

                 standard_adequate_job: standard_adequate_job,
                 standard_established: standard_established,
                 standard_being_maintained: standard_being_maintained,
                 isolation_lockout: isolation_lockout,
                 hot_work_permit: hot_work_permit,
                 confined_space_permit: confined_space_permit,
                 electrical_permit: electrical_permit,
                 work_height_permit: work_height_permit,
                 rescue_plan_place: rescue_plan_place,

                 standards_established_understood: standards_established_understood,
                 walkway_passageways: walkway_passageways,
                 disorganized_tools_bench: disorganized_tools_bench,
                 materials_storage: materials_storage,
                 obstruction_leaning_items: obstruction_leaning_items,
                 stairs_platforms: stairs_platforms,

                 housekeeping: housekeeping,
                 chemical_storage: chemical_storage,
                 waste_diposal: waste_diposal,
                 walking_working_surface: walking_working_surface,


                 reactions_people:reactions_people,
                 postion_people: postion_people,
                 personal_protection_equipment: personal_protection_equipment,
                 tools_equipment: tools_equipment,
                 procedures: procedures,
                 orderliness_tidiness: orderliness_tidiness,
                 environment:environment,

                

             });


             $.ajax({
                 type: "POST",
                 data: data_post,
                 url: 'Actionevent.asmx/createSot',
                // dataType: 'json',
                 contentType: "application/json; charset=utf-8",
                 success: function (id) {

                     closeLoading();
                     var result_save = '<%= Resources.Main.success %>';
                     alert(result_save);

                     window.location.href = "sotform.aspx?pagetype=view&id=" + id;

                 }, error: function (data) {
                     console.log(data);
                 }
             });
             return false;

         }
         else {
             return false;
         }


     }



     function changDepartment()
     {
         var ddl_department_id = $('#MainContent_dddepartment').val();

         $.ajax({
             type: "POST",
             data: { department: ddl_department_id, lang: lang },
             url: 'Masterdata.asmx/getDivisionbyDepartment',
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

                // $('#MainContent_dddivision').val(ddl_department_id + 'D');
                
             }
         });



     }


     function setTypeEmployment()
     {

         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getTypeEmployment',
             dataType: 'json',
             success: function (json) {

                 var $el = $("#MainContent_ddlTypeemployment");
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
   
</script>
   <div id="dialog-confirm" title="">
  
    </div>
   <input type="hidden" id="employee_id" name="employee_id" runat="server">
    <input type="hidden" id="contractor_id" name="contractor_id" runat="server">
  




     <div id="create_reason_reject">
       <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Sot.lbreasonreject %></label><div class="lbrequire"> *</div>
                           
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



  

       <div id="process_action_form" title="<%= Resources.Sot.process_action_form %>">     
         
          	<div class="row">

                  <div class="col-sm-6">
					<div class="form-group">
						<label class="control-label"><%= Resources.Sot.typecontrol %></label><div class="lbrequire"> *</div>
						 <select id="ddtypecontrol" class="form-control" runat="server">
                            
                        </select>
                        
                         <asp:RequiredFieldValidator ID="rqtypecontrl" runat="server" ControlToValidate ="ddtypecontrol" ErrorMessage="<%$ Resources:Sot, rqtypecontrol %>" 
                             ValidationGroup="process" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				
				<div class="col-sm-6">
					<div class="form-group">
						<label class="control-label"><%= Resources.Sot.action %></label><div class="lbrequire"> *</div>
						<input id="txtaction" name="txtaction"  type="text" class="form-control" runat="server">
                        
                         <asp:RequiredFieldValidator ID="rqaction" runat="server" ControlToValidate ="txtaction" ErrorMessage="<%$ Resources:Sot, rqaction %>" 
                             ValidationGroup="process" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
                  
		   </div>

          <div class="row">
                <div class="col-sm-4">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Sot.responsible_person %></label><div class="lbrequire"> *</div>
					    <input id="txtresponsible_person" name="txtresponsible_person"  type="text" class="form-control" runat="server">
                        
                         <asp:RequiredFieldValidator ID="rqresponsible_person" runat="server" ControlToValidate ="txtresponsible_person" ErrorMessage="<%$ Resources:Sot, rqresponsible_person %>" 
                             ValidationGroup="process" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				
				<div class="col-sm-4">
					<div id="due_date" class="form-group">
						<label class="control-label"><%= Resources.Sot.due_date %></label><div class="lbrequire"> *</div>
						 <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtdue_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                        </div>
                        <asp:RequiredFieldValidator ID="rqduedate" runat="server" ValidationGroup="process" ControlToValidate ="txtdue_date" ErrorMessage="<%$ Resources:Sot, rqduedate %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>			
                    </div>
				</div>

                 
		   </div>


         
          <div class="row">
                <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Sot.notify_contractor %></label>
					    <input id="txtnotyfy_contractor" name="txtnotyfy_contractor"  type="text" class="form-control" runat="server">
                       
					</div>
				</div>

			
		   </div>
        

          <div class="row">
                <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Sot.remark %></label>
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


    <div class="row setup-content" id="step-1">
        <div class="col-xs-12  form-horizontal">
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
                             string id4 = Request.QueryString["id"];
                             ArrayList per4 = Session["permission"] as ArrayList;
                            
                             if (per4.IndexOf("sot submit") > -1)         
                           {                            
       
                          %>
                                <asp:Button ID="btSubmit" runat="server" Text="<%$ Resources:Main, btClose %>"  CssClass="btn btn-primary" OnClick="btSubmit_Click" />           
                        <%                      
                            }
       
                          %>
                        
                         <%
                             string id5 = Request.QueryString["id"];
                             ArrayList per5 = Session["permission"] as ArrayList;
                            
                             if (per5.IndexOf("sot edit") > -1)         
                           {                            
       
                          %>
                                <asp:Button ID="btSotformedit" runat="server" Text="<%$ Resources:Sot, btSotformedit %>" CssClass="btn btn-primary"  CausesValidation="False" OnClick="btSotformedit_Click"/>

                          <%                      
                            }
       
                          %>
                       </div>
                    </div>
                  </div>
                           
                
               <strong><h4> <%= Resources.Sot.hd_hierarchy %></h4></strong>
                                       
                <div class="form-group">
                  <%--  <label class="col-sm-2 control-label"><%= Resources.Sot.lbcountry %><div class="lbrequire"> *</div></label>

                    <div class="col-sm-4">
                        <select id="ddcountry" name="ddcountry" class="form-control" onchange="changCountry();" runat="server">
                          
                        </select>
                         <asp:RequiredFieldValidator ID="rqCountry" runat="server" ValidationGroup="main" ControlToValidate ="ddcountry" ErrorMessage="<%$ Resources:Sot, rqcountry %>" CssClass="text-danger" Display="Dynamic">
                         </asp:RequiredFieldValidator>
                    </div>--%>

                    <label class="col-sm-2 control-label"><%= Resources.Sot.lbcompany %><div class="lbrequire"> *</div></label>

                    <div class="col-sm-4">
                        <select id="ddcompany" name="ddcompany" class="form-control" onchange="changCompany();" runat="server">
                       
                        </select>
                        <asp:RequiredFieldValidator ID="rqCompany" runat="server" ValidationGroup="main" ControlToValidate ="ddcompany" ErrorMessage="<%$ Resources:Sot, rqcompany %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
                    </div>

                    <label class="col-sm-2 control-label"><%= Resources.Sot.lbfunction%><div class="lbrequire"> *</div></label>

                    <div class="col-sm-4">
                        <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server">
                      
                        </select>
                          <asp:RequiredFieldValidator ID="rqFunction" runat="server" ValidationGroup="main" ControlToValidate ="ddfunction" ErrorMessage="<%$ Resources:Sot, rqfunction %>" CssClass="text-danger" Display="Dynamic">
                         </asp:RequiredFieldValidator>
                    </div>

                </div>         


                 <div class="form-group">
                  
                    <label class="col-sm-2 control-label"><%= Resources.Sot.lbdepartment %><div class="lbrequire"> *</div></label>

                    <div class="col-sm-4">
                        <select id="dddepartment" class="form-control"  runat="server" onchange="changDepartment();">
                       
                        </select>
                      <asp:RequiredFieldValidator ID="rqDepartment" runat="server" ValidationGroup="main" ControlToValidate ="dddepartment" ErrorMessage="<%$ Resources:Sot, rqdepartment %>" CssClass="text-danger" Display="Dynamic">
                      </asp:RequiredFieldValidator>
                    </div>

                  
                    <label class="col-sm-2 control-label"><%= Resources.Sot.lbdivision %><div class="lbrequire"> *</div></label>

                    <div class="col-sm-4">
                         
                        <select id="dddivision" class="form-control"  runat="server">
                       
                    </select>
                      <asp:RequiredFieldValidator ID="rqDivision" runat="server" ValidationGroup="main" ControlToValidate ="dddivision" ErrorMessage="<%$ Resources:Sot, rqdivision %>" CssClass="text-danger" Display="Dynamic">
                      </asp:RequiredFieldValidator>
                    </div>
           
                
                </div>   
                

         
              <%--   <div class="form-group">
                    <label class="col-sm-2 control-label"><%= Resources.Sot.lbsite %><div class="lbrequire"> *</div></label>

                    <div class="col-sm-4">
                        <select id="ddsite" class="form-control" runat="server">
                       
                        </select>
                       <asp:RequiredFieldValidator ID="rqSite" runat="server" ValidationGroup="main" ControlToValidate ="ddsite" ErrorMessage="<%$ Resources:Sot, rqsite %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
                    </div>

                    <label class="col-sm-2 control-label"></label>

                    <div class="col-sm-4"></div>
                </div>      --%>                     				
             
                <br />
                <hr2>

                <br />
                <strong> <h4><%= Resources.Sot.hd_detail %></h4></strong>
                 <div class="form-group" id="data_sot_date">
                    <label class="col-sm-2 control-label"><%= Resources.Sot.lbsotdate %><div class="lbrequire"> *</div></label>

                    <div class="col-sm-4">
                       <div class="input-group date">
                            <input class="form-control" value="" type="text" id="txtsot_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                      
                        </div>
                        <asp:RequiredFieldValidator ID="rqsotdate" runat="server" ValidationGroup="main" ControlToValidate ="txtsot_date" ErrorMessage="<%$ Resources:Sot, rqsotdate %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>    
                    </div>
                 
                    <label class="col-sm-2 control-label"><%= Resources.Sot.lbsottime %></label>

                    <div class="col-sm-4">
                         <div class="form-inline">
                                             <select id="ddtimehour" name="ddtimehour" class="form-control"  runat="server">
                                                 <option value="00:00">00:00</option>
                                                 <option value="00:15">00:15</option>
                                                 <option value="00:30">00:30</option>
                                                 <option value="00:45">00:45</option>
                                                 <option value="01:00">01:00</option>
                                                 <option value="01:15">01:15</option>
                                                 <option value="01:30">01:30</option>
                                                 <option value="01:45">01:45</option>
                                                 <option value="02:00">02:00</option>
                                                 <option value="02:15">02:15</option>
                                                 <option value="02:30">02:30</option>
                                                 <option value="02:45">02:45</option>
                                                 <option value="03:00">03:00</option>
                                                 <option value="03:15">03:15</option>
                                                 <option value="03:30">03:30</option>
                                                 <option value="03:45">03:45</option>
                                                 <option value="04:00">04:00</option>
                                                 <option value="04:15">04:15</option>
                                                 <option value="04:30">04:30</option>
                                                 <option value="04:45">04:45</option>
                                                 <option value="05:00">05:00</option>
                                                 <option value="05:15">05:15</option>
                                                 <option value="05:30">05:30</option>
                                                 <option value="05:45">05:45</option>
                                                 <option value="06:00">06:00</option>
                                                 <option value="06:15">06:15</option>
                                                 <option value="06:30">06:30</option>
                                                 <option value="06:45">06:45</option>
                                                 <option value="07:00">07:00</option>
                                                 <option value="07:15">07:15</option>
                                                 <option value="07:30">07:30</option>
                                                 <option value="07:45">07:45</option>
                                                 <option value="08:00">08:00</option>
                                                 <option value="08:15">08:15</option>
                                                 <option value="08:30">08:30</option>
                                                 <option value="08:45">08:45</option>
                                                 <option value="09:00">09:00</option>
                                                 <option value="09:15">09:15</option>
                                                 <option value="09:30">09:30</option>
                                                 <option value="09:45">09:45</option>
                                                 <option value="10:00">10:00</option>
                                                 <option value="10:15">10:15</option>
                                                 <option value="10:30">10:30</option>
                                                 <option value="10:45">10:45</option>
                                                 <option value="11:00">11:00</option>
                                                 <option value="11:15">11:15</option>
                                                 <option value="11:30">11:30</option>
                                                 <option value="11:45">11:45</option>
                                                 <option value="12:00">12:00</option>
                                                 <option value="12:15">12:15</option>
                                                 <option value="12:30">12:30</option>
                                                 <option value="12:45">12:45</option>
                                                 <option value="13:00">13:00</option>
                                                 <option value="13:15">13:15</option>
                                                 <option value="13:30">13:30</option>
                                                 <option value="13:45">13:45</option>
                                                 <option value="14:00">14:00</option>
                                                 <option value="14:15">14:15</option>
                                                 <option value="14:30">14:30</option>
                                                 <option value="14:45">14:45</option>
                                                 <option value="15:00">15:00</option>
                                                 <option value="15:15">15:15</option>
                                                 <option value="15:30">15:30</option>
                                                 <option value="15:45">15:45</option>
                                                 <option value="16:00">16:00</option>
                                                 <option value="16:15">16:15</option>
                                                 <option value="16:30">16:30</option>
                                                 <option value="16:45">16:45</option>
                                                 <option value="17:00">17:00</option>
                                                 <option value="17:15">17:15</option>
                                                 <option value="17:30">17:30</option>
                                                 <option value="17:45">17:45</option>
                                                 <option value="18:00">18:00</option>
                                                 <option value="18:15">18:15</option>
                                                 <option value="18:30">18:30</option>
                                                 <option value="18:45">18:45</option>
                                                 <option value="19:00">19:00</option>
                                                 <option value="19:15">19:15</option>
                                                 <option value="19:30">19:30</option>
                                                 <option value="19:45">19:45</option>
                                                 <option value="20:00">20:00</option>
                                                 <option value="20:15">20:15</option>
                                                 <option value="20:30">20:30</option>
                                                 <option value="20:45">20:45</option>
                                                 <option value="21:00">21:00</option>
                                                 <option value="21:15">21:15</option>
                                                 <option value="21:30">21:30</option>
                                                 <option value="21:45">21:45</option>
                                                 <option value="22:00">22:00</option>
                                                 <option value="22:15">22:15</option>
                                                 <option value="22:30">22:30</option>
                                                 <option value="22:45">22:45</option>
                                                 <option value="23:00">23:00</option>
                                                 <option value="23:15">23:15</option>
                                                 <option value="23:30">23:30</option>
                                                 <option value="23:45">23:45</option>
                                            </select>

                                            <select id="ddtimehour2" name="ddtimehour2" class="form-control"  runat="server">
                                                 <option value="00:00">00:00</option>
                                                 <option value="00:15">00:15</option>
                                                 <option value="00:30">00:30</option>
                                                 <option value="00:45">00:45</option>
                                                 <option value="01:00">01:00</option>
                                                 <option value="01:15">01:15</option>
                                                 <option value="01:30">01:30</option>
                                                 <option value="01:45">01:45</option>
                                                 <option value="02:00">02:00</option>
                                                 <option value="02:15">02:15</option>
                                                 <option value="02:30">02:30</option>
                                                 <option value="02:45">02:45</option>
                                                 <option value="03:00">03:00</option>
                                                 <option value="03:15">03:15</option>
                                                 <option value="03:30">03:30</option>
                                                 <option value="03:45">03:45</option>
                                                 <option value="04:00">04:00</option>
                                                 <option value="04:15">04:15</option>
                                                 <option value="04:30">04:30</option>
                                                 <option value="04:45">04:45</option>
                                                 <option value="05:00">05:00</option>
                                                 <option value="05:15">05:15</option>
                                                 <option value="05:30">05:30</option>
                                                 <option value="05:45">05:45</option>
                                                 <option value="06:00">06:00</option>
                                                 <option value="06:15">06:15</option>
                                                 <option value="06:30">06:30</option>
                                                 <option value="06:45">06:45</option>
                                                 <option value="07:00">07:00</option>
                                                 <option value="07:15">07:15</option>
                                                 <option value="07:30">07:30</option>
                                                 <option value="07:45">07:45</option>
                                                 <option value="08:00">08:00</option>
                                                 <option value="08:15">08:15</option>
                                                 <option value="08:30">08:30</option>
                                                 <option value="08:45">08:45</option>
                                                 <option value="09:00">09:00</option>
                                                 <option value="09:15">09:15</option>
                                                 <option value="09:30">09:30</option>
                                                 <option value="09:45">09:45</option>
                                                 <option value="10:00">10:00</option>
                                                 <option value="10:15">10:15</option>
                                                 <option value="10:30">10:30</option>
                                                 <option value="10:45">10:45</option>
                                                 <option value="11:00">11:00</option>
                                                 <option value="11:15">11:15</option>
                                                 <option value="11:30">11:30</option>
                                                 <option value="11:45">11:45</option>
                                                 <option value="12:00">12:00</option>
                                                 <option value="12:15">12:15</option>
                                                 <option value="12:30">12:30</option>
                                                 <option value="12:45">12:45</option>
                                                 <option value="13:00">13:00</option>
                                                 <option value="13:15">13:15</option>
                                                 <option value="13:30">13:30</option>
                                                 <option value="13:45">13:45</option>
                                                 <option value="14:00">14:00</option>
                                                 <option value="14:15">14:15</option>
                                                 <option value="14:30">14:30</option>
                                                 <option value="14:45">14:45</option>
                                                 <option value="15:00">15:00</option>
                                                 <option value="15:15">15:15</option>
                                                 <option value="15:30">15:30</option>
                                                 <option value="15:45">15:45</option>
                                                 <option value="16:00">16:00</option>
                                                 <option value="16:15">16:15</option>
                                                 <option value="16:30">16:30</option>
                                                 <option value="16:45">16:45</option>
                                                 <option value="17:00">17:00</option>
                                                 <option value="17:15">17:15</option>
                                                 <option value="17:30">17:30</option>
                                                 <option value="17:45">17:45</option>
                                                 <option value="18:00">18:00</option>
                                                 <option value="18:15">18:15</option>
                                                 <option value="18:30">18:30</option>
                                                 <option value="18:45">18:45</option>
                                                 <option value="19:00">19:00</option>
                                                 <option value="19:15">19:15</option>
                                                 <option value="19:30">19:30</option>
                                                 <option value="19:45">19:45</option>
                                                 <option value="20:00">20:00</option>
                                                 <option value="20:15">20:15</option>
                                                 <option value="20:30">20:30</option>
                                                 <option value="20:45">20:45</option>
                                                 <option value="21:00">21:00</option>
                                                 <option value="21:15">21:15</option>
                                                 <option value="21:30">21:30</option>
                                                 <option value="21:45">21:45</option>
                                                 <option value="22:00">22:00</option>
                                                 <option value="22:15">22:15</option>
                                                 <option value="22:30">22:30</option>
                                                 <option value="22:45">22:45</option>
                                                 <option value="23:00">23:00</option>
                                                 <option value="23:15">23:15</option>
                                                 <option value="23:30">23:30</option>
                                                 <option value="23:45">23:45</option>
                                            </select>

                                         </div>

                    </div>

                    </div>

                
                 <div class="form-group">
                    <label class="col-sm-2 control-label"><%= Resources.Sot.lblocation %></label>

                    <div class="col-sm-4"><input class="form-control" type="text" id="txtlocation" name="txtlocation" runat="server"></div>

                    <label class="col-sm-2 control-label"><%= Resources.Sot.lbtypework %><div class="lbrequire"> *</div></label>

                    <div class="col-sm-4">
                        <input class="form-control" type="text" id="txttypework" name="txttypework" runat="server">
                        <asp:RequiredFieldValidator ID="rqtypework" runat="server" ValidationGroup="main" ControlToValidate ="txttypework" ErrorMessage="<%$ Resources:Sot, rqtypework %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
                    </div>
                </div>
                
                 <div class="form-group">
                    <label class="col-sm-2 control-label"><%= Resources.Sot.type_employment %><div class="lbrequire"> *</div></label>
					<div class="col-sm-4"><select id="ddlTypeemployment" class="form-control" runat="server">
                            
                        </select>
                        <asp:RequiredFieldValidator ID="rqddltypeemployment" runat="server" ValidationGroup="injury" ControlToValidate ="ddlTypeemployment" ErrorMessage="<%$ Resources:Sot, rqtypeemployment %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>
                     </div>


                     <label class="col-sm-2 control-label"><%= Resources.Sot.report_date %></label>
                      <div class="col-sm-4">           
                            <input class="form-control" value="" type="text" id="txtreport_date" runat="server">
                     </div>
                                        
                </div>  

                 <div class="form-group">
                    <label class="col-sm-2 control-label"><%= Resources.Sot.lbcomment %></label>

                    <div class="col-sm-10"><textarea class="form-control" rows="5" id="txtcomment" runat="server"></textarea></div>

                </div>

                 <div class="row">
                  <div class="col-md-12">
                       <div class="form-group">
                            <label id="lbshowimage" class="control-label" runat="server"><%= Resources.Hazard.showfile %></label>
                            <div id="showimage">
                                
                            </div>
                        </div>
                   </div>
                 </div>

                 <div class="form-group">
                      <div class="col-sm-2" ></div>
                     <div class="col-sm-10 lbrequire" id="infouploadimage">
                          <%= Resources.Sot.infoimage %>
                     </div>
                </div>
                  <div class="form-group">
                        <div class="col-sm-2 lbrequire"></div>
                       <div class="col-sm-10">
                                                    
                                <div  class="dropzone" id="dropzoneForm" style="margin-top:8px;">
                                    <div class="fallback">
                                        <input name="file" type="file" multiple="multiple" />
                                       <input type="submit" value="Upload" />
                                    </div>
                                </div>
                          
                        </div>
                        </div>
                  
            
                 
                     <br />              
                      <hr2></hr2>
                     <br />
                  
                   <div class="form-group">
                      
                       <div class="col-sm-12">   
                              <strong><h3><%= Resources.Sot.process_action_form %></h3></strong>                 
                        </div>     
                   </div>
                     <div class="form-group">         

                        <div class="col-sm-12">                 
                            <table id="tbProcess_action" class="table table-bordered table-hover">
                                 <thead>
                                    <tr>
                                        <th> <%= Resources.Sot.no %></th>
                                        <th></th>
                                        <th> <%= Resources.Sot.typecontrol %></th>
                                        <th> <%= Resources.Sot.action %></th>
                                        <th> <%= Resources.Sot.responsible_person %></th>
                                        <th> <%= Resources.Sot.due_date %></th>
                                        <th> <%= Resources.Sot.status %></th>
                                        <th> <%= Resources.Sot.date_complete %></th>
                                        <th> <%= Resources.Sot.attachment %></th>
                                        <th> <%= Resources.Sot.notify_contractor %></th>
                                        <th> <%= Resources.Sot.close_action %></th>
                                        <th> <%= Resources.Sot.remark %></th>
                                        <th> <%= Resources.Sot.manage %></th>
                    
                                    </tr>
                                </thead>                        
                           </table>

                        </div>

                    </div>      

                    <div class="form-group">
                        <div class="col-md-12">
                           <button type="button" id="btCreateProcessAction" class="btn btn-primary" runat="server" onclick="showCreateProcessAction();"><i class="fa fa-plus"></i></button>  <%= Resources.Sot.add_process_action %>
                       
                           </div>
                              
                       </div>

            <br />
            <hr2>
             <br />

                 <div class="form-group">
                      
                       <div class="col-sm-12">   
                              <strong><h3><%= Resources.Sot.lbsotteam %></h3></strong>                 
                        </div>     
                   </div>

                <div class="form-group">
                  <label class="col-sm-2 control-label"></label>

                    <div class="col-sm-10"><input class="form-control" type="text" id="txtsotteam" name="txtsotteam" runat="server"></div>

              </div> 



            <br />
            <hr2>
             <br />

                 <div class="form-group">
                      
                       <div class="col-sm-12">   
                              <strong><h3><%= Resources.Sot.lbbehavioral %></h3></strong>                 
                        </div>     
                   </div>

       
                <div  class="form-group">
                        <label class="col-sm-2 control-label"></label>      
                        <div class="col-sm-4">
                            <label class="control-label"><h4><%= Resources.Sot.description %></h4> </label>    
                        </div>
                        <div class="col-sm-3">
                            <label class="control-label"><h4><%= Resources.Sot.risk %></h4> </label>    
                        </div>
                        <div class="col-sm-3">
                            <label class="control-label"><h4><%= Resources.Sot.comments %></h4> </label>  
                        </div>
                                        
                </div>


                <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label"><%= Resources.Sot.reaction_people %></label>       
                        </div>
                        <div class="col-sm-3">
                        
                        </div>

                        <div class="col-sm-3">
                          
                        </div>
                                        
                </div>

                 <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label label-font"><%= Resources.Sot.changing_position %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.stopping_work %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.rearranging_job %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.hiding_dodging %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.changing_tools %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.applying_lockout %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.adjusting_ppe %></label><br />
                        </div>
                        <div class="col-sm-3">
                            <label style="margin-right:10px;"> <input  value="R" id="changing_position_risk" name="changing_position" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="changing_position_safe" name="changing_position" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="changing_position_na" name="changing_position" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="stopping_work_risk" name="stopping_work" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="stopping_work_safe" name="stopping_work" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="stopping_work_na" name="stopping_work" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="rearranging_job_risk" name="rearranging_job" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="rearranging_job_safe" name="rearranging_job" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="rearranging_job_na" name="rearranging_job" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="hiding_dodging_risk" name="hiding_dodging" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="hiding_dodging_safe" name="hiding_dodging" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="hiding_dodging_na" name="hiding_dodging" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="changing_tools_risk" name="changing_tools" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="changing_tools_safe" name="changing_tools" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="changing_tools_na" name="changing_tools" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="applying_lockout_risk" name="applying_lockout" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="applying_lockout_safe" name="applying_lockout" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="applying_lockout_na" name="applying_lockout" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="adjusting_ppe_risk" name="adjusting_ppe" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="adjusting_ppe_safe" name="adjusting_ppe" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="adjusting_ppe_na" name="adjusting_ppe" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>

                        </div>

                        <div class="col-sm-3">
                             <textarea class="form-control" rows="5" id="txtreactions_people" runat="server"></textarea>
                        </div>
                                        
                </div>




                <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label"><%= Resources.Sot.position_people %></label>       
                        </div>
                        <div class="col-sm-3">
                        
                        </div>

                        <div class="col-sm-3">
                          
                        </div>
                                        
                </div>
                
                 <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label label-font"><%= Resources.Sot.striking_against %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.caught_between %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.inhaling %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.absorbing %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.electricity %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.falling %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.struck_by %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.line_fire %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.eyes_tasks %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.lifting_lowering %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.posture %></label><br />
                        </div>
                        <div class="col-sm-3">
                            <label style="margin-right:10px;"> <input  value="R" id="striking_against_risk" name="striking_against" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="striking_against_safe" name="striking_against" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="striking_against_na" name="striking_against" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="caught_between_risk" name="caught_between" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="caught_between_safe" name="caught_between" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="caught_between_na" name="caught_between" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="inhaling_risk" name="inhaling" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="inhaling_safe" name="inhaling" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="inhaling_na" name="inhaling" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="absorbing_risk" name="absorbing" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="absorbing_safe" name="absorbing" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="absorbing_na" name="absorbing" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="electricity_risk" name="electricity" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="electricity_safe" name="electricity" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="electricity_na" name="electricity" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="falling_risk" name="falling" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="falling_safe" name="falling" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="falling_na" name="falling" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="struck_by_risk" name="struck_by" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="struck_by_safe" name="struck_by" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="struck_by_na" name="struck_by" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                           <label style="margin-right:10px;"> <input  value="R" id="line_fire_by_risk" name="line_fire" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="line_fire_by_safe" name="line_fire" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="line_fire_by_na" name="line_fire" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                            <label style="margin-right:10px;"> <input  value="R" id="eyes_tasks_by_risk" name="eyes_tasks" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="eyes_tasks_by_safe" name="eyes_tasks" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="eyes_tasks_by_na" name="eyes_tasks" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />
          

                            <label style="margin-right:10px;"> <input  value="R" id="lifting_lowering_by_risk" name="lifting_lowering" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="lifting_lowering_by_safe" name="lifting_lowering" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="lifting_lowering_by_na" name="lifting_lowering" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                            <label style="margin-right:10px;"> <input  value="R" id="posture_by_risk" name="posture" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="posture_by_safe" name="posture" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="posture_by_na" name="posture" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>

                        </div>

                        <div class="col-sm-3">
                             <textarea class="form-control" rows="5" id="txtpostion_people" runat="server"></textarea>
                        </div>
                                        
                </div>


                 <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label"><%= Resources.Sot.personal_protection_equipment %></label>       
                        </div>
                        <div class="col-sm-3">
                        
                        </div>

                        <div class="col-sm-3">
                          
                        </div>
                                        
                </div>
                
                 <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label label-font"><%= Resources.Sot.head %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.ears_eyes %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.face_respiratory %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.hand_arms %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.feet_legs %></label><br />
                        
                        </div>
                        <div class="col-sm-3">
                            <label style="margin-right:10px;"> <input  value="R" id="head_risk" name="head" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="head_safe" name="head" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="head_na" name="head" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="ears_eyes_risk" name="ears_eyes" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="ears_eyes_safe" name="ears_eyes" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="ears_eyes_na" name="ears_eyes" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="face_respiratory_risk" name="face_respiratory" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="face_respiratory_safe" name="face_respiratory" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="face_respiratory_na" name="face_respiratory" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="hands_arms_risk" name="hands_arms" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="hands_arms_safe" name="hands_arms" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="hands_arms_na" name="hands_arms" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="feet_legs_risk" name="feet_legs" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="feet_legs_safe" name="feet_legs" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="feet_legs_na" name="feet_legs" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />
                        
                           

                        </div>

                        <div class="col-sm-3">
                             <textarea class="form-control" rows="5" id="txtpersonal_protection_equipment" runat="server"></textarea>
                        </div>
                                        
                </div>

                 <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label"><%= Resources.Sot.tools_equipment %></label>       
                        </div>
                        <div class="col-sm-3">
                        
                        </div>

                        <div class="col-sm-3">
                          
                        </div>
                                        
                </div>
                
                 <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label label-font"><%= Resources.Sot.right_job %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.used_correctly %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.in_safe_conditions %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.hamesses %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.barricade_warning_lights %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.chocks_restraints %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.pre_job_safe_checks %></label><br />
                        
                        </div>
                        <div class="col-sm-3">
                            <label style="margin-right:10px;"> <input  value="R" id="right_job_risk" name="right_job" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="right_job_safe" name="right_job" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="right_job_na" name="right_job" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="used_correctly_risk" name="used_correctly" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="used_correctly_safe" name="used_correctly" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="used_correctly_na" name="used_correctly" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="in_safe_conditions_risk" name="in_safe_conditions" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="in_safe_conditions_safe" name="in_safe_conditions" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="in_safe_conditions_na" name="in_safe_conditions" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="hamesses_risk" name="hamesses" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="hamesses_safe" name="hamesses" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="hamesses_na" name="hamesses" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="barricade_warning_lights_risk" name="barricade_warning_lights" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="barricade_warning_lights_safe" name="barricade_warning_lights" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="barricade_warning_lights_na" name="barricade_warning_lights" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                            <label style="margin-right:10px;"> <input  value="R" id="chocks_restraints_risk" name="chocks_restraints" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="chocks_restraints_safe" name="chocks_restraints" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="chocks_restraints_na" name="chocks_restraints" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                            <label style="margin-right:10px;"> <input  value="R" id="pre_job_safe_checks_risk" name="pre_job_safe_checks" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="pre_job_safe_checks_safe" name="pre_job_safe_checks" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="pre_job_safe_checks_na" name="pre_job_safe_checks" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />
                        
                           

                        </div>

                        <div class="col-sm-3">
                             <textarea class="form-control" rows="5" id="txttools_equipment" runat="server"></textarea>
                        </div>
                                        
                </div>


                  <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label"><%= Resources.Sot.procedures %></label>       
                        </div>
                        <div class="col-sm-3">
                        
                        </div>

                        <div class="col-sm-3">
                          
                        </div>
                                        
                </div>
                
                 <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label label-font"><%= Resources.Sot.standard_adequate_job %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.standard_established %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.standard_being_maintained %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.isolation_lockout %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.hot_work_permit %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.confined_space_permit%></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.electrical_permit %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.work_height_permit %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.rescue_plan_place %></label><br />
                        
                        </div>
                        <div class="col-sm-3">
                            <label style="margin-right:10px;"> <input  value="R" id="standard_adequate_job_risk" name="standard_adequate_job" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="standard_adequate_job_safe" name="standard_adequate_job" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="standard_adequate_job_na" name="standard_adequate_job" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="standard_established_risk" name="standard_established" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="standard_established_safe" name="standard_established" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="standard_established_na" name="standard_established" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="standard_being_maintained_risk" name="standard_being_maintained" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="standard_being_maintained_safe" name="standard_being_maintained" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="standard_being_maintained_na" name="standard_being_maintained" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="isolation_lockout_risk" name="isolation_lockout" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="isolation_lockout_safe" name="isolation_lockout" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="isolation_lockout_na" name="isolation_lockout" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="hot_work_permit_risk" name="hot_work_permit" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="hot_work_permit_safe" name="hot_work_permit" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="hot_work_permit_na" name="hot_work_permit" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                            <label style="margin-right:10px;"> <input  value="R" id="confined_space_permit_risk" name="confined_space_permit" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="confined_space_permit_safe" name="confined_space_permit" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="confined_space_permit_na" name="confined_space_permit" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                            <label style="margin-right:10px;"> <input  value="R" id="electrical_permit_risk" name="electrical_permit" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="electrical_permit_safe" name="electrical_permit" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="electrical_permit_na" name="electrical_permit" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                            <label style="margin-right:10px;"> <input  value="R" id="work_height_permit_risk" name="work_height_permit" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="work_height_permit_safe" name="work_height_permit" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="work_height_permit_na" name="work_height_permit" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                            <label style="margin-right:10px;"> <input  value="R" id="rescue_plan_place_risk" name="rescue_plan_place" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="rescue_plan_place_safe" name="rescue_plan_place" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="rescue_plan_place_na" name="rescue_plan_place" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />
                        
                           

                        </div>

                        <div class="col-sm-3">
                             <textarea class="form-control" rows="5" id="txtprocedures" runat="server"></textarea>
                        </div>
                                        
                </div>
              


                    <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label"><%= Resources.Sot.orderline_tidiness %></label>       
                        </div>
                        <div class="col-sm-3">
                        
                        </div>

                        <div class="col-sm-3">
                          
                        </div>
                                        
                </div>
                
                 <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label label-font"><%= Resources.Sot.standards_established_understood %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.walkway_passageways %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.disorganized_tools_bench %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.materials_storage %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.obstruction_leaning_items %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.stairs_platforms%></label><br />
                        
                        </div>
                        <div class="col-sm-3">
                            <label style="margin-right:10px;"> <input  value="R" id="standards_established_understood_risk" name="standards_established_understood" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="standards_established_understood_safe" name="standards_established_understood" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="standards_established_understood_na" name="standards_established_understood" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="walkway_passageways_risk" name="walkway_passageways" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="walkway_passageways_safe" name="walkway_passageways" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="walkway_passageways_na" name="walkway_passageways" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="disorganized_tools_bench_risk" name="disorganized_tools_bench" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="disorganized_tools_bench_safe" name="disorganized_tools_bench" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="disorganized_tools_bench_na" name="disorganized_tools_bench" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="materials_storage_risk" name="materials_storage" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="materials_storage_safe" name="materials_storage" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="materials_storage_na" name="materials_storage" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="obstruction_leaning_items_risk" name="obstruction_leaning_items" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="obstruction_leaning_items_safe" name="obstruction_leaning_items" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="obstruction_leaning_items_na" name="obstruction_leaning_items" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                            <label style="margin-right:10px;"> <input  value="R" id="stairs_platforms_risk" name="stairs_platforms" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="stairs_platforms_safe" name="stairs_platforms" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="stairs_platforms_na" name="stairs_platforms" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                             <br />

                           

                        </div>

                        <div class="col-sm-3">
                             <textarea class="form-control" rows="5" id="txtorderliness_tidiness" runat="server"></textarea>
                        </div>
                                        
                </div>

           




                        
                    <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label"><%= Resources.Sot.environment %></label>       
                        </div>
                        <div class="col-sm-3">
                        
                        </div>

                        <div class="col-sm-3">
                          
                        </div>
                                        
                </div>


                        <div  class="form-group">
                         <label class="col-sm-2 control-label"></label>                
                        <div class="col-sm-4">
                           <label class="control-label label-font"><%= Resources.Sot.housekeeping %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.chemical_storage %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.waste_diposal %></label><br />
                           <label class="control-label label-font"><%= Resources.Sot.walking_working_surface %></label><br />
              
                        
                        </div>
                        <div class="col-sm-3">
                            <label style="margin-right:10px;"> <input  value="R" id="housekeeping_by_risk" name="housekeeping" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="housekeeping_by_safe" name="housekeeping" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="housekeeping_by_na" name="housekeeping" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="chemical_storage_by_risk" name="chemical_storage" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="chemical_storage_by_safe" name="chemical_storage" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="chemical_storage_by_na" name="chemical_storage" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="waste_diposal_by_risk" name="waste_diposal" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="waste_diposal_by_safe" name="waste_diposal" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="waste_diposal_by_na" name="waste_diposal" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                 <br />
                        
                            <label style="margin-right:10px;"> <input  value="R" id="walking_working_surface_by_risk" name="walking_working_surface" type="radio" runat="server">
                            <%= Resources.Sot.at_risk %> </label>
                                <label style="margin-right:10px;"> <input value="S" id="walking_working_surface_by_safe" name="walking_working_surface" type="radio" runat="server">
                            <%= Resources.Sot.safe %> </label>
                                <label> <input value="NA" id="walking_working_surface_by_na" name="walking_working_surface" type="radio" runat="server">
                            <%= Resources.Sot.na %> </label>
                                <br />
                        
                           

                        </div>

                        <div class="col-sm-3">
                             <textarea class="form-control" rows="5" id="txtenvironment" runat="server"></textarea>
                        </div>
                                        
                </div>




           
                
                              				
            <br />
            <br />
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
                <div class="col-sm-12"> 
                    <asp:Button ID="btUpdate" runat="server" Text="<%$ Resources:Main, btSave%>"  CssClass="btn btn-primary" OnClientClick="return manageAction();" />           
               </div>
            </div>
              

            </div>
        </div>
    </div>
 



                       
                </div>
            </div>





</asp:Content>
