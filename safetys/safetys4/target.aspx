<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="target.aspx.cs" Inherits="safetys4.target" %>
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
         var function_id = $(rows[i]).find('td:eq(1)').html();
         var tifr_employee = $("#" + function_id + "tifr_employee").val();
         var tifr_contractor_onsite = $("#" + function_id + "tifr_contractor_onsite").val();
         var tifr_contractor_offsite = $("#" + function_id + "tifr_contractor_offsite").val();
         var tifr_all = $("#" + function_id + "tifr_all").val();
         var ltifr_employee = $("#" + function_id + "ltifr_employee").val();
         var ltifr_contractor_onsite = $("#" + function_id + "ltifr_contractor_onsite").val();
         var ltifr_contractor_offsite = $("#" + function_id + "ltifr_contractor_offsite").val();
         var ltifr_all = $("#" + function_id + "ltifr_all").val();
         var multiplier = $("#" + function_id + "multiplier").val();
         var multiplier_contractor_onsite = $("#" + function_id + "multiplier_contractor_onsite").val();
         var multiplier_contractor_offsite = $("#" + function_id + "multiplier_contractor_offsite").val();
         var multiplier_all = $("#" + function_id + "multiplier_all").val();
         
         var ddl_year_id = $('#MainContent_ddyear').val();

         $.ajax({
             type: "POST",
             data: {
                 tifr_employee: tifr_employee,
                 tifr_contractor_onsite: tifr_contractor_onsite,
                 tifr_contractor_offsite: tifr_contractor_offsite,
                 tifr_all : tifr_all,
                 ltifr_employee : ltifr_employee,
                 ltifr_contractor_onsite:ltifr_contractor_onsite,
                 ltifr_contractor_offsite: ltifr_contractor_offsite,
                 ltifr_all : ltifr_all,
                 multiplier: multiplier,
                 multiplier_contractor_onsite: multiplier_contractor_onsite,
                 multiplier_contractor_offsite: multiplier_contractor_offsite,
                 multiplier_all : multiplier_all,
                 function_id: function_id,
                 year: ddl_year_id,
                 lang:lang

             },
             url: 'Actionevent.asmx/createTargetMain',
             dataType: 'json',
             success: function (result) {

                 closeLoading();
                
                 searchTable();

             }
         });

     }
     return false;
 }


 function saveTableSub()
 {
     showLoading();
     var rows = $('#sub_table').find('tbody').find('tr');
     var type = "";
     for (var i = 0; i < rows.length; i++)
     {
         var department_id = $(rows[i]).find('td:eq(1)').html();
         var tifr_employee = $("#" + department_id + "sub_tifr_employee").val();
         var tifr_contractor_onsite = $("#" + department_id + "sub_tifr_contractor_onsite").val();
         var tifr_contractor_offsite = $("#" + department_id + "sub_tifr_contractor_offsite").val();
         var tifr_all = $("#" + department_id + "sub_ltifr_all").val();
         var ltifr_employee = $("#" + department_id + "sub_ltifr_employee").val();
         var ltifr_contractor_onsite = $("#" + department_id + "sub_ltifr_contractor_onsite").val();
         var ltifr_contractor_offsite = $("#" + department_id + "sub_ltifr_contractor_offsite").val();
         var ltifr_all = $("#" + department_id + "sub_tifr_all").val();
         var multiplier = $("#" + department_id + "sub_multiplier").val();
         var multiplier_contractor_onsite = $("#" + department_id + "sub_multiplier_contractor_onsite").val();
         var multiplier_contractor_offsite = $("#" + department_id + "sub_multiplier_contractor_offsite").val();
         var multiplier_all = $("#" + department_id + "sub_multiplier_all").val();

         var ddl_year_id = $('#MainContent_ddyear').val();
         if (i == 0)
         {
             type = "function";
         } else {
             type = "";
         }

         $.ajax({
             type: "POST",
             data: {
                 tifr_employee: tifr_employee,
                 tifr_contractor_onsite: tifr_contractor_onsite,
                 tifr_contractor_offsite: tifr_contractor_offsite,
                 tifr_all : tifr_all,
                 ltifr_employee: ltifr_employee,
                 ltifr_contractor_onsite: ltifr_contractor_onsite,
                 ltifr_contractor_offsite: ltifr_contractor_offsite,
                 ltifr_all : ltifr_all,
                 multiplier: multiplier,
                 multiplier_contractor_onsite: multiplier_contractor_onsite,
                 multiplier_contractor_offsite: multiplier_contractor_offsite,
                 multiplier_all : multiplier_all,
                 function_id: select_function_id,
                 department_id: department_id,
                 year:ddl_year_id,
                 lang: lang,
                 type:type

             },
             url: 'Actionevent.asmx/createTargetSub',
             dataType: 'json',
             success: function (result) {

                 setTimeout(function () { closeLoading(); }, 1000);
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
            url: 'Datatablelist.asmx/getListTarget',
            dataType: 'json',
            success: function (json) {
               // alert("test");
                $("#tb_main").html(json.data);
                callTargetByFunction(select_function_id);

            }
        });

    }
   

    function setYear(current_year)
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getTargetYear',
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

    function getTargetSub(function_id)
    {
        showLoading();
        callTargetByFunction(function_id);
        return false;
    }


    function callTargetByFunction(function_id)
    {       
        select_function_id = function_id;
        var ddl_year_id = $('#MainContent_ddyear').val();


        $.ajax({
            type: "POST",
            data: { function_id : function_id,year: ddl_year_id, lang: lang },
            url: 'Datatablelist.asmx/getListTargetSub',
            dataType: 'json',
            success: function (json) {
                // alert("test");
                $("#tb_sub").html(json.data);

                setTimeout(function () { closeLoading(); }, 1000);
            }
        });


        return false;
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
            <div style="overflow-x:scroll;">
              <table id="main_table" class="table table-bordered table-hover" style="width:100%;">
                 <thead>
                    <tr>
                        <th><%= Resources.Incident.lbCompany %></th>
                        <th><%= Resources.Incident.lbfucntion %></th>
                        <th>LTIFR Employee</th>
                        <th>TIFR Employee</th>
                        <th>Multiplier Employee</th>
                        <th>LTIFR Contractor Onsite</th>
                        <th>LTIFR Contractor Offsite</th>
                        <th>LTIFR All</th>
                        <th>TIFR Contractor Onsite</th>
                        <th>TIFR Contractor Offsite</th>
                        <th>TIFR All</th>
                        <th>Multiplier Contractor Onsite</th>
                        <th>Multiplier Contractor Offsite</th>
                        <th>Multiplier All</th>
                    </tr>
                </thead>
                <tbody id="tb_main">


                </tbody>
                 
         
        </table>
       </div>
        <br />
        <br />

            <hr class="hr-line-solid">
        <br />
       
        <div class="row">
        <div class="col-md-12">
            <div class="pull-right">              
                  <asp:Button ID="btSaveSub"  runat="server" Text="<%$ Resources:Main, btsave %>"  CssClass="btn btn-primary" OnClientClick="return saveTableSub();"/>
            </div>
            
        </div>
        </div>
            <br />
             <br />
            <div style="overflow-x:scroll;">
                     <table id="sub_table" class="table table-bordered table-hover" style="width:100%;">
                         <thead>
                            <tr>
                        
                                <th><%= Resources.Incident.lbfucntion %></th>
                                <th><%= Resources.Incident.lbdepartment %></th>
                                <th>LTIFR Employee</th>
                                <th>TIFR Employee</th>
                                <th>Multiplier Employee</th>
                                <th>LTIFR Contractor Onsite</th>
                                <th>LTIFR Contractor Offsite</th>
                                <th>LTIFR All</th>
                                <th>TIFR Contractor Onsite</th>
                                <th>TIFR Contractor Offsite</th>
                                <th>TIFR All</th>                    
                                <th>Multiplier Contractor Onsite</th>
                                <th>Multiplier Contractor Offsite</th>
                                <th>Multiplier All</th>
                            </tr>
                        </thead>
                        <tbody id="tb_sub">


                        </tbody>
                 
         
                </table>
                </div>

        </div>
    </div>

 
</asp:Content>
