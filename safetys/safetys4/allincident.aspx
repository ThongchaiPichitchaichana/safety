<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="allincident.aspx.cs" Inherits="safetys4.allincident" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


<%--    <link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
    <script src="template/js/plugins/dataTables/jquery.dataTables.min.js"></script>--%>

  <link href="template/css/plugins/dataTables/jquery.datatable2.min.css" rel="stylesheet">
    <script src="template/js/plugins/dataTables/jquery.datatable2.min.js"></script>

      <link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">
  

<script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker-custom.js"></script>
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
        #tbAllincident tbody tr {
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
         var function_id = '<%= Session["function_id"] %>';
         var function_name = '<%= Session["function"] %>';
         var company_id = '<%= Session["company_id"] %>';
         //var current_year = '<%= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year %>';
         var department_id = "";
         var division_id = "";
         var section_id = "";
         var tbAllincident;
         var status = "";

         var date_start_default = "";
         var date_end_default = "";

         var type_search = "";
         var form = "";
         var day = "";

         $(document).ready(function () {


                         <%
                
                        if (Request.QueryString["form"]!=null)         
                        {  

                        %>
                         form = '<%= Request.QueryString["form"] %>';
                         day = '<%= Request.QueryString["day"] %>';
                         company_id = "";
                         function_id = "";


                        <%                      
                        }

                        %>

         

                       <%
                
                        if (Request.QueryString["company_id"]!=null)         
                        {  

                        %>
                         company_id = '<%= Request.QueryString["company_id"] %>';
                         department_id = '<%= Request.QueryString["department_id"] %>';
                         division_id = '<%= Request.QueryString["division_id"] %>';
                         function_id = '<%= Request.QueryString["function_id"] %>';

                     <%                      
                        }else if(Session["company_id_selected_incident"]!=null){

                        %>
                         company_id = '<%= Session["company_id_selected_incident"] %>';
                         department_id = '<%= Session["department_id_selected_incident"] %>';
                         division_id = '<%= Session["division_id_selected_incident"] %>';
                         section_id = '<%= Session["section_id_selected_incident"] %>';
                         function_id = '<%= Session["function_id_selected_incident"] %>';
                         status = '<%= Session["status_selected_incident"] %>';
                         
                         date_start_default = '<%= Session["date_start_selected_incident"] %>';
                         date_end_default = '<%= Session["date_end_selected_incident"] %>';
             
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


             setCompany(company_id, function_id,department_id,division_id,section_id);
           
             $("input:radio[name='ctl00$MainContent$type_area']").filter('[value=AREA]').prop('checked', true);

         });


         function filterAllincident()
         {
             showLoading();
             type_search = "filter";

             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var ddl_division_id = $('#MainContent_dddivision').val();
             var ddl_status_id = $('#MainContent_ddstatus').val();
             var date_start = $("#MainContent_txtstart_date").val();
             var date_end = $("#MainContent_txtend_date").val();
             var employee_id = $("#MainContent_txtemployee_id").val();
             var ddl_section_id = $('#MainContent_ddsection').val();
             var type_area = $("input:radio[name='ctl00$MainContent$type_area']:checked").val();

             if (type_area == undefined)//for srilanka
             {
                 type_area = "AREA"

             }

             tbAllincident.ajax.url('Datatablelist.asmx/getListAllIncident?company_id=' + ddl_company_id +
                                    '&function_id=' + ddl_function_id +
                                    '&department_id=' + ddl_department_id +
                                    '&division_id=' + ddl_division_id +
                                    '&section_id=' + ddl_section_id +
                                    '&status=' + ddl_status_id +
                                    "&date_start=" + date_start +
                                    "&date_end=" + date_end +
                                    "&employee_id=" + employee_id +
                                    '&type=filter' +
                                    "&form=" + form +
                                    "&day=" + day +
                                    "&type_area=" + type_area +
                                    '&lang=' + lang).load();

 

             return false;
         }

     


         function setDatatableAreamanagement()
         {
            
             tbAllincident = $("#tbAllincident").DataTable({
                 "bProcessing": true,
                 "sProcessing": true,

                 "bPaginate": true,
                 "bInfo": true,
                 "bFilter": true,
                 "ordering": false,
                 // "stateSave": true,
                 "responsive": false,
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
                    {
                        "targets": [1],
                        "visible": false,
                    }
                 
                 ],
                 dom: '<"html5buttons"B>lTfgtp<"bottom">i',
                
                 buttons: [                  
                  
                     {
                       
                         extend: 'excel', title: '<%= Resources.Incident.header %>', text: '<%= Resources.Main.btdownload %>', exportOptions: {
                             columns: ':visible'
                         }
                     },
                   
                 ],
                 "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                     //$('td:eq(2)', nRow).addClass("text-red");

                 },
                 "fnDrawCallback": function (oSettings) {
                     closeLoading();

                     //if (type_search=="filter")
                     //{
                     //        var url = "allincident.aspx";
                     //        window.location.href = url;
                     //}


                 }
             });

             var ddl_company_id = $('#MainContent_ddcompany').val();
             var ddl_function_id = $('#MainContent_ddfunction').val();
             var ddl_department_id = $('#MainContent_dddepartment').val();
             var ddl_division_id = $('#MainContent_dddivision').val();
             var ddl_status_id = $('#MainContent_ddstatus').val();
             var date_start = $("#MainContent_txtstart_date").val();
             var date_end = $("#MainContent_txtend_date").val();
             var ddl_section_id = $('#MainContent_ddsection').val();
             var employee_id = $("#MainContent_txtemployee_id").val();
             var type_area = $("input:radio[name='ctl00$MainContent$type_area']:checked").val();
             type_search = "nofilter";


             if (type_area == undefined)//for srilanka
             {
                 type_area = "AREA"

             }

             tbAllincident.ajax.url('Datatablelist.asmx/getListAllIncident?company_id=' + ddl_company_id +
                                    '&function_id=' + ddl_function_id +
                                    '&department_id=' + ddl_department_id +
                                    '&division_id=' + ddl_division_id +
                                    '&section_id=' + ddl_section_id +
                                    '&status=' + ddl_status_id +
                                    "&date_start=" + date_start +
                                    "&date_end=" + date_end +
                                    "&employee_id=" + employee_id +
                                    '&type=nofilter' +
                                    "&form=" + form +
                                    "&day=" + day +
                                    "&type_area=" + type_area +
                                    '&lang=' + lang);

             $('#tbAllincident tbody').on('click', 'td', function () {
                 
                 if ($(this).index() != 6)
                 {
                     var rowIdx = tbAllincident.cell(this).index().row;
                     var data = tbAllincident.row(rowIdx).data();
                     var url = "incidentform.aspx?pagetype=view&id=" + data[0];
                     window.open(url);

                 }
               

                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    tbAllincident.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
                     

                 
                   
             });

           
            

         }


         function setCompany(company_id, function_id, department_id, division_id, section_id)
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
                     setFunction(company_id, function_id, department_id, division_id, section_id);

                 }
             });

         }



         function setFunction(company_id, function_id, department_id, division_id, section_id)
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
                     setDepartment(function_id, department_id, division_id, section_id);
                 }
             });



         }



         function setDepartment(function_id, department_id, division_id, section_id)
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

                     
                     if(department_id!="")
                     {
                         $("#MainContent_dddepartment").val(department_id);

                     }
                     setDivision(department_id, division_id, section_id);
                    
                 }
             });



         }


         function setDivision(department_id, division_id, section_id)
         {

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


                     if(division_id!="")
                     {
                         $("#MainContent_dddivision").val(division_id);

                     }

                     setSection(division_id, section_id);
                 
                 }
             });



         }


         function setSection(division_id, section_id)
         {
             // alert(section_id);
             $.ajax({
                 type: "POST",
                 data: { division: division_id, lang: lang },
                 url: 'Masterdata.asmx/getSectionbyDivision',
                 dataType: 'json',
                 success: function (json) {

                     var all = '<%= Resources.Main.all %>';
                     var $el = $("#MainContent_ddsection");
                     $el.empty(); // remove old options

                     $el.append($("<option></option>")
                                .attr("value", "").text(all));
                     $.each(json, function (value, key) {

                         $el.append($("<option></option>")
                                 .attr("value", key.id).text(key.name));
                     });

                     if (section_id != "") {
                         $('#MainContent_ddsection').val(section_id);
                     }

                     setStatus(status);
                 }
             });



         }



         function setStatus(status)
         {

             $.ajax({
                 type: "POST",
                 data: {lang: lang },
                 url: 'Masterdata.asmx/getIncidentstatus',
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

                     setDatatableAreamanagement();
                 }
             });



         }



      <%--   function setYear(current_year)
         {

             $.ajax({
                 type: "POST",
                 data: { lang: lang },
                 url: 'Masterdata.asmx/getIncidentyear',
                 dataType: 'json',
                 success: function (json) {

                     var all = '<%= Resources.Main.all %>';
                     var $el = $("#MainContent_ddyear");
                     $el.empty(); // remove old options

                     $el.append($("<option></option>")
                                .attr("value", "").text(all));
                     $.each(json, function (value, key) {

                         $el.append($("<option></option>")
                                 .attr("value", key.id).text(key.year));
                     });

                     $('#MainContent_ddyear').val(current_year);
                     setDatatableAreamanagement();
                 }
             });



         }--%>





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


                 }
             });



         }


         function changDivision()
         {
             var ddl_division_id = $('#MainContent_dddivision').val();

             $.ajax({
                 type: "POST",
                 data: { division: ddl_division_id, lang: lang },
                 url: 'Masterdata.asmx/getSectionbyDivision',
                 dataType: 'json',
                 success: function (json) {

                     var all = '<%= Resources.Main.all %>';
                     var $el = $("#MainContent_ddsection");
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

 


    </script>

      
             <div class="ibox float-e-margins">
                <div class="ibox-title">
                    <h5><%= Resources.Incident.header %></h5> 
                                  
                </div>
                <div class="ibox-content">
                                  <%                       
                                if (Session["country"].ToString()=="thailand")        
                                {                            
       
                              %>
                                   <div class="row">
                                            <div class="col-md-12">
                                                <label class="control-label"><%= Resources.Incident.lb_search_area %></label>
                                                 <div class="form-group">                                     
                                                    <div class="col-sm-6">
                                                      <label> <input value="AREA" id="type_area1" name="type_area" type="radio" runat="server">
                                                        <%= Resources.Incident.responsible_area %></label>
                                                    </div>
                                                    <div class="col-sm-6">
                                                     <label> <input value="ACIIVITY" id="type_area2" name="type_area" type="radio" runat="server">
                                                        <%= Resources.Incident.owner_activity %></label>
                                                    </div>
                                       
                                                </div>

                                            </div>
                              
                                        </div>

                              <%                       
                                }                           
                              %>
                      
                              <div class="row" id="filter">
                                <div class="col-md-2">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbCompany %></label>                         

                                       <select id="ddcompany" name="ddcompany" class="form-control" onchange="changCompany();" runat="server">
                       
                                        </select>
                                     
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbfucntion %></label>
                                    
                                     <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server">
                       
                                        </select>
                                  
                                    </div>
                                </div>

                                       <div class="col-md-3">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbdepartment %></label>                                              
                                    
                                        <select id="dddepartment" class="form-control" onchange="changDepartment();" runat="server">
                       
                                        </select>
                                        
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                    <label class="control-label"><%= Resources.Incident.lbdivision %></label>
                                        
                                         <select id="dddivision" class="form-control" onchange="changDivision();" runat="server">
                       
                                        </select>
                                   </div>
                                </div>

                            
                                  <div class="col-md-2">
                                    <div class="form-group">
                                     <label class="control-label"></label>
                                        <div class="form-inline">
                                   
                                         <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterAllincident();" CssClass="btn btn-primary"/>
                                       
                                            
                                        </div>
                                    </div>
                                </div>


                              
                            </div>

                 <div class="row">

                       <div class="col-md-2">
                                     <div class="form-group">
                                        <label class="control-label"><%= Resources.Incident.lbsection %></label>
                                              
                                         <select id="ddsection" class="form-control"  runat="server">
                       
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

                              <table id="tbAllincident" class="table table-bordered table-hover" >
                                 <thead>
                                    <tr>
                                        <th></th>
                                        <th></th>
                                        <th><%= Resources.Incident.lbincidentno %></th>
                                        <th><%= Resources.Incident.lbincidenttitle %></th>
                                        <th><%= Resources.Incident.lbincidentdetail %></th>
                                        <th><%= Resources.Incident.lbincidentlocation %></th>
                                        <th><%= Resources.Incident.lbincidentdate %></th>
                                        <th><%= Resources.Incident.lbstatus %></th>
                                        <th></th>
                    
                                    </tr>
                                </thead>
         
                        </table>


             
                              
                            </div>

            </div>



</asp:Content>
