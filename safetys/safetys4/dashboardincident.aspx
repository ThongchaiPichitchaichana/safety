<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="dashboardincident.aspx.cs" Inherits="safetys4.dashboardincident" %>
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
                                    }).datepicker("setDate", "01/01/"+year);

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
        setDashboard4("");

     

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
                  //  console.log(area_id_list3);
                    setDashboard3(area_id_list3[pointIndex]);
                }
            );

        $('#dashboard4').bind('jqplotDataClick',
               function (ev, seriesIndex, pointIndex, data) {

                   setDashboard4(area_id_list4[pointIndex]);
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
                date_end : date_end,
                lang: lang
            },
            url: 'Dashboard.asmx/getDashboardIncident1',
            dataType: 'json',
            success: function (json) {
               

                if(json.redirect=="")
                {
                    var data1 = [];
                    var data2 = [];
                    var label = [];

                    var data_all = [];
                    area_id_list = [];
                    var count = 0;
              
                    
                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.all);
                        data2.push(key.condition);

                        data_all.push(key.all);
                        data_all.push(key.condition);
                        area_id_list.push(key.area_id);
                        count++;
                  
                    });
                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);

            
                    $.jqplot.config.enablePlugins = true;
              

                    plot1 = $.jqplot('dashboard1', [data1, data2], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Number of incident reported VS Number of incident recorded</b>',
                        seriesColors: ['#00749F', '#C7754C'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50},
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: { show: true },
                            rendererOptions: {
                                barPadding: 10,
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
                            {label:'Report'},
                            {label:'Record'},
                            
                        ],

                    });

                    //

                    var width = count * 70;
                    if (minWidth > width)
                    {
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




    function setDashboard2(area_id)
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
            url: 'Dashboard.asmx/getDashboardIncident2',
            dataType: 'json',
            success: function (json) {

                
                if (json.redirect == "")
                {
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
                    area_id_list2 = [];
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.fatality);
                        data2.push(key.pd);
                        data3.push(key.lti);
                        data4.push(key.rwc);
                        data5.push(key.mti);
                        data6.push(key.mi);
                        data7.push(key.damage);
                        data8.push(key.nm);

                        data_all.push(key.fatality);
                        data_all.push(key.pd);
                        data_all.push(key.lti);
                        data_all.push(key.rwc);
                        data_all.push(key.mti);
                        data_all.push(key.mi);
                        data_all.push(key.damage);
                        data_all.push(key.nm);
                      
                        area_id_list2.push(key.area_id);
                        count++;

                    });

                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);

                   
                   
                    $.jqplot.config.enablePlugins = true;


                    plot2 = $.jqplot('dashboard2', [data1, data2,data3,data4,data5,data6,data7,data8], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Number of all incidents recorded by categories(Fatality/PD/LTI/RWC/MTI/MI/Property damages/Near Miss)</b>',
                        seriesColors: ['#cb4335', '#58d68d', '#a569bd', '#5dade2', '#6e2c00', '#e67e22', '#eef904', '#ff9999'],
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
                               // formatString: '%d'
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
                            { label: 'No. of Fatality' },
                            { label: 'No. of PD' },
                            { label: 'No. of LTI' },
                            { label: 'No. of RWC' },                
                            { label: 'No. of MTI' },
                            { label: 'No. of MI' },
                            { label: 'No. of Damage' },
                            { label: 'No. of Near Miss' },

                        ],

                    });

                    //

                    var width = count * 170;
                    if (minWidth > width) {
                        $('#dashboard2').width(minWidth);

                    } else {
                        $('#dashboard2').width(width);
                    }
                    //plot2.redraw();
                    plot2.replot({ resetAxes: true });
                }else 
                {

                    window.open(json.redirect, '_blank');



                }


            }
        });

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
            url: 'Dashboard.asmx/getDashboardIncident3',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "")
                {
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
                   

                    
                    $.jqplot.config.enablePlugins = true;


                    plot = $.jqplot('dashboard3', [data1, data2, data3], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Action status of all incident recorded</b>',
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
                               // formatString: '%d'
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
                            { label: 'On process' },
                            { label: 'Close' },
                            { label: 'Delay' },

                        ],

                    });

                    //

                    var width = count * 80;
                    if (minWidth > width) {
                        $('#dashboard3').width(minWidth);

                    } else {
                        $('#dashboard3').width(width);
                    }
                    //plot.redraw();
                    plot.replot({ resetAxes: true });

                } else {


                    window.open(json.redirect, '_blank');



                }


            }
        });

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
            url: 'Dashboard.asmx/getDashboardIncident4',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "")
                {
                    var data1 = [];
                    var data2 = [];
                    var data3 = [];
                    var label = [];
                    var data_all = [];
                    area_id_list4 = [];
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.onprocess);
                        data2.push(key.close);
                        data3.push(key.delay);

                        data_all.push(key.onprocess);
                        data_all.push(key.close);
                        data_all.push(key.delay);


                        area_id_list4.push(key.area_id);

                        count++;
                    });

                   
                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);

                    $.jqplot.config.enablePlugins = true;


                    plot = $.jqplot('dashboard4', [data1, data2, data3], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Action status of injury cases: Faltality/PD/LTI/RWC/MTI/MI</b>',
                        seriesColors: ['#f9fc0b', '#22e657', '#f21308'],
                        animate: !$.jqplot.use_excanvas,
                        
                        gridPadding: { left: 50 },
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: { show: true },
                            rendererOptions: {
                                barPadding: 5,
                                barMargin: 50,
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
                            { label: 'On process' },
                            { label: 'Close' },
                            { label: 'Delay' },

                        ],

                    });

                  
                    var width = count * 80;
                    if (minWidth > width) {
                        $('#dashboard4').width(minWidth);

                    } else {
                        $('#dashboard4').width(width);
                    }

                   // plot.redraw();
                    plot.replot({ resetAxes: true });

                } else {


                    window.open(json.redirect, '_blank');



                }


            }
        });

    }




    function filterAllincident()
    {
        showLoading();
        setTimeout(function () { closeLoading(); }, 3000);

        setDashboard1("");
        setDashboard2("");
        setDashboard3("");
        setDashboard4("");


        return false;
    }


    function generateY(max_value) {
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
                y = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

            } else if (max_value <= 5) {

                y = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

            } else if (max_value > 10) {

                y = [0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24];
            }
        }

        return y;

    }

</script>

  


    <div class="tabs-container">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="dashboardincident.aspx" aria-expanded="true">1</a></li>
                             <%
                
                                if (Session["country"].ToString() =="srilanka")         
                                {

                                %>
                                    <li class=""><a href="dashboardincident2.aspx" aria-expanded="false">2</a></li>
                              <%     
                                }
               
                             %>

                              <li class=""><a href="dashboardincident3.aspx" aria-expanded="true">Pending Works</a></li>
                        </ul>
                        <div class="tab-content">
                            <div id="tab-1" class="tab-pane active" >
                                <div class="panel-body">
                                   
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
                                   <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterAllincident();" CssClass="btn btn-primary"/>
                            <%                      
                            }
       
                          %>            
                                            
                        </div>
                    </div>
                </div>
                              
       </div>
    <br />

    <div class="row">
        <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:175px;">
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
        <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:185px;">
             <span class="label label-info" style="font-size:11px;"><%= Resources.Main.lbinfodashboard %></span>
             <br />
            <br />
            <div id="dashboard2" style="height:500px;" ></div>
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
        <div class="col-md-12" style="overflow-x:auto;overflow-y:hidden;padding-bottom:175px;">
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

    <div class="row">
       <div  class="col-md-12" style="overflow-x:scroll;overflow-y:hidden;padding-bottom:175px;">
             <span class="label label-info" style="font-size:11px;"><%= Resources.Main.lbinfodashboard %></span>
             <br />
            <br />
           <div id="dashboard4" style="height:500px;"></div>
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


                                </div>
                            </div>

                             
                            <div id="tab-2" class="tab-pane">
                                <div class="panel-body" >
                                                                  
                                </div>
                            </div>


                             <div id="tab-3" class="tab-pane">
                                <div class="panel-body" >
                                                                  
                                </div>
                            </div>

                            
                        </div>


                    </div>
</asp:Content>
