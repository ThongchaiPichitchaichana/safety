<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="hazardform2.aspx.cs" Inherits="safetys4.hazardform2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">
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


/*.ui-dialog-titlebar-close {
    visibility: hidden;
  }*/

a {
        color:black !important;
    }

.a-step{
    color:white !important;
}


  .wrapper-content {
    margin-left: 0px !important;
} 


</style>

 <script type="text/javascript">

    var id = "";
    var pagetype = "";
    var dialogRisk;
    var dialogReason;
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

        setReasonReject();

               <%
                
                if (Session["lang"] != null)         
                {
                    if (Session["lang"] =="th")
                    {                  
                       %>
                        $('#data_hazard_date .input-group.date').datepicker({
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

                            $('#data_hazard_date .input-group.date').datepicker({
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

      

        dialogRisk = $("#show_risk").dialog({
            autoOpen: false,
            height: 680,
            width: 990,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {

                $("#show_risk").css('overflow-x', 'hidden');
            },
            modal: true,
        });

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

       
    });


     function setReasonReject()
     {
         $.ajax({
             type: "POST",
             data: { lang: lang },
             url: 'Masterdata.asmx/getReasonRejectHazard',
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

    function setSourceHazard(source_hazard_id)
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getSourceHazard',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddsourcehazard");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                $("#MainContent_ddsourcehazard").val(source_hazard_id);
            }
        });



    }
    function showDialogRisk()
    {
      
        dialogRisk.dialog("open");

     

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
  
    function setShowedit()
    {

        $.ajax({
            type: "POST",
            data: { id: id, user_id: user_login_id, lang: lang },
            url: 'Actionevent.asmx/getHazardbyid',
            dataType: 'json',
            success: function (json) {
                
                $.each(json, function (value, key) {
                 
                    $("#MainContent_txtverifydate").val(key.verifying_date);
                    $("#MainContent_ddsourcehazard").val(key.source_hazard);
                    if (key.level_hazard != null)
                    {
                        $("input[name='ctl00$MainContent$level_hazard'][value='" + key.level_hazard + "']").attr('checked', 'checked');
                    }
                    $("#MainContent_txtname_security").val(key.name_security);
                    $("#MainContent_lbEmployee").text(key.name_modify);
                    $("#MainContent_lbUpdate").text(key.datetime_modify);

                    $("#show_doc_status").html(key.doc_no + ' ' + key.status);

                    if (key.level_hazard != null)
                    {
                        $("input[name='ctl00$MainContent$level_hazard'][value='" + key.level_hazard + "']").attr('checked', 'checked');
                    }
                 
                    
                    setSourceHazard(key.source_hazard);
                   

                    $("#MainContent_ddfatality").val(key.fatality_prevention_element_id);
                    $("#MainContent_txtother_please").val(key.faltality_prevention_element_other);

                    setFatalityPrevent(key.fatality_prevention_element_id);
                   // setFunction(key.form2_function_id)
                  
                });

         
              


            }
        });

    }






    function updateHazard(typebutton)
    {
        Page_ClientValidate();

        var level_hazard = $("input:radio[name='ctl00$MainContent$level_hazard']:checked").val();
        if (level_hazard == undefined)
        {
            level_hazard = "";
        }

        if (Page_IsValid && level_hazard!="")
        {
                showLoading();         

                var verifying_date = $("#MainContent_txtverifydate").val();
                var ddsourcehazard = $("#MainContent_ddsourcehazard").val();

                var ddfatality = $("#MainContent_ddfatality").val();
                var txtother_please = $("#MainContent_txtother_please").val();

                var data_post = JSON.stringify({
                                                verifying_date: verifying_date,
                                                source_hazard: ddsourcehazard,
                                                level_hazard: level_hazard,
                                                safety_officer_id: user_login_id,
                                                fatality_prevention_element_id: ddfatality,
                                                faltality_prevention_element_other: txtother_please,
                                                user_id: user_login_id,
                                                typelogin: type_login,
                                                hazardid: id,
                                                stepform: 2,
                                                typebutton: typebutton,
                                                group_id: user_group_id,
                                            });

                $.ajax({
                    type: "POST",
                    data: data_post,
                    url: 'Actionevent.asmx/updateHazard2',
                    contentType: "application/json; charset=utf-8",
                    success: function (id) {
                        $("#rqlevelhazard").text("");
                        window.location.href = "hazardform2.aspx?pagetype=view&id=" + id;
                    }
                });

                return false;

            } else {

                if (level_hazard == "")
                {
                    var message_levelhazard = '<%= Resources.Hazard.rqLevelhazard %>';
                    $("#rqlevelhazard").text(message_levelhazard);

                }
                       
                return false;

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

         if (reason_type != "") {
             showLoading();
             $.ajax({
                 type: "POST",
                 data: {
                     hazardid: id,
                     reason_reject_type: reason_type,
                     reasonreject: reason,
                     userid: user_login_id,
                     typelogin: type_login,
                     step_form: 2,
                     group_id: user_group_id,

                 },
                 url: 'Actionevent.asmx/updateReasonRejectHazard',
                 dataType: 'json',
                 success: function (result) {

                     closeLoading();
                     dialogReason.dialog("close");
                     $("#MainContent_txtreasonreject").val("");
                     $("#rqreason").text("");
                     //setShowedit();//update status
                     window.location.href = "hazardform2.aspx?pagetype=view&id=" + id;
                 }


             });
         } else {

             var require_reason = '<%= Resources.Hazard.rqreasonreject %>';
             $("#rqreason").text(require_reason);
         }



     }


     function CloseReasonReject()
     {
         dialogReason.dialog("close");
         $("#ddReasonreject").val("");
         $("#MainContent_txtreasonreject").val("");


     }


     

   
</script>
 

   <div id="show_risk">
       <div class="row">
            <div class="col-md-12">
               
                          <%
                           
                              if (Session["lang"]=="en")        
                           {                            
       
                          %>
                               <img src="template/img/Risk_en.png"> 
                          <%                    
                           }
                              else if(Session["lang"]=="th") { 
                          %>
                                <img src="template/img/Risk_th.png"> 

                         <%                    
                           } else if(Session["lang"]=="si") { 
                                  
                          %>
                                 <img src="template/img/Risk_si.png"> 
                          <%                    
                           }
                              
                          %>




                </div>           				
            </div>   
        
    </div>

<div id="create_reason_reject">
         <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Hazard.lbreasonreject %></label><div class="lbrequire"> *</div>
                      <select id="ddReasonreject" class="form-control">
                         
                        </select>                        
                    <label id="rqreason" class="text-danger"></label>  
                     
                </div>
                </div>

                              				
            </div>
       <div class="row">
            <div class="col-md-12">
                <div class="form-group">
                    <label class="control-label"><%= Resources.Hazard.detailreject %></label>
                           
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
            <asp:LinkButton ID="step1" runat="server" CssClass="btn btn-default btn-circle" CausesValidation="False" OnClick="step1_Click">1</asp:LinkButton>
        <p><%= Resources.Hazard.hazardstep1 %></p>
        </div>
        <div class="stepwizard-step">
        <asp:LinkButton ID="step2" runat="server" CssClass="btn btn-primary btn-circle a-step" CausesValidation="False" OnClick="step2_Click">2</asp:LinkButton>
            <p><%= Resources.Hazard.hazardstep2 %></p>
        </div>
        <div class="stepwizard-step">
         <asp:LinkButton ID="step3" runat="server" CssClass="btn btn-default btn-circle" CausesValidation="False" OnClick="step3_Click">3</asp:LinkButton>
                            
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
                              
                              bool pa = safetys4.Class.SafetyPermission.checkPermisionAction("report hazard2 process", id, "hazard", Convert.ToInt32(Session["group_value"]));
                              bool area = safetys4.Class.SafetyPermission.checkPermisionInArea(id, "hazard");

                              if (per.IndexOf("report hazard2 process") > -1 && pa == true && area == true)         
                           {                            
       
                          %>
                               <asp:Button ID="btHazardprocessaction" runat="server" Text="<%$ Resources:Hazard, btHazardprocessaction %>" CssClass="btn btn-primary" CausesValidation="true" OnClick="btHazardprocessaction_Click" />
  
                          <%                      
                            }
       
                          %>



                          <%
                              string id2 = Request.QueryString["id"];

                              ArrayList per2 = Session["permission"] as ArrayList;
                              
                              bool pa2 = safetys4.Class.SafetyPermission.checkPermisionAction("report hazard2 submit", id2, "hazard", Convert.ToInt32(Session["group_value"]));
                              bool area2 = safetys4.Class.SafetyPermission.checkPermisionInArea(id2, "hazard");

                              if (per2.IndexOf("report hazard2 submit") > -1 && pa2 == true && area2 == true)         
                           {                            
       
                          %>
                                <asp:Button ID="btUpdateReport" runat="server" Text="<%$ Resources:Main, btSubmitReport %>"  CssClass="btn btn-primary" OnClientClick="return updateHazard('report');" />           
 
                          <%                      
                            }
       
                          %>
                         

                          <%
                              string id3 = Request.QueryString["id"];
                              ArrayList per3 = Session["permission"] as ArrayList;
                             
                              bool pa3 = safetys4.Class.SafetyPermission.checkPermisionAction("report hazard2 reject", id3, "hazard", Convert.ToInt32(Session["group_value"]));
                              bool area3 = safetys4.Class.SafetyPermission.checkPermisionInArea(id3, "hazard");

                              if (per3.IndexOf("report hazard2 reject") > -1 && pa3 == true && area3 == true)         
                           {                            
       
                          %>
                              <asp:Button ID="btHazardreject" runat="server" Text="<%$ Resources:Hazard, btHazardreject %>" CssClass="btn btn-primary" CausesValidation="False" OnClientClick="return showUpdateReasonReject();"/>

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
                             string id4 = Request.QueryString["id"];
                             ArrayList per4 = Session["permission"] as ArrayList;
                             
                             bool pa4 = safetys4.Class.SafetyPermission.checkPermisionAction("report hazard2 edit", id4, "hazard", Convert.ToInt32(Session["group_value"]));
                             bool area4 = safetys4.Class.SafetyPermission.checkPermisionInArea(id4, "hazard");


                             if (per4.IndexOf("report hazard2 edit") > -1 && pa4 == true && area4 == true)         
                           {                            
       
                          %>
                                <asp:Button ID="btHazardedit" runat="server" Text="<%$ Resources:Hazard, btHazardedit %>" CssClass="btn btn-primary"  CausesValidation="False" OnClick="btHazardedit_Click"/>

                          <%                      
                            }
       
                          %>
                       </div>
                    </div>
                  </div>
                           
                
                
                
                 <div class="row" style="padding-bottom:10px;"> 
                     <div class="col-md-4"><strong><h3><%= Resources.Hazard.verification_hazard %></h3></strong></div></div>

                     <div class="row">
                                <div class="col-md-4">
                                     <div id="data_hazard_date" class="form-group">
                                        <label class="control-label"><%= Resources.Hazard.verifying_date %></label><div class="lbrequire"> *</div>          
                                        
                                         <div class="input-group date">
                                         <input class="form-control" value="" type="text" id="txtverifydate" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                                          </div>
                                        <asp:RequiredFieldValidator ID="rqVerifydate" runat="server" ControlToValidate ="txtverifydate" ErrorMessage="<%$ Resources:Hazard, rqVerifydate %>" CssClass="text-danger" Display="Dynamic"> </asp:RequiredFieldValidator>
                                    
                                    </div>


                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Hazard.source_hazard %></label><div class="lbrequire"> *</div>
                                    
                                     <select id="ddsourcehazard" class="form-control"  runat="server">
                       
                                        </select>
                                  
                                       <asp:RequiredFieldValidator ID="rqsourcehazard" runat="server" ControlToValidate ="ddsourcehazard" ErrorMessage="<%$ Resources:Hazard, rqsourcehazard %>" CssClass="text-danger" Display="Dynamic"> </asp:RequiredFieldValidator>
                                    </div>
                                </div>
                              
                      </div>



                  <div class="row">
                        <div class="col-md-4">
                                <div class="form-group">
                                <label class="control-label"><%= Resources.Hazard.fatality_prevention %></label>                                               
                                 
                                <select id="ddfatality" name="ddcompany" class="form-control" runat="server">
                       
                                </select>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                            <label class="control-label"><%= Resources.Incident.other_please %></label>
                                    
                                <input id="txtother_please" name="txtother_please"  type="text" class="form-control" runat="server">
                            </div>
                        </div>
                              
                      </div>


                    <div class="row"> 
                     <div class="col-md-6"><strong><h3><%= Resources.Hazard.level_hazard %><div class="lbrequire"> *</div>
                                <button id="btGuidRisk" type="button" class="btn btn-danger btn-circle-new" data-toggle="tooltip" data-placement="right" 
                                        title="<%= Resources.Hazard.level_hazard_info %>" onclick="showDialogRisk();">
                                <i class="fa fa-info"></i></button>
                            
                        </h3></strong></div>
                        
                       </div>
                       
                             
                			<div class="row">
                                <div class="col-md-12">
                                   
                                     <div  class="form-group">
                                       
                                           <div style="padding-bottom:10px;">
                                             <label> <input  value="H" id="level_hazard1" name="level_hazard" type="radio" runat="server">
                                            <%= Resources.Hazard.high %> </label>
                                       
                                           </div>
                                           
                                         <div style="padding-bottom:10px;">
                                              <label> <input value="M" id="level_hazard2" name="level_hazard" type="radio" runat="server">
                                            <%= Resources.Hazard.medium %> </label>
                                         </div>
                                          <div style="padding-bottom:10px;">
                                          <label> <input value="L" id="level_hazard3" name="level_hazard" type="radio" runat="server">
                                            <%= Resources.Hazard.low %> </label>
                                         </div>
                                         
                                         <label id="rqlevelhazard" class="text-danger"></label> 
                                    </div>

                                </div>
                              
                            </div>


                  <div class="row">
                    <div class="col-md-12">
                        <strong>
                         <h3><asp:Label ID="header_acknowledge" runat="server" Text="<%$ Resources:Hazard, header_acknowledge %>"></asp:Label></h3></strong>
                   
                    </div>
                  </div>

                  <div class="row">                             
                    <div class="col-md-4">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Hazard.hazardnamesecurity %></label>
                        <input id="txtname_security" name="txtname_security"  type="text" class="form-control" runat="server">

                              
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
