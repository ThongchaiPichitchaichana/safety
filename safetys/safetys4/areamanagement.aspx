<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="areamanagement.aspx.cs" Inherits="safetys4.areamanagement" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
 <link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
 <script src="template/js/plugins/dataTables/jquery.dataTables.min.js"></script>

    
<style>

    a {
        color:black !important;
    }

    table.dataTable.select tbody tr,
    table.dataTable thead th:first-child {
         cursor: pointer;
    }

   .cursor {
         cursor: pointer;
    }

   .tabs-container .nav-tabs > li.active > a, .tabs-container .nav-tabs > li.active > a:hover, .tabs-container .nav-tabs > li.active > a:focus {
    background-color:  #f2f3f4 ;
    color: black;
}


.tabs-container .panel-body {
    background-color:  #f2f3f4 !important;
}

.tabs-container .panel-body {
    color: black !important;
}

    .btn_area {
  
        font-size: 12px;
        font-weight: 400;
        line-height: 1.42857;
        padding-bottom: 3px;
        padding-left: 5px;
        padding-right: 5px;
        padding-top: 3px;
        text-align: center;
        vertical-align: middle;
        white-space: nowrap;
    }

 
    td.text-red {
        
        color: red;
    }


    .name_area {

        color: #2E2EFE;
    }

</style>
<script>

    var rows_selected = [];
    var dataTable; //reference to your dataTable   
    var tbAreamanagement;
    var tbAreamanagement2;
    var tbAreamanagement3;

    var company_select = "";
    var function_select = "";
    var department_select = "";
    var division_select = "";
    var section_select = "";

    var active_tab = "#tab-1";

    var id_edit = "";

    var dialog;
    var dialogArea;
   
    $(document).ready(function ()
    {
        dialog = $("#employee-form").dialog({
            autoOpen: false,
            height: 650,
            width: 500,
            modal: true,
          
            close: function () {

            

            },
            open: function (event, ui) {
                
                $("#employee-form").css('overflow-x', 'hidden');
            },
            modal: true,
        });


        dialogArea = $("#create_area").dialog({
            autoOpen: false,
            height: 220,
            width: 400,
            modal: true,

            close: function () {



            },
            open: function (event, ui) {
                clearValidationErrors();
                clearArea();
                $("#create_area").css('overflow-x', 'hidden');
            },
            modal: true,
        });

        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
             active_tab = $(e.target).attr("href") // activated tab
             company_select = "";
             function_select = "";
             department_select = "";
             division_select = "";
             section_select = "";
            
        });


    
        setCompany();
           
    });



    function callOHS()
    {
        // console.log("ddd");
        var ddl_function_id = $('#ddlFunction').val();

        $.ajax({
            type: "POST",
            data: { function_id: ddl_function_id, lang: lang },
            url: 'Datatablelist.asmx/getOHS',
            success: function (result) {

                $("#listOHS").html(result);

            },
        });



    }

    function callDelegateOHS()
    {
        var ddl_function_id = $('#ddlFunction').val();

        $.ajax({
            type: "POST",
            data: {function_id : ddl_function_id, lang: lang },
            url: 'Datatablelist.asmx/getDelegateOHS',
            success: function (result) {

                $("#listDelegate").html(result);

            }
        });



    }





    function AddOHS()
    {
        showLoading();
        var ddl_function_id = $('#ddlFunction').val();
        var data_post = JSON.stringify({ employee_id: rows_selected, function_id: ddl_function_id });
        $.ajax({
            type: "POST",
            data: data_post,
            url: 'Actionevent.asmx/createOHS',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                callOHS();
                
                dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=").load();
                rows_selected = [];
                closeLoading();

            }, error: function (data) {
                console.log(data);
            }
        });

        dialog.dialog("close");

    }

    function AddDelegate()
    {
        var ddl_function_id = $('#ddlFunction').val();
        showLoading();
        var data_post = JSON.stringify({ employee_id: rows_selected, function_id: ddl_function_id });

        $.ajax({
            type: "POST",
            data: data_post,
            url: 'Actionevent.asmx/createDelegateOHS',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                callDelegateOHS();
              
                dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=").load();
                rows_selected = [];
                closeLoading();

            }
        });

        dialog.dialog("close");


    }



    function RemoveAreaOHS()
    {
        $.when(checkSelectArea()).done(function (result) {

            if (result == "false") {
                return;
            }
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: {
                        department_id: department_select,
                        division_id: division_select,
                        section_id: section_select,
                        type: "AreaOHS"

                    },
                    url: 'Actionevent.asmx/removeEmployeeInArea',
                    dataType: 'json',
                    success: function (result) {
                        var ddl_function_id = $('#ddlFunction').val();
                        tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang).load();
                        closeLoading();
                        company_select = "";
                        function_select = "";
                        department_select = "";
                        division_select = "";
                        section_select = "";

                    }
                });
            }

        });
    
    }



    function RemoveAreaAll()
    {
        $.when(checkSelectArea()).done(function (result) {

            if (result == "false") {
                return;
            }
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: {
                        department_id: department_select,
                        division_id: division_select,
                        section_id: section_select,
                        type: "All"

                    },
                    url: 'Actionevent.asmx/removeEmployeeInArea',
                    dataType: 'json',
                    success: function (result) {
                        var ddl_function_id = $('#ddlFunction').val();
                        tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang).load();
                        closeLoading();
                        company_select = "";
                        function_select = "";
                        department_select = "";
                        division_select = "";
                        section_select = "";

                    }
                });
            }

        });

    }


    function RemoveAreaSupervisor()
    {

        $.when(checkSelectArea()).done(function (result) {

            if (result == "false") {
                return;
            }

            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: {
                        department_id: department_select,
                        division_id: division_select,
                        section_id: section_select,
                        type: "AreaSupervisor"

                    },
                    url: 'Actionevent.asmx/removeEmployeeInArea',
                    dataType: 'json',
                    success: function (result) {
                        var ddl_function_id = $('#ddlFunction').val();
                        tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang).load();
                        closeLoading();
                        company_select = "";
                        function_select = "";
                        department_select = "";
                        division_select = "";
                        section_select = "";

                    }
                });
            }

        });

    }


    function RemoveAreaManager()
    {

        $.when(checkSelectArea()).done(function (result) {

            if (result == "false") {
                return;
            }

            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: {
                        department_id: department_select,
                        division_id: division_select,
                        section_id: section_select,
                        type: "AreaManager"

                    },
                    url: 'Actionevent.asmx/removeEmployeeInArea',
                    dataType: 'json',
                    success: function (result) {
                        var ddl_function_id = $('#ddlFunction').val();
                        tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang).load();
                        closeLoading();
                        company_select = "";
                        function_select = "";
                        department_select = "";
                        division_select = "";
                        section_select = "";

                    }
                });
            }

        });

    }



    function RemoveFunctionalManager()
    {

        $.when(checkSelectArea()).done(function (result) {

            if (result == "false") {
                return;
            }

            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete))
            {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: {
                        department_id: department_select,
                        division_id: division_select,
                        section_id: section_select,
                        type: "FunctionalManager"

                    },
                    url: 'Actionevent.asmx/removeEmployeeInArea',
                    dataType: 'json',
                    success: function (result) {
                        var ddl_function_id = $('#ddlFunction').val();
                        tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang).load();
                        closeLoading();
                        company_select = "";
                        function_select = "";
                        department_select = "";
                        division_select = "";
                        section_select = "";

                    }
                });
            }

        });

    }



    function showCreateOHS()
    {
  
        var ddl_function_id = $('#ddlFunction').val();
        dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=").load();

        rows_selected = [];

        $("#MainContent_btAddAreaManager").hide();
        $("#MainContent_btAddAreaOHS").hide();
        $("#MainContent_btAddAreaSupervisor").hide();
        $("#MainContent_btAddFunctionalManager").hide();
        $("#MainContent_btAddDelegate").hide();
        $("#MainContent_btAdd").show();
        dialog.dialog("open");
       

    }


    function setEmployeeSelected()
    {
        dataTable.rows().eq(0).each(function (row) {
            var data = dataTable.cell(row, 0).data();

            if ($.inArray(data, rows_selected) !== -1)
            {
                dataTable.rows(row)
                .nodes()
                .to$()
                .find('input[type="checkbox"]').prop('checked', true);

                dataTable.rows(row)
               .nodes()
               .to$()
               .addClass('selected');

            }

        });

    }

    function showCreateDelegate()
    {
        var ddl_function_id = $('#ddlFunction').val();
        dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=").load();
        rows_selected = [];
        $("#MainContent_btAddDelegate").show();
        $("#MainContent_btAdd").hide();
        $("#MainContent_btAddAreaManager").hide();
        $("#MainContent_btAddAreaOHS").hide();
        $("#MainContent_btAddAreaSupervisor").hide();
        $("#MainContent_btAddFunctionalManager").hide();
        dialog.dialog("open");

    }


    function showCreateAreaManager()
    {
        $.when(checkSelectArea()).done(function (result) {

            if (result == "false") {
                return;
            }

            rows_selected = [];
            showLoading();
            $.ajax({
                type: "POST",
                data: { division_id: division_select },
                url: 'Masterdata.asmx/getEmployeeDivision',
                dataType: 'json',
                success: function (json) {
                    var ddl_function_id = $('#ddlFunction').val();
                    dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=").load();

                    $.each(json, function (value, key) {

                        rows_selected.push(key.employee_id);
                    });

                    setEmployeeSelected();

                    $("#MainContent_btAddDelegate").hide();
                    $("#MainContent_btAdd").hide();
                    $("#MainContent_btAddAreaManager").show();
                    $("#MainContent_btAddAreaOHS").hide();
                    $("#MainContent_btAddAreaSupervisor").hide();
                    $("#MainContent_btAddFunctionalManager").hide();
                    dialog.dialog("open");
                    closeLoading();


                }
            });




        
        });

    }

    function showCreateAreaOHS()
    {
        $.when(checkSelectArea()).done(function (result) {

            if (result == "false") {
                return;
            }

            rows_selected = [];
            showLoading();
            $.ajax({
                type: "POST",
                data: { department_id: department_select },
                url: 'Masterdata.asmx/getEmployeeDepartment',
                dataType: 'json',
                success: function (json) {
                    var ddl_function_id = $('#ddlFunction').val();
                    dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=").load();


                    $.each(json, function (value, key) {

                        rows_selected.push(key.employee_id);
                    });

                    setEmployeeSelected();
                   
                    $("#MainContent_btAddDelegate").hide();
                    $("#MainContent_btAdd").hide();
                    $("#MainContent_btAddAreaManager").hide();
                    $("#MainContent_btAddAreaOHS").show();
                    $("#MainContent_btAddAreaSupervisor").hide();
                    $("#MainContent_btAddFunctionalManager").hide();
                    dialog.dialog("open");
                    closeLoading();


                }
            });

          

          
        });

    }







    function showCreateAreaSupervisor()
    {
        $.when(checkSelectArea()).done(function (result) {

            if (result == "false") {
                return;
            }

            rows_selected = [];
            showLoading();
            $.ajax({
                type: "POST",
                data: { section_id: section_select },
                url: 'Masterdata.asmx/getEmployeeSection',
                dataType: 'json',
                success: function (json) {
                    var ddl_function_id = $('#ddlFunction').val();
                    dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=").load();

                    $.each(json, function (value, key) {

                        rows_selected.push(key.employee_id);
                    });

                    setEmployeeSelected();

                    $("#MainContent_btAddDelegate").hide();
                    $("#MainContent_btAdd").hide();
                    $("#MainContent_btAddAreaManager").hide();
                    $("#MainContent_btAddAreaOHS").hide();
                    $("#MainContent_btAddAreaSupervisor").show();
                    $("#MainContent_btAddFunctionalManager").hide();

                    dialog.dialog("open");
                    closeLoading();


                }
            });


          
        });

    }




    function showCreateFunctionalManager()
    {
        $.when(checkSelectArea()).done(function (result) {

            if (result == "false") {
                return;
            }

            rows_selected = [];
            showLoading();
            $.ajax({
                type: "POST",
                data: { department_id: department_select },
                url: 'Masterdata.asmx/getEmployeeFunctionalManager',
                dataType: 'json',
                success: function (json) {
                    var ddl_function_id = $('#ddlFunction').val();
                    dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=functional").load();

                    $.each(json, function (value, key) {

                        rows_selected.push(key.employee_id);
                    });

                    setEmployeeSelected();

                    $("#MainContent_btAddDelegate").hide();
                    $("#MainContent_btAdd").hide();
                    $("#MainContent_btAddAreaManager").hide();
                    $("#MainContent_btAddAreaOHS").hide();
                    $("#MainContent_btAddAreaSupervisor").hide();
                    $("#MainContent_btAddFunctionalManager").show();

                    dialog.dialog("open");
                    closeLoading();


                }
            });



        });

    }



    function DeleteOHS(id)
    {
        var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
        if (confirm(message_confirm_delete)) {
            showLoading();
            $.ajax({
                type: "POST",
                data: { id: id },
                url: 'Actionevent.asmx/deleteOHS',
                dataType: 'json',
                success: function (json) {

                    callOHS();
                    closeLoading();
                }
            });

        }


    }

    function DeleteDelegateOHS(id)
    {
        var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
        if (confirm(message_confirm_delete)) {
            showLoading();
            $.ajax({
                type: "POST",
                data: { id: id },
                url: 'Actionevent.asmx/deleteDelegateOHS',
                dataType: 'json',
                success: function (json) {

                    callDelegateOHS();
                    closeLoading();
                }
            });
        }


    }


    function CloseDialog()
    {
        dialog.dialog("close");


    }




    function setDataEmployeeList()
    {

        dataTable = $("#tbEmployee").DataTable({
            "bProcessing": true,
            "sProcessing": true,
            "bPaginate": true,
            "bInfo": true,
            "bFilter": true,
            "ordering": true,
            // "stateSave": true,
            "responsive": true,
            "pageLength": 10,
            "lengthChange": true,
            "order": [],
            "language": {
                "url": 'Langdatatable.aspx'
            },
            "columnDefs": [
               {
                   'targets': 0,
                   'searchable': false,
                   'orderable': false,
                   'width': '1%',
                   'className': 'dt-body-center',
                   'render': function (data, type, full, meta) {
                       return '<input type="checkbox">';
                   }
               }
            ],
            'rowCallback': function (row, data, dataIndex) {
                // Get row ID
                var rowId = data[0];
               // alert(row);
                // If row ID is in the list of selected row IDs
                if ($.inArray(rowId, rows_selected) !== -1) {
                    $(row).find('input[type="checkbox"]').prop('checked', true);
                    $(row).addClass('selected');
                }
            }

        });

        var ddl_function_id = $('#ddlFunction').val();

        dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=");
      
        // Handle click on checkbox
        $('#tbEmployee tbody').on('click', 'input[type="checkbox"]', function (e) {
            var $row = $(this).closest('tr');

            // Get row data
            var data = dataTable.row($row).data();

            // Get row ID
            var rowId = data[0];

            // Determine whether row ID is in the list of selected row IDs 
            var index = $.inArray(rowId, rows_selected);

            // If checkbox is checked and row ID is not in list of selected row IDs
            if (this.checked && index === -1) {
                rows_selected.push(rowId);

                // Otherwise, if checkbox is not checked and row ID is in list of selected row IDs
            } else if (!this.checked && index !== -1) {
                rows_selected.splice(index, 1);
            }

            if (this.checked) {
                $row.addClass('selected');
            } else {
                $row.removeClass('selected');
            }

            // Update state of "Select all" control
            updateDataTableSelectAllCtrl(dataTable);

            // Prevent click event from propagating to parent
            e.stopPropagation();
        });

        // Handle click on table cells with checkboxes
        $('#tbEmployee').on('click', 'tbody td, thead th:first-child', function (e) {
            $(this).parent().find('input[type="checkbox"]').trigger('click');
        });

        // Handle click on "Select all" control
        $('thead input[name="select_all"]', dataTable.table().container()).on('click', function (e) {
            if (this.checked) {
                $('#tbEmployee tbody input[type="checkbox"]:not(:checked)').trigger('click');
            } else {
                $('#tbEmployee tbody input[type="checkbox"]:checked').trigger('click');
            }

            // Prevent click event from propagating to parent
            e.stopPropagation();
        });

        // Handle table draw event
        dataTable.on('draw', function () {
            // Update state of "Select all" control
            updateDataTableSelectAllCtrl(dataTable);
        });




    }





    // Updates "Select all" control in a data table
    //
    function updateDataTableSelectAllCtrl(table)
    {
        var $table = table.table().node();
        var $chkbox_all = $('tbody input[type="checkbox"]', $table);
        var $chkbox_checked = $('tbody input[type="checkbox"]:checked', $table);
        var chkbox_select_all = $('thead input[name="select_all"]', $table).get(0);

        // If none of the checkboxes are checked
        if ($chkbox_checked.length === 0) {
            chkbox_select_all.checked = false;
            if ('indeterminate' in chkbox_select_all) {
                chkbox_select_all.indeterminate = false;
            }

            // If all of the checkboxes are checked
        } else if ($chkbox_checked.length === $chkbox_all.length) {
            chkbox_select_all.checked = true;
            if ('indeterminate' in chkbox_select_all) {
                chkbox_select_all.indeterminate = false;
            }

            // If some of the checkboxes are checked
        } else {
            chkbox_select_all.checked = true;
            if ('indeterminate' in chkbox_select_all) {
                chkbox_select_all.indeterminate = true;
            }
        }
    }


    function changCompany()
    {
        var ddl_company_id = $('#ddlCompany').val();
      
        $.ajax({
            type: "POST",
            data: { company: ddl_company_id, lang: lang },
            url: 'Masterdata.asmx/getFuctionByCompanyAll',
            dataType: 'json',
            success: function (json) {

                var $el = $("#ddlFunction");
                $el.empty(); // remove old options
                var select = '<%= Resources.Main.select %>';
                $el.append($("<option></option>")
                           .attr("value", "").text(select));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

    
            }
        });


          
    }



    function setFunction()
    {
        var ddl_company_id = $('#ddlCompany').val();

        $.ajax({
            type: "POST",
            data: { company: ddl_company_id, lang: lang },
            url: 'Masterdata.asmx/getFuctionByCompanyAll',
            dataType: 'json',
            success: function (json) {

                var $el = $("#ddlFunction");
                $el.empty(); // remove old options
                var select = '<%= Resources.Main.select %>';
                $el.append($("<option></option>")
                           .attr("value", "").text(select));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                callOHS();
                callDelegateOHS();
                setDataEmployeeList();
                setDatatableAreamanagement();
                setDatatableAreamanagement2();
                setDatatableAreamanagement3();

            }
        });



    }




    function setCompany()
    {
       
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getCompanyAll',
            dataType: 'json',
            success: function (json) {

                var $el = $("#ddlCompany");
                $el.empty(); // remove old options
                var select = '<%= Resources.Main.select %>';
                $el.append($("<option></option>")
                           .attr("value", "").text(select));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                setFunction();
               
            }
        });

    }



    function filterArea()
    {
        showLoading();
        callOHS();
        callDelegateOHS();

        var ddl_function_id = $('#ddlFunction').val();

        tbAreamanagement.ajax.url('Datatablelist.asmx/getListAreaManagement?function_id=' + ddl_function_id + "&lang=" + lang).load();
        tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang).load();
        tbAreamanagement3.ajax.url('Datatablelist.asmx/getListAreaManagement3?function_id=' + ddl_function_id + "&lang=" + lang).load();
       

        $('#tbAreamanagement tbody')
           .on('mouseenter', 'td', function () {
               var colIdx = tbAreamanagement.cell(this).index().column;
              // if (colIdx == 5)
              // {
                   $(tbAreamanagement.column(colIdx).nodes()).addClass('cursor');

              // }
             
           });


        $('#tbAreamanagement2 tbody')
           .on('mouseenter', 'td', function () {
               var colIdx = tbAreamanagement2.cell(this).index().column;
               // if (colIdx == 5)
               // {
               $(tbAreamanagement2.column(colIdx).nodes()).addClass('cursor');

               // }

           });


        $('#tbAreamanagement3 tbody')
           .on('mouseenter', 'td', function () {
               var colIdx = tbAreamanagement3.cell(this).index().column;
               // if (colIdx == 5)
               // {
               $(tbAreamanagement3.column(colIdx).nodes()).addClass('cursor');

               // }

           });

       

        $('#tbAreamanagement tbody').on('click', 'td', function () {

            var ddl_company_id = $('#ddlCompany').val();
            var ddl_function_id = $('#ddlFunction').val();
            var rowIdx = tbAreamanagement.cell(this).index().row;
            var depart = tbAreamanagement.column(0).data()[rowIdx];
            var div = tbAreamanagement.column(1).data()[rowIdx];
            var sec = tbAreamanagement.column(2).data()[rowIdx];
            //alert(depart);
        
            company_select = ddl_company_id;
            function_select = ddl_function_id;
            department_select = depart;
            division_select = div;
            section_select = sec;


            if (division_select == null)
            {
                $("#btCreatAreaManager").hide();
                division_select = "";

            }else {
                $("#btCreatAreaManager").show();
            }

            if (section_select == null)
            {
                section_select = "";
                $("#btCreateSupervisor").hide();

            } else {

                $("#btCreateSupervisor").show();
            }

           


            callArea();

         

        });




        $('#tbAreamanagement2 tbody').on('click', 'td', function () {

            var ddl_company_id = $('#ddlCompany').val();
            var ddl_function_id = $('#ddlFunction').val();
            var rowIdx = tbAreamanagement2.cell(this).index().row;
            var depart = tbAreamanagement2.column(0).data()[rowIdx];
            var div = tbAreamanagement2.column(1).data()[rowIdx];
            var sec = tbAreamanagement2.column(2).data()[rowIdx];


            company_select = ddl_company_id;
            function_select = ddl_function_id;
            department_select = depart;
            division_select = div;
            section_select = sec;
          

            if (division_select == null) {
                $("#btCreatAreaManager").hide();
                division_select = "";

            } else {
                $("#btCreatAreaManager").show();
            }

            if (section_select == null) {
                section_select = "";
                $("#btCreateSupervisor").hide();

            } else {

                $("#btCreateSupervisor").show();
            }




            callArea2();



        });




        $('#tbAreamanagement3 tbody').on('click', 'td', function () {

            var ddl_company_id = $('#ddlCompany').val();
            var ddl_function_id = $('#ddlFunction').val();
            var rowIdx = tbAreamanagement3.cell(this).index().row;
            var depart = tbAreamanagement3.column(0).data()[rowIdx];
            var div = tbAreamanagement3.column(1).data()[rowIdx];
            var sec = tbAreamanagement3.column(2).data()[rowIdx];

            company_select = ddl_company_id;
            function_select = ddl_function_id;
            department_select = depart;
            division_select = div;
            section_select = sec;

            if (division_select == null) {
                $("#btCreatAreaManager").hide();
                division_select = "";

            } else {
                $("#btCreatAreaManager").show();
            }

            if (section_select == null) {
                section_select = "";
                $("#btCreateSupervisor").hide();

            } else {

                $("#btCreateSupervisor").show();
            }




            callArea3();



        });

       
     


        return false;
    }




    function setDatatableAreamanagement()
    {

        tbAreamanagement = $("#tbAreamanagement").DataTable({
            "bProcessing": true,
            "sProcessing": true,

            "bPaginate": true,
            "bInfo": true,
            "bFilter": true,
            "ordering": false,
           // "stateSave": true,
            "responsive": true,
            "pageLength": 10,
           // "serverSide": true,
            "lengthChange": false,
           // "displayStart": 11,
            "language": {
                "url": 'Langdatatable.aspx'
            },
            select: true,
            "columnDefs": [
               {
                   "targets": [0],
                   "visible": false,
               },
               {
                   "targets": [1],
                   "visible": false,
               },
               {
                   "targets": [2],
                   "visible": false,
               }
              
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {             
                //$('td:eq(2)', nRow).addClass("text-red");
               
            },
            "fnDrawCallback": function (oSettings) {
                closeLoading();

             

            }
        });

        var ddl_function_id = $('#ddlFunction').val();

        tbAreamanagement.ajax.url('Datatablelist.asmx/getListAreaManagement?function_id=' + ddl_function_id +  "&lang=" + lang);
        $('#tbAreamanagement tbody').on('click', 'tr', function () {

            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            }
            else {
                tbAreamanagement.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });
    


    }




    function setDatatableAreamanagement2()
    {

        tbAreamanagement2 = $("#tbAreamanagement2").DataTable({
            "bProcessing": true,
            "sProcessing": true,

            "bPaginate": true,
            "bInfo": true,
            "bFilter": true,
            "ordering": false,
            // "stateSave": true,
            "responsive": true,
            "pageLength": 10,
            // "serverSide": true,
            "lengthChange": false,
            // "displayStart": 11,
            "language": {
                "url": 'Langdatatable.aspx'
            },
            select: true,
            "columnDefs": [
               {
                   "targets": [0],
                   "visible": false,
               },
               {
                   "targets": [1],
                   "visible": false,
               },
               {
                   "targets": [2],
                   "visible": false,
               }

            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                //$('td:eq(2)', nRow).addClass("text-red");

            },
            "fnDrawCallback": function (oSettings) {
                closeLoading();



            }
        });

        var ddl_function_id = $('#ddlFunction').val();

        tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang);
        $('#tbAreamanagement2 tbody').on('click', 'tr', function () {

            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            }
            else {
                tbAreamanagement2.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });



    }




    function setDatatableAreamanagement3()
    {

        tbAreamanagement3 = $("#tbAreamanagement3").DataTable({
            "bProcessing": true,
            "sProcessing": true,

            "bPaginate": true,
            "bInfo": true,
            "bFilter": true,
            "ordering": false,
            // "stateSave": true,
            "responsive": true,
            "pageLength": 10,
            // "serverSide": true,
            "lengthChange": false,
            // "displayStart": 11,
            "language": {
                "url": 'Langdatatable.aspx'
            },
            select: true,
            "columnDefs": [
               {
                   "targets": [0],
                   "visible": false,
               },
               {
                   "targets": [1],
                   "visible": false,
               },
               {
                   "targets": [2],
                   "visible": false,
               }

            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                //$('td:eq(2)', nRow).addClass("text-red");

            },
            "fnDrawCallback": function (oSettings) {
                closeLoading();



            }
        });

        var ddl_function_id = $('#ddlFunction').val();

        tbAreamanagement3.ajax.url('Datatablelist.asmx/getListAreaManagement3?function_id=' + ddl_function_id + "&lang=" + lang);
        $('#tbAreamanagement3 tbody').on('click', 'tr', function () {

            if ($(this).hasClass('selected')) {
                $(this).removeClass('selected');
            }
            else {
                tbAreamanagement3.$('tr.selected').removeClass('selected');
                $(this).addClass('selected');
            }
        });



    }


    function callArea()
    {

        $.ajax({
            type: "POST",
            data: { company_id: company_select, function_id: function_select, department_id: department_select, division_id: division_select, section_id: section_select, lang: lang },
            url: 'Datatablelist.asmx/getArea',
            dataType: 'json',
            success: function (json) {

                var lbmanage = '<%= Resources.Main.manage %>';
                var lbedit = '<%= Resources.Main.edit %>';
                var lbdelete = '<%= Resources.Main.delete %>';
                         
                var table_html = "";

                $.each(json, function (value, key) {
                    
                    table_html += '<tr>';
                    table_html += '<td style="width:70%;">' + key.name + '</td>';

                        <%
                            ArrayList per5 = Session["permission"] as ArrayList;
                            if (per5.IndexOf("area management delete") > -1)         
                          {                            
       
                        %>
                            table_html += '<td><a href="javascript:DeleteArea(' + key.id + ');">' + lbdelete + '</a></td>';

                        <%                      
                          }
       
                        %>


                      <%
                        ArrayList per6 = Session["permission"] as ArrayList;
                        if (per6.IndexOf("area management edit") > -1)         
                        {                            
       
                        %>
                               table_html += '<td><a href="javascript:EditArea(' + key.id + ');">' + lbedit + '</a></td>';

                        <%                      
                        }
       
                        %>
                   
                    
                    table_html += '</tr>';


                });

                $("#tbArea").html(table_html);

               
            }
        });

    }



    function callArea2() {

        $.ajax({
            type: "POST",
            data: { company_id: company_select, function_id: function_select, department_id: department_select, division_id: division_select, section_id: section_select, lang: lang },
            url: 'Datatablelist.asmx/getArea',
            dataType: 'json',
            success: function (json) {

                var lbmanage = '<%= Resources.Main.manage %>';
                var lbedit = '<%= Resources.Main.edit %>';
                var lbdelete = '<%= Resources.Main.delete %>';

                var table_html = "";

                $.each(json, function (value, key) {

                    table_html += '<tr>';
                    table_html += '<td style="width:70%;">' + key.name + '</td>';

                    <%
                            ArrayList per9 = Session["permission"] as ArrayList;
                            if (per5.IndexOf("area management delete") > -1)         
                          {                            
       
                        %>
                    table_html += '<td><a href="javascript:DeleteArea(' + key.id + ');">' + lbdelete + '</a></td>';

                    <%                      
                          }
       
                        %>


                    <%
                        ArrayList per10 = Session["permission"] as ArrayList;
                        if (per6.IndexOf("area management edit") > -1)         
                        {                            
       
                        %>
                    table_html += '<td><a href="javascript:EditArea(' + key.id + ');">' + lbedit + '</a></td>';

                    <%                      
                        }
       
                        %>


                    table_html += '</tr>';


                });

                $("#tbArea2").html(table_html);


            }
        });

    }



    function callArea3()
    {

        $.ajax({
            type: "POST",
            data: { company_id: company_select, function_id: function_select, department_id: department_select, division_id: division_select, section_id: section_select, lang: lang },
            url: 'Datatablelist.asmx/getArea',
            dataType: 'json',
            success: function (json) {

                var lbmanage = '<%= Resources.Main.manage %>';
                var lbedit = '<%= Resources.Main.edit %>';
                var lbdelete = '<%= Resources.Main.delete %>';

                var table_html = "";

                $.each(json, function (value, key) {

                    table_html += '<tr>';
                    table_html += '<td style="width:70%;">' + key.name + '</td>';

                    <%
                            ArrayList per12 = Session["permission"] as ArrayList;
                            if (per5.IndexOf("area management delete") > -1)         
                          {                            
       
                        %>
                    table_html += '<td><a href="javascript:DeleteArea(' + key.id + ');">' + lbdelete + '</a></td>';

                    <%                      
                          }
       
                        %>


                    <%
                        ArrayList per13 = Session["permission"] as ArrayList;
                        if (per6.IndexOf("area management edit") > -1)         
                        {                            
       
                        %>
                    table_html += '<td><a href="javascript:EditArea(' + key.id + ');">' + lbedit + '</a></td>';

                    <%                      
                        }
       
                        %>


                    table_html += '</tr>';


                });

                $("#tbArea3").html(table_html);


            }
        });

    }




    function clearArea()
    {
        $("#MainContent_thai_name_area").val("");
        $("#MainContent_eng_name_area").val("");

    }



    function showCreateArea()
    {
       
        $.when(checkSelectArea()).done(function (result) {
            
            if(result=="false")
            {
                return;
            }
            $("#btAddArea").show();
            $("#btUpdateArea").hide();
            dialogArea.dialog("open");

        })
      
    }


    function CloseArea()
    {
        dialogArea.dialog("close");


    }

    function AddArea()
    {

        showLoading();
        var name_th = $("#MainContent_thai_name_area").val();
        if (name_th == undefined)
        {
            name_th = "";
        }
        var name_en = $("#MainContent_eng_name_area").val();


        $.ajax({
            type: "POST",
            data: {
                name_th: name_th,
                name_en: name_en,
                company_id: company_select,
                function_id: function_select,
                department_id: department_select,
                division_id: division_select,
                section_id: section_select

            },
            url: 'Actionevent.asmx/createArea',
            dataType: 'json',
            success: function (result) {

                closeLoading();
                dialogArea.dialog("close");
                clearArea();

                if (active_tab == "#tab-1") {
                    callArea();
                } else if (active_tab == "#tab-2") {
                    callArea2();
                }else if(active_tab == "#tab-3") {
                    callArea3();
                }
               

            }
        });



    }


    function EditArea(id)
    {
        id_edit = id;
        $("#btAddArea").hide();
        $("#btUpdateArea").show();
        showLoading();
       
        $.ajax({
            type: "POST",
            data: { id: id },
            url: 'Actionevent.asmx/getAreaByid',
            dataType: 'json',
            success: function (json) {
                dialogArea.dialog("open");
                $.each(json, function (value, key) {
                    $("#MainContent_thai_name_area").val(key.name_th);
                    $("#MainContent_eng_name_area").val(key.name_en);                 

                });
               
                closeLoading();
            }
        });

    }


    function UpdateArea() {

        showLoading();
        var name_th = $("#MainContent_thai_name_area").val();
        if (name_th == undefined) {
            name_th = "";
        }
        var name_en = $("#MainContent_eng_name_area").val();


        $.ajax({
            type: "POST",
            data: {
                id: id_edit,
                name_th: name_th,
                name_en: name_en,
              
            },
            url: 'Actionevent.asmx/updateArea',
            dataType: 'json',
            success: function (result) {

                closeLoading();
                dialogArea.dialog("close");
                clearArea();
                callArea();
                id_edit = "";
            }
        });



    }

    function DeleteArea(id)
    {
        var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
        if (confirm(message_confirm_delete)) {
            showLoading();
            $.ajax({
                type: "POST",
                data: { id: id },
                url: 'Actionevent.asmx/deleteArea',
                dataType: 'json',
                success: function (json) {

                    callArea();
                    closeLoading();
                }
            });

        }
    }


    function CheckNameLength(oSrc, args)
    {
        if (args.Value.length > 50) {

            args.IsValid = false;
        } else {

            args.IsValid = true;
        }


    }


    function clearValidationErrors()
    {
        var i;
        for (i = 0; i < Page_Validators.length; i++)
        {

            Page_Validators[i].style.display = "none";

        }

    }


    function checkSelectArea()
    {
        var result = "true";
        var message = '<%= Resources.Main.alertselectarea %>';

        if (company_select.length==0)
        {
            alert(message);
            result = "false";
            return result;
        }

        if (function_select.length == 0)
        {
            alert(message);
            result = "false";
            return result;
        }

        if (department_select.length == 0)
        {
            alert(message);
            result = "false";
            return result;
        }

        //if (division_select.length == 0)
        //{
        //    alert(message);
        //    result = "false";
        //    return result;
        //}

        //if (section_select.length == 0)
        //{
        //    alert(message);
        //    result = "false";
        //    return result;
        //}
    }
   
    function AddAreaOHS()
    {
        showLoading();
        var data_post = JSON.stringify({
            employee_id: rows_selected, department_id: department_select
        });
        $.ajax({
            type: "POST",
            data: data_post,
            url: 'Actionevent.asmx/createAreaOHS',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                callArea();
                var ddl_function_id = $('#ddlFunction').val();
                dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=").load();

                if (active_tab == "#tab-1") {
                    tbAreamanagement.ajax.url('Datatablelist.asmx/getListAreaManagement?function_id=' + ddl_function_id + "&lang=" + lang).load();

                } else if (active_tab == "#tab-2") {
                    tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang).load();

                } else if (active_tab == "#tab-3") {
                    tbAreamanagement3.ajax.url('Datatablelist.asmx/getListAreaManagement3?function_id=' + ddl_function_id + "&lang=" + lang).load();

                }
                rows_selected = [];
                closeLoading();

            }, error: function (data) {
                console.log(data);
            }
        });

        dialog.dialog("close");

    }

    function AddAreaManager()
    {
        showLoading();
        var data_post = JSON.stringify({employee_id: rows_selected, department_id: department_select, division_id: division_select,
        });
        $.ajax({
            type: "POST",
            data: data_post,
            url: 'Actionevent.asmx/createAreaManager',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                callArea();
                var ddl_function_id = $('#ddlFunction').val();
                dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=").load();


                if (active_tab == "#tab-1") {
                    tbAreamanagement.ajax.url('Datatablelist.asmx/getListAreaManagement?function_id=' + ddl_function_id + "&lang=" + lang).load();

                } else if (active_tab == "#tab-2") {
                    tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang).load();

                } else if (active_tab == "#tab-3") {
                    tbAreamanagement3.ajax.url('Datatablelist.asmx/getListAreaManagement3?function_id=' + ddl_function_id + "&lang=" + lang).load();

                }

                rows_selected = [];
                closeLoading();

            }, error: function (data) {
                console.log(data);
            }
        });

        dialog.dialog("close");

    }


    function AddAreaSupervisor()
    {
        showLoading();
        var data_post = JSON.stringify({
            employee_id: rows_selected, department_id: department_select, division_id: division_select, section_id: section_select
        });
        $.ajax({
            type: "POST",
            data: data_post,
            url: 'Actionevent.asmx/createAreaSupervisor',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                callArea();
                var ddl_function_id = $('#ddlFunction').val();
                dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id + "&type=").load();

                if (active_tab == "#tab-1") {
                    tbAreamanagement.ajax.url('Datatablelist.asmx/getListAreaManagement?function_id=' + ddl_function_id + "&lang=" + lang).load();

                } else if (active_tab == "#tab-2") {
                    tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang).load();

                } else if (active_tab == "#tab-3") {
                    tbAreamanagement3.ajax.url('Datatablelist.asmx/getListAreaManagement3?function_id=' + ddl_function_id + "&lang=" + lang).load();

                }
                rows_selected = [];
                closeLoading();

            }, error: function (data) {
                console.log(data);
            }
        });

        dialog.dialog("close");

    }



    function AddFunctionalManager()
    {
        showLoading();
        var data_post = JSON.stringify({
            employee_id: rows_selected, department_id: department_select, division_id: division_select, section_id: section_select
        });
        $.ajax({
            type: "POST",
            data: data_post,
            url: 'Actionevent.asmx/createFunctionalManager',
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                callArea();
                var ddl_function_id = $('#ddlFunction').val();
                dataTable.ajax.url('Datatablelist.asmx/getListemployeeByfunction?function_id=' + ddl_function_id+ "&type=functional").load();

                if (active_tab == "#tab-1") {
                    tbAreamanagement.ajax.url('Datatablelist.asmx/getListAreaManagement?function_id=' + ddl_function_id + "&lang=" + lang).load();

                } else if (active_tab == "#tab-2") {
                    tbAreamanagement2.ajax.url('Datatablelist.asmx/getListAreaManagement2?function_id=' + ddl_function_id + "&lang=" + lang).load();

                } else if (active_tab == "#tab-3") {
                    tbAreamanagement3.ajax.url('Datatablelist.asmx/getListAreaManagement3?function_id=' + ddl_function_id + "&lang=" + lang).load();

                }
                rows_selected = [];
                closeLoading();

            }, error: function (data) {
                console.log(data);
            }
        });

        dialog.dialog("close");

    }


</script>

    <div id="create_area" title="<%= Resources.Main.areaname %>">
        <div class="form-horizontal">  
             <%
              if (Session["country"].ToString() =="thailand")
              {                  
            %>       
                <div class="form-group">
                    <label class="col-lg-3 control-label"><%= Resources.Main.lbNameThArea %></label>           
                    <div class="col-lg-9">
                        <input id="thai_name_area" class="form-control" type="text" runat="server">
                        <asp:CustomValidator id="rqNameThAreaLength" runat="server" ClientValidationFunction="CheckNameLength" Display="Dynamic" ControlToValidate="thai_name_area"  ErrorMessage="<%$ Resources:Main, rqNameThArea %>" CssClass="text-danger"></asp:CustomValidator>

                    </div>
                </div>
             <%
             }           
            %>
            <div class="form-group">
                <label class="col-lg-3 control-label"><%= Resources.Main.lbNameEnArea %></label>
                <div class="col-lg-9">
                    <input id="eng_name_area" class="form-control" type="text" runat="server">
                    <asp:CustomValidator id="rqNameEnAreaLength" runat="server" ClientValidationFunction="CheckNameLength" Display="Dynamic" ControlToValidate="eng_name_area" ErrorMessage="<%$ Resources:Main, rqNameEnArea %>" CssClass="text-danger"></asp:CustomValidator>

                </div>
           </div>
       
            <div class="form-group">
                <div class="col-lg-offset-3 col-lg-9">
                    <button id="btAddArea" class="btn btn-sm btn-primary" onclick="AddArea()"><%= Resources.Main.btsave %></button>
                    <button id="btUpdateArea"class="btn btn-sm btn-primary" onclick="UpdateArea()"><%= Resources.Main.btsave %></button>
                    <button class="btn btn-sm btn-default" onclick="CloseArea()"><%= Resources.Main.btClose %></button>
                </div>
            </div>
      </div>


    </div>


     <div id="employee-form">

             <div class="row">
                 <div class="col-sm-12">
                      <table id="tbEmployee" class="table table-bordered table-hover" cellspacing="0" width="100%">
                         <thead>
                            <tr>
                                
                                <th><input name="select_all" value="1" type="checkbox"></th>
                                <th></th>                           
                                <th></th>
                                
                              
                    
                            </tr>
                        </thead>
         
                    </table>



                 </div>

             </div>
             <div class="row">
                <div class="col-sm-4">
                    
                </div>
               
                 <div class="col-sm-4">
                    
                </div>

                  <div class="col-sm-4">
                    <div class="form-group pull-right">
                        <asp:Button ID="btClose" runat="server" Text="<%$ Resources:Main, btClose %>" OnClientClick="CloseDialog();" CssClass="btn btn-default"/>
                        <asp:Button ID="btAdd" runat="server" Text="<%$ Resources:Main, btsave %>" OnClientClick="AddOHS();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btAddDelegate" runat="server" Text="<%$ Resources:Main, btsave %>" OnClientClick="AddDelegate();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btAddAreaManager" runat="server" Text="<%$ Resources:Main, btsave %>" OnClientClick="AddAreaManager();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btAddAreaOHS" runat="server" Text="<%$ Resources:Main, btsave %>" OnClientClick="AddAreaOHS();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btAddAreaSupervisor" runat="server" Text="<%$ Resources:Main, btsave %>" OnClientClick="AddAreaSupervisor();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btAddFunctionalManager" runat="server" Text="<%$ Resources:Main, btsave %>" OnClientClick="AddFunctionalManager();" CssClass="btn btn-primary"/>
                       
                    </div>
                </div>
             </div>

    </div>
       <div class="row">
        <div class="col-lg-12">
             <table class="form-filter">
         
                 <tr>
                     <td><asp:Label ID="lbCompany" runat="server" Text="<%$ Resources:Contractor, lbCompany %>"></asp:Label></td>
                     <td>
                          <select id="ddlCompany" class="form-control" onchange="changCompany();">
                       
                         </select>

                     </td>
                     <td>
                         <asp:Label ID="lbfunction" runat="server" Text="<%$ Resources:Contractor, lbfunction %>"></asp:Label></td>
                     <td> 
                         <select id="ddlFunction" class="form-control">
                       
                         </select>
                     </td>

                     <td> 
                         <asp:Button ID="btOK" runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterArea();" CssClass="btn btn-primary"/>
                     </td>

                 
                 </tr>

             </table>
       
        </div>
     </div>

    <div class="row">
        <div class="col-lg-12">
         <table style="width:100%;">
             <tr>
                 <td>
                     <h3><asp:Label ID="lbHederOHS" runat="server" Text="<%$ Resources:Main, lbHederOHS %>"></asp:Label></h3>
                 </td>
                 <td class="pull-right">
                         <%
                            ArrayList per = Session["permission"] as ArrayList;
                            if (per.IndexOf("area management oh&s admin add") > -1)         
                           {                            
       
                          %>
                              <button type="button" id="btAddOHS" class="btn btn-primary" runat="server" onclick="showCreateOHS();"><i class="fa fa-plus"></i></button>

                          <%                      
                            }
       
                          %>
                 </td>
                
             </tr>

         </table>
       </div>
       
     </div>

     <ul id="listOHS" class="list-group">
     
    </ul>

       <div class="row">
        <div class="col-lg-12">
         <table style="width:100%;">
             <tr>
                 <td>
                     <h3><asp:Label ID="lbHederDelegateOHS" runat="server" Text="<%$ Resources:Main, lbHederDelegateOHS %>"></asp:Label></h3>
                 </td>
                 <td class="pull-right">
                     
                       <%
                            ArrayList per2 = Session["permission"] as ArrayList;
                            if (per2.IndexOf("area management delegate oh&s admin add") > -1)         
                           {                            
       
                          %>
                             <button type="button" id="btAddDelegateOHS" class="btn btn-primary" runat="server" onclick="showCreateDelegate();"><i class="fa fa-plus"></i></button>
                    
                        <%                      
                        }
       
                        %>
                     
                     
                 </td>
                
             </tr>

         </table>
       </div>
       
     </div>

     <ul id="listDelegate" class="list-group">
      
    </ul>




    <div class="tabs-container">
                        <ul class="nav nav-tabs">
                            <li class="active"><a data-toggle="tab" href="#tab-1" aria-expanded="true"> Active org</a></li>
                            <li class=""><a data-toggle="tab" href="#tab-2" aria-expanded="false">Delimited org</a></li>
                            <li class=""><a data-toggle="tab" href="#tab-3" aria-expanded="false">Vacant org</a></li>
                        </ul>
                        <div class="tab-content">
                            <div id="tab-1" class="tab-pane active">
                                <div class="panel-body">
                                   
								   
                                        <div class="row">
                                            <div class="col-lg-7">
                                                 <table id="tbAreamanagement" class="table table-bordered table-hover" cellspacing="0" width="100%">
                                                             <thead>
                                                                <tr>
                                                                    <th><asp:Label ID="lbDepartmentID" runat="server" Text=""></asp:Label></th>
                                                                    <th><asp:Label ID="lbDivisionID" runat="server" Text=""></asp:Label></th>
                                                                    <th><asp:Label ID="lbSectionID" runat="server" Text=""></asp:Label></th>

                                                                    <%
                                                                      if (Session["country"].ToString() =="thailand")
                                                                      {                  
                                                                    %>  
                                                                       <th><asp:Label ID="lbDepartment_functional" runat="server" Text="<%$ Resources:Main, lbdepartment_functional_area %>"></asp:Label></th>
                                                                    
                                                                     <%
                                                                     }
                                                                    %>    
                                                                    <th><asp:Label ID="lbDepartment" runat="server" Text="<%$ Resources:Main, lbdepartment_area %>"></asp:Label></th>
                                                                    <th><asp:Label ID="lbDivision" runat="server" Text="<%$ Resources:Main, lbdivision_area %>"></asp:Label></th>
                                                                    <th><asp:Label ID="lbSection" runat="server" Text="<%$ Resources:Main, lbsection_area %>"></asp:Label></th>
                                                                </tr>
                                                            </thead>
         
                                                        </table>




                                            </div>
                                            <div class="col-lg-5">
                                                <div class="ibox collapsed">
                                                        <div class="ibox-title">
                                                            <h5><%= Resources.Main.manage %></h5>
                                                            <div class="ibox-tools">

                                                              <%
                                                                    ArrayList per3 = Session["permission"] as ArrayList;
                                                                    if (per3.IndexOf("area management add") > -1)         
                                                                   {                            
       
                                                               %>
                               
                                                                 <button type="button" id="btCreatOHS" class="btn_area btn-primary"   onclick="showCreateAreaOHS();"><i class="fa fa-plus"></i> <%= Resources.Main.areaohs %></button>                                                   
                                                                 <button type="button" id="btCreatAreaManager" class="btn_area btn-primary"  onclick="showCreateAreaManager();"><i class="fa fa-plus"></i> <%= Resources.Main.areamanager %></button>                    
                                                                 <button type="button" id="btCreateSupervisor" class="btn_area btn-primary" style="margin-bottom:2px;"  onclick="showCreateAreaSupervisor();"><i class="fa fa-plus"></i> <%= Resources.Main.areasupervisor %></button>

                                                                  <%
                                                                    if (Session["country"].ToString() =="thailand")
                                                                    {                  
                                                                    %> 
                                                                        <button type="button" id="btCreateFunctionalManager" class="btn_area btn-primary"  onclick="showCreateFunctionalManager();"><i class="fa fa-plus"></i> <%= Resources.Main.functionalmanager %></button>

                                                                 <%
                                                                  }
                                                                  %> 
                           
                                                                <%                      
                                                                    }
       
                                                                %>

 
                                                            </div>
                                                        </div>
                    
                                                    </div>
                                                 <div class="ibox float-e-margins">
                                                        <div class="ibox-title">
                                                            <h5><asp:Label ID="lbArea" runat="server" Text="<%$ Resources:Main, lbArea %>"></asp:Label></h5>
                                                            <div class="ibox-tools">

                            
                                                              <%
                                                                    ArrayList per4 = Session["permission"] as ArrayList;
                                                                    if (per4.IndexOf("area management add") > -1)         
                                                                   {                            
       
                                                               %>
                               
                                                                  <button type="button" id="Button1" class="btn_area btn-primary" runat="server" onclick="showCreateArea();"><i class="fa fa-plus"></i></button>

                                                                <%                      
                                                                    }
       
                                                                %>
                                                                                       </div>
                                                        </div>
                                                        <div  class="ibox-content" style="display: block;">
                                                           <table id="tbArea" style="width:100%;">

                                                           </table>
                                                        </div>
                                                    </div>




                                            </div>
                                        </div>



								   
                                </div>
                            </div>



                            <div id="tab-2" class="tab-pane">
                                <div class="panel-body">
                                          
                                    
                                            <div class="row">
                                            <div class="col-lg-7">
                                                 <table id="tbAreamanagement2" class="table table-bordered table-hover" cellspacing="0" width="100%">
                                                             <thead>
                                                                <tr>
                                                                    <th><asp:Label ID="Label1" runat="server" Text=""></asp:Label></th>
                                                                    <th><asp:Label ID="Label2" runat="server" Text=""></asp:Label></th>
                                                                    <th><asp:Label ID="Label3" runat="server" Text=""></asp:Label></th>
                                                                     <%
                                                                      if (Session["country"].ToString() =="thailand")
                                                                      {                  
                                                                    %>  
                                                                       <th><asp:Label ID="Label15" runat="server" Text="<%$ Resources:Main, lbdepartment_functional_area %>"></asp:Label></th>
                                                                    
                                                                     <%
                                                                     }
                                                                    %>    
                                                                    <th><asp:Label ID="Label4" runat="server" Text="<%$ Resources:Main, lbdepartment_area %>"></asp:Label></th>
                                                                    <th><asp:Label ID="Label5" runat="server" Text="<%$ Resources:Main, lbdivision_area %>"></asp:Label></th>
                                                                    <th><asp:Label ID="Label6" runat="server" Text="<%$ Resources:Main, lbsection_area %>"></asp:Label></th>
                                                                </tr>
                                                            </thead>
         
                                                        </table>




                                            </div>
                                            <div class="col-lg-5">
                                                <div class="ibox collapsed">
                                                        <div class="ibox-title">
                                                            <h5><%= Resources.Main.manage %></h5>
                                                            <div class="ibox-tools">

                                                              <%
                                                                    ArrayList per7 = Session["permission"] as ArrayList;
                                                                    if (per5.IndexOf("area management add") > -1)         
                                                                   {                            
       
                                                               %>
                               
                                                                 <button type="button" id="btRemoveOHS" class="btn_area btn-primary"   onclick="RemoveAreaOHS();"><i class="fa fa-trash-o"></i> <%= Resources.Main.areaohs %></button>                                                   
                                                                 <button type="button" id="btRemoveAreaManager" class="btn_area btn-primary"  onclick="RemoveAreaManager();"><i class="fa fa-trash-o"></i> <%= Resources.Main.areamanager %></button>                    
                                                                 <button type="button" id="btRemoveSupervisor" class="btn_area btn-primary" style="margin-bottom:2px;" onclick="RemoveAreaSupervisor();"><i class="fa fa-trash-o"></i> <%= Resources.Main.areasupervisor %></button>
                                                                 <button type="button" id="btRemoveAll" class="btn_area btn-primary" style="margin-bottom:2px;" onclick="RemoveAreaAll();"><i class="fa fa-trash-o"></i> <%= Resources.Main.areaall %></button>
                                                                 
                                                                  <%
                                                                    if (Session["country"].ToString() =="thailand")
                                                                    {                  
                                                                    %> 
                                                                        <button type="button" id="btRemoveFunctionalManager" class="btn_area btn-primary"  onclick="RemoveFunctionalManager();"><i class="fa fa-trash-o"></i> <%= Resources.Main.functionalmanager %></button>

                                                                 <%
                                                                  }
                                                                  %> 
                           

                                                                <%                      
                                                                    }
       
                                                                %>

 
                                                            </div>
                                                        </div>
                    
                                                    </div>
                                                 <div class="ibox float-e-margins">
                                                        <div class="ibox-title">
                                                            <h5><asp:Label ID="Label7" runat="server" Text="<%$ Resources:Main, lbArea %>"></asp:Label></h5>
                                                          
                                                        </div>
                                                        <div  class="ibox-content" style="display: block;">
                                                           <table id="tbArea2" style="width:100%;">

                                                           </table>
                                                        </div>
                                                    </div>




                                            </div>
                                        </div>
                                    
                                    
                                         
											   
											   
											   
                                </div>
                            </div>



                             <div id="tab-3" class="tab-pane">
                                <div class="panel-body">
                                               
										 <div class="row">
                                            <div class="col-lg-7">
                                                 <table id="tbAreamanagement3" class="table table-bordered table-hover" cellspacing="0" width="100%">
                                                             <thead>
                                                                <tr>
                                                                    <th><asp:Label ID="Label8" runat="server" Text=""></asp:Label></th>
                                                                    <th><asp:Label ID="Label9" runat="server" Text=""></asp:Label></th>
                                                                    <th><asp:Label ID="Label10" runat="server" Text=""></asp:Label></th>
                                                                     <%
                                                                      if (Session["country"].ToString() =="thailand")
                                                                      {                  
                                                                    %>  
                                                                       <th><asp:Label ID="Label16" runat="server" Text="<%$ Resources:Main, lbdepartment_functional_area %>"></asp:Label></th>
                                                                    
                                                                     <%
                                                                     }
                                                                    %>    
                                                                    <th><asp:Label ID="Label11" runat="server" Text="<%$ Resources:Main, lbdepartment_area %>"></asp:Label></th>
                                                                    <th><asp:Label ID="Label12" runat="server" Text="<%$ Resources:Main, lbdivision_area %>"></asp:Label></th>
                                                                    <th><asp:Label ID="Label13" runat="server" Text="<%$ Resources:Main, lbsection_area %>"></asp:Label></th>
                                                                </tr>
                                                            </thead>
         
                                                        </table>




                                            </div>
                                            <div class="col-lg-5">
                                                <div class="ibox collapsed">
                                                        <div class="ibox-title">
                                                            <h5><%= Resources.Main.manage %></h5>
                                                            <div class="ibox-tools">

                                                              <%
                                                                    ArrayList per16 = Session["permission"] as ArrayList;
                                                                    if (per3.IndexOf("area management add") > -1)         
                                                                   {                            
       
                                                               %>
                               
                                                                 <button type="button" id="btCreatOHS2" class="btn_area btn-primary"   onclick="showCreateAreaOHS();"><i class="fa fa-plus"></i> <%= Resources.Main.areaohs %></button>                                                   
                                                                 <button type="button" id="btCreatAreaManager2" class="btn_area btn-primary"  onclick="showCreateAreaManager();"><i class="fa fa-plus"></i> <%= Resources.Main.areamanager %></button>                    
                                                                 <button type="button" id="btCreateSupervisor2" class="btn_area btn-primary" style="margin-bottom:2px;"  onclick="showCreateAreaSupervisor();"><i class="fa fa-plus"></i> <%= Resources.Main.areasupervisor %></button>
                                                                  
                                                                  <%
                                                                    if (Session["country"].ToString() =="thailand")
                                                                    {                  
                                                                    %> 
                                                                        <button type="button" id="btCreateFunctionalManager2" class="btn_area btn-primary"  onclick="showCreateFunctionalManager();"><i class="fa fa-plus"></i> <%= Resources.Main.functionalmanager %></button>

                                                                 <%
                                                                  }
                                                                  %> 
                           
                                                                <%                      
                                                                    }
       
                                                                %>

 
                                                            </div>
                                                        </div>
                    
                                                    </div>
                                                 <div class="ibox float-e-margins">
                                                        <div class="ibox-title">
                                                            <h5><asp:Label ID="Label14" runat="server" Text="<%$ Resources:Main, lbArea %>"></asp:Label></h5>
                                                            <div class="ibox-tools">

                            
                                                              <%
                                                                    ArrayList per17 = Session["permission"] as ArrayList;
                                                                    if (per4.IndexOf("area management add") > -1)         
                                                                   {                            
       
                                                               %>
                               
                                                                  <button type="button" id="Button2" class="btn_area btn-primary" runat="server" onclick="showCreateArea();"><i class="fa fa-plus"></i></button>

                                                                <%                      
                                                                    }
       
                                                                %>
                                                                                       </div>
                                                        </div>
                                                        <div  class="ibox-content" style="display: block;">
                                                           <table id="tbArea3" style="width:100%;">

                                                           </table>
                                                        </div>
                                                    </div>




                                            </div>
                                        </div>

											   
											   
                                </div>
                            </div>
                        </div>


                    </div>
















</asp:Content>
