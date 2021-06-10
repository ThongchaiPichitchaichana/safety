<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="dashboardhazard.aspx.cs" Inherits="safetys4.dashboardhazard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">

  
<script src="template/js/plugins/jquery.jqplot/jquery.jqplot.min.js"></script>

<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.barRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.categoryAxisRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.pointLabels.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.canvasTextRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.canvasAxisLabelRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.canvasAxisTickRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.enhancedLegendRenderer.js"></script>
    

<link href="template/js/plugins/jquery.jqplot/jquery.jqplot.min.css" rel="stylesheet">
<script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker-custom.js"></script>
 <%
                
if (Session["lang"] != null)         
{
    if (Session["lang"] =="th")
    {                  
 %>
<script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker-custom.js"></script>

 <%                      
    }
else { 
       
%>

    <script type="text/javascript" src="template/js/plugins/datepicker/bootstrap-datepicker.js"></script>

<%                      
   }  
 
 }    
%>
<script type="text/javascript" src="template/js/plugins/datepicker/locales/bootstrap-datepicker.th.js"></script>


<style>


    .jqplot-point-label{
        font-size:1em;
        font-weight:bold;
    }

.tabs-container .nav-tabs > li.active > a, .tabs-container .nav-tabs > li.active > a:hover, .tabs-container .nav-tabs > li.active > a:focus {
    background-color: #e0e0d1;
    color: black;
}


.tabs-container .panel-body {
    background-color: #e6e6e8 !important;
}

.tabs-container .panel-body {
    color: black !important;
}
    /*#dashboard4 .jqplot-point-label {
  border: 1.5px solid #aaaaaa;
  padding: 10px 30px;
  background-color: #eeccdd;
}*/

</style>
<script>
    var area_id_list = [];
    var area_id_list2 = [];
    var area_id_list3 = [];
    var area_id_list4 = [];
    var area_id_list5 = [];
    var area_id_list6 = [];
    var area_id_list7 = [];
    var area_id_list8 = [];
    
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

      

        setDashboard1("");
        setDashboard2("");
        setDashboard3("");

         <%
                
        if (Session["country"].ToString() =="srilanka")         
        {

        %>
            setDashboard5("");
            setDashboard6("");
            setDashboard7("");
            setDashboard8("");

         <%     
            }
               
         %>

        $('#dashboard1').bind('jqplotDataClick',
                function (ev, seriesIndex, pointIndex, data) {
               

                    setDashboard1(area_id_list[pointIndex]);
                }
            );


        $('#dashboard2').bind('jqplotDataClick',
                function (ev, seriesIndex, pointIndex, data) {
               

                    setDashboard2(area_id_list2[pointIndex]);
                }
            );

        $('#dashboard3').bind('jqplotDataClick',
                function (ev, seriesIndex, pointIndex, data) {
                   
                    setDashboard3(area_id_list3[pointIndex]);
                }
            );

        //$('#dashboard4').bind('jqplotDataClick',
        //       function (ev, seriesIndex, pointIndex, data) {

        //           setDashboard4(area_id_list4[pointIndex]);
        //       }
        //   );

        $('#dashboard5').bind('jqplotDataClick',
               function (ev, seriesIndex, pointIndex, data) {

                   setDashboard5(area_id_list5[pointIndex]);
               }
           );

        $('#dashboard6').bind('jqplotDataClick',
              function (ev, seriesIndex, pointIndex, data) {

                  setDashboard6(area_id_list6[pointIndex]);
              }
          );

        $('#dashboard7').bind('jqplotDataClick',
             function (ev, seriesIndex, pointIndex, data) {

                 setDashboard7(area_id_list7[pointIndex]);
             }
         );

        $('#dashboard8').bind('jqplotDataClick',
           function (ev, seriesIndex, pointIndex, data) {

               setDashboard8(area_id_list8[pointIndex]);
           }
       );
    });



    function setDashboard1(area_id)
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
            url: 'Dashboard.asmx/getDashboardHazard1',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "") {
                    var data1 = [];
                    var data2 = [];
                    var label = [];

                    var data_all = [];
                    area_id_list = [];
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.count);
                        data2.push(key.condition);

                        data_all.push(key.count);
                        data_all.push(key.condition);
                        area_id_list.push(key.area_id);
                        count++;

                    });
                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);


                    $.jqplot.config.enablePlugins = true;


                    plot1 = $.jqplot('dashboard1', [data1, data2], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Number of Hazard reported VS Number of Hazard recorded</b>',
                        seriesColors: ['#00749F', '#C7754C'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50 },
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: { show: true },
                            rendererOptions: {
                                barPadding: 20,
                                barMargin: 30,
                                barWidth: 15
                            },


                        },
                        axesDefaults: {
                            tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                            tickOptions: {
                                //formatString: '%d'
                                //fontFamily: 'Georgia',
                                //fontSize: '14pt',
                                //angle: -30
                            },

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
                        cursor:{ 
                            show: true,
                            zoom:true, 
                            showTooltip:false
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
                            { label: 'Report' },
                            { label: 'Record' },

                        ],

                    });

                    //

                    var width = count * 70;
                    if (minWidth > width) {
                        $('#dashboard1').width(minWidth);

                    } else {
                        $('#dashboard1').width(width);
                    }

                    // plot1.redraw();
                    plot1.replot({ resetAxes: true });

                } else {


                    window.open(json.redirect, '_blank');



                }


            }
        });

    }





    //function setDashboard1(area_id)
    //{

    //    var date_start = $("#MainContent_txtstart_date").val();
    //    var date_end = $("#MainContent_txtend_date").val();


    //    $.ajax({
    //        type: "GET",
    //        data: {
    //            area_id: area_id,
    //            date_start: date_start,
    //            date_end: date_end,
    //            lang: lang
    //        },
    //        url: 'Dashboard.asmx/getDashboardHazard1',
    //        dataType: 'json',
    //        success: function (json) {


    //            if (json.redirect == "") {
    //                var data1 = [];
    //                var label = [];
    //                var data_all = [];
    //                area_id_list = [];
    //                var count = 0;

    //                $.each(json.result, function (value, key) {
    //                    label.push(key.label);
    //                    data1.push(key.count);

    //                    data_all.push(key.count);
    //                    area_id_list.push(key.area_id);
    //                    count++;

    //                });

    //                var max = Math.max.apply(null, data_all);
    //                var tick_y = generateY(max);



    //                var width = count * 50;
    //                if (minWidth > width) {
    //                    $('#dashboard1').width(minWidth);

    //                } else {
    //                    $('#dashboard1').width(width);
    //                }


    //                $.jqplot.config.enablePlugins = true;


    //                plot1 = $.jqplot('dashboard1', [data1], {
    //                    // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
    //                    title: '<b>All number of Hazards</b>',
    //                    seriesColors: ['#1067ee'],
    //                    animate: !$.jqplot.use_excanvas,
    //                    gridPadding: { left: 50 },
    //                    seriesDefaults: {
    //                        renderer: $.jqplot.BarRenderer,
    //                        pointLabels: { show: true },
    //                        rendererOptions: {
    //                            barPadding: 5,
    //                            barMargin: 30,
    //                            barWidth:15

    //                        },


    //                    },
    //                    axesDefaults: {
    //                        tickRenderer: $.jqplot.CanvasAxisTickRenderer,
    //                        tickOptions: {
    //                            //fontFamily: 'Georgia',
    //                            //fontSize: '14pt',
    //                            //angle: -30
    //                        }
    //                    },
    //                    axes: {
    //                        xaxis: {
    //                            // tickRenderer: $.jqplot.CanvasAxisTickRenderer,
    //                            renderer: $.jqplot.CategoryAxisRenderer,
    //                            ticks: label,
    //                            tickOptions: {
    //                                angle: -30,
    //                                fontFamily: 'open sans',
    //                                fontSize: '10pt',
    //                                // fontSize: '14pt'
    //                            }

    //                        },
    //                        yaxis: {

    //                            tickRenderer: $.jqplot.CanvasAxisTickRenderer,

    //                            tickOptions: {

    //                                fontFamily: 'open sans',
    //                                fontSize: '10pt',
    //                                formatString: '%d',

    //                            },
    //                            ticks: tick_y,


    //                        },

    //                    },
                      

    //                    highlighter: { show: false },
    //                    legend: {
    //                        renderer: $.jqplot.EnhancedLegendRenderer,
    //                        show: true,
    //                        rendererOptions: {
    //                            numberRows: 1
    //                        },
    //                        placement: 'outside',
    //                        location: 's',
    //                        marginTop: '200px',
    //                        fontSize: '13pt'
    //                    }, series: [
    //                        { label: 'Hazard report' }
                           

    //                    ],

    //                });

    //               // plot1.redraw();
    //                plot1.replot({ resetAxes: true });

    //            } else {


    //                window.open(json.redirect, '_blank');



    //            }


    //        }
    //    });

    //}




    function setDashboard2(area_id) {

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
            url: 'Dashboard.asmx/getDashboardHazard2',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "") {
                    var data1 = [];
                    var label = [];
                    var data_all = [];

                    area_id_list2 = [];
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.count);

                        data_all.push(key.count);
                        area_id_list2.push(key.area_id);
                        count++;

                    });

                    // console.log(data1);
                    var width = count * 50;
                    if (minWidth > width) {
                        $('#dashboard2').width(minWidth);

                    } else {
                        $('#dashboard2').width(width);
                    }


                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);



                    $.jqplot.config.enablePlugins = true;


                    plot1 = $.jqplot('dashboard2', [data1], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Number of Hazard report only Medium & High level</b>',
                        seriesColors: ['#1067ee'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50 },
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: { show: true },
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
                            { label: 'Hazard report' }

                        ],

                    });

                    //plot1.redraw();
                    plot1.replot({ resetAxes: true });

                } else {


                    window.open(json.redirect, '_blank');



                }


            }
        });

    }

    function filterAllhazard()
    {
        showLoading();
        setTimeout(function () { closeLoading(); }, 3000);
        
         setDashboard1("");
         setDashboard2("");
         setDashboard3("");

         <%
                
        if (Session["country"].ToString() =="srilanka")         
        {

        %>
            setDashboard5("");
            setDashboard6("");
            setDashboard7("");
            setDashboard8("");

         <%     
            }
               
         %>
      
         return false;
    }


    function setDashboard3(area_id)
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
            url: 'Dashboard.asmx/getDashboardHazard3',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "") {
                    var data1 = [];
                    var data2 = [];
                    var data3 = [];
                    var label = [];

                    var data_all = [];
                    area_id_list3 = [];
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.onprocess);
                        data2.push(key.close);
                        data3.push(key.delay);

                        data_all.push(key.onprocess);
                        data_all.push(key.close);
                        data_all.push(key.delay);

                        area_id_list3.push(key.area_id);
                        count++;


                    });


                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);


                    var width = count * 90;
                    if (minWidth > width) {
                        $('#dashboard3').width(minWidth);

                    } else {
                        $('#dashboard3').width(width);
                    }

                    $.jqplot.config.enablePlugins = true;


                    plot = $.jqplot('dashboard3', [data1, data2, data3], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Action status of hazard report</b>',
                        seriesColors: ['#f9fc0b', '#22e657', '#f21308'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50 },
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: { show: true },
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
                            { label: 'On process' },
                            { label: 'Close' },
                            { label: 'Delay' },

                        ],

                    });

                    //plot.redraw();
                    plot.replot({ resetAxes: true });

                } else {


                    window.open(json.redirect, '_blank');



                }


            }
        });

    }



    function setDashboard5(area_id)
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
            url: 'Dashboard.asmx/getDashboardHazard5',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "") {
                    var data5 = [];
                    var label = [];
                    var data_all = [];
                    area_id_list5 = [];
                    var count = 0;

                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data5.push(key.count);

                        data_all.push(key.count);
                        area_id_list5.push(key.area_id);
                        count++;

                    });

                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);



                    var width = count * 50;
                    if (minWidth > width) {
                        $('#dashboard5').width(minWidth);

                    } else {
                        $('#dashboard5').width(width);
                    }


                    $.jqplot.config.enablePlugins = true;


                    plot1 = $.jqplot('dashboard5', [data5], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>All number of Hazards By Sites</b>',
                        seriesColors: ['#1067ee'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50 },
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: { show: true },
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
                            { label: 'Hazard report by sites' }


                        ],

                    });

                    // plot1.redraw();
                    plot1.replot({ resetAxes: true });

                } else {


                    window.open(json.redirect, '_blank');



                }


            }
        });

    }

    function setDashboard6(area_id)
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
            url: 'Dashboard.asmx/getDashboardHazard6',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "")
                {
                    var data1 = [];
                    var data2 = [];
                    var data3 = [];
                    var label = [];

                    var data_all = [];
                    area_id_list6 = [];
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.onprocess);
                        data2.push(key.close);
                        data3.push(key.reject);

                        data_all.push(key.onprocess);
                        data_all.push(key.close);
                        data_all.push(key.reject);

                        area_id_list6.push(key.area_id);
                        count++;


                    });


                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);


                    var width = count * 90;
                    if (minWidth > width) {
                        $('#dashboard6').width(minWidth);

                    } else {
                        $('#dashboard6').width(width);
                    }

                    $.jqplot.config.enablePlugins = true;


                    plot = $.jqplot('dashboard6', [data1, data2, data3], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Status of hazard report by sites</b>',
                        seriesColors: ['#f9fc0b', '#22e657', '#f21308'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50 },
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: { show: true },
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
                            { label: 'On process' },
                            { label: 'Close' },
                            { label: 'Reject' },

                        ],

                    });

                    //plot.redraw();
                    plot.replot({ resetAxes: true });

                }


            }
        });

    }
   

    function setDashboard7(area_id)
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
            url: 'Dashboard.asmx/getDashboardHazard7',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "") {
                    var data1 = [];
                    var data2 = [];
                    var data3 = [];
                    var label = [];

                    var data_all = [];
                    area_id_list7 = [];
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.onprocess);
                        data2.push(key.close);
                        data3.push(key.delay);

                        data_all.push(key.onprocess);
                        data_all.push(key.close);
                        data_all.push(key.delay);

                        area_id_list7.push(key.area_id);
                        count++;


                    });


                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);


                    var width = count * 90;
                    if (minWidth > width) {
                        $('#dashboard7').width(minWidth);

                    } else {
                        $('#dashboard7').width(width);
                    }

                    $.jqplot.config.enablePlugins = true;


                    plot = $.jqplot('dashboard7', [data1, data2, data3], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Action status of hazard report by sites</b>',
                        seriesColors: ['#f9fc0b', '#22e657', '#f21308'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50 },
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: { show: true },
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
                            { label: 'On process' },
                            { label: 'Close' },
                            { label: 'Delay' },

                        ],

                    });

                    //plot.redraw();
                    plot.replot({ resetAxes: true });

                } 


            }
        });

    }


    function setDashboard8(area_id)
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
            url: 'Dashboard.asmx/getDashboardHazard8',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "") {
                    var data8 = [];
                    var label = [];
                    var data_all = [];
                    area_id_list8 = [];
                    var count = 0;

                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data8.push(key.count);

                        data_all.push(key.count);
                        area_id_list8.push(key.area_id);
                        count++;

                    });

                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);



                    var width = count * 50;
                    if (minWidth > width) {
                        $('#dashboard8').width(minWidth);

                    } else {
                        $('#dashboard8').width(width);
                    }


                    $.jqplot.config.enablePlugins = true;


                    plot1 = $.jqplot('dashboard8', [data8], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>All number of Hazards from reporter By Sites</b>',
                        seriesColors: ['#1067ee'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50 },
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: { show: true },
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
                            { label: 'Hazard report by sites' }


                        ],

                    });

                    // plot1.redraw();
                    plot1.replot({ resetAxes: true });

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
                y = [0,1, 2, 3, 4, 5, 6, 7, 8, 9, 10,11,12];

            } else if (max_value <= 5) {

                y = [0, 1, 2, 3, 4, 5,6,7,8,9,10];

            } else if (max_value > 10) {

                y = [0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20,22,24];
            }
        }

        return y;

    }

</script>








    <div class="tabs-container">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="dashboardhazard.aspx" aria-expanded="true">1</a></li>
                            
                            <li class=""><a  href="dashboardhazard2.aspx" aria-expanded="false">Pending Work</a></li>
                        </ul>
                        <div class="tab-content">
                          
                            <div id="tab-1" class="tab-pane active">
                                <div class="panel-body" style="background-color:white;">
                                                          
                                    
    
           <div class="row" id="filter">
               <div class="col-md-1">
                    <div class="form-group">
                   <label class="control-label"><%= Resources.Incident.date %></label>       
                                    
                                    
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="form-group">
                                      
                      <div id="data_start_date" class="form-group">
                                      
                            <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtstart_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                            </div>
                                        
                    </div>
                                     
                    </div>
                </div>
                <div class="col-md-1">
                    <div class="form-group">
                    <label class="control-label"><%= Resources.Incident.to %></label>
                                    
                                    
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="form-group">
                        <div id="data_end_date" class="form-group">
                                      
                            <div class="input-group date">
                                <input class="form-control" value="" type="text" id="txtend_date" runat="server"><span class="input-group-addon"><i class="fa fa-calendar"></i></span>
                                        
                            </div>
                                        
                    </div>
                                          
                    </div>
                </div>
             

               <div class="col-md-2">
                    <div class="form-group">
                                
                    <div class="form-inline">
                               
                        <%
                            ArrayList per2 = Session["permission"] as ArrayList;
                            if (per2.IndexOf("dashboard search") > -1)         
                           {                            
       
                          %>
                                <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterAllhazard();" CssClass="btn btn-primary"/>
                                                     <%                      
                            }
       
                          %>
                 
                                       
                                            
                        </div>
                    </div>
                </div>
                              
       </div>
    <br />

    <div class="row">
        <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:135px;">
            <span class="label label-info" style="font-size:11px;"><%= Resources.Main.lbinfodashboard %></span>
             <br />
            <br />
            <div id="dashboard1" style="height:500px;"></div>
        </div>
    </div>
    <br />
    <br />
    <br />
     <br />
    <br />
    <br />
     <br />
    <br />
    <br />
    <div class="row">
        <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:135px;">
            <span class="label label-info" style="font-size:11px;"><%= Resources.Main.lbinfodashboard %></span>
             <br />
            <br />
            <div id="dashboard2" style="height:500px;"></div>
        </div>
    </div>


    <br />
    <br />
    <br />
     <br />
    <br />
    <br />
     <br />
    <br />
    <br />


     <div class="row">
        <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:135px;">
            <span class="label label-info" style="font-size:11px;"><%= Resources.Main.lbinfodashboard %></span>
             <br />
            <br />
            <div id="dashboard3" style="height:500px;"></div>
        </div>
    </div>

    <br />
    <br />
    <br />
     <br />
    <br />
    <br />
     <br />
    <br />
    <br />



     <%
                
        if (Session["country"].ToString() =="srilanka")         
        {

            %>
             <div class="row">
                <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:155px;">
                    <br />
                    <div id="dashboard5" style="height:500px;"></div>
                </div>
            </div>

            <br />
            <br />
            <br />
             <br />
            <br />
            <br />
             <br />
            <br />
            <br />
   

     <%
        }

      %>



        <%
                
        if (Session["country"].ToString() =="srilanka")         
        {

            %>
             <div class="row">
                <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:155px;">
                    <br />
                    <div id="dashboard6" style="height:500px;"></div>
                </div>
            </div>

            <br />
            <br />
            <br />
             <br />
            <br />
            <br />
             <br />
            <br />
            <br />
   

     <%
        }

      %>



     <%
                
        if (Session["country"].ToString() =="srilanka")         
        {

            %>
             <div class="row">
                <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:155px;">
                    <br />
                    <div id="dashboard7" style="height:500px;"></div>
                </div>
            </div>

            <br />
            <br />
            <br />
             <br />
            <br />
            <br />
             <br />
            <br />
            <br />
   

     <%
        }

      %>



    <%
                
        if (Session["country"].ToString() =="srilanka")         
        {

            %>
             <div class="row">
                <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:155px;">
                    <br />
                    <div id="dashboard8" style="height:500px;"></div>
                </div>
            </div>

            <br />
            <br />
            <br />
             <br />
            <br />
            <br />
             <br />
            <br />
            <br />
   

     <%
        }

      %>
  






                                    
                                            
                                </div>
                            </div>


                              <div id="tab-2" class="tab-pane" >
                                <div class="panel-body">
                                   
           


                                </div>
                            </div>

                        </div>


                    </div>
</asp:Content>
