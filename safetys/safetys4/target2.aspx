<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="target2.aspx.cs" Inherits="safetys4.target2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    

<style type="text/css">

</style>
<script>
  
    var current_year = '<%= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year %>';
    var select_function_id = "";
 $(document).ready(function () {

     setYear(current_year);
    
     
 });

 function saveTable()
 {
     showLoading();
     var rows = $('#main_table').find('tbody').find('tr');
     for (var i = 0; i < rows.length; i++)
     {
      
         var site_id = $(rows[i]).find('td:eq(0)').html();
       //  console.log(site_id);
         var tifr_employee = $("#" + site_id + "tifr_employee").val();
         var tifr_contractor_onsite = $("#" + site_id + "tifr_contractor_onsite").val();
         var tifr_contractor_offsite = $("#" + site_id + "tifr_contractor_offsite").val();
         var ltifr_employee = $("#" + site_id + "ltifr_employee").val();
         var ltifr_contractor_onsite = $("#" + site_id + "ltifr_contractor_onsite").val();
         var ltifr_contractor_offsite = $("#" + site_id + "ltifr_contractor_offsite").val();
         var multiplier = $("#" + site_id + "multiplier").val();
         var multiplier_contractor = $("#" + site_id + "multiplier_contractor").val();
        
         var ddl_year_id = $('#MainContent_ddyear').val();

         $.ajax({
             type: "POST",
             data: {
                 tifr_employee: tifr_employee,
                 tifr_contractor_onsite: tifr_contractor_onsite,
                 tifr_contractor_offsite: tifr_contractor_offsite,
                 ltifr_employee : ltifr_employee,
                 ltifr_contractor_onsite:ltifr_contractor_onsite,
                 ltifr_contractor_offsite: ltifr_contractor_offsite,
                 multiplier: multiplier,
                 multiplier_contractor: multiplier_contractor,
                 site_id: site_id,
                 year: ddl_year_id,
                 lang:lang

             },
             url: 'Actionevent.asmx/createTargetMainSrilanka',
             dataType: 'json',
             success: function (result) {

                 closeLoading();
                
                 searchTable();

             }
         });

     }
     return false;
 }



    function filterTable()
    {

        showLoading();
        setTimeout(function () { closeLoading(); }, 1000);
        searchTable();

        return false;
    }

    function searchTable()
    {
        var ddl_year_id = $('#MainContent_ddyear').val();
     

        $.ajax({
            type: "POST",
            data: { year: ddl_year_id, lang: lang },
            url: 'Datatablelist.asmx/getListTargetSrilanka',
            dataType: 'json',
            success: function (json) {
               // alert("test");
                $("#tb_main").html(json.data);
               // callTargetByFunction(select_function_id);

            }
        });

    }
   

    function setYear(current_year)
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getTargetYearSrilanka',
            dataType: 'json',
            success: function (json) {

               // var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_ddyear");
                $el.empty(); // remove old options

                //$el.append($("<option></option>")
                //           .attr("value", "2016").text("2559"));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.year));
                });

                $('#MainContent_ddyear').val(current_year);
                searchTable();
            }
        });



    }

  


</script>





               <div class="row" id="filter">
                   
                   <div class="col-md-2">
                       <h2><b>Target</b></h2>
                       <br />
                        <div class="form-group">    
                        <div class="form-inline">
                            <label class="control-label"><%= Resources.Hazard.lbyear %></label>
                            <select id="ddyear" class="form-control"  runat="server">
                       
                            </select>
                                <asp:Button ID="btSearch"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterTable();" CssClass="btn btn-primary"/>
                                       
                                            
                            </div>
                        </div>
                    </div>

   </div>

      <div class="row">
        <div class="col-md-12">
            <div class="pull-right">
                <%-- <asp:Button ID="btCancel"  runat="server" Text="<%$ Resources:Main, btcancel %>"  CssClass="btn btn-primary" OnClientClick="return filterTable();"/>--%>
                    <asp:Button ID="btSave"  runat="server" Text="<%$ Resources:Main, btsave %>"  CssClass="btn btn-primary" OnClientClick="return saveTable();"/>
          

            </div>
            
        </div>
    </div>
    <br />
    <br />
    <div class="row">
        <div class="col-md-12">
              <table id="main_table" class="table table-bordered table-hover">
                 <thead>
                    <tr>
                        
                        <th><%= Resources.Sot.lbsite %></th>
                        <th>TIFR Employee</th>
                        <th>LTIFR Employee</th>
                        <th>Multiplier Employee</th>
                        <th>TIFR Contractor Onsite</th>
                        <th>TIFR Contractor Offsite </th>
                        <th>LTIFR Contractor Onsite</th>
                        <th>LTIFR Contractor Offsite </th>
                        <th>Multiplier Contractor</th>
                    </tr>
                </thead>
                <tbody id="tb_main">


                </tbody>
                 
         
        </table>
        <br />
        <br />



        </div>
    </div>



</asp:Content>
