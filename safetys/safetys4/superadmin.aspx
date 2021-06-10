<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="superadmin.aspx.cs" Inherits="safetys4.superadmin" %>
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

     </style>
    <script>
        var rows_selected = [];
        var dataTable; //reference to your dataTable   
   
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

        $('#ddcountry').val('<%=Session["country"]%>');
        callSuperadmin();
        callDelegateSuperadmin();
        setDataEmployeeList();
      
           
    });



    function callSuperadmin()
    {
        var ddl_country = $('#ddcountry').val();
        $.ajax({
            type: "POST",
            data: { lang: lang, country: ddl_country },
            url: 'Datatablelist.asmx/getSuperadmin',
            success: function (result)
            {
                
                $("#listSuper").html(result);

            },
        });



    }

    function callDelegateSuperadmin()
    {
        var ddl_country = $('#ddcountry').val();
        $.ajax({
            type: "POST",
            data: { lang: lang, country: ddl_country },
            url: 'Datatablelist.asmx/getDelegateSuperadmin',
            success: function (result) {
               
                $("#listDelegate").html(result);

            }
        });



    }



    function AddSuperadmin()
    {
        var ddl_country = $('#ddcountry').val();
        //$.when(getSelectEmployee()).done(function () {
            showLoading();
            var data_post = JSON.stringify({ employee_id: rows_selected, country: ddl_country });
                $.ajax({
                    type: "POST",
                    data: data_post,
                    url: 'Actionevent.asmx/createSuperadmin',
                    contentType: "application/json; charset=utf-8",
                    success: function (result) {
                        callSuperadmin();
                        dataTable.ajax.url('Datatablelist.asmx/getListemployee?country=' + ddl_country).load();
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
            var ddl_country = $('#ddcountry').val();
            // $.when(getSelectEmployee()).done(function () {
                showLoading();
                var data_post = JSON.stringify({ employee_id: rows_selected, country: ddl_country });
               
                $.ajax({
                    type: "POST",
                    data: data_post,
                    url: 'Actionevent.asmx/createDelegateSuperadmin',
                    contentType: "application/json; charset=utf-8",
                    success: function (result)
                    {
                        callDelegateSuperadmin();
                        dataTable.ajax.url('Datatablelist.asmx/getListemployee?country=' + ddl_country).load();
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
            var ddl_country = $('#ddcountry').val();
            dataTable.ajax.url('Datatablelist.asmx/getListemployee?country=' + ddl_country).load();
            rows_selected = [];
            $("#MainContent_btAddDelegate").hide();
            $("#MainContent_btAdd").show();
            dialog.dialog("open");

        }

        function showCreateDelegate()
        {
            var ddl_country = $('#ddcountry').val();
            dataTable.ajax.url('Datatablelist.asmx/getListemployee?country=' + ddl_country).load();
            rows_selected = [];
            $("#MainContent_btAddDelegate").show();
            $("#MainContent_btAdd").hide();
            dialog.dialog("open");

        }


        function DeleteSuperAdmin(id)
        {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete))
            {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: { id: id },
                    url: 'Actionevent.asmx/deleteSuperadmin',
                    dataType: 'json',
                    success: function (json) {

                        callSuperadmin();
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

            var ddl_country = $('#ddcountry').val();
            dataTable.ajax.url('Datatablelist.asmx/getListemployee?country=' + ddl_country);

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





        function changCountry()
        {
            var ddl_country = $('#ddcountry').val();

            callSuperadmin();
            callDelegateSuperadmin();
            
            dataTable.ajax.url('Datatablelist.asmx/getListemployee?country=' + ddl_country).load();

        }



</script>

   <div id="employee-form">

             <div class="row">
                 <div class="col-sm-12">
                      <table id="tbEmployee" class="table table-bordered table-hover">
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
                        <asp:Button ID="btAdd" runat="server" Text="<%$ Resources:Main, btsave %>" OnClientClick="AddSuperadmin();" CssClass="btn btn-primary"/>
                        <asp:Button ID="btAddDelegate" runat="server" Text="<%$ Resources:Main, btsave %>" OnClientClick="AddDelegate();" CssClass="btn btn-primary"/>
                       
                    </div>
                </div>
             </div>

    </div>

     <div class="row">
        <div class="col-lg-12">
         <table>
             <tr>
                 <td>
                     
                     <h3><asp:Label ID="Label1" runat="server" Text="<%$ Resources:Main, lbcountry %>"></asp:Label></h3>
                                       
                </td>
                 <td>
                      <select id="ddcountry" name="ddcountry" class="form-control"  style="width:180px;" onchange="changCountry();">
                        <option value="thailand">Thailand</option>
                        <option value="srilanka">Sri Lanka</option>
                  
                    </select>

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
                     <h3><asp:Label ID="lbHederSuperadminr" runat="server" Text="<%$ Resources:Main, lbHederSuperadmin %>"></asp:Label></h3>
                 </td>
                 <td class="pull-right">
                     <%
                            ArrayList per = Session["permission"] as ArrayList;
                            if (per.IndexOf("super admin add") > -1)         
                           {                            
       
                          %>
                              <button type="button" id="btAddsuperadmin" class="btn btn-primary" runat="server" onclick="showCreateSuperAdmin();"><i class="fa fa-plus"></i></button>


                          <%                      
                            }
       
                          %>
                  </td>
                
             </tr>

         </table>
       </div>
       
     </div>

     <ul id="listSuper" class="list-group">
     
    </ul>

       <div class="row">
        <div class="col-lg-12">
         <table style="width:100%;">
             <tr>
                 <td>
                     <h3><asp:Label ID="lbHederDelegateSuperadmin" runat="server" Text="<%$ Resources:Main, lbHederDelegateSuperadmin %>"></asp:Label></h3>
                 </td>
                 <td class="pull-right">
                      <%
                            ArrayList per2 = Session["permission"] as ArrayList;
                            if (per2.IndexOf("delegate super admin add") > -1)         
                           {                            
       
                          %>
                               <button type="button" id="Button1" class="btn btn-primary" runat="server" onclick="showCreateDelegate();"><i class="fa fa-plus"></i></button>

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












</asp:Content>
