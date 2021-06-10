<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="HealthIndividualReport.aspx.cs" Inherits="safetys4.HealthIndividualReport" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link href="template/css/plugins/select2/select2.min.css" rel="stylesheet">

<script type="text/javascript" src="template/js/plugins/select2/select2.min.js"></script>
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

 $(document).ready(function () {
     $('.select2').select2();
    <%     
     if (!IsPostBack)
     {                  
         %>
          getEmployee();
         
    <%     
                    
        }
               
    %>
  
     
    
     
 });


    function getEmployee()
    {

        $.ajax({
            type: "POST",
            data: { lang: lang, employee_id: "", pagetype: "" },
            url: 'Masterdata.asmx/getEmployeeDropdown',
            dataType: 'json',
            success: function (json) {

                var $el = $("#MainContent_ddemployee");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(""));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.employee_id).text(key.employee_id));

                   
                });

               

            }
        });



    }



</script>



 <div class="row" id="filter">
                   <div class="row">
                        <div class="col-md-12">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Health.lbemployeeid%></label>                         

                          <select id="ddemployee" class="form-control select2" runat="server">
                         </select>
                       </div>
                  </div>
                   
                  
   </div>

      <div class="row">
        <div class="col-md-12">
            <asp:Button ID="btExportpdf"  runat="server" Text="<%$ Resources:Main, btExportpdf %>"  CssClass="btn btn-primary" OnClick="btExportIndividual_Click"/>
        </div>
    </div>




                   

 

</asp:Content>
