<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="incidentform.aspx.cs" Inherits="safetys4.incidentform" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="template/css/plugins/dropzone/dropzone.css" rel="stylesheet" />
<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">
<link href="template/css/plugins/clockpicker/clockpicker.css" rel="stylesheet">
<link href="template/css/jquery-ui.css" rel="stylesheet">
<link rel="stylesheet" href="template/js/plugins/fancybox/jquery.fancybox.css" type="text/css" />
   
<script type="text/javascript" src="template/js/plugins/dropzone/dropzone.min.js"></script>

<script type="text/javascript" src="template/js/plugins/clockpicker/clockpicker.js"></script>
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
<script type="text/javascript" src="template/js/jquery.mcautocomplete.js"></script>
<script type="text/javascript" src="template/js/plugins/fancybox/jquery.fancybox.pack.js"></script>


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

  .wrapper-content {
    margin-left: 0px !important;
} 
</style>

 <script type="text/javascript">

    var id = "";
    var pagetype = "";
    var dialogReason;

    var default_company_id = '<%=Session["company_id"]%>';
    var default_function_id = '<%=Session["function_id"]%>';
    var default_department_id = '<%=Session["department_id"]%>';
    var default_division_id = '<%=Session["division_id"]%>';
    var default_section_id = '<%=Session["section_id"]%>';

     var edit_company_id = "";
     var edit_function_id = "";
     var edit_department_id = "";
     var edit_division_id = "";
     var edit_section_id = "";

     var myDropzone;
     var check_complete = false;


     $(document).ready(function () {


        <%
                
        if (Session["lang"] != null)         
        {
            if (Session["lang"] =="th")
            {                  
            %>
         $('#data_incident_date .input-group.date').datepicker({
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

         $('#data_incident_date .input-group.date').datepicker({
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



         <% if (Session["country"].ToString() == "srilanka")
         {                  
         %>  
             $(".incident_responsible_area_view").hide();
             $(".incident_view").show();
             $(".incident_owner_activity_view").hide();
             $(".incident_activity_view").hide();
    
         <%
          }
           else{
          %>
             $(".incident_responsible_area_view").show();
             $("input[name='ctl00$MainContent$responsible_area']").change(function (e) {

                 if ($(this).val() == "IN") {
                     $(".incident_view").show();
                     $(".incident_owner_activity_view").show();

                 } else if ($(this).val() == "OUT") {

                     setAreaSelect(edit_company_id, edit_function_id, edit_department_id, edit_division_id, edit_section_id, "", "", "", "", "", "", "", "", "", "");
                     $(".incident_view").hide();
                     $(".incident_activity_view").show();
                     $(".incident_owner_activity_view").hide();
                     //$("input[name='ctl00$MainContent$owner_activity'][value='KNOWN']").attr('checked', 'checked');
                     $("input:radio[name='ctl00$MainContent$owner_activity']").filter('[value=KNOWN]').prop('checked', true);

                 }

             });


             $(".incident_owner_activity_view").show();
             $("input[name='ctl00$MainContent$owner_activity']").change(function (e) {

                 if ($(this).val() == "KNOWN") {
                     $(".incident_activity_view").show();

                 } else if ($(this).val() == "UNKNOWN") {

                     $(".incident_activity_view").hide();
                     setActivityAreaSelect("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

                 }

             });

          <%
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
            $("#dropzoneForm").hide();
            $("#infouploadimage").hide();
           

        } else if (pagetype == "edit") {
          
            setShowedit();
            setDropzone();

          

        } else {

            setAreaSelect("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            setActivityAreaSelect("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            var reportdate = $("#MainContent_txtreport_date").val();
            setDropzone();
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
                //clearValidationErrors();
              
                $("#create_reason_reject").css('overflow-x', 'hidden');
            },
            modal: true,
        });
       

        //$('.clockpicker').clockpicker();

      

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
        $("#MainContent_txtincident_area").mcautocomplete({
            // These next two options are what this plugin adds to the autocomplete widget.
            showHeader: true,
            columns: [{
                name: '<%= Resources.Main.lbNameThArea %>',
                minWidth: '30%',
                valueField: 'area_th'
            },
            {
                name: '<%= Resources.Main.lbNameEnArea %>',
                minWidth: '30%',
                valueField: 'area_en'
            },
            {
                name: '<%= Resources.Incident.lbfucntion %>',
                minWidth: '40%',
                valueField: 'function'
            }],

            // Event handler for when a list item is selected.
            select: function (event, ui) {
                var area = $("#MainContent_txtincident_area").val();

                if ((ui.item.area_th).indexOf(area) > -1)
                {
                    this.value = (ui.item ? ui.item.area_th : '');
                } else {

                    this.value = (ui.item ? ui.item.area_en : '');
                }
                
                var responsible_area = $("input:radio[name='ctl00$MainContent$responsible_area']:checked").val();
                if (responsible_area != undefined)
                {
                    if(responsible_area =="OUT")
                    {
                        setActivityAreaSelect(ui.item.company_id, ui.item.function_id, ui.item.department_id, ui.item.division_id, ui.item.section_id, "", "", "", "", "", "", "", "", "", "");

                    } else {

                        setAreaSelect(ui.item.company_id, ui.item.function_id, ui.item.department_id, ui.item.division_id, ui.item.section_id, "", "", "", "", "", "", "", "", "", "");

                    }
                } else {
                    setAreaSelect(ui.item.company_id, ui.item.function_id, ui.item.department_id, ui.item.division_id, ui.item.section_id, "", "", "", "", "", "", "", "", "", "");

                }
                 
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
                        company_id:$("#MainContent_ddcompany").val(),
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



       

       
    });




    function setDropzone()
    {
        var reportdate = $("#MainContent_txtreport_date").val();
        //alert(reportdate);
        //File Upload response from the server
        Dropzone.options.dropzoneForm = {
            maxFiles: 5,
            maxFilesize: 15,
            url: "dropzoneupload.aspx?user_id=" + user_login_id + "&reportdate=" + reportdate+"&id="+id,
            acceptedFiles: "image/jpeg,image/png",
            init: function () {
                myDropzone = this;
                this.on("maxfilesexceeded", function (data) {
                    //var res = eval('(' + data.xhr.responseText + ')');

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
                            data: "folder=" + file.folderimage + "&type=incident&name=" + file.newname,
                            success: function (msg) {

                            },
                            error: function (ex) {

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

                    //file.previewElement.addEventListener("click", function () {
                    //  //  console.log(file);
                    //  //  _this.removeFile(file);
                    //});
                    

                    this.on("success", function (file, response) {
                        //console.log(response);
                        var obj = $.parseJSON(response);
                   
                        file.newname = obj.name;
                        file.folderimage = obj.folder;
                        // console.log(file);

                        setTimeout(function () {
                            
                            var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>"+"/incident/" +obj.folder+"/"+ obj.name;

                            $("img[alt='"+file.name+"']").wrap("<a class='fancybox' href='" + image_name + "'/>");
                          
                            $('.fancybox').fancybox();

                        }, 1000);
                        
                    });

                });
            }
        };

    }


     function CheckIncidentTitleLength(oSrc, args)
     {
         if (args.Value.length > 500) {

             args.IsValid = false;
         } else {
             
             args.IsValid = true;
         }


     }


     function CheckIncidentDetailLength(oSrc, args)
     {
         if (args.Value.length > 4000) {

             args.IsValid = false;
         } else {

             args.IsValid = true;
         }


     }


    function CloseArea()
    {
        dialogReason.dialog("close");


    }


    function setShowImage(incident_id)
    {
        var reportdate = $("#MainContent_txtreport_date").val();
        $.ajax({
            type: "POST",
            data: { id: incident_id },
            url: 'Actionevent.asmx/getImageIncident',
            dataType: 'json',
            success: function (json) {

                var html = "";
                $.each(json, function (value, key) {

                    html = html + '<a href="'+key.path+'" class="fancybox" rel="gallery"><img src="' + key.path + '"  style="width:150px;height:120px;padding-left:5px;padding-right:5px;" runat="server"></a>';
                    
                });

                $("#showimage").html(html);

                $('.fancybox').fancybox();
            }
        });

    }



    function setShowEidtImage(incident_id)
    {
        var reportdate = $("#MainContent_txtreport_date").val();
        $.ajax({
            type: "POST",
            data: { id: incident_id },
            url: 'Actionevent.asmx/getImageIncident',
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
                    // myDropzone.createThumbnailFromUrl(file, "upload/incident/15100028_20170607110927/IMG_20141205_154840.jpg", callback, crossOrigin);
                    myDropzone.emit("complete", mockFile);

                    // If you use the maxFiles option, make sure you adjust it to the
                    // correct amount:
                    // The number of files already uploaded
                    // existingFileCount = existingFileCount + 1;
                    myDropzone.files.push(mockFile);


                    var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/incident/" + key.folder + "/" + key.name;

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
            data: { id: id, lang: lang },
            url: 'Actionevent.asmx/getIncidentbyid',
            dataType: 'json',
            async: false,
            success: function (json) {
         
                $.each(json, function (value, key) {
                   // setCompany(key.company_id);
                    setAreaSelect(key.company_id, key.function_id, key.department_id, key.division_id, key.section_id,
                                  key.location_company_name_th, key.location_function_name_th, key.location_department_name_th, key.location_division_name_th, key.location_section_name_th,
                                  key.location_company_name_en, key.location_function_name_en, key.location_department_name_en, key.location_division_name_en, key.location_section_name_en);


                    edit_company_id = key.reporter_company_id;
                    edit_function_id = key.reporter_function_id;
                    edit_department_id = key.reporter_department_id;


                    if (key.reporter_department_id == "00000000" || key.reporter_department_id == "")
                    {
                        edit_department_id = key.reporter_function_id + "F";

                    } else {
                        edit_department_id = key.reporter_department_id;
                    }

                    if (key.reporter_division_id == "00000000" || key.reporter_division_id == "" || key.reporter_department_id == "00000000")
                    {
                        edit_division_id = edit_department_id + "D";

                    } else {

                        edit_division_id = key.reporter_division_id;
                    }

                    if (key.reporter_section_id == "00000000" || key.reporter_section_id == "" || key.reporter_division_id == "00000000")
                    {
                        edit_section_id = edit_division_id+"D";
                    } else {

                        edit_section_id = key.reporter_section_id;
                    }
                   
                   

                    if (country == "thailand")
                    {
                        setActivityAreaSelect(key.activity_company_id, key.activity_function_id, key.activity_department_id, key.activity_division_id, key.activity_section_id,
                                  key.activity_location_company_name_th, key.activity_location_function_name_th, key.activity_location_department_name_th, key.activity_location_division_name_th, key.activity_location_section_name_th,
                                  key.activity_location_company_name_en, key.activity_location_function_name_en, key.activity_location_department_name_en, key.activity_location_division_name_en, key.activity_location_section_name_en);

                        if (key.responsible_area != null)
                        {
                            $("input[name='ctl00$MainContent$responsible_area'][value='" + key.responsible_area + "']").attr('checked', 'checked');
                        }

                        if (key.owner_activity != null)
                        {
                            $("input[name='ctl00$MainContent$owner_activity'][value='" + key.owner_activity + "']").attr('checked', 'checked');
                        }
                       
                        if (key.responsible_area == "IN") {
                            $(".incident_view").show();

                        } else if (key.responsible_area == "OUT") {
                            $(".incident_view").hide();
                            $(".incident_activity_view").show();
                            $(".incident_owner_activity_view").hide();
                           

                        } else {
                            $(".incident_view").show();
                        }



                        if (key.owner_activity == "KNOWN") {
                            $(".incident_activity_view").show();

                        } else if (key.owner_activity == "UNKNOWN") {

                            $(".incident_activity_view").hide();

                        } else {
                            $(".incident_activity_view").hide();
                        }



                    } else {

                        $(".incident_responsible_area_view").hide();
                        $(".incident_view").show();
                        $(".incident_owner_activity_view").hide();
                        $(".incident_activity_view").hide();



                    }
                    $("#MainContent_txtincident_date").val(key.incident_date);
                    $("#MainContent_ddtimehour").val(key.incident_hour);
                    $("#MainContent_ddtimeminute").val(key.incident_minute);
                    $("#MainContent_txtreport_date").val(key.incident_report);
                 
                    $("#MainContent_txtincident_area").val(key.incident_area);
                    $("#MainContent_txtincidentname").val(key.incident_name);
                    $("#MainContent_txtincidentdetail").val(key.incident_detail);
                    $("#MainContent_txtphone").val(key.phone);
                    $("#MainContent_txtname_surname").val(key.employee_name);
                    $("#MainContent_lbEmployee").text(key.name_modify);
                    $("#MainContent_lbUpdate").text(key.datetime_modify);

                    $("#show_doc_status").html(key.doc_no + ' ' + key.status);

                    if (key.process_status==1)//1 is onprocess
                    {
                        $("#MainContent_btReopenIncident").hide();
                    }

               
                     
                  
                });

                if (pagetype == "view")
                {
                    setShowImage(id);

                } else if (pagetype == "edit") {

                    setShowEidtImage(id);
                }

            }
        });

    }

    //function clearValidationErrors()
    //{
    //    var i;
    //    for (i = 0; i < Page_Validators.length; i++) {

    //        Page_Validators[i].style.display = "none";

    //    }

    //}

    function setAreaSelect(company_id,function_id,department_id,division_id,section_id,
                           location_company_name_th, location_function_name_th,location_department_name_th, location_division_name_th, location_section_name_th,
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

             

                if(company_id=="")
                {
                    $('#MainContent_ddcompany').val(default_company_id);
                    setFunction(default_company_id,default_function_id,default_department_id,default_division_id,default_section_id,
                               location_function_name_th, location_department_name_th, location_division_name_th, location_section_name_th,
                               location_function_name_en, location_department_name_en, location_division_name_en, location_section_name_en);

                }else{
                  
                    if (pagetype == "view")
                    {
                        if (lang == "th") {
                            $('#MainContent_ddcompany').append($('<option></option>').val("old").html(location_company_name_th));
                        } else {
                            $('#MainContent_ddcompany').append($('<option></option>').val("old").html(location_company_name_en));
                        }

                        $('#MainContent_ddcompany').val("old");

                    } else {
                        $('#MainContent_ddcompany').val(company_id);
                    }
                    setFunction(company_id, function_id, department_id, division_id, section_id,
                                location_function_name_th, location_department_name_th, location_division_name_th, location_section_name_th,
                                location_function_name_en, location_department_name_en, location_division_name_en, location_section_name_en);
    
                }

            }
        });




    }




    function setActivityAreaSelect(company_id, function_id, department_id, division_id, section_id,
                         location_company_name_th, location_function_name_th, location_department_name_th, location_division_name_th, location_section_name_th,
                         location_company_name_en, location_function_name_en, location_department_name_en, location_division_name_en, location_section_name_en) {
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getCompanyForm',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddactivitycompany");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });



                //if (company_id == "") {
                //    $('#MainContent_ddactivitycompany').val(default_company_id);
                //    setActivityFunction(default_company_id, default_function_id, default_department_id, default_division_id, default_section_id,
                //               location_function_name_th, location_department_name_th, location_division_name_th, location_section_name_th,
                //               location_function_name_en, location_department_name_en, location_division_name_en, location_section_name_en);

                //} else {

                    if (pagetype == "view") {
                        if (lang == "th") {
                            $('#MainContent_ddactivitycompany').append($('<option></option>').val("old").html(location_company_name_th));
                        } else {
                            $('#MainContent_ddactivitycompany').append($('<option></option>').val("old").html(location_company_name_en));
                        }

                        $('#MainContent_ddactivitycompany').val("old");

                    } else {
                        $('#MainContent_ddactivitycompany').val(company_id);
                    }
                    setActivityFunction(company_id, function_id, department_id, division_id, section_id,
                                location_function_name_th, location_department_name_th, location_division_name_th, location_section_name_th,
                                location_function_name_en, location_department_name_en, location_division_name_en, location_section_name_en);

                //}

            }
        });




    }


    function setReasonReject()
    {
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getReasonReject',
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



    function setActivityFunction(company_id, function_id, department_id, division_id, section_id,
                     location_function_name_th, location_department_name_th, location_division_name_th, location_section_name_th,
                     location_function_name_en, location_department_name_en, location_division_name_en, location_section_name_en) {
        $.ajax({
            type: "POST",
            data: { company: company_id, lang: lang },
            url: 'Masterdata.asmx/getFuctionFormByCompany',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddactivityfunction");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                if (pagetype == "view") {
                    if (lang == "th") {
                        $('#MainContent_ddactivityfunction').append($('<option></option>').val("old").html(location_function_name_th));
                    } else {
                        $('#MainContent_ddactivityfunction').append($('<option></option>').val("old").html(location_function_name_en));
                    }

                    $('#MainContent_ddactivityfunction').val("old");

                } else {
                    $('#MainContent_ddactivityfunction').val(function_id);
                }


                setActivityDepartment(function_id, department_id, division_id, section_id,
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
                   
                    if (lang == "th") {
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
                        setDivision(function_id + "F", division_id, section_id,"","","","");


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




    function setActivityDepartment(function_id, department_id, division_id, section_id,
                        location_department_name_th, location_division_name_th, location_section_name_th,
                        location_department_name_en, location_division_name_en, location_section_name_en) {

        $.ajax({
            type: "POST",
            data: { function: function_id, lang: lang },
            url: 'Masterdata.asmx/getDepartmentFormbyFunction',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddactivitydepartment");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });



                if (pagetype == "view") {

                    if (lang == "th") {
                        $('#MainContent_ddactivitydepartment').append($('<option></option>').val("old").html(location_department_name_th));
                    } else {
                        $('#MainContent_ddactivitydepartment').append($('<option></option>').val("old").html(location_department_name_en));
                    }

                    $('#MainContent_ddactivitydepartment').val("old");

                    setActivityDivision(department_id, division_id, section_id,
                                    location_division_name_th, location_section_name_th,
                                    location_division_name_en, location_section_name_en);

                } else {

                    //if (department_id == "" || department_id == "00000000") {
                    //    $('#MainContent_ddactivitydepartment').val(function_id + "F");
                    //    setActivityDivision(function_id + "F", division_id, section_id, "", "", "", "");


                    //} else {
                        $('#MainContent_ddactivitydepartment').val(department_id);
                        setActivityDivision(department_id, division_id, section_id,
                                    location_division_name_th, location_section_name_th,
                                    location_division_name_en, location_section_name_en);

                   // }
                }



            }
        });



    }



    function setDivision(department_id, division_id, section_id,
                        location_division_name_th, location_section_name_th,
                        location_division_name_en, location_section_name_en)
    {
       
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
               
                if (pagetype == "view")
                {
                   
                    if (lang == "th")
                    {
                        $('#MainContent_dddivision').append($('<option></option>').val("old").html(location_division_name_th));
                    } else {
                        $('#MainContent_dddivision').append($('<option></option>').val("old").html(location_division_name_en));
                    }

                    $('#MainContent_dddivision').val("old");
                    setSection(division_id, section_id, location_section_name_th, location_section_name_en);
                } else {

                    if (division_id == "" || division_id == "00000000" || (department_id.indexOf("F") > -1)) {//มีกรณี ไม่่มี department,division แต่มี section เลยใส่ department_id.indexOf("F")
                        if (pagetype != "view" && pagetype != "edit") {
                            $('#MainContent_dddivision').val(department_id + "D");
                            setSection(department_id + "D", section_id,"","");
                        }


                        if (pagetype == "edit") {

                            if(department_id.indexOf("F") > -1)
                            {
                                $('#MainContent_dddivision').val(division_id);
                                setSection(division_id, section_id, location_section_name_th, location_section_name_en);
                            }

                        }


                    } else {

                        $('#MainContent_dddivision').val(division_id);
                        setSection(division_id, section_id,location_section_name_th,location_section_name_en);
                    }
                }
                
            }
        });



    }



    function setActivityDivision(department_id, division_id, section_id,
                      location_division_name_th, location_section_name_th,
                      location_division_name_en, location_section_name_en) {

        $.ajax({
            type: "POST",
            data: { department: department_id, lang: lang },
            url: 'Masterdata.asmx/getDivisionFormbyDepartment',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddactivitydivision");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                if (pagetype == "view") {

                    if (lang == "th") {
                        $('#MainContent_ddactivitydivision').append($('<option></option>').val("old").html(location_division_name_th));
                    } else {
                        $('#MainContent_ddactivitydivision').append($('<option></option>').val("old").html(location_division_name_en));
                    }

                    $('#MainContent_ddactivitydivision').val("old");
                    setActivitySection(division_id, section_id, location_section_name_th, location_section_name_en);
                } else {

                    //if (division_id == "" || division_id == "00000000") {
                    //    if (pagetype != "view" && pagetype != "edit") {
                    //        $('#MainContent_ddactivitydivision').val(department_id + "D");
                    //        setActivitySection(department_id + "D", section_id, "", "");
                    //    }


                    //} else {

                        $('#MainContent_ddactivitydivision').val(division_id);
                        setActivitySection(division_id, section_id, location_section_name_th, location_section_name_en);
                  //  }
                }

            }
        });



    }



    function setSection(division_id, section_id,location_section_name_th,location_section_name_en)
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
                    
                    if (section_id == "" || section_id == "00000000" || (division_id.indexOf("D") > -1))//มีกรณี ไม่่มี department,division แต่มี section เลยใส่ division_id.indexOf("D")
                    {
                        if (pagetype != "view" && pagetype != "edit") {
                            $('#MainContent_ddsection').val(division_id + "D");

                        }

                        if (pagetype == "edit") {

                            if (division_id.indexOf("D") > -1)
                            {
                                $('#MainContent_ddsection').val(section_id);
                            }

                        }


                    } else {
                        $('#MainContent_ddsection').val(section_id);
                    }
                }

                check_complete = true;
            }
        });



    }



    function setActivitySection(division_id, section_id, location_section_name_th, location_section_name_en) {

        $.ajax({
            type: "POST",
            data: { division: division_id, lang: lang },
            url: 'Masterdata.asmx/getSectionFormbyDivision',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddactivitysection");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


                if (pagetype == "view") {
                    if (lang == "th") {
                        $('#MainContent_ddactivitysection').append($('<option></option>').val("old").html(location_section_name_th));
                    } else {
                        $('#MainContent_ddactivitysection').append($('<option></option>').val("old").html(location_section_name_en));
                    }

                    $('#MainContent_ddactivitysection').val("old");

                } else {

                    //if (section_id == "" || section_id == "00000000") {
                    //    if (pagetype != "view" && pagetype != "edit") {
                    //        $('#MainContent_ddactivitysection').val(division_id + "D");

                    //    }


                    //} else {
                        $('#MainContent_ddactivitysection').val(section_id);
                   // }
                }

                check_complete = true;
            }
        });



    }





  

    function changCompany()
    {
        var ddl_company_id = $('#MainContent_ddcompany').val();
        $("#MainContent_dddivision").val("");
       // MainContent_ddsection
        $("#MainContent_ddsection").val("");

        $("#MainContent_dddepartment").val("");


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



    function changActivityCompany()
    {
        var ddl_company_id = $('#MainContent_ddactivitycompany').val();

        $.ajax({
            type: "POST",
            data: { company: ddl_company_id, lang: lang },
            url: 'Masterdata.asmx/getFuctionFormByCompany',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddactivityfunction");
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



    function changActivityFunction()
    {
        var ddl_function_id = $('#MainContent_ddactivityfunction').val();

        $.ajax({
            type: "POST",
            data: { function: ddl_function_id, lang: lang },
            url: 'Masterdata.asmx/getDepartmentFormbyFunction',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddactivitydepartment");
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


    function changActivityDepartment()
    {
        var ddl_department_id = $('#MainContent_ddactivitydepartment').val();

        $.ajax({
            type: "POST",
            data: { department: ddl_department_id, lang: lang },
            url: 'Masterdata.asmx/getDivisionFormbyDepartment',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddactivitydivision");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                $('#MainContent_ddactivitydivision').val(ddl_department_id + 'D');
                changActivityDivision();
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

                $("#MainContent_ddsection").val(ddl_division_id+"D");
            }
        });



    }



    function changActivityDivision()
    {
        var ddl_division_id = $('#MainContent_ddactivitydivision').val();

        $.ajax({
            type: "POST",
            data: { division: ddl_division_id, lang: lang },
            url: 'Masterdata.asmx/getSectionFormbyDivision',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddactivitysection");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                $("#MainContent_ddactivitysection").val(ddl_division_id + "D");
            }
        });



    }




    function saveIncident()
    {
        Page_ClientValidate();

        var responsible_area = "";
        var owner_activity = "";
        var activity_company_id = "";
        var activity_function_id = "";
        var activity_department_id = "";
        var activity_division_id = "";
        var activity_section_id = "";

        if (country == "thailand") {
            responsible_area = $("input:radio[name='ctl00$MainContent$responsible_area']:checked").val();
            owner_activity = $("input:radio[name='ctl00$MainContent$owner_activity']:checked").val();

            activity_company_id = $("#MainContent_ddactivitycompany").val();
            activity_function_id = $("#MainContent_ddactivityfunction").val();
            activity_department_id = $("#MainContent_ddactivitydepartment").val();
            activity_division_id = $("#MainContent_ddactivitydivision").val();
            activity_section_id = $("#MainContent_ddactivitysection").val();
        }
 
        if (Page_IsValid && ((responsible_area != undefined && country == "thailand") || (responsible_area == "" && country == "srilanka"))
            && ((owner_activity != undefined && country == "thailand") || (owner_activity == "" && country == "srilanka"))) 
        {
            showLoading();
            var hour = $("#MainContent_ddtimehour").val();
            var minute =  $("#MainContent_ddtimeminute").val();
            var incident_date = $("#MainContent_txtincident_date").val();
            var incident_time = hour + ":" + minute;
            var report_date = $("#MainContent_txtreport_date").val();
            var company_id = $("#MainContent_ddcompany").val();
            var function_id = $("#MainContent_ddfunction").val();
            var department_id = $("#MainContent_dddepartment").val();
            var division_id = $("#MainContent_dddivision").val();
            var section_id = $("#MainContent_ddsection").val();
            var incident_area = $("#MainContent_txtincident_area").val();
            var incident_name = $("#MainContent_txtincidentname").val();
            var incident_detail = $("#MainContent_txtincidentdetail").val();
            var phone = $("#MainContent_txtphone").val();
            


            $.ajax({
                type: "POST",
                data: {
                    incidentdate: incident_date,
                    incidenttime: incident_time,
                    reportdate : report_date,
                    company_id: company_id,
                    function_id: function_id,
                    department_id: department_id,
                    division_id: division_id,
                    section_id: section_id,
                    activity_company_id: activity_company_id,
                    activity_function_id: activity_function_id,
                    activity_department_id: activity_department_id,
                    activity_division_id: activity_division_id,
                    activity_section_id: activity_section_id,
                    responsible_area: responsible_area,
                    owner_activity : owner_activity,
                    incidentarea: incident_area,
                    incidentname: incident_name,
                    incidentdetail: incident_detail,
                    userid: user_login_id,
                    typelogin : type_login,
                    phone: phone
                    
                },
                url: 'Actionevent.asmx/createIncident',
                dataType: 'json',
                success: function (id) {

                    closeLoading();
                    var result_save = '<%= Resources.Main.success %>';
                    alert(result_save);

                    var continue_report  = '<%= Resources.Incident.continue_report %>';

                    if (confirm(continue_report))
                    {
                        window.location.href = "incidentform.aspx?pagetype=add";
                        //resumeIncident();

                    } else {
                        window.location.href = "incidentform.aspx?pagetype=view&id=" + id;

                    }
                   
                }
            });
            return false;

        }
        else {

            if ((responsible_area == undefined && country == "thailand")) {
                var require_responsible_area = '<%= Resources.Incident.rqresponsiblearea %>';
                $("#rqresponsiblearea").text(require_responsible_area);
            }

            if ((owner_activity == undefined && country == "thailand")) {
                var require_owner_activity = '<%= Resources.Incident.rqOwneractivity %>';
                $("#rqowneractivity").text(require_owner_activity);
            }

            return false;
        }


    }


   

    function updateIncident()
    {
        Page_ClientValidate();

        var responsible_area = "";
        var owner_activity = "";
        var activity_company_id = "";
        var activity_function_id = "";
        var activity_department_id = "";
        var activity_division_id = "";
        var activity_section_id = "";
        
        if (country == "thailand") {
            responsible_area = $("input:radio[name='ctl00$MainContent$responsible_area']:checked").val();
            owner_activity = $("input:radio[name='ctl00$MainContent$owner_activity']:checked").val();

            activity_company_id = $("#MainContent_ddactivitycompany").val();
            activity_function_id = $("#MainContent_ddactivityfunction").val();
            activity_department_id = $("#MainContent_ddactivitydepartment").val();
            activity_division_id = $("#MainContent_ddactivitydivision").val();
            activity_section_id = $("#MainContent_ddactivitysection").val();

        }


        if (Page_IsValid && ((responsible_area != undefined && country == "thailand") || (responsible_area == "" && country == "srilanka"))
            && ((owner_activity != undefined && country == "thailand") || (owner_activity == "" && country == "srilanka"))) {
            showLoading();
            var hour = $("#MainContent_ddtimehour").val();
            var minute = $("#MainContent_ddtimeminute").val();
            var incident_date = $("#MainContent_txtincident_date").val();
            var incident_time = hour + ":" + minute;
            var report_date = $("#MainContent_txtreport_date").val();
            var company_id = $("#MainContent_ddcompany").val();
            var function_id = $("#MainContent_ddfunction").val();
            var department_id = $("#MainContent_dddepartment").val();
            var division_id = $("#MainContent_dddivision").val();
            var section_id = $("#MainContent_ddsection").val();
        
            var incident_area = $("#MainContent_txtincident_area").val();
            var incident_name = $("#MainContent_txtincidentname").val();
            var incident_detail = $("#MainContent_txtincidentdetail").val();
            var phone = $("#MainContent_txtphone").val();



            $.ajax({
                type: "POST",
                data: {
                    incidentdate: incident_date,
                    incidenttime: incident_time,
                    reportdate: report_date,
                    company_id: company_id,
                    function_id: function_id,
                    department_id: department_id,
                    division_id: division_id,
                    section_id: section_id,
                    activity_company_id: activity_company_id,
                    activity_function_id: activity_function_id,
                    activity_department_id: activity_department_id,
                    activity_division_id: activity_division_id,
                    activity_section_id: activity_section_id,
                    responsible_area: responsible_area,
                    owner_activity : owner_activity,
                    incidentarea: incident_area,
                    incidentname: incident_name,
                    incidentdetail: incident_detail,
                    user_id: user_login_id,
                    typelogin: type_login,
                    phone: phone,
                    incidentid: id,
                    stepform: 1,
                    group_id:user_group_id,

                },
                url: 'Actionevent.asmx/updateIncident',
                dataType: 'json',
                success: function (id) {
                    window.location.href = "incidentform.aspx?pagetype=view&id=" + id;
                }
            });
            return false;

        }
        else {

            if ((responsible_area == undefined && country == "thailand"))
            {
                var require_responsible_area = '<%= Resources.Incident.rqresponsiblearea %>';
                $("#rqresponsiblearea").text(require_responsible_area);
            }

            if ((owner_activity == undefined && country == "thailand")) {
                var require_owner_activity = '<%= Resources.Incident.rqOwneractivity %>';
                $("#rqowneractivity").text(require_owner_activity);
            }
            

            return false;
        }


    }


    function CheckMobilePhoneLength(oSrc, args)
    {
        if (args.Value.length > 10) {

            args.IsValid = false;
        } else {

            args.IsValid = true;
        }


    }


    function showUpdateReasonReject()
    {

        
        dialogReason.dialog("open");

        return false;

    }


    function UpdateReason() 
    {

        var reason_type = $("#ddReasonreject").val();
        var reason = $("#MainContent_txtreasonreject").val();

        if (reason_type != "")
        {
            showLoading();
            $.ajax({
                type: "POST",
                data: {
                    incidentid: id,
                    reason_reject_type: reason_type,
                    reasonreject: reason,
                    userid: user_login_id,
                    typelogin: type_login,
                    step_form: 1,
                    group_id: user_group_id,

                },
                url: 'Actionevent.asmx/updateReasonRejectIncident',
                dataType: 'json',
                success: function (result) {

                    closeLoading();
                    dialogReason.dialog("close");
                    $("#MainContent_txtreasonreject").val("");
                    $("#rqreason").text("");
                    //setShowedit();//update status
                    window.location.href = "incidentform.aspx?pagetype=view&id=" + id;
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



    function validateDateIncident(oSrc, args)
    {

        var case_date = $("#MainContent_txtincident_date").val();
        $.ajax({
            type: "POST",
            data: { casedate: case_date, lang: lang },
            url: 'Actionevent.asmx/checkDateIncidentHazard',
            dataType: 'json',
            async: false,
            cache: false,
            success: function (result) {

                if (result == true) {//วันที่ date < datenow

                    args.IsValid = true;


                } else {

                    args.IsValid = false;

                }

            }
        });


    }



    function CheckActivityCompany(oSrc, args)
    {
    
        var owner_activity = $("input:radio[name='ctl00$MainContent$owner_activity']:checked").val();

        if (owner_activity == "KNOWN")
        {
            if (args.Value.length == 0) {

                args.IsValid = false;
            } else {

                args.IsValid = true;
            }
           

        } else
        {
            args.IsValid = true;

        }
       

    }



    function CheckActivityFunction(oSrc, args)
    {
        var owner_activity = $("input:radio[name='ctl00$MainContent$owner_activity']:checked").val();

        if (owner_activity == "KNOWN") {
          
            if (args.Value.length == 0) {

                args.IsValid = false;
            } else {

                args.IsValid = true;
            }


        } else {
            args.IsValid = true;

        }


    }


    function CheckActivityDepartment(oSrc, args)
    {
        var owner_activity = $("input:radio[name='ctl00$MainContent$owner_activity']:checked").val();

        if (owner_activity == "KNOWN") {

            if (args.Value.length == 0) {

                args.IsValid = false;
            } else {

                args.IsValid = true;
            }


        } else {
            args.IsValid = true;

        }


    }


    function CheckActivityDivision(oSrc, args)
    {
        var owner_activity = $("input:radio[name='ctl00$MainContent$owner_activity']:checked").val();

        if (owner_activity == "KNOWN") {

            if (args.Value.length == 0) {

                args.IsValid = false;
            } else {

                args.IsValid = true;
            }


        } else {
            args.IsValid = true;

        }


    }



    function CheckActivitySection(oSrc, args)
    {
        var owner_activity = $("input:radio[name='ctl00$MainContent$owner_activity']:checked").val();

        if (owner_activity == "KNOWN") {

            if (args.Value.length == 0) {

                args.IsValid = false;
            } else {

                args.IsValid = true;
            }


        } else {
            args.IsValid = true;

        }


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
  
            <div class="ibox float-e-margins">
                
                <div class="ibox-content" style="display: block;">

               
<div class="stepwizard">
      <div class="stepwizard-row setup-panel">
        <div class="stepwizard-step">
            <asp:LinkButton ID="step1" runat="server" CssClass="btn btn-primary btn-circle" CausesValidation="False" OnClick="step1_Click">1</asp:LinkButton>
        <p><%= Resources.Incident.incidentstep1 %></p>
        </div>
        <div class="stepwizard-step">
        <asp:LinkButton ID="step2" runat="server" CssClass="btn btn-default btn-circle" CausesValidation="False" OnClick="step2_Click">2</asp:LinkButton>
            <p><%= Resources.Incident.incidentstep2 %></p>
        </div>
        <div class="stepwizard-step">
         <asp:LinkButton ID="step3" runat="server" CssClass="btn btn-default btn-circle" CausesValidation="False" OnClick="step3_Click">3</asp:LinkButton>
                            
        <p><%= Resources.Incident.incidentstep3 %></p>
        </div>
         <div class="stepwizard-step">
            <asp:LinkButton ID="step4" runat="server" CssClass="btn btn-default btn-circle" CausesValidation="False" OnClick="step4_Click">4</asp:LinkButton>
                            
        <p><%= Resources.Incident.incidentstep4 %></p>
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
                        
                            ArrayList per4 = Session["permission"] as ArrayList;
                           
                           
                            if (per4.IndexOf("reopen incident") > -1)         
                           {                            
       
                          %>
                              <asp:Button ID="btReopenIncident" runat="server" Text="<%$ Resources:Main, btReopen %>" CssClass="btn btn-primary" CausesValidation="False" OnClick="btReopenIncident_Click"/>

                          <%                      
                            }
       
                          %>


                         <%
                            string PageType = Request.QueryString["PageType"];
                            string  id = "";

                            if (PageType == "edit" || PageType == "view")
                            {
                                 id = Request.QueryString["id"];
                            }
                            ArrayList per = Session["permission"] as ArrayList;
                           
                            bool pa = safetys4.Class.SafetyPermission.checkPermisionAction("report incident1 verify", id, "incident", Convert.ToInt32(Session["group_value"]));
                            bool area = safetys4.Class.SafetyPermission.checkPermisionInArea(id, "incident");
                           
                            if (per.IndexOf("report incident1 verify") > -1 && pa == true && area == true)         
                           {                            
       
                          %>
                              <asp:Button ID="btIncidentcheck" runat="server" Text="<%$ Resources:Incident, btIncidentcheck %>" CssClass="btn btn-primary" CausesValidation="False" OnClick="btIncidentcheck_Click"/>

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
                            
                            bool pa2 = safetys4.Class.SafetyPermission.checkPermisionAction("report incident1 reject", id2, "incident", Convert.ToInt32(Session["group_value"]));
                            bool area2 = safetys4.Class.SafetyPermission.checkPermisionInArea(id2, "incident");


                            if (per2.IndexOf("report incident1 reject") > -1 && pa2 == true && area2 == true)         
                           {                            
       
                          %>
                             <asp:Button ID="btIncidentreject" runat="server" Text="<%$ Resources:Incident, btIncidentreject %>" CssClass="btn btn-primary" CausesValidation="False" OnClientClick="return showUpdateReasonReject();"/>

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
                            string PageType3 = Request.QueryString["PageType"];
                            string id3 = "";

                            if (PageType3 == "edit" || PageType == "view")
                            {
                                id3 = Request.QueryString["id"];
                            }
                            ArrayList per3 = Session["permission"] as ArrayList;
                           
                            bool pa3 = safetys4.Class.SafetyPermission.checkPermisionAction("report incident1 edit", id3, "incident", Convert.ToInt32(Session["group_value"]));
                            bool area3 = safetys4.Class.SafetyPermission.checkPermisionInArea(id3, "incident");

                            if (per3.IndexOf("report incident1 edit") > -1 && pa3 == true && area3 == true)         
                           {                            
       
                          %>
                             <asp:Button ID="btIncidentedit" runat="server" Text="<%$ Resources:Incident, btIncidentedit %>" CssClass="btn btn-primary"  CausesValidation="False" OnClick="btIncidentedit_Click"/>

                          <%                      
                            }
       
                          %>
                    </div>
                    </div>
                  </div>
                           
                
                
                
                 <div class="row" style="padding-bottom:10px;"> <div class="col-md-4"><strong>
                         <h3><%= Resources.Incident.HeaderDate %></h3></strong></div></div>

                			<div class="row">
                                <div class="col-md-5">
                                     <div id="data_incident_date" class="form-group">
                                        <label class="control-label"><%= Resources.Incident.incident_date %></label><div class="lbrequire"> *</div>
                                          <div class="input-group date">
                                         <input class="form-control" value="" type="text" id="txtincident_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                                          </div>
                                           <asp:RequiredFieldValidator ID="rqincidentdate" runat="server" ControlToValidate ="txtincident_date" ErrorMessage="<%$ Resources:Incident, rqincidentdate %>" CssClass="text-danger" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                          <asp:CustomValidator id="rqincidentdate2" runat="server"  ControlToValidate = "txtincident_date" ErrorMessage = "<%$ Resources:Incident, rqincidentdate2 %>"  CssClass="text-danger"  Display="Dynamic"  ClientValidationFunction="validateDateIncident" ValidateEmptyText="true" >
                                        </asp:CustomValidator>		
                                    </div>

                                </div>
                                <div class="col-md-2">
                                     <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.incident_time %></label><div class="lbrequire"> *</div>
                                         <div class="form-inline">
                                             <select id="ddtimehour" name="ddtimehour" class="form-control"  runat="server">
                                                 <option value="00">00</option>
                                                 <option value="01">01</option>
                                                 <option value="02">02</option>
                                                 <option value="03">03</option>
                                                 <option value="04">04</option>
                                                 <option value="05">05</option>
                                                 <option value="06">06</option>
                                                 <option value="07">07</option>
                                                 <option value="08">08</option>
                                                 <option value="09">09</option>
                                                 <option value="10">10</option>
                                                 <option value="11">11</option>
                                                 <option value="12">12</option>
                                                 <option value="13">13</option>
                                                 <option value="14">14</option>
                                                 <option value="15">15</option>
                                                 <option value="16">16</option>
                                                 <option value="17">17</option>
                                                 <option value="18">18</option>
                                                 <option value="19">19</option>
                                                 <option value="20">20</option>
                                                 <option value="21">21</option>
                                                 <option value="22">22</option>
                                                 <option value="23">23</option>
                                            </select>

                                        
                                              <select id="ddtimeminute" name="ddtimeminute" class="form-control"  runat="server">
                                                 <option value="00">00</option>
                                                 <option value="01">01</option>
                                                 <option value="02">02</option>
                                                 <option value="03">03</option>
                                                 <option value="04">04</option>
                                                 <option value="05">05</option>
                                                 <option value="06">06</option>
                                                 <option value="07">07</option>
                                                 <option value="08">08</option>
                                                 <option value="09">09</option>
                                                 <option value="10">10</option>
                                                 <option value="11">11</option>
                                                 <option value="12">12</option>
                                                 <option value="13">13</option>
                                                 <option value="14">14</option>
                                                 <option value="15">15</option>
                                                 <option value="16">16</option>
                                                 <option value="17">17</option>
                                                 <option value="18">18</option>
                                                 <option value="19">19</option>
                                                 <option value="20">20</option>
                                                 <option value="21">21</option>
                                                 <option value="22">22</option>
                                                 <option value="23">23</option>

                                                
                                                 <option value="24">24</option>
                                                 <option value="25">25</option>
                                                 <option value="26">26</option>
                                                 <option value="27">27</option>
                                                 <option value="28">28</option>
                                                 <option value="29">29</option>
                                                 <option value="30">30</option>
                                                 <option value="31">31</option>
                                                 <option value="32">32</option>
                                                 <option value="33">33</option>
                                                 <option value="34">34</option>
                                                 <option value="35">35</option>
                                                 <option value="36">36</option>
                                                 <option value="37">37</option>
                                                 <option value="38">38</option>
                                                 <option value="39">39</option>
                                                 <option value="40">40</option>

                                               
                                                 <option value="41">41</option>
                                                 <option value="42">42</option>
                                                 <option value="43">43</option>
                                                 <option value="44">44</option>
                                                 <option value="45">45</option>
                                                 <option value="46">46</option>
                                                 <option value="47">47</option>
                                                 <option value="48">48</option>
                                                 <option value="49">49</option>
                                                 <option value="50">50</option>

                                                 <option value="51">51</option>
                                                 <option value="52">52</option>
                                                 <option value="53">53</option>
                                                 <option value="54">54</option>
                                                 <option value="55">55</option>
                                                 <option value="56">56</option>
                                                 <option value="57">57</option>
                                                 <option value="58">58</option>
                                                 <option value="59">59</option>
                                                
                                                
                                               </select>

                                         </div>
                                    
                                          <asp:RequiredFieldValidator ID="rqincidenttime" runat="server" ControlToValidate ="ddtimehour" ErrorMessage="<%$ Resources:Incident, rqincidenttime %>" CssClass="text-danger" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                     </div>
                                </div>
                                <div class="col-md-5">
                                     <div id="data_report_date" class="form-group">
                                       <label class="control-label"><%= Resources.Incident.report_date %></label><div class="lbrequire"> *</div>
                                 
                                         <input class="form-control" value="" type="text" id="txtreport_date" runat="server">
                                        
                                     </div>
                                </div>
                          
                            </div>
                          
                    <div class="incident_responsible_area_view">
                            <div class="row">
                                <div class="col-md-12">
                                    <label class="control-label"><strong><h3><%= Resources.Incident.responsible_area %></h3></strong></label><div class="lbrequire"> *</div>    
                                     <div class="form-group">
                                        
                                        <div class="col-sm-6">
                                          <label> <input value="IN" id="responsible_area1" name="responsible_area" type="radio" runat="server">
                                            <%= Resources.Incident.onsite %></label>
                                        </div>
                                        <div class="col-sm-6">
                                         <label> <input value="OUT" id="responsible_area2" name="responsible_area" type="radio" runat="server">
                                            <%= Resources.Incident.offsite %></label>
                                        </div>
                                         <label id="rqresponsiblearea" class="text-danger"></label>  
                                    </div>

                                </div>
                              
                            </div>

                     </div>
                         <div class="incident_view">

                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbCompany %></label><div class="lbrequire"> *</div>                            

                                       <select id="ddcompany" name="ddcompany" class="form-control" onchange="changCompany();" runat="server">
                       
                                        </select>
                                            <asp:RequiredFieldValidator ID="rqCompany" runat="server" ControlToValidate ="ddcompany" ErrorMessage="<%$ Resources:Incident, rqcompany %>" CssClass="text-danger" Display="Dynamic">
                                      </asp:RequiredFieldValidator>
                                    
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbfucntion %></label><div class="lbrequire"> *</div>
                                    
                                     <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server">
                       
                                        </select>
                                  
                                       <asp:RequiredFieldValidator ID="rqFunction" runat="server" ControlToValidate ="ddfunction" ErrorMessage="<%$ Resources:Incident, rqfunction %>" CssClass="text-danger" Display="Dynamic">
                                      </asp:RequiredFieldValidator>
                                    </div>
                                </div>
                              
                            </div>



                          <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbdepartment %></label><div class="lbrequire"> *</div>                                               
                                    
                                        <select id="dddepartment" class="form-control" onchange="changDepartment();" runat="server">
                       
                                        </select>
                                            <asp:RequiredFieldValidator ID="rqDepartment" runat="server" ControlToValidate ="dddepartment" ErrorMessage="<%$ Resources:Incident, rqdepartment %>" CssClass="text-danger" Display="Dynamic">
                                      </asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbdivision %></label>
                                        
                                         <select id="dddivision" class="form-control" onchange="changDivision();" runat="server">
                       
                                        </select>
                                   </div>
                                </div>

                               <div class="col-md-4">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbsection %></label>
                                    
                                    
                                     <select id="ddsection" class="form-control"  runat="server">
                       
                                        </select>
                                    </div>
                                </div>

                              	
                            </div>
                        </div>

                            <div class="row">
                              

                               <div class="col-md-12">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.incidentarea %></label>
                                         <%
                                          if (Session["country"].ToString() =="thailand")
                                          {                  
                                        %>  
                                            <div class="lbrequire"> *</div>    
                                         <%
                                         }           
                                        %>
                                        <input id="txtincident_area" name="txtincident_area"  type="text" class="form-control" runat="server">
                                         <%
                                          if (Session["country"].ToString() =="thailand")
                                          {                  
                                        %>  
                                              <asp:RequiredFieldValidator ID="rqIncidentArea" runat="server" ControlToValidate ="txtincident_area" ErrorMessage="<%$ Resources:Incident, rqincidentarea %>" CssClass="text-danger" Display="Dynamic">
                                              </asp:RequiredFieldValidator>
                                       <%
                                         }           
                                        %>
                                    </div>
                                </div>

                              				
                            </div>


                    <div class="incident_owner_activity_view">
                            <div class="row">
                                <div class="col-md-12">
                                    <label class="control-label"><strong><h3><%= Resources.Incident.owner_activity %></h3></strong></label><div class="lbrequire"> *</div>    
                                     <div class="form-group">
                                        
                                        <div class="col-sm-6">
                                          <label> <input value="KNOWN" id="owner_activity1" name="owner_activity" type="radio" runat="server">
                                            <%= Resources.Incident.owner_activity_known %></label>
                                        </div>
                                        <div class="col-sm-6">
                                         <label> <input value="UNKNOWN" id="owner_activity2" name="owner_activity" type="radio" runat="server">
                                            <%= Resources.Incident.owner_activity_unknown %></label>
                                        </div>
                                         <label id="rqowneractivity" class="text-danger"></label>  
                                    </div>

                                </div>
                              
                            </div>

                     </div>


                    <div class="incident_activity_view">

                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbActivityCompany %></label><div class="lbrequire acivity-view"> *</div>                            

                                       <select id="ddactivitycompany" name="ddactivitycompany" class="form-control" onchange="changActivityCompany();" runat="server">
                       
                                        </select>

                                         <asp:CustomValidator id="rqActivityCompany" runat="server" ClientValidationFunction="CheckActivityCompany" Display="Dynamic" ControlToValidate="ddactivitycompany" ValidateEmptyText="True" ErrorMessage="<%$ Resources:Incident, rqActivityCompany %>" CssClass="text-danger"></asp:CustomValidator>

                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbActivityFunction %></label><div class="lbrequire acivity-view"> *</div>
                                    
                                     <select id="ddactivityfunction" class="form-control" onchange="changActivityFunction();" runat="server">
                       
                                        </select>
                                    <asp:CustomValidator id="rqActivityFucntion" runat="server" ClientValidationFunction="CheckActivityFunction" Display="Dynamic" ControlToValidate="ddactivityfunction" ValidateEmptyText="True"  ErrorMessage="<%$ Resources:Incident, rqActivityFucntion %>" CssClass="text-danger"></asp:CustomValidator>

                                    </div>
                                </div>
                              
                            </div>



                          <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbActivityDepartment %></label><div class="lbrequire acivity-view"> *</div>                                               
                                    
                                        <select id="ddactivitydepartment" class="form-control" onchange="changActivityDepartment();" runat="server">
                       
                                        </select>
                                          <asp:CustomValidator id="rqActivityDepartment" runat="server" ClientValidationFunction="CheckActivityDepartment" Display="Dynamic" ControlToValidate="ddactivitydepartment" ValidateEmptyText="True"  ErrorMessage="<%$ Resources:Incident, rqActivityDepartment %>" CssClass="text-danger"></asp:CustomValidator>

                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbActivityDivision %></label><div class="lbrequire acivity-view"> *</div>     
                                        
                                         <select id="ddactivitydivision" class="form-control" onchange="changActivityDivision();" runat="server">
                       
                                        </select>
                                          <asp:CustomValidator id="rqActivityDivision" runat="server" ClientValidationFunction="CheckActivityDivision" Display="Dynamic" ControlToValidate="ddactivitydivision" ValidateEmptyText="True"  ErrorMessage="<%$ Resources:Incident, rqActivityDivision %>" CssClass="text-danger"></asp:CustomValidator>

                                   </div>
                                </div>

                               <div class="col-md-4">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbActivitySection %></label><div class="lbrequire acivity-view"> *</div>     
                                    
                                    
                                     <select id="ddactivitysection" class="form-control"  runat="server">
                       
                                        </select>
                                          <asp:CustomValidator id="rqActivitySection" runat="server" ClientValidationFunction="CheckActivitySection" Display="Dynamic" ControlToValidate="ddactivitysection" ValidateEmptyText="True"  ErrorMessage="<%$ Resources:Incident, rqActivitySection %>" CssClass="text-danger"></asp:CustomValidator>

                                    </div>
                                </div>

                              	
                            </div>
                        </div>



                   <div class="row">
                    <div class="col-md-12">
                       <strong>
                         <h3> <asp:Label ID="lbDescriptionHeader" runat="server" Text="<%$ Resources:Incident, lbDescriptionHeader %>"></asp:Label></h3></strong>
                   
                    </div>
                  </div>

                   <div class="row">
                              

                        <div class="col-md-12">
                            <div class="form-group">
                            <label class="control-label"><%= Resources.Incident.incidentname %> (500 <%= Resources.Incident.rqCharacters %>)</label><div class="lbrequire"> * </div>
                            <input id="txtincidentname" name="txtincidentname"  type="text" class="form-control" runat="server">
                                <asp:RequiredFieldValidator ID="rqincidentname" runat="server" ControlToValidate ="txtincidentname" ErrorMessage="<%$ Resources:Incident, rqincidentname %>" CssClass="text-danger" Display="Dynamic">
                                </asp:RequiredFieldValidator>
                                <asp:CustomValidator id="rqIncidentTitleLength" runat="server" ClientValidationFunction="CheckIncidentTitleLength" Display="Dynamic" ControlToValidate="txtincidentname"  ErrorMessage="<%$ Resources:Incident, rqIncidentTitleLength %>" CssClass="text-danger"></asp:CustomValidator>

                            </div>
                        </div>

                              				
                   </div>

                 <div class="row">
                     <div class="col-md-12">
                           <div class="form-group">
                                <label class="control-label"><%= Resources.Incident.incidentdetail %> (4000 <%= Resources.Incident.rqCharacters %>)</label><div class="lbrequire"> * </div>
                           
                                <textarea class="form-control" rows="5" id="txtincidentdetail" runat="server"></textarea>
                                 <asp:RequiredFieldValidator ID="rqincidentdetail" runat="server" ControlToValidate ="txtincidentdetail" ErrorMessage="<%$ Resources:Incident, rqincidentdetail %>" CssClass="text-danger" Display="Dynamic">
                                </asp:RequiredFieldValidator>

                                <asp:CustomValidator id="rqIncidentDetailLength" runat="server" ClientValidationFunction="CheckIncidentDetailLength" Display="Dynamic" ControlToValidate="txtincidentdetail"  ErrorMessage="<%$ Resources:Incident, rqIncidentDetailLength %>" CssClass="text-danger"></asp:CustomValidator>

                            </div>
                            </div>

                              				
                       </div>
                </div>
                  <div class="row">
                  <div class="col-md-12">
                       <div class="form-group">
                            <label id="lbshowimage" class="control-label" runat="server"><%= Resources.Incident.showfile %></label>
                            <div id="showimage">
                                
                            </div>
                        </div>
                   </div>
                 </div>
                 <div class="row">
                  <div class="col-md-12">
                      <div class="lbrequire" id="infouploadimage"> <%= Resources.Incident.infoimage %></div>
                       <div class="form-group">
                                                    
                                <div  class="dropzone" id="dropzoneForm" style="margin-top:8px;">
                                    <div class="fallback">
                                        <input name="file" type="file" multiple="multiple" />
                                       <input type="submit" value="Upload" />
                                    </div>
                                </div>
                          
                        </div>
                        </div>
                     </div>
                              				
                 <div class="row">
                    <div class="col-md-12">
                        <strong>
                         <h3><asp:Label ID="lbnameReportHeader" runat="server" Text="<%$ Resources:Incident, lbNameReportHeader %>"></asp:Label></h3></strong>
                   
                    </div>
                  </div>

                  <div class="row">                             
                    <div class="col-md-4">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.incidentnamesurname %></label>
                        <input id="txtname_surname" name="txtname_surname"  type="text" class="form-control" runat="server">

                              
                        </div>
                    </div>

                      <div class="col-md-4">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.incidentphone %></label><div class="lbrequire"> *</div>
                        <input id="txtphone" name="txtphone"  type="text" class="form-control" placeholder="0899999999" runat="server">
                        
                        <asp:RequiredFieldValidator ID="rqmobilephone" runat="server" ControlToValidate ="txtphone" ErrorMessage="<%$ Resources:Incident, rqmobilephone %>" CssClass="text-danger"  Display="Dynamic">
                                                    </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="rqformphone" runat="server" ErrorMessage="<%$ Resources:Incident, rqmobileformphone %>" ControlToValidate="txtphone"  Display="Dynamic" CssClass="text-danger"
                            ValidationExpression= "^[0-9]*$"></asp:RegularExpressionValidator>
                        <asp:CustomValidator id="rqmobilephonelength" runat="server" ClientValidationFunction="CheckMobilePhoneLength" ControlToValidate="txtphone" Display="Dynamic" ErrorMessage="<%$ Resources:Incident, rqmobilephonelength %>" CssClass="text-danger"></asp:CustomValidator>

                              
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
                <asp:Button ID="btSubmit" runat="server" Text="<%$ Resources:Main, btSubmit %>"  CssClass="btn btn-primary" OnClientClick="return saveIncident();" />  
                <asp:Button ID="btUpdate" runat="server" Text="<%$ Resources:Main, btSubmit %>"  CssClass="btn btn-primary" OnClientClick="return updateIncident();" />    
                 

               </div>
            </div>
              

            </div>
        </div>
    </div>
 



                       
                </div>
           

</asp:Content>
