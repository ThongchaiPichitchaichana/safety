<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainMenu.aspx.cs" Inherits="safetys4.MainMenu" %>

<!DOCTYPE html>
<html>
 <head>
       <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
       <meta http-equiv="X-UA-Compatible" content="IE=edge" />

        <title>Safety</title>
       

        
        <link href="template/css/bootstrap.min.css" rel="stylesheet">
        <link href="template/font-awesome/css/font-awesome.css" rel="stylesheet">

        <link href="template/css/animate.css" rel="stylesheet">
        <link href="template/css/style.css" rel="stylesheet">
       
        <link href="template/css/plugins/jquery-ui-1.12.0/jquery-ui.min.css" rel="stylesheet">
        <link href="template/css/plugins/loading/HoldOn.min.css" rel="stylesheet" type="text/css" />
      
  
        <script src="template/js/jquery-2.1.1.js"></script>
        <script src="template/js/bootstrap.min.js"></script>   
        <script src="template/js/plugins/jquery-ui-1.12.0/jquery-ui.min.js"></script> 


   <style type="text/css">
	.btn-menu
	{
		height: 120px;
	    width: 130px;
        font-size: 16px !important;
	}


	.btn-menu-side
	{
		height: 100px;
    	width: 290px;
        font-size: 16px !important;
	}
	/*table,tr,td {

		padding-right: : 5px;
		padding-left: : 5px;
		padding-bottom : 5px;
	}*/

	.bt-report-acc{

		background-color:#DF2000 !important;
		color:#FFF !important;
	}

	.bt-report-haz{

		background-color: #fff40e  !important;
		color: #17202a  !important;
	}

    .bt-report-sot{

		background-color: #fb932a  !important;
		color:#17202a !important;
	}


      .bt-report-health{

		background-color: #076633  !important;
		color:#FFF !important;
	}


</style>



 </head>
<body class="top-navigation">
<form  runat="server">
<div id="wrapper">
        <div id="page-wrapper" class="gray-bg" style="padding-left:0px !important;">
        <div class="row border-bottom white-bg">
        <nav class="navbar navbar-static-top" role="navigation">
            <div class="navbar-header">
             

               <%
                
                if (Session["lang"] != null)         
                {
                    if (Session["lang"] =="th")
                    {                  
                    %>
                        <a href="MainMenu.aspx"><img src="template/img/Logo_Thai.png"   style="width:176px;height:56px;"></a>

                    <%                      
                    }
                    else if (Session["lang"] == "en")
                    { 
       
                    %>

                        <a href="MainMenu.aspx"><img src="template/img/Logo_Eng.png" style="width:176px;height:56px;"></a> 

                    <%     
                    }
                    else if (Session["lang"] == "si")
                    {                   
                    %>
                         <a href="MainMenu.aspx"><img src="template/img/Logo_Srilanka.png" style="width:176px;height:56px;padding-left:5px;"></a> 
                    <%
                    }
                } 
               
                %>

            </div>
            <div class="navbar-collapse collapse" id="navbar">
                <ul class="nav navbar-nav">
                 
                </ul>
                <ul class="nav navbar-top-links navbar-right">
                  <li><strong><i class="fa fa-user" aria-hidden="true"></i> <%= Session["name"] %></strong></li>
                  <li class="dropdown">
                        <a aria-expanded="false" role="button" href="#" class="dropdown-toggle" data-toggle="dropdown"> <%= Session["langShow"] %> <span class="caret"></span></a>
                        <ul role="menu" class="dropdown-menu">
                           <li>
                              <asp:LinkButton ID="LinkLanguageTH" runat="server" OnClick="LinkLanguageTH_Click"><img src="template/img/language/th.png"><strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ไทย</strong></asp:LinkButton>
                         </li>
                          <li>
                              <asp:LinkButton ID="LinkLanguageEN" runat="server" OnClick="LinkLanguageEN_Click"><img src="template/img/language/gb.png"><strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English</strong></asp:LinkButton>       
                          </li>
                              <% if (Session["country"].ToString() == "srilanka")
                            {                  
                            %> 
                          <li>
                              <asp:LinkButton ID="LinkLanguageSI" runat="server" OnClick="LinkLanguageSI_Click"><img src="template/img/language/si.png"><strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English</strong></asp:LinkButton>       
                          </li>
                             <%}
                             %>

                        </ul>
                    </li>
                    <li>
                      
                    
                    <asp:LinkButton ID="LinkLogout" runat="server" OnClick="LinkLogout_Click">
                        <i class="fa fa-sign-out"></i> <asp:Label ID="lbLogout" runat="server" Text="<%$ Resources:Main, btLogout %>"></asp:Label></asp:LinkButton>
                     
                    </li>
                </ul>
            </div>
        </nav>
        </div>
        <div class="wrapper wrapper-content">
            <div class="container" style="margin-left: 0px !important;">
                <div class="row">
                     <div class="col-lg-12">
                         <table class="table-condensed">
	                        <tr>
		                      
                               <td colspan="3" align="right" style="padding-top:0px !important;"><button type="button" id="btReportIncident" class="btn btn-menu-side btn-default bt-report-acc" runat="server" onServerclick="btIncident_Click"> <div><div style="float:left;display:inline-block;margin-left:15px;"><img src="template/img/reportincident.png"  style="width:52px;height:52px;"></div><div style="float:right;display:inline-block;margin-right:15px;"><strong><asp:Label ID="lbReportIncident" runat="server" Text="<%$ Resources:Main, lbReportIncident %>"></asp:Label></strong></div></div></button></td>
		
		                      
		                        <td rowspan="4" colspan="4">


                                     <%
                
                                         if (Session["country"] != null)         
                                        {
                                            if (Session["country"].ToString() =="thailand")
                                            {                  
                                            %>
                                               <img src="template/img/img_inside.jpg"  style="width:480px;height:290px;">

                                            <%                      
                                            }
       
                                            %>

                                            <%     
                                            
                                            else if (Session["country"].ToString() == "srilanka")
                                            {                   
                                            %>
                                                 <img src="template/img/img_inside_lk.jpg"  style="width:580px;height:320px;">
                                            <%
                                            }
                                        } 
               
                                     %>


		                        </td>
	                        </tr>

	                        

	                        <tr>
		                        <td colspan="3" align="right" style="padding-top:0px !important;"><button type="button" id="btReportHazard" class="btn btn-menu-side btn-default bt-report-haz" runat="server" onServerclick="btHazard_Click"> <div><div style="float:left;display:inline-block;margin-left:55px;"><img src="template/img/reporthazard.png"  style="width:52px;height:52px;"></div><div style="float:right;display:inline-block;margin-right:55px;"><strong><asp:Label ID="lbReportHazard" runat="server" Text="<%$ Resources:Main, lbReportHazard %>"></asp:Label></strong></div></div></button></td>
		
		
	                        </tr>

                              <tr>
		                        <td colspan="3" align="right" style="padding-top:0px !important;"><button type="button" id="btReportSOT" class="btn btn-menu-side btn-default bt-report-sot" runat="server" onServerclick="btSot_Click"> <div><div style="float:left;display:inline-block;margin-left:55px;margin-top:5px;"><img src="template/img/reporthazard.png"  style="width:52px;height:52px;"></div><div style="float:right;display:inline-block;margin-right:55px;"><strong><asp:Label ID="lbReportSot" runat="server" Text="<%$ Resources:Main, lbReportSot %>"></asp:Label></strong></div></div></button></td>
		
		
	                        </tr>
                              <tr>
		                        <td colspan="3" align="right" style="padding-top:0px !important;"><button type="button" id="btReportHealth" class="btn btn-menu-side btn-default bt-report-health" runat="server" onServerclick="btHealth_Click"> <div><div style="float:left;display:inline-block;margin-left:55px;margin-top:0px;"><img src="template/img/reporthealth.png"  style="width:52px;height:52px;"></div><div style="float:right;display:inline-block;margin-right:25px;"><strong><asp:Label ID="Label1" runat="server" Text="<%$ Resources:Main, lbReportHealth %>"></asp:Label></strong></div></div></button></td>
		
		
	                        </tr>

	                        <tr>
		                        <td colspan="3"></td>
		
	                        </tr>


	                        <tr>
		                        <td>
                               
		                        <button type="button" id="btMyAction" class="btn btn-menu btn-default" runat="server" onServerClick="btMyAction_Click"><img src="template/img/myjob.png"  style="width:37px;height:37px;"> 
		                        <br/><asp:Label ID="lbMyAction" runat="server" Text="<%$ Resources:Main, lbMyAction %>"></asp:Label></button></td>
		                        <td><button type="button" id="btAllIncident" class="btn btn-menu btn-default" runat="server" onServerClick="btAllIncident_Click"><img src="template/img/incident.png"  style="width:37px;height:37px;">
		                        <br/><asp:Label ID="lbAllIncident" runat="server" Text="<%$ Resources:Main, lbAllIncident %>" ></asp:Label></button></td>
		                        <td><button type="button" id="btAllHazard" class="btn btn-menu btn-default" runat="server" onServerClick="btAllHazard_Click"><img src="template/img/hazard.png"  style="width:37px;height:37px;">
		                        <br/><asp:Label ID="lbAllHazard" runat="server" Text="<%$ Resources:Main, lbAllHazard %>"></asp:Label></button></td>
                                <td id="td_sot" runat="server"><button type="button" id="btAllSot" class="btn btn-menu btn-default" runat="server" onServerClick="btAllSot_Click"><img src="template/img/hazard.png"  style="width:37px;height:37px;">
		                        <br/><asp:Label ID="lbAllSot" runat="server" Text="<%$ Resources:Main, lbAllSot %>"></asp:Label></button></td>
                                <td id="td_health" runat="server"><button type="button" id="btAllHealth" class="btn btn-menu btn-default" runat="server" onServerClick="btAllHealth_Click"><img src="template/img/health2.png"  style="width:37px;height:37px;">
		                        <br/><asp:Label ID="lbAllHealth" runat="server" Text="<%$ Resources:Main, lbAllHealth %>"></asp:Label></button></td>
		                        <td><button type="button" id="btDashboard" class="btn btn-menu btn-default" runat="server" onServerClick="btDashboard_Click"><img src="template/img/dashboard.png"  style="width:37px;height:37px;">
		                        <br/><asp:Label ID="lbDashboard" runat="server" Text="<%$ Resources:Main, lbDashboard %>"></asp:Label></button></td>
		                        <td><button type="button" id="btReport" class="btn btn-menu btn-default" runat="server" onServerClick="btReport_Click"><img src="template/img/report.png"  style="width:37px;height:37px;">
		                        <br/><asp:Label ID="lbReport" runat="server" Text="<%$ Resources:Main, lbReport %>"></asp:Label></button></td>
		                        <td><button type="button" id="btContractor" class="btn btn-menu btn-default"  runat="server" onServerClick="btContractor_Click"><img src="template/img/contractor.png"  style="width:37px;height:37px;">
		                        <br/><asp:Label ID="lbContractor" runat="server" Text="<%$ Resources:Main, lbContractor %>"></asp:Label></button></td>
		                        <td><button type="button" id="btAdmin" class="btn btn-menu btn-default" runat="server" onServerClick="btAdmin_Click"><img src="template/img/setting.png"  style="width:37px;height:37px;">
		                        <br/><asp:Label ID="lbAdmin" runat="server" Text="<%$ Resources:Main, lbAdmin %>"></asp:Label></button></td>
		
	                        </tr>


                        </table>




                    </div>

                </div>



            </div>

        </div>
     

        </div>
        </div>

    </form>
                       


<script src="template/js/plugins/metisMenu/jquery.metisMenu.js"></script>
<script src="template/js/plugins/slimscroll/jquery.slimscroll.min.js"></script>

<!-- Custom and plugin javascript -->
<script src="template/js/inspinia.js"></script>
<script src="template/js/plugins/pace/pace.min.js"></script>

<script src="template/js/plugins/loading/HoldOn.min.js"></script>


<script type="text/javascript">



 $(document).ready(function(){

 });

       


    
</script> 

</body>
</html>