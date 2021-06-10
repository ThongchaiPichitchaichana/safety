<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="workhour2.aspx.cs" Inherits="safetys4.workhour2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


        
    
<style type="text/css">

</style>
<script>

    var current_year = '<%= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year %>';
    var select_function_id = "";
    $(document).ready(function () {

        
        setMonth();
       


    });

    function saveTable()
    {
        showLoading();
        var rows = $('#main_table').find('tbody').find('tr');
        for (var i = 0; i < rows.length; i++)
        {

            var site_id = $(rows[i]).find('td:eq(0)').html();
            
            var employee = $("#" + site_id + "employee").val();
            var contractor_onsite = $("#" + site_id + "contractor_onsite").val();
            var contractor_offsite = $("#" + site_id + "contractor_offsite").val();
          
            var ddl_year_id = $('#MainContent_ddyear').val();
            var ddl_month_id = $('#MainContent_ddmonth').val();
            $.ajax({
                type: "POST",
                data: {
                    employee: employee,
                    contractor_onsite: contractor_onsite,
                    contractor_offsite: contractor_offsite,
                    site_id: site_id,
                    month:ddl_month_id,
                    year: ddl_year_id,
                    lang: lang

                },
                url: 'Actionevent.asmx/createWorkhourMainSrilanka',
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
        var ddl_month_id = $('#MainContent_ddmonth').val();
        var ddl_year_id = $('#MainContent_ddyear').val();


        $.ajax({
            type: "POST",
            data: { month: ddl_month_id, year: ddl_year_id, lang: lang },
            url: 'Datatablelist.asmx/getListWorkhourSrilanka',
            dataType: 'json',
            success: function (json) {
                // alert("test");
                $("#tb_main").html(json.data);
               
            }
        });

    }


    function setYear(current_year) {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getWorkhourYearSrilanka',
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


    function setMonth()
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getMonth',
            dataType: 'json',
            success: function (json) {

              
                var $el = $("#MainContent_ddmonth");
                $el.empty(); // remove old options

                //$el.append($("<option></option>")
                //           .attr("value", "2016").text("2559"));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                setYear(current_year);
            }
        });



    }



</script>

         <div class="row">
             <h2><b>Work hour</b></h2>
             <br />
         <table class="form-filter">
           
             <tr>
                 <td>
                     <label class="control-label"><%= Resources.Hazard.lbmonth %></label></td>
                 <td> 
                      
                        <select id="ddmonth" class="form-control"  runat="server"> </select>
                 </td>

                 <td><label class="control-label"><%= Resources.Hazard.lbyear %></label></td>
                 <td>
                      
                        <select id="ddyear" class="form-control"  runat="server">
                       
                        </select>
                 </td>
                 <td>
                         <asp:Button ID="btSearch"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterTable();" CssClass="btn btn-primary"/>
                        
                 </td>
             </tr>

         </table>
       
       
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
                        <th>Employee</th>
                        <th>Contractor Onsite</th>
                        <th>Contractor Offsite </th>
                       
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
