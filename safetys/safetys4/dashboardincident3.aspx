<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="dashboardincident3.aspx.cs" Inherits="safetys4.dashboardincident3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">




    <script src="template/js/plugins/jquery.jqplot/jquery.jqplot.min.js"></script>

    <script type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.barRenderer.js"></script>
    <script type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.categoryAxisRenderer.js"></script>
    <script type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.pointLabels.js"></script>
    <script type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.canvasTextRenderer.js"></script>
    <script type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.canvasAxisLabelRenderer.js"></script>
    <script type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.canvasAxisTickRenderer.js"></script>
    <script type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.enhancedLegendRenderer.js"></script>

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
        .jqplot-point-label {
            font-size: 1em;
            font-weight: bold;
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
    

    var minWidth = $(window).width() - 200;
    
    $(document).ready(function () {


      <%--  <%
                
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
               
                                        %>--%>

            setDashboardFormOneToTwo("");
            setDashboardFormTwoToThree("");
 
       
     

        $('#dashboard').bind('jqplotDataClick',
              function (ev, seriesIndex, pointIndex, data) {

                  setDashboardFormOneToTwo(area_id_list[pointIndex]);
              }
          );


        $('#dashboard2').bind('jqplotDataClick',
             function (ev, seriesIndex, pointIndex, data) {

                 setDashboardFormTwoToThree(area_id_list2[pointIndex]);
             }
         );

       

     
    });






   

    function setDashboardFormOneToTwo(area_id)
    {

        var date_start = "";//$("#MainContent_txtstart_date").val();
        var date_end = "";//$("#MainContent_txtend_date").val();


        $.ajax({
            type: "GET",
            data: {
                area_id: area_id,
                date_start: date_start,
                date_end: date_end,
                lang: lang
            },
            url: 'Dashboard.asmx/getDashboardIncidentFormOneToTwo',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "")
                {
                    var data1 = [];
                    var label = [];

                    var data_all = [];
                    area_id_list = [];
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.work);

                        data_all.push(key.work);
                        area_id_list.push(key.area_id);
                        count++;

                    });
                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);


                    $.jqplot.config.enablePlugins = true;


                    plot1 = $.jqplot('dashboard', [data1], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Number of pending reports form 1-2 (verify incident report) </b>',
                        seriesColors: ['#00749F'],
                        animate: !$.jqplot.use_excanvas,
                        gridPadding: { left: 50 },
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
                            marginTop: '100px',
                            fontSize: '13pt'
                        }, series: [
                            { label: 'No. of report' },

                        ],

                    });

                    //

                    //var width = count * 70;
                    //if (minWidth > width) {
                    //    $('#dashboard').width(minWidth);

                    //} else {
                    //    $('#dashboard').width(width);
                    //}

                    // plot1.redraw();
                    plot1.replot({ resetAxes: true });

                } else {


                    window.open(json.redirect, '_blank');



                }



            }
        });

    }



    function setDashboardFormTwoToThree(area_id)
    {

        var date_start = "";//$("#MainContent_txtstart_date").val();
        var date_end = "";//$("#MainContent_txtend_date").val();


        $.ajax({
            type: "GET",
            data: {
                area_id: area_id,
                date_start: date_start,
                date_end: date_end,
                lang: lang
            },
            url: 'Dashboard.asmx/getDashboardIncidentFormTwoToThree',
            dataType: 'json',
            success: function (json) {


                if (json.redirect == "")
                {
                    var data1 = [];
                    var label = [];

                    var data_all = [];
                    area_id_list6 = [];
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        label.push(key.label);
                        data1.push(key.work);
                     

                        data_all.push(key.work);

                        area_id_list2.push(key.area_id);
                        count++;

                    });

                    var max = Math.max.apply(null, data_all);
                    var tick_y = generateY(max);



                    $.jqplot.config.enablePlugins = true;


                    plot2 = $.jqplot('dashboard2', [data1], {
                        // Only animate if we're not using excanvas (not in IE 7 or IE 8)..
                        title: '<b>Number of pending reports form 2-3 (investigation and action)</b>',
                        seriesColors: ['#cb4335'],
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
                            marginTop: '100px',
                            fontSize: '13pt'
                        }, series: [
                            { label: 'No. of report' },
                         
                        ],

                    });

                    //

                    //var width = count * 170;
                    //if (minWidth > width) {
                    //    $('#dashboard2').width(minWidth);

                    //} else {
                    //    $('#dashboard2').width(width);
                    //}
                    //plot2.redraw();
                    plot2.replot({ resetAxes: true });

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

        setDashboardFormOneToTwo("");
        setDashboardFormTwoToThree("");
       
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
            <li class=""><a href="dashboardincident.aspx" aria-expanded="true">1</a></li>
            <%
                
                                if (Session["country"].ToString() =="srilanka")         
                                {

            %>
            <li class=""><a href="dashboardincident2.aspx" aria-expanded="false">2</a></li>
            <%     
                                }
               
            %>
            <li class="active"><a href="dashboardincident3.aspx" aria-expanded="false">Pending Work</a></li>
        </ul>
        <div class="tab-content">
            <div id="tab-1" class="tab-pane">
                <div class="panel-body">
                </div>
            </div>

            <div id="tab-2" class="tab-pane">
                <div class="panel-body">
                </div>
            </div>
            <div id="tab-3" class="tab-pane active">
                <div class="panel-body" style="background-color: white;">

                    <%--                                        <div class="row" id="filter">
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
                              
       </div>--%>
                    <br />


                    <div class="row">
                        <div class="col-md-12">
                            <span class="label label-info" style="font-size: 11px;"><%= Resources.Main.lbinfodashboard %></span>
                            <br />
                            <br />
                            <div id="dashboard" style="height: 500px;"></div>
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
                        <div class="col-md-12">
                            <span class="label label-info" style="font-size: 11px;"><%= Resources.Main.lbinfodashboard %></span>
                            <br />
                            <br />
                            <div id="dashboard2" style="height: 500px;"></div>
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
        </div>


    </div>









</asp:Content>
