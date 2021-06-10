<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="LTIFRSrilankaReport.aspx.cs" Inherits="safetys4.LTIFRSrilankaReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">


<script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker-custom.js"></script>
<script type="text/javascript" src="template/js/plugins/datepicker/locales/bootstrap-datepicker.th.js"></script>



<style type="text/css">
.jqplot-target {
   
    font-size: 14pt !important;
    font-weight:bold;
}

.wrapper-content {
        margin-left: 300px;
    }
</style>
<script>
    var dataTable; //reference to your dataTable
 $(document).ready(function () {


        <%
                
                            if (Session["lang"] != null)         
                            {
                                if (Session["lang"] =="th")
                                {                  
                                %>
                                    $('#data_start_date .input-group.date').datepicker({
                                        todayBtn: "linked",
                                        keyboardNavigation: false,
                                        forceParse: false,
                                        autoclose: true,
                                        format: "dd/mm/yyyy",
                                        language: 'th',
                                        thaiyear: true
                                    });

                                    $('#data_end_date .input-group.date').datepicker({
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

                                    $('#data_start_date .input-group.date').datepicker({
                                        todayBtn: "linked",
                                        keyboardNavigation: false,
                                        forceParse: false,
                                        autoclose: true,
                                        format: "dd/mm/yyyy",
                                        language: 'en',
                                        thaiyear: false
                                    });

                                    $('#data_end_date .input-group.date').datepicker({
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




     setSite();
   //  searchTable();
 
    
     
 });

    

    function filterTable()
    {

        showLoading();
        searchTable();

        return false;
    }

    function searchTable()
    {
        var site_id = $("#MainContent_ddsite").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();


        $.ajax({
            type: "POST",
            data: {
                siteid: site_id,
                date_start: date_start,
                date_end: date_end,
                lang: lang
            },
            url: 'Report.asmx/getListLTIFREmployeeReportSrilanka',
            dataType: 'json',
            success: function (json) {

                $("#bd_employee").html(json.data);
                
            }
        });



        $.ajax({
            type: "POST",
            data: {
                siteid: site_id,
                date_start: date_start,
                date_end: date_end,
                lang: lang
            },
            url: 'Report.asmx/getListLTIFRContractorOnsiteReportSrilanka',
            dataType: 'json',
            success: function (json) {

                $("#bd_contractor_onsite").html(json.data);

            }
        });



        $.ajax({
            type: "POST",
            data: {
                siteid: site_id,
                date_start: date_start,
                date_end: date_end,
                lang: lang
            },
            url: 'Report.asmx/getListLTIFRContractorOffsiteReportSrilanka',
            dataType: 'json',
            success: function (json) {
                closeLoading();
                $("#bd_contractor_offsite").html(json.data);

            }
        });

    }

   

    function setSite()
    {
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getPersonnelSubareaList',
            dataType: 'json',
            success: function (json) {

                var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_ddsite");
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





               <div class="row" id="filter">
                <div class="col-md-3">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Sot.lbsite %></label>
                                    
                            <select id="ddsite" class="form-control"  runat="server">
                       
                            </select>
                                  
                        </div>
                    </div>

                
                   
               
                <div class="col-md-3">
                    <div class="form-group">
                          <label class="control-label"><%= Resources.Incident.date %></label>                     
                      <div id="data_start_date" class="form-group">
                                      
                            <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtstart_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                            </div>
                                        
                    </div>
                                     
                    </div>
                </div>
              

                <div class="col-md-3">
                    <div class="form-group">
                         <label class="control-label"><%= Resources.Incident.to %></label>
                        <div id="data_end_date" class="form-group">
                                      
                            <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtend_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                            </div>
                                        
                    </div>
                                          
                    </div>
                </div>
             

               <div class="col-md-3">
                    <div class="form-group">
                        <br />
                               
                        <%--                        <%
                            ArrayList per2 = Session["permission"] as ArrayList;
                            if (per2.IndexOf("report search") > -1)         
                           {                            
       
                          %>--%>
                                   <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterTable();" CssClass="btn btn-primary"/>
                          <%--  <%                      
                            }
       
                          %>     --%>       
                                            
                        
                    </div>
       
       </div>
   </div>

      <div class="row">
        <div class="col-md-12">
            <asp:Button ID="btExportExcel"  runat="server" Text="<%$ Resources:Main, btExport %>"  CssClass="btn btn-primary" OnClick="btExportExcel_Click"/>
        </div>
    </div>

     
    <br />
    <br />
    <div class="row">
        <div class="col-md-12">
            <h3>LTIFR Employee</h3>
              <table id="tb_employee" class="table table-bordered table-hover">
                 <thead>
                    <tr>
                        <th><%= Resources.Sot.lbsite %></th>
                        <th>Target</th>
                        <th>LTI</th>
                        <th>Work hours</th>
                        <th>LTIFR</th>
                      
                    </tr>
                </thead>
                <tbody id="bd_employee">


                </tbody>
                 
         
        </table>
        <br />
        <br />

            <hr class="hr-line-solid">
        <br />
       
        <div class="row">
        <div class="col-md-12">
           
        </div>
        </div>
            <br />
             <br />
             <h3>LTIFR Contractor Onsite</h3>
             <table id="tb_contractor_onsite" class="table table-bordered table-hover">
                 <thead>
                    <tr>
                         <th><%= Resources.Sot.lbsite %></th>
                        <th>Target</th>
                        <th>LTI</th>
                        <th>Work hours</th>
                        <th>LTIFR</th>
                      
                    </tr>
                </thead>
                <tbody id="bd_contractor_onsite">


                </tbody>
                 
         
        </table>



             <br />
        <br />

            <hr class="hr-line-solid">
        <br />
       
        <div class="row">
        <div class="col-md-12">
           
        </div>
        </div>
            <br />
             <br />
            <h3>LTI Contractor Offsite</h3>
             <table id="tb_contractor_offsite" class="table table-bordered table-hover">
                 <thead>
                    <tr>
                        <th><%= Resources.Sot.lbsite %></th>
                        <th>Target</th>
                        <th>LTI</th>
                        <th>Work hours</th>
                        <%--<th>LTIFR</th>--%>
                      
                    </tr>
                </thead>
                <tbody id="bd_contractor_offsite">


                </tbody>
                 
         
        </table>
        </div>
    </div>

 




</asp:Content>
