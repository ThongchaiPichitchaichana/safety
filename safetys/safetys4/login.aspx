<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="safetys4.login" %>

<!DOCTYPE html>
<html>

<head>

    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />

    <title>safety</title>

    <link href="template/css/bootstrap.min.css" rel="stylesheet">
    <link href="template/font-awesome/css/font-awesome.css" rel="stylesheet">
    <link href="template/css/animate.css" rel="stylesheet">
    <link href="template/css/style.css" rel="stylesheet">
    <link href="template/css/plugins/jquery-ui-1.12.0/jquery-ui.min.css" rel="stylesheet">
    <link href="template/css/plugins/loading/HoldOn.min.css" rel="stylesheet" type="text/css" />
     
    <style type="text/css">

    .tab_login
    {
      padding-top: 2%;
      margin-left: 25%;
      margin-right: 25%;

    }

    .logo_login
    {
  
      position:relative;
      left:50%;
      margin-left: -210px;
    }

/*
    .form-control-new, .single-line {
   
     background-color: #ea2306 !important;
     width:100%;

    } 
*/
  .form-control-new, .single-line {
      -moz-border-bottom-colors: none;
      border:none;
      background-color: #ea2306;
      background-image: none;
      border-bottom-color: #e5e6e7;
      border-bottom-left-radius: 1px;
      border-bottom-right-radius: 1px;
      border-bottom-style: solid;
      border-bottom-width: 1px;

      color: inherit;
      display: block;
      font-size: 14px;
      padding-bottom: 6px;
      padding-left: 12px;
      padding-right: 12px;
      padding-top: 6px;
      transition-delay: 0s, 0s;
      transition-duration: 0.15s, 0.15s;
      transition-property: border-color, box-shadow;
      transition-timing-function: ease-in-out, ease-in-out;
      margin-bottom: 30px;
      width: 100%;
  }

     .dropdown
    {
      position:relative;
      left:50%;
      margin-left: -100px;

    }

     .menu_language
    {
      margin-left: 20%;
      margin-right: 20%;
    }


  .btn-default-new {
    -moz-border-bottom-colors: none;
    -moz-border-left-colors: none;
    -moz-border-right-colors: none;
    -moz-border-top-colors: none;
    background-attachment: scroll;
    background-clip: border-box;
    background-color: #ffffff;
    background-image: none;
    background-origin: padding-box;
    background-position-x: 0;
    background-position-y: 0;
    background-repeat: repeat;
    background-size: auto auto;
    border-bottom-color: #e7eaec;
    border-bottom-style: solid;
    border-bottom-width: 1px;
    border-image-outset: 0 0 0 0;
    border-image-repeat: stretch stretch;
    border-image-slice: 100% 100% 100% 100%;
    border-image-source: none;
    border-image-width: 1 1 1 1;
    border-left-color: #e7eaec;
    border-left-style: solid;
    border-left-width: 1px;
    border-right-color: #e7eaec;
    border-right-style: solid;
    border-right-width: 1px;
    border-top-color: #e7eaec;
    border-top-style: solid;
    border-top-width: 1px;
    color: inherit;
    width:200px;
  }
  .btn-new {
      border-bottom-left-radius: 3px;
      border-bottom-right-radius: 3px;
      border-top-left-radius: 3px;
      border-top-right-radius: 3px;
  }
  .btn-new {
      -moz-user-select: none;
      cursor: pointer;
      display: inline-block;
      font-size: 14px;
      font-weight: 400;
      line-height: 1.42857;
      margin-bottom: 0;
      padding-bottom: 6px;
      padding-left: 12px;
      padding-right: 12px;
      padding-top: 6px;
      text-align: center;
      vertical-align: middle;
      white-space: nowrap;
      background-color: #cacfd2;
  }


.btn-default-new:hover, .btn-default-new:focus, .btn-default-new:active, .btn-default-new.active, .open .dropdown-toggle.btn-default-new, .btn-default-new:active:focus, .btn-default-new:active:hover, .btn-default-new.active:hover, .btn-default-new.active:focus {
    -moz-border-bottom-colors: none;
    -moz-border-left-colors: none;
    -moz-border-right-colors: none;
    -moz-border-top-colors: none;
    border-bottom-color: #d2d2d2;
    border-bottom-style: solid;
    border-bottom-width: 1px;
    border-image-outset: 0 0 0 0;
    border-image-repeat: stretch stretch;
    border-image-slice: 100% 100% 100% 100%;
    border-image-source: none;
    border-image-width: 1 1 1 1;
    border-left-color: #d2d2d2;
    border-left-style: solid;
    border-left-width: 1px;
    border-right-color: #d2d2d2;
    border-right-style: solid;
    border-right-width: 1px;
    border-top-color: #d2d2d2;
    border-top-style: solid;
    border-top-width: 1px;
    color: inherit;
}


.btn-default-new.active, .btn-default-new:active, .open > .dropdown-toggle.btn-default-new {
    background-color: #e6e6e6;
}

.error_msg{
  font-weight: bold;
  font-size: 16px;
  text-align: center;
  margin-bottom: 20px;
}



</style>
</head>

<body>
 <form id="login_notify_form" action="" method="POST" class="form-horizontal" autocomplete="off" runat="server">
  <div class="logo_login">


                <%
                
                if (Session["lang"] != null)         
                {
                    if (Session["lang"] =="th")
                    {                  
                    %>
                        <img src="template/img/Logo_Thai.png"   width="400" height="132"> 

                    <%                      
                    }
                    else if (Session["lang"] == "en")
                    { 
       
                    %>

                        <img src="template/img/Logo_Eng.png" width="400" height="132"> 

                    <%     
                    }
                    else if (Session["lang"] == "si")
                    {                   
                    %>
                         <img src="template/img/Logo_Eng.png" width="400" height="132"> 
                    <%
                    }
                } 
               
                %>
     
  </div>
  
  <div class="menu_language">
    <div class="dropdown">
    <button id="bt_language" class="btn-new btn-default-new dropdown-toggle" type="button" data-toggle="dropdown">
        <asp:Label ID="lbLanguageShow" runat="server" Text=""></asp:Label>
    <span class="caret"></span></button>
    <ul class="dropdown-menu">
      <li>
          <asp:LinkButton ID="LinkLanguageTH" runat="server" OnClick="LinkLanguageTH_Click"><img src="template/img/language/th.png"><strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ไทย</strong></asp:LinkButton>
     </li>
      <li>
          <asp:LinkButton ID="LinkLanguageEN" runat="server" OnClick="LinkLanguageEN_Click"><img src="template/img/language/gb.png"><strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English</strong></asp:LinkButton>       
      </li>
        <li>
          <asp:LinkButton ID="LinkLanguageSI" runat="server" OnClick="LinkLanguageSI_Click"><img src="template/img/language/si.png"><strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;English</strong></asp:LinkButton>       
      </li>
     
    </ul>
 </div>

  </div>
  <div class="tab_login">
     <div class="tabs-container">
                        <ul class="nav nav-tabs">
                            <li id="list_tab-1" class=""><a data-toggle="tab" href="#tab-1" runat="server">
                                <asp:Label ID="listTab1" runat="server" Text="<%$Resources:Main, listTab1 %>"></asp:Label></a>

                            </li>
                              <li id="list_tab-2" class=""><a data-toggle="tab" href="#tab-2" runat="server">
                                 <asp:Label ID="listTab3" runat="server" Text="<%$Resources:Main, listTab2 %>"></asp:Label>
                            </a>

                            </li>
                            <li id="list_tab-3" class=""><a data-toggle="tab" href="#tab-3" runat="server">
                                <asp:Label ID="listTab2" runat="server" Text="<%$Resources:Main, listTab3 %>"></asp:Label>
                             </a>

                            </li>
                          
                        </ul>
                        <div class="tab-content">
                         
                            <div id="tab-1" class="tab-pane">
                                <div class="panel-body">
                                   <div id="error_notify" class="error_msg">
                                       <asp:Label ID="errorNotify" runat="server" Text=""></asp:Label></div>
                                   
                                      <div class="form-group">
                                          <div class="col-sm-12">
                                              <asp:TextBox runat="server" ID="txtUserNotify" CssClass="form-control-new"  placeholder="<%$Resources:Main, txtUserNotify %>"/>
                                             
                                          </div>
                                      </div>
                                      <div class="form-group">
                                          <div class="col-sm-12">
                                              <asp:TextBox runat="server" ID="txtFirstnameNotify" CssClass="form-control-new"  placeholder="<%$Resources:Main, txtFirstnameNotify %>" onkeydown="return checkTab1(event);"/>
                                            
                                          </div>
                                      </div>
                                  
                                       <asp:Button ID="btLoginNotify" runat="server" Text="<%$Resources:Main, btLoginNotify %>" CssClass="btn btn-md btn-default btn-block" style="color:#000;font-weight: bold;" OnClick="btLoginNotify_Click"  onclientclick="showLoading();"/>
                                  
                                </div>
                            </div>
                           
                             <div id="tab-2" class="tab-pane">
                                <div class="panel-body">
                                    <div id="error_contractor" class="error_msg"><asp:Label ID="errorContractor" runat="server" Text=""></asp:Label></div>
                                
                                      <div class="form-group">
                                          <div class="col-sm-12">
                                               <asp:TextBox runat="server" ID="txtEmailRegister" CssClass="form-control-new"  placeholder="<%$Resources:Main, txtEmailRegister %>" onkeydown="return checkTab2(event);"/>
                                        
                                          </div>
                                      </div>
                                     <asp:Button ID="btLoginContractor" runat="server" Text="<%$Resources:Main, btLoginContractor %>" CssClass="btn btn-md btn-default btn-block" style="color:#000;font-weight: bold;" OnClick="btLoginContractor_Click" onclientclick="showLoading();"/>               
                                      <div class="line line-dashed"></div>
                        
                                </div>
                            </div>

                             <div id="tab-3" class="tab-pane">
                                <div class="panel-body">
                                    <div id="error_check" class="error_msg"><asp:Label ID="errorCheck" runat="server" Text=""></asp:Label></div>
                                     
                                      <div class="form-group">
                                          <div class="col-sm-12">
                                               <asp:TextBox runat="server" ID="txtUsernameCheck" CssClass="form-control-new"  placeholder="<%$Resources:Main, txtUsernameCheck %>"/>
                                             
                                          </div>
                                      </div>
                                      <div class="form-group">
                                          <div class="col-sm-12">
                                               <asp:TextBox runat="server" ID="txtPasswordCheck" TextMode="Password" CssClass="form-control-new" placeholder="<%$Resources:Main,txtPasswordCheck %>" onkeydown="return  checkTab3(event);"/>
                                              
                                          </div>
                                      </div>
                                    <asp:Button ID="btLoginCheck" runat="server" Text="<%$ Resources:Main, btLoginCheck %>" CssClass="btn btn-md btn-default btn-block" style="color:#000;font-weight: bold;" OnClick="btLoginCheck_Click" onclientclick="showLoading();"/>               
                                    <div class="line line-dashed"></div>
                                
                                </div>
                            </div>


                           
                        
                        </div>

  </div>
</div>
 </form>


    <!-- Mainly scripts -->
    <script src="template/js/jquery-2.1.1.js"></script>
    <script src="template/js/bootstrap.min.js"></script>
    <script src="template/js/plugins/jquery-ui-1.12.0/jquery-ui.min.js"></script> 
    <script src="template/js/plugins/loading/HoldOn.min.js"></script>


</body>

<script type="text/javascript">
    var type_login = '<%= Session["typeLogin"] %>';
    

    $(document).ready(function () {

        //$('#txtFirstnameNotify').keydown(function (event) {
        //    if (event.keyCode == 13) {
        //        $('#btLoginNotify').click();
        //    }
        //});

        
        //$('#txtEmailRegister').keydown(function (event) {
        //    if (event.keyCode == 13) {
        //        $('#btLoginContractor').click();
        //    }
        //});

        //$('#txtPasswordCheck').keydown(function (event) {
        //    if (event.keyCode == 13) {
        //        $('#btLoginCheck').click();
        //    }
        //});

        $("#tab-1").removeClass("active");
        $("#tab-2").removeClass("active");
        $("#tab-3").removeClass("active");

        $("#list_tab-1").removeClass("active");
        $("#list_tab-2").removeClass("active");
        $("#list_tab-3").removeClass("active");


        if (type_login == "super") {
            $("#tab-3").addClass("active");
            $("#list_tab-3").addClass("active");
        

        } else if (type_login == "contractor") {

            $("#tab-2").addClass("active");
            $("#list_tab-2").addClass("active");
       
        } else {
            $("#tab-1").addClass("active");
            $("#list_tab-1").addClass("active");
    
        }

    });

    function checkTab1(evt)
    {
        if (evt.keyCode == 13)
        {
            $('#btLoginNotify').click();
            return false;
        }

        //return false;
    }


    function checkTab2(evt)
    {
       
        if (evt.keyCode == 13) {
           // alert(evt.keyCode);
            $('#btLoginContractor').click();
            //document.getElementById("btLoginContractor").click();
            return false;
        }
        //return false;
    }

    function checkTab3(evt) {
        if (evt.keyCode == 13) {
            $('#btLoginCheck').click();
            return false;
        }

        //return false;
    }


    function showLoading() {
        HoldOn.open({
            theme: 'sk-rect',
            message: "<h4><%= Resources.Main.loading %></h4>"
        });

    }

    function closeLoading() {
        setTimeout(function () {
            HoldOn.close();
        }, 1000);

    }




</script> 

</html>