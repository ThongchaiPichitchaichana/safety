<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="allsot.aspx.cs" Inherits="safetys4.allsot" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <META HTTP-EQUIV="CACHE-CONTROL" CONTENT="NO-CACHE">
<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">
  
<script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker-custom.js"></script>
<script type="text/javascript" src="template/js/plugins/datepicker/locales/bootstrap-datepicker.th.js"></script>
    
  <link href="template/css/plugins/dataTables/jquery.datatable2.min.css" rel="stylesheet">
    <script src="template/js/plugins/dataTables/jquery.datatable2.min.js"></script>


    <style>
        #tbAllsot tbody tr {
            cursor: pointer;
        }

        .col-lg-12{
             padding-left: 10px;
             padding-right: 10px;
        }

        .buttons-excel {
              background-color: #DE2F13 !important;
              color: white !important;
            }
         .wrapper-content {
            margin-left: 0px !important;
        } 

    </style>
   
     <script>
         var dataTable; //reference to your dataTable
         var country = '<%=Session["country"]%>';
         var function_id = '<%= Session["function_id"] %>';
         var function_name = '<%= Session["function"] %>';
         var company_id = '<%= Session["company_id"] %>';
        // var current_year = '<%= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year %>';
         var department_id = "";
         var division_id = "";
         var tbAllsot;
         var status = "";
         
         var date_start_default = "";
         var date_end_default = "";

         var type_search = "";

         $(document).ready(function () {
            
             <%
                
                if (Request.QueryString["company_id"]!=null)         
                {  

                %>
           
                company_id = '<%= Request.QueryString["company_id"] %>';
                department_id = '<%= Request.QueryString["department_id"] %>';
                function_id = '<%= Request.QueryString["function_id"] %>';
                
             <%                      
                }
                else if (Session["company_id_selected_sot"] != null)
                {

                %>
            
                 company_id = '<%= Session["company_id_selected_sot"] %>';
                 department_id = '<%= Session["department_id_selected_sot"] %>';
                 function_id = '<%= Session["function_id_selected_sot"] %>';
                 status = '<%= Session["status_selected_sot"] %>';
                 date_start_default = '<%= Session["date_start_selected_sot"] %>';
                 date_end_default = '<%= Session["date_end_selected_sot"] %>';
             
             <%  
                                  
                }
                
                %>



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
         
             setCompany(company_id, function_id, department_id);
            
           

         });


       

         function filterAllsot()
         {
             showLoading();
             type_search = "filter";

            // var ddl_country = $('#MainContent_ddcountry').val();
             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
            // var ddl_site_id = $('#MainContent_ddsite').val();
             // var ddl_year_id = $('#MainContent_ddyear').val();
             var ddl_status_id = $('#MainContent_ddstatus').val();
             var date_start = $("#MainContent_txtstart_date").val();
             var date_end = $("#MainContent_txtend_date").val();
             var employee_id = $("#MainContent_txtemployee_id").val();
           
             tbAllsot.ajax.url('Datatablelist.asmx/getListAllsot?company_id=' + ddl_company_id +
                                   //'&country=' + country +
                                   '&function_id=' + ddl_function_id +
                                   '&department_id=' + ddl_department_id +
                                   //'&site_id=' + ddl_site_id +
                                   '&status=' + ddl_status_id +
                                   "&date_start=" + date_start +
                                   "&date_end=" + date_end +
                                   "&employee_id=" + employee_id +
                                   '&type=filter' +
                                   '&lang=' + lang).load();
            

             return false;
         }

      



         function setDatatableAllsot()
         {
            
             tbAllsot = $("#tbAllsot").DataTable({
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
                
                 "language": {
                     "url": 'Langdatatable.aspx'
                 },
                
                 "columnDefs": [
                    {
                        "targets": [0],
                        "visible": false,
                    },
                  
                 
                 ],
                 dom: '<"html5buttons"B>lTfgtp<"bottom">i',
                
                 buttons: [
                    
                  
                     {
                         extend: 'excel', title: '<%= Resources.Sot.all_sot_header %>', text: '<%= Resources.Main.btdownload %>', exportOptions: {
                             columns: ':visible'
                         }
                     },
                   
                 ],
                 "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                     //$('td:eq(2)', nRow).addClass("text-red");

                 },
                 "fnDrawCallback": function (oSettings) {
                     closeLoading();

                     //if (type_search == "filter")
                     //{
                     //    var url = "Allsot.aspx";
                     //    window.location.href = url;
                     //}



                 }
             });

            // var ddl_country = $('#MainContent_ddcountry').val();
             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
            // var ddl_site_id = $('#MainContent_ddsite').val();
             // var ddl_year_id = $('#MainContent_ddyear').val();
             var ddl_status_id = $('#MainContent_ddstatus').val();
             var date_start = $("#MainContent_txtstart_date").val();
             var date_end = $("#MainContent_txtend_date").val();
             var employee_id = $("#MainContent_txtemployee_id").val();

             type_search = "nofilter";

             tbAllsot.ajax.url('Datatablelist.asmx/getListAllsot?company_id=' + ddl_company_id +
                                    //'&country=' + country +
                                    '&function_id=' + ddl_function_id +
                                    '&department_id=' + ddl_department_id +
                                   // '&site_id=' + ddl_site_id +
                                    '&status=' + ddl_status_id +
                                    "&date_start=" + date_start +
                                    "&date_end=" + date_end +
                                    "&employee_id=" + employee_id +
                                    '&type=nofilter' +
                                    '&lang=' + lang);

             $('#tbAllsot tbody').on('click', 'tr', function () {
               
                 var data = tbAllsot.row(this).data();
                 var url = "sotform.aspx?pagetype=view&id=" + data[0];
                 //window.location.href = url;
                 window.open(url);
                 //console.log(data[1]);//process status



                 if ($(this).hasClass('selected')) {
                     $(this).removeClass('selected');
                 }
                 else {
                     tbAllsot.$('tr.selected').removeClass('selected');
                     $(this).addClass('selected');
                 }
             });

           
            

         }


         //function setSite(site_id)
         //{

         //    $.ajax({
         //        type: "POST",
         //        data: { lang: lang },
         //        url: 'Masterdata.asmx/getSitelist',
         //        dataType: 'json',
         //        success: function (json) {

         //            var $el = $("#MainContent_ddsite");
         //            $el.empty(); // remove old options

         //            $el.append($("<option></option>")
         //                       .attr("value", "").text(""));
         //            $.each(json, function (value, key) {

         //                $el.append($("<option></option>")
         //                        .attr("value", key.id).text(key.name));
         //            });



         //            $('#MainContent_ddsite').val(site_id);


         //        }
         //    });



         //}


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

                     $('#MainContent_ddcompany').val(company_id);
                     setFunction(company_id, function_id, department_id);

                 }
             });

         }



         function setFunction(company_id, function_id, department_id)
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
                     setDepartment(function_id, department_id);
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

                     
                     if (department_id != "") {
                         $("#MainContent_dddepartment").val(department_id);

                     }

                     setStatus(status);
                    
                    
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





         function setStatus(status)
         {

             $.ajax({
                 type: "POST",
                 data: { lang: lang },
                 url: 'Masterdata.asmx/getSotstatus',
                 dataType: 'json',
                 success: function (json) {

                     var all = '<%= Resources.Main.all %>';
                     var $el = $("#MainContent_ddstatus");
                     $el.empty(); // remove old options

                     $el.append($("<option></option>")
                                .attr("value", "").text(all));
                     $.each(json, function (value, key) {

                         $el.append($("<option></option>")
                                 .attr("value", key.id).text(key.name));
                     });

                     $("#MainContent_ddstatus").val(status);

                     $("#MainContent_txtstart_date").val(date_start_default);
                     $("#MainContent_txtend_date").val(date_end_default);

                     setDatatableAllsot();

                 }
             });



         }


    </script>

      
             <div class="ibox float-e-margins">
                <div class="ibox-title">
                    <h5><%= Resources.Sot.all_sot_header %></h5> 
                                  
                </div>
                <div class="ibox-content">
                      
                              <div class="row" id="filter">
                                 <%--  <div class="col-md-2">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Sot.lbcountry %></label>                         

                                       <select id="Select1" name="ddcountry" class="form-control" runat="server">
                       
                                        </select>
                                     
                                    </div>
                                </div>--%>
                                <div class="col-md-2">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Sot.lbcompany %></label>                         

                                       <select id="ddcompany" name="ddcompany" class="form-control" onchange="changCompany();" runat="server">
                       
                                        </select>
                                     
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Sot.lbfunction %></label>
                                    
                                     <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server">
                       
                                        </select>
                                  
                                    </div>
                                </div>

                                       <div class="col-md-2">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Sot.lbdepartment %></label>                                              
                                    
                                        <select id="dddepartment" class="form-control"  runat="server">
                       
                                        </select>
                                        
                                    </div>
                                </div>


                                <div class="col-md-2">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbstatus %></label>
                                    
                                  
                                     <select id="ddstatus" class="form-control"  runat="server">
                       
                                        </select>
                                      
                                     
                                    </div>
                                </div>

                               

                              <%--  <div class="col-md-2">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Sot.lbsite %></label>                                              
                                    
                                        <select id="ddsite" class="form-control"  runat="server">
                       
                                        </select>
                                        
                                    </div>
                                </div>--%>

                              

                           

                                  <div class="col-md-2">
                                    <div class="form-group">
                                          <label class="control-label"></label>
                                   <div class="form-inline">
                                  
                                         <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterAllsot();" CssClass="btn btn-primary"/>
                                       
                                            
                                        </div>
                                    </div>
                                </div>


                              
                            </div>

                        <div class="row">
                                        <div class="col-md-2">
                                    <div class="form-group">
                                          <label class="control-label"><%= Resources.Incident.date %></label>                     
                                          <div id="data_start_date" class="form-group">
                                      
                                                <div class="input-group date">
                                                    <input class="form-control" value="" type="text" id="txtstart_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                                                </div>
                                        
                                        </div>
                                     
                                    </div>
                                </div>

                                    <div class="col-md-2">
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
                                     <label class="control-label"><%= Resources.Main.employee_reporter %></label>
                                    <div class="form-group">
                        
                                            <input class="form-control" value="" type="text" id="txtemployee_id" runat="server">                                   
                        
                                        </div>
                                          
                                        </div>
                                    </div>






                                  </div>



                 

                              <table id="tbAllsot" class="table table-bordered table-hover" width="100%" >
                                 <thead>
                                    <tr>
                                        <th></th>
                                        <th><%= Resources.Sot.doc_no %></th>
                                        <th><%= Resources.Sot.lbtypework %></th>
                                       <%-- <th><%= Resources.Sot.lbobservation %></th>--%>
                                        <th><%= Resources.Sot.lbsotdate %></th>
                                        <th><%= Resources.Sot.lbstatus %></th>
                    
                                    </tr>
                                </thead>
         
                        </table>


             
                              
                            </div>

            </div>


</asp:Content>
