<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="workhour.aspx.cs" Inherits="safetys4.workhour" %>

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
           
            var function_id = $(rows[i]).find('td:eq(1)').html();
            var employee = $("#" + function_id + "employee").val();
            var contractor_onsite = $("#" + function_id + "contractor_onsite").val();
            var contractor_offsite = $("#" + function_id + "contractor_offsite").val();
          
            var ddl_year_id = $('#MainContent_ddyear').val();
            var ddl_month_id = $('#MainContent_ddmonth').val();
            $.ajax({
                type: "POST",
                data: {
                    employee: employee,
                    contractor_onsite: contractor_onsite,
                    contractor_offsite: contractor_offsite,
                    function_id: function_id,
                    month:ddl_month_id,
                    year: ddl_year_id,
                    lang: lang

                },
                url: 'Actionevent.asmx/createWorkhourMain',
                dataType: 'json',
                success: function (result) {
                  
                       // closeLoading();
                       // searchTable();
                    
                   

                }
            });

            if (i == (rows.length - 1))
            {
                setTimeout(function () { searchTable(); }, 2000);
               
            }

        }
        return false;
    }


    function saveTableSub()
    {
        showLoading();
        var rows = $('#sub_table').find('tbody').find('tr');
        for (var i = 0; i < rows.length; i++)
        {
            var division_id = $(rows[i]).find('td:eq(1)').html();
            var employee = $("#" + division_id + "employee").val();
            var contractor_onsite = $("#" + division_id + "contractor_onsite").val();
            var contractor_offsite = $("#" + division_id + "contractor_offsite").val();
            var training_hour = $("#" + division_id + "training_hour").val();
           
            var ddl_year_id = $('#MainContent_ddyear').val();
            var ddl_month_id = $('#MainContent_ddmonth').val();
            var function_id = "";
            if(i==(rows.length-1))
            {
                function_id = select_function_id;
            }
            $.ajax({
                type: "POST",
                data: {
                    employee: employee,
                    contractor_onsite: contractor_onsite,
                    contractor_offsite: contractor_offsite,
                    training_hour:training_hour,
                    function_id: function_id,
                    division_id: division_id,
                    month:ddl_month_id,
                    year: ddl_year_id,
                    lang: lang

                },
                url: 'Actionevent.asmx/createWorkhourSub',
                dataType: 'json',
                success: function (result) {

                    //searchTable();

                   // setTimeout(function () { closeLoading(); }, 1000);

                }
            });


            if (i == (rows.length - 1)) {

                setTimeout(function () { searchTable(); }, 2000);

            }

        }
        return false;
    }

    function filterTable() {

        showLoading();
        //setTimeout(function () { closeLoading(); }, 1000);
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
            url: 'Datatablelist.asmx/getListWorkhour',
            dataType: 'json',
            success: function (json) {
                // alert("test");
                $("#tb_main").html(json.data);
                callWorkhourByFunction(select_function_id);

            }
        });

    }


    function setYear(current_year) {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getWorkhourYear',
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

    function getWorkhourSub(function_id)
    {
        showLoading();
        callWorkhourByFunction(function_id);
        return false;
    }


    function callWorkhourByFunction(function_id)
    {
        select_function_id = function_id;
        var ddl_year_id = $('#MainContent_ddyear').val();
        var ddl_month_id = $('#MainContent_ddmonth').val();

        $.ajax({
            type: "POST",
            data: { month:ddl_month_id,function_id: function_id, year: ddl_year_id, lang: lang },
            url: 'Datatablelist.asmx/getListWorkhourSub',
            dataType: 'json',
            success: function (json) {
                // alert("test");
                $("#tb_sub").html(json.data);

                setTimeout(function () { closeLoading(); }, 5000);
            }
        });


        return false;
    }



</script>

         <div class="row">
             <h2><b>Work and Training Hour</b></h2>
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
                  <%--  <asp:Button ID="btSave"  runat="server" Text="<%$ Resources:Main, btsave %>"  CssClass="btn btn-primary" OnClientClick="return saveTable();"/>--%>
          

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
                        <th><%= Resources.Incident.lbCompany %></th>
                        <th><%= Resources.Incident.lbfucntion %></th>
                        <th>Employee</th>
                        <th>Contractor Onsite</th>
                        <th>Contractor Offsite </th>
                        <th>Training Hour </th>
                       
                    </tr>
                </thead>
                <tbody id="tb_main">


                </tbody>
                 
         
        </table>
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
             <table id="sub_table" class="table table-bordered table-hover">
                 <thead>
                    <tr>
                        <th><%= Resources.Incident.lbdepartment %></th>
                        <th><%= Resources.Incident.lbdivision %></th>
                        <th>Employee</th>
                        <th>Contractor Onsite</th>
                        <th>Contractor Offsite </th>
                        <th>Training Hour </th>
                      
                    </tr>
                </thead>
                <tbody id="tb_sub">


                </tbody>
                 
         
        </table>

        </div>
    </div>




</asp:Content>
