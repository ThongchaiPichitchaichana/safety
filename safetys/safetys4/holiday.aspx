<%@ Page Title="" Language="C#" MasterPageFile="~/Saftetymain.Master" AutoEventWireup="true" CodeBehind="holiday.aspx.cs" Inherits="safetys4.holiday" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style type="text/css">

    </style>
<script>
  
    var current_year = '<%= DateTime.UtcNow.AddHours(Convert.ToDouble(Session["timezone"].ToString())).Year %>';
    var select_function_id = "";
 $(document).ready(function () {

     setYear(current_year);
    
     
 });


    function setYear(current_year)
    {

        $.ajax({
            type: "POST",
            data: { lang: lang },
            url: 'Masterdata.asmx/getHolidayYear',
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
              
            }
        });



    }




</script>





               <div class="row" id="filter">
                   
                   <div class="col-md-2">
                       <h2><b>Holiday</b></h2>
                       <br />
                        <div class="form-group">    
                        <div class="form-inline">
                            <label class="control-label"><%= Resources.Hazard.lbyear %></label>
                            <select id="ddyear" class="form-control"  runat="server">
                       
                            </select>
                                      
                                            
                            </div>
                        </div>
                    </div>

   </div>
    <br />
    
    <div class="row">
        <div class="col-md-12">
           
            <asp:Label ID="lbshowfile" runat="server" Text=""></asp:Label>
            <asp:Label ID="lbname" runat="server" Text=""></asp:Label>
            <br />
             <table>
                 <tr>
                     <td> <asp:FileUpload ID="uploadHoliday" runat="server" CssClass="btn btn-primary" accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"/></td>
                       <td> <asp:Button ID="btSearch"  runat="server" Text="<%$ Resources:Main, btOK %>"  CssClass="btn btn-primary" OnClick="btSearch_Click"/>
            
                    </td>
                 </tr>
             </table>
           
            
        </div>
    </div>

 

</asp:Content>
