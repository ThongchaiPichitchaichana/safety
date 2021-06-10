<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="contractor.aspx.cs" Inherits="safetys4.contractor1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="template/css/plugins/dataTables/jquery.dataTables.min.css" rel="stylesheet">
    <script src="template/js/plugins/dataTables/jquery.dataTables.min.js"></script>
   <%-- <script src="template/js/plugins/dataTables/fnReloadAjax.js"></script>--%>

    <style>
        #tbContractor tbody tr {
            cursor: pointer;
        }

        .wrapper-content {
            margin-left: 0px !important;
        } 
    </style>

     <script>
         var dataTable; //reference to your dataTable
         var dialog;
         var function_id = '<%= Session["function_id"] %>';
         var function_name = '<%= Session["function"] %>';

         $(document).ready(function () {
           
            // $('#ddlFunction').val(detault_org_id)
           
               <%
                ArrayList per = Session["permission"] as ArrayList;
                if (!(per.IndexOf("contractor search") > -1))         
                {                            
       
                %>
                    $("#ddlFunction").attr('disabled', 'disabled');
               <%                      
                }
       
                %>

             dialog = $("#contractor-form").dialog({
                 autoOpen: false,
                 height: 600,
                 width: 800,
                 modal: true,
                 //buttons: {
                 //    Cancel: function () {
                 //        dialog.dialog("close");
                 //    }
                 //},
                 close: function () {
                     
                     //allFields.removeClass("ui-state-error");
                     
                 },
                 open: function (event, ui) {
                     clearValidationErrors();
                     clearData();
                     $('#MainContent_ddlFunction2').val(function_id);

                     $("#contractor-form").css('overflow-x', 'hidden');
                 },
                 modal: true,
             });



             $.ajax({
                 type: "POST",
                 data: {lang :lang },
                 url: 'Masterdata.asmx/getFuctionlist',
                 dataType: 'json',
                 success: function (json) {
                    
                     var $el = $("#ddlFunction");
                     $el.empty(); // remove old options
                    
                     $.each(json, function (value, key) {
                        
                         $el.append($("<option></option>")
                                 .attr("value", key.id).text(key.name));
                     });

                     $('#ddlFunction').val(function_id);
                     setDropdownStatus();


                 }
             });


             $.ajax({
                 type: "POST",
                 data: { lang: lang },
                 url: 'Masterdata.asmx/getFuctionlist',
                 dataType: 'json',
                 success: function (json) {

                     var $el = $("#MainContent_ddlFunction2");
                     $el.empty(); // remove old options
                     $el.append($("<option></option>")
                                .attr("value", "").text(""));
                     $.each(json, function (value, key) {

                         $el.append($("<option></option>")
                                 .attr("value", key.id).text(key.name));
                     });

                   

                 }
             });

            <%
             if (Session["country"].ToString() =="thailand")
             {                  
            %>
             $("#MainContent_ddlFunction2").attr('disabled', 'disabled');

            <%                      
            }
           
            %>

                


         });


         function setDropdownStatus()
         {
             $.ajax({
                 type: "POST",
                 data: { lang: lang },
                 url: 'Masterdata.asmx/getStatuscontractor',
                 dataType: 'json',
                 success: function (json) {

                     var all = '<%= Resources.Main.all %>';
                     var $el = $("#ddlStatus");
                     $el.empty(); // remove old options
                     $el.append($("<option></option>")
                           .attr("value", '').text(all));
                     $.each(json, function (value, key) {

                         $el.append($("<option></option>")
                                 .attr("value", key.id).text(key.name));
                     });

                     setDatatableList();


                 }
             });


         }


         function setDatatableList()
         {

             var ddl_function_id = $('#ddlFunction').val();
             var ddl_status = $('#ddlStatus').val();


             dataTable = $("#tbContractor").DataTable({
                 "bProcessing": true,
                 "sProcessing": true,
               
                /* "ajax": {
                     "url": 'Datatablelist.asmx/getListcontractor',
                     "type": 'GET',
                     "data": {
                         functionid: $('#ddlFunction').val(),
                         status: ddl_status,
                         lang: lang

                     },
                 },*/
                 "bPaginate": true,
                 "bInfo": true,
                 "bFilter": true,
                 "ordering": true,
                 // "stateSave": true,
                 "responsive": true,
                 "pageLength": 50,
                 "serverSide": true,
                 "lengthChange": false,
                 "order": [],
                 "language": {
                     "url": 'Langdatatable.aspx'
                 },
                 "columnDefs": [
                    {
                        "targets": [1],
                        "visible": false,
                    }
                 ]

             });


             dataTable.ajax.url('Datatablelist.asmx/getListcontractor?functionid=' + ddl_function_id + "&status=" + ddl_status + "&lang=" + lang);

             $('#tbContractor tbody').on('click', 'tr', function () {
                 var data = dataTable.row(this).data();
                 var url = "contractoredit.aspx?id=" + data[1];
                 window.location.href = url;
                
             });





         }

         function showCreate()
         {
             $("#MainContent_lbresultfucntion").text(function_name)
             dialog.dialog("open");

         }
       

         function changFucntion()
         {
             var ddl_function_id = $('#ddlFunction').val();
             var ddl_status = $('#ddlStatus').val();

             //dataTable = $('#tbContractor').data('dt_params', {
             //                    functionid: ddl_function_id,
             //                    status: ddl_status,
             //                    lang: lang

             //                });
             // Redraw data table, causes data to be reloaded
             //dataTable = $('#tbContractor').DataTable().draw();
            // console.log(ddl_function_id + "," + ddl_status)
             //dataTable.fnReloadAjax('Datatablelist.asmx/getListcontractor?functionid=' + ddl_function_id+"&status"+ddl_status+"&lang"+lang);
             //dataTable.ajax.reload();
            
            dataTable.ajax.url('Datatablelist.asmx/getListcontractor?functionid=' + ddl_function_id + "&status=" + ddl_status + "&lang=" + lang).load();

         }


         function addContractor()
         {
             Page_ClientValidate();
             if (Page_IsValid)
             {
                 showLoading();
                 var function_id = $("#MainContent_ddlFunction2").val();
                 var email = $("#MainContent_txtEmail").val();
                 var prefix_th = $("#MainContent_ddlPrefixth").val();

                 if (prefix_th == undefined)
                 {
                     prefix_th = "";
                 }

                 var firstname_th = $("#MainContent_txtFirstname_th").val();
                 if (firstname_th == undefined)
                 {
                     firstname_th = "";
                 }
                 var lastname_th = $("#MainContent_txtLastname_th").val();
                 if (lastname_th == undefined)
                 {
                     lastname_th = "";
                 }

                 var prefix_en = $("#MainContent_ddlPrefixen").val();
                 var firstname_en = $("#MainContent_txtFirstname_en").val();
                 var lastname_en = $("#MainContent_txtLastname_en").val();
                 var company = $("#MainContent_txtCompany_en").val();
                 var phone = $("#MainContent_txtPhone").val();
                 var mobilephone = $("#MainContent_txtMobilePhone").val();
                 var chactive = "";
             

                 if ($('#MainContent_chActive').is(":checked"))
                 {
                     chactive = "valid"
                 } else {

                     chactive = "invalid"
                 }
          
                 $.ajax({
                     type: "POST",
                     data: {
                             function_id: function_id,
                             userid :user_login_id,
                             email: email,
                             prefixth: prefix_th,
                             firstnameth: firstname_th,
                             lastnameth: lastname_th,
                             prefixen: prefix_en,
                             firstnameen: firstname_en,
                             lastnameen: lastname_en,
                             companyname: company,
                             mobilephone: mobilephone,
                             phone: phone,
                             active : chactive
                            },
                     url: 'Actionevent.asmx/createContractor',
                     dataType: 'json',
                     success: function (result) {
                        
                         closeLoading();
                         changFucntion();
                         dialog.dialog("close");
                         clearData();
                         //return true;
                     }
                 });
                 //do some stuff
               
             }
             else {
                 return false;
             }
           

         }


         function CheckDuplicateEmail(oSrc, args)
         {
              //var email = $('#txtEmail').val();
             var isValid;
             $.ajax({
                 type: "POST",
                 data: { email: args.Value, function_id: function_id, id: '' },
                 url: 'Actionevent.asmx/checkEmailContractor',
                 dataType: 'json',
                 async:false,
                 success: function (result) {

                     if (result == true)//duplicate
                     {
                         isValid = false;

                     } else {

                         isValid = true;
                       ;
                     }
                     
                 }
             });
            // console.log(isValid);
             args.IsValid = isValid;

         }


         function CheckCompanyLength(oSrc, args)
         {
             if (args.Value.length > 100)
             {

                 args.IsValid = false;
             } else {

                 args.IsValid = true;
             }


         }

         function CheckMobilePhoneLength(oSrc, args)
         {
             if (args.Value.length > 12)
             {

                 args.IsValid = false;
             } else {

                 args.IsValid = true;
             }


         }

         function CheckPhoneLength(oSrc, args) {
             if (args.Value.length > 50) {

                 args.IsValid = false;
             } else {

                 args.IsValid = true;
             }


         }



         function clearData()
         {
             //$("#MainContent_ddlFunction2").val("");
             $("#MainContent_txtEmail").val("");
             $("#MainContent_ddlPrefixth").val("");
             $("#MainContent_txtFirstname_th").val("");
             $("#MainContent_txtLastname_th").val("");
             $("#MainContent_ddlPrefixen").val("");
             $("#MainContent_txtFirstname_en").val("");
             $("#MainContent_txtLastname_en").val("");
             $("#MainContent_txtCompany_en").val("");
             $("#MainContent_txtPhone").val("");
             $("#MainContent_txtMobilePhone").val("");
             $('#MainContent_chActive').attr('checked', false);



         }


         function clearValidationErrors()
         {
             var i;
             for (i = 0; i < Page_Validators.length; i++)
             {

                 if (Page_Validators[i].id == "MainContent_rqfunction"
                     ||Page_Validators[i].id == "MainContent_rqemail"
                     || Page_Validators[i].id == "MainContent_rqemail2"
                     || Page_Validators[i].id == "MainContent_rqemail3"
                     || Page_Validators[i].id == "MainContent_rqcompany"
                     || Page_Validators[i].id == "MainContent_rqcompanylength"
                     || Page_Validators[i].id == "MainContent_rqmobilephone"
                     || Page_Validators[i].id == "MainContent_rqformphone"
                     || Page_Validators[i].id == "MainContent_rqmobilephonelength"
                     || Page_Validators[i].id == "MainContent_rqphonelength"
                     || Page_Validators[i].id == "MainContent_rqddlprefixth"
                     || Page_Validators[i].id == "MainContent_rqfirstnameth"
                     || Page_Validators[i].id == "MainContent_rqlastnameth"
                     || Page_Validators[i].id == "MainContent_rqddlprefixen"
                     || Page_Validators[i].id == "MainContent_rqfirstnameen"
                     || Page_Validators[i].id == "MainContent_rqlastnameen"

                     )
                {
              
                     Page_Validators[i].style.display = "none";

                 } else {
                     Page_Validators[i].style.visibility = "hidden";

                 }
                
             }
         }

         function setPrefixEn()
         {
             var index_select = $("select[name='ctl00$MainContent$ddlPrefixth'] option:selected").index();
             $("select#MainContent_ddlPrefixen").prop('selectedIndex', index_select);

         }

    </script>
    <div id="contractor-form">
        <h3><asp:Label ID="lbAddcontractor" runat="server" Text="<%$ Resources:Contractor, lbAddcontractor %>"></asp:Label></h3>
        <fieldset>
             <div class="row">
				<div class="col-sm-12">
					<div class="form-group">
                       <asp:Label ID="lbshowfucntion" runat="server" Text="<%$ Resources:Contractor, lbfunction %>"></asp:Label><div class="lbrequire"> *</div>
                        <%--<asp:Label ID="lbresultfucntion" runat="server" Text="" class="form-control"></asp:Label>--%>
                         <select id="ddlFunction2" class="form-control" runat="server">
                          
                         </select>
                         <asp:RequiredFieldValidator ID="rqfunction" runat="server" ControlToValidate ="ddlFunction2" ErrorMessage="<%$ Resources:Contractor, rqddlfunction %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				
		   </div>
            <div class="row">
				<div class="col-sm-12">
					<div class="form-group">
                        <asp:Label ID="lbemailform" runat="server" Text="<%$ Resources:Contractor, lbemail %>"></asp:Label><div class="lbrequire"> *</div>
						<input id="txtEmail" name="txtEmail"  type="text" class="form-control" runat="server">
                        <asp:RequiredFieldValidator ID="rqemail" runat="server" ControlToValidate ="txtEmail" ErrorMessage="<%$ Resources:Contractor, rqemail %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="rqemail2" runat="server" ControlToValidate="txtEmail" ErrorMessage="<%$ Resources:Contractor, rqemailExpression %>" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic" CssClass="text-danger">
                        </asp:RegularExpressionValidator>
                        <asp:CustomValidator id="rqemail3" runat="server" ClientValidationFunction="CheckDuplicateEmail" ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="<%$ Resources:Contractor, rqemailDuplicate %>" CssClass="text-danger"></asp:CustomValidator>
					</div>
				</div>
				
		   </div>

            <%
              if (Session["country"].ToString() =="thailand")
              {                  
            %>
          	<div class="row">
				<div class="col-sm-2">
					<div class="form-group">
						<asp:Label ID="lbprefix_th" runat="server" Text="<%$ Resources:Contractor, lbprefixth %>"></asp:Label><div class="lbrequire"> *</div>
                        <select id="ddlPrefixth" class="form-control" runat="server" onchange="setPrefixEn();">
                            <option value=""></option>
                            <option value="นาย">นาย</option>
                            <option value="นาง">นาง</option>
                            <option value="นางสาว">นางสาว</option>
                        </select>
                        <asp:RequiredFieldValidator ID="rqddlprefixth" runat="server" ControlToValidate ="ddlPrefixth" ErrorMessage="<%$ Resources:Contractor, rqddlprefixth %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				<div class="col-sm-5">
					<div class="form-group">
						<asp:Label ID="lbFirstname_th" runat="server" Text="<%$ Resources:Contractor, lbfristnameth %>"></asp:Label><div class="lbrequire"> *</div>
						<input id="txtFirstname_th" name="txtFirstname_th"  type="text" class="form-control" runat="server">
                         <asp:RequiredFieldValidator ID="rqfirstnameth" runat="server" ControlToValidate ="txtFirstname_th" ErrorMessage="<%$ Resources:Contractor, rqfirstnameth %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				<div class="col-sm-5">
					<div class="form-group">
						<asp:Label ID="lbLastname_th" runat="server" Text="<%$ Resources:Contractor, lblastnameth %>"></asp:Label><div class="lbrequire"> *</div>
						<input id="txtLastname_th" name="txtLastname_th" type="text" class="form-control" runat="server">
                         <asp:RequiredFieldValidator ID="rqlastnameth" runat="server" ControlToValidate ="txtLastname_th" ErrorMessage="<%$ Resources:Contractor, rqlastnameth %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
		   </div>

             <%
             }           
            %>


            <div class="row">
				<div class="col-sm-2">
					<div class="form-group">
						<asp:Label ID="lbprefix_en" runat="server" Text="<%$ Resources:Contractor, lbprefixen %>"></asp:Label><div class="lbrequire"> *</div>
						 <select id="ddlPrefixen" class="form-control" runat="server">
                            <option value=""></option>
                            <option value="Mr.">Mr.</option>
                            <option value="Mrs.">Mrs.</option>
                            <option value="Miss">Miss</option>
                        </select>
                         <asp:RequiredFieldValidator ID="rqddlprefixen" runat="server" ControlToValidate ="ddlPrefixen" ErrorMessage="<%$ Resources:Contractor, rqddlprefixen %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				<div class="col-sm-5">
					<div class="form-group">
						<asp:Label ID="lbFirstname_en" runat="server" Text="<%$ Resources:Contractor, lbfirstnameen %>"></asp:Label><div class="lbrequire"> *</div>
						<input id="txtFirstname_en" name="txtFirstname_en" type="text" class="form-control" runat="server">
                         <asp:RequiredFieldValidator ID="rqfirstnameen" runat="server" ControlToValidate ="txtFirstname_en" ErrorMessage="<%$ Resources:Contractor, rqfirstnameen %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
				<div class="col-sm-5">
					<div class="form-group">
						<asp:Label ID="lbLastname_en" runat="server" Text="<%$ Resources:Contractor, lblastnameen %>"></asp:Label>
						<input id="txtLastname_en" name="txtLastname_en" placeholder="" type="text" class="form-control" runat="server">
                        <asp:RequiredFieldValidator ID="rqlastnameen" runat="server" ControlToValidate ="txtLastname_en" ErrorMessage="<%$ Resources:Contractor, rqlastnameen %>" CssClass="text-danger" Display="Dynamic">
                        </asp:RequiredFieldValidator>
					</div>
				</div>
		   </div>

           <div class="row">
				<div class="col-sm-12">
					<div class="form-group">
						<asp:Label ID="lbCompany_en" runat="server" Text="<%$ Resources:Contractor, lbcompanyen %>"></asp:Label><div class="lbrequire"> *</div>
						<input id="txtCompany_en" name="txtCompany_en" type="text" class="form-control" runat="server">
                        <asp:RequiredFieldValidator ID="rqcompany" runat="server" ControlToValidate ="txtCompany_en" ErrorMessage="<%$ Resources:Contractor, rqcomapany %>" CssClass="text-danger"  Display="Dynamic">
                        </asp:RequiredFieldValidator>
                        <asp:CustomValidator id="rqcompanylength" runat="server" ClientValidationFunction="CheckCompanyLength" ControlToValidate="txtCompany_en" Display="Dynamic" ErrorMessage="<%$ Resources:Contractor, rqcompanylength %>" CssClass="text-danger"></asp:CustomValidator>
					</div>
				</div>
				
		   </div>

           <div class="row">
               <div class="col-sm-6">
					<div class="form-group">
						<asp:Label ID="lbmobilephone" runat="server" Text="<%$ Resources:Contractor, lbmobilephone %>"></asp:Label><div class="lbrequire"> *</div>
						<input id="txtMobilePhone" name="txtMobilePhone"  type="text" class="form-control" runat="server">
                         <asp:RequiredFieldValidator ID="rqmobilephone" runat="server" ControlToValidate ="txtMobilePhone" ErrorMessage="<%$ Resources:Contractor, rqmobilephone %>" CssClass="text-danger"  Display="Dynamic">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="rqformphone" runat="server" ErrorMessage="<%$ Resources:Contractor, rqmobileformphone %>" ControlToValidate="txtMobilePhone"  Display="Dynamic" CssClass="text-danger"
                            ValidationExpression= "^[0-9]*$"></asp:RegularExpressionValidator>
                        <asp:CustomValidator id="rqmobilephonelength" runat="server" ClientValidationFunction="CheckMobilePhoneLength" ControlToValidate="txtMobilePhone" Display="Dynamic" ErrorMessage="<%$ Resources:Contractor, rqmobilephonelength %>" CssClass="text-danger"></asp:CustomValidator>

					</div>
				</div>

               <div class="col-sm-6">
					<div class="form-group">
						<asp:Label ID="lbPhoneform" runat="server" Text="<%$ Resources:Contractor, lbphone %>"></asp:Label>
						<input id="txtPhone" name="txtPhone"  type="text" class="form-control" runat="server">
                        <asp:CustomValidator id="rqphonelength" runat="server" ClientValidationFunction="CheckPhoneLength" ControlToValidate="txtPhone"  ErrorMessage="<%$ Resources:Contractor, rqphonelength %>" CssClass="text-danger"></asp:CustomValidator>

                      <%--   <asp:RequiredFieldValidator ID="rqphone" runat="server" ControlToValidate ="txtPhone" ErrorMessage="<%$ Resources:Contractor, rqphone %>" CssClass="text-danger">
                        </asp:RequiredFieldValidator>--%>
					</div>
				</div>
				
		   </div>

            <div class="row">
				<div class="col-sm-12">
					<div class="form-group">			
						<input  type="checkbox" id="chActive" name="chActive" runat="server"><asp:Label ID="lbchActive" runat="server" Text="<%$ Resources:Contractor, chactive %>"></asp:Label>
					</div>
				</div>
		   </div>
             <div class="row">
                <div class="col-sm-4">
                    
                </div>
               
                 <div class="col-sm-4">
                    
                </div>

                  <div class="col-sm-4">
                    <div class="form-group pull-right">
                        <asp:Button ID="btAdd" runat="server" Text="<%$ Resources:Main, btadd %>" OnClientClick="addContractor();" CssClass="btn btn-primary"/>
                      
                    </div>
                </div>
             </div>

        </fieldset>

    </div>

  
     <div class="row">
         <table class="form-filter">
             <tr>
                 <td>
                     <asp:Label ID="lbFormheader" runat="server" Text="<%$ Resources:Contractor, lbformheader %>"></asp:Label> 
                 </td>
                 <td>
                        <%
                            ArrayList per2 = Session["permission"] as ArrayList;
                            if (per2.IndexOf("contractor add") > -1)         
                           {                            
       
                          %>
                                <button type="button" id="btAddContractor" class="btn btn-primary" runat="server" onclick="showCreate();"><i class="fa fa-plus"></i></button>
                          <%                      
                            }
       
                          %>
                 
                 </td>
                 <td></td>
                 <td></td>
                
             </tr>
             <tr>
                 <td>
                     <asp:Label ID="lbfunction" runat="server" Text="<%$ Resources:Contractor, lbfunction %>"></asp:Label></td>
                 <td> 
                     <select id="ddlFunction" class="form-control" onchange="changFucntion();">
                       
                     </select>
                 </td>

                 <td><asp:Label ID="lbstatusform" runat="server" Text="<%$ Resources:Contractor, lbstatus %>"></asp:Label></td>
                 <td>
                      <select id="ddlStatus" class="form-control" onchange="changFucntion();">
                       
                     </select>

                 </td>
             </tr>

         </table>
       
       
     </div>
      <table id="tbContractor" class="table table-bordered table-hover" cellspacing="0" width="100%">
                 <thead>
                    <tr>
                        <th><asp:Label ID="lbno" runat="server" Text="<%$ Resources:Contractor, lbno %>"></asp:Label></th>
                        <th><asp:Label ID="lbID" runat="server" Text=""></asp:Label></th>
                        <th><asp:Label ID="lbcompany" runat="server" Text="<%$ Resources:Contractor, lbcompany %>"></asp:Label></th>
                        <th><asp:Label ID="lbfullname" runat="server" Text="<%$ Resources:Contractor, lbfullname %>"></asp:Label></th>
                        <th><asp:Label ID="lbphone" runat="server" Text="<%$ Resources:Contractor, lbphone %>"></asp:Label></th>
                        <th><asp:Label ID="lbemail" runat="server" Text="<%$ Resources:Contractor, lbemail %>"></asp:Label></th>
                        <th><asp:Label ID="lbstatus" runat="server" Text="<%$ Resources:Contractor, lbstatus %>"></asp:Label></th>
                       
                    
                    </tr>
                </thead>
         
        </table>



</asp:Content>
