<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="contractoredit.aspx.cs" Inherits="safetys4.contractoredit" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .ibox-footer {
           
            padding-bottom: 30px !important;
          
        }

         .wrapper-content {
            margin-left: 0px !important;
        } 

    </style>

    <script>
     var function_id = '<%= Session["function_id"] %>';
     var function_name = '<%= Session["function"] %>';
     var id = "";

     $(document).ready(function () {
        var url = window.location.href;
        var urlarr = url.split("=");
        id = urlarr[1];

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

                
       
        setShowedit();

    });


         function setShowedit()
         {
            
           
            // console.log(id);
             $.ajax({
                 type: "POST",
                 data: {id: id, lang : lang},
                 url: 'Actionevent.asmx/getContractorbyid',
                 dataType: 'json',
                 success: function (json) {

                     $.each(json, function (value, key)
                     {
                        
                         //$("#MainContent_lbresultfucntion").text(key.function_name);
                        $("#MainContent_ddlFunction2").val(key.function_id);
                        $("#MainContent_txtEmail").val(key.email);
                        $("#MainContent_ddlPrefixth").val(key.prefix_th);
                        $("#MainContent_txtFirstname_th").val(key.first_name_th);
                        $("#MainContent_txtLastname_th").val(key.last_name_th);
                        $("#MainContent_ddlPrefixen").val(key.prefix_en);
                        $("#MainContent_txtFirstname_en").val(key.first_name_en);
                        $("#MainContent_txtLastname_en").val(key.last_name_en);
                        $("#MainContent_txtCompany_en").val(key.company);
                        $("#MainContent_txtPhone").val(key.phone);
                        $("#MainContent_txtMobilePhone").val(key.mobile_phone);
                        $("#MainContent_lbEmployee").text(key.employee_firstname+" "+key.employee_lastname);
                        $("#MainContent_lbUpdate").text(key.update_at);
                        
                        

                        if (key.status == "valid")
                        {
                            $('#MainContent_chActive').prop('checked', true);
                        } else {
                            $('#MainContent_chActive').prop('checked', false);
                        }

                     });

                


                 }
             });


         }



         function saveContractor()
         {
             Page_ClientValidate();

             if (Page_IsValid) {
                 // showLoading();
                 var function_id = $("#MainContent_ddlFunction2").val();
                 var email = $("#MainContent_txtEmail").val();
                 var prefix_th = $("#MainContent_ddlPrefixth").val();

                 if (prefix_th == undefined) {
                     prefix_th = "";
                 }

                 var firstname_th = $("#MainContent_txtFirstname_th").val();
                 if (firstname_th == undefined) {
                     firstname_th = "";
                 }
                 var lastname_th = $("#MainContent_txtLastname_th").val();
                 if (lastname_th == undefined) {
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
                         id :id,
                         userid :user_login_id,
                         email: email,
                         prefixth: prefix_th,
                         firstnameth: firstname_th,
                         lastnameth: lastname_th,
                         prefixen: prefix_en,
                         firstnameen: firstname_en,
                         lastnameen: lastname_en,
                         companyname: company,
                         mobilephone : mobilephone,
                         phone: phone,
                         active: chactive
                     },
                     url: 'Actionevent.asmx/updateContractor',
                     dataType: 'json',
                     success: function (result) {
                         window.location.href = "contractor.aspx";

                         //document.location.href = "contractor.aspx";
                         //return true;
                     }
                 });
                 return false;

             }
             else {
                 return false;
             }

             
         }


         function CheckDuplicateEmail(oSrc, args)
         {
             var email = $('#txtEmail').val();
             var isValid;
             $.ajax({
                 type: "POST",
                 data: { email: args.Value, function_id: function_id, id: id },
                 url: 'Actionevent.asmx/checkEmailContractor',
                 dataType: 'json',
                 async: false,
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
             if (args.Value.length > 100) {

                 args.IsValid = false;
             } else {

                 args.IsValid = true;
             }


         }


         function CheckMobilePhoneLength(oSrc, args)
         {
             if (args.Value.length > 12) {

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



         function setPrefixEn()
         {
             var index_select = $("select[name='ctl00$MainContent$ddlPrefixth'] option:selected").index();
             $("select#MainContent_ddlPrefixen").prop('selectedIndex', index_select);

         }


</script>

    <div class="ibox float-e-margins">
                    <div class="ibox-title">
                        <h3><asp:Label ID="Label1" runat="server" Text="<%$ Resources:Contractor, lbDetailcontractor %>"></asp:Label></h3>
                      
                    </div>
                    <div class="ibox-content">
                  
                        <div id="contractor-form">
                              
                                             <div class="row">
				                                <div class="col-sm-12">
					                                <div class="form-group">
                                                       <asp:Label ID="lbshowfucntion" runat="server" Text="<%$ Resources:Contractor, lbfunction %>"></asp:Label><div class="lbrequire"> *</div>
                                                       <%-- <asp:Label ID="lbresultfucntion" runat="server" Text="" class="form-control"></asp:Label>--%>
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
                                                        <asp:RequiredFieldValidator ID="rqddlprefixth" runat="server" ControlToValidate ="ddlPrefixth" ErrorMessage="<%$ Resources:Contractor, rqddlprefixth %>" CssClass="text-danger">
                                                        </asp:RequiredFieldValidator>
					                                </div>
				                                </div>
				                                <div class="col-sm-5">
					                                <div class="form-group">
						                                <asp:Label ID="lbFirstname_th" runat="server" Text="<%$ Resources:Contractor, lbfristnameth %>"></asp:Label><div class="lbrequire"> *</div>
						                                <input id="txtFirstname_th" name="txtFirstname_th"  type="text" class="form-control" runat="server">
                                                         <asp:RequiredFieldValidator ID="rqfirstnameth" runat="server" ControlToValidate ="txtFirstname_th" ErrorMessage="<%$ Resources:Contractor, rqfirstnameth %>" CssClass="text-danger">
                                                        </asp:RequiredFieldValidator>
					                                </div>
				                                </div>
				                                <div class="col-sm-5">
					                                <div class="form-group">
						                                <asp:Label ID="lbLastname_th" runat="server" Text="<%$ Resources:Contractor, lblastnameth %>"></asp:Label><div class="lbrequire"> *</div>
						                                <input id="txtLastname_th" name="txtLastname_th" type="text" class="form-control" runat="server">
                                                         <asp:RequiredFieldValidator ID="rqlastnameth" runat="server" ControlToValidate ="txtLastname_th" ErrorMessage="<%$ Resources:Contractor, rqlastnameth %>" CssClass="text-danger">
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
                                                         <asp:RequiredFieldValidator ID="rqddlprefixen" runat="server" ControlToValidate ="ddlPrefixen" ErrorMessage="<%$ Resources:Contractor, rqddlprefixen %>" CssClass="text-danger">
                                                        </asp:RequiredFieldValidator>
					                                </div>
				                                </div>
				                                <div class="col-sm-5">
					                                <div class="form-group">
						                                <asp:Label ID="lbFirstname_en" runat="server" Text="<%$ Resources:Contractor, lbfirstnameen %>"></asp:Label><div class="lbrequire"> *</div>
						                                <input id="txtFirstname_en" name="txtFirstname_en" type="text" class="form-control" runat="server">
                                                         <asp:RequiredFieldValidator ID="rqfirstnameen" runat="server" ControlToValidate ="txtFirstname_en" ErrorMessage="<%$ Resources:Contractor, rqfirstnameen %>" CssClass="text-danger">
                                                        </asp:RequiredFieldValidator>
					                                </div>
				                                </div>
				                                <div class="col-sm-5">
					                                <div class="form-group">
						                                <asp:Label ID="lbLastname_en" runat="server" Text="<%$ Resources:Contractor, lblastnameen %>"></asp:Label>
						                                <input id="txtLastname_en" name="txtLastname_en" placeholder="" type="text" class="form-control" runat="server">
                                                        <asp:RequiredFieldValidator ID="rqlastnameen" runat="server" ControlToValidate ="txtLastname_en" ErrorMessage="<%$ Resources:Contractor, rqlastnameen %>" CssClass="text-danger">
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
                                                     <div class="form-group pull-left">
                                                         <%
                                                            ArrayList per = Session["permission"] as ArrayList;
                                                            if (per.IndexOf("contractor edit") > -1)         
                                                            {                            
       
                                                            %>
                                                                  <asp:Button ID="btSave" runat="server" Text="<%$ Resources:Main, btsave %>"  CssClass="btn btn-primary" OnClientClick="return saveContractor();"  />

                                                           <%                      
                                                            }
       
                                                            %>

                                                              <asp:Button ID="btCancel" runat="server" Text="<%$ Resources:Main, btCancel %>"  CssClass="btn btn-default" CausesValidation="False" OnClick="btCancel_Click"/>
                      
                                                    </div>
                                                </div>
               
                                                 <div class="col-sm-4">
                    
                                                </div>

                                                  <div class="col-sm-4">
                                                   
                                                </div>
                                             </div>

                                    </div>


                    </div>
                    <div class="ibox-footer">
                        <span class="pull-right">
                            <asp:Label ID="lbModify" runat="server" Text="<%$ Resources:Contractor, lbModify %>"></asp:Label>
                            <asp:Label ID="lbEmployee" runat="server" Text=""></asp:Label>
                            <asp:Label ID="lbDate" runat="server" Text="<%$ Resources:Contractor, lbUpdate %>"></asp:Label>
                            <asp:Label ID="lbUpdate" runat="server" Text=""></asp:Label>
                        </span>
                      
                    </div>
                </div>


    

  





</asp:Content>
