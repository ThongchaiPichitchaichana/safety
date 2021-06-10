<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="incidentform4.aspx.cs" Inherits="safetys4.incidentform4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    
<link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
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
        C: \inetpub\wwwroot\safetys4\safetys4\incidentform4.aspx opacity: 1 !important;
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
    margin-top: 0px !important;
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


.a-step{
    color:black !important;
}



     a {
        color:black ;
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




    #tbDefinitionLevel tbody tr {
         cursor: pointer;
    }

  .selected_colunm {
    background-color: #B0BED9;
  }
     
    .wrapper-content {
    margin-left: 0px !important;
} 

</style>

 <script type="text/javascript">

    var id = "";
    var pagetype = "";
  
    var dataTable; 
  

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
        setShowedit();
        setDatatable();
        $("#hd_boxreason").hide();

        $("input[type=radio][name='ctl00$MainContent$request_close']").change(function () {
            if (this.value == 'C') {
                $("#hd_boxreason").hide();
              

            }
            else if (this.value == 'NC') {
                $("#hd_boxreason").show();
              
            }
        });
       
    });


    function updateIncident()
    {     
      
        var request_close = $("input:radio[name='ctl00$MainContent$request_close']:checked").val();
        if (request_close != undefined)
        {
            showLoading();
           // request_close = "";


            var reason = $("#MainContent_txtreason").val();

            var data_post = JSON.stringify({
                request_close: request_close,
                reason: reason,
                employee_id: user_login_id,
                incidentid: id,
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
                        window.location.href = "incidentform4.aspx?pagetype=view&id=" + id;
                    }

                },
                error: function (xhr) {
                    //  alert('Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
                }
            });

        }else {

            var message = '<%= Resources.Incident.request_close_form4 %>';
            alert(message);
        }

            return false;
  

    }



     
    function setDatatable()
    {

        dataTable = $("#tbList").DataTable({
            "bProcessing": true,
            "sProcessing": true,
       
            "bPaginate": false,
            "bInfo": false,
            "bFilter": false,
            "ordering": false,
            // "stateSave": true,
            "responsive": true,
           // "pageLength": 25,
            "lengthChange": false,
            "order": [],
            "language": {
                "url": 'Langdatatable.aspx'
               
            },
            "columnDefs": [
               {
                   "targets": [0],
                   "visible": false,
               }
            ],

           

        });


        dataTable.ajax.url('Datatablelist.asmx/getListLogRequestCloseIncident?incident_id=' + id + "&lang=" + lang );



    }



    function setShowedit()
    {

        // console.log(id);
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



                });





            }
        });

    }



     //function callDatatable()
     //{

     //    dataTable.ajax.url('Datatablelist.asmx/getListLogRequestCloseIncident?incident_id=' + id + "&lang=" + lang ).load();

     //}


   
</script>

  
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
         <asp:LinkButton ID="step3" runat="server" CssClass="btn btn-default btn-circle a-step" CausesValidation="False" OnClick="step3_Click">3</asp:LinkButton>
                            
        <p><%= Resources.Incident.incidentstep3 %></p>
        </div>
         <div class="stepwizard-step">
            <asp:LinkButton ID="step4" runat="server" CssClass="btn btn-primary btn-circle" CausesValidation="False">4</asp:LinkButton>
                            
        <p><%= Resources.Incident.incidentstep4 %></p>
        </div>
    </div>
</div>

    <div class="row setup-content" id="step-1">
        <div class="col-xs-12">
            <div class="col-lg-12">
                <div class="row">
                    <div class="col-md-12">
                        <br />
                    <div style="font-weight:bold;" id="show_doc_status"></div>              
                    </div>
                  </div>
                <hr>
                 <div class="row">
                    <div class="col-md-12">
                    <div class="pull-right">

                          <%
                              string id = Request.QueryString["id"];

                              ArrayList per = Session["permission"] as ArrayList;
                             
                              bool pa = safetys4.Class.SafetyPermission.checkPermisionAction("report incident4 edit", id, "incident", Convert.ToInt32(Session["group_value"]));
                              bool area = safetys4.Class.SafetyPermission.checkPermisionInArea(id, "incident");

                              if (per.IndexOf("report incident4 edit") > -1 && pa == true && area == true)         
                           {                            
       
                          %>
                               <asp:Button ID="btIncidentedit" runat="server" Text="<%$ Resources:Incident, btIncidentedit %>" CssClass="btn btn-primary"  CausesValidation="False" OnClick="btIncidentedit_Click"/>

                          <%                      
                            }
       
                          %>
                       </div>
                    </div>
                  </div>
                           
                

                 <div  class="row">
                    <div class="col-md-12">
                          <table id="tbList" class="table table-bordered table-hover">
                             <thead>
                                <tr>
                                    <th></th>
                                    <th> <%= Resources.Incident.postion %></th>
                                    <th> <%= Resources.Incident.name_surname %></th>
                                    <th> <%= Resources.Incident.date %></th>
                                    <th> <%= Resources.Incident.approval %></th>               
                                    <th> <%= Resources.Incident.remark %></th>
                                    
                    
                                </tr>
                            </thead>
                           
         
                            </table>




                       </div>
                              
                   </div>
                
                
                 <div  class="row" style="padding-bottom:10px;"> 
                     

                			<div class="row">
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
                    <asp:Button ID="btUpdate" runat="server" Text="<%$ Resources:Main, btSubmit%>"  CssClass="btn btn-primary" OnClientClick="return updateIncident();" />           
               </div>
            </div>
              

            </div>
        </div>
    </div>
 



                       
                </div>
            </div>

</asp:Content>
