<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="setting.aspx.cs" Inherits="safetys4.setting" %>
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
        var dialogArea;
        var setting_page_type = "TypeOfEmployee";

   
    $(document).ready(function ()
    {
        dialogArea = $("#create_area").dialog({
            autoOpen: false,
            height: 230,
            width: 430,
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

        /*dialog = $("#employee-form").dialog({
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
        });*/


       <%
        if (Request.QueryString["smenu"] != "" && Request.QueryString["smenu"] != null)
        {

            Response.Write("setting_page_type = '" + Request.QueryString["smenu"] + "';");
            Response.Write("callSettingList();");
            
        }
        /*else if (Request.QueryString["smenu"] == "LegalDepartment")
        {
            Response.Write("callLegalDepartment();");
        }
        else if (Request.QueryString["smenu"] == "GroupOHS")
        {
            Response.Write("callGroupOHS();");
        }*/
        %>
        //callGCVP();
        //callDelegateSuperadmin();
        //setDataEmployeeList();
      
           
    });

    function callSettingList() {
        // console.log("ddd");
        $.ajax({
            type: "POST",
            data: { lang: lang, setting_page_type: setting_page_type },
            url: 'Datatablelist.asmx/getSettingList',
            success: function (result) {

                $("#listSuper").html(result);

            },
        });



    }

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
                        dataTable.ajax.url('Datatablelist.asmx/getListemployee').load();
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
                        dataTable.ajax.url('Datatablelist.asmx/getListemployee').load();
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


        function showCreateSuperAdmin() {
            dataTable.ajax.url('Datatablelist.asmx/getListemployee').load();
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


            dataTable.ajax.url('Datatablelist.asmx/getListemployee');

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

        function showCreateData()
        {  
            $("#btAddArea").show();
            $("#btUpdateArea").hide();
            dialogArea.dialog("open");

        }

        function clearValidationErrors()
        {
            var i;
            for (i = 0; i < Page_Validators.length; i++) {

                Page_Validators[i].style.display = "none";

            }

        }

        function clearArea()
        {
            $("#MainContent_thai_name_area").val("");
            $("#MainContent_eng_name_area").val("");

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
            if (name_en == undefined)
            {
                name_en = "";
            }

            var checkother = "";
            if ($('#MainContent_check_other').is(":checked"))
            {
                checkother = "other"

            }

            $.ajax({
                type: "POST",
                data: {
                    name_th: name_th,
                    name_en: name_en,
                    code : checkother,
                    setting_page_type: setting_page_type

                },
                url: 'Actionevent.asmx/createSetting',
                dataType: 'json',
                success: function (result) {

                    closeLoading();
                    dialogArea.dialog("close");
                    clearArea();
                    callSettingList();

                }
            });



        }

        function CloseArea()
        {
            dialogArea.dialog("close");
        }

        function EditArea(id)
        {
            id_edit = id;
            $("#btAddArea").hide();
            $("#btUpdateArea").show();
            showLoading();

            $.ajax({
                type: "POST",
                data: { id: id, setting_page_type: setting_page_type },
                url: 'Actionevent.asmx/getSettingByid',
                dataType: 'json',
                success: function (json) {
                    dialogArea.dialog("open");
                    $.each(json, function (value, key) {
                        $("#MainContent_thai_name_area").val(key.name_th);
                        $("#MainContent_eng_name_area").val(key.name_en);

                        if (key.code == "other")
                        {
                            $("#MainContent_check_other").prop('checked', true);

                        } else {
                            $("#MainContent_check_other").prop('checked', false);
                        }
                       

                    });

                    closeLoading();
                }
            });

        }

        function UpdateArea()
        {

            showLoading();
            var name_th = $("#MainContent_thai_name_area").val();
            if (name_th == undefined) {
                name_th = "";
            }
            var name_en = $("#MainContent_eng_name_area").val();
            if (name_en == undefined) {
                name_en = "";
            }


            var checkother = "";
            if ($('#MainContent_check_other').is(":checked"))
            {
                checkother = "other"

            }


            $.ajax({
                type: "POST",
                data: {
                    id: id_edit,
                    name_th: name_th,
                    name_en: name_en,
                    code: checkother,
                    setting_page_type: setting_page_type
                },
                url: 'Actionevent.asmx/updateSetting',
                dataType: 'json',
                success: function (result) {

                    closeLoading();
                    dialogArea.dialog("close");
                    clearArea();
                    callSettingList();
                    id_edit = "";
                }
            });



        }

        function DeleteSetting(id)
        {
            var message_confirm_delete = '<%= Resources.Main.confirm_delete %>';
            if (confirm(message_confirm_delete)) {
                showLoading();
                $.ajax({
                    type: "POST",
                    data: { id: id, setting_page_type: setting_page_type },
                    url: 'Actionevent.asmx/deleteSetting',
                    dataType: 'json',
                    success: function (json) {

                        callSettingList();
                        closeLoading();
                    }
                });

            }
        }

        function CheckNameLength(oSrc, args)
        {

            if (args.Value.length > 60) {

                args.IsValid = false;
            } else {

                args.IsValid = true;
            }


        }


</script>

    <div id="create_area" title="">
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

              <%
              if (Request.QueryString["smenu"] == "RiskFactorRelateToWork")
              {                  
            %>   
              <div class="form-group">
                <label class="col-lg-3 control-label"></label>
                <div class="col-lg-9">
                     <input type="checkbox" id="check_other" runat="server"> <%= Resources.Setting.lbOther %>
                   
                </div>
             </div>
            </div>
              <%
             }           
            %>
       
            <div class="form-group">
                <div class="col-lg-offset-3 col-lg-9">
                    <button id="btAddArea" class="btn btn-sm btn-primary" onclick="AddArea()"><%= Resources.Main.btsave %></button>
                    <button id="btUpdateArea"class="btn btn-sm btn-primary" onclick="UpdateArea()"><%= Resources.Main.btsave %></button>
                    <button class="btn btn-sm btn-default" onclick="CloseArea()"><%= Resources.Main.btClose %></button>
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
                             <button type="button" id="btAddsuperadmin" class="btn btn-primary" runat="server" onclick="showCreateData();"><i class="fa fa-plus"></i></button>
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
