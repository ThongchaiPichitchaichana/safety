<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="dashboardsot.aspx.cs" Inherits="safetys4.dashboardsot" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">

<script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker-custom.js"></script>
<script type="text/javascript" src="template/js/plugins/datepicker/locales/bootstrap-datepicker.th.js"></script>      
<script src="template/js/plugins/jquery.jqplot/jquery.jqplot.min.js"></script>

<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.barRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.categoryAxisRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.pointLabels.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.canvasTextRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.canvasAxisLabelRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.canvasAxisTickRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.enhancedLegendRenderer.js"></script>
    

<link href="template/js/plugins/jquery.jqplot/jquery.jqplot.min.css" rel="stylesheet">
<style>


    .jqplot-point-label{
        font-size:1em;
        font-weight:bold;
    }

</style>
<script>
    var area_id_list = [];

    var minWidth = $(window).width() - 200;
    var year = '<%= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year %>'; 

    $(document).ready(function () {


        <%
                
                if (Session["lang"] != null)         
                {
                    if (Session["lang"] =="th")
                    {                  
                    %>
                        $('#data_start_date .input-group.date').datepicker({
                            todayBtn: "linked",
                            keyboardNavigation: false,
                            forceParse: false,
                            autoclose: true,
                            format: "dd/mm/yyyy",
                            language: 'th',
                            thaiyear: true
                        }).datepicker("setDate", "01/01/" + year);

                        $('#data_end_date .input-group.date').datepicker({
                            todayBtn: "linked",
                            keyboardNavigation: false,
                            forceParse: false,
                            autoclose: true,
                            format: "dd/mm/yyyy",
                            language: 'th',
                            thaiyear: true
                        }).datepicker("setDate", "31/12/" + year);
                    <%                      
                    }
                    else { 
       
                    %>

                        $('#data_start_date .input-group.date').datepicker({
                            todayBtn: "linked",
                            keyboardNavigation: false,
                            forceParse: false,
                            autoclose: true,
                            format: "dd/mm/yyyy",
                            language: 'en',
                            thaiyear: false
                        }).datepicker("setDate", "01/01/" + year);

                        $('#data_end_date .input-group.date').datepicker({
                            todayBtn: "linked",
                            keyboardNavigation: false,
                            forceParse: false,
                            autoclose: true,
                            format: "dd/mm/yyyy",
                            language: 'en',
                            thaiyear: false
                        }).datepicker("setDate", "31/12/" + year);
                                <%     
     
                                            }                 
                                        }
               
                            %>

      

      
        setCompany('', '', '','')

        $('#dashboard1').bind('jqplotDataClick',
                function (ev, seriesIndex, pointIndex, data) {
               

                    setDashboard1(area_id_list[pointIndex]);
                }
            );


   
    });






    function setDashboard1(area_id)
    {
        var company_id = $('#MainContent_ddcompany').val();
        var function_id = $("#MainContent_ddfunction").val();
        var department_id = $("#MainContent_dddepartment").val();
        var division_id = $("#MainContent_dddivision").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();
        var mnglevel = $("#MainContent_ddmanagelevel").val();


        $.ajax({
            type: "GET",
            data: {
                company_id: company_id,
                function_id: function_id,
                department_id: department_id,
                division_id:division_id,
                area_id: area_id,
                mnglevel: mnglevel,
                date_start: date_start,
                date_end: date_end,
                lang: lang
            },
            url: 'Dashboard.asmx/getDashboardSOT1',
            dataType: 'json',
            success: function (json) {


               
                    var data1 = [];
                    var label = [];
                    var data_all = [];
                    area_id_list = [];
                    var count = 0;

                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.sot);

                        data_all.push(key.sot);
                        area_id_list.push(key.area_id);
                        count++;

                    });

                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);



                    var width = count * 50;
                    if (minWidth > width) {
                        $('#dashboard1').width(minWidth);

                    } else {
                        $('#dashboard1').width(width);
                    }


                    $.jqplot.config.enablePlugins = true;


                    plot1 = $.jqplot('dashboard1', [data1], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Safety Observation Tours </b>',
                        seriesColors: ['#1067ee'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50 },
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: { show: true },
                            rendererOptions: {
                                barPadding: 5,
                                barMargin: 30,
                                barWidth:15

                            },


                        },
                        axesDefaults: {
                            tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                            tickOptions: {
                                //fontFamily: 'Georgia',
                                //fontSize: '14pt',
                                //angle: -30
                            }
                        },
                        axes: {
                            xaxis: {
                                // tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                                renderer: $.jqplot.CategoryAxisRenderer,
                                ticks: label,
                                tickOptions: {
                                    angle: -30,
                                    fontFamily: 'open sans',
                                    fontSize: '10pt',
                                    // fontSize: '14pt'
                                }

                            },
                            yaxis: {

                                tickRenderer: $.jqplot.CanvasAxisTickRenderer,

                                tickOptions: {

                                    fontFamily: 'open sans',
                                    fontSize: '10pt',
                                    formatString: '%d',

                                },
                                ticks: tick_y,


                            },

                        },
                      

                        highlighter: { show: false },
                        legend: {
                            renderer: $.jqplot.EnhancedLegendRenderer,
                            show: true,
                            rendererOptions: {
                                numberRows: 1
                            },
                            placement: 'outside',
                            location: 's',
                            marginTop: '200px',
                            fontSize: '13pt'
                        }, series: [
                            { label: 'SOT report' }
                           

                        ],

                    });

                   // plot1.redraw();
                    plot1.replot({ resetAxes: true });

                } 


            
        });

    }

    function filterAllSot()
    {
        showLoading();
        setTimeout(function () { closeLoading(); }, 3000);

        setDashboard1("");
    

        return false;
    }





    function generateY(max_value)
    {
        var y = [];

        if (max_value > 20) {
            var step1 = max_value / 10;//จะแสดง 10 ช่อง
            if (step1 < 1) {
                step1 = Math.ceil(step1);//ปัดขึ้นหมด
            }

            var step2 = step1 / 5;
            if (step2 < 1) {
                step2 = Math.ceil(step2);//ปัดขึ้นหมด
            } else {
                step2 = Math.floor(step2);//ปัดลงหมด

            }

            var step3 = step2 * 5;
            var i_end = 0;
            for (var i = 0; i < max_value; i += step3) {
                y.push(i);
                i_end = i;
            }
            i_end = i_end + step3;
            y.push(i_end);
            y.push(i_end + step3);
            y.push(i_end + step3 + step3);

        } else {


            if (max_value > 5 && max_value <= 10) {
                y = [0,1, 2, 3, 4, 5, 6, 7, 8, 9, 10,11,12];

            } else if (max_value <= 5) {

                y = [0, 1, 2, 3, 4, 5,6,7,8,9,10];

            } else if (max_value > 10) {

                y = [0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20,22,24];
            }
        }

        return y;

    }








    function setCompany(company_id, function_id, department_id, division_id)
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getCompany',
            dataType: 'json',
            success: function (json) {

                var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_ddcompany");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(all));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                $('#MainContent_ddcompany').val(company_id);
                setFunction(company_id, function_id, department_id);

            }
        });

    }


    function setFunction(company_id, function_id, department_id, division_id)
    {
        $.ajax({
            type: "POST",
            data: { company: company_id, lang: lang },
            url: 'Masterdata.asmx/getFuctionByCompany',
            dataType: 'json',
            success: function (json) {

                var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_ddfunction");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(all));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });

                $('#MainContent_ddfunction').val(function_id);
                setDepartment(function_id, department_id, division_id);
            }
        });



    }



    function setDepartment(function_id, department_id, division_id)
    {

        $.ajax({
            type: "POST",
            data: { function: function_id, lang: lang },
            url: 'Masterdata.asmx/getDepartmentbyFunction',
            dataType: 'json',
            success: function (json) {

                var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_dddepartment");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(all));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


                $('#MainContent_dddepartment').val(department_id);
                setDivision(department_id, division_id);
               
            }

        });



    }


    function setDivision(department_id, division_id)
    {

        $.ajax({
            type: "POST",
            data: { department: department_id, lang: lang },
            url: 'Masterdata.asmx/getDivisionbyDepartment',
            dataType: 'json',
            success: function (json) {


                var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_dddivision");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(all));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });



                $('#MainContent_dddivision').val(division_id);
                setDashboard1("");

            }
        });



    }









    function changCompany()
    {
        var ddl_company_id = $('#MainContent_ddcompany').val();

        $.ajax({
            type: "POST",
            data: { company: ddl_company_id, lang: lang },
            url: 'Masterdata.asmx/getFuctionByCompany',
            dataType: 'json',
            success: function (json) {

                var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_ddfunction");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(all));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


            }
        });



    }


    function changFunction()
    {
        var ddl_function_id = $('#MainContent_ddfunction').val();

        $.ajax({
            type: "POST",
            data: { function: ddl_function_id, lang: lang },
            url: 'Masterdata.asmx/getDepartmentbyFunction',
            dataType: 'json',
            success: function (json) {

                var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_dddepartment");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(all));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


            }
        });



    }




    function changDepartment()
    {
        var ddl_department_id = $('#MainContent_dddepartment').val();

        $.ajax({
            type: "POST",
            data: { department: ddl_department_id, lang: lang },
            url: 'Masterdata.asmx/getDivisionbyDepartment',
            dataType: 'json',
            success: function (json) {

                var all = '<%= Resources.Main.all %>';
                var $el = $("#MainContent_dddivision");
                $el.empty(); // remove old options

                $el.append($("<option></option>")
                           .attr("value", "").text(all));
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


            }
        });



    }




</script>





    
           <div class="row" id="filter">
                 <div class="row">
                        <div class="col-md-3">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.lbCompany %></label>                         

                            <select id="ddcompany" name="ddcompany" class="form-control" onchange="changCompany();" runat="server">
                       
                            </select>
                                     
                        </div>
                  </div>
                <div class="col-md-4">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.lbfucntion %></label>
                                    
                            <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server">
                       
                            </select>
                                  
                        </div>
                    </div>

                   <div class="col-md-3">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.lbdepartment %></label>                                              
                                    
                            <select id="dddepartment" class="form-control" runat="server" onchange="changDepartment();">
                       
                            </select>
                                        
                        </div>
                    </div>


                       <div class="col-md-2">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.lbdivision %></label>
                                        
                                <select id="dddivision" class="form-control" runat="server">
                       
                            </select>
                        </div>
                    </div>
                    


                   </div>
               <div class="row">
                   
                     <div class="col-md-3">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Sot.lbmng_level %></label>                         

                            <select id="ddmanagelevel" name="ddmanagelevel" class="form-control" runat="server">
                                 <option value="FML">FML</option>
                                 <option value="MML">MML</option>
                                 <option value="NML">NML</option>
                                 <option value="SML">SML</option>
                                 <option value="TML">TML</option>
                            </select>
                                     
                        </div>
                    </div>
              
                <div class="col-md-3">
                    <div class="form-group">
                          <label class="control-label"><%= Resources.Incident.date %></label>       
                  
                                      
                      <div id="data_start_date" class="form-group">
                                      
                            <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtstart_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                            </div>
                                        
                    </div>
                                     
                    </div>
                </div>
               

                <div class="col-md-3">
                    <div class="form-group">
                         <label class="control-label"><%= Resources.Incident.to %></label>
                    
                        <div id="data_end_date" class="form-group">
                                      
                            <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtend_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                            </div>
                                        
                    </div>
                                          
                    </div>
                </div>
             

               <div class="col-md-2">
                    <div class="form-group">
                          <label class="control-label"></label> 
                          <div class="form-group">
                             
                        <%
                            ArrayList per2 = Session["permission"] as ArrayList;
                            if (per2.IndexOf("dashboard search") > -1)         
                           {                            
       
                          %>
                                <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterAllSot();" CssClass="btn btn-primary"/>
                                                     <%                      
                            }
       
                          %>
                 
                                       
                                            
                        </div>
                    </div>
                </div>


               </div>
               
                              
       </div>
    <br />

    <div class="row">
        <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:135px;">
            
            <br />
            <div id="dashboard1" style="height:500px;"></div>
        </div>
    </div>
    <br />
    <br />
    <br />
     <br />
    <br />
 


</asp:Content>
