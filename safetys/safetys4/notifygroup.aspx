<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="notifygroup.aspx.cs" Inherits="safetys4.notifygroup" %>
<%@ MasterType VirtualPath="~/Saftetymain.Master" %>
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

    .wrapper-content {
        margin-left: 330px;
    }

     </style>
    <script>
        var rows_selected = [];
        var dataTable; //reference to your dataTable   
        var group_type = "";
   
    $(document).ready(function ()
    {
        var url = window.location.href;
        var urlarr = url.split("=");
        group_type = urlarr[1];


        

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

       <%
        if (Request.QueryString["smenu"] == "GroupCommunicationVP")
        {
            Response.Write("callGCVP();");
            
        }
        else if (Request.QueryString["smenu"] == "LegalDepartment")
        {
            Response.Write("callLegalDepartment();");
        }
        else if (Request.QueryString["smenu"] == "GroupOHS")
        {
            Response.Write("callGroupOHS();");
        }
        else if (Request.QueryString["smenu"] == "GroupOHSHazard")
        {
            Response.Write("callGroupOHSHazard();");
        }
        else if (Request.QueryString["smenu"] == "GroupOHSHealth")
        {
            Response.Write("callGroupOHSHealth();");
        }
        else if (Request.QueryString["smenu"] == "GroupEXCO")
        {
            Response.Write("callGroupEXCO();");
        }
        else if (Request.QueryString["smenu"] == "GroupCEO")
        {
            Response.Write("callGroupCEO();");
        }
        %>
        //callGCVP();
        //callDelegateSuperadmin();
        setDataEmployeeList();
      
           
    });



    function callGCVP()
    {
       // console.log("ddd");
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Datatablelist.asmx/getGCVP',
            success: function (result)
            {
                
                $("#listSuper").html(result);

            },
        });



    }

    function callLegalDepartment() {
        // console.log("ddd");
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Datatablelist.asmx/getLegalDepartment',
            success: function (result) {

                $("#listSuper").html(result);

            },
        });



    }

    function callGroupOHS() {
        // console.log("ddd");
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Datatablelist.asmx/getGroupOHS',
            success: function (result) {

                $("#listSuper").html(result);

            },
        });



    }

    function callGroupOHSHazard() {
        // console.log("ddd");
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Datatablelist.asmx/getGroupOHSHazard',
            success: function (result) {

                $("#listSuper").html(result);

            },
        });



    }



    function callGroupOHSHealth()
    {
        // console.log("ddd");
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Datatablelist.asmx/getGroupOHSHealth',
            success: function (result) {

                $("#listSuper").html(result);

            },
        });



    }



    function callGroupEXCO()
    {
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Datatablelist.asmx/getGroupEXCO',
            success: function (result) {

                $("#listSuper").html(result);

            },
        });



    }


    function callGroupCEO()
    {
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Datatablelist.asmx/getGroupCEO',
            success: function (result) {

                $("#listSuper").html(result);

            },
        });



    }



    function callGroupEXCO()
    {
        // console.log("ddd");
        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Datatablelist.asmx/getGroupEXCO',
            success: function (result) {

                $("#listSuper").html(result);

            },
        });



    }



    function AddEmpToNotifyGroup(uri,call)
    {
        //$.when(getSelectEmployee()).done(function () {
            showLoading();
            var data_post = JSON.stringify({ employee_id: rows_selected });
                $.ajax({
                    type: "POST",
                    data: data_post,
                    url: uri,
                    contentType: "application/json; charset=utf-8",
                    success: function (result) {
                        //callSuperadmin();
                        call();
                        dataTable.ajax.url('Datatablelist.asmx/getListnotifyGroupEmployee?group_type=' + group_type).load();
                        rows_selected = [];
                        closeLoading();

                    },error: function(data){
                        console.log(data);
                    }
                });

                dialog.dialog("close");
            
           //  });

          

        }

        function AddDelegate()
        {
            // $.when(getSelectEmployee()).done(function () {
                showLoading();
                var data_post = JSON.stringify({ employee_id: rows_selected });
               
                $.ajax({
                    type: "POST",
                    data: data_post,
                    url: 'Actionevent.asmx/createDelegateSuperadmin',
                    contentType: "application/json; charset=utf-8",
                    success: function (result)
                    {
                        callDelegateSuperadmin();
                        dataTable.ajax.url('Datatablelist.asmx/getListnotifyGroupEmployee?group_type=' + group_type).load();
                        rows_selected = [];
                        closeLoading();

                    }
                });

                dialog.dialog("close");
           // });


       }


        function getSelectEmployee()
        {
            //$.each(rows_selected, function (index, rowId) {
            //    alert(rowId);
            //});

            //console.log(rows_selected);
          
        }


        function showCreateSuperAdmin()
        {
            dataTable.ajax.url('Datatablelist.asmx/getListnotifyGroupEmployee?group_type=' + group_type).load();
            rows_selected = [];
            $("#MainContent_btAdd").show();
            dialog.dialog("open");

        }

        function DeleteGCVP(id) {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: { id: id },
                    url: 'Actionevent.asmx/deleteGroupCommunicationVP',
                    dataType: 'json',
                    success: function (json) {

                        callGCVP();
                        closeLoading();
                    }
                });

            }


        }

        function DeleteGCVP(id) {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
             if (confirm(message_confirm_delete)) {
                 showLoading();
                 $.ajax({
                     type: "POST",
                     data: { id: id },
                     url: 'Actionevent.asmx/deleteGroupCommunicationVP',
                     dataType: 'json',
                     success: function (json) {

                         callGCVP();
                         closeLoading();
                     }
                 });

             }


        }

        function DeleteLegalDepartment(id) {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
             if (confirm(message_confirm_delete)) {
                 showLoading();
                 $.ajax({
                     type: "POST",
                     data: { id: id },
                     url: 'Actionevent.asmx/deleteLegalDepartment',
                     dataType: 'json',
                     success: function (json) {

                         callLegalDepartment();
                         closeLoading();
                     }
                 });

             }


         }


        function DeleteGroupOHS(id)
        {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete))
            {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: { id: id },
                    url: 'Actionevent.asmx/deleteGroupOHS',
                    dataType: 'json',
                    success: function (json) {

                        callGroupOHS();
                        closeLoading();
                    }
                });

            } 
           

        }

        function DeleteGroupOHSHazard(id) {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: { id: id },
                    url: 'Actionevent.asmx/deleteGroupOHSHazard',
                    dataType: 'json',
                    success: function (json) {

                        callGroupOHSHazard();
                        closeLoading();
                    }
                });

            }


        }


        function DeleteGroupOHSHealth(id)
        {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: { id: id },
                    url: 'Actionevent.asmx/deleteGroupOHSHealth',
                    dataType: 'json',
                    success: function (json) {

                        callGroupOHSHealth();
                        closeLoading();
                    }
                });

            }

        }
        


        function DeleteGroupEXCO(id)
        {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: { id: id },
                    url: 'Actionevent.asmx/deleteGroupEXCO',
                    dataType: 'json',
                    success: function (json) {

                        callGroupEXCO();
                        closeLoading();
                    }
                });

            }


        }


        function DeleteGroupCEO(id)
        {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: { id: id },
                    url: 'Actionevent.asmx/deleteGroupCEO',
                    dataType: 'json',
                    success: function (json) {

                        callGroupCEO();
                        closeLoading();
                    }
                });

            }


        }

        function DeleteDelegateSuperAdmin(id)
        {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: { id: id },
                    url: 'Actionevent.asmx/deleteDelegateSuperadmin',
                    dataType: 'json',
                    success: function (json) {

                        callDelegateSuperadmin();
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

                    // If row ID is in the list of selected row IDs
                    if ($.inArray(rowId, rows_selected) !== -1) {
                        $(row).find('input[type="checkbox"]').prop('checked', true);
                        $(row).addClass('selected');
                    }
                }

            });


            dataTable.ajax.url('Datatablelist.asmx/getListnotifyGroupEmployee?group_type=' + group_type).load();

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
            var $table             = table.table().node();
            var $chkbox_all        = $('tbody input[type="checkbox"]', $table);
            var $chkbox_checked    = $('tbody input[type="checkbox"]:checked', $table);
            var chkbox_select_all  = $('thead input[name="select_all"]', $table).get(0);

            // If none of the checkboxes are checked
            if($chkbox_checked.length === 0){
                chkbox_select_all.checked = false;
                if('indeterminate' in chkbox_select_all){
                    chkbox_select_all.indeterminate = false;
                }

                // If all of the checkboxes are checked
            } else if ($chkbox_checked.length === $chkbox_all.length){
                chkbox_select_all.checked = true;
                if('indeterminate' in chkbox_select_all){
                    chkbox_select_all.indeterminate = false;
                }

                // If some of the checkboxes are checked
            } else {
                chkbox_select_all.checked = true;
                if('indeterminate' in chkbox_select_all){
                    chkbox_select_all.indeterminate = true;
                }
            }
        }




</script>

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
                        <asp:Button ID="btAdd" runat="server" Text="<%$ Resources:Main, btsave %>" CssClass="btn btn-primary"/>
                       
                    </div>
                </div>
             </div>

    </div>

    <div class="row">
        <div class="col-lg-8">

    
            <div class="row">
                <div class="col-lg-12">
                 <table style="width:100%;">
                     <tr>
                         <td>
                             <h3><asp:Label ID="lbHeaderNotifyGroup" runat="server" Text=""></asp:Label></h3>
                         </td>
                         <td class="pull-right">
                             <button type="button" id="btAddsuperadmin" class="btn btn-primary" runat="server" onclick="showCreateSuperAdmin();"><i class="fa fa-plus"></i></button>
                         </td>
                
                     </tr>

                 </table>
               </div>
       
             </div>

     
            <ul id="listSuper" class="list-group">
     
            </ul>

        </div>
       
    </div>

       












</asp:Content>
