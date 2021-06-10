<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="incidentform3.aspx.cs" Inherits="safetys4.incidentform3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

<link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
<link href="template/css/plugins/dropzone/dropzone.css" rel="stylesheet" />
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
    color:white;
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


.row {
 
    padding-bottom: 20px !important;
}



.btn-circle-new {
    border-bottom-left-radius: 15px;
    border-bottom-right-radius: 15px;
    border-top-left-radius: 15px;
    border-top-right-radius: 15px;
    font-size: 12px;
    height: 20px;
    line-height: 1.42857;
    padding-bottom: 6px;
    padding-left: 0;
    padding-right: 0;
    padding-top: 2px;
    text-align: center;
    width: 20px;
}

.a-step{
    color:black !important;
}



     a {
        color:black ;
    }


    #tbDefinitionLevel tbody tr {
         cursor: pointer;
    }

  .selected_colunm {
    background-color: #B0BED9;
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
    var dialogFactFinding;
    var dialogCorrectivePreventive;
    var dialogRootCauseAction;
    var dataTableFactFinding; //reference to your dataTable
    var dataTableCorrectivePreventive; //reference to your dataTable
    var dataTablePreventive; //reference to your dataTable
    var dataTableConsequence; //reference to your dataTable
    var dataTableRootCauseAction;

    var filename_fact = "";
    var filename_corrective = "";
    var filename_investigation_committee="";

    var fact_id = 0;
    var corrective_id = 0;
    var root_cause_id = 0;

    var drop_fact;

    var dialogReason;
    var dialogReasonExcept;

    var action_reason_id = 0;
    var type_action = "";
   

    $(document).ready(function () {

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
            setShowfile();
            $("#dropzoneForm").hide();

        } else if (pagetype == "edit") {
          
            setShowedit();
            $("#dropzoneForm").show();
        }


        dialogFactFinding = $("#fact_finding_form").dialog({
            autoOpen: false,
            height: 580,
            width: 650,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {
                //clearValidationErrors();

                $("#fact_finding_form").css('overflow-x', 'hidden');
            },
            modal: true,
        });



        dialogCorrectivePreventive = $("#corrective_preventive_form").dialog({
            autoOpen: false,
            height: 650,
            width: 650,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {
                //clearValidationErrors();
               // setRootCauseAction();
                $("#corrective_preventive_form").css('overflow-x', 'hidden');
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



        dialogRootCauseAction = $("#root_cause_action_form").dialog({
            autoOpen: false,
            height: 290,
            width: 400,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {

                $("#root_cause_action_form").css('overflow-x', 'hidden');
            },
            modal: true,
        });


        dialogReasonExcept = $("#create_reason_except").dialog({
            autoOpen: false,
            height: 380,
            width: 400,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {

                $("#create_reason_except").css('overflow-x', 'hidden');
            },
            modal: true,
        });



       
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

        //$('#date_complete .input-group.date').datepicker({
        //    todayBtn: "linked",
        //    keyboardNavigation: false,
        //    forceParse: false,
        //    autoclose: true,
        //    format: "dd/mm/yyyy"
        //});


        $("input[name='ctl00$MainContent$culpability']").change(function (e) {

            if ($(this).val() == "N") {

                $(".row-roadaccident").hide();
               
            } else{

                $(".row-roadaccident").show();


            }

        });



        $("#MainContent_txtresponsible_person").autocomplete({
            source: "Masterdata.asmx/getEmployeeautocompleteofaction",
            select: function (event, ui) {
                $("#MainContent_employee_id").val(ui.item.employee_id);

            }
        });



        $("#MainContent_txtnotyfy_contractor").autocomplete({
            source: "Masterdata.asmx/getContractorautocomplete",
            select: function (event, ui) {
                $("#MainContent_contractor_id").val(ui.item.id);

            }
        });

        setDropzoneForm();
        setDropzoneFact();
      //  setDropzoneCorrective();
      
        setSourceIncident();
        setEventExposure();

        setDatatableCorrectivePreventive();
        setDatatablePreventive();
        setDatatableConsequence();

        setDatatableFactFinding();
        setDatatableRootCauseAction();

        setReasonExcept();


       
        
    });



     function CheckIncidentContributingFactorLength(oSrc, args)
     {
         if (args.Value.length > 1000) {

             args.IsValid = false;
         } else {

             args.IsValid = true;
         }


     }


    function setDropzoneForm()
    {
      
        Dropzone.options.dropzoneForm = {
            maxFiles: 1,
            maxFilesize: 20,
            url: "dropzoneuploadform3.aspx?id=" + id,
            acceptedFiles: "image/jpeg,image/png,.pdf",
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
                        e.preventDefault();
                        e.stopPropagation();
                        // Remove the file preview.
                        _this.removeFile(file);
                        // If you want to the delete the file on the server as well,
                        // you can do the AJAX request here.
                    });

                    // Add the button to the file preview element.
                    file.previewElement.appendChild(removeButton);
                });


                this.on("success", function (file, response) {
                    filename_investigation_committee = response;

                });


            }
        };

    }



    function setDropzoneFact()
    {

        drop_fact = Dropzone.options.dropzoneFact = {
            maxFiles: 1,
            maxFilesize: 5,
            url: "dropzoneuploadfact.aspx?id=" + id,
            acceptedFiles: "image/jpeg,image/png,.pdf",
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
                        // Make sure the button click doesn't submit the form:
                        e.preventDefault();
                        e.stopPropagation();
                        // Remove the file preview.
                        _this.removeFile(file);
                        // If you want to the delete the file on the server as well,
                        // you can do the AJAX request here.
                    });

                    document.querySelector("button#MainContent_btCloseFactFinding").addEventListener("click", function () {
                     
                        _this.removeAllFiles();
                     
                    });

                    document.querySelector("#MainContent_btCreatfactfinding").addEventListener("click", function () {
                       
                        _this.removeAllFiles();
                       
                    });

                    document.querySelector("#MainContent_btUpdatefactfinding").addEventListener("click", function () {

                        _this.removeAllFiles();

                    });


                    // Add the button to the file preview element.
                    file.previewElement.appendChild(removeButton);
                });

              
                this.on("success", function (file, response) {
                    var obj = $.parseJSON(response);

                    filename_fact = obj.name;

                    file.newname = obj.name;
                    file.folderimage = obj.folder;
                    //console.log(file.name);

                    setTimeout(function () {

                        var image_name = "<%=System.Configuration.ConfigurationManager.AppSettings["pathimage"]  %>" + "/incident/step3/" + "<%= Session["country"].ToString()%>" + "/" + obj.folder + "/" + obj.name;

                        $("img[alt='" + file.name + "']").wrap("<a class='fancybox' href='" + image_name + "'/>");

                        $('.fancybox').fancybox();

                    }, 1000);
                });
            }
            
            
        };

    }




    function setDropzoneCorrective()
    {

        Dropzone.options.dropzoneCorrective = {
            maxFiles: 1,
            maxFilesize: 5,
            url: "dropzoneuploadcorrective.aspx?id=" + id,
            acceptedFiles: ".pdf",
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

                    document.querySelector("#MainContent_btCreateCorrective").addEventListener("click", function () {

                        _this.removeAllFiles();

                    });


                    document.querySelector("#MainContent_btUpdateCorrective").addEventListener("click", function () {

                      _this.removeAllFiles();

                   });

                    // Add the button to the file preview element.
                    file.previewElement.appendChild(removeButton);
                });

                this.on("success", function (file, response) {
                    filename_corrective = response;
                    //alert(response);
                });

            }
            
        };

    }

    function closeFactFinding()
    {
        dialogFactFinding.dialog("close");
        clearValidationErrors();
        clearFactFinding();

    }


    function closeCorrectivePreventive()
    {
        dialogCorrectivePreventive.dialog("close");
        clearValidationErrors();
        clearCorrectivePreventive();
    }


    function closeRootCauseAction()
    {
        dialogRootCauseAction.dialog("close");
        clearRootCauseAction();
    }



    function clearFactFinding()
    {
        $("#MainContent_txtfact_finding").val("");
        $("#MainContent_ddlSourceincident").val("");
        $("#MainContent_ddlEventexposure").val("");
        $('#MainContent_chUnsafeaction').prop('checked', false);
        $('#MainContent_chUnsafecondition').prop('checked', false);
       
      
        fact_id = 0;
        filename_fact = "";
    }

    function clearCorrectivePreventive()
    {
        $("#MainContent_txtRootcauseaction").val("");
        $("#MainContent_txtcorrective_preventive").val("");
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


    function clearRootCauseAction()
    {
        $("#MainContent_txtrootcause_action").val("");
       
        root_cause_id = 0;
       
    }

    function clearValidationErrors()
    {

        var i;
        for (i = 0; i < Page_Validators.length; i++) {

            Page_Validators[i].style.display = "none";

        }
    }



    function showCreateFactFinding()
    {
        
        $("#MainContent_btCreatfactfinding").show();
        $("#MainContent_btUpdatefactfinding").hide();
        dialogFactFinding.dialog("open");

        return false;

    }

    function showCreateCorrectivePreventive()
    {
        $("#MainContent_btCreateCorrective").show();
        $("#MainContent_btUpdateCorrective").hide();
        type_action = "corrective";
        $('#lb_main_action').html("<%= Resources.Incident.corrective_preventive_form %>");
        dialogCorrectivePreventive.dialog("option", "title", "<%= Resources.Incident.corrective_preventive_form %>").dialog('open');

        return false;

    }


    function showCreateConsequence()
    {
        $("#MainContent_btCreateCorrective").show();
        $("#MainContent_btUpdateCorrective").hide();
        type_action = "consequence";
        $('#lb_main_action').html("<%= Resources.Incident.lb_consequence %>");
        dialogCorrectivePreventive.dialog("option", "title", "<%= Resources.Incident.lb_consequence %>").dialog('open');

        return false;

    }


    function showCreatePreventive()
    {
        $("#MainContent_btCreateCorrective").show();
        $("#MainContent_btUpdateCorrective").hide();
        type_action = "preventive";
        $('#lb_main_action').html("<%= Resources.Incident.lb_preventive %>");
        dialogCorrectivePreventive.dialog("option", "title", "<%= Resources.Incident.lb_preventive %>").dialog('open');

        return false;

    }



    function showCreateRootCauseAction()
    {
        $("#MainContent_btAddRootCauseAction").show();
        $("#MainContent_btUpdateRootCauseAction").hide();
        dialogRootCauseAction.dialog("open");

        return false;

    }





    function setShowedit()
    {

        $.ajax({
            type: "POST",
            data: { id: id, lang: lang },
            url: 'Actionevent.asmx/getIncidentbyid',
            dataType: 'json',
            success: function (json) {

                $.each(json, function (value, key) {

                    // $("#MainContent_txtincident_date").val(key.incident_date);

                    $("#MainContent_lbEmployee").text(key.name_modify);
                    $("#MainContent_lbUpdate").text(key.datetime_modify);

                    $("#show_doc_status").html(key.doc_no + ' ' + key.status);

                    if (key.culpability != null) {
                        $("input[name='ctl00$MainContent$culpability'][value='" + key.culpability + "']").attr('checked', 'checked');

                        if (key.culpability == "N") {

                            $(".row-roadaccident").hide();

                        } else {

                            $(".row-roadaccident").show();


                        }
                    }
                    
                    if (key.road_accident != null) {
                        $("input[name='ctl00$MainContent$roadaccident'][value='" + key.road_accident + "']").attr('checked', 'checked');
                    }

                  
                    var root_causes = key.root_cause;
                    $('#root_cause').find(':checkbox[name^="ctl00$MainContent$rootcause"]').each(function () {
                        $(this).prop("checked", ($.inArray($(this).val(), root_causes) != -1));

                    });

                    $("#MainContent_txtcontributing_factor").val(key.contributing_factor);

                    $("#MainContent_ddfatality").val(key.fatality_prevention_element_id);
                    $("#MainContent_txtother_please").val(key.faltality_prevention_element_other);

                    setFatalityPrevent(key.fatality_prevention_element_id);
                    setFunction(key.form2_function_id, key.form3_department_id)



                  
                });





            }
        });

    }



    function updateIncident()
    {
        var culpability = $("input:radio[name='ctl00$MainContent$culpability']:checked").val();
      
        if (Page_ClientValidate("incident") && culpability != undefined)
        {
            showLoading();
            //var culpability = $("input:radio[name='ctl00$MainContent$culpability']:checked").val();
            //if (culpability == undefined) {
            //    culpability = "";
            //}


            var road_accident = $("input:radio[name='ctl00$MainContent$roadaccident']:checked").val();
            if (road_accident == undefined) {
                road_accident = "";
            }


            var ddfatality = $("#MainContent_ddfatality").val();
            var txtother_please = $("#MainContent_txtother_please").val();

            var root_causes = [];
            $('#root_cause input:checked').each(function () {
                root_causes.push(this.value);
            });

            var contributing_factor = $("#MainContent_txtcontributing_factor").val();
            var function_id = $("#MainContent_ddfunction").val();
            var department_id = $('#MainContent_dddepartment').val();

            var data_post = JSON.stringify({
                culpability: culpability,
                road_accident: road_accident,
                root_cause: root_causes,
                fatality_prevention_element_id: ddfatality,
                faltality_prevention_element_other: txtother_please,
                contributing_factor: contributing_factor,
                form2_function_id: function_id,
                form3_department_id: department_id,
                user_id: user_login_id,
                typelogin: type_login,
                incidentid: id,
                investigation_committee_file: filename_investigation_committee,
                group_id: user_group_id
            });

            $.ajax({
                type: "POST",
                data: data_post,
                url: 'Actionevent.asmx/updateIncident3',
                contentType: "application/json; charset=utf-8",
                success: function (id) {
                    window.location.href = "incidentform3.aspx?pagetype=view&id=" + id;
                }
            });


            return false;

        } else {

            var require_culpability = '<%= Resources.Incident.rqculpability %>';
            $("#rqculpability").text(require_culpability);

            return false;

        }

           

    }


    function setFunction(function_id,department_id)
    {
        $.ajax({
            type: "POST",
            data: { lang: lang ,pagetype:pagetype},
            url: 'Masterdata.asmx/getFuctionlistform3',
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

                $('#MainContent_ddfunction').val(function_id);

                setDepartment(function_id, department_id);
              
            }
        });



    }



    function setDepartment(function_id, department_id)
    {

        $.ajax({
            type: "POST",
            data: { function_id: function_id, lang: lang, pagetype: pagetype },
            url: 'Masterdata.asmx/getDepartmentlistform3',
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


                $('#MainContent_dddepartment').val(department_id);

            }
        });



    }



    function changFunction()
    {
        var ddl_function_id = $('#MainContent_ddfunction').val();

        $.ajax({
            type: "POST",
            data: { function_id: ddl_function_id, lang: lang, pagetype: pagetype },
            url: 'Masterdata.asmx/getDepartmentlistform3',
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


    function setSourceIncident()
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getSourceIncident',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddlSourceincident");
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





    function setEventExposure()
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getEventExposure',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddlEventexposure");
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



    function setFatalityPrevent(faltality_prevent_id)
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getFatalityPrevention',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddfatality");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                $("#MainContent_ddfatality").val(faltality_prevent_id);
            }
        });



    }


    //function setRootCauseAction()
    //{

    //    $.ajax({
    //        type: "POST",
    //        data: { id:id },
    //        url: 'Masterdata.asmx/getRootCauseActionByIncidentID',
    //        dataType: 'json',
    //        success: function (json) {

    //            var $el = $("#MainContent_ddlRootcauseaction");
    //            $el.empty(); // remove old options

    //            $el.append($("<option></option>")
    //                       .attr("value", "").text(""));
    //            $.each(json, function (value, key) {

    //                $el.append($("<option></option>")
    //                        .attr("value", key.id).text(key.name));
    //            });

               
    //        }
    //    });



    //}



    function setShowfile()
    {
        $.ajax({
            type: "POST",
            data: { id: id },
            url: 'Actionevent.asmx/getFileInvestigation',
            dataType: 'json',
            success: function (json) {

                var html = "";
                $.each(json, function (value, key) {

                    if (key.name!="")
                    {
                         html = html + '<a href="' + key.name + '"><img src="template/img/pdf_icon.jpg"  style="width:110px;height:120px;padding-left:5px;padding-right:5px;" runat="server"></a>';

                    }
                   
                });

                $("#showfile").html(html);

            }
        });

    }




    function addFactFinding()
    {

        if (Page_ClientValidate("factfinding"))
        {
            showLoading();
            var fact_finding = $("#MainContent_txtfact_finding").val();
            var source_incident = $("#MainContent_ddlSourceincident").val();
            var event_exposure = $("#MainContent_ddlEventexposure").val();

            var unsafe_action = "";
            if ($('#MainContent_chUnsafeaction').is(":checked"))
            {
                unsafe_action = "Y";
            } else {

                unsafe_action = "N";
            }

            var unsafe_condition = "";
            if ($('#MainContent_chUnsafecondition').is(":checked"))
            {
                unsafe_condition = "Y";
            } else {

                unsafe_condition = "N";
            }


            $.ajax({
                type: "POST",
                data: {
                    fact_finding: fact_finding,
                    source_incident: source_incident,
                    event_exposure: event_exposure,
                    unsafe_action: unsafe_action,
                    unsafe_condition: unsafe_condition,
                    evidence_file: filename_fact,
                    incident_id: id,

                },
                url: 'Actionevent.asmx/createFactFinding',
                dataType: 'json',
                success: function (result) {
                    closeLoading();
                    closeFactFinding();
                    clearFactFinding();
                    callFactFinding();
                }
            });


        }
        else {
            return false;
        }


    }




    function updateFactFinding()
    {

        if (Page_ClientValidate("factfinding"))
        {
            showLoading();
            var fact_finding = $("#MainContent_txtfact_finding").val();
            var source_incident = $("#MainContent_ddlSourceincident").val();
            var event_exposure = $("#MainContent_ddlEventexposure").val();

            var unsafe_action = "";
            if ($('#MainContent_chUnsafeaction').is(":checked")) {
                unsafe_action = "Y";
            } else {

                unsafe_action = "N";
            }

            var unsafe_condition = "";
            if ($('#MainContent_chUnsafecondition').is(":checked")) {
                unsafe_condition = "Y";
            } else {

                unsafe_condition = "N";
            }


            $.ajax({
                type: "POST",
                data: {
                    fact_finding: fact_finding,
                    source_incident: source_incident,
                    event_exposure: event_exposure,
                    unsafe_action: unsafe_action,
                    unsafe_condition: unsafe_condition,
                    evidence_file: filename_fact,
                    id: fact_id,

                },
                url: 'Actionevent.asmx/updateFactFinding',
                dataType: 'json',
                success: function (result) {
                    closeLoading();
                    closeFactFinding();
                    clearFactFinding();
                    callFactFinding();
                }
            });


        }
        else {
            return false;
        }


    }



    function DeleteFactFinding(id)
    {
        var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
        if (confirm(message_confirm_delete)) {
            showLoading();
            $.ajax({
                type: "POST",
                data: { id: id },
                url: 'Actionevent.asmx/deleteFactFinding',
                dataType: 'json',
                success: function (json) {

                    closeLoading();
                    callFactFinding();

                }
            });

        }


    }


     function DeleteRootCauseAction(id)
     {
         var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
         if (confirm(message_confirm_delete)) {
             showLoading();
             $.ajax({
                 type: "POST",
                 data: { id: id },
                 url: 'Actionevent.asmx/deleteRootCauseAction',
                 dataType: 'json',
                 success: function (json) {

                     closeLoading();
                     callRootCauseAction();
                     callCorrectivePreventive();

                 }
             });

         }


     }



     function addCorrectivePreventive()
     {

         if (Page_ClientValidate("corrective"))
         {
             showLoading();
             var corrective_preventive = $("#MainContent_txtcorrective_preventive").val();
             var responsible_person = $("#MainContent_txtresponsible_person").val();
             var due_date = $("#MainContent_txtdue_date").val();
             //var date_complete = $("#MainContent_txtdate_complete").val();
             var notyfy_contractor = $("#MainContent_txtnotyfy_contractor").val();
             var remark = $("#MainContent_txtremark").val();

             var employee_id = $("#MainContent_employee_id").val();
             var contractor_id = $("#MainContent_contractor_id").val();
             var root_cause_action = $("#MainContent_txtRootcauseaction").val();
             $.ajax({
                 type: "POST",
                 data: {
                     corrective_preventive: corrective_preventive,
                     responsible_person: responsible_person,
                     due_date: due_date,
                     date_complete: "",
                     notify_contractor: notyfy_contractor,
                     remark: remark,
                     attachment_file: filename_corrective,
                     employee_id: employee_id,
                     contractor_id: contractor_id,
                     root_cause_action: root_cause_action,
                     user_id: user_login_id,
                     incident_id: id,
                     type_action:type_action,

                 },
                 url: 'Actionevent.asmx/createCorrectivePreventive',
                 dataType: 'json',
                 success: function (result) {
                     closeLoading();
                     closeCorrectivePreventive();
                     clearCorrectivePreventive();

                     if (type_action=="corrective")
                     {
                         callCorrectivePreventive();

                     } else if (type_action == "preventive") {

                         callPreventive();

                     } else if (type_action == "consequence") {

                         callConsequence();
                     }
                     
                    
                    
                    
                 }
             });


         }
         else {
             return false;
         }


     }




     function updateCorrectivePreventive()
     {

         if (Page_ClientValidate("corrective")) {
             showLoading();
             var corrective_preventive = $("#MainContent_txtcorrective_preventive").val();
             var responsible_person = $("#MainContent_txtresponsible_person").val();
             var due_date = $("#MainContent_txtdue_date").val();
            // var date_complete = $("#MainContent_txtdate_complete").val();
             var notyfy_contractor = $("#MainContent_txtnotyfy_contractor").val();
             var remark = $("#MainContent_txtremark").val();

             var employee_id = $("#MainContent_employee_id").val();
             var contractor_id = $("#MainContent_contractor_id").val();
             var root_cause_action = $("#MainContent_txtRootcauseaction").val();
             $.ajax({
                 type: "POST",
                 data: {
                     corrective_preventive: corrective_preventive,
                     responsible_person: responsible_person,
                     due_date: due_date,
                     date_complete: "",
                     notify_contractor: notyfy_contractor,
                     remark: remark,
                     attachment_file: filename_corrective,
                     employee_id: employee_id,
                     contractor_id: contractor_id,
                     root_cause_action :root_cause_action,
                     id: corrective_id,
                     type_action: type_action,

                 },
                 url: 'Actionevent.asmx/updateCorrectivePreventive',
                 dataType: 'json',
                 success: function (result) {
                     closeLoading();
                     closeCorrectivePreventive();
                     clearCorrectivePreventive();
                     if (type_action == "corrective") {
                         callCorrectivePreventive();

                     } else if (type_action == "preventive") {

                         callPreventive();

                     } else if (type_action == "consequence") {

                         callConsequence();
                     }


                 }
             });


         }
         else {
             return false;
         }


     }






     function addRootCauseAction()
     {

             showLoading();
             var root_cause_action = $("#MainContent_txtrootcause_action").val();

             $.ajax({
                 type: "POST",
                 data: {
                     root_cause_action :root_cause_action,
                     incident_id: id,

                 },
                 url: 'Actionevent.asmx/createRootCauseAction',
                 dataType: 'json',
                 success: function (result) {
                     closeLoading();
                     closeRootCauseAction();
                     clearRootCauseAction();
                     callRootCauseAction();

                 }
             });


             return false;
   
     }



     function updateRootCauseAction()
     {

         showLoading();
         var root_cause_action = $("#MainContent_txtrootcause_action").val();

         $.ajax({
             type: "POST",
             data: {
                 root_cause_action: root_cause_action,
                 id: root_cause_id,

             },
             url: 'Actionevent.asmx/updateRootCauseAction',
             dataType: 'json',
             success: function (result) {
                 closeLoading();
                 closeRootCauseAction();
                 clearRootCauseAction();
                 callRootCauseAction();
                 callCorrectivePreventive();

             }
         });


         return false;

     }

     function ShowEditFactFinding(fact_finding_id)
     {
         $("#MainContent_btCreatfactfinding").hide();
         $("#MainContent_btUpdatefactfinding").show();
         fact_id = fact_finding_id;
         dialogFactFinding.dialog("open");
         $.ajax({
             type: "POST",
             data: { id: fact_finding_id },
             url: 'Actionevent.asmx/getFactFindingByID',
             dataType: 'json',
             success: function (json) {

                 $.each(json, function (value, key) {

                     $("#MainContent_txtfact_finding").val(key.fact_finding_name);
                     $("#MainContent_ddlSourceincident").val(key.source_incident_id);
                     $("#MainContent_ddlEventexposure").val(key.event_exposure_id);

                     if (key.unsafe_action == "Y")
                     {
                         $('#MainContent_chUnsafeaction').prop('checked', true);
                     } else {
                         $('#MainContent_chUnsafeaction').prop('checked', false);
                     }

                     if (key.unsafe_condition == "Y")
                     {
                         $('#MainContent_chUnsafecondition').prop('checked', true);
                     } else {
                         $('#MainContent_chUnsafecondition').prop('checked', false);
                     }
                     
                    


                 });





             }
         });

     }



     function ShowEditCorrectivePreventive(corrective_preventive_id, type_actions)
     {
         $("#MainContent_btCreateCorrective").hide();
         $("#MainContent_btUpdateCorrective").show();

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
         corrective_id = corrective_preventive_id;
         type_action = type_actions;
         dialogCorrectivePreventive.dialog("open");
         $.ajax({
             type: "POST",
             data: { id: corrective_preventive_id,lang:lang ,type_action:type_action},
             url: 'Actionevent.asmx/getCorrectivePreventiveByID',
             dataType: 'json',
             success: function (json) {

                 $.each(json, function (value, key) {

                     $("#MainContent_txtcorrective_preventive").val(key.corrective_preventive_action);
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


     function ShowEditRootCauseAction(root_cause_action_id)
     {
         $("#MainContent_btAddRootCauseAction").hide();
         $("#MainContent_btUpdateRootCauseAction").show();
         root_cause_id = root_cause_action_id;
         dialogRootCauseAction.dialog("open");
         $.ajax({
             type: "POST",
             data: { id: root_cause_action_id, lang: lang },
             url: 'Actionevent.asmx/getRootCauseActionByID',
             dataType: 'json',
             success: function (json) {

                 $.each(json, function (value, key) {

                     $("#MainContent_txtrootcause_action").val(key.name);
                    

                 });



             }
         });

     }



     function callFactFinding()
     {

         dataTableFactFinding.ajax.url('Datatablelist.asmx/getListFactFinding?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype).load();

     }

     function callCorrectivePreventive() 
     {

         dataTableCorrectivePreventive.ajax.url('Datatablelist.asmx/getListCorrectivePreventive?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype).load();

     }

     function callPreventive()
     {

         dataTablePreventive.ajax.url('Datatablelist.asmx/getListPreventive?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype).load();

     }


     function callConsequence()
     {

         dataTableConsequence.ajax.url('Datatablelist.asmx/getListConsequence?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype).load();

     }

     function callRootCauseAction()
     {

         dataTableRootCauseAction.ajax.url('Datatablelist.asmx/getListRootCauseAction?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype).load();

     }




     function setDatatableFactFinding()
     {

         dataTableFactFinding = $("#tbFactFinding").DataTable({
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


         dataTableFactFinding.ajax.url('Datatablelist.asmx/getListFactFinding?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype);



     }


     function setDatatableCorrectivePreventive()
     {

         dataTableCorrectivePreventive = $("#tbCorrective_preventive").DataTable({
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


         dataTableCorrectivePreventive.ajax.url('Datatablelist.asmx/getListCorrectivePreventive?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype);



     }


     function setDatatablePreventive()
     {

         dataTablePreventive = $("#tbPreventive").DataTable({
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


         dataTablePreventive.ajax.url('Datatablelist.asmx/getListPreventive?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype);



     }


     function setDatatableConsequence()
     {

         dataTableConsequence = $("#tbConsequence").DataTable({
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


         dataTableConsequence.ajax.url('Datatablelist.asmx/getListConsequence?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype);



     }

     function setDatatableRootCauseAction()
     {

         dataTableRootCauseAction = $("#tbRootCauseAction").DataTable({
             "bProcessing": true,
             "sProcessing": true,

             "bPaginate": false,
             "bInfo": false,
             "bFilter": false,
             "ordering": false,
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
                    "targets": [1],
                    "visible": false,
                }
             ]
         });


         dataTableRootCauseAction.ajax.url('Datatablelist.asmx/getListRootCauseAction?incident_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype);



     }




     function closeAction(action_id, attachment_file,type_actions)
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

                     if (attachment_file != "")
                     {
                         showLoading();
                         $.ajax({
                             type: "POST",
                             data: { id: action_id, type: "close", remark: "", type_action:type_actions},
                             url: 'Actionevent.asmx/requestActionIncident',
                             dataType: 'json',
                             success: function (json) {
                                 closeLoading();
                                 callCorrectivePreventive();
                                 callConsequence();
                                 callPreventive();
                             }
                         });

                         

                     } else {
                         
                         alert('<%= Resources.Main.require_attach_file %>')
                     }

                     $(this).dialog("close");
                    
                 },
                 '<%= Resources.Main.btCancel %>': function () {
                     $(this).dialog("close");
                 }
             }
         });

         return false;
     }


     function rejectAction(action_id,type_actions)
     {
         
         action_reason_id = action_id;
         type_action = type_actions;
         dialogReason.dialog("open");

         return false;
     }


     function cancelAction(action_id,type_actions)
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
                         data: { id: action_id, type: "cancel", remark: "", type_action: type_actions },
                         url: 'Actionevent.asmx/requestActionIncident',
                         dataType: 'json',
                         success: function (json) {
                             closeLoading();
                             dialogReason.dialog("close");
                             callCorrectivePreventive();
                             callConsequence();
                             callPreventive();
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
             data: { id: action_reason_id, type: "reject", remark: reason,type_action:type_action },
             url: 'Actionevent.asmx/requestActionIncident',
             dataType: 'json',
             success: function (json) {
                 closeLoading();
                 action_reason_id = 0;
                 callCorrectivePreventive();
                 callConsequence();
                 callPreventive();
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



     function validateDuedate(oSrc, args)
     {
         
         var due_date = $("#MainContent_txtdue_date").val();
         $.ajax({
             type: "POST",
             data: { duedate: due_date, lang:lang },
             url: 'Actionevent.asmx/checkDuedate',
             dataType: 'json',
             async: false,
             cache: false,
             success: function (result)
             {
                 
                 if (result == true) {//วันที่ duedate < datenow
                    
                     args.IsValid = false;
                   
                     
                 } else {
                    
                     args.IsValid = true;;

                 }

             }
         });

       
     }




     function setReasonExcept()
     {
         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getReasonExcept',
             dataType: 'json',
             success: function (json) {

                 var $el = $("#ddReasonexcept");
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



     function showUpdateReasonExcept()
     {


         dialogReasonExcept.dialog("open");

         return false;

     }


     function UpdateReasonExcept()
     {

         var reason_type = $("#ddReasonexcept").val();
         var reason = $("#MainContent_txtreasonexcept").val();

         if (reason_type != "") {
             showLoading();
             $.ajax({
                 type: "POST",
                 data: {
                     incidentid: id,
                     reason_except_type: reason_type,
                     reasonexcept: reason,
                     userid: user_login_id,
                     typelogin: type_login,
                     step_form: 1,
                     group_id: user_group_id,

                 },
                 url: 'Actionevent.asmx/updateReasonExceptIncident',
                 dataType: 'json',
                 success: function (result) {

                     closeLoading();
                     dialogReason.dialog("close");
                     $("#MainContent_txtreasonexcept").val("");
                     $("#rqreasonexcept").text("");
                     //setShowedit();//update status
                     window.location.href = "incidentform3.aspx?pagetype=view&id=" + id;
                 }


             });
         } else {

             var require_reason_except = '<%= Resources.Incident.rqreasonexcept %>';
             $("#rqreasonexcept").text(require_reason_except);
         }



     }


     function CloseReasonExcept()
     {
         dialogReasonExcept.dialog("close");
         $("#ddReasonexcept").val("");
         $("#MainContent_txtreasonexcept").val("");


     }





   
</script>
      <div id="dialog-confirm" title="">
  
    </div>

    <input type="hidden" id="employee_id" name="employee_id" runat="server">
    <input type="hidden" id="contractor_id" name="contractor_id" runat="server">


      <div id="create_reason_except">
         <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.lbreasonexcept %></label><div class="lbrequire"> *</div>
                      <select id="ddReasonexcept" class="form-control">
                         
                        </select>                        
                    <label id="rqreasonexcept" class="text-danger"></label>  
                     
                </div>
                </div>

                              				
            </div>
       <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.detailexcept %></label>
                           
                    <textarea class="form-control" rows="4" id="txtreasonexcept" runat="server"></textarea>
                    
                     
                </div>
                </div>

                              				
            </div>
       <div class="row">
             
            <div class="form-group">
                <div class="pull-right">
                 <button id="btConfirmExcept"class="btn btn-sm btn-primary" onclick="UpdateReasonExcept()"><%= Resources.Main.btconfirm %></button>
                 <button class="btn btn-sm btn-default" onclick="CloseReasonExcept();"><%= Resources.Main.btCancel %></button>
                 </div>
             </div>  
        </div>
        
    </div>

   




     <div id="create_reason_reject">
       <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.lbreasonreject %></label><div class="lbrequire"> *</div>
                           
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





  <div id="root_cause_action_form" title="<%= Resources.Incident.lb_root_cause_action %>">
       <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.root_cause_action %></label>
                           
                    <textarea class="form-control" rows="4" id="txtrootcause_action" runat="server"></textarea>
                     
                </div>
                </div>

                              				
            </div>
      <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                        <asp:Button ID="btAddRootCauseAction" runat="server"  Text="<%$ Resources:Main, btadd %>" OnClientClick="addRootCauseAction();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btUpdateRootCauseAction" runat="server"  Text="<%$ Resources:Main, btsave %>" OnClientClick="updateRootCauseAction();" CssClass="btn btn-primary"/>
                        <button type="button" id="btCloseRootCauseAction" class="btn btn-default" runat="server" onclick="closeRootCauseAction();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>

        
    </div>


 <div id="fact_finding_form" title="<%= Resources.Incident.fact_finding_form %>">     
         
          	<div class="row">
				
				<div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"><%= Resources.Incident.fact_finding %></label><div class="lbrequire"> *</div>
						<input id="txtfact_finding" name="txtfact_finding"  type="text" class="form-control" runat="server">
                        
                         <asp:RequiredFieldValidator ID="rqpfactfinding" runat="server" ControlToValidate ="txtfact_finding" ErrorMessage="<%$ Resources:Incident, rqpfactfinding %>" 
                             ValidationGroup="factfinding" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
                  
		   </div>

          <div class="row">
                <div class="col-sm-6">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Incident.source_incident %></label><div class="lbrequire"> *</div>
					    <select id="ddlSourceincident" class="form-control" runat="server">
                            
                        </select>
                         <asp:RequiredFieldValidator ID="rqsourceincident" runat="server" ValidationGroup="factfinding" ControlToValidate ="ddlSourceincident" ErrorMessage="<%$ Resources:Incident, rqsourceincident %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				
				<div class="col-sm-6">
					<div class="form-group">
						<label class="control-label"><%= Resources.Incident.event_exposure %></label><div class="lbrequire"> *</div>
						 <select id="ddlEventexposure" class="form-control" runat="server">
                            
                        </select>
                        <asp:RequiredFieldValidator ID="rqeventexposure" runat="server" ValidationGroup="factfinding" ControlToValidate ="ddlEventexposure" ErrorMessage="<%$ Resources:Incident, rqeventexposure %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>			
                    </div>
				</div>
			
                 
		   </div>



       <div class="row">
           <div class="col-sm-12">
                 <div class="form-group">		
		            <div class="col-sm-6">
				
				        <input  type="checkbox" id="chUnsafeaction" name="chUnsafeaction" runat="server" style="padding-right:10px;"><%= Resources.Incident.unsafe_action %>
			        </div>
		 
                    <div class="col-sm-6">							
					    <input  type="checkbox" id="chUnsafecondition" name="chUnsafecondition" runat="server" style="padding-right:10px;"><%= Resources.Incident.unsafe_condition %>
				    </div>
		          </div>
               </div>
	    </div>
           

      <div class="row">
                  <div class="col-sm-12">
                      
                       <div class="form-group">
                                                    
                                <div  class="dropzone" id="dropzoneFact" style="margin-top:8px;">
                                    <div class="fallback">
                                        <input name="filefact" type="file"/>
                                       <input type="submit" value="Upload" />
                                    </div>
                                </div>
                          
                        </div>
                        </div>
                     </div>

          

             <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                        <asp:Button ID="btCreatfactfinding" runat="server" ValidationGroup="factfinding"  Text="<%$ Resources:Main, btadd %>" OnClientClick="addFactFinding();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btUpdatefactfinding" runat="server" ValidationGroup="factfinding"  Text="<%$ Resources:Main, btsave %>" OnClientClick="updateFactFinding();" CssClass="btn btn-primary"/>
                        <button type="button" id="btCloseFactFinding" class="btn btn-default" runat="server" onclick="closeFactFinding();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>

      
    </div>






     <div id="corrective_preventive_form" title="<%= Resources.Incident.corrective_preventive_form %>">     
         
          	<div class="row">
				
				<div class="col-sm-12">
					<div class="form-group">
						<label class="control-label" id="lb_main_action"><%= Resources.Incident.tb_corrective_preventive %></label><div class="lbrequire"> *</div>
						<input id="txtcorrective_preventive" name="txtcorrective_preventive"  type="text" class="form-control" runat="server">
                        
                         <asp:RequiredFieldValidator ID="rqcorrective_preventive" runat="server" ControlToValidate ="txtcorrective_preventive" ErrorMessage="<%$ Resources:Incident, rqcorrective_preventive %>" 
                             ValidationGroup="corrective" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
                  
		   </div>

          <div class="row">
                <div class="col-sm-4">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Incident.responsible_person %></label><div class="lbrequire"> *</div>
					    <input id="txtresponsible_person" name="txtresponsible_person"  type="text" class="form-control" runat="server">
                        
                         <asp:RequiredFieldValidator ID="rqresponsible_person" runat="server" ControlToValidate ="txtresponsible_person" ErrorMessage="<%$ Resources:Incident, rqresponsible_person %>" 
                             ValidationGroup="corrective" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				
				<div class="col-sm-4">
					<div id="due_date" class="form-group">
						<label class="control-label"><%= Resources.Incident.due_date %></label><div class="lbrequire"> *</div>
						 <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtdue_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                        </div>
                        <asp:RequiredFieldValidator ID="rqduedate" runat="server" ValidationGroup="corrective" ControlToValidate ="txtdue_date" ErrorMessage="<%$ Resources:Incident, rqduedate %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>	
                         <asp:CustomValidator id="rqduedate2" runat="server"  ValidationGroup="corrective" ControlToValidate = "txtdue_date" ErrorMessage = "<%$ Resources:Incident, rqduedate2 %>"  CssClass="text-danger"  Display="Dynamic"  ClientValidationFunction="validateDuedate" ValidateEmptyText="true" >
                        </asp:CustomValidator>		
                    </div>
				</div>

       
                 
		   </div>


         
          <div class="row">
                <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Incident.notify_contractor %></label>
					    <input id="txtnotyfy_contractor" name="txtnotyfy_contractor"  type="text" class="form-control" runat="server">
                       
					</div>
				</div>

			
		   </div>
         <div class="row">
            <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"><%= Resources.Incident.root_cause_action %></label>
						
                         <input id="txtRootcauseaction" name="txtRootcauseaction"  type="text" class="form-control" runat="server">
                       		
                    </div>
				</div>
         </div>

          <div class="row">
                <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Incident.remark %></label>
					     <textarea class="form-control" rows="3" id="txtremark" runat="server"></textarea>

					</div>
				</div>
				
                 
		   </div>



             <div class="row">
      
                  <div class="col-sm-12">
                    <div class="form-group pull-right">
                        <asp:Button ID="btCreateCorrective" runat="server" ValidationGroup="corrective"  Text="<%$ Resources:Main, btadd %>" OnClientClick="addCorrectivePreventive();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btUpdateCorrective" runat="server" ValidationGroup="corrective"  Text="<%$ Resources:Main, btsave %>" OnClientClick="updateCorrectivePreventive();" CssClass="btn btn-primary"/>
                        <button type="button" id="btCloseCorrective" class="btn btn-default" runat="server" onclick="closeCorrectivePreventive();"><%= Resources.Main.btCancel %></button>
                    </div>
                </div>
             </div>

      
    </div>







  
<div class="ibox float-e-margins">
                
    <div class="ibox-content" style="display: block;">

               
<div class="stepwizard">
      <div class="stepwizard-row setup-panel">
        <div class="stepwizard-step">
            <asp:LinkButton ID="step1" runat="server" CssClass="btn btn-default btn-circle a-step" CausesValidation="False" OnClick="step1_Click">1</asp:LinkButton>
        <p><%= Resources.Incident.incidentstep1 %></p>
        </div>
        <div class="stepwizard-step">
        <asp:LinkButton ID="step2" runat="server" CssClass="btn btn-default btn-circle a-step" CausesValidation="False" OnClick="step2_Click">2</asp:LinkButton>
            <p><%= Resources.Incident.incidentstep2 %></p>
        </div>
        <div class="stepwizard-step">
         <asp:LinkButton ID="step3" runat="server" CssClass="btn btn-primary btn-circle" CausesValidation="False" OnClick="step3_Click">3</asp:LinkButton>
                            
        <p><%= Resources.Incident.incidentstep3 %></p>
        </div>
         <div class="stepwizard-step">
            <asp:LinkButton ID="step4" runat="server" CssClass="btn btn-default btn-circle a-step" CausesValidation="False" OnClick="step4_Click">4</asp:LinkButton>
                            
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
                            string PageType3 = Request.QueryString["PageType"];
                            string id3 = "";

                            if (PageType3 == "edit" || PageType3 == "view")
                            {
                                id3 = Request.QueryString["id"];
                            }
                            ArrayList per3 = Session["permission"] as ArrayList;
                            
                            bool pa3 = safetys4.Class.SafetyPermission.checkPermisionAction("report incident3 except", id3, "incident", Convert.ToInt32(Session["group_value"]));
                            bool area3  = safetys4.Class.SafetyPermission.checkPermisionInArea(id3, "incident");


                            if (per3.IndexOf("report incident3 except") > -1 && pa3 == true && area3 == true && Session["country"].ToString() == "thailand")         
                           {                            
       
                          %>
                             <asp:Button ID="btIncidentexcept" runat="server" Text="<%$ Resources:Incident, btIncidentexcept %>" CssClass="btn btn-primary" CausesValidation="False" OnClientClick="return showUpdateReasonExcept();"/>

                          <%                      
                            }
       
                          %>

                          <%
                              string id = Request.QueryString["id"];

                              ArrayList per = Session["permission"] as ArrayList;
                              
                              bool pa = safetys4.Class.SafetyPermission.checkPermisionAction("report incident3 request close", id, "incident", Convert.ToInt32(Session["group_value"]));
                              bool area = safetys4.Class.SafetyPermission.checkPermisionInArea(id, "incident");

                           if (per.IndexOf("report incident3 request close") > -1 && pa == true && area == true)         
                           {                            
       
                          %>
                                <asp:Button ID="btrequestclose" runat="server" Text="<%$ Resources:Incident, btRequestclose %>" CssClass="btn btn-primary" CausesValidation="true" ValidationGroup="incident" OnClick="btrequestclose_Click"/>
               
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
                              string id2 = Request.QueryString["id"];

                              ArrayList per2 = Session["permission"] as ArrayList;
                             
                              bool pa2 = safetys4.Class.SafetyPermission.checkPermisionAction("report incident3 edit", id2, "incident", Convert.ToInt32(Session["group_value"]));
                              bool area2 = safetys4.Class.SafetyPermission.checkPermisionInArea(id2, "incident");

                              if (per2.IndexOf("report incident3 edit") > -1 && pa2 == true && area2 == true)         
                           {                            
       
                          %>
                                <asp:Button ID="btIncidentedit" runat="server" Text="<%$ Resources:Incident, btIncidentedit %>" CssClass="btn btn-primary"  CausesValidation="False" OnClick="btIncidentedit_Click"/>

                          <%                      
                            }
       
                          %>


                    </div>
                    </div>
                  </div>
                           
                
                <div class="row">
                    <div class="col-md-12">
                          <strong><h3><%= Resources.Incident.lb_fact_finding %></h3></strong>
                       </div>
                              
                   </div>
     

                 <div  class="row">
                    <div class="col-md-12">
                          <table id="tbFactFinding" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <th> <%= Resources.Incident.no %></th>
                                    <th></th>
                                    <th> <%= Resources.Incident.fact_finding %></th>
                                    <th> <%= Resources.Incident.source_incident %></th>
                                    <th> <%= Resources.Incident.event_exposure %></th>
                                    <th> <%= Resources.Incident.unsafe_action %></th>
                                    <th> <%= Resources.Incident.unsafe_condition %></th>
                                    <th> <%= Resources.Incident.evidence %></th>
                                    <th> <%= Resources.Incident.manage %></th>
                       
                    
                                </tr>
                            </thead>
                            
                            </table>




                       </div>
                              
                   </div>

                <div  class=="row">
                    <div class="col-md-12">
                       <button type="button" id="btAddFactFinding" class="btn btn-primary" runat="server" onclick="showCreateFactFinding();"><i class="fa fa-plus"></i></button>  <%= Resources.Incident.add_fact_finding %>

                       </div>
                              
                   </div>

                 <div class=="row">
                    <div class="col-md-12">
                        <br/>
                        <br/>
                       </div>                             
                   </div>



                



    
                 <div class="row">
                    <div class="col-md-12">
                          <strong><h3><%= Resources.Incident.lb_root_cause_action %></h3></strong>
                       </div>
                              
                   </div>
     

                 <div  class="row">
                    <div class="col-md-12">
                          <table id="tbRootCauseAction" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <th> <%= Resources.Incident.root_cause_action_no %></th>
                                    <th></th>
                                    <th> <%= Resources.Incident.root_cause_action %></th>                                 
                                    <th> <%= Resources.Incident.manage %></th>
                       
                    
                                </tr>
                            </thead>
                            
                            </table>




                       </div>
                              
                   </div>

                <div  class=="row">
                    <div class="col-md-12">
                       <button type="button" id="btCreateRootCauseAction" class="btn btn-primary" runat="server" onclick="showCreateRootCauseAction();"><i class="fa fa-plus"></i></button>  <%= Resources.Incident.add_root_cause_action %>

                       </div>
                              
                   </div>

                
                
                <div  class="row">
                    <div class="col-md-12">
                        <br />
                        <br />
                       </div>
                              
                   </div>

               <div class="row">
                <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Incident.contributing_factor %> (1000 <%= Resources.Incident.rqCharacters %>)</label>
					     <textarea class="form-control" rows="5" id="txtcontributing_factor" runat="server"></textarea>
                          <asp:CustomValidator id="rqIncidentContributingFactorLength" runat="server" ValidationGroup="incident" ClientValidationFunction="CheckIncidentContributingFactorLength" Display="Dynamic" ControlToValidate="txtcontributing_factor"  ErrorMessage="<%$ Resources:Incident, rqIncidentContributingFactorLength %>" CssClass="text-danger"></asp:CustomValidator>

					</div>
				</div>				              
		         </div>

                 <div  class="row">
                    <div class="col-md-12">
                        <br />
                        <br />
                       </div>
                              
                   </div>




                 <div class="row">
                    <div class="col-md-12">
                          <strong><h3><%= Resources.Incident.lb_corrective_preventive %></h3></strong>
                       </div>
                              
                   </div>
     

                 <div  class="row">
                    <div class="col-md-12">
                          <table id="tbCorrective_preventive" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <th> <%= Resources.Incident.no %></th>
                                    <th></th>
                                    <th> <%= Resources.Incident.tb_corrective_preventive %></th>
                                    <th> <%= Resources.Incident.responsible_person %></th>
                                    <th> <%= Resources.Incident.lbdepartment_action %></th>
                                    <th> <%= Resources.Incident.due_date %></th>
                                    <th> <%= Resources.Incident.status %></th>
                                    <th> <%= Resources.Incident.date_complete %></th>
                                    <th> <%= Resources.Incident.attachment %></th>
                                    <th> <%= Resources.Incident.notify_contractor %></th>
                                    <th> <%= Resources.Incident.root_cause_action %></th>
                                    <th> <%= Resources.Incident.close_action %></th>
                                    <th> <%= Resources.Incident.remark %></th>
                                    <th> <%= Resources.Incident.manage %></th>
                       
                    
                                </tr>
                            </thead>
                            
                            </table>




                       </div>
                              
                   </div>

                <div  class=="row">
                    <div class="col-md-12">
                       <button type="button" id="btCreateCorrectivePreentive" class="btn btn-primary" runat="server" onclick="showCreateCorrectivePreventive();"><i class="fa fa-plus"></i></button>  <%= Resources.Incident.add_corrective_preventive %>

                       </div>
                              
                   </div>

                
                 <div  class="row">
                    <div class="col-md-12">
                        <br />
                        <br />
                       </div>
                              
                   </div>



                
                 <div class="row">
                    <div class="col-md-12">
                          <strong><h3><%= Resources.Incident.lb_preventive %></h3></strong>
                       </div>
                              
                   </div>
     

                 <div  class="row">
                    <div class="col-md-12">
                          <table id="tbPreventive" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <th> <%= Resources.Incident.no %></th>
                                    <th></th>
                                    <th> <%= Resources.Incident.tb_preventive %></th>
                                    <th> <%= Resources.Incident.responsible_person %></th>
                                    <th> <%= Resources.Incident.lbdepartment_action %></th>
                                    <th> <%= Resources.Incident.due_date %></th>
                                    <th> <%= Resources.Incident.status %></th>
                                    <th> <%= Resources.Incident.date_complete %></th>
                                    <th> <%= Resources.Incident.attachment %></th>
                                    <th> <%= Resources.Incident.notify_contractor %></th>
                                    <th> <%= Resources.Incident.root_cause_action %></th>
                                    <th> <%= Resources.Incident.close_action %></th>
                                    <th> <%= Resources.Incident.remark %></th>
                                    <th> <%= Resources.Incident.manage %></th>
                       
                    
                                </tr>
                            </thead>
                            
                            </table>




                       </div>
                              
                   </div>

                <div  class=="row">
                    <div class="col-md-12">
                       <button type="button" id="btCreatePreentive" class="btn btn-primary" runat="server" onclick="showCreatePreventive();"><i class="fa fa-plus"></i></button>  <%= Resources.Incident.add_preventive %>

                       </div>
                              
                </div>


                 <div  class="row">
                    <div class="col-md-12">
                        <br />
                        <br />
                       </div>
                              
                   </div>


                                
                 <div class="row">
                    <div class="col-md-12">
                          <strong><h3><%= Resources.Incident.lb_consequence %></h3></strong>
                       </div>
                              
                   </div>
     

                 <div  class="row">
                    <div class="col-md-12">
                          <table id="tbConsequence" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <th> <%= Resources.Incident.no %></th>
                                    <th></th>
                                    <th> <%= Resources.Incident.tb_consequence %></th>
                                    <th> <%= Resources.Incident.responsible_person %></th>
                                    <th> <%= Resources.Incident.lbdepartment_action %></th>
                                    <th> <%= Resources.Incident.due_date %></th>
                                    <th> <%= Resources.Incident.status %></th>
                                    <th> <%= Resources.Incident.date_complete %></th>
                                    <th> <%= Resources.Incident.attachment %></th>
                                    <th> <%= Resources.Incident.notify_contractor %></th>
                                    <th> <%= Resources.Incident.root_cause_action %></th>
                                    <th> <%= Resources.Incident.close_action %></th>
                                    <th> <%= Resources.Incident.remark %></th>
                                    <th> <%= Resources.Incident.manage %></th>
                       
                    
                                </tr>
                            </thead>
                            
                            </table>




                       </div>
                              
                   </div>

                <div  class=="row">
                    <div class="col-md-12">
                       <button type="button" id="btCreateConsequence" class="btn btn-primary" runat="server" onclick="showCreateConsequence();"><i class="fa fa-plus"></i></button>  <%= Resources.Incident.add_consequence %>

                       </div>
                              
                   </div>


                
                <div  class="row">
                    <div class="col-md-12">
                        <br />
                        <br />
                       </div>
                              
                   </div>











                <div class="row" style="padding-bottom:10px;"> 
                     <div class="col-md-4"><strong><h3><%= Resources.Incident.culpability %><div class="lbrequire"> *</div></h3></strong></div></div>

                			<div class="row">
                                <div class="col-md-12">
                                   
                                     <div  class="form-group">
                                       
                                           <div style="padding-bottom:10px;">
                                             <label> <input  value="G" id="culpability1" name="culpability" type="radio" runat="server">
                                            <%= Resources.Incident.guilty %> </label>
                                       
                                           </div>
                                           
                                         <div style="padding-bottom:10px;">
                                              <label> <input value="P" id="culpability2" name="culpability" type="radio" runat="server">
                                            <%= Resources.Incident.partial %> </label>
                                         </div>
                                          <div style="padding-bottom:10px;">
                                          <label> <input value="N" id="culpability3" name="culpability" type="radio" runat="server">
                                            <%= Resources.Incident.no_guilty %> </label>
                                         </div>
                                         <label id="rqculpability" class="text-danger"></label>  
                                         
                                        

                                          <div style="padding-bottom:10px;">
                                           <label class="control-label"><%= Resources.Incident.lbfunction_culpability %></label><div class="lbrequire"> *</div>                                 
                                                <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server">
                       
                                                </select>
                                              <asp:RequiredFieldValidator ID="rqFunction"  ValidationGroup="incident" runat="server" ControlToValidate ="ddfunction" ErrorMessage="<%$ Resources:Incident, rqfunction %>" CssClass="text-danger" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                         </div>


                                         <div style="padding-bottom:10px;">
                                           <label class="control-label"><%= Resources.Incident.lbdepartment_culpability %></label><div class="lbrequire"> *</div>                                 
                                                <select id="dddepartment" class="form-control" runat="server">
                       
                                                </select>
                                             <asp:RequiredFieldValidator ID="rqDepartment" ValidationGroup="incident" runat="server" ControlToValidate ="dddepartment" ErrorMessage="<%$ Resources:Incident, rqdepartment %>" CssClass="text-danger" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                         </div>
                                    </div>

                                </div>
                              
                            </div>

                     
                
                 <div class="row" style="padding-bottom:10px;"> 
                     <div class="col-md-4 row row-roadaccident"><strong><h3><%= Resources.Incident.road_accident %></h3></strong></div></div>

                			<div class="row row-roadaccident">
                                <div class="col-md-12">
                                    
                                     <div  class="form-group">
                                        
                                         <div class="col-sm-6">
                                            <label> <input  value="N" id="road_accident1" name="roadaccident" type="radio" runat="server">
                                            <%= Resources.Incident.no %> </label>
                                         </div>
                                         <div class="col-sm-6">
                                              <label> <input value="Y" id="road_accident2" name="roadaccident" type="radio" runat="server">
                                            <%= Resources.Incident.yes %> </label>
                                         </div>
                                        
                                    </div>

                                </div>
                              
                            </div>

                     


                    <div class="row">
                    <div class="col-md-12">
                            <label class="control-label"> <strong><h3><%= Resources.Incident.root_cause %></h3></strong></label> 
                            <div class="form-group" id="root_cause">
                                <div class="row">
                                     <div class="col-sm-6" >                                     
                                    <label> 
                                       
                                        <input  value="policy_planning" id="rootcause1" name="rootcause" type="checkbox" runat="server">
                                    <%= Resources.Incident.policy_planning %> </label>
                                    </div>
                                   <div class="col-sm-6">   
                                        <label> 
                                        
                                          <input value="document_reporting" id="rootcause2" name="rootcause" type="checkbox" runat="server">
                                       <%= Resources.Incident.document_reporting %> </label>
                                    </div>                        

                                </div>   
                                 <div class="row">
                                     <div class="col-sm-6" >                                     
                                    <label> 
                                       
                                        <input  value="responsibilities_resourses" id="rootcause3" name="rootcause" type="checkbox" runat="server">
                                    <%= Resources.Incident.responsibilities_resourses %> </label>
                                    </div>
                                   <div class="col-sm-6">   
                                        <label> 
                                        
                                          <input value="standard_controls" id="rootcause4" name="rootcause" type="checkbox" runat="server">
                                       <%= Resources.Incident.standard_controls %> </label>
                                    </div>                        

                                </div>  
                                
                                  <div class="row">
                                     <div class="col-sm-6" >                                     
                                    <label> 
                                       
                                        <input  value="hazard_assesment" id="rootcause5" name="rootcause" type="checkbox" runat="server">
                                    <%= Resources.Incident.hazard_assesment %> </label>
                                    </div>
                                   <div class="col-sm-6">   
                                        <label> 
                                        
                                          <input value="inspection_monitoring" id="rootcause6" name="rootcause" type="checkbox" runat="server">
                                       <%= Resources.Incident.inspection_monitoring %> </label>
                                    </div>                        

                                </div>    

                                  <div class="row">
                                     <div class="col-sm-6" >                                     
                                    <label> 
                                       
                                        <input  value="legal_compliance" id="rootcause7" name="rootcause" type="checkbox" runat="server">
                                    <%= Resources.Incident.legal_compliance %> </label>
                                    </div>
                                   <div class="col-sm-6">   
                                        <label> 
                                        
                                          <input value="emergency_preparation" id="rootcause8" name="rootcause" type="checkbox" runat="server">
                                       <%= Resources.Incident.emergency_preparation %> </label>
                                    </div>                        

                                </div>    


                                <div class="row">
                                     <div class="col-sm-6" >                                     
                                    <label> 
                                       
                                        <input  value="safety_installation" id="rootcause9" name="rootcause" type="checkbox" runat="server">
                                    <%= Resources.Incident.safety_installation %> </label>
                                    </div>
                                   <div class="col-sm-6">   
                                        <label> 
                                        
                                          <input value="management" id="rootcause10" name="rootcause" type="checkbox" runat="server">
                                       <%= Resources.Incident.management %> </label>
                                    </div>                        

                                </div>   
                                
                                 <div class="row">
                                     <div class="col-sm-6" >                                     
                                    <label> 
                                       
                                        <input  value="purchasing_contractor" id="rootcause11" name="rootcause" type="checkbox" runat="server">
                                    <%= Resources.Incident.purchasing_contractor %> </label>
                                    </div>
                                   <div class="col-sm-6">   
                                        <label> 
                                        
                                          <input value="occupational" id="rootcause12" name="rootcause" type="checkbox" runat="server">
                                       <%= Resources.Incident.occupational %> </label>
                                    </div>                        

                                </div>  
                                
                                
                                 <div class="row">
                                     <div class="col-sm-6" >                                     
                                    <label> 
                                       
                                        <input  value="selection_competency" id="rootcause13" name="rootcause" type="checkbox" runat="server">
                                    <%= Resources.Incident.selection_competency %> </label>
                                    </div>
                                   <div class="col-sm-6">   
                                        <label> 
                                        
                                          <input value="corrective_preventive" id="rootcause14" name="rootcause" type="checkbox" runat="server">
                                       <%= Resources.Incident.corrective_preventive %> </label>
                                    </div>                        

                                </div>    


                                   <div class="row">
                                     <div class="col-sm-6" >                                     
                                    <label> 
                                       
                                        <input  value="incident_hazard" id="rootcause15" name="rootcause" type="checkbox" runat="server">
                                    <%= Resources.Incident.incident_hazard %> </label>
                                    </div>
                                   <div class="col-sm-6">   
                                        <label> 
                                        
                                          <input value="health_wellness" id="rootcause16" name="rootcause" type="checkbox" runat="server">
                                       <%= Resources.Incident.health_wellness %> </label>
                                    </div>                        

                                </div>    

                                <div class="row">
                                     <div class="col-sm-6" >                                     
                                    <label> 
                                       
                                        <input  value="hygience" id="rootcause17" name="rootcause" type="checkbox" runat="server">
                                    <%= Resources.Incident.hygience %> </label>
                                    </div>
                                   <div class="col-sm-6">   
                                        <label> 
                                        
                                          <input value="system_performance" id="rootcause18" name="rootcause" type="checkbox" runat="server">
                                       <%= Resources.Incident.system_performance %> </label>
                                    </div>                        

                                </div>    


                                <div class="row">
                                     <div class="col-sm-6" >                                     
                                    <label> 
                                       
                                        <input  value="communication_involvement" id="rootcause19" name="rootcause" type="checkbox" runat="server">
                                    <%= Resources.Incident.communication_involvement %> </label>
                                    </div>
                                                        

                                </div>    
                               
                        </div>
                        
                       </div>
                              
                   </div>
             

             <div class="row"> 
                     <div class="col-md-4"><strong><h3><%= Resources.Incident.fatality_prevention %></h3></strong></div></div>

                			<div class="row">
                                <div class="col-md-12">
                                   
                                     <div  class="form-group">                                       
                                         <div class="col-sm-4">
                                              <select id="ddfatality" name="ddfatality" class="form-control" runat="server">
                       
                                               </select>
                                         </div>
                                        
                                    </div>

                                </div>
                              
                            </div>


                 <div class="row">
                    <div class="col-md-12">
                      
                       </div>
                              
                   </div>

                <div class="row">
                    <div class="col-md-12">
                                   
                            <div  class="form-group">                                       
                                <label class="col-sm-2 control-label"><%= Resources.Incident.other_please %></label>
                                 <div class="col-sm-4">
                                    <input id="txtother_please" name="txtother_please"  type="text" class="form-control" runat="server">
                                </div>
                                      
                        </div>
                        </div>

                 </div>
             




                  <div class="row">
                  <div class="col-md-12">
                      <div class="lbrequire" id="infouploadimage"> <%= Resources.Incident.investigation_committee %></div>
                       <div class="form-group">
                                <div id="showfile">
                                
                                 </div>      
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
                    <asp:Button ID="btUpdate" runat="server" ValidationGroup="incident" Text="<%$ Resources:Main, btSubmit%>"  CssClass="btn btn-primary" OnClientClick="return updateIncident();" />           
               </div>
            </div>
              

            </div>
        </div>
    </div>
 



                       
                </div>
            </div>





</asp:Content>
