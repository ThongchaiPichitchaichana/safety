<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="hazardform3.aspx.cs" Inherits="safetys4.hazardform3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">

<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">
<link rel="stylesheet" href="template/js/plugins/fancybox/jquery.fancybox.css" type="text/css" />

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

    var id = "";
    var pagetype = "";
   
    var dialogProcessAction;

    var dataTableProcessAction; //reference to your dataTable

    var process_action_id = 0;  

    var dialogReason;

    var action_reason_id = 0;



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



      

        //$('#date_complete .input-group.date').datepicker({
        //    todayBtn: "linked",
        //    keyboardNavigation: false,
        //    forceParse: false,
        //    autoclose: true,
        //    format: "dd/mm/yyyy"
        //});



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


        setDatatableProcessAction();
        setTypeControl();


    });



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

    function showCreateProcessAction()
    {
        $("#MainContent_btCreateProcess").show();
        $("#MainContent_btUpdateProcess").hide();
        dialogProcessAction.dialog("open");

        return false;

    }



    function setShowedit()
    {
       
        $.ajax({
            type: "POST",
            data: { id: id, user_id: user_login_id, lang: lang },
            url: 'Actionevent.asmx/getHazardbyid',
            dataType: 'json',
            success: function (json) {

                $.each(json, function (value, key) {

                   
                    $("#MainContent_txtname_areaowner").val(key.name_area_owner);
                    $("#MainContent_lbEmployee").text(key.name_modify);
                    $("#MainContent_lbUpdate").text(key.datetime_modify);

                    $("#show_doc_status").html(key.doc_no + ' ' + key.status);

                


                });

               



            }
        });

    }






    function updateHazard(typebutton)
    {
       
            showLoading();

    
            var data_post = JSON.stringify({
                area_owner_id: user_login_id,
                user_id: user_login_id,
                typelogin: type_login,
                hazardid: id,
                stepform: 3,
                typebutton: typebutton,
                group_id: user_group_id
            });

            $.ajax({
                type: "POST",
                data: data_post,
                url: 'Actionevent.asmx/updateHazard3',
                contentType: "application/json; charset=utf-8",
                success: function (id) {
                    $("#rqlevelhazard").text("");
                    window.location.href = "hazardform3.aspx?pagetype=view&id=" + id;
                }
            });

            return false;

       

    }

    function addProcessAction() {

        if (Page_ClientValidate("process")) {
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
                    user_id: user_login_id,
                   // root_cause_action: root_cause_action,
                    hazard_id: id,

                },
                url: 'Actionevent.asmx/createProcessAction',
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




    function updateProcessAction() {

        if (Page_ClientValidate("process")) {
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
                    type_control : type_control,
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
                    hazard_id: id,
                    id: process_action_id,

                },
                url: 'Actionevent.asmx/updateProcessAction',
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
            data: { id: process_action_id, lang: lang },
            url: 'Actionevent.asmx/getProcessActionByID',
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

    function callProcessAction() {

        dataTableProcessAction.ajax.url('Datatablelist.asmx/getListProcessAction?hazard_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype).load();

    }


    function setDatatableProcessAction() {

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


        dataTableProcessAction.ajax.url('Datatablelist.asmx/getListProcessAction?hazard_id=' + id + "&lang=" + lang + '&pagetype=' + pagetype);



    }


    function closeAction(action_id, attachment_file)
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
                            data: { id: action_id, type: "close", remark: "" },
                            url: 'Actionevent.asmx/requestActionHazard',
                            dataType: 'json',
                            success: function (json) {
                                closeLoading();
                                callProcessAction();
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
                         url: 'Actionevent.asmx/requestActionHazard',
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
            url: 'Actionevent.asmx/requestActionHazard',
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


</script>
    <div id="dialog-confirm" title="">
  
    </div>
       <input type="hidden" id="employee_id" name="employee_id" runat="server">
    <input type="hidden" id="contractor_id" name="contractor_id" runat="server">





     <div id="create_reason_reject">
       <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Hazard.lbreasonreject %></label><div class="lbrequire"> *</div>
                           
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





       <div id="process_action_form" title="<%= Resources.Hazard.process_action_form %>">     
         
          	<div class="row">

                  <div class="col-sm-6">
					<div class="form-group">
						<label class="control-label"><%= Resources.Hazard.typecontrol %></label><div class="lbrequire"> *</div>
						 <select id="ddtypecontrol" class="form-control" runat="server">
                            
                        </select>
                        
                         <asp:RequiredFieldValidator ID="rqtypecontrl" runat="server" ControlToValidate ="ddtypecontrol" ErrorMessage="<%$ Resources:Hazard, rqtypecontrol %>" 
                             ValidationGroup="process" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				
				<div class="col-sm-6">
					<div class="form-group">
						<label class="control-label"><%= Resources.Hazard.action %></label><div class="lbrequire"> *</div>
						<input id="txtaction" name="txtaction"  type="text" class="form-control" runat="server">
                        
                         <asp:RequiredFieldValidator ID="rqaction" runat="server" ControlToValidate ="txtaction" ErrorMessage="<%$ Resources:Hazard, rqaction %>" 
                             ValidationGroup="process" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
                  
		   </div>

          <div class="row">
                <div class="col-sm-4">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Hazard.responsible_person %></label><div class="lbrequire"> *</div>
					    <input id="txtresponsible_person" name="txtresponsible_person"  type="text" class="form-control" runat="server">
                        
                         <asp:RequiredFieldValidator ID="rqresponsible_person" runat="server" ControlToValidate ="txtresponsible_person" ErrorMessage="<%$ Resources:Hazard, rqresponsible_person %>" 
                             ValidationGroup="process" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				
				<div class="col-sm-4">
					<div id="due_date" class="form-group">
						<label class="control-label"><%= Resources.Hazard.due_date %></label><div class="lbrequire"> *</div>
						 <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtdue_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                        </div>
                        <asp:RequiredFieldValidator ID="rqduedate" runat="server" ValidationGroup="process" ControlToValidate ="txtdue_date" ErrorMessage="<%$ Resources:Hazard, rqduedate %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>		
                          <asp:CustomValidator id="rqduedate2" runat="server"  ValidationGroup="process" ControlToValidate = "txtdue_date" ErrorMessage = "<%$ Resources:Incident, rqduedate2 %>"  CssClass="text-danger"  Display="Dynamic"  ClientValidationFunction="validateDuedate" ValidateEmptyText="true" >
                        </asp:CustomValidator>	
                    </div>
				</div>

              <%--<div class="col-sm-4">
					<div id="date_complete" class="form-group">
						<label class="control-label"><%= Resources.Hazard.date_complete %></label>
						 <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtdate_complete" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                   --%>     
                       <%-- </div>
                        <asp:RequiredFieldValidator ID="rqdatecomplete" runat="server" ValidationGroup="corrective" ControlToValidate ="txtdate_complete" ErrorMessage="<%$ Resources:Incident, rqdatecomplete %>" CssClass="text-danger"  Display="Dynamic">
                       </asp:RequiredFieldValidator>	
                    </div>
				</div>--%>
			
                 
		   </div>


         
          <div class="row">
                <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Hazard.notify_contractor %></label>
					    <input id="txtnotyfy_contractor" name="txtnotyfy_contractor"  type="text" class="form-control" runat="server">
                       
					</div>
				</div>

			
		   </div>
        

          <div class="row">
                <div class="col-sm-12">
					<div class="form-group">
						<label class="control-label"> <%= Resources.Hazard.remark %></label>
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
        <p><%= Resources.Hazard.hazardstep1 %></p>
        </div>
        <div class="stepwizard-step">
        <asp:LinkButton ID="step2" runat="server" CssClass="btn btn-default btn-circle" CausesValidation="False" OnClick="step2_Click">2</asp:LinkButton>
            <p><%= Resources.Hazard.hazardstep2 %></p>
        </div>
        <div class="stepwizard-step">
         <asp:LinkButton ID="step3" runat="server" CssClass="btn btn-primary btn-circle a-step" CausesValidation="False" OnClick="step3_Click">3</asp:LinkButton>
                            
        <p><%= Resources.Hazard.hazardstep3 %></p>
        </div>
         <div class="stepwizard-step">
            <asp:LinkButton ID="step4" runat="server" CssClass="btn btn-default btn-circle" CausesValidation="False" OnClick="step4_Click">4</asp:LinkButton>
                            
        <p><%= Resources.Hazard.hazardstep4 %></p>
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

                              ArrayList per = Session["permission"] as ArrayList;
                             
                              bool pa = safetys4.Class.SafetyPermission.checkPermisionAction("report hazard3 request close", id, "hazard", Convert.ToInt32(Session["group_value"]));
                              bool area = safetys4.Class.SafetyPermission.checkPermisionInArea( id, "hazard");

                              if (per.IndexOf("report hazard3 request close") > -1 && pa == true && area == true)         
                           {                            
       
                          %>
                                  <asp:Button ID="btrequestclose" runat="server" Text="<%$ Resources:Hazard, btRequestclose %>" CssClass="btn btn-primary" CausesValidation="False" OnClick="btrequestclose_Click"/>
               
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
                            
                            bool pa2 = safetys4.Class.SafetyPermission.checkPermisionAction("report hazard3 edit", id2, "hazard", Convert.ToInt32(Session["group_value"]));
                            bool area2 = safetys4.Class.SafetyPermission.checkPermisionInArea(id2, "hazard");

                            if (per2.IndexOf("report hazard3 edit") > -1 && pa2 == true && area2 == true)         
                           {                            
       
                          %>
                               <asp:Button ID="btHazardedit" runat="server" Text="<%$ Resources:Hazard, btHazardedit %>" CssClass="btn btn-primary"  CausesValidation="False" OnClick="btHazardedit_Click"/>

                          <%                      
                            }
       
                          %>
                    </div>
                    </div>
                  </div>
                           
                
         


    
                 <div class="row">
                    <div class="col-md-12">
                          <strong><h3><%= Resources.Hazard.process_action_form %></h3></strong>
                       </div>
                              
                   </div>
     

                 <div  class="row">
                    <div class="col-md-12">
                          <table id="tbProcess_action" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <th> <%= Resources.Hazard.no %></th>
                                    <th></th>
                                    <th> <%= Resources.Hazard.typecontrol %></th>
                                    <th> <%= Resources.Hazard.action %></th>
                                    <th> <%= Resources.Hazard.responsible_person %></th>
                                    <th> <%= Resources.Hazard.lbdepartment_action %></th>
                                    <th> <%= Resources.Hazard.due_date %></th>
                                    <th> <%= Resources.Hazard.status %></th>
                                    <th> <%= Resources.Hazard.date_complete %></th>
                                    <th> <%= Resources.Hazard.attachment %></th>
                                    <th> <%= Resources.Hazard.notify_contractor %></th>
                                    <th> <%= Resources.Hazard.close_action %></th>
                                    <th> <%= Resources.Hazard.remark %></th>
                                    <th> <%= Resources.Incident.manage %></th>
                    
                                </tr>
                            </thead>
                            
                            </table>




                       </div>
                              
                   </div>

                <div  class="row">
                    <div class="col-md-12">
                       <button type="button" id="btCreateProcessAction" class="btn btn-primary" runat="server" onclick="showCreateProcessAction();"><i class="fa fa-plus"></i></button>  <%= Resources.Hazard.add_process_action %>
                       
                       </div>
                              
                   </div>

                <div  class=="row">
                    <div class="col-md-12">
                        <br />
                    </div>
                              
                 </div>
               
              <div class="row">
                    <div class="col-md-12">
                        <strong>
                         <h3><asp:Label ID="header_acknowledge" runat="server" Text="<%$ Resources:Hazard, hazardnameareaowner %>"></asp:Label></h3></strong>
                   
                    </div>
                  </div>

                  <div class="row">                             
                    <div class="col-md-4">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Hazard.hazardnamearea %></label>
                        <input id="txtname_areaowner" name="txtname_areaowner"  type="text" class="form-control" runat="server">

                              
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
                    <asp:Button ID="btUpdate" runat="server" Text="<%$ Resources:Main, btSubmit%>"  CssClass="btn btn-primary" OnClientClick="return updateHazard('');" />           
               </div>
            </div>


              

            </div>
        </div>
    </div>
 



                       
                </div>
            </div>
























</asp:Content>
