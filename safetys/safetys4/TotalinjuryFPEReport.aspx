<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="TotalinjuryFPEReport.aspx.cs" Inherits="safetys4.TotalinjuryFPEReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<link href="template/css/plugins/datapicker/datepicker3.css" rel="stylesheet">



<script src="template/js/plugins/jquery.jqplot/jquery.jqplot.min.js"></script>

<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.pieRenderer.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.pointLabels.js"></script>
<script  type="text/javascript" src="template/js/plugins/jquery.jqplot/plugins/jqplot.highlighter.js"></script>

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



<style type="text/css">
.jqplot-target {
    font-family:open sans;
    font-size: 13pt !important;
    font-weight:bold;
}
.wrapper-content {
        margin-left: 300px;
    }


</style>
<script>
    var dialog;
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
           
             $("#contractor-form").css('overflow-x', 'hidden');
         },
         modal: true,
     });


     setCompany('', '', '');
     setPiechart();



     
     
 });

    function downloadCanvas(link, canvasId, filename)
    {
        link.href = document.getElementById(canvasId).toDataURL();
        link.download = filename;
    }

    /** 
     * The event handler for the link's onclick event. We give THIS as a
     * parameter (=the link element), ID of the canvas and a filename.
    */


    function filterAllincident() {

        showLoading();
        setTimeout(function () { closeLoading(); }, 3000);
        setPiechart();


        return false;
    }


    function setPiechart()
    {
        var company_id = $('#MainContent_ddcompany').val();
        var function_id = $("#MainContent_ddfunction").val();
        var department_id = $("#MainContent_dddepartment").val();
        var date_start = $("#MainContent_txtstart_date").val();
        var date_end = $("#MainContent_txtend_date").val();


        $.ajax({
            type: "GET",
            data: {
                company_id : company_id,
                function_id: function_id,
                department_id : department_id,
                date_start: date_start,
                date_end: date_end,
                lang: lang
            },
            url: 'Report.asmx/getTotalinjuryFPE',
            dataType: 'json',
            success: function (json) {


                    var data = [];
                   
                    var count = 0;


                    $.each(json.result, function (value, key) {
                        data.push([key.label, key.value]);
                       
                        count++;

                    });
                  
                    
                    var total = 0;

                    $(data).map(function () { total += this[1]; })

                    myLabels = $.makeArray($(data).map(function () { return this[1] + ", " + Math.round(this[1] / total * 100) + "%"; }));
                //  plot1.redraw();
                    //console.log(data);
                   // data.sort(function (a, b) { return a - b });
                    piechart = jQuery.jqplot('report_piechart',
                        [data],
                        {
                            title: '<b>Total Injury by OH&S FPE Report</b>',
                            seriesColors: ['#8181F7', '#A9D0F5', '#58FA58', ' #229954', '#F4FA58', '#FFBF00', '#fbeee6', '#FA5858', '#B40404', '#BDBDBD', '#B404AE', '#FF8000', '#AC58FA', '#FA5882'],
                            seriesDefaults: {
                                shadow: false,
                                renderer: jQuery.jqplot.PieRenderer,
                                rendererOptions: {
                                    sliceMargin: 4,
                                    showDataLabels: true,
                                    dataLabels: myLabels,
                                    
                                    //dataLabelPositionFactor: 1.2,
                                    // default dataLabelThreshold  value is 3,  hence values <=3 are not displayed hence make it to 0
                                   // dataLabelThreshold: 0
                                    //dataLabelFormatString: '%.0f'
                                    
                                },
                            
                            },
                            highlighter: {
                                show: true,
                                sizeAdjust: 10,
                                //tooltipLocation: 'n',
                                //tooltipAxes: 'pieref',
                                //tooltipAxisX: 60,
                                //tooltipAxisY: 90,
                                tooltipContentEditor:tooltipContentEditor,
                                useAxesFormatters: false,
                                formatString:'%s, %P'
                            
                            },

                            legend: { show: true, location: 'e', fontSize: '10pt' }
                        }
                        );

                    piechart.redraw();

                    var imgData = $("#report_piechart").jqplotToImageStr({}); // given the div id of your plot, get the img data
                    var imgElem = $("#image").attr("src",imgData); // create an img and add the data to it
                  
                    //$("#report_piechart").bind('jqplotDataHighlight', function (ev, seriesIndex, pointIndex, plot) {
                    //    var $this = $(this);

                    //    $this.attr('title', data[0]);
                    //});

                    //$("#report_piechart").bind('jqplotDataUnhighlight', function (ev, seriesIndex, pointIndex, data) {
                    //    var $this = $(this);

                    //    $this.attr('title', "");
                    //});

                    //var canvas = document.getElementById("canvas");
                    //ctx = canvas.getContext("2d");

                    //canvas.width = 500;
                    //canvas.height = 700;

                    //var background = new Image();
                    //background.src = imgData;
  
                    //ctx.drawImage(background,0,0);   
                    //console.log("test");

                    //document.getElementById('download').addEventListener('click', function () {
                    //    downloadCanvas(this, 'canvas', 'TotalinjuryFPEReport.jpg');
                    //}, false);

            }
        });

    }

    function tooltipContentEditor(str, seriesIndex, pointIndex, plot)
    {
        // display series_label, x-axis_tick, y-axis value
        return plot.data[seriesIndex][pointIndex]+"  ";
    }



    function setCompany(company_id, function_id, department_id)
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


    function setFunction(company_id, function_id, department_id)
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
                setDepartment(function_id, department_id);
            }
        });



    }




    function setDepartment(function_id, department_id)
    {
        var all = '<%= Resources.Main.all %>';
        var $el = $("#MainContent_dddepartment");
        $el.empty(); // remove old options

        $el.append($("<option></option>")
                   .attr("value", "").text(all));
        $.ajax({
            type: "POST",
            data: { function: function_id, lang: lang },
            url: 'Masterdata.asmx/getDepartmentbyFunction',
            dataType: 'json',
            success: function (json) {

              
                $.each(json, function (value, key) {

                    $el.append($("<option></option>")
                            .attr("value", key.id).text(key.name));
                });


                if (department_id != "") {
                    $("#MainContent_dddepartment").val(department_id);

                }
               
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


    function openChart()
    {
        dialog.dialog("open");
     
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
                                    
                            <select id="dddepartment" class="form-control" runat="server">
                       
                            </select>
                                        
                        </div>
                    </div>


                   </div>


                   <div class="row">
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
             

               <div class="col-md-3">
                    <div class="form-group">
                        <br />
                               
                        <%--                        <%
                            ArrayList per2 = Session["permission"] as ArrayList;
                            if (per2.IndexOf("report search") > -1)         
                           {                            
       
                          %>--%>
                                   <asp:Button ID="btOK"  runat="server" Text="<%$ Resources:Main, btOK %>" OnClientClick="return filterAllincident();" CssClass="btn btn-primary"/>
                          <%--  <%                      
                            }
       
                          %>     --%>       
                                            
                        
                    </div>
       
       </div>


                   </div>
                   
               
               
   </div>

    <div class="row">
        <div class="col-md-12">
             <br />
            <div id="report_piechart" style="height:600px;"></div>
            
        </div>
    </div>

    <div id="download_chart">
        <div class="row">
             <img id="image"/>
           <%-- <canvas id="canvas"></canvas>--%>
        </div>

    </div>
   
    <%--<a id="download" class="btn btn-primary"><i class="fa fa-download"></i></a>--%>
   <button type="button" id="btOpenChart" class="btn btn-primary"  onclick="openChart();"><i class="fa fa-download" aria-hidden="true"></i></button>

</asp:Content>
