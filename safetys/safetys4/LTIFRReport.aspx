<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="LTIFRReport.aspx.cs" Inherits="safetys4.LTIFRReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">
    
<link href="template/css/plugins/jqGrid/ui.jqgrid-bootstrap-ui.css" rel="stylesheet">
<link href="template/css/plugins/jqGrid/ui.jqgrid.css" rel="stylesheet">

<script type="text/javascript" src="template/js/plugins/jqGrid/i18n/grid.locale-en.js"></script>
<script type="text/javascript" src="template/js/plugins/jqGrid/jquery.jqGrid.min.js"></script>
<script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker-custom.js"></script>
<script type="text/javascript" src="template/js/plugins/datepicker/locales/bootstrap-datepicker.th.js"></script>

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
    $.jgrid.defaults.width = 780;
    $.jgrid.defaults.responsive = true;
    $.jgrid.defaults.styleUI = 'Bootstrap';
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



     setCompany('', '', '');



     
 });

    function showChildGrid(parentRowID, parentRowKey)
    {
        var function_id = $("#MainContent_ddfunction").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();

        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";


        if (parentRowKey != "")
        {
            // send the parent row primary key to the server so that we know which grid to show
            var childGridURL = 'Report.asmx/getListLTIFREmployeeFunctionReport?companyid=' + parentRowKey + '&functionid=' + function_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;

            // add a table and pager HTML elements to the parent grid row - we will render the child grid here
            $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

            $("#" + childGridID).jqGrid({
                url: childGridURL,
                mtype: "GET",
                datatype: "json",
                page: 1,
                colModel: [
                    { label: 'ID', name: 'function_id', key: true, hidden: true },
                    { label: '<%= Resources.Incident.lbfucntion %>', name: 'functionname' },
                    { label: 'Target', name: 'target' },
                    { label: 'No. of LTI', name: 'lti' },
                    { label: 'Hours worked', name: 'workhours' },
                    { label: 'LTIFR', name: 'Ltifr' },
                    { label: 'Day lost', name: 'daylost' },
                    { label: 'LTISR', name: 'Ltisr' }
                ],
                // loadonce: true,
                rowNum: 1000,
                height: '90%',
                //width: '100%',
                autowidth: true,
                shrinkToFit: true,
                subGrid: true, // set the subGrid property to true to show expand buttons for each row
                subGridRowExpanded: showThirdLevelChildGrid, // javascript function that will take care of showing the child grid
                //pager: "#" + childGridPagerID
            });

        }
       

    }


    function showChildOnSiteGrid(parentRowID, parentRowKey)
    {
        var function_id = $("#MainContent_ddfunction").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();

        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";


        if (parentRowKey != "") {
            // send the parent row primary key to the server so that we know which grid to show
            var childGridURL = 'Report.asmx/getListLTIFRContractorOnsiteFunctionReport?companyid=' + parentRowKey + '&functionid=' + function_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;

            // add a table and pager HTML elements to the parent grid row - we will render the child grid here
            $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

            $("#" + childGridID).jqGrid({
                url: childGridURL,
                mtype: "GET",
                datatype: "json",
                page: 1,
                colModel: [
                    { label: 'ID', name: 'function_id', key: true, hidden: true },
                    { label: '<%= Resources.Incident.lbfucntion %>', name: 'functionname' },
                    { label: 'Target', name: 'target' },
                    { label: 'No. of LTI', name: 'lti' },
                    { label: 'Hours worked', name: 'workhours' },
                    { label: 'Day lost', name: 'daylost' },
                    { label: 'LTIFR', name: 'Ltifr' }
                ],
                // loadonce: true,
                rowNum: 1000,
                height: '90%',
                //width: '100%',
                autowidth: true,
                shrinkToFit: true,
                subGrid: true, // set the subGrid property to true to show expand buttons for each row
                subGridRowExpanded: showThirdLevelChildOnSiteGrid, // javascript function that will take care of showing the child grid
                //pager: "#" + childGridPagerID
            });

        }


    }


    function showChildEmployeeContractorOnsiteOffsiteGrid(parentRowID, parentRowKey) {
        var function_id = $("#MainContent_ddfunction").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();

        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";


        if (parentRowKey != "") {
            // send the parent row primary key to the server so that we know which grid to show
            var childGridURL = 'Report.asmx/getListLTIFREmployeeContractorOnsiteOffsiteFunctionReport?companyid=' + parentRowKey + '&functionid=' + function_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;

            // add a table and pager HTML elements to the parent grid row - we will render the child grid here
            $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

            $("#" + childGridID).jqGrid({
                url: childGridURL,
                mtype: "GET",
                datatype: "json",
                page: 1,
                colModel: [
                    { label: 'ID', name: 'function_id', key: true, hidden: true },
                    { label: '<%= Resources.Incident.lbfucntion %>', name: 'functionname' },
                    { label: 'Target', name: 'target' },
                    { label: 'No. of LTI', name: 'lti' },
                    { label: 'Hours worked', name: 'workhours' },
                    { label: 'Day lost', name: 'daylost' },
                    { label: 'LTIFR', name: 'Ltifr' },
                
                ],
                // loadonce: true,
                rowNum: 1000,
                height: '90%',
                //width: '100%',
                autowidth: true,
                shrinkToFit: true,
                subGrid: true, // set the subGrid property to true to show expand buttons for each row
                subGridRowExpanded: showThirdLevelChildEmployeeContractorOnsiteOffsiteGrid, // javascript function that will take care of showing the child grid
                //pager: "#" + childGridPagerID
            });

        }


    }


    function showChildOffSiteGrid(parentRowID, parentRowKey)
    {
        var function_id = $("#MainContent_ddfunction").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();

        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";


        if (parentRowKey != "") {
            // send the parent row primary key to the server so that we know which grid to show
            var childGridURL = 'Report.asmx/getListLTIFRContractorOffsiteFunctionReport?companyid=' + parentRowKey + '&functionid=' + function_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;

            // add a table and pager HTML elements to the parent grid row - we will render the child grid here
            $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

            $("#" + childGridID).jqGrid({
                url: childGridURL,
                mtype: "GET",
                datatype: "json",
                page: 1,
                colModel: [
                    { label: 'ID', name: 'function_id', key: true, hidden: true },
                    { label: '<%= Resources.Incident.lbfucntion %>', name: 'functionname' },
                    { label: 'Target', name: 'target' },
                    { label: 'No. of LTI', name: 'lti' },
                    { label: 'Hours worked', name: 'workhours' },
                    { label: 'Day lost', name: 'daylost' },
                    { label: 'LTIFR', name: 'Ltifr' }
                ],
                // loadonce: true,
                rowNum: 1000,
                height: '90%',
                //width: '100%',
                autowidth: true,
                shrinkToFit: true,
                subGrid: true, // set the subGrid property to true to show expand buttons for each row
                subGridRowExpanded: showThirdLevelChildOffSiteGrid, // javascript function that will take care of showing the child grid
                //pager: "#" + childGridPagerID
            });

        }


    }



    // the event handler on expanding parent row receives two parameters
    // the ID of the grid tow  and the primary key of the row
    function showThirdLevelChildGrid(parentRowID, parentRowKey)
    {
        var function_id = $("#MainContent_ddfunction").val();
        var department_id = $("#MainContent_dddepartment").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();


        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";


        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = 'Report.asmx/getListLTIFREmployeeDepartmentReport?functionid=' + parentRowKey + '&departmentid=' + department_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;


        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: childGridURL,
            mtype: "GET",
            datatype: "json",
            colModel: [
                { label: 'ID', name: 'department_id', key: true, hidden: true },
                { label: '<%= Resources.Incident.lbdepartment %>', name: 'departmentname'},
                { label: 'Target', name: 'target'},
                { label: 'No. of LTI', name: 'lti' },
                { label: 'Hours worked', name: 'workhours' },
                { label: 'LTIFR', name: 'Ltifr' },
                { label: 'Day lost', name: 'daylost' },
                { label: 'LTISR', name: 'Ltisr' }
            ],
            //loadonce: true,
            rowNum: 1000,
            height: '90%',
            width: '100%',
            shrinkToFit: true,
           // pager: "#" + childGridPagerID
        });

    }


    // the event handler on expanding parent row receives two parameters
    // the ID of the grid tow  and the primary key of the row
    function showThirdLevelChildEmployeeContractorOnsiteOffsiteGrid(parentRowID, parentRowKey) {
        var function_id = $("#MainContent_ddfunction").val();
        var department_id = $("#MainContent_dddepartment").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();


        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";


        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = 'Report.asmx/getListLTIFREmployeeContractorOnsiteOffsiteDepartmentReport?functionid=' + parentRowKey + '&departmentid=' + department_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;


        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: childGridURL,
            mtype: "GET",
            datatype: "json",
            colModel: [
                { label: 'ID', name: 'department_id', key: true, hidden: true },
                { label: '<%= Resources.Incident.lbdepartment %>', name: 'departmentname' },
                { label: 'Target', name: 'target' },
                { label: 'No. of LTI', name: 'lti' },
                { label: 'Hours worked', name: 'workhours' },
                { label: 'Day lost', name: 'daylost' },
                { label: 'LTIFR', name: 'Ltifr' },
            ],
            //loadonce: true,
            rowNum: 1000,
            height: '90%',
            width: '100%',
            shrinkToFit: true,
            // pager: "#" + childGridPagerID
        });

    }



    function showThirdLevelChildOnSiteGrid(parentRowID, parentRowKey)
    {
        var function_id = $("#MainContent_ddfunction").val();
        var department_id = $("#MainContent_dddepartment").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();


        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";


        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = 'Report.asmx/getListLTIFRContractorOnsiteDepartmentReport?functionid=' + parentRowKey + '&departmentid=' + department_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;


        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: childGridURL,
            mtype: "GET",
            datatype: "json",
            colModel: [
                { label: 'ID', name: 'department_id', key: true, hidden: true },
                { label: '<%= Resources.Incident.lbdepartment %>', name: 'departmentname' },
                { label: 'Target', name: 'target' },
                { label: 'No. of LTI', name: 'lti' },
                { label: 'Hours worked', name: 'workhours' },
                { label: 'Day lost', name: 'daylost' },
                { label: 'LTIFR', name: 'Ltifr' }
            ],
            //loadonce: true,
            rowNum: 1000,
            height: '90%',
            width: '100%',
            shrinkToFit: true,
            // pager: "#" + childGridPagerID
        });

    }


    function showThirdLevelChildOffSiteGrid(parentRowID, parentRowKey)
    {
        var function_id = $("#MainContent_ddfunction").val();
        var department_id = $("#MainContent_dddepartment").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();


        var childGridID = parentRowID + "_table";
        var childGridPagerID = parentRowID + "_pager";


        // send the parent row primary key to the server so that we know which grid to show
        var childGridURL = 'Report.asmx/getListLTIFRContractorOffsiteDepartmentReport?functionid=' + parentRowKey + '&departmentid=' + department_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;


        // add a table and pager HTML elements to the parent grid row - we will render the child grid here
        $('#' + parentRowID).append('<table id=' + childGridID + '></table><div id=' + childGridPagerID + ' class=scroll></div>');

        $("#" + childGridID).jqGrid({
            url: childGridURL,
            mtype: "GET",
            datatype: "json",
            colModel: [
                { label: 'ID', name: 'department_id', key: true, hidden: true },
                { label: '<%= Resources.Incident.lbdepartment %>', name: 'departmentname' },
                { label: 'Target', name: 'target' },
                { label: 'No. of LTI', name: 'lti' },
                { label: 'Hours worked', name: 'workhours' },
                { label: 'Day lost', name: 'daylost' },
                { label: 'LTIFR', name: 'Ltifr' }
            ],
            //loadonce: true,
            rowNum: 1000,
            height: '90%',
            width: '100%',
            shrinkToFit: true,
            // pager: "#" + childGridPagerID
        });

    }


    function filterTable()
    { 
        Page_ClientValidate();
 
        if (Page_IsValid)
        {

            showLoading();
            //searchTable();
            reloadGrid();

        }
        return false;
    }


    function reloadGrid()
    {
        //$("#tb_employee").trigger("reloadGrid");

        var company_id = $('#MainContent_ddcompany').val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();


        var data_employee_contractor = 'Report.asmx/getListLTIFREmployeeContractorOnsiteOffsiteCompanyReport?companyid=' + company_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;

        $("#tb_employee_contractor_onsite_offsite").jqGrid('setGridParam', {
            url: data_employee_contractor
        }).trigger('reloadGrid');


        var data = 'Report.asmx/getListLTIFREmployeeCompanyReport?companyid=' + company_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;

        $("#tb_employee").jqGrid('setGridParam', {
           url:data
        }).trigger('reloadGrid');


        var data_onsite = 'Report.asmx/getListLTIFRContractorOnsiteCompanyReport?companyid=' + company_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;

        $("#tb_contractor_onsite").jqGrid('setGridParam', {
            url: data_onsite
        }).trigger('reloadGrid');

        

        var data_offsite = 'Report.asmx/getListLTIFRContractorOffsiteCompanyReport?companyid=' + company_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;

        $("#tb_contractor_offsite").jqGrid('setGridParam', {
            url: data_offsite
        }).trigger('reloadGrid');

        

        

    }

    function searchTable()
    {
        var company_id = $('#MainContent_ddcompany').val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();


        var data_employee_onsite_offsite = "";


        $("#tb_employee_contractor_onsite_offsite").jqGrid({
            url: data_employee_onsite_offsite,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { label: 'ID', name: 'company_id', key: true, hidden: true },
                { label: '<%= Resources.Incident.lbCompany %>', name: 'companyname' },
                { label: 'Target', name: 'target' },
                { label: 'No. of LTI', name: 'lti' },
                { label: 'Hours worked', name: 'workhours' },
                { label: 'Day lost', name: 'daylost' },
                { label: 'LTIFR', name: 'Ltifr' },
            ],
            // loadonce: true,
            // autoheight: true,
            autowidth: true,
            height: '100%',
            // width: '100%',
            shrinkToFit: true,
            rowNum: 1000,
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridRowExpanded: showChildEmployeeContractorOnsiteOffsiteGrid, // javascript function that will take care of showing the child grid
            pager: "#jqGridPagerEmployeeContractorOnsiteOffsite",
            gridComplete: function () {
                closeLoading();
            }
        });






        var data = "";//'Report.asmx/getListLTIFREmployeeCompanyReport?companyid='+company_id+'&date_start='+date_start+'&date_end='+date_end+'&lang='+lang;
        

       $("#tb_employee").jqGrid({
            url: data,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { label: 'ID', name: 'company_id', key: true, hidden: true},
                { label: '<%= Resources.Incident.lbCompany %>', name: 'companyname'},
                { label: 'Target', name: 'target'},
                { label: 'No. of LTI', name: 'lti' },
                { label: 'Hours worked', name: 'workhours' },
                { label: 'LTIFR', name: 'Ltifr' },
                { label: 'Day lost', name: 'daylost' },
                { label: 'LTISR', name: 'Ltisr' }
            ],
           // loadonce: true,
           // autoheight: true,
            autowidth :true,
            height: '100%',
           // width: '100%',
            shrinkToFit: true,
            rowNum: 1000,
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridRowExpanded: showChildGrid, // javascript function that will take care of showing the child grid
            pager: "#jqGridPagerEmployee",
            gridComplete: function () {
              //  closeLoading();
            }
       });



        var data_onsite = "";//'Report.asmx/getListLTIFRContractorOnsiteCompanyReport?companyid=' + company_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;


        $("#tb_contractor_onsite").jqGrid({
            url: data_onsite,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { label: 'ID', name: 'company_id', key: true, hidden: true },
                { label: '<%= Resources.Incident.lbCompany %>', name: 'companyname' },
                { label: 'Target', name: 'target' },
                { label: 'No. of LTI', name: 'lti' },
                { label: 'Hours worked', name: 'workhours' },
                { label: 'Day lost', name: 'daylost' },
                { label: 'LTIFR', name: 'Ltifr' }
            ],
            // loadonce: true,
            // autoheight: true,
            autowidth: true,
            height: '100%',
            // width: '100%',
            shrinkToFit: true,
            rowNum: 1000,
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridRowExpanded: showChildOnSiteGrid, // javascript function that will take care of showing the child grid
            pager: "#jqGridPagerOnsite",
            gridComplete: function () {
               // closeLoading();
            }
        });


        var data_offsite = "";//'Report.asmx/getListLTIFRContractorOffsiteCompanyReport?companyid=' + company_id + '&date_start=' + date_start + '&date_end=' + date_end + '&lang=' + lang;


        $("#tb_contractor_offsite").jqGrid({
            url: data_offsite,
            mtype: "GET",
            datatype: "json",
            page: 1,
            colModel: [
                { label: 'ID', name: 'company_id', key: true, hidden: true },
                { label: '<%= Resources.Incident.lbCompany %>', name: 'companyname' },
                { label: 'Target', name: 'target' },
                { label: 'No. of LTI', name: 'lti' },
                { label: 'Hours worked', name: 'workhours' },
                { label: 'Day lost', name: 'daylost' },
                { label: 'LTIFR', name: 'Ltifr' }
            ],
            // loadonce: true,
            // autoheight: true,
            autowidth: true,
            height: '100%',
            // width: '100%',
            shrinkToFit: true,
            rowNum: 1000,
            subGrid: true, // set the subGrid property to true to show expand buttons for each row
            subGridRowExpanded: showChildOffSiteGrid, // javascript function that will take care of showing the child grid
            pager: "#jqGridPagerOffsite",
            gridComplete: function () {
               // closeLoading();
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


    function setDepartment(function_id, department_id)
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
                searchTable();
               
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
                                    
                            <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server" name="ddfunction">
                              
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
       
               <div class ="row">
                   
                 <div class="col-md-3">
                    <div class="form-group">
                          <label class="control-label"><%= Resources.Incident.date %></label> <div class="lbrequire"> *</div>                    
                      <div id="data_start_date" class="form-group">
                                      
                            <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtstart_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                             
                            </div>
                            <asp:RequiredFieldValidator ID="rqstartdate" runat="server" ControlToValidate ="txtstart_date" ErrorMessage="<%$ Resources:Main, lbstartdate %>" CssClass="text-danger" Display="Dynamic">
                               </asp:RequiredFieldValidator>   
                                        
                    </div>
                                     
                    </div>
                </div>
              

                <div class="col-md-3">
                    <div class="form-group">
                         <label class="control-label"><%= Resources.Incident.to %></label><div class="lbrequire"> *</div>
                        <div id="data_end_date" class="form-group">
                                      
                            <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtend_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                       
                            </div>
                             <asp:RequiredFieldValidator ID="rqenddate" runat="server" ControlToValidate ="txtend_date" ErrorMessage="<%$ Resources:Main, lbenddate %>" CssClass="text-danger" Display="Dynamic">
                               </asp:RequiredFieldValidator>   
                                        
                    </div>
                                          
                    </div>
                </div>
             

               <div class="col-md-2">
                    <div class="form-group">
                        <br />
                               
                
                         <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterTable();" CssClass="btn btn-primary"/>
                      
                    </div>
       
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
        <h3>LTIFR Employee and Contractor onsite & offsite</h3>
        <table id="tb_employee_contractor_onsite_offsite">
        </table>
           
        <br />
        <br />

            <hr class="hr-line-solid">
        <br />
       
        <div class="row">
        <div class="col-md-12">
           
        </div>
        </div>
            <h3>LTIFR Employee</h3>
              <table id="tb_employee">
               </table>
             <%-- <div id="jqGridPagerEmployee"></div>--%>
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
             <table id="tb_contractor_onsite">
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
            <h3>LTIFR Contractor Offsite</h3>
             <table id="tb_contractor_offsite">             
            </table>
        </div>
    </div>

 

</asp:Content>
