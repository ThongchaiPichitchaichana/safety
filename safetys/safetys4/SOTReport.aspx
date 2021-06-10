﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="SOTReport.aspx.cs" Inherits="safetys4.SOTReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
       <link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
    <script src="template/js/plugins/dataTables/jquery.dataTables.min.js"></script>
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



    <%     
     if (!IsPostBack)
     {                  
         %>
            setCompany('', '', '','');
         //alert("dd");
    <%     
                    
        }
               
    %>
     // setPiechart();
     setDatatableList();


    
     
    
     
 });

    function setDatatableList()
    {
        var ddl_company_id = $('#MainContent_ddcompany').val();
        var function_id = $("#MainContent_ddfunction").val();
        var department_id = $("#MainContent_dddepartment").val();
        var division_id = $("#MainContent_dddivision").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();


        dataTable = $("#tbReport").DataTable({
            //"bProcessing": true,
            //"sProcessing": true,

            "bPaginate": true,
            "bInfo": true,
            "bFilter": true,
            "ordering": false,
            // "stateSave": true,
            "responsive": true,
            "pageLength": 20,
            "serverSide": true,
            "lengthChange": false,
            "scrollX": true,
            "order": [],
            "language": {
                "url": 'Langdatatable.aspx'
            },
            "columnDefs": [
               {
                   "targets": [1],
                   //"visible": false,
               }
            ]

        });


        dataTable.ajax.url('Report.asmx/getListSotReport?companyid=' + ddl_company_id + '&functionid=' + function_id + "&departmentid=" + department_id +
            "&divisionid="+division_id+"&date_start="+date_start+"&date_end="+date_end+ "&lang=" + lang);



    }


    function filterAllSOT()
    {

        showLoading();
        setTimeout(function () { closeLoading(); }, 3000);
        searchReport();


        return false;
    }

    function searchReport()
    {
        var ddl_company_id = $('#MainContent_ddcompany').val();
        var function_id = $("#MainContent_ddfunction").val();
        var department_id = $("#MainContent_dddepartment").val();
        var division_id = $("#MainContent_dddivision").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();

        dataTable.ajax.url('Report.asmx/getListSotReport?companyid=' + ddl_company_id + '&functionid=' + function_id + "&departmentid=" + department_id +
           "&divisionid=" + division_id + "&date_start=" + date_start + "&date_end=" + date_end + "&lang=" + lang).load();



    }
   

    function setCompany(company_id, function_id, department_id, division_id)
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

                $('#MainContent_ddcompany').val(company_id);
                setFunction(company_id, function_id, department_id, division_id);

            }
        });

    }


    function setFunction(company_id, function_id, department_id, division_id)
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

                $('#MainContent_ddfunction').val(function_id);
                setDepartment(function_id, department_id, division_id);
            }
        });



    }



    function setDepartment(function_id, department_id, division_id)
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


                $('#MainContent_dddepartment').val(department_id);
                setDivision(department_id, division_id);
            }

        });



    }



    function setDivision(department_id, division_id) {

        $.ajax({
            type: "POST",
            data: { department: department_id, lang: lang },
            url: 'Masterdata.asmx/getDivisionbyDepartment',
            dataType: 'json',
            success: function (json) {


                var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_dddivision");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(all));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

              

                $('#MainContent_dddivision').val(division_id);
                   

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



    function changDepartment()
    {
        var ddl_department_id = $('#MainContent_dddepartment').val();

        $.ajax({
            type: "POST",
            data: { department: ddl_department_id, lang: lang },
            url: 'Masterdata.asmx/getDivisionbyDepartment',
            dataType: 'json',
            success: function (json) {

                var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_dddivision");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(all));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

               
                //changDivision();
            }
        });



    }




</script>





               <div class="row" id="filter">
                   <div class="row">
                        <div class="col-md-3">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.lbCompany %></label>                         

                            <select id="ddcompany" name="ddcompany" class="form-control" onchange="changCompany();" runat="server">
                       
                            </select>
                                     
                        </div>
                  </div>
                <div class="col-md-4">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.lbfucntion %></label>
                                    
                            <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server">
                       
                            </select>
                                  
                        </div>
                    </div>

                   <div class="col-md-3">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.lbdepartment %></label>                                              
                                    
                            <select id="dddepartment" class="form-control" runat="server" onchange="changDepartment();">
                       
                            </select>
                                        
                        </div>
                    </div>


                   </div>
                   


                   <div class="row">
                          <div class="col-md-3">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.lbdivision %></label>
                                        
                                <select id="dddivision" class="form-control" runat="server">
                       
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
             

               <div class="col-md-2">
                    <div class="form-group">
                        <br />
                   
                         <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterAllSOT();" CssClass="btn btn-primary"/>
                            
                                            
                        
                    </div>
       
       </div>


                   </div>
                  
   </div>

      <div class="row">
        <div class="col-md-12">
            <asp:Button ID="btExportExcel"  runat="server" Text="<%$ Resources:Main, btExport %>"  CssClass="btn btn-primary" OnClick="btExportExcel_Click"/>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
              <table id="tbReport" class="table table-bordered table-hover">
                 <thead>
                    <tr>
                        <th><%= Resources.Incident.sequence %></th>
                        <th><%= Resources.Sot.doc_no %></th>
                        <th><%= Resources.Sot.report_date%></th>
                        <th><%= Resources.Sot.lbdate_observation %></th>
                        <th><%= Resources.Sot.lbsottime %></th>
                        <th><%= Resources.Sot.lbcompany %></th>
                        <th><%= Resources.Sot.lbfunction %></th>
                        <th><%= Resources.Sot.lbdepartment%></th>
                        <th><%= Resources.Sot.lbdivision%></th>
                        <th><%= Resources.Sot.lblocation %></th>
                        <th><%= Resources.Sot.lbtypework %></th>
                        <th><%= Resources.Sot.type_employment %></th>
                        <th><%= Resources.Sot.lbcomment %></th>
                        <th><%= Resources.Sot.lbname_reporter %></th>
                        <th><%= Resources.Sot.lbfunction %></th>
                        <th><%= Resources.Sot.lbdepartment %></th>
                        <th><%= Resources.Sot.lbsotteam %></th>
                        <th><%= Resources.Sot.lbmng_level %></th>
                        <th><%= Resources.Sot.lbfunction %></th>
                        <th><%= Resources.Sot.lbdepartment %></th>
                        <%--<th><%= Resources.Sot.lbdivision %></th>--%>
                        <th><%= Resources.Main.lbareamanager %></th>
                        <th><%= Resources.Sot.lbstatus %></th>
                        <th><%= Resources.Main.lbdateclose %></th>

      
                       
                    
                    </tr>
                </thead>
         
        </table>

        </div>
    </div>
</asp:Content>
