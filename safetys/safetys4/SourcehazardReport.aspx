<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="SourcehazardReport.aspx.cs" Inherits="safetys4.SourcehazardReport" %>
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

<style type="text/css">

.wrapper-content {
        margin-left: 300px;
    }


</style>
<script>
    var dialog;
    var area_id_list4 = [];

    var minWidth = $(window).width() - 200;
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
                                    });

                                    $('#data_end_date .input-group.date').datepicker({
                                        todayBtn: "linked",
                                        keyboardNavigation: false,
                                        forceParse: false,
                                        autoclose: true,
                                        format: "dd/mm/yyyy",
                                        language: 'th',
                                        thaiyear: true
                                    });
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
                                    });

                                    $('#data_end_date .input-group.date').datepicker({
                                        todayBtn: "linked",
                                        keyboardNavigation: false,
                                        forceParse: false,
                                        autoclose: true,
                                        format: "dd/mm/yyyy",
                                        language: 'en',
                                        thaiyear: false
                                    });
                                            <%     
     
                                                        }                 
                                                    }
               
                                        %>


     dialog = $("#download_chart").dialog({
         autoOpen: false,
         height: 600,
         width: 1200,
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
           
             $("#download_chart").css('overflow-x', 'hidden');
         },
         modal: true,
     });


     
     setDashboard4("");

     
     $('#dashboard4').bind('jqplotDataClick',
            function (ev, seriesIndex, pointIndex, data) {

                setDashboard4(area_id_list4[pointIndex]);
            }
        );

    
     
    
     
 });

    function downloadCanvas(link, canvasId, filename)
    {
        link.href = document.getElementById(canvasId).toDataURL();
        link.download = filename;
    }



    function filterHazard() {

        showLoading();
        setTimeout(function () { closeLoading(); }, 3000);
        setDashboard4("");


        return false;
    }



    function setDashboard4(area_id)
    {

        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();


        $.ajax({
            type: "GET",
            data: {
                area_id: area_id,
                date_start: date_start,
                date_end: date_end,
                lang: lang
            },
            url: 'Dashboard.asmx/getDashboardHazard4',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "") {
                    var data1 = [];
                    var data2 = [];
                    var data3 = [];
                    var data4 = [];
                    var data5 = [];
                    var data6 = [];
                    var data7 = [];
                    var data8 = [];
                    var label = [];

                    var data_all = [];
                    area_id_list4 = [];
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.vehicle_tracffic);
                        data2.push(key.fall_gravity);
                        data3.push(key.electricity);
                        data4.push(key.machine);
                        data5.push(key.chemical);
                        data6.push(key.fire_heat);
                        data7.push(key.slip_fall);
                        data8.push(key.animal);

                        data_all.push(key.vehicle_tracffic);
                        data_all.push(key.fall_gravity);
                        data_all.push(key.electricity);
                        data_all.push(key.machine);
                        data_all.push(key.chemical);
                        data_all.push(key.fire_heat);
                        data_all.push(key.slip_fall);
                        data_all.push(key.animal);

                        area_id_list4.push(key.area_id);
                        count++;

                    });

                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);


                    var width = count * 60;
                    if (minWidth > width) {
                        $('#dashboard4').width(minWidth);

                    } else {
                        $('#dashboard4').width(width);
                    }

                    $.jqplot.config.enablePlugins = true;


                    plot2 = $.jqplot('dashboard4', [data1, data2, data3, data4, data5, data6, data7, data8], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        stackSeries: true,

                        title: '<b>Hazard report by source of hazards</b>',
                        seriesColors: ['#3d81f8', '#a1370f', '#139309', '#850da5', '#25e6ec', '#faa635', '#84c7e7', '#f83d95'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50 },
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: {
                                show: true,
                                stackedValue: false,
                                location: 's'
                            },
                            rendererOptions: {
                                barPadding: 5,
                                barMargin: 30,
                                barWidth: 15
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
                                //ticks: tick_y,
                                // renderer: $.jqplot.CategoryAxisRenderer,
                                tickOptions: {
                                    // min: 0,
                                    //  numberTicks: 10,
                                    // max:100,
                                    //labelPosition: 'middle', 
                                    // angle:-30,
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
                             {
                                 label: 'Vehicle and traffic',
                                 pointLabels: {
                                     labels: data1
                                 }
                             },
                             {
                                 label: 'Fall/gravity',
                                 pointLabels: {
                                     labels: data2
                                 }
                             },

                             {
                                 label: 'Electricity',
                                 pointLabels: {
                                     labels: data3
                                 }
                             },
                             {
                                 label: 'Machine',
                                 pointLabels: {
                                     labels: data4
                                 }
                             },
                            {
                                label: 'Chemical',
                                pointLabels: {
                                    labels: data5
                                }
                            },
                            {
                                label: 'Fire/heat',
                                pointLabels: {
                                    labels: data6
                                }
                            },
                            {
                                label: 'Slip and fall',
                                pointLabels: {
                                    labels: data7
                                }
                            },
                            {
                                label: 'Animal',
                                pointLabels: {
                                    labels: data8
                                }
                            },
                          

                        ],

                    });

                    //  plot2.redraw();
                    plot2.replot({ resetAxes: true });

                    var imgData = $("#dashboard4").jqplotToImageStr({}); // given the div id of your plot, get the img data
                    var imgElem = $("#image").attr("src", imgData); // create an img and add the data to it



                } else {


                    window.open(json.redirect, '_blank');



                }


            }
        });

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
                y = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

            } else if (max_value <= 5) {

                y = [0, 1, 2, 3, 4, 5];

            } else if (max_value > 10) {

                y = [0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20];
            }
        }

        return y;

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


    function openChart()
    {
        dialog.dialog("open");
     
    }






</script>





               <div class="row" id="filter">
                <%--<div class="col-md-3">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.lbfucntion %></label>
                                    
                            <select id="ddfunction" class="form-control" onchange="changFunction();" runat="server">
                       
                            </select>
                                  
                        </div>
                    </div>

                   <div class="col-md-2">
                        <div class="form-group">
                        <label class="control-label"><%= Resources.Incident.lbdepartment %></label>                                              
                                    
                            <select id="dddepartment" class="form-control" runat="server">
                       
                            </select>
                                        
                        </div>
                    </div>
               --%>
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
             

               <div class="col-md-1">
                    <div class="form-group">
                        <br />
                               
                        <%--                        <%
                            ArrayList per2 = Session["permission"] as ArrayList;
                            if (per2.IndexOf("report search") > -1)         
                           {                            
       
                          %>--%>
                                   <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterHazard();" CssClass="btn btn-primary"/>
                          <%--  <%                      
                            }
       
                          %>     --%>       
                                            
                        
                    </div>
       
       </div>
   </div>

   <div class="row">
        <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:135px;">
            <br />
            <div id="dashboard4" style="height:500px;padding-left:50px;"></div>
        </div>
    </div>


    <div id="download_chart">
        <div class="row">
             <img id="image"/>
           <%-- <canvas id="canvas"></canvas>--%>
        </div>

    </div>
   <br />
    <br />
    <%--<a id="download" class="btn btn-primary"><i class="fa fa-download"></i></a>--%>
   <button type="button" id="btOpenChart" class="btn btn-primary"  onclick="openChart();"><i class="fa fa-download" aria-hidden="true"></i></button>

</asp:Content>
